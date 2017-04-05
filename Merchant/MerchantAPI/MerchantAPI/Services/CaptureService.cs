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
    public class CaptureService
    {

        public ServiceTransitionResult CaptureSingleCurrency(
            int endpointId,
            CaptureRequestModel model,
            string rawModel)
        {
            Transaction transactionData = new Transaction(TransactionType.Capture, model.client_orderid);
            try
            {
                NameValueCollection referenceQuery = ControllerHelper.DeserializeHttpParameters(rawModel);
                ControllerHelper.EliminateCardData(referenceQuery);

                transactionData.State = TransactionState.Started;
                transactionData.Status = TransactionStatus.Undefined;
                transactionData.ReferenceQuery = ControllerHelper.SerializeHttpParameters(referenceQuery);

                TransactionsDataStorage.Store(transactionData);

                // abv: cache version
                //TransactionsDataStorage.UpdateTransaction(transactionData.TransactionId, TransactionState.Started,
                //    TransactionStatus.Undefined);
                //Cache.setCaptureRequestData(transactionData.TransactionId, model);

                Transaction preAuthTransactionData = null;

                preAuthTransactionData = TransactionsDataStorage
                    .FindByTransactionIdAndType(model.orderid, TransactionType.PreAuth);

                if (model.orderid == "ffffffff-ffff-ffff-ffff-fffffffffffi" ) {
                    preAuthTransactionData = TransactionsDataStorage.CreateNewTransaction(TransactionType.Capture, model.client_orderid);
                    preAuthTransactionData.Status = TransactionStatus.Approved;
                    preAuthTransactionData.ProcessingTransactionId = "410198004";
                }

                CommDoo.BackEnd.Requests.CaptureReservedAmountRequest request = CommDoo.BackEnd.Requests.CaptureReservedAmountRequest
                    .createRequestByModel(model, endpointId, preAuthTransactionData.ProcessingTransactionId);
                string commdooResponse = request.executeRequest();
                CommDoo.BackEnd.Responses.Response xmlResponse = CommDoo.BackEnd.Responses.Response
                    .DeserializeFromString(commdooResponse);

                Cache.setBackendResponseData(transactionData.TransactionId, xmlResponse);

                string response;
                if (xmlResponse.Error == null && xmlResponse.Payment != null)
                {
                    TransactionStatus status = xmlResponse.Payment.Status == "Charged" 
                        ? TransactionStatus.Approved 
                        : TransactionStatus.Declined;
                    TransactionsDataStorage.UpdateTransaction(transactionData.TransactionId, 
                        TransactionState.Finished, status);

                    response = "type=async-response\n" +
                               $"&status={(status == TransactionStatus.Approved ? "approved" : "declined")}\n" +
                               $"&serial-number={transactionData.SerialNumber}\n" +
                               $"&merchant-order-id={model.client_orderid}\n" +
                               $"&paynet-order-id={transactionData.TransactionId}";
                }
                else
                {
                    TransactionsDataStorage.UpdateTransaction(transactionData.TransactionId, TransactionState.Finished,
                        TransactionStatus.Error);

                    response = "type=error\n" +
                               $"&serial-number={transactionData.SerialNumber}\n" +
                               $"&merchant-order-id={model.client_orderid}\n" +
                               $"&paynet-order-id={transactionData.TransactionId}\n" +
                               $"&error-message={HttpUtility.UrlEncode(xmlResponse.Error.ErrorMessage)}\n" +
                               $"&error-code={xmlResponse.Error.ErrorNumber}";
                }

                return new ServiceTransitionResult(HttpStatusCode.OK,
                    response + "\n");
            }
            catch (Exception e)
            {
                TransactionsDataStorage.Store(transactionData, e);

                return new ServiceTransitionResult(HttpStatusCode.InternalServerError,
                    $"EXCP: Processing Capture for [client_orderid={transactionData.TransactionId}] failed\n");
            } finally { }
        }
    }
}