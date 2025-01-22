using IMI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace AadharAdmin.BAL.Common
{
    public class RC4
    {
        private const int N = 256;
        private int[] sbox;
        public string Password { get; set; }
        public string Text { get; set; }

        public RC4(string password, string text)
        {
            this.Password = password;
            this.Text = text;
        }

        public RC4(string text)
        {
            this.Password = General.GetConfigVal("RC4_KEY");
            this.Text = text;
        }

        public String Encrypt()
        {
            return StrToHexStr(EnDeCrypt(Text));
        }

        public String Decrypt()
        {
            return EnDeCrypt(HexStrToStr(Text));
        }

        public string EnDeCrypt()
        {
            RC4Initialize();
            int i = 0, j = 0, k = 0;
            StringBuilder cipher = new StringBuilder();
            for (int a = 0; a < Text.Length; a++)
            {
                i = (i + 1) % N;
                j = (j + sbox[i]) % N;
                int tempSwap = sbox[i];
                sbox[i] = sbox[j];
                sbox[j] = tempSwap;

                k = sbox[(sbox[i] + sbox[j]) % N];
                int cipherBy = ((int)Text[a]) ^ k;
                cipher.Append(Convert.ToChar(cipherBy));
            }
            return cipher.ToString();
        }

        public string EnDeCrypt(String text)
        {
            RC4Initialize();
            int i = 0, j = 0, k = 0;
            StringBuilder cipher = new StringBuilder();
            for (int a = 0; a < text.Length; a++)
            {
                i = (i + 1) % N;
                j = (j + sbox[i]) % N;
                int tempSwap = sbox[i];
                sbox[i] = sbox[j];
                sbox[j] = tempSwap;

                k = sbox[(sbox[i] + sbox[j]) % N];
                int cipherBy = ((int)text[a]) ^ k;
                cipher.Append(Convert.ToChar(cipherBy));
            }
            return cipher.ToString();
        }

        public static string StrToHexStr(string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                int v = Convert.ToInt32(str[i]);
                sb.Append(string.Format("{0:X2}", v));
            }
            return sb.ToString();
        }

        public static string HexStrToStr(string hexStr)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hexStr.Length; i += 2)
            {
                int n = Convert.ToInt32(hexStr.Substring(i, 2), 16);
                sb.Append(Convert.ToChar(n));
            }
            return sb.ToString();
        }

        private void RC4Initialize()
        {
            sbox = new int[N];
            int[] key = new int[N];
            int n = Password.Length;
            for (int a = 0; a < N; a++)
            {
                key[a] = (int)Password[a % n];
                sbox[a] = a;
            }

            int b = 0;
            for (int a = 0; a < N; a++)
            {
                b = (b + sbox[a] + key[a]) % N;
                int tempSwap = sbox[a];
                sbox[a] = sbox[b];
                sbox[b] = tempSwap;
            }
        }
    }
}