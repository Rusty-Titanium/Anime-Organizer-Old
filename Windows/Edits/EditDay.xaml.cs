using Anime_Organizer.Classes;
using Anime_Organizer.Windows.Other;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.Edits
{
    /// <summary>
    /// Interaction logic for EditDay.xaml
    /// </summary>
    public partial class EditDay : UserControl
    {
        private Anime anime;

        public EditDay(Object obj)
        {
            InitializeComponent();

            anime = (Anime)obj;
            int localoffset = DateTimeOffset.Now.Offset.Hours;

            broadcast.daysCBox.Text = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[anime.CurrentSeason.Broadcast.ConvertDay(localoffset, anime.CurrentSeason.Broadcast.Hour, anime.CurrentSeason.Broadcast.Day)] + "s";
            broadcast.timeLabel.Content = TimeInfo.AbbreviationOfLocalTimeZone();

            if (Config.isMilitaryTime) // If true, collapse am/pm comboBox
            {
                broadcast.dayNightCBox.Visibility = Visibility.Collapsed;

                for (int i = 0; i < 24; i++)
                    broadcast.hoursCBox.Items.Add(i.ToString("D2")); // This "D2" means that if there is less than 2 characters, there will be added zeros.

                broadcast.hoursCBox.Text = anime.CurrentSeason.Broadcast.ConvertHour(localoffset, anime.CurrentSeason.Broadcast.Hour).ToString("D2");
            }
            else
            {
                // This shifts over the text so it will fit properly on screen. (Scuffed I know)
                broadcast.timeLabel.Content = "        " + broadcast.timeLabel.Content;

                for (int i = 1; i < 13; i++)
                    broadcast.hoursCBox.Items.Add(i.ToString());

                // This will give the correct offset time in military.
                int hours = anime.CurrentSeason.Broadcast.ConvertHour(localoffset, anime.CurrentSeason.Broadcast.Hour);

                if (hours > 12)
                {
                    hours -= 12;
                    broadcast.dayNightCBox.Text = "pm";
                }
                else if (hours == 12)
                {
                    broadcast.dayNightCBox.Text = "pm";
                }
                else if (hours == 0)
                {
                    hours = 12;
                    broadcast.dayNightCBox.Text = "am";
                }
                else
                {
                    broadcast.dayNightCBox.Text = "am";
                }

                broadcast.hoursCBox.Text = hours.ToString();
            }

            broadcast.minutesCBox.Text = anime.CurrentSeason.Broadcast.Minute.ToString("D2");

            confirmCancel.confirm.Click += Confirm_Click;
            confirmCancel.cancel.Click += Cancel_Click;

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// If valid, saves the broadcast for the anime then goes back to the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            // Sets the values of these variables from the numbers given from the comboboxes
            int newDay = broadcast.daysCBox.SelectedIndex, newHour = int.Parse(broadcast.hoursCBox.Text), newMinute = int.Parse(broadcast.minutesCBox.Text);

            // The offset needs to be its opposite to go back to +00:00
            int localoffset = DateTimeOffset.Now.Offset.Hours * -1;

            TimeInfo newTimeInfo = new TimeInfo();
            newTimeInfo.Day = anime.CurrentSeason.Broadcast.ConvertDay(localoffset, newHour, newDay);

            if (!Config.isMilitaryTime)
            {
                if (broadcast.dayNightCBox.Text.Equals("am"))
                {
                    // This is here so if its 12am, in military that means its 00:00
                    if (newHour == 12)
                        newHour = 0;
                }
                else if (broadcast.dayNightCBox.Text.Equals("pm"))
                {
                    // As long as it doesn't equal 12 (because that breaks everything), then add 12 so it can be put in proper military time.
                    if (newHour != 12)
                        newHour += 12;
                }
            }

            newTimeInfo.Hour = anime.CurrentSeason.Broadcast.ConvertHour(localoffset, newHour);
            newTimeInfo.Minute = newMinute;
            anime.CurrentSeason.Broadcast = newTimeInfo;

            Config.changesSaved = false;

            ((Grid)this.Parent).Visibility = Visibility.Collapsed;
            ((Grid)this.Parent).Children.Remove(this);
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

        /// <summary>
        /// Runs once this control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditDay_Loaded(object sender, RoutedEventArgs e)
        {
            // Focuses the first control the user can edit.
            broadcast.daysCBox.Focus();
        }

    }
}
