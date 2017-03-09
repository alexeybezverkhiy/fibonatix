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
    [XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class SOAPVirtualCardResponse : SOAPResponse
    {
        [XmlElement(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public string Header { get; set; }

        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public SoapBody Body { get; set; }
        public class SoapBody
        {
            [XmlElement(ElementName = "getVirtualCardResponse", Namespace = "http://Borgun/Heimir/pub/ws/Authorization")]
            public SoapBorgunXMLResponse getVirtualCardResponse { get; set; }

            public class SoapBorgunXMLResponse
            {
                [XmlElement(ElementName = "virtualCardResponseXML", Namespace = "")]
                public string virtualCardResponseXML { get; set; }
            }
        }

        public static SOAPVirtualCardResponse DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(SOAPVirtualCardResponse));

            var reader = new XmlNodeReader(doc.DocumentElement);
            SOAPVirtualCardResponse entry = (SOAPVirtualCardResponse)seri.Deserialize(reader);
            return entry;
        }

        public static SOAPVirtualCardResponse DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }

        public static SOAPVirtualCardResponse DeserializeFromStringSafe(string xmlData) {
            SOAPVirtualCardResponse ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }


    [Serializable()]
    [XmlRoot("VirtualCardResponse")]
    public class VirtualCardResponse : Response
    {
        [XmlElement(ElementName = "VirtualCard")]
        public string VirtualCard { get; set; }

        [XmlElement(ElementName = "Status")]
        public StatusSection Status { get; set; }

        public class StatusSection
        {
            [XmlElement(ElementName = "ResultCode")]
            public string ResultCode { get; set; }
            [XmlElement(ElementName = "ResultText")]
            public string ResultText { get; set; }
        }

        public static VirtualCardResponse DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(VirtualCardResponse));

            var reader = new XmlNodeReader(doc.DocumentElement);
            VirtualCardResponse entry = (VirtualCardResponse)seri.Deserialize(reader);
            return entry;
        }

        public static VirtualCardResponse DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }

        public static VirtualCardResponse DeserializeFromStringSafe(string xmlData) {
            VirtualCardResponse ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }
}
