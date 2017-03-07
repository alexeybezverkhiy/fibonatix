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
    public class SOAPTransactionInfoRequest : SOAPRequest
    {
        public override string SOAPAction() { return "Heimir_pub_ws_Authorization_Binder_getTransactionList"; }

        [XmlElement(ElementName = "Header")]
        public string Header { get { return ""; } set { } }

        [XmlElement(ElementName = "Body")]
        public SOAPBody Body { get; set; }

        [XmlRoot("Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public class SOAPBody
        {
            [XmlElement(ElementName = "getTransactionList", Namespace = "http://Borgun/Heimir/pub/ws/Authorization")]
            public RequestContainer getTransactionList { get; set; }

            [XmlRoot(Namespace = "http://Borgun/Heimir/pub/ws/Authorization")]
            public class RequestContainer
            {
                [XmlElement(ElementName = "transactionListReqXML", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
                public string transactionListReqXML { get; set; }
            }
        }
    }


    [Serializable()]
    [XmlRoot("TransactionListRequest")]
    public class TransactionInfoRequest : Request
    {
        [XmlElement(ElementName = "Version")]
        public string Version { get; set; }
        [XmlElement(ElementName = "Processor")]
        public string Processor { get; set; }
        [XmlElement(ElementName = "MerchantId")]
        public string MerchantID { get; set; }
        [XmlElement(ElementName = "TerminalId")]
        public string TerminalID { get; set; }
        [XmlElement(ElementName = "BatchNumber")]
        public string BatchNumber { get; set; }
        [XmlElement(ElementName = "FromDate")]
        public string FromDate { get; set; }
        [XmlElement(ElementName = "ToDate")]
        public string ToDate { get; set; }
        [XmlElement(ElementName = "RRN")]
        public string RRN { get; set; }
    }
}
