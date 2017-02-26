using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using MerchantAPI.Helpers;

namespace MerchantAPI.Models
{
    public class CallbackRequestModel : BaseFibonatixModel
    {
        private readonly string merchantControlKey;
        private readonly string merchantOwnParams;

        public string status { get; set; }
        public string orderid { get; set; }
        public string type { get; set; }
        public string amount { get; set; }
        public string descriptor { get; set; }
        public string error_code { get; set; }
        public string error_message { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string approval_code { get; set; }
        public int last_four_digits { get; set; }
        public string bin { get; set; }
        public string card_type { get; set; }
        public string gate_partial_reversal { get; set; }
        public string gate_partial_capture { get; set; }
        public string reason_code { get; set; }
        public string processor_rrn { get; set; }
        public string comment { get; set; }
        public string rapida_balance { get; set; }
        public string merchantdata { get; set; }

        public CallbackRequestModel(string merchantControlKey, string merchantOwnParams)
        {
            this.merchantControlKey = merchantControlKey;
            this.merchantOwnParams = merchantOwnParams;
        }

        public string ToHttpParameters()
        {
            StringBuilder builder = new StringBuilder(256);
            if (!string.IsNullOrEmpty(merchantOwnParams))
            {
                builder.Append(merchantOwnParams).Append('&');
            }

            builder
                .Append("status=").Append(status)
                .Append("&merchant_order=").Append(client_orderid)
                .Append("&client_orderid=").Append(client_orderid)
                .Append("&orderid=").Append(orderid)
                .Append("&type=").Append(type)
                .Append("&amount=").Append(amount);

            AppendIfNotEmpty("descriptor", HttpUtility.UrlEncode(descriptor), builder);
            AppendIfNotEmpty("error_code", error_code, builder);
            AppendIfNotEmpty("error_message", HttpUtility.UrlEncode(error_message), builder);
            AppendIfNotEmpty("name", HttpUtility.UrlEncode(name), builder);
            AppendIfNotEmpty("email", email, builder);
            AppendIfNotEmpty("approval-code", approval_code, builder);

            builder.Append("&last-four-digits=").Append(last_four_digits);

            AppendIfNotEmpty("bin", bin, builder);
            AppendIfNotEmpty("card-type", card_type, builder);
            AppendIfNotEmpty("gate-partial-reversal", gate_partial_reversal, builder);
            AppendIfNotEmpty("gate-partial-capture", gate_partial_capture, builder);
            AppendIfNotEmpty("reason-code", reason_code, builder);
            AppendIfNotEmpty("processor-rrn", processor_rrn, builder);
            AppendIfNotEmpty("comment", HttpUtility.UrlEncode(comment), builder);
            AppendIfNotEmpty("rapida-balance", rapida_balance, builder);

            control = CalculateHash();
            builder.Append("&control=").Append(control);

            AppendIfNotEmpty("merchantdata", HttpUtility.UrlEncode(merchantdata), builder);
            return builder.ToString();
        }

        private static void AppendIfNotEmpty(string key, string value, StringBuilder builder)
        {
            if (string.IsNullOrEmpty(value)) return;
            builder.Append('&').Append(key).Append('=').Append(value);
        }

        protected override StringBuilder FillHashContent(StringBuilder builder, int endpoint, string merchantControlKey)
        {
            return builder
                .Append(status)
                .Append(orderid)
                .Append(client_orderid)
                .Append(merchantControlKey);
        }

        private string CalculateHash()
        {
            const int ANY_ENDPOINTID = -1;
            return HashHelper.SHA1(AssemblyHashContent(ANY_ENDPOINTID, merchantControlKey)); ;
        }
    }
}