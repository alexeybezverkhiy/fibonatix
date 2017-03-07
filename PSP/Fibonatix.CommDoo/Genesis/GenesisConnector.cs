using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Security.Cryptography.X509Certificates;

using Genesis.Net;
using Genesis.Net.Common;
using Genesis.Net.Entities;
using Genesis.Net.Entities.Requests.Initial;
using Genesis.Net.Entities.Requests.Initial.ThreeD;
using Genesis.Net.Entities.Requests.Query;
using Genesis.Net.Entities.Requests.Referential;
using Genesis.Net.Specs;
using Genesis.Net.Specs.Mocks;
using Genesis.Net.Errors;

using Fibonatix.CommDoo.Requests;
using Fibonatix.CommDoo.Responses;
using Fibonatix.CommDoo.Helpers;
using System.Globalization;

namespace Fibonatix.CommDoo.Genesis
{
    internal class GenesisConnector : IConnector
    {
        private Configuration configuration;
        private IGenesisClient genesis;

        static private Dictionary<string, GenesisConnector> allconnectors;

        static public GenesisConnector getConnector(string login, string password, string token, bool sandbox) {
            if (login == null || password == null || token == null)
                return null;
            string key = login + ":" + password + ":" + token + ":" + sandbox.ToString();
            if(allconnectors == null) allconnectors = new Dictionary<string, GenesisConnector>();
            if ( !allconnectors.ContainsKey(key)) {
                GenesisConnector conn = new GenesisConnector(login, password, token, sandbox);
                allconnectors[key] = conn;
            }
            return allconnectors[key];
        }

        static public GenesisConnector getConnector(Request request) {
            bool sanbox = false;
            sanbox = String.Equals(request.getConfigValue("testmode"), "true", StringComparison.OrdinalIgnoreCase);
            return getConnector(request.getConfigValue("login"), request.getConfigValue("password"), request.getConfigValue("token"), sanbox);
        }

        public GenesisConnector(string login, string password, string token, bool sandbox) {
            configuration = new Configuration(
                environment: sandbox ? Environments.Staging : Environments.Production,
                // terminalToken: "9cc366d693e008bb7036d8be24ad1bcb4cae3057",
                // terminalToken: "9d669ebe5f5bbffcbeea5565a636608669f8b037",
                // apiLogin: "7d5ff68740b50e12dad17a51749c0529d4ca32cd",
                // apiPassword: "58ac52a8e4d4e01c73b36d77b053217a4471210f",
                terminalToken: token,
                apiLogin: login,
                apiPassword: password,
                certificate: null,
                endpoint: Endpoints.EComProcessing);
            genesis = GenesisClientFactory.Create(configuration);
        }

        static string GenesisStatusToGommDoo(string status) {
            if (status == "pending" || status == "pending_async" || status == "pending_review")
                return "PENDING";
            else
                return "ACK";
        }
        private string GenesisNotificationStatusToCommDoo(string status) {
            if (status == "approved" || status == "refunded" || status == "chargebacked" ||
                status == "voided" || status == "chargeback reversed")
                return "ACK";
            else if (status == "declined" || status == "error")
                return "NOK";
            else if (status == "pending_async" || status == "pending" ||
                status == "pending_review" || status == "pre_arbitrated")
                return "PENDING";
            return "NOK";
        }
        private string GenesisTransactionTypeToGenesis(string type, string status = null) {
            if(status != null) {
                if (status == "chargeback")
                    return "CHARGEBACK";
                else if (status == "chargeback_reversed")
                    return "CHARGEBACKREVERSAL";
            }
            if (type == "sale" || type == "sale3d" || type == "init_recurring_sale" || type == "init_recurring_sale3d" || type == "recurring_sale")
                return "PURCHASE";
            else if (type == "authorize" || type == "authorize3d")
                return "PREAUTHORIZATION";
            else if (type == "capture")
                return "CAPTURE";
            else if (type == "refund")
                return "REFUND";
            return "NONE";
        }


        public PreauthResponse Preauthorize(PreauthRequest request) {
            Authorize authorize = null;
            PreauthResponse response = null;

            try {
                authorize = RequestTransform.getGenesisAuthorize(request);
                if (authorize.Id == null)
                    authorize.Id = Guid.NewGuid().ToString();

                var result = genesis.Execute(authorize);
                response = new PreauthResponse() {
                    preAuth = new PreauthResponse.ResponseFunction() {
                        transaction = new PreauthResponse.Transaction() {
                            reference_id = authorize.Id,
                            processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                            }
                        }
                    }
                };

                if (result.IsSuccessful) {
                    response.preAuth.transaction.processing_status.ProviderTransactionID = result.SuccessResponse.UniqueId;
                    response.preAuth.transaction.processing_status.FunctionResult = GenesisStatusToGommDoo(result.SuccessResponse.Status);
                } else {
                        response.preAuth.transaction.processing_status.FunctionResult = "NOK";
                    response.preAuth.transaction.processing_status.error = new PreauthResponse.Transaction.ProcessingStatus.Error() {
                        type = "PROVIDER", // "REJECTED"
                        number = result.ErrorResponse.Code.ToString(),
                        message = result.ErrorResponse.Message + " " + result.ErrorResponse.TechnicalMessage,
                    };
                }
            } catch (System.ComponentModel.DataAnnotations.ValidationException ex) {
                var id = authorize != null ? authorize.Id : null;
                response = new PreauthResponse() {
                    preAuth = new PreauthResponse.ResponseFunction() {
                        transaction = new PreauthResponse.Transaction() {
                            reference_id = id,
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
            } catch (Exception ex) {
                var id = authorize != null ? authorize.Id : null;
                response = new PreauthResponse() {
                    preAuth = new PreauthResponse.ResponseFunction() {
                        transaction = new PreauthResponse.Transaction() {
                            reference_id = id,
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
            } finally { }

            return response;
        }


        public CaptureResponse Capture(CaptureRequest request) {
            Capture capture = null;
            CaptureResponse response = null;

            try {
                capture = RequestTransform.getGenesisCapture(request);
                if (capture.Id == null)
                    capture.Id = Guid.NewGuid().ToString();

                var result = genesis.Execute(capture);

                response = new CaptureResponse() {
                    capture = new CaptureResponse.ResponseFunction() {
                        transaction = new CaptureResponse.Transaction() {
                            reference_id = capture.Id,
                            processing_status = new CaptureResponse.Transaction.ProcessingStatus() {
                            }
                        }
                    }
                };

                if (result.IsSuccessful) {
                    response.capture.transaction.processing_status.ProviderTransactionID = result.SuccessResponse.UniqueId;
                    response.capture.transaction.processing_status.FunctionResult = GenesisStatusToGommDoo(result.SuccessResponse.Status);
                } else {
                    response.capture.transaction.processing_status.FunctionResult = "NOK";
                    response.capture.transaction.processing_status.error = new CaptureResponse.Transaction.ProcessingStatus.Error() {
                        type = "PROVIDER", // "REJECTED"
                        number = result.ErrorResponse.Code.ToString(),
                        message = result.ErrorResponse.Message + " " + result.ErrorResponse.TechnicalMessage,
                    };
                }

            } catch (System.ComponentModel.DataAnnotations.ValidationException ex) {
                var id = capture != null ? capture.Id : null;
                response = new CaptureResponse() {
                    capture = new CaptureResponse.ResponseFunction() {
                        transaction = new CaptureResponse.Transaction() {
                            reference_id = id,
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
            } catch (Exception ex) {
                var id = capture != null ? capture.Id : null;
                response = new CaptureResponse() {
                    capture = new CaptureResponse.ResponseFunction() {
                        transaction = new CaptureResponse.Transaction() {
                            reference_id = id,
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
            } finally { }

            return response;
        }


        public PurchaseResponse Purchase(PurchaseRequest request) {
            PurchaseResponse ret;

            try {
                switch(request.getRequestType()) {
                    case RequestType.Initial:
                        ret = InitialRecurringPurchase(request);
                        break;
                    case RequestType.Repeated:
                        ret = RecurringPurchase(request);
                        break;
                    case RequestType.Single:
                    default:
                        ret = SinglePurchase(request);
                        break;
                }
            } catch (Exception ex) {
                ret = new PurchaseResponse() {
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

            return ret;
        }


        PurchaseResponse SinglePurchase(PurchaseRequest request) {
            Sale sale = null;
            PurchaseResponse response = null;

            try {
                sale = RequestTransform.getGenesisSale(request);
                if (sale.Id == null)
                    sale.Id = Guid.NewGuid().ToString();

                var result = genesis.Execute(sale);

                response = new PurchaseResponse() {
                    purchase = new PurchaseResponse.ResponseFunction() {
                        transaction = new PurchaseResponse.Transaction() {
                            reference_id = sale.Id,
                            processing_status = new PurchaseResponse.Transaction.ProcessingStatus() {
                            }
                        }
                    }
                };

                if (result.IsSuccessful) {
                    response.purchase.transaction.processing_status.ProviderTransactionID = result.SuccessResponse.UniqueId;
                    response.purchase.transaction.processing_status.FunctionResult = GenesisStatusToGommDoo(result.SuccessResponse.Status);
                } else {
                    response.purchase.transaction.processing_status.FunctionResult = "NOK";
                    response.purchase.transaction.processing_status.error = new PurchaseResponse.Transaction.ProcessingStatus.Error() {
                        type = "PROVIDER", // "REJECTED"
                        number = result.ErrorResponse.Code.ToString(),
                        message = result.ErrorResponse.Message + " " + result.ErrorResponse.TechnicalMessage,
                    };
                }

            } catch (System.ComponentModel.DataAnnotations.ValidationException ex) {
                var id = sale != null ? sale.Id : null;
                response = new PurchaseResponse() {
                    purchase = new PurchaseResponse.ResponseFunction() {
                        transaction = new PurchaseResponse.Transaction() {
                            reference_id = id,
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
            } catch (Exception ex) {
                var id = sale != null ? sale.Id : null;
                response = new PurchaseResponse() {
                    purchase = new PurchaseResponse.ResponseFunction() {
                        transaction = new PurchaseResponse.Transaction() {
                            reference_id = id,
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


        PurchaseResponse InitialRecurringPurchase(PurchaseRequest request) {
            InitRecurringSale sale = null;
            PurchaseResponse response = null;

            try {
                sale = RequestTransform.getGenesisInitRecurringSale(request);
                if (sale.Id == null)
                    sale.Id = Guid.NewGuid().ToString();

                var result = genesis.Execute(sale);

                response = new PurchaseResponse() {
                    purchase = new PurchaseResponse.ResponseFunction() {
                        transaction = new PurchaseResponse.Transaction() {
                            reference_id = sale.Id,
                            processing_status = new PurchaseResponse.Transaction.ProcessingStatus() {
                            }
                        }
                    }
                };

                if (result.IsSuccessful) {
                    response.purchase.transaction.processing_status.ProviderTransactionID = result.SuccessResponse.UniqueId;
                    response.purchase.transaction.processing_status.FunctionResult = GenesisStatusToGommDoo(result.SuccessResponse.Status);
                } else {
                    response.purchase.transaction.processing_status.FunctionResult = "NOK";
                    response.purchase.transaction.processing_status.error = new PurchaseResponse.Transaction.ProcessingStatus.Error() {
                        type = "PROVIDER", // "REJECTED"
                        number = result.ErrorResponse.Code.ToString(),
                        message = result.ErrorResponse.Message + " " + result.ErrorResponse.TechnicalMessage,
                    };
                }

            } catch (System.ComponentModel.DataAnnotations.ValidationException ex) {
                var id = sale != null ? sale.Id : null;
                response = new PurchaseResponse() {
                    purchase = new PurchaseResponse.ResponseFunction() {
                        transaction = new PurchaseResponse.Transaction() {
                            reference_id = id,
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
            } catch (Exception ex) {
                var id = sale != null ? sale.Id : null;
                response = new PurchaseResponse() {
                    purchase = new PurchaseResponse.ResponseFunction() {
                        transaction = new PurchaseResponse.Transaction() {
                            reference_id = id,
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


        PurchaseResponse RecurringPurchase(PurchaseRequest request) {
            RecurringSale sale = null;
            PurchaseResponse response = null;

            try {
                sale = RequestTransform.getGenesisRecurringSale(request);
                if (sale.Id == null)
                    sale.Id = Guid.NewGuid().ToString();

                var result = genesis.Execute(sale);

                response = new PurchaseResponse() {
                    purchase = new PurchaseResponse.ResponseFunction() {
                        transaction = new PurchaseResponse.Transaction() {
                            reference_id = sale.Id,
                            processing_status = new PurchaseResponse.Transaction.ProcessingStatus() {
                            }
                        }
                    }
                };

                if (result.IsSuccessful) {
                    response.purchase.transaction.processing_status.ProviderTransactionID = result.SuccessResponse.UniqueId;
                    response.purchase.transaction.processing_status.FunctionResult = GenesisStatusToGommDoo(result.SuccessResponse.Status);
                } else {
                    response.purchase.transaction.processing_status.FunctionResult = "NOK";
                    response.purchase.transaction.processing_status.error = new PurchaseResponse.Transaction.ProcessingStatus.Error() {
                        type = "PROVIDER", // "REJECTED"
                        number = result.ErrorResponse.Code.ToString(),
                        message = result.ErrorResponse.Message + " " + result.ErrorResponse.TechnicalMessage,
                    };
                }

            } catch (System.ComponentModel.DataAnnotations.ValidationException ex) {
                var id = sale != null ? sale.Id : null;
                response = new PurchaseResponse() {
                    purchase = new PurchaseResponse.ResponseFunction() {
                        transaction = new PurchaseResponse.Transaction() {
                            reference_id = id,
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
            } catch (Exception ex) {
                var id = sale != null ? sale.Id : null;
                response = new PurchaseResponse() {
                    purchase = new PurchaseResponse.ResponseFunction() {
                        transaction = new PurchaseResponse.Transaction() {
                            reference_id = id,
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
            Refund refund = null;
            RefundResponse response = null;

            try {
                refund = RequestTransform.getGenesisRefund(request);
                if (refund.Id == null)
                    refund.Id = Guid.NewGuid().ToString();

                var result = genesis.Execute(refund);

                response = new RefundResponse() {
                    refund = new RefundResponse.ResponseFunction() {
                        transaction = new RefundResponse.Transaction() {
                            reference_id = refund.Id,
                            processing_status = new RefundResponse.Transaction.ProcessingStatus() {
                            }
                        }
                    }
                };

                if (result.IsSuccessful) {
                    response.refund.transaction.processing_status.ProviderTransactionID = result.SuccessResponse.UniqueId;
                    response.refund.transaction.processing_status.FunctionResult = GenesisStatusToGommDoo(result.SuccessResponse.Status);
                } else {
                    response.refund.transaction.processing_status.FunctionResult = "NOK";
                    response.refund.transaction.processing_status.error = new RefundResponse.Transaction.ProcessingStatus.Error() {
                        type = "PROVIDER", // "REJECTED"
                        number = result.ErrorResponse.Code.ToString(),
                        message = result.ErrorResponse.Message + " " + result.ErrorResponse.TechnicalMessage,
                    };
                }

            } catch (System.ComponentModel.DataAnnotations.ValidationException ex) {
                var id = refund != null ? refund.Id : null;
                response = new RefundResponse() {
                    refund = new RefundResponse.ResponseFunction() {
                        transaction = new RefundResponse.Transaction() {
                            reference_id = id,
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
            } catch (Exception ex) {
                var id = refund != null ? refund.Id : null;
                response = new RefundResponse() {
                    refund = new RefundResponse.ResponseFunction() {
                        transaction = new RefundResponse.Transaction() {
                            reference_id = id,
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
            } finally { }

            return response;
        }


        public ReversalResponse Reversal(ReversalRequest request) {
            VoidRequest voidReq = null;
            ReversalResponse response = null;

            try {
                voidReq = RequestTransform.getGenesisVoid(request);
                if (voidReq.Id == null)
                    voidReq.Id = Guid.NewGuid().ToString();

                var result = genesis.Execute(voidReq);

                response = new ReversalResponse() {
                    reversal = new ReversalResponse.ResponseFunction() {
                        transaction = new ReversalResponse.Transaction() {
                            reference_id = voidReq.Id,
                            processing_status = new ReversalResponse.Transaction.ProcessingStatus() {
                            }
                        }
                    }
                };

                if (result.IsSuccessful) {
                    response.reversal.transaction.processing_status.ProviderTransactionID = result.SuccessResponse.UniqueId;
                    response.reversal.transaction.processing_status.FunctionResult = GenesisStatusToGommDoo(result.SuccessResponse.Status);
                } else {
                    response.reversal.transaction.processing_status.FunctionResult = "NOK";
                    response.reversal.transaction.processing_status.error = new ReversalResponse.Transaction.ProcessingStatus.Error() {
                        type = "PROVIDER", // "REJECTED"
                        number = result.ErrorResponse.Code.ToString(),
                        message = result.ErrorResponse.Message + " " + result.ErrorResponse.TechnicalMessage,
                    };
                }

            } catch (System.ComponentModel.DataAnnotations.ValidationException ex) {
                var id = voidReq != null ? voidReq.Id : null;
                response = new ReversalResponse() {
                    reversal = new ReversalResponse.ResponseFunction() {
                        transaction = new ReversalResponse.Transaction() {
                            reference_id = id,
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
            } catch (Exception ex) {
                var id = voidReq != null ? voidReq.Id : null;
                response = new ReversalResponse() {
                    reversal = new ReversalResponse.ResponseFunction() {
                        transaction = new ReversalResponse.Transaction() {
                            reference_id = id,
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
            } finally { }

            return response;
        }


        public EnrollmentCheck3DResponse EnrollmentCheck3D(EnrollmentCheck3DRequest request) {
            EnrollmentCheck3DResponse response = null;

            try {
                RequestTransform.getGenesisEnrollmentCheck3D(request);
                // throw new Exception("Function not supported");
            } catch (System.ComponentModel.DataAnnotations.ValidationException ex) {
                response = new EnrollmentCheck3DResponse() {
                    enrolment_check = new EnrollmentCheck3DResponse.ResponseFunction() {
                        transaction = new EnrollmentCheck3DResponse.Transaction() {
                            reference_id = "",
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
            } catch (Exception ex) {
                response = new EnrollmentCheck3DResponse() {
                    enrolment_check = new EnrollmentCheck3DResponse.ResponseFunction() {
                        transaction = new EnrollmentCheck3DResponse.Transaction() {
                            reference_id = "",
                            processing_status = new EnrollmentCheck3DResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new EnrollmentCheck3DResponse.Transaction.ProcessingStatus.Error() {
                                    type = "SYSTEM",
                                    number = "1",
                                    message = ex.Message,
                                }
                            }
                        }
                    }
                };
            } finally { }

            return response;
        }


        public Preauth3DResponse Preauthorize3D(Preauth3DRequest request) {
            Authorize3dAsync authorize = null;
            Preauth3DResponse response = null;

            try {
                authorize = RequestTransform.getGenesisAuthorize3D(request);
                if (authorize.Id == null)
                    authorize.Id = Guid.NewGuid().ToString();

                var result = genesis.Execute(authorize);
                response = new Preauth3DResponse() {
                    preAuth3D = new Preauth3DResponse.ResponseFunction() {
                        transaction = new Preauth3DResponse.Transaction() {
                            reference_id = authorize.Id,
                            processing_status = new Preauth3DResponse.Transaction.ProcessingStatus() {
                            }
                        }
                    }
                };

                if (result.IsSuccessful) {
                    response.preAuth3D.transaction.processing_status.ProviderTransactionID = result.SuccessResponse.UniqueId;
                    response.preAuth3D.transaction.processing_status.FunctionResult = GenesisStatusToGommDoo(result.SuccessResponse.Status);
                    response.preAuth3D.transaction.secure3D = new Preauth3DResponse.Transaction.Secure3D() {
                        acs_url = result.SuccessResponse.RedirectUrl
                    };
                } else {
                    response.preAuth3D.transaction.processing_status.FunctionResult = "NOK";
                    response.preAuth3D.transaction.processing_status.error = new Preauth3DResponse.Transaction.ProcessingStatus.Error() {
                        type = "PROVIDER", // "REJECTED"
                        number = result.ErrorResponse.Code.ToString(),
                        message = result.ErrorResponse.Message + " " + result.ErrorResponse.TechnicalMessage,
                    };
                }
            } catch (System.ComponentModel.DataAnnotations.ValidationException ex) {
                var id = authorize != null ? authorize.Id : null;
                response = new Preauth3DResponse() {
                    preAuth3D = new Preauth3DResponse.ResponseFunction() {
                        transaction = new Preauth3DResponse.Transaction() {
                            reference_id = id,
                            processing_status = new Preauth3DResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new Preauth3DResponse.Transaction.ProcessingStatus.Error() {
                                    type = "SYSTEM", // "DATA"
                                    number = ((UInt32)ex.HResult).ToString(),
                                    message = ex.Message,
                                }
                            }
                        }
                    }
                };
            } catch (Exception ex) {
                var id = authorize != null ? authorize.Id : null;
                response = new Preauth3DResponse() {
                    preAuth3D = new Preauth3DResponse.ResponseFunction() {
                        transaction = new Preauth3DResponse.Transaction() {
                            reference_id = id,
                            processing_status = new Preauth3DResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new Preauth3DResponse.Transaction.ProcessingStatus.Error() {
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

        public Purchase3DResponse Purchase3D(Purchase3DRequest request) {
            Purchase3DResponse ret;

            try {
                switch(request.getRequestType()) {
                    case RequestType.Initial:
                        ret = InitialRecurringPurchase3D(request);
                        break;
                    case RequestType.Single:
                    default:
                        ret = SinglePurchase3D(request);
                        break;
                }
            } catch (Exception ex) {
                ret = new Purchase3DResponse() {
                    purchase3D = new Purchase3DResponse.ResponseFunction() {
                        transaction = new Purchase3DResponse.Transaction() {
                            processing_status = new Purchase3DResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new Purchase3DResponse.Transaction.ProcessingStatus.Error() {
                                    type = "SYSTEM",
                                    number = ((UInt32)ex.HResult).ToString(),
                                    message = ex.Message,
                                }
                            }
                        }
                    }
                };
            } finally { }

            return ret;
        }


        Purchase3DResponse SinglePurchase3D(Purchase3DRequest request) {
            Sale3dAsync sale = null;
            Purchase3DResponse response = null;

            try {
                sale = RequestTransform.getGenesisSale3D(request);
                if (sale.Id == null)
                    sale.Id = Guid.NewGuid().ToString();

                var result = genesis.Execute(sale);

                response = new Purchase3DResponse() {
                    purchase3D = new Purchase3DResponse.ResponseFunction() {
                        transaction = new Purchase3DResponse.Transaction() {
                            reference_id = sale.Id,
                            processing_status = new Purchase3DResponse.Transaction.ProcessingStatus() {
                            }
                        }
                    }
                };

                if (result.IsSuccessful) {
                    response.purchase3D.transaction.processing_status.ProviderTransactionID = result.SuccessResponse.UniqueId;
                    response.purchase3D.transaction.processing_status.FunctionResult = GenesisStatusToGommDoo(result.SuccessResponse.Status);
                    response.purchase3D.transaction.secure3D = new Preauth3DResponse.Transaction.Secure3D() {
                        acs_url = result.SuccessResponse.RedirectUrl
                    };
                } else {
                    response.purchase3D.transaction.processing_status.FunctionResult = "NOK";
                    response.purchase3D.transaction.processing_status.error = new PurchaseResponse.Transaction.ProcessingStatus.Error() {
                        type = "PROVIDER", // "REJECTED"
                        number = result.ErrorResponse.Code.ToString(),
                        message = result.ErrorResponse.Message + " " + result.ErrorResponse.TechnicalMessage,
                    };
                }

            } catch (System.ComponentModel.DataAnnotations.ValidationException ex) {
                var id = sale != null ? sale.Id : null;
                response = new Purchase3DResponse() {
                    purchase3D = new Purchase3DResponse.ResponseFunction() {
                        transaction = new Purchase3DResponse.Transaction() {
                            reference_id = id,
                            processing_status = new Purchase3DResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new Purchase3DResponse.Transaction.ProcessingStatus.Error() {
                                    type = "SYSTEM", // "DATA"
                                    number = ((UInt32)ex.HResult).ToString(),
                                    message = ex.Message,
                                }
                            }
                        }
                    }
                };
            } catch (Exception ex) {
                var id = sale != null ? sale.Id : null;
                response = new Purchase3DResponse() {
                    purchase3D = new Purchase3DResponse.ResponseFunction() {
                        transaction = new Purchase3DResponse.Transaction() {
                            reference_id = id,
                            processing_status = new Purchase3DResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new Purchase3DResponse.Transaction.ProcessingStatus.Error() {
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


        Purchase3DResponse InitialRecurringPurchase3D(Purchase3DRequest request) {
            InitRecurringSale3dAsync sale = null;
            Purchase3DResponse response = null;

            try {
                sale = RequestTransform.getGenesisInitRecurring3DSale(request);
                if (sale.Id == null)
                    sale.Id = Guid.NewGuid().ToString();

                var result = genesis.Execute(sale);

                response = new Purchase3DResponse() {
                    purchase3D = new Purchase3DResponse.ResponseFunction() {
                        transaction = new Purchase3DResponse.Transaction() {
                            reference_id = sale.Id,
                            processing_status = new Purchase3DResponse.Transaction.ProcessingStatus() {
                            }
                        }
                    }
                };

                if (result.IsSuccessful) {
                    response.purchase3D.transaction.processing_status.ProviderTransactionID = result.SuccessResponse.UniqueId;
                    response.purchase3D.transaction.processing_status.FunctionResult = GenesisStatusToGommDoo(result.SuccessResponse.Status);
                    response.purchase3D.transaction.secure3D = new Preauth3DResponse.Transaction.Secure3D() {
                        acs_url = result.SuccessResponse.RedirectUrl
                    };
                } else {
                    response.purchase3D.transaction.processing_status.FunctionResult = "NOK";
                    response.purchase3D.transaction.processing_status.error = new PurchaseResponse.Transaction.ProcessingStatus.Error() {
                        type = "PROVIDER", // "REJECTED"
                        number = result.ErrorResponse.Code.ToString(),
                        message = result.ErrorResponse.Message + " " + result.ErrorResponse.TechnicalMessage,
                    };
                }

            } catch (System.ComponentModel.DataAnnotations.ValidationException ex) {
                var id = sale != null ? sale.Id : null;
                response = new Purchase3DResponse() {
                    purchase3D = new Purchase3DResponse.ResponseFunction() {
                        transaction = new Purchase3DResponse.Transaction() {
                            reference_id = id,
                            processing_status = new Purchase3DResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new Purchase3DResponse.Transaction.ProcessingStatus.Error() {
                                    type = "SYSTEM", // "DATA"
                                    number = ((UInt32)ex.HResult).ToString(),
                                    message = ex.Message,
                                }
                            }
                        }
                    }
                };
            } catch (Exception ex) {
                var id = sale != null ? sale.Id : null;
                response = new Purchase3DResponse() {
                    purchase3D = new Purchase3DResponse.ResponseFunction() {
                        transaction = new Purchase3DResponse.Transaction() {
                            reference_id = id,
                            processing_status = new Purchase3DResponse.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                                error = new Purchase3DResponse.Transaction.ProcessingStatus.Error() {
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


        public NotificationProcessingResponse NotificationProcessing(NotificationProcessingRequest request) {
            NotificationProcessingResponse response = null;
            ThreeDNotification notificationData = null;
            try {
                string requestString = "";
                // if (request.notification.raw_data.get_part != null)
                //     requestString = requestString + request.notification.raw_data.get_part + "&amp;";
                if (request.notification.raw_data.post_part != null)
                    requestString = requestString + request.notification.raw_data.post_part + "&amp;";

                notificationData = Notification.Parse(requestString) as ThreeDNotification;

                if (Helpers.Crypto.sha1(notificationData.UniqueId + request.getConfigValue("password")) != notificationData.Signature) {
                    throw new System.ComponentModel.DataAnnotations.ValidationException("Signature verification failed").SetCode((int)ErrorCodes.InputDataError);
                }

                /* */
                SingleReconcileRequest reconRequest = new SingleReconcileRequest() {
                    reconcile = new SingleReconcileRequest.SingleReconcile() {
                        configurations = request.notification.configurations,
                        transaction = new SingleReconcileRequest.SingleReconcile.Transaction() {
                            provider_transaction_id = notificationData.UniqueId,
                            reference_id = notificationData.TransactionId,
                        },
                    },
                };
                SingleReconcileResponse reconResp = SingleReconcile(reconRequest);
                /* */
                // if (notificationData != null) {
                //     genesis.Execute(notificationData.GetEchoResponseBody());
                // }

                var parsed = HttpUtility.ParseQueryString(requestString);
                response = new NotificationProcessingResponse() {
                    notification = new NotificationProcessingResponse.NotificationProcessingSection() {
                        transaction = new NotificationProcessingResponse.NotificationProcessingSection.Transaction() {
                            transaction_type = GenesisTransactionTypeToGenesis(notificationData.TransactionType, notificationData.Status),
                            processing_status = new NotificationProcessingResponse.NotificationProcessingSection.Transaction.ProcessingStatus() {
                                FunctionResult = ( notificationData.Status == "approved" && reconResp.reconcile.ext_status == "approved" ) ? "ACK" : "NOK",
                                amount = Decimal.Parse(parsed["amount"], NumberStyles.Currency, CultureInfo.InvariantCulture),
                                currency = reconResp.reconcile.currency,
                                ProviderTransactionID = notificationData.UniqueId,
                                reference_id = notificationData.TransactionId,
                            }
                        },
                        raw_data = new NotificationProcessingResponse.NotificationProcessingSection.RawResponse() {
                            content_type = "text/xml",
                            content_data = notificationData != null ? System.Text.Encoding.UTF8.GetString(notificationData.GetEchoResponseBody()) : "",
                        }
                    }
                };

                if (response.notification.transaction.processing_status.FunctionResult == "NOK") {
                    response.notification.transaction.error = new NotificationProcessingResponse.NotificationProcessingSection.Transaction.Error() {
                        message = reconResp.reconcile.transaction.processing_status.error.message,
                        number = reconResp.reconcile.transaction.processing_status.error.number,
                        type = "PROVIDER"
                    };
                }
            } catch (System.ComponentModel.DataAnnotations.ValidationException ex) {
                response = new NotificationProcessingResponse() {
                    notification = new NotificationProcessingResponse.NotificationProcessingSection() {
                        transaction = new NotificationProcessingResponse.NotificationProcessingSection.Transaction() {
                            transaction_type = "NONE",
                            processing_status = new NotificationProcessingResponse.NotificationProcessingSection.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                            },
                            error = new NotificationProcessingResponse.NotificationProcessingSection.Transaction.Error() {
                                type = "SYSTEM", // "DATA"
                                number = ((UInt32)ex.HResult).ToString(),
                                message = ex.Message,
                            },
                        },
                        raw_data = new NotificationProcessingResponse.NotificationProcessingSection.RawResponse() {
                            content_type = "text/xml",
                            content_data = notificationData != null ? System.Text.Encoding.UTF8.GetString(notificationData.GetEchoResponseBody()) : "",
                        },
                    }
                };
            } catch (Exception) {
                response = new NotificationProcessingResponse() {
                    notification = new NotificationProcessingResponse.NotificationProcessingSection() {
                        transaction = new NotificationProcessingResponse.NotificationProcessingSection.Transaction() {
                            transaction_type = "NONE",
                            processing_status = new NotificationProcessingResponse.NotificationProcessingSection.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                            },
                            error = new NotificationProcessingResponse.NotificationProcessingSection.Transaction.Error() {
                                message = "Error from Notification center",
                                number = "600",
                                type = "SYSTEM"
                            },
                        },
                        raw_data = new NotificationProcessingResponse.NotificationProcessingSection.RawResponse() {
                            content_type = "text/xml",
                            content_data = notificationData != null ? System.Text.Encoding.UTF8.GetString(notificationData.GetEchoResponseBody()) : "",
                        }
                    }
                };
            } finally {
            }

            return response;
        }


        public EvaluateProviderResponseResponse EvaluateProviderResponse(EvaluateProviderResponseRequest request) {
            EvaluateProviderResponseResponse response = null;

            try {
                response = new EvaluateProviderResponseResponse() {
                    evaluate_provider = new EvaluateProviderResponseResponse.EvaluateProviderResponseSection() {
                        transaction = new EvaluateProviderResponseResponse.EvaluateProviderResponseSection.Transaction() {
                            transaction_type = "NONE",
                            processing_status = new EvaluateProviderResponseResponse.EvaluateProviderResponseSection.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                            },
                            error = new EvaluateProviderResponseResponse.EvaluateProviderResponseSection.Transaction.Error() {
                                message = "Function is not support by Genesis",
                                number = "600",
                                type = "PROVIDER"
                            },
                        },
                    }
                };
            } catch (Exception) {
                response = new EvaluateProviderResponseResponse() {
                    evaluate_provider = new EvaluateProviderResponseResponse.EvaluateProviderResponseSection() {
                        transaction = new EvaluateProviderResponseResponse.EvaluateProviderResponseSection.Transaction() {
                            transaction_type = "NONE",
                            processing_status = new EvaluateProviderResponseResponse.EvaluateProviderResponseSection.Transaction.ProcessingStatus() {
                                FunctionResult = "NOK",
                            },
                            error = new EvaluateProviderResponseResponse.EvaluateProviderResponseSection.Transaction.Error() {
                                message = "Unknown Error",
                                number = "600",
                                type = "SYSTEM"
                            },
                        },
                    }
                };
            } finally {
            }

            return response;
        }


        public SingleReconcileResponse SingleReconcile(SingleReconcileRequest request) {
            SingleReconcile reconcile = null;
            SingleReconcileResponse response = null;

            try {
                reconcile = RequestTransform.getSingleReconcile(request);

                var result = genesis.Execute(reconcile);

                response = new SingleReconcileResponse() {
                    reconcile = new SingleReconcileResponse.ResponseReconcileFunction() {                        
                        transaction = new SingleReconcileResponse.Transaction() {
                            processing_status = new SingleReconcileResponse.Transaction.ProcessingStatus() {
                            },
                        }
                    }
                };

                if (result.IsSuccessful) {
                    response.reconcile.type = result.SuccessResponse.TransactionType.ToString();
                    response.reconcile.message = result.SuccessResponse.Message;
                    response.reconcile.amount = Helpers.Convertors.MajorAmountToMinor(result.SuccessResponse.Amount, result.SuccessResponse.Currency).ToString();
                    response.reconcile.currency = Enum.GetName(typeof(Iso4217CurrencyCodes), result.SuccessResponse.Currency);
                    response.reconcile.ext_status = result.SuccessResponse.Status;
                    response.reconcile.auth_code = result.SuccessResponse.AuthorizationCode;
                    response.reconcile.mode = result.SuccessResponse.Mode;
                    response.reconcile.transaction.reference_id = result.SuccessResponse.TransactionId;
                    response.reconcile.transaction.processing_status.ProviderTransactionID = result.SuccessResponse.UniqueId;
                    response.reconcile.transaction.processing_status.FunctionResult = GenesisStatusToGommDoo(result.SuccessResponse.Status);
                    // response.reconcile.transaction.processing_status.
                    
                } else {
                    response.reconcile.transaction.processing_status.FunctionResult = "NOK";
                    response.reconcile.ext_status = result.ErrorResponse.Status;
                    response.reconcile.transaction.processing_status.error = new SingleReconcileResponse.Transaction.ProcessingStatus.Error() {
                        type = "PROVIDER", // "REJECTED"
                        number = result.ErrorResponse.Code.ToString(),
                        message = result.ErrorResponse.Message + " " + result.ErrorResponse.TechnicalMessage,
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
