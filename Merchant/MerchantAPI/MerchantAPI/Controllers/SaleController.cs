using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            //            SaleResponseModel result = _service.SaleMultiCurrency(endpointGroupId, model);
            //            return result.ToHttpResponse();
            ServiceTransitionResult result = _service.SaleSingleCurrency(endpointId, model);

            HttpResponseMessage response = MerchantResponseFactory.CreateTextHtmlResponseMessage(result);
            return response;
        }

        [HttpPost]
        public HttpResponseMessage MultiCurrency(
            [FromUri] int endpointGroupId,
            [FromBody] SaleRequestModel model)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [ActionName("success")]
        public string MultiCurrencySucc(
            [FromUri] int endpointGroupId,
            [FromUri] SuccessPaymentModel model)
        {
            SaleResponseModel result = new SaleResponseModel();
            return result.ToHttpResponse();
        }

        [HttpGet]
        [ActionName("failure")]
        public string MultiCurrencyFail(
            [FromUri] int endpointGroupId,
            [FromUri] FailurePaymentModel model)
        {
            SaleResponseModel result = new SaleResponseModel();
            result.SetError(model.errornumber, model.errortext);
            return result.ToHttpResponse();
        }

    }
}
