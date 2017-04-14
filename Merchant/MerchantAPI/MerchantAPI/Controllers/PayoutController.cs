using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MerchantAPI.Controllers.Factories;
using MerchantAPI.Controllers.Exceptions;
using MerchantAPI.Models;
using MerchantAPI.Services;
using System.Net.Http.Headers;
using MerchantAPI.Helpers;

namespace MerchantAPI.Controllers
{
    public class PayoutController : ApiController
    {
        private const string HEADER_AUTH = "Authorization";

        private PayoutService _service;

        public PayoutController()
        {
            _service = new PayoutService();
        }

        [HttpPost]
        public HttpResponseMessage SingleCurrency(
            [FromUri] int endpointId,
            [FromBody] PayoutRequestModel model)
        {
            PayoutResponseModel err = null;
            ServiceTransitionResult result = null;

            try
            {
                string controlKey = WebApiConfig.Settings.GetMerchantControlKey(endpointId);
                if (string.IsNullOrEmpty(controlKey))
                {
                    err = new PayoutResponseModel(model.client_orderid);
                    err.SetValidationError("2", "UNREACHABLE_CONTROL_CODE");
                }
                else
                {
                    if (model.IsHashValid(endpointId, controlKey))
                    {
                        ValidateAuthHeader(endpointId, model, Request.Headers);

                        string raw = RawContentReader.Read(Request).Result;
                        result = _service.PayoutSingleCurrency(endpointId, model, raw);
                    }
                    else
                    {
                        err = new PayoutResponseModel(model.client_orderid);
                        err.SetValidationError("2", "INVALID_CONTROL_CODE");
                    }
                }
            }
            catch (InvalidAuthHeaderException e)
            {
                err = new PayoutResponseModel(model.client_orderid);
                err.SetValidationError("2", $"INVALID_AUTHORIZATION_HEADER: {e.Message}");
            }

            if (err != null)
            {
                result = new ServiceTransitionResult(HttpStatusCode.OK, err.ToHttpResponse());
            }
            HttpResponseMessage response = MerchantResponseFactory.CreateTextHtmlResponseMessage(result);
            return response;
        }

        private void ValidateAuthHeader(int endpointId, PayoutRequestModel model, HttpRequestHeaders headers)
        {
            if (!headers.Contains(HEADER_AUTH))
            {
                throw new InvalidAuthHeaderException($"Header '{HEADER_AUTH}' is absent");
            }
            string authHeader = headers.GetValues(HEADER_AUTH).First() ?? string.Empty;
            NameValueCollection authCollecion = null;
            try
            {
                authCollecion = ControllerHelper.DeserializeHeader(HEADER_AUTH, authHeader);
            }
            catch (Exception e)
            {
                throw new InvalidAuthHeaderException($"Header '{HEADER_AUTH}' is invalid: {e.Message}");
            }
            if (
                !"".Equals(authCollecion["OAuth realm"] ?? null)
                || !"1.0".Equals(authCollecion["oauth_version"] ?? null)
                || !"HMAC-SHA1".Equals(authCollecion["oauth_signature_method"] ?? null)
                || !model.oauth_consumer_key.Equals(authCollecion["oauth_consumer_key"] ?? null)
                || !model.oauth_nonce.Equals(authCollecion["oauth_nonce"] ?? null)
                || !model.oauth_signature_method.Equals(authCollecion["oauth_signature_method"] ?? null)
                || !model.oauth_timestamp.Equals(authCollecion["oauth_timestamp"] ?? null)
                || !model.oauth_version.Equals(authCollecion["oauth_version"] ?? null)
                )
            {
                throw new InvalidAuthHeaderException($"Header '{HEADER_AUTH}' is invalid: inconsistent field set with parameter set");
            }

            string authSignature = authCollecion["oauth_signature"] ?? string.Empty;
            // TODO validate 'oauth_signature'
        }
    }
}
