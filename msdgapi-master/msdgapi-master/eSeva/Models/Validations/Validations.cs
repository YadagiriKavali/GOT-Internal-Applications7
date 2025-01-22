using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace eSeva.Models.Validations
{
    public class Validations
    {
        public static bool IsValidMobileNumber(string mobilNo)
        {
            string mobileNoPattern = "^([0-9]{10})$";
            var result = Regex.Match(mobilNo, mobileNoPattern);
            return result.Success;
        }

        public static bool IsValidLandLineNo(string landLineNo)
        {
            string landLineNoPattern = "^([0-9]{10})$";
            var result = Regex.Match(landLineNo, landLineNoPattern);
            return result.Success;
        }

        public static bool IsValidWaterCanNo(string waterCanNo)
        {
            string waterCanNoPattern = "^([0-9]{9})$";
            var result = Regex.Match(waterCanNo, waterCanNoPattern);
            return result.Success;
        }
    }
}
