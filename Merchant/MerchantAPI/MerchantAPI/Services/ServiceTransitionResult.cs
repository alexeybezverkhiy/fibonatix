using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace MerchantAPI.Services
{
    public class ServiceTransitionResult
    {
        public ServiceTransitionResult(HttpStatusCode status, string stringContent)
        {
            Status = status;
            StringContent = stringContent;
        }

        public HttpStatusCode Status { get; set; }
        public string StringContent { get; set; }
    }
}