using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Fibonatix.CommDoo.Borgun.Entities.Requests
{
    [Serializable()]
    [XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class SOAPAuthRequest : SOAPRequest
    {
        public override string SOAPAction() { return "Heimir_pub_ws_Authorization_Binder_getAuthorization"; }

        [XmlElement(ElementName = "Header")]
        public string Header { get { return ""; } set { } }

        [XmlElement(ElementName = "Body")]
        public SOAPBody Body { get; set; }

        [XmlRoot("Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public class SOAPBody
        {
            [XmlElement(ElementName = "getAuthorizationInput", Namespace = "http://Borgun/Heimir/pub/ws/Authorization")]
            public RequestContainer getAuthorizationInput { get; set; }

            [XmlRoot(Namespace = "http://Borgun/Heimir/pub/ws/Authorization")]
            public class RequestContainer
            {
                [XmlElement(ElementName = "getAuthReqXml", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
                public string getAuthReqXml { get; set; }
            }
        }
    }


    [Serializable()]
    [XmlRoot("getAuthorization")]
    public class AuthRequest : Request
    {
        public AuthRequest() {
            TransType = ((int)TransactionType.Undefined).ToString();
        }
        public AuthRequest(TransactionType type) {
            TransType = ((int)type).ToString();
        }

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
        [XmlElement(ElementName = "NewAmount")]
        public string NewAmount { get; set; }
        [XmlElement(ElementName = "TrCurrency")]
        public string TrCurrency { get; set; }
        [XmlElement(ElementName = "DateAndTime")]
        public string DateAndTime { get; set; }
        [XmlElement(ElementName = "PAN")]
        public string PAN { get; set; }
        [XmlElement(ElementName = "ExpDate")]
        public string ExpDate { get; set; }
        [XmlElement(ElementName = "Track1")]
        public string Track1 { get; set; }
        [XmlElement(ElementName = "Track2")]
        public string Track2 { get; set; }
        [XmlElement(ElementName = "RRN")]
        public string RRN { get; set; }
        [XmlElement(ElementName = "AuthCode")]
        public string AuthCode { get; set; }
        [XmlElement(ElementName = "CVC2")]
        public string CVC2 { get; set; }
        [XmlElement(ElementName = "SecurityLevelInd")]
        public string SecurityLevelInd { get; set; }
        [XmlElement(ElementName = "UCAF")]
        public string UCAF { get; set; }
        [XmlElement(ElementName = "CAVV")]
        public string CAVV { get; set; }
        [XmlElement(ElementName = "XID")]
        public string XID { get; set; }
        [XmlElement(ElementName = "MerchantName")]
        public string MerchantName { get; set; }
        [XmlElement(ElementName = "MerchantHome")]
        public string MerchantHome { get; set; }
        [XmlElement(ElementName = "MerchantCity")]
        public string MerchantCity { get; set; }
        [XmlElement(ElementName = "MerchantZipCode")]
        public string MerchantZipCode { get; set; }
        [XmlElement(ElementName = "MerchantCountry")]
        public string MerchantCountry { get; set; }
    }
}
