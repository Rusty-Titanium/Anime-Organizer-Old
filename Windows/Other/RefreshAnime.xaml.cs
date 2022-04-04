using Anime_Organizer.Classes;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Anime_Organizer.Windows.Other
{
    /// <summary>
    /// Interaction logic for RefreshAnime.xaml
    /// </summary>
    public partial class RefreshAnime : UserControl
    {
        /**
         * ImageURL only gets an image if it didn't exist. If it already exists, it does not update the image.
         */

        private DispatcherTimer timer;
        private int indexOfList;

        public RefreshAnime()
        {
            InitializeComponent();

            updateButton.Visibility = Visibility.Hidden;
            
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Locks in the category and will update the UI telling the user roughly how long it will take to update said category.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Lock_Click(object sender, RoutedEventArgs e)
        {
            if (category.categoryCBox.SelectedIndex != -1)
            {
                int amountOfVar = 0;

                foreach (Anime anime in Anime.animeLists[category.categoryCBox.SelectedIndex])
                    amountOfVar += anime.Seasons.Count;

                indexOfList = category.categoryCBox.SelectedIndex;

                progressBar.Minimum = 0;
                progressBar.Maximum = amountOfVar;

                int seconds = amountOfVar * 5;
                int minutes = seconds / 60;
                seconds = seconds % 60;

                estimateLabel.Content = "Estimated Time: " + minutes + " minute(s) and " + seconds + " seconds.";

                updateButton.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("You need to select a category first.", "Anime Organizer", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// This will initiate updating the list the user selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            lockButton.Visibility = Visibility.Collapsed;
            label1.Visibility = Visibility.Collapsed;

            label2.Text = "To avoid issues, please do not close out of the program while this is running. This process will end abruptly if internet connection is lost, leaving only a portion of it updated if saved.";
            Thickness margin = new Thickness();
            margin.Top = 125;
            label2.Margin = margin;

            category.Visibility = Visibility.Collapsed;
            estimateLabel.Content = "Time Elapsed: ";

            backButton.Visibility = Visibility.Collapsed;
            updateButton.Visibility = Visibility.Collapsed;

            timer.Start();

            List<Season> list = new List<Season>();

            foreach (Anime anime in Anime.animeLists[indexOfList])
                list.AddRange(anime.Seasons);

            try
            {
                foreach (Season season in list)
                {
                    // If it doesn't start with A, its a proper MAL item and can be checked
                    if (!season.MalId.StartsWith("A"))
                    {
                        IJikan jikan = new Jikan();
                        JikanDotNet.Anime anime = await jikan.GetAnime(int.Parse(season.MalId));

                        Update_Check(season, anime);

                        await Task.Delay(5000);
                    }

                    progressBar.Value++;
                }

                timer.Stop();

                estimateLabel.Content = "Refreshing complete!";
                backButton.Visibility = Visibility.Visible;

                Anime.Save();
            }
            catch (Exception ex) when (ex is WebException || ex is HttpRequestException)
            {
                ExceptionMessage.InternetError();

                ((Grid)this.Parent).Visibility = Visibility.Collapsed;
                ((Grid)this.Parent).Children.Remove(this);
            }
        }

        private int secondsElapsed = 1, minutesElapsed = 0;

        /// <summary>
        /// This updates the UI every second to tell the user how much time has pasted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            estimateLabel.Content = "Time Elapsed: " + minutesElapsed + " minute(s) and " + secondsElapsed + " seconds";
            secondsElapsed++;

            if (secondsElapsed == 60)
            {
                secondsElapsed = 0;
                minutesElapsed++;
            }
        }

        /// <summary>
        /// This method is where the updating takes place.
        /// </summary>
        /// <param name="season"></param>
        /// <param name="anime"></param>
        private void Update_Check(Season season, JikanDotNet.Anime anime)
        {
            season.MainTitle = anime.Title;
            season.AlternateTitle = anime.TitleEnglish;

            if (anime.Aired.From != null)
            {
                DateTime date = (DateTime)anime.Aired.From;
                season.Premiered = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month) + " " + date.Day + ", " + date.Year;
            }
            else
            {
                season.Premiered = "Unknown";
            }

            season.Type = anime.Type;
            season.Description = anime.Synopsis;

            if (anime.Episodes == null)
                season.Episodes = -1;
            else
                season.Episodes = (int)anime.Episodes;

            season.Airing = anime.Airing;
            season.Tags = new List<String>();

            foreach (JikanDotNet.MALSubItem tag in anime.Genres)
            {
                season.Tags.Add(tag.Name);
            }

            season.Tags.Sort();

            season.Broadcast = Anime.CreateBroadcast(anime);
            
            // Only adds the file if it's missing. It does not replace Images for the sake of not a lot of file craziness.
            if (!File.Exists(Config.environmentPath + "Image Cache\\" + anime.MalId + ".jpg"))
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile(anime.ImageURL, Config.environmentPath + "Image Cache\\" + anime.MalId + ".jpg");
                webClient.Dispose();
            }
        }

        /// <summary>
        /// This runs once the control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshAnime_Loaded(object sender, RoutedEventArgs e)
        {
            // Focuses the first control the user can edit.
            category.categoryCBox.Focus();
        }

        /// <summary>
        /// Closes this user control and goes back to the settings user control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Visibility = Visibility.Collapsed;
            ((Grid)this.Parent).Children.Remove(this);
        }

    }
}
