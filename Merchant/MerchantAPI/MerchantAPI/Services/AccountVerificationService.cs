using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using MerchantAPI.Models;

namespace MerchantAPI.Services
{
    public class AccountVerificationService
    {
        public ServiceTransitionResult AccountVerificationSingleCurrency(
            int endpointId,
            SaleRequestModel model)
        {
            return new ServiceTransitionResult(HttpStatusCode.OK,
                "Method [AccountVerificationService.AccountVerificationSingleCurrency] is not implemented yet");
        }

    }
}