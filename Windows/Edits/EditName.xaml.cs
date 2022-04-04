using Anime_Organizer.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Anime_Organizer.Windows.Edits
{
    /// <summary>
    /// Interaction logic for EditName.xaml
    /// </summary>
    public partial class EditName : UserControl
    {
        private Anime anime;

        public EditName(Object obj)
        {
            InitializeComponent();

            anime = (Anime)obj;

            // Textboxes are set to the different titles.
            titles.nicknameTBox.Text = anime.NickNameTitle;
            titles.mainTBox.Text = anime.CurrentSeason.MainTitle;
            titles.altTBox.Text = anime.CurrentSeason.AlternateTitle;
            titles.nicknameTBox.KeyDown += Confirm_KeyDown;

            confirmCancel.confirm.Click += Confirm_Click;
            confirmCancel.cancel.Click += Cancel_Click;

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Runs the confirm method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            Confirm_Check();
        }

        /// <summary>
        /// If the key was enter, runs the confirm method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Confirm_Check();
        }

        /// <summary>
        /// If valid, will save the nickname and hten go back to the main window.
        /// </summary>
        private void Confirm_Check()
        {
            if (titles.nicknameTBox.Text.Length != 0) // True as long as nickname isn't blank.
            {
                Config.changesSaved = false;
                anime.NickNameTitle = titles.nicknameTBox.Text;

                ((Grid)this.Parent).Visibility = Visibility.Collapsed;
                ((Grid)this.Parent).Children.Remove(this);
            }
            else
            {
                MessageBox.Show("The Anime's nickname has to be at least 1 character long.", "Anime Organizer", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
        private void EditName_Loaded(object sender, RoutedEventArgs e)
        {
            // Focuses the first control the user can edit.
            titles.nicknameTBox.Focus();
            titles.nicknameTBox.CaretIndex = titles.nicknameTBox.Text.Length;
        }


    }
}
