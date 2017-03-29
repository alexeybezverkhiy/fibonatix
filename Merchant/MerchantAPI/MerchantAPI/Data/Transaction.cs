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
        Undefined = 0,
        Created = 1,
        Started = 2,
        Redirected = 3,
        Finished = 20,
        Failed = 30,
    }

    public enum TransactionStatus
    {
        Undefined = 0,
        Approved = 1,
        Declined = 2,
        Error = 100,
    }

    public enum TransactionType
    {
        Sale = 0,
        Verify = 1,
        Preauth = 2,
        Capture = 3,
        Return = 4,
        Void = 5,
        SaleForm = 6,
        PreauthForm = 7,
    }

    public class Transaction
    {
        [Key]
        public int ID { get; set; }
        [Index("TransactionTransactionId_UIDX", IsUnique = true, Order = 100)]
        [StringLength(36)]   // 36 = size of Guid type
        public string TransactionId { get; set; }
        [StringLength(36)]   // 36 = size of Guid type
        public string SerialNumber { get; set; }
        [StringLength(48)]
        public string ProcessingTransactionId { get; set; }
        [StringLength(48)]
        public string MerchantTransactionId { get; set; }
        [Index("TransactionTransactionId_UIDX", IsUnique = true, Order = 200)]
        [Index("TransactionType_IDX")]
        public TransactionType Type { get; set; }
        [Index("TransactionState_IDX")]
        public TransactionState State { get; set; }
        [StringLength(128)]
        public string StateReason { get; set; }
        [Index("TransactionStatus_IDX")]
        public TransactionStatus Status { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime CreationTime { get; set; }
        public string ReferenceQuery { get; set; }
        public string RedirectUri { get; set; }

        public Transaction() {}

        public Transaction(TransactionType type)
        {
            // ID =
            TransactionId = Guid.NewGuid().ToString();
            SerialNumber = Guid.NewGuid().ToString();
            // ProcessingTransactionId =
            // MerchantTransactionId =
            Type = type;
            State = TransactionState.Created;
            Status = TransactionStatus.Undefined;
            LastModified = DateTime.UtcNow;
            CreationTime = DateTime.UtcNow;
            // ReferenceQuery =
        }

        public Transaction(TransactionType type, string merchantTransactionID) : this(type)
        {
            MerchantTransactionId = merchantTransactionID;
        }
    }

    public static class StringExtensions
    {
        public static T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}