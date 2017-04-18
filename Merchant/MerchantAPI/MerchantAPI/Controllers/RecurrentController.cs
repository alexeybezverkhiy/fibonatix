using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MerchantAPI.Controllers.Factories;
using MerchantAPI.Models;
using MerchantAPI.Services;
using MerchantAPI.Helpers;

namespace MerchantAPI.Controllers
{
    public class CreateCardRefController : ApiController
    {
        private VoidService _service;

        public CreateCardRefController() {
            _service = new VoidService();
        }

        [HttpPost]
        public HttpResponseMessage SingleCurrency(
            [FromUri] int endpointId,
            [FromBody] CreateCardRefRequestModel model)
        {
            CreateCardRefResponseModel err = null;
            ServiceTransitionResult result = null;

            string controlKey = WebApiConfig.Settings.GetMerchantControlKey(endpointId);
            if (string.IsNullOrEmpty(controlKey))
            {
                err = new CreateCardRefResponseModel();
                err.SetValidationError("2", "UNREACHABLE_CONTROL_CODE");
            }
            else
            {
                if (model.IsHashValid(endpointId, controlKey))
                {
                    string raw = RawContentReader.Read(Request).Result;
                    // TODO - implement service call
                    //result = _service.VoidSingleCurrency(endpointId, model, raw);
                }
                else
                {
                    err = new CreateCardRefResponseModel();
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
            [FromBody] CreateCardRefRequestModel model)
        {
            return SingleCurrency(endpointGroupId, model);
        }
    }

    public class GetCardInfoController : ApiController
    {
        private VoidService _service;

        public GetCardInfoController()
        {
            _service = new VoidService();
        }

        [HttpPost]
        public HttpResponseMessage SingleCurrency(
            [FromUri] int endpointId,
            [FromBody] GetCardInfoRequestModel model)
        {
            GetCardInfoResponseModel err = null;
            ServiceTransitionResult result = null;

            string controlKey = WebApiConfig.Settings.GetMerchantControlKey(endpointId);
            if (string.IsNullOrEmpty(controlKey))
            {
                err = new GetCardInfoResponseModel();
                err.SetValidationError("2", "UNREACHABLE_CONTROL_CODE");
            }
            else
            {
                if (model.IsHashValid(endpointId, controlKey))
                {
                    string raw = RawContentReader.Read(Request).Result;
                    // TODO - implement service call
                    //result = _service.VoidSingleCurrency(endpointId, model, raw);
                }
                else
                {
                    err = new GetCardInfoResponseModel();
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
            [FromBody] GetCardInfoRequestModel model)
        {
            return SingleCurrency(endpointGroupId, model);
        }
    }

    public class MakeRebillController : ApiController
    {
        private VoidService _service;

        public MakeRebillController()
        {
            _service = new VoidService();
        }

        [HttpPost]
        public HttpResponseMessage SingleCurrency(
            [FromUri] int endpointId,
            [FromBody] RecurrentRequestModel model)
        {
            RecurrentResponseModel err = null;
            ServiceTransitionResult result = null;

            string controlKey = WebApiConfig.Settings.GetMerchantControlKey(endpointId);
            if (string.IsNullOrEmpty(controlKey))
            {
                err = new RecurrentResponseModel();
                err.SetValidationError("2", "UNREACHABLE_CONTROL_CODE");
            }
            else
            {
                if (model.IsHashValid(endpointId, controlKey))
                {
                    string raw = RawContentReader.Read(Request).Result;
                    // TODO - implement service call
                    //result = _service.VoidSingleCurrency(endpointId, model, raw);
                }
                else
                {
                    err = new RecurrentResponseModel();
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
            [FromBody] RecurrentRequestModel model)
        {
            return SingleCurrency(endpointGroupId, model);
        }
    }

    public class MakeRebillPreAuthController : ApiController
    {
        private VoidService _service;

        public MakeRebillPreAuthController()
        {
            _service = new VoidService();
        }

        [HttpPost]
        public HttpResponseMessage SingleCurrency(
            [FromUri] int endpointId,
            [FromBody] RecurrentRequestModel model)
        {
            RecurrentResponseModel err = null;
            ServiceTransitionResult result = null;

            string controlKey = WebApiConfig.Settings.GetMerchantControlKey(endpointId);
            if (string.IsNullOrEmpty(controlKey))
            {
                err = new RecurrentResponseModel();
                err.SetValidationError("2", "UNREACHABLE_CONTROL_CODE");
            }
            else
            {
                if (model.IsHashValid(endpointId, controlKey))
                {
                    string raw = RawContentReader.Read(Request).Result;
                    // TODO - implement service call
                    //result = _service.VoidSingleCurrency(endpointId, model, raw);
                }
                else
                {
                    err = new RecurrentResponseModel();
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
            [FromBody] RecurrentRequestModel model)
        {
            return SingleCurrency(endpointGroupId, model);
        }
    }

    //[RoutePrefix("paynet/api/v2/sync/make-rebill-sale")]
    public class MakeRebillSaleController : ApiController
    {
        private VoidService _service;

        public MakeRebillSaleController()
        {
            _service = new VoidService();
        }

        [HttpPost]
        public HttpResponseMessage SingleCurrency(
            [FromUri] int endpointId,
            [FromBody] RecurrentRequestModel model)
        {
            RecurrentResponseModel err = null;
            ServiceTransitionResult result = null;

            string controlKey = WebApiConfig.Settings.GetMerchantControlKey(endpointId);
            if (string.IsNullOrEmpty(controlKey))
            {
                err = new RecurrentResponseModel();
                err.SetValidationError("2", "UNREACHABLE_CONTROL_CODE");
            }
            else
            {
                if (model.IsHashValid(endpointId, controlKey))
                {
                    string raw = RawContentReader.Read(Request).Result;
                    // TODO - implement service call
                    //result = _service.VoidSingleCurrency(endpointId, model, raw);
                }
                else
                {
                    err = new RecurrentResponseModel();
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
            [FromBody] RecurrentRequestModel model)
        {
            return SingleCurrency(endpointGroupId, model);
        }
    }

}
