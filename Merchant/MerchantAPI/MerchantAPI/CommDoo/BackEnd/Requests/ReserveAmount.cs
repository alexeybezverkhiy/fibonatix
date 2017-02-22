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
    public class ReserveAmountRequest : StartRequest
    {
        public override string executeRequest() {
            string requestURL = serviceURL + "/ReserveAmount";
            return sendRequest(requestURL);
        }
    }
}