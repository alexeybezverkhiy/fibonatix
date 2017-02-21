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
    public class CaptureService
    {

        public ServiceTransitionResult CaptureSingleCurrency(
            int endpointId,
            CaptureRequestModel model)
        {
            byte[] partnerResponse = new byte[0];
            CommDoo.BackEnd.Requests.CaptureReservedAmountRequest request = null;
            string response = "";
            try {

                Transaction preAuthTransactionData = TransactionsDataStorage.FindByTransactionId(model.orderid);
                Transaction transactionData = TransactionsDataStorage.CreateNewTransaction(TransactionType.Capture, model.client_orderid);
                TransactionsDataStorage.UpdateTransactionState(transactionData.TransactionId, TransactionState.Started);
                TransactionsDataStorage.UpdateTransactionStatus(transactionData.TransactionId, TransactionStatus.Undefined);
                Cache.setCaptureRequestData(transactionData.TransactionId, model);

                request = CommDoo.BackEnd.Requests.CaptureReservedAmountRequest.createRequestByModel(model, preAuthTransactionData.ProcessingTransactionId);

                string resp = request.executeRequest();

                CommDoo.BackEnd.Responses.Response xmlResponse = CommDoo.BackEnd.Responses.Response.DeserializeFromString(resp);

                Cache.setBackendResponseData(transactionData.TransactionId, xmlResponse);

                if (xmlResponse.Error == null && xmlResponse.Payment != null) {
                    TransactionsDataStorage.UpdateTransactionState(transactionData.TransactionId, TransactionState.Finished);
                    if(xmlResponse.Payment.Status == "Charged")
                        TransactionsDataStorage.UpdateTransactionStatus(transactionData.TransactionId, TransactionStatus.Approved);
                    else
                        TransactionsDataStorage.UpdateTransactionStatus(transactionData.TransactionId, TransactionStatus.Declined);
                    response = "type=async-response" + "\n" +
                               "&serial-number=" + transactionData.SerialNumber + "\n" +
                               "&merchant-order-id=" + model.client_orderid + "\n" +
                               "&paynet-order-id=" + transactionData.TransactionId + "\n";
                } else {
                    TransactionsDataStorage.UpdateTransactionState(transactionData.TransactionId, TransactionState.Finished);
                    TransactionsDataStorage.UpdateTransactionStatus(transactionData.TransactionId, TransactionStatus.Error);

                    response = "type=error" + "\n" +
                               "&serial-number=" + transactionData.SerialNumber + "\n" +
                               "&merchant-order-id=" + model.client_orderid + "\n" +
                               "&paynet-order-id=" + transactionData.TransactionId + "\n" +
                               "&error-message=" + xmlResponse.Error.ErrorMessage + "\n" +
                               "&error-code=" + xmlResponse.Error.ErrorNumber + "\n";
                }
                partnerResponse = Encoding.UTF8.GetBytes(response);

            } catch (Exception e) {
                return new ServiceTransitionResult(HttpStatusCode.InternalServerError,
                    "CONNECTION ERROR: " + e.Message + "\n");
            } finally { }

            string strResponse = Encoding.UTF8.GetString(partnerResponse);

            return new ServiceTransitionResult(HttpStatusCode.OK,
                strResponse + "\n");
        }

    }
}