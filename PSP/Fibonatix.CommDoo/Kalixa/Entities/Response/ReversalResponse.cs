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
    public class ReversalResponse : Response
    {
        [Required]
        [XmlElement(ElementName = "statusCode")]
        public string statusCode { get; set; }

        [XmlArray("actionResults")]
        [XmlArrayItem(typeof(keyStringValuePair), ElementName = "result")]
        public dataList actionResults { get; set; }

        public static ReversalResponse DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(ReversalResponse));

            var reader = new XmlNodeReader(doc.DocumentElement);
            ReversalResponse entry = (ReversalResponse)seri.Deserialize(reader);
            return entry;
        }

        public static ReversalResponse DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static ReversalResponse DeserializeFromStringSafe(string xmlData) {
            ReversalResponse ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }
}
