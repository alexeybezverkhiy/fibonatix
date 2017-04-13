using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MerchantAPI.Helpers;

namespace MerchantAPI.Models
{
    public class PreAuthFormRequestModel : BaseFibonatixModel
    {
        [Required]
        [StringLength(65535)]
        public string order_desc { get; set; }

        [Required]
        [StringLength(50)]
        public string first_name { get; set; }

        [Required]
        [StringLength(50)]
        public string last_name { get; set; }

        public int ssn { get; set; }   // OPTIONAL

        public long birthday { get; set; }  // OPTIONAL

        [Required]
        [StringLength(50)]
        public string address1 { get; set; }

        [Required]
        [StringLength(50)]
        public string city { get; set; }

        [StringLength(3, MinimumLength = 2)]
        public string state { get; set; }   // CONDITIONAL

        [Required]
        [StringLength(10)]
        public string zip_code { get; set; }

        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string country { get; set; }

        [Required]
        [StringLength(15)]
        public string phone { get; set; }

        [StringLength(15)]
        public string cell_phone { get; set; }  // OPTIONAL

        [Required]
        [StringLength(50)]
        public string email { get; set; }

        [Required]
        [StringLength(12)]
        public string amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string currency { get; set; }

        [Required]
        [StringLength(20)]
        public string ipaddress { get; set; }

        [StringLength(128)]
        public string site_url { get; set; }    // OPTIONAL

        [StringLength(128)]
        public string purpose { get; set; }     // OPTIONAL

        [Required]
        [StringLength(128)]
        public string redirect_url { get; set; }

        [StringLength(128)]
        public string server_callback_url { get; set; } // OPTIONAL

        protected override StringBuilder FillHashContent(StringBuilder builder, int endpoint, string merchantControlKey)
        {
            return builder
                .Append(endpoint)
                .Append(string.IsNullOrEmpty(client_orderid) ? string.Empty : client_orderid)
                .Append(CurrencyConverter.MajorAmountToMinor(amount, currency))
                .Append(string.IsNullOrEmpty(email) ? string.Empty : email)
                .Append(merchantControlKey);
        }
    }

    public class PreAuthFormResponseModel
    {
        private static string SUCC_ASYNCFORM_RESPONSE = "async-form-response";
        private static string FAIL_ERROR = "error";
        private static string FAIL_VALIDATION_ERROR = "validation-error";

        public PreAuthFormResponseModel(string clientOrderId)
        {
            merchant_order_id = clientOrderId;
        }

        public string type { get; set; }
        public string status { get; set; }
        public string paynet_order_id { get; set; }
        public string merchant_order_id { get; set; }
        public string serial_number { get; set; }
        public string error_message { get; set; }
        public string error_code { get; set; }
        public string redirect_url { get; set; }

        public bool IsSucc()
        {
            return string.Equals(SUCC_ASYNCFORM_RESPONSE, type);
        }

        public string ToHttpResponse()
        {
            if (IsSucc())
            {
                return string.Format(
                    "type={0}\n" +
                    "&status={1}\n" +
                    "&paynet-order-id={2}\n" +
                    "&merchant-order-id={3}\n" +
                    "&serial-number={4}\n" +
                    "&redirect_url={5}\n",
                    SUCC_ASYNCFORM_RESPONSE, status, paynet_order_id, merchant_order_id, serial_number, redirect_url);
            }
            return string.Format(
                    "type={0}\n" +
                    "&paynet-order-id={1}\n" +
                    "&merchant-order-id={2}\n" +
                    "&serial-number={3}\n" +
                    "&error-message={4}\n" +
                    "&error-code={5}\n",
                    type, paynet_order_id, merchant_order_id, serial_number,
                    HttpUtility.UrlEncode(error_message), error_code);
        }

        public void SetSucc()
        {
            type = SUCC_ASYNCFORM_RESPONSE;
        }

        public void SetValidationError(string code, string message)
        {
            type = FAIL_VALIDATION_ERROR;
            error_code = code;
            error_message = message;
        }

        public void SetError(string code, string message)
        {
            type = FAIL_ERROR;
            error_code = code;
            error_message = message;
        }
    }

}