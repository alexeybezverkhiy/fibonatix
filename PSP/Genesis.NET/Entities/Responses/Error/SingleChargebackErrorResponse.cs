using CopaceticSoftware.pMixins.Attributes;
using System.Xml.Serialization;

namespace Genesis.Net.Entities.Responses.Error
{
    [pMixin(Mixin = typeof(TransactionStatusModel))]
    [pMixin(Mixin = typeof(ErrorModel))]
    [XmlRoot("chargeback_response", Namespace = "SingleChargebackErrorResponse")]
    public partial class SingleChargebackErrorResponse : Entity, IErrorResponse
    {
    }
}