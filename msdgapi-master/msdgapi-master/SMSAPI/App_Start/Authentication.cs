using System.Security.Cryptography;
using System.Text;

namespace SMSAPI
{
    public class Authentication
    {
        public static string GetSHA512Hash(string input)
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
    }
}
