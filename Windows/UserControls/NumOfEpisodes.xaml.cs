using Anime_Organizer.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for NumOfEpisodes.xaml
    /// </summary>
    public partial class NumOfEpisodes : UserControl
    {
        /**
         * Controls this is used in:
         * - AddNonMALAnime - Done
         * - AddNonMALSeason - Done
         * - EditNonMALSeason - Done
         */

        public NumOfEpisodes()
        {
            InitializeComponent();

            episodesTBox.PreviewTextInput += TextInputConfig.TextBox_PreviewTextInput;
            episodesTBox.PreviewKeyDown += TextInputConfig.TextNoSpace_PreviewKeyDown;
            DataObject.AddPastingHandler(episodesTBox, TextInputConfig.OnPasteNoAlphabet); // This is to add the paste handler.

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }
    }
}
