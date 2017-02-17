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
            string response = "";
            byte[] partnerResponse = new byte[0];
            try {
                Data.TransactionData transactionData = Data.TransactionsDataStorage.getTransactionData(model.orderid);

                if (transactionData.transactionType == "sale" ) {

                    SaleRequestModel sale_model = Data.Cache.getSaleRequestData(model.orderid);
                    string redirectURL = Data.Cache.getRedirectUrlForRequest(model.orderid);

                    switch (transactionData.transactionState) {
                        case Data.TransactionData.TransactionState.Started:
                        case Data.TransactionData.TransactionState.Redirected: {
                                if (redirectURL != null || sale_model == null) {
                                    string redirectHTML = redirectTemplate.Replace("{0}", redirectURL);
                                    // Add to cache with key requestParameters['client_orderid'] and data redirectToCommDoo
                                    response =
                                        "type=status-response\n" +
                                        "&status=processing\n" +
                                        "&amount=" + sale_model.amount + "\n" +
                                        "&paynet-order-id=" + model.orderid + "\n" +
                                        "&merchant-order-id=" + model.client_orderid + "\n" +
                                        "&phone=" + sale_model.phone + "\n" +
                                        "&html=" + HttpUtility.UrlEncode(redirectHTML) + "\n" +
                                        "&serial-number=00000000-0000-0000-0000-status-" + model.client_orderid + "\n" +
                                        "&last-four-digits=" + sale_model.credit_card_number.Substring(sale_model.credit_card_number.Length - 4) + "\n" +
                                        "&bin=" + "" + "\n" +
                                        "&card-type=" + "" + "\n" +
                                        "&gate-partial-reversal=" + "" + "\n" +
                                        "&gate-partial-capture=" + "" + "\n" +
                                        "&transaction-type=" + "sale" + "\n" +
                                        "&processor-rrn=" + "" + "\n" +
                                        "&processor-tx-id=" + "" + "\n" +
                                        "&receipt-id=" + "" + "\n" +
                                        "&name=" + HttpUtility.UrlEncode(sale_model.first_name + " " + sale_model.last_name) + "\n" +
                                        "&cardholder-name=" + HttpUtility.UrlEncode(sale_model.card_printed_name) + "\n" +
                                        "&card-exp-month=" + sale_model.expire_month + "\n" +
                                        "&card-exp-year=" + sale_model.expire_year + "\n" +
                                        "&card-hash-id=" + "" + "\n" +
                                        "&email=" + sale_model.email + "\n" +
                                        "&bank-name=" + "" + "\n" +
                                        "&terminal-id=" + "" + "\n" +
                                        "&paynet-processing-date=" + "" + "\n" +
                                        "&approval-code=" + "" + "\n" +
                                        "&order-stage=" + "sale_3d_validating" + "\n" +
                                        "&descriptor=" + sale_model.order_desc + "\n" +
                                        "&by-request-sn=" + model.by_request_sn + "\n";
                                } else {
                                    response =
                                        "type=validation-error\n" +
                                        "&serial-number=00000000-0000-0000-0000-status-" + model.client_orderid + "\n" +
                                        "&error-code=1\n" +
                                        "&error-message=" + HttpUtility.UrlEncode("Error parsing " + model.client_orderid + " as client order ID and " + model.orderid + " as order ID");
                                }
                            }
                            break;
                        case Data.TransactionData.TransactionState.Finished: {
                                if (redirectURL != null || sale_model == null) {
                                    string redirectHTML = redirectTemplate.Replace("{0}", redirectURL);
                                    response =
                                        "type=status-response\n" +
                                        "&status=approved\n" +
                                        "&amount=" + sale_model.amount + "\n" +
                                        "&paynet-order-id=" + model.orderid + "\n" +
                                        "&merchant-order-id=" + model.client_orderid + "\n" +
                                        "&phone=" + sale_model.phone + "\n" +
                                        "&html=" + "" + "\n" +
                                        "&serial-number=00000000-0000-0000-0000-status-" + model.client_orderid + "\n" +
                                        "&last-four-digits=" + sale_model.credit_card_number.Substring(sale_model.credit_card_number.Length - 4) + "\n" +
                                        "&bin=" + "" + "\n" +
                                        "&card-type=" + "" + "\n" +
                                        "&gate-partial-reversal=" + "" + "\n" +
                                        "&gate-partial-capture=" + "" + "\n" +
                                        "&transaction-type=" + "sale" + "\n" +
                                        "&processor-rrn=" + "" + "\n" +
                                        "&processor-tx-id=" + "" + "\n" +
                                        "&receipt-id=" + "" + "\n" +
                                        "&name=" + HttpUtility.UrlEncode(sale_model.first_name + " " + sale_model.last_name) + "\n" +
                                        "&cardholder-name=" + HttpUtility.UrlEncode(sale_model.card_printed_name) + "\n" +
                                        "&card-exp-month=" + sale_model.expire_month + "\n" +
                                        "&card-exp-year=" + sale_model.expire_year + "\n" +
                                        "&card-hash-id=" + "" + "\n" +
                                        "&email=" + sale_model.email + "\n" +
                                        "&bank-name=" + "" + "\n" +
                                        "&terminal-id=" + "" + "\n" +
                                        "&paynet-processing-date=" + "" + "\n" +
                                        "&approval-code=" + "" + "\n" +
                                        "&order-stage=" + "sale_approved" + "\n" +
                                        "&descriptor=" + sale_model.order_desc + "\n" +
                                        "&by-request-sn=" + model.by_request_sn + "\n";
                                } else {
                                    response =
                                        "type=validation-error\n" +
                                        "&serial-number=00000000-0000-0000-0000-status-" + model.client_orderid + "\n" +
                                        "&error-code=1\n" +
                                        "&error-message=" + HttpUtility.UrlEncode("Error parsing " + model.client_orderid + " as client order ID and " + model.orderid + " as order ID");
                                }
                            }
                            break;
                        case Data.TransactionData.TransactionState.Undefined:
                        default: {
                                response =
                                    "type=validation-error\n" +
                                    "&serial-number=00000000-0000-0000-0000-status-" + model.client_orderid + "\n" +
                                    "&error-code=1\n" +
                                    "&error-message=" + HttpUtility.UrlEncode("Undefined state of transaction");
                            }
                            break;
                    }
                }
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