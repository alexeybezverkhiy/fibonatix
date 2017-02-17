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
    public class Refund : Request
    {
        [XmlAttribute("Client")]
        public ClientData Client { get; set; }
        [XmlAttribute("Security")]
        public SecurityData Security { get; set; }
        [XmlAttribute("Order")]
        public PaymentData Order { get; set; }
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
                strToHashCal += Order.Amount;
                strToHashCal += Order.Currency;
                if (Order.RelatedInformation != null) {
                    strToHashCal += Order.RelatedInformation.ReferencedTransactionID;
                    strToHashCal += Order.RelatedInformation.RefundType;
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
                strToHashCal += sharedSecret;
                strHash = sha1(strToHashCal);
            } catch {
                strHash = null;
            }
            return strHash;
        }
    }
}