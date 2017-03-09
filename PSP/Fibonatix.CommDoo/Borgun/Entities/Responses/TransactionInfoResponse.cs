using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Fibonatix.CommDoo.Borgun.Entities.Responses
{
    [Serializable()]
    [XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class SOAPTransactionInfoResponse : SOAPResponse
    {
        [XmlElement(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public string Header { get; set; }

        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public SoapBody Body { get; set; }
        public class SoapBody
        {
            [XmlElement(ElementName = "getTransactionListResponse", Namespace = "http://Borgun/Heimir/pub/ws/Authorization")]
            public SoapBorgunXMLResponse getTransactionListResponse { get; set; }

            public class SoapBorgunXMLResponse
            {
                [XmlElement(ElementName = "transactionListXML", Namespace = "")]
                public string transactionListXML { get; set; }
            }
        }

        public static SOAPTransactionInfoResponse DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(SOAPTransactionInfoResponse));

            var reader = new XmlNodeReader(doc.DocumentElement);
            SOAPTransactionInfoResponse entry = (SOAPTransactionInfoResponse)seri.Deserialize(reader);
            return entry;
        }

        public static SOAPTransactionInfoResponse DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }

        public static SOAPTransactionInfoResponse DeserializeFromStringSafe(string xmlData) {
            SOAPTransactionInfoResponse ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }

    [Serializable()]
    [XmlRoot("TransactionList")]
    //[XmlArray("TransactionList")]
    public class TransactionInfoResponse : Response
    {
        [XmlElement(ElementName = "Version")]
        public string Version { get; set; }
        [XmlElement(ElementName = "Processor")]
        public string Processor { get; set; }
        [XmlElement(ElementName = "MerchantID")]
        public string MerchantID { get; set; }
        [XmlElement(ElementName = "TerminalID")]
        public string TerminalID { get; set; }
        [XmlElement(ElementName = "ActionCode")]
        public string ActionCode { get; set; }


        [XmlElement(ElementName = "Transaction")]
        public TransactionInfo[] transactions { get; set; }

        public class TransactionInfo {
            [XmlElement(ElementName = "TransactionType")]
            public string TransactionType { get; set; }
            [XmlElement(ElementName = "TransactionNumber")]
            public string TransactionNumber { get; set; }
            [XmlElement(ElementName = "BatchNumber")]
            public string BatchNumber { get; set; }
            [XmlElement(ElementName = "TransactionDate")]
            public string TransactionDate { get; set; }
            [XmlElement(ElementName = "PAN")]
            public string PAN { get; set; }
            [XmlElement(ElementName = "RRN")]
            public string RRN { get; set; }
            [XmlElement(ElementName = "ActionCode")]
            public string ActionCode { get; set; }
            [XmlElement(ElementName = "AuthorizationCode")]
            public string AuthorizationCode { get; set; }
            [XmlElement(ElementName = "TrAmount")]
            public string TrAmount { get; set; }
            [XmlElement(ElementName = "TrCurrency")]
            public string TrCurrency { get; set; }
            [XmlElement(ElementName = "CardType")]
            public string CardType { get; set; }
            [XmlElement(ElementName = "Voided")]
            public string Voided { get; set; }
            [XmlElement(ElementName = "Status")]
            public string Status { get; set; }
            [XmlElement(ElementName = "TerminalNr")]
            public string TerminalNr { get; set; }
            [XmlElement(ElementName = "Credit")]
            public string Credit { get; set; }
        }

        public static TransactionInfoResponse DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(TransactionInfoResponse));

            var reader = new XmlNodeReader(doc.DocumentElement);
            TransactionInfoResponse entry = (TransactionInfoResponse)seri.Deserialize(reader);
            return entry;
        }

        public static TransactionInfoResponse DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }

        public static TransactionInfoResponse DeserializeFromStringSafe(string xmlData) {
            TransactionInfoResponse ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }
}
