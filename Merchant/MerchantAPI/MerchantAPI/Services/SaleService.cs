using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using MerchantAPI.Connectors;
using MerchantAPI.Controllers.Factories;
using MerchantAPI.Data;
using MerchantAPI.Models;

namespace MerchantAPI.Services
{
    public class SaleService
    {
        public const string ESCAPE = "(escape('";

        public ServiceTransitionResult SaleSingleCurrency(int endpointId, SaleRequestModel model)
        {
            byte[] partnerResponse = new byte[0];
            NameValueCollection requestParameters;
            try
            {
                Transaction transactionData = TransactionsDataStorage.CreateNewTransaction(TransactionType.Sale, model.client_orderid);

                requestParameters = CommDooFrontendFactory.CreateMultyCurrencyPaymentParams(endpointId, model, transactionData.TransactionId);

                var parameters = new StringBuilder();
                foreach (string key in requestParameters.Keys) {
                    parameters
                        .Append(parameters.Length > 0 ? "&" : string.Empty)
                        .Append(HttpUtility.UrlEncode(key))
                        .Append('=')
                        .Append(HttpUtility.UrlEncode(requestParameters[key]));
                }
                
                string redirectToCommDoo = WebApiConfig.Settings.PaymentASPXEndpoint + "?" + parameters;

                // Add to cache with key requestParameters['client_orderid'] and data redirectToCommDoo
                TransactionsDataStorage.UpdateTransaction(transactionData.TransactionId, 
                    TransactionState.Started, TransactionStatus.Undefined);
                Cache.setRedirectUrlForRequest(transactionData.TransactionId, redirectToCommDoo);
                Cache.setSaleRequestData(transactionData.TransactionId, model);

                string response = "type=async-response" +
                                  "&serial-number=" + transactionData.SerialNumber +
                                  "&merchant-order-id=" + model.client_orderid + 
                                  "&paynet-order-id=" + transactionData.TransactionId;

                partnerResponse = Encoding.UTF8.GetBytes(response);

            } catch (Exception e)
            {
                return new ServiceTransitionResult(HttpStatusCode.InternalServerError,
                    "CONNECTION ERROR: " + e.Message + "\n" );
            }
            finally { }

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
                strResponse + "\n" );
            //            SaleResponseModel succ = new SaleResponseModel();
            //            succ.SetSucc();
            //            return succ;
        }
    }

}