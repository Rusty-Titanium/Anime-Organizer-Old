using Anime_Organizer.Classes;
using Anime_Organizer.Windows.Adding;
using Anime_Organizer.Windows.Other;
using Anime_Organizer.Windows.UserControls;
using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Anime_Organizer.Windows.Edits
{
    /// <summary>
    /// Interaction logic for EditAnime.xaml
    /// </summary>
    public partial class EditAnime : UserControl
    {
        private Anime anime;
        public bool isAnimeDeleted = false;

        public EditAnime(Object obj1, int tab)
        {
            InitializeComponent();

            anime = (Anime)obj1;
            this.DataContext = anime;
            nicknameLabel.Content = anime.NickNameTitle;
            
            for (int i = 0; i < anime.Seasons.Count; i++)
                CreatePanel(i);

            ShiftNewSeasonPanel();
            FixButtonVisibility();

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Opens a user control to edit non MAL Anime.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit_Non_Mal_Click(object sender, RoutedEventArgs e)
        {
            SeasonControl control = (SeasonControl)((Grid)((Border)((WrapPanel)((Button)sender).Parent).Parent).Parent).Parent;

            EditNonMALSeason control2 = new EditNonMALSeason(anime.Seasons[control.SeasonIndex]);
            editGrid.Children.Add(control2);
            editGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Opens a user control to add a season.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Season_Click(object sender, RoutedEventArgs e)
        {
            AddSeason control = new AddSeason(anime);
            editGrid.Children.Add(control);
            editGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Opens a control to remove a season.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Season_Click(object sender, RoutedEventArgs e)
        {
            SeasonControl control = (SeasonControl)((Grid)((Border)((WrapPanel)((Button)sender).Parent).Parent).Parent).Parent;

            DeleteSeason control2 = new DeleteSeason(anime, anime.Seasons[control.SeasonIndex]);
            editGrid.Children.Add(control2);
            editGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Opens a control to remove this anime.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Anime_Click(object sender, RoutedEventArgs e)
        {
            DeleteAnime control = new DeleteAnime(anime);
            editGrid.Children.Add(control);
            editGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// This will make sure the Add season button is all the way to the right.
        /// </summary>
        public void ShiftNewSeasonPanel()
        {
            dockPanel.Children.Remove(addSeason);
            dockPanel.Children.Add(addSeason);
        }

        /// <summary>
        /// Closes this control and goes back to the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_EditAnime_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Visibility = Visibility.Collapsed;
            ((Grid)this.Parent).Children.Remove(this);

            Application.Current.MainWindow.MinHeight = 410;
        }

        /// <summary>
        /// This plays a system sound "Beep" if a user is trying to interact with something they can't.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (editGrid.Visibility == Visibility.Visible)
                SystemSounds.Beep.Play();
        }

        /// <summary>
        /// Attempts to move this season up 1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Move_Left_Click(object sender, RoutedEventArgs e)
        {
            SeasonControl control = (SeasonControl)((Grid)((Border)((Grid)((Button)sender).Parent).Parent).Parent).Parent;
            int index = control.SeasonIndex;

            if (index != 0)
            {
                Season season1 = anime.Seasons[index];
                Season season2 = anime.Seasons[index - 1];

                if (season1 == anime.CurrentSeason)
                    anime.CurrentSeasonIndex--;
                else if (season2 == anime.CurrentSeason)
                    anime.CurrentSeasonIndex++;

                anime.Seasons.Remove(season1);
                anime.Seasons.Insert(index - 1, season1);

                this.DataContext = null;
                this.DataContext = anime;

                Config.changesSaved = false;
            }
        }

        /// <summary>
        /// Attempts to move the season down 1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Move_Right_Click(object sender, RoutedEventArgs e)
        {
            SeasonControl control = (SeasonControl)((Grid)((Border)((Grid)((Button)sender).Parent).Parent).Parent).Parent;
            int index = control.SeasonIndex;

            if (index != dockPanel.Children.Count - 2)
            {
                Season season1 = anime.Seasons[index];
                Season season2 = anime.Seasons[index + 1];

                if (season1 == anime.CurrentSeason)
                    anime.CurrentSeasonIndex++;
                else if (season2 == anime.CurrentSeason)
                    anime.CurrentSeasonIndex--;

                anime.Seasons.Remove(season1);
                anime.Seasons.Insert(index + 1, season1);

                this.DataContext = null;
                this.DataContext = anime;

                Config.changesSaved = false;
            }
        }

        /// <summary>
        /// Runs to update information or close depending on what occured.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue && isAnimeDeleted)
            {
                ((Grid)this.Parent).Visibility = Visibility.Collapsed;
                ((Grid)this.Parent).Children.Remove(this);
            }
            else
            {
                // Needed for visual updating when an anime is deleted.
                this.DataContext = null;
                this.DataContext = anime;
            }
        }

        /// <summary>
        /// Sets the visibility of the move up or down 1 buttons  to ensure nothing can break the program.
        /// </summary>
        public void FixButtonVisibility()
        {
            // Sets all the buttons visible initially to avoid issues.
            for (int i = 0; i < dockPanel.Children.Count - 1; i++)
            {
                ((SeasonControl)dockPanel.Children[i]).leftButton.Visibility = Visibility.Visible;
                ((SeasonControl)dockPanel.Children[i]).rightButton.Visibility = Visibility.Visible;
            }

            if (dockPanel.Children.Count == 2)
            {
                SeasonControl control = (SeasonControl)dockPanel.Children[0];

                control.leftButton.Visibility = Visibility.Hidden;
                control.rightButton.Visibility = Visibility.Hidden;
            }
            else
            {
                for (int i = 0; i < dockPanel.Children.Count - 1; i++)
                {
                    if (i == 0)
                        ((SeasonControl)dockPanel.Children[i]).leftButton.Visibility = Visibility.Hidden;
                    else if (i == dockPanel.Children.Count - 2)
                        ((SeasonControl)dockPanel.Children[i]).rightButton.Visibility = Visibility.Hidden;
                }
            }
        }

        /// <summary>
        /// This creates a new season panel.
        /// </summary>
        /// <param name="i"></param>
        public void CreatePanel(int i)
        {
            SeasonControl control = new SeasonControl();

            control.seasonIndexLabel.Content = "Season Index: " + (i + 1);
            control.leftButton.Click += Move_Left_Click;
            control.rightButton.Click += Move_Right_Click;

            Binding binding1 = new Binding("Seasons[" + i + "].ImageURL");
            binding1.Converter = new ImageConverter();
            control.picture.SetBinding(Image.SourceProperty, binding1);

            Binding binding2 = new Binding("Seasons[" + i + "].MainTitle");
            control.mainTitleTBlock.SetBinding(TextBlock.TextProperty, binding2);

            Binding binding3 = new Binding("Seasons[" + i + "].AlternateTitle");
            control.altTitleTBlock.SetBinding(TextBlock.TextProperty, binding3);

            control.editNonMALButton.Click += Edit_Non_Mal_Click;

            if (!anime.Seasons[i].MalId.StartsWith("A"))
                control.editNonMALButton.Visibility = Visibility.Collapsed;

            control.deleteSeasonButton.Click += Delete_Season_Click;
            
            Binding binding4 = new Binding();
            binding4.ElementName = "addSeason";
            binding4.Path = new PropertyPath("ActualHeight");
            control.SetBinding(Border.HeightProperty, binding4);

            Binding binding5 = new Binding();
            binding5.ElementName = "addSeason";
            binding5.Path = new PropertyPath("ActualWidth");
            control.SetBinding(Border.WidthProperty, binding5);

            dockPanel.Children.Add(control);
        }

        /// <summary>
        /// When you need to scroll left or right, doing a normal scroll wheel up or down will move it properly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;

            if (e.Delta > 0)
            {
                scrollviewer.LineLeft();
                scrollviewer.LineLeft();
                scrollviewer.LineLeft();
                scrollviewer.LineLeft();
            }
            else
            {
                scrollviewer.LineRight();
                scrollviewer.LineRight();
                scrollviewer.LineRight();
                scrollviewer.LineRight();
            }

            e.Handled = true;
        }

    }
}
