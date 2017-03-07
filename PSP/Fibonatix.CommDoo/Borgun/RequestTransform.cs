using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.Net.Common;
using Genesis.Net.Errors;
using Fibonatix.CommDoo.Helpers;
using System.Globalization;
using System.Net;
using System.Threading;
using System.ComponentModel;
using System.Web;

namespace Fibonatix.CommDoo.Borgun
{
    [System.ComponentModel.DataObject]

    internal class RequestTransform
    {
        static internal string version = "1000";
        static internal string timestampPattern = @"yyMMddHHmmss";

        static internal int sequental = 0;
        static DateTime dt = DateTime.Now.ToUniversalTime();
        private static Object thisLock = new Object();

        public static string GetCurrentDateTime() {
            return DateTime.Now.ToUniversalTime().ToString(timestampPattern);
        }

        private static string getMerchantID(Fibonatix.CommDoo.Requests.Request req) {
            return req.getConfigValue("MerchantID") == null ? "247" : req.getConfigValue("MerchantID");
        }
        private static string getMerchantContractNumber(Fibonatix.CommDoo.Requests.Request req) {
            return req.getConfigValue("MerchantContractNumber") == null ? "9256684" : req.getConfigValue("MerchantContractNumber");
        }
        private static string getProcessor(Fibonatix.CommDoo.Requests.Request req) {
            return req.getConfigValue("Processor") == null ? "247" : req.getConfigValue("Processor");
        }

        internal static bool isRecurrent(Fibonatix.CommDoo.Requests.Request req) {
            bool res = false;

            if (req.GetType() == typeof(Requests.PreauthRequest)) {
                Requests.PreauthRequest preauth = (Requests.PreauthRequest)req;
                if (preauth.getRequestType() == Requests.RequestType.Repeated || preauth.getRequestType() == Requests.RequestType.Initial)
                    res = true;
            } else if (req.GetType() == typeof(Requests.Preauth3DRequest)) {
                Requests.Preauth3DRequest preauth3D = (Requests.Preauth3DRequest)req;
                if (preauth3D.getRequestType() == Requests.RequestType.Repeated || preauth3D.getRequestType() == Requests.RequestType.Initial)
                    res = true;
            } else if (req.GetType() == typeof(Requests.PurchaseRequest)) {
                Requests.PurchaseRequest purchase = (Requests.PurchaseRequest)req;
                if (purchase.getRequestType() == Requests.RequestType.Repeated || purchase.getRequestType() == Requests.RequestType.Initial)
                    res = true;
            } else if (req.GetType() == typeof(Requests.Purchase3DRequest)) {
                Requests.Purchase3DRequest purchase3D = (Requests.Purchase3DRequest)req;
                if (purchase3D.getRequestType() == Requests.RequestType.Repeated || purchase3D.getRequestType() == Requests.RequestType.Initial)
                    res = true;
            }
            return res;
        }

        private static string getTerminalID(Fibonatix.CommDoo.Requests.Request req) {
            if (req.GetType() == typeof(Requests.PreauthRequest)) {
                Requests.PreauthRequest preauth = (Requests.PreauthRequest)req;
                if (preauth.getRequestType() == Requests.RequestType.Repeated)
                    return req.getConfigValue("TerminalRecurrent") != null ? req.getConfigValue("TerminalRecurrent") : "3";
                else if (preauth.preAuth.transaction.terminal != null)
                    return preauth.preAuth.transaction.terminal;
                else if (preauth.preAuth.transaction.threed_secure != null)
                    return req.getConfigValue("Terminal3DS") != null ? req.getConfigValue("Terminal3DS") : "2";
                else
                    return req.getConfigValue("TerminalN3DS") != null ? req.getConfigValue("TerminalN3DS") : "1";
            }
            else if (req.GetType() == typeof(Requests.Preauth3DRequest)) {
                Requests.Preauth3DRequest preauth3D = (Requests.Preauth3DRequest)req;
                if (preauth3D.getRequestType() == Requests.RequestType.Repeated)
                    return req.getConfigValue("TerminalRecurrent") != null ? req.getConfigValue("TerminalRecurrent") : "3";
                else if (preauth3D.preAuth3D.transaction.terminal != null)
                    return preauth3D.preAuth3D.transaction.terminal;
                else
                    return req.getConfigValue("TerminalN3DS") != null ? req.getConfigValue("TerminalN3DS") : "1";
            }
            else if (req.GetType() == typeof(Requests.PurchaseRequest)) {
                Requests.PurchaseRequest purchase = (Requests.PurchaseRequest)req;
                if (purchase.getRequestType() == Requests.RequestType.Repeated)
                    return req.getConfigValue("TerminalRecurrent") != null ? req.getConfigValue("TerminalRecurrent") : "3";
                else if (purchase.purchase.transaction.terminal != null)
                    return purchase.purchase.transaction.terminal;
                else if (purchase.purchase.transaction.threed_secure != null)
                    return req.getConfigValue("Terminal3DS") != null ? req.getConfigValue("Terminal3DS") : "2";
                else
                    return req.getConfigValue("TerminalN3DS") != null ? req.getConfigValue("TerminalN3DS") : "1";
            }
            else if (req.GetType() == typeof(Requests.Purchase3DRequest)) {
                Requests.Purchase3DRequest purchase3D = (Requests.Purchase3DRequest)req;
                if (purchase3D.getRequestType() == Requests.RequestType.Repeated)
                    return req.getConfigValue("TerminalRecurrent") != null ? req.getConfigValue("TerminalRecurrent") : "3";
                else if (purchase3D.purchase3D.transaction.terminal != null)
                    return purchase3D.purchase3D.transaction.terminal;
                else
                    return req.getConfigValue("TerminalN3DS") != null ? req.getConfigValue("TerminalN3DS") : "1";
            }
            else if (req.GetType() == typeof(Requests.RefundRequest)) {
                Requests.RefundRequest refund = (Requests.RefundRequest)req;
                return refund.refund.transaction.terminal;
            }
            else if (req.GetType() == typeof(Requests.ReversalRequest)) {
                Requests.ReversalRequest reversal = (Requests.ReversalRequest)req;
                return reversal.reversal.transaction.terminal;
            }
            else if (req.GetType() == typeof(Requests.SingleReconcileRequest)) {
                Requests.SingleReconcileRequest reconcile = (Requests.SingleReconcileRequest)req;
                return reconcile.reconcile.transaction.terminal;
            }

            return req.getConfigValue("TerminalN3DS") != null ? req.getConfigValue("TerminalN3DS") : "1";
        }

        private static string getNumForRRN() {
            lock (thisLock) {
                DateTime curr = DateTime.Now.ToUniversalTime();
                if (curr.Year != dt.Year || curr.Month != dt.Month || curr.Day != dt.Day || curr.Hour != dt.Hour || curr.Minute != dt.Minute) {
                    dt = curr;
                    sequental = 1;
                }
            }
            int seq = Interlocked.Increment(ref sequental);
            string ret = String.Format("F{0}{1}{2}{3:D2}{4:D2}{5:D4}", (char)(dt.Year - 2017 + 'A'), (char)(dt.Month + 'A' - 1), dt.Day < 15 ? (char)(dt.Day + 'A' - 1) : (char)(dt.Day + 'a' - 16),
                dt.Hour, dt.Minute, sequental);
            return ret;
        }


        public static Entities.Requests.SOAPAuthRequest getBorgunAuthorize(Requests.PreauthRequest commDooAuth, string virtualCard = null) { // exception

            commDooAuth.verification(); // exception

            Entities.Requests.AuthRequest xmlBody = new Entities.Requests.AuthRequest(Entities.Requests.Request.TransactionType.PreAuthorizeOperation) {
                Version = version,
                Processor = getProcessor(commDooAuth),
                MerchantID = getMerchantID(commDooAuth),
                TerminalID = getTerminalID(commDooAuth),
                TrAmount = commDooAuth.preAuth.transaction.amount.ToString(CultureInfo.InvariantCulture),
                TrCurrency = Currencies.getCurrencyNumCode(Currencies.currencyCodeFromString(commDooAuth.preAuth.transaction.currency)),
                DateAndTime = commDooAuth.preAuth.transaction.datetime != null ? commDooAuth.preAuth.transaction.datetime : GetCurrentDateTime(),
                RRN = commDooAuth.preAuth.transaction.rrn != null ? commDooAuth.preAuth.transaction.rrn : getNumForRRN(),
            };

            if (commDooAuth.preAuth.transaction.cred_card_data != null) {
                xmlBody.PAN = commDooAuth.preAuth.transaction.cred_card_data.credit_card_number;
                xmlBody.ExpDate = commDooAuth.preAuth.transaction.cred_card_data.expiration_year == 0 ? null :
                        (((commDooAuth.preAuth.transaction.cred_card_data.expiration_year % 100) * 100 + commDooAuth.preAuth.transaction.cred_card_data.expiration_month).ToString());
                xmlBody.CVC2 = commDooAuth.preAuth.transaction.cred_card_data.cvv;
            }

            if (virtualCard != null)
                xmlBody.PAN = virtualCard;

            if (commDooAuth.preAuth.transaction.customer_data != null) {
                xmlBody.MerchantName = commDooAuth.preAuth.transaction.customer_data.firstname + " " + commDooAuth.preAuth.transaction.customer_data.lastname;
                xmlBody.MerchantHome = commDooAuth.preAuth.transaction.customer_data.street;
                xmlBody.MerchantCity = commDooAuth.preAuth.transaction.customer_data.city;
                xmlBody.MerchantCountry = Countries.countryAlpha2String(commDooAuth.preAuth.transaction.customer_data.country);
                xmlBody.MerchantZipCode = commDooAuth.preAuth.transaction.customer_data.postalcode;
            }

            if (commDooAuth.preAuth.transaction.threed_secure != null) {
                xmlBody.SecurityLevelInd = commDooAuth.preAuth.transaction.threed_secure.security_level;
                xmlBody.CAVV = commDooAuth.preAuth.transaction.threed_secure.cavv;
                xmlBody.UCAF = commDooAuth.preAuth.transaction.threed_secure.ucaf;
                xmlBody.XID = commDooAuth.preAuth.transaction.threed_secure.xid;
            }


            Entities.Requests.SOAPAuthRequest soapResp = new Entities.Requests.SOAPAuthRequest() {
                Body = new Entities.Requests.SOAPAuthRequest.SOAPBody() {
                    getAuthorizationInput = new Entities.Requests.SOAPAuthRequest.SOAPBody.RequestContainer() {
                        getAuthReqXml = xmlBody.getXml(),
                    }
                }
            };
            return soapResp;
        }
        
        public static Entities.Requests.SOAPAuthRequest getBorgunCapture(Requests.CaptureRequest commDooCapture) { // exception

            commDooCapture.verification(); // exception

            Entities.Requests.AuthRequest xmlBody = new Entities.Requests.AuthRequest(Entities.Requests.Request.TransactionType.CaptureOperation) {
                Version = version,
                Processor = getProcessor(commDooCapture),
                MerchantID = getMerchantID(commDooCapture),
                TerminalID = getTerminalID(commDooCapture),
                TrAmount = commDooCapture.capture.transaction.amount.ToString(CultureInfo.InvariantCulture),
                TrCurrency = Currencies.getCurrencyNumCode(Currencies.currencyCodeFromString(commDooCapture.capture.transaction.currency)),
                DateAndTime = commDooCapture.capture.transaction.datetime,
                RRN = commDooCapture.capture.transaction.rrn,
                AuthCode = commDooCapture.capture.transaction.auth_code,
            };

            Entities.Requests.SOAPAuthRequest soapResp = new Entities.Requests.SOAPAuthRequest() {
                Body = new Entities.Requests.SOAPAuthRequest.SOAPBody() {
                    getAuthorizationInput = new Entities.Requests.SOAPAuthRequest.SOAPBody.RequestContainer() {
                        getAuthReqXml = xmlBody.getXml(),
                    }
                }
            };
            return soapResp;
        }

        public static Entities.Requests.SOAPAuthRequest getBorgunPurchase(Requests.PurchaseRequest commDooPurchase, string virtualCard = null) { // exception

            commDooPurchase.verification(); // exception

            Entities.Requests.AuthRequest xmlBody = new Entities.Requests.AuthRequest(Entities.Requests.Request.TransactionType.CaptureOperation) {
                Version = version,
                Processor = getProcessor(commDooPurchase),
                MerchantID = getMerchantID(commDooPurchase),
                TerminalID = getTerminalID(commDooPurchase),
                TrAmount = commDooPurchase.purchase.transaction.amount.ToString(CultureInfo.InvariantCulture),
                TrCurrency = Currencies.getCurrencyNumCode(Currencies.currencyCodeFromString(commDooPurchase.purchase.transaction.currency)),
                DateAndTime = commDooPurchase.purchase.transaction.datetime != null ? commDooPurchase.purchase.transaction.datetime : GetCurrentDateTime(),
                RRN = commDooPurchase.purchase.transaction.rrn != null ? commDooPurchase.purchase.transaction.rrn : getNumForRRN(),
                PAN = virtualCard != null ? virtualCard : commDooPurchase.purchase.transaction.cred_card_data.credit_card_number,
            };
            
            if( commDooPurchase.purchase.transaction.cred_card_data != null )
            {
                xmlBody.PAN = commDooPurchase.purchase.transaction.cred_card_data.credit_card_number;
                xmlBody.ExpDate = commDooPurchase.purchase.transaction.cred_card_data.expiration_year == 0 ? null :
                        (((commDooPurchase.purchase.transaction.cred_card_data.expiration_year % 100) * 100 + commDooPurchase.purchase.transaction.cred_card_data.expiration_month).ToString());
                xmlBody.CVC2 = commDooPurchase.purchase.transaction.cred_card_data.cvv;
            }

            if (virtualCard != null)
                xmlBody.PAN = virtualCard;

            if (commDooPurchase.purchase.transaction.customer_data != null) {
                xmlBody.MerchantName = commDooPurchase.purchase.transaction.customer_data.firstname + " " + commDooPurchase.purchase.transaction.customer_data.lastname;
                xmlBody.MerchantHome = commDooPurchase.purchase.transaction.customer_data.street;
                xmlBody.MerchantCity = commDooPurchase.purchase.transaction.customer_data.city;
                xmlBody.MerchantCountry = Countries.countryAlpha2String(commDooPurchase.purchase.transaction.customer_data.country);
                xmlBody.MerchantZipCode = commDooPurchase.purchase.transaction.customer_data.postalcode;
            }

            if (commDooPurchase.purchase.transaction.threed_secure != null) {
                xmlBody.SecurityLevelInd = commDooPurchase.purchase.transaction.threed_secure.security_level;
                xmlBody.CAVV = commDooPurchase.purchase.transaction.threed_secure.cavv;
                xmlBody.UCAF = commDooPurchase.purchase.transaction.threed_secure.ucaf;
                xmlBody.XID = commDooPurchase.purchase.transaction.threed_secure.xid;
            }

            Entities.Requests.SOAPAuthRequest soapResp = new Entities.Requests.SOAPAuthRequest() {
                Body = new Entities.Requests.SOAPAuthRequest.SOAPBody() {
                    getAuthorizationInput = new Entities.Requests.SOAPAuthRequest.SOAPBody.RequestContainer() {
                        getAuthReqXml = xmlBody.getXml(),
                    }
                }
            };
            return soapResp;
        }

        public static Entities.Requests.SOAPAuthRequest getBorgunRefund(Requests.RefundRequest commDooRefund, string virtualCard = null) { // exception

            commDooRefund.verification(); // exception

            Entities.Requests.AuthRequest xmlBody = new Entities.Requests.AuthRequest(Entities.Requests.Request.TransactionType.RefundOperation) {
                Version = version,
                Processor = getProcessor(commDooRefund),
                MerchantID = getMerchantID(commDooRefund),
                TerminalID = getTerminalID(commDooRefund),
                TrAmount = commDooRefund.refund.transaction.amount.ToString(CultureInfo.InvariantCulture),
                TrCurrency = Currencies.getCurrencyNumCode(Currencies.currencyCodeFromString(commDooRefund.refund.transaction.currency)),
                DateAndTime = commDooRefund.refund.transaction.datetime,
                RRN = commDooRefund.refund.transaction.rrn,
            };

            Entities.Requests.SOAPAuthRequest soapResp = new Entities.Requests.SOAPAuthRequest() {
                Body = new Entities.Requests.SOAPAuthRequest.SOAPBody() {
                    getAuthorizationInput = new Entities.Requests.SOAPAuthRequest.SOAPBody.RequestContainer() {
                        getAuthReqXml = xmlBody.getXml(),
                    }
                }
            };
            return soapResp;
        }
        
        public static Entities.Requests.SOAPCancelRequest getBorgunReversal(Requests.ReversalRequest commDooReversal, string virtualCard = null) { // exception

            commDooReversal.verification(); // exception

            Entities.Requests.CancelRequest xmlBody = new Entities.Requests.CancelRequest(Entities.Requests.Request.TransactionType.PreAuthorizeOperation) {
                Version = version,
                Processor = getProcessor(commDooReversal),
                MerchantID = getMerchantID(commDooReversal),
                TerminalID = getTerminalID(commDooReversal),
                TrAmount = commDooReversal.reversal.transaction.amount.ToString(CultureInfo.InvariantCulture),
                TrCurrency = Currencies.getCurrencyNumCode(Currencies.currencyCodeFromString(commDooReversal.reversal.transaction.currency)),
                DateAndTime = commDooReversal.reversal.transaction.datetime,
                RRN = commDooReversal.reversal.transaction.rrn,
                AuthCode = commDooReversal.reversal.transaction.auth_code,
            };

            Entities.Requests.SOAPCancelRequest soapResp = new Entities.Requests.SOAPCancelRequest() {
                Body = new Entities.Requests.SOAPCancelRequest.SOAPBody() {
                    cancelAuthorizationInput = new Entities.Requests.SOAPCancelRequest.SOAPBody.RequestContainer() {
                        cancelAuthReqXml = xmlBody.getXml(),
                    }
                }
            };
            return soapResp;
        }

        
        public static Entities.Requests.SOAPTransactionInfoRequest getBorgunSingleReconcile(Requests.SingleReconcileRequest commDooReconcile) { // exception

            commDooReconcile.verification(); // exception

            Entities.Requests.TransactionInfoRequest xmlBody = new Entities.Requests.TransactionInfoRequest() {
                Version = version,
                Processor = getProcessor(commDooReconcile),
                MerchantID = getMerchantID(commDooReconcile),
                TerminalID = getTerminalID(commDooReconcile),
                RRN = commDooReconcile.reconcile.transaction.rrn,                
            };

            Entities.Requests.SOAPTransactionInfoRequest soapResp = new Entities.Requests.SOAPTransactionInfoRequest() {
                Body = new Entities.Requests.SOAPTransactionInfoRequest.SOAPBody() {
                    getTransactionList = new Entities.Requests.SOAPTransactionInfoRequest.SOAPBody.RequestContainer() {
                        transactionListReqXML = xmlBody.getXml(),
                    }
                }
            };
            return soapResp;
        }
        

        /*
        public static Fibonatix.CommDoo.Kalixa.Entities.Requests.EnrollmentCheckRequest getKalixaEnrollmentCheck(Fibonatix.CommDoo.Requests.EnrollmentCheck3DRequest commDooEnroll) { // exception

            commDooEnroll.verification(); // exception

            Fibonatix.CommDoo.Kalixa.Entities.Requests.EnrollmentCheckRequest ret = new Fibonatix.CommDoo.Kalixa.Entities.Requests.EnrollmentCheckRequest() {
                amount = new Fibonatix.CommDoo.Kalixa.Entities.Requests.Request.Amount() {
                    value = Convertors.MinorAmountToMajor((int)commDooEnroll.enrollment_check.transaction.amount, Currencies.currencyCodeFromString(commDooEnroll.enrollment_check.transaction.currency)).ToString(CultureInfo.InvariantCulture),
                    currencyCode = commDooEnroll.enrollment_check.transaction.currency
                },
                creationTypeID = "1",
                merchantID = getMerchantID(commDooEnroll),
                shopID = getShopID(commDooEnroll),
                merchantTransactionID = commDooEnroll.enrollment_check.transaction.reference_id,
                paymentAccountID = getUserID(commDooEnroll.enrollment_check.transaction.customer_data.email, commDooEnroll.enrollment_check.transaction.customer_data.firstname,
                        commDooEnroll.enrollment_check.transaction.customer_data.lastname, commDooEnroll.enrollment_check.transaction.credit_card_alias),
                paymentMethodID = getPreauthPaymentMethod(commDooEnroll.enrollment_check.transaction.cred_card_data.getCreditCardType()),
                specificPaymentData = new Entities.Requests.Request.dataList() {
                    {
                        new Entities.Requests.Request.keyStringValuePair() { type = "keyStringValuePair", key = "PaymentDescription", value = commDooEnroll.enrollment_check.transaction.usage }
                    },
                    {
                        new Entities.Requests.Request.keyStringValuePair() { type = "keyStringValuePair", key = "PaymentDescriptionLanguageCode", value = "en" }
                    },
                    {
                        new Entities.Requests.Request.keyStringValuePair() { type = "keyBooleanValuePair", key = "IsThreeDSecureRequired", value = "true" }
                    },
                },
                userID = getUserID(commDooEnroll.enrollment_check.transaction.customer_data.email, commDooEnroll.enrollment_check.transaction.customer_data.firstname,
                        commDooEnroll.enrollment_check.transaction.customer_data.lastname, commDooEnroll.enrollment_check.transaction.credit_card_alias),
                userSessionID = commDooEnroll.enrollment_check.transaction.customer_data.email + DateTime.Now.ToBinary().ToString(),
                userData = new Entities.Requests.Request.UserData() {
                    address = new Entities.Requests.Request.UserData.Address() {
                        city = commDooEnroll.enrollment_check.transaction.customer_data.city,
                        countryCode2 = Countries.countryAlpha2String(commDooEnroll.enrollment_check.transaction.customer_data.country),
                        // houseName = commDooAuth.preAuth.transaction.customer_data.street,
                        //houseNumber = "12",
                        //houseNumberExtension = "1B",
                        postalCode = commDooEnroll.enrollment_check.transaction.customer_data.postalcode,
                        state = commDooEnroll.enrollment_check.transaction.customer_data.state,
                        street = commDooEnroll.enrollment_check.transaction.customer_data.street,
                        telephoneNumber = commDooEnroll.enrollment_check.transaction.customer_data.phone,
                    },
                    currencyCode = commDooEnroll.enrollment_check.transaction.currency,
                    // dateOfBirth = "1950-01-01T00:00:00",
                    email = commDooEnroll.enrollment_check.transaction.customer_data.email,
                    firstname = commDooEnroll.enrollment_check.transaction.customer_data.firstname,
                    // gender = "Female",
                    // identificationNumber = "111",
                    languageCode = "EN",
                    lastname = commDooEnroll.enrollment_check.transaction.customer_data.lastname,
                    username = commDooEnroll.enrollment_check.transaction.customer_data.firstname + " " + commDooEnroll.enrollment_check.transaction.customer_data.lastname,
                },
                userIP = commDooEnroll.enrollment_check.transaction.customer_data.ipaddress,
            };

            if (commDooEnroll.enrollment_check.transaction.cred_card_data != null) {
                ret.paymentAccount = new Entities.Requests.EnrollmentCheckRequest.PaymentAccount() {
                    specificPaymentAccountData = new Entities.Requests.Request.dataList() {
                        {
                            new Entities.Requests.Request.keyStringValuePair() { type = "keyStringValuePair", key = "CardNumber", value = commDooEnroll.enrollment_check.transaction.cred_card_data.credit_card_number }
                        },
                        {
                            new Entities.Requests.Request.keyStringValuePair() { type = "keyStringValuePair", key = "CardVerificationCode", value = commDooEnroll.enrollment_check.transaction.cred_card_data.cvv }
                        },
                        {
                            new Entities.Requests.Request.keyStringValuePair() { type = "keyStringValuePair", key = "HolderName", value = commDooEnroll.enrollment_check.transaction.cred_card_data.cardholder_name }
                        },
                        {
                            new Entities.Requests.Request.keyStringValuePair() { type = "keyIntValuePair", key = "ExpiryMonth", value = commDooEnroll.enrollment_check.transaction.cred_card_data.expiration_month.ToString() }
                        },
                        {
                            new Entities.Requests.Request.keyStringValuePair() { type = "keyIntValuePair", key = "ExpiryYear", value = commDooEnroll.enrollment_check.transaction.cred_card_data.expuration_year.ToString() }
                        },
                    }
                };
            }

            return ret;
        }

        public static Fibonatix.CommDoo.Kalixa.Entities.Requests.Preauth3DRequest getKalixaAuthorize3D(Fibonatix.CommDoo.Requests.Preauth3DRequest commDooAuth) { // exception
            throw new System.ComponentModel.DataAnnotations.ValidationException("Kalixa aquirer doesn't support 'Preauthorize 3D' request").SetCode((int)ErrorCodes.InvalidTransactionTypeError);
        }

        public static Fibonatix.CommDoo.Kalixa.Entities.Requests.Purchase3DRequest getKalixaPurchase3D(Fibonatix.CommDoo.Requests.Purchase3DRequest commDooPurchase) { // exception
            throw new System.ComponentModel.DataAnnotations.ValidationException("Kalixa aquirer doesn't support 'Purchase 3D' request").SetCode((int)ErrorCodes.InvalidTransactionTypeError);
        }


        public static Fibonatix.CommDoo.Kalixa.Entities.Requests.CaptureRequest getKalixaCaptureForPreauth90(Fibonatix.CommDoo.Requests.PreauthRequest commDooPre) { // exception

            commDooPre.verification(); // exception

            if (commDooPre.preAuth.transaction.threed_secure == null) {
                string ExceptionMessage = "'ThreeDSecure' section is not exist in Preauth (90) request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            }

            Fibonatix.CommDoo.Kalixa.Entities.Requests.CaptureRequest ret = new Fibonatix.CommDoo.Kalixa.Entities.Requests.CaptureRequest() {
                merchantID = getMerchantID(commDooPre),
                shopID = getShopID(commDooPre),
                paymentID = commDooPre.preAuth.transaction.provider_transaction_id,
                actionID = "90", // 90 - HandleThreeDSecureAuthentificationResponse
                actionData = new Entities.Requests.Request.dataList() {
                        {
                            new Entities.Requests.Request.keyStringValuePair() { type = "keyStringValuePair", key = "PaRes", value = commDooPre.preAuth.transaction.threed_secure.pa_res }
                        },
                        {
                            new Entities.Requests.Request.keyStringValuePair() { type = "keyStringValuePair", key = "MD", value = commDooPre.preAuth.transaction.threed_secure.md }
                        },
                    },
                remark = commDooPre.preAuth.transaction.usage,
            };
            return ret;
        }
        public static Fibonatix.CommDoo.Kalixa.Entities.Requests.CaptureRequest getKalixaCaptureForPreauth120(Fibonatix.CommDoo.Requests.PreauthRequest commDooPre) {

            commDooPre.verification(); // exception

            if (commDooPre.preAuth.transaction.threed_secure == null) {
                string ExceptionMessage = "'ThreeDSecure' section is not exist in Preauth (120) request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            }

            Fibonatix.CommDoo.Kalixa.Entities.Requests.CaptureRequest ret = new Fibonatix.CommDoo.Kalixa.Entities.Requests.CaptureRequest() {
                merchantID = getMerchantID(commDooPre),
                shopID = getShopID(commDooPre),
                paymentID = commDooPre.preAuth.transaction.provider_transaction_id,
                actionID = "120", // 120 - Authorize
                actionData = new Entities.Requests.Request.dataList() {
                        {
                            new Entities.Requests.Request.keyStringValuePair() { type = "keyStringValuePair", key = "PaRes", value = commDooPre.preAuth.transaction.threed_secure.pa_res }
                        },
                        {
                            new Entities.Requests.Request.keyStringValuePair() { type = "keyStringValuePair", key = "MD", value = commDooPre.preAuth.transaction.threed_secure.md }
                        },
                    },
                remark = commDooPre.preAuth.transaction.usage,
            };
            if(commDooPre.preAuth.transaction.cred_card_data != null && commDooPre.preAuth.transaction.cred_card_data.cvv != null) {
                ret.actionData.Add(new Entities.Requests.Request.keyStringValuePair() { type = "keyStringValuePair", key = "CardVerificationCode", value = commDooPre.preAuth.transaction.cred_card_data.cvv });
            }

            return ret;
        }
        */
    }
}
