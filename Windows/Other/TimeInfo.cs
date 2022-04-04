using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Anime_Organizer.Windows.Other
{
    class TimeInfo
    {
        private int year, month, day, hour, minute;
        private String time;

        public TimeInfo() { }

        public TimeInfo(int day, int hour, int minute)
        {
            year = -1;
            month = -1;
            this.day = day;
            this.hour = hour;
            this.minute = minute;
        }

        public TimeInfo (int day, int hour, int minute, String time)
        {
            year = -1;
            month = -1;
            this.day = day;
            this.hour = hour;
            this.minute = minute;
            this.time = time;
        }

        // This one will be used for StartDate and FinishDate
        public TimeInfo(DateTime date)
        {
            year = date.Year;
            month = date.Month;
            day = date.Day;
            hour = -1;
            minute = -1;
        }

        /// <summary>
		/// Gets or sets the year of this TimeInfo variable
		/// </summary>
        public int Year
        {
            get { return year; }

            set { year = value; }
        }

        /// <summary>
		/// Gets or sets the month of this TimeInfo variable
		/// </summary>
        public int Month
        {
            get { return month; }

            set { month = value; }
        }

        /// <summary>
		/// Gets or sets the day of this TimeInfo variable
		/// </summary>
        public int Day
        {
            get { return day; }

            set { day = value; }
        }

        /// <summary>
		/// Gets or sets the hour of this TimeInfo variable
		/// </summary>
        public int Hour
        {
            get { return hour; }

            set { hour = value; }
        }

        /// <summary>
		/// Gets or sets the minute of this TimeInfo variable
		/// </summary>
        public int Minute
        {
            get { return minute; }

            set { minute = value; }
        }

        /// <summary>
		/// Gets or sets the time of day of this TimeInfo variable. Used mainly for the Converter in the datagrid. Has no other real use.
		/// </summary>
        [JsonIgnore]
        public String Time
        {
            get { return time; }

            set { time = value; }
        }


        /**
         * Original TimeInfo Use:
         * - Day, hour, minute
         * 
         * TimeInfo for Start and finish date.
         * - year, month, day
         * 
         * The check will basically be if the variable has a year or not
         * - I will make it so any non start or finish date will have year and month -1 and maybe the same the other way around?
         * 
         */
        public override String ToString()
        {
            if (year == -1)
            {
                if (time == null) // Military Time
                {
                    return CultureInfo.CurrentCulture.DateTimeFormat.DayNames[day] + "s at " + hour.ToString("D2") + ":" + minute.ToString("D2") + " " + AbbreviationOfLocalTimeZone();
                }
                else // 12-Hour Clock
                {
                    return CultureInfo.CurrentCulture.DateTimeFormat.DayNames[day] + "s at " + hour.ToString() + ":" + minute.ToString("D2") + " " + time + " " + AbbreviationOfLocalTimeZone();
                }
            }
            else
            {
                return month + "/" + day + "/" + year;
            }
        }


        public int ConvertDay(int houroffset, int convertHour, int convertDay)
        {
            int newValue = convertDay, finalHours = convertHour + houroffset;

            if (finalHours < 0) // This means the day has to be changed backwards 1 day
            {
                newValue--;

                if (newValue == -1)
                    newValue = 6;

            }
            else if (finalHours > 23) // This means the day has to be changed forwards 1 day
            {
                newValue++;

                if (newValue == 7)
                    newValue = 0;
            }

            return newValue;
        }

        /// <summary>
        /// Converts the time to local given an offset. this can also be used in reverse to change it back to UTC +00:00.
        /// </summary>
        /// <param name="houroffset"></param>
        /// <param name="convertHour"></param>
        /// <returns></returns>
        public int ConvertHour(int houroffset, int convertHour)
        {
            int finalHours = convertHour + houroffset;

            if (finalHours < 0) // This means the day has to be changed backwards 1 day
            {
                finalHours = 24 + finalHours;
            }
            else if (finalHours > 23) // This means the day has to be changed forwards 1 day
            {
                finalHours = finalHours - 24;
            }

            return finalHours;
        }


        // This runs whenever a column header is clicked (like to sort them columns) for some reason.
        public static String AbbreviationOfLocalTimeZone()
        {
            String timezone = TimeZoneInfo.Local.Id;

            if (TimeZoneInfo.Local.SupportsDaylightSavingTime)
            {
                if (TimeZoneInfo.Local.IsDaylightSavingTime(DateTimeOffset.Now))
                    timezone = TimeZoneInfo.Local.DaylightName;
                else
                    timezone = TimeZoneInfo.Local.StandardName;
            }

            String temp = timezone;
            timezone = "";

            foreach (String word in temp.Split()) // This loop Abbreviates the timezone (ex. Eastern Standard Time --> EST)
                timezone += word.Substring(0, 1);

            return "(" + timezone + ")";
        }



    }
}
