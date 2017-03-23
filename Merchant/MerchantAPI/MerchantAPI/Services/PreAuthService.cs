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
using MerchantAPI.Helpers;

namespace MerchantAPI.Services
{
    public class PreAuthService
    {

        public ServiceTransitionResult PreAuthSingleCurrency(
            int endpointId, 
            PreAuthRequestModel model,
            string rawModel)
        {
            Transaction transactionData = new Transaction(TransactionType.Preauth, model.client_orderid);
            try
            {
                NameValueCollection requestParameters = CommDooFrontendFactory.CreateMultyCurrencyPaymentParams(
                    endpointId, model, transactionData.TransactionId);

                var parameters = new StringBuilder(256)
                    .Append(WebApiConfig.Settings.PaymentASPXEndpoint);
                char delim = '?';
                foreach (string key in requestParameters.Keys)
                {
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
                //transactionData.RedirectUri = parameters.ToString(); // don't save the url in DB, as it may contain cvv and credit card number
                transactionData.ReferenceQuery = ControllerHelper.SerializeHttpParameters(referenceQuery);

                TransactionsDataStorage.Store(transactionData);

                // abv: cache version
                // Add to cache with key requestParameters['client_orderid'] and data redirectToCommDoo
                //TransactionsDataStorage.UpdateTransaction(transactionData.TransactionId, 
                //    TransactionState.Started, TransactionStatus.Undefined);
                Cache.setRedirectUrlForRequest(transactionData.TransactionId, parameters.ToString());
                //Cache.setPreAuthRequestData(transactionData.TransactionId, model);

                string response = "type=async-response" +
                                  "&serial-number=" + transactionData.SerialNumber +
                                  "&merchant-order-id=" + model.client_orderid +
                                  "&paynet-order-id=" + transactionData.TransactionId;

                return new ServiceTransitionResult(HttpStatusCode.OK,
                    response + "\n");
            }
            catch (Exception e)
            {
                TransactionsDataStorage.Store(transactionData, e);

                return new ServiceTransitionResult(HttpStatusCode.InternalServerError,
                    $"EXCP: Processing PreAuth for [client_orderid={transactionData.TransactionId}] failed\n");
            }
            finally { }
        }
    }
}