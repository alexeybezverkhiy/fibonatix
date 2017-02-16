using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using MerchantAPI.Models;
using System.Text;

namespace MerchantAPI.Services
{
    public class StatusService
    {
        static string redirectTemplate =
"<!DOCTYPE html>\n" +
"<html>\n" +
"<head>\n" +
    "<meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\">\n" +
    "<title>Redirecting...</title>\n" +
    "<script type = \"text/javascript\" language=\"javascript\">\n" +
        " function makeSubmit() {\n" +
            " document.returnform.submit();\n" +
        " }\n" +
    "</script>\n" +
"</head>\n" +
"\n" +
"<body onLoad = \"makeSubmit()\" >\n" +
"<form name=\"returnform\" action=\"{0}\" method=\"POST\">\n" +
    "<noscript>\n" +
        "<input type = \"submit\" name=\"submit\" value=\"Press this button to continue\"/>\n" +
    "</noscript>\n" +
"</form>\n" +
"</body>\n" +
"</html>\n";


        public ServiceTransitionResult StatusSingleCurrency(
            int endpointId,
            StatusRequestModel model)
        {
            byte[] partnerResponse = new byte[0];
            try {
                string redirectURL = Data.Cache.getRedirectUrlForRequest(model.client_orderid);
                string redirectHTML = redirectTemplate.Replace("{0}", redirectURL);
                // Add to cache with key requestParameters['client_orderid'] and data redirectToCommDoo
                string response = "type=status-response&serial-number=00000000-0000-0000-0000-status-" + model.client_orderid + "&status=approved&html=" + HttpUtility.UrlEncode(redirectHTML);
                partnerResponse = Encoding.UTF8.GetBytes(response);

            } catch (Exception e) {
                return new ServiceTransitionResult(HttpStatusCode.InternalServerError,
                    "CONNECTION ERROR: " + e.Message + "\n");
            } finally { }

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
                strResponse + "\n");
            //            SaleResponseModel succ = new SaleResponseModel();
            //            succ.SetSucc();
            //            return succ;
        }
    }
}