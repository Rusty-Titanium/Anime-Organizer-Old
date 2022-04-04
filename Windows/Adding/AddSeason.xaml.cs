using Anime_Organizer.Classes;
using Anime_Organizer.Windows.Edits;
using Anime_Organizer.Windows.UserControls;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.Adding
{
    /// <summary>
    /// Interaction logic for AddSeason.xaml
    /// </summary>
    public partial class AddSeason : UserControl
    {
        public JikanDotNet.Anime newAnime;
        private Anime originalAnime;

        // This is to see if its the same variable or if im going to have to find a way to send it back
        public AddSeason(Object obj)
        {
            InitializeComponent();

            originalAnime = (Anime)obj;

            malid.idTBox.TextChanged += ParameterChanged.General_TextChanged;
            malid.createNewButton.Click += createNewButton_Click;
            websites.websiteCBox.SelectionChanged += ParameterChanged.General_SelectionChanged;
            websites.CreateItems();
            confirmCancel.confirm.Click += Confirm_Click;
            confirmCancel.cancel.Click += Cancel_Click;
            confirmCancel.confirm.Content = "Add";

            ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged += websiteGrid_IsVisibleChanged;

            scrollViewer.PreviewMouseWheel += new ScrollViewerTimer().PreviewMouseWheel;

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Attempt at adding a season to the current anime.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            String website = "";
            bool satisfied = true;

            // Start of a new Season
            if (newAnime == null)
            {
                malid.idTBox.BorderBrush = Brushes.Red;
                satisfied = false;
            }

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

                    originalAnime.addSeason(newAnime, website);

                    if (!File.Exists(Config.environmentPath + "Image Cache\\" + newAnime.MalId + ".jpg"))
                    {
                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(newAnime.ImageURL, Config.environmentPath + "Image Cache\\" + newAnime.MalId + ".jpg");
                        webClient.Dispose();
                    }

                    Config.changesSaved = false;

                    ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged -= websiteGrid_IsVisibleChanged;

                    EditAnime editAnime = (EditAnime)((Grid)((Grid)this.Parent).Parent).Parent;
                    editAnime.CreatePanel(originalAnime.Seasons.Count - 1);
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
        /// Closes this control for the AddNonMALSeason control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createNewButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged -= websiteGrid_IsVisibleChanged;

            Grid grid = (Grid)this.Parent;
            grid.Children.Remove(this);

            AddNonMALSeason control = new AddNonMALSeason(originalAnime);
            grid.Children.Add(control);
        }

        /// <summary>
        /// This control closes and goes back to the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged -= websiteGrid_IsVisibleChanged;

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
                titles.mainTBox.IsTabStop = false;
                titles.altTBox.IsTabStop = false;

                websites.websiteCBox.IsTabStop = false;
                websites.websitesButton.IsTabStop = false;
                confirmCancel.confirm.IsTabStop = false;
                confirmCancel.cancel.IsTabStop = false;
            }
            else
            {
                malid.idTBox.IsTabStop = true;
                malid.idButton.IsTabStop = true;
                malid.createNewButton.IsTabStop = true;
                titles.mainTBox.IsTabStop = true;
                titles.altTBox.IsTabStop = true;

                websites.websiteCBox.IsTabStop = true;
                websites.websitesButton.IsTabStop = true;
                confirmCancel.confirm.IsTabStop = true;
                confirmCancel.cancel.IsTabStop = true;
            }
        }

        /// <summary>
        /// Runs once this control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSeason_Loaded(object sender, RoutedEventArgs e)
        {
            malid.idTBox.Focus();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_For_Anime_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged -= websiteGrid_IsVisibleChanged;

            Grid grid = (Grid)this.Parent;
            grid.Children.Remove(this);

            SearchForAnime control = new SearchForAnime(false, originalAnime);
            grid.Children.Add(control);
        }
    }
}
