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
    public class CaptureReservedAmount : Request
    {
        [XmlAttribute("Client")]
        public ClientData Client { get; set; }
        [XmlAttribute("Security")]
        public SecurityData Security { get; set; }
        [XmlAttribute("Payment")]
        public PaymentData Payment { get; set; }
        [XmlAttribute("Notification")]
        public NotificationData Notification { get; set; }
        [XmlAttribute("Customer")]
        public CustomerData Customer { get; set; }
        [XmlAttribute("Purchase")]
        public PurchaseData Purchase { get; set; }

        public override string calculateHash() {

            string strToHashCal = "";
            string strHash = null;

            try {
                if (Security == null) Security = new SecurityData();

                Security.Timestamp = GetWesternEuropeDateTime();

                strToHashCal += Client.ClientID;
                strToHashCal += Security.Timestamp;
                strToHashCal += Payment.Amount;
                strToHashCal += Payment.ReferenceID;
                strToHashCal += Payment.AdditionalData;
                if (Payment.RelatedInformation != null)
                    strToHashCal += Payment.RelatedInformation.ReferencedTransactionID;
                if (Notification != null)
                    strToHashCal += Notification.NotificationURL;
                if (Purchase != null) {
                    if (Purchase.Delivery != null) {
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
                    }
                    if (Purchase.Items != null) {
                        foreach( ItemData item in Purchase.Items) {
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
                strToHashCal += sharedSecret;
                strHash = sha1(strToHashCal);
            } catch {
                strHash = null;
            }
            return strHash;
        }
    }
}