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
    public class NotificationProcessingResponse : Response
    {
        [XmlElement(ElementName = "NotificationProcessing")]
        public NotificationProcessingSection notification { get; set; }
        public class NotificationProcessingSection
        {
            [XmlElement(ElementName = "Transaction")]
            public Transaction transaction { get; set; }
            public class Transaction
            {
                [XmlElement(ElementName = "TransactionType")]
                public string transaction_type { get; set; }
                [XmlElement(ElementName = "ProcessingStatus")]
                public ProcessingStatus processing_status { get; set; }
                public class ProcessingStatus
                {
                    [XmlElement(ElementName = "ReferenceID")]
                    public string reference_id { get; set; }
                    [XmlElement(ElementName = "ProviderTransactionID")]
                    public string ProviderTransactionID { get; set; }
                    [XmlElement(ElementName = "FunctionResult")]
                    public string FunctionResult { get; set; }
                    [XmlElement(ElementName = "Amount")]
                    public decimal amount { get; set; }
                    [XmlElement(ElementName = "Currency")]
                    public string currency { get; set; }
                }
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
            [XmlElement(ElementName = "RawResponse")]
            public RawResponse raw_data { get; set; }
            public class RawResponse
            {
                [XmlElement(ElementName = "ContentType")]
                public string content_type { get; set; }
                [XmlElement(ElementName = "ContentData")]
                public string content_data { get; set; }
            }
        }
    }
}
