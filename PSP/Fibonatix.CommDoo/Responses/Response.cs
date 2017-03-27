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
    public class Response
    {
        public class Transaction
        {
            [XmlElement(ElementName = "ReferenceID")]
            public string reference_id { get; set; }

            [XmlElement(ElementName = "ProcessingStatus")]
            public ProcessingStatus processing_status { get; set; }
            public class ProcessingStatus {
                /*
                [XmlIgnore]
                static string datePatt = @"yyyy-MM-dd HH:mm:ss";
                public ProcessingStatus() {
                    var utc = DateTime.Now.ToUniversalTime();
                    TimeStamp = utc.ToString(datePatt);
                }
                */

                [XmlElement(ElementName = "ProviderTransactionID")]
                public string ProviderTransactionID { get; set; }
                [XmlElement(ElementName = "CreditCardAlias")]
                public string CreditCardAlias { get; set; }
                [XmlElement(ElementName = "AuthCode")]
                public string AuthCode { get; set; }
                [XmlElement(ElementName = "RRN")]
                public string RRN { get; set; }
                [XmlElement(ElementName = "DateAndTime")]
                public string DateAndTime { get; set; }
                [XmlElement(ElementName = "TerminalID")]
                public string TerminalID { get; set; }

                [XmlElement(ElementName = "StatusType")]
                public string StatusType { get; set; }
                [XmlElement(ElementName = "FunctionResult")]
                public string FunctionResult { get; set; }

                // [XmlElement(ElementName = "TimeStamp")]
                // public string TimeStamp { get; set; }

                [XmlElement(ElementName = "Error")]
                public Error error { get; set; }

                public class Error
                {
                    [XmlElement(ElementName = "Type")]
                    public string type { get; set; }
                    [XmlElement(ElementName = "Number")]
                    public string number { get; set; }
                    [XmlElement(ElementName = "Message")]
                    public string message { get; set; }
                }
            }

            [XmlElement(ElementName = "ThreeDSecure")]
            public Secure3D secure3D { get; set; }
            public class Secure3D
            {
                [XmlElement(ElementName = "PAReq")]
                public string pa_req { get; set; }
                [XmlElement(ElementName = "MD")]
                public string md { get; set; }
                [XmlElement(ElementName = "TermUrl")]
                public string term_url { get; set; }
                [XmlElement(ElementName = "AcsUrl")]
                public string acs_url { get; set; }
                [XmlElement(ElementName = "PostData")]
                public string post_data { get; set; }
            }
        }

        public class ResponseFunction
        {
            [XmlElement(ElementName = "Transaction")]
            public Transaction transaction { get; set; }
        }

        public string getXml() {
            XmlSerializer formatter = new XmlSerializer(this.GetType());
            StringWriter writer = new Utf8StringWriter();
            formatter.Serialize(writer, this);
            var serializedValue = writer.ToString();
            return serializedValue;
        }
        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}