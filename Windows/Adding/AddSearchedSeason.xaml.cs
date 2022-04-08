using Anime_Organizer.Classes;
using Anime_Organizer.Windows.Edits;
using Anime_Organizer.Windows.Other;
using Anime_Organizer.Windows.UserControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Anime_Organizer.Windows.Adding
{
    /// <summary>
    /// Interaction logic for AddSearchedSeason.xaml
    /// </summary>
    public partial class AddSearchedSeason : UserControl
    {
        /**

        private SearchForAnime parentControl;
        private JikanDotNet.Anime chosenAnime;
        private Anime animeAddingTo;

        private DispatcherTimer timer;
        private double timerNum = 5.0;

        public AddSearchedSeason(SearchForAnime parentControl, JikanDotNet.Anime chosenAnime, Object obj)
        {
            InitializeComponent();

            titles.nicknameTBox.TextChanged += ParameterChanged.General_TextChanged;
            websites.websiteCBox.SelectionChanged += ParameterChanged.General_SelectionChanged;
            websites.CreateItems();
            confirmCancel.confirm.Click += Confirm_Click;
            confirmCancel.cancel.Click += Cancel_Click;
            confirmCancel.cancel.IsEnabled = false;

            ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged += websiteGrid_IsVisibleChanged;

            scrollViewer.PreviewMouseWheel += new ScrollViewerTimer().PreviewMouseWheel;

            this.parentControl = parentControl;
            this.chosenAnime = chosenAnime;
            animeAddingTo = (Anime)obj;

            titles.mainTBox.Text = chosenAnime.Title;
            titles.altTBox.Text = chosenAnime.TitleEnglish;

            timer = new DispatcherTimer(DispatcherPriority.Send);
            timer.Interval = TimeSpan.FromMilliseconds(94); // .94 Seconds
            timer.Tick += Timer_Tick;

            timer.Start();

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// A visual timer that is on a 5 second cooldown when ever it is ran.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            timerNum = Math.Round(timerNum - 0.1, 1);
            cooldown.Content = "Cooldown: " + timerNum + " sec";

            if (timerNum == 0.0)
            {
                cooldown.Visibility = Visibility.Collapsed;
                timerNum = 5.0;
                cooldown.Content = "Cooldown: " + timerNum + " sec";

                confirmCancel.cancel.IsEnabled = true;

                timer.Stop();
            }
        }

        /// <summary>
        /// Changes tab navigation to avoid issues.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void websiteGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // If true, it removes all other controls from this section from the tab thing
            if ((bool)e.NewValue)
            {
                titles.nicknameTBox.IsTabStop = false;
                titles.setNick1.IsTabStop = false;
                titles.setNick2.IsTabStop = false;
                websites.websiteCBox.IsTabStop = false;
                websites.websitesButton.IsTabStop = false;
                confirmCancel.confirm.IsTabStop = false;
                confirmCancel.cancel.IsTabStop = false;
            }
            else
            {
                titles.nicknameTBox.IsTabStop = true;
                titles.setNick1.IsTabStop = true;
                titles.setNick2.IsTabStop = true;
                websites.websiteCBox.IsTabStop = true;
                websites.websitesButton.IsTabStop = true;
                confirmCancel.confirm.IsTabStop = true;
                confirmCancel.cancel.IsTabStop = true;
            }
        }

        /// <summary>
        /// Attempts to add the anime to the users list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Confirm_Click(object sender, RoutedEventArgs e)
        {
            String website = "";
            bool satisfied = true;

            if (websites.websiteCBox.Text.Length == 0)
            {
                websites.websiteCBox.BorderBrush = Brushes.Red;
                satisfied = false;
            }


            if (satisfied)
            {
                try
                {
                    website = websites.websiteCBox.Text;

                    animeAddingTo.addSeason(chosenAnime, website);

                    if (!File.Exists(Config.environmentPath + "Image Cache\\" + chosenAnime.MalId + ".jpg"))
                    {
                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(chosenAnime.ImageURL, Config.environmentPath + "Image Cache\\" + chosenAnime.MalId + ".jpg");
                        webClient.Dispose();
                    }

                    Config.changesSaved = false;

                    ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged -= websiteGrid_IsVisibleChanged;
                    
                    EditAnime editAnime = (EditAnime)((Grid)((Grid)parentControl.Parent).Parent).Parent;
                    editAnime.CreatePanel(animeAddingTo.Seasons.Count - 1);
                    editAnime.ShiftNewSeasonPanel();
                    editAnime.FixButtonVisibility();

                    ((Grid)this.Parent).Visibility = Visibility.Collapsed;
                    ((Grid)this.Parent).Children.Remove(this);
                }
                catch (Exception ex) when (ex is WebException || ex is HttpRequestException)
                {
                    ExceptionMessage.InternetError();
                }
            }
        }

        /// <summary>
        /// Closes this control and goes back to the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged -= websiteGrid_IsVisibleChanged;

            parentControl.editGrid.Visibility = Visibility.Collapsed;
            parentControl.editGrid.Children.Remove(this);
        }

        /// <summary>
        /// Runs once this control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSearchedSeason_Loaded(object sender, RoutedEventArgs e)
        {
            titles.nicknameTBox.Focus();
        }
         */

    }
}
