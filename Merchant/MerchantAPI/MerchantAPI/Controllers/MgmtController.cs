using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web.Http;
using System.Web.Http.Results;
using MerchantAPI.Controllers.Factories;

namespace MerchantAPI.Controllers
{
    public class MgmtController : ApiController
    {

        [HttpGet]
        [ActionName("settings")]
        public JsonResult<SettingsModel> Settings()
        {
            SettingsModel settings = new SettingsModel
            {
                IsTestingMode = WebApiConfig.Settings.IsTestingMode,
                Version = WebApiConfig.Settings.Version,
                ClientId = WebApiConfig.Settings.ClientId,
                PublicServerName = WebApiConfig.Settings.PublicServerName,
                PaymentASPXEndpoint = WebApiConfig.Settings.PaymentASPXEndpoint
            };
            return Json(settings, SerializerFactory.CreateJsonSerializerSettings());
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
