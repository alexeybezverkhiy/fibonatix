using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fibonatix.CommDoo.Requests;
using Fibonatix.CommDoo.Responses;

namespace Fibonatix.CommDoo
{
    public interface IConnector
    {
        PreauthResponse Preauthorize(PreauthRequest request);
        CaptureResponse Capture(CaptureRequest request);
        PurchaseResponse Purchase(PurchaseRequest request);
        RefundResponse Refund(RefundRequest request);
        ReversalResponse Reversal(ReversalRequest request);
        EnrollmentCheck3DResponse EnrollmentCheck3D(EnrollmentCheck3DRequest request);
        Preauth3DResponse Preauthorize3D(Preauth3DRequest request);
        Purchase3DResponse Purchase3D(Purchase3DRequest request);
        NotificationProcessingResponse NotificationProcessing(NotificationProcessingRequest request);
        EvaluateProviderResponseResponse EvaluateProviderResponse(EvaluateProviderResponseRequest request);
        SingleReconcileResponse SingleReconcile(SingleReconcileRequest request);
    }
}
