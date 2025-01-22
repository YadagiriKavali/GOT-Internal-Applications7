using System.Security.Cryptography;
using System.Text;
using IMI.Logger;

namespace MSDGAPI.BL
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
                LogData.Write("MSDGAPI", "EncrptedData", LogMode.Debug, "PostText: " + PostText + "\nSignature Data: " + sSignatureData + "\n\n");
                if (string.Compare(receivedHash.ToLower(), sSignatureData.ToLower(), true) == 0)
                    return true;
            }
            catch { }

            return false;
        }

        #endregion Public Methods

        #region Private Methods

        private static string GetSHA512Hash(string input)
        {
            var sbSHA512Hash = new StringBuilder();
            SHA512Managed sha512 = null;
            try
            {
                sha512 = new SHA512Managed();
                byte[] data = System.Text.Encoding.UTF8.GetBytes(input);
                data = sha512.ComputeHash(data);
                for (int i = 0; i < data.Length; i++)
                    sbSHA512Hash.Append(data[i].ToString("x2"));
            }
            catch { }
            finally
            {
                sha512.Clear();
            }

            return sbSHA512Hash.ToString();
        }

        private static string GetOdd(string data)
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

        #endregion Private Methods
    }
}