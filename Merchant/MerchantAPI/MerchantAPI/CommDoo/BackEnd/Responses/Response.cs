using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Net;

namespace MerchantAPI.CommDoo.BackEnd.Responses
{
    [Serializable()]
    [XmlRoot("Response")]
    public class Response
    {
        [XmlElement("Payment")]
        public PaymentData Payment { get; set; }
        [XmlElement("Redirection")]
        public RedirectionData Redirection { get; set; }
        [XmlElement("Order")]
        public OrderData Order { get; set; }
        [XmlElement("Error")]
        public ErrorData Error { get; set; }
        [XmlElement("Security")]
        public SecurityData Security { get; set; }

        public class PaymentData
        {
            [XmlElement("TransactionID")]
            public string TransactionID { get; set; }
            // [XmlElement("PaymentType")]
            // public string PaymentType { get; set; }
            [XmlElement("PaymentAdvice")]
            public string PaymentAdvice { get; set; }
            [XmlElement("Status")]
            public string Status { get; set; }
            [XmlElement("StatusAddition")]
            public string StatusAddition { get; set; }
            [XmlElement("Amount")]
            public string Amount { get; set; }
            [XmlElement("Currency")]
            public string Currency { get; set; }
            [XmlElement("ReferenceID")]
            public string ReferenceID { get; set; }
            [XmlElement("ProviderTransactionID")]
            public string ProviderTransactionID { get; set; }
            [XmlElement("AdditionalData")]
            public string AdditionalData { get; set; }
            [XmlElement("Subscription")]
            public SubscriptionData Subscription { get; set; }
            [XmlElement("Customer")]
            public CustomerData Customer { get; set; }
        }
        public class SubscriptionData
        {
            [XmlElement("SubscriptionID")]
            public string SubscriptionID { get; set; }
        }
        public class CustomerData
        {
            [XmlElement("CustomerID")]
            public string CustomerID { get; set; }
            [XmlElement("ReferenceCustomerID")]
            public string ReferenceCustomerID { get; set; }
            [XmlElement("CreditCard")]
            public CreditCardData CreditCard { get; set; }
        }
        public class CreditCardData
        {
            [XmlElement("CreditCardType")]
            public string CreditCardType { get; set; }
        }
        public class RedirectionData
        {
            [XmlElement("RedirectURL")]
            public string RedirectURL { get; set; }
        }
        public class ErrorData
        {
            [XmlElement("ErrorNumber")]
            public string ErrorNumber { get; set; }
            [XmlElement("ErrorMessage")]
            public string ErrorMessage { get; set; }
        }
        public class OrderData
        {
            [XmlElement("OrderID")]
            public string OrderID { get; set; }
            [XmlElement("Status")]
            public string Status { get; set; }
        }
        public class SecurityData
        {
            [XmlElement("Timestamp")]
            public string Timestamp { get; set; }
            [XmlElement("Hash")]
            public string Hash { get; set; }
        }

        public static Response DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(Response));

            var reader = new XmlNodeReader(doc.DocumentElement);
            Response entry = (Response)seri.Deserialize(reader);
            return entry;
        }

        public static Response DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static Response DeserializeFromStringSafe(string xmlData) {
            Response ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }
}