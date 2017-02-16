using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using MerchantAPI.Connectors;
using MerchantAPI.Controllers.Factories;
using MerchantAPI.Models;

namespace MerchantAPI.Services
{
    public class SaleService
    {
        public const string ESCAPE = "(escape('";


        public ServiceTransitionResult SaleSingleCurrency(int endpointId, SaleRequestModel model)
        {
            byte[] partnerResponse = new byte[0];
            NameValueCollection requestParameters = null;
            try
            {
                requestParameters = CommDooFrontendFactory.CreateMultyCurrencyPaymentParams(endpointId, model);

                var parameters = new StringBuilder();
                foreach (string key in requestParameters.Keys) {
                    parameters.AppendFormat("{0}={1}&",
                        HttpUtility.UrlEncode(key),
                        HttpUtility.UrlEncode(requestParameters[key]));
                }
                if (requestParameters.Count > 0)
                    parameters.Length -= 1;

                string redirectToCommDoo = WebApiConfig.Settings.PaymentASPXEndpoint + "?" + parameters.ToString();

                // Add to cache with key requestParameters['client_orderid'] and data redirectToCommDoo
                Data.Cache.setRedirectUrlForRequest(model.client_orderid, redirectToCommDoo);

                string response = "type=async-response&serial-number=00000000-0000-0000-0000-sale-" + model.client_orderid + "&merchant-order-id=" + model.client_orderid;

                partnerResponse = Encoding.UTF8.GetBytes(response);

            } catch (Exception e)
            {
                return new ServiceTransitionResult(HttpStatusCode.InternalServerError,
                    "CONNECTION ERROR: " + e.Message + "\n" );
            }
            finally { }

            string strResponse = Encoding.UTF8.GetString(partnerResponse);
            //            int begin = strResponse.IndexOf(ESCAPE);
            //            if (begin >= 0)
            //            {
            //                begin += ESCAPE.Length;
            //                int end = strResponse.IndexOf("')");
            //                string redirectUrl = strResponse.Substring(begin, end - begin);
            //                partnerResponse = client.UploadValues(new Uri(redirectUrl), "GET", new NameValueCollection());
            //                strResponse = u8.GetString(partnerResponse);
            //            }

            return new ServiceTransitionResult(HttpStatusCode.OK,
                strResponse + "\n" );
            //            SaleResponseModel succ = new SaleResponseModel();
            //            succ.SetSucc();
            //            return succ;
        }
    }

}