using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MerchantAPI.Data
{
    public class Cache
    {
        public static void setRedirectUrlForRequest(string transactionId, string redirect_url)
        {
            try
            {
                HttpContext.Current.Cache.Insert("redirect_url:" + transactionId, redirect_url, null,
                    System.Web.Caching.Cache.NoAbsoluteExpiration,
                    WebApiConfig.SettingsFactory.CreateCacheSlidingExpiration());
            }
            catch
            {
            }
        }

        public static string getRedirectUrlForRequest(string transactionId)
        {
            string redirectURL = null;
            try
            {
                redirectURL = HttpContext.Current.Cache.Get("redirect_url:" + transactionId).ToString();
            }
            catch
            {
            }
            return redirectURL;
        }

        /*
    public static void setSaleRequestData(string merchant_orderid, Models.SaleRequestModel model) {
        try {
            HttpContext.Current.Cache.Insert("sale_data:" + merchant_orderid, model, null,
                System.Web.Caching.Cache.NoAbsoluteExpiration,
                WebApiConfig.SettingsFactory.CreateCacheSlidingExpiration());
        } catch {
        }
    }
    public static Models.SaleRequestModel getSaleRequestData(string merchant_orderid) {
        Models.SaleRequestModel data = null;
        try {
            data = (Models.SaleRequestModel)HttpContext.Current.Cache.Get("sale_data:" + merchant_orderid);
        } catch {
        }
        return data;
    }
    */
        /*
        public static void setPreAuthRequestData(string merchant_orderid, Models.PreAuthRequestModel model) {
            try {
                HttpContext.Current.Cache.Insert("preauth_data:" + merchant_orderid, model, null,
                    System.Web.Caching.Cache.NoAbsoluteExpiration,
                    WebApiConfig.SettingsFactory.CreateCacheSlidingExpiration());
            } catch {
            }
        }
        public static Models.PreAuthRequestModel getPreAuthRequestData(string merchant_orderid) {
            Models.PreAuthRequestModel data = null;
            try {
                data = (Models.PreAuthRequestModel)HttpContext.Current.Cache.Get("preauth_data:" + merchant_orderid);
            } catch {
            }
            return data;
        }
        */
        /*
        public static void setCaptureRequestData(string merchant_orderid, Models.CaptureRequestModel model) {
            try {
                HttpContext.Current.Cache.Insert("capture_data:" + merchant_orderid, model, null,
                    System.Web.Caching.Cache.NoAbsoluteExpiration,
                    WebApiConfig.SettingsFactory.CreateCacheSlidingExpiration());
            } catch {
            }
        }
        public static Models.CaptureRequestModel getCaptureRequestData(string merchant_orderid) {
            Models.CaptureRequestModel data = null;
            try {
                data = (Models.CaptureRequestModel)HttpContext.Current.Cache.Get("capture_data:" + merchant_orderid);
            } catch {
            }
            return data;
        }
        */

        public static void SetBackendResponseData(string transactionid, CommDoo.BackEnd.Responses.Response.ErrorData reponseError)
        {
            try
            {
                HttpContext.Current.Cache.Insert("backend_response_error:" + transactionid, reponseError, null,
                    System.Web.Caching.Cache.NoAbsoluteExpiration,
                    WebApiConfig.SettingsFactory.CreateCacheSlidingExpiration());
            }
            catch
            {
            }
        }

        public static CommDoo.BackEnd.Responses.Response.ErrorData GetBackendResponseData(string transactionid)
        {
            CommDoo.BackEnd.Responses.Response.ErrorData data = null;
            try
            {
                data = (CommDoo.BackEnd.Responses.Response.ErrorData)HttpContext.Current.Cache.Get("backend_response_error:" + transactionid);
            }
            catch
            {
            }
            return data;
        }

        public static void SetConfiguration(int endpointId, App_Start.MerchantConfig config)
        {
            try
            {
                HttpContext.Current.Cache.Insert("configuration:" + endpointId, config, null,
                    System.Web.Caching.Cache.NoAbsoluteExpiration,
                    WebApiConfig.SettingsFactory.CreateConfigurationExpirationTimeSpan());
            }
            catch
            {
            }
        }

        public static App_Start.MerchantConfig GetConfiguration(int endpointId)
        {
            App_Start.MerchantConfig config = null;
            try
            {
                config = (App_Start.MerchantConfig)HttpContext.Current.Cache.Get("configuration:" + endpointId);
            }
            catch
            {
            }

            return config;
        }
    }
}