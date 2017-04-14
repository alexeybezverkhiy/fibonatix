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
using System.ComponentModel.DataAnnotations;

namespace MerchantAPI.CommDoo.BackEnd.Requests
{
    public abstract class Request
    {
        static internal string timestampPattern = @"ddMMyyyyHHmmss";

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
            [XmlElement(ElementName = "ClientID")]
            public string ClientID { get; set; }
            [XmlIgnore]
            public string SharedSecret { get; set; }
        }
        public class SecurityData
        {
            [XmlElement(ElementName = "Timestamp")]
            public string Timestamp { get; set; }
            [XmlElement(ElementName = "Hash")]
            public string Hash { get; set; }
        }
        public class PaymentData
        {
            [XmlElement(ElementName = "TransactionID")]
            public string TransactionID { get; set; }
            [XmlElement(ElementName = "PaymentType")]
            public string PaymentType { get; set; }
            [XmlElement(ElementName = "PaymentMode")]
            public string PaymentMode { get; set; }
            [XmlElement(ElementName = "Amount")]
            public string Amount { get; set; }
            [XmlElement(ElementName = "Currency")]
            public string Currency { get; set; }
            [XmlElement(ElementName = "ReferenceID")]
            public string ReferenceID { get; set; }
            [XmlElement(ElementName = "AdditionalData")]
            public string AdditionalData { get; set; }
            [XmlElement(ElementName = "RelatedInformation")]
            public RelatedInformationData RelatedInformation { get; set; }
            [XmlElement(ElementName = "SubscriptionData")]
            public SubscriptionData Subscription { get; set; }
        }
        public class RelatedInformationData
        {
            [XmlElement(ElementName = "RiskCheckID")]
            public string RiskCheckID { get; set; }
            [XmlElement(ElementName = "OrderID")]
            public string OrderID { get; set; }
            [XmlElement(ElementName = "ReferencedTransactionID")]
            public string ReferencedTransactionID { get; set; }
            [XmlElement(ElementName = "RefundType")]
            public string RefundType { get; set; }
            [XmlElement(ElementName = "Purpose")]
            public string Purpose { get; set; }
            [XmlElement(ElementName = "Website")]
            public string Website { get; set; }
            [XmlElement(ElementName = "SEPADirectDebitMandateReferenceID")]
            public string SEPADirectDebitMandateReferenceID { get; set; }
            [XmlElement(ElementName = "SEPADirectDebitMandateDate")]
            public string SEPADirectDebitMandateDate { get; set; }
            [XmlElement(ElementName = "Username")]
            public string Username { get; set; }
            [XmlElement(ElementName = "Tariff")]
            public string Tariff { get; set; }
            [XmlElement(ElementName = "DateOfRegistration")]
            public string DateOfRegistration { get; set; }
            [XmlElement(ElementName = "AmazonOrderReferenceID")]
            public string AmazonOrderReferenceID { get; set; }
        }
        public class SubscriptionData
        {
            [XmlElement(ElementName = "Timeunit")]
            public string Timeunit { get; set; }
            [XmlElement(ElementName = "BillingCycle")]
            public string BillingCycle { get; set; }
            [XmlElement(ElementName = "DurationPeriod")]
            public string DurationPeriod { get; set; }
            [XmlElement(ElementName = "RenewalPeriod")]
            public string RenewalPeriod { get; set; }
            [XmlElement(ElementName = "MinimumDurationPeriod")]
            public string MinimumDurationPeriod { get; set; }
            [XmlElement(ElementName = "MaximumDurationPeriod")]
            public string MaximumDurationPeriod { get; set; }
            [XmlElement(ElementName = "CancellationPeriod")]
            public string CancellationPeriod { get; set; }
            [XmlElement(ElementName = "Trial")]
            public TrialData Trial { get; set; }
            public class TrialData
            {
                [XmlElement(ElementName = "DurationPeriod")]
                public string DurationPeriod { get; set; }
                [XmlElement(ElementName = "Timeunit")]
                public string Timeunit { get; set; }
                [XmlElement(ElementName = "Amount")]
                public string Amount { get; set; }
            }
        }
        public class RedirectionData
        {
            [XmlElement(ElementName = "SuccessURL")]
            public string SuccessURL { get; set; }
            [XmlElement(ElementName = "FailURL")]
            public string FailURL { get; set; }
        }
        public class NotificationData
        {
            [XmlElement(ElementName = "NotificationURL")]
            public string NotificationURL { get; set; }
        }
        public class CustomerData
        {
            [XmlElement(ElementName = "CustomerID")]
            public string CustomerID { get; set; }
            [XmlElement(ElementName = "ReferenceCustomerID")]
            public string ReferenceCustomerID { get; set; }
            [XmlElement(ElementName = "Person")]
            public PersonData Person { get; set; }
            [XmlElement(ElementName = "Address")]
            public AddressData Address { get; set; }
            [XmlElement(ElementName = "Bank")]
            public BankData Bank { get; set; }
            [XmlElement(ElementName = "CreditCard")]
            public CreditCardData CreditCard { get; set; }
        }
        public class PurchaseData
        {
            [XmlElement(ElementName = "Delivery")]
            public DeliveryData Delivery { get; set; }
            [XmlArray("Items")]
            [XmlArrayItem(typeof(ItemData), ElementName = "Item")]
            public List<ItemData> Items { get; set; }
        }
        public class DeliveryData
        {
            [XmlElement(ElementName = "Person")]
            public PersonData Person { get; set; }
            [XmlElement(ElementName = "Address")]
            public AddressData Address { get; set; }
        }
        public class PersonData
        {
            [XmlElement(ElementName = "CompanyName")]
            public string CompanyName { get; set; }
            [XmlElement(ElementName = "FirstName")]
            public string FirstName { get; set; }
            [XmlElement(ElementName = "LastName")]
            public string LastName { get; set; }
            [XmlElement(ElementName = "Salutation")]
            public string Salutation { get; set; }
            [XmlElement(ElementName = "Title")]
            public string Title { get; set; }
            [XmlElement(ElementName = "DateOfBirth")]
            public string DateOfBirth { get; set; }
            [XmlElement(ElementName = "Gender")]
            public string Gender { get; set; }
            [XmlElement(ElementName = "IDCardNumber")]
            public string IDCardNumber { get; set; }
        }
        public class AddressData
        {
            [XmlElement(ElementName = "Street")]
            public string Street { get; set; }
            [XmlElement(ElementName = "HouseNumber")]
            public string HouseNumber { get; set; }
            [XmlElement(ElementName = "PostalCode")]
            public string PostalCode { get; set; }
            [XmlElement(ElementName = "City")]
            public string City { get; set; }
            [XmlElement(ElementName = "State")]
            public string State { get; set; }
            [XmlElement(ElementName = "Country")]
            public string Country { get; set; }
            [XmlElement(ElementName = "PhoneNumber")]
            public string PhoneNumber { get; set; }
            [XmlElement(ElementName = "EmailAddress")]
            public string EmailAddress { get; set; }
            [XmlElement(ElementName = "IPAddress")]
            public string IPAddress { get; set; }
        }
        public class BankData
        {
            [XmlElement(ElementName = "BankAccountNumber")]
            public string BankAccountNumber { get; set; }
            [XmlElement(ElementName = "BankCode")]
            public string BankCode { get; set; }
            [XmlElement(ElementName = "BankCountry")]
            public string BankCountry { get; set; }
            [XmlElement(ElementName = "BankAccountHolder")]
            public string BankAccountHolder { get; set; }
            [XmlElement(ElementName = "IBAN")]
            public string IBAN { get; set; }
            [XmlElement(ElementName = "BIC")]
            public string BIC { get; set; }
        }
        public class CreditCardData
        {
            [XmlElement(ElementName = "CreditCardNumber")]
            public string CreditCardNumber { get; set; }
            [XmlElement(ElementName = "CreditCardExpirationMonth")]
            public string CreditCardExpirationMonth { get; set; }
            [XmlElement(ElementName = "CreditCardExpirationYear")]
            public string CreditCardExpirationYear { get; set; }
            [XmlElement(ElementName = "CreditCardType")]
            public string CreditCardType { get; set; }
            [XmlElement(ElementName = "CreditCardValidationValue")]
            public string CreditCardValidationValue { get; set; }
        }
        public class ItemData
        {
            [XmlElement(ElementName = "ID")]
            public string ID { get; set; }
            [XmlElement(ElementName = "Name")]
            public string Name { get; set; }
            [XmlElement(ElementName = "Description")]
            public string Description { get; set; }
            [XmlElement(ElementName = "Quantity")]
            public string Quantity { get; set; }
            [XmlElement(ElementName = "TotalPrice")]
            public string TotalPrice { get; set; }
            [XmlElement(ElementName = "Currency")]
            public string Currency { get; set; }
            [XmlElement(ElementName = "TaxPercentage")]
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
            xmlReq = xmlReq.Replace("<PaymentType>CreditCard</PaymentType>", "<PaymentType><CreditCard /></PaymentType>");
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