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
using MerchantAPI.Services.Exceptions;

namespace MerchantAPI.Services
{
    public class NotificationService
    {
        public ServiceTransitionResult Notified(
            int endpointId, 
            NotificationRequestModel model)
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
                try
                {
                    NotifyMerchant(endpointId, model.customernotifyurl, newStatus, model.fibonatixID,
                        model.transactionid,
                        model.transactionstatus, model.clientid, model.amount, model.currency, string.Empty,
                        string.Empty);
                }
                catch (TransactionNotFoundException e)
                {
                    return new ServiceTransitionResult(HttpStatusCode.BadRequest, e.Message);
                }
                /*
                string response = 
                            "status=" + ( newStatus == TransactionStatus.Approved ? "accepted" : "declined " ) + "\n" +
                            "&merchant-order-id=" + model.referenceid + "\n" +
                            "&paynet-order-id=" + model.transactionid + "\n";

                WebResponse webResponse = null;
                try {
                    var webRequest = WebRequest.Create(model.customernotifyurl);
                    webRequest.Method = "POST";
                    webRequest.ContentType = "application/x-www-form-urlencoded";
                    // If required by the server, set the credentials.  
                    //webRequest.Credentials = CredentialCache.DefaultCredentials;

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
            if (transaction == null)
            {
                throw new TransactionNotFoundException($"ERROR: Unknown 'transactionId'[{transactionId}] to process NotifyMerchant()\n");
            }
            NameValueCollection originalRequest = ControllerHelper.DeserializeHttpParameters(transaction.ReferenceQuery);

            string controlKey = WebApiConfig.Settings.GetMerchantControlKey(endpointId);
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
                name = originalRequest["first_name"] + ' ' + originalRequest["last_name"],
                email = originalRequest["email"],
                approval_code = "",
                last_four_digits = int.Parse(ControllerHelper.LastFourDigits(originalRequest["credit_card_number"])),
                bin = "",
                card_type = ResolveCardType(originalRequest["credit_card_number"]), // cos 'credit_card_number' was eliminated earlier!
                gate_partial_reversal = "disabled",
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
                ((HttpWebRequest)webRequest).UserAgent = $"Fibonatix.CommDoo.WebGate {this.GetType().Assembly.GetName().Version}";
                ((HttpWebRequest)webRequest).KeepAlive = false;

                webResponse = webRequest.GetResponse();
                if (((HttpWebResponse)webResponse).StatusCode == HttpStatusCode.OK)
                {
                    MerchantCallbackStorage.UpdateMerchantCallback(callbackEntity, CallbackState.Delivered, null);
                }
                else
                {
                    MerchantCallbackStorage.UpdateMerchantCallback(callbackEntity, CallbackState.Repeating,
                        $"Repeating due to HTTP status [{((HttpWebResponse)webResponse).StatusCode}] of request to Merchant site");
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

        private string ResolveCardType(string cardNumber)
        {
            return Helpers.CommDooTargetConverter.getCardType(cardNumber);
        }
    }
}