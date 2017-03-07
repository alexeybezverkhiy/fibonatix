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
    public class EvaluateProviderResponseResponse : Response
    {
        [XmlElement(ElementName = "EvaluateProviderResponse")]
        public EvaluateProviderResponseSection evaluate_provider { get; set; }
        public class EvaluateProviderResponseSection
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
        }
    }
}
