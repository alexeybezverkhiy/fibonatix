using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace MerchantAPI.Connectors
{
    public class CommDooFrontendConnector
    {
        public static WebClient CreateWebClient()
        {
            WebClient client = new WebClient();

            //client.UploadValuesCompleted += UploadValuesCompleted;
            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            return client;
        }
    }

    public class MerchantConnector
    {
        public static WebClient CreateWebClient()
        {
            WebClient client = new WebClient();

            //client.UploadValuesCompleted += UploadValuesCompleted;
            //client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            return client;
        }
    }
}