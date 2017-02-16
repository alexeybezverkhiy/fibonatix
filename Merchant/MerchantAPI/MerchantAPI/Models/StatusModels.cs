using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MerchantAPI.Models
{
    public class StatusRequestModel
    {
        [Required]
        [StringLength(128)]
        public string login { get; set; }

        [Required]
        [StringLength(128)]
        public string client_orderid { get; set; }

        [Required]
        [StringLength(128)]
        public string orderid { get; set; }

        [Required]
        [StringLength(128)]
        public string by_request_sn { get; set; }

        [Required]
        [StringLength(40, MinimumLength = 40)]
        public string control { get; set; }

    }

    public class StatusResponseModel
    {
        public string type { get; set; }
        public string status { get; set; }
        public string html { get; set; }

        public string ToHttpResponse()
        {
            return String.Format(
                "type={0}" +
                "&status={1}" +
                "&html={2}",
                    type, status, html);
        }
    }
}