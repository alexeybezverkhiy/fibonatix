using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using MerchantAPI.Helpers;

namespace MerchantAPI.Models
{

    public abstract class PostbackBaseModel
    {
        [Required]
        public string clientid { get; set; }

        [Required]
        [StringLength(50)]
        public string referenceid { get; set; }

        [Required]
        public string customerredirecturl { get; set; }

        [Required]
        public string fibonatixID { get; set; }

        [StringLength(1024)]
        public string additionaldata { get; set; }

        [Required]
        public string timestamp { get; set; }

        [Required]
        [StringLength(40)]
        public string hash { get; set; }

        protected abstract bool Invalidate();

        private bool BaseInvalidate()
        {
            return !(
                string.IsNullOrEmpty(clientid) ||
                string.IsNullOrEmpty(referenceid) ||
                string.IsNullOrEmpty(timestamp) ||
                string.IsNullOrEmpty(hash)
                );
        }

        protected abstract StringBuilder FillHashContent(StringBuilder builder);

        private string AssemblyHashContent(string sharedSecret)
        {
            return FillHashContent(new StringBuilder(128))
                .Append(sharedSecret)
                .ToString();
        }

        public bool IsHashValid(string sharedSecret)
        {
            if (BaseInvalidate() && Invalidate())
            {
                string calculatedHash = HashHelper.SHA1(AssemblyHashContent(sharedSecret));
                return string.Equals(hash.Trim(), calculatedHash.Trim(),
                    StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }

    public class PostbackFailureModel : PostbackBaseModel
    {
        [Required]
        [StringLength(50)]
        public string errornumber { get; set; }

        [Required]
        [StringLength(1024)]
        public string errortext { get; set; }

        protected override bool Invalidate()
        {
            return !(
                string.IsNullOrEmpty(errornumber) ||
                string.IsNullOrEmpty(errortext)
                );
        }

        private static string[] FAIL_HASH_KEY_SEQUENCE =
        {
            "clientid",
            "referenceid",
            "errornumber",
            "errortext",
            "additionaldata",
            "timestamp"
        };

        protected override StringBuilder FillHashContent(StringBuilder builder)
        {
            return builder
                    .Append(clientid)
                    .Append(referenceid)
                    .Append(errornumber)
                    .Append(errortext)
                    .Append(string.IsNullOrEmpty(additionaldata) ? string.Empty : additionaldata)
                    .Append(timestamp)
                    ;
        }
    }

    public class PostbackSuccessModel : PostbackBaseModel
    {
        [Required]
        public string transactionid { get; set; }

        public string subscriptionid { get; set; }

        [Required]
        public long amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string currency { get; set; }

        [Required]
        [StringLength(512)]
        public string paymentmethod { get; set; }

        public string customerid { get; set; }

        [Required]
        [StringLength(50)]
        public string transactionstatus { get; set; }

        [StringLength(256)]
        public string transactionstatusaddition { get; set; }

        [StringLength(50)]
        public string creditcardtype { get; set; }

        [StringLength(256)]
        public string providertransactionid { get; set; }

        protected override bool Invalidate()
        {
            return !(
                string.IsNullOrEmpty(transactionid) ||
                amount < 0 ||
                string.IsNullOrEmpty(currency) ||
                string.IsNullOrEmpty(paymentmethod) ||
                string.IsNullOrEmpty(transactionstatus)
                );
        }

        private static string[] SUCC_HASH_KEY_SEQUENCE =
        {
            "clientid",
            "transactionid",
            "referenceid",
            "subscriptionid",
            "amount",
            "currency",
            "paymentmethod",
            "customerid",
            "transactionstatus",
            "transactionstatusaddition",
            "creditcardtype",
            "providertransactionid",
            "additionaldata",
            "timestamp"
        };

        protected override StringBuilder FillHashContent(StringBuilder builder)
        {
            return builder
                    .Append(clientid)
                    .Append(transactionid)
                    .Append(referenceid)
                    .Append(string.IsNullOrEmpty(subscriptionid) ? string.Empty : subscriptionid)
                    .Append(amount)
                    .Append(currency)
                    .Append(paymentmethod)
                    .Append(string.IsNullOrEmpty(customerid) ? string.Empty : customerid)
                    .Append(transactionstatus)
                    .Append(string.IsNullOrEmpty(transactionstatusaddition) ? string.Empty : transactionstatusaddition)
                    .Append(string.IsNullOrEmpty(creditcardtype) ? string.Empty : creditcardtype)
                    .Append(string.IsNullOrEmpty(providertransactionid) ? string.Empty : providertransactionid)
                    .Append(string.IsNullOrEmpty(additionaldata) ? string.Empty : additionaldata)
                    .Append(timestamp)
                    ;
        }
    }

}