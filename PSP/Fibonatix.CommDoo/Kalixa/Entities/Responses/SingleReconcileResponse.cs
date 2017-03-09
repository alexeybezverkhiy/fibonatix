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
    [XmlRoot("getPaymentsResponse", Namespace = "http://www.cqrpayments.com/PaymentProcessing")]
    public class SingeReconcileResponse : Response
    {
        [XmlArray("payments")]
        [XmlArrayItem(typeof(paymentWithPaymentAccount), ElementName = "payment")]
        public paymentsList paymentStateDetails { get; set; }


        public class paymentsList : List<paymentWithPaymentAccount>
        {
            public paymentWithPaymentAccount GetValueByKey(string key) {
                var f = Find(x => String.Equals(x.paymentID, key));
                if (f != null)
                    return f;
                else
                    return null;
            }
        }

        public static SingeReconcileResponse DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(SingeReconcileResponse));

            var reader = new XmlNodeReader(doc.DocumentElement);
            SingeReconcileResponse entry = (SingeReconcileResponse)seri.Deserialize(reader);
            return entry;
        }

        public static SingeReconcileResponse DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static SingeReconcileResponse DeserializeFromStringSafe(string xmlData) {
            SingeReconcileResponse ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }
}
