using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using MerchantAPI.Controllers.Factories;
using MerchantAPI.Models;
using MerchantAPI.Services;

namespace MerchantAPI.Helpers
{
    public class PostbackHelper
    {
        public static HttpResponseMessage CreateRedirectContent(
            int endpointId,
            PostbackBaseModel model,
            RedirectStatus finalStatus)
        {
            RedirectResponseModel responseData = new RedirectResponseModel(model.referenceid);
            responseData.merchant_order = model.fibonatixID;
            responseData.client_orderid = model.referenceid;
            responseData.status = finalStatus;
            string controlKey = WebApiConfig.Settings.GetMerchantControlKey(endpointId);
            responseData.control = HashHelper.SHA1(responseData.AssemblyHashContent(endpointId, controlKey));

            var result = new ServiceTransitionResult(HttpStatusCode.OK, responseData.ToHttpResponse(model.customerredirecturl));

            HttpResponseMessage response = MerchantResponseFactory.CreateTextHtmlResponseMessage(result);
            return response;
        }
    }
}