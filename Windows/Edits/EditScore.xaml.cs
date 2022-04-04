using Anime_Organizer.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.Edits
{
    /// <summary>
    /// Interaction logic for EditScore.xaml
    /// </summary>
    public partial class EditScore : UserControl
    {
        private Anime anime;

        public EditScore(Object obj)
        {
            InitializeComponent();

            anime = (Anime)obj;

            score.CreateItems();
            score.ChangeNoLabel();

            score.scoreCBox.Text = anime.Score.ToString();
            confirmCancel.confirm.Click += Confirm_Click;
            confirmCancel.cancel.Click += Cancel_Click;

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// This saves the score then closes this control to go back to the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            Config.changesSaved = false;
            anime.Score = double.Parse(score.scoreCBox.Text);

            ((Grid)this.Parent).Visibility = Visibility.Collapsed;
            ((Grid)this.Parent).Children.Remove(this);
        }

        /// <summary>
        /// This closes this control and goes back to the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Visibility = Visibility.Collapsed;
            ((Grid)this.Parent).Children.Remove(this);
        }

        /// <summary>
        /// Runs once this control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditScore_Loaded(object sender, RoutedEventArgs e)
        {
            // Focuses the first control the user can edit.
            score.scoreCBox.Focus();
        }


    }
}
