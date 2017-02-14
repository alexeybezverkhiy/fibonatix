using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using MerchantAPI.Models;

namespace MerchantAPI.Services
{
    public class CaptureService
    {

        public ServiceTransitionResult CaptureSingleCurrency(
            int endpointId,
            SaleRequestModel model)
        {
            return new ServiceTransitionResult(HttpStatusCode.OK,
                "Method [CaptureService.CaptureSingleCurrency] is not implemented yet");
        }

    }
}