using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Anime_Organizer.Windows.Edits;
using Anime_Organizer.Windows.Other;
using Anime_Organizer.Windows.Startup;
using System.Windows.Media.Animation;
using Anime_Organizer.Windows.Adding;
using System.Text.Json;
using System.Media;
using Anime_Organizer.Windows.UserControls;
using Anime_Organizer.Classes;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;

namespace Anime_Organizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        //Link for the template I heavily edited to create my own light mode and dark mode: https://github.com/kalatchev/WPFDarkTheme

        /**
         * //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
         * Current Issues With the Window
         * 
         * - Figure out how to put on a publisher name
         * - Find a way to add a key to it (though i think it already does)
         * - Find a way to have it installable from the mincrosoft store.
         * - Find a way to have the program trustable to other pc's.
         * 
         * //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
         * OPTIONAL: These could be unneeded/wanted fixes to the program or something to help better QOL
         * 
         * - Have the ability to scroll left and right on the datagrid usig a touch pad, as a mouse cant.
         * 
         * - Fairly Optional: Create a method that runs through all save files (not just yours) and checks if there are any images that are no longer 
         *      used. If this is the case, the program will automatically delete them.
         * 
         * - Very Optional: Figure out if its possible to more generalize the process for disabling controls for tab navigation into a single method
         *      that can be called from anywhere. This is so I can remove any inconsistency. This will also have to account for ones that already have 
         *      tabstopped false already, like checkBoxes which break the program if its true. Have it set up to generalize get a list of the controls,
         *      figure out which ones are already set to false an whitelist them so those can not be changed, and yeah.
         * 
         * - Possible answer for my scrolling issues: (This will probably be a long winded fix and this isnt even guarenteed to fix what I want to fix.)
         * https://stackoverflow.com/questions/52665473/wpf-how-to-affect-the-scroll-amount-of-a-scrollview
         * 
         * --------------------------------------------
         * To find where the debuggin files are, they are located in hte debug folder in hte bin file of the project in question
         * -----------------------------------------------------------------------------
         * 
         * --------------------------------------------------------------------------------------------------------------
         * 
         * - Need to remove More Info if I dont need it for test designing anymore
         * 
         * ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
         * 
         * This prints out every time zone that the users system has on it. This was used for testig purposes.
		 * foreach (TimeZoneInfo yeet in TimeZoneInfo.GetSystemTimeZones())
		 * 	Console.WriteLine(yeet.DisplayName);
         * 
         * //To get path of assembly.
         * Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Name);
         * 
         * Link for Some methods for the jikan Stuff: https://github.com/Ervie/jikan.net/wiki#usage-example
         * - going about a request policy of 120 request/minute will be blocked for 3 hours
         * - if you try to go above 300request/minute, your IP will be blocked for 12 hours 
         * - Keep the time between jikan uses 4 seconds at the minimum.
         * 
         */

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenu contextMenu;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.ComponentModel.IContainer components;

        public DataGrid[] dataGrids;
        private ObservableCollection<Anime>[] animeList;

        private CollectionViewSource[] collection = new CollectionViewSource[] { new CollectionViewSource(), new CollectionViewSource(), new CollectionViewSource(),
        new CollectionViewSource(), new CollectionViewSource(), new CollectionViewSource(), new CollectionViewSource()} ;

        public App CurrentApplication { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            
            // this will check for the file to exist, if it doesn't it will create one for the user.
            if (!File.Exists(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\useFont.txt"))
                File.AppendAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\useFont.txt", "./#CC Wild Words");

            Config.useFont = new FontFamily(File.ReadAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\useFont.txt"));

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;


            Config.selectedTab = 0;

            this.components = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            
            this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { this.menuItem1 }); // Initialize contextMenu.

            this.menuItem1.Index = 0; // Initialize menuItem1
            this.menuItem1.Text = "E&xit";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);

            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components); // Create the NotifyIcon.
            
            // The Icon property sets the icon that will appear in the systray for this application.
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location); //I believe this is better?
            notifyIcon.ContextMenu = this.contextMenu; // The ContextMenu property sets the menu that will appear when the systray icon is right clicked.
            notifyIcon.Text = "Anime Organizer"; // Set Text property to show the name of the Application.
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += new System.EventHandler(this.NotifyIcon_DoubleClick);

            // Start of actual anime adding
            animeList = Anime.indexAnimeList();
            dataGrids = new DataGrid[] { dataGrid1, dataGrid2, dataGrid3, dataGrid4, dataGrid5, dataGrid6, dataGrid7 };
            
            for (int i = 0; i < animeList.Length; i++)
            {
                collection[i].Source = animeList[i];
                collection[i].Filter += new FilterEventHandler(RowFilter);
                dataGrids[i].ItemsSource = collection[i].View;
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // This part is for the Settings Cog animation.
            Storyboard storyboard = new Storyboard();
            storyboard.Duration = new Duration(TimeSpan.FromSeconds(0.4));

            DoubleAnimation animation = new DoubleAnimation() { From = 0, To = -90, Duration = storyboard.Duration };

            Storyboard.SetTarget(animation, settingsCog);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));

            storyboard.Children.Add(animation);
            Resources.Add("Storyboard", storyboard);
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            
            notesTBox.Text = File.ReadAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\general notes.txt");
            notesTBox.TextChanged += notesTBox_TextChanged;
            notesTBox.CaretIndex = notesTBox.Text.Length;

            String jsonString = File.ReadAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\columns.json");
            List<List<int>> columnList = JsonSerializer.Deserialize<List<List<int>>>(jsonString);

            for (int i = 0; i < 7; i++)
            {
                List<int> tempList = columnList[i];

                for (int c = 0; c < 8; c++)
                    dataGrids[i].Columns[c].Width = tempList[c];
            }


            randomizer.Visibility = Visibility.Collapsed;

            /////////////////////////////////////////////////////
            // Past this point is testing of balloon tips

            /**
            // Was just fooling around testing this. Will probably not get used.
            notifyIcon1.BalloonTipClicked += NotifyIcon_BalloonTipClicked;
            notifyIcon1.ShowBalloonTip(3000, "Title Yeet", "This is a message", System.Windows.Forms.ToolTipIcon.Info);
             */

            // This is added more recently Just so I can discern if its using the correct profiles
            if (Config.environmentPath == Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Anime Organizer Test Folder\\")
                this.Title = "Anime Organizer Test Environment";


        }

        

        // Used when messing around with balloon tool tips.
        /**
        private void NotifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            //This is here for the balloon testing thing
            Console.WriteLine("This was ran in balloon testing");


            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
                this.Activate();
            }
        }
         */

        /// <summary>
        /// This saves the current info from the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save(object sender, RoutedEventArgs e)
        {
            Anime.Save();
        }

        /// <summary>
        /// Returns the String contained in notes so it can be saved.
        /// </summary>
        /// <returns></returns>
        public String getNotes()
        {
            return notesTBox.Text;
        }

        /// <summary>
        /// Sets savedChanges to false so it can warn the user if they attempt to close the program without saving.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notesTBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Config.changesSaved != false)
                Config.changesSaved = false;
        }

        /// <summary>
        /// If the window is either minimized or behind windows, attempts to pull window to the front.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_DoubleClick(object Sender, EventArgs e)
        {
            // Show the form when the user double clicks on the notify icon.
            if (this.WindowState == WindowState.Minimized)
                this.WindowState = WindowState.Normal;

            this.Activate(); // Activate the form.
        }

        /// <summary>
        /// This closes the program from the menuItem from the NotifyIcon in the system tray.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private void menuItem1_Click(object Sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// This will first check if any changes were made and if so to warn the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!Config.changesSaved)
            {
                MessageBoxResult result = MessageBox.Show("There are changes that need to be saved. Do you want to save?", "Anime Organizer", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    Anime.Save();
                    notifyIcon.Visible = false;
                }
                else if (result == MessageBoxResult.Cancel)
                    e.Cancel = true;
                else
                    notifyIcon.Visible = false;
            }
            else
                notifyIcon.Visible = false;
        }

        /// <summary>
        /// Opens a user control to add an anime.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Anime(object sender, RoutedEventArgs e)
        {
            AddAnime control = new AddAnime();
            editGrid.Children.Add(control);
            editGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Will search anime for nicknames with the given parameters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            collection[Config.selectedTab].View.Refresh();

            if (((TextBox)sender).Text.Length != 0)
            {
                // The last one (seventh is all anime) which does not need reordering.
                Style newStyle = (Style)FindResource("DataGridRowHeaderAlt");

                if (dataGrids[Config.selectedTab].RowHeaderStyle != newStyle)
                    dataGrids[Config.selectedTab].RowHeaderStyle = newStyle;
            }
            else
            {
                bool isAnySorted = false;

                foreach (DataGridColumn column in dataGrids[Config.selectedTab].Columns)
                {
                    if (column.SortDirection != null)
                        isAnySorted = true;
                }

                if (!isAnySorted && Config.selectedTab != 4 && Config.selectedTab != 6)
                    dataGrids[Config.selectedTab].RowHeaderStyle = (Style)FindResource("DataGridRowHeader");
            } 
        }

        /// <summary>
        /// This is the actual filter that determines if an anime will be visible or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RowFilter(object sender, FilterEventArgs e)
        {
            var obj = (Anime) e.Item;

            if (obj != null)
            {
                if (obj.NickNameTitle.ToLower().Contains(searchAnime.Text.ToLower()))
                    e.Accepted = true;
                else
                    e.Accepted = false;
            }
        }

        /// <summary>
        /// Opens up a user control to choose a random anime.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Randomizer_Click(object sender, RoutedEventArgs e)
        {
            //This will use the editGrid thingy to run the new window
            RandomAnime control = new RandomAnime();
            editGrid.Children.Add(control);
            editGrid.Visibility = Visibility.Visible;

        }

        /// <summary>
        /// Opens a user control with addditional settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_and_Info_Click(object sender, RoutedEventArgs e)
        {
            Settings control = new Settings(dataGrids, Config.selectedTab);
            editGrid.Children.Add(control);
            editGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Starts an animation once the Context Menu of the button closes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Closed(object sender, RoutedEventArgs e)
        {
            ((Storyboard)Resources["Storyboard"]).Begin();
        }

        /// <summary>
        /// Keeps track of the currently selected tab in a variable, as well as making sure nothing breaks when moving between them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex == 7)
            {
                dataGrids[Config.selectedTab].UnselectAllCells();
                searchAnime.Text = "";
                Config.selectedTab = tabControl.SelectedIndex;
            }
            else if (tabControl.SelectedIndex != 7 && Config.selectedTab != 7)
            {
                dataGrids[Config.selectedTab].UnselectAllCells();
                searchAnime.Text = "";
                Config.selectedTab = tabControl.SelectedIndex;
                dataGrids[Config.selectedTab].Items.Refresh();
            }
            else
            {
                searchAnime.Text = "";
                Config.selectedTab = tabControl.SelectedIndex;
                dataGrids[Config.selectedTab].Items.Refresh();
            }

            if (tabControl.SelectedIndex == 3)
                randomizer.Visibility = Visibility.Visible;
            else
                randomizer.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// This numbers the Row Headers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        /// <summary>
        /// This checks which column started the edit, and will open the necessary user control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (String.Equals((String) dataGrids[Config.selectedTab].CurrentCell.Column.Header, "Name", StringComparison.CurrentCultureIgnoreCase))
            {
                EditName control = new EditName(dataGrids[Config.selectedTab].SelectedCells[0].Item);
                dataGrids[Config.selectedTab].UnselectAll();
                editGrid.Children.Add(control);
                editGrid.Visibility = Visibility.Visible;
            }
            else if (String.Equals((String)dataGrids[Config.selectedTab].CurrentCell.Column.Header, "Website", StringComparison.CurrentCultureIgnoreCase))
            {
                EditWebsite control = new EditWebsite(dataGrids[Config.selectedTab].SelectedCells[0].Item);
                dataGrids[Config.selectedTab].UnselectAll();
                editGrid.Children.Add(control);
                editGrid.Visibility = Visibility.Visible;
            }
            else if (String.Equals((String)dataGrids[Config.selectedTab].CurrentCell.Column.Header, "Score", StringComparison.CurrentCultureIgnoreCase))
            {
                EditScore control = new EditScore(dataGrids[Config.selectedTab].SelectedCells[0].Item);
                dataGrids[Config.selectedTab].UnselectAll();
                editGrid.Children.Add(control);
                editGrid.Visibility = Visibility.Visible;
            }
            else if (String.Equals((String)dataGrids[Config.selectedTab].CurrentCell.Column.Header, "Last Watched", StringComparison.CurrentCultureIgnoreCase))
            {
                EditLastWatched control = new EditLastWatched(dataGrids[Config.selectedTab].SelectedCells[0].Item);
                dataGrids[Config.selectedTab].UnselectAll();
                editGrid.Children.Add(control);
                editGrid.Visibility = Visibility.Visible;
            }
            else if (String.Equals((String)dataGrids[Config.selectedTab].CurrentCell.Column.Header, "Day", StringComparison.CurrentCultureIgnoreCase))
            {
                EditDay control = new EditDay(dataGrids[Config.selectedTab].SelectedCells[0].Item);
                dataGrids[Config.selectedTab].UnselectAll();
                editGrid.Children.Add(control);
                editGrid.Visibility = Visibility.Visible;

            }
            else if (String.Equals((String)dataGrids[Config.selectedTab].CurrentCell.Column.Header, "Notes", StringComparison.CurrentCultureIgnoreCase))
            {
                EditNotes control = new EditNotes(dataGrids[Config.selectedTab].SelectedCells[0].Item);
                dataGrids[Config.selectedTab].UnselectAll();
                editGrid.Children.Add(control);
                editGrid.Visibility = Visibility.Visible;
            }

            e.Cancel = true;
        }

        /// <summary>
        /// Checks if the "More Info" column was chosen. If so then row details is shown for that row only.
        /// Props to this man on figuring it out to work with single selection: https://stackoverflow.com/questions/3436973/displaying-rowdetails-programmatically
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (e.RemovedCells.Count != 0)
            {
                DataGridRow row = (DataGridRow)dataGrids[Config.selectedTab].ItemContainerGenerator.ContainerFromItem(e.RemovedCells[0].Item);

                if (row != null && row.DetailsVisibility == Visibility.Visible)
                {
                    row.DetailsVisibility = Visibility.Collapsed;
                    currentScrollViewer = null;
                }
            }

            // This should never call a null, but you never know.
            if (e.AddedCells.Count != 0 && e.AddedCells[0].Column.DisplayIndex == 0)
                ((DataGridRow)dataGrids[Config.selectedTab].ItemContainerGenerator.ContainerFromItem(e.AddedCells[0].Item)).DetailsVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Changes the deault datagrid behavior so it will only begin edit when the enter key is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                dataGrids[Config.selectedTab].BeginEdit();
            else if (!(e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right))
                e.Handled = true;
        }

        /// <summary>
        /// This is for sorting the datagrids via the column headers, you can sort starting at default; "Natural", "Ascending", and "Descending".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (!(((DataGrid)sender).Name.Equals("dataGrid5") || ((DataGrid)sender).Name.Equals("dataGrid7")))
            {
                // This is here to avoid manual user sorting when the column is sorted.
                if (((DataGridTextColumn)e.Column).SortDirection != ListSortDirection.Descending)
                    dataGrids[Config.selectedTab].RowHeaderStyle = (Style)FindResource("DataGridRowHeaderAlt");
                else if (searchAnime.Text.Length == 0)
                    dataGrids[Config.selectedTab].RowHeaderStyle = (Style)FindResource("DataGridRowHeader");
            }

            // If this is descending, that means we are going from descending to natural.
            if (((DataGridTextColumn)e.Column).SortDirection == ListSortDirection.Descending)
            {
                e.Handled = true; // This is to prevent the in-house sorting to occur.
                ((DataGridTextColumn)e.Column).SortDirection = null;
                ((ICollectionView)dataGrids[Config.selectedTab].ItemsSource).SortDescriptions.Clear();
            }
        }

        /// <summary>
        /// Opens a user control that can move an anime to another category.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Move(object sender, RoutedEventArgs e)
        {
            if (dataGrids[Config.selectedTab].SelectedCells.Count != 0)
            {
                MoveAnime control = new MoveAnime(dataGrids[Config.selectedTab].SelectedCells[0].Item);
                editGrid.Children.Add(control);
                editGrid.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Opens a user control that allows the user to edit that anime.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Edit_Anime_Click(object sender, RoutedEventArgs e)
        {
            EditAnime control = new EditAnime(dataGrids[Config.selectedTab].SelectedCells[0].Item, Config.selectedTab);
            dataGrids[Config.selectedTab].UnselectAll();
            editGrid.Children.Add(control);
            editGrid.Visibility = Visibility.Visible;

            this.MinHeight = 450;
        }

        /// <summary>
        /// This will move backwards in seasons by 1 unless its already at the first index.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_One_Season(object sender, RoutedEventArgs e)
        {
            Anime anime = (Anime) dataGrids[Config.selectedTab].SelectedCells[0].Item;
            int index = anime.CurrentSeasonIndex - 1;

            if (index != -1)
            {
                anime.CurrentSeason = anime.Seasons[index];
                anime.CurrentSeasonIndex = index;
            }
        }

        /// <summary>
        /// This will move forwards in seasons by 1 unless its already at the last index.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Forward_One_Season(object sender, RoutedEventArgs e)
        {
            Anime anime = (Anime) dataGrids[Config.selectedTab].SelectedCells[0].Item;
            int index = anime.CurrentSeasonIndex + 1;

            if (index < anime.Seasons.Count)
            {
                anime.CurrentSeason = anime.Seasons[index];
                anime.CurrentSeasonIndex = index;
            }

        }

        /// <summary>
        /// Attempts to close this profile to go back to profile selection window. Will give user a warning if they have not saved changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Change_Profile_Click(object sender, RoutedEventArgs e)
        {
            if (!Config.changesSaved)
            {
                MessageBoxResult result = MessageBox.Show("There are changes that need to be saved. Do you want to save?", "Anime Organizer", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    Anime.Save();

                    Profiles win = new Profiles();
                    win.CurrentApplication = this.CurrentApplication;
                    this.CurrentApplication.MainWindow = win;
                    win.Show();

                    notifyIcon.Visible = false;

                    this.Close();
                }
                else if (result == MessageBoxResult.No)
                {
                    Config.changesSaved = true;

                    Profiles win = new Profiles();
                    win.CurrentApplication = this.CurrentApplication;
                    this.CurrentApplication.MainWindow = win;
                    win.Show();

                    notifyIcon.Visible = false;

                    this.Close();
                }
            }
            else
            {
                Profiles win = new Profiles();
                win.CurrentApplication = this.CurrentApplication;
                this.CurrentApplication.MainWindow = win;
                win.Show();

                this.Close();
            }

        }

        /// <summary>
        /// Will play the system sound "Beep" if user attempts to click in an area they shouldn't.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editGridAndWebsiteGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (editGrid.Visibility == Visibility.Visible || websiteGrid.Visibility == Visibility.Visible)
                SystemSounds.Beep.Play();
        }

        /// <summary>
        /// To avoid tab navigation issues, this will enable/disable controls as necessary when this grid is visible/invisible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // If true, it removes all other controls from this section from the tab thing
            if ((bool)e.NewValue)
            {
                ((TabItem)tabControl.Items[Config.selectedTab]).IsTabStop = false;

                if (Config.selectedTab == 7)
                    notesTBox.IsEnabled = false;
                else
                    dataGrids[Config.selectedTab].IsEnabled = false;
            }
            else
            {
                ((TabItem)tabControl.Items[Config.selectedTab]).IsTabStop = true;

                if (Config.selectedTab == 7)
                    notesTBox.IsEnabled = true;
                else
                {
                    dataGrids[Config.selectedTab].IsEnabled = true;
                    dataGrids[Config.selectedTab].Items.Refresh();
                }
            }
        }

        /// <summary>
        /// When the grid is invisible, it refreshes a websites combobox items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void websiteGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                websiteVar.websiteCBox.Text = "";
                websiteVar.websiteCBox.Items.Clear();
                websiteVar.CreateItems();
            }
        }

        /// <summary>
        /// The Websites variable to help with information movement/flow.
        /// </summary>
        public Websites websiteVar { get; set; }

        /// <summary>
        /// Returns the website grid to allow ease of access to put controls in them.
        /// </summary>
        /// <returns></returns>
        public Grid getWebsiteGrid()
        {
            return websiteGrid;
        }

        /// <summary>
        /// Runs after the window is fully loaded and finishes up with checking for military time and if it needs to change themes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, EventArgs e)
        {
            bool isLightMode = bool.Parse(File.ReadAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\light mode.txt"));

            if (!isLightMode)
                CurrentApplication.SetTheme(App.Theme.Dark);

            // This is for militaryTime
            Config.isMilitaryTime = bool.Parse(File.ReadAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\military time.txt"));

            this.Activate();
        }


        private int scrollValuePos = 0, scrollValueNeg = 0;

        private DispatcherTimer timer;
        private double timerNum = 0.0;

        private childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        

        private void LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            currentScrollViewer = e.DetailsElement.FindName("scrollViewer") as ScrollViewer;
        }

        private ScrollViewer currentScrollViewer;

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (currentScrollViewer == null) //True if no Row Details is shown
            {
                if (timer == null)
                {
                    timer = new DispatcherTimer();
                    timer.Tick += Timer_Tick;
                    timer.Interval = TimeSpan.FromMilliseconds(100);
                    timer.Start();
                }
                else
                    timerNum = 0.0;

                e.Handled = true;

                if (e.Delta > 0)
                    scrollValuePos += e.Delta;
                else
                    scrollValueNeg += e.Delta;


                if (scrollValuePos >= 20) //true if scrolling is upwards
                {
                    var scrollViewer = GetScrollViewer(dataGrids[Config.selectedTab]) as ScrollViewer;

                    if (scrollViewer != null)
                        scrollViewer.LineUp();

                    scrollValuePos = 0;
                }
                else if (scrollValueNeg <= -20)
                {
                    var scrollViewer = GetScrollViewer(dataGrids[Config.selectedTab]) as ScrollViewer;

                    if (scrollViewer != null)
                        scrollViewer.LineDown();

                    scrollValueNeg = 0;
                }
            }
        }

        // This FINALLY fucking got it so it can scroll to a normal speed now. There can definitely be some added math behind it, but for now I'm happy.
        //https://stackoverflow.com/questions/1009036/how-can-i-programmatically-scroll-a-wpf-listview
        public static DependencyObject GetScrollViewer(DependencyObject o)
        {
            // Return the DependencyObject if it is a ScrollViewer
            if (o is ScrollViewer)
            { return o; }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result == null)
                    continue;
                else
                    return result;
            }
            return null;
        }

        

        private void Timer_Tick(object sender, EventArgs e)
        {
            timerNum += 0.1;

            if (timerNum == 1.0)
            {
                timer.Stop();
                timer = null;
                scrollValuePos = 0;
                scrollValueNeg = 0;
            }
        }


    }
}
