using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Fibonatix.CommDoo.Responses
{
    [Serializable()]
    [XmlRoot("Response")]
    public class RefundResponse : Response
    {
        [XmlElement(ElementName = "Refund")]
        public ResponseFunction refund { get; set; }
    }
}


