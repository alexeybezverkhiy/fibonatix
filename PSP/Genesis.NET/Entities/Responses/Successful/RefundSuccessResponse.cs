using System.Xml.Serialization;
using CopaceticSoftware.pMixins.Attributes;

namespace Genesis.Net.Entities.Responses.Successful
{
    [pMixin(Mixin = typeof(AuthorizationModel))]
    [pMixin(Mixin = typeof(IssuerModel))]
    [pMixin(Mixin = typeof(TransactionTypeModel))]
    [pMixin(Mixin = typeof(TransactionStatusModel))]
    [pMixin(Mixin = typeof(TransactionModel))]
    [pMixin(Mixin = typeof(MoneyModel))]
    [XmlRoot("payment_response", Namespace = "RefundSuccessResponse")]
    public partial class RefundSuccessResponse : Entity
    {
        [XmlElement(ElementName="authorization_code")]
        public string AuthorizationCode { get;  set; }
    }
}