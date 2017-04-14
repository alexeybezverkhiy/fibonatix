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
using MerchantAPI.App_Start;

namespace MerchantAPI.Services
{
    public class AccountVerificationService
    {

        public ServiceTransitionResult AccountVerificationSingleCurrency(
                int endpointId,
                AccountVerificationRequestModel model,
                string rawModel) {

            string fibonatixId = Transaction.CreateTransactionId();

            ServiceTransitionResult accountVerification = AccountVerification(endpointId, model, rawModel, fibonatixId);

            return accountVerification;
        }
        public ServiceTransitionResult AccountVerificationMultiCurrency(
                int endpointId,
                AccountVerificationRequestModel model,
                string rawModel) {

            string fibonatixId = Transaction.CreateTransactionId();

            ServiceTransitionResult accountVerification = AccountVerification(endpointId, model, rawModel, fibonatixId);

            return accountVerification;
        }

        public ServiceTransitionResult AccountVerification(
                int endpointId,
                AccountVerificationRequestModel model,
                string rawModel,
                string fibonatixId) {

            Transaction transactionData = null;

            try {

                transactionData = new Transaction(TransactionType.Verify, model.client_orderid, fibonatixId);
                transactionData.State = TransactionState.Started;
                transactionData.Status = TransactionStatus.Undefined;

                TransactionsDataStorage.Store(transactionData);

                CommDoo.BackEnd.Requests.ReserveAmountRequest request = CommDoo.BackEnd.Requests.ReserveAmountRequest
                    .createRequestByModel(model, endpointId, fibonatixId);

                string commdooResponse = request.executeRequest();
                CommDoo.BackEnd.Responses.Response xmlResponse = CommDoo.BackEnd.Responses.Response
                    .DeserializeFromString(commdooResponse);

                Cache.SetBackendResponseData(transactionData.TransactionId, xmlResponse.Error);

                AccountVerificationResponseModel avResponse = new AccountVerificationResponseModel(model.client_orderid);
                avResponse.serial_number = transactionData.SerialNumber;
                avResponse.merchant_order_id = model.client_orderid;
                avResponse.paynet_order_id = transactionData.TransactionId;

                if (xmlResponse.Error == null && xmlResponse.Payment != null) {
                    TransactionStatus status = ( xmlResponse.Payment.Status == "Charged" || xmlResponse.Payment.Status == "Reserved" ) ? 
                            TransactionStatus.Approved : TransactionStatus.Declined;
                    TransactionsDataStorage.UpdateTransaction(transactionData.TransactionId, TransactionState.Finished, status);
                    avResponse.SetSucc();
                    avResponse.status = (status == TransactionStatus.Approved ? "approved" : "declined");

                } else {
                    TransactionsDataStorage.UpdateTransaction(transactionData.TransactionId, TransactionState.Finished, TransactionStatus.Error);
                    avResponse.SetError(xmlResponse.Error.ErrorNumber, xmlResponse.Error.ErrorMessage);
                }

                avResponse.asyncSendPostCallback(model.server_callback_url, endpointId);

                return new ServiceTransitionResult(HttpStatusCode.OK, avResponse.ToHttpResponse());

            } catch (Exception e) {
                TransactionsDataStorage.Store(transactionData, e);

                return new ServiceTransitionResult(HttpStatusCode.InternalServerError,
                    $"EXCP: Processing Capture for [client_orderid={transactionData.TransactionId}] failed\n");
            } finally { }

        }
    }
}