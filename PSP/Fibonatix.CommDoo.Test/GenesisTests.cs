using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fibonatix.CommDoo;
using Fibonatix.CommDoo.Genesis;

namespace Fibonatix.CommDoo.Test
{
    class GenesisTests
    {
        string reference_id = "0000-0000";
        string amount = "133";

        internal static class CardsNumbers
        {
            public const string VisaSuccessfulTransaction = "4200000000000000";
            public const string VisaDeclinedTransaction = "4111111111111111";
            public const string MasterCardSuccessfulTransaction = "5555555555554444";
            public const string MasterCardDeclinedTransaction = "5105105105105100";
            public const string Visa3dSecureEnrolled = "4711100000000000";
        }

        public Responses.PreauthResponse PreauthTest() {

            string xml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<Request>" +
                "<Preauthorization>" +
                "<Configurations>" +
                "<Configuration>" +
                "<Name>login</Name>" +
                "<Value>7d5ff68740b50e12dad17a51749c0529d4ca32cd</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>password</Name>" +
                "<Value>58ac52a8e4d4e01c73b36d77b053217a4471210f</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>token</Name>" +
                "<Value>9cc366d693e008bb7036d8be24ad1bcb4cae3057</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>acquirer</Name>" +
                "<Value>genesis</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>testmode</Name>" +
                "<Value>true</Value>" +
                "</Configuration>" +
                "</Configurations>" +
                "<Transaction>" +
                "<ReferenceID>" + reference_id + "-PREA-" + String.Format("{0,8:X8}", DateTime.Now.ToBinary()) + "</ReferenceID>" +
                "<Amount>" + amount + "</Amount>" +
                "<Currency>USD</Currency>" +
                "<Usage>Usage Test</Usage>" +
                "<CreditCardData>" +
                "<CVV>111</CVV>" +
                "<ExpirationYear>2019</ExpirationYear>" +
                "<ExpirationMonth>01</ExpirationMonth>" +
                "<CardHolderName>John Doe</CardHolderName>" +
                "<CreditCardNumber>" + CardsNumbers.VisaSuccessfulTransaction + "</CreditCardNumber>" +
                "</CreditCardData>" +
                "<CustomerData>" +
                "<Firstname>John</Firstname>" +
                "<Lastname>Doe</Lastname>" +
                "<Street>Trump bld. 1</Street>" +
                "<PostalCode>10000</PostalCode>" +
                "<City>New York</City>" +
                "<Country>USA</Country>" +
                "<State>NY</State>" +
                "<Email>john.doe@trump.com</Email>" +
                "<Phone>+18001234567</Phone>" +
                "<IPAddress>8.8.8.8</IPAddress>" +
                "</CustomerData>" +
                "</Transaction>" +
                "</Preauthorization>" +
                "</Request>";

            Requests.PreauthRequest request = Requests.PreauthRequest.DeserializeFromString(xml);
            var conn = ConnectorFactory.Create(request);
            var res = conn.Preauthorize(request);

            Console.WriteLine("Preauthorize request: {0}", xml);
            Console.WriteLine("Preauthorize response: {0}", res.getXml());
            return res;
        }

        public Responses.CaptureResponse CaptureTest(string providerID = null) {

            string xml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<Request>" +
                "<Capture>" +
                "<Configurations>" +
                "<Configuration>" +
                "<Name>login</Name>" +
                "<Value>7d5ff68740b50e12dad17a51749c0529d4ca32cd</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>password</Name>" +
                "<Value>58ac52a8e4d4e01c73b36d77b053217a4471210f</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>token</Name>" +
                "<Value>9cc366d693e008bb7036d8be24ad1bcb4cae3057</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>acquirer</Name>" +
                "<Value>genesis</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>testmode</Name>" +
                "<Value>true</Value>" +
                "</Configuration>" +
                "</Configurations>" +
                "<Transaction>" +
                "<ReferenceID>" + reference_id + "-CAPT-" + String.Format("{0,8:X8}", DateTime.Now.ToBinary()) + "</ReferenceID>" +
                "<ProviderTransactionID>" + ( providerID != null ? providerID  : (reference_id + "-" + String.Format("{0,8:X8}", DateTime.Now.ToBinary()))) + "</ProviderTransactionID>" +
                "<Amount>" + amount + "</Amount>" +
                "<Currency>USD</Currency>" +
                "<Usage>Capture test</Usage>" +
                "</Transaction>" +
                "</Capture>" +
                "</Request>";

            Requests.CaptureRequest request = Requests.CaptureRequest.DeserializeFromString(xml);
            var conn = ConnectorFactory.Create(request);
            var res = conn.Capture(request);

            Console.WriteLine("Capture request: {0}", xml);
            Console.WriteLine("Capture response: {0}", res.getXml());
            return res;
        }

        public void PreauthPlusCaptureTest() {

            var preRes = PreauthTest();
            var capRes = CaptureTest(preRes.preAuth.transaction.processing_status.ProviderTransactionID);
        }

        public Responses.PurchaseResponse PurchaseTest() {

            string xml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<Request>" +
                "<Purchase>" +
                "<Configurations>" +
                "<Configuration>" +
                "<Name>login</Name>" +
                "<Value>7d5ff68740b50e12dad17a51749c0529d4ca32cd</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>password</Name>" +
                "<Value>58ac52a8e4d4e01c73b36d77b053217a4471210f</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>token</Name>" +
                "<Value>9cc366d693e008bb7036d8be24ad1bcb4cae3057</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>acquirer</Name>" +
                "<Value>genesis</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>testmode</Name>" +
                "<Value>true</Value>" +
                "</Configuration>" +
                "</Configurations>" +
                "<Transaction>" +
                "<ReferenceID>" + reference_id + "-PRCH-" + String.Format("{0,8:X8}", DateTime.Now.ToBinary()) + "</ReferenceID>" +
                "<Amount>" + amount + "</Amount>" +
                "<Currency>USD</Currency>" +
                "<Usage>Usage Test</Usage>" +
                "<CreditCardData>" +
                "<CVV>111</CVV>" +
                "<ExpirationYear>2019</ExpirationYear>" +
                "<ExpirationMonth>01</ExpirationMonth>" +
                "<CardHolderName>John Doe</CardHolderName>" +
                "<CreditCardNumber>" + CardsNumbers.VisaSuccessfulTransaction + "</CreditCardNumber>" +
                "</CreditCardData>" +
                "<CustomerData>" +
                "<Firstname>John</Firstname>" +
                "<Lastname>Doe</Lastname>" +
                "<Street>Trump bld. 1</Street>" +
                "<PostalCode>10000</PostalCode>" +
                "<City>New York</City>" +
                "<Country>USA</Country>" +
                "<State>NY</State>" +
                "<Email>john.doe@trump.com</Email>" +
                "<Phone>+18001234567</Phone>" +
                "<IPAddress>8.8.8.8</IPAddress>" +
                "</CustomerData>" +
                "</Transaction>" +
                "</Purchase>" +
                "</Request>";

            Requests.PurchaseRequest request = Requests.PurchaseRequest.DeserializeFromString(xml);
            var conn = ConnectorFactory.Create(request);
            var res = conn.Purchase(request);

            Console.WriteLine("Purchase request: {0}", xml);
            Console.WriteLine("Purchase response: {0}", res.getXml());

            return res;
        }

        public Responses.ReversalResponse ReversalTest(string providerID = null) {

            string xml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<Request>" +
                "<Reversal>" +
                "<Configurations>" +
                "<Configuration>" +
                "<Name>login</Name>" +
                "<Value>7d5ff68740b50e12dad17a51749c0529d4ca32cd</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>password</Name>" +
                "<Value>58ac52a8e4d4e01c73b36d77b053217a4471210f</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>token</Name>" +
                "<Value>9cc366d693e008bb7036d8be24ad1bcb4cae3057</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>acquirer</Name>" +
                "<Value>genesis</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>testmode</Name>" +
                "<Value>true</Value>" +
                "</Configuration>" +
                "</Configurations>" +
                "<Transaction>" +
                "<ReferenceID>" + reference_id + "-VOID-" + String.Format("{0,8:X8}", DateTime.Now.ToBinary()) + "</ReferenceID>" +
                "<ProviderTransactionID>" + (providerID != null ? providerID : (reference_id + "-" + String.Format("{0,8:X8}", DateTime.Now.ToBinary()))) + "</ProviderTransactionID>" +
                "<Amount>" + amount + "</Amount>" +
                "<Currency>USD</Currency>" +
                "<Usage>Capture test (unexisting ReferenceID)</Usage>" +
                "</Transaction>" +
                "</Reversal>" +
                "</Request>";

            Requests.ReversalRequest request = Requests.ReversalRequest.DeserializeFromString(xml);
            var conn = ConnectorFactory.Create(request);
            var res = conn.Reversal(request);

            Console.WriteLine("Reversal request: {0}", xml);
            Console.WriteLine("Reversal response: {0}", res.getXml());

            return res;
        }

        public Responses.RefundResponse RefundTest(string providerID = null) {

            string xml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<Request>" +
                "<Refund>" +
                "<Configurations>" +
                "<Configuration>" +
                "<Name>login</Name>" +
                "<Value>7d5ff68740b50e12dad17a51749c0529d4ca32cd</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>password</Name>" +
                "<Value>58ac52a8e4d4e01c73b36d77b053217a4471210f</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>token</Name>" +
                "<Value>9cc366d693e008bb7036d8be24ad1bcb4cae3057</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>acquirer</Name>" +
                "<Value>genesis</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>testmode</Name>" +
                "<Value>true</Value>" +
                "</Configuration>" +
                "</Configurations>" +
                "<Transaction>" +
                "<ReferenceID>" + reference_id + "-RFND-" + String.Format("{0,8:X8}", DateTime.Now.ToBinary()) + "</ReferenceID>" +
                "<ProviderTransactionID>" + (providerID != null ? providerID : (reference_id + "-" + String.Format("{0,8:X8}", DateTime.Now.ToBinary()))) + "</ProviderTransactionID>" +
                "<Amount>" + amount + "</Amount>" +
                "<Currency>USD</Currency>" +
                "<Usage>Capture test (unexisting ReferenceID)</Usage>" +
                "</Transaction>" +
                "</Refund>" +
                "</Request>";

            Requests.RefundRequest request = Requests.RefundRequest.DeserializeFromString(xml);
            var conn = ConnectorFactory.Create(request);
            var res = conn.Refund(request);

            Console.WriteLine("Refund request: {0}", xml);
            Console.WriteLine("Refund response: {0}", res.getXml());

            return res;
        }

        public void PreauthPlusReversalTest() {
            var preRes = PreauthTest();
            var revRes = ReversalTest(preRes.preAuth.transaction.processing_status.ProviderTransactionID);
        }
        public void PurchasePlusRefundTest() {
            var purchRes = PurchaseTest();
            var refundRes = RefundTest(purchRes.purchase.transaction.processing_status.ProviderTransactionID);
        }


        public Responses.PreauthResponse PreauthRecurrenceTest(string recc = "SINGLE") {

            string xml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<Request>" +
                "<Preauthorization>" +
                "<Configurations>" +
                "<Configuration>" +
                "<Name>login</Name>" +
                "<Value>7d5ff68740b50e12dad17a51749c0529d4ca32cd</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>password</Name>" +
                "<Value>58ac52a8e4d4e01c73b36d77b053217a4471210f</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>token</Name>" +
                "<Value>9cc366d693e008bb7036d8be24ad1bcb4cae3057</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>acquirer</Name>" +
                "<Value>genesis</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>testmode</Name>" +
                "<Value>true</Value>" +
                "</Configuration>" +
                "</Configurations>" +
                "<Transaction>" +
                "<ReferenceID>" + reference_id + "-RPREA-" + String.Format("{0,8:X8}", DateTime.Now.ToBinary()) + "</ReferenceID>" +
                "<Amount>" + amount + "</Amount>" +
                "<Currency>USD</Currency>" +
                "<Usage>Usage Test</Usage>" +
                "<RecurringTransaction>" +
                "<Type>" + recc +"</Type>" +
                "</RecurringTransaction>" +
                "<CreditCardData>" +
                "<CVV>111</CVV>" +
                "<ExpirationYear>2019</ExpirationYear>" +
                "<ExpirationMonth>01</ExpirationMonth>" +
                "<CardHolderName>John Doe</CardHolderName>" +
                "<CreditCardNumber>" + CardsNumbers.VisaSuccessfulTransaction + "</CreditCardNumber>" +
                "</CreditCardData>" +
                "<CustomerData>" +
                "<Firstname>John</Firstname>" +
                "<Lastname>Doe</Lastname>" +
                "<Street>Trump bld. 1</Street>" +
                "<PostalCode>10000</PostalCode>" +
                "<City>New York</City>" +
                "<Country>USA</Country>" +
                "<State>NY</State>" +
                "<Email>john.doe@trump.com</Email>" +
                "<Phone>+18001234567</Phone>" +
                "<IPAddress>8.8.8.8</IPAddress>" +
                "</CustomerData>" +
                "</Transaction>" +
                "</Preauthorization>" +
                "</Request>";

            Requests.PreauthRequest request = Requests.PreauthRequest.DeserializeFromString(xml);
            var conn = ConnectorFactory.Create(request);
            var res = conn.Preauthorize(request);

            Console.WriteLine("Reccuring Preauthorize request: {0}", xml);
            Console.WriteLine("Reccuring Preauthorize response: {0}", res.getXml());
            return res;
        }

        public Responses.PurchaseResponse PurchaseRecurrenceTest(string recc = "SINGLE", string providerID = null) {

            string xml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<Request>" +
                "<Purchase>" +
                "<Configurations>" +
                "<Configuration>" +
                "<Name>login</Name>" +
                "<Value>7d5ff68740b50e12dad17a51749c0529d4ca32cd</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>password</Name>" +
                "<Value>58ac52a8e4d4e01c73b36d77b053217a4471210f</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>token</Name>" +
                "<Value>9cc366d693e008bb7036d8be24ad1bcb4cae3057</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>acquirer</Name>" +
                "<Value>genesis</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>testmode</Name>" +
                "<Value>true</Value>" +
                "</Configuration>" +
                "</Configurations>" +
                "<Transaction>" +
                "<ReferenceID>" + reference_id + "-RPRCH-" + String.Format("{0,8:X8}", DateTime.Now.ToBinary()) + "</ReferenceID>" +
                "<ProviderTransactionID>" + (providerID != null ? providerID : (reference_id + "-" + String.Format("{0,8:X8}", DateTime.Now.ToBinary()))) + "</ProviderTransactionID>" +
                "<Amount>" + amount + "</Amount>" +
                "<Currency>USD</Currency>" +
                "<Usage>Usage Test</Usage>" +
                "<RecurringTransaction>" +
                "<Type>" + recc + "</Type>" +
                "</RecurringTransaction>" +
                "<CreditCardData>" +
                "<CVV>111</CVV>" +
                "<ExpirationYear>2019</ExpirationYear>" +
                "<ExpirationMonth>01</ExpirationMonth>" +
                "<CardHolderName>John Doe</CardHolderName>" +
                "<CreditCardNumber>" + CardsNumbers.VisaSuccessfulTransaction + "</CreditCardNumber>" +
                "</CreditCardData>" +
                "<CustomerData>" +
                "<Firstname>John</Firstname>" +
                "<Lastname>Doe</Lastname>" +
                "<Street>Trump bld. 1</Street>" +
                "<PostalCode>10000</PostalCode>" +
                "<City>New York</City>" +
                "<Country>USA</Country>" +
                "<State>NY</State>" +
                "<Email>john.doe@trump.com</Email>" +
                "<Phone>+18001234567</Phone>" +
                "<IPAddress>8.8.8.8</IPAddress>" +
                "</CustomerData>" +
                "</Transaction>" +
                "</Purchase>" +
                "</Request>";

            Requests.PurchaseRequest request = Requests.PurchaseRequest.DeserializeFromString(xml);
            var conn = ConnectorFactory.Create(request);
            var res = conn.Purchase(request);

            Console.WriteLine("Reccuring Purchase request: {0}", xml);
            Console.WriteLine("Reccuring Purchase response: {0}", res.getXml());

            return res;
        }

        public Responses.EnrollmentCheck3DResponse EnrollmentCheckTest() {

            string xml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<Request>" +
                "<EnrollmentCheck>" +
                "<Configurations>" +
                "<Configuration>" +
                "<Name>login</Name>" +
                "<Value>7d5ff68740b50e12dad17a51749c0529d4ca32cd</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>password</Name>" +
                "<Value>58ac52a8e4d4e01c73b36d77b053217a4471210f</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>token</Name>" +
                "<Value>9cc366d693e008bb7036d8be24ad1bcb4cae3057</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>acquirer</Name>" +
                "<Value>genesis</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>testmode</Name>" +
                "<Value>true</Value>" +
                "</Configuration>" +
                "</Configurations>" +
                "<Transaction>" +
                "<ReferenceID>" + reference_id + "-3DENRL-" + String.Format("{0,8:X8}", DateTime.Now.ToBinary()) + "</ReferenceID>" +
                "<Amount>500</Amount>" +
                "<Currency>USD</Currency>" +
                "<Usage>123456</Usage>" +
                "<CreditCardData>" +
                "<CVV>001</CVV>" +
                "<ExpirationYear>2019</ExpirationYear>" +
                "<ExpirationMonth>01</ExpirationMonth>" +
                "<CardHolderName>John Doe</CardHolderName>" +
                "<CreditCardNumber>4711100000000000</CreditCardNumber>" +
                "</CreditCardData>" +
                "<CustomerData>" +
                "<Firstname>John</Firstname>" +
                "<Lastname>Doe</Lastname>" +
                "<Street>Trump bld. 1</Street>" +
                "<PostalCode>10000</PostalCode>" +
                "<City>New York</City>" +
                "<Country>USA</Country>" +
                "<State>NY</State>" +
                "<Email>john.doe@trump.com</Email>" +
                "<Phone>+18001234567</Phone>" +
                "<IPAddress>8.8.8.8</IPAddress>" +
                "</CustomerData>" +
                "</Transaction>" +
                "</EnrollmentCheck>" +
                "</Request>";

            Requests.EnrollmentCheck3DRequest request = Requests.EnrollmentCheck3DRequest.DeserializeFromString(xml);
            var conn = ConnectorFactory.Create(request);
            var res = conn.EnrollmentCheck3D(request);

            Console.WriteLine("Enrollment Check request: {0}", xml);
            Console.WriteLine("Enrollment Check response: {0}", res.getXml());

            return res;
        }


        public Responses.Preauth3DResponse Preauth3DTest() {

            string xml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<Request>" +
                "<Preauthorization3DSecure>" +
                "<Configurations>" +
                "<Configuration>" +
                "<Name>login</Name>" +
                "<Value>7d5ff68740b50e12dad17a51749c0529d4ca32cd</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>password</Name>" +
                "<Value>58ac52a8e4d4e01c73b36d77b053217a4471210f</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>token</Name>" +
                "<Value>9d669ebe5f5bbffcbeea5565a636608669f8b037</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>acquirer</Name>" +
                "<Value>genesis</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>testmode</Name>" +
                "<Value>true</Value>" +
                "</Configuration>" +
                "</Configurations>" +
                "<Transaction>" +
                "<ReferenceID>" + reference_id + "-RPREA-" + String.Format("{0,8:X8}", DateTime.Now.ToBinary()) + "</ReferenceID>" +
                "<Amount>" + amount + "</Amount>" +
                "<Currency>USD</Currency>" +
                "<Usage>Usage Test</Usage>" +
                "<RecurringTransaction>" +
                "<Type>SINGLE</Type>" +
                "</RecurringTransaction>" +
                "<Communication>" +
                "<NotificationURL>https://mynotification.com/notification.aspx/processid=12&amp;processkey=AB</NotificationURL>" +
                "<SuccessURL>https://mynotification.com/success.aspx/processid=12&amp;processkey=AB</SuccessURL>" +
                "<FailURL>https://mynotification.com/fail.aspx/processid=12&amp;processkey=AB</FailURL>" +
                "</Communication>" +
                "<CreditCardData>" +
                "<CVV>111</CVV>" +
                "<ExpirationYear>2019</ExpirationYear>" +
                "<ExpirationMonth>01</ExpirationMonth>" +
                "<CardHolderName>John Doe</CardHolderName>" +
                "<CreditCardNumber>" + CardsNumbers.Visa3dSecureEnrolled + "</CreditCardNumber>" +
                "</CreditCardData>" +
                "<CustomerData>" +
                "<Firstname>John</Firstname>" +
                "<Lastname>Doe</Lastname>" +
                "<Street>Trump bld. 1</Street>" +
                "<PostalCode>10000</PostalCode>" +
                "<City>New York</City>" +
                "<Country>USA</Country>" +
                "<State>NY</State>" +
                "<Email>john.doe@trump.com</Email>" +
                "<Phone>+18001234567</Phone>" +
                "<IPAddress>8.8.8.8</IPAddress>" +
                "</CustomerData>" +
                "</Transaction>" +
                "</Preauthorization3DSecure>" +
                "</Request>";

            Requests.Preauth3DRequest request = Requests.Preauth3DRequest.DeserializeFromString(xml);
            var conn = ConnectorFactory.Create(request);
            var res = conn.Preauthorize3D(request);

            Console.WriteLine("Preauthorize 3D request: {0}", xml);
            Console.WriteLine("Preauthorize 3D response: {0}", res.getXml());
            return res;
        }


        public Responses.Purchase3DResponse Purchase3DTest() {

            string xml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<Request>" +
                "<Purchase3DSecure>" +
                "<Configurations>" +
                "<Configuration>" +
                "<Name>login</Name>" +
                "<Value>7d5ff68740b50e12dad17a51749c0529d4ca32cd</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>password</Name>" +
                "<Value>58ac52a8e4d4e01c73b36d77b053217a4471210f</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>token</Name>" +
                "<Value>9d669ebe5f5bbffcbeea5565a636608669f8b037</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>acquirer</Name>" +
                "<Value>genesis</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>testmode</Name>" +
                "<Value>true</Value>" +
                "</Configuration>" +
                "</Configurations>" +
                "<Transaction>" +
                "<ReferenceID>" + reference_id + "-RPREA-" + String.Format("{0,8:X8}", DateTime.Now.ToBinary()) + "</ReferenceID>" +
                "<Amount>" + amount + "</Amount>" +
                "<Currency>USD</Currency>" +
                "<Usage>Usage Test</Usage>" +
                "<Communication>" +
                "<NotificationURL>https://mynotification.com/notification.aspx/processid=12&amp;processkey=AB</NotificationURL>" +
                "<SuccessURL>https://mynotification.com/success.aspx/processid=12&amp;processkey=AB</SuccessURL>" +
                "<FailURL>https://mynotification.com/fail.aspx/processid=12&amp;processkey=AB</FailURL>" +
                "</Communication>" +
                "<CreditCardData>" +
                "<CVV>111</CVV>" +
                "<ExpirationYear>2019</ExpirationYear>" +
                "<ExpirationMonth>01</ExpirationMonth>" +
                "<CardHolderName>John Doe</CardHolderName>" +
                "<CreditCardNumber>" + CardsNumbers.Visa3dSecureEnrolled + "</CreditCardNumber>" +
                "</CreditCardData>" +
                "<CustomerData>" +
                "<Firstname>John</Firstname>" +
                "<Lastname>Doe</Lastname>" +
                "<Street>Trump bld. 1</Street>" +
                "<PostalCode>10000</PostalCode>" +
                "<City>New York</City>" +
                "<Country>USA</Country>" +
                "<State>NY</State>" +
                "<Email>john.doe@trump.com</Email>" +
                "<Phone>+18001234567</Phone>" +
                "<IPAddress>8.8.8.8</IPAddress>" +
                "</CustomerData>" +
                "</Transaction>" +
                "</Purchase3DSecure>" +
                "</Request>";

            Requests.Purchase3DRequest request = Requests.Purchase3DRequest.DeserializeFromString(xml);
            var conn = ConnectorFactory.Create(request);
            var res = conn.Purchase3D(request);

            Console.WriteLine("Purchase 3D request: {0}", xml);
            Console.WriteLine("Purchase 3D response: {0}", res.getXml());
            return res;
        }

        public Responses.Purchase3DResponse PurchaseRecurrence3DTest(string recc = "SINGLE", string providerID = null) {

            string xml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<Request>" +
                "<Purchase3DSecure>" +
                "<Configurations>" +
                "<Configuration>" +
                "<Name>login</Name>" +
                "<Value>7d5ff68740b50e12dad17a51749c0529d4ca32cd</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>password</Name>" +
                "<Value>58ac52a8e4d4e01c73b36d77b053217a4471210f</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>token</Name>" +
                "<Value>9d669ebe5f5bbffcbeea5565a636608669f8b037</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>acquirer</Name>" +
                "<Value>genesis</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>testmode</Name>" +
                "<Value>true</Value>" +
                "</Configuration>" +
                "</Configurations>" +
                "<Transaction>" +
                "<ReferenceID>" + reference_id + "-RPREA-" + String.Format("{0,8:X8}", DateTime.Now.ToBinary()) + "</ReferenceID>" +
                "<ProviderTransactionID>" + (providerID != null ? providerID : (reference_id + "-" + String.Format("{0,8:X8}", DateTime.Now.ToBinary()))) + "</ProviderTransactionID>" +
                "<Amount>" + amount + "</Amount>" +
                "<Currency>USD</Currency>" +
                "<Usage>Usage Test</Usage>" +
                "<RecurringTransaction>" +
                "<Type>" + recc + "</Type>" +
                "</RecurringTransaction>" +
                "<Communication>" +
                "<NotificationURL>https://mynotification.com/notification.aspx/processid=12&amp;processkey=AB</NotificationURL>" +
                "<SuccessURL>https://mynotification.com/success.aspx/processid=12&amp;processkey=AB</SuccessURL>" +
                "<FailURL>https://mynotification.com/fail.aspx/processid=12&amp;processkey=AB</FailURL>" +
                "</Communication>" +
                "<CreditCardData>" +
                "<CVV>111</CVV>" +
                "<ExpirationYear>2019</ExpirationYear>" +
                "<ExpirationMonth>01</ExpirationMonth>" +
                "<CardHolderName>John Doe</CardHolderName>" +
                "<CreditCardNumber>" + CardsNumbers.Visa3dSecureEnrolled + "</CreditCardNumber>" +
                "</CreditCardData>" +
                "<CustomerData>" +
                "<Firstname>John</Firstname>" +
                "<Lastname>Doe</Lastname>" +
                "<Street>Trump bld. 1</Street>" +
                "<PostalCode>10000</PostalCode>" +
                "<City>New York</City>" +
                "<Country>USA</Country>" +
                "<State>NY</State>" +
                "<Email>john.doe@trump.com</Email>" +
                "<Phone>+18001234567</Phone>" +
                "<IPAddress>8.8.8.8</IPAddress>" +
                "</CustomerData>" +
                "</Transaction>" +
                "</Purchase3DSecure>" +
                "</Request>";

            Requests.Purchase3DRequest request = Requests.Purchase3DRequest.DeserializeFromString(xml);
            var conn = ConnectorFactory.Create(request);
            var res = conn.Purchase3D(request);

            Console.WriteLine("Recurring Purchase 3D request: {0}", xml);
            Console.WriteLine("Recurring Purchase 3D response: {0}", res.getXml());
            return res;
        }

        public Responses.NotificationProcessingResponse NotificationProcessing() {

            string xml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<Request>" +
                "<NotificationProcessing>" +
                "<Configurations>" +
                "<Configuration>" +
                "<Name>login</Name>" +
                "<Value>7d5ff68740b50e12dad17a51749c0529d4ca32cd</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>password</Name>" +
                "<Value>58ac52a8e4d4e01c73b36d77b053217a4471210f</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>token</Name>" +
                "<Value>9d669ebe5f5bbffcbeea5565a636608669f8b037</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>acquirer</Name>" +
                "<Value>genesis</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>testmode</Name>" +
                "<Value>true</Value>" +
                "</Configuration>" +
                "</Configurations>" +
                "<RawData>" +
                "<Get>nkey=354076423_QPQQ91C9BU</Get>" +
                "<Post>transaction_id=ORDER_30012017_3D2&amp;terminal_token=9d669ebe5f5bbffcbeea5565a636608669f8b037&amp;unique_id=6b7d6547ab6e97da77084550853c87ba&amp;transaction_type=init_recurring_sale3d&amp;status=approved&amp;signature=4bea0c8efe49da2920d8a6295f4e26086e33733a&amp;amount=12190&amp;eci=05</Post>" +
                "</RawData>" +
                "</NotificationProcessing>" +
                "</Request>";

            Requests.NotificationProcessingRequest request = Requests.NotificationProcessingRequest.DeserializeFromString(xml);
            var conn = ConnectorFactory.Create(request);
            var res = conn.NotificationProcessing(request);

            Console.WriteLine("Notification Processing request: {0}", xml);
            Console.WriteLine("Notification Processing response: {0}", res.getXml());

            return res;
        }

        public Responses.EvaluateProviderResponseResponse EvaluateProviderResponse() {

            string xml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<Request>" +
                "<EvaluateProviderResponse>" +
                "<Configurations>" +
                "<Configuration>" +
                "<Name>login</Name>" +
                "<Value>7d5ff68740b50e12dad17a51749c0529d4ca32cd</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>password</Name>" +
                "<Value>58ac52a8e4d4e01c73b36d77b053217a4471210f</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>token</Name>" +
                "<Value>9d669ebe5f5bbffcbeea5565a636608669f8b037</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>acquirer</Name>" +
                "<Value>genesis</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>testmode</Name>" +
                "<Value>true</Value>" +
                "</Configuration>" +
                "</Configurations>" +
                "<RawData>" +
                "<Get>nkey=354076423_QPQQ91C9BU</Get>" +
                "<Post>transaction_id=ORDER_30012017_3D2&amp;terminal_token=9d669ebe5f5bbffcbeea5565a636608669f8b037&amp;unique_id=6b7d6547ab6e97da77084550853c87ba&amp;transaction_type=init_recurring_sale3d&amp;status=approved&amp;signature=4bea0c8efe49da2920d8a6295f4e26086e33733a&amp;amount=12190&amp;eci=05</Post>" +
                "</RawData>" +
                "</EvaluateProviderResponse>" +
                "</Request>";

            Requests.EvaluateProviderResponseRequest request = Requests.EvaluateProviderResponseRequest.DeserializeFromString(xml);
            var conn = ConnectorFactory.Create(request);
            var res = conn.EvaluateProviderResponse(request);

            Console.WriteLine("Evaluate Provider Response request: {0}", xml);
            Console.WriteLine("Evaluate Provider Response response: {0}", res.getXml());

            return res;
        }


        public Responses.SingleReconcileResponse Reconcile() {

            string xml =
                "<?xml version =\"1.0\" encoding=\"UTF-8\"?>" +
                "<Request>" +
                "<Reconcile>" +
                "<Configurations>" +
                "<Configuration>" +
                "<Name>acquirer</Name>" +
                "<Value>Genesis</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>token</Name>" +
                "<Value>9d669ebe5f5bbffcbeea5565a636608669f8b037</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>password</Name>" +
                "<Value>58ac52a8e4d4e01c73b36d77b053217a4471210f</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>testmode</Name>" +
                "<Value>true</Value>" +
                "</Configuration>" +
                "<Configuration>" +
                "<Name>login</Name>" +
                "<Value>7d5ff68740b50e12dad17a51749c0529d4ca32cd</Value>" +
                "</Configuration>" +
                "</Configurations>" +
                "<Transaction>" +
                "<ReferenceID>ORDER_30012017_3D2</ReferenceID>" +
                "<ProviderTransactionID>6b7d6547ab6e97da77084550853c87ba</ProviderTransactionID>" +
                "</Transaction>" +
                "</Reconcile>" +
                "</Request>";

            Requests.SingleReconcileRequest request = Requests.SingleReconcileRequest.DeserializeFromString(xml);
            var conn = ConnectorFactory.Create(request);
            var res = conn.SingleReconcile(request);

            Console.WriteLine("Reconcile request: {0}", xml);
            Console.WriteLine("Reconcile response: {0}", res.getXml());

            return res;
        }



        public void FullTests() {
            // PreauthTest();               // ACK
            // CaptureTest();               // NOK
            // PreauthPlusCaptureTest();    // ACK + ACK
            PurchaseTest();              // ACK
            // ReversalTest();              // NOK
            // RefundTest();                // NOK
            // PreauthPlusReversalTest();   // ACK + ACK
            // PurchasePlusRefundTest();    // ACK + ACK

            // PreauthRecurrenceTest("SINGLE");        // ACK
            // PreauthRecurrenceTest("INITIAL");       // NOK
            // PreauthRecurrenceTest("REPEATED");      // NOK

            // PurchaseRecurrenceTest("SINGLE");               // ACK
            // var r = PurchaseRecurrenceTest("INITIAL");      // ACK
            // PurchaseRecurrenceTest("REPEATED", r.purchase.transaction.processing_status.ProviderTransactionID);      // ACK, but NOK - recurring_sale not support by terminal!

            // EnrollmentCheckTest();   // NOK
            // Preauth3DTest();         // PENDING
            // Purchase3DTest();           // PENDING
            // PurchaseRecurrence3DTest("SINGLE");             // PENDING
            // var r = PurchaseRecurrence3DTest("INITIAL");    // PENDING
            // PurchaseRecurrence3DTest("REPEATED", r.purchase3D.transaction.processing_status.ProviderTransactionID);   // NOK

            // NotificationProcessing();
            // EvaluateProviderResponse();
            // Reconcile();
        }
    }
}
