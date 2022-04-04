using Anime_Organizer.Classes;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for SeasonControl.xaml
    /// </summary>
    public partial class SeasonControl : UserControl
    {
        public SeasonControl()
        {
            InitializeComponent();

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Returns the index this control is in.
        /// </summary>
        public int SeasonIndex
        {
            get { return int.Parse(((String)seasonIndexLabel.Content).Split()[2]) - 1; }
        }
    }
}
