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
    public class Query : Request
    {
        [XmlAttribute("Client")]
        public ClientData Client { get; set; }
        [XmlAttribute("Security")]
        public SecurityData Security { get; set; }
        [XmlAttribute("Payment")]
        public PaymentData Payment { get; set; }

        public override string calculateHash() {

            string strToHashCal = "";
            string strHash = null;

            try {
                if (Security == null) Security = new SecurityData();

                Security.Timestamp = GetWesternEuropeDateTime();

                strToHashCal += Client.ClientID;
                strToHashCal += Security.Timestamp;
                strToHashCal += Payment.TransactionID;
                strToHashCal += sharedSecret;
                strHash = sha1(strToHashCal);
            } catch {
                strHash = null;
            }
            return strHash;
        }
    }
}