using JikanDotNet;
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
using System.Windows.Shapes;

namespace Anime_Organizer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MoreInfoWindow : Window
    {
        public MoreInfoWindow(Object obj)
        {
            InitializeComponent();

            Anime anime = (Anime) obj;

            //though i should save this in the event I may need it somewhere else, i should really just cache the images into a separate folder with a specified ID on it
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(anime.CurrentSeason.ImageURL, UriKind.Absolute);
            bitmap.EndInit();
            animeImage.Source = bitmap;

        }

        private async Task<JikanDotNet.Anime> isMethod()
        {
            IJikan jikan = new Jikan();
            JikanDotNet.Anime anime = (await jikan.GetAnimeAsync(35790)).Data;
            
            
            Console.WriteLine(anime.Broadcast); //(ex Wednesday's at 22:00 (JST)) It's a string, so figure it out
            Console.WriteLine(anime.Episodes); //This just gives a number (in string form, but special cases like One Piece have an unknown amount of episodes)
            Console.WriteLine(anime.Genres);   //Tags
            Console.WriteLine(anime.Images.JPG.ImageUrl);    //I plan on using this where if you hover over the anime name for over 2 seconds (or hover an "i" button) a tooltip image will show
            Console.WriteLine(anime.MalId);        //definitely will need this if im going to keep tabs on anime that are currently airing
            Console.WriteLine(anime.Season + " " + anime.Year); //gives season and year (ex. Winter 2019)
            Console.WriteLine(anime.Status); //(ex Finished Airing, Currently Airing)
            Console.WriteLine(anime.Synopsis); //This is basically the description of the anime, not sure if i will use this.
            Console.WriteLine(anime.Title); //(Japanese pronunciation in English) unless this is also in English as i know that is a thing sometimes
            Console.WriteLine(anime.TitleEnglish); //Definitely will be using this. (will need a way to check if the names are too similar so the option to switch aren't there)
            Console.WriteLine(anime.Type);
            //////////////////////////////////
            Console.WriteLine(TimeZoneInfo.Local);

            

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(anime.Images.JPG.ImageUrl, UriKind.Absolute);
            bitmap.EndInit();
            animeImage.Source = bitmap;

            //animeImage.Source = anime.ImageURL;

            return null;
        }



    }




}
