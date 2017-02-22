using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;
using System.Collections.Specialized;

namespace Fibonatix.CommDoo.ProcessingCom.Entities
{
    class Response
    {
        private NameValueCollection responseParameters = new NameValueCollection();

        public Response(string data) {
            try {
                responseParameters = HttpUtility.ParseQueryString(data);
            }
            catch {
                responseParameters.Clear();
            }
        }
        public string getParameter(string key) {
            return responseParameters.Get(key);
        }


        public string getErrorMessage() {
            string msg = "";
            if (responseParameters.Get("code") != null && responseParameters.Get("code") != "") {
                msg += messageByCode(responseParameters.Get("code"));
                if (responseParameters.Get("sub_code") != null && responseParameters.Get("sub_code") != "") {
                    msg += ": " + messageBySubCode(responseParameters.Get("sub_code"));
                }
            } else if (responseParameters.Get("sub_code") != null && responseParameters.Get("sub_code") != "") {
                msg += messageBySubCode(responseParameters.Get("sub_code"));
            } else  {
                msg += "Response has not code/subcode";
            }
            return msg;
        }

        public string messageByCode(string code) {

            switch (code) {
                case "0": //  SUCCESS 
                    return "Transaction Successful";
                case "1": //  BADACCT 
                    return "Bad Account Information";
                case "2": //  BADMID 
                    return "Bad MID Identifier";
                case "3": //  BADACCT 
                    return "Bad Account Information";
                case "4": //  BADACCT 
                    return "Bad Account Information";
                case "11": //  BADTRANSTYPE 
                    return "Unrecognized Transaction Type";
                case "12": //  BADTRANSAMT 
                    return "Transaction Amount of Bounds";
                case "21": //  BADUSEGWID 
                    return "Bad Value for Use Gateway ID";
                case "22": //  BADRECUR 
                    return "Bad Value for Recurring Flag";
                case "2X": //  BAD2COMM 
                    return "There was a Stage 2 Communication Error";
                case "31": //  BADORIGIN 
                    return "Bad Original Transaction ID";
                case "32": //  BADAUTHCODE 
                    return "Invalid  Authorization Code";
                case "3X": //  BADCOMM3 
                    return "There was a Stage 3 Communication Error";
                case "41": //  BADCUSTNAME 
                    return "Bad or Invalid Customer Information";
                case "42": //  BADCUSTNAME 
                    return "Bad or Invalid Customer Information";
                case "43": //  BADCUSTPHONE 
                    return "Bad or Invalid Customer Phone Number";
                case "44": //  BADCUSTADDR 
                    return "Bad or Invalid Customer Address";
                case "45": //  BADCUSTADDR 
                    return "Bad or Invalid Customer Address";
                case "46": //  BADCUSTADDR 
                    return "Bad or Invalid Customer Address";
                case "47": //  BADCUSTADDR 
                    return "Bad or Invalid Customer Address";
                case "48": //  BADCUSTADDR 
                    return "Bad or Invalid Customer Address";
                case "49": //  BADCOUNTRY 
                    return "Bad or Invalid Country Association";
                case "4X": //  BADCOMM4 
                    return "There was a Stage 4 Communication Error";
                case "5X": //  
                    return "Request Timeout";
                case "51": //  BADEMAIL 
                    return "Bad or Invalid Email Address value";
                case "52": //  BADIPADDR 
                    return "Bad or Invalid IP Address value";
                case "61": //  BADCNUM 
                    return "Bad or Invalid CC Number Passed";
                case "62": //  BADCEXPIRE 
                    return "Bad, Invalid, or Expired CC Expire Info";
                case "63": //  BADCEXPIRE 
                    return "Bad, Invalid, or Expired CC Expire Info";
                case "64": //  BADCCVV2 
                    return "Bad or Invalid CVV2 Information";
                case "65": //  BADCRDTYPE 
                    return "Card Type Not Supported	for	this MID";
                case "71": //  BADGWTRANSID 
                    return "Passed Gateway Transaction ID Not Found";
                case "72": //  BADCUSTOMFIELD 
                    return "Bad or Invalid Custom Field Value";
                case "73": //  BADCUSTOMFIELD 
                    return "Bad or Invalid Custom Field Value";
                case "74": //  BADCUSTOMFIELD 
                    return "Bad or Invalid Custom Field Value";
                case "75": //  BADCUSTOMFIELD 
                    return "Bad or Invalid Custom Field Value";
                case "99": //  DECLINED 
                    return "Transaction Declined by Bank";
                case "A1": //  BADAUTH 
                    return "Authorization Attempt Failure";
                case "AE": //  BADAUTH 
                    return "Authorization Attempt Failure";
                case "AX": //  BADAUTH 
                    return "Authorization Attempt Failure";
                case "80": //  BADRECIPIENTFIELD 
                    return "UK MCC 6012 required recipient fields are not present in request";
                case "B1": //  BADREVISS 
                    return "Reversal Cannot Be Issued on Requested Transaction ID";
                case "B2": //  BADREVSTS 
                    return "Reversal Cannot Be Issued on Requested Transaction ID due to it’s status";
                case "B3": //  BADTRNSTYP 
                    return "Reversal Cannot Be Issued on Requested Transaction ID due to it’s type";
                case "B4": //  BADPRVISS 
                    return "Reversal Cannot Be Issued on Requested Transaction ID due to previous actions";
                case "BA": //  BADCNTR 
                    return "Transaction Cannot Complete Due to Blacklist";
                case "BB": //  BADENTRYDB 
                    return "Transaction Cannot Complete Due to Blacklist";
                case "BC": //  BADENTRYDB 
                    return "Transaction Cannot Complete Due to Blacklist";
                case "BE": //  BADENTRYDB 
                    return "Transaction Cannot Complete Due to Blacklist";
                case "BI": //  BADCNTR 
                    return "Transaction Cannot Complete Due to Blacklist";
                case "BP": //  BADENTRYDB 
                    return "Transaction Cannot Complete Due to Blacklist";
                case "C0": //  BADCLOSEAMT 
                    return "Capture Cannot Be Issued on Authorization for Requested Amount";
                case "CP": //  BADAUTHST 
                    return "Capture Cannot Be Issued on Authorization in Current State";
                case "CX": //  BADAUTHCRT 
                    return "Capture Cannot Be Issued Due to Admin Blocks";
                case "F1": //  Card trx limit   
                    return "Card Transaction Limit";
                case "F2": //  Bad ticket range 
                    return "Bad ticket range";
                case "F3": //  Bad ticket amt 
                    return "Bad ticket amt";
                case "F5": //  IP Velocity check 
                    return "IP Velocity check";
                case "F6": //  BIN overuse 
                    return "BIN overuse";
                case "F7": //  Card Velocity check 
                    return "Card Velocity check";
                case "OF": //  BADORPROC 
                    return "Bad Transaction Request Process";
                case "OI": //  BADGWID 
                    return "Account Lookup Error";
                case "R1": //  BADRFDISS 
                    return "Refund Cannot Be Issued	as Requested";
                case "R2": //  BADRFDISS 
                    return "Refund Cannot Be Issued	as Requested";
                case "R3": //  BADRFDSTS 
                    return "Refund Cannot Be Issued	as Requested";
                case "TV": //  BADTHROT 
                    return "The transaction volume has exceeded the set caps.";
                case "V1": //  BADVOIDISS 
                    return "Void Cannot Be Issued on requested Transaction ID due to previous actions";
                case "V2": //  BADVOIDSTS 
                    return "Void Cannot Be Issued on requested Transaction ID due to it’s status";
                case "V3": //  BADTRNSTYP 
                    return "Void Cannot Be Issued on Transaction of this type";
                case "V4": //  BADPRVISS 
                    return "Void Cannot Be Issued on requested Transaction ID due to previous actions";
                case "XX": //  BADAPIREQ 
                default:
                    return "Malformed API request";
            }
        }

        public string messageBySubCode(string subcode)
        {
            switch(subcode) {
                case "00":
                    return "Approved";
                case "01":
                    return "Refer to issuer";
                case "03":
                    return "Error - Security  Restrictions - Call Help";
                case "04":
                    return "Pick up card";
                case "05":
                    return "Do not honor";
                case "06":
                    return "Error";
                case "07":
                    return "Pick up card (special)";
                case "10":
                    return "Approved for partial amount";
                case "12":
                    return "Invalid transaction";
                case "13":
                    return "Invalid amount";
                case "14":
                    return "Card number does not exist";
                case "30":
                    return "Format error";
                case "31":
                    return "Invalid Issuer";
                case "41":
                    return "Pick up card (lost card)";
                case "43":
                    return "Pick up card (stolen card)";
                case "51":
                    return "Not sufficient funds";
                case "54":
                    return "Expired card";
                case "55":
                    return "Incorrect PIN";
                case "57":
                    return "Transaction not permitted to card";
                case "58":
                    return "Transaction not permitted to terminal";
                case "78":
                    return "Previous message not found";
                case "79":
                    return "Invalid CVV2";
                case "91":
                    return "Issuer or switch inoperative";
                case "94":
                    return "Duplicate transmission";
                case "95":
                    return "Reconcile error";
                case "96":
                default:
                    return "System malfunction";
            }
        }

    }
}
