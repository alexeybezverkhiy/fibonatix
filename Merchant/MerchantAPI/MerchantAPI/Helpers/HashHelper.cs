using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MerchantAPI.Helpers
{
    public class HashHelper
    {
        public static string SHA1(string content)
        {
            SHA1CryptoServiceProvider crypto = new SHA1CryptoServiceProvider();
            byte[] digest = crypto.ComputeHash(Encoding.UTF8.GetBytes(content));
            var sb = new StringBuilder(48);
            foreach (byte b in digest)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        public static StringBuilder AssemblyHashContent(string[] calculationMap, NameValueCollection data, string salt)
        {
            StringBuilder content = new StringBuilder(256);
            foreach (var key in calculationMap)
            {
                string value = data[key];
                if (!string.IsNullOrEmpty(value))
                {
                    content.Append(value);
                }
            }
            content.Append(salt);
            return content;
        }

        public static string CalculateHash(string[] calculationMap, NameValueCollection data, string salt)
        {
            string content = AssemblyHashContent(calculationMap, data, salt).ToString();
            return SHA1(content);
        }

    }
}