using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using MerchantAPI.Models;

namespace MerchantAPI.Services
{
    public class StatusService
    {
        public ServiceTransitionResult StatusSingleCurrency(
            int endpointId, 
            SaleRequestModel model)
        {
            return new ServiceTransitionResult(HttpStatusCode.OK,
                "Method [StatusService.StatusSingleCurrency] is not implemented yet");
        }
    }
}