using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MerchantAPI.Helpers;

namespace MerchantAPI.Models
{
    public class SaleRequestModel : BaseFibonatixModel
    {
        [Required]
        [StringLength(65535)]
        public string order_desc { get; set; }

        [Required]
        [StringLength(128)]
        public string card_printed_name { get; set; }

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
//        [StringLength(20, MinimumLength = 16)]
        public string credit_card_number { get; set; }

        [Required]
        public int expire_month { get; set; }

        [Required]
        public int expire_year { get; set; }

        [Required]
        public int cvv2  { get; set; }

        [Required]
        [StringLength(20)]
        public string ipaddress { get; set; }

        [StringLength(128)]
        public string site_url { get; set; }    // OPTIONAL

        [StringLength(19, MinimumLength = 16)]
        public string destination_card_no { get; set; } // CONDITIONAL for Money Send transactions

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
                .Append(string.IsNullOrEmpty(amount) ? string.Empty : amount.Replace(".", string.Empty))
                .Append(string.IsNullOrEmpty(email) ? string.Empty : email)
                .Append(merchantControlKey);
        }
    }

    public class SaleResponseModel
    {
        private static string SUCC_ASYNC_RESPONSE = "async-response";
        private static string FAIL_ERROR = "error";
        private static string FAIL_VALIDATION_ERROR = "validation-error";

        public SaleResponseModel(string clientOrderId)
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
            return String.Equals(SUCC_ASYNC_RESPONSE, type);
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

    public class BasePaymentModel
    {
        [Required]
        public long clientid { get; set; }

        [Required]
        [StringLength(50)]
        public string referenceid { get; set; }

        [StringLength(1024)]
        public string additionaldata { get; set; }

        [Required]
        public long timestamp { get; set; }

        [Required]
        [StringLength(40)]
        public string hash { get; set; }
    }

    public class FailurePaymentModel : BasePaymentModel
    {
        [Required]
        [StringLength(50)]
        public string errornumber { get; set; }

        [Required]
        [StringLength(1024)]
        public string errortext { get; set; }
    }

    public class SuccessPaymentModel : BasePaymentModel
    {
        [Required]
        public long transactionid { get; set; }

        public long subscriptionid { get; set; }

        [Required]
        public long amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string currency { get; set; }

        [Required]
        [StringLength(512)]
        public string paymentmethod { get; set; }

        public long customerid { get; set; }

        [Required]
        [StringLength(50)]
        public string transactionstatus { get; set; }

        [StringLength(256)]
        public string transactionstatusaddition { get; set; }

        [StringLength(50)]
        public string creditcardtype { get; set; }

        [StringLength(256)]
        public string providertransactionid { get; set; }
    }
}