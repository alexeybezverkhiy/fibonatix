using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using MerchantAPI.Helpers;

namespace MerchantAPI.Models
{
    public abstract class BaseFibonatixModel
    {
        [Required]
        [StringLength(128)]
        public string client_orderid { get; set; }

        [Required]
        [StringLength(40, MinimumLength = 40)]
        public string control { get; set; }

        protected abstract StringBuilder FillHashContent(StringBuilder builder, int endpoint, string merchantControlKey);

        public string AssemblyHashContent(int endpoint, string merchantControlKey)
        {
            StringBuilder result = new StringBuilder(128);
            return FillHashContent(result, endpoint, merchantControlKey).ToString();
        }

        public bool IsHashValid(int endpoint, string merchantControlKey)
        {
            if (WebApiConfig.Settings.IsTestingMode) return true;

            string calulatedHash = HashHelper.SHA1(AssemblyHashContent(endpoint, merchantControlKey));
            return string.Equals(control, calulatedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}