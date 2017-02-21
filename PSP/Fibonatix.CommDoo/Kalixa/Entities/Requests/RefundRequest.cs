using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;

namespace Fibonatix.CommDoo.Kalixa.Entities.Requests
{
    [Serializable()]
    [XmlRoot("initiatePaymentFromReferenceRequest", Namespace = "http://www.cqrpayments.com/PaymentProcessing", IsNullable = true)]
    [XmlInclude(typeof(Request.keyStringValuePair))]
    public class RefundRequest : Request
    {
        public override string getAPIPath() {
            return "initiatePaymentFromReference";
        }

        [Required]
        [XmlElement(ElementName = "merchantID")]
        public string merchantID { get; set; }
        [Required]
        [XmlElement(ElementName = "shopID")]
        public string shopID { get; set; }
        [Required]
        [XmlElement(ElementName = "originalPaymentID")]
        public string originalPaymentID { get; set; }
        [Required]
        [XmlElement(ElementName = "merchantTransactionID")]
        public string merchantTransactionID { get; set; }
        [Required]
        [XmlElement(ElementName = "paymentMethodID")]
        public string paymentMethodID { get; set; } // Credit Card Refund = 88 ?
        [Required]
        [XmlElement(ElementName = "amount")]
        public Amount amount { get; set; }
        [XmlArray("specificPaymentData")]
        [XmlArrayItem(typeof(keyStringValuePair), ElementName = "data")]
        public dataList specificPaymentData { get; set; }
    }
}
