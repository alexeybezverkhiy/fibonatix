using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using MerchantAPI.Data;
using MerchantAPI.Models;

namespace MerchantAPI.Services
{
    public class NotificationService
    {

        public ServiceTransitionResult Notified(
            NotificationRequestModel model)
        {
            if (model.referenceid != null) {
                TransactionsDataStorage.UpdateTransaction(model.fibonatixID, model.transactionid, TransactionState.Finished);
                if(model.transactionstatus == "Charged" ||
                    model.transactionstatus == "Reserved" ||
                    model.transactionstatus == "ChargedBack" ||
                    model.transactionstatus == "ChargedBackReserved" ||
                    model.transactionstatus == "Refunded")
                    TransactionsDataStorage.UpdateTransactionStatus(model.fibonatixID, TransactionStatus.Approved);
                else
                    TransactionsDataStorage.UpdateTransactionStatus(model.fibonatixID, TransactionStatus.Declined);
            }

            if (model.customernotifyurl != null) {
                // TODO - send Notification to Customer
            }
            
            return new ServiceTransitionResult(HttpStatusCode.OK, "Notification received");
        }

    }
}