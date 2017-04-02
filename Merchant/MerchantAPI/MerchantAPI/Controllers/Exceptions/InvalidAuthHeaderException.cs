using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MerchantAPI.Controllers.Exceptions
{
    public class InvalidAuthHeaderException : ArgumentException
    {
        public InvalidAuthHeaderException() : base()
        {
        }

        public InvalidAuthHeaderException(string message) : base(message)
        {
        }

        public InvalidAuthHeaderException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}