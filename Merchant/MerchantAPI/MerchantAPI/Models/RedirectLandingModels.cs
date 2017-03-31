using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MerchantAPI.Helpers;



namespace MerchantAPI.Models
{
    public class RedirectResponseModel : BaseFibonatixModel
    {
        public RedirectResponseModel(string clientOrderId, bool succ = true) {
            merchant_order = clientOrderId;
            success = succ;
        }

        public bool success { get; set; }
        public string status { get; set; }
        public string orderid { get; set; }
        public string merchant_order { get; set; }
        public string error_message { get; set; }
        public string descriptor { get; set; }

        protected override StringBuilder FillHashContent(StringBuilder builder, int endpoint, string merchantControlKey) {
            return builder
                .Append(status)
                .Append(string.IsNullOrEmpty(orderid) ? string.Empty : orderid)
                .Append(string.IsNullOrEmpty(merchant_order) ? string.Empty : merchant_order)
                .Append(merchantControlKey);
        }

        public bool IsSucc() {
            return success;
        }

        public string ToHttpResponse(string redirectUrl) {
            if (IsSucc()) {

                return
@"<!DOCTYPE html>
<html>
<head>
    <meta http-equiv=""content-type"" content=""text/html; charset=UTF-8"">
    <title>Redirecting...</title>
    <script type=""text/javascript"" language=""javascript"">
         function makeSubmit() {
            document.returnform.submit();
         }
    </script>
</head>
<body onLoad = ""makeSubmit()"">
    <form name=""returnform"" id=""returnform"" action=""" + redirectUrl + @""" method=""POST"">
        <input type=""hidden"" name=""status"" value=""" + status + @""" >
        <input type=""hidden"" name=""orderid"" value=""" + orderid + @""" >
        <input type=""hidden"" name=""merchant-order"" value=""" + merchant_order + @""" >
        <input type=""hidden"" name=""client-orderid"" value=""" + client_orderid + @""" >
        <input type=""hidden"" name=""control"" value=""" + control + @""" >
        <input type=""hidden"" name=""descriptor"" value=""" + HttpUtility.UrlEncode(descriptor != null ? descriptor : "") + @""" >
        <noscript>
            <input type=""submit"" name=""submit"" value=""Press this button to continue"" />
        </noscript>
    </form>
</body>";
            }

            return 
@"<!DOCTYPE html>
<html>
<head>
    <meta http-equiv=""content-type"" content=""text/html; charset=UTF-8"">
    <title>Redirecting...</title>
    <script type=""text/javascript"" language=""javascript"">
         function makeSubmit() {
            document.returnform.submit();
         }
    </script>
</head>
<body onLoad = ""makeSubmit()"">
    <form name=""returnform"" id=""returnform"" action=""" + redirectUrl + @""" method=""POST"">
        <input type=""hidden"" name=""paynet-order-id"" value=""" + orderid + @""" >
        <input type=""hidden"" name=""merchant-order-id"" value=""" + merchant_order + @""" >
        <input type=""hidden"" name=""error-message"" value=""" + HttpUtility.UrlEncode(error_message) + @""" >
        <input type=""hidden"" name=""error-code"" value=""" + "100" +@""" >
        <input type=""hidden"" name=""control"" value=""" + control + @""" >
        <noscript>
            <input type=""submit"" name=""submit"" value=""Press this button to continue"" />
        </noscript>
    </form>
</body>";
        }
    }
}