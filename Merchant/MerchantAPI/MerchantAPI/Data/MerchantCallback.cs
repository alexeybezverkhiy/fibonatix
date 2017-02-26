using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MerchantAPI.Data
{
    public enum CallbackState
    {
        Undefined = 0,
        Created = 1,
        Repeating = 2,
        Failed = 3,
        Delivered = 10,
        Cancelled = 20,
    }

    public class MerchantCallback
    {
        [Key]
        public int ID { get; set; }
        [Index("CallbackTransactionId_UIDX", IsUnique = true)]
        [StringLength(36)]   // 36 = size of Guid type
        public string TransactionId { get; set; }
        [Index("CallbackState_IDX")]
        public CallbackState State { get; set; }
        [StringLength(128)]
        public string StateReason { get; set; }
        public DateTime LastModified { get; set; }
        public int AttemptNo { get; set; }
        public DateTime CreationTime { get; set; }
        [Index("CallbackNextAttemptTime_IDX")]
        public DateTime? NextAttemptTime { get; set; }
        [StringLength(128)]
        public string CallbackUri { get; set; }
        public string CallbackQuery { get; set; }

        public MerchantCallback() {}

        public MerchantCallback(string transactionId, string callbackUri, string callbackQuery)
        {
            // ID = skip ID due to [Key]
            TransactionId = transactionId;
            State = CallbackState.Created;
            //StateReason =
            AttemptNo = 0;
            CreationTime = DateTime.UtcNow;
            LastModified = DateTime.UtcNow;
            // NextAttemptTime = 
            CallbackUri = callbackUri;
            CallbackQuery = callbackQuery;
        }
    }
}