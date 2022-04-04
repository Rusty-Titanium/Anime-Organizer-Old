using Anime_Organizer.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.Other
{
    /// <summary>
    /// Interaction logic for MoveAnime.xaml
    /// </summary>
    public partial class MoveAnime : UserControl
    {
        private Anime anime;

        public MoveAnime(Object obj)
        {
            InitializeComponent();

            anime = (Anime)obj;
            animeTBlock.Text = anime.NickNameTitle;
            confirmCancel.confirm.Click += Confirm_Click;
            confirmCancel.cancel.Click += Cancel_Click;

            ((ComboBoxItem)category.categoryCBox.Items[Config.selectedTab]).IsEnabled = false;

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// This will move the anime to its desired category.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (category.categoryCBox.SelectedIndex != -1)
            {
                if (category.categoryCBox.SelectedIndex == 4 && Config.selectedTab != category.categoryCBox.SelectedIndex)
                {
                    DateTime date = DateTime.Now;

                    anime.FinishDate = new TimeInfo();
                    anime.FinishDate.Year = date.Year;
                    anime.FinishDate.Month = date.Month;
                    anime.FinishDate.Day = date.Day;
                    anime.FinishDate.Hour = -1;
                    anime.FinishDate.Minute = -1;

                    if (Config.selectedTab != 0)
                    {
                        anime.StartDate = new TimeInfo();
                        anime.StartDate.Year = date.Year;
                        anime.StartDate.Month = date.Month;
                        anime.StartDate.Day = date.Day;
                        anime.StartDate.Hour = -1;
                        anime.StartDate.Minute = -1;
                    }
                }
                else if (category.categoryCBox.SelectedIndex == 0 && Config.selectedTab != category.categoryCBox.SelectedIndex)
                {
                    DateTime date = DateTime.Now;

                    anime.StartDate = new TimeInfo();
                    anime.StartDate.Year = date.Year;
                    anime.StartDate.Month = date.Month;
                    anime.StartDate.Day = date.Day;
                    anime.StartDate.Hour = -1;
                    anime.StartDate.Minute = -1;
                }

                Anime.animeLists[Config.selectedTab].Remove(anime);
                Anime.animeLists[category.categoryCBox.SelectedIndex].Add(anime);

                Config.changesSaved = false;

                ((Grid)this.Parent).Visibility = Visibility.Collapsed;
                ((Grid)this.Parent).Children.Remove(this);
            }
            else
            {
                MessageBox.Show("Category can not be blank.", "Anime Organizer", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// This will close this control and go back to the main window.
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
