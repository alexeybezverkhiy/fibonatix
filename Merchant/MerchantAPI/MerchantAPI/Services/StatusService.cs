using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using MerchantAPI.Models;
using System.Text;
using MerchantAPI.Data;
using System.Collections.Specialized;

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
                Transaction transactionData = TransactionsDataStorage.FindByTransactionId(model.orderid);

                switch(transactionData.Type) {
                    case TransactionType.Sale:
                        response = PrepareStringResponse(StatusSaleSingleCurrency(transactionData));
                        break;
                    case TransactionType.Capture:
                    case TransactionType.Preauth:
                    case TransactionType.Return:
                    case TransactionType.Verify:
                    case TransactionType.Void:
                    default:
                        break;
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

        private NameValueCollection StatusSaleSingleCurrency(Transaction transactionData) {

            NameValueCollection response = new NameValueCollection();

            SaleRequestModel sale_model = Cache.getSaleRequestData(transactionData.TransactionId);
            string redirectURL = Cache.getRedirectUrlForRequest(transactionData.TransactionId);

            switch (transactionData.State) {
                case TransactionState.Started:
                case TransactionState.Redirected: {
                        if (transactionData.State == TransactionState.Started)
                            TransactionsDataStorage.UpdateTransactionState(transactionData.TransactionId, TransactionState.Created);
                        if (redirectURL != null && sale_model != null) {
                            string redirectHTML = redirectTemplate.Replace("{0}", redirectURL);
                            // Add to cache with key requestParameters['client_orderid'] and data redirectToCommDoo
                            response["type"] = "status-response";
                            response["status"] = "processing";
                            response["amount"] = sale_model.amount;
                            response["paynet-order-id"] = transactionData.TransactionId;
                            response["merchant-order-id"] = transactionData.MerchantTransactionId;
                            response["phone"] = sale_model.phone;
                            response["html"] = HttpUtility.UrlEncode(redirectHTML);
                            response["serial-number"] = Guid.NewGuid().ToString();
                            response["last-four-digits"] = sale_model.credit_card_number.Substring(sale_model.credit_card_number.Length - 4);
                            response["bin"] = "";
                            response["card-type"] = "";
                            response["gate-partial-reversal"] = "";
                            response["gate-partial-capture"] = "";
                            response["transaction-type"] = "sale";
                            response["processor-rrn"] = "";
                            response["processor-tx-id"] = "";
                            response["receipt-id"] = "";
                            response["name"] = HttpUtility.UrlEncode(sale_model.first_name + " " + sale_model.last_name);
                            response["cardholder-name"] = HttpUtility.UrlEncode(sale_model.card_printed_name);
                            response["card-exp-month"] = sale_model.expire_month.ToString();
                            response["card-exp-year"] = sale_model.expire_year.ToString();
                            response["card-hash-id"] = "";
                            response["email"] = sale_model.email;
                            response["bank-name"] = "";
                            response["terminal-id"] = "";
                            response["paynet-processing-date"] = "";
                            response["approval-code"] = "";
                            response["order-stage"] = "sale_3d_validating";
                            response["descriptor"] = sale_model.order_desc;
                            response["by-request-sn"] = transactionData.SerialNumber;
                            response = CalculateHashForSaleStatus(response);
                        } else {
                            TransactionsDataStorage.UpdateTransactionState(transactionData.TransactionId, TransactionState.Finished);
                            TransactionsDataStorage.UpdateTransactionStatus(transactionData.TransactionId, TransactionStatus.Error);
                            response["type"] = "validation-error";
                            response["serial-number"] = Guid.NewGuid().ToString();
                            response["error-code"] = "1";
                            response["error-message"] = HttpUtility.UrlEncode("Error parsing " + transactionData.MerchantTransactionId + " as client order ID and " + transactionData.TransactionId + " as order ID");
                        }
                    }
                    break;
                case TransactionState.Finished: {
                        switch (transactionData.Status) {
                            case TransactionStatus.Approved:
                                response["type"] = "status-response";
                                response["status"] = "approved";
                                response["amount"] = sale_model.amount;
                                response["paynet-order-id"] = transactionData.TransactionId;
                                response["merchant-order-id"] = transactionData.MerchantTransactionId;
                                response["phone"] = sale_model.phone;
                                response["html"] = "";
                                response["serial-number"] = Guid.NewGuid().ToString();
                                response["last-four-digits"] = sale_model.credit_card_number.Substring(sale_model.credit_card_number.Length - 4);
                                response["bin"] = "";
                                response["card-type"] = "";
                                response["gate-partial-reversal"] = "";
                                response["gate-partial-capture"] = "";
                                response["transaction-type"] = "sale";
                                response["processor-rrn"] = "";
                                response["processor-tx-id"] = "";
                                response["receipt-id"] = "";
                                response["name"] = HttpUtility.UrlEncode(sale_model.first_name + " " + sale_model.last_name);
                                response["cardholder-name"] = HttpUtility.UrlEncode(sale_model.card_printed_name);
                                response["card-exp-month"] = sale_model.expire_month.ToString();
                                response["card-exp-year"] = sale_model.expire_year.ToString();
                                response["card-hash-id"] = "";
                                response["email"] = sale_model.email;
                                response["bank-name"] = "";
                                response["terminal-id"] = "";
                                response["paynet-processing-date"] = "";
                                response["approval-code"] = "";
                                response["order-stage"] = "sale_approved";
                                response["descriptor"] = sale_model.order_desc;
                                response["by-request-sn"] = transactionData.SerialNumber;
                                response = CalculateHashForSaleStatus(response);
                                break;
                            case TransactionStatus.Declined:
                                response["type"] = "status-response";
                                response["status"] = "declined";
                                response["amount"] = sale_model.amount;
                                response["paynet-order-id"] = transactionData.TransactionId;
                                response["merchant-order-id"] = transactionData.MerchantTransactionId;
                                response["phone"] = sale_model.phone;
                                response["html"] = "";
                                response["serial-number"] = Guid.NewGuid().ToString();
                                response["last-four-digits"] = sale_model.credit_card_number.Substring(sale_model.credit_card_number.Length - 4);
                                response["bin"] = "";
                                response["card-type"] = "";
                                response["gate-partial-reversal"] = "";
                                response["gate-partial-capture"] = "";
                                response["transaction-type"] = "sale";
                                response["processor-rrn"] = "";
                                response["processor-tx-id"] = "";
                                response["receipt-id"] = "";
                                response["name"] = HttpUtility.UrlEncode(sale_model.first_name + " " + sale_model.last_name);
                                response["cardholder-name"] = HttpUtility.UrlEncode(sale_model.card_printed_name);
                                response["card-exp-month"] = sale_model.expire_month.ToString();
                                response["card-exp-year"] = sale_model.expire_year.ToString();
                                response["card-hash-id"] = "";
                                response["email"] = sale_model.email;
                                response["bank-name"] = "";
                                response["terminal-id"] = "";
                                response["paynet-processing-date"] = "";
                                response["approval-code"] = "";
                                response["order-stage"] = "sale_declined";
                                response["descriptor"] = sale_model.order_desc;
                                response["by-request-sn"] = transactionData.SerialNumber;
                                response = CalculateHashForSaleStatus(response);
                                break;
                            case TransactionStatus.Error:
                            case TransactionStatus.Undefined:
                            default:
                                response["type"] = "validation-error";
                                response["serial-number"] = Guid.NewGuid().ToString();
                                response["error-code"] = "1";
                                response["error-message"] = HttpUtility.UrlEncode("Error parsing " + transactionData.MerchantTransactionId + " as client order ID and " + transactionData.TransactionId + " as order ID");
                                break;
                        }
                    }
                    break;
                case TransactionState.Created:
                default: {
                        response["type"] = "validation-error";
                        response["serial-number"] = Guid.NewGuid().ToString();
                        response["error-code"] = "1";
                        response["error-message"] = HttpUtility.UrlEncode("Undefined state of transaction");
                    }
                    break;
            }
            return response;
        }

        private NameValueCollection CalculateHashForSaleStatus(NameValueCollection response) {
            return response;
        }

        private string PrepareStringResponse(NameValueCollection response) {
            var stringResponse = new StringBuilder();
            foreach (string key in response.Keys) {
                stringResponse.AppendFormat("{0}={1}&\n",
                    HttpUtility.UrlEncode(key),
                    HttpUtility.UrlEncode(response[key]));
            }
            if (response.Count > 0)
                stringResponse.Length -= 2;

            return stringResponse.ToString();
        }
    }
}