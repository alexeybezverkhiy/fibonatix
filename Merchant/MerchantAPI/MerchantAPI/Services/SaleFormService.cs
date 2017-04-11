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
    public class SaleFormService
    {
        public ServiceTransitionResult SaleFormSingleCurrency(
            int endpointId, 
            SaleFormRequestModel model, 
            string rawModel)
        {
            string fibonatixId = Transaction.CreateTransactionId();
            NameValueCollection requestParameters = CommDooFrontendFactory.CreateSingleCurrencyPaymentParams(
                endpointId, model, fibonatixId);

            ServiceTransitionResult saleForm = SaleForm(rawModel, model.client_orderid, fibonatixId, requestParameters);
            return saleForm;
        }

        public ServiceTransitionResult SaleFormMultiCurrency(
            int endpointGroupId, 
            SaleFormRequestModel model,
            string rawModel)
        {
            string fibonatixId = Transaction.CreateTransactionId();
            NameValueCollection requestParameters = CommDooFrontendFactory.CreateMultiCurrencyPaymentParams(
                endpointGroupId, model, fibonatixId);

            ServiceTransitionResult saleForm = SaleForm(rawModel, model.client_orderid, fibonatixId, requestParameters);
            return saleForm;
        }

        protected ServiceTransitionResult SaleForm(
            string rawModel,
            string merchantOrderId,
            string fibonatixId,
            NameValueCollection commDooRequestParams)
        {
            Transaction transactionData = new Transaction(TransactionType.SaleForm, merchantOrderId, fibonatixId);
            try
            {
                var parameters = new StringBuilder(256)
                    .Append(WebApiConfig.Settings.PaymentASPXEndpoint);
                char delim = '?';
                foreach (string key in commDooRequestParams.Keys)
                {
                    parameters
                        .Append(delim)
                        .Append(key)
                        .Append('=')
                        .Append(HttpUtility.UrlEncode(commDooRequestParams[key]));
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
                                  "&merchant-order-id=" + merchantOrderId + "\n" +
                                  "&paynet-order-id=" + transactionData.TransactionId + "\n" +
                                  "&redirect_url=" + HttpUtility.UrlEncode(transactionData.RedirectUri);

                return new ServiceTransitionResult(HttpStatusCode.OK,
                    response + "\n");
            }
            catch (Exception e)
            {
                TransactionsDataStorage.Store(transactionData, e);

                return new ServiceTransitionResult(HttpStatusCode.InternalServerError,
                    $"EXCP: Processing SaleForm for [client_orderid={transactionData.TransactionId}] failed\n");
            }
            finally { }
        }
    }
}