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
    public class RefundRequest : Request
    {
        [XmlElement(ElementName = "Client")]
        public ClientData Client { get; set; }
        [XmlElement(ElementName = "Security")]
        public SecurityData Security { get; set; }
        [XmlElement(ElementName = "Order")]
        public PaymentData Order { get; set; }
        [XmlElement(ElementName = "Purchase")]
        public PurchaseData Purchase { get; set; }

        public static RefundRequest createRequestByModel(ReturnRequestModel model, int endpointId, string commDooReferencedTransactionID) {
            RefundRequest request = new RefundRequest() {
                Client = new ClientData() {
                    ClientID = WebApiConfig.Settings.GetClientID(endpointId),
                    SharedSecret = WebApiConfig.Settings.GetSharedSecret(endpointId),
                },
                Order = new PaymentData() {
                    Amount = string.IsNullOrEmpty(model.amount) ? null : CurrencyConverter.MajorAmountToMinor(model.amount, model.currency),
                    Currency = string.IsNullOrEmpty(model.currency) ? null : model.currency,
                    RelatedInformation = new RelatedInformationData() {
                        ReferencedTransactionID = commDooReferencedTransactionID,
                        // RefundType = "Accommodation",
                    },
                },
            };
            return request;
        }

        public override string executeRequest() {
            string requestURL = WebApiConfig.Settings.BackendServiceUrlRefund + "/CreditNote";
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
                strToHashCal += Order.Amount;
                strToHashCal += Order.Currency;
                if (Order.RelatedInformation != null) {
                    if(!string.IsNullOrEmpty(Order.RelatedInformation.ReferencedTransactionID))
                        strToHashCal += /* "ReferencedTransactionID" + */ Order.RelatedInformation.ReferencedTransactionID;
                    if (!string.IsNullOrEmpty(Order.RelatedInformation.RefundType))
                        strToHashCal += /* "RefundType" + */ Order.RelatedInformation.RefundType;
                }
                if (Purchase != null) {
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