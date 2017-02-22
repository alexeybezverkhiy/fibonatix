using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Net;
using Genesis.Net.Common;
using Genesis.Net.Entities;
using Genesis.Net.Errors;

namespace Fibonatix.CommDoo.Helpers
{
    public class Convertors
    {
        public static decimal MinorAmountToMajor(int minor, Iso4217CurrencyCodes code) {
            decimal majorAmount;
            bool res = Iso4217Currencies.TryConvertMinorToMajor(code, (int)minor, out majorAmount);
            if (!res)
                throw new System.ComponentModel.DataAnnotations.ValidationException("Invalid Amount or Currency section, cannot convert from minor to major amount").SetCode((int)ErrorCodes.InputDataInvalidError);
            return majorAmount;
        }
        public static int MajorAmountToMinor(decimal major, Iso4217CurrencyCodes code) {
            int minorAmount;
            bool res = Iso4217Currencies.TryConvertMajorToMinor(code, major, out minorAmount);
            if (!res)
                throw new System.ComponentModel.DataAnnotations.ValidationException("Invalid Amount or Currency section, cannot convert from minor to major amount").SetCode((int)ErrorCodes.InputDataInvalidError);
            return minorAmount;
        }

    }
}
