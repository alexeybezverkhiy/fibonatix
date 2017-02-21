﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MerchantAPI.Controllers.Factories;
using MerchantAPI.Models;
using MerchantAPI.Services;
using MerchantAPI.Data;

namespace MerchantAPI.Controllers
{
    public class PreAuthController : ApiController
    {
        private PreAuthService _service;

        public PreAuthController()
        {
            _service = new PreAuthService();
        }

        [HttpPost]
        public HttpResponseMessage SingleCurrency(
            [FromUri] int endpointId,
            [FromBody] PreAuthRequestModel model)
        {
            ServiceTransitionResult result = null;

            string controlKey = WebApiConfig.Settings.MerchantControlKeys["" + endpointId];
            if (string.IsNullOrEmpty(controlKey)) {
                PreAuthResponseModel err = new PreAuthResponseModel(model.client_orderid);
                err.SetValidationError("2", "INVALID_CONTROL_CODE");
            } else {
                if (model.IsHashValid(endpointId, controlKey)) {
                    result = _service.PreAuthSingleCurrency(endpointId, model);
                } else {
                    PreAuthResponseModel err = new PreAuthResponseModel(model.client_orderid);
                    err.SetValidationError("2", "INVALID_CONTROL_CODE");

                    result = new ServiceTransitionResult(HttpStatusCode.OK, err.ToHttpResponse());
                }
            }
            HttpResponseMessage response = MerchantResponseFactory.CreateTextHtmlResponseMessage(result);
            return response;
        }

        [HttpPost]
        public HttpResponseMessage MultiCurrency(
            [FromUri] int endpointGroupId,
            [FromBody] SaleRequestModel model) {
            throw new NotImplementedException();
        }

        [HttpGet]
        [ActionName("success")]
        public HttpResponseMessage SuccessPostback(
            [FromUri] int endpointId,
            [FromUri] PreAuthSuccessPaymentModel model) {
            if (model.transactionid != null)
            {
                TransactionsDataStorage.UpdateTransaction(model.fibonatixID, model.transactionid,
                    TransactionState.Finished);
            }
            else
            {
                TransactionsDataStorage.UpdateTransactionState(model.fibonatixID, 
                    TransactionState.Finished);
            }
            TransactionsDataStorage.UpdateTransactionStatus(model.fibonatixID, TransactionStatus.Approved);

            var result = new ServiceTransitionResult(HttpStatusCode.Moved, model.customerredirecturl);
            HttpResponseMessage response = MerchantResponseFactory.CreateTextHtmlResponseMessage(result);
            return response;
        }

        [HttpGet]
        [ActionName("failure")]
        public HttpResponseMessage FailurePostback(
            [FromUri] int endpointId,
            [FromUri] PreAuthFailurePaymentModel model) {
            TransactionsDataStorage.UpdateTransaction(model.fibonatixID, 
                TransactionState.Finished, TransactionStatus.Declined);
            var result = new ServiceTransitionResult(HttpStatusCode.Moved, model.customerredirecturl);
            HttpResponseMessage response = MerchantResponseFactory.CreateTextHtmlResponseMessage(result);
            return response;
        }
    }
}
