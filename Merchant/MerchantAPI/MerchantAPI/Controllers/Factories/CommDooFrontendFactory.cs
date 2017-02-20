using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using MerchantAPI.Helpers;
using MerchantAPI.Models;
using System.Globalization;

namespace MerchantAPI.Controllers.Factories
{

    public class CommDooFrontendFactory
    {
        private static readonly string SUCC_EXTRA_PATH = "/success";
        private static readonly string FAIL_EXTRA_PATH = "/failure";

        public static NameValueCollection CreateMultyCurrencyPaymentParams(
            int endpointGroupId,
            SaleRequestModel model,
            string fibonatixID)
        {
            NameValueCollection data = CreatePaymentParams(model, fibonatixID);
            data.Add("relatedinformation-endpointgroupid", "" + endpointGroupId);
            data.Add("hash", CommDooHashHelper.CalculateHash(PAYMENT_HASH_KEY_SEQUENSE, data, 
                WebApiConfig.Settings.SharedSecret));
            return data;
        }

        public static NameValueCollection CreateSingleCurrencyPaymentParams(
            int endpointId,
            SaleRequestModel model,
            string fibonatixID)
        {
            NameValueCollection data = CreatePaymentParams(model, fibonatixID);
            data.Add("relatedinformation-endpointid", "" + endpointId);
            data.Add("hash", CommDooHashHelper.CalculateHash(PAYMENT_HASH_KEY_SEQUENSE, data,
                WebApiConfig.Settings.SharedSecret));
            return data;
        }

        private static NameValueCollection CreatePaymentParams(
            SaleRequestModel model, string fibonatixID)
        {
            DateTime now = DateTime.Now;
            NameValueCollection data = new NameValueCollection
            {
                {"clientid", WebApiConfig.Settings.ClientId},
                {"payment", "creditcard_fibonatix"},
                {"referenceid", model.client_orderid + "-" + now.ToString("yyyyMMddHHmmss.fff")},
                {"orderid", model.client_orderid},
                {"creditcardowner", model.card_printed_name},
                {"firstname", model.first_name},
                {"lastname", model.last_name},
                {"idcardnumber", "" + model.ssn},
                {"dateofbirth", CommDooTargetConverter.ConvertBirthdayToString(model.birthday)},
                {"street", model.address1},
                {"city", model.city},
                {"state", model.state},
                {"postalcode", model.zip_code},
                {"country", CountryConverter.ConvertCountryToCommDooSpace(model.country)},
                {"phonenumber", String.IsNullOrEmpty(model.phone) ? model.cell_phone : model.phone},
                {"emailaddress", model.email},
                {"amount", CurrencyConverter.MajorAmountToMinor(model.amount, model.currency)},
                {"currency", model.currency},
                {"creditcardnumber", model.credit_card_number},
                {"expirationmonth", "" + model.expire_month},
                {"expirationyear", "" + model.expire_year},
                {"cvv", "" + model.cvv2},
                {"ipaddress", model.ipaddress},
                {"website", model.site_url},
                {"successurl", ResolveInternalUrl(SUCC_EXTRA_PATH) + "?customerredirecturl=" + model.redirect_url + "&fibonatixID=" + fibonatixID},
                {"notificationurl", ResolveInternalNotificationUrl(SUCC_EXTRA_PATH + "?customernotifyurl=" + model.server_callback_url + "&fibonatixID=" + fibonatixID)},
                {"failurl", ResolveInternalUrl(FAIL_EXTRA_PATH) + "?customerredirecturl=" + model.redirect_url + "&fibonatixID=" + fibonatixID},
                {"timestamp", CommDooTargetConverter.ConvertToWesternEurope(now).ToString("ddMMyyyyHHmmss")},
                {"relatedinformation-orderdescription", model.order_desc}
            };
            return data;
        }

        public static string ResolveInternalUrl(string extraPath) {
            return String.Format("{0}://{1}:{2}{3}{4}",
                HttpContext.Current.Request.Url.Scheme,
                // WebApiConfig.Settings.PublicServerName,
                "localhost",
                HttpContext.Current.Request.Url.Port,
                HttpContext.Current.Request.Url.AbsolutePath, extraPath);
        }

        public static string ResolveInternalNotificationUrl(string extraPath) {
            return String.Format("{0}://{1}:{2}{3}{4}",
                HttpContext.Current.Request.Url.Scheme,
                WebApiConfig.Settings.PublicServerName,
                HttpContext.Current.Request.Url.Port,
                "/paynet/api/v2/notification", extraPath);
        }

        private static string[] PAYMENT_HASH_KEY_SEQUENSE =
        {
            "clientid",
            "payment",
            "avskey",
            "paymentmode",
            "referenceid",
            "orderid",
            "customerid",
            "referencecustomerid",
            "language",
            "partially",
            "makepayment",
            "amount",
            "currency",
            "companyname",
            "title",
            "salutation",
            "firstname",
            "lastname",
            "dateofbirth",
            "idcardnumber",
            "street",
            "housenumber",
            "postalcode",
            "city",
            "state",
            "country",
            "phonenumber",
            "emailaddress",
            "deviceid1",
            "deviceid2",
            "ipaddress",
            "bankaccountholder",
            "bankcountry",
            "iban",
            "bic",
            "creditcardnumber",
            "expirationmonth",
            "expirationyear",
            "creditcardtype",
            "cvv",
            "creditcardowner",
            "timeunit",
            "billingcycle",
            "durationperiod",
            "renewalperiod",
            "minimumdurationperiod",
            "maximumdurationperiod",
            "cancellationperiod",
            "trialdurationperiod",
            "trialtimeunit",
            "trialamount",
            "successurl",
            "failurl",
            "backurl",
            "notificationurl",
            "additionaldata",
            "timestamp",
            "delivery-companyname",
            "delivery-salutation",
            "delivery-title",
            "delivery-firstname",
            "delivery-lastname",
            "delivery-street",
            "delivery-housenumber",
            "delivery-postalcode",
            "delivery-city",
            "delivery-state",
            "delivery-country",
            "delivery-phonenumber",
            "delivery-emailaddress",
            // skip ANY 'item[x]-' keys
            "website",
            "layout",
            "relatedinformation-username",
            "relatedinformation-tariff",
            "relatedinformation-tarifftitle",
            "relatedinformation-dateofregistration",
            "relatedinformation-orderdescription",
            "relatedinformation-purpose",
            "relatedinformation-endpointid",
            "relatedinformation-endpointgroupid"
        };

        private static string[] FAIL_HASH_KEY_SEQUENSE =
        {
            "clientid",
            "referenceid",
            "errornumber",
            "errortext",
            "additionaldata",
            "timestamp"
        };

        private static string[] SUCC_HASH_KEY_SEQUENSE =
        {
            "clientid",
            "transactionid",
            "referenceid",
            "subscriptionid",
            "amount",
            "currency",
            "paymentmethod",
            "customerid",
            "transactionstatus",
            "transactionstatusaddition",
            "creditcardtype",
            "providertransactionid",
            "additionaldata",
            "timestamp"
        };
    }
}