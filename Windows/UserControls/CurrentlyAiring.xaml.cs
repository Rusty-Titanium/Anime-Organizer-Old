using Anime_Organizer.Classes;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for CurrentlyAiring.xaml
    /// </summary>
    public partial class CurrentlyAiring : UserControl
    {
        /**
         * Controls this is used in:
         * - AddNonMALAnime - Done
         * - AddNonMALSeason - Done
         * - EditNonMALSeason - Done
         */

        public CurrentlyAiring()
        {
            InitializeComponent();

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }
    }
}
