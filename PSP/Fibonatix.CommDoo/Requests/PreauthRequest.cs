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
    public class PreauthRequest : Request
    {
        public override AcquirerType getAcquirer() {
            return preAuth.configurations.getAcquirer();
        }
        public override string getConfigValue(string key) {
            return preAuth.configurations.GetConfigurationValueNoCase(key);
        }

        [Required]
        [XmlElement(ElementName = "Preauthorization")]
        public PreAuthorization preAuth { get; set; }
        public class PreAuthorization
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
            if (preAuth == null) {
                string ExceptionMessage = "Incorrect XML for Preauthorization request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InvalidTransactionTypeError);
            } else if (preAuth.transaction == null) {
                string ExceptionMessage = "'Transaction' section is not exist in Preauthorization request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            } else if (getRequestType() == RequestType.NotSupported) {
                string ExceptionMessage = "Not supported 'Recurrence type' in Preauthorization request for selected Acquirer";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataInvalidError);
            } else if (preAuth.transaction.cred_card_data == null && (getAcquirer() != AcquirerType.Kalixa || getRequestType() != RequestType.Repeated)) {
                string ExceptionMessage = "'Credit card' section is not exist in Preauthorization request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            } else if (preAuth.transaction.cred_card_data == null && preAuth.transaction.credit_card_alias == null && getAcquirer() == AcquirerType.Kalixa && getRequestType() == RequestType.Repeated) {
                string ExceptionMessage = "'Credit card' section and 'CreditCardAlias' field are not exist in Preauthorization request for Aquirer who need CreditCard or CreditCardAlias data";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            }
        }

        public static PreauthRequest DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(PreauthRequest));

            var reader = new XmlNodeReader(doc.DocumentElement);
            PreauthRequest entry = (PreauthRequest)seri.Deserialize(reader);
            return entry;
        }

        public static PreauthRequest DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static PreauthRequest DeserializeFromStringSafe(string xmlData) {
            PreauthRequest ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
        public RequestType getRequestType() {
            RequestType ret = RequestType.NotSupported;
            try {
                if (preAuth.transaction != null) {
                    if (preAuth.transaction.recurring_transaction == null) {
                        ret = RequestType.Single;
                    } else if (preAuth.transaction.recurring_transaction != null && preAuth.transaction.recurring_transaction.type != null) {
                        if (String.Equals(preAuth.transaction.recurring_transaction.type, "initial", StringComparison.InvariantCultureIgnoreCase) && getAcquirer() == AcquirerType.Kalixa) {
                            ret = RequestType.Initial;
                        } else if (String.Equals(preAuth.transaction.recurring_transaction.type, "repeated", StringComparison.InvariantCultureIgnoreCase) && getAcquirer() == AcquirerType.Kalixa) {
                            ret = RequestType.Repeated;
                        } else if (String.Equals(preAuth.transaction.recurring_transaction.type, "single", StringComparison.InvariantCultureIgnoreCase)) {
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