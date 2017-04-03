using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using MerchantAPI.Controllers.Factories;
using MerchantAPI.Models;
using MerchantAPI.Services;
using MerchantAPI.Data;
using MerchantAPI.Helpers;

namespace MerchantAPI.Controllers
{
    public class PreAuthFormController : ApiController {
        private PreAuthFormService _service;

        public PreAuthFormController() {
            _service = new PreAuthFormService();
        }

        [HttpPost]
        public HttpResponseMessage SingleCurrency(
            [FromUri] int endpointId,
            [FromBody] PreAuthFormRequestModel model) {
            PreAuthFormResponseModel err = null;
            ServiceTransitionResult result = null;

            string controlKey = WebApiConfig.Settings.GetMerchantControlKey(endpointId);
            if (string.IsNullOrEmpty(controlKey)) {
                err = new PreAuthFormResponseModel(model.client_orderid);
                err.SetValidationError("2", "INVALID_CONTROL_CODE");
            } else if (string.IsNullOrEmpty(model.client_orderid)) {
                err = new PreAuthFormResponseModel(null);
                err.SetValidationError("2", "INVALID_INCOMING_DATA");
            } else {
                if (model.IsHashValid(endpointId, controlKey)) {
                    string raw = RawContentReader.Read(Request).Result;
                    result = _service.PreAuthFormSingleCurrency(endpointId, model, raw);
                } else {
                    err = new PreAuthFormResponseModel(model.client_orderid);
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
            [FromBody] PreAuthFormRequestModel model) {
            return SingleCurrency(endpointGroupId, model);
        }

        [HttpGet]
        [ActionName("success")]
        public HttpResponseMessage SingleSuccessPostback(
            [FromUri] int endpointId,
            [FromUri] PreAuthFormSuccessPaymentModel model) {

            if (!CommDooFrontendFactory.SuccessHashIsValid(endpointId, model)) {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            if (model.transactionid != null) {
                TransactionsDataStorage.UpdateTransaction(model.fibonatixID, model.transactionid,
                    TransactionState.Finished);
            } else {
                TransactionsDataStorage.UpdateTransactionState(model.fibonatixID,
                    TransactionState.Finished);
            }

            RedirectResponseModel responseData = new RedirectResponseModel(model.referenceid);
            responseData.merchant_order = model.fibonatixID;
            responseData.status = "approved";
            string controlKey = WebApiConfig.Settings.GetMerchantControlKey(endpointId);
            responseData.control = HashHelper.SHA1(responseData.AssemblyHashContent(endpointId, controlKey));
            var result = new ServiceTransitionResult(HttpStatusCode.OK, responseData.ToHttpResponse(model.customerredirecturl));
            HttpResponseMessage response = MerchantResponseFactory.CreateTextHtmlResponseMessage(result);
            return response;
        }

        [HttpGet]
        [ActionName("failure")]
        public HttpResponseMessage SingleFailurePostback(
            [FromUri] int endpointId,
            [FromUri] PreAuthFormFailurePaymentModel model) {

            if (!CommDooFrontendFactory.FailureHashIsValid(endpointId, model)) {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            TransactionsDataStorage.UpdateTransaction(model.fibonatixID,
                TransactionState.Finished, TransactionStatus.Declined);

            RedirectResponseModel responseData = new RedirectResponseModel(model.referenceid);
            responseData.merchant_order = model.fibonatixID;
            responseData.status = "declined";
            string controlKey = WebApiConfig.Settings.GetMerchantControlKey(endpointId);
            responseData.control = HashHelper.SHA1(responseData.AssemblyHashContent(endpointId, controlKey));

            var result = new ServiceTransitionResult(HttpStatusCode.OK, responseData.ToHttpResponse(model.customerredirecturl));
            HttpResponseMessage response = MerchantResponseFactory.CreateTextHtmlResponseMessage(result);
            return response;
        }

        [HttpGet]
        [ActionName("success")]
        public HttpResponseMessage MultiSuccessPostback(
            [FromUri] int endpointGroupId,
            [FromUri] PreAuthFormSuccessPaymentModel model) {

            return SingleSuccessPostback(endpointGroupId, model);
        }

        [HttpGet]
        [ActionName("failure")]
        public HttpResponseMessage MultiFailurePostback(
        [FromUri] int endpointGroupId,
        [FromUri] PreAuthFormFailurePaymentModel model) {

            return SingleFailurePostback(endpointGroupId, model);
        }
    }
}
