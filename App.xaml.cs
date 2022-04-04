using Anime_Organizer.Classes;
using Anime_Organizer.Windows.Startup;
using System;
using System.IO;
using System.Windows;

namespace Anime_Organizer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public enum Theme
        {
            Light, ColourfulLight,
            Dark, ColourfulDark
        }
        private ResourceDictionary ThemeDictionary
        {
            // You could probably get it via its name with some query logic as well.
            get { return Resources.MergedDictionaries[0]; }
            set { Resources.MergedDictionaries[0] = value; }
        }

        private void ChangeTheme(Uri uri)
        {
            ThemeDictionary = new ResourceDictionary() { Source = uri };
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (!Directory.Exists(Config.environmentPath))
            {
                InitialSetup win = new InitialSetup();
                win.CurrentApplication = this;
                this.MainWindow = win;
                win.Show();
            }
            else
            {
                Config.profileNumber = int.Parse(File.ReadAllText(Config.environmentPath + "Startup Profile.txt"));

                //True: No last used Profiles.  |  False: There was a last used profile and defaults to that.
                if (Config.profileNumber == 0)
                {
                    Profiles win = new Profiles();
                    win.CurrentApplication = this;
                    this.MainWindow = win;
                    win.Show();
                }
                else
                {
                    MainWindow win = new MainWindow();
                    win.CurrentApplication = this;
                    this.MainWindow = win;
                    win.Show();
                }
            }
        }

        public void SetTheme(Theme theme)
        {
            string themeName = null;
            switch (theme)
            {
                case Theme.Dark: themeName = "DarkTheme"; break;
                case Theme.Light: themeName = "LightTheme"; break;
                case Theme.ColourfulDark: themeName = "ColourfulDarkTheme"; break;
                case Theme.ColourfulLight: themeName = "ColourfulLightTheme"; break;
            }

            try
            {
                if (!string.IsNullOrEmpty(themeName))
                    ChangeTheme(new Uri($"Themes/{themeName}.xaml", UriKind.Relative));
            }
            catch { }
        }

    }
}
