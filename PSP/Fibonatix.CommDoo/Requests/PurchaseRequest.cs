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
    public class PurchaseRequest : Request
    {
        public override AcquirerType getAcquirer() {
            return purchase.configurations.getAcquirer();
        }
        public override string getConfigValue(string key) {
            return purchase.configurations.GetConfigurationValueNoCase(key);
        }

        [Required]
        [XmlElement(ElementName = "Purchase")]
        public Purchase purchase { get; set; }
        public class Purchase
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
                [XmlElement(ElementName = "ProviderTransactionID")]
                public string provider_transaction_id { get; set; }
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
                [XmlElement(ElementName = "CreditCardData")]
                public CreditCardData cred_card_data { get; set; }
                [XmlElement(ElementName = "ThreeDSecure")]
                public Secure3D threed_secure { get; set; }
                [XmlElement(ElementName = "CustomerData")]
                public CustomerData customer_data { get; set; }
            }
        }

        public override void verification() { // exception
            if (purchase == null) {
                string ExceptionMessage = "Incorrect XML for Purchase request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InvalidTransactionTypeError);
            } else if (purchase.transaction == null) {
                string ExceptionMessage = "'Transaction' section is not exist in Purchase request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            } else if (getRequestType() == RequestType.NotSupported) {
                string ExceptionMessage = "Unsupported 'Recurrence type' in Purchase request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataInvalidError);
            } else if (purchase.transaction.cred_card_data == null && 
                    ((getAcquirer() != AcquirerType.Kalixa && getAcquirer() != AcquirerType.Borgun) || getRequestType() != RequestType.Repeated)) {
                string ExceptionMessage = "'Credit card' section is not exist in Purchase request for Aquirer who need CreditCard data";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            } else if (purchase.transaction.cred_card_data == null && 
                    purchase.transaction.credit_card_alias == null && 
                    (getAcquirer() == AcquirerType.Kalixa || getAcquirer() == AcquirerType.Borgun) && 
                    getRequestType() == RequestType.Repeated) {
                string ExceptionMessage = "'Credit card' section and 'CreditCardAlias' field are not exist in Purchase request for Aquirer who need CreditCard or CreditCardAlias data";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            }
        }

        public static PurchaseRequest DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(PurchaseRequest));

            var reader = new XmlNodeReader(doc.DocumentElement);
            PurchaseRequest entry = (PurchaseRequest)seri.Deserialize(reader);
            return entry;
        }

        public static PurchaseRequest DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static PurchaseRequest DeserializeFromStringSafe(string xmlData) {
            PurchaseRequest ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }

        public RequestType getRequestType( ) {
            RequestType ret = RequestType.NotSupported;
            try {
                if (purchase.transaction != null ) {
                    if (purchase.transaction.recurring_transaction == null) {
                        ret = RequestType.Single;
                    } else if (purchase.transaction.recurring_transaction != null && purchase.transaction.recurring_transaction.type != null) {
                        if (String.Equals(purchase.transaction.recurring_transaction.type, "initial", StringComparison.OrdinalIgnoreCase)) {
                            ret = RequestType.Initial;
                        } else if (String.Equals(purchase.transaction.recurring_transaction.type, "repeated", StringComparison.OrdinalIgnoreCase)) {
                            ret = RequestType.Repeated;
                        } else if (String.Equals(purchase.transaction.recurring_transaction.type, "single", StringComparison.OrdinalIgnoreCase)) {
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

