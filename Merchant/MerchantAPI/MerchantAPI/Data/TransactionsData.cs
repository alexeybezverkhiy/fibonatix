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
        public Guid getNewTransactionID()
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// Set real transaction ID from CommDoo for assigned ID for Merchant 
        /// </summary>
        /// <param name="transID">ID that was assigned for transaction in first part of Sale/Preauth request</param>
        /// <param name="realID">ID that was returned from CommDoo in notification callback and used for next calls to CommDoo</param>
        public void setRealTransactionID(Guid transID, Guid realID)
        {
            using (var db = new PersistenceContext())
            {
                // Query for the Blog named ADO.NET Blog 
                var tr = db.Transactions
                    .Where(t => t.TransactionId == transID)
                    .FirstOrDefault();
                tr.ProcessingTransactionId = realID;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Return transaction ID of operation that can be used for access to CommDoo by ID assigned by Fibonatix side
        /// </summary>
        /// <param name="transID">ID assigned by Fibonatix and returns to Merchant in first part of Sale/Preauth request</param>
        /// <returns>ID assigned bu CommDoo and used for access to CommDoo side</returns>
        public Guid getRealTransactionID(Guid transID)
        {
            using (var db = new PersistenceContext())
            {
                var tr = db.Transactions
                    .Where(t => t.TransactionId == transID)
                    .FirstOrDefault();
                return (Guid) tr.ProcessingTransactionId;
            }
        } 
    }
}