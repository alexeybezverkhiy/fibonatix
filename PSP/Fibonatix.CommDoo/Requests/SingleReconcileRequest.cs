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
    public class SingleReconcileRequest : Request
    {
        public override AcquirerType getAcquirer() {
            return reconcile.configurations.getAcquirer();
        }
        public override string getConfigValue(string key) {
            return reconcile.configurations.GetConfigurationValueNoCase(key);
        }

        [Required]
        [XmlElement(ElementName = "Reconcile")]
        public SingleReconcile reconcile { get; set; }
        public class SingleReconcile
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
                [Required]
                [XmlElement(ElementName = "ProviderTransactionID")]
                public string provider_transaction_id { get; set; }

                // Three fields only for Borgun - RRN, DateAndTime and TerminalID
                [XmlElement(ElementName = "RRN")]
                public string rrn { get; set; }
                [XmlElement(ElementName = "DateAndTime")]
                public string datetime { get; set; }
                [XmlElement(ElementName = "TerminalID")]
                public string terminal { get; set; }

                [XmlElement(ElementName = "CreditCardAlias")]
                public string credit_card_alias { get; set; }
            }
        }

        public override void verification() { // exception
            if (reconcile == null) {
                string ExceptionMessage = "Incorrect XML for Reconcile request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InvalidTransactionTypeError);
            } else if (reconcile.transaction == null) {
                string ExceptionMessage = "'Transaction' section is not exist in Reconcile request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            } else if (reconcile.transaction == null) {
                string ExceptionMessage = "'Transaction' section is not exist in Reconcile request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            } else if (reconcile.transaction.provider_transaction_id == null || reconcile.transaction.reference_id == null) {
                string ExceptionMessage = "Transaction IDs are not set properly in Reconcile request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            }
        }

        public static SingleReconcileRequest DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(SingleReconcileRequest));

            var reader = new XmlNodeReader(doc.DocumentElement);
            SingleReconcileRequest entry = (SingleReconcileRequest)seri.Deserialize(reader);
            return entry;
        }

        public static SingleReconcileRequest DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static SingleReconcileRequest DeserializeFromStringSafe(string xmlData) {
            SingleReconcileRequest ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
    }
}
