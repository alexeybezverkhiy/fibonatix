using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using MerchantAPI.Models;

namespace MerchantAPI.Services
{
    public class PreAuthService
    {

        public ServiceTransitionResult PreAuthSingleCurrency(
            int endpointId, 
            SaleRequestModel model)
        {
            return new ServiceTransitionResult(HttpStatusCode.OK,
                "Method [PreAuthService.PreAuthSingleCurrency] is not implemented yet");
        }
    }
}