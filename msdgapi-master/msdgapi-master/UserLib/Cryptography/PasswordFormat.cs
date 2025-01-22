#region  UsingDirectives
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using IMI.SqlWrapper;
#endregion

namespace User.Cryptography
{
    public class PasswordFormat
    {
        #region GlobalDeclarations
        private byte[] IV = new byte[8];
        private string m_key = "@$%*&!#$";
        Dictionary<string, string> dicPwd = null;
        #endregion

        #region Constructor
        public PasswordFormat()
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
        #endregion

        #region Methods

        public enum FORMATTYPE
        {
            NORMAL,
            HASH,
            ENCRYPT
        }

        /// <summary>
        /// Returns the password based on the settings
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string GetPassword(string text)
        {
            string paSSword = string.Empty;
            try
            {
                string _strPasswordType = clGeneral.GetConfigValue("PASSWORD_TYPE");
                if (_strPasswordType == string.Empty || _strPasswordType == "NORMAL")
                    paSSword = GetPassword(text, PasswordFormat.FORMATTYPE.NORMAL);
                else if (_strPasswordType == "HASH")
                    paSSword = GetPassword(text, PasswordFormat.FORMATTYPE.HASH);
                else if (_strPasswordType == "ENCRYPT")
                    paSSword = GetPassword(text, PasswordFormat.FORMATTYPE.ENCRYPT);

            }
            catch (Exception ex)
            {
            }
            return paSSword;
        }

        /// <summary>
        /// Returns Password based on the type of Algorithm Passed    
        /// </summary>
        /// <param name="text">text to be modified</param>
        /// <param name="type">type of algorithm NORMAL/HASH/ENCRYPT</param>
        /// <returns></returns>
        private string GetPassword(string text, FORMATTYPE type)
        {
            string passWord = string.Empty;

            if (type == FORMATTYPE.NORMAL)
                passWord = NormalData(text.Trim());
            if (type == FORMATTYPE.HASH)
                passWord = HashData(text.Trim());
            if (type == FORMATTYPE.ENCRYPT)
                passWord = Encrypt(text.Trim());

            return passWord;
        }

        /// <summary>
        /// Retunrs the Normal Text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string NormalData(string text)
        {
            try
            {
                return text.Replace("'", "''");
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Return the Text in hash Algorithm
        /// </summary>
        /// <param name="textTohash"></param>
        /// <returns></returns>
        public string HashData(string textTohash)
        {
            string paSSword = string.Empty;
            StringBuilder sbHash = new StringBuilder();
            MD5CryptoServiceProvider md5 = null;
            try
            {
                //byte[] pass = Encoding.UTF8.GetBytes(textTohash);
                //MD5 md5 = new MD5CryptoServiceProvider();
                //string paSSword = Encoding.UTF8.GetString(md5.ComputeHash(pass));
                //return paSSword;

                md5 = new MD5CryptoServiceProvider();
                byte[] data = System.Text.Encoding.ASCII.GetBytes(textTohash);
                data = md5.ComputeHash(data);
                for (int i = 0; i < data.Length; i++)
                    sbHash.Append(data[i].ToString("x2"));
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
            finally
            {
                md5.Clear();
            }

            paSSword = sbHash.ToString();
            return paSSword;
        }

        /// <summary>
        /// Returns the text in Encrypted Format
        /// </summary>
        /// <param name="textToEncrypt"></param>
        /// <returns></returns>
        public string Encrypt(string textToEncrypt)
        {

            try
            {
                if (string.IsNullOrEmpty(textToEncrypt))
                    return string.Empty;

                byte[] key = Encoding.UTF8.GetBytes(m_key.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByreArray = Encoding.UTF8.GetBytes(textToEncrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByreArray, 0, inputByreArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the text in Decrypted Format
        /// </summary>
        /// <param name="textToEncrypt"></param>
        /// <returns></returns>
        public string Decrypt(string textToDecrypt)
        {
            try
            {
                if (string.IsNullOrEmpty(textToDecrypt))
                    return string.Empty;

                byte[] inputByreArray = new byte[textToDecrypt.Length];
                byte[] key = Encoding.UTF8.GetBytes(m_key.Substring(0, 8));

                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByreArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByreArray, 0, inputByreArray.Length);
                cs.FlushFinalBlock();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the text in Encrypted Format
        /// </summary>
        /// <param name="textToEncrypt"></param>
        /// <returns></returns>
        public string Encrypt(string textToEncrypt, string secKey)
        {

            try
            {
                if (string.IsNullOrEmpty(textToEncrypt))
                    return string.Empty;

                byte[] key = Encoding.UTF8.GetBytes(secKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByreArray = Encoding.UTF8.GetBytes(textToEncrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByreArray, 0, inputByreArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the text in Decrypted Format
        /// </summary>
        /// <param name="textToEncrypt"></param>
        /// <returns></returns>
        public string Decrypt(string textToDecrypt, string secKey)
        {
            try
            {
                if (string.IsNullOrEmpty(textToDecrypt))
                    return string.Empty;

                byte[] inputByreArray = new byte[textToDecrypt.Length];
                byte[] key = Encoding.UTF8.GetBytes(secKey.Substring(0, 8));

                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByreArray = Convert.FromBase64String(textToDecrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByreArray, 0, inputByreArray.Length);
                cs.FlushFinalBlock();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Generate random password based on the password policy
        /// </summary>
        /// <returns></returns>
        public string GetRandomPassword()
        {
            string randomPassword = string.Empty;

            try
            {
                dicPwd = new Dictionary<string, string>();
                StringBuilder pwdFormat = new StringBuilder();
                DataTable dtPasswordPolicies = GetPasswordPolicy();

                if (dtPasswordPolicies != null && dtPasswordPolicies.Rows.Count > 0)
                {
                    if (dtPasswordPolicies.Rows[0]["active"].ToString().Trim().ToUpper() == "N")
                    {
                        return RandomPassword();
                    }

                    #region VALIDATIONS
                    int charGrpCnt = 0;
                    string grpSelected = string.Empty;

                    bool grpCaps = false;
                    bool grpSmalls = false;
                    bool grpNums = false;
                    bool grpSpl = false;

                    string policyId = dtPasswordPolicies.Rows[0]["PolicyID"].ToString();
                    int pwdLength = Int32.Parse(dtPasswordPolicies.Rows[0]["MinLength"].ToString());
                    string prevMatches = dtPasswordPolicies.Rows[0]["NotInPrevMatches"].ToString();

                    grpSelected = dtPasswordPolicies.Rows[0]["GrpCaps"].ToString();
                    if (grpSelected.Trim().ToUpper() == "Y")
                    {
                        charGrpCnt++;
                        grpCaps = true;
                    }

                    grpSelected = dtPasswordPolicies.Rows[0]["GrpSmalls"].ToString();
                    if (grpSelected.Trim().ToUpper() == "Y")
                    {
                        charGrpCnt++;
                        grpSmalls = true;
                    }
                    grpSelected = dtPasswordPolicies.Rows[0]["GrpNums"].ToString();
                    if (grpSelected.Trim().ToUpper() == "Y")
                    {
                        charGrpCnt++;
                        grpNums = true;
                    }

                    grpSelected = dtPasswordPolicies.Rows[0]["GrpSplChars"].ToString();
                    if (grpSelected.Trim().ToUpper() == "Y")
                    {
                        charGrpCnt++;
                        grpSpl = true;
                    }
                    #endregion

                    int generateChars = Int32.Parse(Math.Round((decimal)(pwdLength / charGrpCnt)).ToString());
                    if ((generateChars * charGrpCnt) < pwdLength)
                    {
                        generateChars++;
                    }
                    if (grpCaps)
                    {
                        GeneratePasswordFormat("CAPS", generateChars, ref randomPassword);
                    }
                    if (grpSmalls)
                    {
                        GeneratePasswordFormat("SMALLS", generateChars, ref randomPassword);
                    }
                    if (grpNums)
                    {
                        GeneratePasswordFormat("NUMS", generateChars, ref randomPassword);
                    }
                    if (grpSpl)
                    {
                        GeneratePasswordFormat("SPLS", generateChars, ref randomPassword);
                    }

                    //This Code jumbles the word
                    Random num = new Random();
                    string rand = new string(randomPassword.ToCharArray().OrderBy(s => (num.Next(2) % 2) == 0).ToArray());
                    randomPassword = rand;
                    num = null;
                }
            }
            catch (Exception ex)
            {

            }

            return randomPassword;
        }

        private void GeneratePasswordFormat(string type, int cnt, ref string pwdFormate)
        {
            int fromAscii;
            int toAscii;
            Random objRandom = new Random();
            switch (type)
            {
                case "CAPS":
                    fromAscii = 65;
                    toAscii = 90;
                    string tmp = string.Empty;
                    while (tmp.Length < cnt)
                    {
                        int asciiCnt = objRandom.Next(fromAscii, toAscii);
                        char c = (char)asciiCnt;
                        if (!dicPwd.ContainsKey(c.ToString().ToUpper()))
                        {
                            pwdFormate += c.ToString();
                            dicPwd.Add(c.ToString().ToUpper(), c.ToString().ToUpper());
                            tmp += c.ToString();
                        }
                    }
                    break;
                case "SMALLS":
                    fromAscii = 97;
                    toAscii = 122;
                    tmp = string.Empty;
                    while (tmp.Length < cnt)
                    {
                        int asciiCnt = objRandom.Next(fromAscii, toAscii);
                        char c = (char)asciiCnt;
                        if (!dicPwd.ContainsKey(c.ToString().ToUpper()))
                        {
                            pwdFormate += c.ToString();
                            dicPwd.Add(c.ToString().ToUpper(), c.ToString().ToUpper());
                            tmp += c.ToString();
                        }
                    }
                    break;
                case "NUMS":
                    fromAscii = 48;
                    toAscii = 57;
                    tmp = string.Empty;
                    while (tmp.Length < cnt)
                    {
                        int asciiCnt = objRandom.Next(fromAscii, toAscii);
                        char c = (char)asciiCnt;
                        if (!dicPwd.ContainsKey(c.ToString()))
                        {
                            pwdFormate += c.ToString();
                            dicPwd.Add(c.ToString(), c.ToString());
                            tmp += c.ToString();
                        }
                    }
                    break;
                case "SPLS":
                    string splChars = clGeneral.GetConfigValue("PWD_SPL_CHARS"); //"@,#,$,*,!,^,~";
                    string[] splArray = splChars.Trim().TrimEnd(',').Split(',');
                    for (int i = 0; i < cnt; i++)
                    {
                        if (cnt < splArray.Length)
                        {
                            pwdFormate += splArray[i];
                        }
                        else
                        {
                            if (i < splArray.Length)
                            {
                                pwdFormate += splArray[i];
                            }
                            else
                            {
                                pwdFormate += splArray[0];
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// generates random password
        /// </summary>
        /// <returns></returns>
        private string RandomPassword()
        {
            try
            {
                //char[] arrPossibleChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@$%^^*()_-+=[{]};:>|./?".ToCharArray();
                string possChar = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" + clGeneral.GetConfigValue("PWD_SPL_CHARS").Replace(",", string.Empty);
                char[] arrPossibleChars = possChar.ToCharArray();
                int passwordLength = 8;
                string passWord = null;
                System.Random rand = new Random();
                int i = 1;
                for (i = 1; i <= passwordLength; i++)
                {
                    int ranDom = rand.Next(arrPossibleChars.Length);
                    passWord = passWord + arrPossibleChars[ranDom].ToString();
                }
                return passWord;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

       

        public DataTable GetPasswordPolicy()
        {

            DataTable objDt_Ret = null;
            try
            {
                using (DBFactory oDbFactory = new DBFactory())
                {
                    oDbFactory.RunProc("UCA_GET_PWDPOLICY", out objDt_Ret);
                }
            }
            catch (Exception Ex)
            {
            }

            return objDt_Ret;

        }

        /// <summary>
        /// password username check
        /// </summary>
        /// <param name="Src"></param>
        /// <param name="Dest"></param>
        /// <param name="check_len"></param>
        /// <param name="strOutMsg"></param>
        /// <returns></returns>
        public static bool IsNotSequentialChars(string Src, string Dest, int check_len, out string strOutMsg)
        {
            strOutMsg = string.Empty;
            if (check_len < 1 || Src.Length < check_len) return true;
            Match m = Regex.Match(Src, "(?=(.{" + check_len + "})).");
            bool bOK = m.Success;

            while (bOK && m.Success)
            {
                // Edit: remove unnecessary '.*' from regex.
                // And btw, is regex needed at all in this case?
                bOK = !Regex.Match(Dest, "(?i)" + Regex.Escape(m.Groups[1].Value)).Success;
                if (!bOK)
                {
                    //Console.WriteLine("Destination contains " + check_len + " sequential source letter(s) '" + m.Groups[1].Value + "'");
                    strOutMsg = "Destination contains " + check_len + " sequential source letter(s) '" + m.Groups[1].Value + "'";
                }
                m = m.NextMatch();
            }
            return bOK;
        }
        #endregion
    }
}