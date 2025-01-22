//$Author: Anilkumar.ko $
//$Date: 4/03/18 6:55p $
//$Header: /Mobileapps/Operator/Davinci/TS/FailedTransUpdate/FailedTransUpdate/CommonHelper/General.cs 1     4/03/18 6:55p Anilkumar.ko $
//$Modtime: 1/18/18 6:38p $
//$Revision: 1 $
//Copyright @2012 IMImobile Pvt. Ltd.

using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Web;

namespace CommonHelper
{
    public class General
    {
        public static int WebReqTimeOut = 30000;
        public static int CONNECTION_LIMIT = 10;

        /// <summary>
        /// Purpose: Common HTTP POST method contentype Jsons.
        /// Author:  SARATH CHANDRA K
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="strAction"></param>
        /// <param name="sPayload"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DoHttpPostBaicAuthenticationPayment(string strUrl, string sPayload)
        {
            long lStartTime = DateTime.Now.Ticks;
            WebResponse result = null;
            string _strResponse = "";
            string credentials = string.Empty;
            try
            {
                // Getting and Adding credentials
                credentials = General.GetConfigVal("CESCOMRURAL_AUTHENITCATION");
                if (string.IsNullOrEmpty(credentials))
                    credentials = "imimobile:imimobile@cesc";
                byte[] authBytes = Encoding.UTF8.GetBytes(credentials.ToCharArray());
                string sPass = Convert.ToBase64String(authBytes);

                WebRequest req = WebRequest.Create(strUrl);
                String strProxyIP = GetConfigVal("GOK_PROXY_IP");
                String strProxyPort = GetConfigVal("GOK_PROXY_PORT");

                if (strProxyIP.Trim().Length > 0)
                {
                    WebProxy objWebProxy = null;
                    if (strProxyPort.Trim().Length > 0)
                        objWebProxy = new WebProxy(strProxyIP, strProxyPort.ToInt());
                    else if (strProxyIP.Trim().ToLower().Contains("http"))
                        objWebProxy = new WebProxy(strProxyIP + ":" + strProxyPort + "/", true);
                    else
                        objWebProxy = new WebProxy(strProxyIP, 80);
                    req.Proxy = objWebProxy;
                }
                req.Headers.Add("Authorization", "Basic " + sPass);
                req.Timeout = WebReqTimeOut;
                req.Method = "POST";
                if (!string.IsNullOrEmpty(sPayload))
                    req.ContentType = "application/x-www-form-urlencoded";
                StringBuilder UrlEncoded = new StringBuilder();
                Char[] reserved = { '?', '=', '&' };
                byte[] SomeBytes = null;

                try
                {
                    ServicePointManager.DefaultConnectionLimit = CONNECTION_LIMIT;
                }
                catch { }
                sPayload = "imionlinepayment=" + sPayload;
                if (!string.IsNullOrEmpty(sPayload))
                {
                    int i = 0, j;
                    while (i < sPayload.Length)
                    {
                        j = sPayload.IndexOfAny(reserved, i);
                        if (j == -1)
                        {
                            UrlEncoded.Append(sPayload.Substring(i, sPayload.Length - i));
                            break;
                        }
                        UrlEncoded.Append(sPayload.Substring(i, j - i));
                        UrlEncoded.Append(sPayload.Substring(j, 1));
                        i = j + 1;
                    }
                    SomeBytes = Encoding.UTF8.GetBytes(UrlEncoded.ToString());
                    req.ContentLength = SomeBytes.Length;
                    using (Stream newStream = req.GetRequestStream())
                    {
                        newStream.Write(SomeBytes, 0, SomeBytes.Length);
                    }
                }
                else
                {
                    req.ContentLength = 0;
                }
                result = req.GetResponse();
                using (Stream ReceiveStream = result.GetResponseStream())
                {
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    using (StreamReader sr = new StreamReader(ReceiveStream, encode))
                    {
                        Char[] read = new Char[256];
                        int count = sr.Read(read, 0, 256);
                        StringBuilder sb = new StringBuilder();
                        while (count > 0)
                        {
                            String str = new String(read, 0, count);
                            sb.Append(str);
                            count = sr.Read(read, 0, 256);
                        }
                        _strResponse = sb.ToString();
                    }
                }
                if (result != null)
                {
                    result.Close();
                }
            }
            catch (WebException Ex)
            {
                General.WriteLog("General", "Error in  General  WebException--> DoHttpPostBaicAuthenticationPayment -->Url:" + strUrl + "  :,  " + "PAyload:" + sPayload + "Message::" + Ex.Message + Environment.NewLine + "StackTrace :" + Ex.StackTrace);
            }
            catch (Exception e)
            {
                General.WriteLog("General", "Error in  General Catch --> DoHttpPostBaicAuthenticationPayment -->Url:" + strUrl + "  :,  " + "PAyload:" + sPayload + "Message::" + e.Message + Environment.NewLine + "StackTrace :" + e.StackTrace);
            }
            finally
            {
                General.WriteLog("General", "DoHttpPostBaicAuthenticationPayment --> Message:: Url:" + strUrl + "  :,Res:" + _strResponse + ":tt:" + (DateTime.Now.Ticks - lStartTime) / 10000);
            }
            return _strResponse;
        }


        /// <summary>
        /// Purpose: Common HTTP POST method contentype Jsons.
        /// Author:  SARATH CHANDRA K
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="strAction"></param>
        /// <param name="sPayload"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DoHttpPostBaicAuthentication(string strUrl, string sPayload)
        {
            long lStartTime = DateTime.Now.Ticks;
            WebResponse result = null;
            string _strResponse = "";
            string credentials = string.Empty;
            try
            {
                // Getting and Adding credentials
                credentials = General.GetConfigVal("CESCOMRURAL_AUTHENITCATION");
                if (string.IsNullOrEmpty(credentials))
                    credentials = "imimobile:imimobile@cesc";
                byte[] authBytes = Encoding.UTF8.GetBytes(credentials.ToCharArray());
                string sPass = Convert.ToBase64String(authBytes);

                WebRequest req = WebRequest.Create(strUrl);
                String strProxyIP = GetConfigVal("GOK_PROXY_IP");
                String strProxyPort = GetConfigVal("GOK_PROXY_PORT");

                if (strProxyIP.Trim().Length > 0)
                {
                    WebProxy objWebProxy = null;
                    if (strProxyPort.Trim().Length > 0)
                        objWebProxy = new WebProxy(strProxyIP, strProxyPort.ToInt());
                    else if (strProxyIP.Trim().ToLower().Contains("http"))
                        objWebProxy = new WebProxy(strProxyIP + ":" + strProxyPort + "/", true);
                    else
                        objWebProxy = new WebProxy(strProxyIP, 80);
                    req.Proxy = objWebProxy;
                }
                req.Headers.Add("Authorization", "Basic " + sPass);
                req.Timeout = WebReqTimeOut;
                req.Method = "POST";
                if (!string.IsNullOrEmpty(sPayload))
                    req.ContentType = "application/json";
                StringBuilder UrlEncoded = new StringBuilder();
                Char[] reserved = { '?', '=', '&' };
                byte[] SomeBytes = null;

                try
                {
                    ServicePointManager.DefaultConnectionLimit = CONNECTION_LIMIT;
                }
                catch { }

                if (!string.IsNullOrEmpty(sPayload))
                {
                    int i = 0, j;
                    while (i < sPayload.Length)
                    {
                        j = sPayload.IndexOfAny(reserved, i);
                        if (j == -1)
                        {
                            UrlEncoded.Append(sPayload.Substring(i, sPayload.Length - i));
                            break;
                        }
                        UrlEncoded.Append(sPayload.Substring(i, j - i));
                        UrlEncoded.Append(sPayload.Substring(j, 1));
                        i = j + 1;
                    }
                    SomeBytes = Encoding.UTF8.GetBytes(UrlEncoded.ToString());
                    req.ContentLength = SomeBytes.Length;
                    using (Stream newStream = req.GetRequestStream())
                    {
                        newStream.Write(SomeBytes, 0, SomeBytes.Length);
                    }
                }
                else
                {
                    req.ContentLength = 0;
                }
                result = req.GetResponse();
                using (Stream ReceiveStream = result.GetResponseStream())
                {
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    using (StreamReader sr = new StreamReader(ReceiveStream, encode))
                    {
                        Char[] read = new Char[256];
                        int count = sr.Read(read, 0, 256);
                        StringBuilder sb = new StringBuilder();
                        while (count > 0)
                        {
                            String str = new String(read, 0, count);
                            sb.Append(str);
                            count = sr.Read(read, 0, 256);
                        }
                        _strResponse = sb.ToString();
                    }
                }
                if (result != null)
                {
                    result.Close();
                }
            }
            catch (WebException Ex)
            {
                General.WriteLog("General", "Error in  General  WebException--> DoHttpPostBaicAuthentication -->Url:" + strUrl + "  :,  " + "PAyload:" + sPayload + "Message::" + Ex.Message + Environment.NewLine + "StackTrace :" + Ex.StackTrace);
            }
            catch (Exception e)
            {
                General.WriteLog("General", "Error in  General Catch --> DoHttpPostBaicAuthentication -->Url:" + strUrl + "  :,  " + "PAyload:" + sPayload + "Message::" + e.Message + Environment.NewLine + "StackTrace :" + e.StackTrace);
            }
            finally
            {
                General.WriteLog("General", "DoHttpPostBaicAuthentication --> Url:" + strUrl + " Request:" + sPayload + Environment.NewLine + ",Response:" + _strResponse + Environment.NewLine + ",:tt:" + (DateTime.Now.Ticks - lStartTime) / 10000);
            }
            return _strResponse;
        }

        /// <summary>
        /// Purpose: Common HTTP POST method contentype Jsons.
        /// Author:  SARATH CHANDRA K
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="strAction"></param>
        /// <param name="sPayload"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DoHttpGetWithBasicAuthentication(string strUrl)
        {
            long lStartTime = DateTime.Now.Ticks;
            WebResponse result = null;
            string _strResponse = "";
            string credentials = string.Empty;
            try
            {
                // Getting and Adding credentials
                credentials = General.GetConfigVal("CESCOMRURAL_AUTHENITCATION");
                if (string.IsNullOrEmpty(credentials))
                    credentials = "imimobile:imimobile@cesc";
                byte[] authBytes = Encoding.UTF8.GetBytes(credentials.ToCharArray());
                string sPass = Convert.ToBase64String(authBytes);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strUrl));
                request.Headers.Add("Authorization", "Basic " + sPass);
                request.Timeout = WebReqTimeOut;
                String strProxyIP = GetConfigVal("GOK_PROXY_IP");
                String strProxyPort = GetConfigVal("GOK_PROXY_PORT");

                if (strProxyIP.Trim().Length > 0)
                {
                    WebProxy objWebProxy = null;
                    if (strProxyPort.Trim().Length > 0)
                        objWebProxy = new WebProxy(strProxyIP, strProxyPort.ToInt());
                    else if (strProxyIP.Trim().ToLower().Contains("http"))
                        objWebProxy = new WebProxy(strProxyIP + ":" + strProxyPort + "/", true);
                    else
                        objWebProxy = new WebProxy(strProxyIP, 80);
                    request.Proxy = objWebProxy;
                }
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.DefaultConnectionLimit = CONNECTION_LIMIT;
                ServicePointManager.UseNagleAlgorithm = false;
                using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
                {
                    _strResponse = reader.ReadToEnd().ToString();
                }
                return _strResponse;
            }
            catch (WebException Ex)
            {
                General.WriteLog("General", "Error in  General --> DoHttpPostBaicAuthentication -->Url:" + strUrl + "  :, Message:: " + Ex.Message + Environment.NewLine + "StackTrace :" + Ex.StackTrace);
            }
            catch (Exception e)
            {
                General.WriteLog("General", "Error in  General --> DoHttpPostBaicAuthentication -->Url:" + strUrl + "  :, Message:: " + e.Message + Environment.NewLine + "StackTrace :" + e.StackTrace);
            }
            finally
            {
                General.WriteLog("General", "DoHttpPostBaicAuthentication --> Message:: Url:" + strUrl + "  :,Res:" + _strResponse + ":tt:" + (DateTime.Now.Ticks - lStartTime) / 10000);
            }
            return _strResponse;
        }

        /// <summary>
        /// Get key value from web.config file
        /// </summary>
        /// <param name="strKey">String Key</param>
        /// <returns>String Key Value</returns>
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

        /// <summary>
        /// Converts String to Int
        /// </summary>
        /// <param name="strVal">String</param>
        /// <returns>Int</returns>
        public static int ConvertInt(string strVal)
        {
            try
            {
                return Convert.ToInt32(strVal);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Converts String to Double
        /// </summary>
        /// <param name="strVal">String</param>
        /// <returns>Double</returns>
        public static double ConvertDouble(string strVal)
        {
            try
            {
                return double.Parse(strVal);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Validate email address
        /// </summary>
        /// <param name="emailId">String Email</param>
        /// <returns>True/False</returns>
        public static bool IsValidEmail(string emailId)
        {
            String strRegex = @"^([a-zA-Z0-9_\-\.\+]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            return new Regex(strRegex).IsMatch(emailId);
        }

        /// <summary>
        /// To do a web request and get the url response
        /// </summary>
        /// <param name="strURL">String URL</param>
        /// <returns>String URL Response</returns>
        public static String DoWebRequest(String strURL)
        {
            long iStart = DateTime.Now.Ticks;
            WebRequest objReq;
            WebResponse objRes;
            StreamReader objSr;
            try
            {
                String strRetVal = String.Empty;

                System.Net.ServicePointManager.Expect100Continue = false;
                if (GetConfigVal("DEFAULT_CONNECTION_LIMIT").Length > 0)
                    ServicePointManager.DefaultConnectionLimit = GetConfigVal("DEFAULT_CONNECTION_LIMIT").ToInt();
                else
                    ServicePointManager.DefaultConnectionLimit = 5;
                objReq = WebRequest.Create(strURL);
                objReq.ConnectionGroupName = Guid.NewGuid().ToString();
                objReq.Method = "GET";
                if (!String.IsNullOrEmpty(GetConfigVal("TIMEOUT")))
                    objReq.Timeout = int.Parse(GetConfigVal("TIMEOUT"));
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
                    WriteLog("Url: " + strURL + ", Error: " + reader.ReadToEnd());
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
        /// To do a web request with method POST and get the url response
        /// </summary>
        /// <param name="strURL">String URL</param>
        /// <param name="strData">String Post Data</param>
        /// <returns>String URL Response</returns>
        public static String DoPostRequest(String strURL, String strData)
        {
            long iStart = DateTime.Now.Ticks;
            WebRequest objReq;
            WebResponse objRes;
            StreamReader objSr;
            String strRetVal = String.Empty;
            try
            {
                System.Net.ServicePointManager.Expect100Continue = false;
                if (GetConfigVal("DEFAULT_CONNECTION_LIMIT").Length > 0)
                    ServicePointManager.DefaultConnectionLimit = GetConfigVal("DEFAULT_CONNECTION_LIMIT").ToInt();
                else
                    ServicePointManager.DefaultConnectionLimit = 5;
                objReq = WebRequest.Create(strURL);
                objReq.ConnectionGroupName = Guid.NewGuid().ToString();
                objReq.Method = "POST";
                if (!String.IsNullOrEmpty(GetConfigVal("TIMEOUT")))
                    objReq.Timeout = int.Parse(GetConfigVal("TIMEOUT"));
                else
                    objReq.Timeout = 20000;
                objReq.ContentType = "application/x-www-form-urlencoded";
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
                    WriteLog("Url: " + strURL + ", Error: " + reader.ReadToEnd());
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
        /// To do a web request with method POST and data as JSON and get the url response
        /// </summary>
        /// <param name="strURL">String URL</param>
        /// <param name="strData">String Post JSON Data</param>
        /// <returns>String URL Response</returns>
        public static String DoJSONPostRequest(String strURL, String strData)
        {
            long iStart = DateTime.Now.Ticks;
            WebRequest objReq;
            WebResponse objRes;
            StreamReader objSr;
            try
            {
                String strRetVal = String.Empty;
                ServicePointManager.Expect100Continue = false;
                if (GetConfigVal("DEFAULT_CONNECTION_LIMIT").Length > 0)
                    ServicePointManager.DefaultConnectionLimit = GetConfigVal("DEFAULT_CONNECTION_LIMIT").ToInt();
                else
                    ServicePointManager.DefaultConnectionLimit = 5;
                objReq = WebRequest.Create(strURL);
                //objReq.ConnectionGroupName = Guid.NewGuid().ToString();
                objReq.Method = "POST";
                if (!String.IsNullOrEmpty(GetConfigVal("TIMEOUT")))
                    objReq.Timeout = int.Parse(GetConfigVal("TIMEOUT"));
                else
                    objReq.Timeout = 20000;
                objReq.ContentType = "application/json";
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
                    WriteLog("Url: " + strURL + ", Error: " + reader.ReadToEnd());
                return "Dear User, currently we are unable to process your request. Please try after sometime.";
            }
            finally
            {
                objReq = null;
                objRes = null;
                objSr = null;
            }
        }

        public static String DoJSONPostRequest(String strURL, String strData, int iTimeOut)
        {
            long lTimeTaken = -1, lStart = DateTime.Now.Ticks;
            WebRequest objReq;
            WebResponse objRes = null;
            StreamReader objSr = null;
            try
            {
                String strRetVal = String.Empty;
                ServicePointManager.Expect100Continue = false;
                if (GetConfigVal("DEFAULT_CONNECTION_LIMIT").Length > 0)
                    ServicePointManager.DefaultConnectionLimit = GetConfigVal("DEFAULT_CONNECTION_LIMIT").ToInt();
                else
                    ServicePointManager.DefaultConnectionLimit = 5;
                objReq = WebRequest.Create(strURL);
                //if (GetConfigVal("HTTP_REQ_CON_GRP_NAME_ENABLED").ToUpper() == "Y")
                //{
                //    objReq.ConnectionGroupName = Guid.NewGuid().ToString();
                //}
                objReq.Method = "POST";
                objReq.Timeout = iTimeOut;
                objReq.ContentType = "application/json";
                byte[] byteArray = Encoding.UTF8.GetBytes(strData);
                objReq.ContentLength = byteArray.Length;

                Stream dataStream = objReq.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                objRes = objReq.GetResponse();
                objSr = new StreamReader(objRes.GetResponseStream());
                strRetVal = objSr.ReadToEnd();
                if (lStart > 0)
                    lTimeTaken = (DateTime.Now.Ticks - lStart) / 10000;
                //objSr.Close();
                // objRes.Close();
                return strRetVal;
            }
            //catch (WebException webex)
            //{
            //    if (lStart > 0)
            //        lTimeTaken = (DateTime.Now.Ticks - lStart) / 10000;
            //    if (webex.Status == WebExceptionStatus.Timeout)
            //    {
            //        General.WriteLog("DoJSONPostRequest", string.Format("DoJSONPostRequest::Url:{1}{0}.Timetaken:{2}{0}.WebException Message:{3}{0}", Environment.NewLine, strURL, lTimeTaken.ToStr(), webex.Message));
            //    }
            //    else
            //    {
            //        if (webex.Response != null)
            //        {
            //            using (var reader = new StreamReader(webex.Response.GetResponseStream()))
            //                General.WriteLog("DoJSONPostRequest", string.Format("DoJSONPostRequest::Url:{1}{0}.Timetaken:{2}{0}.WebException Message:{3}{0}.StackTrace{4}{0}", Environment.NewLine, strURL, lTimeTaken.ToStr(), webex.Message, reader.ReadToEnd()));
            //        }
            //        else
            //            General.WriteLog("DoJSONPostRequest", string.Format("DoJSONPostRequest::Url:{1}{0}.Timetaken:{2}{0}.WebException Message:{3}{0}", Environment.NewLine, strURL, lTimeTaken.ToStr(), webex.Message));
            //    }
            //    return "Dear User, currently we are unable to process your request. Please try after sometime.";
            //}              
            catch (Exception ex)
            {
                if (lStart > 0)
                    lTimeTaken = (DateTime.Now.Ticks - lStart) / 10000;
                General.WriteLog("DoJSONPostRequest", string.Format("DoJSONPostRequest::Url:{1}{0}Timetaken:{2}{0}Exception Message:{3}{0}", Environment.NewLine, strURL, lTimeTaken.ToStr(), ex.Message));
                return "Dear User, currently we are unable to process your request. Please try after sometime.";
            }
            finally
            {
                if (objSr != null) objSr.Close();
                if (objRes != null) objRes.Close();
                objReq = null;
                objRes = null;
                objSr = null;
                HttpContext.Current.Response.AppendToLog("&url=" + strURL + "&tt=" + lTimeTaken.ToString());
            }
        }


        /// <summary>
        /// Do a web request with differnt methods and different content types
        /// </summary>
        /// <param name="strURL">String URL</param>
        /// <param name="strData">String POST Data</param>
        /// <param name="strMethod">String method (GET or POST or DELETE or PUT)</param>
        /// <param name="strContentType">String Content Type (application/json or application/xml or application/x-www-form-urlencoded)</param>
        /// <returns></returns>
        public static String DoRequest(String strURL, String strData, String strMethod, String strContentType)
        {
            long iStart = DateTime.Now.Ticks;
            WebRequest objReq;
            WebResponse objRes;
            StreamReader objSr;
            try
            {
                String strRetVal = String.Empty;

                ServicePointManager.Expect100Continue = false;
                if (GetConfigVal("DEFAULT_CONNECTION_LIMIT").Length > 0)
                    ServicePointManager.DefaultConnectionLimit = GetConfigVal("DEFAULT_CONNECTION_LIMIT").ToInt();
                else
                    ServicePointManager.DefaultConnectionLimit = 5;
                //System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                objReq = WebRequest.Create(strURL);
                objReq.ConnectionGroupName = Guid.NewGuid().ToString();
                objReq.Method = strMethod;//GET/POST/DELETE/PUT
                if (!String.IsNullOrEmpty(GetConfigVal("TIMEOUT")))
                    objReq.Timeout = int.Parse(GetConfigVal("TIMEOUT"));
                else
                    objReq.Timeout = 20000;
                objReq.ContentType = strContentType;//"application/json","application/xml","application/x-www-form-urlencoded";
                if (strData.Trim().Length > 0)
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(strData);
                    objReq.ContentLength = byteArray.Length;

                    Stream dataStream = objReq.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }

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
                if (ex.Response != null)
                {
                    using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                        WriteLog("Url: " + strURL + ", Error: " + reader.ReadToEnd());
                }
                else
                    WriteLog("DoRequest", "Exception in DoRequest: url:" + strURL + ",Exception:" + ex.ToStr());
                return "";
            }
            finally
            {
                objReq = null;
                objRes = null;
                objSr = null;
            }
        }

        /// <summary>
        /// Do a web request with differnt methods and different content types
        /// </summary>
        /// <param name="strURL">String URL</param>
        /// <param name="strData">String POST Data</param>
        /// <param name="strMethod">String method (GET or POST or DELETE or PUT)</param>
        /// <param name="htHeaders">Hashtable headers</param>
        /// <param name="strContentType">String Content Type (application/json or application/xml or application/x-www-form-urlencoded)</param>
        /// <returns></returns>
        public static String DoRequest(String strURL, String strData, String strMethod, String strContentType, Hashtable htHeaders)
        {
            long iStart = DateTime.Now.Ticks;
            WebRequest objReq;
            WebResponse objRes;
            StreamReader objSr;
            try
            {
                string strRetVal = string.Empty;

                ServicePointManager.Expect100Continue = false;
                if (GetConfigVal("DEFAULT_CONNECTION_LIMIT").Length > 0)
                    ServicePointManager.DefaultConnectionLimit = GetConfigVal("DEFAULT_CONNECTION_LIMIT").ToInt();
                else
                    ServicePointManager.DefaultConnectionLimit = 5;
                objReq = WebRequest.Create(strURL);
                if (GetConfigVal("HTTP_REQ_CON_GRP_NAME_ENABLED").ToUpper() == "Y")
                {
                    objReq.ConnectionGroupName = Guid.NewGuid().ToString();
                }
                if (htHeaders != null)
                {
                    foreach (DictionaryEntry entry in htHeaders)
                        objReq.Headers.Add(entry.Key.ToString(), entry.Value.ToString());
                }
                objReq.Method = strMethod;//GET/POST/DELETE/PUT
                if (!string.IsNullOrEmpty(GetConfigVal("TIMEOUT")))
                    objReq.Timeout = int.Parse(GetConfigVal("TIMEOUT"));
                else
                    objReq.Timeout = 20000;
                objReq.ContentType = strContentType;//"application/json","application/xml","application/x-www-form-urlencoded";
                if (strData.Trim().Length > 0)
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(strData);
                    objReq.ContentLength = byteArray.Length;

                    Stream dataStream = objReq.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }

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
                    WriteLog("Url: " + strURL + ", Error: " + reader.ReadToEnd());
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
        /// Do a web request with differnt methods and different content types
        /// </summary>
        /// <param name="strURL">String URL</param>
        /// <param name="strData">String POST Data</param>
        /// <param name="strMethod">String method (GET or POST or DELETE or PUT)</param>
        /// <param name="htHeaders">Hashtable headers</param>
        /// <param name="strContentType">String Content Type (application/json or application/xml or application/x-www-form-urlencoded)</param>
        /// <returns></returns>
        public static String DoRequest(String strURL, String strData, String strMethod, String strContentType, Hashtable htHeaders, String strProxyIP, String strProxyPort)
        {
            long iStart = DateTime.Now.Ticks;
            WebRequest objReq;
            WebResponse objRes;
            StreamReader objSr;
            try
            {
                string strRetVal = string.Empty;

                ServicePointManager.Expect100Continue = false;
                if (GetConfigVal("DEFAULT_CONNECTION_LIMIT").Length > 0)
                    ServicePointManager.DefaultConnectionLimit = GetConfigVal("DEFAULT_CONNECTION_LIMIT").ToInt();
                else
                    ServicePointManager.DefaultConnectionLimit = 5;
                objReq = WebRequest.Create(strURL);
                objReq.ConnectionGroupName = Guid.NewGuid().ToString();
                if (strProxyIP.Trim().Length > 0)
                {
                    WebProxy objWebProxy = null;
                    if (strProxyPort.Trim().Length > 0)
                        objWebProxy = new WebProxy(strProxyIP, strProxyPort.ToInt());
                    else if (strProxyIP.Trim().ToLower().Contains("http"))
                        objWebProxy = new WebProxy(strProxyIP + ":" + strProxyPort + "/", true);
                    else
                        objWebProxy = new WebProxy(strProxyIP, 80);
                    objReq.Proxy = objWebProxy;
                }

                if (htHeaders != null)
                {
                    foreach (DictionaryEntry entry in htHeaders)
                        objReq.Headers.Add(entry.Key.ToString(), entry.Value.ToString());
                }

                objReq.Method = strMethod;//GET/POST/DELETE/PUT
                if (!string.IsNullOrEmpty(GetConfigVal("TIMEOUT")))
                    objReq.Timeout = int.Parse(GetConfigVal("TIMEOUT"));
                else
                    objReq.Timeout = 20000;
                objReq.ContentType = strContentType;//"application/json","application/xml","application/x-www-form-urlencoded";
                if (strData.Trim().Length > 0)
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(strData);
                    objReq.ContentLength = byteArray.Length;

                    Stream dataStream = objReq.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }

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
                    WriteLog("Url: " + strURL + ", Error: " + reader.ReadToEnd());
                return "Dear User, currently we are unable to process your request. Please try after sometime.";
            }
            finally
            {
                objReq = null;
                objRes = null;
                objSr = null;
            }
        }


        public static String DoHttpWebRequest(String strURL, String strData, String strMethod, String strContentType, Hashtable htHeaders, int iTimeOut, bool IsKeepAlive)
        {
            long lTimeTaken = -1, lStart = DateTime.Now.Ticks;
            HttpWebRequest objReq;
            HttpWebResponse objRes = null;
            StreamReader objSr = null;
            try
            {
                string strRetVal = string.Empty;

                ServicePointManager.Expect100Continue = false;
                if (GetConfigVal("DEFAULT_CONNECTION_LIMIT").Length > 0)
                    ServicePointManager.DefaultConnectionLimit = GetConfigVal("DEFAULT_CONNECTION_LIMIT").ToInt();
                else
                    ServicePointManager.DefaultConnectionLimit = 5;
                objReq = (HttpWebRequest)WebRequest.Create(strURL);
                if (GetConfigVal("HTTP_REQ_CON_GRP_NAME_ENABLED").ToUpper() == "Y")
                {
                    objReq.ConnectionGroupName = Guid.NewGuid().ToString();
                }
                if (htHeaders != null)
                {
                    foreach (DictionaryEntry entry in htHeaders)
                        objReq.Headers.Add(entry.Key.ToString(), entry.Value.ToString());
                }
                objReq.KeepAlive = IsKeepAlive;

                objReq.Method = strMethod;//GET/POST/DELETE/PUT
                objReq.Timeout = iTimeOut == 0 ? 20000 : iTimeOut;
                objReq.ContentType = strContentType;//"application/json","application/xml","application/x-www-form-urlencoded";
                if (strData.Trim().Length > 0)
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(strData);
                    objReq.ContentLength = byteArray.Length;

                    Stream dataStream = objReq.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }
                objRes = (HttpWebResponse)objReq.GetResponse();
                objSr = new StreamReader(objRes.GetResponseStream());
                strRetVal = objSr.ReadToEnd();
                // objSr.Close();
                //objRes.Close();
                if (lStart > 0)
                    lTimeTaken = (DateTime.Now.Ticks - lStart) / 10000;
                return strRetVal;
            }
            catch (WebException webex)
            {
                if (lStart > 0)
                    lTimeTaken = (DateTime.Now.Ticks - lStart) / 10000;
                using (var reader = new StreamReader(webex.Response.GetResponseStream()))
                    General.WriteLog("DoHttpWebRequest", string.Format("DoHttpWebRequest::Url:{1}{0}.Timetaken:{2}{0}.WebException Message:{3}{0}.StackTrace{4}{0}", Environment.NewLine, strURL, lTimeTaken.ToStr(), webex.Message, reader.ReadToEnd()));
                return "Dear User, currently we are unable to process your request. Please try after sometime.";
            }
            catch (Exception ex)
            {
                if (lStart > 0)
                    lTimeTaken = (DateTime.Now.Ticks - lStart) / 10000;
                General.WriteLog("DoHttpWebRequest", string.Format("DoHttpWebRequest::Url:{1}{0}.Timetaken:{2}{0}.Exception Message:{3}{0}.StackTrace{4}{0}", Environment.NewLine, strURL, lTimeTaken.ToStr(), ex.Message, ex.StackTrace));
                return "";
            }
            finally
            {

                if (objSr != null) objSr.Close();
                if (objRes != null) objRes.Close();
                objReq = null;
                objRes = null;
                objSr = null;
                HttpContext.Current.Response.AppendToLog("&url=" + strURL + "&tt=" + lTimeTaken.ToString());
            }
        }




        /// <summary>
        /// To write log - For this we need to configure this ENABLELOG (Y or N), ERRORLOG_PATH(Physical path) in web.config file.
        /// </summary>
        /// <param name="strMessage">String log message</param>
        public static void WriteLog(String strMessage)
        {
            try
            {
                if (GetConfigVal("ENABLELOG") == "N")
                    return;

                StreamWriter stm;
                string FilePath = GetConfigVal("ERRORLOG_PATH");
                if (!Directory.Exists(FilePath))
                    Directory.CreateDirectory(FilePath);

                FilePath = FilePath + "LOG_" + DateTime.Now.ToString("dd_MMM_yyyy") + ".txt";
                if (File.Exists(FilePath))
                {
                    FileInfo sfInfo = new FileInfo(FilePath);
                    if (sfInfo.Length > 524288)
                        File.Move(FilePath, FilePath.Replace(".txt", "-" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt"));
                    sfInfo = null;
                }

                stm = File.AppendText(FilePath);
                stm.WriteLine(DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + "|  " + strMessage);
                stm.Close();
                stm = null;
            }
            catch
            {
            }
        }

        /// <summary>
        /// To write log - For this we need to configure this ENABLELOG (Y or N), ERRORLOG_PATH(Physical path) in web.config file.
        /// </summary>
        /// <param name="strMessage">String log message</param>
        /// <param name="strFileName">String file name</param>
        public static void WriteLog(String strFileName, String strMessage)
        {
            try
            {
                if (GetConfigVal("ENABLELOG") == "N")
                    return;

                StreamWriter stm;
                string FilePath = GetConfigVal("ERRORLOG_PATH");
                if (!Directory.Exists(FilePath))
                    Directory.CreateDirectory(FilePath);

                FilePath = FilePath + strFileName + "_" + DateTime.Now.ToString("dd_MMM_yyyy") + ".txt";
                if (File.Exists(FilePath))
                {
                    FileInfo sfInfo = new FileInfo(FilePath);
                    if (sfInfo.Length > 524288)
                        File.Move(FilePath, FilePath.Replace(".txt", "-" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt"));
                    sfInfo = null;
                }

                stm = File.AppendText(FilePath);
                stm.WriteLine(DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + "|  " + strMessage);
                stm.Close();
                stm = null;
            }
            catch
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strErrorCode"></param>
        /// <param name="strErrorMsg"></param>
        /// <returns></returns>
        public static XmlElement GetErrorMsg(String strErrorCode, String strErrorMsg)
        {
            XmlDocument xDoc = new XmlDocument();
            XmlCreator.AppendDeclaration(ref xDoc);
            XmlNode xNode = XmlCreator.CreateNode("xml", "", ref xDoc);
            XmlAttribute xAttr = xDoc.CreateAttribute("code");
            xAttr.Value = strErrorCode;
            xNode.Attributes.Append(xAttr);
            xNode.InnerText = strErrorMsg;
            xDoc.AppendChild(xNode);
            return xDoc.DocumentElement;
        }

        public static String FormatMongoResponse(String strMsg)
        {
            try
            {
                strMsg = strMsg.Replace("]", "");
                strMsg = strMsg.Replace("[", "");
                strMsg = strMsg.Replace(" ", "");
                strMsg = strMsg.Replace("\r\n", "");
                strMsg = strMsg.Replace("\"", "");
                return strMsg;
            }
            catch
            {
                return strMsg;
            }
        }

        public static String FormatGetMongoResponse(String strMsg)
        {
            try
            {
                strMsg = strMsg.Replace("\r\n", "");
                strMsg = strMsg.TrimStart('[').TrimEnd(']');
                return strMsg;
            }
            catch
            {
                return strMsg;
            }
        }

        public static Boolean isNumeric(String sMsg)
        {
            try
            {
                foreach (char sChar in sMsg.ToCharArray())
                {
                    if (!char.IsNumber(sChar))
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public class XmlCreator
        {
            /// <summary>
            /// Append Xml Declaration 
            /// </summary>

            public static void AppendDeclaration(ref XmlDocument xDoc)
            {
                XmlDeclaration xmlD = xDoc.CreateXmlDeclaration("1.0", "utf-8", "");
                xDoc.AppendChild(xmlD);
                xmlD = null;
            }

            public static XmlNode CreateNode(string _NodeName, string _InnerText, ref XmlDocument xDoc)
            {
                XmlNode xn = xDoc.CreateNode(XmlNodeType.Element, _NodeName, "");
                if (_InnerText.Trim() != "") xn.InnerText = _InnerText;
                return xn;
            }

            public static void CreateNode(string _NodeName, string _InnerText, ref XmlNode xParentNode, ref XmlDocument xDoc)
            {
                XmlNode xn = xDoc.CreateNode(XmlNodeType.Element, _NodeName, "");
                if (_InnerText.Trim() != "") xn.InnerText = _InnerText;
                xParentNode.AppendChild(xn);
            }

            public static XmlAttribute CreateAttribute(string _AttrName, string _AttrValue, ref XmlDocument xDoc)
            {
                XmlAttribute xAttribute = xDoc.CreateAttribute(_AttrName);
                xAttribute.Value = _AttrValue;
                return xAttribute;
            }

            public static void ModifyAttribute(string _AttrName, string _AttrValue, ref XmlNode xNode, ref XmlDocument xDoc)
            {
                if (xNode.Attributes[_AttrName] == null)
                {
                    XmlAttribute xAttribute = xDoc.CreateAttribute(_AttrName);
                    xAttribute.Value = _AttrValue;
                    xNode.Attributes.Append(xAttribute);
                    xAttribute = null;
                }
                else
                {
                    xNode.Attributes[_AttrName].Value = _AttrValue;
                }
            }

            public static string GetAttributeValue(XmlNode xnTmp, string sAttr)
            {
                return xnTmp.Attributes[sAttr] != null ? xnTmp.Attributes[sAttr].Value.Trim() : "";
            }

            public static XmlNodeList GetXmlNodeList(ref XmlDocument _xDoc, string strxmlNode)
            {
                XmlNodeList xmlnode;
                try
                {
                    xmlnode = _xDoc.DocumentElement.SelectNodes(strxmlNode);
                    return xmlnode;
                }
                catch
                {
                }
                return null;
            }
        }

        public static bool SendMail(String strTo, String strCC, String strBCC, String strSubject, String strBody, String strAttachment)
        {
            EmailBO objBO = new EmailBO();
            bool bRetVal = false;
            try
            {
                objBO.MobileNo = String.Empty;
                objBO.Channel = String.Empty;
                objBO.Department = String.Empty;
                objBO.MailTo = strTo;
                objBO.MailCC = strCC;
                objBO.MailBCC = strBCC;
                objBO.Subject = strSubject;
                objBO.Body = strBody;
                objBO.Attachment = strAttachment;
                objBO.TransDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                bRetVal = objBO.LogMessage();
            }
            catch (Exception ex)
            {
                General.WriteLog("EMAIL_QUEUE_FAIL", "Error in SendMail(), Error-" + ex.Message.ToString());
            }
            finally
            {
                objBO = null;
            }
            return bRetVal;
            #region Commented code
            //try
            //{
            //    string strFrom = General.GetConfigVal("MAIL_FROM");
            //    string strUserId = General.GetConfigVal("MAIL_USER");
            //    string strPassword = General.GetConfigVal("MAIL_PASSWORD");
            //    string strMailHost = General.GetConfigVal("MAIL_HOST");
            //    string strMailPort = General.GetConfigVal("MAIL_PORT");
            //    string strMailSSL = General.GetConfigVal("MAIL_SSL");

            //    MailMessage objMail = new MailMessage();
            //    objMail.Subject = strSubject;
            //    string[] strToAddress = strTo.Split(new char[] { ',', ';' });
            //    foreach (string strToMail in strToAddress)
            //        objMail.To.Add(new MailAddress(strToMail));

            //    if (strBCC != string.Empty)
            //    {
            //        string[] strBCCAddress = strBCC.Split(new char[] { ',', ';' });
            //        foreach (string strBCCMail in strBCCAddress)
            //            objMail.Bcc.Add(new MailAddress(strBCCMail));
            //    }

            //    if (strCC != string.Empty)
            //    {
            //        string[] strCCAddress = strCC.Split(new char[] { ',', ';' });
            //        foreach (string strCCMail in strCCAddress)
            //            objMail.CC.Add(new MailAddress(strCCMail));
            //    }
            //    objMail.From = new MailAddress(strFrom, strFrom);
            //    //AlternateView objPlainAltView = AlternateView.CreateAlternateViewFromString(strBody, System.Text.Encoding.UTF8, MediaTypeNames.Text.Plain);
            //    //objMail.AlternateViews.Add(objPlainAltView);
            //    AlternateView objHTLMAltView = AlternateView.CreateAlternateViewFromString(strBody, System.Text.Encoding.UTF8, MediaTypeNames.Text.Html);
            //    objMail.AlternateViews.Add(objHTLMAltView);
            //    if (strAttachment != string.Empty)
            //    {
            //        Attachment at = new Attachment(strAttachment);
            //        objMail.Attachments.Add(at);
            //        objMail.Priority = MailPriority.High;
            //    }

            //    objMail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            //    SmtpClient objSMTPClient = null;
            //    if (strMailPort.Trim().Length > 0)
            //        objSMTPClient = new SmtpClient(strMailHost, strMailPort.ToInt());
            //    else
            //        objSMTPClient = new SmtpClient(strMailHost);
            //    objSMTPClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            //    if (strMailSSL.ToLower() == "y")
            //        objSMTPClient.EnableSsl = true;
            //    objSMTPClient.Credentials = new System.Net.NetworkCredential(strUserId, strPassword);
            //    objSMTPClient.Send(objMail);
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    General.WriteLog("Helper", "Exception at SendMail:" + ex.Message);
            //}
            //return false;
            #endregion
        }

        public static bool SendMail(String strTo, String strCC, String strBCC, String strSubject, String strBody, String strAttachment, String strMobileNo, String strChannel, String strDept)
        {
            EmailBO objBO = new EmailBO();
            bool bRetVal = false;
            try
            {
                objBO.MobileNo = strMobileNo;
                objBO.Channel = strChannel;
                objBO.Department = strDept;
                objBO.MailTo = strTo;
                objBO.MailCC = strCC;
                objBO.MailBCC = strBCC;
                objBO.Subject = strSubject;
                objBO.Body = strBody;
                objBO.Attachment = strAttachment;
                objBO.TransDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                bRetVal = objBO.LogMessage();
            }
            catch (Exception ex)
            {
                General.WriteLog("EMAIL_QUEUE_FAIL", "Error in SendMail(), Error-" + ex.Message.ToString());
            }
            finally
            {
                objBO = null;
            }
            return bRetVal;
            #region Commented code
            //try
            //{
            //    string strFrom = General.GetConfigVal("MAIL_FROM");
            //    string strUserId = General.GetConfigVal("MAIL_USER");
            //    string strPassword = General.GetConfigVal("MAIL_PASSWORD");
            //    string strMailHost = General.GetConfigVal("MAIL_HOST");
            //    string strMailPort = General.GetConfigVal("MAIL_PORT");
            //    string strMailSSL = General.GetConfigVal("MAIL_SSL");

            //    MailMessage objMail = new MailMessage();
            //    objMail.Subject = strSubject;
            //    string[] strToAddress = strTo.Split(new char[] { ',', ';' });
            //    foreach (string strToMail in strToAddress)
            //        objMail.To.Add(new MailAddress(strToMail));

            //    if (strBCC != string.Empty)
            //    {
            //        string[] strBCCAddress = strBCC.Split(new char[] { ',', ';' });
            //        foreach (string strBCCMail in strBCCAddress)
            //            objMail.Bcc.Add(new MailAddress(strBCCMail));
            //    }

            //    if (strCC != string.Empty)
            //    {
            //        string[] strCCAddress = strCC.Split(new char[] { ',', ';' });
            //        foreach (string strCCMail in strCCAddress)
            //            objMail.CC.Add(new MailAddress(strCCMail));
            //    }
            //    objMail.From = new MailAddress(strFrom, strFrom);
            //    //AlternateView objPlainAltView = AlternateView.CreateAlternateViewFromString(strBody, System.Text.Encoding.UTF8, MediaTypeNames.Text.Plain);
            //    //objMail.AlternateViews.Add(objPlainAltView);
            //    AlternateView objHTLMAltView = AlternateView.CreateAlternateViewFromString(strBody, System.Text.Encoding.UTF8, MediaTypeNames.Text.Html);
            //    objMail.AlternateViews.Add(objHTLMAltView);
            //    if (strAttachment != string.Empty)
            //    {
            //        Attachment at = new Attachment(strAttachment);
            //        objMail.Attachments.Add(at);
            //        objMail.Priority = MailPriority.High;
            //    }

            //    objMail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            //    SmtpClient objSMTPClient = null;
            //    if (strMailPort.Trim().Length > 0)
            //        objSMTPClient = new SmtpClient(strMailHost, strMailPort.ToInt());
            //    else
            //        objSMTPClient = new SmtpClient(strMailHost);
            //    objSMTPClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            //    if (strMailSSL.ToLower() == "y")
            //        objSMTPClient.EnableSsl = true;
            //    objSMTPClient.Credentials = new System.Net.NetworkCredential(strUserId, strPassword);
            //    objSMTPClient.Send(objMail);
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    General.WriteLog("Helper", "Exception at SendMail:" + ex.Message);
            //}
            //return false;
            #endregion
        }

        public static string ConvertStringArrayToString(string[] array)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append(value);
                builder.Append(',');
            }
            return builder.ToString();
        }
    }
}
