using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using MerchantAPI.Models;
using System.Text;
using MerchantAPI.Data;
using System.Collections.Specialized;
using MerchantAPI.Helpers;
using Microsoft.Ajax.Utilities;

namespace MerchantAPI.Services
{
    public class StatusService
    {
        private readonly string redirectTemplate = 
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
    <form name=""returnform"" action=""{0}"" method=""POST"">
    <noscript>
        <input type=""submit"" name=""submit"" value=""Press this button to continue"" />
    </noscript>
</form>
</body>
</html>";


        public ServiceTransitionResult StatusSingleCurrency(int endpointId, StatusRequestModel model, string merchantControlKey)
        {
            Transaction transactionData = null;
            try
            {
                transactionData = TransactionsDataStorage.FindByTransactionId(model.orderid);
                if (transactionData == null)
                {
                    return new ServiceTransitionResult(HttpStatusCode.BadRequest,
                        $"ERROR: Unknown 'orderid' [{model.orderid}] for Status request\n");
                }

                NameValueCollection response;
                switch (transactionData.Type) {
                    case TransactionType.Sale:
                        response = StatusSaleSingleCurrency(transactionData, merchantControlKey);
                        break;
                    case TransactionType.Preauth:
                        response = StatusPreAuthSingleCurrency(transactionData, merchantControlKey);
                        break;
                    case TransactionType.Capture:
                        response = StatusCaptureSingleCurrency(transactionData, merchantControlKey);
                        break;
                    case TransactionType.Return:
                    case TransactionType.Verify:
                    case TransactionType.Void:
                    default:
                        throw new ArgumentException($"Unsupported transaction type [{transactionData.Type}] for Status request");
                }

                string responseStr = PrepareStringResponse(response);
                return new ServiceTransitionResult(HttpStatusCode.OK,
                    responseStr + "\n");
            }
            catch (Exception e)
            {
                if (transactionData != null)
                {
                    TransactionsDataStorage.Store(transactionData, e);
                }
                return new ServiceTransitionResult(HttpStatusCode.InternalServerError,
                    $"EXCP: Processing Status for [orderid={model.orderid}] failed\n");
            }
            finally { }
        }

        private NameValueCollection StatusSaleSingleCurrency(Transaction transactionData, string merchantControlKey) {

            NameValueCollection response = new NameValueCollection();

            NameValueCollection sale_model = ControllerHelper.DeserializeHttpParameters(transactionData.ReferenceQuery);
            string redirectURL = transactionData.RedirectUri;

            switch (transactionData.State) {
                case TransactionState.Started:
                case TransactionState.Redirected:
                    if (transactionData.State == TransactionState.Started)
                    {
                        TransactionsDataStorage.UpdateTransactionState(transactionData.TransactionId, TransactionState.Redirected);
                    }
                    if (redirectURL != null && sale_model != null) {
                        string redirectHTML = redirectTemplate.Replace("{0}", redirectURL);
                        // Add to cache with key requestParameters['client_orderid'] and data redirectToCommDoo
                        response["type"] = "status-response";
                        response["status"] = "processing";
                        response["amount"] = sale_model["amount"];
                        response["paynet-order-id"] = transactionData.TransactionId;
                        response["merchant-order-id"] = transactionData.MerchantTransactionId;
                        response["phone"] = sale_model["phone"];
                        response["html"] = HttpUtility.UrlEncode(redirectHTML);
                        response["serial-number"] = Guid.NewGuid().ToString();
                        response["last-four-digits"] = ControllerHelper.LastFourDigits(sale_model["credit_card_number"]);
                        response["bin"] = "";
                        response["card-type"] = "";
                        response["gate-partial-reversal"] = "";
                        response["gate-partial-capture"] = "";
                        response["transaction-type"] = "sale";
                        response["processor-rrn"] = "";
                        response["processor-tx-id"] = "";
                        response["receipt-id"] = "";
                        response["name"] = HttpUtility.UrlEncode(sale_model["first_name"] + " " + sale_model["last_name"]);
                        response["cardholder-name"] = HttpUtility.UrlEncode(sale_model["card_printed_name"]);
                        response["card-exp-month"] = sale_model["expire_month"];
                        response["card-exp-year"] = sale_model["expire_year"];
                        response["card-hash-id"] = "";
                        response["email"] = sale_model["email"];
                        response["bank-name"] = "";
                        response["terminal-id"] = "";
                        response["paynet-processing-date"] = "";
                        response["approval-code"] = "";
                        response["order-stage"] = "sale_3d_validating";
                        response["descriptor"] = sale_model["order_desc"];
                        response["by-request-sn"] = transactionData.SerialNumber;
                        response["control"] = CalculateHash(response, merchantControlKey);
                    } else {
                        TransactionsDataStorage.UpdateTransaction(transactionData.TransactionId, TransactionState.Finished, TransactionStatus.Error);
                        response["type"] = "validation-error";
                        response["serial-number"] = Guid.NewGuid().ToString();
                        response["error-code"] = "1";
                        response["error-message"] = HttpUtility.UrlEncode($"Unknown 'orderid' [{transactionData.TransactionId}] to find stored transaction data");
                    }
                    break;
                case TransactionState.Finished: {
                        switch (transactionData.Status) {
                            case TransactionStatus.Approved:
                                response["type"] = "status-response";
                                response["status"] = "approved";
                                response["amount"] = sale_model["amount"];
                                response["paynet-order-id"] = transactionData.TransactionId;
                                response["merchant-order-id"] = transactionData.MerchantTransactionId;
                                response["phone"] = sale_model["phone"];
                                response["html"] = "";
                                response["serial-number"] = Guid.NewGuid().ToString();
                                response["last-four-digits"] = ControllerHelper.LastFourDigits(sale_model["credit_card_number"]);
                                response["bin"] = "";
                                response["card-type"] = "";
                                response["gate-partial-reversal"] = "";
                                response["gate-partial-capture"] = "";
                                response["transaction-type"] = "sale";
                                response["processor-rrn"] = "";
                                response["processor-tx-id"] = "";
                                response["receipt-id"] = "";
                                response["name"] = HttpUtility.UrlEncode(sale_model["first_name"] + " " + sale_model["last_name"]);
                                response["cardholder-name"] = HttpUtility.UrlEncode(sale_model["card_printed_name"]);
                                response["card-exp-month"] = sale_model["expire_month"];
                                response["card-exp-year"] = sale_model["expire_year"];
                                response["card-hash-id"] = "";
                                response["email"] = sale_model["email"];
                                response["bank-name"] = "";
                                response["terminal-id"] = "";
                                response["paynet-processing-date"] = "";
                                response["approval-code"] = "";
                                response["order-stage"] = "sale_approved";
                                response["descriptor"] = sale_model["order_desc"];
                                response["by-request-sn"] = transactionData.SerialNumber;
                                response["control"] = CalculateHash(response, merchantControlKey);
                                break;
                            case TransactionStatus.Declined:
                                response["type"] = "status-response";
                                response["status"] = "declined";
                                response["amount"] = sale_model["amount"];
                                response["paynet-order-id"] = transactionData.TransactionId;
                                response["merchant-order-id"] = transactionData.MerchantTransactionId;
                                response["phone"] = sale_model["phone"];
                                response["html"] = "";
                                response["serial-number"] = Guid.NewGuid().ToString();
                                response["last-four-digits"] =  ControllerHelper.LastFourDigits(sale_model["credit_card_number"]);
                                response["bin"] = "";
                                response["card-type"] = "";
                                response["gate-partial-reversal"] = "";
                                response["gate-partial-capture"] = "";
                                response["transaction-type"] = "sale";
                                response["processor-rrn"] = "";
                                response["processor-tx-id"] = "";
                                response["receipt-id"] = "";
                                response["name"] = HttpUtility.UrlEncode(sale_model["first_name"] + " " + sale_model["last_name"]);
                                response["cardholder-name"] = HttpUtility.UrlEncode(sale_model["card_printed_name"]);
                                response["card-exp-month"] = sale_model["expire_month"];
                                response["card-exp-year"] = sale_model["expire_year"];
                                response["card-hash-id"] = "";
                                response["email"] = sale_model["email"];
                                response["bank-name"] = "";
                                response["terminal-id"] = "";
                                response["paynet-processing-date"] = "";
                                response["approval-code"] = "";
                                response["order-stage"] = "sale_failed";
                                response["descriptor"] = sale_model["order_desc"];
                                response["by-request-sn"] = transactionData.SerialNumber;
                                response["control"] = CalculateHash(response, merchantControlKey);
                                break;
                            case TransactionStatus.Error:
                            case TransactionStatus.Undefined:
                            default:
                                response["type"] = "validation-error";
                                response["serial-number"] = Guid.NewGuid().ToString();
                                response["error-code"] = "1";
                                response["error-message"] = HttpUtility.UrlEncode($"Transaction [orderid={transactionData.TransactionId}] has State [{transactionData.State}] but its Status is [{transactionData.Status}]");
                                break;
                        }
                    }
                    break;
                case TransactionState.Created:
                default:
                    response["type"] = "validation-error";
                    response["serial-number"] = Guid.NewGuid().ToString();
                    response["error-code"] = "1";
                    response["error-message"] = HttpUtility.UrlEncode("Undefined state of transaction");
                    break;
            }
            return response;
        }

        private static string[] STATUS_HASH_KEY_SEQUENSE =
        {
            "login",
            "client_orderid",
            "orderid",
        };

        private string CalculateHash(NameValueCollection response, string salt)
        {
            return HashHelper.SHA1(HashHelper
                .AssemblyHashContent(STATUS_HASH_KEY_SEQUENSE, response, salt)
                .Append(salt)
                .ToString());
        }

        private NameValueCollection StatusPreAuthSingleCurrency(Transaction transactionData, string merchantControlKey) {

            NameValueCollection response = new NameValueCollection();

            NameValueCollection preauth_model = ControllerHelper.DeserializeHttpParameters(transactionData.ReferenceQuery);
            string redirectURL = transactionData.RedirectUri;

            switch (transactionData.State) {
                case TransactionState.Started:
                case TransactionState.Redirected:
                    if (transactionData.State == TransactionState.Started)
                    {
                        TransactionsDataStorage.UpdateTransactionState(transactionData.TransactionId,
                            TransactionState.Redirected);
                    }
                    if (redirectURL != null && preauth_model != null) {
                        string redirectHTML = redirectTemplate.Replace("{0}", redirectURL);
                        // Add to cache with key requestParameters['client_orderid'] and data redirectToCommDoo
                        response["type"] = "status-response";
                        response["status"] = "processing";
                        response["amount"] = preauth_model["amount"];
                        response["paynet-order-id"] = transactionData.TransactionId;
                        response["merchant-order-id"] = transactionData.MerchantTransactionId;
                        response["phone"] = preauth_model["phone"];
                        response["html"] = HttpUtility.UrlEncode(redirectHTML);
                        response["serial-number"] = Guid.NewGuid().ToString();
                        response["last-four-digits"] = ControllerHelper.LastFourDigits(preauth_model["credit_card_number"]);
                        response["bin"] = "";
                        response["card-type"] = "";
                        response["gate-partial-reversal"] = "";
                        response["gate-partial-capture"] = "";
                        response["transaction-type"] = "preauth";
                        response["processor-rrn"] = "";
                        response["processor-tx-id"] = "";
                        response["receipt-id"] = "";
                        response["name"] = HttpUtility.UrlEncode(preauth_model["first_name"] + " " + preauth_model["last_name"]);
                        response["cardholder-name"] = HttpUtility.UrlEncode(preauth_model["card_printed_name"]);
                        response["card-exp-month"] = preauth_model["expire_month"];
                        response["card-exp-year"] = preauth_model["expire_year"];
                        response["card-hash-id"] = "";
                        response["email"] = preauth_model["email"];
                        response["bank-name"] = "";
                        response["terminal-id"] = "";
                        response["paynet-processing-date"] = "";
                        response["approval-code"] = "";
                        response["order-stage"] = "auth_3d_validating";
                        response["descriptor"] = preauth_model["order_desc"];
                        response["by-request-sn"] = transactionData.SerialNumber;
                        response["control"] = CalculateHash(response, merchantControlKey);
                    } else {
                        TransactionsDataStorage.UpdateTransaction(transactionData.TransactionId, TransactionState.Finished, TransactionStatus.Error);
                        response["type"] = "validation-error";
                        response["serial-number"] = Guid.NewGuid().ToString();
                        response["error-code"] = "1";
                        response["error-message"] = HttpUtility.UrlEncode($"Unknown 'orderid' [{transactionData.TransactionId}] to find stored transaction data");
                    }
                    break;
                case TransactionState.Finished: 
                    switch (transactionData.Status) {
                        case TransactionStatus.Approved:
                            response["type"] = "status-response";
                            response["status"] = "approved";
                            response["amount"] = preauth_model["amount"];
                            response["paynet-order-id"] = transactionData.TransactionId;
                            response["merchant-order-id"] = transactionData.MerchantTransactionId;
                            response["phone"] = preauth_model["phone"];
                            response["html"] = "";
                            response["serial-number"] = Guid.NewGuid().ToString();
                            response["last-four-digits"] = ControllerHelper.LastFourDigits(preauth_model["credit_card_number"]);
                            response["bin"] = "";
                            response["card-type"] = "";
                            response["gate-partial-reversal"] = "";
                            response["gate-partial-capture"] = "";
                            response["transaction-type"] = "preauth";
                            response["processor-rrn"] = "";
                            response["processor-tx-id"] = "";
                            response["receipt-id"] = "";
                            response["name"] = HttpUtility.UrlEncode(preauth_model["first_name"] + " " + preauth_model["last_name"]);
                            response["cardholder-name"] = HttpUtility.UrlEncode(preauth_model["card_printed_name"]);
                            response["card-exp-month"] = preauth_model["expire_month"];
                            response["card-exp-year"] = preauth_model["expire_year"];
                            response["card-hash-id"] = "";
                            response["email"] = preauth_model["email"];
                            response["bank-name"] = "";
                            response["terminal-id"] = "";
                            response["paynet-processing-date"] = "";
                            response["approval-code"] = "";
                            response["order-stage"] = "auth_approved";
                            response["descriptor"] = preauth_model["order_desc"];
                            response["by-request-sn"] = transactionData.SerialNumber;
                            response["control"] = CalculateHash(response, merchantControlKey);
                            break;
                        case TransactionStatus.Declined:
                            response["type"] = "status-response";
                            response["status"] = "declined";
                            response["amount"] = preauth_model["amount"];
                            response["paynet-order-id"] = transactionData.TransactionId;
                            response["merchant-order-id"] = transactionData.MerchantTransactionId;
                            response["phone"] = preauth_model["phone"];
                            response["html"] = "";
                            response["serial-number"] = Guid.NewGuid().ToString();
                            response["last-four-digits"] = ControllerHelper.LastFourDigits(preauth_model["credit_card_number"]);
                            response["bin"] = "";
                            response["card-type"] = "";
                            response["gate-partial-reversal"] = "";
                            response["gate-partial-capture"] = "";
                            response["transaction-type"] = "preauth";
                            response["processor-rrn"] = "";
                            response["processor-tx-id"] = "";
                            response["receipt-id"] = "";
                            response["name"] = HttpUtility.UrlEncode(preauth_model["first_name"] + " " + preauth_model["last_name"]);
                            response["cardholder-name"] = HttpUtility.UrlEncode(preauth_model["card_printed_name"]);
                            response["card-exp-month"] = preauth_model["expire_month"];
                            response["card-exp-year"] = preauth_model["expire_year"];
                            response["card-hash-id"] = "";
                            response["email"] = preauth_model["email"];
                            response["bank-name"] = "";
                            response["terminal-id"] = "";
                            response["paynet-processing-date"] = "";
                            response["approval-code"] = "";
                            response["order-stage"] = "auth_failed";
                            response["descriptor"] = preauth_model["order_desc"];
                            response["by-request-sn"] = transactionData.SerialNumber;
                            response["control"] = CalculateHash(response, merchantControlKey);
                            break;
                        case TransactionStatus.Error:
                        case TransactionStatus.Undefined:
                        default:
                            response["type"] = "validation-error";
                            response["serial-number"] = Guid.NewGuid().ToString();
                            response["error-code"] = "1";
                            response["error-message"] = HttpUtility.UrlEncode($"Transaction [orderid={transactionData.TransactionId}] has State [{transactionData.State}] but its Status is [{transactionData.Status}]");
                            break;
                    }
                    break;
                case TransactionState.Created:
                default: 
                    response["type"] = "validation-error";
                    response["serial-number"] = Guid.NewGuid().ToString();
                    response["error-code"] = "1";
                    response["error-message"] = HttpUtility.UrlEncode("Undefined state of transaction");
                    break;
            }
            return response;
        }

        private NameValueCollection StatusCaptureSingleCurrency(Transaction transactionData, string merchantControlKey)
        {
            NameValueCollection response = new NameValueCollection();

            string rawPreAuthModel = RestorePreAuthQuery(transactionData.TransactionId);
            NameValueCollection preauth_model = ControllerHelper.DeserializeHttpParameters(rawPreAuthModel);
            NameValueCollection capture_model = ControllerHelper.DeserializeHttpParameters(transactionData.ReferenceQuery);
            CommDoo.BackEnd.Responses.Response backResponse = Cache.getBackendResponseData(transactionData.TransactionId);

            if (preauth_model.Count == 0)
            {
                // todo - 
                // throw new ...
            }
            switch (transactionData.State) {
                case TransactionState.Started:
                case TransactionState.Redirected: {
                        if (transactionData.State != TransactionState.Started)
                        {
                            TransactionsDataStorage.UpdateTransactionState(transactionData.TransactionId, TransactionState.Started);
                        }
                        if (capture_model != null) {
                            // Add to cache with key requestParameters['client_orderid'] and data redirectToCommDoo
                            response["type"] = "status-response";
                            response["status"] = "processing";
                            response["amount"] = capture_model["amount"];
                            response["paynet-order-id"] = transactionData.TransactionId;
                            response["merchant-order-id"] = transactionData.MerchantTransactionId;
                            response["phone"] = preauth_model["phone"];
                            response["html"] = "";
                            response["serial-number"] = Guid.NewGuid().ToString();
                            response["last-four-digits"] = ControllerHelper.LastFourDigits(preauth_model["credit_card_number"]);
                            response["bin"] = "";
                            response["card-type"] = "";
                            response["gate-partial-reversal"] = "";
                            response["gate-partial-capture"] = "";
                            response["transaction-type"] = "capture";
                            response["processor-rrn"] = "";
                            response["processor-tx-id"] = "";
                            response["receipt-id"] = "";
                            response["name"] = HttpUtility.UrlEncode(preauth_model["first_name"] + " " + preauth_model["last_name"]);
                            response["cardholder-name"] = HttpUtility.UrlEncode(preauth_model["card_printed_name"]);
                            response["card-exp-month"] = preauth_model["expire_month"];
                            response["card-exp-year"] = preauth_model["expire_year"];
                            response["card-hash-id"] = "";
                            response["email"] = preauth_model["email"];
                            response["bank-name"] = "";
                            response["terminal-id"] = "";
                            response["paynet-processing-date"] = "";
                            response["approval-code"] = "";
                            response["order-stage"] = "capture_validating";
                            response["descriptor"] = preauth_model["order_desc"];
                            response["by-request-sn"] = transactionData.SerialNumber;
                            response["control"] = CalculateHash(response, merchantControlKey);
                        } else {
                            TransactionsDataStorage.UpdateTransaction(transactionData.TransactionId, TransactionState.Finished, TransactionStatus.Error);
                            response["type"] = "validation-error";
                            response["serial-number"] = Guid.NewGuid().ToString();
                            response["error-code"] = "1";
                            response["error-message"] = HttpUtility.UrlEncode($"Unknown 'orderid' [{transactionData.TransactionId}] to find stored transaction data");
                        }
                    }
                    break;
                case TransactionState.Finished: {
                        switch (transactionData.Status) {
                            case TransactionStatus.Approved:
                                response["type"] = "status-response";
                                response["status"] = "approved";
                                response["amount"] = capture_model["amount"];
                                response["paynet-order-id"] = transactionData.TransactionId;
                                response["merchant-order-id"] = transactionData.MerchantTransactionId;
                                response["phone"] = preauth_model["phone"];
                                response["html"] = "";
                                response["serial-number"] = Guid.NewGuid().ToString();
                                response["last-four-digits"] = ControllerHelper.LastFourDigits(preauth_model["credit_card_number"]);
                                response["bin"] = "";
                                response["card-type"] = "";
                                response["gate-partial-reversal"] = "";
                                response["gate-partial-capture"] = "";
                                response["transaction-type"] = "capture";
                                response["processor-rrn"] = "";
                                response["processor-tx-id"] = "";
                                response["receipt-id"] = "";
                                response["name"] = HttpUtility.UrlEncode(preauth_model["first_name"] + " " + preauth_model["last_name"]);
                                response["cardholder-name"] = HttpUtility.UrlEncode(preauth_model["card_printed_name"]);
                                response["card-exp-month"] = preauth_model["expire_month"];
                                response["card-exp-year"] = preauth_model["expire_year"];
                                response["card-hash-id"] = "";
                                response["email"] = preauth_model["email"];
                                response["bank-name"] = "";
                                response["terminal-id"] = "";
                                response["paynet-processing-date"] = "";
                                response["approval-code"] = "";
                                response["order-stage"] = "capture_approved";
                                response["descriptor"] = preauth_model["order_desc"];
                                response["by-request-sn"] = transactionData.SerialNumber;
                                response["control"] = CalculateHash(response, merchantControlKey);
                                break;
                            case TransactionStatus.Declined:
                                response["type"] = "status-response";
                                response["status"] = "declined";
                                response["amount"] = capture_model["amount"];
                                response["paynet-order-id"] = transactionData.TransactionId;
                                response["merchant-order-id"] = transactionData.MerchantTransactionId;
                                response["phone"] = preauth_model["phone"];
                                response["html"] = "";
                                response["serial-number"] = Guid.NewGuid().ToString();
                                response["last-four-digits"] = ControllerHelper.LastFourDigits(preauth_model["credit_card_number"]);
                                response["bin"] = "";
                                response["card-type"] = "";
                                response["gate-partial-reversal"] = "";
                                response["gate-partial-capture"] = "";
                                response["transaction-type"] = "capture";
                                response["processor-rrn"] = "";
                                response["processor-tx-id"] = "";
                                response["receipt-id"] = "";
                                response["name"] = HttpUtility.UrlEncode(preauth_model["first_name"] + " " + preauth_model["last_name"]);
                                response["cardholder-name"] = HttpUtility.UrlEncode(preauth_model["card_printed_name"]);
                                response["card-exp-month"] = preauth_model["expire_month"];
                                response["card-exp-year"] = preauth_model["expire_year"];
                                response["card-hash-id"] = "";
                                response["email"] = preauth_model["email"];
                                response["bank-name"] = "";
                                response["terminal-id"] = "";
                                response["paynet-processing-date"] = "";
                                response["approval-code"] = "";
                                response["order-stage"] = "capture_failed";
                                response["descriptor"] = preauth_model["order_desc"];
                                response["by-request-sn"] = transactionData.SerialNumber;
                                response["control"] = CalculateHash(response, merchantControlKey);
                                break;
                            case TransactionStatus.Error:
                                response["type"] = "error";
                                response["serial-number"] = Guid.NewGuid().ToString();
                                response["error-code"] = backResponse.Error.ErrorNumber;
                                response["error-message"] = HttpUtility.UrlEncode(backResponse.Error.ErrorMessage);
                                break;
                            case TransactionStatus.Undefined:
                            default:
                                response["type"] = "validation-error";
                                response["serial-number"] = Guid.NewGuid().ToString();
                                response["error-code"] = "1";
                                response["error-message"] = HttpUtility.UrlEncode($"Transaction [orderid={transactionData.TransactionId}] has State [{transactionData.State}] but its Status is [{transactionData.Status}]");
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

        private string RestorePreAuthQuery(string transactionId)
        {
            Transaction transaction = TransactionsDataStorage.FindByTransactionIdAndType(
                transactionId, TransactionType.Preauth);
            if (transaction == null)
            {
                // todo - throw an exception
            }
            return transaction.ReferenceQuery;
        }

        private string PrepareStringResponse(NameValueCollection response) {
            var stringResponse = new StringBuilder(256);
            foreach (string key in response.Keys) {
                if (stringResponse.Length > 0)
                {
                    stringResponse.Append("\n&");
                }
                stringResponse
                    .Append(key)
                    .Append('=')
                    .Append(HttpUtility.UrlEncode(response[key]));
            }
            return stringResponse.ToString();
        }
    }
}