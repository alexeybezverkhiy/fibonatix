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
    public class EvaluateProviderResponseRequest : Request
    {
        public override AcquirerType getAcquirer() {
            return evaluation.configurations.getAcquirer();
        }
        public override string getConfigValue(string key) {
            return evaluation.configurations.GetConfigurationValueNoCase(key);
        }

        [Required]
        [XmlElement(ElementName = "EvaluateProviderResponse")]
        public Purchase evaluation { get; set; }
        public class Purchase
        {
            [XmlArray("Configurations")]
            [XmlArrayItem(typeof(Configuration), ElementName = "Configuration")]
            public Configurations configurations { get; set; }

            [Required]
            [XmlElement(ElementName = "RawData")]
            public Transaction raw_data { get; set; }
            public class Transaction
            {
                [XmlElement(ElementName = "Get")]
                public string get_part { get; set; }
                [XmlElement(ElementName = "Post")]
                public string post_part { get; set; }
            }
        }

        public override void verification() { // exception
        }

        public static EvaluateProviderResponseRequest DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(EvaluateProviderResponseRequest));

            var reader = new XmlNodeReader(doc.DocumentElement);
            EvaluateProviderResponseRequest entry = (EvaluateProviderResponseRequest)seri.Deserialize(reader);
            return entry;
        }

        public static EvaluateProviderResponseRequest DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static EvaluateProviderResponseRequest DeserializeFromStringSafe(string xmlData) {
            EvaluateProviderResponseRequest ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }
}
