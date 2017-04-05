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
    public class ReturnService
    {
        public ServiceTransitionResult ReturnSingleCurrency(
                    int endpointId,
                    ReturnRequestModel model,
                    string rawModel) {

            Transaction transactionData = new Transaction(TransactionType.Return, model.client_orderid);
            try {
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

                Transaction saleTransactionData = null;
                CommDoo.BackEnd.Requests.Request request = null;

                if (request == null) {
                    if (saleTransactionData == null) saleTransactionData = TransactionsDataStorage.FindByTransactionIdAndType(model.orderid, TransactionType.Sale);
                    if (saleTransactionData == null) saleTransactionData = TransactionsDataStorage.FindByTransactionIdAndType(model.orderid, TransactionType.SaleForm);
                    if (saleTransactionData == null) saleTransactionData = TransactionsDataStorage.FindByTransactionIdAndType(model.orderid, TransactionType.Capture);

                    // test -
                    if (model.orderid == "ffffffff-ffff-ffff-ffff-fffffffffffg") {
                        saleTransactionData = TransactionsDataStorage.CreateNewTransaction(TransactionType.Capture, model.client_orderid);
                        saleTransactionData.Status = TransactionStatus.Approved;
                        saleTransactionData.ProcessingTransactionId = "763120942";
                    }

                    if (saleTransactionData != null)
                        request = CommDoo.BackEnd.Requests.RefundRequest.createRequestByModel(model, endpointId, saleTransactionData.ProcessingTransactionId);
                }

                if (request == null) {
                    if (saleTransactionData == null) saleTransactionData = TransactionsDataStorage.FindByTransactionIdAndType(model.orderid, TransactionType.PreAuth);
                    if (saleTransactionData == null) saleTransactionData = TransactionsDataStorage.FindByTransactionIdAndType(model.orderid, TransactionType.PreAuthForm);

                    if (model.orderid == "ffffffff-ffff-ffff-ffff-fffffffffffh") {
                        saleTransactionData = TransactionsDataStorage.CreateNewTransaction(TransactionType.PreAuth, model.client_orderid);
                        saleTransactionData.Status = TransactionStatus.Approved;
                        saleTransactionData.ProcessingTransactionId = "410198004";
                    }
                    if (saleTransactionData != null)
                        request = CommDoo.BackEnd.Requests.CancelReservedAmountRequest.createRequestByModel(model, endpointId, saleTransactionData.ProcessingTransactionId);
                }

                if (request == null) {
                    return new ServiceTransitionResult(HttpStatusCode.OK,
                               "type=error\n" +
                               $"&serial-number={transactionData.SerialNumber}\n" +
                               $"&merchant-order-id={model.client_orderid}\n" +
                               $"&paynet-order-id={transactionData.TransactionId}\n" +
                               $"&error-message={HttpUtility.UrlEncode("Cannot find original transaction for Return")}\n" +
                               $"&error-code=5002");
                }

                string commdooResponse = request.executeRequest();
                CommDoo.BackEnd.Responses.Response xmlResponse = CommDoo.BackEnd.Responses.Response
                    .DeserializeFromString(commdooResponse);

                Cache.setBackendResponseData(transactionData.TransactionId, xmlResponse);

                string response;
                // Refund
                if (xmlResponse.Error == null && xmlResponse.Order != null) {
                    TransactionStatus status = xmlResponse.Order.Status != null
                        ? TransactionStatus.Approved
                        : TransactionStatus.Declined;
                    TransactionsDataStorage.UpdateTransaction(transactionData.TransactionId,
                        TransactionState.Finished, status);

                    response = "type=async-response\n" +
                               $"&status={(status == TransactionStatus.Approved ? "approved" : "declined")}\n" +
                               $"&serial-number={transactionData.SerialNumber}\n" +
                               $"&merchant-order-id={model.client_orderid}\n" +
                               $"&paynet-order-id={transactionData.TransactionId}\n" +
                               $"&extstatus={xmlResponse.Order.Status}\n";
                } else if (xmlResponse.Error == null && xmlResponse.Payment != null) {
                    TransactionStatus status = xmlResponse.Payment.Status != null
                        ? TransactionStatus.Approved
                        : TransactionStatus.Declined;
                    TransactionsDataStorage.UpdateTransaction(transactionData.TransactionId,
                        TransactionState.Finished, status);

                    response = "type=async-response\n" +
                               $"&status={(status == TransactionStatus.Approved ? "approved" : "declined")}\n" +
                               $"&serial-number={transactionData.SerialNumber}\n" +
                               $"&merchant-order-id={model.client_orderid}\n" +
                               $"&paynet-order-id={transactionData.TransactionId}\n" +
                               $"&extstatus={xmlResponse.Payment.Status}\n" + 
                               $"&extstatus2={xmlResponse.Payment.StatusAddition}\n";
                } else {
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
            } catch (Exception e) {
                TransactionsDataStorage.Store(transactionData, e);

                return new ServiceTransitionResult(HttpStatusCode.InternalServerError,
                    $"EXCP: Processing Return for [client_orderid={transactionData.TransactionId}] failed\n");
            } finally { }
        }

    }
}