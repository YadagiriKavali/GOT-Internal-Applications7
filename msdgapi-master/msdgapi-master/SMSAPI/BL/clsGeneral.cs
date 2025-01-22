using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Net;
using IMI.Logger;
using System.Configuration;
using System.Collections;

namespace SMSAPI.BL
{
    public class clsGeneral
    {

        public static int WebReqTimeOut = 30000;
        public static int CONNECTION_LIMIT = 10;


        /// <summary>
        ///  Purpose: Common http web request method.
        ///  Author :Raghava Reddy.K
        ///  
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="strAction"></param>
        public string DoWebRequest(string strUrl, string strAction)
        {
            string _strResponse = "";
            if (!string.IsNullOrEmpty(strUrl))
            {
                long lStartTime = DateTime.Now.Ticks;
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strUrl));
                    request.Timeout = WebReqTimeOut;
                    ServicePointManager.Expect100Continue = false;
                    ServicePointManager.DefaultConnectionLimit = CONNECTION_LIMIT;
                    ServicePointManager.UseNagleAlgorithm = false;
                    using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
                    {
                        _strResponse = reader.ReadToEnd().ToString();
                    }
                    return _strResponse;
                }
                catch (WebException WebEx)
                {
                    if (WebEx.Status == WebExceptionStatus.Timeout)
                    {
                        LogData.Write("SMSAPI", "SMSAPI", LogMode.Error, string.Format("WebRequestHandler-WebReqGetMethod->WebEx:{0}", WebEx.Message));
                    }
                    else
                    {
                        LogData.Write("SMSAPI", "SMSAPI", LogMode.Error, string.Format("WebRequestHandler-WebReqGetMethod->WebEx:{0}", WebEx.Message));
                    }

                }
                catch (Exception ex)
                {
                    LogData.Write("SMSAPI", "SMSAPI", LogMode.Error, string.Format("WebRequestHandler-WebReqGetMethod->Ex:{0}", ex.Message));

                }
                finally
                {
                    LogData.Write("SMSAPI", "SMSAPI", LogMode.Debug, string.Format("WebRequestHandler-WebReqGetMethod->Req:{0} | Res:{1} | tt: {2} ", strUrl, _strResponse, (DateTime.Now.Ticks - lStartTime) / 10000));
                }
            }
            else
            {
                // url should not blank
            }

            return "";
        }
        /// <summary>
        /// Purpose: Common HTTP POST method.
        /// Author:  Raghava Reddy.K
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="strAction"></param>
        public string DoHttpPost(string strUrl, string strAction, string sPayload)
        {
            long lStartTime = DateTime.Now.Ticks;
            WebResponse result = null;
            string _strResponse = "";
            try
            {
                WebRequest req = WebRequest.Create(strUrl);
                req.Timeout = WebReqTimeOut;
                req.Method = "POST";
                req.ContentType = "text/xml";
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
                LogData.Write("SMSAPI","TPAPI", LogMode.Excep, "WebException URL:" + strUrl + "Payload: " + sPayload + "Exception " + Ex);

            }
            catch (Exception e)
            {
                LogData.Write("SMSAPI", "TPAPI", LogMode.Excep, "Ex URL:" + strUrl + "Payload: " + sPayload + "Exception " + e);

            }
            finally
            {
                LogData.Write("SMSAPI", "TPAPI", LogMode.Debug, "Ex URL:" + strUrl + "Payload: " + sPayload + ":Res:" + _strResponse + ":tt:" + (DateTime.Now.Ticks - lStartTime) / 10000);
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
       /* public string DoHttpPost(string strUrl, string strAction, string sPayload, List<clEHeader> heades, string headers)
        {
            long lStartTime = DateTime.Now.Ticks;
            WebResponse result = null;
            string _strResponse = "";
            WebRequest req = null;
            try
            {
                req = WebRequest.Create(strUrl);
                req.Timeout = WebReqTimeOut;
                req.Method = "POST";
                req.ContentType = "application/json";
                StringBuilder UrlEncoded = new StringBuilder();
                Char[] reserved = { '?', '=', '&' };
                byte[] SomeBytes = null;

                //Put key as header
                if (heades != null)
                {
                    foreach (clEHeader hea in heades)
                        req.Headers.Add(hea.headerName, hea.headerValue);
                }

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
                LogData.Write("SMSAPI", "General", LogMode.Excep, "WebException URL:" + strUrl + "Payload: " + sPayload + "Exception " + Ex);

            }
            catch (Exception e)
            {
                LogData.Write("SMSAPI", "General", LogMode.Excep, "Ex URL:" + strUrl + "Payload: " + sPayload + "Exception " + e);

            }
            finally
            {
                LogData.Write("SMSAPI", "General", LogMode.Debug, "Ex URL:" + strUrl + "Payload: " + sPayload + ":Res:" + _strResponse + ":tt:" + (DateTime.Now.Ticks - lStartTime) / 10000 + ":headers:" + headers);
            }
            return _strResponse;
        }

        */


        /// <summary>
        /// Purpose: Common HTTP POST method contentype Jsons.
        /// Author:  SARATH CHANDRA K
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="strAction"></param>
        /// <param name="sPayload"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string DoHttpPost(string strUrl, string strAction, string sPayload, string skey)
        {
            long lStartTime = DateTime.Now.Ticks;
            WebResponse result = null;
            string _strResponse = "";
            try
            {
                WebRequest req = WebRequest.Create(strUrl);
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
                LogData.Write("SMSAPI", "General", LogMode.Excep, "WebException URL:" + strUrl + "Payload: " + sPayload + "Exception " + Ex);

            }
            catch (Exception e)
            {
                LogData.Write("SMSAPI", "General", LogMode.Excep, "Ex URL:" + strUrl + "Payload: " + sPayload + "Exception " + e);

            }
            finally
            {
                LogData.Write("SMSAPI", "General", LogMode.Debug, "Ex URL:" + strUrl + "Payload: " + sPayload + ":Res:" + _strResponse + ":tt:" + (DateTime.Now.Ticks - lStartTime) / 10000);
            }
            return _strResponse;
        }
        /// <summary>
        /// Purpose: Common OBD API
        /// Author : Raghaa Reddy.K
        /// </summary>
        /// <param name="strMsisdn"></param>
        /// <param name="strCFid"></param>
        /// <param name="strKey"></param>
        public void OBDCall(string strMsisdn, string strCFid, string strKey)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }
        public void DebugLogs()
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// Get value from configuration file.
        /// </summary>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string GetConfigValue(string sKey)
        {
            return ConfigurationManager.AppSettings[sKey] ?? "";
        }

        /// <summary>
        /// This method is to get the formmated msisdn
        /// </summary>
        /// <param name="msisdn"></param>
        /// <returns></returns>
        public string getMsisdn(string msisdn)
        {
            try
            {
                if (msisdn.Length == 10)
                    msisdn = string.Format("{0}{1}", "91", msisdn);
                else if (msisdn.Length == 11)
                    msisdn = string.Format("{0}{1}", "91", msisdn.Substring(1));
            }
            catch (Exception ex)
            {
                LogData.Write("SMSAPI", "General", LogMode.Excep, ex, "SMSAPI - clGeneral- getMsisdn " + msisdn);
            }
            finally
            {

            }
            return msisdn;
        }

        /// <summary>
        /// This method is to get the formmated msisdn
        /// </summary>
        /// <param name="msisdn"></param>
        /// <returns></returns>
        public string getFormattedMsisdn(string msisdn)
        {
            try
            {
                if (msisdn.Length > 10)
                    msisdn = msisdn.Substring(msisdn.Length - 10, 10);
                if (msisdn.Length == 10)
                    msisdn = string.Format("{0}{1}", "91", msisdn);
                else if (msisdn.Length == 11)
                    msisdn = string.Format("{0}{1}", "91", msisdn.Substring(1));
            }
            catch (Exception ex)
            {
                LogData.Write("SMSAPI", "General", LogMode.Excep, ex, "SMSAPI - clGeneral- getMsisdn " + msisdn);
            }
            finally
            {

            }
            return msisdn;
        }

        /// <summary>
        /// This method is to get the formmated msisdn for IVR
        /// </summary>
        /// <param name="msisdn"></param>
        /// <returns></returns>
        public string getMsisdnforIVR(string msisdn)
        {
            try
            {
                if (msisdn.Length == 10)
                    msisdn = string.Format("{0}{1}", "0", msisdn);
                else if (msisdn.Length == 11)
                    msisdn = string.Format("{0}{1}", "0", msisdn.Substring(1));
            }
            catch (Exception ex)
            {
                LogData.Write("SMSAPI", "General", LogMode.Excep, ex, "SMSAPI - clGeneral- getMsisdn " + msisdn);
            }
            finally
            {

            }
            return msisdn;
        }

        /// <summary>
        /// This method is to get the formmated msisdn
        /// </summary>
        /// <param name="msisdn"></param>
        /// <returns></returns>
        public string setMsisdnto10digit(string msisdn)
        {
            try
            {
                if (msisdn.Length == 11)
                    msisdn = msisdn.Substring(1);
                else if (msisdn.Length == 12)
                    msisdn = msisdn.Substring(2);
            }
            catch (Exception ex)
            {
                LogData.Write("SMSAPI", "General", LogMode.Excep, ex, "SMSAPI - clGeneral- getMsisdn " + msisdn);
            }
            finally
            {

            }
            return msisdn;
        }

        /// <summary>
        /// This method is to get the formmated msisdn
        /// </summary>
        /// <param name="msisdn"></param>
        /// <returns></returns>
        public string setMsisdnto11digit(string msisdn)
        {
            try
            {
                if (msisdn.Length == 12)
                    msisdn = string.Format("0{0}", msisdn.Substring(2));
                else if (msisdn.Length == 10)
                    msisdn = string.Format("0{0}", msisdn);
            }
            catch (Exception ex)
            {
                LogData.Write("SMSAPI", "General", LogMode.Excep, ex, "SMSAPI - clGeneral- getMsisdn " + msisdn);
            }
            finally
            {

            }
            return msisdn;
        }

        /// <summary>
        /// To Send the email
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
    /*    public void sendEmail(string subject, string body, string controller)
        {
            clEEmail email = null;
            clEmailXML exml = null;
            string shostaddress = string.Empty;
            string userid = string.Empty;
            string password = string.Empty;
            try
            {
                exml = new clEmailXML();
                if (exml.checkCotroller(controller))
                {
                    email = new clEEmail()
                    {
                        from = exml.getFromAddress(controller),
                        to = exml.getToAddress(controller),
                        subject = exml.getSubject(controller).Replace("$(SUBJECT)", subject),
                        body = exml.getBody(controller).Replace("$(BODY)", body)
                    };

                    exml.getSettings(controller, ref shostaddress, ref userid, ref password);
                    using (MailMessage oMail1 = new MailMessage(email.from, email.to, email.subject, email.body))
                    {

                        oMail1.IsBodyHtml = true;
                        SmtpClient smtp1 = new SmtpClient(shostaddress);
                        smtp1.Timeout = 100;

                        smtp1.UseDefaultCredentials = false;
                        smtp1.Credentials = new System.Net.NetworkCredential(userid, password, "");
                        smtp1.Send(oMail1);
                    }
                }
                else
                    LogData.Write("SMSAPI", "General", LogMode.Error, controller + "Not defined in EMAIL.xml.");

            }
            catch (Exception ex)
            {
                LogData.Write("SMSAPI", "General", LogMode.Excep, ex, "GenAPI - clGeneral- getMsisdn " + email.ToString());
            }
            finally
            {
                exml = null;
            }
        }*/
        public static string GetConfigVal(string strTag)
        {
            try
            {
                if (ConfigurationManager.AppSettings[strTag] != null)
                    return ConfigurationManager.AppSettings[strTag].ToString().Trim();
                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine("error" + ex.Message);
                return "";
            }
        }

        public static string GetConfig(string strTag)
        {
            try
            {
                return ConfigurationManager.AppSettings[strTag] ?? null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error" + ex.Message);
                return null;
            }
        }

        public static int ConvertInt(string strVal)
        {
            int intValue;
            int.TryParse(strVal, out intValue);
            return intValue;
        }

        public static decimal ConvertDecimal(string strVal)
        {
            decimal dValue;
            decimal.TryParse(strVal, out dValue);
            return dValue;
        }

        public static long ConvertLong(string sVal)
        {
            long lValue;
            long.TryParse(sVal, out lValue);
            return lValue;
        }

        public static DateTime ConvertDateTime(string sVal)
        {
            DateTime dtDate = DateTime.Now;
            if (!DateTime.TryParse(sVal, out dtDate))
                return DateTime.Now;
            return dtDate;
        }
        public static DateTime DateTimeFailedToMin(string sVal)
        {
            DateTime dtDate = DateTime.Now;
            DateTime.TryParse(sVal, out dtDate);
            return dtDate;
        }

        public static String DoHttpWebRequest(String strURL, String strData, String strMethod, String strContentType, Hashtable htHeaders, int iTimeOut)
        {
            long lTimeTaken = -1, lStart = DateTime.Now.Ticks;
            HttpWebRequest objReq;
            HttpWebResponse objRes = null;
            StreamReader objSr = null;
            string strRetVal = string.Empty;
            try
            {
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
                // objReq.KeepAlive = IsKeepAlive;

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
                TextLog.Exception("SMSAPI", "DoHttpWebRequest_EX", string.Format("DoHttpWebRequest::Url:{1}{0}Request:{5}{0}Response{6}{0}Timetaken:{2}{0}WebException Message:{3}{0}.StackTrace{4}{0}", Environment.NewLine, strURL, lTimeTaken.ToStr(), webex.Message, webex.StackTrace, strData, strRetVal));
                //return "Dear User, currently we are unable to process your request. Please try after sometime.";
                return "";
            }
            catch (Exception ex)
            {
                TextLog.Exception("SMSAPI", "DoHttpWebRequest_EX", string.Format("DoHttpWebRequest::Url:{1}{0}Request:{5}{0}Response{6}{0}Timetaken:{2}{0}WebException Message:{3}{0}.StackTrace{4}{0}", Environment.NewLine, strURL, lTimeTaken.ToStr(), ex.Message, ex.StackTrace, strData, strRetVal));
                return "";
            }
            finally
            {
                if (lStart > 0)
                    lTimeTaken = (DateTime.Now.Ticks - lStart) / 10000;
                if (objSr != null) objSr.Close();
                if (objRes != null) objRes.Close();
                objReq = null;
                objRes = null;
                objSr = null;
                HttpContext.Current.Response.AppendToLog("&url=" + strURL + "&tt=" + lTimeTaken.ToString());
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
                if (!String.IsNullOrEmpty(GetConfigVal("REQ_TIMEOUT")))
                    objReq.Timeout = int.Parse(GetConfigVal("REQ_TIMEOUT"));
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
            catch (Exception ex)
            {
                TextLog.Exception("SMSAPI", "DoPostRequest_ex", "url:" + strURL + Environment.NewLine + "Message:" + ex.Message);
                return "";
            }
            finally
            {
                objReq = null;
                objRes = null;
                objSr = null;
            }
        }
    }
}