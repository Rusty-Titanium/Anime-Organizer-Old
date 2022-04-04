using Anime_Organizer.Classes;
using IWshRuntimeLibrary;
using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Anime_Organizer.Windows.Startup
{
    /// <summary>
    /// Interaction logic for InitialSetup.xaml
    /// </summary>
    public partial class InitialSetup : Window
    {
        public App CurrentApplication { get; set; }

        public InitialSetup()
        {
            InitializeComponent();

            confirmCancel.confirm.Click += Ok_Click;
            confirmCancel.cancel.Click += Cancel_Click;
        }

        /// <summary>
        /// This will create the additional files needed to run the program.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            String mainFolderPath = Config.environmentPath; // THIS COULD JUST BE REPLACED BY THE CONFIG VARIABLE INSTEAD.

            // Creates the main directory that will be used for saving information.
            if (!Directory.Exists(mainFolderPath))
                Directory.CreateDirectory(mainFolderPath);

            // Creates the startup profile text file so it can launch the program properly.
            System.IO.File.AppendAllText(mainFolderPath + "Startup Profile.txt", "0");

            // This creates the image cache folder while also adding the error image.
            Directory.CreateDirectory(mainFolderPath + "Image Cache");
            Uri uri = new Uri("pack://application:,,,/" + Assembly.GetExecutingAssembly().GetName().Name + ";component/" + "Resources/errorImage.jpg");
            System.Drawing.Image image = System.Drawing.Image.FromStream(Application.GetResourceStream(uri).Stream);
            image.Save(mainFolderPath + "Image Cache\\errorImage.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

            // This loop creates the rest of the necessary files needed to run the program.
            for (int i = 1; i < 4; i++)
            {
                Directory.CreateDirectory(mainFolderPath + "Profiles\\Profile " + i + "\\nonMAL Image Cache");
                System.IO.File.AppendAllText(mainFolderPath + "Profiles\\Profile " + i + "\\animelist.json", "[\n  [],\n  [],\n  [],\n  [],\n  [],\n  [],\n  []\n]");
                System.IO.File.AppendAllText(mainFolderPath + "Profiles\\Profile " + i + "\\columns.json",
                    "[\n  [60, 200, 200, 75, 200, 200, 200, 129],\n  " +
                    "[60, 200, 200, 75, 200, 200, 200, 129],\n  " +
                    "[60, 200, 200, 75, 200, 200, 200, 129],\n  " +
                    "[60, 200, 200, 75, 200, 200, 200, 129],\n  " +
                    "[60, 200, 200, 75, 200, 200, 200, 129],\n  " +
                    "[60, 200, 200, 75, 200, 200, 200, 129],\n  " +
                    "[60, 200, 200, 75, 200, 200, 200, 129]\n]");
                System.IO.File.AppendAllText(mainFolderPath + "Profiles\\Profile " + i + "\\general notes.txt", "");
                System.IO.File.AppendAllText(mainFolderPath + "Profiles\\Profile " + i + "\\light mode.txt", "true");
                System.IO.File.AppendAllText(mainFolderPath + "Profiles\\Profile " + i + "\\military time.txt", "false");
                System.IO.File.AppendAllText(mainFolderPath + "Profiles\\Profile " + i + "\\nonMALid.txt", "0");
                System.IO.File.AppendAllText(mainFolderPath + "Profiles\\Profile " + i + "\\profilename.txt", "New Profile");
                System.IO.File.AppendAllText(mainFolderPath + "Profiles\\Profile " + i + "\\scoringsystem.json", "[]");
                System.IO.File.AppendAllText(mainFolderPath + "Profiles\\Profile " + i + "\\websites.json", "[]");
                System.IO.File.AppendAllText(mainFolderPath + "Profiles\\Profile " + i + "\\useFont.txt", "./#CC Wild Words");
            }

            Config.profileNumber = int.Parse(System.IO.File.ReadAllText(mainFolderPath + "Startup Profile.txt"));

            // This creates the shortcut of the program onto the desktop.
            object shDesktop = (object)"Desktop";
            WshShell shell = new WshShell();
            string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\Anime Organizer.lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "Shortcut for Anime Organizer";
            shortcut.TargetPath = AppDomain.CurrentDomain.BaseDirectory + "Anime Organizer.exe"; //This I believe stays so the shortcut still works.
            shortcut.Save();

            // With all files created, launch the profiles window so the user can start up.
            Profiles win = new Profiles();
            win.CurrentApplication = this.CurrentApplication;
            this.CurrentApplication.MainWindow = win;
            win.Show();

            this.Close();
        }

        /// <summary>
        /// This closes the program.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
