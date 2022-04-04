using Anime_Organizer.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.Edits
{
    /// <summary>
    /// Interaction logic for EditWebsite.xaml
    /// </summary>
    public partial class EditWebsite : UserControl
    {
        private Anime anime;

        public EditWebsite(Object obj)
        {
            InitializeComponent();

            anime = (Anime)obj;

            websites.CreateItems();
            websites.websiteCBox.Text = anime.CurrentSeason.Website;
            confirmCancel.confirm.Click += Confirm_Click;
            confirmCancel.cancel.Click += Cancel_Click;

            ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged += websiteGrid_IsVisibleChanged;

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Saves the website and closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (websites.websiteCBox.SelectedIndex != -1)
            {
                Config.changesSaved = false;
                anime.CurrentSeason.Website = websites.websiteCBox.Text;

                ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged -= websiteGrid_IsVisibleChanged;

                ((Grid)this.Parent).Visibility = Visibility.Collapsed;
                ((Grid)this.Parent).Children.Remove(this);
            }
            else
                MessageBox.Show("Website can not be blank.", "Anime Organizer", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Closes this control and goes back to the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).websiteGrid.IsVisibleChanged += websiteGrid_IsVisibleChanged;

            ((Grid)this.Parent).Visibility = Visibility.Collapsed;
            ((Grid)this.Parent).Children.Remove(this);
        }

        /// <summary>
        /// Runs once this control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditWebsite_Loaded(object sender, RoutedEventArgs e)
        {
            // Focuses the first control the user can edit.
            websites.websiteCBox.Focus();
        }

        /// <summary>
        /// This is to avoid tab navigation problems.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void websiteGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // If true, it removes all other controls from this section from the tab thing
            if ((bool)e.NewValue)
            {
                websites.websiteCBox.IsTabStop = false;
                websites.websitesButton.IsTabStop = false;
                confirmCancel.confirm.IsTabStop = false;
                confirmCancel.cancel.IsTabStop = false;
            }
            else
            {
                websites.websiteCBox.IsTabStop = true;
                websites.websitesButton.IsTabStop = true;
                confirmCancel.confirm.IsTabStop = true;
                confirmCancel.cancel.IsTabStop = true;
            }
        }

    }
}
