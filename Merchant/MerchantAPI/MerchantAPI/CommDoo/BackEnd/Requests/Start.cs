using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;

namespace MerchantAPI.CommDoo.BackEnd.Requests
{
    [Serializable()]
    [XmlRoot("Request")]
    public class StartRequest : Request
    {
        [XmlElement(ElementName = "Client")]
        public ClientData Client { get; set; }
        [XmlElement(ElementName = "Security")]
        public SecurityData Security { get; set; }
        [XmlElement(ElementName = "Payment")]
        public PaymentData Payment { get; set; }
        [XmlElement(ElementName = "Redirection")]
        public RedirectionData Redirection { get; set; }
        [XmlElement(ElementName = "Notification")]
        public NotificationData Notification { get; set; }
        [XmlElement(ElementName = "Customer")]
        public CustomerData Customer { get; set; }
        [XmlElement(ElementName = "Purchase")]
        public PurchaseData Purchase { get; set; }

        public override string executeRequest() {
            string requestURL = WebApiConfig.Settings.BackendServiceUrl + "/Start";
            return sendRequest(requestURL);
        }

        public override string calculateHash() {

            string strToHashCal = "";
            string strHash = null;

            try {
                if (Security == null) Security = new SecurityData();

                Security.Timestamp = GetWesternEuropeDateTime();

                strToHashCal += Client.ClientID;
                strToHashCal += Security.Timestamp;
                strToHashCal += Payment.PaymentType;
                strToHashCal += Payment.PaymentMode;
                strToHashCal += Payment.Amount;
                strToHashCal += Payment.Currency;
                strToHashCal += Payment.ReferenceID;
                strToHashCal += Payment.AdditionalData;
                if (Payment.RelatedInformation != null) {
                    strToHashCal += Payment.RelatedInformation.RiskCheckID;
                    strToHashCal += Payment.RelatedInformation.OrderID;
                    strToHashCal += Payment.RelatedInformation.Purpose;
                    strToHashCal += Payment.RelatedInformation.Website;
                    strToHashCal += Payment.RelatedInformation.SEPADirectDebitMandateReferenceID;
                    strToHashCal += Payment.RelatedInformation.SEPADirectDebitMandateDate;
                    strToHashCal += Payment.RelatedInformation.Username;
                    strToHashCal += Payment.RelatedInformation.Tariff;
                    strToHashCal += Payment.RelatedInformation.DateOfRegistration;
                }
                if (Payment.Subscription != null) {
                    strToHashCal += Payment.Subscription.Timeunit;
                    strToHashCal += Payment.Subscription.BillingCycle;
                    strToHashCal += Payment.Subscription.DurationPeriod;
                    strToHashCal += Payment.Subscription.RenewalPeriod;
                    strToHashCal += Payment.Subscription.MinimumDurationPeriod;
                    strToHashCal += Payment.Subscription.MaximumDurationPeriod;
                    strToHashCal += Payment.Subscription.CancellationPeriod;
                    if (Payment.Subscription.Trial != null) {
                        strToHashCal += Payment.Subscription.Trial.DurationPeriod;
                        strToHashCal += Payment.Subscription.Trial.Timeunit;
                        strToHashCal += Payment.Subscription.Trial.Amount;
                    }
                }
                if (Redirection != null) {
                    strToHashCal += Redirection.SuccessURL;
                    strToHashCal += Redirection.FailURL;
                }
                if (Notification != null)
                    strToHashCal += Notification.NotificationURL;
                if (Customer != null) {
                    strToHashCal += Customer.ReferenceCustomerID;
                    if (Customer.Person != null) {
                        strToHashCal += Customer.Person.CompanyName;
                        strToHashCal += Customer.Person.FirstName;
                        strToHashCal += Customer.Person.LastName;
                        strToHashCal += Customer.Person.Salutation;
                        strToHashCal += Customer.Person.Title;
                        strToHashCal += Customer.Person.DateOfBirth;
                        strToHashCal += Customer.Person.Gender;
                        strToHashCal += Customer.Person.IDCardNumber;
                    }
                    if (Customer.Address != null) {
                        strToHashCal += Customer.Address.Street;
                        strToHashCal += Customer.Address.HouseNumber;
                        strToHashCal += Customer.Address.PostalCode;
                        strToHashCal += Customer.Address.City;
                        strToHashCal += Customer.Address.State;
                        strToHashCal += Customer.Address.Country;
                        strToHashCal += Customer.Address.PhoneNumber;
                        strToHashCal += Customer.Address.EmailAddress;
                        strToHashCal += Customer.Address.IPAddress;
                    }
                    if (Customer.Bank != null) {
                        strToHashCal += Customer.Bank.BankAccountNumber;
                        strToHashCal += Customer.Bank.BankCode;
                        strToHashCal += Customer.Bank.BankCountry;
                        strToHashCal += Customer.Bank.BankAccountHolder;
                        strToHashCal += Customer.Bank.IBAN;
                        strToHashCal += Customer.Bank.BIC;
                    }
                    if (Customer.CreditCard != null) {
                        strToHashCal += Customer.CreditCard.CreditCardNumber;
                        strToHashCal += Customer.CreditCard.CreditCardExpirationMonth;
                        strToHashCal += Customer.CreditCard.CreditCardExpirationYear;
                        strToHashCal += Customer.CreditCard.CreditCardType;
                        strToHashCal += Customer.CreditCard.CreditCardValidationValue;
                    }
                }
                if (Purchase != null) {
                    if (Purchase.Delivery != null) {
                        if (Purchase.Delivery.Person != null) {
                            strToHashCal += Purchase.Delivery.Person.CompanyName;
                            strToHashCal += Purchase.Delivery.Person.FirstName;
                            strToHashCal += Purchase.Delivery.Person.LastName;
                            strToHashCal += Purchase.Delivery.Person.Salutation;
                            strToHashCal += Purchase.Delivery.Person.Title;
                        }
                        if (Purchase.Delivery.Address != null) {
                            strToHashCal += Purchase.Delivery.Address.Street;
                            strToHashCal += Purchase.Delivery.Address.HouseNumber;
                            strToHashCal += Purchase.Delivery.Address.PostalCode;
                            strToHashCal += Purchase.Delivery.Address.City;
                            strToHashCal += Purchase.Delivery.Address.State;
                            strToHashCal += Purchase.Delivery.Address.Country;
                            strToHashCal += Purchase.Delivery.Address.PhoneNumber;
                            strToHashCal += Purchase.Delivery.Address.EmailAddress;
                        }
                    }
                    if (Purchase.Items != null) {
                        foreach (ItemData item in Purchase.Items) {
                            strToHashCal += item.ID;
                            strToHashCal += item.Name;
                            strToHashCal += item.Description;
                            strToHashCal += item.Quantity;
                            strToHashCal += item.TotalPrice;
                            strToHashCal += item.Currency;
                            strToHashCal += item.TaxPercentage;
                        }
                    }
                }
                strToHashCal += Client.SharedSecret;
                strHash = sha1(strToHashCal);
            } catch {
                strHash = null;
            }
            Security.Hash = strHash;
            return strHash;
        }
    }
}