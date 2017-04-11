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

        //
        // Sale operation block
        //
        public static NameValueCollection CreateMultiCurrencyPaymentParams(
            int endpointGroupId,
            SaleRequestModel model,
            string fibonatixID)
        {
            NameValueCollection data = CreatePaymentParams(model, fibonatixID, endpointGroupId);
            data.Add("relatedinformation-endpointgroupid", "" + endpointGroupId);
            data.Add("hash", HashHelper.CalculateHash(PAYMENT_HASH_KEY_SEQUENCE, data,
                WebApiConfig.Settings.GetSharedSecret(endpointGroupId)));
            return data;
        }

        public static NameValueCollection CreateSingleCurrencyPaymentParams(
            int endpointId,
            SaleRequestModel model,
            string fibonatixID)
        {
            NameValueCollection data = CreatePaymentParams(model, fibonatixID, endpointId);
            data.Add("relatedinformation-endpointid", "" + endpointId);
            data.Add("hash", HashHelper.CalculateHash(PAYMENT_HASH_KEY_SEQUENCE, data,
                WebApiConfig.Settings.GetSharedSecret(endpointId)));
            return data;
        }

        private static NameValueCollection CreatePaymentParams(
            SaleRequestModel model,
            string fibonatixID,
            int endpointId)
        {
            DateTime now = DateTime.Now;
            NameValueCollection data = new NameValueCollection
            {
                {"clientid", WebApiConfig.Settings.GetClientID(endpointId)},
                {"payment", WebApiConfig.Settings.PaymentKey},
                {"referenceid", model.client_orderid},
                //{"orderid", model.client_orderid},
                {"creditcardowner", model.card_printed_name},
                {"firstname", model.first_name},
                {"lastname", model.last_name},
                {"street", model.address1},
                {"city", model.city},
                {"postalcode", model.zip_code},
                {"country", CountryConverter.ConvertCountryToCommDooSpace(model.country)},
                {"emailaddress", model.email},
                {"amount", CurrencyConverter.MajorAmountToMinor(model.amount, model.currency)},
                {"currency", model.currency},
                {"creditcardnumber", model.credit_card_number},
                {"expirationmonth", "" + model.expire_month},
                {"expirationyear", "" + model.expire_year},
                {"cvv", "" + model.cvv2},
                {"ipaddress", model.ipaddress},
                {"website", model.site_url},
                {"successurl", ResolveInternalUrl(SUCC_EXTRA_PATH) + CreateRedirectParams(model.redirect_url, fibonatixID)},
                {"failurl", ResolveInternalUrl(FAIL_EXTRA_PATH) + CreateRedirectParams(model.redirect_url, fibonatixID)},
                {"timestamp", now.ToString("ddMMyyyyHHmmss")},
            };
            if (model.ssn >= 0)
            {
                data.Add("idcardnumber", "" + model.ssn);
            }
            if (model.birthday >= 0)
            {
                data.Add("dateofbirth", CommDooTargetConverter.ConvertBirthdayToString(model.birthday));
            }
            if (!string.IsNullOrEmpty(model.state))
            {
                data.Add("state", model.state);
            }
            if (!string.IsNullOrEmpty(model.phone))
            {
                data.Add("phonenumber", model.phone);
            }
            else if (!string.IsNullOrEmpty(model.cell_phone))
            {
                data.Add("phonenumber", model.cell_phone);
            }
            if (!string.IsNullOrEmpty(model.server_callback_url))
            {
                data.Add("notificationurl", ResolveInternalNotificationUrl(endpointId, SUCC_EXTRA_PATH)
                    + CreateNotifyParams(model.server_callback_url, fibonatixID));
            }
            if (!string.IsNullOrEmpty(model.order_desc))
            {
                data.Add("relatedinformation-orderdescription", model.order_desc);
            }
            return data;
        }

        //
        // Sale-Form operation block
        //
        public static NameValueCollection CreateMultiCurrencyPaymentParams(
            int endpointGroupId,
            SaleFormRequestModel model,
            string fibonatixID)
        {
            NameValueCollection data = CreatePaymentParams(model, fibonatixID, endpointGroupId);
            data.Add("relatedinformation-endpointgroupid", "" + endpointGroupId);
            data.Add("hash", HashHelper.CalculateHash(PAYMENT_HASH_KEY_SEQUENCE, data,
                WebApiConfig.Settings.GetSharedSecret(endpointGroupId)));
            return data;
        }

        public static NameValueCollection CreateSingleCurrencyPaymentParams(
            int endpointId,
            SaleFormRequestModel model,
            string fibonatixID)
        {
            NameValueCollection data = CreatePaymentParams(model, fibonatixID, endpointId);
            data.Add("relatedinformation-endpointid", "" + endpointId);
            data.Add("hash", HashHelper.CalculateHash(PAYMENT_HASH_KEY_SEQUENCE, data,
                WebApiConfig.Settings.GetSharedSecret(endpointId)));
            return data;
        }

        private static NameValueCollection CreatePaymentParams(
            SaleFormRequestModel model,
            string fibonatixID,
            int endpointId) {
            DateTime now = DateTime.Now;
            NameValueCollection data = new NameValueCollection
            {
                {"clientid", WebApiConfig.Settings.GetClientID(endpointId)},
                {"payment", WebApiConfig.Settings.PaymentKey},
                {"referenceid", model.client_orderid},
                //{"orderid", model.client_orderid},
                {"firstname", model.first_name},
                {"lastname", model.last_name},
                {"street", model.address1},
                {"city", model.city},
                {"postalcode", model.zip_code},
                {"country", CountryConverter.ConvertCountryToCommDooSpace(model.country)},
                {"emailaddress", model.email},
                {"amount", CurrencyConverter.MajorAmountToMinor(model.amount, model.currency)},
                {"currency", model.currency},
                {"ipaddress", model.ipaddress},
                {"website", model.site_url},
                {"successurl", ResolveInternalUrl(SUCC_EXTRA_PATH) + CreateRedirectParams(model.redirect_url, fibonatixID)},
                {"failurl", ResolveInternalUrl(FAIL_EXTRA_PATH) + CreateRedirectParams(model.redirect_url, fibonatixID)},
                {"timestamp", now.ToString("ddMMyyyyHHmmss")},
            };
            if (model.ssn >= 0)
            {
                data.Add("idcardnumber", "" + model.ssn);
            }
            if (model.birthday >= 0)
            {
                data.Add("dateofbirth", CommDooTargetConverter.ConvertBirthdayToString(model.birthday));
            }
            if (!string.IsNullOrEmpty(model.state))
            {
                data.Add("state", model.state);
            }
            if (!string.IsNullOrEmpty(model.phone))
            {
                data.Add("phonenumber", model.phone);
            }
            else if(!string.IsNullOrEmpty(model.cell_phone))
            {
                data.Add("phonenumber", model.cell_phone);
            }
            if (!string.IsNullOrEmpty(model.server_callback_url))
            {
                data.Add("notificationurl", ResolveInternalNotificationUrl(endpointId, SUCC_EXTRA_PATH) 
                    + CreateNotifyParams(model.server_callback_url, fibonatixID));
            }
            if (!string.IsNullOrEmpty(model.order_desc))
            {
                data.Add("relatedinformation-orderdescription", model.order_desc);
            }
            return data;
        }


        //
        // PreAuth operation block
        //
        public static NameValueCollection CreateMultiCurrencyPaymentParams(
            int endpointGroupId,
            PreAuthRequestModel model,
            string fibonatixID)
        {
            NameValueCollection data = CreatePaymentParams(model, fibonatixID, endpointGroupId);
            data.Add("relatedinformation-endpointgroupid", "" + endpointGroupId);
            data.Add("hash", HashHelper.CalculateHash(PAYMENT_HASH_KEY_SEQUENCE, data,
                WebApiConfig.Settings.GetSharedSecret(endpointGroupId)));
            return data;
        }

        public static NameValueCollection CreateSingleCurrencyPaymentParams(
            int endpointId,
            PreAuthRequestModel model,
            string fibonatixID)
        {
            NameValueCollection data = CreatePaymentParams(model, fibonatixID, endpointId);
            data.Add("relatedinformation-endpointid", "" + endpointId);
            data.Add("hash", HashHelper.CalculateHash(PAYMENT_HASH_KEY_SEQUENCE, data,
                WebApiConfig.Settings.GetSharedSecret(endpointId)));
            return data;
        }

        private static NameValueCollection CreatePaymentParams(PreAuthRequestModel model, string fibonatixID, int endpointId)
        {
            DateTime now = DateTime.Now;
            NameValueCollection data = new NameValueCollection
            {
                {"clientid", WebApiConfig.Settings.GetClientID(endpointId)},
                {"payment", WebApiConfig.Settings.PaymentKey},
                {"paymentmode", "reservation" },
                {"referenceid", model.client_orderid},
                //{"orderid", model.client_orderid},
                {"creditcardowner", model.card_printed_name},
                {"firstname", model.first_name},
                {"lastname", model.last_name},
                {"street", model.address1},
                {"city", model.city},
                {"postalcode", model.zip_code},
                {"country", CountryConverter.ConvertCountryToCommDooSpace(model.country)},
                {"emailaddress", model.email},
                {"amount", CurrencyConverter.MajorAmountToMinor(model.amount, model.currency)},
                {"currency", model.currency},
                {"creditcardnumber", model.credit_card_number},
                {"expirationmonth", "" + model.expire_month},
                {"expirationyear", "" + model.expire_year},
                {"cvv", "" + model.cvv2},
                {"ipaddress", model.ipaddress},
                {"website", model.site_url},
                {"successurl", ResolveInternalUrl(SUCC_EXTRA_PATH) + CreateRedirectParams(model.redirect_url, fibonatixID)},
                {"failurl", ResolveInternalUrl(FAIL_EXTRA_PATH) + CreateRedirectParams(model.redirect_url, fibonatixID)},
                {"timestamp", now.ToString("ddMMyyyyHHmmss")},
            };
            if (model.ssn >= 0)
            {
                data.Add("idcardnumber", "" + model.ssn);
            }
            if (model.birthday >= 0)
            {
                data.Add("dateofbirth", CommDooTargetConverter.ConvertBirthdayToString(model.birthday));
            }
            if (!string.IsNullOrEmpty(model.state))
            {
                data.Add("state", model.state);
            }
            if (!string.IsNullOrEmpty(model.phone))
            {
                data.Add("phonenumber", model.phone);
            }
            else if (!string.IsNullOrEmpty(model.cell_phone))
            {
                data.Add("phonenumber", model.cell_phone);
            }
            if (!string.IsNullOrEmpty(model.server_callback_url))
            {
                data.Add("notificationurl", ResolveInternalNotificationUrl(endpointId, SUCC_EXTRA_PATH)
                    + CreateNotifyParams(model.server_callback_url, fibonatixID));
            }
            if (!string.IsNullOrEmpty(model.order_desc))
            {
                data.Add("relatedinformation-orderdescription", model.order_desc);
            }
            return data;
        }


        //
        // PreAuth-Form operation block
        //
        public static NameValueCollection CreateMultiCurrencyPaymentParams(
            int endpointGroupId,
            PreAuthFormRequestModel model,
            string fibonatixID)
        {
            NameValueCollection data = CreatePaymentParams(model, fibonatixID, endpointGroupId);
            data.Add("relatedinformation-endpointgroupid", "" + endpointGroupId);
            data.Add("hash", HashHelper.CalculateHash(PAYMENT_HASH_KEY_SEQUENCE, data,
                WebApiConfig.Settings.GetSharedSecret(endpointGroupId)));
            return data;
        }

        public static NameValueCollection CreateSingleCurrencyPaymentParams(
            int endpointId,
            PreAuthFormRequestModel model,
            string fibonatixID)
        {
            NameValueCollection data = CreatePaymentParams(model, fibonatixID, endpointId);
            data.Add("relatedinformation-endpointid", "" + endpointId);
            data.Add("hash", HashHelper.CalculateHash(PAYMENT_HASH_KEY_SEQUENCE, data,
                WebApiConfig.Settings.GetSharedSecret(endpointId)));
            return data;
        }

        private static NameValueCollection CreatePaymentParams(
            PreAuthFormRequestModel model,
            string fibonatixID,
            int endpointId)
        {
            DateTime now = DateTime.Now;
            NameValueCollection data = new NameValueCollection
            {
                {"clientid", WebApiConfig.Settings.GetClientID(endpointId)},
                {"payment", WebApiConfig.Settings.PaymentKey},
                {"paymentmode", "reservation" },
                {"referenceid", model.client_orderid},
                //{"orderid", model.client_orderid},
                {"firstname", model.first_name},
                {"lastname", model.last_name},
                {"street", model.address1},
                {"city", model.city},
                {"postalcode", model.zip_code},
                {"country", CountryConverter.ConvertCountryToCommDooSpace(model.country)},
                {"emailaddress", model.email},
                {"amount", CurrencyConverter.MajorAmountToMinor(model.amount, model.currency)},
                {"currency", model.currency},
                {"ipaddress", model.ipaddress},
                {"website", model.site_url},
                {"successurl", ResolveInternalUrl(SUCC_EXTRA_PATH) + CreateRedirectParams(model.redirect_url, fibonatixID)},
                {"failurl", ResolveInternalUrl(FAIL_EXTRA_PATH) + CreateRedirectParams(model.redirect_url, fibonatixID)},
                {"timestamp", now.ToString("ddMMyyyyHHmmss")},
            };
            if (model.ssn >= 0) {
                data.Add("idcardnumber", "" + model.ssn);
            }
            if (model.birthday >= 0) {
                data.Add("dateofbirth", CommDooTargetConverter.ConvertBirthdayToString(model.birthday));
            }
            if (!string.IsNullOrEmpty(model.state)) {
                data.Add("state", model.state);
            }
            if (!string.IsNullOrEmpty(model.phone)) {
                data.Add("phonenumber", model.phone);
            } else if (!string.IsNullOrEmpty(model.cell_phone)) {
                data.Add("phonenumber", model.cell_phone);
            }
            if (!string.IsNullOrEmpty(model.server_callback_url)) {
                data.Add("notificationurl", ResolveInternalNotificationUrl(endpointId, SUCC_EXTRA_PATH)
                    + CreateNotifyParams(model.server_callback_url, fibonatixID));
            }
            if (!string.IsNullOrEmpty(model.order_desc)) {
                data.Add("relatedinformation-orderdescription", model.order_desc);
            }
            return data;
        }


        //
        // URL helpers
        //
        public static string CreateNotifyParams(string url, string id)
        {
            return $"?customernotifyurl={HttpUtility.UrlEncode(url)}&fibonatixID={id}";
        }

        public static string CreateRedirectParams(string url, string id)
        {
            return $"?customerredirecturl={HttpUtility.UrlEncode(url)}&fibonatixID={id}";
        }

        public static string ResolveInternalUrl(string extraPath)
        {
            return
                $"{HttpContext.Current.Request.Url.Scheme}://" +
                $"{/*"localhost"*/ WebApiConfig.Settings.PublicServerName}:{HttpContext.Current.Request.Url.Port}" +
                $"{HttpContext.Current.Request.Url.AbsolutePath}" +
                $"{extraPath}";
        }

        public static string ResolveInternalNotificationUrl(int endpointId, string extraPath)
        {
            return
                $"{HttpContext.Current.Request.Url.Scheme}://" +
                $"{WebApiConfig.Settings.PublicServerName}:{HttpContext.Current.Request.Url.Port}" +
                $"/paynet/api/v2/notification/{endpointId}" +
                $"{extraPath}";
        }

        //
        // hash checking methods
        //
        public static bool SuccessHashIsValid(int endpointId, SaleSuccessPaymentModel model)
        {
            if (model == null ||
                string.IsNullOrEmpty(model.clientid) ||
                string.IsNullOrEmpty(model.transactionid) ||
                string.IsNullOrEmpty(model.referenceid) ||
                model.amount < 0 ||
                string.IsNullOrEmpty(model.currency) ||
                string.IsNullOrEmpty(model.paymentmethod) ||
                string.IsNullOrEmpty(model.transactionstatus) ||
                string.IsNullOrEmpty(model.timestamp) ||
                string.IsNullOrEmpty(model.hash))
            {
                return false;
            }

            NameValueCollection data = new NameValueCollection
            {
                {"clientid", model.clientid.ToString()},
                {"transactionid", model.transactionid},
                {"referenceid", model.referenceid},
                {"subscriptionid", string.IsNullOrEmpty(model.subscriptionid) ? string.Empty : model.subscriptionid},
                {"amount", model.amount.ToString()},
                {"currency", model.currency},
                {"paymentmethod", model.paymentmethod},
                {"customerid", string.IsNullOrEmpty(model.customerid) ? string.Empty : model.customerid},
                {"transactionstatus", model.transactionstatus},
                {"transactionstatusaddition", string.IsNullOrEmpty(model.transactionstatusaddition) ? string.Empty : model.transactionstatusaddition},
                {"creditcardtype", string.IsNullOrEmpty(model.creditcardtype) ? string.Empty : model.creditcardtype},
                {"providertransactionid", string.IsNullOrEmpty(model.providertransactionid) ? string.Empty : model.providertransactionid},
                {"additionaldata", string.IsNullOrEmpty(model.additionaldata) ? string.Empty : model.additionaldata},
                {"timestamp", model.timestamp.ToString()},
            };

            string calculatedHash = HashHelper.CalculateHash(SUCC_HASH_KEY_SEQUENCE, data, WebApiConfig.Settings.GetSharedSecret(endpointId));
            return model.hash.Trim().ToLowerInvariant() == calculatedHash.Trim().ToLowerInvariant();
        }

        public static bool SuccessHashIsValid(int endpointId, SaleFormSuccessPaymentModel model)
        {
            if (model == null ||
                string.IsNullOrEmpty(model.clientid) ||
                string.IsNullOrEmpty(model.transactionid) ||
                string.IsNullOrEmpty(model.referenceid) ||
                model.amount < 0 ||
                string.IsNullOrEmpty(model.currency) ||
                string.IsNullOrEmpty(model.paymentmethod) ||
                string.IsNullOrEmpty(model.transactionstatus) ||
                string.IsNullOrEmpty(model.timestamp) ||
                string.IsNullOrEmpty(model.hash))
            {
                return false;
            }

            NameValueCollection data = new NameValueCollection
            {
                {"clientid", model.clientid.ToString()},
                {"transactionid", model.transactionid},
                {"referenceid", model.referenceid},
                {"subscriptionid", string.IsNullOrEmpty(model.subscriptionid) ? string.Empty : model.subscriptionid},
                {"amount", model.amount.ToString()},
                {"currency", model.currency},
                {"paymentmethod", model.paymentmethod},
                {"customerid", string.IsNullOrEmpty(model.customerid) ? string.Empty : model.customerid},
                {"transactionstatus", model.transactionstatus},
                {"transactionstatusaddition", string.IsNullOrEmpty(model.transactionstatusaddition) ? string.Empty : model.transactionstatusaddition},
                {"creditcardtype", string.IsNullOrEmpty(model.creditcardtype) ? string.Empty : model.creditcardtype},
                {"providertransactionid", string.IsNullOrEmpty(model.providertransactionid) ? string.Empty : model.providertransactionid},
                {"additionaldata", string.IsNullOrEmpty(model.additionaldata) ? string.Empty : model.additionaldata},
                {"timestamp", model.timestamp.ToString()},
            };

            string calculatedHash = HashHelper.CalculateHash(SUCC_HASH_KEY_SEQUENCE, data, WebApiConfig.Settings.GetSharedSecret(endpointId));
            return model.hash.Trim().ToLowerInvariant() == calculatedHash.Trim().ToLowerInvariant();
        }

        public static bool FailureHashIsValid(int endpointId, SaleFailurePaymentModel model) {
            if (model == null ||
                string.IsNullOrEmpty(model.clientid) ||
                string.IsNullOrEmpty(model.referenceid) ||
                string.IsNullOrEmpty(model.errornumber) ||
                string.IsNullOrEmpty(model.errortext) ||
                string.IsNullOrEmpty(model.timestamp) ||
                string.IsNullOrEmpty(model.hash))
            {
                return false;
            }

            NameValueCollection data = new NameValueCollection
            {
                {"clientid", model.clientid.ToString()},
                {"referenceid", model.referenceid},
                {"errornumber", model.errornumber},
                {"errortext", model.errortext},
                {"additionaldata", string.IsNullOrEmpty(model.additionaldata) ? string.Empty : model.additionaldata},
                {"timestamp", model.timestamp.ToString()},
            };

            string calculatedHash = HashHelper.CalculateHash(FAIL_HASH_KEY_SEQUENCE, data, WebApiConfig.Settings.GetSharedSecret(endpointId));
            return model.hash.Trim().ToLowerInvariant() == calculatedHash.Trim().ToLowerInvariant();
        }

        public static bool FailureHashIsValid(int endpointId, SaleFormFailurePaymentModel model) {
            if (model == null ||
                string.IsNullOrEmpty(model.clientid) ||
                string.IsNullOrEmpty(model.referenceid) ||
                string.IsNullOrEmpty(model.errornumber) ||
                string.IsNullOrEmpty(model.errortext) ||
                string.IsNullOrEmpty(model.timestamp) ||
                string.IsNullOrEmpty(model.hash))
            {
                return false;
            }

            NameValueCollection data = new NameValueCollection
            {
                {"clientid", model.clientid.ToString()},
                {"referenceid", model.referenceid},
                {"errornumber", model.errornumber},
                {"errortext", model.errortext},
                {"additionaldata", string.IsNullOrEmpty(model.additionaldata) ? string.Empty : model.additionaldata},
                {"timestamp", model.timestamp.ToString()},
            };

            string calculatedHash = HashHelper.CalculateHash(FAIL_HASH_KEY_SEQUENCE, data, WebApiConfig.Settings.GetSharedSecret(endpointId));
            return model.hash.Trim().ToLowerInvariant() == calculatedHash.Trim().ToLowerInvariant();
        }


        public static bool SuccessHashIsValid(int endpointId, PreAuthSuccessPaymentModel model) {
            if (model == null ||
                string.IsNullOrEmpty(model.clientid) ||
                string.IsNullOrEmpty(model.transactionid) ||
                string.IsNullOrEmpty(model.referenceid) ||
                model.amount < 0 ||
                string.IsNullOrEmpty(model.currency) ||
                string.IsNullOrEmpty(model.paymentmethod) ||
                string.IsNullOrEmpty(model.transactionstatus) ||
                string.IsNullOrEmpty(model.timestamp) ||
                string.IsNullOrEmpty(model.hash)) {
                return false;
            }

            NameValueCollection data = new NameValueCollection
            {
                {"clientid", model.clientid.ToString()},
                {"transactionid", model.transactionid},
                {"referenceid", model.referenceid},
                {"subscriptionid", string.IsNullOrEmpty(model.subscriptionid) ? string.Empty : model.subscriptionid},
                {"amount", model.amount.ToString()},
                {"currency", model.currency},
                {"paymentmethod", model.paymentmethod},
                {"customerid", string.IsNullOrEmpty(model.customerid) ? string.Empty : model.customerid},
                {"transactionstatus", model.transactionstatus},
                {"transactionstatusaddition", string.IsNullOrEmpty(model.transactionstatusaddition) ? string.Empty : model.transactionstatusaddition},
                {"creditcardtype", string.IsNullOrEmpty(model.creditcardtype) ? string.Empty : model.creditcardtype},
                {"providertransactionid", string.IsNullOrEmpty(model.providertransactionid) ? string.Empty : model.providertransactionid},
                {"additionaldata", string.IsNullOrEmpty(model.additionaldata) ? string.Empty : model.additionaldata},
                {"timestamp", model.timestamp.ToString()},
            };

            string calculatedHash = HashHelper.CalculateHash(SUCC_HASH_KEY_SEQUENCE, data, WebApiConfig.Settings.GetSharedSecret(endpointId));
            return model.hash.Trim().ToLowerInvariant() == calculatedHash.Trim().ToLowerInvariant();
        }

        public static bool SuccessHashIsValid(int endpointId, PreAuthFormSuccessPaymentModel model) {
            if (model == null ||
                string.IsNullOrEmpty(model.clientid) ||
                string.IsNullOrEmpty(model.transactionid) ||
                string.IsNullOrEmpty(model.referenceid) ||
                model.amount < 0 ||
                string.IsNullOrEmpty(model.currency) ||
                string.IsNullOrEmpty(model.paymentmethod) ||
                string.IsNullOrEmpty(model.transactionstatus) ||
                string.IsNullOrEmpty(model.timestamp) ||
                string.IsNullOrEmpty(model.hash)) {
                return false;
            }

            NameValueCollection data = new NameValueCollection
            {
                {"clientid", model.clientid.ToString()},
                {"transactionid", model.transactionid},
                {"referenceid", model.referenceid},
                {"subscriptionid", string.IsNullOrEmpty(model.subscriptionid) ? string.Empty : model.subscriptionid},
                {"amount", model.amount.ToString()},
                {"currency", model.currency},
                {"paymentmethod", model.paymentmethod},
                {"customerid", string.IsNullOrEmpty(model.customerid) ? string.Empty : model.customerid},
                {"transactionstatus", model.transactionstatus},
                {"transactionstatusaddition", string.IsNullOrEmpty(model.transactionstatusaddition) ? string.Empty : model.transactionstatusaddition},
                {"creditcardtype", string.IsNullOrEmpty(model.creditcardtype) ? string.Empty : model.creditcardtype},
                {"providertransactionid", string.IsNullOrEmpty(model.providertransactionid) ? string.Empty : model.providertransactionid},
                {"additionaldata", string.IsNullOrEmpty(model.additionaldata) ? string.Empty : model.additionaldata},
                {"timestamp", model.timestamp.ToString()},
            };

            string calculatedHash = HashHelper.CalculateHash(SUCC_HASH_KEY_SEQUENCE, data, WebApiConfig.Settings.GetSharedSecret(endpointId));
            return model.hash.Trim().ToLowerInvariant() == calculatedHash.Trim().ToLowerInvariant();
        }

        public static bool FailureHashIsValid(int endpointId, PreAuthFailurePaymentModel model) {
            if (model == null ||
                string.IsNullOrEmpty(model.clientid) ||
                string.IsNullOrEmpty(model.referenceid) ||
                string.IsNullOrEmpty(model.errornumber) ||
                string.IsNullOrEmpty(model.errortext) ||
                string.IsNullOrEmpty(model.timestamp) ||
                string.IsNullOrEmpty(model.hash)) {
                return false;
            }

            NameValueCollection data = new NameValueCollection
            {
                {"clientid", model.clientid.ToString()},
                {"referenceid", model.referenceid},
                {"errornumber", model.errornumber},
                {"errortext", model.errortext},
                {"additionaldata", string.IsNullOrEmpty(model.additionaldata) ? string.Empty : model.additionaldata},
                {"timestamp", model.timestamp.ToString()},
            };

            string calculatedHash = HashHelper.CalculateHash(FAIL_HASH_KEY_SEQUENCE, data, WebApiConfig.Settings.GetSharedSecret(endpointId));
            return model.hash.Trim().ToLowerInvariant() == calculatedHash.Trim().ToLowerInvariant();
        }

        public static bool FailureHashIsValid(int endpointId, PreAuthFormFailurePaymentModel model) {
            if (model == null ||
                string.IsNullOrEmpty(model.clientid) ||
                string.IsNullOrEmpty(model.referenceid) ||
                string.IsNullOrEmpty(model.errornumber) ||
                string.IsNullOrEmpty(model.errortext) ||
                string.IsNullOrEmpty(model.timestamp) ||
                string.IsNullOrEmpty(model.hash)) {
                return false;
            }

            NameValueCollection data = new NameValueCollection
            {
                {"clientid", model.clientid.ToString()},
                {"referenceid", model.referenceid},
                {"errornumber", model.errornumber},
                {"errortext", model.errortext},
                {"additionaldata", string.IsNullOrEmpty(model.additionaldata) ? string.Empty : model.additionaldata},
                {"timestamp", model.timestamp.ToString()},
            };

            string calculatedHash = HashHelper.CalculateHash(FAIL_HASH_KEY_SEQUENCE, data, WebApiConfig.Settings.GetSharedSecret(endpointId));
            return model.hash.Trim().ToLowerInvariant() == calculatedHash.Trim().ToLowerInvariant();
        }



        private static string[] PAYMENT_HASH_KEY_SEQUENCE =
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

        private static string[] FAIL_HASH_KEY_SEQUENCE =
        {
            "clientid",
            "referenceid",
            "errornumber",
            "errortext",
            "additionaldata",
            "timestamp"
        };

        private static string[] SUCC_HASH_KEY_SEQUENCE =
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