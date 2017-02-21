using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fibonatix.CommDoo.Kalixa.Helpers
{
    class StatusCodes
    {
        public static string getStatusCodeMessage(int Code) {
            switch(Code) {
                case 3: return "InitiatedByProvider";
                case 4: return "InitiateErrorReportedByProvider";
                case 13: return "AuthorisedByProvider";
                case 14: return "AuthoriseErrorReportedByProvider";
                case 20: return "WithdrawnByProvider";
                case 21: return "WithdrawErrorReportedByProvider";
                case 27: return "CapturedByProvider";
                case 28: return "CaptureErrorReportedByProvider";
                case 29: return "DepositedByProvider";
                case 30: return "RedirectURLCreated";
                case 43: return "PaymentResultErrorReportedByProvider";
                case 54: return "StartErrorReportedByProvider";
                case 56: return "CommittedByProvider";
                case 63: return "FinalizeErrorReportedByProvider";
                case 100: return "RefusedByProvider";
                case 113: return "Cancelled";
                case 114: return "CancelErrorReportedByProvider";
                case 118: return "ChargedBackByProvider";
                case 120: return "PendingOnProvider";
                case 121: return "RefusedByPaymentScoring";
                case 125: return "Refunded";
                case 132: return "SupplyPaymentInfoErrorReportedByProvider";
                case 142: return "ExecutedByProvider";
                case 143: return "ExecuteErrorReportedByProvider";
                case 152: return "DepositErrorReportedByProvider";
                case 155: return "InquiryErrorReportedByProvider";
                case 202: return "BlockedByPaymentScoring";
                case 212: return "BlockedByMerchantScoring";
                case 213: return "RefusedByMerchantScoring";
                case 221: return "ToBeApprovedBySupervisor";
                case 225: return "ValidationFailed";
                case 229: return "RefusedByProviderUserEnteredWrongData";
                case 232: return "Created";
                case 233: return "ProviderCommunicationErrorOccurred";
                case 239: return "ChargedBackReversedByProvider";
                case 265: return "NotifyPaymentStateErrorReportedByMerchant";
                case 276: return "RefusedByPaymentScoringPhase2";
                case 283: return "VerifyThreeDSecureEnrollmentErrorOccurred";
                case 284: return "NotEnrolledInThreeDSecure";
                case 285: return "NotEnrolledInThreeDSecureADSAvailable";
                case 287: return "RedirectDataCreated";
                case 289: return "AuthenticateByThreeDSecureErrorOccurred";
                case 290: return "NotAuthenticatedByThreeDSecure";
                case 291: return "AuthenticatedByThreeDSecure";
                case 301: return "NotifyPaymentStateRefusedByMerchant";
                case 302: return "VerifyThreeDSecureEnrollmentErrorReported";
                case 303: return "AuthenticateByThreeDSecureErrorReported";
                case 304: return "AuthenticateByThreeDSecureAttemptsPerformed";
                case 305: return "AuthenticateByThreeDSecureFailed";
                case 306: return "PendingToBeCaptured";
                case 309: return "RefundRefusedByProvider";
                case 310: return "RefundErrorOccurred";
                case 320: return "RefundInitiated";
                case 329: return "PreInitiateErrorReportedByProvider";
                case 333: return "PreCaptureErrorReportedByProvider";
                case 336: return "AuthoriseCommunicationErrorOccurred";
                case 355: return "UpdatePaymentDetailsErrorReportedByProvider";
                case 368: return "DuplicatePaymentValidationFailed";
                case 369: return "DuplicatePaymentValidationFailed";
                case 395: return "AbortErrorReportedByProvider";
                case 403: return "QueryPaymentAccountDetailsErrorReportedByProvider";
                default: return "Unknown Code";
            }
        }
        public static string getStatusCodeMessage(string Code) {
            return getStatusCodeMessage(Int32.Parse(Code));
        }
    }
}
