using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using MerchantAPI.Connectors;
using MerchantAPI.Data;
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
                newStatus = TransactionStatus.Approved;
            else
                newStatus = TransactionStatus.Declined;

            if (string.IsNullOrEmpty(model.fibonatixID)) {
                TransactionsDataStorage.UpdateTransaction(model.fibonatixID, model.transactionid, TransactionState.Finished);                
                TransactionsDataStorage.UpdateTransactionStatus(model.fibonatixID, newStatus);
            }

            if (string.IsNullOrEmpty(model.customernotifyurl)) {
                // TODO - send Notification to Customer
                NotifyMerchant(endpointId, model.customernotifyurl, newStatus, model.referenceid, model.transactionid);
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

        private void NotifyMerchant(int endpointId, string merchantServerCallbackUrl, TransactionStatus newStatus, string transactionId, string processingTransactionId)
        {
            MerchantCallback callbackEntity = MerchantCallbackStorage.FindByTransactionId(transactionId);
            // check if callback already processssed
            if (callbackEntity != null) return;

            string controlKey = WebApiConfig.Settings.MerchantControlKeys["" + endpointId];
            string[] splittedTargetUrl = merchantServerCallbackUrl.Split('?');
            CallbackRequestModel callback = new CallbackRequestModel(controlKey, splittedTargetUrl.Length == 1 ? null : splittedTargetUrl[1]);

            using (var db = new PersistenceContext())
            {
                callbackEntity = new MerchantCallback(transactionId, splittedTargetUrl[0], callback.ToHttpParameters());
                if (string.IsNullOrEmpty(controlKey))
                {
                    callbackEntity.State = CallbackState.Failed;
                    callbackEntity.StateReason = $"Unknown 'Merchant Control Key' for endpointId [{endpointId}]";
                }

                db.MerchantCallbacks.Add(callbackEntity);
                db.SaveChanges();
            }

            WebResponse webResponse = null;
            try
            {
                var webRequest = WebRequest.Create(callbackEntity.CallbackUri);
                webRequest.Method = WebRequestMethods.Http.Get;
                byte[] data = System.Text.Encoding.UTF8.GetBytes(callbackEntity.CallbackQuery);
                webRequest.ContentLength = data.Length;

                var httpWebRequest = webRequest as HttpWebRequest;
                if (httpWebRequest != null)
                {
                    httpWebRequest.UserAgent = $"Fibonatix.CommDoo.WebGate {this.GetType().Assembly.GetName().Version}";
                    httpWebRequest.KeepAlive = false;
                }

                using (var requestStream = webRequest.GetRequestStream())
                {
                    requestStream.Write(data, 0, data.Length);
                }

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
                    "EXCP occurs: " + e.GetType());
            }
            finally
            {
                webResponse?.Close();
            }
        }
    }
}