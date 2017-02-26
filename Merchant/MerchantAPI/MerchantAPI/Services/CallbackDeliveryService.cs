using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MerchantAPI.Data;

namespace MerchantAPI.Services
{
    public class CallbackDeliveryService
    {
        private static TimeSpan DURATION_OF_DELIVERY = new TimeSpan(14, 0, 0, 0);
        private const int NUMBER_OF_DELIVERY_ATTEMPTS = 30;
        private static TimeSpan[] ATTEMPTS_ARRAY = new TimeSpan[1 + NUMBER_OF_DELIVERY_ATTEMPTS]
        {
            new TimeSpan(0, 0, 0, 0),   // just stub for ZERO-index
            new TimeSpan(0, 0, 2, 0),
            new TimeSpan(0, 0, 5, 0),
            new TimeSpan(0, 0, 10, 0),
            new TimeSpan(0, 0, 20, 0),
            new TimeSpan(0, 0, 30, 0),
            new TimeSpan(0, 1, 0, 0),
            new TimeSpan(0, 2, 0, 0),
            new TimeSpan(0, 3, 0, 0),
            new TimeSpan(0, 6, 0, 0),
            new TimeSpan(0, 12, 0, 0),
            new TimeSpan(0, 18, 0, 0),
            new TimeSpan(1, 0, 0, 0),
            new TimeSpan(2, 0, 0, 0),
            new TimeSpan(3, 0, 0, 0),
            new TimeSpan(4, 0, 0, 0),
            new TimeSpan(5, 0, 0, 0),
            new TimeSpan(6, 0, 0, 0),
            new TimeSpan(7, 0, 0, 0),
            new TimeSpan(8, 0, 0, 0),
            new TimeSpan(9, 0, 0, 0),
            new TimeSpan(10, 0, 0, 0),
            new TimeSpan(11, 0, 0, 0),
            new TimeSpan(11, 6, 0, 0),
            new TimeSpan(12, 0, 0, 0),
            new TimeSpan(12, 6, 0, 0),
            new TimeSpan(12, 12, 0, 0),
            new TimeSpan(13, 0, 0, 0),
            new TimeSpan(13, 6, 0, 0),
            new TimeSpan(13, 12, 0, 0),
            DURATION_OF_DELIVERY
        };

        public static MerchantCallback AdjustNextAttempt(MerchantCallback callback, string stateReason)
        {
            if (callback.AttemptNo == NUMBER_OF_DELIVERY_ATTEMPTS)
            {
                callback.State = CallbackState.Cancelled;
                string lastAttempt = callback.NextAttemptTime?.ToString("yyyy-MM-dd'T'HH:mm:ss.fff") 
                    ?? "<undefined>";
                callback.StateReason = $"Cancelled due to the last delivery attempt [{callback.AttemptNo}]" +
                                       $" occurred at [{lastAttempt} UTC]";
            }
            else
            {
                callback.AttemptNo++;
                callback.NextAttemptTime = callback.CreationTime + ATTEMPTS_ARRAY[callback.AttemptNo];
                callback.StateReason = stateReason;
            }
            return callback;
        }
    }
}