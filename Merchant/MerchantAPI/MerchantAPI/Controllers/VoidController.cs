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
    public class VoidController : ApiController
    {
        private VoidService _service;

        public VoidController()
        {
            _service = new VoidService();
        }

        [HttpPost]
        public HttpResponseMessage SingleCurrency(
            [FromUri] int endpointId,
            [FromBody] SaleRequestModel model)
        {
            ServiceTransitionResult result = _service.VoidSingleCurrency(endpointId, model);

            HttpResponseMessage response = MerchantResponseFactory.CreateTextHtmlResponseMessage(result);
            return response;
        }
    }
}
