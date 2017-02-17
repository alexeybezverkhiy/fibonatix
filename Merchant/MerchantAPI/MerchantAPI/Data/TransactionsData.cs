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
    }
}