using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using MerchantAPI.Controllers.Factories;

namespace MerchantAPI.Controllers
{
    [RoutePrefix("paynet/api/v2/mgmt")]
    public class MgmtController : ApiController
    {
        [HttpGet]
        [Route("settings")]
        public JsonResult<SettingsModel> Settings()
        {
            SettingsModel settings = new SettingsModel
            {
                IsTestingMode = WebApiConfig.Settings.IsTestingMode,
                Version = WebApiConfig.Settings.Version,
                ClientId = WebApiConfig.Settings.ClientId,
                SharedSecret = "<invisibly>",
                PublicServerName = WebApiConfig.Settings.PublicServerName,
                PaymentASPXEndpoint = WebApiConfig.Settings.PaymentASPXEndpoint,
                CacheSlidingExpirationSeconds = WebApiConfig.Settings.CacheSlidingExpirationSeconds,
                MerchantControlKeys = "Collection has " + WebApiConfig.Settings.MerchantControlKeys.Count + " pair(s)"
            };
            return Json(settings, SerializerFactory.CreateJsonSerializerSettings());
        }

        [HttpGet]
        [Route("cache")]
        public JsonResult<CacheData[]> Cache()
        {
            CacheData[] cache = new CacheData[HttpContext.Current.Cache.Count];
            int i = 0;
            foreach (DictionaryEntry entry in HttpContext.Current.Cache)
            {
                cache[i++] = new CacheData(entry.Key.ToString(), entry.Value.ToString());
            }
            return Json(cache, SerializerFactory.CreateJsonSerializerSettings());
        }
    }

    public class SettingsModel
    {
        [DataMember]
        public bool IsTestingMode { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public string ClientId { get; set; }

        [DataMember]
        public string SharedSecret { get; set; }

        [DataMember]
        public string PublicServerName { get; set; }

        [DataMember]
        public string PaymentASPXEndpoint { get; set; }

        [DataMember]
        public int CacheSlidingExpirationSeconds{ get; set; }

        [DataMember]
        public string MerchantControlKeys { get; set; }
    }

    public class CacheData
    {
        public CacheData(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        [DataMember]
        public string key { get; set; }

        [DataMember]
        public string value { get; set; }
    }
}
