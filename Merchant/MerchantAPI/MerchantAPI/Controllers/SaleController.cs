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
    public class SaleController : ApiController
    {
        private SaleService _service;

        public SaleController()
        {
            _service = new SaleService();
        }

        [HttpPost]
        public HttpResponseMessage SingleCurrency(
            [FromUri] int endpointId,
            [FromBody] SaleRequestModel model)
        {
            SaleResponseModel err = null;
            ServiceTransitionResult result = null;

            string controlKey = WebApiConfig.Settings.GetMerchantControlKey(endpointId);
            if (string.IsNullOrEmpty(controlKey)) {
                err = new SaleResponseModel(model.client_orderid);
                err.SetValidationError("2", "INVALID_CONTROL_CODE");
            } else if (string.IsNullOrEmpty(model.client_orderid)) {
                err = new SaleResponseModel(null);
                err.SetValidationError("2", "INVALID_INCOMING_DATA");
            } else {
                if (model.IsHashValid(endpointId, controlKey)) {
                    string raw = RawContentReader.Read(Request).Result;
                    result = _service.SaleSingleCurrency(endpointId, model, raw);
                } else {
                    err = new SaleResponseModel(model.client_orderid);
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
            [FromBody] SaleRequestModel model)
        {
            SaleResponseModel err = null;
            ServiceTransitionResult result = null;

            string controlKey = WebApiConfig.Settings.GetMerchantControlKey(endpointGroupId);
            if (string.IsNullOrEmpty(controlKey))
            {
                err = new SaleResponseModel(model.client_orderid);
                err.SetValidationError("2", "INVALID_CONTROL_CODE");
            }
            else if (string.IsNullOrEmpty(model.client_orderid))
            {
                err = new SaleResponseModel(null);
                err.SetValidationError("2", "INVALID_INCOMING_DATA");
            }
            else
            {
                if (model.IsHashValid(endpointGroupId, controlKey))
                {
                    string raw = RawContentReader.Read(Request).Result;
                    result = _service.SaleMultiCurrency(endpointGroupId, model, raw);
                }
                else
                {
                    err = new SaleResponseModel(model.client_orderid);
                    err.SetValidationError("2", "INVALID_CONTROL_CODE");
                }
            }

            if (err != null)
            {
                result = new ServiceTransitionResult(HttpStatusCode.OK, err.ToHttpResponse());
            }
            HttpResponseMessage response = MerchantResponseFactory.CreateTextHtmlResponseMessage(result);
            return response;
        }

        [HttpGet]
        [ActionName("success")]
        public HttpResponseMessage SingleSuccessPostback(
            [FromUri] int endpointId,
            [FromUri] PostbackSuccessModel model)
        {
            string sharedSecret = WebApiConfig.Settings.GetSharedSecret(endpointId);
            if (!model.IsHashValid(sharedSecret))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            if (model.transactionid != null)
            {
                TransactionsDataStorage.UpdateTransaction(model.fibonatixID, model.transactionid,
                    TransactionState.Finished, TransactionStatus.Approved);
            }
            else
            {
                TransactionsDataStorage.UpdateTransaction(model.fibonatixID,
                    TransactionState.Finished, TransactionStatus.Approved);
            }

            string redirectHTML = RedirectHelper.CreateRedirectHtml(RedirectHelper.RedirectTemplate,
                model.customerredirecturl);
            var result = new ServiceTransitionResult(HttpStatusCode.OK, redirectHTML);

            HttpResponseMessage response = MerchantResponseFactory.CreateTextHtmlResponseMessage(result);
            return response;
        }

        [HttpGet]
        [ActionName("failure")]
        public HttpResponseMessage SingleFailurePostback(
            [FromUri] int endpointId,
            [FromUri] PostbackFailureModel model)
        {
            string sharedSecret = WebApiConfig.Settings.GetSharedSecret(endpointId);
            if (!model.IsHashValid(sharedSecret))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

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
            [FromUri] PostbackSuccessModel model)
        {
            return SingleSuccessPostback(endpointGroupId, model);
        }

        [HttpGet]
        [ActionName("failure")]
        public HttpResponseMessage MultiFailurePostback(
            [FromUri] int endpointGroupId,
            [FromUri] PostbackFailureModel model)
        {
            return SingleFailurePostback(endpointGroupId, model);
        }
    }
}
