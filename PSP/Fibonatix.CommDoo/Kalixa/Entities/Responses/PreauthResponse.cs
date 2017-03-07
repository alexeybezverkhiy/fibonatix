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
    public class PreauthResponse : Response
    {
        [Required]
        [XmlElement(ElementName = "payment")]        
        public paymentWithPaymentAccount payment { get; set; }

        public static PreauthResponse DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(PreauthResponse));

            var reader = new XmlNodeReader(doc.DocumentElement);
            PreauthResponse entry = (PreauthResponse)seri.Deserialize(reader);
            return entry;
        }

        public static PreauthResponse DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static PreauthResponse DeserializeFromStringSafe(string xmlData) {
            PreauthResponse ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }
}
