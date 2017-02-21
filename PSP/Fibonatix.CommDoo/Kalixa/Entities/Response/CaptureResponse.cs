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
    [XmlRoot("executePaymentActionResponse", Namespace = "http://www.cqrpayments.com/PaymentProcessing")]
    public class CaptureResponse : Response
    {
        [Required]
        [XmlElement(ElementName = "statusCode")]
        public string statusCode { get; set; }

        [XmlArray("actionResults")]
        [XmlArrayItem(typeof(keyStringValuePair), ElementName = "result")]
        public dataList actionResults { get; set; }

        public static CaptureResponse DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(CaptureResponse));

            var reader = new XmlNodeReader(doc.DocumentElement);
            CaptureResponse entry = (CaptureResponse)seri.Deserialize(reader);
            return entry;
        }

        public static CaptureResponse DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static CaptureResponse DeserializeFromStringSafe(string xmlData) {
            CaptureResponse ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }
}
