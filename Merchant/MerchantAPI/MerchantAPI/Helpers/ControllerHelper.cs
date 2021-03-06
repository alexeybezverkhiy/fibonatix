﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MerchantAPI.Helpers
{
    public class ControllerHelper
    {
        private static char[] HttpParameterDelimiterChars =
        {
            '=',
            '&'
        };

        private static char[] HttpAuthHeaderDelimiterChars =
        {
            '=',
            ','
        };

        private const string PARAM_CVV = "cvv2";
        private const string PARAM_CARD_NUMBER = "credit_card_number";

        public static NameValueCollection EliminateCardData(NameValueCollection pairs)
        {
            string cvv = pairs[PARAM_CVV];
            if (!string.IsNullOrEmpty(cvv))
            {
                pairs[PARAM_CVV] = "***";
            }
            string cardNumber = pairs[PARAM_CARD_NUMBER];
            if (!string.IsNullOrEmpty(cardNumber))
            {
                pairs[PARAM_CARD_NUMBER] = cardNumber.Substring(0, 6) + "******" + LastFourDigits(cardNumber);
            }
            return pairs;
        }

        public static string LastFourDigits(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber)) return string.Empty;
            return cardNumber.Length >= 4
                ? cardNumber.Substring(cardNumber.Length - 4)
                : cardNumber;
        }

        public static NameValueCollection DeserializeHttpParameters(string raw)
        {
            string[] pairs = raw.Split(HttpParameterDelimiterChars);
            if (pairs.Length % 2 != 0)
            {
                throw new ArgumentException($"Length of parsed raw parameters is odd [actual={pairs.Length}], but must be even");
            }
            NameValueCollection result = new NameValueCollection();
            for (int i = 0; i < pairs.Length; i += 2)
            {
                result.Add(pairs[i], HttpUtility.UrlDecode(pairs[i + 1]));
            }
            return result;
        }

        public static string SerializeHttpParameters(NameValueCollection pairs)
        {
            StringBuilder builder = new StringBuilder(256);
            foreach (string key in pairs.AllKeys)
            {
                string value = pairs[key];
                builder
                    .Append(builder.Length > 0 ? "&" : string.Empty)
                    .Append(key)
                    .Append('=')
                    .Append(HttpUtility.UrlEncode(value));
            }
            return builder.ToString();
        }

        public static NameValueCollection DeserializeHeader(string headerName, string raw)
        {
            string[] pairs = raw.Split(HttpAuthHeaderDelimiterChars);
            if (pairs.Length % 2 != 0)
            {
                throw new ArgumentException($"Length of parsed raw parameters of '{headerName}' header" +
                    $" is odd [actual={pairs.Length}], but must be even");
            }
            NameValueCollection result = new NameValueCollection();
            for (int i = 0; i < pairs.Length; i += 2)
            {
                result.Add(pairs[i], HttpUtility.UrlDecode(pairs[i + 1]));
            }
            return result;
        }
    }

    public class RawContentReader
    {
        public static async Task<string> Read(HttpRequestMessage req)
        {
            using (var contentStream = await req.Content.ReadAsStreamAsync())
            {
                contentStream.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(contentStream))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}