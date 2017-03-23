using System;
using System.Xml;
using System.Xml.Serialization;

namespace MerchantAPI.CommDoo.Configuration.Requests
{
    [Serializable()]
    [XmlRoot("Request")]
    public class QueryReqeust : Request
    {
        [XmlElement(ElementName = "EndPointID")]
        public string EndPointID { get; set; }
        [XmlElement(ElementName = "EndPointGroupID")]
        public string EndPointGroupID { get; set; }

        public override string executeRequest()
        {
            string requestURL = WebApiConfig.Settings.ConfigurationServiceUrl + "/Query";
            return sendRequest(requestURL);
        }
    }
}