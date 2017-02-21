using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fibonatix.CommDoo.Requests;
using Fibonatix.CommDoo.Responses;

namespace Fibonatix.CommDoo
{
    public class NullConnector : IConnector {
        public PreauthResponse Preauthorize(PreauthRequest request) {
            PreauthResponse response = new PreauthResponse() {
                preAuth = new PreauthResponse.ResponseFunction() {
                    transaction = new PreauthResponse.Transaction() {
                        processing_status = new PreauthResponse.Transaction.ProcessingStatus() {
                            FunctionResult = "NOK",
                            error = new PreauthResponse.Transaction.ProcessingStatus.Error() {
                                type = "PROVIDER", // "DATA"
                                number = (int)120,
                                message = "Not Supported Acquier",
                            }
                        }
                    }
                }
            };
            return response;
        }
        public CaptureResponse Capture(CaptureRequest request) {
            CaptureResponse response = new CaptureResponse() {
                capture = new CaptureResponse.ResponseFunction() {
                    transaction = new CaptureResponse.Transaction() {
                        processing_status = new CaptureResponse.Transaction.ProcessingStatus() {
                            FunctionResult = "NOK",
                            error = new CaptureResponse.Transaction.ProcessingStatus.Error() {
                                type = "PROVIDER", // "DATA"
                                number = (int)120,
                                message = "Not Supported Acquier",
                            }
                        }
                    }
                }
            };
            return response;
        }
        public PurchaseResponse Purchase(PurchaseRequest request) {
            PurchaseResponse response = new PurchaseResponse() {
                purchase = new PurchaseResponse.ResponseFunction() {
                    transaction = new PurchaseResponse.Transaction() {
                        processing_status = new PurchaseResponse.Transaction.ProcessingStatus() {
                            FunctionResult = "NOK",
                            error = new PurchaseResponse.Transaction.ProcessingStatus.Error() {
                                type = "PROVIDER",
                                number = (int)120,
                                message = "Not Supported Acquier",
                            }
                        }
                    }
                }
            };
            return response;
        }
        public RefundResponse Refund(RefundRequest request) {
            RefundResponse response = new RefundResponse() {
                refund = new RefundResponse.ResponseFunction() {
                    transaction = new RefundResponse.Transaction() {
                        processing_status = new RefundResponse.Transaction.ProcessingStatus() {
                            FunctionResult = "NOK",
                            error = new RefundResponse.Transaction.ProcessingStatus.Error() {
                                type = "PROVIDER",
                                number = (int)120,
                                message = "Not Supported Acquier",
                            }
                        }
                    }
                }
            };
            return response;
        }
        public ReversalResponse Reversal(ReversalRequest request) {
            ReversalResponse response = new ReversalResponse() {
                reversal = new ReversalResponse.ResponseFunction() {
                    transaction = new ReversalResponse.Transaction() {
                        processing_status = new ReversalResponse.Transaction.ProcessingStatus() {
                            FunctionResult = "NOK",
                            error = new ReversalResponse.Transaction.ProcessingStatus.Error() {
                                type = "PROVIDER",
                                number = (int)120,
                                message = "Not Supported Acquier",
                            }
                        }
                    }
                }
            };
            return response;
        }
        public EnrollmentCheck3DResponse EnrollmentCheck3D(EnrollmentCheck3DRequest request) {
            EnrollmentCheck3DResponse response = new EnrollmentCheck3DResponse() {
                enrolment_check = new EnrollmentCheck3DResponse.ResponseFunction() {
                    transaction = new EnrollmentCheck3DResponse.Transaction() {
                        processing_status = new EnrollmentCheck3DResponse.Transaction.ProcessingStatus() {
                            FunctionResult = "NOK",
                            error = new EnrollmentCheck3DResponse.Transaction.ProcessingStatus.Error() {
                                type = "PROVIDER",
                                number = (int)120,
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
                                number = (int)120,
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
                                number = (int)120,
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
                            number = (int)120,
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
                            number = (int)120,
                            message = "Not Supported Acquier",
                        },
                    },
                }
            };
            return response;
        }

        public SingleReconcileResponse SingleReconcile(SingleReconcileRequest request) {
            SingleReconcileResponse response = new SingleReconcileResponse() {
                reconcile = new SingleReconcileResponse.ResponseReconcileFunction() {
                    transaction = new SingleReconcileResponse.Transaction() {
                        processing_status = new SingleReconcileResponse.Transaction.ProcessingStatus() {
                            FunctionResult = "NOK",
                            error = new SingleReconcileResponse.Transaction.ProcessingStatus.Error() {
                                type = "PROVIDER",
                                number = (int)120,
                                message = "Not Supported Acquier",
                            }
                        }
                    }
                }
            };
            return response;
        }
    }
}
