using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using MerchantAPI.App_Start;
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
            return Json(CollectSettings(), 
                SerializerFactory.CreateJsonSerializerSettings());
        }

        private SettingsModel CollectSettings()
        {
            SettingsModel settings = new SettingsModel
            {
                ApplicationMode = WebApiConfig.Settings.ApplicationMode.ToString(),
                Version = WebApiConfig.Settings.Version,
                PaymentKey = WebApiConfig.Settings.PaymentKey,
                PublicServerName = WebApiConfig.Settings.PublicServerName,
                PaymentASPXEndpoint = WebApiConfig.Settings.PaymentASPXEndpoint,
                BackendServiceUrl = WebApiConfig.Settings.BackendServiceUrlMain,
                ConfigurationServiceUrl = WebApiConfig.Settings.ConfigurationServiceUrl,
                CacheSlidingExpirationSeconds = WebApiConfig.Settings.CacheSlidingExpirationSeconds,
                CacheSlidingExpirationTimeSpan = WebApiConfig.SettingsFactory.CreateCacheSlidingExpiration(),
                ConfigurationsLoaded = $"Collection has {WebApiConfig.Settings.ConfigurationsLoaded} config(s)",
                ConfigurationExpirationSeconds = WebApiConfig.Settings.ConfigurationExpirationSeconds,
                ConfigurationExpirationTimeSpan = WebApiConfig.SettingsFactory.CreateConfigurationExpirationTimeSpan(),
            };
            return settings;
        }

        [HttpGet]
        [Route("cache")]
        public JsonResult<CacheData[]> Cache()
        {
            return Json(CollectCacheData(),
                SerializerFactory.CreateJsonSerializerSettings());
        }

        private CacheData[] CollectCacheData()
        {
            CacheData[] cache = new CacheData[HttpContext.Current.Cache.Count];
            int i = 0;
            foreach (DictionaryEntry entry in HttpContext.Current.Cache)
            {
//                cache[i++] = new CacheData(entry.Key.ToString(), entry.Value.ToString());
                cache[i++] = new CacheData(entry.Key.ToString(), "<hidden>"); // may contain cvv, credit card card number, merchant control key, etc.
            }
            return cache;
        }
    }

    public class SettingsModel
    {
        [DataMember]
        public string ApplicationMode { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public string ClientId { get; set; }

        [DataMember]
        public string SharedSecret { get; set; }

        [DataMember]
        public string PaymentKey { get; set; }

        [DataMember]
        public string PublicServerName { get; set; }

        [DataMember]
        public string PaymentASPXEndpoint { get; set; }

        [DataMember]
        public string BackendServiceUrl { get; set; }

        [DataMember]
        public string ConfigurationServiceUrl { get; set; }

        [DataMember]
        public int CacheSlidingExpirationSeconds { get; set; }

        [DataMember]
        public TimeSpan CacheSlidingExpirationTimeSpan { get; set; }

        [DataMember]
        public string ConfigurationsLoaded { get; set; }

        [DataMember]
        public int ConfigurationExpirationSeconds { get; set; }

        [DataMember]
        public TimeSpan ConfigurationExpirationTimeSpan { get; set; }
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
