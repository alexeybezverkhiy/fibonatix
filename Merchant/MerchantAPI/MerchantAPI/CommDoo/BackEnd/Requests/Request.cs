using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace MerchantAPI.CommDoo.BackEnd.Requests
{
    public abstract class Request
    {
        static internal string timestampPattern = @"ddMMyyyyHHmmss";

        static internal string sharedSecret = "test";

        public static string GetWesternEuropeDateTime() {
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time" /* "Central European Standard Time" */);
            DateTime westernEurope = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), cstZone);
            return westernEurope.ToString(timestampPattern);
        }
        public static string sha1(string value) {
            System.Security.Cryptography.SHA1Managed crypt = new System.Security.Cryptography.SHA1Managed();
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(value), 0, Encoding.UTF8.GetByteCount(value));
            foreach (byte theByte in crypto) {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }


        public abstract string calculateHash();

        public class ClientData
        {
            [XmlAttribute("ClientID")]
            public string ClientID { get; set; }
        }
        public class SecurityData
        {
            [XmlAttribute("Timestamp")]
            public string Timestamp { get; set; }
            [XmlAttribute("Hash")]
            public string Hash { get; set; }
        }
        public class PaymentData
        {
            [XmlAttribute("TransactionID")]
            public string TransactionID { get; set; }
            [XmlAttribute("PaymentType")]
            public string PaymentType { get; set; }
            [XmlAttribute("PaymentMode")]
            public string PaymentMode { get; set; }
            [XmlAttribute("Amount")]
            public string Amount { get; set; }
            [XmlAttribute("Currency")]
            public string Currency { get; set; }
            [XmlAttribute("ReferenceID")]
            public string ReferenceID { get; set; }
            [XmlAttribute("AdditionalData")]
            public string AdditionalData { get; set; }
            [XmlAttribute("RelatedInformation")]
            public RelatedInformationData RelatedInformation { get; set; }
            [XmlAttribute("SubscriptionData")]
            public SubscriptionData Subscription { get; set; }
        }
        public class RelatedInformationData
        {
            [XmlAttribute("RiskCheckID")]
            public string RiskCheckID { get; set; }
            [XmlAttribute("OrderID")]
            public string OrderID { get; set; }
            [XmlAttribute("ReferencedTransactionID")]
            public string ReferencedTransactionID { get; set; }
            [XmlAttribute("RefundType")]
            public string RefundType { get; set; }
            [XmlAttribute("Purpose")]
            public string Purpose { get; set; }
            [XmlAttribute("Website")]
            public string Website { get; set; }
            [XmlAttribute("SEPADirectDebitMandateReferenceID")]
            public string SEPADirectDebitMandateReferenceID { get; set; }
            [XmlAttribute("SEPADirectDebitMandateDate")]
            public string SEPADirectDebitMandateDate { get; set; }
            [XmlAttribute("Username")]
            public string Username { get; set; }
            [XmlAttribute("Tariff")]
            public string Tariff { get; set; }
            [XmlAttribute("DateOfRegistration")]
            public string DateOfRegistration { get; set; }
            [XmlAttribute("AmazonOrderReferenceID")]
            public string AmazonOrderReferenceID { get; set; }
        }
        public class SubscriptionData
        {
            [XmlAttribute("Timeunit")]
            public string Timeunit { get; set; }
            [XmlAttribute("BillingCycle")]
            public string BillingCycle { get; set; }
            [XmlAttribute("DurationPeriod")]
            public string DurationPeriod { get; set; }
            [XmlAttribute("RenewalPeriod")]
            public string RenewalPeriod { get; set; }
            [XmlAttribute("MinimumDurationPeriod")]
            public string MinimumDurationPeriod { get; set; }
            [XmlAttribute("MaximumDurationPeriod")]
            public string MaximumDurationPeriod { get; set; }
            [XmlAttribute("CancellationPeriod")]
            public string CancellationPeriod { get; set; }
            [XmlAttribute("Trial")]
            public TrialData Trial { get; set; }
            public class TrialData
            {
                [XmlAttribute("DurationPeriod")]
                public string DurationPeriod { get; set; }
                [XmlAttribute("Timeunit")]
                public string Timeunit { get; set; }
                [XmlAttribute("Amount")]
                public string Amount { get; set; }
            }
        }
        public class RedirectionData
        {
            [XmlAttribute("SuccessURL")]
            public string SuccessURL { get; set; }
            [XmlAttribute("FailURL")]
            public string FailURL { get; set; }
        }
        public class NotificationData
        {
            [XmlAttribute("NotificationURL")]
            public string NotificationURL { get; set; }
        }
        public class CustomerData
        {
            [XmlAttribute("CustomerID")]
            public string CustomerID { get; set; }
            [XmlAttribute("ReferenceCustomerID")]
            public string ReferenceCustomerID { get; set; }

            [XmlAttribute("Person")]
            public PersonData Person { get; set; }
            [XmlAttribute("Address")]
            public AddressData Address { get; set; }
            [XmlAttribute("Person")]
            public BankData Bank { get; set; }
            [XmlAttribute("Person")]
            public CreditCardData CreditCard { get; set; }
        }

        public class PurchaseData
        {
            [XmlAttribute("Delivery")]
            public DeliveryData Delivery { get; set; }

            [XmlArray("Items")]
            [XmlArrayItem(typeof(ItemData), ElementName = "Item")]
            public List<ItemData> Items { get; set; }
        }

        public class DeliveryData
        {
            [XmlAttribute("Person")]
            public PersonData Person { get; set; }
            [XmlAttribute("Address")]
            public AddressData Address { get; set; }

        }

        public class PersonData
        {
            [XmlAttribute("CompanyName")]
            public string CompanyName { get; set; }
            [XmlAttribute("FirstName")]
            public string FirstName { get; set; }
            [XmlAttribute("LastName")]
            public string LastName { get; set; }
            [XmlAttribute("Salutation")]
            public string Salutation { get; set; }
            [XmlAttribute("Title")]
            public string Title { get; set; }
            [XmlAttribute("DateOfBirth")]
            public string DateOfBirth { get; set; }
            [XmlAttribute("Gender")]
            public string Gender { get; set; }
            [XmlAttribute("IDCardNumber")]
            public string IDCardNumber { get; set; }
        }
        public class AddressData
        {
            [XmlAttribute("Street")]
            public string Street { get; set; }
            [XmlAttribute("HouseNumber")]
            public string HouseNumber { get; set; }
            [XmlAttribute("PostalCode")]
            public string PostalCode { get; set; }
            [XmlAttribute("City")]
            public string City { get; set; }
            [XmlAttribute("State")]
            public string State { get; set; }
            [XmlAttribute("Country")]
            public string Country { get; set; }
            [XmlAttribute("PhoneNumber")]
            public string PhoneNumber { get; set; }
            [XmlAttribute("EmailAddress")]
            public string EmailAddress { get; set; }
            [XmlAttribute("IPAddress")]
            public string IPAddress { get; set; }
        }
        public class BankData
        {
            [XmlAttribute("BankAccountNumber")]
            public string BankAccountNumber { get; set; }
            [XmlAttribute("BankCode")]
            public string BankCode { get; set; }
            [XmlAttribute("BankCountry")]
            public string BankCountry { get; set; }
            [XmlAttribute("BankAccountHolder")]
            public string BankAccountHolder { get; set; }
            [XmlAttribute("IBAN")]
            public string IBAN { get; set; }
            [XmlAttribute("BIC")]
            public string BIC { get; set; }
        }
        public class CreditCardData
        {
            [XmlAttribute("CreditCardNumber")]
            public string CreditCardNumber { get; set; }
            [XmlAttribute("CreditCardExpirationMonth")]
            public string CreditCardExpirationMonth { get; set; }
            [XmlAttribute("CreditCardExpirationYear")]
            public string CreditCardExpirationYear { get; set; }
            [XmlAttribute("CreditCardType")]
            public string CreditCardType { get; set; }
            [XmlAttribute("CreditCardValidationValue")]
            public string CreditCardValidationValue { get; set; }
        }

        public class ItemData
        {
            [XmlAttribute("ID")]
            public string ID { get; set; }
            [XmlAttribute("Name")]
            public string Name { get; set; }
            [XmlAttribute("Description")]
            public string Description { get; set; }
            [XmlAttribute("Quantity")]
            public string Quantity { get; set; }
            [XmlAttribute("TotalPrice")]
            public string TotalPrice { get; set; }
            [XmlAttribute("Currency")]
            public string Currency { get; set; }
            [XmlAttribute("TaxPercentage")]
            public string TaxPercentage { get; set; }

        }
    }
}