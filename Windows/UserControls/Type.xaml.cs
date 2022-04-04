using Anime_Organizer.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for Type.xaml
    /// </summary>
    public partial class Type : UserControl
    {
        /**
         * Spots this is used:
         * - AddNonMALAnime - Done
         * - AddNonMALSeason - Done
         * - EditNonMALSeason - Done
         * - SearchforAnime - Done
         */

        public int ChooseAltStyle { get; set; }

        public Type()
        {
            InitializeComponent();

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Changes design based of a value to see if it will use an alternate style.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Type_Loaded(object sender, RoutedEventArgs e)
        {
            if (ChooseAltStyle == 1)
                typeCBox.Items.RemoveAt(0);
        }

    }
}
