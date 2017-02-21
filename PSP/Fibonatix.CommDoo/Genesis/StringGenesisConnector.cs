using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;

using Genesis.Net;
using Genesis.Net.Common;
using Genesis.Net.Entities;
using Genesis.Net.Entities.Requests.Initial;
using Genesis.Net.Entities.Requests.Initial.ThreeD;
using Genesis.Net.Entities.Requests.Query;
using Genesis.Net.Entities.Requests.Referential;
using Genesis.Net.Specs;
using Genesis.Net.Specs.Mocks;

using Fibonatix.CommDoo.Requests;
using Fibonatix.CommDoo.Responses;

namespace Fibonatix.CommDoo.Genesis
{
    public class StringGenesisConnector
    {
        private GenesisConnector connector;
        static private Dictionary<string, StringGenesisConnector> allconnectors = new Dictionary<string, StringGenesisConnector>();

        static public StringGenesisConnector getConnector(string login, string password, string token, bool sandbox) {
            string key = login + ":" + password + ":" + token + ":" + sandbox.ToString();
            if (!allconnectors.ContainsKey(key)) {
                StringGenesisConnector conn = new StringGenesisConnector(login, password, token, sandbox);
                allconnectors[key] = conn;
            }
            return allconnectors[key];
        }

        public StringGenesisConnector(string login, string password, string token, bool sandbox) {
            connector = new GenesisConnector(login, password, token, sandbox);
        }

        public string Preauthorize(string xml) {
            PreauthRequest commDooAuth = PreauthRequest.DeserializeFromString(xml);
            var response = connector.Preauthorize(commDooAuth);
            return response.getXml();
        }


        public string Capture(string xml) {
            CaptureRequest commDooCapture = CaptureRequest.DeserializeFromString(xml);
            var response = connector.Capture(commDooCapture);
            return response.getXml();
        }

        public string Purchase(string xml) {
            PurchaseRequest commDooPurchase = PurchaseRequest.DeserializeFromString(xml);
            var response = connector.Purchase(commDooPurchase);
            return response.getXml();
        }


        public string Refund(string xml) {
            RefundRequest commDooRefund = RefundRequest.DeserializeFromString(xml);
            var response = connector.Refund(commDooRefund);
            return response.getXml();
        }


        public string Reversal(string xml) {
            ReversalRequest commDooReversal = ReversalRequest.DeserializeFromString(xml);
            var response = connector.Reversal(commDooReversal);
            return response.getXml();
        }

        public string EnrollmentCheck3D(string xml) {
            EnrollmentCheck3DRequest enrollment3D = EnrollmentCheck3DRequest.DeserializeFromString(xml);
            var response = connector.EnrollmentCheck3D(enrollment3D);
            return response.getXml();
        }

        public string Preauthorize3D(string xml) {
            Preauth3DRequest commDooAuth3D = Preauth3DRequest.DeserializeFromString(xml);
            var response = connector.Preauthorize3D(commDooAuth3D);
            return response.getXml();
        }

        public string Purchase3D(string xml) {
            Purchase3DRequest commDooPurchase = Purchase3DRequest.DeserializeFromString(xml);
            var response = connector.Purchase3D(commDooPurchase);
            return response.getXml();
        }

    }
}
