using Anime_Organizer.Classes;
using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for Score.xaml
    /// </summary>
    public partial class Score : UserControl
    {

        /**
         * Controls this is in:
         * - AddAnime - Done
         * - AddNonMALAnime - Done
         * - EditScore - Done
         */

        public int ChooseAltStyle { get; set; }

        public Score()
        {
            InitializeComponent();

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Creates all the combobox items filled with the increments of scoring.
        /// </summary>
        public void CreateItems()
        {
            String jsonString = File.ReadAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\scoringsystem.json");
            double[] scoringsystem = JsonSerializer.Deserialize<double[]>(jsonString);

            for (double i = scoringsystem[0]; i <= scoringsystem[1]; i = i + scoringsystem[2])
            {
                ComboBoxItem cbItem = new ComboBoxItem();
                cbItem.Content = i;
                scoreCBox.Items.Add(cbItem);
            }
        }

        /// <summary>
        /// Changes the design of this control.
        /// </summary>
        public void ChangeNoLabel()
        {
            scoreLabel.Visibility = Visibility.Collapsed;
            scoreCBox.Margin = new Thickness(0);
        }

        /// <summary>
        /// Runs once this control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Score_Loaded(object sender, RoutedEventArgs e)
        {
            if (ChooseAltStyle == 1)
                ChangeNoLabel();
        }
    }
}
