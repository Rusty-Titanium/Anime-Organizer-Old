using Anime_Organizer.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.Edits
{
    /// <summary>
    /// Interaction logic for EditNotes.xaml
    /// </summary>
    public partial class EditNotes : UserControl
    {
        private Anime anime;

        public EditNotes(Object obj)
        {
            InitializeComponent();

            anime = (Anime)obj;
            notesTBox.Text = anime.Notes;

            notesTBox.Focus();
            notesTBox.CaretIndex = notesTBox.Text.Length;

            confirmCancel.confirm.Click += Confirm_Click;
            confirmCancel.cancel.Click += Cancel_Click;

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Saves the notes then closes this control and goes back to the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            Config.changesSaved = false;
            anime.Notes = notesTBox.Text;

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
        private void EditNotes_Loaded(object sender, RoutedEventArgs e)
        {
            // Focuses the first textbox the user can edit.
            notesTBox.Focus();
            notesTBox.CaretIndex = notesTBox.Text.Length;
        }


    }
}
