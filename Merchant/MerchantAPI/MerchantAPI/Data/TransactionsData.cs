using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MerchantAPI.Data
{
    public class TransactionsData
    {
        /// <summary>
        /// Assign ID of transaction for return to Merchants
        /// </summary>
        /// <returns>unique string that pass to Merchant as paynet transaction ID</returns>
        public string getNewTransactionID() { return ""; } 
        /// <summary>
        /// Set real transaction ID from CommDoo for assigned ID for Merchant 
        /// </summary>
        /// <param name="transID">ID that was assigned for transaction in first part of Sale/Preauth request</param>
        /// <param name="realID">ID that was returned from CommDoo in notification callback and used for next calls to CommDoo</param>
        public void setRealTransactionID(string transID, string realID) { }
        /// <summary>
        /// Return transaction ID of operation that can be used for access to CommDoo by ID assigned by Fibonatix side
        /// </summary>
        /// <param name="transID">ID assigned by Fibonatix and returns to Merchant in first part of Sale/Preauth request</param>
        /// <returns>ID assigned bu CommDoo and used for access to CommDoo side</returns>
        public string getRealTransactionID(string transID) { return null; } 
    }
}