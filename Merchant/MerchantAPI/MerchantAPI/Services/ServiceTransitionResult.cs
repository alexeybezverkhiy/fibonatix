using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace MerchantAPI.Services
{
    public class ServiceTransitionResult
    {
        public ServiceTransitionResult(HttpStatusCode status, string content, string newUrl = null)
        {
            Status = status;
            StringContent = content;
            StringLocation = newUrl;
        }

        public HttpStatusCode Status { get; set; }
        public string StringLocation { get; set; }
        public string StringContent { get; set; }
    }
}