using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using MerchantAPI.Helpers;
using MerchantAPI.Models;

namespace MerchantAPI.Controllers.Factories
{
    public class CommDooFrontendFactory
    {
        private const string DestinationTimeZoneId = "W. Europe Standard Time";
        private const string CONFIG_SERVER_NAME = "5.149.150.98";
        private const string CONFIG_CLIENT_ID = "99999999";
        private const string CONFIG_SHARED_SECRET = "test";

        private static readonly string SUCC_EXTRA_PATH = "/success";
        private static readonly string FAIL_EXTRA_PATH = "/failure";

        public static NameValueCollection CreateMultyCurrencyPaymentParams(
            int endpointGroupId,
            SaleRequestModel model)
        {
            NameValueCollection data = CreatePaymentParams(model);
            data.Add("relatedinformation-endpointgroupid", "" + endpointGroupId);
            data.Add("hash", CalculateHash(PAYMENT_HASH_KEY_SEQUENSE, data, CONFIG_SHARED_SECRET));
            return data;
        }

        public static NameValueCollection CreateSingleCurrencyPaymentParams(
            int endpointId, 
            SaleRequestModel model)
        {
            NameValueCollection data = CreatePaymentParams(model);
            data.Add("relatedinformation-endpointid", "" + endpointId);
            data.Add("hash", CalculateHash(PAYMENT_HASH_KEY_SEQUENSE, data, CONFIG_SHARED_SECRET));
            return data;
        }

        private static NameValueCollection CreatePaymentParams(
            SaleRequestModel model)
        {
            DateTime now = DateTime.Now;
            NameValueCollection data = new NameValueCollection
            {
                {"clientid", CONFIG_CLIENT_ID},
                {"payment", "creditcard"},
                {"referenceid", model.client_orderid + "-" + now.ToString("yyyyMMddHHmmss.fff")},
                {"orderid", model.client_orderid},
                {"creditcardowner", model.card_printed_name},
                {"firstname", model.first_name},
                {"lastname", model.last_name},
                {"idcardnumber", "" + model.ssn},
                {"dateofbirth", ConvertBirthdayToString(model.birthday)},
                {"street", model.address1},
                {"city", model.city},
                {"state", model.state},
                {"postalcode", model.zip_code},
                {"country", CountryConverter.ConvertCountryToCommDooSpace(model.country)},
                {"phonenumber", String.IsNullOrEmpty(model.phone) ? model.cell_phone : model.phone},
                {"emailaddress", model.email},
                {"amount", model.amount.Replace(".", "")},
                {"currency", model.currency},
                {"creditcardnumber", model.credit_card_number},
                {"expirationmonth", "" + model.expire_month},
                {"expirationyear", "" + model.expire_year},
                {"cvv", "" + model.cvv2},
                {"ipaddress", model.ipaddress},
                {"website", model.site_url},
                {"successurl", ResolveInternalUrl(SUCC_EXTRA_PATH)},
                // {"notificationurl", ""},
                {"failurl", ResolveInternalUrl(FAIL_EXTRA_PATH)},
                {"timestamp", ConvertToWesternEurope(now).ToString("ddMMyyyyHHmmss")},
                {"relatedinformation-orderdescription", model.order_desc}
            };
            return data;
        }

        public static DateTime ConvertToWesternEurope(DateTime utc)
        {
            DateTime westernEurope = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utc, DestinationTimeZoneId);
            return westernEurope;
        }

        public static string ResolveInternalUrl(string extraPath)
        {
            return String.Format("{0}://{1}:{2}{3}{4}",
                HttpContext.Current.Request.Url.Scheme, CONFIG_SERVER_NAME, HttpContext.Current.Request.Url.Port, 
                HttpContext.Current.Request.Url.AbsolutePath, extraPath);
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

        private static string AssemblyHashContent(string[] calculationMap, NameValueCollection data, string sharedSecret)
        {
            StringBuilder content = new StringBuilder(256);
            foreach (var key in calculationMap)
            {
                string value = data[key];
                if (!String.IsNullOrEmpty(value))
                {
                    content.Append(value);
                }
            }
            content.Append(sharedSecret);
            return content.ToString();
        }

        private static string CalculateHash(string[] calculationMap, NameValueCollection data, string sharedSecret)
        {
            string content = AssemblyHashContent(calculationMap, data, sharedSecret);

//            MD5 md5 = new MD5CryptoServiceProvider();
//            byte[] digest = md5.ComputeHash(Encoding.UTF8.GetBytes(hash.ToString()));
//            string base64Digest = Convert.ToBase64String(digest, 0, digest.Length);
//            return base64Digest.Substring(0, base64Digest.Length - 2);
            SHA1CryptoServiceProvider crypto = new SHA1CryptoServiceProvider();
            byte[] digest = crypto.ComputeHash(Encoding.UTF8.GetBytes(content));
            var sb = new StringBuilder(48);
            foreach (byte b in digest)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        /*
         * From YYYYMMDD to DDMMYYYY
         */
        public static long ConvertBirthday(long birthday)
        {
            long day = birthday % 100;
            long month = (birthday % 10000) / 100;
            long year = birthday / 10000;

            return day * 1000000 + month * 10000 + year;
        }

        /*
         * From YYYYMMDD to DDMMYYYY
         */
        public static string ConvertBirthdayToString(long birthday)
        {
            long day = birthday % 100;
            long month = (birthday % 10000) / 100;
            long year = birthday / 10000;

            return String.Format("{0:00}.{1:00}.{2:0000}", day, month, year);
        }
    }
}