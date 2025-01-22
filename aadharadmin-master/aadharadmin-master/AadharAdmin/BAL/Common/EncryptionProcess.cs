using IMI.Logger;
using System;
using System.Security.Cryptography;
using System.Text;

namespace AadharAdmin.BAL.Common
{
    public class EncryptionProcess
    {
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
                LogData.Write("IsValidSignature", "LOGIN", LogMode.Excep, "ex " + ex.Message);
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
            catch (Exception ex)
            {
                LogData.Write("GetSHA512Hash", "LOGIN", LogMode.Excep, "ex " + ex.Message);
            }
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
                LogData.Write("GenerateSignature", "LOGIN", LogMode.Excep, "ex " + ex.Message);
            }

            return "";
        }
        #region This is to Geneate Captcha Images
        public static string GenerateRandomCode()
        {
            Random r = new Random();
            string s = "";
            for (int j = 0; j < 5; j++)
            {
                int i = r.Next(1, 3);
                int ch;
                switch (i)
                {
                    case 1:
                        ch = r.Next(1, 9);
                        s = s + ch.ToString();
                        break;
                    case 2:
                        ch = r.Next(65, 90);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                    case 3:
                        ch = r.Next(65, 90);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                    default:
                        ch = r.Next(1, 9);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                }
                r.NextDouble();
                r.Next(100, 1999);
            }
            return s;
        }

        #endregion
    }
}