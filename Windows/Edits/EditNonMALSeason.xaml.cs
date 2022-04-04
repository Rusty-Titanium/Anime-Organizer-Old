using Anime_Organizer.Classes;
using Anime_Organizer.Windows.Other;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.Edits
{
    /// <summary>
    /// Interaction logic for EditNonMALSeason.xaml
    /// </summary>
    public partial class EditNonMALSeason : UserControl
    {
        private Season originalSeason;

        public EditNonMALSeason(Object obj)
        {
            InitializeComponent();

            originalSeason = (Season)obj;

            titles.mainTBox.Foreground = (Brush)FindResource("ControlForeground");
            titles.altTBox.Foreground = (Brush)FindResource("ControlForeground");

            titles.mainTBox.Text = originalSeason.MainTitle;
            titles.altTBox.Text = originalSeason.AlternateTitle;
            type.typeCBox.Text = originalSeason.Type;
            numOfEpisodes.episodesTBox.TextChanged += ParameterChanged.General_TextChanged;
            premiere.premiereDayCBox.SelectionChanged += ParameterChanged.General_SelectionChanged;
            premiere.premiereMonthCBox.SelectionChanged += ParameterChanged.General_SelectionChanged;
            premiere.premiereYearCBox.SelectionChanged += ParameterChanged.General_SelectionChanged;
            confirmCancel.confirm.Click += Edit_Season_Click;
            confirmCancel.cancel.Click += Cancel_Click;

            scrollViewer.PreviewMouseWheel += new ScrollViewerTimer().PreviewMouseWheel;

            if (originalSeason.Airing)
                currentlyAiring.airingCBox.Text = "true";
            else
                currentlyAiring.airingCBox.Text = "false";

            if (originalSeason.Episodes == -1)
                numOfEpisodes.episodesTBox.Text = "0";
            else
                numOfEpisodes.episodesTBox.Text = originalSeason.Episodes.ToString();


            String[] array = originalSeason.Premiered.Split();

            premiere.premiereMonthCBox.Text = array[0];
            premiere.premiereDayCBox.Text = array[1].Substring(0, 1);
            premiere.premiereYearCBox.Text = array[2];

            foreach (String tag in originalSeason.Tags)
                tags.Add_From_EditNonMAL(tag);

            description.descriptionTBox.Text = originalSeason.Description;

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Saves the season then closes this control and goes back to the EditAnime control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit_Season_Click(object sender, RoutedEventArgs e)
        {
            bool satisfied = true;

            // Start of creation of season

            if (titles.mainTBox.Text.Length == 0)
            {
                titles.mainTBox.BorderBrush = Brushes.Red;
                satisfied = false;
            }

            if (titles.altTBox.Text.Length == 0)
            {
                titles.altTBox.BorderBrush = Brushes.Red;
                satisfied = false;
            }

            if (numOfEpisodes.episodesTBox.Text.Length == 0)
            {
                numOfEpisodes.episodesTBox.BorderBrush = Brushes.Red;
                satisfied = false;
            }

            if (tags.tagPanel.Children.Count == 0)
            {
                tags.borderForWrapper.BorderBrush = Brushes.Red;
                satisfied = false;
            }

            // If it gets in here, that means at this point everything should be good to go and nothing should break.
            if (satisfied)
            {
                // Creation of Broadcast.
                originalSeason.Broadcast = new TimeInfo();
                originalSeason.Broadcast.Year = -1;
                originalSeason.Broadcast.Month = -1;
                originalSeason.Broadcast.Day = 0;
                originalSeason.Broadcast.Hour = 8;
                originalSeason.Broadcast.Minute = 0;


                originalSeason.MainTitle = titles.mainTBox.Text;
                originalSeason.AlternateTitle = titles.altTBox.Text;
                originalSeason.Type = type.typeCBox.Text;
                originalSeason.Airing = bool.Parse(currentlyAiring.airingCBox.Text);

                if (double.Parse(numOfEpisodes.episodesTBox.Text) == 0)
                    originalSeason.Episodes = -1;
                else
                    originalSeason.Episodes = double.Parse(numOfEpisodes.episodesTBox.Text);


                originalSeason.Tags.Clear();

                foreach (UIElement element in tags.tagPanel.Children)
                    originalSeason.Tags.Add(((Button)element).Content.ToString());

                originalSeason.Tags.Sort();


                if (description.descriptionTBox.Text.Length == 0)
                    originalSeason.Description = "";
                else
                    originalSeason.Description = description.descriptionTBox.Text;

                originalSeason.Premiered = premiere.premiereMonthCBox.Text + " " + premiere.premiereDayCBox.Text + ", " + premiere.premiereYearCBox.Text;

                Config.changesSaved = false;

                ((Grid)this.Parent).Visibility = Visibility.Collapsed;
                ((Grid)this.Parent).Children.Remove(this);
            }


        }

        /// <summary>
        /// This closes this control and goes back to the EditAnime control.
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
