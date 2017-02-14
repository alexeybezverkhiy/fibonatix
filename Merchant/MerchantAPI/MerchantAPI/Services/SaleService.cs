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
        public const string PAYMENT_URL = "https://frontend.payment-transaction.net/payment.aspx";
        public const string ESCAPE = "(escape('";

        public SaleServiceTransitionResult SaleMultiCurrency(int endpointGroupId, SaleRequestModel model)
        {
            byte[] partnerResponse = new byte[0];
            WebClient client = CommDooFrontendConnector.CreateWebClient();

            try
            {
                NameValueCollection data = CommDooFrontendFactory.CreateMultyCurrencyPaymentParams(endpointGroupId, model);

                partnerResponse = client.UploadValues(new Uri(PAYMENT_URL), "POST", data);
            }
            catch (Exception e)
            {
                //                SaleResponseModel err = new SaleResponseModel();
                //                err.SetError("1", "CONNECTION ERROR: " + e.Message);
                //                return err;
                return new SaleServiceTransitionResult(HttpStatusCode.InternalServerError,
                    "CONNECTION ERROR: " + e.Message);

            }
            finally { }

            Encoding u8 = Encoding.UTF8;
            string strResponse = u8.GetString(partnerResponse);
//            int begin = strResponse.IndexOf(ESCAPE);
//            if (begin >= 0)
//            {
//                begin += ESCAPE.Length;
//                int end = strResponse.IndexOf("')");
//                string redirectUrl = strResponse.Substring(begin, end - begin);
//                partnerResponse = client.UploadValues(new Uri(redirectUrl), "GET", new NameValueCollection());
//                strResponse = u8.GetString(partnerResponse);
//            }
            return new SaleServiceTransitionResult(HttpStatusCode.OK,
                strResponse);
            //            SaleResponseModel succ = new SaleResponseModel();
            //            succ.SetSucc();
            //            return succ;
        }
    }

    public class SaleServiceTransitionResult
    {
        public SaleServiceTransitionResult(HttpStatusCode status, string stringContent)
        {
            Status = status;
            StringContent = stringContent;
        }

        public HttpStatusCode Status { get; set; }
        public string StringContent { get; set; }
    }
}