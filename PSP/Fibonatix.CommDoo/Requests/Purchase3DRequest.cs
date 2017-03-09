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
    public class Purchase3DRequest : Request
    {
        public override AcquirerType getAcquirer() {
            return purchase3D.configurations.getAcquirer();
        }
        public override string getConfigValue(string key) {
            return purchase3D.configurations.GetConfigurationValueNoCase(key);
        }

        [Required]
        [XmlElement(ElementName = "Purchase3DSecure")]
        public Purchase3D purchase3D { get; set; }
        public class Purchase3D
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

                // Three fields only for Borgun - RRN, DateAndTime and TerminalID
                [XmlElement(ElementName = "RRN")]
                public string rrn { get; set; }
                [XmlElement(ElementName = "DateAndTime")]
                public string datetime { get; set; }
                [XmlElement(ElementName = "TerminalID")]
                public string terminal { get; set; }

                [XmlElement(ElementName = "RecurringTransaction")]
                public RecurringTransaction recurring_transaction { get; set; }
                [XmlElement(ElementName = "Communication")]
                public Communication3D communication3D { get; set; }
                [XmlElement(ElementName = "CreditCardData")]
                public CreditCardData cred_card_data { get; set; }
                [XmlElement(ElementName = "CustomerData")]
                public CustomerData customer_data { get; set; }
            }
        }

        public override void verification() { // exception
            if (purchase3D == null) {
                string ExceptionMessage = "Incorrect XML for Purchase 3D request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InvalidTransactionTypeError);
            } else if (purchase3D.transaction == null) {
                string ExceptionMessage = "'Transaction' section is not exist in Purchase 3D request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            } else if (getRequestType() == RequestType.NotSupported) {
                string ExceptionMessage = "Unsupported 'Recurrence type' in Purchase 3D request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataInvalidError);
            } else if (purchase3D.transaction.cred_card_data == null && (getAcquirer() != AcquirerType.Kalixa || getRequestType() != RequestType.Repeated)) {
                string ExceptionMessage = "'Credit card' section is not exist in Purchase 3D request for Aquirer who need CreditCard data";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            } else if (purchase3D.transaction.cred_card_data == null && purchase3D.transaction.credit_card_alias == null && getAcquirer() == AcquirerType.Kalixa && getRequestType() == RequestType.Repeated) {
                string ExceptionMessage = "'Credit card' section and 'CreditCardAlias' field are not exist in Purchase 3D request for Aquirer who need CreditCard or CreditCardAlias data";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            } else if (purchase3D.transaction.communication3D == null) {
                string ExceptionMessage = "'Communication' section is not exist in Purchase 3D request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            }
        }

        public static Purchase3DRequest DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(Purchase3DRequest));

            var reader = new XmlNodeReader(doc.DocumentElement);
            Purchase3DRequest entry = (Purchase3DRequest)seri.Deserialize(reader);
            return entry;
        }

        public static Purchase3DRequest DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static Purchase3DRequest DeserializeFromStringSafe(string xmlData) {
            Purchase3DRequest ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
        public RequestType getRequestType() {
            RequestType ret = RequestType.NotSupported;
            try {
                if (purchase3D.transaction != null) {
                    if (purchase3D.transaction.recurring_transaction == null) {
                        ret = RequestType.Single;
                    } else if (purchase3D.transaction.recurring_transaction != null && purchase3D.transaction.recurring_transaction.type != null) {
                        if (String.Equals(purchase3D.transaction.recurring_transaction.type, "initial", StringComparison.OrdinalIgnoreCase)) {
                            ret = RequestType.Initial;
                        } else if (String.Equals(purchase3D.transaction.recurring_transaction.type, "repeated", StringComparison.OrdinalIgnoreCase)) {
                            ret = RequestType.Repeated;
                        } else if (String.Equals(purchase3D.transaction.recurring_transaction.type, "single", StringComparison.OrdinalIgnoreCase)) {
                            ret = RequestType.Single;
                        }
                    }
                }
            } catch (Exception) {
                ret = RequestType.NotSupported;
            } finally { }

            return ret;
        }
    }
}