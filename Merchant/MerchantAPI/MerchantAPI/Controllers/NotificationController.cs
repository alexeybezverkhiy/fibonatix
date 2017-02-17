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
    public class NotificationController : ApiController
    {
        private NotificationService _service;

        public NotificationController()
        {
            _service = new NotificationService();
        }

        [HttpPost]
        [ActionName("success")]
        public HttpResponseMessage Success(
            [FromBody] NotificationRequestModel model)
        {
            ServiceTransitionResult result;
            if (model.IsHashValid(WebApiConfig.Settings.SharedSecret))
            {
                result = _service.Notified(model);
            }
            else
            {
                result = new ServiceTransitionResult(HttpStatusCode.BadRequest, 
                    "Broken value of parameter 'hash' (broken message digest)");
            }
            HttpResponseMessage response = MerchantResponseFactory.CreateTextHtmlResponseMessage(result);
            return response;
        }
    }
}
