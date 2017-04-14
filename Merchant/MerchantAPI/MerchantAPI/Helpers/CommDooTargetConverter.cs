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

        public static DateTime ConvertToCentralEurope(DateTime utc)
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
            if (birthday == 0)
                return null;

            long day = birthday % 100;
            long month = (birthday % 10000) / 100;
            long year = birthday / 10000;

            return $"{day:00}.{month:00}.{year:0000}";
        }

        // Rules from https://creditcardjs.com/credit-card-type-detection
        private static string[] AmericanExpress = { "American Express", "34", "37" };
        private static string[] ChinaUnionPay = { "China UnionPay", "62", "88" };
        private static string[] DinersClub = { "Diners Club", "300", "301", "302", "303", "304", "305", "309", "36", "38", "39", "54", "55" };
        private static string[] DiscoverCard = { "Discover Card", "6011", "622126..622925", "644", "645", "646", "647", "648", "649", "65" };
        private static string[] JCB = { "JCB", "3528..3589" };
        private static string[] Laser = { "Laser", "6304", "6706", "6771", "6709" };
        private static string[] Maestro = { "Maestro", "5018", "5020", "5038", "5612", "5893", "6304", "6759", "6761", "6762", "6763", "0604", "6390" };
        private static string[] Dankort = { "Dankort", "5019" };
        private static string[] Visa = { "Visa", "4" };
        private static string[] MasterCard = { "MasterCard", "50", "51", "52", "53", "54", "55" };
        private static string[] VisaElectron = { "Visa Electron", "4026", "417500", "4405", "4508", "4844", "4913", "4917" };

        public static string getCardType(string cardNumber) {
            foreach (var prefix in AmericanExpress)
                if (cardNumber.StartsWith(prefix)) return AmericanExpress[0].ToUpper();
            foreach (var prefix in ChinaUnionPay)
                if (cardNumber.StartsWith(prefix)) return ChinaUnionPay[0].ToUpper();
            foreach (var prefix in DinersClub)
                if (cardNumber.StartsWith(prefix)) return DinersClub[0].ToUpper();

            foreach (var prefix in DiscoverCard)
                if (cardNumber.StartsWith(prefix)) return DiscoverCard[0].ToUpper();
            int iprefix = int.Parse(cardNumber.Substring(0, 6));
            if (iprefix >= 622126 && iprefix <= 622925) return DiscoverCard[0].ToUpper();

            foreach (var prefix in JCB)
                if (cardNumber.StartsWith(prefix)) return JCB[0].ToUpper();
            iprefix = int.Parse(cardNumber.Substring(0, 4));
            if (iprefix >= 3528 && iprefix <= 3589) return JCB[0].ToUpper();

            foreach (var prefix in Laser)
                if (cardNumber.StartsWith(prefix)) return Laser[0].ToUpper();
            foreach (var prefix in Maestro)
                if (cardNumber.StartsWith(prefix)) return Maestro[0].ToUpper();
            foreach (var prefix in Dankort)
                if (cardNumber.StartsWith(prefix)) return Dankort[0].ToUpper();
            foreach (var prefix in MasterCard)
                if (cardNumber.StartsWith(prefix)) return MasterCard[0].ToUpper();
            foreach (var prefix in VisaElectron)
                if (cardNumber.StartsWith(prefix)) return VisaElectron[0].ToUpper();
            foreach (var prefix in Visa)
                if (cardNumber.StartsWith(prefix)) return Visa[0].ToUpper();

            return "UNKNOWN";
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