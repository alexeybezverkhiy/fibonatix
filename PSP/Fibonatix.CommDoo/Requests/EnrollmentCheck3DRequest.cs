using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;
using Fibonatix.CommDoo.Helpers;
using Genesis.Net.Errors;

namespace Fibonatix.CommDoo.Requests
{
    [Serializable()]
    [XmlRoot("Request")]
    public class EnrollmentCheck3DRequest : Request
    {
        public override AcquirerType getAcquirer() {
            return enrollment_check.configurations.getAcquirer();
        }
        public override string getConfigValue(string key) {
            return enrollment_check.configurations.GetConfigurationValueNoCase(key);
        }

        [Required]
        [XmlElement(ElementName = "EnrollmentCheck")]
        public EnrollmetCheck3D enrollment_check { get; set; }
        public class EnrollmetCheck3D
        {
            [XmlArray("Configurations")]
            [XmlArrayItem(typeof(Configuration), ElementName = "Configuration")]
            public Configurations configurations { get; set; }
            [Required]
            [XmlElement(ElementName = "Transaction")]
            public Transaction transaction { get; set; }
            public class Transaction
            {
                [XmlElement(ElementName = "ReferenceID")]
                public string reference_id { get; set; }
                [XmlElement(ElementName = "CreditCardAlias")]
                public string credit_card_alias { get; set; }
                [XmlElement(ElementName = "Amount")]
                public decimal amount { get; set; }
                [XmlElement(ElementName = "Currency")]
                public string currency { get; set; }
                [XmlElement(ElementName = "Usage")]
                public string usage { get; set; }
                [XmlElement(ElementName = "CreditCardData")]
                public CreditCardData cred_card_data { get; set; }
                [XmlElement(ElementName = "CustomerData")]
                public CustomerData customer_data { get; set; }
                [XmlElement(ElementName = "Communication")]
                public Communication3D communication3D { get; set; }
            }
        }

        public override void verification() { // exception
            if (enrollment_check == null) {
                string ExceptionMessage = "Incorrect XML for Enrollment Check request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InvalidTransactionTypeError);
            } else if (enrollment_check.transaction == null) {
                string ExceptionMessage = "'Transaction' section is not exist in Enrollment Check request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            } else if (enrollment_check.transaction.cred_card_data == null && getAcquirer() == AcquirerType.Kalixa) {
                string ExceptionMessage = "'CreditCardData' section is not exist in Enrollment Check request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            }
        }

        public static EnrollmentCheck3DRequest DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(EnrollmentCheck3DRequest));

            var reader = new XmlNodeReader(doc.DocumentElement);
            EnrollmentCheck3DRequest entry = (EnrollmentCheck3DRequest)seri.Deserialize(reader);
            return entry;
        }

        public static EnrollmentCheck3DRequest DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static EnrollmentCheck3DRequest DeserializeFromStringSafe(string xmlData) {
            EnrollmentCheck3DRequest ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }
}