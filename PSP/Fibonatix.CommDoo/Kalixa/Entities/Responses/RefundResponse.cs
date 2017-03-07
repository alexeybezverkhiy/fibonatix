using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;

namespace Fibonatix.CommDoo.Kalixa.Entities.Response
{
    [Serializable()]
    [XmlRoot("initiatePaymentFromReferenceResponse", Namespace = "http://www.cqrpayments.com/PaymentProcessing")]
    public class RefundResponse : Response
    {
        [Required]
        [XmlElement(ElementName = "payment")]
        public paymentWithPaymentAccount payment { get; set; }

        public static RefundResponse DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(RefundResponse));

            var reader = new XmlNodeReader(doc.DocumentElement);
            RefundResponse entry = (RefundResponse)seri.Deserialize(reader);
            return entry;
        }

        public static RefundResponse DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static RefundResponse DeserializeFromStringSafe(string xmlData) {
            RefundResponse ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }
}
