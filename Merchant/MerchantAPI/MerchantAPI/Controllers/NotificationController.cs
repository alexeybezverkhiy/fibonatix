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
        public HttpResponseMessage SingleSuccess(
            int endpointId,
            [FromBody] NotificationRequestModel model, 
            [FromUri] NotificationRequestExtraModel uriModel)
        {
            ServiceTransitionResult result;

            model.customernotifyurl = uriModel.customernotifyurl;
            model.fibonatixID = uriModel.fibonatixID;

            string sharedSecret = WebApiConfig.Settings.GetSharedSecret(endpointId);
            if (model.IsHashValid(sharedSecret))
            {
                result = _service.Notified(endpointId, model);
            }
            else
            {
                result = new ServiceTransitionResult(HttpStatusCode.BadRequest, 
                    "Broken value of parameter 'hash' (broken message digest)");
            }
            HttpResponseMessage response = MerchantResponseFactory.CreateTextHtmlResponseMessage(result);
            return response;
        }

        [HttpPost]
        [ActionName("success")]
        public HttpResponseMessage MultiSuccess(
            int endpointGroupId,
            [FromBody] NotificationRequestModel model,
            [FromUri] NotificationRequestExtraModel uriModel)
        {
            return SingleSuccess(endpointGroupId, model, uriModel);
        }

    }
}
