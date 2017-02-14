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
            [FromBody] SaleRequestModel model)
        {
            ServiceTransitionResult result = _service.PreAuthSingleCurrency(endpointId, model);

            HttpResponseMessage response = MerchantResponseFactory.CreateTextHtmlResponseMessage(result);
            return response;
        }

    }
}
