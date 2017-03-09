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

namespace Fibonatix.CommDoo.Kalixa
{
    internal class RequestTransform
    {
        public enum PreauthRequestType
        {
            PreauthNon3D = 0,
            Preauth3DPart1 = 1,
            Preauth3DPart2 = 2,
        }


        private static string getBase64Hash(string email, string firstname, string lastname) {
            return ((UInt32)((email + firstname + lastname).GetHashCode())).ToString();
        }

        private static string getUserID(string email, string firstname, string lastname, string credit_card_alias) {
            if (credit_card_alias != null)
                return credit_card_alias;
            else
                return ((UInt32)((email + firstname + lastname).GetHashCode())).ToString();
        }

        private static string getMerchantID(Fibonatix.CommDoo.Requests.Request req) {
            return req.getConfigValue("MerchantID") == null ? "Fibonatix" : req.getConfigValue("MerchantID");
        }
        private static string getShopID(Fibonatix.CommDoo.Requests.Request req) {
            return req.getConfigValue("ShopID") == null ? "Fibonatix" : req.getConfigValue("ShopID");
        }

        private static string getPreauthPaymentMethod(Requests.Request.CreditCardData.CreditCardType cardType) {
            switch (cardType) {
                case Requests.Request.CreditCardData.CreditCardType.Visa: return "2";
                case Requests.Request.CreditCardData.CreditCardType.MasterCard: return "1";
                case Requests.Request.CreditCardData.CreditCardType.Maestro: return "73";
                case Requests.Request.CreditCardData.CreditCardType.Diners: return "3";
                case Requests.Request.CreditCardData.CreditCardType.AmericanExpress: return "113";
                default: return "2";
            }
        }

        private static string getRefundPaymentMethod(Requests.Request.CreditCardData.CreditCardType cardType) {
            switch (cardType) {
                case Requests.Request.CreditCardData.CreditCardType.Visa: return "88";
                case Requests.Request.CreditCardData.CreditCardType.MasterCard: return "89";
                case Requests.Request.CreditCardData.CreditCardType.Maestro: return "90";
                case Requests.Request.CreditCardData.CreditCardType.Diners: return "154";
                case Requests.Request.CreditCardData.CreditCardType.AmericanExpress: return "115";
                default: return "88";
            }
        }

        private static string getCreationTypeByRecurrence(Requests.Request.RecurringTransaction recurrence) {
            if (recurrence == null)
                return "1";
            else if (String.Equals(recurrence.type, "repeated", StringComparison.InvariantCultureIgnoreCase))
                return "3";
            else
                return "1";            
        }

        public static Fibonatix.CommDoo.Kalixa.Entities.Requests.PreauthRequest getKalixaAuthorize(Fibonatix.CommDoo.Requests.PreauthRequest commDooAuth, PreauthRequestType type = PreauthRequestType.PreauthNon3D) { // exception

            commDooAuth.verification(); // exception

            Requests.Request.CreditCardData.CreditCardType ccType = commDooAuth.preAuth.transaction.cred_card_data == null ? Requests.Request.CreditCardData.CreditCardType.Visa : commDooAuth.preAuth.transaction.cred_card_data.getCreditCardType();

            Fibonatix.CommDoo.Kalixa.Entities.Requests.PreauthRequest ret = new Fibonatix.CommDoo.Kalixa.Entities.Requests.PreauthRequest() {
                amount = new Fibonatix.CommDoo.Kalixa.Entities.Requests.Request.Amount() {
                    value = Convertors.MinorAmountToMajor((int)commDooAuth.preAuth.transaction.amount, Currencies.currencyCodeFromString(commDooAuth.preAuth.transaction.currency)).ToString(CultureInfo.InvariantCulture),
                    currencyCode = commDooAuth.preAuth.transaction.currency
                },
                creationTypeID = getCreationTypeByRecurrence(commDooAuth.preAuth.transaction.recurring_transaction),
                merchantID = getMerchantID(commDooAuth),
                shopID = getShopID(commDooAuth),
                merchantTransactionID = commDooAuth.preAuth.transaction.reference_id,
                paymentAccountID = getUserID(commDooAuth.preAuth.transaction.customer_data.email, commDooAuth.preAuth.transaction.customer_data.firstname, commDooAuth.preAuth.transaction.customer_data.lastname, commDooAuth.preAuth.transaction.credit_card_alias),
                paymentMethodID = getPreauthPaymentMethod(ccType),
                specificPaymentData = new Entities.Requests.Request.dataList() {
                    {
                        new Entities.Requests.Request.keyStringValuePair() { type = "keyStringValuePair", key = "PaymentDescription", value = commDooAuth.preAuth.transaction.usage }
                    },
                    {
                        new Entities.Requests.Request.keyStringValuePair() { type = "keyStringValuePair", key = "PaymentDescriptionLanguageCode", value = "en" }
                    },
                },
                userID = getUserID(commDooAuth.preAuth.transaction.customer_data.email, commDooAuth.preAuth.transaction.customer_data.firstname, commDooAuth.preAuth.transaction.customer_data.lastname, commDooAuth.preAuth.transaction.credit_card_alias),
                userSessionID = getUserID(commDooAuth.preAuth.transaction.customer_data.email, commDooAuth.preAuth.transaction.customer_data.firstname, commDooAuth.preAuth.transaction.customer_data.lastname, commDooAuth.preAuth.transaction.credit_card_alias) + DateTime.Now.ToBinary().ToString(),
                userIP = commDooAuth.preAuth.transaction.customer_data.ipaddress,
            };
            if (commDooAuth.preAuth.transaction.cred_card_data != null) {
                if(commDooAuth.preAuth.transaction.cred_card_data.credit_card_number != null && commDooAuth.preAuth.transaction.cred_card_data.cvv != null)
                {
                    ret.paymentAccount = new Entities.Requests.PreauthRequest.PaymentAccount()
                    {
                        specificPaymentAccountData = new Entities.Requests.Request.dataList() {
                            {
                                new Entities.Requests.Request.keyStringValuePair() { type = "keyStringValuePair", key = "CardNumber", value = commDooAuth.preAuth.transaction.cred_card_data.credit_card_number }
                            },
                            {
                                new Entities.Requests.Request.keyStringValuePair() { type = "keyStringValuePair", key = "CardVerificationCode", value = commDooAuth.preAuth.transaction.cred_card_data.cvv }
                            },
                            {
                                new Entities.Requests.Request.keyStringValuePair() { type = "keyStringValuePair", key = "HolderName", value = commDooAuth.preAuth.transaction.cred_card_data.cardholder_name }
                            },
                            {
                                new Entities.Requests.Request.keyStringValuePair() { type = "keyIntValuePair", key = "ExpiryMonth", value = commDooAuth.preAuth.transaction.cred_card_data.expiration_month.ToString() }
                            },
                            {
                                new Entities.Requests.Request.keyStringValuePair() { type = "keyIntValuePair", key = "ExpiryYear", value = commDooAuth.preAuth.transaction.cred_card_data.expiration_year.ToString() }
                            },
                        }
                    };
                }
            }
            if(commDooAuth.preAuth.transaction.customer_data != null) {
                ret.userData = new Entities.Requests.Request.UserData() {
                    address = new Entities.Requests.Request.UserData.Address() {
                        city = commDooAuth.preAuth.transaction.customer_data.city,
                        countryCode2 = Countries.countryAlpha2String(commDooAuth.preAuth.transaction.customer_data.country),
                        // houseName = commDooAuth.preAuth.transaction.customer_data.street,
                        //houseNumber = "12",
                        //houseNumberExtension = "1B",
                        postalCode = commDooAuth.preAuth.transaction.customer_data.postalcode,
                        state = commDooAuth.preAuth.transaction.customer_data.state,
                        street = commDooAuth.preAuth.transaction.customer_data.street,
                        telephoneNumber = commDooAuth.preAuth.transaction.customer_data.phone,
                    },
                    currencyCode = commDooAuth.preAuth.transaction.currency,
                    // dateOfBirth = "1950-01-01T00:00:00",
                    email = commDooAuth.preAuth.transaction.customer_data.email,
                    firstname = commDooAuth.preAuth.transaction.customer_data.firstname,
                    // gender = "Female",
                    // identificationNumber = "111",
                    languageCode = "EN",
                    lastname = commDooAuth.preAuth.transaction.customer_data.lastname,
                    username = commDooAuth.preAuth.transaction.customer_data.firstname + " " + commDooAuth.preAuth.transaction.customer_data.lastname,
                };
            }

            return ret;
        }

        public static Fibonatix.CommDoo.Kalixa.Entities.Requests.CaptureRequest getKalixaCapture(Fibonatix.CommDoo.Requests.CaptureRequest commDooCapture) { // exception

            commDooCapture.verification(); // exception

            Fibonatix.CommDoo.Kalixa.Entities.Requests.CaptureRequest ret = new Fibonatix.CommDoo.Kalixa.Entities.Requests.CaptureRequest() {
                merchantID = getMerchantID(commDooCapture),
                shopID = getShopID(commDooCapture),
                paymentID = commDooCapture.capture.transaction.provider_transaction_id,
                actionID = "205", // 205 - InitiateCapturing
                remark = commDooCapture.capture.transaction.usage,
            };
            return ret;
        }

        public static Fibonatix.CommDoo.Kalixa.Entities.Requests.RefundRequest getKalixaRefund(Fibonatix.CommDoo.Requests.RefundRequest commDooRefund) { // exception

            commDooRefund.verification(); // exception

            var refund = new Fibonatix.CommDoo.Kalixa.Entities.Requests.RefundRequest() {
                merchantID = getMerchantID(commDooRefund),
                shopID = getShopID(commDooRefund),
                amount = new Fibonatix.CommDoo.Kalixa.Entities.Requests.Request.Amount() {
                    value = Convertors.MinorAmountToMajor((int)commDooRefund.refund.transaction.amount, Currencies.currencyCodeFromString(commDooRefund.refund.transaction.currency)).ToString(CultureInfo.InvariantCulture),
                    currencyCode = commDooRefund.refund.transaction.currency
                },
                merchantTransactionID = commDooRefund.refund.transaction.reference_id,
                originalPaymentID = commDooRefund.refund.transaction.provider_transaction_id,
                paymentMethodID = getRefundPaymentMethod(commDooRefund.refund.transaction.cred_card_data.getCreditCardType()),
                specificPaymentData = new Fibonatix.CommDoo.Kalixa.Entities.Requests.Request.dataList() {
                            new Entities.Requests.Request.keyStringValuePair() { type = "keyStringValuePair", key = "PaymentDescription", value = commDooRefund.refund.transaction.usage }
                },
            };
            return refund;
        }

        public static Fibonatix.CommDoo.Kalixa.Entities.Requests.ReversalRequest getKalixaVoid(Fibonatix.CommDoo.Requests.ReversalRequest commDooReversal) { // exception

            commDooReversal.verification(); // exception

            var voidReq = new Fibonatix.CommDoo.Kalixa.Entities.Requests.ReversalRequest() {
                merchantID = getMerchantID(commDooReversal),
                shopID = getShopID(commDooReversal),
                paymentID = commDooReversal.reversal.transaction.provider_transaction_id,
                actionID = "1",  // 1 - Cancel Payment
                remark = "void payment",
            };
            return voidReq;
        }

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
                userSessionID = getUserID(commDooEnroll.enrollment_check.transaction.customer_data.email, commDooEnroll.enrollment_check.transaction.customer_data.firstname,
                        commDooEnroll.enrollment_check.transaction.customer_data.lastname, commDooEnroll.enrollment_check.transaction.credit_card_alias) + DateTime.Now.ToBinary().ToString(),
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
                            new Entities.Requests.Request.keyStringValuePair() { type = "keyIntValuePair", key = "ExpiryYear", value = commDooEnroll.enrollment_check.transaction.cred_card_data.expiration_year.ToString() }
                        },
                    }
                };
            }

            if(commDooEnroll.enrollment_check.transaction.customer_data != null) {
                ret.userData = new Entities.Requests.Request.UserData() {
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

        public static Fibonatix.CommDoo.Kalixa.Entities.Requests.SingleReconcileRequest getKalixaSingleReconcile(Fibonatix.CommDoo.Requests.SingleReconcileRequest commDooReconcile) { // exception

            commDooReconcile.verification(); // exception

            var reconcileReq = new Fibonatix.CommDoo.Kalixa.Entities.Requests.SingleReconcileRequest() {
                merchantID = getMerchantID(commDooReconcile),
                shopID = getShopID(commDooReconcile),
                merchantTransactionID = commDooReconcile.reconcile.transaction.reference_id,
            };
            return reconcileReq;
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
    }
}
