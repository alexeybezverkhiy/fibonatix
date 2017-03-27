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
    [XmlRoot("initiatePaymentResponse", Namespace = "http://www.cqrpayments.com/PaymentProcessing")]
    [XmlInclude(typeof(paymentWithPaymentAccount))]
    public class Preauth3DResponse : Response
    {
        [Required]
        [XmlElement(ElementName = "payment")]
        public paymentWithPaymentAccount payment { get; set; }

        public static Preauth3DResponse DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(Preauth3DResponse));

            var reader = new XmlNodeReader(doc.DocumentElement);
            Preauth3DResponse entry = (Preauth3DResponse)seri.Deserialize(reader);
            return entry;
        }

        public static Preauth3DResponse DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static Preauth3DResponse DeserializeFromStringSafe(string xmlData) {
            Preauth3DResponse ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }
}
