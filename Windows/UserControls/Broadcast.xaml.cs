using Anime_Organizer.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for Broadcast.xaml
    /// </summary>
    public partial class Broadcast : UserControl
    {
        /**
         * Controls this is used in:
         * - AddNonMALAnime - Done
         * - AddNonMALSeason - Done
         * - EditDay - Done
         */

        public int ChooseAltStyle { get; set; }

        public Broadcast()
        {
            InitializeComponent();

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Method to check if user uses 24-hour clock and will populate the comboboxes accordingly.
        /// </summary>
        public void checkMilitaryTime()
        {
            if (Config.isMilitaryTime)
            {
                // If it is military time, then collapse the am/pm combobox.
                dayNightCBox.Visibility = Visibility.Collapsed;

                for (int i = 0; i < 24; i++)
                    hoursCBox.Items.Add(i.ToString("D2")); // This "D2" means that if there is less than 2 characters, there will be added zeros.
            }
            else
            {
                // This shifts over the text so it will fit properly on screen. (Scuffed I know)
                timeLabel.Content = "        " + timeLabel.Content;

                for (int i = 1; i < 13; i++)
                    hoursCBox.Items.Add(i.ToString());
            }
        }

        /// <summary>
        /// Changes the design of this control.
        /// </summary>
        public void ChangeNoLabel()
        {
            broadcastLabel.Visibility = Visibility.Collapsed;

            daysCBox.Margin = new Thickness(0, 0, 306, 0);
            atLabel.Margin = new Thickness(0, 0, 114, 0);
            hoursCBox.Margin = new Thickness(0, 0, 20, 0);
            colonLabel.Margin = new Thickness(52, 0, 0, 0);
            minutesCBox.Margin = new Thickness(126, 0, 0, 0);
            timeLabel.Margin = new Thickness(320, 0, 0, 0);
            dayNightCBox.Margin = new Thickness(250, 0, 0, 0);
            broadcastNoteLabel.Margin = new Thickness(0, 33, 0, 0);
        }

        /// <summary>
        /// Runs after this control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Broadcast_Loaded(object sender, RoutedEventArgs e)
        {
            if (ChooseAltStyle == 1)
                ChangeNoLabel();
        }
    }
}
