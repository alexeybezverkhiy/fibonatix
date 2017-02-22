using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Text;


namespace Fibonatix.CommDoo.ProcessingCom.Network
{
    internal class Client
    {
        // string baseURL = "https://integration.processing.com/secure/htrem_post.php";
        string baseSandBoxURL = "https://intsecure.processing.com/secure/htrem_post.php";
        string baseProductURL = "https://intportal.processing.com/secure/htrem_post.php";

        string threeDACSSandBoxURL = "https://3dstest.processing.com/acs_url.php";
        string threeDACSProductURL = "https://3ds.processing.com/acs_url.php";

        string threeDPaResSandBoxURL = "https://3dstest.processing.com/pares.php";
        string threeDPaResProductURL = "https://3ds.processing.com/pares.php";

        public Client() {
        }

        public string ProcessRequest(Entities.Request req) {
            
            MemoryStream ret = ProcessRequest(System.Text.Encoding.UTF8.GetBytes(req.getRequestString()), req.inSandBox, req.reqType);
            return System.Text.Encoding.UTF8.GetString(ret.ToArray());
        }

        private MemoryStream ProcessRequest(byte[] request, bool sandbox, Entities.Request.ReqType reqType) {
            var webRequest = CreateWebRequest(request, sandbox, reqType);
            return GetResponseStream(webRequest);
        }

        private WebRequest CreateWebRequest(byte[] request, bool sandbox, Entities.Request.ReqType reqType) {
            var webRequest = WebRequest.Create(ComposeUrl(sandbox, reqType));
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            byte[] data = request;
            webRequest.ContentLength = data.Length;

            // var dump = System.Text.Encoding.UTF8.GetString(data);
            // Console.WriteLine("Processing.Com Request: {0}", dump);

            var httpWebRequest = webRequest as HttpWebRequest;
            if (httpWebRequest != null) {
                httpWebRequest.UserAgent = String.Format("Fibonatix.CommDoo.Kalixa {0}", this.GetType().Assembly.GetName().Version.ToString());
                httpWebRequest.KeepAlive = false;
            }

            using (var requestStream = webRequest.GetRequestStream()) {
                requestStream.Write(data, 0, data.Length);
            }

            return webRequest;
        }

        private string ComposeUrl(bool sandbox, Entities.Request.ReqType reqType) {
            string url = null;
            switch(reqType) {
                case Entities.Request.ReqType.ACSURL_REQ:
                    url = String.Format("{0}", sandbox ? threeDACSSandBoxURL : threeDACSProductURL);
                    break;
                case Entities.Request.ReqType.PARES_REQ:
                    url = String.Format("{0}", sandbox ? threeDPaResSandBoxURL : threeDPaResProductURL);
                    break;
                case Entities.Request.ReqType.HTREM_REQ:
                default:
                    url = String.Format("{0}", sandbox ? baseSandBoxURL : baseProductURL);
                    break;
            }
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
