using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace MerchantAPI.Data
{
    public enum TransactionState
    {
        Created,
        Redirected,
        Active,
        Finished,
    }

    public enum TransactionStatus
    {
        Approved,
        Declined,
        Undefined,
    }

    public enum TransactionType
    {
        Sale,
        Verify,
        Preauth,
        Capture,
        Return,
        Void,
    }

    public class Transaction
    {
        public Transaction(TransactionType type)
        {
            // ID =
            TransactionId = Guid.NewGuid();
            SerialNumber = Guid.NewGuid();
            // ProcessingTransactionId =
            Type = type;
            State = TransactionState.Created;
            Status = TransactionStatus.Undefined;
            LastModified = DateTime.UtcNow;
            // ReferenceQuery =
        }

        public int ID { get; set; }
        public Guid TransactionId { get; set; }
        public Guid SerialNumber { get; set; }
        public Guid? ProcessingTransactionId { get; set; }  //  The '?' after the Guid type declaration indicates that the property is NULLABLE
        public TransactionType Type { get; set; }
        public TransactionState State { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime LastModified { get; set; }
        public string ReferenceQuery { get; set; }
    }
}