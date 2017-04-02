using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using MerchantAPI.Models;

namespace MerchantAPI.Services
{
    public class PayoutService
    {

        public ServiceTransitionResult PayoutSingleCurrency(
            int endpointId,
            PayoutRequestModel model,
            string rawModel)
        {
            return new ServiceTransitionResult(HttpStatusCode.OK,
                "Method [PayoutService.PayoutSingleCurrency] is not implemented yet");
        }

    }
}