using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using MerchantAPI.Connectors;
using MerchantAPI.Data;
using MerchantAPI.Helpers;
using MerchantAPI.Models;

namespace MerchantAPI.Services
{
    public class NotificationService
    {
        public ServiceTransitionResult Notified(int endpointId, NotificationRequestModel model)
        {
            TransactionStatus newStatus = TransactionStatus.Undefined;
            if (model.transactionstatus == "Charged" ||
                model.transactionstatus == "Reserved" ||
                model.transactionstatus == "ChargedBack" ||
                model.transactionstatus == "ChargedBackReserved" ||
                model.transactionstatus == "Refunded")
            {
                newStatus = TransactionStatus.Approved;
            }
            else
            {
                newStatus = TransactionStatus.Declined;
            }

            if (!string.IsNullOrEmpty(model.fibonatixID))
            {
                TransactionsDataStorage.UpdateTransaction(model.fibonatixID, model.transactionid, TransactionState.Finished, newStatus);
            }

            if (!string.IsNullOrEmpty(model.customernotifyurl))
            {
                NotifyMerchant(endpointId, model.customernotifyurl, newStatus, model.referenceid, model.transactionid,
                    model.transactionstatus, model.clientid, model.amount, model.currency, string.Empty, string.Empty);
                /*
                string response = 
                            "status=" + ( newStatus == TransactionStatus.Approved ? "accepted" : "declined " ) + "\n" +
                            "&merchant-order-id=" + model.referenceid + "\n" +
                            "&paynet-order-id=" + model.transactionid + "\n";

                WebResponse webResponse = null;
                try {
                    var webRequest = WebRequest.Create(model.customernotifyurl);
                    webRequest.Method = "POST";     // WebRequestMethods.Http.Get;
                    webRequest.ContentType = "application/x-www-form-urlencoded";

                    byte[] data = System.Text.Encoding.UTF8.GetBytes(response) ;
                    webRequest.ContentLength = data.Length;

                    var httpWebRequest = webRequest as HttpWebRequest;
                    if (httpWebRequest != null) {
                        httpWebRequest.UserAgent = $"Fibonatix.CommDoo.WebGate {this.GetType().Assembly.GetName().Version}";
                        httpWebRequest.KeepAlive = false;
                    }

                    using (var requestStream = webRequest.GetRequestStream()) {
                        requestStream.Write(data, 0, data.Length);
                    }

                    webResponse = webRequest.GetResponse();
                } catch {
                } finally {
                    if (webResponse != null) {
                        webResponse.Close();
                    }
                }
                */
            }

            return new ServiceTransitionResult(HttpStatusCode.OK, "Notification received");
        }

        private void NotifyMerchant(
            int endpointId, 
            string merchantServerCallbackUrl, 
            TransactionStatus transactionStatus, 
            string transactionId, 
            string processingTransactionId, 
            string processingStatus, 
            string clientOrderId, 
            int amountInMinor, 
            string currency, 
            string errorCode, 
            string errorMessage)
        {
            MerchantCallback callbackEntity = MerchantCallbackStorage.FindByTransactionId(transactionId);
            // check if callback already processed
            if (callbackEntity != null) return;

            Transaction transaction = TransactionsDataStorage.FindByTransactionId(transactionId);
            NameValueCollection originalRequest = ControllerHelper.DeserializeHttpParameters(transaction.ReferenceQuery);

            string controlKey = WebApiConfig.Settings.MerchantControlKeys["" + endpointId];
            string[] splittedTargetUrl = merchantServerCallbackUrl.Split('?');

            CallbackRequestModel callback = new CallbackRequestModel(controlKey, 
                splittedTargetUrl.Length == 1 ? null : splittedTargetUrl[1])
            {
                status = transactionStatus == TransactionStatus.Approved ? "approved" : "declined",
                client_orderid = clientOrderId,
                orderid = transactionId,
                type = transaction.Type.ToString().ToLower(),
                amount = CurrencyConverter.MinorAmountToMajor(amountInMinor, currency).ToString().Replace(',', '.'),
                descriptor = "CommDoo Processing Platform",
                error_code = errorCode,
                error_message = errorMessage,
                name = originalRequest["first_name"] + ' ' + originalRequest["first_name"],
                email = originalRequest["email"],
                approval_code = "",
                last_four_digits = int.Parse(ControllerHelper.LastFourDigits(originalRequest["credit_card_number"])),
                bin = "",
                card_type = ResolveCardType(originalRequest["credit_card_number"]),
                gate_partial_reversal = "disable",
                gate_partial_capture = "disabled",
                reason_code = "",
                processor_rrn = "",
                comment = "",
                rapida_balance = "",
                merchantdata = "",
            };

            callbackEntity = new MerchantCallback(transactionId, splittedTargetUrl[0], callback.ToHttpParameters());
            if (string.IsNullOrEmpty(controlKey))
            {
                callbackEntity.State = CallbackState.Failed;
                callbackEntity.StateReason = $"Unknown 'Merchant Control Key' for endpointId [{endpointId}]";
                MerchantCallbackStorage.Store(callbackEntity);
                return;
            }
            MerchantCallbackStorage.Store(callbackEntity);

            // GET HTTP was helped by https://msdn.microsoft.com/en-us/library/456dfw4f(v=vs.110).aspx
            WebResponse webResponse = null;
            try
            {
                var webRequest = WebRequest.Create(callbackEntity.CallbackUri + '?' + callbackEntity.CallbackQuery);
                //webRequest.Method = WebRequestMethods.Http.Get;
                //byte[] data = System.Text.Encoding.UTF8.GetBytes(callbackEntity.CallbackQuery);
                //webRequest.ContentLength = data.Length;
                // If required by the server, set the credentials.  
                //webRequest.Credentials = CredentialCache.DefaultCredentials;

                var httpWebRequest = webRequest as HttpWebRequest;
                if (httpWebRequest != null)
                {
                    httpWebRequest.UserAgent = $"Fibonatix.CommDoo.WebGate {this.GetType().Assembly.GetName().Version}";
                    httpWebRequest.KeepAlive = false;
                }

                //using (var requestStream = webRequest.GetRequestStream())
                //{
                    //requestStream.Write(data, 0, data.Length);
                    //requestStream.Flush();
                //}

                webResponse = webRequest.GetResponse();
                var httpWebResponse = webResponse as HttpWebResponse;
                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    MerchantCallbackStorage.UpdateMerchantCallback(callbackEntity, CallbackState.Delivered, null);
                }
                else
                {
                    MerchantCallbackStorage.UpdateMerchantCallback(callbackEntity, CallbackState.Repeating, 
                        $"Repeating due to HTTP status [{httpWebResponse.StatusCode}] of request to Merchant site");
                }
            }
            catch (Exception e)
            {
                MerchantCallbackStorage.UpdateMerchantCallback(callbackEntity, CallbackState.Failed, 
                    $"EXCP: [{e.GetType()}] {e.Message}\n");
            }
            finally
            {
                webResponse?.Close();
            }
        }

        private static string[] AmericanExpress = { "American Express", "34", "37" };
        private static string[] ChinaUnionPay = { "China UnionPay", "62", "88" };
        private static string[] DinersClub = { "Diners Club", "300", "301", "302", "303", "304", "305", "309", "36", "38", "39", "54", "55" };
        private static string[] DiscoverCard = { "Discover Card", "6011", "622126..622925", "644", "645", "646", "647", "648", "649", "65" };
        private static string[] JCB = { "JCB", "3528..3589" };
        private static string[] Laser = { "Laser", "6304", "6706", "6771", "6709" };
        private static string[] Maestro = { "Maestro", "5018", "5020", "5038", "5612", "5893", "6304", "6759", "6761", "6762", "6763", "0604", "6390" };
        private static string[] Dankort = { "Dankort", "5019" };
        private static string[] Visa = { "Visa", "4" };
        private static string[] MasterCard = { "MasterCard", "50", "51", "52", "53", "54", "55" };
        private static string[] VisaElectron = { "Visa Electron", "4026", "417500", "4405", "4508", "4844", "4913", "4917" };

        private string ResolveCardType(string cardNumber)
        {
            foreach (var prefix in AmericanExpress)
                if (cardNumber.StartsWith(prefix)) return AmericanExpress[0].ToUpper();
            foreach (var prefix in ChinaUnionPay)
                if (cardNumber.StartsWith(prefix)) return ChinaUnionPay[0].ToUpper();
            foreach (var prefix in DinersClub)
                if (cardNumber.StartsWith(prefix)) return DinersClub[0].ToUpper();

            foreach (var prefix in DiscoverCard)
                if (cardNumber.StartsWith(prefix)) return DiscoverCard[0].ToUpper();
            int iprefix = int.Parse(cardNumber.Substring(0, 6));
            if (iprefix >= 622126 && iprefix <= 622925) return DiscoverCard[0].ToUpper();

            foreach (var prefix in JCB)
                if (cardNumber.StartsWith(prefix)) return JCB[0].ToUpper();
            iprefix = int.Parse(cardNumber.Substring(0, 4));
            if (iprefix >= 3528 && iprefix <= 3589) return JCB[0].ToUpper();

            foreach (var prefix in Laser)
                if (cardNumber.StartsWith(prefix)) return Laser[0].ToUpper();
            foreach (var prefix in Maestro)
                if (cardNumber.StartsWith(prefix)) return Maestro[0].ToUpper();
            foreach (var prefix in Dankort)
                if (cardNumber.StartsWith(prefix)) return Dankort[0].ToUpper();
            foreach (var prefix in Visa)
                if (cardNumber.StartsWith(prefix)) return Visa[0].ToUpper();
            foreach (var prefix in MasterCard)
                if (cardNumber.StartsWith(prefix)) return MasterCard[0].ToUpper();
            foreach (var prefix in VisaElectron)
                if (cardNumber.StartsWith(prefix)) return VisaElectron[0].ToUpper();

            return "UNKNOWN";
        }
    }
}