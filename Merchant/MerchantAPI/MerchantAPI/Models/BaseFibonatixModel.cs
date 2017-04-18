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
    public abstract class AbstractFibonatixModel
    {
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

    public abstract class BaseFibonatixModel : AbstractFibonatixModel
    {
        [Required]
        [StringLength(128)]
        public string client_orderid { get; set; }
    }

    public enum FibonatixTransactionStatus
    {
        Approved,
        Declined,
        Error,
        Filtered,
        Processing,
        Unknown,
    }

    public enum FibonatixResponseType
    {
        Success,
        Error,
        ValidationError,
    }

    public abstract class AbstractFibonatixResponseModel
    {
        protected static string SUCC_ASYNC_RESPONSE = "async-response";

        protected static string FAIL_ERROR = "error";
        protected static string FAIL_VALIDATION_ERROR = "validation-error";

        public AbstractFibonatixResponseModel()
        {
        }

        protected FibonatixResponseType responseType;

        public string type { get; set; }
        public string serial_number { get; set; }
        public string error_message { get; set; }
        public string error_code { get; set; }

        public bool IsSucc()
        {
            return responseType == FibonatixResponseType.Success;
        }

        public string ToHttpResponse()
        {
            return IsSucc() ? CreateSuccResponse() : CreateFailResponse();
        }

        protected string CreateSuccResponse(string aType)
        {
            return
                $"type={aType}\n" +
                $"&serial-number={serial_number}\n";
        }

        protected virtual string CreateSuccResponse()
        {
            return CreateSuccResponse(SUCC_ASYNC_RESPONSE);
        }

        protected virtual string CreateFailResponse()
        {
            return
                $"type={type}\n" +
                $"&serial-number={serial_number}\n" +
                $"&error-message={HttpUtility.HtmlEncode(error_message)}\n" +
                $"&error-code={error_code}\n";
        }

        public void SetSucc()
        {
            responseType = FibonatixResponseType.Success;
        }

        public void SetValidationError(string code, string message)
        {
            responseType = FibonatixResponseType.ValidationError;
            type = FAIL_VALIDATION_ERROR;
            error_code = code;
            error_message = message;
        }

        public void SetError(string code, string message)
        {
            responseType = FibonatixResponseType.Error;
            type = FAIL_ERROR;
            error_code = code;
            error_message = message;
        }
    }

    public abstract class BaseFibonatixResponseModel : AbstractFibonatixResponseModel
    {
        public BaseFibonatixResponseModel(string clientOrderId) : base()
        {
            merchant_order_id = clientOrderId;
        }

        public string paynet_order_id { get; set; }
        public string merchant_order_id { get; set; }

        protected override string CreateSuccResponse()
        {
            return
                base.CreateSuccResponse() +
                $"&paynet-order-id={paynet_order_id}\n" +
                $"&merchant-order-id={merchant_order_id}\n";
        }

        protected override string CreateFailResponse()
        { 
            return 
                base.CreateFailResponse() +
                $"&paynet-order-id={paynet_order_id}\n" +
                $"&merchant-order-id={merchant_order_id}\n";
        }
    }

}