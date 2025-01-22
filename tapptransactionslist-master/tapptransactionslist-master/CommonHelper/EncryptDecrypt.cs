//$Author: Anilkumar.ko $
//$Date: 12/05/17 6:26p $
//$Header: /Mobileapps/Operator/Davinci/TS/TAppTransactionsList/TAppTransactionsList/CommonHelper/EncryptDecrypt.cs 1     12/05/17 6:26p Anilkumar.ko $
//$Modtime: 8/27/15 10:29a $
//$Revision: 1 $
//Copyright @2012 IMImobile Pvt. Ltd.

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace CommonHelper
{
    public class EncryptDecrypt
    {
        private byte[] IV = new byte[8];
        private string m_key = "IMI@1234";

        public EncryptDecrypt()
        {
            IV[0] = 18;
            IV[1] = 52;
            IV[2] = 86;
            IV[3] = 120;
            IV[4] = 144;
            IV[5] = 171;
            IV[6] = 205;
            IV[7] = 239;
        }

        public string Decrypt(string textToDecrypt, string keyVal)
        {
            if (keyVal == string.Empty)
                keyVal = m_key;
            byte[] inputByreArray = new byte[textToDecrypt.Length];
            try
            {
                byte[] key = Encoding.UTF8.GetBytes(keyVal.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByreArray = Convert.FromBase64String(textToDecrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByreArray, 0, inputByreArray.Length);
                cs.FlushFinalBlock();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch
            {
                return string.Empty;
            }
        }

        public string Encrypt(string textToEncrypt, string keyVal)
        {
            try
            {
                if (keyVal == string.Empty)
                    keyVal = m_key;
                byte[] key = Encoding.UTF8.GetBytes(keyVal.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByreArray = Encoding.UTF8.GetBytes(textToEncrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByreArray, 0, inputByreArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
