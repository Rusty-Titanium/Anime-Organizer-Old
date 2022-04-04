using Anime_Organizer.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Anime_Organizer.Windows.Adding
{
    /// <summary>
    /// Interaction logic for AddWebsite.xaml
    /// </summary>
    public partial class AddWebsite : UserControl
    {
        public AddWebsite()
        {
            InitializeComponent();

            String jsonString = File.ReadAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\websites.json");
            String[] websites = JsonSerializer.Deserialize<String[]>(jsonString);

            for (int i = 0; i < websites.Length; i++)
            {
                ListBoxItem lItem = new ListBoxItem();
                lItem.MouseDoubleClick += ListBoxItem_MouseDoubleClick;
                lItem.Content = websites[i];
                websiteLBox.Items.Add(lItem);
            }

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }
        
        /// <summary>
        /// This will prompt the user if they want to delete the website.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Do you want to delete this website? \nIf you continue, everything up to this point will immediately be saved.", "Anime Organizer", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                // This checks if the website thats being deleted is being used by an Anime object or not.

                bool websiteUsed = false;

                foreach (ObservableCollection<Anime> list in Anime.animeLists)
                {
                    foreach (Anime anime in list)
                    {
                        foreach (Season season in anime.Seasons)
                        {
                            if (String.Equals(season.Website, (String)((ListBoxItem)sender).Content, StringComparison.CurrentCultureIgnoreCase))
                            {
                                websiteUsed = true;
                                break;
                            }
                        }

                        if (websiteUsed) // These breaks happen if the program finds the website is used, and breaks out so it isnt spamming the entire list.
                            break;
                    }

                    if (websiteUsed)
                        break;
                }

                if (websiteUsed)
                {
                    MessageBox.Show("This website is currently being used and can not be deleted.", "Anime Organizer", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    websiteLBox.Items.Remove(sender);
                    SaveWebsites();
                }

            }

        }

        /// <summary>
        /// Adds website if eligible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Website_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Add_Website();
        }

        /// <summary>
        /// Adds website if eligible.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Website_Click(object sender, RoutedEventArgs e)
        {
            Add_Website();
        }

        /// <summary>
        /// Actual method that attempts to add the website.
        /// </summary>
        private void Add_Website()
        {
            // This needs to make sure there is no websites that are exactly the same.

            bool websiteAlreadyExist = false;

            foreach (ListBoxItem website in websiteLBox.Items)
            {
                if (String.Equals(websiteTBox.Text, (String)website.Content, StringComparison.CurrentCultureIgnoreCase))
                {
                    websiteAlreadyExist = true;
                    break;
                }
            }

            if (websiteAlreadyExist)
            {
                MessageBox.Show("This website already exists.", "Anime Organizer", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (websiteTBox.Text.Equals(""))
                {
                    MessageBox.Show("The website name can not be blank.", "Anime Organizer", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    ListBoxItem item = new ListBoxItem();
                    item.MouseDoubleClick += ListBoxItem_MouseDoubleClick;
                    item.Content = websiteTBox.Text;
                    websiteLBox.Items.Add(item);

                    SaveWebsites();
                    websiteTBox.Text = "";
                }
            }
        }


        /// <summary>
        /// This will save the websites and save any current progress made to the anime variables so far.
        /// </summary>
        private void SaveWebsites()
        {
            List<String> websites = new List<String>();

            // This loop re-adds the websites without the deleted one into a list.
            foreach (ListBoxItem item in websiteLBox.Items)
            {
                websites.Add(((String)item.Content).ToLower());
            }

            // The file is then edited.
            String jsonString = JsonSerializer.Serialize(websites);
            File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\websites.json", jsonString);
            Anime.Save();
        }

        /// <summary>
        /// This control closes and goes back to which ever control it came from.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Done_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Visibility = Visibility.Collapsed;
            ((Grid)this.Parent).Children.Remove(this);
        }
        
        /// <summary>
        /// Runs once this control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddWebsite_Loaded(object sender, RoutedEventArgs e)
        {
            // Focuses the first textbox the user can edit.
            websiteTBox.Focus();
        }


    }
}
