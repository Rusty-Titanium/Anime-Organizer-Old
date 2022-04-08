using Anime_Organizer.Windows.Adding;
using Anime_Organizer.Classes;
using JikanDotNet;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Net;
using System.Net.Http;
using System.Windows.Media;

namespace Anime_Organizer.Windows.UserControls
{
    /// <summary>
    /// Interaction logic for MALID.xaml
    /// </summary>
    public partial class MALID : UserControl
    {
        /**
         * Controls that use this:
         * - AddAnime - Done
         * - AddSeason - Done
         */

        private FiveSecondTimer timer;

        public MALID()
        {
            InitializeComponent();

            idTBox.PreviewTextInput += TextInputConfig.TextBox_PreviewTextInput;
            idTBox.PreviewKeyDown += TextInputConfig.TextNoSpace_PreviewKeyDown;

            DataObject.AddPastingHandler(idTBox, TextInputConfig.OnPasteNoAlphabet); // This is to add the paste handler.

            cooldown.Visibility = Visibility.Collapsed;
            timer = new FiveSecondTimer(cooldown, this);

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Runs when the timer is finished.
        /// </summary>
        public void Timer_Finished()
        {
            idButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Gets anime based on ID.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            idButton.Visibility = Visibility.Hidden;

            if (idTBox.Text.Length == 0)
            {
                MessageBox.Show("The MAL ID entered was not valid.", "Anime Organizer", MessageBoxButton.OK, MessageBoxImage.Error);
                idButton.Visibility = Visibility.Visible;
            }
            else
            {
                try
                {
                    IJikan jikan = new Jikan();
                    JikanDotNet.Anime jikanAnime = (await jikan.GetAnimeAsync(int.Parse(idTBox.Text))).Data;

                    if (jikanAnime == null)
                    {
                        MessageBox.Show("The MAL ID entered was not valid.", "Anime Organizer", MessageBoxButton.OK, MessageBoxImage.Error);
                        idButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (((Border)((ScrollViewer)((StackPanel)this.Parent).Parent).Parent).Parent.GetType().Name.Equals("AddAnime"))
                        {
                            AddAnime addAnime = (AddAnime)((Border)((ScrollViewer)((StackPanel)this.Parent).Parent).Parent).Parent;
                            Titles titles = addAnime.titles;

                            addAnime.newAnime = jikanAnime;
                            titles.nicknameTBox.Text = jikanAnime.Title;
                            titles.mainTBox.Text = jikanAnime.Title;
                            titles.altTBox.Text = jikanAnime.TitleEnglish;
                        }
                        else // This means it is AddSeason
                        {
                            AddSeason addSeason = (AddSeason)((Border)((ScrollViewer)((StackPanel)this.Parent).Parent).Parent).Parent;
                            Titles titles = addSeason.titles;

                            addSeason.newAnime = jikanAnime;
                            titles.mainTBox.Text = jikanAnime.Title;
                            titles.altTBox.Text = jikanAnime.TitleEnglish;
                        }

                        cooldown.Visibility = Visibility.Visible;
                        timer.Start();

                    }
                }
                catch (Exception ex) when (ex is WebException || ex is HttpRequestException)
                {
                    ExceptionMessage.InternetError();
                    idButton.Visibility = Visibility.Visible;
                }
            }
        }



    }
}
