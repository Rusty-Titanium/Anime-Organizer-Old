using Anime_Organizer.Classes;
using Anime_Organizer.Windows.Edits;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.Other
{
    /// <summary>
    /// Interaction logic for DeleteSeason.xaml
    /// </summary>
    public partial class DeleteSeason : UserControl
    {
        private Anime anime;
        private Season season;

        public DeleteSeason(Object obj1, Object obj2)
        {
            InitializeComponent();

            anime = (Anime)obj1;
            season = (Season)obj2;

            animeTBlock.Text = season.MainTitle;
            confirmCancel.confirm.Click += Confirm_Click;
            confirmCancel.cancel.Click += Cancel_Click;

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// This will remove the selected season from the anime.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (anime.Seasons.Count == 1)
            {
                MessageBox.Show("You can not delete a season when the anime only has one season.", "Anime Organizer", MessageBoxButton.OK, MessageBoxImage.Error);

                ((Grid)this.Parent).Visibility = Visibility.Collapsed;
                ((Grid)this.Parent).Children.Remove(this);
            }
            else
            {
                anime.Seasons.Remove(season);

                // If CurrentSeason was the one deleted, it needs to be changed to a new season variable so problems don't arise.
                if (anime.CurrentSeason == season)
                {
                    // If CurrentSeasonIndex = 0, change CurrentSeason to now current index 0, otherwise minus index by 1 and change CurrentSeason
                    if (anime.CurrentSeasonIndex == 0)
                    {
                        anime.CurrentSeason = anime.Seasons[anime.CurrentSeasonIndex];
                    }
                    else
                    {
                        anime.CurrentSeasonIndex--;
                        anime.CurrentSeason = anime.Seasons[anime.CurrentSeasonIndex];
                    }
                }
                else
                {
                    if (anime.CurrentSeasonIndex != 0)
                        anime.CurrentSeasonIndex--;
                }

                Config.changesSaved = false;

                // Removes the last panel from anime list as the indexes are now changed so the last one is no longer needed.
                EditAnime editAnime = (EditAnime)((Grid)((Grid)this.Parent).Parent).Parent;
                editAnime.dockPanel.Children.RemoveAt(editAnime.dockPanel.Children.Count - 2);
                editAnime.FixButtonVisibility();

                ((Grid)this.Parent).Visibility = Visibility.Collapsed;
                ((Grid)this.Parent).Children.Remove(this);
            }

        }

        /// <summary>
        /// This will close this control and go back to the edit anime window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Visibility = Visibility.Collapsed;
            ((Grid)this.Parent).Children.Remove(this);
        }

    }
}
