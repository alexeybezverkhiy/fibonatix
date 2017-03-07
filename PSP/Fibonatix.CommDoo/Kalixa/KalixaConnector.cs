using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Genesis.Net.Errors;
using Fibonatix.CommDoo.Requests;
using Fibonatix.CommDoo.Responses;
using System.Globalization;

namespace Fibonatix.CommDoo.Kalixa
{
    internal class KalixaConnector : IConnector
    {
        static private Dictionary<string, KalixaConnector> allconnectors;

        Network.Client client;

        public KalixaConnector(string login, string password, bool sandbox) {
            client = new Network.Client(login, password, sandbox);
        }

        static public KalixaConnector getConnector(string login, string password, bool sandbox) {
            if (login == null || password == null)
                return null;
            string key = login + ":" + password + ":" + sandbox.ToString();
            if (allconnectors == null) allconnectors = new Dictionary<string, KalixaConnector>();
            if (!allconnectors.ContainsKey(key)) {
                KalixaConnector conn = new KalixaConnector(login, password, sandbox);
                allconnectors[key] = conn;
            }
            return allconnectors[key];
        }

        static public KalixaConnector getConnector(Request request) {
            bool sanbox = false;
            sanbox = String.Equals(request.getConfigValue("testmode"), "true", StringComparison.OrdinalIgnoreCase);
            return getConnector(request.getConfigValue("login"), request.getConfigValue("password"), sanbox);
        }

        public PreauthResponse Preauthorize(PreauthRequest request) {
            if (request.preAuth.transaction.threed_secure != null &&
                (request.preAuth.transaction.threed_secure.enrolment_status == "Y" ||
                request.preAuth.transaction.threed_secure.enrolment_status == "N" || 
                request.preAuth.transaction.threed_secure.enrolment_status == "U")) {
                return PreauthorizeAfterEnroll(request);
            } else {
                return PreauthorizeNon3D(request);
            }
        }
        private PreauthResponse PreauthorizeNon3D(PreauthRequest request) {
            PreauthResponse response = null;

            try {
                var kalixaPre = RequestTransform.getKalixaAuthorize(request);


                string xml = kalixaPre.getXml();

                Console.WriteLine("Kalixa Preauthorization request: {0}", xml);

                string res = client.ProcessRequest(kalixaPre);

                Console.WriteLine("Kalixa Preauthorization response: {0}", res);
                try {
                    Kalixa.Entities.Response.PreauthResponse resp = Kalixa.Entities.Response.PreauthResponse.DeserializeFromString(res);
                    if (resp.payment.state.definition.key == "13" ||
                        resp.payment.state.definition.key == "27" ||
                        resp.payment.state.definition.key == "142") {
                        // success
                        response = new PreauthResponse() {
                            preAuth = new PreauthResponse.ResponseFunction() {
                                transaction = new PreauthResponse.Transaction() {
                                    reference_id = resp.payment.merchantTransactionID,                                                                        
                                    processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                        CreditCardAlias = resp.payment.userID,
                                        ProviderTransactionID = resp.payment.paymentID,
                                        FunctionResult = "ACK",
                                    }
                                }
                            }
                        };
                    } else {
                        response = new PreauthResponse() {
                            preAuth = new PreauthResponse.ResponseFunction() {
                                transaction = new PreauthResponse.Transaction() {
                                    reference_id = resp.payment.merchantTransactionID,
                                    processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                        ProviderTransactionID = resp.payment.paymentID,
                                        FunctionResult = "NOK",
                                        error = new PreauthResponse.Transaction.ProcessingStatus.Error() {
                                            type = "PROVIDER", // "DATA"
                                            number = resp.payment.state.definition.key,
                                            message = resp.payment.state.definition.value + ": " + resp.payment.state.description,
                                        }
                                    }
                                }
                            }
                        };
                    }
                } catch {
                    Kalixa.Entities.Response.ExceptionResponse resp = Kalixa.Entities.Response.ExceptionResponse.DeserializeFromString(res);
                    response = new PreauthResponse() {
                        preAuth = new PreauthResponse.ResponseFunction() {
                            transaction = new PreauthResponse.Transaction() {
                                processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                    FunctionResult = "NOK",
                                    error = new PreauthResponse.Transaction.ProcessingStatus.Error() {
                                        type = "PROVIDER", // "DATA"
                                        number = resp.errorCode,
                                        message = resp.errorMessage,
                                    }
                                }
                            }
                        }
                    };
                }
            } catch (Exception ex) {
                response = new PreauthResponse() {
                    preAuth = new PreauthResponse.ResponseFunction() {
                        transaction = new PreauthResponse.Transaction() {
                            processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new PreauthResponse.Transaction.ProcessingStatus.Error() {
                                    type = "SYSTEM", // "DATA"
                                    number = ((UInt32)ex.HResult).ToString(),
                                    message = ex.Message,
                                }
                            }
                        }
                    }
                };
            }
            return response;
        }
        private PreauthResponse PreauthorizeAfterEnroll(PreauthRequest request) {
            PreauthResponse response = null;
            try {
                if (request.preAuth.transaction.threed_secure.enrolment_status == "Y") {
                    response = PreauthorizeAfterEnroll90(request);
                    if (response.preAuth.transaction.processing_status.FunctionResult == "ACK") {
                        response = PreauthorizeAfterEnroll120(request);
                    }
                } else {
                    response = PreauthorizeAfterEnroll120(request);
                }
            } catch (Exception ex) {
                response = new PreauthResponse() {
                    preAuth = new PreauthResponse.ResponseFunction() {
                        transaction = new PreauthResponse.Transaction() {
                            processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new PreauthResponse.Transaction.ProcessingStatus.Error() {
                                    type = "SYSTEM", // "DATA"
                                    number = ((UInt32)ex.HResult).ToString(),
                                    message = ex.Message,
                                }
                            }
                        }
                    }
                };
            }
            return response;
        }
        private PreauthResponse PreauthorizeAfterEnroll90(PreauthRequest request) {
            PreauthResponse response = null;

            try {
                var kalixaPre90 = RequestTransform.getKalixaCaptureForPreauth90(request);

                string xml = kalixaPre90.getXml();

                // Console.WriteLine("Kalixa Preauthorization 90 request: {0}", xml);

                string res = client.ProcessRequest(kalixaPre90);

                // Console.WriteLine("Kalixa Preauthorization 90 response: {0}", res);
                try {
                    // for preauth after enroll we are using executePayment request... and response for this request is in CaptureResponse format
                    Kalixa.Entities.Response.CaptureResponse resp = Kalixa.Entities.Response.CaptureResponse.DeserializeFromString(res);
                    if (resp.statusCode == "0" &&
                        (resp.actionResults.GetValueByKey("lastStateDefinition") == "304" || resp.actionResults.GetValueByKey("lastStateDefinition") == "291")) {
                        // success
                        response = new PreauthResponse() {
                            preAuth = new PreauthResponse.ResponseFunction() {
                                transaction = new PreauthResponse.Transaction() {
                                    reference_id = request.preAuth.transaction.reference_id,
                                    processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                        CreditCardAlias = request.preAuth.transaction.credit_card_alias,
                                        ProviderTransactionID = request.preAuth.transaction.provider_transaction_id,
                                        FunctionResult = "ACK",
                                    }
                                }
                            }
                        };
                    } else {
                        response = new PreauthResponse() {
                            preAuth = new PreauthResponse.ResponseFunction() {
                                transaction = new PreauthResponse.Transaction() {
                                    reference_id = request.preAuth.transaction.reference_id,
                                    processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                        ProviderTransactionID = request.preAuth.transaction.provider_transaction_id,
                                        FunctionResult = "NOK",
                                        error = new PreauthResponse.Transaction.ProcessingStatus.Error() {
                                            type = "PROVIDER", // "DATA"
                                            number = resp.statusCode,
                                            message = "Status code: " + resp.actionResults.GetValueByKey("lastStateDefinition") + "/" + Helpers.StatusCodes.getStatusCodeMessage(resp.actionResults.GetValueByKey("lastStateDefinition")),
                                        }
                                    }
                                }
                            }
                        };                        
                    }
                } catch {
                    Kalixa.Entities.Response.ExceptionResponse resp = Kalixa.Entities.Response.ExceptionResponse.DeserializeFromString(res);
                    response = new PreauthResponse() {
                        preAuth = new PreauthResponse.ResponseFunction() {
                            transaction = new PreauthResponse.Transaction() {
                                processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                    FunctionResult = "NOK",
                                    error = new PreauthResponse.Transaction.ProcessingStatus.Error() {
                                        type = "PROVIDER", // "DATA"
                                        number = resp.errorCode,
                                        message = resp.errorMessage,
                                    }
                                }
                            }
                        }
                    };
                }
            } catch (Exception ex) {
                response = new PreauthResponse() {
                    preAuth = new PreauthResponse.ResponseFunction() {
                        transaction = new PreauthResponse.Transaction() {
                            processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new PreauthResponse.Transaction.ProcessingStatus.Error() {
                                    type = "SYSTEM", // "DATA"
                                    number = ((UInt32)ex.HResult).ToString(),
                                    message = ex.Message,
                                }
                            }
                        }
                    }
                };

            }
            return response;
        }
        private PreauthResponse PreauthorizeAfterEnroll120(PreauthRequest request) {
            PreauthResponse response = null;

            try {
                var kalixaPre120 = RequestTransform.getKalixaCaptureForPreauth120(request);

                string xml = kalixaPre120.getXml();

                // Console.WriteLine("Kalixa Preauthorization 120 request: {0}", xml);

                string res = client.ProcessRequest(kalixaPre120);

                // Console.WriteLine("Kalixa Preauthorization 120 response: {0}", res);
                try {
                    // for preauth after enroll we are using executePayment request... and response for this request is in CaptureResponse format
                    Kalixa.Entities.Response.CaptureResponse resp = Kalixa.Entities.Response.CaptureResponse.DeserializeFromString(res);
                    if (resp.statusCode == "0" &&
                        (resp.actionResults.GetValueByKey("lastStateDefinition") == "13")) {
                        // success
                        response = new PreauthResponse() {
                            preAuth = new PreauthResponse.ResponseFunction() {
                                transaction = new PreauthResponse.Transaction() {
                                    reference_id = request.preAuth.transaction.reference_id,
                                    processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                        CreditCardAlias = request.preAuth.transaction.credit_card_alias,
                                        ProviderTransactionID = request.preAuth.transaction.provider_transaction_id,
                                        FunctionResult = "ACK",
                                    }
                                }
                            }
                        };
                    } else {
                        response = new PreauthResponse() {
                            preAuth = new PreauthResponse.ResponseFunction() {
                                transaction = new PreauthResponse.Transaction() {
                                    reference_id = request.preAuth.transaction.reference_id,
                                    processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                        ProviderTransactionID = request.preAuth.transaction.provider_transaction_id,
                                        FunctionResult = "NOK",
                                        error = new PreauthResponse.Transaction.ProcessingStatus.Error() {
                                            type = "PROVIDER", // "DATA"
                                            number = resp.statusCode,
                                            message = "Status code: " + resp.actionResults.GetValueByKey("lastStateDefinition") + "/" + Helpers.StatusCodes.getStatusCodeMessage(resp.actionResults.GetValueByKey("lastStateDefinition")),
                                        }
                                    }
                                }
                            }
                        };
                    }
                } catch {
                    Kalixa.Entities.Response.ExceptionResponse resp = Kalixa.Entities.Response.ExceptionResponse.DeserializeFromString(res);
                    response = new PreauthResponse() {
                        preAuth = new PreauthResponse.ResponseFunction() {
                            transaction = new PreauthResponse.Transaction() {
                                processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                    FunctionResult = "NOK",
                                    error = new PreauthResponse.Transaction.ProcessingStatus.Error() {
                                        type = "PROVIDER", // "DATA"
                                        number = resp.errorCode,
                                        message = resp.errorMessage,
                                    }
                                }
                            }
                        }
                    };
                }
            } catch (Exception ex) {
                response = new PreauthResponse() {
                    preAuth = new PreauthResponse.ResponseFunction() {
                        transaction = new PreauthResponse.Transaction() {
                            processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new PreauthResponse.Transaction.ProcessingStatus.Error() {
                                    type = "SYSTEM", // "DATA"
                                    number = ((UInt32)ex.HResult).ToString(),
                                    message = ex.Message,
                                }
                            }
                        }
                    }
                };

            }
            return response;
        }

        public CaptureResponse Capture(CaptureRequest request) {
            CaptureResponse response = null;

            try {
                var kalixaCapture = RequestTransform.getKalixaCapture(request);
                string xml = kalixaCapture.getXml();

                // Console.WriteLine("Kalixa Capture request: {0}", xml);

                string res = client.ProcessRequest(kalixaCapture);

                // Console.WriteLine("Kalixa Capture response: {0}", res);
                try {
                    Kalixa.Entities.Response.CaptureResponse resp = Kalixa.Entities.Response.CaptureResponse.DeserializeFromString(res);
                    if (resp.actionResults.GetValueByKey("lastStateDefinition") == "306") {
                        // success
                        response = new CaptureResponse() {
                            capture = new CaptureResponse.ResponseFunction() {
                                transaction = new CaptureResponse.Transaction() {
                                    reference_id = request.capture.transaction.reference_id,
                                    processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                        CreditCardAlias = request.capture.transaction.credit_card_alias,
                                        ProviderTransactionID = request.capture.transaction.provider_transaction_id,
                                        FunctionResult = "ACK",
                                    }
                                }
                            }
                        };
                    } else {
                        response = new CaptureResponse() {
                            capture = new CaptureResponse.ResponseFunction() {
                                transaction = new CaptureResponse.Transaction() {
                                    reference_id = request.capture.transaction.reference_id,
                                    processing_status = new CaptureResponse.Transaction.ProcessingStatus() {
                                        ProviderTransactionID = request.capture.transaction.provider_transaction_id,
                                        FunctionResult = "NOK",
                                        error = new CaptureResponse.Transaction.ProcessingStatus.Error() {
                                            type = "PROVIDER", // "DATA"
                                            number = resp.statusCode,
                                            message = "Status code: " + resp.actionResults.GetValueByKey("lastStateDefinition")  + "/" + Helpers.StatusCodes.getStatusCodeMessage(resp.actionResults.GetValueByKey("lastStateDefinition")),
                                        }
                                    }
                                }
                            }
                        };
                    }
                } catch {
                    Kalixa.Entities.Response.ExceptionResponse resp = Kalixa.Entities.Response.ExceptionResponse.DeserializeFromString(res);
                    response = new CaptureResponse() {
                        capture = new CaptureResponse.ResponseFunction() {
                            transaction = new CaptureResponse.Transaction() {
                                processing_status = new CaptureResponse.Transaction.ProcessingStatus() {
                                    FunctionResult = "NOK",
                                    error = new CaptureResponse.Transaction.ProcessingStatus.Error() {
                                        type = "PROVIDER", // "DATA"
                                        number = resp.errorCode,
                                        message = resp.errorMessage,
                                    }
                                }
                            }
                        }
                    };
                }
            } catch (Exception ex) {
                response = new CaptureResponse() {
                    capture = new CaptureResponse.ResponseFunction() {
                        transaction = new CaptureResponse.Transaction() {
                            processing_status = new CaptureResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new CaptureResponse.Transaction.ProcessingStatus.Error() {
                                    type = "SYSTEM", // "DATA"
                                    number = ((UInt32)ex.HResult).ToString(),
                                    message = ex.Message,
                                }
                            }
                        }
                    }
                };

            }
            return response;
        }

        // Refund - TODO - make implementation this request in two - preauth + capture
        public PurchaseResponse Purchase(PurchaseRequest request) {
            PurchaseResponse response = null;
            try {
                PreauthRequest preAuth = new PreauthRequest {
                    preAuth = new PreauthRequest.PreAuthorization() {
                        configurations = request.purchase.configurations,
                        transaction = new PreauthRequest.PreAuthorization.Transaction() {
                            credit_card_alias = request.purchase.transaction.credit_card_alias,
                            amount = request.purchase.transaction.amount,
                            cred_card_data = request.purchase.transaction.cred_card_data,
                            currency = request.purchase.transaction.currency,
                            customer_data = request.purchase.transaction.customer_data,
                            provider_transaction_id = request.purchase.transaction.provider_transaction_id,
                            recurring_transaction = request.purchase.transaction.recurring_transaction,
                            reference_id = request.purchase.transaction.reference_id,
                            threed_secure = request.purchase.transaction.threed_secure,
                            usage = request.purchase.transaction.usage,
                        },
                    }
                };
                PreauthResponse preAuthResp = Preauthorize(preAuth);
                if (preAuthResp.preAuth.transaction.processing_status.FunctionResult == "ACK") {
                    CaptureRequest capture = new CaptureRequest {
                        capture = new CaptureRequest.Capture() {
                            configurations = request.purchase.configurations,
                            transaction = new CaptureRequest.Capture.Transaction() {
                                credit_card_alias = preAuthResp.preAuth.transaction.processing_status.CreditCardAlias,
                                amount = request.purchase.transaction.amount,
                                cred_card_data = request.purchase.transaction.cred_card_data,
                                currency = request.purchase.transaction.currency,
                                provider_transaction_id = preAuthResp.preAuth.transaction.processing_status.ProviderTransactionID,
                                recurring_transaction = request.purchase.transaction.recurring_transaction,
                                reference_id = request.purchase.transaction.reference_id,
                                threed_secure = request.purchase.transaction.threed_secure,
                                usage = request.purchase.transaction.usage,
                                capture_type = "FULL",
                            },
                        }
                    };
                    CaptureResponse captureResp = Capture(capture);
                    response = new PurchaseResponse() {
                        purchase = captureResp.capture,
                    };
                } else {
                    response = new PurchaseResponse() {
                        purchase = preAuthResp.preAuth,
                    };
                }
            } catch (Exception ex) {
                response = new PurchaseResponse() {
                    purchase = new PurchaseResponse.ResponseFunction() {
                        transaction = new PurchaseResponse.Transaction() {
                            processing_status = new PurchaseResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new PurchaseResponse.Transaction.ProcessingStatus.Error() {
                                    type = "SYSTEM", // "DATA"
                                    number = ((UInt32)ex.HResult).ToString(),
                                    message = ex.Message,
                                }
                            }
                        }
                    }
                };
            }
            return response;
        }

        // Refund - TODO - test it
        public RefundResponse Refund(RefundRequest request) {
            RefundResponse response = null;

            try {
                var kalixaRefund = RequestTransform.getKalixaRefund(request);
                string xml = kalixaRefund.getXml();

                // Console.WriteLine("Kalixa Refund request: {0}", xml);

                string res = client.ProcessRequest(kalixaRefund);

                // Console.WriteLine("Kalixa Refund response: {0}", res);
                try {
                    Kalixa.Entities.Response.RefundResponse resp = Kalixa.Entities.Response.RefundResponse.DeserializeFromString(res);
                    if (resp.payment.state.definition.key == "306") {
                        // success
                        response = new RefundResponse() {
                            refund = new RefundResponse.ResponseFunction() {
                                transaction = new RefundResponse.Transaction() {
                                    reference_id = request.refund.transaction.reference_id,
                                    processing_status = new RefundResponse.Transaction.ProcessingStatus() {
                                        CreditCardAlias = resp.payment.userID,
                                        ProviderTransactionID = resp.payment.paymentID,
                                        FunctionResult = "ACK",
                                    }
                                }
                            }
                        };
                    } else {
                        response = new RefundResponse() {
                            refund = new RefundResponse.ResponseFunction() {
                                transaction = new RefundResponse.Transaction() {
                                    reference_id = request.refund.transaction.reference_id,
                                    processing_status = new RefundResponse.Transaction.ProcessingStatus() {
                                        ProviderTransactionID = request.refund.transaction.provider_transaction_id,
                                        FunctionResult = "NOK",
                                        error = new RefundResponse.Transaction.ProcessingStatus.Error() {
                                            type = "PROVIDER", // "DATA"
                                            number = resp.payment.state.definition.key,
                                            message = resp.payment.state.definition.value,
                                        }
                                    }
                                }
                            }
                        };
                    }
                } catch {
                    Kalixa.Entities.Response.ExceptionResponse resp = Kalixa.Entities.Response.ExceptionResponse.DeserializeFromString(res);
                    response = new RefundResponse() {
                        refund = new RefundResponse.ResponseFunction() {
                            transaction = new RefundResponse.Transaction() {
                                processing_status = new RefundResponse.Transaction.ProcessingStatus() {
                                    FunctionResult = "NOK",
                                    error = new RefundResponse.Transaction.ProcessingStatus.Error() {
                                        type = "PROVIDER", // "DATA"
                                        number = resp.errorCode,
                                        message = resp.errorMessage,
                                    }
                                }
                            }
                        }
                    };
                }
            } catch (Exception ex) {
                response = new RefundResponse() {
                    refund = new RefundResponse.ResponseFunction() {
                        transaction = new RefundResponse.Transaction() {
                            processing_status = new RefundResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new RefundResponse.Transaction.ProcessingStatus.Error() {
                                    type = "SYSTEM", // "DATA"
                                    number = ((UInt32)ex.HResult).ToString(),
                                    message = ex.Message,
                                }
                            }
                        }
                    }
                };

            }
            return response;
        }

        // Reversal - TODO - test it
        public ReversalResponse Reversal(ReversalRequest request) {
            ReversalResponse response = null;

            try {
                var kalixaVoid = RequestTransform.getKalixaVoid(request);
                string xml = kalixaVoid.getXml();

                // Console.WriteLine("Kalixa Void request: {0}", xml);

                string res = client.ProcessRequest(kalixaVoid);

                // Console.WriteLine("Kalixa Void response: {0}", res);
                try {
                    Kalixa.Entities.Response.ReversalResponse resp = Kalixa.Entities.Response.ReversalResponse.DeserializeFromString(res);
                    if (resp.actionResults.GetValueByKey("lastStateDefinition") == "113") {
                        // success
                        response = new ReversalResponse() {
                            reversal = new ReversalResponse.ResponseFunction() {
                                transaction = new ReversalResponse.Transaction() {
                                    reference_id = request.reversal.transaction.reference_id,
                                    processing_status = new ReversalResponse.Transaction.ProcessingStatus() {
                                        CreditCardAlias = request.reversal.transaction.credit_card_alias,
                                        ProviderTransactionID = request.reversal.transaction.provider_transaction_id,
                                        FunctionResult = "ACK",
                                    }
                                }
                            }
                        };
                    } else {
                        response = new ReversalResponse() {
                            reversal = new ReversalResponse.ResponseFunction() {
                                transaction = new ReversalResponse.Transaction() {
                                    reference_id = request.reversal.transaction.reference_id,
                                    processing_status = new ReversalResponse.Transaction.ProcessingStatus() {
                                        ProviderTransactionID = request.reversal.transaction.provider_transaction_id,
                                        FunctionResult = "NOK",
                                        error = new ReversalResponse.Transaction.ProcessingStatus.Error() {
                                            type = "PROVIDER", // "DATA"
                                            number = resp.statusCode,
                                            message = "Unsuccess status :" + resp.statusCode,
                                        }
                                    }
                                }
                            }
                        };
                    }
                } catch {
                    Kalixa.Entities.Response.ExceptionResponse resp = Kalixa.Entities.Response.ExceptionResponse.DeserializeFromString(res);
                    response = new ReversalResponse() {
                        reversal = new ReversalResponse.ResponseFunction() {
                            transaction = new ReversalResponse.Transaction() {
                                processing_status = new ReversalResponse.Transaction.ProcessingStatus() {
                                    FunctionResult = "NOK",
                                    error = new ReversalResponse.Transaction.ProcessingStatus.Error() {
                                        type = "PROVIDER", // "DATA"
                                        number = resp.errorCode,
                                        message = resp.errorMessage,
                                    }
                                }
                            }
                        }
                    };
                }
            } catch (Exception ex) {
                response = new ReversalResponse() {
                    reversal = new ReversalResponse.ResponseFunction() {
                        transaction = new ReversalResponse.Transaction() {
                            processing_status = new ReversalResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new ReversalResponse.Transaction.ProcessingStatus.Error() {
                                    type = "SYSTEM", // "DATA"
                                    number = ((UInt32)ex.HResult).ToString(),
                                    message = ex.Message,
                                }
                            }
                        }
                    }
                };

            }
            return response;
        }

        // Enrollment Check - TODO - test it
        public EnrollmentCheck3DResponse EnrollmentCheck3D(EnrollmentCheck3DRequest request) {
            EnrollmentCheck3DResponse response = null;

            try {
                var kalixEnroll = RequestTransform.getKalixaEnrollmentCheck(request);
                string xml = kalixEnroll.getXml();

                // Console.WriteLine("Kalixa Enrollment Check request: {0}", xml);

                string res = client.ProcessRequest(kalixEnroll);

                // Console.WriteLine("Kalixa Enrollment Check response: {0}", res);

                try {
                    Kalixa.Entities.Response.EnrollmentCheckResponse resp = Kalixa.Entities.Response.EnrollmentCheckResponse.DeserializeFromString(res);
                    // 3D enrolled
                    if (resp.payment.state.definition.key == "287") {
                        response = new EnrollmentCheck3DResponse() {
                            enrolment_check = new EnrollmentCheck3DResponse.ResponseFunction() {
                                transaction = new EnrollmentCheck3DResponse.Transaction() {
                                    processing_status = new EnrollmentCheck3DResponse.Transaction.ProcessingStatus() {
                                        StatusType = "Y",
                                        FunctionResult = "ACK",
                                        ProviderTransactionID = resp.payment.paymentID,
                                        CreditCardAlias = resp.payment.userID,
                                    },
                                    reference_id = resp.payment.merchantTransactionID,
                                    secure3D = new Response.Transaction.Secure3D() {
                                        acs_url = resp.payment.state.paymentStateDetails.GetValueByKey("RedirectionUrl"),
                                        md = resp.payment.state.paymentStateDetails.GetValueByKey("PostDataMD"),
                                        pa_req = resp.payment.state.paymentStateDetails.GetValueByKey("PostDataPaReq"),
                                        term_url = resp.payment.state.paymentStateDetails.GetValueByKey("PostDataTermUrl"),
                                    },
                                },
                            },
                        };
                    }
                    // non 3D enrolled
                    else if (resp.payment.state.definition.key == "283" || resp.payment.state.definition.key == "284" ||
                        resp.payment.state.definition.key == "285" || resp.payment.state.definition.key == "302") {
                        response = new EnrollmentCheck3DResponse() {
                            enrolment_check = new EnrollmentCheck3DResponse.ResponseFunction() {
                                transaction = new EnrollmentCheck3DResponse.Transaction() {
                                    processing_status = new EnrollmentCheck3DResponse.Transaction.ProcessingStatus() {
                                        StatusType = (resp.payment.state.definition.key == "284" || resp.payment.state.definition.key == "285") ? "N" : "U",
                                        FunctionResult = "ACK",
                                        ProviderTransactionID = resp.payment.paymentID,
                                        CreditCardAlias = resp.payment.userID,
                                    },
                                    reference_id = resp.payment.merchantTransactionID,
                                    secure3D = new Response.Transaction.Secure3D() {
                                        acs_url = resp.payment.state.paymentStateDetails.GetValueByKey("RedirectionUrl"),
                                        md = resp.payment.state.paymentStateDetails.GetValueByKey("PostDataMD"),
                                        pa_req = resp.payment.state.paymentStateDetails.GetValueByKey("PostDataPaReq"),
                                        term_url = resp.payment.state.paymentStateDetails.GetValueByKey("PostDataTermUrl"),
                                    },
                                },
                            },
                        };
                    } else {
                        response = new EnrollmentCheck3DResponse() {
                            enrolment_check = new EnrollmentCheck3DResponse.ResponseFunction() {
                                transaction = new EnrollmentCheck3DResponse.Transaction() {
                                    processing_status = new EnrollmentCheck3DResponse.Transaction.ProcessingStatus() {
                                        FunctionResult = "NOK",
                                        ProviderTransactionID = resp.payment.paymentID,
                                        error = new ReversalResponse.Transaction.ProcessingStatus.Error() {
                                            type = "PROVIDER", // "DATA"
                                            number = resp.payment.state.definition.value,
                                            message = resp.payment.state.definition.key + " : " + resp.payment.state.description,
                                        }
                                    },
                                    reference_id = resp.payment.merchantTransactionID,
                                },
                            },
                        };
                    }
                } catch {
                    Kalixa.Entities.Response.ExceptionResponse resp = Kalixa.Entities.Response.ExceptionResponse.DeserializeFromString(res);
                    response = new EnrollmentCheck3DResponse() {
                        enrolment_check = new EnrollmentCheck3DResponse.ResponseFunction() {
                            transaction = new EnrollmentCheck3DResponse.Transaction() {
                                processing_status = new EnrollmentCheck3DResponse.Transaction.ProcessingStatus() {
                                    FunctionResult = "NOK",
                                    error = new ReversalResponse.Transaction.ProcessingStatus.Error() {
                                        type = "PROVIDER", // "DATA"
                                        number = resp.errorCode,
                                        message = resp.errorMessage,
                                    }
                                },
                            },
                        },
                    };
                }
            } catch (Exception ex) {
                response = new EnrollmentCheck3DResponse() {
                    enrolment_check = new EnrollmentCheck3DResponse.ResponseFunction() {
                        transaction = new EnrollmentCheck3DResponse.Transaction() {
                            processing_status = new EnrollmentCheck3DResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new EnrollmentCheck3DResponse.Transaction.ProcessingStatus.Error() {
                                    type = "SYSTEM", // "DATA"
                                    number = ((UInt32)ex.HResult).ToString(),
                                    message = ex.Message,
                                }
                            }
                        }
                    }
                };

            }
            return response;
        }

        // Preauth 3D - not supported
        public Preauth3DResponse Preauthorize3D(Preauth3DRequest request) {
            Preauth3DResponse response = new Preauth3DResponse() {
                preAuth3D = new Preauth3DResponse.ResponseFunction() {
                    transaction = new Preauth3DResponse.Transaction() {
                        processing_status = new Preauth3DResponse.Transaction.ProcessingStatus() {
                            FunctionResult = "NOK",
                            error = new Preauth3DResponse.Transaction.ProcessingStatus.Error() {
                                type = "PROVIDER",
                                number = ErrorCodes.InvalidTransactionTypeError.ToString(),
                                message = "Kalixa aquirer doesn't support 'Preauthorization 3D' request",
                            },
                        },                        
                    },
                }
            };
            return response;
        }

        // Purchase 3D - not supported
        public Purchase3DResponse Purchase3D(Purchase3DRequest request) {
            Purchase3DResponse response = new Purchase3DResponse() {
                purchase3D = new Purchase3DResponse.ResponseFunction() {
                    transaction = new Purchase3DResponse.Transaction() {
                        processing_status = new Purchase3DResponse.Transaction.ProcessingStatus() {
                            FunctionResult = "NOK",
                            error = new Purchase3DResponse.Transaction.ProcessingStatus.Error() {
                                type = "PROVIDER",
                                number = ErrorCodes.InvalidTransactionTypeError.ToString(),
                                message = "Kalixa aquirer doesn't support 'Purchase 3D' request",
                            },
                        },
                    },
                }
            };
            return response;
        }

        // Notification Processing - not supported
        public NotificationProcessingResponse NotificationProcessing(NotificationProcessingRequest request) {
            NotificationProcessingResponse response = new NotificationProcessingResponse() {
                notification = new NotificationProcessingResponse.NotificationProcessingSection() {
                    transaction = new NotificationProcessingResponse.NotificationProcessingSection.Transaction() {
                        transaction_type = "NONE",
                        processing_status = new NotificationProcessingResponse.NotificationProcessingSection.Transaction.ProcessingStatus() {
                            FunctionResult = "NOK",
                        },
                        error = new NotificationProcessingResponse.NotificationProcessingSection.Transaction.Error() {
                            type = "PROVIDER",
                            number = ErrorCodes.InvalidTransactionTypeError.ToString(),
                            message = "Kalixa aquirer doesn't support 'Notification Processing' request",
                        },
                    },
                    raw_data = new NotificationProcessingResponse.NotificationProcessingSection.RawResponse() {
                    }
                }
            };
            return response;
        }

        // Evaluate Provider Response - not supported
        public EvaluateProviderResponseResponse EvaluateProviderResponse(EvaluateProviderResponseRequest request) {
            EvaluateProviderResponseResponse response = new EvaluateProviderResponseResponse() {
                evaluate_provider = new EvaluateProviderResponseResponse.EvaluateProviderResponseSection() {
                    transaction = new EvaluateProviderResponseResponse.EvaluateProviderResponseSection.Transaction() {
                        transaction_type = "NONE",
                        processing_status = new EvaluateProviderResponseResponse.EvaluateProviderResponseSection.Transaction.ProcessingStatus() {
                            FunctionResult = "NOK",
                        },
                        error = new EvaluateProviderResponseResponse.EvaluateProviderResponseSection.Transaction.Error() {
                            type = "PROVIDER",
                            number = ErrorCodes.InvalidTransactionTypeError.ToString(),
                            message = "Kalixa aquirer doesn't support 'Evaluate Provider Response' request",
                        },
                    },
                }
            };
            return response;
        }

        // Single Reconcile - TODO - test it
        public SingleReconcileResponse SingleReconcile(SingleReconcileRequest request) {
            Fibonatix.CommDoo.Kalixa.Entities.Requests.SingleReconcileRequest reconcile = null;
            SingleReconcileResponse response = null;
            try {
                reconcile = RequestTransform.getKalixaSingleReconcile(request);

                string xml = reconcile.getXml();

                // Console.WriteLine("Kalixa Reconcile request: {0}", xml);

                string res = client.ProcessRequest(reconcile);

                // Console.WriteLine("Kalixa Reconcile response: {0}", res);

                try {
                    Kalixa.Entities.Response.SingeReconcileResponse resp = Kalixa.Entities.Response.SingeReconcileResponse.DeserializeFromString(res);

                    response = new SingleReconcileResponse() {
                        reconcile = new SingleReconcileResponse.ResponseReconcileFunction() {
                            transaction = new SingleReconcileResponse.Transaction() {
                                processing_status = new SingleReconcileResponse.Transaction.ProcessingStatus() {
                                },
                            }
                        }
                    };

                    Kalixa.Entities.Response.SingeReconcileResponse.paymentWithPaymentAccount payment = resp.paymentStateDetails.GetValueByKey(request.reconcile.transaction.provider_transaction_id);


                    if (payment!= null) {
                        response.reconcile.type = payment.paymentMethod.value;
                        response.reconcile.message = payment.paymentMethod.value;
                        response.reconcile.amount = Fibonatix.CommDoo.Helpers.Convertors.MajorAmountToMinor(Decimal.Parse(payment.amount.value, NumberStyles.Currency, CultureInfo.InvariantCulture), Fibonatix.CommDoo.Helpers.Currencies.currencyCodeFromString(payment.amount.currencyCode)).ToString();
                        response.reconcile.currency = payment.amount.currencyCode;
                        response.reconcile.ext_status = payment.state.definition.value;
                        response.reconcile.transaction.reference_id = payment.merchantTransactionID;
                        response.reconcile.transaction.processing_status.ProviderTransactionID = payment.paymentID;
                        response.reconcile.transaction.processing_status.FunctionResult = "ACK";
                        response.reconcile.transaction.processing_status.CreditCardAlias = payment.userID;
                        // response.reconcile.transaction.processing_status.

                    } else {
                        response.reconcile.transaction.processing_status.FunctionResult = "NOK";
                        response.reconcile.ext_status = "";
                        response.reconcile.transaction.processing_status.error = new SingleReconcileResponse.Transaction.ProcessingStatus.Error() {
                            type = "PROVIDER", // "REJECTED"
                            number = ErrorCodes.ReferenceNotFoundError.ToString(),
                            message = "Cannot find transaction with ID = " + request.reconcile.transaction.provider_transaction_id,
                        };
                    }
                } catch {
                    Kalixa.Entities.Response.ExceptionResponse resp = Kalixa.Entities.Response.ExceptionResponse.DeserializeFromString(res);
                    response = new SingleReconcileResponse() {
                        reconcile = new SingleReconcileResponse.ResponseReconcileFunction() {
                            transaction = new SingleReconcileResponse.Transaction() {
                                processing_status = new SingleReconcileResponse.Transaction.ProcessingStatus() {
                                    FunctionResult = "NOK",
                                    error = new SingleReconcileResponse.Transaction.ProcessingStatus.Error() {
                                        type = "PROVIDER", // "DATA"
                                        number = resp.errorCode,
                                        message = resp.errorMessage,
                                    }
                                }
                            }
                        }
                    };
                }

            } catch (System.ComponentModel.DataAnnotations.ValidationException ex) {
                response = new SingleReconcileResponse() {
                    reconcile = new SingleReconcileResponse.ResponseReconcileFunction() {
                        transaction = new SingleReconcileResponse.Transaction() {
                            reference_id = request.reconcile.transaction.reference_id,
                            processing_status = new SingleReconcileResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new SingleReconcileResponse.Transaction.ProcessingStatus.Error() {
                                    type = "SYSTEM", // "DATA"
                                    number = ((UInt32)ex.HResult).ToString(),
                                    message = ex.Message,
                                }
                            }
                        }
                    }
                };
            } catch (Exception ex) {
                response = new SingleReconcileResponse() {
                    reconcile = new SingleReconcileResponse.ResponseReconcileFunction() {
                        transaction = new SingleReconcileResponse.Transaction() {
                            reference_id = request.reconcile.transaction.reference_id,
                            processing_status = new SingleReconcileResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new SingleReconcileResponse.Transaction.ProcessingStatus.Error() {
                                    type = "SYSTEM",
                                    number = ((UInt32)ex.HResult).ToString(),
                                    message = ex.Message,
                                }
                            }
                        }
                    }
                };
            } finally { }

            return response;
        }
    }
}
