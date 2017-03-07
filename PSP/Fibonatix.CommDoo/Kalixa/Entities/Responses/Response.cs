using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Fibonatix.CommDoo.Kalixa.Entities.Response
{
    public class Response
    {
        public class Amount
        {
            [XmlAttribute("currencyCode")]
            public string currencyCode { get; set; }

            [XmlText]
            public string value { get; set; }
        }

        public class ResposneState
        {
            [XmlElement("id")]
            public string id { get; set; }
            [XmlElement("definition")]
            public keyStringValuePair definition { get; set; }
            [XmlElement("createdOn")]
            public string createdOn { get; set; }
            [XmlElement("description")]
            public string description { get; set; }

            [XmlArray("paymentStateDetails")]
            [XmlArrayItem(typeof(keyStringValuePair), ElementName = "detail")]
            public dataList paymentStateDetails { get; set; }
        }

        [XmlInclude(typeof(paymentWithPaymentAccount))]
        public class paymentWithPaymentAccount
        {
            [XmlAttribute("type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
            public string type { get; set; }
            [Required]
            [XmlElement(ElementName = "merchantID")]
            public string merchantID { get; set; }
            [Required]
            [XmlElement(ElementName = "shopID")]
            public string shopID { get; set; }
            [Required]
            [XmlElement(ElementName = "paymentMethod")]
            public keyStringValuePair paymentMethod { get; set; }
            [Required]
            [XmlElement(ElementName = "merchantTransactionID")]
            public string merchantTransactionID { get; set; }
            [Required]
            [XmlElement(ElementName = "paymentID")]
            public string paymentID { get; set; }
            [Required]
            [XmlElement(ElementName = "userID")]
            public string userID { get; set; }
            [Required]
            [XmlElement(ElementName = "paymentProvider")]
            public keyStringValuePair paymentProvider { get; set; }
            [Required]
            [XmlElement(ElementName = "amount")]
            public Amount amount { get; set; }
            [Required]
            [XmlElement(ElementName = "creationType")]
            public keyStringValuePair creationType { get; set; }
            [Required]
            [XmlElement(ElementName = "userIP")]
            public string userIP { get; set; }
            [Required]
            [XmlElement(ElementName = "state")]
            public ResposneState state { get; set; }
            [Required]
            [XmlElement(ElementName = "isExecuted")]
            public string isExecuted { get; set; }
            [Required]
            [XmlElement(ElementName = "baseAmount")]
            public Amount baseAmount { get; set; }
            [Required]
            [XmlElement(ElementName = "paymentDetails")]
            public string paymentDetails { get; set; }

            [Required]
            [XmlElement(ElementName = "paymentAccount")]
            public PaymentAccount paymentAccount { get; set; }
            public class PaymentAccount
            {
                [Required]
                [XmlElement(ElementName = "paymentAccountID")]
                public string paymentAccountID { get; set; }
            }
        }


        public class keyStringValuePair
        {
            [XmlAttribute("type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
            public string type { get; set; }
            [XmlElement("key")]
            public string key { get; set; }
            [XmlElement("value")]
            public string value { get; set; }
        }
        public class dataList : List<keyStringValuePair>
        {
            public string GetValueByKey(string key, StringComparison comp = StringComparison.InvariantCulture) {
                var f = Find(x => String.Equals(x.key, key, comp));
                if (f != null)
                    return f.value;
                else
                    return null;
            }
            public string GetValueByKeyNoCase(string key) {
                var f = Find(x => String.Equals(x.key, key, StringComparison.OrdinalIgnoreCase));
                if (f != null)
                    return f.value;
                else
                    return null;
            }
        }
    }
}
