using Anime_Organizer.Classes;
using System;
using System.IO;
using System.Windows;

namespace Anime_Organizer.Windows.Startup
{
    /// <summary>
    /// Interaction logic for Profiles.xaml
    /// </summary>
    public partial class Profiles : Window
    {
        public Profiles()
        {
            InitializeComponent();

            // Note: Buttons are named "New Profile" if it is not used. Buttons can only be 16 characters long.

            String mainFolderPath = Config.environmentPath; // THIS COULD JUST BE REPLACED BY THE CONFIG VARIABLE.

            // This sets the text of each button.
            button1.Content = File.ReadAllText(mainFolderPath + "Profiles\\Profile 1\\profilename.txt");
            button2.Content = File.ReadAllText(mainFolderPath + "Profiles\\Profile 2\\profilename.txt");
            button3.Content = File.ReadAllText(mainFolderPath + "Profiles\\Profile 3\\profilename.txt");
        }

        
        public App CurrentApplication { get; set; }

        /// <summary>
        /// This will attempt to run the profile in the first spot. If there is no profile, prompts to make a profile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Profile_1(object sender, RoutedEventArgs e)
        {
            Config.profileNumber = 1;

            if (String.Equals(button1.Content.ToString(), "New Profile", StringComparison.CurrentCultureIgnoreCase))
            {
                NewProfile win = new NewProfile();
                win.CurrentApplication = this.CurrentApplication;
                this.CurrentApplication.MainWindow = win;
                win.Show();
            }
            else
            {
                File.WriteAllText(Config.environmentPath + "Startup Profile.txt", "1");

                MainWindow win = new MainWindow();
                win.CurrentApplication = this.CurrentApplication;
                this.CurrentApplication.MainWindow = win;
                win.Show();
            }

            this.Close();
        }

        /// <summary>
        /// This will attempt to run the profile in the second spot. If there is no profile, prompts to make a profile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Profile_2(object sender, RoutedEventArgs e)
        {
            Config.profileNumber = 2;

            if (String.Equals(button2.Content.ToString(), "New Profile", StringComparison.CurrentCultureIgnoreCase))
            {
                NewProfile win = new NewProfile();
                win.CurrentApplication = this.CurrentApplication;
                this.CurrentApplication.MainWindow = win;
                win.Show();
            }
            else
            {
                File.WriteAllText(Config.environmentPath + "Startup Profile.txt", "2");

                MainWindow win = new MainWindow();
                win.CurrentApplication = this.CurrentApplication;
                this.CurrentApplication.MainWindow = win;
                win.Show();
            }

            this.Close();
        }

        /// <summary>
        /// This will attempt to run the profile in the third spot. If there is no profile, prompts to make a profile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Profile_3(object sender, RoutedEventArgs e)
        {
            Config.profileNumber = 3;

            if (String.Equals(button3.Content.ToString(), "New Profile", StringComparison.CurrentCultureIgnoreCase))
            {
                NewProfile win = new NewProfile();
                win.CurrentApplication = this.CurrentApplication;
                this.CurrentApplication.MainWindow = win;
                win.Show();
            }
            else
            {
                File.WriteAllText(Config.environmentPath + "Startup Profile.txt", "3");

                MainWindow win = new MainWindow();
                win.CurrentApplication = this.CurrentApplication;
                this.CurrentApplication.MainWindow = win;
                win.Show();
            }

            this.Close();
        }

        /// <summary>
        /// Runs once this control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, EventArgs e)
        {
            CurrentApplication.SetTheme(App.Theme.Light);
        }
        
    }
}
