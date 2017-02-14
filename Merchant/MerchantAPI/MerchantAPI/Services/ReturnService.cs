using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using MerchantAPI.Models;

namespace MerchantAPI.Services
{
    public class ReturnService
    {

        public ServiceTransitionResult ReturnSingleCurrency(
            int endpointId,
            SaleRequestModel model)
        {
            return new ServiceTransitionResult(HttpStatusCode.OK,
                "Method [ReturnService.ReturnSingleCurrency] is not implemented yet");
        }

    }
}