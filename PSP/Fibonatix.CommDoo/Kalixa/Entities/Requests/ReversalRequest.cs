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
    [XmlRoot("executePaymentActionRequest", Namespace = "http://www.cqrpayments.com/PaymentProcessing", IsNullable = true)]
    public class ReversalRequest : Request
    {
        public override string getAPIPath() {
            return "executePaymentAction";
        }

        [Required]
        [XmlElement(ElementName = "merchantID")]
        public string merchantID { get; set; }
        [Required]
        [XmlElement(ElementName = "shopID")]
        public string shopID { get; set; }
        [Required]
        [XmlElement(ElementName = "paymentID")]
        public string paymentID { get; set; }
        [Required]
        [XmlElement(ElementName = "actionID")]
        public string actionID { get; set; }

        [XmlElement(ElementName = "remark")]
        public string remark { get; set; }
    }
}
