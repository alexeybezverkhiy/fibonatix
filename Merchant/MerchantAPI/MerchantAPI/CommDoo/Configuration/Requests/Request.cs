using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace MerchantAPI.CommDoo.Configuration.Requests
{
    public abstract class Request
    {
        public abstract string executeRequest();
        
        public string getXml()
        {
            XmlSerializer formatter = new XmlSerializer(this.GetType());
            StringWriter writer = new Utf8StringWriter();
            formatter.Serialize(writer, this);
            var serializedValue = writer.ToString();
            return serializedValue;
        }

        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        internal string sendRequest(string url)
        {
            string xmlReq = getXml();
            var ret = ProcessRequest(url, System.Text.Encoding.UTF8.GetBytes("xml=" + HttpUtility.UrlEncode(xmlReq)));
            return System.Text.Encoding.UTF8.GetString(ret.ToArray());
        }

        internal MemoryStream ProcessRequest(string url, byte[] request)
        {
            var webRequest = CreateWebRequest(url, request);
            return GetResponseStream(webRequest);
        }

        private WebRequest CreateWebRequest(string url, byte[] request)
        {
            var webRequest = WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            byte[] data = request;
            webRequest.ContentLength = data.Length;

            var httpWebRequest = webRequest as HttpWebRequest;
            if (httpWebRequest != null)
            {
                httpWebRequest.UserAgent = String.Format("Fibonatix.CommDoo.WebGate {0}", this.GetType().Assembly.GetName().Version.ToString());
                httpWebRequest.KeepAlive = false;
            }

            using (var requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
            }

            return webRequest;
        }

        private MemoryStream GetResponseStream(WebRequest webRequest)
        {
            WebResponse webResponse = null;
            try
            {
                webResponse = webRequest.GetResponse();
                return Copy(webResponse.GetResponseStream());
            }
            catch (WebException ex)
            {
                Stream responseStream;
                if (TryGetResponseDataFromWebException(ex, out responseStream))
                {
                    return Copy(responseStream);
                }
                throw ex;
            }
            finally
            {
                if (webResponse != null)
                {
                    webResponse.Close();
                }
            }
        }

        private MemoryStream Copy(Stream stream)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream;
        }

        private bool TryGetResponseDataFromWebException(WebException webException, out Stream responseData)
        {
            responseData = null;

            var response = webException.Response as HttpWebResponse;
            if (response == null)
            {
                return false;
            }

            // Unprocessable Entity (The request was well-formed but was unable to be followed due to semantic errors.)
            if (response.StatusCode == (HttpStatusCode)422)
            {
                responseData = response.GetResponseStream();
                response.Close();
                return true;
            }

            return false;
        }
    }
}