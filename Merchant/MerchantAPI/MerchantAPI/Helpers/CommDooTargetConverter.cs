using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MerchantAPI.Helpers
{
    public class CommDooTargetConverter
    {
        // See https://www.timeanddate.com/time/zones/cet
        private const string DestinationTimeZoneId = "Central Europe Standard Time";

        public static DateTime ConvertToWesternEurope(DateTime utc)
        {
            DateTime westernEurope = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utc, DestinationTimeZoneId);
            return westernEurope;
        }

        /*
         * From YYYYMMDD to DDMMYYYY
         */
        public static long ConvertBirthday(long birthday)
        {
            long day = birthday % 100;
            long month = (birthday % 10000) / 100;
            long year = birthday / 10000;

            return day * 1000000 + month * 10000 + year;
        }

        /*
         * From YYYYMMDD to DDMMYYYY
         */
        public static string ConvertBirthdayToString(long birthday)
        {
            long day = birthday % 100;
            long month = (birthday % 10000) / 100;
            long year = birthday / 10000;

            return $"{day:00}.{month:00}.{year:0000}";
        }

        /*
        public static string ConvertToMinimalMonetaryUnits(string amount)
        {
            return string.IsNullOrEmpty(amount) ? string.Empty : amount.Replace(".", string.Empty);
        }

        public static string ConvertToMinimalMonetaryUnits(string amount, string currency)
        {
            return ConvertToMinimalMonetaryUnits(amount) + 
                (string.IsNullOrEmpty(amount) ? string.Empty : currency);
        }
        */
    }
}