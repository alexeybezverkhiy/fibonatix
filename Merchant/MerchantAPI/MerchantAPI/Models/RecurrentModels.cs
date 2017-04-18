using MerchantAPI.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace MerchantAPI.Models
{
    public class CreateCardRefRequestModel : BaseFibonatixModel
    {
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string login { get; set; }

        [Required]
        [StringLength(128)]
        public string orderid { get; set; }

        protected override StringBuilder FillHashContent(StringBuilder builder, int endpoint)
        {
            return builder
                .Append(login)
                .Append(client_orderid)
                .Append(orderid)
                ;
        }
    }

    public class CreateCardRefResponseModel : AbstractFibonatixResponseModel
    {
        private readonly static string CREATE_CARD_REF_RESPONSE = "create-card-ref-response";

        public string status { get; set; }
        public string card_ref_id { get; set; }

        public CreateCardRefResponseModel() : base()
        {
        }

        protected override string CreateSuccResponse()
        {
            return
                CreateSuccResponse(CREATE_CARD_REF_RESPONSE) +
                $"&status={status}" +
                $"&card-ref-id={card_ref_id}";
        }

        protected override string CreateFailResponse()
        {
            return base.CreateFailResponse() + 
                $"&status={status}";
        }
    }

    public class GetCardInfoRequestModel : AbstractFibonatixModel
    {
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string login { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string cardrefid { get; set; }

        protected override StringBuilder FillHashContent(StringBuilder builder, int endpoint)
        {
            return builder
                .Append(login)
                .Append(cardrefid)
                ;
        }
    }

    public class GetCardInfoResponseModel : AbstractFibonatixResponseModel
    {
        private readonly static string GET_CARD_INFO_RESPONSE = "get-card-info-response";

        public string card_printed_name { get; set; }
        public string expire_year { get; set; }
        public string expire_month { get; set; }
        public string bin { get; set; }
        public string last_four_digits { get; set; }
        
        public GetCardInfoResponseModel() : base()
        {
        }

        protected override string CreateSuccResponse()
        {
            return
                CreateSuccResponse(GET_CARD_INFO_RESPONSE) +
                $"&card-printed-name={card_printed_name}" +
                $"&expire-year={expire_year}" +
                $"&expire-month={expire_month}" +
                $"&bin={bin}" +
                $"&last-four-digits={last_four_digits}";
        }
    }

    public class RecurrentRequestModel : BaseFibonatixModel
    {
        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string login { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string cardrefid { get; set; }

        [Required]
        [StringLength(65535)]
        public string order_desc { get; set; }

        [Required]
        [StringLength(12)]
        public string amount { get; set; }

        [StringLength(128)]
        public string enumerate_amounts { get; set; }   // OPTIONAL

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string currency { get; set; }

        public int cvv2 { get; set; }       // OPTIONAL

        [Required]
        [StringLength(20)]
        public string ipaddress { get; set; }

        [StringLength(50)]
        public string comment { get; set; }     // OPTIONAL

        [Required]
        [StringLength(128)]
        public string redirect_url { get; set; }

        [StringLength(128)]
        public string server_callback_url { get; set; }     // OPTIONAL

        protected override StringBuilder FillHashContent(StringBuilder builder, int endpoint)
        {
            return builder
                .Append(login)
                .Append(client_orderid)
                .Append(cardrefid)
                .Append(CurrencyConverter.MajorAmountToMinor(amount, currency))
                .Append(currency)
                ;
        }
    }

    public class RecurrentResponseModel : SaleResponseModel
    {
        public RecurrentResponseModel() : base()
        {
        }

        public RecurrentResponseModel(string clientOrderId) : base(clientOrderId)
        {
            merchant_order_id = clientOrderId;
        }

        public string status { get; set; }

        protected override string CreateSuccResponse()
        {
            return
                base.CreateSuccResponse() +
                $"&status={status}\n";
        }

        protected override string CreateFailResponse()
        {
            return
                base.CreateFailResponse() +
                $"&status={status}\n";
        }
    }

}