using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Fibonatix.CommDoo.Borgun.Helpers
{
    public class TransactionsTypes
    {

        public static string getTransactionType(string Code) {
            switch (Code) {
                case "1": return "Sale";
                case "3": return "Refund";
                case "4": return "Partial Reversal";
                case "5": return "Preauthorize";
                default: return null;
            }
        }
        public static string getTransactionStatus(string Code) {
            switch (Code) {
                case "1": return "Sale initial status";
                case "2": return "Sale successful";
                case "3": return "Sale failed";
                case "4": return "Refund initial status";
                case "5": return "Refund successful";
                case "6": return "Refund failed";
                case "7": return "Preauthorization initial status";
                case "8": return "Preauthorization successful";
                case "9": return "Preauthorization failed";
                case "10": return "Reversal initial status";
                case "11": return "Reversal successful";
                case "12": return "Reversal failed";
                case "14": return "Financial record sent";
                default: return null;
            }
        }
    }
}