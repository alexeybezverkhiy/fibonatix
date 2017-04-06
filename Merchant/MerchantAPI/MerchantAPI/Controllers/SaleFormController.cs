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
    public class SaleFormController : ApiController {
        private SaleFormService _service;

        public SaleFormController() {
            _service = new SaleFormService();
        }

        [HttpPost]
        public HttpResponseMessage SingleCurrency(
            [FromUri] int endpointId,
            [FromBody] SaleFormRequestModel model) {

            SaleFormResponseModel err = null;
            ServiceTransitionResult result = null;

            string controlKey = WebApiConfig.Settings.GetMerchantControlKey(endpointId);
            if (string.IsNullOrEmpty(controlKey)) {
                err = new SaleFormResponseModel(model.client_orderid);
                err.SetValidationError("2", "INVALID_CONTROL_CODE");
            } else if (string.IsNullOrEmpty(model.client_orderid)) {
                err = new SaleFormResponseModel(null);
                err.SetValidationError("2", "INVALID_INCOMING_DATA");
            } else {
                if (model.IsHashValid(endpointId, controlKey)) {
                    string raw = RawContentReader.Read(Request).Result;
                    result = _service.SaleFormSingleCurrency(endpointId, model, raw);
                } else {
                    err = new SaleFormResponseModel(model.client_orderid);
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
            [FromBody] SaleFormRequestModel model) {
            return SingleCurrency(endpointGroupId, model);
        }

        [HttpGet]
        [ActionName("success")]
        public HttpResponseMessage SingleSuccessPostback(
            [FromUri] int endpointId,
            [FromUri] SaleFormSuccessPaymentModel model) {

            if (!CommDooFrontendFactory.SuccessHashIsValid(endpointId, model)) {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            if (model.transactionid != null) {
                TransactionsDataStorage.UpdateTransaction(model.fibonatixID, model.transactionid,
                    TransactionState.Finished, TransactionStatus.Approved);
            } else {
                TransactionsDataStorage.UpdateTransaction(model.fibonatixID,
                    TransactionState.Finished, TransactionStatus.Approved);
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
            [FromUri] SaleFormFailurePaymentModel model) {

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
            [FromUri] SaleFormSuccessPaymentModel model) {

            return SingleSuccessPostback(endpointGroupId, model);
        }

        [HttpGet]
        [ActionName("failure")]
        public HttpResponseMessage MultiFailurePostback(
        [FromUri] int endpointGroupId,
        [FromUri] SaleFormFailurePaymentModel model) {

            return SingleFailurePostback(endpointGroupId, model);
        }

    }
}
