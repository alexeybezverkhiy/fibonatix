using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;
using System.Collections.Specialized;

namespace Fibonatix.CommDoo.ProcessingCom.Entities
{
    class Request
    {
        public enum ReqType
        {
            HTREM_REQ = 0,
            ACSURL_REQ = 1,
            PARES_REQ = 2,
        };

        private NameValueCollection requestParameters = new NameValueCollection();
        public bool inSandBox { get; set; }
        public ReqType reqType { get; set; }

        public string getRequestString() {
            var parameters = new StringBuilder();

            try {
                foreach (string key in requestParameters.Keys) {
                    parameters.AppendFormat("{0}={1}&",
                        HttpUtility.UrlEncode(key),
                        HttpUtility.UrlEncode(requestParameters[key]));
                }
                if (requestParameters.Count > 0)
                    parameters.Length -= 1;
            } catch {
            }

            return parameters.ToString();
        }

        public void setParameter(string key, string value) {
            requestParameters.Remove(key);
            requestParameters.Set(key, value);
        }
        public string getParameter(string key) {
            return requestParameters.Get(key);
        }
    }
}
