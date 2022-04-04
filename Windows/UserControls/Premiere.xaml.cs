using Anime_Organizer.Classes;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for Premiere.xaml
    /// </summary>
    public partial class Premiere : UserControl
    {
        /**
         * Controls this is used in:
         * - AddNonMALAnime - Done
         * - AddNonMALSeason - Done
         * - EditNonMALSeason - Done
         */

        public Premiere()
        {
            InitializeComponent();

            for (int i = 1950; i < DateTime.Now.Year + 5; i++)
                premiereYearCBox.Items.Add(i);

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }
    }
}
