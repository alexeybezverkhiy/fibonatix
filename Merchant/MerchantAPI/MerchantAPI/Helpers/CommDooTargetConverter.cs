using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MerchantAPI.Helpers
{
    public class CommDooTargetConverter
    {
        // Either https://www.timeanddate.com/time/zones/west
        // Or https://www.timeanddate.com/time/zones/cet
        private const string DestinationTimeZoneId = "W. Europe Standard Time";

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

            return string.Format("{0:00}.{1:00}.{2:0000}", day, month, year);
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