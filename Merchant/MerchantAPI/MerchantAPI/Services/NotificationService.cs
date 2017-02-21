using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using MerchantAPI.Data;
using MerchantAPI.Models;

namespace MerchantAPI.Services
{
    public class NotificationService
    {

        public ServiceTransitionResult Notified(
            NotificationRequestModel model)
        {
            // TODO - сделать проверку hash в полученной нотификации - это обязательно !!!!
            // отрабатывать только те нотификации которые проходят валидацию

            TransactionStatus newStatus = TransactionStatus.Undefined;
            if (model.transactionstatus == "Charged" ||
                model.transactionstatus == "Reserved" ||
                model.transactionstatus == "ChargedBack" ||
                model.transactionstatus == "ChargedBackReserved" ||
                model.transactionstatus == "Refunded")
                newStatus = TransactionStatus.Approved;
            else
                newStatus = TransactionStatus.Declined;

            if (model.fibonatixID != null) {
                TransactionsDataStorage.UpdateTransaction(model.fibonatixID, model.transactionid, TransactionState.Finished);                
                TransactionsDataStorage.UpdateTransactionStatus(model.fibonatixID, newStatus);
            }

            if (model.customernotifyurl != null) {
                // TODO - send Notification to Customer
                string response = 
                            "status=" + ( newStatus == TransactionStatus.Approved ? "accepted" : "declined " ) + "\n" +
                            "&merchant-order-id=" + model.referenceid + "\n" +
                            "&paynet-order-id=" + model.transactionid + "\n";

                WebResponse webResponse = null;
                try {
                    var webRequest = WebRequest.Create(model.customernotifyurl);
                    webRequest.Method = "POST";
                    webRequest.ContentType = "application/x-www-form-urlencoded";

                    byte[] data = System.Text.Encoding.UTF8.GetBytes(response) ;
                    webRequest.ContentLength = data.Length;

                    var httpWebRequest = webRequest as HttpWebRequest;
                    if (httpWebRequest != null) {
                        httpWebRequest.UserAgent = String.Format("Fibonatix.CommDoo.WebGate {0}", this.GetType().Assembly.GetName().Version.ToString());
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
            }

            return new ServiceTransitionResult(HttpStatusCode.OK, "Notification received");
        }

    }
}