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
    public class PreAuthFormService
    {
        public ServiceTransitionResult PreAuthFormSingleCurrency(
            int endpointId, 
            PreAuthFormRequestModel model, 
            string rawModel)
        {
            Transaction transactionData = new Transaction(TransactionType.PreAuthForm, model.client_orderid);
            try
            {
                NameValueCollection requestParameters = CommDooFrontendFactory.CreateSingleCurrencyPaymentParams(
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
                //transactionData.RedirectUri = parameters.ToString(); // don't save the url in DB, as it may contain cvv and credit card number
                transactionData.ReferenceQuery = ControllerHelper.SerializeHttpParameters(referenceQuery);
                transactionData.RedirectUri = parameters.ToString();

                TransactionsDataStorage.Store(transactionData);

                // abv: cache version
                // Add to cache with key requestParameters['client_orderid'] and data redirectToCommDoo
                //TransactionsDataStorage.UpdateTransaction(transactionData.TransactionId, 
                //    TransactionState.Started, TransactionStatus.Undefined);
                Cache.setRedirectUrlForRequest(transactionData.TransactionId, parameters.ToString());
                //Cache.setSaleRequestData(transactionData.TransactionId, model);

                string response = "type=" + "async-form-response" + "\n" +
                                  "&status=" + "processing" + "\n" +
                                  "&serial-number=" + transactionData.SerialNumber + "\n" +
                                  "&merchant-order-id=" + model.client_orderid + "\n" +
                                  "&paynet-order-id=" + transactionData.TransactionId + "\n" +
                                  "&redirect_url=" + HttpUtility.UrlEncode(transactionData.RedirectUri);

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

        internal ServiceTransitionResult PreAuthFormMultiCurrency(int endpointGroupId, PreAuthFormRequestModel model)
        {
            return new ServiceTransitionResult(HttpStatusCode.OK,
                "Method [PreAuthFormService.PreAuthFormMultiCurrency] is not supported yet");
        }
    }

}