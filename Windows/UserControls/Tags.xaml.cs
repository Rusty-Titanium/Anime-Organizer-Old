using Anime_Organizer.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Anime_Organizer.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for Tags.xaml
    /// </summary>
    public partial class Tags : UserControl
    {
        /**
         * Controls this is used in:
         * - AddNonMALAnime - Done
         * - AddNonMALSeason - Done
         * - EditNonMALSeason - Done
         */

        public Tags()
        {
            InitializeComponent();

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Attempts to add a tag if the key that was pressed was the enter key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Add_Tag();
        }

        /// <summary>
        /// Attempts to add a tag given the string from the text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Tag_Click(object sender, RoutedEventArgs e)
        {
            Add_Tag();
        }

        /// <summary>
        /// Attempts to add a tag.
        /// </summary>
        public void Add_Tag()
        {
            if (tagTBox.Text.Trim().Length != 0)
            {
                Button button = new Button();
                button.Style = (Style)FindResource("CloseButton");
                button.Margin = new Thickness(2);
                button.Content = tagTBox.Text.Trim();
                button.Click += Remove_Tag_Click;

                tagPanel.Children.Add(button);

                tagTBox.Text = "";

                if (borderForWrapper.BorderBrush == Brushes.Red)
                    borderForWrapper.BorderBrush = (Brush)FindResource("ControlBorderBrush");
            }
            else
                tagTBox.Text = "";
        }

        /// <summary>
        /// Specific made method for EditNonMALSeason to avoid issues.
        /// </summary>
        /// <param name="tag"></param>
        public void Add_From_EditNonMAL(String tag)
        {
            Button button = new Button();
            button.Style = (Style)FindResource("CloseButton");
            button.Margin = new Thickness(2);
            button.Content = tag;
            button.Click += Remove_Tag_Click;

            tagPanel.Children.Add(button);
        }
        
        /// <summary>
        /// Removes the button that was clicked on. Equivalent of removing the tag.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Remove_Tag_Click(object sender, RoutedEventArgs e)
        {
            tagPanel.Children.Remove((Button)e.Source);
        }

    }
}
