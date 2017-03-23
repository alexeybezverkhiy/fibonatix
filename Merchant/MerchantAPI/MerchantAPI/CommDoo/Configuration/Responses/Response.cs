using System;
using System.Xml;
using System.Xml.Serialization;

namespace MerchantAPI.CommDoo.Configuration.Responses
{
    [Serializable()]
    [XmlRoot("Response")]
    public class Response
    {
        [XmlElement("ClientID")]
        public string ClientID { get; set; }
        [XmlElement("SharedSecret")]
        public string SharedSecret { get; set; }
        [XmlElement("FrontendRequired")]
        public bool FrontendRequired { get; set; }
        [XmlElement("Error")]
        public ErrorData Error { get; set; }

        public class ErrorData
        {
            [XmlElement("ErrorNumber")]
            public string ErrorNumber { get; set; }
            [XmlElement("ErrorMessage")]
            public string ErrorMessage { get; set; }
        }

        public static Response DeserializeFromXmlDocument(XmlDocument doc)
        {
            XmlSerializer seri = new XmlSerializer(typeof(Response));

            var reader = new XmlNodeReader(doc.DocumentElement);
            Response entry = (Response)seri.Deserialize(reader);
            return entry;
        }

        public static Response DeserializeFromString(string xmlData)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }

        public static Response DeserializeFromStringSafe(string xmlData)
        {
            Response ret = null;
            try
            {
                ret = DeserializeFromString(xmlData);
            }
            catch (Exception)
            {
            }

            return ret;
        }
    }
}