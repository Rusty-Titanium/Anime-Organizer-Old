using Anime_Organizer.Classes;
using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace Anime_Organizer.Windows.Startup
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class NewProfile : Window
    {
        public App CurrentApplication { get; set; }

        public NewProfile()
        {
            InitializeComponent();

            createButton.Visibility = Visibility.Hidden;
            scoringSystem.minCBox.SelectionChanged += ComboBox_SelectionChanged;
            scoringSystem.maxCBox.SelectionChanged += ComboBox_SelectionChanged;
            scoringSystem.incrementCBox.SelectionChanged += ComboBox_SelectionChanged;
        }

        // This creates the new profile
        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            // Profile Name
            File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\profilename.txt", profileTextBox.Text);

            // Scoring System
            var options = new JsonSerializerOptions { WriteIndented = true };
            String jsonString = JsonSerializer.Serialize(new double[] { double.Parse(scoringSystem.minCBox.Text), double.Parse(scoringSystem.maxCBox.Text), double.Parse(scoringSystem.incrementCBox.Text) }, options);
            File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\scoringsystem.json", jsonString);

            // Sets new starting profile
            File.WriteAllText(Config.environmentPath + "Startup Profile.txt", Config.profileNumber.ToString());

            MainWindow win = new MainWindow();
            win.CurrentApplication = this.CurrentApplication;
            this.CurrentApplication.MainWindow = win;
            win.Show();

            this.Close();
        }

        /// <summary>
        /// Checks if all requirements are met, otherwise the button won't be visible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void profileTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (profileTextBox.Text.Trim().Length != 0 && !String.Equals(profileTextBox.Text.Trim(), "New Profile", StringComparison.CurrentCultureIgnoreCase) && scoringSystem.minCBox.SelectedIndex != -1 && scoringSystem.maxCBox.SelectedIndex != -1 && scoringSystem.incrementCBox.SelectedIndex != -1)
                createButton.Visibility = Visibility.Visible;
            else
                createButton.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Checks if all requirements are met, otherwise the button won't be visible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (profileTextBox.Text.Trim().Length != 0 && !String.Equals(profileTextBox.Text.Trim(), "New Profile", StringComparison.CurrentCultureIgnoreCase) && scoringSystem.minCBox.SelectedIndex != -1 && scoringSystem.maxCBox.SelectedIndex != -1 && scoringSystem.incrementCBox.SelectedIndex != -1)
                createButton.Visibility = Visibility.Visible;
            else
                createButton.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Runs once this control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewProfile_Loaded(object sender, EventArgs e)
        {
            CurrentApplication.SetTheme(App.Theme.Light); // This default sets the new profile to light mode to avoid certain issues this window had.
            profileTextBox.Focus();
        }

        /// <summary>
        /// This will cancel the creation of a new profile and will bring you back to choose from the 3 profiles.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Go_Back_Click(object sender, RoutedEventArgs e)
        {
            Profiles win = new Profiles();
            win.CurrentApplication = this.CurrentApplication;
            this.CurrentApplication.MainWindow = win;
            win.Show();

            this.Close();
        }
    }
}
