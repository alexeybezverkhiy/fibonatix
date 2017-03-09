using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Genesis.Net.Errors;
using Fibonatix.CommDoo.Requests;
using Fibonatix.CommDoo.Responses;
using System.Globalization;

namespace Fibonatix.CommDoo.Borgun
{
    internal class BorgunConnector : IConnector
    {
        static private Dictionary<string, BorgunConnector> allconnectors;

        Network.Client client;

        public BorgunConnector(string login, string password, bool sandbox) {
            client = new Network.Client(login, password, sandbox);
        }

        static public BorgunConnector getConnector(string login, string password, bool sandbox) {
            if (login == null || password == null)
                return null;
            string key = login + ":" + password + ":" + sandbox.ToString();
            if (allconnectors == null) allconnectors = new Dictionary<string, BorgunConnector>();
            if (!allconnectors.ContainsKey(key)) {
                BorgunConnector conn = new BorgunConnector(login, password, sandbox);
                allconnectors[key] = conn;
            }
            return allconnectors[key];
        }

        static public BorgunConnector getConnector(Request request) {
            bool sanbox = false;
            sanbox = String.Equals(request.getConfigValue("testmode"), "true", StringComparison.OrdinalIgnoreCase);
            return getConnector(request.getConfigValue("login"), request.getConfigValue("password"), sanbox);
        }


        internal string getVirtualCardNumber(string PAN, string MerchantContractNumber) {

            string virtualCard = null;

            try {

                Entities.Requests.VirtualCardRequest vcRequest = new Entities.Requests.VirtualCardRequest() {
                    MerchantContractNumber = MerchantContractNumber,
                    PAN = PAN,
                };
                Entities.Requests.SOAPVirtualCardRequest soapRequest = new Entities.Requests.SOAPVirtualCardRequest() {
                    Body = new Entities.Requests.SOAPVirtualCardRequest.SOAPBody() {
                        getVirtualCard = new Entities.Requests.SOAPVirtualCardRequest.SOAPBody.RequestContainer() {
                            virtualCardRequestXML = vcRequest.getXml()
                        }
                    }
                };

                string xml = soapRequest.getXml();
                Console.WriteLine("Borgun virtual card soap request: {0}", xml);
                string res = client.ProcessRequest(soapRequest);
                Console.WriteLine("Borgun virtual card soap response: {0}", res);

                Entities.Responses.SOAPVirtualCardResponse resp = Entities.Responses.SOAPVirtualCardResponse.DeserializeFromString(res);

                Console.WriteLine("Borgun virtual card xml response: {0}", resp.Body.getVirtualCardResponse.virtualCardResponseXML);

                Entities.Responses.VirtualCardResponse xmlResp = Entities.Responses.VirtualCardResponse.DeserializeFromString(resp.Body.getVirtualCardResponse.virtualCardResponseXML);

                if (xmlResp.Status.ResultCode == "0" && xmlResp.VirtualCard != null)
                    virtualCard = xmlResp.VirtualCard;
            }
            catch (Exception e) {
                Console.WriteLine("Exception: {0}", e.Message);
            }
            return virtualCard;
        }

        public PreauthResponse Preauthorize(PreauthRequest request) {

            PreauthResponse response = null;

            try {
                string virtualCard = null;

                if (request.getRequestType() == Requests.RequestType.Initial) {
                    virtualCard = getVirtualCardNumber(request.preAuth.transaction.cred_card_data.credit_card_number, request.getConfigValue("MerchantContractNumber"));
                } else if (request.getRequestType() == Requests.RequestType.Repeated) {
                    virtualCard = request.preAuth.transaction.credit_card_alias;
                }

                var borgunPre = RequestTransform.getBorgunAuthorize(request, virtualCard);

                string xml = borgunPre.getXml();
                Console.WriteLine("Borgun Preauthorization request: {0}", xml);
                string res = client.ProcessRequest(borgunPre);
                Console.WriteLine("Borgun Preauthorization response: {0}", res);
                Entities.Responses.SOAPAuthResponse resp = Entities.Responses.SOAPAuthResponse.DeserializeFromString(res);
                Console.WriteLine("Borgun Preauthorization xml response: {0}", resp.Body.getAuthorizationOutput.getAuthResXml);
                Entities.Responses.AuthResponse xmlResp = Entities.Responses.AuthResponse.DeserializeFromString(resp.Body.getAuthorizationOutput.getAuthResXml);

                if (Int32.Parse(xmlResp.ActionCode) == 0) {
                    response = new PreauthResponse() {
                        preAuth = new PreauthResponse.ResponseFunction() {
                            transaction = new PreauthResponse.Transaction() {
                                reference_id = request.preAuth.transaction.reference_id,                                
                                processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                    ProviderTransactionID = xmlResp.Transaction,
                                    RRN = xmlResp.RRN,
                                    DateAndTime = xmlResp.DateAndTime,
                                    AuthCode = xmlResp.AuthCode,
                                    TerminalID = xmlResp.TerminalID,
                                    CreditCardAlias = virtualCard,
                                    FunctionResult = "ACK",
                                }
                            }
                        }
                    };
                } else {
                    response = new PreauthResponse() {
                        preAuth = new PreauthResponse.ResponseFunction() {
                            transaction = new PreauthResponse.Transaction() {
                                processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                    FunctionResult = "NOK",
                                    error = new PreauthResponse.Transaction.ProcessingStatus.Error() {
                                        type = "PROVIDER", // "DATA"
                                        number = xmlResp.ActionCode,
                                        message = xmlResp.Message != null ? xmlResp.Message : Helpers.ActionCodes.getActionCodeMessage(xmlResp.ActionCode),
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
                                    type = "SYSTEM",
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
                var borgunCapture = RequestTransform.getBorgunCapture(request);
                string xml = borgunCapture.getXml();

                Console.WriteLine("Borgun Capture request: {0}", xml);
                string res = client.ProcessRequest(borgunCapture);
                Console.WriteLine("Borgun Capture response: {0}", res);
                Entities.Responses.SOAPAuthResponse resp = Entities.Responses.SOAPAuthResponse.DeserializeFromString(res);
                Console.WriteLine("Borgun Capture xml response: {0}", resp.Body.getAuthorizationOutput.getAuthResXml);
                Entities.Responses.AuthResponse xmlResp = Entities.Responses.AuthResponse.DeserializeFromString(resp.Body.getAuthorizationOutput.getAuthResXml);

                if (Int32.Parse(xmlResp.ActionCode) == 0) {
                    response = new CaptureResponse() {
                        capture = new CaptureResponse.ResponseFunction() {
                            transaction = new CaptureResponse.Transaction() {
                                reference_id = request.capture.transaction.reference_id,
                                processing_status = new CaptureResponse.Transaction.ProcessingStatus() {
                                    ProviderTransactionID = xmlResp.Transaction,
                                    RRN = xmlResp.RRN,
                                    DateAndTime = xmlResp.DateAndTime,
                                    TerminalID = xmlResp.TerminalID,
                                    FunctionResult = "ACK",
                                }
                            }
                        }
                    };
                } else {
                    response = new CaptureResponse() {
                        capture = new CaptureResponse.ResponseFunction() {
                            transaction = new CaptureResponse.Transaction() {
                                processing_status = new CaptureResponse.Transaction.ProcessingStatus() {
                                    FunctionResult = "NOK",
                                    error = new CaptureResponse.Transaction.ProcessingStatus.Error() {
                                        type = "PROVIDER", // "DATA"
                                        number = xmlResp.ActionCode,
                                        message = xmlResp.Message != null ? xmlResp.Message : Helpers.ActionCodes.getActionCodeMessage(xmlResp.ActionCode),
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
                                    type = "SYSTEM",
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

        public PurchaseResponse Purchase(PurchaseRequest request) {

            PurchaseResponse response = null;

            try {
                string virtualCard = null;

                if (request.getRequestType() == Requests.RequestType.Initial) {
                    virtualCard = getVirtualCardNumber(request.purchase.transaction.cred_card_data.credit_card_number, request.getConfigValue("MerchantContractNumber"));
                } else if (request.getRequestType() == Requests.RequestType.Repeated) {
                    virtualCard = request.purchase.transaction.credit_card_alias;
                }

                var borgunPurchase = RequestTransform.getBorgunPurchase(request, virtualCard);
                string xml = borgunPurchase.getXml();

                Console.WriteLine("Borgun Purchase request: {0}", xml);
                string res = client.ProcessRequest(borgunPurchase);
                Console.WriteLine("Borgun Purchase response: {0}", res);
                Entities.Responses.SOAPAuthResponse resp = Entities.Responses.SOAPAuthResponse.DeserializeFromString(res);
                Console.WriteLine("Borgun Purchase xml response: {0}", resp.Body.getAuthorizationOutput.getAuthResXml);
                Entities.Responses.AuthResponse xmlResp = Entities.Responses.AuthResponse.DeserializeFromString(resp.Body.getAuthorizationOutput.getAuthResXml);

                if (Int32.Parse(xmlResp.ActionCode) == 0) {
                    response = new PurchaseResponse() {
                        purchase = new PurchaseResponse.ResponseFunction() {
                            transaction = new PurchaseResponse.Transaction() {
                                reference_id = request.purchase.transaction.reference_id,
                                processing_status = new PurchaseResponse.Transaction.ProcessingStatus() {
                                    ProviderTransactionID = xmlResp.Transaction,
                                    RRN = xmlResp.RRN,
                                    DateAndTime = xmlResp.DateAndTime,
                                    TerminalID = xmlResp.TerminalID,
                                    CreditCardAlias = virtualCard,
                                    FunctionResult = "ACK",
                                }
                            }
                        }
                    };
                } else {
                    response = new PurchaseResponse() {
                        purchase = new PurchaseResponse.ResponseFunction() {
                            transaction = new PurchaseResponse.Transaction() {
                                processing_status = new PurchaseResponse.Transaction.ProcessingStatus() {
                                    FunctionResult = "NOK",
                                    error = new PurchaseResponse.Transaction.ProcessingStatus.Error() {
                                        type = "PROVIDER", // "DATA"
                                        number = xmlResp.ActionCode,
                                        message = xmlResp.Message != null ? xmlResp.Message : Helpers.ActionCodes.getActionCodeMessage(xmlResp.ActionCode),
                                    }
                                }
                            }
                        }
                    };
                }
            } catch (Exception ex) {
                response = new PurchaseResponse() {
                    purchase = new PurchaseResponse.ResponseFunction() {
                        transaction = new PurchaseResponse.Transaction() {
                            processing_status = new PurchaseResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new PurchaseResponse.Transaction.ProcessingStatus.Error() {
                                    type = "SYSTEM",
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
                var borgunRefund = RequestTransform.getBorgunRefund(request);
                string xml = borgunRefund.getXml();

                Console.WriteLine("Borgun Refund request: {0}", xml);
                string res = client.ProcessRequest(borgunRefund);
                Console.WriteLine("Borgun Refund response: {0}", res);
                Entities.Responses.SOAPAuthResponse resp = Entities.Responses.SOAPAuthResponse.DeserializeFromString(res);
                Console.WriteLine("Borgun Refund xml response: {0}", resp.Body.getAuthorizationOutput.getAuthResXml);
                Entities.Responses.AuthResponse xmlResp = Entities.Responses.AuthResponse.DeserializeFromString(resp.Body.getAuthorizationOutput.getAuthResXml);

                if (Int32.Parse(xmlResp.ActionCode) == 0) {
                    response = new RefundResponse() {
                        refund = new RefundResponse.ResponseFunction() {
                            transaction = new RefundResponse.Transaction() {
                                reference_id = request.refund.transaction.reference_id,
                                processing_status = new RefundResponse.Transaction.ProcessingStatus() {
                                    ProviderTransactionID = xmlResp.Transaction,
                                    RRN = xmlResp.RRN,
                                    DateAndTime = xmlResp.DateAndTime,
                                    TerminalID = xmlResp.TerminalID,
                                    FunctionResult = "ACK",
                                }
                            }
                        }
                    };
                } else {
                    response = new RefundResponse() {
                        refund = new RefundResponse.ResponseFunction() {
                            transaction = new RefundResponse.Transaction() {
                                processing_status = new RefundResponse.Transaction.ProcessingStatus() {
                                    FunctionResult = "NOK",
                                    error = new RefundResponse.Transaction.ProcessingStatus.Error() {
                                        type = "PROVIDER", // "DATA"
                                        number = xmlResp.ActionCode,
                                        message = xmlResp.Message != null ? xmlResp.Message : Helpers.ActionCodes.getActionCodeMessage(xmlResp.ActionCode),
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
                                    type = "SYSTEM",
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
                var borgunReversal = RequestTransform.getBorgunReversal(request);
                string xml = borgunReversal.getXml();

                Console.WriteLine("Borgun Reversal request: {0}", xml);
                string res = client.ProcessRequest(borgunReversal);
                Console.WriteLine("Borgun Reversal response: {0}", res);
                Entities.Responses.SOAPCancelResponse resp = Entities.Responses.SOAPCancelResponse.DeserializeFromString(res);
                Console.WriteLine("Borgun Reversal xml response: {0}", resp.Body.cancelAuthorizationResponse.cancelAuthResXml);
                Entities.Responses.CancelResponse xmlResp = Entities.Responses.CancelResponse.DeserializeFromString(resp.Body.cancelAuthorizationResponse.cancelAuthResXml);

                if (Int32.Parse(xmlResp.ActionCode) == 0) {
                    response = new ReversalResponse() {
                        reversal = new ReversalResponse.ResponseFunction() {
                            transaction = new ReversalResponse.Transaction() {
                                reference_id = request.reversal.transaction.reference_id,
                                processing_status = new ReversalResponse.Transaction.ProcessingStatus() {
                                    ProviderTransactionID = xmlResp.Transaction,
                                    RRN = xmlResp.RRN,
                                    DateAndTime = xmlResp.DateAndTime,
                                    TerminalID = xmlResp.TerminalID,
                                    FunctionResult = "ACK",
                                }
                            }
                        }
                    };
                } else {
                    response = new ReversalResponse() {
                        reversal = new ReversalResponse.ResponseFunction() {
                            transaction = new ReversalResponse.Transaction() {
                                processing_status = new ReversalResponse.Transaction.ProcessingStatus() {
                                    FunctionResult = "NOK",
                                    error = new ReversalResponse.Transaction.ProcessingStatus.Error() {
                                        type = "PROVIDER", // "DATA"
                                        number = xmlResp.ActionCode,
                                        message = xmlResp.Message != null ? xmlResp.Message : Helpers.ActionCodes.getActionCodeMessage(xmlResp.ActionCode),
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
                                    type = "SYSTEM",
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

        public SingleReconcileResponse SingleReconcile(SingleReconcileRequest request) {

            SingleReconcileResponse response = null;

            try {
                var borgunReconsile = RequestTransform.getBorgunSingleReconcile(request);
                string xml = borgunReconsile.getXml();

                Console.WriteLine("Borgun Reconcile request: {0}", xml);
                string res = client.ProcessRequest(borgunReconsile);
                Console.WriteLine("Borgun Reconcile response: {0}", res);
                Entities.Responses.SOAPTransactionInfoResponse resp = Entities.Responses.SOAPTransactionInfoResponse.DeserializeFromString(res);
                Console.WriteLine("Borgun Reconcile xml response: {0}", resp.Body);
                Entities.Responses.TransactionInfoResponse xmlResp = Entities.Responses.TransactionInfoResponse.DeserializeFromString(resp.Body.getTransactionListResponse.transactionListXML);

                if (Int32.Parse(xmlResp.ActionCode) == 0) {
                    Entities.Responses.TransactionInfoResponse.TransactionInfo resultTransaction = null;
                    foreach (var tr in xmlResp.transactions) {
                        if(tr.TransactionNumber == request.reconcile.transaction.provider_transaction_id) {
                            resultTransaction = tr;
                            break;
                        }
                    }
                    if(resultTransaction != null) {
                        response = new SingleReconcileResponse() {
                            reconcile = new SingleReconcileResponse.ResponseReconcileFunction() {
                                transaction = new SingleReconcileResponse.Transaction() {
                                    reference_id = request.reconcile.transaction.reference_id,
                                    processing_status = new SingleReconcileResponse.Transaction.ProcessingStatus() {
                                        FunctionResult = "ACK",
                                        ProviderTransactionID = resultTransaction.TransactionNumber,
                                        TerminalID = resultTransaction.TerminalNr,
                                    }
                                },
                                amount = resultTransaction.TrAmount,
                                currency = Fibonatix.CommDoo.Helpers.Currencies.getCurrencyCountryCode(resultTransaction.TrCurrency),
                                auth_code = resultTransaction.AuthorizationCode,
                                type = Helpers.TransactionsTypes.getTransactionType(resultTransaction.TransactionType),
                                ext_status = Helpers.TransactionsTypes.getTransactionStatus(resultTransaction.Status),
                                message = Helpers.TransactionsTypes.getTransactionType(resultTransaction.TransactionType) + ": " + Helpers.TransactionsTypes.getTransactionStatus(resultTransaction.Status),
                                response_code = resultTransaction.ActionCode,
                            }
                        };
                    }
                    else {
                        response = new SingleReconcileResponse() {
                            reconcile = new SingleReconcileResponse.ResponseReconcileFunction() {
                                transaction = new SingleReconcileResponse.Transaction() {
                                    reference_id = request.reconcile.transaction.reference_id,
                                    processing_status = new SingleReconcileResponse.Transaction.ProcessingStatus() {
                                        FunctionResult = "NOK",
                                        error = new Response.Transaction.ProcessingStatus.Error() {
                                            type = "PROVIDER", // "REJECTED"
                                            number = ErrorCodes.ReferenceNotFoundError.ToString(),
                                            message = "Cannot find transaction with ID = " + request.reconcile.transaction.provider_transaction_id + ", and RRN = " + request.reconcile.transaction.rrn,
                                        }
                                    }
                                },
                                ext_status = "",
                            }
                        };
                    }
                } else {
                    response = new SingleReconcileResponse() {
                        reconcile = new SingleReconcileResponse.ResponseReconcileFunction() {
                            transaction = new SingleReconcileResponse.Transaction() {
                                processing_status = new SingleReconcileResponse.Transaction.ProcessingStatus() {
                                    FunctionResult = "NOK",
                                    error = new SingleReconcileResponse.Transaction.ProcessingStatus.Error() {
                                        type = "PROVIDER", // "DATA"
                                        number = xmlResp.ActionCode,
                                        message = Helpers.ActionCodes.getActionCodeMessage(xmlResp.ActionCode),
                                    }
                                }
                            }
                        }
                    };
                }
            } catch (Exception ex) {
                response = new SingleReconcileResponse() {
                    reconcile = new SingleReconcileResponse.ResponseReconcileFunction() {
                        transaction = new SingleReconcileResponse.Transaction() {
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
            }
            return response;
        }


        // Enrollment Check - TODO - test it
        public EnrollmentCheck3DResponse EnrollmentCheck3D(EnrollmentCheck3DRequest request) {
            EnrollmentCheck3DResponse response = new EnrollmentCheck3DResponse() {
                enrolment_check = new EnrollmentCheck3DResponse.ResponseFunction() {
                    transaction = new EnrollmentCheck3DResponse.Transaction() {
                        processing_status = new EnrollmentCheck3DResponse.Transaction.ProcessingStatus() {
                            FunctionResult = "NOK",
                            error = new EnrollmentCheck3DResponse.Transaction.ProcessingStatus.Error() {
                                type = "PROVIDER",
                                number = "120",
                                message = "Not Supported Acquier",
                            }
                        }
                    }
                }
            };
            return response;
        }

        public Preauth3DResponse Preauthorize3D(Preauth3DRequest request) {
            Preauth3DResponse response = new Preauth3DResponse() {
                preAuth3D = new Preauth3DResponse.ResponseFunction() {
                    transaction = new Preauth3DResponse.Transaction() {
                        processing_status = new Preauth3DResponse.Transaction.ProcessingStatus() {
                            FunctionResult = "NOK",
                            error = new Preauth3DResponse.Transaction.ProcessingStatus.Error() {
                                type = "PROVIDER",
                                number = "120",
                                message = "Not Supported Acquier",
                            }
                        }
                    }
                }
            };
            return response;
        }
        public Purchase3DResponse Purchase3D(Purchase3DRequest request) {
            Purchase3DResponse response = new Purchase3DResponse() {
                purchase3D = new Purchase3DResponse.ResponseFunction() {
                    transaction = new Purchase3DResponse.Transaction() {
                        processing_status = new Purchase3DResponse.Transaction.ProcessingStatus() {
                            FunctionResult = "NOK",
                            error = new Purchase3DResponse.Transaction.ProcessingStatus.Error() {
                                type = "PROVIDER",
                                number = "120",
                                message = "Not Supported Acquier",
                            }
                        }
                    }
                }
            };
            return response;
        }
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
                            number = "120",
                            message = "Not Supported Acquier",
                        },
                    },
                    raw_data = new NotificationProcessingResponse.NotificationProcessingSection.RawResponse() {
                    }
                }
            };
            return response;
        }
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
                            number = "120",
                            message = "Not Supported Acquier",
                        },
                    },
                }
            };
            return response;
        }

    }
}
