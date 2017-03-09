using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Globalization;
using Fibonatix.CommDoo.Requests;
using Fibonatix.CommDoo.Responses;
using Fibonatix.CommDoo.Helpers;
using Genesis.Net.Errors;


namespace Fibonatix.CommDoo.ProcessingCom
{
    public class PComConnector : IConnector
    {
        static Network.Client client = new Network.Client();

        static PComConnector g_connector = new PComConnector();

        static public PComConnector getConnector(Request request) {
            return g_connector;
        }

        private void FillRequestAuthParam(Entities.Request request, Request.Configurations configurations) {
            request.setParameter("account_username", configurations.GetConfigurationValue("account_username") != null ? configurations.GetConfigurationValue("account_username") : configurations.GetConfigurationValue("login"));
            request.setParameter("account_password", configurations.GetConfigurationValue("account_password") != null ? configurations.GetConfigurationValue("account_password") : configurations.GetConfigurationValue("password"));
            request.setParameter("mid", configurations.GetConfigurationValue("mid"));
            request.setParameter("mid_q", configurations.GetConfigurationValue("mid_q"));
            request.inSandBox = (configurations.GetConfigurationValue("testmode").ToLower() == "true");
        }

        private void FillParameter(Entities.Request request, string key, string value) {
            if (key != null && value != null) {
                request.setParameter(key, value);
            }
        }
        private void FillParameter(Entities.Request request, string key, int value) {
            if (key != null) {
                request.setParameter(key, value.ToString(CultureInfo.InvariantCulture));
            }
        }
        private void FillParameter(Entities.Request request, string key, decimal value) {
            if (key != null) {
                request.setParameter(key, value.ToString(CultureInfo.InvariantCulture));
            }
        }
        private void FillParameter(Entities.Request request, string key, bool value) {
            if (key != null) {
                request.setParameter(key, value ? "true" : "false");
            }
        }

        public PreauthResponse Preauthorize(PreauthRequest request) {
            PreauthResponse response = null;

            try {
                request.verification(); // exception

                string cavv = null;
                string xid = null;
                string eci = null;
                string secure_hash = null;

                try {
                    if (request.preAuth.transaction.threed_secure != null) {
                        if (request.preAuth.transaction.threed_secure.md != null && request.preAuth.transaction.threed_secure.pa_res != null) {
                            Entities.Request reqPaResParse = new Entities.Request();
                            FillRequestAuthParam(reqPaResParse, request.preAuth.configurations);
                            Entities.ResponseJSON responsePaResParse = ParsePaRes(reqPaResParse, request.preAuth.transaction.threed_secure.pa_res, request.preAuth.transaction.threed_secure.md);
                            if (responsePaResParse.status.code == 200) {
                                cavv = responsePaResParse.result.cavv;
                                xid = responsePaResParse.result.xid;
                                eci = responsePaResParse.result.eci;
                                secure_hash = responsePaResParse.result.secure_hash;
                            } else {
                                response = new PreauthResponse() {
                                    preAuth = new PreauthResponse.ResponseFunction() {
                                        transaction = new PreauthResponse.Transaction() {
                                            processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                                FunctionResult = "NOK",
                                                error = new Response.Transaction.ProcessingStatus.Error() {
                                                    message = responsePaResParse.status.message,
                                                    number = responsePaResParse.status.code.ToString(),
                                                    type = "PROVIDER",
                                                },
                                            }
                                        }
                                    }
                                };
                                return response;
                            }
                        }
                    }
                } catch {
                }

                Entities.Request req = new Entities.Request();
                FillRequestAuthParam(req, request.preAuth.configurations);
                req.reqType = Entities.Request.ReqType.HTREM_REQ;
                FillParameter(req, "type", "authorization");
                FillParameter(req, "amount", Fibonatix.CommDoo.Helpers.Convertors.MinorAmountToMajor((int)request.preAuth.transaction.amount, Fibonatix.CommDoo.Helpers.Currencies.currencyCodeFromString(request.preAuth.transaction.currency)));
                FillParameter(req, "card_number", request.preAuth.transaction.cred_card_data.credit_card_number);
                FillParameter(req, "card_expiry_month", request.preAuth.transaction.cred_card_data.expiration_month.ToString("0,2"));
                FillParameter(req, "card_expiry_year", request.preAuth.transaction.cred_card_data.expiration_year.ToString());
                FillParameter(req, "card_cvv2", request.preAuth.transaction.cred_card_data.cvv);
                FillParameter(req, "ip_address", request.preAuth.transaction.customer_data.ipaddress);
                FillParameter(req, "cavv", cavv);
                FillParameter(req, "xid", xid);
                FillParameter(req, "eci", eci);
                FillParameter(req, "secure_hash", secure_hash);
                FillParameter(req, "first_name", request.preAuth.transaction.customer_data.firstname);
                FillParameter(req, "last_name", request.preAuth.transaction.customer_data.lastname);
                FillParameter(req, "street_address_1", request.preAuth.transaction.customer_data.street);
                FillParameter(req, "street_address_2", null);
                FillParameter(req, "city", request.preAuth.transaction.customer_data.city);
                FillParameter(req, "state", request.preAuth.transaction.customer_data.state);
                FillParameter(req, "zip", request.preAuth.transaction.customer_data.postalcode);
                FillParameter(req, "country", Countries.countryAlpha2String(request.preAuth.transaction.customer_data.country));
                FillParameter(req, "phone_number", request.preAuth.transaction.customer_data.phone);
                FillParameter(req, "email_address", request.preAuth.transaction.customer_data.email);
                FillParameter(req, "dynamic_descriptor", null);
                FillParameter(req, "recipient_dob", null);
                FillParameter(req, "recipient_account_no", null);
                FillParameter(req, "recipient_postal_code", null);
                FillParameter(req, "recipient_surname", null);

                string result = client.ProcessRequest(req);

                Entities.Response resp = new Entities.Response(result);

                response = new PreauthResponse() {
                    preAuth = new PreauthResponse.ResponseFunction() {
                        transaction = new PreauthResponse.Transaction() {
                            processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                FunctionResult = resp.getParameter("code") == "00" ? "ACK" : "NOK",
                            }
                        }
                    }
                };

                response.preAuth.transaction.reference_id = request.preAuth.transaction.reference_id;
                if (resp.getParameter("code") == "00") {
                    response.preAuth.transaction.processing_status.ProviderTransactionID = resp.getParameter("transaction_id");
                    response.preAuth.transaction.processing_status.AuthCode = resp.getParameter("auth_code");
                } else {
                    response.preAuth.transaction.processing_status.error = new Response.Transaction.ProcessingStatus.Error() {
                        message = resp.getErrorMessage(),
                        number = resp.getParameter("code"),
                        type = "PROVIDER",
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
                Entities.Request req = new Entities.Request();
                FillRequestAuthParam(req, request.capture.configurations);
                req.reqType = Entities.Request.ReqType.HTREM_REQ;
                FillParameter(req, "type", "capture");
                if (request.capture.transaction.amount != 0)
                    FillParameter(req, "amount", Fibonatix.CommDoo.Helpers.Convertors.MinorAmountToMajor((int)request.capture.transaction.amount, Fibonatix.CommDoo.Helpers.Currencies.currencyCodeFromString(request.capture.transaction.currency)));
                FillParameter(req, "auth_code", request.capture.transaction.auth_code);
                FillParameter(req, "origin", request.capture.transaction.provider_transaction_id);

                string result = client.ProcessRequest(req);

                Entities.Response resp = new Entities.Response(result);

                response = new CaptureResponse() {
                    capture = new CaptureResponse.ResponseFunction() {
                        transaction = new PreauthResponse.Transaction() {
                            processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                                FunctionResult = resp.getParameter("code") == "00" ? "ACK" : "NOK",
                            }
                        }
                    }
                };

                response.capture.transaction.reference_id = request.capture.transaction.reference_id;
                if (resp.getParameter("code") == "00") {
                    response.capture.transaction.processing_status.ProviderTransactionID = resp.getParameter("transaction_id");
                } else {
                    response.capture.transaction.processing_status.error = new Response.Transaction.ProcessingStatus.Error() {
                        message = resp.getErrorMessage(),
                        number = resp.getParameter("code"),
                        type = "PROVIDER",
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
        public PurchaseResponse Purchase(PurchaseRequest request) {

            PurchaseResponse response = null;

            try {
                string cavv = null;
                string xid = null;
                string eci = null;
                string secure_hash = null;

                try {
                    if (request.purchase.transaction.threed_secure != null) {
                        if (request.purchase.transaction.threed_secure.md != null && request.purchase.transaction.threed_secure.pa_res != null) {
                            Entities.Request reqPaResParse = new Entities.Request();
                            FillRequestAuthParam(reqPaResParse, request.purchase.configurations);
                            Entities.ResponseJSON responsePaResParse = ParsePaRes(reqPaResParse, request.purchase.transaction.threed_secure.pa_res, request.purchase.transaction.threed_secure.md);
                            if (responsePaResParse.status.code == 200) {
                                cavv = responsePaResParse.result.cavv;
                                xid = responsePaResParse.result.xid;
                                eci = responsePaResParse.result.eci;
                                secure_hash = responsePaResParse.result.secure_hash;
                            } else {
                                response = new PurchaseResponse() {
                                    purchase = new PurchaseResponse.ResponseFunction() {
                                        transaction = new PurchaseResponse.Transaction() {
                                            processing_status = new PurchaseResponse.Transaction.ProcessingStatus() {
                                                FunctionResult = "NOK",
                                                error = new Response.Transaction.ProcessingStatus.Error() {
                                                    message = responsePaResParse.status.message,
                                                    number = responsePaResParse.status.code.ToString(),
                                                    type = "PROVIDER",
                                                },
                                            }
                                        }
                                    }
                                };
                                return response;
                            }
                        }
                    }
                } catch {
                }

                Entities.Request req = new Entities.Request();
                FillRequestAuthParam(req, request.purchase.configurations);
                req.reqType = Entities.Request.ReqType.HTREM_REQ;

                if (request.getRequestType() == RequestType.Single || request.getRequestType() == RequestType.Initial) {
                    FillParameter(req, "type", "purchase");
                    FillParameter(req, "amount", Fibonatix.CommDoo.Helpers.Convertors.MinorAmountToMajor((int)request.purchase.transaction.amount, Fibonatix.CommDoo.Helpers.Currencies.currencyCodeFromString(request.purchase.transaction.currency)));
                    FillParameter(req, "card_number", request.purchase.transaction.cred_card_data.credit_card_number);
                    FillParameter(req, "card_expiry_month", request.purchase.transaction.cred_card_data.expiration_month.ToString("0,2"));
                    FillParameter(req, "card_expiry_year", request.purchase.transaction.cred_card_data.expiration_year.ToString());
                    FillParameter(req, "card_cvv2", request.purchase.transaction.cred_card_data.cvv);
                    FillParameter(req, "ip_address", request.purchase.transaction.customer_data.ipaddress);
                    if (request.purchase.transaction.recurring_transaction != null) {
                        if (String.Equals(request.purchase.transaction.recurring_transaction.type, "INITIAL", StringComparison.InvariantCultureIgnoreCase)) {
                            FillParameter(req, "recurring", "1");
                        } else if (String.Equals(request.purchase.transaction.recurring_transaction.type, "REPEATED", StringComparison.InvariantCultureIgnoreCase)) {
                            FillParameter(req, "recurring", "1");
                            FillParameter(req, "referenceid", request.purchase.transaction.provider_transaction_id);
                        }
                    }
                    FillParameter(req, "cavv", cavv);
                    FillParameter(req, "xid", xid);
                    FillParameter(req, "eci", eci);
                    FillParameter(req, "secure_hash", secure_hash);
                    FillParameter(req, "first_name", request.purchase.transaction.customer_data.firstname);
                    FillParameter(req, "last_name", request.purchase.transaction.customer_data.lastname);
                    FillParameter(req, "street_address_1", request.purchase.transaction.customer_data.street);
                    FillParameter(req, "street_address_2", null);
                    FillParameter(req, "city", request.purchase.transaction.customer_data.city);
                    FillParameter(req, "state", request.purchase.transaction.customer_data.state);
                    FillParameter(req, "zip", request.purchase.transaction.customer_data.postalcode);
                    FillParameter(req, "country", Countries.countryAlpha2String(request.purchase.transaction.customer_data.country));
                    FillParameter(req, "phone_number", request.purchase.transaction.customer_data.phone);
                    FillParameter(req, "email_address", request.purchase.transaction.customer_data.email);
                    FillParameter(req, "dynamic_descriptor", null);
                    FillParameter(req, "recipient_dob", null);
                    FillParameter(req, "recipient_account_no", null);
                    FillParameter(req, "recipient_postal_code", null);
                    FillParameter(req, "recipient_surname", null);
                }
                else if (request.getRequestType() == RequestType.Repeated) {
                    FillParameter(req, "type", "recurring");
                    FillParameter(req, "amount", Fibonatix.CommDoo.Helpers.Convertors.MinorAmountToMajor((int)request.purchase.transaction.amount, Fibonatix.CommDoo.Helpers.Currencies.currencyCodeFromString(request.purchase.transaction.currency)));
                    FillParameter(req, "origin", request.purchase.transaction.provider_transaction_id);
                    FillParameter(req, "dynamic_descriptor", null);
                    FillParameter(req, "recipient_dob", null);
                    FillParameter(req, "recipient_account_no", null);
                    FillParameter(req, "recipient_postal_code", null);
                    FillParameter(req, "recipient_surname", null);
                }

                string result = client.ProcessRequest(req);

                Entities.Response resp = new Entities.Response(result);

                response = new PurchaseResponse() {
                    purchase = new PurchaseResponse.ResponseFunction() {
                        transaction = new PurchaseResponse.Transaction() {
                            processing_status = new PurchaseResponse.Transaction.ProcessingStatus() {
                                FunctionResult = resp.getParameter("code") == "00" ? "ACK" : "NOK",
                            }
                        }
                    }
                };

                response.purchase.transaction.reference_id = request.purchase.transaction.reference_id;
                if (resp.getParameter("code") == "00") {
                    response.purchase.transaction.processing_status.ProviderTransactionID = resp.getParameter("transaction_id");
                    // response.purchase.transaction.processing_status.AuthCode = resp.getParameter("auth_code");
                } else {
                    response.purchase.transaction.processing_status.error = new Response.Transaction.ProcessingStatus.Error() {
                        message = resp.getErrorMessage(),
                        number = resp.getParameter("code"),
                        type = "PROVIDER",
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
            } finally { }

            return response;
        }

        public RefundResponse Refund(RefundRequest request) {
            RefundResponse response = null;

            try {
                Entities.Request req = new Entities.Request();
                FillRequestAuthParam(req, request.refund.configurations);
                req.reqType = Entities.Request.ReqType.HTREM_REQ;
                FillParameter(req, "type", "refund");
                FillParameter(req, "amount", Fibonatix.CommDoo.Helpers.Convertors.MinorAmountToMajor((int)request.refund.transaction.amount, Fibonatix.CommDoo.Helpers.Currencies.currencyCodeFromString(request.refund.transaction.currency)));
                FillParameter(req, "origin", request.refund.transaction.provider_transaction_id);

                string result = client.ProcessRequest(req);

                Entities.Response resp = new Entities.Response(result);

                response = new RefundResponse() {
                    refund = new RefundResponse.ResponseFunction() {
                        transaction = new RefundResponse.Transaction() {
                            processing_status = new RefundResponse.Transaction.ProcessingStatus() {
                                FunctionResult = resp.getParameter("code") == "00" ? "ACK" : "NOK",
                            }
                        }
                    }
                };

                response.refund.transaction.reference_id = request.refund.transaction.reference_id;
                if (resp.getParameter("code") == "00") {
                    response.refund.transaction.processing_status.ProviderTransactionID = resp.getParameter("transaction_id");
                } else {
                    response.refund.transaction.processing_status.error = new Response.Transaction.ProcessingStatus.Error() {
                        message = resp.getErrorMessage(),
                        number = resp.getParameter("code"),
                        type = "PROVIDER",
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
        public ReversalResponse Reversal(ReversalRequest request) {
            ReversalResponse response = null;

            try {
                Entities.Request req = new Entities.Request();
                FillRequestAuthParam(req, request.reversal.configurations);
                req.reqType = Entities.Request.ReqType.HTREM_REQ;
                FillParameter(req, "type", "void");
                FillParameter(req, "origin", request.reversal.transaction.provider_transaction_id);

                string result = client.ProcessRequest(req);

                Entities.Response resp = new Entities.Response(result);

                response = new ReversalResponse() {
                    reversal = new ReversalResponse.ResponseFunction() {
                        transaction = new ReversalResponse.Transaction() {
                            processing_status = new ReversalResponse.Transaction.ProcessingStatus() {
                                FunctionResult = resp.getParameter("code") == "00" ? "ACK" : "NOK",
                            }
                        }
                    }
                };

                response.reversal.transaction.reference_id = request.reversal.transaction.reference_id;
                if (resp.getParameter("code") == "00") {
                    response.reversal.transaction.processing_status.ProviderTransactionID = resp.getParameter("transaction_id");
                } else {
                    response.reversal.transaction.processing_status.error = new Response.Transaction.ProcessingStatus.Error() {
                        message = resp.getErrorMessage(),
                        number = resp.getParameter("code"),
                        type = "PROVIDER",
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
        public EnrollmentCheck3DResponse EnrollmentCheck3D(EnrollmentCheck3DRequest request) {

            EnrollmentCheck3DResponse response = null;

            try {
                Entities.Request req = new Entities.Request();
                FillRequestAuthParam(req, request.enrollment_check.configurations);
                req.reqType = Entities.Request.ReqType.ACSURL_REQ;
                FillParameter(req, "trans_amount", Convertors.MinorAmountToMajor((int)request.enrollment_check.transaction.amount, Currencies.currencyCodeFromString(request.enrollment_check.transaction.currency)));
                FillParameter(req, "trans_id", request.enrollment_check.transaction.reference_id);
                FillParameter(req, "cc_num", request.enrollment_check.transaction.cred_card_data.credit_card_number);
                FillParameter(req, "cc_exp_yr", request.enrollment_check.transaction.cred_card_data.expiration_year % 100 );
                FillParameter(req, "cc_exp_mth", request.enrollment_check.transaction.cred_card_data.expiration_month);

                string result = client.ProcessRequest(req);

                Entities.ResponseJSON resp = Entities.ResponseJSON.parseResponseJSON(result);

                if (resp.status.code == 200) {
                    response = new EnrollmentCheck3DResponse() {
                        enrolment_check = new EnrollmentCheck3DResponse.ResponseFunction() {
                            transaction = new EnrollmentCheck3DResponse.Transaction() {
                                processing_status = new EnrollmentCheck3DResponse.Transaction.ProcessingStatus() {
                                    StatusType = "Y",
                                    FunctionResult = "ACK",
                                    ProviderTransactionID = Guid.NewGuid().ToString(),
                                },
                                reference_id = request.enrollment_check.transaction.reference_id,
                                secure3D = new Response.Transaction.Secure3D() {
                                    acs_url = resp.result.acs_url,
                                    md = resp.result.MD,
                                    pa_req = resp.result.PaReq,
                                },
                            },
                        },
                    };
                } else if (resp.status.code == 404 || resp.status.code == 500) {
                    response = new EnrollmentCheck3DResponse() {
                        enrolment_check = new EnrollmentCheck3DResponse.ResponseFunction() {
                            transaction = new EnrollmentCheck3DResponse.Transaction() {
                                processing_status = new EnrollmentCheck3DResponse.Transaction.ProcessingStatus() {
                                    StatusType = (resp.status.code == 404) ? "N" : "U",
                                    FunctionResult = "ACK",
                                    ProviderTransactionID = Guid.NewGuid().ToString(),
                                },
                                reference_id = request.enrollment_check.transaction.reference_id,
                            },
                        },
                    };
                } else {
                    response = new EnrollmentCheck3DResponse() {
                        enrolment_check = new EnrollmentCheck3DResponse.ResponseFunction() {
                            transaction = new EnrollmentCheck3DResponse.Transaction() {
                                processing_status = new EnrollmentCheck3DResponse.Transaction.ProcessingStatus() {
                                    FunctionResult = "NOK",
                                    ProviderTransactionID = "",
                                    error = new ReversalResponse.Transaction.ProcessingStatus.Error() {
                                        type = "PROVIDER", // "DATA"
                                        number = resp.status.code.ToString(),
                                        message = resp.status.message,
                                    }
                                },
                                reference_id = request.enrollment_check.transaction.reference_id,
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

        public Preauth3DResponse Preauthorize3D(Preauth3DRequest request) {
            Preauth3DResponse response = new Preauth3DResponse() {
                preAuth3D = new Preauth3DResponse.ResponseFunction() {
                    transaction = new Preauth3DResponse.Transaction() {
                        processing_status = new Preauth3DResponse.Transaction.ProcessingStatus() {
                            FunctionResult = "NOK",
                            error = new Preauth3DResponse.Transaction.ProcessingStatus.Error() {
                                type = "PROVIDER",
                                number = "120",
                                message = "'Preauthorize 3D' Request Is Not Supported By Acquier",
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
                                message = "'Purchase 3D' Request Is Not Supported By Acquier",
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
                            message = "'Notification Processing' Request Is Not Supported By Acquier",
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
                            message = "'Evaluate Provider Response' Request Is Not Supported By Acquier",
                        },
                    },
                }
            };
            return response;
        }

        public SingleReconcileResponse SingleReconcile(SingleReconcileRequest request) {
            SingleReconcileResponse response = null;

            try {
                Entities.Request req = new Entities.Request();
                FillRequestAuthParam(req, request.reconcile.configurations);
                req.reqType = Entities.Request.ReqType.HTREM_REQ;
                FillParameter(req, "type", "query");
                FillParameter(req, "origin", request.reconcile.transaction.provider_transaction_id);

                string result = client.ProcessRequest(req);

                Entities.Response resp = new Entities.Response(result);

                response = new SingleReconcileResponse() {
                    reconcile = new SingleReconcileResponse.ResponseReconcileFunction() {
                        transaction = new SingleReconcileResponse.Transaction() {
                            processing_status = new SingleReconcileResponse.Transaction.ProcessingStatus() {
                                FunctionResult = resp.getParameter("code") == "00" ? "ACK" : "NOK",
                            }
                        }
                    }
                };

                response.reconcile.transaction.reference_id = request.reconcile.transaction.reference_id;
                if (resp.getParameter("code") == "00") {
                    response.reconcile.transaction.processing_status.ProviderTransactionID = resp.getParameter("transaction_id");
                    // response.reconcile.amount = resp.getParameter("amount"); // Cannot convert major value to minor without currency know
                    response.reconcile.type = resp.getParameter("type");
                    response.reconcile.ext_status = resp.getParameter("state");
                    response.reconcile.response_code = resp.getParameter("state");
                } else {
                    response.reconcile.transaction.processing_status.error = new Response.Transaction.ProcessingStatus.Error() {
                        message = resp.getErrorMessage(),
                        number = resp.getParameter("code"),
                        type = "PROVIDER",
                    };
                }
            } catch (Exception ex) {
                response = new SingleReconcileResponse() {
                    reconcile = new SingleReconcileResponse.ResponseReconcileFunction() {
                        transaction = new SingleReconcileResponse.Transaction() {
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
            }
            return response;
        }

        private Entities.ResponseJSON ParsePaRes(Entities.Request req, string PaRes, string MD) {            
            req.reqType = Entities.Request.ReqType.PARES_REQ;
            FillParameter(req, "PaRes", PaRes);
            FillParameter(req, "MD", MD);
            string result = client.ProcessRequest(req);
            Entities.ResponseJSON resp = Entities.ResponseJSON.parseResponseJSON(result);
            return resp;
        }
    }
}
