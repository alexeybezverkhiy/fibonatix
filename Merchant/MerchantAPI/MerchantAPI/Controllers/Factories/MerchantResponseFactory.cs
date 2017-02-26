﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using MerchantAPI.Services;

namespace MerchantAPI.Controllers.Factories
{
    public class MerchantResponseFactory
    {
        public static HttpResponseMessage CreateTextHtmlResponseMessage(
            ServiceTransitionResult serviceResult)
        {
            HttpResponseMessage response = new HttpResponseMessage(serviceResult.Status);

            if(serviceResult.Status == System.Net.HttpStatusCode.Redirect ||
                serviceResult.Status == System.Net.HttpStatusCode.TemporaryRedirect ||
                serviceResult.Status == System.Net.HttpStatusCode.MovedPermanently ||
                serviceResult.Status == System.Net.HttpStatusCode.Moved)
            {
                response.Headers.Location = new Uri(serviceResult.StringContent);
            }
            else
            {
                response.Content = new StringContent(serviceResult.StringContent);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            }
            return response;
        }
    }
}