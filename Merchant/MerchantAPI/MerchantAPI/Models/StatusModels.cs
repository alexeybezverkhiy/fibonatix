using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MerchantAPI.Helpers;

namespace MerchantAPI.Models
{
    public class StatusRequestModel : BaseFibonatixModel
    {
        [Required]
        [StringLength(128)]
        public string login { get; set; }

        [Required]
        [StringLength(128)]
        public string orderid { get; set; }

        [Required]
        [StringLength(128)]
        public string by_request_sn { get; set; }

        protected override StringBuilder FillHashContent(StringBuilder builder, int endpoint)
        {
            return builder
                .Append(login)
                .Append(string.IsNullOrEmpty(client_orderid) ? string.Empty : client_orderid)
                .Append(string.IsNullOrEmpty(orderid) ? string.Empty : orderid)
                ;
        }
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