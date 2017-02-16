using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using MerchantAPI.Models;

namespace MerchantAPI.Services
{
    public class NotificationService
    {

        public ServiceTransitionResult Notified(
            NotificationRequestModel model)
        {
            return new ServiceTransitionResult(HttpStatusCode.OK,
                "Method [NotificationService.Notified] is not implemented yet");
        }

    }
}