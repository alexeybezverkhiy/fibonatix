using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MerchantAPI.Data
{
    public class Cache
    {
        public static void setRedirectUrlForRequest(string merchant_orderid, string redirect_url) {
            HttpContext.Current.Cache.Insert("redirect_url:" + merchant_orderid, redirect_url, null, 
                System.Web.Caching.Cache.NoAbsoluteExpiration, 
                WebApiConfig.SettingsFactory.CreateCacheSlidingExpiration());
        }
        public static string getRedirectUrlForRequest(string merchant_orderid) {
            string redirectURL = HttpContext.Current.Cache.Get("redirect_url:" + merchant_orderid).ToString();
            return redirectURL;
        }
    }
}