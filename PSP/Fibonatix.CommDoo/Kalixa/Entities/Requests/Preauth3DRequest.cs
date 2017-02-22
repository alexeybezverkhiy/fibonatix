﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;
using Genesis.Net.Errors;
using Fibonatix.CommDoo.Helpers;
using System.Globalization;

namespace Fibonatix.CommDoo.Kalixa.Entities.Requests
{
    [Serializable()]
    [XmlRoot("initiatePaymentRequest", Namespace = "http://www.cqrpayments.com/PaymentProcessing", IsNullable = true)]
    [XmlInclude(typeof(Request.keyStringValuePair))]
    public class Preauth3DRequest : Request
    {
        public override string getAPIPath() {
            throw new System.ComponentModel.DataAnnotations.ValidationException("Kalixa aquirer doesn't support Preauthorize 3D request").SetCode((int)ErrorCodes.InvalidTransactionTypeError);
        }
    }
}
