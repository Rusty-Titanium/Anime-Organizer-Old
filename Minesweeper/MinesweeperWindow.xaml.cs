using Anime_Organizer.Classes;
using Anime_Organizer.Windows.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Anime_Organizer.MineSweeper
{
    /// <summary>
    /// Interaction logic for MinesweeperWindow.xaml
    /// </summary>
    public partial class MinesweeperWindow : Window, INotifyPropertyChanged
    {
        private List<Tile> listOfMines = new List<Tile>(); // Still useful for checking win conditions.
        private int numTilesDisabled = 0, remainingMines = 0;
        private Settings settingsControl;

        public int RemainingMines
        {
            get { return remainingMines; }

            set
            {
                remainingMines = value;
                OnPropertyChanged();
            }
        }

        public int NumTilesDisabled
        {
            get { return numTilesDisabled; }

            set { numTilesDisabled = value; }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }



        public MinesweeperWindow(Settings settings)
        {
            InitializeComponent();

            settingsControl = settings;

            rowsTBox.PreviewKeyDown += TextInputConfig.TextNoSpace_PreviewKeyDown;
            rowsTBox.PreviewTextInput += TextInputConfig.TextBox_PreviewTextInput;
            DataObject.AddPastingHandler(rowsTBox, TextInputConfig.OnPasteNoAlphabet);

            columnsTBox.PreviewKeyDown += TextInputConfig.TextNoSpace_PreviewKeyDown;
            columnsTBox.PreviewTextInput += TextInputConfig.TextBox_PreviewTextInput;
            DataObject.AddPastingHandler(columnsTBox, TextInputConfig.OnPasteNoAlphabet);

            minesTBox.PreviewKeyDown += TextInputConfig.TextNoSpace_PreviewKeyDown;
            minesTBox.PreviewTextInput += TextInputConfig.TextBox_PreviewTextInput;
            DataObject.AddPastingHandler(minesTBox, TextInputConfig.OnPasteNoAlphabet);

            Generate_Grid(10, 10, 10);
        }




        /// <summary>
        /// This checks if values sent to it are valid and will generate the grid if the values are valid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Generate_Grid_Click(object sender, RoutedEventArgs e)
        {
            if (rowsTBox.Text.Length == 0 || columnsTBox.Text.Length == 0 || minesTBox.Text.Length == 0)
                MessageBox.Show("One or more textboxes do not have a value.", "MineSweeper", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (int.Parse(rowsTBox.Text) < 10 || int.Parse(rowsTBox.Text) > 50 || int.Parse(columnsTBox.Text) < 10 || int.Parse(columnsTBox.Text) > 50)
                MessageBox.Show("Row or column has an invalid value. These are valid parameters:\nMin R: 10, Max R: 50\nMin C: 10, Max C: 50", "MineSweeper", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (int.Parse(minesTBox.Text) < 1 || int.Parse(minesTBox.Text) > int.Parse(rowsTBox.Text) * int.Parse(columnsTBox.Text) - 1)
                MessageBox.Show("Mines have an invalid value. These are valid parameters:\nMin M: 1, Max M: R * C - 1", "MineSweeper", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                Generate_Grid(int.Parse(rowsTBox.Text), int.Parse(columnsTBox.Text), int.Parse(minesTBox.Text));
        }

        private void Easy_Click(object sender, RoutedEventArgs e)
        {
            Generate_Grid(10, 10, 10);
        }

        private void Medium_Click(object sender, RoutedEventArgs e)
        {
            Generate_Grid(16, 16, 40);
        }

        private void Hard_Click(object sender, RoutedEventArgs e)
        {
            Generate_Grid(16, 30, 99);
        }

        /// <summary>
        /// This actually generates the grid.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="mines"></param>
        private void Generate_Grid(int rows, int columns, int mines)
        {
            conditionMetRectangle.Visibility = Visibility.Collapsed;

            winLoseLabel.Visibility = Visibility.Collapsed;
            numTilesDisabled = 0;
            grid.Children.Clear();
            listOfMines.Clear();

            RemainingMines = mines;
            grid.Rows = rows;
            grid.Columns = columns;

            for (int i = 0; i < rows * columns; i++)
            {
                Tile tile = new Tile(this);
                grid.Children.Add(tile);
            }

            grid.Width = columns * 40;
            grid.Height = rows * 40;

            List<int> mineList = new List<int>();
            Random random = new Random();
            int numOfMinesPlaced = 0;


            List<int> listOfIndexes = new List<int>();
            for (int i = 0; i < rows * columns; i++)
                listOfIndexes.Add(i);

            
            // Original one
            while (numOfMinesPlaced < mines)
            {
                int mine = random.Next(0, listOfIndexes.Count);
                listOfIndexes.Remove(mine);

                ((Tile)grid.Children[mine]).ActualValue = 9;
                numOfMinesPlaced++;
                mineList.Add(mine);
                listOfMines.Add((Tile)grid.Children[mine]);
            }
            
            // top left, top middle, bottom right, bottom middle, fucking basically all of them wtf

            /**
            // 2, 11, 12, 15, 29, 39, 48, 54, 58, 85
            numOfMinesPlaced = 10;

            ((Tile)grid.Children[2]).ActualValue = 9;
            mineList.Add(2);
            listOfMines.Add((Tile)grid.Children[2]);
            ((Tile)grid.Children[11]).ActualValue = 9;
            mineList.Add(11);
            listOfMines.Add((Tile)grid.Children[11]);
            ((Tile)grid.Children[12]).ActualValue = 9;
            mineList.Add(12);
            listOfMines.Add((Tile)grid.Children[12]);
            ((Tile)grid.Children[15]).ActualValue = 9;
            mineList.Add(15);
            listOfMines.Add((Tile)grid.Children[15]);
            ((Tile)grid.Children[29]).ActualValue = 9;
            mineList.Add(29);
            listOfMines.Add((Tile)grid.Children[29]);
            ((Tile)grid.Children[39]).ActualValue = 9;
            mineList.Add(39);
            listOfMines.Add((Tile)grid.Children[39]);
            ((Tile)grid.Children[48]).ActualValue = 9;
            mineList.Add(48);
            listOfMines.Add((Tile)grid.Children[48]);
            ((Tile)grid.Children[54]).ActualValue = 9;
            mineList.Add(54);
            listOfMines.Add((Tile)grid.Children[54]);
            ((Tile)grid.Children[58]).ActualValue = 9;
            mineList.Add(58);
            listOfMines.Add((Tile)grid.Children[58]);
            ((Tile)grid.Children[85]).ActualValue = 9;
            mineList.Add(85);
            listOfMines.Add((Tile)grid.Children[85]);
             */

            // The numbering here is kind of confusing ngl.
            // This loop creates the placements for the mines
            foreach (int i in mineList)
            {
                if (((Tile)grid.Children[i]).ActualValue == 9)
                {
                    int tempIndex = i - (columns + 1);

                    // Top Left
                    if (tempIndex >= 0 && ((Tile)grid.Children[tempIndex]).ActualValue != 9 && (i - columns) % columns != 0)
                        ((Tile)grid.Children[tempIndex]).ActualValue++;

                    tempIndex = i - columns;

                    // Top Middle
                    if (tempIndex >= 0 && ((Tile)grid.Children[tempIndex]).ActualValue != 9)
                        ((Tile)grid.Children[tempIndex]).ActualValue++;

                    tempIndex = i - (columns - 1);

                    // Top Right
                    if (tempIndex >= 0 && ((Tile)grid.Children[tempIndex]).ActualValue != 9 && (i - columns + 1) % columns != 0)
                        ((Tile)grid.Children[tempIndex]).ActualValue++;

                    tempIndex = i - 1;

                    //Left Middle
                    if (tempIndex >= 0 && ((Tile)grid.Children[tempIndex]).ActualValue != 9 && i % columns != 0)
                        ((Tile)grid.Children[tempIndex]).ActualValue++;

                    tempIndex = i + 1;

                    // Right Middle
                    if (tempIndex < rows * columns && ((Tile)grid.Children[tempIndex]).ActualValue != 9 && tempIndex % columns != 0)
                        ((Tile)grid.Children[tempIndex]).ActualValue++;

                    tempIndex = i + (columns - 1);

                    // Bottom Left
                    if (tempIndex < rows * columns && ((Tile)grid.Children[tempIndex]).ActualValue != 9 && (i + columns) % columns != 0)
                        ((Tile)grid.Children[tempIndex]).ActualValue++;

                    tempIndex = i + columns;

                    // Bottom Middle
                    if (tempIndex < rows * columns && ((Tile)grid.Children[tempIndex]).ActualValue != 9)
                        ((Tile)grid.Children[tempIndex]).ActualValue++;

                    tempIndex = i + columns + 1;

                    // Bottom Right
                    if (tempIndex < rows * columns && ((Tile)grid.Children[tempIndex]).ActualValue != 9 && tempIndex % columns != 0)
                        ((Tile)grid.Children[tempIndex]).ActualValue++;
                }
            }

            // This loop populates the tiles neighbors for use of space clearing later on.

            for (int i = 0; i < grid.Children.Count; i++)
            {
                Tile tile = (Tile)grid.Children[i];
                int tempIndex = i - (columns + 1);

                // Top Left
                if (tempIndex >= 0 && (i - columns) % columns != 0)
                    tile.topLeft = (Tile)grid.Children[tempIndex];

                tempIndex = i - columns;

                // Top Middle
                if (tempIndex >= 0)
                    tile.topMiddle = (Tile)grid.Children[tempIndex];

                tempIndex = i - (columns - 1);

                // Top Right
                if (tempIndex >= 0 && (i - columns + 1) % columns != 0)
                    tile.topRight = (Tile)grid.Children[tempIndex];

                tempIndex = i - 1;

                //Left Middle
                if (tempIndex >= 0 && i % columns != 0)
                    tile.middleLeft = (Tile)grid.Children[tempIndex];

                tempIndex = i + 1;

                // Right Middle
                if (tempIndex < rows * columns && tempIndex % columns != 0)
                    tile.middleRight = (Tile)grid.Children[tempIndex];

                tempIndex = i + (columns - 1);

                // Bottom Left
                if (tempIndex < rows * columns && (i + columns) % columns != 0)
                    tile.bottomLeft = (Tile)grid.Children[tempIndex];

                tempIndex = i + columns;

                // Bottom Middle
                if (tempIndex < rows * columns && ((Tile)grid.Children[tempIndex]).ActualValue != 9)
                    tile.bottomMiddle = (Tile)grid.Children[tempIndex];

                tempIndex = i + columns + 1;

                // Bottom Right
                if (i + columns + 1 < rows * columns && tempIndex % columns != 0)
                    tile.bottomRight = (Tile)grid.Children[tempIndex];



                // This section will change the border thickness of the tile depending where it is.

                int top = 1, left = 1, right = 1, bottom = 1;

                // Thicken Top
                if (i < columns)
                    top = 2;

                // Thicken Left
                if (i % columns == 0)
                    left = 2;

                // Thicken Right
                if (i % columns == columns - 1)
                    right = 2;

                // Thicken Bottom
                if (i >= rows * columns - columns)
                    bottom = 2;

                tile.button.BorderThickness = new Thickness(left, top, right, bottom);
            }
        }

        /// <summary>
        /// runs if a user left clicked a mine.
        /// </summary>
        /// <param name="clickedTile"></param>
        public void Clicked_On_Mine(Tile clickedTile)
        {
            conditionMetRectangle.Visibility = Visibility.Visible;
            winLoseLabel.Visibility = Visibility.Visible;
            winLoseLabel.Content = "You Lose!";

            foreach (Tile tile in grid.Children)
            {
                if (tile == clickedTile)
                    ((ContentControl)tile.button.Content).Style = (Style)tile.FindResource("Minesweeper_ExplodedMine");
                else if (tile.ActualValue == 9)
                    ((ContentControl)tile.button.Content).Style = (Style)tile.FindResource("Minesweeper_Mine");
                else if (tile.isFlagged)
                    ((ContentControl)tile.button.Content).Style = (Style)tile.FindResource("Minesweeper_NotAMine");
            }
        }

        /// <summary>
        /// Runs when the winodw is closing as this prevents multiple minesweeper window to be open.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MineSweeperWindow_Closed(object sender, EventArgs e)
        {
            settingsControl.Minesweeper_Is_Closed();
        }

        /// <summary>
        /// Runs anytime a tile is clicked on to check for win conditions.
        /// </summary>
        public void Check_Win_Conditions()
        {
            if (remainingMines == 0)
            {
                bool allMinesFlagged = true;

                foreach (Tile tile in listOfMines)
                {
                    if (!tile.isFlagged)
                    {
                        allMinesFlagged = false;
                        break;
                    }
                }

                if (allMinesFlagged && numTilesDisabled == grid.Children.Count - listOfMines.Count)
                {
                    winLoseLabel.Content = "You Win!";
                    winLoseLabel.Visibility = Visibility.Visible;
                    conditionMetRectangle.Visibility = Visibility.Visible;
                }
            }
        }


    }
}
