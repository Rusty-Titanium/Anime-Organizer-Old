using Anime_Organizer.Classes;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Anime_Organizer.Windows.Edits
{
    /// <summary>
    /// Interaction logic for EditLastWatched.xaml
    /// </summary>
    public partial class EditLastWatched : UserControl
    {
        private Anime anime;

        public EditLastWatched(Object obj)
        {
            InitializeComponent();

            anime = (Anime)obj;

            episodeTBox.Text = anime.CurrentSeason.LastWatched.Episode.ToString();
            seasonTBox.Text = anime.CurrentSeason.LastWatched.Season.ToString();

            if (anime.CurrentSeason.Episodes == -1)
                numOfEpisodes.Text = numOfEpisodes.Text + "????";
            else
                numOfEpisodes.Text = numOfEpisodes.Text + ((int)anime.CurrentSeason.Episodes);

            confirmCancel.confirm.Click += Confirm_Click;
            confirmCancel.cancel.Click += Cancel_Click;

            episodeTBox.PreviewKeyDown += TextInputConfig.TextNoSpace_PreviewKeyDown;
            seasonTBox.PreviewKeyDown += TextInputConfig.TextNoSpace_PreviewKeyDown;
            DataObject.AddPastingHandler(episodeTBox, TextInputConfig.OnPasteNoAlphabet);
            DataObject.AddPastingHandler(seasonTBox, TextInputConfig.OnPasteNoAlphabet);

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
        /// If the key pressed was enter, runs the confirm method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Confirm_Check();
        }

        /// <summary>
        /// If valid, saves last watched and goes back to the main window.
        /// </summary>
        private void Confirm_Check()
        {
            if (episodeTBox.Text.Equals("") || seasonTBox.Text.Equals(""))
            {
                MessageBox.Show("Episode/Season can not be blank.", "Anime Organizer", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (anime.CurrentSeason.Episodes != -1 && double.Parse(episodeTBox.Text) > anime.CurrentSeason.Episodes)
            {
                MessageBox.Show("Number of episodes watched exceeds total number of episodes", "Anime Organizer", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Config.changesSaved = false;
                LastEPWatched newLastWatched = new LastEPWatched(double.Parse(episodeTBox.Text), double.Parse(seasonTBox.Text));
                anime.CurrentSeason.LastWatched = newLastWatched;

                ((Grid)this.Parent).Visibility = Visibility.Collapsed;
                ((Grid)this.Parent).Children.Remove(this);
            }
        }

        /// <summary>
        /// Closes this control and goes back to the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Visibility = Visibility.Collapsed;
            ((Grid)this.Parent).Children.Remove(this);
        }

        
        private static readonly Regex _regex = new Regex("[^0-9]+");

        /// <summary>
        /// Special previewtextinput for numbers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textbox = (TextBox)sender;

            if (e.Text.Equals("=") || e.Text.Equals("+")) //This is a plus 1
            {
                if (textbox.Text.Length == 0)
                    textbox.Text = "1";
                else
                    textbox.Text = (double.Parse(textbox.Text) + 1).ToString();

                textbox.CaretIndex = textbox.Text.Length;
            }
            else if (e.Text.Equals("-") && double.Parse(textbox.Text) > 0) //This is a minus 1
            {
                textbox.Text = (double.Parse(textbox.Text) - 1).ToString();
                textbox.CaretIndex = textbox.Text.Length;
            }

            e.Handled = _regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Runs once this control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditLastWatched_Loaded(object sender, RoutedEventArgs e)
        {
            // Focuses the first control the user can edit.
            episodeTBox.Focus();
            episodeTBox.CaretIndex = episodeTBox.Text.Length;
            seasonTBox.CaretIndex = seasonTBox.Text.Length;
        }


    }
}
