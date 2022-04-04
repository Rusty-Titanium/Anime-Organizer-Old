using Anime_Organizer.Classes;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for ImageControl.xaml
    /// </summary>
    public partial class ImageControl : UserControl
    {
        /**
         * Controls this is used in:
         * - AddNonMALAnime - Done
         * - AddNonMALSeason - Done
         */

        public ImageControl()
        {
            InitializeComponent();

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Opens an OpenFileDialog to get an image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowsePC_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            openFile.ShowDialog();

            imageTBox.Text = openFile.FileName;
        }


    }
}
