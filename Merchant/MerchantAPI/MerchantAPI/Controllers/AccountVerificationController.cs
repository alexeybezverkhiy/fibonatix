using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MerchantAPI.Models;

namespace MerchantAPI.Controllers
{
    public class AccountVerificationController : ApiController
    {

        [HttpPost]
        public string SingleCurrency(
            [FromUri] int endpointId,
            [FromBody] SaleRequestModel model)
        {
            throw new NotImplementedException();
        }

    }
}
