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
            ServiceTransitionResult result = null;

            string controlKey = WebApiConfig.Settings.MerchantControlKeys["" + endpointId];
            if (string.IsNullOrEmpty(controlKey))
            {
                SaleResponseModel err = new SaleResponseModel(model.client_orderid);
                err.SetValidationError("2", "INVALID_CONTROL_CODE");
            }
            else
            {
                if (model.IsHashValid(endpointId, controlKey))
                {
                    result = _service.SaleSingleCurrency(endpointId, model);
                }
                else
                {
                    SaleResponseModel err = new SaleResponseModel(model.client_orderid);
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
            [FromBody] SaleRequestModel model)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [ActionName("success")]
        public string SuccessPostback(
            [FromUri] int endpointGroupId,
            [FromUri] SuccessPaymentModel model)
        {
            SaleResponseModel result = new SaleResponseModel(model.referenceid);
            return result.ToHttpResponse();
        }

        [HttpGet]
        [ActionName("failure")]
        public string FailurePostback(
            [FromUri] int endpointGroupId,
            [FromUri] FailurePaymentModel model)
        {
            SaleResponseModel result = new SaleResponseModel(model.referenceid);
            result.SetError(model.errornumber, model.errortext);
            return result.ToHttpResponse();
        }

    }
}
