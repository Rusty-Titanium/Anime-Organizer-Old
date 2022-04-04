using Anime_Organizer.Classes;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.Other
{
    /// <summary>
    /// Interaction logic for AddRandomAnime.xaml
    /// </summary>
    public partial class AddRandomAnime : UserControl
    {
        private Anime anime;
        private bool isRecommended;

        public AddRandomAnime(Object obj, bool recommended)
        {
            InitializeComponent();

            anime = (Anime)obj;
            isRecommended = recommended;

            ((ComboBoxItem)category.categoryCBox.Items[2]).IsEnabled = false;
            ((ComboBoxItem)category.categoryCBox.Items[4]).IsEnabled = false;
            ((ComboBoxItem)category.categoryCBox.Items[5]).IsEnabled = false;

            // If anime is from recommended, disable placing it back in recommended as it's already there.
            if (isRecommended)
            {
                ((ComboBoxItem)category.categoryCBox.Items[3]).IsEnabled = false;
                titles.Visibility = Visibility.Collapsed;
                websites.Visibility = Visibility.Collapsed;
            }
            else
                recommendedLabel.Visibility = Visibility.Collapsed;


            titles.mainTBox.Text = anime.CurrentSeason.MainTitle;
            titles.altTBox.Text = anime.CurrentSeason.AlternateTitle;

            category.categoryCBox.SelectionChanged += ParameterChanged.General_SelectionChanged;
            titles.nicknameTBox.TextChanged += ParameterChanged.General_TextChanged;
            websites.websiteCBox.SelectionChanged += ParameterChanged.General_SelectionChanged;
            websites.CreateItems();
            confirmCancel.confirm.Click += Confirm_Click;
            confirmCancel.cancel.Click += Close_Click;

            ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged += websiteGrid_IsVisibleChanged;

            scrollViewer.PreviewMouseWheel += new ScrollViewerTimer().PreviewMouseWheel;

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// This will add the anime the user has chosen to the desired category.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            bool satisfied = true;

            if (category.categoryCBox.SelectedIndex == -1)
            {
                category.categoryCBox.BorderBrush = Brushes.Red;
                satisfied = false;
            }

            if (titles.nicknameTBox.Text.Length == 0)
            {
                titles.nicknameTBox.BorderBrush = Brushes.Red;
                satisfied = false;
            }

            if (websites.websiteCBox.SelectedIndex == -1)
            {
                websites.websiteCBox.BorderBrush = Brushes.Red;
                satisfied = false;
            }



            // True if the category is selected and this was originally in the Recommended cateogry.
            if (isRecommended && category.categoryCBox.SelectedIndex != -1)
            {
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

                Anime.animeLists[3].Remove(anime);
                Anime.animeLists[6].Remove(anime);

                Anime.animeLists[category.categoryCBox.SelectedIndex].Add(anime);
                Anime.animeLists[6].Add(anime);
                Config.changesSaved = false;

                ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged -= websiteGrid_IsVisibleChanged;

                RandomAnime randomAnime = (RandomAnime)((Grid)((Grid)this.Parent).Parent).Parent;
                randomAnime.DataContext = null;
                randomAnime.addToList.Visibility = Visibility.Collapsed;

                ((Grid)this.Parent).Visibility = Visibility.Collapsed;
                ((Grid)this.Parent).Children.Remove(this);
            }

            if (satisfied && !isRecommended)
            {
                try
                {
                    anime.NickNameTitle = titles.nicknameTBox.Text;
                    anime.CurrentSeason.Website = websites.websiteCBox.Text;
                    anime.CurrentSeason.LastWatched = new LastEPWatched(0, 0);

                    if (!File.Exists(Config.environmentPath + "Image Cache\\" + anime.CurrentSeason.MalId + ".jpg"))
                    {
                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(anime.CurrentSeason.ImageURL, Config.environmentPath + "Image Cache\\" + anime.CurrentSeason.MalId + ".jpg");
                        webClient.Dispose();
                    }

                    anime.CurrentSeason.ImageURL = Config.environmentPath + "Image Cache\\" + anime.CurrentSeason.MalId + ".jpg";

                    if (category.categoryCBox.SelectedIndex == 0) //might want to create a method for this in the category user control.
                    {
                        DateTime date = DateTime.Now;
                        anime.StartDate = new TimeInfo();
                        anime.StartDate.Year = date.Year;
                        anime.StartDate.Month = date.Month;
                        anime.StartDate.Day = date.Day;
                        anime.StartDate.Hour = -1;
                        anime.StartDate.Minute = -1;
                    }

                    Anime.animeLists[category.categoryCBox.SelectedIndex].Add(anime);
                    Anime.animeLists[6].Add(anime);
                    Config.changesSaved = false;

                    ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged -= websiteGrid_IsVisibleChanged;

                    RandomAnime randomAnime = (RandomAnime)((Grid)((Grid)this.Parent).Parent).Parent;
                    randomAnime.DataContext = null;
                    randomAnime.addToList.Visibility = Visibility.Collapsed;

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
        /// Disables tab navigation to avoid issues with the program.
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
                category.categoryCBox.IsTabStop = false;
                confirmCancel.confirm.IsTabStop = false;
                confirmCancel.cancel.IsTabStop = false;
            }
            else
            {
                titles.nicknameTBox.IsTabStop = true;
                titles.setNick1.IsTabStop = false;
                titles.setNick2.IsTabStop = false;
                websites.websiteCBox.IsTabStop = true;
                websites.websitesButton.IsTabStop = true;
                category.categoryCBox.IsTabStop = true;
                confirmCancel.confirm.IsTabStop = true;
                confirmCancel.cancel.IsTabStop = true;
            }
        }

        /// <summary>
        /// This will close this control and go back to the RandomAnime control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Visibility = Visibility.Collapsed;
            ((Grid)this.Parent).Children.Remove(this);
        }

    }
}
