using System.Text.RegularExpressions;

namespace eseva.Validations
{
    public class Validations
    {
        public static bool IsValidMobileNumber(string mobilNo)
        {
            if (string.IsNullOrEmpty(mobilNo))
                return false;

            var mobileNoPattern = "^([0-9]{10})$";
            var result = Regex.Match(mobilNo, mobileNoPattern);
            return result.Success;
        }

        public static bool IsValidLandLineNo(string landLineNo)
        {
            if (string.IsNullOrEmpty(landLineNo))
                return false;

            var landLineNoPattern = "^([0-9]{10})$";
            var result = Regex.Match(landLineNo, landLineNoPattern);
            return result.Success;
        }

        public static bool IsValidWaterCanNo(string waterCanNo)
        {
            if (string.IsNullOrEmpty(waterCanNo))
                return false;

            var waterCanNoPattern = "^([0-9]{9})$";
            var result = Regex.Match(waterCanNo, waterCanNoPattern);
            return result.Success;
        }
    }
}
