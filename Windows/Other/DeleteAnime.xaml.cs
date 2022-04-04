using Anime_Organizer.Classes;
using Anime_Organizer.Windows.Edits;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.Other
{
    /// <summary>
    /// Interaction logic for DeleteAnime.xaml
    /// </summary>
    public partial class DeleteAnime : UserControl
    {
        private Anime anime;

        public DeleteAnime(Object obj)
        {
            InitializeComponent();

            anime = (Anime)obj;

            animeTBlock.Text = anime.NickNameTitle;
            confirmCancel.confirm.Click += Confirm_Click;
            confirmCancel.cancel.Click += Cancel_Click;

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// This will delete the selected anime and then return the user back to the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            // This sets numWnidow to 2 so the program now knows
            EditAnime edit = (EditAnime)((Grid)((Grid)this.Parent).Parent).Parent;
            
            Anime.animeLists[Config.selectedTab].Remove(anime);
            Anime.animeLists[6].Remove(anime);

            Config.changesSaved = false;

            EditAnime editAnime = (EditAnime)((Grid)((Grid)this.Parent).Parent).Parent;
            editAnime.isAnimeDeleted = true;

            ((Grid)this.Parent).Visibility = Visibility.Collapsed;
            ((Grid)this.Parent).Children.Remove(this);
        }

        /// <summary>
        /// This will close this control and bring the user back to the EditAnime control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            EditAnime edit = (EditAnime)((Grid)((Grid)this.Parent).Parent).Parent;

            ((Grid)this.Parent).Visibility = Visibility.Collapsed;
            ((Grid)this.Parent).Children.Remove(this);
        }
    }
}
