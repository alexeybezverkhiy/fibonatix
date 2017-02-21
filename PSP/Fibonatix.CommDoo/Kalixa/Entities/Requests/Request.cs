using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Fibonatix.CommDoo.Kalixa.Entities.Requests
{
    [XmlRoot(Namespace = "http://www.cqrpayments.com/PaymentProcessing", IsNullable = true)]
    [XmlInclude(typeof(Request.keyStringValuePair))]
    public abstract class Request
    {
        public abstract string getAPIPath();

        public class Amount
        {
            [XmlAttribute("currencyCode")]
            public string currencyCode { get; set; }

            [XmlText]
            public string value { get; set; }
        }
        public class UserData
        {
            [XmlElement("username")]
            public string username { get; set; }
            [XmlElement("firstname")]
            public string firstname { get; set; }
            [XmlElement("lastname")]
            public string lastname { get; set; }
            [XmlElement("currencyCode")]
            public string currencyCode { get; set; }
            [XmlElement("languageCode")]
            public string languageCode { get; set; }
            [XmlElement("email")]
            public string email { get; set; }
            [XmlElement("address")]
            public Address address { get; set; }
            public class Address
            {
                [XmlElement("street")]
                public string street { get; set; }
                [XmlElement("houseName")]
                public string houseName { get; set; }
                [XmlElement("houseNumber")]
                public string houseNumber { get; set; }
                [XmlElement("houseNumberExtension")]
                public string houseNumberExtension { get; set; }
                [XmlElement("postalCode")]
                public string postalCode { get; set; }
                [XmlElement("city")]
                public string city { get; set; }
                [XmlElement("state")]
                public string state { get; set; }
                [XmlElement("countryCode2")]
                public string countryCode2 { get; set; }
                [XmlElement("telephoneNumber")]
                public string telephoneNumber { get; set; }
            }
            [XmlElement("dateOfBirth")]
            public string dateOfBirth { get; set; }
            [XmlElement("gender")]
            public string gender { get; set; }
            [XmlElement("identificationNumber")]
            public string identificationNumber { get; set; }
        }

        public class keyStringValuePair
        {
            [XmlAttribute("type", Namespace = "http://www.w3.org/2001/XMLSchema-instance") ]
            public string type { get; set; }
            [XmlElement("key")]
            public string key { get; set; }
            [XmlElement("value")]
            public string value { get; set; }
        }

        public class dataList : List<keyStringValuePair>
        {
        }

        public string getXml() {
            XmlSerializer formatter = new XmlSerializer(this.GetType());
            StringWriter writer = new Utf8StringWriter();
            formatter.Serialize(writer, this);
            var serializedValue = writer.ToString();
            return serializedValue;
        }
        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

    }
}
