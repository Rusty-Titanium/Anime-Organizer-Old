using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Anime_Organizer.Classes
{
    class ExceptionMessage
    {
        /// <summary>
        /// Message generally ran when an exception occurs with connection to the internet.
        /// </summary>
        /// <returns></returns>
        public static MessageBoxResult InternetError()
        {
            return MessageBox.Show("Something went wrong. Please try again once you are connected to the internet.", "Anime Organizer", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
