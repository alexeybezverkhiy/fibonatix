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
using MerchantAPI.Helpers;
using MerchantAPI.Models;

namespace MerchantAPI.Services
{
    public class SaleService
    {
        public const string ESCAPE = "(escape('";

        public ServiceTransitionResult SaleSingleCurrency(int endpointId, SaleRequestModel model, string rawModel)
        {
            Transaction transactionData = new Transaction(TransactionType.Sale, model.client_orderid);
            try
            {
                NameValueCollection requestParameters = CommDooFrontendFactory.CreateMultyCurrencyPaymentParams(
                    endpointId, model, transactionData.TransactionId);

                var parameters = new StringBuilder(256)
                    .Append(WebApiConfig.Settings.PaymentASPXEndpoint);
                char delim = '?';
                foreach (string key in requestParameters.Keys) {
                    parameters
                        .Append(delim)
                        .Append(key)
                        .Append('=')
                        .Append(HttpUtility.UrlEncode(requestParameters[key]));
                    delim = '&';
                }

                NameValueCollection referenceQuery = ControllerHelper.DeserializeHttpParameters(rawModel);
                ControllerHelper.EliminateCardData(referenceQuery);

                transactionData.State = TransactionState.Started;
                transactionData.Status = TransactionStatus.Undefined;
                transactionData.RedirectUri = parameters.ToString();
                transactionData.ReferenceQuery = ControllerHelper.SerializeHttpParameters(referenceQuery);

                TransactionsDataStorage.Store(transactionData);

                // abv: cache version
                // Add to cache with key requestParameters['client_orderid'] and data redirectToCommDoo
                //TransactionsDataStorage.UpdateTransaction(transactionData.TransactionId, 
                //    TransactionState.Started, TransactionStatus.Undefined);
                //Cache.setRedirectUrlForRequest(transactionData.TransactionId, redirectToCommDoo);
                //Cache.setSaleRequestData(transactionData.TransactionId, model);

                string response = "type=async-response" + "\n" +
                                  "&serial-number=" + transactionData.SerialNumber + "\n" +
                                  "&merchant-order-id=" + model.client_orderid + "\n" +
                                  "&paynet-order-id=" + transactionData.TransactionId + "\n";

                return new ServiceTransitionResult(HttpStatusCode.OK,
                    response + "\n");
            }
            catch (Exception e)
            {
                TransactionsDataStorage.Store(transactionData, e);

                return new ServiceTransitionResult(HttpStatusCode.InternalServerError,
                    $"EXCP: Processing Sale for [client_orderid={transactionData.TransactionId}] failed\n");
            }
            finally { }
        }
    }

}