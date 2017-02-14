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
        private SaleService _saleService;

        public SaleController()
        {
            _saleService = new SaleService();
        }

        /*        // GET: api/MerchantApi
                public IEnumerable<string> Get()
                {
                    return new string[] { "value1", "value2" };
                }
        */

        // GET: api/MerchantApi/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public string SingleCurrency(
            [FromUri] int endpointId,
            [FromBody] SaleRequestModel model)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public HttpResponseMessage MultiCurrency(
            [FromUri] int endpointGroupId,
            [FromUri] SaleRequestModel model)
        {
            //            SaleResponseModel result = _saleService.SaleMultiCurrency(endpointGroupId, model);
            //            return result.ToHttpResponse();
            SaleServiceTransitionResult result = _saleService.SaleMultiCurrency(endpointGroupId, model);

            HttpResponseMessage response = new HttpResponseMessage(result.Status);
            response.Content = new StringContent(result.StringContent);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;

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
