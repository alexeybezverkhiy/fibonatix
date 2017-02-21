using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using MerchantAPI.Helpers;

namespace MerchantAPI.Models
{
    public class CaptureRequestModel : BaseFibonatixModel
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string login { get; set; }

        [Required]
        [StringLength(128)]
        public string orderid { get; set; }

        [StringLength(12)]
        public string amount { get; set; }      // OPTIONAL

        [StringLength(3, MinimumLength = 3)]
        public string currency { get; set; }    // OPTIONAL

        protected override StringBuilder FillHashContent(StringBuilder builder, int endpoint, string merchantControlKey)
        {
            return builder
                .Append(login)
                .Append(client_orderid)
                .Append(orderid)
                .Append(CurrencyConverter.MajorAmountToMinor(amount, currency))
                .Append(currency)
                .Append(merchantControlKey);
        }
    }

    public class CaptureResponseModel : SaleResponseModel
    {
        public CaptureResponseModel(string clientOrderId) : base(clientOrderId)
        {
        }

        public string status { get; set; }
   }
}