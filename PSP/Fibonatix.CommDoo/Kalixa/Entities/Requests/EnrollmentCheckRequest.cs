using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;

namespace Fibonatix.CommDoo.Kalixa.Entities.Requests
{
    [Serializable()]
    [XmlRoot("initiatePaymentRequest", Namespace = "http://www.cqrpayments.com/PaymentProcessing", IsNullable = true)]
    [XmlInclude(typeof(Request.keyStringValuePair))]
    public class EnrollmentCheckRequest : Request
    {
        public override string getAPIPath() {
            return "initiatePayment";
        }

        [XmlElement(ElementName = "merchantID")]
        public string merchantID { get; set; }
        [XmlElement(ElementName = "shopID")]
        public string shopID { get; set; }
        [XmlElement(ElementName = "merchantTransactionID")]
        public string merchantTransactionID { get; set; }
        [XmlElement(ElementName = "paymentMethodID")]
        public string paymentMethodID { get; set; }
        [XmlElement(ElementName = "amount")]
        public Amount amount { get; set; }
        [XmlElement(ElementName = "userID")]
        public string userID { get; set; }
        [XmlElement(ElementName = "userData")]
        public UserData userData { get; set; }
        [XmlElement(ElementName = "userIP")]
        public string userIP { get; set; }
        [XmlElement(ElementName = "userSessionID")]
        public string userSessionID { get; set; }
        [XmlElement(ElementName = "creationTypeID")]
        public string creationTypeID { get; set; }

        [XmlArray("specificPaymentData")]
        [XmlArrayItem(typeof(keyStringValuePair), ElementName = "data")]
        public dataList specificPaymentData { get; set; }

        [XmlElement(ElementName = "paymentAccountID")]
        public string paymentAccountID { get; set; }
        [XmlElement(ElementName = "paymentAccount")]
        public PaymentAccount paymentAccount { get; set; }
        public class PaymentAccount
        {
            [XmlArray("specificPaymentAccountData")]
            [XmlArrayItem(typeof(keyStringValuePair), ElementName = "data")]
            public dataList specificPaymentAccountData { get; set; }
        }
    }
}
