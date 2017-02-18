using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MerchantAPI.Data
{
    public class TransactionData
    {
        public enum TransactionState {
            Undefined = 0,
            Started = 1,
            Redirected = 2,
            Finished = 3,
        };

        public string fibonatixTransactionID { set; get; }
        public string merchantTransactionID { set; get; }
        public string commdooTransactionID { set; get; }
        public string transactionType { set; get; }
        public TransactionState transactionState { set; get; }
    }

    public class TransactionsDataStorage
    {
        private static void insertDataToDB(TransactionData newData) {
            HttpContext.Current.Cache.Insert("TransactionData:" + newData.fibonatixTransactionID, newData, null,
                System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0, 0));
        }
        private static TransactionData loadDataFromDB(string fibonatixTransactionID) {
            TransactionData data = (TransactionData)HttpContext.Current.Cache.Get("TransactionData:" + fibonatixTransactionID);
            return data;
        }

        public static TransactionData setNewFibonatixTransaction(string merchantTransactionID, string type) {
            TransactionData newData = new TransactionData();
            newData.fibonatixTransactionID = Guid.NewGuid().ToString();
            newData.merchantTransactionID = merchantTransactionID;
            newData.transactionType = type;
            newData.transactionState = TransactionData.TransactionState.Undefined;
            insertDataToDB(newData);
            return newData;
        }
        public static TransactionData setRealTransactionID(string fibonatixTransactionID, string commdooTransactionID) {
            TransactionData data = loadDataFromDB(fibonatixTransactionID);
            data.commdooTransactionID = commdooTransactionID;
            insertDataToDB(data);
            return data;
        }
        public static TransactionData setTransactionState(string fibonatixTransactionID, TransactionData.TransactionState state) {
            TransactionData data = loadDataFromDB(fibonatixTransactionID);
            data.transactionState = state;
            insertDataToDB(data);
            return data;
        }
        public static TransactionData getTransactionData(string fibonatixTransactionID) {
            TransactionData data = loadDataFromDB(fibonatixTransactionID);
            return data;
        }
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