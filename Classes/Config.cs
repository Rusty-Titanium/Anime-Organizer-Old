using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Anime_Organizer.Classes
{
    class Config
    {
        public static int profileNumber = 0, selectedTab = 0;
        public static bool isMilitaryTime = false, changesSaved = true;
        public static FontFamily useFont = new FontFamily("./#CC Wild Words");
        //public static String environmentPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Anime Organizer\\"; //The normal path

        // This is the path used when testing. This is so I can avoid accidentally messing with my own anime list.
        public static String environmentPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Anime Organizer Test Folder\\";
    }
}
