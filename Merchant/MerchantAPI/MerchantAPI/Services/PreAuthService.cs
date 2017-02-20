using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using MerchantAPI.Models;
using System.Collections.Specialized;
using MerchantAPI.Data;
using MerchantAPI.Controllers.Factories;
using System.Text;

namespace MerchantAPI.Services
{
    public class PreAuthService
    {

        public ServiceTransitionResult PreAuthSingleCurrency(
            int endpointId, 
            PreAuthRequestModel model)
        {
            byte[] partnerResponse = new byte[0];
            NameValueCollection requestParameters;
            try {
                Transaction transactionData = TransactionsDataStorage.CreateNewTransaction(TransactionType.Preauth, model.client_orderid);

                requestParameters = CommDooFrontendFactory.CreateMultyCurrencyPaymentParams(endpointId, model, transactionData.TransactionId);

                var parameters = new StringBuilder();
                foreach (string key in requestParameters.Keys) {
                    parameters.AppendFormat("{0}={1}&",
                        HttpUtility.UrlEncode(key),
                        HttpUtility.UrlEncode(requestParameters[key]));
                }
                if (requestParameters.Count > 0)
                    parameters.Length -= 1;

                string redirectToCommDoo = WebApiConfig.Settings.PaymentASPXEndpoint + "?" + parameters.ToString();

                // Add to cache with key requestParameters['client_orderid'] and data redirectToCommDoo
                TransactionsDataStorage.UpdateTransactionState(transactionData.TransactionId, TransactionState.Started);
                TransactionsDataStorage.UpdateTransactionStatus(transactionData.TransactionId, TransactionStatus.Undefined);
                Cache.setRedirectUrlForRequest(transactionData.TransactionId, redirectToCommDoo);
                Cache.setPreAuthRequestData(transactionData.TransactionId, model);

                string response = "type=async-response" +
                                  "&serial-number=" + transactionData.SerialNumber +
                                  "&merchant-order-id=" + model.client_orderid +
                                  "&paynet-order-id=" + transactionData.TransactionId;

                partnerResponse = Encoding.UTF8.GetBytes(response);

            } catch (Exception e) {
                return new ServiceTransitionResult(HttpStatusCode.InternalServerError,
                    "CONNECTION ERROR: " + e.Message + "\n");
            } finally { }

            string strResponse = Encoding.UTF8.GetString(partnerResponse);
            //            int begin = strResponse.IndexOf(ESCAPE);
            //            if (begin >= 0)
            //            {
            //                begin += ESCAPE.Length;
            //                int end = strResponse.IndexOf("')");
            //                string redirectUrl = strResponse.Substring(begin, end - begin);
            //                partnerResponse = client.UploadValues(new Uri(redirectUrl), "GET", new NameValueCollection());
            //                strResponse = u8.GetString(partnerResponse);
            //            }

            return new ServiceTransitionResult(HttpStatusCode.OK,
                strResponse + "\n");
            //            SaleResponseModel succ = new SaleResponseModel();
            //            succ.SetSucc();
            //            return succ;
        }
    }
}