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
    public class Preauth3DRequest : Request
    {
        public override AcquirerType getAcquirer() {
            return preAuth3D.configurations.getAcquirer();
        }
        public override string getConfigValue(string key) {
            return preAuth3D.configurations.GetConfigurationValueNoCase(key);
        }

        [Required]                                   
        [XmlElement(ElementName = "Preauthorization3DSecure")]
        public PreAuthorization3D preAuth3D { get; set; }
        public class PreAuthorization3D
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
            if (preAuth3D == null) {
                string ExceptionMessage = "Incorrect XML for Preauthorization 3D request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InvalidTransactionTypeError);
            } else if (preAuth3D.transaction == null) {
                string ExceptionMessage = "'Transaction' section is not exist in Preauthorization 3D request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            } else if (getRequestType() == RequestType.NotSupported) {
                string ExceptionMessage = "'Recurrence type != SINGLE' not supported in Preauthorization 3D request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataInvalidError);
            } else if (preAuth3D.transaction.cred_card_data == null && (getAcquirer() != AcquirerType.Kalixa || getRequestType() != RequestType.Repeated)) {
                string ExceptionMessage = "'Credit card' section is not exist in Preauthorization 3D request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            } else if (preAuth3D.transaction.cred_card_data == null && preAuth3D.transaction.credit_card_alias == null && getAcquirer() == AcquirerType.Kalixa && getRequestType() == RequestType.Repeated) {
                string ExceptionMessage = "'Credit card' section and 'CreditCardAlias' field are not exist in Preauthorization 3D request for Aquirer who need CreditCard or CreditCardAlias data";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            } else if (preAuth3D.transaction.communication3D == null) {
                string ExceptionMessage = "'Communication' section is not exist in Preauthorization 3D request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            }
        }

        public static Preauth3DRequest DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(Preauth3DRequest));

            var reader = new XmlNodeReader(doc.DocumentElement);
            Preauth3DRequest entry = (Preauth3DRequest)seri.Deserialize(reader);
            return entry;
        }

        public static Preauth3DRequest DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static Preauth3DRequest DeserializeFromStringSafe(string xmlData) {
            Preauth3DRequest ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
        public RequestType getRequestType() {
            RequestType ret = RequestType.NotSupported;
            try {
                if (preAuth3D.transaction != null) {
                    if (preAuth3D.transaction.recurring_transaction == null) {
                        ret = RequestType.Single;
                    } else if (preAuth3D.transaction.recurring_transaction != null && preAuth3D.transaction.recurring_transaction.type != null) {
                        if (String.Equals(preAuth3D.transaction.recurring_transaction.type, "single", StringComparison.OrdinalIgnoreCase)) {
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