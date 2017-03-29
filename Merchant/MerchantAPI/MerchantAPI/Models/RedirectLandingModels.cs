using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MerchantAPI.Helpers;

namespace MerchantAPI.Models
{
    public class RedirectResponseModel : BaseFibonatixModel
    {
        public RedirectResponseModel(string clientOrderId, bool succ = true) {
            merchant_order = clientOrderId;
            success = succ;
        }

        public bool success { get; set; }
        public string status { get; set; }
        public string orderid { get; set; }
        public string merchant_order { get; set; }
        public string error_message { get; set; }
        public string descriptor { get; set; }

        protected override StringBuilder FillHashContent(StringBuilder builder, int endpoint, string merchantControlKey) {
            return builder
                .Append(status)
                .Append(string.IsNullOrEmpty(orderid) ? string.Empty : orderid)
                .Append(string.IsNullOrEmpty(merchant_order) ? string.Empty : merchant_order)
                .Append(merchantControlKey);
        }

        public bool IsSucc() {
            return success;
        }

        public string ToHttpResponse() {
            if (IsSucc()) {
                return String.Format(
                    "status={0}\n" +
                    "&orderid={1}\n" +
                    "&merchant-order={2}\n" +
                    "&client-orderid={2}\n" +
                    "&control={3}\n" +
                    "&descriptor={4}\n",
                    status, orderid, merchant_order, client_orderid, control, descriptor);
            }
            return String.Format(
                    "&paynet-order-id={0}\n" +
                    "&merchant-order-id={1}\n" +
                    "&error-message={2}\n" +
                    "&error-code={3}\n",
                    orderid, merchant_order,
                    HttpUtility.UrlEncode(error_message));
        }
    }
}