using Anime_Organizer.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Anime_Organizer.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for Titles.xaml
    /// </summary>
    public partial class Titles : UserControl
    {
        /**
         * Control used in:
         * - AddAnime - Done
         * - AddNonMALAnime - Done
         * - AddNonMALSeason - Done
         * - AddSeason - Done
         * - EditName - Done
         */

        public int ChooseAltStyle { get; set; }

        public Titles()
        {
            InitializeComponent();

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Sets the nicknameTBox to the text in the mainTitleTBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Set_Nickname_1_Click(object sender, RoutedEventArgs e)
        {
            if (!mainTBox.Text.Equals("(Japanese Pronunciation)"))
                nicknameTBox.Text = mainTBox.Text;
        }

        /// <summary>
        /// Sets the nicknameTBox to the text in alternateTitleTBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Set_Nickname_2_Click(object sender, RoutedEventArgs e)
        {
            if (!mainTBox.Text.Equals("(Japanese Pronunciation)"))
                nicknameTBox.Text = altTBox.Text;
        }

        /// <summary>
        /// A change of focus to determine if this sample text appears or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainTBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (mainTBox.Text.Equals("(Japanese Pronunciation)"))
            {
                mainTBox.Text = "";
                mainTBox.Foreground = (Brush)FindResource("ControlForeground");
            }
        }

        /// <summary>
        /// A change of focus to determine if this sample text appears or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainTBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (mainTBox.Text.Length == 0)
            {
                mainTBox.Text = "(Japanese Pronunciation)";
                mainTBox.Foreground = (Brush)FindResource("ControlDisabledGlythColor");
            }
        }

        /// <summary>
        /// A change of focus to determine if this sample text appears or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void altTBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (altTBox.Text.Equals("(English Translation)"))
            {
                altTBox.Text = "";
                altTBox.Foreground = (Brush)FindResource("ControlForeground");
            }
        }

        /// <summary>
        /// A change of focus to determine if this sample text appears or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void altTBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (altTBox.Text.Length == 0)
            {
                altTBox.Text = "(English Translation)";
                altTBox.Foreground = (Brush)FindResource("ControlDisabledGlythColor");
            }
        }

        /// <summary>
        /// Changes the layout of the control and edits certain controls to comply with design.
        /// </summary>
        public void ChangeToNoNickname()
        {
            nicknameLabel.Visibility = Visibility.Collapsed;
            nicknameTBox.Visibility = Visibility.Collapsed;
            setNick1.Visibility = Visibility.Collapsed;
            setNick2.Visibility = Visibility.Collapsed;

            mainLabel.Margin = new Thickness(17, 0, 0, 0);
            mainTBox.Margin = new Thickness(128, 2, 20, 0);
            altLabel.Margin = new Thickness(17, 38, 0, 0);
            altTBox.Margin = new Thickness(128, 40, 20, 0);
        }

        /// <summary>
        /// Changes the layout of the control and edits certain controls to comply with design.
        /// </summary>
        public void ReEnable()
        {
            mainTBox.IsEnabled = true;
            altTBox.IsEnabled = true;

            mainTBox.Foreground = (Brush)FindResource("ControlDisabledGlythColor");
            altTBox.Foreground = (Brush)FindResource("ControlDisabledGlythColor");
        }

        /// <summary>
        /// Changes the layout of the control and edits certain controls to comply with design.
        /// </summary>
        public void RemoveTipText()
        {
            mainTBox.GotKeyboardFocus -= mainTBox_GotKeyboardFocus;
            mainTBox.LostKeyboardFocus -= mainTBox_LostKeyboardFocus;

            altTBox.GotKeyboardFocus -= altTBox_GotKeyboardFocus;
            altTBox.LostKeyboardFocus -= altTBox_LostKeyboardFocus;
        }

        /// <summary>
        /// Changes the layout of the control and edits certain controls to comply with design.
        /// </summary>
        public void FixForeground()
        {
            mainTBox.Foreground = (Brush)FindResource("ControlForeground");
            altTBox.Foreground = (Brush)FindResource("ControlForeground");
        }

        /// <summary>
        /// Changes design based of a value to see if it will use an alternate style.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Titles_Loaded(object sender, RoutedEventArgs e)
        {
            if (ChooseAltStyle == 1)
            {
                ChangeToNoNickname();
                ReEnable();
            }
            else if (ChooseAltStyle == 2)
            {
                ReEnable();
            }
            else if (ChooseAltStyle == 3)
            {
                ChangeToNoNickname();
            }
            else if (ChooseAltStyle == 4)
            {
                ChangeToNoNickname();
                ReEnable();
                RemoveTipText();
                FixForeground();
            }
        }
    }
}
