using Anime_Organizer.Classes;
using AO_Random_Search_Gen;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Anime_Organizer.Windows.Other
{
    /// <summary>
    /// Interaction logic for RandomAnime.xaml
    /// </summary>
    public partial class RandomAnime : UserControl
    {
        public Object AnimeData { get; set; }

        private List<int> animeIDs = new List<int>(), recommendedIDs = new List<int>(), currentFilteredList = new List<int>();
        private List<String> typeList = new List<String>();
        private List<AnimeSimplified> totalAnimeList = new List<AnimeSimplified>();
        private Anime tempAnime;

        private FiveSecondTimer timer;
        private bool searchChanged = true;

        public RandomAnime()
        {
            InitializeComponent();

            for (int i = 0; i < 6; i++)
            {
                foreach (Anime anime in Anime.animeLists[i])
                {
                    foreach (Season season in anime.Seasons)
                    {
                        if (!season.MalId.StartsWith("A") && i != 3)
                            animeIDs.Add(int.Parse(season.MalId));
                        else if (!season.MalId.StartsWith("A")) // List filled with ID's specifically from the recommended category.
                            recommendedIDs.Add(int.Parse(season.MalId));
                    }
                }
            }

            addToList.Visibility = Visibility.Collapsed;

            timer = new FiveSecondTimer(cooldown, this);

            scrollViewer.PreviewMouseWheel += new ScrollViewerTimer().PreviewMouseWheel;

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Attempts to set up this window. will give the user an error if there is no internet connection.
        /// </summary>
        private void WindowSetup()
        {
            try
            {
                WebClient webClient1 = new WebClient();
                String jsonString = webClient1.DownloadString("https://raw.githubusercontent.com/GxRustyxG/AO-MAL-Simplified/main/AOSimplifiedList.json");
                webClient1.Dispose();

                totalAnimeList = JsonSerializer.Deserialize<List<AnimeSimplified>>(jsonString);
                cooldown.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex) when (ex is WebException || ex is HttpRequestException)
            {
                MessageBoxResult result = MessageBox.Show("Something went wrong when connecting to the internet or the file containing the randomized information is not accessible at this time. Would you like to try again?", "Anime Organizer", MessageBoxButton.YesNo, MessageBoxImage.Error);

                if (result == MessageBoxResult.Yes)
                    WindowSetup();
                else
                {
                    ((Grid)this.Parent).Visibility = Visibility.Collapsed;
                    ((Grid)this.Parent).Children.Remove(this);
                }
            }
        }

        /// <summary>
        /// Runs when the timer is finished.
        /// </summary>
        public void Timer_Finished()
        {
            randomize.IsEnabled = true;
        }

        /// <summary>
        /// This is where it finds the random anime.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Random_Anime_Click(object sender, RoutedEventArgs e)
        {
            Random generator = new Random();
            int idValue = 0;
            bool isRecommendedAnime = false;

            randomize.IsEnabled = false;

            if (searchChanged) // True if search filters changed
            {
                typeList = new List<String>();

                if (tv.IsChecked == true)
                    typeList.Add("TV");
                if (ova.IsChecked == true)
                    typeList.Add("OVA");
                if (ona.IsChecked == true)
                    typeList.Add("ONA");
                if (movie.IsChecked == true)
                    typeList.Add("Movie");
                if (special.IsChecked == true)
                    typeList.Add("Special");
                if (music.IsChecked == true)
                    typeList.Add("Music");
                if (unknown.IsChecked == true)
                    typeList.Add("Unknown");

                // This clears and refills the values that can be chosen randomly.
                currentFilteredList = new List<int>();

                foreach (AnimeSimplified anime in totalAnimeList)
                {
                    if (typeList.Contains(anime.Type) && !animeIDs.Contains(anime.MalID))
                        currentFilteredList.Add(anime.MalID);
                }
            }

            idValue = currentFilteredList[generator.Next(0, currentFilteredList.Count - 1)];
            //idValue = 37440; // Testing for the difference with recommended chosen variables.
            //idValue = 1; // Testing for internet connectivity.

            if (recommendedIDs.Contains(idValue)) // If true, set it to recommended, otherwise set it to my anime list.
            {
                isRecommendedAnime = true;
                retrieveLabel.Text = "Retrieved From:\nRecommended Category";
            }
            else
                retrieveLabel.Text = "Retrieved From:\nMy Anime List";

            if (isRecommendedAnime) // If true, runs through recommended list to find the anime with cooresponding ID.
            {
                bool isAnimeFound = false;

                foreach (Anime animeValue in Anime.animeLists[3])
                {
                    foreach (Season season in animeValue.Seasons)
                    {
                        if (!season.MalId.StartsWith("A") && int.Parse(season.MalId) == idValue)
                        {
                            isAnimeFound = true;
                            break;
                        }
                    }

                    if (isAnimeFound)
                    {
                        tempAnime = animeValue;
                        break;
                    }
                }

                this.DataContext = tempAnime; // Seems to be throwing some exceptions but isn't breaking the program. Not sure how to fix this.
                addToList.Visibility = Visibility.Visible;
            }
            else // This means we need to retrieve the anime from the MAL site.
            {
                try
                {
                    IJikan jikan = new Jikan();
                    JikanDotNet.Anime jikanAnime = (await jikan.GetAnimeAsync(idValue)).Data;

                    tempAnime = Anime.AnimeObjectConverter(jikanAnime, jikanAnime.Title, "Undetermined", -1);
                    tempAnime.CurrentSeason.ImageURL = jikanAnime.Images.JPG.ImageUrl;


                    this.DataContext = tempAnime; // Seems to be throwing some exceptions but isn't breaking the program. Not sure how to fix this.
                    addToList.Visibility = Visibility.Visible;
                }
                catch (Exception ex) when (ex is WebException || ex is HttpRequestException)
                {
                    ExceptionMessage.InternetError();
                }
            }


            cooldown.Visibility = Visibility.Visible;
            searchChanged = false;
            timer.Start();
        }

        /// <summary>
        /// Close this control and go back to the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Visibility = Visibility.Collapsed;
            ((Grid)this.Parent).Children.Remove(this);
        }

        /// <summary>
        /// This runs once this control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RandomAnime_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = AnimeData;
            close.Focus();

            WindowSetup();
        }

        /// <summary>
        /// This plays the system sound "Beep" if the user is trying to interact with something they can't.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editGridAndWebsiteGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (editGrid.Visibility == Visibility.Visible)
                SystemSounds.Beep.Play();
        }

        /// <summary>
        /// Opens the filter popup so the user can filter out what they want in the random search.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_Popup_Click(object sender, RoutedEventArgs e)
        {
            filterBorder.Visibility = Visibility.Visible;
            editGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// This runs when ever the filter is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            searchChanged = true;
        }

        /// <summary>
        /// This hides the filter control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Popup_Click(object sender, RoutedEventArgs e)
        {
            filterBorder.Visibility = Visibility.Collapsed;
            editGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Opens a user control that can add this anime to the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            bool isRecommended = false;

            if (retrieveLabel.Text.EndsWith("Recommended Category"))
                isRecommended = true;

            AddRandomAnime control = new AddRandomAnime(tempAnime, isRecommended);
            editGrid.Children.Add(control);
            editGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// This runs when the grid visibility changes to avoid breaking tab navigation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // If true, disables tab navigation.
            if ((bool)e.NewValue)
            {
                addToList.IsTabStop = false;
                filter.IsTabStop = false;
                backButton.IsTabStop = false;
                randomize.IsTabStop = false;
            }
            else
            {
                addToList.IsTabStop = true;
                filter.IsTabStop = true;
                backButton.IsTabStop = true;
                randomize.IsTabStop = true;
            }
        }


    }

    class ImageURLConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // This needs to check if the value is legal.

            if (File.Exists((String)value) || (Uri.TryCreate((String)value, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)))
                return value;
            else
                return Config.environmentPath + "Image Cache\\errorImage.jpg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(); // Might need change if these start getting thrown
        }
    }


}
