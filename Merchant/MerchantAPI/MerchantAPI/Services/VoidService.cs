﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using MerchantAPI.Models;

namespace MerchantAPI.Services
{
    public class VoidService
    {

        public ServiceTransitionResult VoidSingleCurrency(
            int endpointId,
            SaleRequestModel model)
        {
            return new ServiceTransitionResult(HttpStatusCode.OK,
                "Method [VoidService.VoidSingleCurrency] is not implemented yet");
        }

    }
}