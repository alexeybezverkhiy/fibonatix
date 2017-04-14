using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using MerchantAPI.App_Start;
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

        protected abstract StringBuilder FillHashContent(StringBuilder builder, int endpoint);

        public string AssemblyHashContent(int endpoint, string merchantControlKey)
        {
            return FillHashContent(new StringBuilder(128), endpoint)
                .Append(merchantControlKey)
                .ToString();
        }

        public bool IsHashValid(int endpoint, string merchantControlKey)
        {
            if (WebApiConfig.Settings.ApplicationMode == ApplicationMode.TESTING) return true;

            string calulatedHash = HashHelper.SHA1(AssemblyHashContent(endpoint, merchantControlKey));
            return string.Equals(control.Trim(), calulatedHash.Trim(), 
                StringComparison.OrdinalIgnoreCase);
        }
    }

    public abstract class BaseFibonatixResponseModel
    {
        private static string SUCC_ASYNC_RESPONSE = "async-response";
        private static string FAIL_ERROR = "error";
        private static string FAIL_VALIDATION_ERROR = "validation-error";

        public BaseFibonatixResponseModel(string clientOrderId)
        {
            merchant_order_id = clientOrderId;
        }

        public string type { get; set; }
        public string paynet_order_id { get; set; }
        public string merchant_order_id { get; set; }
        public string serial_number { get; set; }
        public string error_message { get; set; }
        public string error_code { get; set; }

        public bool IsSucc()
        {
            return string.Equals(SUCC_ASYNC_RESPONSE, type);
        }

        public string ToHttpResponse()
        {
            if (IsSucc())
            {
                return String.Format(
                    "type={0}" +
                    "&paynet-order-id={1}" +
                    "&merchant-order-id={2}" +
                    "&serial-number={3}",
                    SUCC_ASYNC_RESPONSE, paynet_order_id, merchant_order_id, serial_number);
            }
            return String.Format(
                    "type={0}" +
                    "&paynet-order-id={1}" +
                    "&merchant-order-id={2}" +
                    "&serial-number={3}" +
                    "&error-message={4}" +
                    "&error-code={5}",
                    type, paynet_order_id, merchant_order_id, serial_number,
                    HttpUtility.UrlEncode(error_message), error_code);
        }

        public void SetSucc()
        {
            type = SUCC_ASYNC_RESPONSE;
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