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
using System.Net;

namespace MerchantAPI.CommDoo.BackEnd.Requests
{
    public abstract class Request
    {
        static internal string timestampPattern = @"ddMMyyyyHHmmss";

        static internal string sharedSecret = "test";
        static internal string serviceURL = "https://service.commpay.net/server2server/payment/transaction.asmx";

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


        public abstract string executeRequest();
        public abstract string calculateHash();

        public class ClientData
        {
            [XmlElement("ClientID")]
            public string ClientID { get; set; }
        }
        public class SecurityData
        {
            [XmlElement("Timestamp")]
            public string Timestamp { get; set; }
            [XmlElement("Hash")]
            public string Hash { get; set; }
        }
        public class PaymentData
        {
            [XmlElement("TransactionID")]
            public string TransactionID { get; set; }
            [XmlElement("PaymentType")]
            public string PaymentType { get; set; }
            [XmlElement("PaymentMode")]
            public string PaymentMode { get; set; }
            [XmlElement("Amount")]
            public string Amount { get; set; }
            [XmlElement("Currency")]
            public string Currency { get; set; }
            [XmlElement("ReferenceID")]
            public string ReferenceID { get; set; }
            [XmlElement("AdditionalData")]
            public string AdditionalData { get; set; }
            [XmlElement("RelatedInformation")]
            public RelatedInformationData RelatedInformation { get; set; }
            [XmlElement("SubscriptionData")]
            public SubscriptionData Subscription { get; set; }
        }
        public class RelatedInformationData
        {
            [XmlElement("RiskCheckID")]
            public string RiskCheckID { get; set; }
            [XmlElement("OrderID")]
            public string OrderID { get; set; }
            [XmlElement("ReferencedTransactionID")]
            public string ReferencedTransactionID { get; set; }
            [XmlElement("RefundType")]
            public string RefundType { get; set; }
            [XmlElement("Purpose")]
            public string Purpose { get; set; }
            [XmlElement("Website")]
            public string Website { get; set; }
            [XmlElement("SEPADirectDebitMandateReferenceID")]
            public string SEPADirectDebitMandateReferenceID { get; set; }
            [XmlElement("SEPADirectDebitMandateDate")]
            public string SEPADirectDebitMandateDate { get; set; }
            [XmlElement("Username")]
            public string Username { get; set; }
            [XmlElement("Tariff")]
            public string Tariff { get; set; }
            [XmlElement("DateOfRegistration")]
            public string DateOfRegistration { get; set; }
            [XmlElement("AmazonOrderReferenceID")]
            public string AmazonOrderReferenceID { get; set; }
        }
        public class SubscriptionData
        {
            [XmlElement("Timeunit")]
            public string Timeunit { get; set; }
            [XmlElement("BillingCycle")]
            public string BillingCycle { get; set; }
            [XmlElement("DurationPeriod")]
            public string DurationPeriod { get; set; }
            [XmlElement("RenewalPeriod")]
            public string RenewalPeriod { get; set; }
            [XmlElement("MinimumDurationPeriod")]
            public string MinimumDurationPeriod { get; set; }
            [XmlElement("MaximumDurationPeriod")]
            public string MaximumDurationPeriod { get; set; }
            [XmlElement("CancellationPeriod")]
            public string CancellationPeriod { get; set; }
            [XmlElement("Trial")]
            public TrialData Trial { get; set; }
            public class TrialData
            {
                [XmlElement("DurationPeriod")]
                public string DurationPeriod { get; set; }
                [XmlElement("Timeunit")]
                public string Timeunit { get; set; }
                [XmlElement("Amount")]
                public string Amount { get; set; }
            }
        }
        public class RedirectionData
        {
            [XmlElement("SuccessURL")]
            public string SuccessURL { get; set; }
            [XmlElement("FailURL")]
            public string FailURL { get; set; }
        }
        public class NotificationData
        {
            [XmlElement("NotificationURL")]
            public string NotificationURL { get; set; }
        }
        public class CustomerData
        {
            [XmlElement("CustomerID")]
            public string CustomerID { get; set; }
            [XmlElement("ReferenceCustomerID")]
            public string ReferenceCustomerID { get; set; }
            [XmlElement("Person")]
            public PersonData Person { get; set; }
            [XmlElement("Address")]
            public AddressData Address { get; set; }
            [XmlElement("Person")]
            public BankData Bank { get; set; }
            [XmlElement("Person")]
            public CreditCardData CreditCard { get; set; }
        }

        public class PurchaseData
        {
            [XmlElement("Delivery")]
            public DeliveryData Delivery { get; set; }
            [XmlArray("Items")]
            [XmlArrayItem(typeof(ItemData), ElementName = "Item")]
            public List<ItemData> Items { get; set; }
        }

        public class DeliveryData
        {
            [XmlElement("Person")]
            public PersonData Person { get; set; }
            [XmlElement("Address")]
            public AddressData Address { get; set; }
        }

        public class PersonData
        {
            [XmlElement("CompanyName")]
            public string CompanyName { get; set; }
            [XmlElement("FirstName")]
            public string FirstName { get; set; }
            [XmlElement("LastName")]
            public string LastName { get; set; }
            [XmlElement("Salutation")]
            public string Salutation { get; set; }
            [XmlElement("Title")]
            public string Title { get; set; }
            [XmlElement("DateOfBirth")]
            public string DateOfBirth { get; set; }
            [XmlElement("Gender")]
            public string Gender { get; set; }
            [XmlElement("IDCardNumber")]
            public string IDCardNumber { get; set; }
        }
        public class AddressData
        {
            [XmlElement("Street")]
            public string Street { get; set; }
            [XmlElement("HouseNumber")]
            public string HouseNumber { get; set; }
            [XmlElement("PostalCode")]
            public string PostalCode { get; set; }
            [XmlElement("City")]
            public string City { get; set; }
            [XmlElement("State")]
            public string State { get; set; }
            [XmlElement("Country")]
            public string Country { get; set; }
            [XmlElement("PhoneNumber")]
            public string PhoneNumber { get; set; }
            [XmlElement("EmailAddress")]
            public string EmailAddress { get; set; }
            [XmlElement("IPAddress")]
            public string IPAddress { get; set; }
        }
        public class BankData
        {
            [XmlElement("BankAccountNumber")]
            public string BankAccountNumber { get; set; }
            [XmlElement("BankCode")]
            public string BankCode { get; set; }
            [XmlElement("BankCountry")]
            public string BankCountry { get; set; }
            [XmlElement("BankAccountHolder")]
            public string BankAccountHolder { get; set; }
            [XmlElement("IBAN")]
            public string IBAN { get; set; }
            [XmlElement("BIC")]
            public string BIC { get; set; }
        }
        public class CreditCardData
        {
            [XmlElement("CreditCardNumber")]
            public string CreditCardNumber { get; set; }
            [XmlElement("CreditCardExpirationMonth")]
            public string CreditCardExpirationMonth { get; set; }
            [XmlElement("CreditCardExpirationYear")]
            public string CreditCardExpirationYear { get; set; }
            [XmlElement("CreditCardType")]
            public string CreditCardType { get; set; }
            [XmlElement("CreditCardValidationValue")]
            public string CreditCardValidationValue { get; set; }
        }

        public class ItemData
        {
            [XmlElement("ID")]
            public string ID { get; set; }
            [XmlElement("Name")]
            public string Name { get; set; }
            [XmlElement("Description")]
            public string Description { get; set; }
            [XmlElement("Quantity")]
            public string Quantity { get; set; }
            [XmlElement("TotalPrice")]
            public string TotalPrice { get; set; }
            [XmlElement("Currency")]
            public string Currency { get; set; }
            [XmlElement("TaxPercentage")]
            public string TaxPercentage { get; set; }

        }

        public string getXml() {
            XmlSerializer formatter = new XmlSerializer(this.GetType());
            StringWriter writer = new Utf8StringWriter();
            formatter.Serialize(writer, this);
            var serializedValue = writer.ToString();
            return serializedValue;
        }
        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        internal string sendRequest(string url) {
            calculateHash();
            string xmlReq = getXml();
            var ret = ProcessRequest(url, System.Text.Encoding.UTF8.GetBytes("xml=" + HttpUtility.UrlEncode(xmlReq)));
            return System.Text.Encoding.UTF8.GetString(ret.ToArray());
        }

        internal MemoryStream ProcessRequest(string url, byte[] request) {
            var webRequest = CreateWebRequest(url, request);
            return GetResponseStream(webRequest);
        }

        private WebRequest CreateWebRequest(string url, byte[] request) {
            var webRequest = WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            byte[] data = request;
            webRequest.ContentLength = data.Length;

            var httpWebRequest = webRequest as HttpWebRequest;
            if (httpWebRequest != null) {
                httpWebRequest.UserAgent = String.Format("Fibonatix.CommDoo.WebGate {0}", this.GetType().Assembly.GetName().Version.ToString());
                httpWebRequest.KeepAlive = false;
            }

            using (var requestStream = webRequest.GetRequestStream()) {
                requestStream.Write(data, 0, data.Length);
            }

            return webRequest;
        }

        private MemoryStream GetResponseStream(WebRequest webRequest) {
            WebResponse webResponse = null;
            try {
                webResponse = webRequest.GetResponse();
                return Copy(webResponse.GetResponseStream());
            } catch (WebException ex) {
                Stream responseStream;
                if (TryGetResponseDataFromWebException(ex, out responseStream)) {
                    return Copy(responseStream);
                }
                throw ex;
            } finally {
                if (webResponse != null) {
                    webResponse.Close();
                }
            }
        }
        private MemoryStream Copy(Stream stream) {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream;
        }
        private bool TryGetResponseDataFromWebException(WebException webException, out Stream responseData) {
            responseData = null;

            var response = webException.Response as HttpWebResponse;
            if (response == null) {
                return false;
            }

            // Unprocessable Entity (The request was well-formed but was unable to be followed due to semantic errors.)
            if (response.StatusCode == (HttpStatusCode)422) {
                responseData = response.GetResponseStream();
                response.Close();
                return true;
            }
            return false;
        }
    }
}