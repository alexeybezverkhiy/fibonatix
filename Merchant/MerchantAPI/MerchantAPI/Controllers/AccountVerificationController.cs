using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MerchantAPI.Controllers.Factories;
using MerchantAPI.Helpers;
using MerchantAPI.Models;
using MerchantAPI.Services;

namespace MerchantAPI.Controllers
{

    public class AccountVerificationController : ApiController
    {
        private AccountVerificationService _service;

        public AccountVerificationController()
        {
            _service = new AccountVerificationService();
        }

        [HttpPost]
        public HttpResponseMessage SingleCurrency(
            [FromUri] int endpointId,
            [FromBody] AccountVerificationRequestModel model)
        {
            AccountVerificationResponseModel err = null;
            ServiceTransitionResult result = null;

            string controlKey = WebApiConfig.Settings.GetMerchantControlKey(endpointId);
            if (string.IsNullOrEmpty(controlKey))
            {
                err = new AccountVerificationResponseModel(model.client_orderid);
                err.SetValidationError("2", "UNREACHABLE_CONTROL_CODE");
            }
            else
            {
                if (model.IsHashValid(endpointId, controlKey))
                {
                    string raw = RawContentReader.Read(Request).Result;
                    result = _service.AccountVerificationSingleCurrency(endpointId, model, raw);
                }
                else
                {
                    err = new AccountVerificationResponseModel(model.client_orderid);
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

        [HttpPost]
        public HttpResponseMessage MultiCurrency(
            [FromUri] int endpointGroupId,
            [FromBody] AccountVerificationRequestModel model)
        {
            return SingleCurrency(endpointGroupId, model);
        }
    }
}
