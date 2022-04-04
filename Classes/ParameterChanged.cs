using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Classes
{
    class ParameterChanged
    {
        /// <summary>
        /// Selection change that if red will change the controls border color back to normal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void General_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            if (comboBox.BorderBrush == Brushes.Red)
                comboBox.BorderBrush = (Brush)Application.Current.FindResource("ControlBorderBrush");
        }

        /// <summary>
        /// Text change that if red will change the controls border color back to normal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void General_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textbox = (TextBox)sender;

            if (textbox.BorderBrush == Brushes.Red)
                textbox.BorderBrush = (Brush)Application.Current.FindResource("ControlBorderBrush");
        }
    }
}
