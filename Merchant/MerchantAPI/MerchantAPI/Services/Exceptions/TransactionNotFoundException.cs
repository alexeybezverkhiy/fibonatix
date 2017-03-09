using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MerchantAPI.Services.Exceptions
{
    public class TransactionNotFoundException : ArgumentException
    {
        public TransactionNotFoundException() : base()
        {
        }

        public TransactionNotFoundException(string message) : base(message)
        {
        }

        public TransactionNotFoundException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}