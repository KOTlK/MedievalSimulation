using System;
using static Simulation.Runtime.Vars;

namespace Simulation.Runtime.TimeManagement
{
    //Days, months and years since game start
    public struct Date
    {
        public int Days;
        public int Months;
        public int Years;

        public override string ToString()
        {
            return $"{Days.ToString()}, {Months.ToString()}, {Years.ToString()}";
        }
    }

    public enum Season
    {
        Spring,
        Summer,
        Autumn,
        Winter
    }

    public struct DateRange
    {
        public Date From;
        public Date To;
    }

    public static class Clock
    {
        public static Date CurrentDate;
        public static Season CurrentSeason;

        public static void SetStartDate()
        {
            CurrentDate = new Date()
            {
                Days = 1,
                Months = 3,
                Years = 1
            };

            CurrentSeason = Season.Spring;
        }
        
        public static void IncreaseDaysCount()
        {
            CurrentDate.Days++;
            if (CurrentDate.Days > DaysInMonth)
            {
                CurrentDate.Months++;
                CurrentDate.Days = 1;
                if (CurrentDate.Months > MonthsInYear)
                {
                    CurrentDate.Years++;
                    CurrentDate.Months = 1;
                }
                UpdateSeason();
            }
        }

        public static bool DateInRange(Date date, DateRange range)
        {
            if (date.Months < range.From.Months)
                return false;

            if (date.Months > range.To.Months)
                return false;

            if (date.Days < range.From.Days)
                return false;

            if (date.Days > range.To.Days)
                return false;

            return true;
        }

        public static bool CurrentDateInRange(DateRange range)
        {
            return DateInRange(CurrentDate, range);
        }

        private static void UpdateSeason()
        {
            CurrentSeason = CurrentDate.Months switch
            {
                1 => Season.Winter,
                2 => Season.Winter,
                3 => Season.Spring,
                4 => Season.Spring,
                5 => Season.Spring,
                6 => Season.Summer,
                7 => Season.Summer,
                8 => Season.Summer,
                9 => Season.Autumn,
                10 => Season.Autumn,
                11 => Season.Autumn,
                12 => Season.Winter,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}