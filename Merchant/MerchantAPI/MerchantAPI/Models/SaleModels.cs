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

        protected override StringBuilder FillHashContent(StringBuilder builder, int endpoint)
        {
            return builder
                .Append(endpoint)
                .Append(string.IsNullOrEmpty(client_orderid) ? string.Empty : client_orderid)
                .Append(CurrencyConverter.MajorAmountToMinor(amount, currency))
                .Append(string.IsNullOrEmpty(email) ? string.Empty : email)
                ;
        }
    }

    public class SaleResponseModel : AbstractFibonatixResponseModel
    {
        public SaleResponseModel() : base()
        {
        }

        public SaleResponseModel(string clientOrderId) : base()
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