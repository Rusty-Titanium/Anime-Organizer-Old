using Anime_Organizer.Classes;
using Anime_Organizer.Windows.Adding;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Anime_Organizer.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for SearchForAnime.xaml
    /// </summary>
    public partial class SearchForAnime : UserControl
    {
        private List<ICollection<AnimeSearchEntry>> collectionOfPages = new List<ICollection<AnimeSearchEntry>>();
        private List<CheckBox> listOfCBox = new List<CheckBox>();
        private AnimeSearchConfig animeSearchConfig = new AnimeSearchConfig(){};

        private Anime anime;
        private bool isAnime = true, finishedInitializing = false; // The ReEnable() Method breaks when this control is initialized, so this is here to stop that.

        private FiveSecondTimer timer;

        // Older Values so you can change between pages without any issues.
        private String oldQuery = "";
        private AnimeSearchConfig oldSearchConfig = new AnimeSearchConfig(){};

        public SearchForAnime(bool isAnime, Object obj)
        {
            InitializeComponent();

            this.isAnime = isAnime;
            anime = (Anime)obj;

            foreach (Border border in genreWPanel.Children)
                listOfCBox.Add((CheckBox)border.Child);

            filterBorder.Visibility = Visibility.Collapsed;
            pageBorder.Visibility = Visibility.Collapsed;

            cooldown.Visibility = Visibility.Collapsed;
            timer = new FiveSecondTimer(cooldown, this);

            type.typeCBox.SelectedIndex = 0;
            type.typeCBox.Width = 150;
            type.typeCBox.SelectionChanged += Type_SelectionChanged;

            // This binds the comboBoxes to its appropriate enum value.

            Array tempArray1 = Enum.GetValues(typeof(GenreSearch));
            GenreSearch[] tempArray2 = new GenreSearch[45];

            for (int i = 0; i < tempArray1.Length; i++)
                tempArray2.SetValue(tempArray1.GetValue(i), i);

            List<GenreSearch> tempList = tempArray2.OrderBy(genre => genre.ToString()).ToList();

            for (int i = 0; i < genreWPanel.Children.Count; i++)
                ((CheckBox)((Border)genreWPanel.Children[i]).Child).DataContext = tempList[i];

            //////////////////////////////////////////

            tempArray1 = Enum.GetValues(typeof(AnimeType));

            for (int i = 0; i < type.typeCBox.Items.Count; i++)
                ((ComboBoxItem)type.typeCBox.Items[i]).DataContext = tempArray1.GetValue(i);

            scrollViewer2.PreviewMouseWheel += new ScrollViewerTimer().PreviewMouseWheel; // This is for the non datagrid scrollviewer

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }


        /// <summary>
        /// Runs when the timer is finished.
        /// </summary>
        public void Timer_Finished()
        {
            searchButton.IsEnabled = canReEnable();
            pageRightButton.IsEnabled = true;
            dataGridContextMenu.IsEnabled = true;
            ((System.Windows.Shapes.Path)pageRightButton.Content).Fill = (Brush)FindResource("ControlForeground");
        }


        /// <summary>
        /// Changes visibility for the advanced search filters and seeing the datagrid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Advanced_Search_Click(object sender, RoutedEventArgs e)
        {
            // It will check if something is invisible or not, if its invisible, make it visible and vice versa
            if (filterBorder.Visibility == Visibility.Visible)
            {
                filterBorder.Visibility = Visibility.Collapsed;
                dataGrid.Visibility = Visibility.Visible;
            }
            else
            {
                filterBorder.Visibility = Visibility.Visible;
                dataGrid.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Searches using the current Anime Search Config settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dataGrid.IsEnabled = false;
                dataGridContextMenu.IsEnabled = false;
                searchButton.IsEnabled = false;
                pageRightButton.IsEnabled = false;
                ((System.Windows.Shapes.Path)pageRightButton.Content).Fill = (Brush)FindResource("ControlDisabledForeground");

                AnimeSearchResult animeSearchResult = await new Jikan().SearchAnime(queryTBox.Text, animeSearchConfig);
                selectedPage.Text = "1";
                totalPages.Text = animeSearchResult.ResultLastPage.ToString();
                
                
                filterBorder.Visibility = Visibility.Collapsed;
                collectionOfPages.Clear();

                collectionOfPages.Add(animeSearchResult.Results);
                dataGrid.IsEnabled = true;
                dataGrid.ItemsSource = collectionOfPages[0];
                dataGrid.Visibility = Visibility.Visible;
                pageBorder.Visibility = Visibility.Visible;

                cooldown.Visibility = Visibility.Visible;
                timer.Start();

                oldQuery = queryTBox.Text;
                oldSearchConfig = animeSearchConfig;
            }
            catch (Exception ex) when (ex is WebException || ex is HttpRequestException)
            {
                ExceptionMessage.InternetError();

                dataGrid.IsEnabled = true;
                dataGridContextMenu.IsEnabled = true;
                searchButton.IsEnabled = true;
                pageRightButton.IsEnabled = true;
                ((System.Windows.Shapes.Path)pageRightButton.Content).Fill = (Brush)FindResource("ControlForeground");
            }
        }

        /// <summary>
        /// This control closes and goes back to the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Visibility = Visibility.Collapsed;
            ((Grid)this.Parent).Children.Remove(this);
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
        /// Checks if the "More Info" column was chosen. If so then row details is shown for that row only.
        /// Props to this man on figuring it out to work with single selection: https://stackoverflow.com/questions/3436973/displaying-rowdetails-programmatically
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (e.RemovedCells.Count != 0)
            {
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(e.RemovedCells[0].Item);

                if (row != null && row.DetailsVisibility == Visibility.Visible)
                {
                    row.DetailsVisibility = Visibility.Collapsed;
                    currentScrollViewer = null;
                }

            }

            // This should never call a null, but you never know.
            if (e.AddedCells.Count != 0 && e.AddedCells[0].Column.DisplayIndex == 0)
                ((DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(e.AddedCells[0].Item)).DetailsVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Page forward 1 page if allowed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Right_Click(object sender, RoutedEventArgs e)
        {
            filterBorder.Visibility = Visibility.Collapsed;
            dataGrid.Visibility = Visibility.Visible;

            int currentIndex = int.Parse(selectedPage.Text);

            try
            {
                if (currentIndex < int.Parse(totalPages.Text))
                {
                    currentIndex++;

                    if (currentIndex > collectionOfPages.Count) // True if it needs to search with Jikan again.
                    {
                        dataGrid.IsEnabled = false;
                        dataGridContextMenu.IsEnabled = false;
                        searchButton.IsEnabled = false;
                        pageRightButton.IsEnabled = false;
                        ((System.Windows.Shapes.Path)pageRightButton.Content).Fill = (Brush)FindResource("ControlDisabledForeground");

                        AnimeSearchResult animeSearchResult = await new Jikan().SearchAnime(oldQuery, currentIndex, oldSearchConfig);
                        collectionOfPages.Add(animeSearchResult.Results);
                        dataGrid.IsEnabled = true;
                        dataGrid.ItemsSource = collectionOfPages[currentIndex - 1];

                        cooldown.Visibility = Visibility.Visible;
                        timer.Start();
                    }
                    else // Runs if the list already exists and just needs to be applied.
                    {
                        dataGrid.ItemsSource = collectionOfPages[currentIndex - 1];
                    }

                    selectedPage.Text = (currentIndex).ToString();
                }
            }
            catch (Exception ex) when (ex is WebException || ex is HttpRequestException)
            {
                ExceptionMessage.InternetError();

                dataGrid.IsEnabled = true;
                dataGridContextMenu.IsEnabled = true;
                searchButton.IsEnabled = true;
                pageRightButton.IsEnabled = true;
                ((System.Windows.Shapes.Path)pageRightButton.Content).Fill = (Brush)FindResource("ControlForeground");
            }
        }

        /// <summary>
        /// Page back 1 page if allowed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Left_Click(object sender, RoutedEventArgs e)
        {
            int currentIndex = int.Parse(selectedPage.Text);

            if (currentIndex != 1)
            {
                filterBorder.Visibility = Visibility.Collapsed;
                dataGrid.Visibility = Visibility.Visible;

                currentIndex--;
                selectedPage.Text = currentIndex.ToString();
                dataGrid.ItemsSource = collectionOfPages[currentIndex - 1];
            }
        }

        /// <summary>
        /// Returns a bool value if the search button can be Enabled.
        /// </summary>
        /// <returns></returns>
        private bool canReEnable()
        {
            bool validParameters = false;

            foreach (CheckBox cBox in listOfCBox)
            {
                if (cBox.IsChecked == true)
                {
                    validParameters = true;
                    break;
                }
            }

            if ((validParameters || queryTBox.Text.Length > 2 || scoreCBox.SelectedIndex != 0 || statusCBox.SelectedIndex != 0 || ratingCBox.SelectedIndex != 0) && timer.timerNum == 5.0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Change the rating in the Anime Search Config.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rating_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ratingCBox.SelectedIndex)
            {
                case 0:
                    animeSearchConfig.Rating = AgeRating.EveryRating;
                    break;
                case 1:
                    animeSearchConfig.Rating = AgeRating.G;
                    break;
                case 2:
                    animeSearchConfig.Rating = AgeRating.PG;
                    break;
                case 3:
                    animeSearchConfig.Rating = AgeRating.PG13;
                    break;
                case 4:
                    animeSearchConfig.Rating = AgeRating.R17;
                    break;
                case 5:
                    animeSearchConfig.Rating = AgeRating.R;
                    break;
                case 6:
                    animeSearchConfig.Rating = AgeRating.RX;
                    break;
            }

            if (finishedInitializing)
                searchButton.IsEnabled = canReEnable();
        }

        /// <summary>
        /// Changes the status in the Anime Search Config.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (statusCBox.SelectedIndex)
            {
                case 0:
                    animeSearchConfig.Status = AiringStatus.EveryStatus;
                    break;
                case 1:
                    animeSearchConfig.Status = AiringStatus.Airing;
                    break;
                case 2:
                    animeSearchConfig.Status = AiringStatus.Completed;
                    break;
                case 3:
                    animeSearchConfig.Status = AiringStatus.Upcoming;
                    break;
            }

            if (finishedInitializing)
                searchButton.IsEnabled = canReEnable();
        }

        /// <summary>
        /// Changes the score in the Anime Search Config.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Score_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (scoreCBox.SelectedIndex)
            {
                case 0:
                    animeSearchConfig.Score = null;
                    break;
                case 1:
                    animeSearchConfig.Score = 10;
                    break;
                case 2:
                    animeSearchConfig.Score = 9;
                    break;
                case 3:
                    animeSearchConfig.Score = 8;
                    break;
                case 4:
                    animeSearchConfig.Score = 7;
                    break;
                case 5:
                    animeSearchConfig.Score = 6;
                    break;
                case 6:
                    animeSearchConfig.Score = 5;
                    break;
                case 7:
                    animeSearchConfig.Score = 4;
                    break;
                case 8:
                    animeSearchConfig.Score = 3;
                    break;
                case 9:
                    animeSearchConfig.Score = 2;
                    break;
                case 10:
                    animeSearchConfig.Score = 1;
                    break;
            }

            if (finishedInitializing)
                searchButton.IsEnabled = canReEnable();
        }

        /// <summary>
        /// Changes the Type in the Anime Search Config.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (finishedInitializing)
                animeSearchConfig.Type = (AnimeType)((ComboBoxItem)((ComboBox)sender).SelectedItem).DataContext;
        }

        /// <summary>
        /// Add a Genre to the list given the name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            animeSearchConfig.Genres.Add((GenreSearch)((CheckBox)sender).DataContext);

            if (finishedInitializing)
                searchButton.IsEnabled = canReEnable();
        }

        /// <summary>
        /// Removes a Genre from the list given the name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            animeSearchConfig.Genres.Remove((GenreSearch)((CheckBox)sender).DataContext);

            if (finishedInitializing)
                searchButton.IsEnabled = canReEnable();
        }

        /// <summary>
        /// Runs when text is typed into it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Query_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchButton.IsEnabled = canReEnable();
        }

        /// <summary>
        /// Changes Selection on how genres will be included or excluded in an anime search.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Include_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (includeCBox.SelectedIndex)
            {
                case 0:
                    animeSearchConfig.GenreIncluded = true;
                    break;
                case 1:
                    animeSearchConfig.GenreIncluded = false;
                    break;
            }
        }

        /// <summary>
        /// Runs to update information or close depending on what occured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue) //If visible, tab stop other controls.
            {
                closeButton.IsTabStop = false;
                queryTBox.IsTabStop = false;
                searchButton.IsTabStop = false;
                advancedSearchButton.IsTabStop = false;
                pageLeftButton.IsTabStop = false;
                pageRightButton.IsTabStop = false;
                dataGrid.IsEnabled = false;
            }
            else
            {
                closeButton.IsTabStop = true;
                queryTBox.IsTabStop = true;
                searchButton.IsTabStop = true;
                advancedSearchButton.IsTabStop = true;
                pageLeftButton.IsTabStop = true;
                pageRightButton.IsTabStop = true;
                dataGrid.IsEnabled = true;
            }
        }

        /// <summary>
        /// Runs once this control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchForAnime_Loaded(object sender, RoutedEventArgs e)
        {
            queryTBox.Focus();
            finishedInitializing = true;
        }

        /// <summary>
        /// This will attempt to open another control while getting the anime from Jikan for the user to then add to their list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Anime_Chosen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isAnime)
                    editGrid.Children.Add(new AddSearchedAnime(this, await new Jikan().GetAnime(((AnimeSearchEntry)dataGrid.SelectedCells[0].Item).MalId)));
                else
                    editGrid.Children.Add(new AddSearchedSeason(this, await new Jikan().GetAnime(((AnimeSearchEntry)dataGrid.SelectedCells[0].Item).MalId), anime));

                editGrid.Visibility = Visibility.Visible;
            }
            catch (Exception ex) when (ex is WebException || ex is HttpRequestException)
            {
                ExceptionMessage.InternetError();
            }
        }



        private int scrollValuePos = 0, scrollValueNeg = 0;

        private DispatcherTimer dispatcherTimer;
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
                if (dispatcherTimer == null)
                {
                    dispatcherTimer = new DispatcherTimer();
                    dispatcherTimer.Tick += Timer_Tick;
                    dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
                    dispatcherTimer.Start();
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
                    var scrollViewer = GetScrollViewer(dataGrid) as ScrollViewer;

                    if (scrollViewer != null)
                        scrollViewer.LineUp();

                    scrollValuePos = 0;
                }
                else if (scrollValueNeg <= -20)
                {
                    var scrollViewer = GetScrollViewer(dataGrid) as ScrollViewer;

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
                dispatcherTimer.Stop();
                dispatcherTimer = null;
                scrollValuePos = 0;
                scrollValueNeg = 0;

            }
        }






    }




    
    class ImageURLConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // This needs to check if the value is legal.

            if (Uri.TryCreate((String)value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                return value;
            else
                return Config.environmentPath + "Image Cache\\errorImage.jpg";
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    



}
