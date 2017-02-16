using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web.Http;

namespace MerchantAPI.Controllers
{
    public class MgmtController : ApiController
    {

        [HttpGet]
        [ActionName("settings")]
        public HttpResponseMessage Settings()
        {
            SettingsModel settings = new SettingsModel
            {
                IsTestingMode = WebApiConfig.Settings.IsTestingMode,
                ClientId = WebApiConfig.Settings.ClientId,
                Version = WebApiConfig.Settings.Version,
                PublicServerName = WebApiConfig.Settings.PublicServerName,
                PaymentASPXEndpoint = WebApiConfig.Settings.PaymentASPXEndpoint
            };
            return Request.CreateResponse(HttpStatusCode.OK, settings);
        }
    }

    public class SettingsModel
    {
        [DataMember]
        public bool IsTestingMode { get; set; }
        [DataMember]
        public string ClientId { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string PublicServerName { get; set; }
        [DataMember]
        public string PaymentASPXEndpoint { get; set; }
    }
}
