using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MerchantAPI.Helpers;
using System.Threading.Tasks;

namespace MerchantAPI.Models
{
    public class AccountVerificationRequestModel : BaseFibonatixModel
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

        [StringLength(128)]
        public string server_callback_url { get; set; } // OPTIONAL

        protected override StringBuilder FillHashContent(StringBuilder builder, int endpoint, string merchantControlKey)
        {
            return builder
                .Append(endpoint)
                .Append(string.IsNullOrEmpty(client_orderid) ? string.Empty : client_orderid)
                .Append(string.IsNullOrEmpty(email) ? string.Empty : email)
                .Append(merchantControlKey);
        }
    }

    public class AccountVerificationResponseModel
    {
        private readonly static string SUCC_ASYNC_RESPONSE = "async-response";
        private readonly static string FAIL_ERROR = "error";
        private readonly static string FAIL_VALIDATION_ERROR = "validation-error";

        public AccountVerificationResponseModel(string clientOrderId)
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

        public bool IsSucc()
        {
            return string.Equals(SUCC_ASYNC_RESPONSE, type);
        }

        public string ToHttpResponse()
        {
            if (IsSucc())
            {
                return 
                    $"type={SUCC_ASYNC_RESPONSE}\n" +
                    $"&status={status}\n" +
                    $"&paynet-order-id={paynet_order_id}\n" +
                    $"&merchant-order-id={merchant_order_id}\n" + 
                    $"&serial-number={serial_number}\n";
            }
            return 
                $"type={type}\n" + 
                $"&paynet-order-id={paynet_order_id}\n" + 
                $"&merchant-order-id={merchant_order_id}\n" +
                $"&serial-number={serial_number}\n" +
                $"&error-message={HttpUtility.UrlEncode(error_message)}\n" +
                $"&error-code={error_code}\n";
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

        private string assembleStringForHash(string controlKey) {
            string acc = "";
            acc += status;
            acc += paynet_order_id;
            acc += merchant_order_id;
            acc += controlKey;
            return acc;
        }

        public async void asyncSendPostCallback(string url, int endpointId) {
            try {
                if (!String.IsNullOrEmpty(url)) {
                    Helpers.PostRequest post = new PostRequest();

                    string controlKey = WebApiConfig.Settings.GetMerchantControlKey(endpointId);
                    string hash = HashHelper.SHA1(assembleStringForHash(controlKey));

                    string postData = "";
                    if (status != null) postData += ("status=" + status + "&");
                    if (paynet_order_id != null) postData += ("paynet-order-id=" + paynet_order_id + "&");
                    if (error_code != null) postData += ("error-code=" + error_code + "&");
                    if (error_message != null) postData += ("error-message=" + error_message + "&");

                    postData += ("control=" + hash);

                    await Task.Run(() => post.sendRequest(url, System.Text.Encoding.UTF8.GetBytes(postData)));
                }
            } catch { }
        }
    }
}