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
    public class EnrollmentCheckResponse : Response
    {
        [Required]
        [XmlElement(ElementName = "payment")]        
        public paymentWithPaymentAccount payment { get; set; }

        public static EnrollmentCheckResponse DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(EnrollmentCheckResponse));

            var reader = new XmlNodeReader(doc.DocumentElement);
            EnrollmentCheckResponse entry = (EnrollmentCheckResponse)seri.Deserialize(reader);
            return entry;
        }

        public static EnrollmentCheckResponse DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static EnrollmentCheckResponse DeserializeFromStringSafe(string xmlData) {
            EnrollmentCheckResponse ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }
}
