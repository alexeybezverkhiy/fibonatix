using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Fibonatix.CommDoo.Requests
{
    public enum RequestType
    {
        Single = 0,
        Initial = 1,
        Repeated = 2,
        NotSupported = -1
    }
    public enum AcquirerType
    {
        Unknown         = 0,
        Genesis         = 1,
        Kalixa          = 2,
        ProcessingCom   = 3,
    }

    [Serializable()]
    public abstract class Request
    {
        public abstract AcquirerType getAcquirer();
        public abstract string getConfigValue(string key);
        public abstract void verification(); // exception

        public class Configuration
        {
            [XmlElement(ElementName = "Name")]
            public string name { get; set; }
            [XmlElement(ElementName = "Value")]
            public string value { get; set; }
            Configuration() { name = ""; value = ""; }
        }
        public class Configurations : List<Configuration>
        {
            public string GetConfigurationValue(string key, StringComparison comp = StringComparison.InvariantCulture) {
                var f = Find(x => String.Equals(x.name, key, comp));
                if (f != null)
                    return f.value;
                else
                    return null;
            }
            public string GetConfigurationValueNoCase(string key) {
                var f = Find(x => String.Equals(x.name, key, StringComparison.OrdinalIgnoreCase));
                if (f != null)
                    return f.value;
                else
                    return null;
            }
            public AcquirerType getAcquirer() {
                var f = Find(x => String.Equals(x.name, "acquirer", StringComparison.OrdinalIgnoreCase));
                if (f != null) {
                    if (String.Equals(f.value, "Genesis", StringComparison.OrdinalIgnoreCase))
                        return AcquirerType.Genesis;
                    else if (String.Equals(f.value, "Kalixa", StringComparison.OrdinalIgnoreCase))
                        return AcquirerType.Kalixa;
                    else if (String.Equals(f.value, "PCom", StringComparison.OrdinalIgnoreCase) ||
                        String.Equals(f.value, "Processing.Com", StringComparison.OrdinalIgnoreCase) ||
                        String.Equals(f.value, "ProcessingCom", StringComparison.OrdinalIgnoreCase))
                        return AcquirerType.ProcessingCom;
                    else
                        return AcquirerType.Unknown;
                } else
                    return AcquirerType.Unknown;
            }
        }

        public class RecurringTransaction
        {
            [XmlElement(ElementName = "Type")]
            public string type { get; set; }
        }

        public class CreditCardData
        {
            public enum CreditCardType
            {
                Unknown = 0,
                Visa = 1,
                MasterCard = 2,
                Maestro = 3,
                Diners = 4,
                AmericanExpress = 5,
            }
            public CreditCardType getCreditCardType() {
                if (credit_card_type == null)
                    return CreditCardType.Unknown;
                else if (String.Equals(credit_card_type, "Visa", StringComparison.InvariantCultureIgnoreCase))
                    return CreditCardType.Visa;
                else if (String.Equals(credit_card_type, "Mastercard", StringComparison.InvariantCultureIgnoreCase))
                    return CreditCardType.MasterCard;
                else if (String.Equals(credit_card_type, "Maestro", StringComparison.InvariantCultureIgnoreCase))
                    return CreditCardType.Maestro;
                else if (String.Equals(credit_card_type, "Diners", StringComparison.InvariantCultureIgnoreCase))
                    return CreditCardType.Diners;
                else if (String.Equals(credit_card_type, "AmericanExpress", StringComparison.InvariantCultureIgnoreCase))
                    return CreditCardType.AmericanExpress;
                else
                    return CreditCardType.Unknown;
            }

            [XmlElement(ElementName = "CreditCardNumber")]
            public string credit_card_number { get; set; }
            [XmlElement(ElementName = "CVV")]
            public string cvv { get; set; }
            [XmlElement(ElementName = "ExpirationYear")]
            public int expuration_year { get; set; }
            [XmlElement(ElementName = "ExpirationMonth")]
            public int expiration_month { get; set; }
            [XmlElement(ElementName = "CardHolderName")]
            public string cardholder_name { get; set; }
            [XmlElement(ElementName = "CreditCardType")]
            public string credit_card_type { get; set; }
        }

        public class Secure3D
        {
            [XmlElement(ElementName = "PARes")]
            public string pa_res { get; set; }
            [XmlElement(ElementName = "MD")]
            public string md { get; set; }
            [XmlElement(ElementName = "TermUrl")]
            public string term_url { get; set; }
            [XmlElement(ElementName = "AcsUrl")]
            public string acs_url { get; set; }
            [XmlElement(ElementName = "EnrollmentRes")]
            public string enrolment_res { get; set; }
        }

        public class Communication3D
        {
            [XmlElement(ElementName = "NotificationURL")]
            public string notification_url { get; set; }
            [XmlElement(ElementName = "SuccessURL")]
            public string success_url { get; set; }
            [XmlElement(ElementName = "FailURL")]
            public string fail_url { get; set; }
        }

        public class CustomerData
        {
            [XmlElement(ElementName = "Firstname")]
            public string firstname { get; set; }
            [XmlElement(ElementName = "Lastname")]
            public string lastname { get; set; }
            [XmlElement(ElementName = "Street")]
            public string street { get; set; }
            [XmlElement(ElementName = "PostalCode")]
            public string postalcode { get; set; }
            [XmlElement(ElementName = "City")]
            public string city { get; set; }
            [XmlElement(ElementName = "Country")]
            public string country { get; set; }
            [XmlElement(ElementName = "State")]
            public string state { get; set; }
            [XmlElement(ElementName = "Email")]
            public string email { get; set; }
            [XmlElement(ElementName = "Phone")]
            public string phone { get; set; }
            [XmlElement(ElementName = "IPAddress")]
            public string ipaddress { get; set; }
        }


    }
}
