using IMI.Helper;
using IMI.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for General
/// </summary>
public class clGeneral
{
    public static String GetConfigVal(String strKey)
    {
        try
        {
            if (ConfigurationManager.AppSettings[strKey] != null)
                return ConfigurationManager.AppSettings[strKey].ToString().Trim();
            return String.Empty;
        }
        catch
        {
            return String.Empty;
        }
    }

    public static string QueryString(string key)
    {
        if (HttpContext.Current.Request[key] != null
            && string.IsNullOrEmpty(HttpContext.Current.Request[key].ToString()) == false)
        {
            return HttpContext.Current.Request[key].ToString().Trim();
        }
        return string.Empty;
    }

    public static long ConvertToTimestamp(DateTime value)
    {
        long epoch = (value.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        return epoch;
    }

    public static String sha256_hash(String value)
    {
        StringBuilder Sb = new StringBuilder();

        using (SHA256 hash = SHA256Managed.Create())
        {
            Encoding enc = Encoding.UTF8;
            Byte[] result = hash.ComputeHash(enc.GetBytes(value));

            foreach (Byte b in result)
                Sb.Append(b.ToString("x2"));
        }
        return Sb.ToString();
    }

    /// <summary>
    /// Do a web request with differnt methods and different content types
    /// </summary>
    /// <param name="strURL">String URL</param>
    /// <param name="strData">String POST Data</param>
    /// <param name="strMethod">String method (GET or POST or DELETE or PUT)</param>
    /// <param name="strContentType">String Content Type (application/json or application/xml or application/x-www-form-urlencoded)</param>
    /// <returns></returns>
    public static String DoRequest(String strURL, String strData, String strMethod, String strContentType, String strHeaders)
    {
        long iStart = DateTime.Now.Ticks;
        WebRequest objReq;
        WebResponse objRes;
        StreamReader objSr;
        try
        {
            String strRetVal = String.Empty;

            ServicePointManager.Expect100Continue = false;
            int iConnLimit = 5;
            int.TryParse(General.GetConfigVal("DEFAULT_CONNECTION_LIMIT"), out iConnLimit);
            if (General.GetConfigVal("DEFAULT_CONNECTION_LIMIT").Length > 0)
                ServicePointManager.DefaultConnectionLimit = iConnLimit;
            else
                ServicePointManager.DefaultConnectionLimit = 5;
            //System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            objReq = WebRequest.Create(strURL);
            if (General.GetConfigVal("HTTP_REQ_CON_GRP_NAME_ENABLED").ToUpper() == "Y")
            {
                objReq.ConnectionGroupName = Guid.NewGuid().ToString();
            }
            objReq.Method = strMethod;//GET/POST/DELETE/PUT
            if (!String.IsNullOrEmpty(General.GetConfigVal("TIMEOUT")))
                objReq.Timeout = int.Parse(General.GetConfigVal("TIMEOUT"));
            else
                objReq.Timeout = 20000;

            if (!string.IsNullOrEmpty(strHeaders))
                objReq.Headers.Add("Authorization", strHeaders);

            objReq.ContentType = strContentType;//"application/json","application/xml","application/x-www-form-urlencoded";
            byte[] byteArray = Encoding.UTF8.GetBytes(strData);
            objReq.ContentLength = byteArray.Length;

            Stream dataStream = objReq.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            objRes = objReq.GetResponse();
            objSr = new StreamReader(objRes.GetResponseStream());
            strRetVal = objSr.ReadToEnd();
            objSr.Close();
            objRes.Close();
            //WriteLog("Url: " + strURL + ", Response: " + strRetVal + ", PostData: " + strData);
            return strRetVal;
        }
        catch (WebException ex)
        {
            using (var reader = new StreamReader(ex.Response.GetResponseStream()))
            {
                //WriteLog("Url: " + strURL + ", Error: " + reader.ReadToEnd());
                LogData.Write("GOTAPI", "DIGILOCKER", LogMode.Excep, ex, string.Format("clGeneral.cs/DoRequest() Url: {0}, Error: {1}", strURL, reader.ReadToEnd()));
            }
            return "Dear User, currently we are unable to process your request. Please try after sometime.";
        }
        finally
        {
            objReq = null;
            objRes = null;
            objSr = null;
        }
    }

    /// <summary>
    /// To do a web request and get the url response
    /// </summary>
    /// <param name="strURL">String URL</param>
    /// <returns>String URL Response</returns>
    public static String DoGETRequest(String strURL, String strHeaders)
    {
        long iStart = DateTime.Now.Ticks;
        WebRequest objReq;
        WebResponse objRes;
        StreamReader objSr;
        try
        {
            String strRetVal = String.Empty;

            System.Net.ServicePointManager.Expect100Continue = false;

            int iConnLimit = 5;
            int.TryParse(General.GetConfigVal("DEFAULT_CONNECTION_LIMIT"), out iConnLimit);
            if (General.GetConfigVal("DEFAULT_CONNECTION_LIMIT").Length > 0)
                ServicePointManager.DefaultConnectionLimit = iConnLimit;
            else
                ServicePointManager.DefaultConnectionLimit = 5;
            objReq = WebRequest.Create(strURL);
            if (General.GetConfigVal("HTTP_REQ_CON_GRP_NAME_ENABLED").ToUpper() == "Y")
            {
                objReq.ConnectionGroupName = Guid.NewGuid().ToString();
            }
            objReq.Method = "GET";
            if (!string.IsNullOrEmpty(strHeaders))
                objReq.Headers.Add("Authorization", strHeaders);
            if (!String.IsNullOrEmpty(General.GetConfigVal("TIMEOUT")))
                objReq.Timeout = int.Parse(General.GetConfigVal("TIMEOUT"));
            else
                objReq.Timeout = 20000;
            objRes = objReq.GetResponse();
            objSr = new StreamReader(objRes.GetResponseStream());
            strRetVal = objSr.ReadToEnd();
            objSr.Close();
            objRes.Close();
            //WriteLog("Url: " + strURL + ", Response: " + strRetVal);
            return strRetVal;
        }
        catch (WebException ex)
        {
            using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                LogData.Write("GOTAPI", "DIGILOCKER", LogMode.Excep, ex, string.Format("clGeneral.cs/DoGETRequest() Url: {0}, Error: {1}", strURL, reader.ReadToEnd()));
            return "Dear User, currently we are unable to process your request. Please try after sometime.";
        }
        finally
        {
            objReq = null;
            objRes = null;
            objSr = null;
        }
    }


    public static String DoGETRequestAndDownload(String strURL, String strHeaders, String sFileName)
    {
        long iStart = DateTime.Now.Ticks;
        WebRequest objReq;
        WebResponse objRes;
        StreamReader objSr;
        try
        {
            String strRetVal = String.Empty;

            System.Net.ServicePointManager.Expect100Continue = false;
            //LogData.Write("clGeneral", "DoGETRequestAndDownload_Check", LogMode.Audit, "DoGETRequestAndDownload_1: Landed");
            int iConnLimit = 5;
            int.TryParse(General.GetConfigVal("DEFAULT_CONNECTION_LIMIT"), out iConnLimit);
            if (General.GetConfigVal("DEFAULT_CONNECTION_LIMIT").Length > 0)
                ServicePointManager.DefaultConnectionLimit = iConnLimit;
            else
                ServicePointManager.DefaultConnectionLimit = 5;
            objReq = WebRequest.Create(strURL);
            //LogData.Write("clGeneral", "DoGETRequestAndDownload_Check", LogMode.Audit, "DoGETRequestAndDownload_2: Web Request Url:" + strURL);
            if (General.GetConfigVal("HTTP_REQ_CON_GRP_NAME_ENABLED").ToUpper() == "Y")
            {
                objReq.ConnectionGroupName = Guid.NewGuid().ToString();
            }
            objReq.Method = "GET";
            if (!string.IsNullOrEmpty(strHeaders))
                objReq.Headers.Add("Authorization", strHeaders);
            if (!String.IsNullOrEmpty(General.GetConfigVal("CONN_TIME_OUT")))
                objReq.Timeout = int.Parse(General.GetConfigVal("CONN_TIME_OUT"));
            else
                objReq.Timeout = 60000;
            objRes = objReq.GetResponse();
            //LogData.Write("clGeneral", "DoGETRequestAndDownload_Check", LogMode.Audit, "DoGETRequestAndDownload_3: Response is coming Smoothly");
            //objRes.ContentType = "application/pdf";
            objSr = new StreamReader(objRes.GetResponseStream());
            strRetVal = objRes.ResponseUri.AbsoluteUri;//objSr.ReadToEnd();//
            objSr.Close();
            objRes.Close();
            LogData.Write("clGeneral", "DoGETRequestAndDownload_Check", LogMode.Audit, "DoGETRequestAndDownload: Result:" + strRetVal);
            return strRetVal;
        }
        catch (WebException ex)
        {
            using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                LogData.Write("clGeneral", "DIGILOCKER_Web", LogMode.Excep, ex, string.Format("clGeneral.cs/DoGETRequestAndDownload() Url: {0}, Error: {1}", strURL, reader.ReadToEnd()));
            return "Dear User, currently we are unable to process your request. Please try after sometime.";
        }
        catch (Exception ex)
        {
            LogData.Write("clGeneral", "DIGILOCKER", LogMode.Excep, string.Format("Message:{0}, StackTrace:{1}", ex.Message, ex.StackTrace.ToStr()));
            return "Dear User, currently we are unable to process your request. Please try after sometime.";
        }
        finally
        {
            objReq = null;
            objRes = null;
            objSr = null;
        }
    }
}