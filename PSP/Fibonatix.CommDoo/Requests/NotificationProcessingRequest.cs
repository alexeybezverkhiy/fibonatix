using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;

namespace Fibonatix.CommDoo.Requests
{
    [Serializable()]
    [XmlRoot("Request")]
    public class NotificationProcessingRequest : Request
    {
        public override AcquirerType getAcquirer() {
            return notification.configurations.getAcquirer();
        }
        public override string getConfigValue(string key) {
            return notification.configurations.GetConfigurationValueNoCase(key);
        }

        [Required]
        [XmlElement(ElementName = "NotificationProcessing")]
        public Purchase notification { get; set; }
        public class Purchase
        {
            [XmlArray("Configurations")]
            [XmlArrayItem(typeof(Configuration), ElementName = "Configuration")]
            public Configurations configurations { get; set; }

            [Required]
            [XmlElement(ElementName = "RawData")]
            public RawData raw_data { get; set; }
            public class RawData
            {
                [XmlElement(ElementName = "Get")]
                public string get_part { get; set; }
                [XmlElement(ElementName = "Post")]
                public string post_part { get; set; }
            }
        }

        public override void verification() { // exception
        }

        public static NotificationProcessingRequest DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(NotificationProcessingRequest));

            var reader = new XmlNodeReader(doc.DocumentElement);
            NotificationProcessingRequest entry = (NotificationProcessingRequest)seri.Deserialize(reader);
            return entry;
        }

        public static NotificationProcessingRequest DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static NotificationProcessingRequest DeserializeFromStringSafe(string xmlData) {
            NotificationProcessingRequest ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }
}
