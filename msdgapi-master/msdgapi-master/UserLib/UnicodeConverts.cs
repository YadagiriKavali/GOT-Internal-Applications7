using System;
using System.Globalization;
using System.Text;

namespace User
{
	/// <summary>
	/// Summary description for UnicodeConverts.
	/// </summary>
	public class UnicodeConverts
	{
		public UnicodeConverts()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static string GetHexUnicode(string strHexa)
		{
			//byte[] btCheck = System.Text.UTF8Encoding.Unicode.GetBytes(strArabicCheck);

			byte[] Bytearray=new byte[strHexa.Length/2];  						
			int j = 0;
			for(int i=0;i<strHexa.Length;i+=4,j+=2)
			{				
				string hexNumber=strHexa.Substring(i,4);
				int iTemp= Int32.Parse(hexNumber.Substring(0,2), NumberStyles.HexNumber);
				Bytearray[j+1] =  byte.Parse(iTemp.ToString());
				iTemp= Int32.Parse(hexNumber.Substring(2,2), NumberStyles.HexNumber);
				Bytearray[j] = byte.Parse(iTemp.ToString());				
			}		
			return System.Text.UTF8Encoding.Unicode.GetString(Bytearray);		
		}
        public static string GetSingleUnicodeHex(string strTextMsg)
        {
            byte[] s1 = UTF8Encoding.Unicode.GetBytes(strTextMsg);
            string strUnicode = "";
            string strTmp1 = "";
            string strTmp2 = "";

            for (int i = 0; i < s1.Length; i += 2)
            {
                strTmp1 = int.Parse(s1[i + 1].ToString()).ToString("X");
                if (strTmp1.Length == 1)
                    strTmp1 = "0" + strTmp1;

                strTmp2 = int.Parse(s1[i].ToString()).ToString("X");
                if (strTmp2.Length == 1)
                    strTmp2 = "0" + strTmp2;

                strUnicode += strTmp1 + strTmp2;
            }
            return strUnicode;
        }
		public static string GetUnicodeHex(string strTextMsg)
		{
			//convert string into byes 
			
			byte[] s1= UTF8Encoding.Unicode.GetBytes(strTextMsg);			
			string strUnicode="";
			string _strCon="";
			string strTmp1 = "";
			string strTmp2 = "";
			
			for(int i=0; i<s1.Length;  i+=2)
			{
				strTmp1 = int.Parse(s1[i+1].ToString()).ToString("X");
				if (strTmp1.Length == 1)
					strTmp1 = "0" + strTmp1;

				strTmp2 = int.Parse(s1[i].ToString()).ToString("X");
				if (strTmp2.Length == 1)
					strTmp2 = "0" + strTmp2;
			    	
				strUnicode += strTmp1  + strTmp2;	
			}

			for(int i=0;i < strUnicode.Length; i=i+256)
			{	
				if( (i + 256) <= strUnicode.Length)
				{
					_strCon += strUnicode.Substring(i,256) + "|"; 
				}
				else
				{
					_strCon += strUnicode.Substring(i); 
				}
			}
			

			Array arrMsg = _strCon.Split('|');
			int iCount = arrMsg.Length;

			Random endVal = new Random();
			int iRet = endVal.Next(1,254);
			
			string strHexVal = iRet.ToString("x").ToUpper();
			if (strHexVal.Length == 1)
				strHexVal = "0" + strHexVal;

			string strHeader = "050003" + strHexVal + "0" + iCount.ToString() + "0";
			strHexVal = "";
			for(int i = 1;i<=iCount;i++)
			{
				if (strHexVal == "")
					strHexVal = strHeader + i.ToString() +","+arrMsg.GetValue(i-1).ToString();
				else
					strHexVal = strHexVal + "|" + strHeader + i.ToString() +","+ arrMsg.GetValue(i-1).ToString();
			}
			arrMsg = null;

			return strHexVal;
		}
        public static string GetConcatHex(string strTextMsg)
        {
            //convert string into byes 

            byte[] s1 = System.Text.UTF8Encoding.UTF8.GetBytes(strTextMsg);
            string strUnicode = "";
            string _strCon = "";
            //string strTmp1 = "";
            string strTmp2 = "";

            for (int i = 0; i < s1.Length; i++)
            {
                strTmp2 = int.Parse(s1[i].ToString()).ToString("X");
                if (strTmp2.Length == 1)
                    strTmp2 = "0" + strTmp2;

                strUnicode += strTmp2;
            }

            for (int i = 0; i < strUnicode.Length; i = i + 300)
            {
                if ((i + 300) <= strUnicode.Length)
                {
                    _strCon += strUnicode.Substring(i, 300) + "|";
                }
                else
                {
                    _strCon += strUnicode.Substring(i);
                }
            }


            Array arrMsg = _strCon.Split('|');
            int iCount = arrMsg.Length;

            Random endVal = new Random();
            int iRet = endVal.Next(1, 254);

            string strHexVal = iRet.ToString("x").ToUpper();
            if (strHexVal.Length == 1)
                strHexVal = "0" + strHexVal;

            string strHeader = "050003" + strHexVal + "0" + iCount.ToString() + "0";
            strHexVal = "";
            for (int i = 1; i <= iCount; i++)
            {
                if (strHexVal == "")
                    strHexVal = strHeader + i.ToString() + "," + arrMsg.GetValue(i - 1).ToString();
                else
                    strHexVal = strHexVal + "|" + strHeader + i.ToString() + "," + arrMsg.GetValue(i - 1).ToString();
            }
            arrMsg = null;

            return strHexVal;
        }
	}

}
