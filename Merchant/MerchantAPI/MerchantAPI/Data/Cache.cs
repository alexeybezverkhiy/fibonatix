using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MerchantAPI.Data
{
    public class Cache
    {
        /*
        public static void setRedirectUrlForRequest(string merchant_orderid, string redirect_url) {
            try {
                HttpContext.Current.Cache.Insert("redirect_url:" + merchant_orderid, redirect_url, null,
                    System.Web.Caching.Cache.NoAbsoluteExpiration,
                    WebApiConfig.SettingsFactory.CreateCacheSlidingExpiration());
            } catch {
            }
        }
        public static string getRedirectUrlForRequest(string merchant_orderid) {
            string redirectURL = null;
            try {
                redirectURL = HttpContext.Current.Cache.Get("redirect_url:" + merchant_orderid).ToString();
            } catch {
            }
            return redirectURL;
        }
        */

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

        public static void setBackendResponseData(string transactionid, CommDoo.BackEnd.Responses.Response reponse) {
            try {
                HttpContext.Current.Cache.Insert("backend_response:" + transactionid, reponse, null,
                    System.Web.Caching.Cache.NoAbsoluteExpiration,
                    WebApiConfig.SettingsFactory.CreateCacheSlidingExpiration());
            } catch {
            }
        }
        public static CommDoo.BackEnd.Responses.Response getBackendResponseData(string transactionid) {
            CommDoo.BackEnd.Responses.Response data = null;
            try {
                data = (CommDoo.BackEnd.Responses.Response)HttpContext.Current.Cache.Get("backend_response:" + transactionid);
            } catch {
            }
            return data;
        }

    }
}