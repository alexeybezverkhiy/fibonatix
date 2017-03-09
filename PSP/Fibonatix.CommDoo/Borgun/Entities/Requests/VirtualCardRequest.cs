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
    public class SOAPVirtualCardRequest : SOAPRequest
    {
        public override string SOAPAction() { return "Heimir_pub_ws_Authorization_Binder_getVirtualCard"; }

        [XmlElement(ElementName = "Header")]
        public string Header { get { return ""; } set { } }

        [XmlElement(ElementName = "Body")]
        public SOAPBody Body { get; set; }

        [XmlRoot("Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public class SOAPBody
        {
            [XmlElement(ElementName = "getVirtualCard", Namespace = "http://Borgun/Heimir/pub/ws/Authorization")]
            public RequestContainer getVirtualCard { get; set; }

            [XmlRoot(Namespace = "http://Borgun/Heimir/pub/ws/Authorization")]
            public class RequestContainer
            {
                [XmlElement(ElementName = "virtualCardRequestXML", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
                public string virtualCardRequestXML { get; set; }
            }
        }
    }


    [Serializable()]
    [XmlRoot("getVirtualCard")]
    public class VirtualCardRequest : Request
    {
        [XmlElement(ElementName = "MerchantContractNumber")]
        public string MerchantContractNumber { get; set; }
        [XmlElement(ElementName = "PAN")]
        public string PAN { get; set; }
    }
}
