using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using CommonHelper;
using System.Collections;

/// <summary>
/// Summary description for ServiceHelper
/// </summary>
public class ServiceHelper
{
    public static string SendSMSInUniCodeFormat(String strUrl, String strServiceKey, String strSenderID, String strMessage, String strMobileNo)
    {
       
        
        String strResult = String.Empty;
        if (string.IsNullOrEmpty(strMessage) || string.IsNullOrEmpty(strUrl) || string.IsNullOrEmpty(strMobileNo))
        {
            General.WriteLog("SENDUNICODESMS_FAIL", "Invalid SMS content:strMobileNo" + strMobileNo + ",strMessage:" + strMessage);
            strResult = "INVALID SMS CONTENT.";
            return strResult;
        }

        long lTimeTaken = -1, lStart = DateTime.Now.Ticks;
        XDocument xDoc;

        String strMobileNumber = String.Empty;
        if (strMobileNo.Length > 10)
            strMobileNumber = strMobileNo.Substring(strMobileNo.Length - 10);
        else
            strMobileNumber = strMobileNo;

        Hashtable htHeaders = new Hashtable();
        APIHelper objAPIHelper = new APIHelper();

        String strResponse = String.Empty;
        String strMainContent = String.Empty;
        try
        {

            strUrl = strUrl.Replace("{senderAddress}", HttpUtility.UrlEncode(strSenderID));
            strMainContent = "{\"outboundSMSMessageRequest\":{\"address\":[\"tel:!address!\"],\"senderAddress\":\"tel:!sendername!\",\"outboundSMSTextMessage\":{\"message\":\"!message!\"},\"clientCorrelator\":\"\",\"receiptRequest\": {\"notifyURL\":\"\",\"callbackData\":\"$(callbackData)\"} ,\"messageType\":\"4\",\"senderName\":\"\"}}";
            strMainContent = strMainContent.Replace("!address!", strMobileNumber);
            strMainContent = strMainContent.Replace("!sendername!", HttpUtility.UrlEncode(strSenderID));
            strMainContent = strMainContent.Replace("!key!", strServiceKey);
            strMainContent = strMainContent.Replace("!message!", strMessage);
            //General.WriteLog("SENDUNICODESMS", " strMainContent:" + strMainContent);
            htHeaders.Add("key", strServiceKey);

            strResponse = General.DoHttpWebRequest(strUrl, strMainContent, "POST", "application/json", htHeaders, General.GetConfigVal("SMS_SEND_TIMEOUT").ToInt(), General.GetConfigVal("HTTP_REQ_KEEPALIVE").ToBool());
            // strResponse = General.DoRequest(strUrl, strMainContent, "POST", "application/json", htHeaders);
            if (lStart > 0)
                lTimeTaken = (DateTime.Now.Ticks - lStart) / 10000;

            if (strResponse.Trim().Length > 0 && strResponse.ToLower().Contains("outboundsmsmessagerequest"))
            {
                xDoc = new XDocument(new XElement("root",
                                new XElement("code", "0"),
                                new XElement("desc", "success")
                                ));
                strResult = "0$^*" + xDoc.ToString();
            }
            else
            {
                General.WriteLog("SENDUNICODESMS_FAIL", "Timetaken:" + lTimeTaken.ToStr() + ", strResponse:" + strResponse);
                strResult = APIHelper.GetMessage("057010");
            }
        }
        catch (Exception ex)
        {
            General.WriteLog("SENDUNICODESMS_EX", "Exp:" + ex.Message + ", Timetaken:" + lTimeTaken.ToStr() + ", InnerExp:" + ex.InnerException);
            strResult = APIHelper.GetMessage("057010");
        }
        finally
        {
            xDoc = null;
            objAPIHelper = null;
           // HttpContext.Current.Response.AppendToLog("&url=" + strUrl + "&tt=" + lTimeTaken.ToString());
            General.WriteLog("SENDUNICODESMS_TIME", "Mobile No:" + strMobileNo + ",Timetaken:" + lTimeTaken.ToStr());
        }
        return strResult;
    }
}
