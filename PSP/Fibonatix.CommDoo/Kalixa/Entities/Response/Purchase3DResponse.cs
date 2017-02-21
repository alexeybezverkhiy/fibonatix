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
    public class Purchase3DResponse : Response
    {
        public static Purchase3DResponse DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(Purchase3DResponse));

            var reader = new XmlNodeReader(doc.DocumentElement);
            Purchase3DResponse entry = (Purchase3DResponse)seri.Deserialize(reader);
            return entry;
        }

        public static Purchase3DResponse DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static Purchase3DResponse DeserializeFromStringSafe(string xmlData) {
            Purchase3DResponse ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }
}
