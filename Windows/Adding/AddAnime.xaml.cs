using Anime_Organizer.Classes;
using Anime_Organizer.Windows.Other;
using Anime_Organizer.Windows.UserControls;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace Anime_Organizer.Windows.Adding
{
    /// <summary>
    /// Interaction logic for AddAnime.xaml
    /// </summary>
    public partial class AddAnime : UserControl
    {
        public JikanDotNet.Anime newAnime;

        public AddAnime()
        {
            InitializeComponent();

            malid.idTBox.TextChanged += ParameterChanged.General_TextChanged;
            malid.createNewButton.Click += createNewButton_Click;
            titles.nicknameTBox.TextChanged += ParameterChanged.General_TextChanged;
            websites.websiteCBox.SelectionChanged += ParameterChanged.General_SelectionChanged;
            websites.CreateItems();
            category.categoryCBox.SelectionChanged += ParameterChanged.General_SelectionChanged;
            category.categoryCBox.SelectionChanged += Category_SelectionChanged;
            score.scoreCBox.SelectionChanged += ParameterChanged.General_SelectionChanged;
            score.scoreCBox.IsEnabled = false;
            score.scoreLabel.IsEnabled = false;
            score.CreateItems();
            confirmCancel.confirm.Click += Confirm_Click;
            confirmCancel.cancel.Click += Cancel_Click;
            confirmCancel.confirm.Content = "Create";

            ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged += websiteGrid_IsVisibleChanged;

            scrollViewer.PreviewMouseWheel += new ScrollViewerTimer().PreviewMouseWheel;

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Attempts to add an anime.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            int categoryIndex = -1;
            String nickname = "", website = "";
            double scoreNum = -1;
            bool satisfied = true;

            //Start of a new thingy
            if (newAnime == null)
            {
                malid.idTBox.BorderBrush = Brushes.Red;
                satisfied = false;
            }
            
            if (titles.nicknameTBox.Text.Length == 0)
            {
                titles.nicknameTBox.BorderBrush = Brushes.Red;
                satisfied = false;
            }

            if (websites.websiteCBox.Text.Length == 0)
            {
                websites.websiteCBox.BorderBrush = Brushes.Red;
                satisfied = false;
            }

            if (category.categoryCBox.Text.Length == 0)
            {
                category.categoryCBox.BorderBrush = Brushes.Red;
                satisfied = false;
            }

            if (category.categoryCBox.SelectedIndex == 4 && score.scoreCBox.IsEnabled && score.scoreCBox.Text.Length == 0)
            {
                score.scoreCBox.BorderBrush = Brushes.Red;
                satisfied = false;
            }


            if (satisfied)
            {
                try
                {
                    // Sets variables necessary for Anime variable creation.
                    nickname = titles.nicknameTBox.Text;
                    website = websites.websiteCBox.Text;
                    scoreNum = score.scoreCBox.SelectedIndex;

                    // Converts to the programs local Anime variable.
                    Anime anime = Anime.AnimeObjectConverter(newAnime, nickname, website, scoreNum);


                    if (score.scoreCBox.IsEnabled == false)
                        anime.CurrentSeason.LastWatched = new LastEPWatched(0, 0);
                    else if (score.scoreCBox.IsEnabled)
                        anime.CurrentSeason.LastWatched = new LastEPWatched(anime.CurrentSeason.Episodes, 0);

                    if (!File.Exists(Config.environmentPath + "Image Cache\\" + anime.CurrentSeason.MalId + ".jpg"))
                    {
                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(newAnime.ImageURL, Config.environmentPath + "Image Cache\\" + anime.CurrentSeason.MalId + ".jpg");
                        webClient.Dispose();
                    }

                    if (category.categoryCBox.SelectedIndex == 0)
                    {
                        DateTime date = DateTime.Now;
                        anime.StartDate = new TimeInfo();
                        anime.StartDate.Year = date.Year;
                        anime.StartDate.Month = date.Month;
                        anime.StartDate.Day = date.Day;
                        anime.StartDate.Hour = -1;
                        anime.StartDate.Minute = -1;
                    }

                    if (category.categoryCBox.SelectedIndex == 4)
                    {
                        DateTime date = DateTime.Now;

                        anime.StartDate = new TimeInfo();
                        anime.StartDate.Year = date.Year;
                        anime.StartDate.Month = date.Month;
                        anime.StartDate.Day = date.Day;
                        anime.StartDate.Hour = -1;
                        anime.StartDate.Minute = -1;

                        anime.FinishDate = new TimeInfo();
                        anime.FinishDate.Year = date.Year;
                        anime.FinishDate.Month = date.Month;
                        anime.FinishDate.Day = date.Day;
                        anime.FinishDate.Hour = -1;
                        anime.FinishDate.Minute = -1;
                    }

                    categoryIndex = category.categoryCBox.SelectedIndex;
                    Anime.animeLists[categoryIndex].Add(anime);
                    Anime.animeLists[6].Add(anime);
                    Config.changesSaved = false;

                    ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged -= websiteGrid_IsVisibleChanged;

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
        /// Replaces this control with AddNonMALAnime.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createNewButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged -= websiteGrid_IsVisibleChanged;

            Grid grid = (Grid)this.Parent;
            grid.Children.Remove(this);

            AddNonMALAnime control = new AddNonMALAnime();
            grid.Children.Add(control);
        }
        
        /// <summary>
        /// Changs score enabled depending on selected item index.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Category_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            if (comboBox.SelectedIndex == 4)
            {
                score.scoreLabel.IsEnabled = true;
                score.scoreCBox.IsEnabled = true;
            }
            else
            {
                //Always runs if its not index 4 chosen, even if it's already disabled, which could be fixed.
                score.scoreLabel.IsEnabled = false;
                score.scoreCBox.IsEnabled = false;
                score.scoreCBox.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Closes this control and goes back to the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged += websiteGrid_IsVisibleChanged;

            ((Grid)this.Parent).Visibility = Visibility.Collapsed;
            ((Grid)this.Parent).Children.Remove(this);
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
                malid.idTBox.IsTabStop = false;
                malid.idButton.IsTabStop = false;
                malid.createNewButton.IsTabStop = false;
                titles.nicknameTBox.IsTabStop = false;
                titles.setNick1.IsTabStop = false;
                titles.setNick2.IsTabStop = false;
                websites.websiteCBox.IsTabStop = false;
                websites.websitesButton.IsTabStop = false;
                category.categoryCBox.IsTabStop = false;
                score.scoreCBox.IsTabStop = false;
                confirmCancel.confirm.IsTabStop = false;
                confirmCancel.cancel.IsTabStop = false;
            }
            else
            {
                malid.idTBox.IsTabStop = true;
                malid.idButton.IsTabStop = true;
                malid.createNewButton.IsTabStop = true;
                titles.nicknameTBox.IsTabStop = true;
                titles.setNick1.IsTabStop = true;
                titles.setNick2.IsTabStop = true;
                websites.websiteCBox.IsTabStop = true;
                websites.websitesButton.IsTabStop = true;
                category.categoryCBox.IsTabStop = true;
                score.scoreCBox.IsTabStop = true;
                confirmCancel.confirm.IsTabStop = true;
                confirmCancel.cancel.IsTabStop = true;
            }
        }

        /// <summary>
        /// Runs once this control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddAnime_Loaded(object sender, RoutedEventArgs e)
        {
            malid.idTBox.Focus();
        }

        private void Search_For_Anime_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged -= websiteGrid_IsVisibleChanged;

            Grid grid = (Grid)this.Parent;
            grid.Children.Remove(this);

            SearchForAnime control = new SearchForAnime(true, null);
            grid.Children.Add(control);
        }
    }
}
