using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MerchantAPI.Controllers.Factories;
using MerchantAPI.Models;
using MerchantAPI.Services;

namespace MerchantAPI.Controllers
{
    public class CaptureController : ApiController
    {
        private CaptureService _service;

        public CaptureController()
        {
            _service = new CaptureService();
        }

        [HttpPost]
        public HttpResponseMessage SingleCurrency(
            [FromUri] int endpointId,
            [FromBody] CaptureRequestModel model)
        {
            ServiceTransitionResult result = null;

            string controlKey = WebApiConfig.Settings.MerchantControlKeys["" + endpointId];
            if (string.IsNullOrEmpty(controlKey)) {
                CaptureResponseModel err = new CaptureResponseModel(model.client_orderid);
                err.SetValidationError("2", "INVALID_CONTROL_CODE");
            } else {
                if (model.IsHashValid(endpointId, controlKey)) {
                    result = _service.CaptureSingleCurrency(endpointId, model);
                } else {
                    CaptureResponseModel err = new CaptureResponseModel(model.client_orderid);
                    err.SetValidationError("2", "INVALID_CONTROL_CODE");

                    result = new ServiceTransitionResult(HttpStatusCode.OK, err.ToHttpResponse());
                }
            }
            HttpResponseMessage response = MerchantResponseFactory.CreateTextHtmlResponseMessage(result);
            return response;
        }
    }
}
