using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GetApplicationStatus
{
   public class EncryptionProcess
    {
        #region Public Methods

        public static bool IsValidSignature(string receivedHash, string PostText, string saltText)
        {
            try
            {
                string sAccessToken = GetOdd(saltText);
                string sBody = string.Format("REQBODY={0}&SALT={1}", PostText, sAccessToken);
                string sSignatureData = GetSHA512Hash(sBody);
                if (string.Compare(receivedHash.ToLower(), sSignatureData.ToLower(), true) == 0)
                    return true;
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        public static string GetSHA512Hash(string input)
        {
            var sbSHA512Hash = new StringBuilder();
            SHA512 sha512 = null;
            try
            {
                sha512 = SHA512.Create();
                byte[] data = System.Text.Encoding.UTF8.GetBytes(input);
                data = sha512.ComputeHash(data);
                for (int i = 0; i < data.Length; i++)
                    sbSHA512Hash.Append(data[i].ToString("x2"));
            }
            catch { }
            finally
            {
                sha512.Dispose();
            }

            return sbSHA512Hash.ToString();
        }

        public static string GetOdd(string data)
        {
            string retText = string.Empty;
            var charText = data.ToCharArray();

            for (int index = 1; index <= charText.Length; index++)
            {
                if (index % 2 != 0)
                    retText += charText[index - 1];
            }

            return retText;
        }

        public static string GenerateSignature(string PostText, string saltText)
        {
            try
            {
                string sAccessToken = GetOdd(saltText);
                string sBody = string.Format("REQBODY={0}&SALT={1}", PostText, sAccessToken);
                string sSignatureData = GetSHA512Hash(sBody);
                return sSignatureData;
            }
            catch (Exception ex)
            {
                //  Logger.Exception("Error occurred while validating data. \n Exception: " + ex.Message, "Consent");
            }

            return "";
        }

        #endregion Public Methods
    }
}
