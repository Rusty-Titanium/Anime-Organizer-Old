using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Anime_Organizer.MineSweeper
{
    /// <summary>
    /// Interaction logic for Tile.xaml
    /// </summary>
    public partial class Tile : UserControl
    {
        private int actualValue = 0;
        public bool isFlagged = false;
        private MinesweeperWindow window;
        public Tile topLeft, topMiddle, topRight, middleLeft, middleRight, bottomLeft, bottomMiddle, bottomRight;


        public int ActualValue
        {
            get { return actualValue; }

            set { actualValue = value; }
        }

        public Tile(MinesweeperWindow window)
        {
            InitializeComponent();

            this.window = window;
        }

        /// <summary>
        /// Reveals the tile unless flagged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tile_LeftClick(object sender, RoutedEventArgs e)
        {
            if (!isFlagged)
            {
                if (actualValue == 9)
                    window.Clicked_On_Mine(this);
                else if (actualValue != 0)
                {
                    ((ContentControl)button.Content).Style = (Style)FindResource("Minesweeper_" + actualValue);
                    window.NumTilesDisabled++;
                    this.IsEnabled = false;
                    window.Check_Win_Conditions();
                }
                else
                {
                    Reveal_Spaces();
                    window.Check_Win_Conditions();
                }
            }
        }

        /// <summary>
        /// Flags or unflags the clicked tile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tile_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isFlagged)
            {
                ((ContentControl)button.Content).Style = (Style)FindResource("Minesweeper_Flag");
                isFlagged = true;
                window.RemainingMines--;
                window.Check_Win_Conditions();
            }
            else
            {
                ((ContentControl)button.Content).Style = (Style)FindResource("Minesweeper_Hidden");
                isFlagged = false;
                window.RemainingMines++;
            }
        }

        /// <summary>
        /// Reveals the current tile and then runs this method for other tiles depending on certain conditions.
        /// </summary>
        private void Reveal_Spaces()
        {
            ((ContentControl)button.Content).Style = (Style)FindResource("Minesweeper_" + actualValue);
            window.NumTilesDisabled++;
            this.IsEnabled = false;

            if (actualValue == 0)
            {
                if (topLeft != null && topLeft.IsEnabled && !topLeft.isFlagged)
                    topLeft.Reveal_Spaces();

                if (topMiddle != null && topMiddle.IsEnabled && !topMiddle.isFlagged)
                    topMiddle.Reveal_Spaces();

                if (topRight != null && topRight.IsEnabled && !topRight.isFlagged)
                    topRight.Reveal_Spaces();

                if (middleLeft != null && middleLeft.IsEnabled && !middleLeft.isFlagged)
                    middleLeft.Reveal_Spaces();

                if (middleRight != null && middleRight.IsEnabled && !middleRight.isFlagged)
                    middleRight.Reveal_Spaces();

                if (bottomLeft != null && bottomLeft.IsEnabled && !bottomLeft.isFlagged)
                    bottomLeft.Reveal_Spaces();

                if (bottomMiddle != null && bottomMiddle.IsEnabled && !bottomMiddle.isFlagged)
                    bottomMiddle.Reveal_Spaces();

                if (bottomRight != null && bottomRight.IsEnabled && !bottomRight.isFlagged)
                    bottomRight.Reveal_Spaces();
            }

        }



    }
}
