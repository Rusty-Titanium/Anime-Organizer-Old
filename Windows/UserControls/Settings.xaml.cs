using Anime_Organizer.Classes;
using Anime_Organizer.MineSweeper;
using Anime_Organizer.Windows.Edits;
using Anime_Organizer.Windows.Other;
using Anime_Organizer.Windows.Startup;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Media;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Anime_Organizer.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        private DataGrid[] dataGrids;
        private bool minesweeperIsRunning = false;

        public Settings(Object obj, int tab)
        {
            InitializeComponent();

            dataGrids = (DataGrid[])obj;

            bool isLightMode = bool.Parse(File.ReadAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\light mode.txt"));

            if (!isLightMode)
                DarkModeCheckBox.IsChecked = true;


            Config.isMilitaryTime = bool.Parse(File.ReadAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\military time.txt"));

            if (Config.isMilitaryTime)
                MilitaryTimeCheckBox.IsChecked = true;

            installTBox.Text = AppDomain.CurrentDomain.BaseDirectory;
            saveFileTBox.Text = Config.environmentPath;

            // These 2 rectangles are translucent and infront of the textboxs (which are disabled) so they act as a middle man by holding the tooltip.
            ToolTip toolTip = new ToolTip();
            toolTip.Content = installTBox.Text;
            toolTip.Width = 600;
            rec1.ToolTip = toolTip;

            toolTip.Content = saveFileTBox.Text;
            toolTip.Width = 600;
            rec2.ToolTip = toolTip;

            // everything under here is for the font thing

            foreach (FontFamily font in Fonts.SystemFontFamilies)
                fontCBox.Items.Add(font.Source);

            if (Config.useFont.Source == "./#CC Wild Words")
                fontCBox.SelectedIndex = 0;
            else
                fontCBox.SelectedItem = Config.useFont.Source;
        }

        /// <summary>
        /// Closes this control to get back to main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Settings_Click(object sender, RoutedEventArgs e)
        {
            Grid grid = (Grid)this.Parent;

            grid.Children.Remove(this);
            grid.Visibility = Visibility.Collapsed;

            if (Config.selectedTab < 7)
                dataGrids[Config.selectedTab].Items.Refresh();
        }

        /// <summary>
        /// If the user confirms, this will remove all data in the current profile and reset. the user will then be brought back t the choose profile screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Profile_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete your current profile?", "Anime Organizer", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (MessageBox.Show("Are you sure? This will remove all data immediately.", "Anime Organizer", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    DirectoryInfo directory = new DirectoryInfo(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\nonMAL Image Cache");

                    // This will delete all the extra files ni the nonMAL folder
                    foreach (FileInfo file in directory.GetFiles())
                    {
                        file.Delete();
                    }

                    Anime.animeLists[0] = new ObservableCollection<Anime>();
                    Anime.animeLists[1] = new ObservableCollection<Anime>();
                    Anime.animeLists[2] = new ObservableCollection<Anime>();
                    Anime.animeLists[3] = new ObservableCollection<Anime>();
                    Anime.animeLists[4] = new ObservableCollection<Anime>();
                    Anime.animeLists[5] = new ObservableCollection<Anime>();
                    Anime.animeLists[6] = new ObservableCollection<Anime>();

                    var options = new JsonSerializerOptions { WriteIndented = true };
                    String jsonString = JsonSerializer.Serialize(Anime.animeLists, options);

                    File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\animelist.json", jsonString);

                    File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\columns.json",
                    "[\n  [60, 200, 200, 75, 200, 200, 200, 129],\n  " +
                    "[60, 200, 200, 75, 200, 200, 200, 129],\n  " +
                    "[60, 200, 200, 75, 200, 200, 200, 129],\n  " +
                    "[60, 200, 200, 75, 200, 200, 200, 129],\n  " +
                    "[60, 200, 200, 75, 200, 200, 200, 129],\n  " +
                    "[60, 200, 200, 75, 200, 200, 200, 129],\n  " +
                    "[60, 200, 200, 75, 200, 200, 200, 129]\n]");

                    File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\general notes.txt", "");
                    File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\light mode.txt", "true");
                    File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\military time.txt", "false");
                    File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\nonMALid.txt", "0");
                    File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\profilename.txt", "New Profile");
                    File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\scoringsystem.json", "[]");
                    File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\websites.json", "[]");
                    File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\useFont.txt", "./#CC Wild Words");

                    File.WriteAllText(Config.environmentPath + "Startup Profile.txt", "0");

                    Config.profileNumber = 0;
                    Config.changesSaved = true;
                    Config.selectedTab = 0;

                    MainWindow oldWindow = (MainWindow)App.Current.MainWindow;

                    Profiles win = new Profiles();
                    win.CurrentApplication = oldWindow.CurrentApplication;
                    oldWindow.CurrentApplication.MainWindow = win;
                    win.Show();

                    oldWindow.Close();
                }
            }
        }

        /// <summary>
        /// Opens a user control to allow to edit the scoring system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit_Scoring_System_Click(object sender, RoutedEventArgs e)
        {
            EditScoringSystem control = new EditScoringSystem();
            editGrid.Children.Add(control);
            editGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Opens a user control to be able to refresh the anime in the selected category.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshAnime control = new RefreshAnime();
            editGrid.Children.Add(control);
            editGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Opens a user control to play the classic game, Minesweeper
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Minesweeper_Click(object sender, RoutedEventArgs e)
        {
            if (minesweeperIsRunning)
                MessageBox.Show("There is already an instance of mine sweeper running.", "Anime Organizer", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                minesweeperIsRunning = true;
                MinesweeperWindow window = new MinesweeperWindow(this);
                window.Show();
            }
        }

        public void Minesweeper_Is_Closed()
        {
            minesweeperIsRunning = false;
        }
        
        // This answer helped me get this: https://stackoverflow.com/questions/48318107/modify-properties-inside-a-style-programmatically
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ((MainWindow)App.Current.MainWindow).CurrentApplication.SetTheme(App.Theme.Dark);
            File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\light mode.txt", "false");
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ((MainWindow)App.Current.MainWindow).CurrentApplication.SetTheme(App.Theme.Light);
            File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\light mode.txt", "true");
        }

        private void MilitaryTimeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Config.isMilitaryTime = true;
            File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\military time.txt", "true");
        }

        private void MilitaryTimeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Config.isMilitaryTime = false;
            File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\military time.txt", "false");
        }

        /// <summary>
        /// Saves the information of the column widths to use for the next time the program is launched.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Column_Save_Click(object sender, RoutedEventArgs e)
        {
            // Columns should all be there for each category, though they will not be visible.

            List<List<int>> columnList = new List<List<int>>();

            foreach (DataGrid datagrid in dataGrids)
            {
                List<int> tempList = new List<int>();

                foreach (DataGridColumn column in datagrid.Columns)
                    tempList.Add((int)column.ActualWidth);

                columnList.Add(tempList);
            }

            String jsonString = JsonSerializer.Serialize(columnList);
            File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\columns.json", jsonString);
        }

        /// <summary>
        /// Plays the system sound "Beep" when the user clicks somewhere that is uninteractable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (editGrid.Visibility == Visibility.Visible)
                SystemSounds.Beep.Play();
        }

        /// <summary>
        /// Runs to make sure there is no tab navigation problems.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // If true, it removes all other controls from this section from the tab thing
            if ((bool)e.NewValue)
            {
                backButton.IsTabStop = false;
                saveButton.IsTabStop = false;
                editButton.IsTabStop = false;
                refreshButton.IsTabStop = false;
                deleteButton.IsTabStop = false;
            }
            else
            {
                backButton.IsTabStop = true;
                saveButton.IsTabStop = true;
                editButton.IsTabStop = true;
                refreshButton.IsTabStop = true;
                deleteButton.IsTabStop = true;
            }
        }

        /// <summary>
        /// Focus a button once this control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            // Focuses the first control the user can edit.
            backButton.Focus();
        }

        private void FontCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fontCBox.SelectedIndex == 0)
                Config.useFont = new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words");
            else
                Config.useFont = new FontFamily((String)fontCBox.Items.GetItemAt(fontCBox.SelectedIndex));

            this.FontFamily = Config.useFont;
            ((MainWindow)Application.Current.MainWindow).FontFamily = Config.useFont;

            File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\useFont.txt", Config.useFont.Source);
        }

        private void Information_Click(object sender, RoutedEventArgs e)
        {
            // Runs a window with showdialog 



        }
    }
}
