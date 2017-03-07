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
    public class SOAPCancelResponse : SOAPResponse
    {
        [XmlElement(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public string Header { get; set; }

        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public SoapBody Body { get; set; }
        public class SoapBody
        {
            [XmlElement(ElementName = "cancelAuthorizationResponse", Namespace = "http://Borgun/Heimir/pub/ws/Authorization")]
            public SoapBorgunXMLResponse cancelAuthorizationResponse { get; set; }

            public class SoapBorgunXMLResponse
            {
                [XmlElement(ElementName = "cancelAuthResXml", Namespace = "")]
                public string cancelAuthResXml { get; set; }
            }
        }

        public static SOAPCancelResponse DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(SOAPCancelResponse));

            var reader = new XmlNodeReader(doc.DocumentElement);
            SOAPCancelResponse entry = (SOAPCancelResponse)seri.Deserialize(reader);
            return entry;
        }

        public static SOAPCancelResponse DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }

        public static SOAPCancelResponse DeserializeFromStringSafe(string xmlData) {
            SOAPCancelResponse ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }

    [Serializable()]
    [XmlRoot("cancelAuthorizationReply")]
    public class CancelResponse : Response
    {
        [XmlElement(ElementName = "Version")]
        public string Version { get; set; }
        [XmlElement(ElementName = "Processor")]
        public string Processor { get; set; }
        [XmlElement(ElementName = "MerchantID")]
        public string MerchantID { get; set; }
        [XmlElement(ElementName = "TerminalID")]
        public string TerminalID { get; set; }
        [XmlElement(ElementName = "TransType")]
        public string TransType { get; set; }
        [XmlElement(ElementName = "TrAmount")]
        public string TrAmount { get; set; }
        [XmlElement(ElementName = "TrCurrency")]
        public string TrCurrency { get; set; }
        [XmlElement(ElementName = "DateAndTime")]
        public string DateAndTime { get; set; }
        [XmlElement(ElementName = "PAN")]
        public string PAN { get; set; }
        [XmlElement(ElementName = "RRN")]
        public string RRN { get; set; }
        [XmlElement(ElementName = "Transaction")]
        public string Transaction { get; set; }
        [XmlElement(ElementName = "Batch")]
        public string Batch { get; set; }
        [XmlElement(ElementName = "CVCResult")]
        public string CVCResult { get; set; }
        [XmlElement(ElementName = "CardAccId")]
        public string CardAccId { get; set; }
        [XmlElement(ElementName = "CardAccTerminal")]
        public string CardAccTerminal { get; set; }
        [XmlElement(ElementName = "CardAccName")]
        public string CardAccName { get; set; }
        [XmlElement(ElementName = "AuthCode")]
        public string AuthCode { get; set; }
        [XmlElement(ElementName = "ActionCode")]
        public string ActionCode { get; set; }
        [XmlElement(ElementName = "StoreTerminal")]
        public string StoreTerminal { get; set; }
        [XmlElement(ElementName = "CardType")]
        public string CardType { get; set; }
        [XmlElement(ElementName = "Message")]
        public string Message { get; set; }

        public static CancelResponse DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(CancelResponse));

            var reader = new XmlNodeReader(doc.DocumentElement);
            CancelResponse entry = (CancelResponse)seri.Deserialize(reader);
            return entry;
        }

        public static CancelResponse DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }

        public static CancelResponse DeserializeFromStringSafe(string xmlData) {
            CancelResponse ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }
}
