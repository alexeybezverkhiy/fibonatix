using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace MerchantAPI.Models
{
    public class PayoutRequestModel : BaseFibonatixModel
    {
        [Required]
        [StringLength(24)]
        public string account_number { get; set; }

        [Required]
        [StringLength(255)]
        public string bank_name { get; set; }

        [Required]
        [StringLength(255)]
        public string bank_branch { get; set; }

        [Required]
        [StringLength(12)]
        public string amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string currency { get; set; }

        [StringLength(50)]
        public string first_name { get; set; }      // CONDITIONAL

        [StringLength(50)]
        public string last_name { get; set; }       // CONDITIONAL

        [StringLength(15)]
        public string phone { get; set; }           // CONDITIONAL

        [StringLength(16)]
        public string routing_number { get; set; }  // CONDITIONAL

        [StringLength(255)]
        public string bank_province { get; set; }   // OPTIONAL

        [StringLength(255)]
        public string bank_area { get; set; }       // OPTIONAL

        [StringLength(128)]
        public string account_name { get; set; }    // OPTIONAL

        [StringLength(65535)]
        public string order_desc { get; set; }      // OPTIONAL

        [StringLength(128)]
        public string server_callback_url { get; set; } // OPTIONAL

        // OAUTH params

        [Required]
        [StringLength(50)]
        public string oauth_consumer_key { get; set; }

        [Required]
        [StringLength(50)]
        public string oauth_nonce { get; set; }

        [Required]
        [StringLength(50)]
        public string oauth_signature_method { get; set; }

        [Required]
        [StringLength(50)]
        public string oauth_timestamp { get; set; }

        [Required]
        [StringLength(50)]
        public string oauth_version { get; set; }

        protected override StringBuilder FillHashContent(StringBuilder builder, int endpoint)
        {
            return builder
                ;
        }
    }

    public class PayoutResponseModel : BaseFibonatixResponseModel
    {
        public PayoutResponseModel(string clientOrderId) : base(clientOrderId)
        {
        }
    }
}