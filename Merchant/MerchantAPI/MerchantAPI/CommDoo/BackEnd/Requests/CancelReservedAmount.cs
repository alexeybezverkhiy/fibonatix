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
    public class CancelReservedAmountRequest : Request
    {
        [XmlElement(ElementName = "Client")]
        public ClientData Client { get; set; }
        [XmlElement(ElementName = "Security")]
        public SecurityData Security { get; set; }
        [XmlElement(ElementName = "Payment")]
        public PaymentData Payment { get; set; }

        public override string executeRequest() {
            string requestURL = WebApiConfig.Settings.BackendServiceUrl + "/CancelReservedAmount";
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
                strToHashCal += Payment.RelatedInformation.ReferencedTransactionID;
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