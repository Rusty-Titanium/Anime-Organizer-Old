using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Anime_Organizer.Classes
{
    class TextInputConfig
    {
        private static readonly Regex _regexNoAlphabet = new Regex("[^0-9]+$"); // Regex that matches disallowed text.

        /// <summary>
        /// Textbox that does not allow spaces to be typed, this will not allow them tp be typed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void TextNoSpace_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = e.Key == Key.Space;
        }

        /// <summary>
        /// Text input that checks and allows text that only allows numbers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regexNoAlphabet.IsMatch(e.Text);
        }

        /// <summary>
        /// Paste Handler to deny any spaces in the string being pasted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnPasteNoAlphabet(object sender, DataObjectPastingEventArgs e)
        {
            String text = (String)e.SourceDataObject.GetData(DataFormats.UnicodeText);

            if (text.Contains(' ') || _regexNoAlphabet.IsMatch(text))
                e.CancelCommand();
        }
    }
}
