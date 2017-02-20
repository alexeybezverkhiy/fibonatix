using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Http.ModelBinding;
using MerchantAPI.Helpers;

namespace MerchantAPI.Models
{
    public class NotificationRequestExtraModel
    {
        [StringLength(512)]
        public string customernotifyurl { get; set; }
        [StringLength(512)]
        public string fibonatixID { get; set; }
    }

    public class NotificationRequestModel
    {
        [StringLength(512)]
        public string customernotifyurl { get; set; }
        [StringLength(512)]
        public string fibonatixID { get; set; }

        [Required]
        [StringLength(128)]
        public string clientid { get; set; }

        [Required]
        [StringLength(128)]
        public string transactionid { get; set; }

        [Required]
        [StringLength(128)]
        public string referenceid { get; set; }

        [Required]
        public int amount { get; set; } // the paid amount in minimal monetary unit (e.g. cent, penny, etc.)

        /**
         * the triple-digit ISO-Code of currency (ISO 4217) used for this transaction
         * See https://en.wikipedia.org/wiki/ISO_4217
         */
        [Required]
        public int currency { get; set; }

        [Required]
        [StringLength(32)]
        public string paymentmethod { get; set; }

        /**
         * Set of { Charged | Reserved | ChargedBack | ChargeBackReversed | Refunded }
         */
        [Required]
        [StringLength(32)]
        public string transactionstatus { get; set; }   

        public string transactionstatusaddition { get; set; }   // OPTIONAL

        /**
         * Format: ddmmyyyyhhmmss  (Central Europ. Time) 
         */
        [Required]
        [StringLength(14)]
        public string transactiondate { get; set; }

        /**
         * Format: ddmmyyyyhhmmss  (Central Europ. Time) 
         */
        [Required]
        [StringLength(14)]
        public string timestamp { get; set; }     

        /**
         * the triple-digit ISO-Code of user’s country  (ISO 3166-1 alpha-3) 
         * See http://www.nationsonline.org/oneworld/country_code_list.htm
         */
        [Required]
        // will tryed to use https://ole.michelsen.dk/blog/bind-a-model-property-to-a-different-named-query-string-field.html
        public int customer_country { get; set; }

        [Required]
        [StringLength(40)]
        public string hash { get; set; }

        public bool IsHashValid(string sharedSecret)
        {
            if (WebApiConfig.Settings.IsTestingMode) return true;
            
            string calculatedHash = HashHelper.SHA1(AssemblyHashContent(sharedSecret));
            return string.Equals(hash, calculatedHash, StringComparison.OrdinalIgnoreCase);
        }

        public string AssemblyHashContent(string sharedSecret)
        {
            StringBuilder result = new StringBuilder(128);
            return FillHashContent(result, sharedSecret).ToString();
        }

        private static string[] NOTIFICATION_HASH_KEY_SEQUENSE =
        {
            "notificationtype",
            "notificationsubtype",
            "clientid",
            "transactionid",
            "referenceid",
            "amount",
            "currency",
            "paymentmethod",
            "transactionstatus",
            "transactionstatusaddition",
            "transactiondate",
            "timestamp",
            "customer-country",
            "custparam",
            "customer-id",
            "customer-firstname",
            "customer-lastname",
            "customer-street",
            "customer-housenumber",
            "customer-postalcode",
            "customer-city",
            "customer-phonenumber",
            "customer-emailaddress",
            "customer-bankaccountnumber",
            "customer-bankcode",
            "customer-creditcardtype",
            "delivery-firstname",
            "delivery-lastname",
            "delivery-street",
            "delivery-housenumber",
            "delivery-postalcode",
            "delivery-city",
            "delivery-state",
            /**
             * skip [x] params a.k.a.
             *             "delivery-country",
             *             "purchaseitem[x]-id",
             *             "purchaseitem[x]-name",
             *             "purchaseitem[x]-description",
             *             "purchaseitem[x]-quantity",
             *             "purchaseitem[x]-totalprice",
             *             "purchaseitem[x]-currency",
             *             "purchaseitem[x]-taxpercentage",
            */
            "providertransactionid",
            "membershipid",
            "membershipstatus",
            "callertelephonenetwork",
            "chargeback-date",
            "chargeback-reason",
            "chargeback-reason2",
            "chargebackreversal-date",
            "chargebackreversal-reason",
            "Shared Secret"
        };

        protected StringBuilder FillHashContent(StringBuilder builder, string sharedSecret)
        {
            return builder
                    .Append(clientid)
                    .Append(transactionid)
                    .Append(referenceid)
                    .Append(amount)
                    .Append(currency)
                    .Append(paymentmethod)
                    .Append(transactionstatus)
                    .Append(transactionstatusaddition)
                    .Append(transactiondate)
                    .Append(timestamp)
                    .Append(customer_country)
//                    .Append()
                    .Append(sharedSecret);
        }
    }
}
 
 