using Anime_Organizer.Classes;
using Anime_Organizer.Windows.Adding;
using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for Websites.xaml
    /// </summary>
    public partial class Websites : UserControl
    {
        /**
         * - AddAnime - Done
         * - AddNonMALAnime - Done
         * - AddNonMALSeason - Done
         * - AddSeason - Done
         * - EditWebsite - Done
         */

        public int ChooseAltStyle { get; set; }

        public Websites()
        {
            InitializeComponent();

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Adds in items relating to a list of websites the user can choose from.
        /// </summary>
        public void CreateItems()
        {
            String jsonString = File.ReadAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\websites.json");
            String[] websiteList = JsonSerializer.Deserialize<String[]>(jsonString);

            // Fill the combobox with the websites.
            for (int i = 0; i < websiteList.Length; i++)
            {
                ComboBoxItem cbItem = new ComboBoxItem();
                cbItem.Content = websiteList[i];
                websiteCBox.Items.Add(cbItem);
            }
        }

        /// <summary>
        /// Opens up a user control that can add or remove websites.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Websites_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).websiteVar = this;
            Grid websiteGrid = ((MainWindow)Application.Current.MainWindow).getWebsiteGrid();

            AddWebsite control = new AddWebsite();
            websiteGrid.Children.Add(control);
            websiteGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// This removes the label and shifts everything over to comply with design.
        /// </summary>
        public void ChangeNoLabel()
        {
            websiteLabel.Visibility = Visibility.Collapsed;
            websiteCBox.Margin = new Thickness(0,0,0,5);
            websiteTBlock.Margin = new Thickness(0, 5, 0, 5);
            websitesButton.Margin = new Thickness(0, 5, 0, 5);
            this.Height = this.Height - 32;
        }

        /// <summary>
        /// Runs once this control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Websites_Loaded(object sender, RoutedEventArgs e)
        {
            if (ChooseAltStyle == 1)
                ChangeNoLabel();
        }
    }
}
