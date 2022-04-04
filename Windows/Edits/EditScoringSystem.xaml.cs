using Anime_Organizer.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Anime_Organizer.Windows.Edits
{
    /// <summary>
    /// Interaction logic for EditScoringSystem.xaml
    /// </summary>
    public partial class EditScoringSystem : UserControl
    {
        public EditScoringSystem()
        {
            InitializeComponent();

            String jsonString = File.ReadAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\scoringsystem.json");
            double[] scoringsystem = JsonSerializer.Deserialize<double[]>(jsonString);

            scoringSystem.minCBox.Text = scoringsystem[0].ToString();
            scoringSystem.maxCBox.Text = scoringsystem[1].ToString();
            scoringSystem.incrementCBox.Text = scoringsystem[2].ToString();
            confirmCancel.confirm.Click += Edit_Save_Click;
            confirmCancel.cancel.Click += Cancel_Click;

            if (Config.useFont.Source != new FontFamily(new Uri("pack://application:,,,/Anime Organizer;component/Fonts/"), "./#CC Wild Words").Source)
                this.FontFamily = Config.useFont;
        }

        /// <summary>
        /// Saves the new scoring system then closes this control to go back to the settings control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit_Save_Click(object sender, RoutedEventArgs e)
        {
            List<double> scoringsystem = new List<double>();

            scoringsystem.Add(double.Parse(scoringSystem.minCBox.Text));
            scoringsystem.Add(double.Parse(scoringSystem.maxCBox.Text));
            scoringsystem.Add(double.Parse(scoringSystem.incrementCBox.Text));

            String jsonString = JsonSerializer.Serialize(scoringsystem);
            File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\scoringsystem.json", jsonString);

            ((Grid)this.Parent).Visibility = Visibility.Collapsed;
            ((Grid)this.Parent).Children.Remove(this);
        }

        /// <summary>
        /// This closes this control and goes back to the settings control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)this.Parent).Visibility = Visibility.Collapsed;
            ((Grid)this.Parent).Children.Remove(this);
        }

        /// <summary>
        /// Runs once this control is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditScoringSystem_Loaded(object sender, RoutedEventArgs e)
        {
            // Focuses the first control the user can edit.
            scoringSystem.minCBox.Focus();
        }


    }
}
