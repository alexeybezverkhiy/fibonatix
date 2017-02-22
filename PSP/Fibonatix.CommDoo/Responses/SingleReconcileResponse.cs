using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using System.IO;

namespace Fibonatix.CommDoo.Responses
{
    [Serializable()]
    [XmlRoot("Response")]
    public class SingleReconcileResponse : Response
    {
        [XmlElement(ElementName = "Reconcile")]
        public ResponseReconcileFunction reconcile { get; set; }

        public class ResponseReconcileFunction : ResponseFunction
        {
            [XmlElement(ElementName = "TransactionType")]
            public string type { get; set; }
            [XmlElement(ElementName = "ExtendedStatus")]
            public string ext_status { get; set; }
            [XmlElement(ElementName = "Message")]
            public string message { get; set; }
            [XmlElement(ElementName = "Amount")]
            public string amount { get; set; }
            [XmlElement(ElementName = "Currency")]
            public string currency { get; set; }
            [XmlElement(ElementName = "AuthorizationCode")]
            public string auth_code { get; set; }
            [XmlElement(ElementName = "ResponseCode")]
            public string response_code { get; set; }
            [XmlElement(ElementName = "Mode")]
            public string mode { get; set; }

        }
    }
}
