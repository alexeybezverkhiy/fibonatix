using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using MerchantAPI.Models;

namespace MerchantAPI.Services
{
    public class NotificationService
    {

        public ServiceTransitionResult Notified(
            NotificationRequestModel model)
        {
            if (model.referenceid != null) {
                Data.TransactionsDataStorage.setRealTransactionID(model.fibonatixID, model.transactionid);
                Data.TransactionsDataStorage.setTransactionState(model.fibonatixID, Data.TransactionData.TransactionState.Finished);
            }

            if (model.customernotifyurl != null) {
                // TODO - send Notification to Customer
            }
            
            return new ServiceTransitionResult(HttpStatusCode.OK, "Notification received");
        }

    }
}