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
    [XmlRoot("paymentServiceException" /*, Namespace = "http://www.cqrpayments.com/PaymentProcessing"*/)]
    public class ExceptionResponse : Response
    {
        [Required]
        [XmlElement(ElementName = "id")]
        public string id { get; set; }
        [Required]
        [XmlElement(ElementName = "errorCode")]
        public string errorCode { get; set; }
        [Required]
        [XmlElement(ElementName = "errorMessage")]
        public string errorMessage { get; set; }

        public static ExceptionResponse DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(ExceptionResponse));

            var reader = new XmlNodeReader(doc.DocumentElement);
            ExceptionResponse entry = (ExceptionResponse)seri.Deserialize(reader);
            return entry;
        }

        public static ExceptionResponse DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static ExceptionResponse DeserializeFromStringSafe(string xmlData) {
            ExceptionResponse ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }

    }
}
