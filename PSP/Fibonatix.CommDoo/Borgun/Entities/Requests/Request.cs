using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Fibonatix.CommDoo.Borgun.Entities.Requests
{
    [Serializable()]
    [XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public abstract class SOAPRequest
    {
        public abstract string SOAPAction();

        public string getXml() {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.Add("SOAP-ENC", "http://schemas.xmlsoap.org/soap/encoding/");
            ns.Add("ser-root", "http://Borgun/Heimir/pub/ws/Authorization");
            XmlSerializer formatter = new XmlSerializer(this.GetType());
            StringWriter writer = new Utf8StringWriter();
            formatter.Serialize(writer, this, ns);
            var serializedValue = writer.ToString();
            return serializedValue;
        }
        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }

    public class Request
    {
        public enum TransactionType
        {
            Undefined = 0,
            CaptureOperation = 1,
            RefundOperation = 3,
            PartialReversalOperation = 4,
            PreAuthorizeOperation = 5,
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
