using Anime_Organizer.Classes;
using Anime_Organizer.Windows.Other;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Data;

namespace Anime_Organizer
{

    // Main Window Converters

    class ScoreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (((double) value) == -1.0)
                return null;
            else
                return (double) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(); // Might need change if these start getting thrown
        }
    }

    public class BroadcastConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool airing = (bool) values[1]; //The one to actually use. This determines if the time it airs will show or not.
            //bool airing = true; // THIS IS USED FOR TESTING CURRENTLY. This shows broadcast day regardless if its airing or not.

            if (airing && values[0] != null) // Runs if the anime is currently airing
            {
                values[0] = ConvertTime(values[0]);

                if (Config.isMilitaryTime)
                {
                    // I'm pretty sure I made these null regardless, as they aren't saved in the file. I'll need to make sure they aren't explicitly set anywhere.
                    ((TimeInfo)values[0]).Time = null;
                    return values;
                }
                else
                {
                    TimeInfo newValue = new TimeInfo();

                    newValue.Year = ((TimeInfo)values[0]).Year;
                    newValue.Month = ((TimeInfo)values[0]).Month;
                    newValue.Day = ((TimeInfo)values[0]).Day;
                    newValue.Hour = ((TimeInfo)values[0]).Hour;
                    newValue.Minute = ((TimeInfo)values[0]).Minute;

                    if (newValue.Hour > 12)
                    {
                        newValue.Hour -= 12;
                        newValue.Time = "pm";
                    }
                    else if (newValue.Hour == 12)
                    {
                        newValue.Time = "pm";
                    }
                    else if (newValue.Hour == 0)
                    {
                        newValue.Hour = 12;
                        newValue.Time = "am";
                    }
                    else
                    {
                        newValue.Time = "am";
                    }

                    // I'm going to be setting values[0] to the new value, but im not entirely sure if this is going to actually set the variable or not.
                    values[0] = newValue;

                    return values;
                }

            }
            else // This will not show the value if the anime is already done airing
            {
                values[0] = null;
                return values;
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(); // Might need change if these start getting thrown
        }

        public Object ConvertTime(Object value)
        {
            TimeInfo newValue = new TimeInfo();

            // localoffset gets the proper offset even if its daylight savings time.
            int localoffset = DateTimeOffset.Now.Offset.Hours;

            newValue.Year = -1;
            newValue.Month = -1;
            newValue.Day = ((TimeInfo) value).ConvertDay(localoffset, ((TimeInfo)value).Hour, ((TimeInfo)value).Day);
            newValue.Hour = ((TimeInfo) value).ConvertHour(localoffset, ((TimeInfo)value).Hour);
            newValue.Minute = ((TimeInfo) value).Minute;

            return newValue;
        }

    }

    class NotesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "Edit Notes";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(); // Might need change if these start getting thrown
        }
    }

    class PlusOneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((int) value) + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(); // Might need change if these start getting thrown
        }
    }

    class TagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<String> array = (List<String>)value;
            String newValue = "";

            foreach (String tag in array)
            {
                newValue += tag + ", ";
            }

            newValue = newValue.Substring(0, newValue.Length - 2);

            return newValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(); // Might need change if these start getting thrown
        }
    }


    class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (File.Exists((String) value))
                return value;
            else
                return Config.environmentPath + "Image Cache\\errorImage.jpg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(); // Might need change if these start getting thrown
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // This is for the dock panel which holds everything in the row details thing.
    class RowDMainWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (double)value - 82; //80 was original
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(); // Might need change if these start getting thrown
        }
    }

    // This is exclusively for the height of the scrollviewer in row details
    class RowDMainHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (double)value - 130;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(); // Might need change if these start getting thrown
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // This is for the editAnime control.

    class EditAnimeMainHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((double)value >= 530)
                return 400;
            else
                return 400 - (530 - (double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(); // Might need change if these start getting thrown
        }
    }

    class EditAnimeMainWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((double)value >= 530 || (530 - (double)value) + 350 <= 350)
                return 350;
            else
                return (540 - (double)value) + 350;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(); // Might need change if these start getting thrown
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Converters That have to do with the MineSweeper window

    class RemainingMinesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "Remaining Mines: " + value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(); // Might need change if these start getting thrown
        }
    }

    class BorderOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (double)value + 2; //30
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(); // Might need change if these start getting thrown
        }
    }



}
