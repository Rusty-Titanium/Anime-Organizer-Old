using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AO_Random_Search_Gen
{
    class AnimeSimplified
    {
        private int malID;
        private String type;

        public AnimeSimplified()
        {
        }

		public AnimeSimplified(int malID, String type)
		{
			MalID = malID;
			Type = type;
		}

		/// <summary>
		/// The MyAnimeList ID for an Anime.
		/// </summary>
		public int MalID
		{
			get { return malID; }

			set { malID = value; }
		}

		/// <summary>
		/// The type of an Anime. (Tv, Movie, OVA, etc.)
		/// </summary>
		public String Type
		{
			get { return type; }

			set { type = value; }
		}

	}
}
