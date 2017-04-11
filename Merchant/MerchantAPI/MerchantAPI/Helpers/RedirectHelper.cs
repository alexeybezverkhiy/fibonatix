using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace MerchantAPI.Helpers
{
    public class RedirectHelper
    {
        public static readonly string RedirectTemplate =
@"<!DOCTYPE html><html><head>
<meta http-equiv=""content-type"" content=""text/html; charset=UTF-8"">
<title>Redirecting...</title>
<script type=""text/javascript"" language=""javascript"">function makeSubmit(){document.returnform.submit();}</script>
</head><body onLoad=""makeSubmit()"">
<form name=""returnform"" action=""{0}"" method=""POST"">
{1}
<noscript><input type=""submit"" name=""submit"" value=""Press this button to continue""/></noscript>
</form></body></html>";

        public static readonly string HiddenInputTemplate = @"<input type=""hidden"" name=""{0}"" value=""{1}"">\n";


        public static string CreateRedirectHtml(string redirectTemplate, string redirectUrl)
        {
            string[] splittedUrl = redirectUrl.Split('?');
            string redirectHTML = redirectTemplate.Replace("{0}", splittedUrl[0]);
            StringBuilder sb = new StringBuilder(1024);
            if (splittedUrl.Length > 1)
            {
                NameValueCollection redirectParams = ControllerHelper.DeserializeHttpParameters(splittedUrl[1]);
                foreach (string name in redirectParams)
                {
                    string value = redirectParams[name];
                    sb.Append(HiddenInputTemplate.Replace("{0}", name).Replace("{1}", value));
                }
            }
            return redirectTemplate.Replace("{1}", sb.ToString());
        }
    }
}