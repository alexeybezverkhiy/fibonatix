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
    public class CaptureRequest : Request
    {
        public override AcquirerType getAcquirer() {
            return capture.configurations.getAcquirer();
        }
        public override string getConfigValue(string key) {
            return capture.configurations.GetConfigurationValueNoCase(key);
        }

        [Required]
        [XmlElement(ElementName = "Capture")]
        public Capture capture { get; set; }
        public class Capture
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
                [XmlElement(ElementName = "AuthCode")]
                public string auth_code { get; set; }
                [XmlElement(ElementName = "Amount")]
                public decimal amount { get; set; }
                [XmlElement(ElementName = "Currency")]
                public string currency { get; set; }
                [XmlElement(ElementName = "Usage")]
                public string usage { get; set; }
                [XmlElement(ElementName = "CaptureType")]
                public string capture_type { get; set; }
                [XmlElement(ElementName = "RecurringTransaction")]
                public RecurringTransaction recurring_transaction { get; set; }
                [XmlElement(ElementName = "CreditCardData")]
                public CreditCardData cred_card_data { get; set; }
                [XmlElement(ElementName = "ThreeDSecure")]
                public Secure3D threed_secure { get; set; }
            }
        }

        public override void verification() { // exception
            if (capture == null) {
                string ExceptionMessage = "Incorrect XML for Capture request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InvalidTransactionTypeError);
            } else if (capture.transaction == null) {
                string ExceptionMessage = "'Transaction' section is not exist in Capture request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataMissingError);
            } else if (getRequestType() == RequestType.NotSupported) {
                string ExceptionMessage = "'Recurrence type != SINGLE' not supported in Capture request";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataInvalidError);
            } else if (String.Equals(capture.transaction.capture_type, "PARTIAL" ) && getAcquirer() == AcquirerType.Kalixa) {
                string ExceptionMessage = "'Capture Type == PARTIAL' not supported in Capture request for selected Acquirer";
                throw new System.ComponentModel.DataAnnotations.ValidationException(ExceptionMessage).SetCode((int)ErrorCodes.InputDataInvalidError);
            }
        }
 

        public static CaptureRequest DeserializeFromXmlDocument(XmlDocument doc) {
            XmlSerializer seri = new XmlSerializer(typeof(CaptureRequest));

            var reader = new XmlNodeReader(doc.DocumentElement);
            CaptureRequest entry = (CaptureRequest)seri.Deserialize(reader);
            return entry;
        }

        public static CaptureRequest DeserializeFromString(string xmlData) {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            return DeserializeFromXmlDocument(xml);
        }
        public static CaptureRequest DeserializeFromStringSafe(string xmlData) {
            CaptureRequest ret = null;
            try {
                ret = DeserializeFromString(xmlData);
            } catch (Exception) {
            }
            return ret;
        }
        public RequestType getRequestType() {
            RequestType ret = RequestType.NotSupported;
            try {
                if (capture.transaction != null) {
                    if (capture.transaction.recurring_transaction == null) {
                        ret = RequestType.Single;
                    } else if (capture.transaction.recurring_transaction != null && capture.transaction.recurring_transaction.type != null) {
                        if (String.Equals(capture.transaction.recurring_transaction.type, "SINGLE", StringComparison.InvariantCultureIgnoreCase)) {
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
