using Anime_Organizer.Classes;
using Anime_Organizer.Windows.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Anime_Organizer.Themes
{
    /// <summary>
    /// Interaction logic for CustomStyles.xaml
    /// </summary>
    public partial class CustomStyles
    {
        private void Move_Up(object sender, RoutedEventArgs e)
        {
            String rowString = ((System.Windows.Controls.Primitives.DataGridRowHeader)((ContentPresenter)((Button)(sender)).TemplatedParent).TemplatedParent).ToString();
            String number = rowString.Split()[1];
            int index = int.Parse(number) - 1; //This will give us the index of the anime that was clicked

            if (index != 0)
            {
                Anime firstAnime = Anime.animeLists[Config.selectedTab][index - 1];
                Anime secondAnime = Anime.animeLists[Config.selectedTab][index];

                Anime.animeLists[Config.selectedTab].Remove(firstAnime);
                Anime.animeLists[Config.selectedTab].Remove(secondAnime);

                Anime.animeLists[Config.selectedTab].Insert(index - 1, secondAnime);
                Anime.animeLists[Config.selectedTab].Insert(index, firstAnime);

                Config.changesSaved = false;
            }

        }

        private void Move_Down(object sender, RoutedEventArgs e)
        {
            String rowString = ((System.Windows.Controls.Primitives.DataGridRowHeader)((ContentPresenter)((Button)(sender)).TemplatedParent).TemplatedParent).ToString();
            String number = rowString.Split()[1];
            int index = int.Parse(number) - 1; //This will give us the index of the anime that was clicked

            if (index != Anime.animeLists[Config.selectedTab].Count - 1)
            {
                Anime firstAnime = Anime.animeLists[Config.selectedTab][index];
                Anime secondAnime = Anime.animeLists[Config.selectedTab][index + 1];

                Anime.animeLists[Config.selectedTab].Remove(firstAnime);
                Anime.animeLists[Config.selectedTab].Remove(secondAnime);

                Anime.animeLists[Config.selectedTab].Insert(index, secondAnime);
                Anime.animeLists[Config.selectedTab].Insert(index + 1, firstAnime);

                Config.changesSaved = false;
            }
        }


    }
}
