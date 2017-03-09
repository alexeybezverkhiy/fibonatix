using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Text;


// https://gatewaytest.borgun.is/ws/Heimir.pub.ws:Authorization"

namespace Fibonatix.CommDoo.Borgun.Network
{
    internal class Client
    {
        string Authorization = "";
        string baseURL = "";

        public Client(string login, string password, bool sandbox) {
            string authInfo = String.Format("{0}:{1}", login, password);
            Authorization = String.Format("Basic {0}", Convert.ToBase64String(Encoding.Default.GetBytes(authInfo)));
            if (sandbox)
                baseURL = "https://gatewaytest.borgun.is/ws/Heimir.pub.ws:Authorization";
            else
                baseURL = "https://gatewaytest.borgun.is/ws/Heimir.pub.ws:Authorization";
        }

        public string ProcessRequest(Fibonatix.CommDoo.Borgun.Entities.Requests.SOAPRequest req) {
            MemoryStream ret = ProcessRequest(req.SOAPAction(), System.Text.Encoding.UTF8.GetBytes(req.getXml()));
            return System.Text.Encoding.UTF8.GetString(ret.ToArray());
        }

        private MemoryStream ProcessRequest(string soapAction, byte[] request) {
            var webRequest = CreateWebRequest(soapAction, request);
            return GetResponseStream(webRequest);
        }

        private WebRequest CreateWebRequest(string soapAction, byte[] request) {
            var webRequest = WebRequest.Create(ComposeUrl());
            webRequest.Headers["Authorization"] = Authorization;
            webRequest.Headers["SOAPAction"] = soapAction;
            webRequest.Method = "POST";
            webRequest.ContentType = "text/xml";

            byte[] data = request;
            webRequest.ContentLength = data.Length;

            var httpWebRequest = webRequest as HttpWebRequest;
            if (httpWebRequest != null) {
                httpWebRequest.UserAgent = String.Format("Fibonatix.CommDoo.Borgun {0}", this.GetType().Assembly.GetName().Version.ToString());
                httpWebRequest.KeepAlive = false;
            }

            using (var requestStream = webRequest.GetRequestStream()) {
                requestStream.Write(data, 0, data.Length);
            }

            return webRequest;
        }

        private string ComposeUrl() {
            string url = null;
            url = String.Format("{0}", baseURL);
            return url;
        }


        private MemoryStream GetResponseStream(WebRequest webRequest) {
            WebResponse webResponse = null;
            try {
                webResponse = webRequest.GetResponse();
                return Copy(webResponse.GetResponseStream());
            } catch (WebException ex) {
                Stream responseStream;
                if (TryGetResponseDataFromWebException(ex, out responseStream)) {
                    return Copy(responseStream);
                }

                throw ex;
            } finally {
                if (webResponse != null) {
                    webResponse.Close();
                }
            }
        }
        private MemoryStream Copy(Stream stream) {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream;
        }

        private bool TryGetResponseDataFromWebException(WebException webException, out Stream responseData) {
            responseData = null;

            var response = webException.Response as HttpWebResponse;
            if (response == null) {
                return false;
            }

            // Unprocessable Entity (The request was well-formed but was unable to be followed due to semantic errors.)
            if (response.StatusCode == (HttpStatusCode)422) {
                responseData = response.GetResponseStream();
                response.Close();
                return true;
            }

            return false;
        }

    }
}
