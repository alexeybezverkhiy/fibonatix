using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Globalization;

namespace MerchantAPI.Data
{
    public enum TransactionState
    {
        Created = 1,
        Started = 2,
        Redirected = 3,
        Finished = 20,
    }

    public enum TransactionStatus
    {
        Undefined = 0,
        Approved = 1,
        Declined = 2,
    }

    public enum TransactionType
    {
        Sale = 0,
        Verify = 1,
        Preauth = 2,
        Capture = 3,
        Return = 4,
        Void = 5,
    }

    public class Transaction
    {
        public Transaction()
        {
        }

        public Transaction(TransactionType type)
        {
            // ID =
            TransactionId = Guid.NewGuid();
            SerialNumber = Guid.NewGuid();
            // ProcessingTransactionId =
            // MerchantTransactionID =
            Type = type;
            State = TransactionState.Created;
            Status = TransactionStatus.Undefined;
            LastModified = DateTime.UtcNow;
            // ReferenceQuery =
        }

        [Key]
        public int ID { get; set; }
        public Guid TransactionId { get; set; }
        public Guid SerialNumber { get; set; }
        public Guid? ProcessingTransactionId { get; set; }  //  The '?' after the Guid type declaration indicates that the property is NULLABLE
        public string MerchantTransactionID { get; set; }
        public TransactionType Type { get; set; }
        public TransactionState State { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime LastModified { get; set; }
        public string ReferenceQuery { get; set; }
    }

    public static class StringExtensions
    {
        public static T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}