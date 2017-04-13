using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MerchantAPI.Models
{

    public class PostbackBaseModel
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
    }

    public class PostbackFailureModel : PostbackBaseModel
    {
        [Required]
        [StringLength(50)]
        public string errornumber { get; set; }

        [Required]
        [StringLength(1024)]
        public string errortext { get; set; }
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
    }

}