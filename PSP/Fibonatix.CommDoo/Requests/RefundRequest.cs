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
    public class RefundRequest : Request
    {
        public override AcquirerType getAcquirer() {
            return refund.configurations.getAcquirer();
        }
        public override string getConfigValue(string key) {
            return refund.configurations.GetConfigurationValueNoCase(key);
        }

        [Required]
        [XmlElement(ElementName = "Refund")]
        public Refund refund { get; set; }
        public class Refund
        {
            [XmlArray("Configurations")]
            [XmlArrayItem(typeof(Configuration), ElementName = "Configuration")]
            public Configurations configurations { get; set; }

            [Required]
            [XmlElement(ElementName = "Transaction")]
            public Transaction transaction { get; set; }
            public class Transaction
            {
                [Required]
                [XmlElement(ElementName = "ReferenceID")]
                public string reference_id { get; set; }
                [Required]
                [XmlElement(ElementName = "ProviderTransactionID")]
                public string provider_transaction_id { get; set; }
                [Required]
                [XmlElement(ElementName = "CreditCardAlias")]
                public string credit_card_alias { get; set; }
                [XmlElement(ElementName = "Amount")]
                public decimal amount { get; set; }
                [XmlElement(ElementName = "Currency")]
                public string currency { get; set; }
                [XmlElement(ElementName = "Usage")]
                public string usage { get; set; }
                [XmlElement(ElementName = "RefundType")]
                public string refund_type { get; set; }

                // Three fields only for Borgun - RRN, DateAndTime and TerminalID
                [XmlElement(ElementName = "RRN")]
                public string rrn { get; set; }
                [XmlElement(ElementName = "DateAndTime")]
                public string datetime { get; set; }
                [XmlElement(ElementName = "TerminalID")]
                public string terminal { get; set; }

                [XmlElement(ElementName = "CreditCardData")]
                public CreditCardData cred_card_data { get; set; }
            }
        }

        public override void verification() { // exception
            if (refund == null) {
                string ExceptionMessage = "Incorrect XML for Refund request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InvalidTransactionTypeError);
            } else if (refund.transaction == null) {
                string ExceptionMessage = "'Transaction' section is not exist in Refund request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            } else if (refund.transaction.cred_card_data == null && getAcquirer() == AcquirerType.Kalixa) {
                string ExceptionMessage = "'CreditCardData' section is not exist in Refund request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            }
        }

        public static RefundRequest DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(RefundRequest));

            var reader = new XmlNodeReader(doc.DocumentElement);
            RefundRequest entry = (RefundRequest)seri.Deserialize(reader);
            return entry;
        }

        public static RefundRequest DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static RefundRequest DeserializeFromStringSafe(string xmlData) {
            RefundRequest ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }
}


