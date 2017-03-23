using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;
using MerchantAPI.Models;
using MerchantAPI.Helpers;

namespace MerchantAPI.CommDoo.BackEnd.Requests
{
    [Serializable()]
    [XmlRoot("Request")]
    public class CaptureReservedAmountRequest : Request
    {
        [Required]
        [XmlElement(ElementName = "Client")]
        public ClientData Client { get; set; }
        [Required]
        [XmlElement(ElementName = "Security")]
        public SecurityData Security { get; set; }
        [Required]
        [XmlElement(ElementName = "Payment")]
        public PaymentData Payment { get; set; }
        [XmlElement(ElementName = "Notification")]
        public NotificationData Notification { get; set; }
        [XmlElement(ElementName = "Customer")]
        public CustomerData Customer { get; set; }
        [XmlElement(ElementName = "Purchase")]
        public PurchaseData Purchase { get; set; }

        public static CaptureReservedAmountRequest createRequestByModel(CaptureRequestModel model, int endpointId, string commDooReferencedTransactionID)
        {
            CaptureReservedAmountRequest request = new CaptureReservedAmountRequest()
            {
                Client = new ClientData()
                {
                    ClientID = WebApiConfig.Settings.GetClientID(endpointId),
                    SharedSecret = WebApiConfig.Settings.GetSharedSecret(endpointId),
                },
                Payment = new PaymentData()
                {
                    Amount = CurrencyConverter.MajorAmountToMinor(model.amount, model.currency),
                    // Currency = model.currency,
                    ReferenceID = model.client_orderid + "-" + DateTime.Now.ToString("yyyyMMddHHmmss.fff"),
                    RelatedInformation = new RelatedInformationData()
                    {
                        ReferencedTransactionID = commDooReferencedTransactionID,
                    },
                },
            };
            return request;
        }

        public override string executeRequest()
        {
            string requestURL = WebApiConfig.Settings.BackendServiceUrl + "/CaptureReservedAmount";
            return sendRequest(requestURL);
        }

        public override string calculateHash()
        {

            string strToHashCal = "";
            string strHash = null;

            try
            {
                if (Security == null) Security = new SecurityData();

                Security.Timestamp = GetWesternEuropeDateTime();

                strToHashCal += Client.ClientID;
                strToHashCal += Security.Timestamp;
                strToHashCal += Payment.Amount;
                strToHashCal += Payment.ReferenceID;
                strToHashCal += Payment.AdditionalData;
                if (Payment.RelatedInformation != null)
                    strToHashCal += "ReferencedTransactionID" + Payment.RelatedInformation.ReferencedTransactionID;
                if (Notification != null)
                    strToHashCal += Notification.NotificationURL;
                if (Purchase != null)
                {
                    if (Purchase.Delivery != null)
                    {
                        if (Purchase.Delivery != null)
                        {
                            if (Purchase.Delivery.Person != null)
                            {
                                strToHashCal += Purchase.Delivery.Person.CompanyName;
                                strToHashCal += Purchase.Delivery.Person.FirstName;
                                strToHashCal += Purchase.Delivery.Person.LastName;
                                strToHashCal += Purchase.Delivery.Person.Salutation;
                                strToHashCal += Purchase.Delivery.Person.Title;
                            }
                            if (Purchase.Delivery.Address != null)
                            {
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
                    if (Purchase.Items != null)
                    {
                        foreach (ItemData item in Purchase.Items)
                        {
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
            }
            catch
            {
                strHash = null;
            }
            Security.Hash = strHash;
            return strHash;
        }
    }
}