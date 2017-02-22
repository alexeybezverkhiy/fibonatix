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
    [XmlRoot("getPaymentsRequest", Namespace = "http://www.cqrpayments.com/PaymentProcessing", IsNullable = true)]
    public class SingleReconcileRequest : Request
    {
        public override string getAPIPath() {
            return "getPayments";
        }

        [Required]
        [XmlElement(ElementName = "merchantID")]
        public string merchantID { get; set; }
        [Required]
        [XmlElement(ElementName = "shopID")]
        public string shopID { get; set; }
        [Required]
        [XmlElement(ElementName = "merchantTransactionID")]
        public string merchantTransactionID { get; set; }
    }
}

