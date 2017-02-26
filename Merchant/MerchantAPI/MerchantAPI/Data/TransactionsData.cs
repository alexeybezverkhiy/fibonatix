using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MerchantAPI.Services;

namespace MerchantAPI.Data
{

    public class TransactionsDataStorage
    {
        /*
         * abv: version with Cache
         * 
        private static void insertDataToDB(TransactionData newData) {
            HttpContext.Current.Cache.Insert("TransactionData:" + newData.fibonatixTransactionID, newData, null,
                System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0, 0));
        }
        private static TransactionData loadDataFromDB(string fibonatixTransactionID) {
            TransactionData data = (TransactionData)HttpContext.Current.Cache.Get("TransactionData:" + fibonatixTransactionID);
            return data;
        }
        */

        private static void CommitChanges(PersistenceContext db, Transaction transaction)
        {
            transaction.LastModified = DateTime.UtcNow;
            db.SaveChanges();
        }

        public static Transaction CreateNewTransaction(TransactionType type, string merchantTransactionID)
        {
            using (var db = new PersistenceContext())
            {
                Transaction created = new Transaction(type, merchantTransactionID);
                db.Transactions.Add(created);
                db.SaveChanges();
                return created;
            }
        }

        public static Transaction UpdateTransactionState(string transactionID, TransactionState state) {
            using (var db = new PersistenceContext()) {
                Transaction updated = db.Transactions
                    .FirstOrDefault(t => t.TransactionId == transactionID);
                if (updated == null) return null;
                updated.State = state;
                CommitChanges(db, updated);
                return updated;
            }
        }

        public static Transaction UpdateTransactionStatus(string transactionID, TransactionStatus status) {
            using (var db = new PersistenceContext()) {
                Transaction updated = db.Transactions
                    .FirstOrDefault(t => t.TransactionId == transactionID);
                if (updated == null) return null;
                updated.Status = status;
                CommitChanges(db, updated);
                return updated;
            }
        }

        public static Transaction UpdateTransaction(string transactionID, string processingTransactionID, TransactionState state)
        {
            using (var db = new PersistenceContext())
            {
                Transaction updated = db.Transactions
                    .FirstOrDefault(t => t.TransactionId == transactionID);
                if (updated == null) return null;
                updated.ProcessingTransactionId = processingTransactionID;
                updated.State = state;
                CommitChanges(db, updated);
                return updated;
            }
        }

        public static Transaction UpdateTransaction(
            string transactionID, 
            string processingTransactionID, 
            TransactionState state, 
            TransactionStatus status)
        {
            using (var db = new PersistenceContext())
            {
                Transaction updated = db.Transactions
                    .FirstOrDefault(t => t.TransactionId == transactionID);
                if (updated == null) return null;
                updated.ProcessingTransactionId = processingTransactionID;
                updated.State = state;
                updated.Status = status;
                CommitChanges(db, updated);
                return updated;
            }
        }

        public static Transaction UpdateTransaction(
            string transactionID, 
            TransactionState state,
            TransactionStatus status)
        {
            using (var db = new PersistenceContext())
            {
                Transaction updated = db.Transactions
                    .FirstOrDefault(t => t.TransactionId == transactionID);
                if (updated == null) return null;
                updated.State = state;
                updated.Status = status;
                CommitChanges(db, updated);
                return updated;
            }
        }

        public static Transaction FindByTransactionId(string transactionID)
        {
            using (var db = new PersistenceContext())
            {
                return db.Transactions
                    .SingleOrDefault(t => t.TransactionId == transactionID);
            }
        }

        /*        
                    /// <summary>
                    /// Assign ID of transaction for return to Merchants
                    /// </summary>
                    /// <returns>unique string that pass to Merchant as paynet transaction ID</returns>
                    public Transaction CreateNewTransaction(TransactionType type)
                    {
                        return new Transaction(type);
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
                            var tr = db.Transactions
                                .FirstOrDefault(t => t.TransactionId == transID);
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
                                .FirstOrDefault(t => t.TransactionId == transID);
                            return (Guid) tr.ProcessingTransactionId;
                        }
                    }
                    */
        }

    public class MerchantCallbackStorage
    {
        private static void CommitChanges(PersistenceContext db, MerchantCallback callback)
        {
            callback.LastModified = DateTime.UtcNow;
            db.SaveChanges();
        }

        public static MerchantCallback FindByTransactionId(string transactionID)
        {
            using (var db = new PersistenceContext())
            {
                return db.MerchantCallbacks
                    .SingleOrDefault(c => c.TransactionId == transactionID);
            }
        }

        public static MerchantCallback UpdateMerchantCallback(
            MerchantCallback updated,
            CallbackState state,
            string stateReason)
        {
            using (var db = new PersistenceContext())
            {
                updated.State = state;
                updated.StateReason = stateReason;
                if (state == CallbackState.Delivered)
                {
                    updated.NextAttemptTime = null;
                    updated.StateReason = "";
                }
                else if (state == CallbackState.Repeating || state == CallbackState.Failed)
                {
                    CallbackDeliveryService.AdjustNextAttempt(updated, stateReason);
                }
                CommitChanges(db, updated);
                return updated;
            }
        }
    }
}