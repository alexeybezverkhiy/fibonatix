using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MerchantAPI.Helpers
{
    public class CommDooHashHelper
    {
        public static string AssemblyHashContent(string[] calculationMap, NameValueCollection data, string sharedSecret)
        {
            StringBuilder content = new StringBuilder(256);
            foreach (var key in calculationMap)
            {
                string value = data[key];
                if (!String.IsNullOrEmpty(value))
                {
                    content.Append(value);
                }
            }
            content.Append(sharedSecret);
            return content.ToString();
        }

        public static string CalculateHash(string[] calculationMap, NameValueCollection data, string sharedSecret)
        {
            string content = AssemblyHashContent(calculationMap, data, sharedSecret);
            return HashHelper.SHA1(content);
        }

    }
}