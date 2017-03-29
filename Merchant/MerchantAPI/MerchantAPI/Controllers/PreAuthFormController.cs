using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MerchantAPI.Controllers.Factories;
using MerchantAPI.Models;
using MerchantAPI.Services;
using MerchantAPI.Data;
using MerchantAPI.Helpers;

namespace MerchantAPI.Controllers
{
    public class PreAuthFormController : ApiController {
        private PreAuthService _service;

        public PreAuthFormController() {
            _service = new PreAuthService();
        }

        [HttpPost]
        public HttpResponseMessage SingleCurrency(
            [FromUri] int endpointId,
            [FromBody] PreAuthRequestModel model) {
            PreAuthResponseModel err = null;
            ServiceTransitionResult result = null;

            string controlKey = WebApiConfig.Settings.GetMerchantControlKey(endpointId);
            if (string.IsNullOrEmpty(controlKey)) {
                err = new PreAuthResponseModel(model.client_orderid);
                err.SetValidationError("2", "INVALID_CONTROL_CODE");
            } else if (string.IsNullOrEmpty(model.client_orderid)) {
                err = new PreAuthResponseModel(null);
                err.SetValidationError("2", "INVALID_INCOMING_DATA");
            } else {
                if (model.IsHashValid(endpointId, controlKey)) {
                    string raw = RawContentReader.Read(Request).Result;
                    result = _service.PreAuthSingleCurrency(endpointId, model, raw);
                } else {
                    err = new PreAuthResponseModel(model.client_orderid);
                    err.SetValidationError("2", "INVALID_CONTROL_CODE");
                }
            }

            if (err != null) {
                result = new ServiceTransitionResult(HttpStatusCode.OK, err.ToHttpResponse());
            }
            HttpResponseMessage response = MerchantResponseFactory.CreateTextHtmlResponseMessage(result);
            return response;
        }

        [HttpPost]
        public HttpResponseMessage MultiCurrency(
            [FromUri] int endpointGroupId,
            [FromBody] PreAuthRequestModel model) {

            return SingleCurrency(endpointGroupId, model);
        }

        [HttpGet]
        [ActionName("success")]
        public HttpResponseMessage SingleSuccessPostback(
            [FromUri] int endpointId,
            [FromUri] PreAuthSuccessPaymentModel model) {
            if (model.transactionid != null) {
                TransactionsDataStorage.UpdateTransaction(model.fibonatixID, model.transactionid,
                    TransactionState.Finished);
            } else {
                TransactionsDataStorage.UpdateTransactionState(model.fibonatixID,
                    TransactionState.Finished);
            }
            TransactionsDataStorage.UpdateTransactionStatus(model.fibonatixID, TransactionStatus.Approved);

            var result = new ServiceTransitionResult(HttpStatusCode.Redirect, "", model.customerredirecturl);
            HttpResponseMessage response = MerchantResponseFactory.CreateTextHtmlResponseMessage(result);
            return response;
        }

        [HttpGet]
        [ActionName("failure")]
        public HttpResponseMessage SingleFailurePostback(
            [FromUri] int endpointId,
            [FromUri] PreAuthFailurePaymentModel model) {
            TransactionsDataStorage.UpdateTransaction(model.fibonatixID,
                TransactionState.Finished, TransactionStatus.Declined);
            var result = new ServiceTransitionResult(HttpStatusCode.Redirect, "", model.customerredirecturl);
            HttpResponseMessage response = MerchantResponseFactory.CreateTextHtmlResponseMessage(result);
            return response;
        }

        [HttpGet]
        [ActionName("success")]
        public HttpResponseMessage MultiSuccessPostback(
            [FromUri] int endpointGroupId,
            [FromUri] PreAuthSuccessPaymentModel model) {

            return SingleSuccessPostback(endpointGroupId, model);
        }

        [HttpGet]
        [ActionName("failure")]
        public HttpResponseMessage MultiFailurePostback(
        [FromUri] int endpointGroupId,
        [FromUri] PreAuthFailurePaymentModel model) {

            return SingleFailurePostback(endpointGroupId, model);
        }
    }
}
