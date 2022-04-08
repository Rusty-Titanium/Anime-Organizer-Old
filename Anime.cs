using JikanDotNet;
using Anime_Organizer.Windows.Other;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Anime_Organizer.Classes;
using System.Diagnostics;

namespace Anime_Organizer
{
	class Anime : INotifyPropertyChanged
	{
		private String nicknameTitle, notes;
		private double score;
		private int currentSeasonIndex;
		private Season currentSeason;
		private List<Season> seasons;
		private TimeInfo startDate, finishDate;

		public event PropertyChangedEventHandler PropertyChanged;

		//public static int selectedTab = 0;
		//public static int profileNumber = 0, selectedTab = 0;
		//public static bool isMilitaryTime = false, changesSaved = true;

		public Anime()
		{
			currentSeasonIndex = 0;
			seasons = new List<Season>();
			startDate = new TimeInfo();
			finishDate = new TimeInfo();
		}

		public static ObservableCollection<Anime>[] animeLists = new ObservableCollection<Anime>[] { new ObservableCollection<Anime>(),
			new ObservableCollection<Anime>(), new ObservableCollection<Anime>(), new ObservableCollection<Anime>(),
			new ObservableCollection<Anime>(), new ObservableCollection<Anime>(), new ObservableCollection<Anime>()};

		public static ObservableCollection<Anime>[] indexAnimeList()
		{

			String jsonString = File.ReadAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\animelist.json");
			animeLists = JsonSerializer.Deserialize<ObservableCollection<Anime>[]>(jsonString);

			for (int i = 0; i < 6; i++)
			{
				foreach (Anime anime in animeLists[i])
				{
					int index = 0;

					if (anime.Seasons[index].Episodes != -1)
					{
						if (i != 4) // This checks if it's in the finished category.
						{
							while (anime.CurrentSeason == null)
							{
								// This sets the Currently Used Season based on if episodes is less than the max count
								if (anime.Seasons[index].LastWatched.Episode < anime.Seasons[index].Episodes || anime.seasons.Count == index + 1)
								{
									anime.CurrentSeasonIndex = index;
									anime.CurrentSeason = anime.Seasons[anime.CurrentSeasonIndex];
								}
								else
									index++;
							}
						}
						else // Runs only if it's in the finished category.
						{
							anime.CurrentSeasonIndex = anime.Seasons.Count - 1;
							anime.CurrentSeason = anime.Seasons[anime.CurrentSeasonIndex];
						}
					}
					else // This runs if episodes = -1. this means it is unknown how many there are (I.E. One Piece)
					{
						anime.CurrentSeasonIndex = 0;
						anime.CurrentSeason = anime.Seasons[anime.CurrentSeasonIndex];
					}

					// Sets the ImageURL for Season variables to remove the hard coded aspect this had prior.
					foreach (Season season in anime.Seasons)
					{
						if (!season.MalId.StartsWith("A"))
							season.ImageURL = Config.environmentPath + "Image Cache\\" + season.MalId + ".jpg";
						else
							season.ImageURL = Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\nonMAL Image Cache\\" + season.MalId + ".jpg";
					}

					animeLists[6].Add(anime);
				}
			}

			return animeLists;
		}

		/// <summary>
		/// Gets or sets the title that will be shown in the datagrid.
		/// </summary>
		public String NickNameTitle
		{
			get { return nicknameTitle; }

			set
			{
				nicknameTitle = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets the notes that the used created for the Anime.
		/// </summary>
		public String Notes
		{
			get { return notes; }

			set
			{
				notes = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets the score of an Anime.
		/// </summary>
		public double Score
		{
			get { return score; }

			set
			{
				score = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets the current season index for the index of the currently viewed season.
		/// </summary>
		[JsonIgnore]
		public int CurrentSeasonIndex
		{
			get { return currentSeasonIndex; }

			set
			{
				currentSeasonIndex = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets the current season the program has in view.
		/// </summary>
		[JsonIgnore]
		public Season CurrentSeason
		{
			get { return currentSeason; }

			set
			{
				currentSeason = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets the List of Season variables.
		/// </summary>
		public List<Season> Seasons
		{
			get { return seasons; }

			set { seasons = value; }
		}

		/// <summary>
		/// Gets or sets the Time Info for the date the user started the anime.
		/// </summary>
		public TimeInfo StartDate
		{
			get { return startDate; }

			set { startDate = value; }
		}

		/// <summary>
		/// Gets or sets the TimeInfo for the date the user finished the anime.
		/// </summary>
		public TimeInfo FinishDate
		{
			get { return finishDate; }

			set { finishDate = value; }
		}

		public void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// <summary>
		/// Saves the current information that is stored in the program.
		/// </summary>
		public static void Save()
		{
			Config.changesSaved = true;

			var options = new JsonSerializerOptions { WriteIndented = true };

			// This gets a blank collection as it would not be connected to the variables from the other list anymore after the program is used again.
			animeLists[6] = new ObservableCollection<Anime>();

			String jsonString = JsonSerializer.Serialize(animeLists, options);
			File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\animelist.json", jsonString);

			File.WriteAllText(Config.environmentPath + "Profiles\\Profile " + Config.profileNumber + "\\general notes.txt", ((MainWindow)App.Current.MainWindow).getNotes());
		}

		/// <summary>
		/// Converts the jikan Anime to this Anime object.
		/// </summary>
		/// <param name="anime"></param>
		/// <param name="nickname"></param>
		/// <param name="newWebsite"></param>
		/// <param name="newScore"></param>
		/// <returns></returns>
		public static Anime AnimeObjectConverter(JikanDotNet.Anime anime, String nickname, String newWebsite, double newScore)
		{
			Anime newAnime = new Anime();

			newAnime.NickNameTitle = nickname;
			newAnime.Score = newScore;
			newAnime.Notes = "";

			newAnime.addSeason(anime, newWebsite);

			return newAnime;
		}

		/// <summary>
		/// Adds a season to the list of seasons.
		/// </summary>
		/// <param name="season"></param>
		public void addSeason(Season season)
		{
			seasons.Add(season);
		}

		public void addSeason(JikanDotNet.Anime anime, String newWebsite)
		{
			Season season = new Season();

			season.MainTitle = anime.Title;
			season.AlternateTitle = anime.TitleEnglish;
			season.Website = newWebsite;
			season.ImageURL = Config.environmentPath + "Image Cache\\" + anime.MalId + ".jpg";

			if (anime.Aired.From != null)
            {
				DateTime date = (DateTime)anime.Aired.From;
				season.Premiered = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month) + " " + date.Day + ", " + date.Year;
			}
            else
            {
				season.Premiered = "Unknown";
            }

			season.Type = anime.Type;
			season.Description = anime.Synopsis;

			if (anime.Episodes == null)
				season.Episodes = -1;
			else
				season.Episodes = (int)anime.Episodes;

			season.LastWatched = new LastEPWatched();

			season.MalId = anime.MalId.ToString();
			season.Airing = anime.Airing;
			season.Tags = new List<String>();

			foreach (JikanDotNet.MalUrl tag in anime.Genres)
			{
				season.Tags.Add(tag.Name);
			}

			season.Tags.Sort();
			season.Broadcast = CreateBroadcast(anime);
			seasons.Add(season);

			if(seasons.Count == 1)
				CurrentSeason = season;
		}

		/// <summary>
		/// Creates the broadcast for an Anime object.
		/// </summary>
		/// <param name="anime"></param>
		/// <returns></returns>
		public static TimeInfo CreateBroadcast(JikanDotNet.Anime anime)
        {
			// THIS SECTION NEEDS A REVISION IF POSSIBLE. (Probably not going to happen. Same exact thing used in refresh anime so if this is changed, change it there too)
			if (anime.Broadcast != null && !anime.Broadcast.String.Contains("Unknown") && !anime.Broadcast.String.Contains("Not scheduled"))
			{
				// -9 so we can get to UTC +00:00 as native time in Japan is -9.
				int hourDifference = -9;

				// I feel like there has to be a better way to go about this part, but maybe not.
				String[] daysOfTheWeek = CultureInfo.CurrentCulture.DateTimeFormat.DayNames;
				int day = 0;

				for (int i = 0; i < 7; i++)
				{
					if (anime.Broadcast.String.StartsWith(daysOfTheWeek[i]))
						day = i;
				}

				String[] broadcastArray = anime.Broadcast.String.Split();

				int startingHours = int.Parse(broadcastArray[2].Substring(0, 2)), finalHours = startingHours + hourDifference;

				if (finalHours < 0) // This means the day has to be changed backwards 1 day
				{
					day--;
					finalHours = 24 + finalHours;

					if (day == -1)
						day = 6;

				}
				else if (finalHours > 23) // This means the day has to be changed forwards 1 day
				{
					day++;
					finalHours = finalHours - 24;

					if (day == 7)
						day = 0;
				}

				return new TimeInfo(day, finalHours, int.Parse(broadcastArray[2].Substring(3, 2)), null);
			}
			else
            {
				return new TimeInfo();
            }

		}



	}



	class Season : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private String mainTitle, alternateTitle, website, imageURL, premiered, type, description;
		private List<String> tags;
		private double episodes;
		private TimeInfo broadcast;
		private LastEPWatched lastWatched;
		private String malId;
		private bool airing;

		public Season()
		{
			broadcast = new TimeInfo();
		}

		/// <summary>
		/// Gets or sets the main title for the season. (Usually the Japanese Pronunciation)
		/// </summary>
		public String MainTitle
		{
			get { return mainTitle; }

			set
			{
				mainTitle = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets the alternate title for the season. (Usually the English Translation)
		/// </summary>
		public String AlternateTitle
		{
			get { return alternateTitle; }

			set
			{
				alternateTitle = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets the website the user watches this season.
		/// </summary>
		public String Website
		{
			get { return website; }

			set
			{
				website = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets the TimeInfo for when the season air's each week.
		/// </summary>
		public TimeInfo Broadcast
		{
			get { return broadcast; }

			set
			{
				broadcast = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets the file path to an image file. 
		/// </summary>
		[JsonIgnore]
		public String ImageURL
		{
			get { return imageURL; }

			set { imageURL = value; }
		}

		/// <summary>
		/// Gets or sets the string of the date of when this season aired.
		/// </summary>
		public String Premiered
		{
			get { return premiered; }

			set { premiered = value; }
		}

		/// <summary>
		/// Gets or sets the type of Season it is. (TV, movie, OVa, etc.)
		/// </summary>
		public String Type
		{
			get { return type; }

			set { type = value; }
		}

		/// <summary>
		/// Gets or sets the total number of episodes in this season.
		/// </summary>
		public double Episodes
		{
			get { return episodes; }

			set { episodes = value; }
		}

		/// <summary>
		/// Gets or sets the description of the Season.
		/// </summary>
		public String Description
		{
			get { return description; }

			set { description = value; }
		}

		/// <summary>
		/// Gets or sets the list of tags for the Season as strings.
		/// </summary>
		public List<String> Tags
		{
			get { return tags; }

			set { tags = value; }
		}

		/// <summary>
		/// Gets or sets the value of a LastEPWatched variable
		/// </summary>
		public LastEPWatched LastWatched
		{
			get { return lastWatched; }

			set
			{
				lastWatched = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets or Sets the value for the anime's MalId
		/// </summary>
		public String MalId
		{
			get { return malId; }

			set { malId = value; }
		}

		/// <summary>
		/// Gets or sets the value for if the Anime is currently airing.
		/// </summary>
		public bool Airing
		{
			get { return airing; }

			set { airing = value; }
		}

		
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

	}

	class LastEPWatched
	{
		private double episode, season;

		/// <summary>
		/// Creates a LastEPWatched variable with the starting values of 0 for both Episode and Season
		/// </summary>
		public LastEPWatched ()
		{
			episode = 0;
			season = 0;
		}

		/// <summary>
		/// Creates a LastEPWatched variable that sets Episode and Season given the values.
		/// </summary>
		public LastEPWatched(double episode, double season)
		{
			this.episode = episode;
			this.season = season;
		}

		/// <summary>
		/// Gets or sets the episode number the user has currently watched.
		/// </summary>
		public double Episode
		{
			get { return episode; }

			set { episode = value; }
		}

		/// <summary>
		/// Gets or sets the season number the user has currently watched.
		/// </summary>
		public double Season
		{
			get { return season; }

			set { season = value; }
		}

		public override string ToString()
		{
			return "Episode " + episode + "  Season " + season;  
		}

	}




}
