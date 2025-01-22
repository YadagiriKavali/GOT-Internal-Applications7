using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using System.Configuration;
using IMI.Logger;
using System.Net.Mail;
using System.Data;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Reflection;
using System.Globalization;


    public class clgeneral
    {

        public static int WebReqTimeOut = 30000;
        public static int CONNECTION_LIMIT = 10;


        /// <summary>
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="strAction"></param>
        public static string DoWebRequest(string strUrl, string strAction)
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
                        LogData.Write("TwalletIVRS_bal", "TwalletIVRS_bal", LogMode.Error, string.Format("WebRequestHandler-WebReqGetMethod->WebEx:{0}", WebEx.Message));
                    }
                    else
                    {
                        LogData.Write("TwalletIVRS_bal", "TwalletIVRS_bal", LogMode.Error, string.Format("WebRequestHandler-WebReqGetMethod->WebEx:{0}", WebEx.Message));
                    }

                }
                catch (Exception ex)
                {
                    LogData.Write("TwalletIVRS_bal", "TwalletIVRS_bal", LogMode.Error, string.Format("WebRequestHandler-WebReqGetMethod->Ex:{0}", ex.Message));

                }
                finally
                {
                    LogData.Write("TwalletIVRS_bal", "TwalletIVRS_bal", LogMode.Debug, string.Format("WebRequestHandler-WebReqGetMethod->Req:{0} | Res:{1} | tt: {2} ", strUrl, _strResponse, (DateTime.Now.Ticks - lStartTime) / 10000));
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
                    ServicePointManager.UseNagleAlgorithm = false;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    // ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                //    ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    // ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
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
                LogData.Write("TwalletIVRS_bal", "TwalletIVRS_bal", LogMode.Excep, "WebException URL:" + strUrl + "Payload: " + sPayload + "Exception " + Ex);

            }
            catch (Exception e)
            {
                LogData.Write("TwalletIVRS_bal", "TwalletIVRS_bal", LogMode.Excep, "Ex URL:" + strUrl + "Payload: " + sPayload + "Exception " + e);

            }
            finally
            {
                LogData.Write("TwalletIVRS_bal", "TwalletIVRS_bal", LogMode.Debug, "Ex URL:" + strUrl + "Payload: " + sPayload + ":Res:" + _strResponse + ":tt:" + (DateTime.Now.Ticks - lStartTime) / 10000);
            }
            return _strResponse;
        }

        /// <summary>
        /// ENT_API calling the sms to push sms
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="strAction"></param>
        /// <param name="sPayload"></param>
        /// <param name="credentials"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string DoHttpPost_ENT(string strUrl, string sPayload, string credentials, string key)
        {
            long lStartTime = DateTime.Now.Ticks;
            WebResponse result = null;
            string _strResponse = "";
            try
            {
                WebRequest req = WebRequest.Create(strUrl);
                req.Timeout = WebReqTimeOut;
                req.Method = "POST";
                req.ContentType = "application/json";
                req.Headers.Add("Authorization", "Basic " + credentials);
                req.Headers.Add("key", key);
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
                LogData.Write("TwalletIVRS_bal", "TwalletIVRS_bal", LogMode.Excep, "WebException URL:" + strUrl + "Payload: " + sPayload + "Exception " + Ex);

            }
            catch (Exception e)
            {
                LogData.Write("TwalletIVRS_bal", "TwalletIVRS_bal", LogMode.Excep, "Ex URL:" + strUrl + "Payload: " + sPayload + "Exception " + e);

            }
            finally
            {
                LogData.Write("TwalletIVRS_bal", "TwalletIVRS_bal", LogMode.Debug, "Ex URL:" + strUrl + "Payload: " + sPayload + ":Res:" + _strResponse + ":tt:" + (DateTime.Now.Ticks - lStartTime) / 10000);
            }
            return _strResponse;
        }

        /// <summary>
        //Purpose: this http web request method for getting customer data from client api.
        ///  Author :anil kumar.b
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="strAction"></param>
        /// <returns></returns>
        public static string DoHttpGet_customer(string strUrl, string credentials, string key)
        {
            string _strResponse = "";
            if (!string.IsNullOrEmpty(strUrl))
            {
                long lStartTime = DateTime.Now.Ticks;
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strUrl));
                    request.Timeout = WebReqTimeOut;
                    request.Headers.Add("Authorization", "Basic " + credentials);
                    request.Headers.Add("key", key);
                    request.Method = "POST";
                    request.ContentLength = 0;
                    ServicePointManager.Expect100Continue = false;
                    ServicePointManager.DefaultConnectionLimit = CONNECTION_LIMIT;
                    ServicePointManager.UseNagleAlgorithm = false;
                    using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
                    {
                        _strResponse = reader.ReadToEnd().ToString();
                    }

                }
                catch (WebException WebEx)
                {
                    if (WebEx.Status == WebExceptionStatus.Timeout)
                    {
                        LogData.Write("clsGeneral", "DoHttpGet_smstatus", LogMode.Error, string.Format("WebRequestHandler-WebReqGetMethod->WebEx:{0}", WebEx.Message));
                    }
                    else
                    {
                        LogData.Write("clsGeneral", "DoHttpGet_smstatus", LogMode.Error, string.Format("WebRequestHandler-WebReqGetMethod->WebEx:{0}", WebEx.Message));
                    }

                }
                catch (Exception ex)
                {
                    LogData.Write("clsGeneral", "DoHttpGet_smstatus", LogMode.Error, string.Format("WebRequestHandler-WebReqGetMethod->Ex:{0}", ex.Message));

                }
                finally
                {
                    LogData.Write("clsGeneral", "DoHttpGet_smstatus", LogMode.Debug, string.Format("WebRequestHandler-WebReqGetMethod->Req:{0} | Res:{1} | tt: {2} ", strUrl, _strResponse, (DateTime.Now.Ticks - lStartTime) / 10000));
                }
            }
            else
            {
                // url should not blank
            }

            return _strResponse;
        }

        /// <summary>
        //Purpose: this http web request method for getting sms status.
        ///  Author :anil kumar.b
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="strAction"></param>
        /// <returns></returns>
        public static string DoHttpGet_smstatus(string strUrl, string credentials, string key)
        {
            string _strResponse = "";
            if (!string.IsNullOrEmpty(strUrl))
            {
                long lStartTime = DateTime.Now.Ticks;
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strUrl));
                    request.Timeout = WebReqTimeOut;
                    request.Headers.Add("Authorization", "Basic " + credentials);
                    request.Headers.Add("key", key);
                    ServicePointManager.Expect100Continue = false;
                    ServicePointManager.DefaultConnectionLimit = CONNECTION_LIMIT;
                    ServicePointManager.UseNagleAlgorithm = false;
                    using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
                    {
                        _strResponse = reader.ReadToEnd().ToString();
                    }

                }
                catch (WebException WebEx)
                {
                    if (WebEx.Status == WebExceptionStatus.Timeout)
                    {
                        LogData.Write("clsGeneral", "DoHttpGet_smstatus", LogMode.Error, string.Format("WebRequestHandler-WebReqGetMethod->WebEx:{0}", WebEx.Message));
                    }
                    else
                    {
                        LogData.Write("clsGeneral", "DoHttpGet_smstatus", LogMode.Error, string.Format("WebRequestHandler-WebReqGetMethod->WebEx:{0}", WebEx.Message));
                    }

                }
                catch (Exception ex)
                {
                    LogData.Write("clsGeneral", "DoHttpGet_smstatus", LogMode.Error, string.Format("WebRequestHandler-WebReqGetMethod->Ex:{0}", ex.Message));

                }
                finally
                {
                    LogData.Write("clsGeneral", "DoHttpGet_smstatus", LogMode.Debug, string.Format("WebRequestHandler-WebReqGetMethod->Req:{0} | Res:{1} | tt: {2} ", strUrl, _strResponse, (DateTime.Now.Ticks - lStartTime) / 10000));
                }
            }
            else
            {
                // url should not blank
            }

            return _strResponse;
        }

        public static string DoHttpGet_mcall(string strUrl)
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

                }
                catch (WebException WebEx)
                {
                    if (WebEx.Status == WebExceptionStatus.Timeout)
                    {
                        LogData.Write("clsGeneral", "DoHttpGet_smstatus", LogMode.Error, string.Format("WebRequestHandler-WebReqGetMethod->WebEx:{0}", WebEx.Message));
                    }
                    else
                    {
                        LogData.Write("clsGeneral", "DoHttpGet_smstatus", LogMode.Error, string.Format("WebRequestHandler-WebReqGetMethod->WebEx:{0}", WebEx.Message));
                    }

                }
                catch (Exception ex)
                {
                    LogData.Write("clsGeneral", "DoHttpGet_smstatus", LogMode.Error, string.Format("WebRequestHandler-WebReqGetMethod->Ex:{0}", ex.Message));

                }
                finally
                {
                    LogData.Write("clsGeneral", "DoHttpGet_smstatus", LogMode.Debug, string.Format("WebRequestHandler-WebReqGetMethod->Req:{0} | Res:{1} | tt: {2} ", strUrl, _strResponse, (DateTime.Now.Ticks - lStartTime) / 10000));
                }
            }
            else
            {
                // url should not blank
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

                //Put key as header
                if (!string.IsNullOrEmpty(skey))
                    req.Headers.Add("key", skey);

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
                LogData.Write("TwalletIVRS_bal", "General", LogMode.Excep, "WebException URL:" + strUrl + "Payload: " + sPayload + "Exception " + Ex);

            }
            catch (Exception e)
            {
                LogData.Write("TwalletIVRS_bal", "General", LogMode.Excep, "Ex URL:" + strUrl + "Payload: " + sPayload + "Exception " + e);

            }
            finally
            {
                LogData.Write("TwalletIVRS_bal", "General", LogMode.Debug, "Ex URL:" + strUrl + "Payload: " + sPayload + ":Res:" + _strResponse + ":tt:" + (DateTime.Now.Ticks - lStartTime) / 10000);
            }
            return _strResponse;
        }

        /// <summary>
        /// Purpose: Common SEND SMS
        /// Author:  Raghava Reddy.K 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="key"></param>
        /// <param name="msisdn"></param>


        /// <summary>
        /// To get the sms uuid from the response
        /// </summary>
        /// <param name="jsondata"></param>
        /// <returns></returns>
        //private string getUUID (string jsondata)
        //{
        //    clESMSResp oresp = null;
        //    string uuid = string.Empty;
        //    try
        //    {
        //        oresp = (clESMSResp)JsonConvert.DeserializeObject(jsondata, typeof(clESMSResp));
        //        if (oresp != null)
        //        {
        //            if (oresp.outboundSMSMessageRequest != null && !string.IsNullOrEmpty(oresp.outboundSMSMessageRequest.resourceURL))
        //            {
        //                int iLength = oresp.outboundSMSMessageRequest.resourceURL.Split('/').Length;
        //                if (iLength - 1 > 0)
        //                    uuid = oresp.outboundSMSMessageRequest.resourceURL.Split('/')[iLength - 1];
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogData.Write("TwalletIVRS_bal", "General", LogMode.Excep, ex, "getUUID " + jsondata);
        //    }
        //    return uuid;
        //}


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
            return ConfigurationSettings.AppSettings[sKey] ?? "";
        }

        /// <summary>
        /// This method is to get the formmated msisdn
        /// </summary>
        /// <param name="msisdn"></param>
        /// <returns></returns>
        public static string getMsisdn(string msisdn)
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
                LogData.Write("TwalletIVRS_bal", "General", LogMode.Excep, ex, "TwalletIVRS_bal - clGeneral- getMsisdn " + msisdn);
            }
            finally
            {

            }
            return msisdn;
        }
        public string getMsisdnwithcountrycode(string msisdn, string countrycode)
        {
            try
            {
                if (msisdn.Length == 10)
                    msisdn = string.Format("{0}{1}", countrycode, msisdn);
                else if (msisdn.Length == 11)
                    msisdn = string.Format("{0}{1}", countrycode, msisdn.Substring(1));
            }
            catch (Exception ex)
            {
                LogData.Write("TwalletIVRS_bal", "General", LogMode.Excep, ex, "TwalletIVRS_bal - clGeneral- getMsisdn " + msisdn);
            }
            finally
            {

            }
            return msisdn;
        }
        public string getMsisdnwithcountrycode_internationl(string msisdn, string countrycode)
        {
            try
            {
                msisdn = string.Format("{0}{1}", countrycode, msisdn);
            }
            catch (Exception ex)
            {
                LogData.Write("TwalletIVRS_bal", "General", LogMode.Excep, ex, "TwalletIVRS_bal - clGeneral- getMsisdn " + msisdn);
            }
            finally
            {

            }
            return msisdn;
        }
        public bool validatemsisdn(string msisdn)
        {
            bool bmsisdn = false;
            try
            {
                bmsisdn = Regex.IsMatch(msisdn, @"^\d{10}$");
            }
            catch (Exception ex)
            {
                LogData.Write("TwalletIVRS_bal", "General", LogMode.Excep, ex, "TwalletIVRS_bal - clGeneral- validatemsisdn " + msisdn);
            }
            finally
            {

            }
            return bmsisdn;
        }
 
        public bool validateemail(string emaiid)
        {
            bool bmailid = false;
            try
            {
                //string[] strArrMailId = emaiid.Split(',');
                //for (int iMailId = 0; iMailId < strArrMailId.Length; iMailId++)
                //{
                if (!string.IsNullOrEmpty(emaiid))
                {
                    bmailid = Regex.IsMatch(emaiid, @"^[-a-z0-9_}{\'?]+(\.[-a-z0-9~!$%^&*_=+}{\'?]+)*@([a-z0-9_][-a-z0-9_]*(\.[-a-z0-9_]+)*\.(aero|arpa|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org|pro|travel|mobi|[a-z][a-z])|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?$", RegexOptions.IgnoreCase);
                }
                // }
                //bmailid = Regex.IsMatch(emaiid, @"\A(?:[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?)\Z");
            }
            catch (Exception ex)
            {
                LogData.Write("TwalletIVRS_bal", "General", LogMode.Excep, ex, "TwalletIVRS_bal - clGeneral- validateemail " + emaiid);
            }
            finally
            {

            }
            return bmailid;
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
                LogData.Write("TwalletIVRS_bal", "General", LogMode.Excep, ex, "TwalletIVRS_bal - clGeneral- getMsisdn " + msisdn);
            }
            finally
            {

            }
            return msisdn;
        }
        public static void WriteLog(string sSubModule, string sFileName, LogMode logMode, string sMessage)
        {
            try
            {
                if (LogMode.Debug == logMode || LogMode.Info == logMode || LogMode.Audit == logMode)
                {
                    if (string.Compare(GetConfigValue("LOGSTATUS"), "Y", true) != 0)
                        return;
                }
                LogData.Write(sSubModule, sFileName, logMode, sMessage);

            }
            catch (Exception ex)
            {
            }
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
                LogData.Write("TwalletIVRS_bal", "General", LogMode.Excep, ex, "TwalletIVRS_bal - clGeneral- getMsisdn " + msisdn);
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
                LogData.Write("TwalletIVRS_bal", "General", LogMode.Excep, ex, "TwalletIVRS_bal - clGeneral- getMsisdn " + msisdn);
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
                LogData.Write("TwalletIVRS_bal", "General", LogMode.Excep, ex, "TwalletIVRS_bal - clGeneral- getMsisdn " + msisdn);
            }
            finally
            {

            }
            return msisdn;
        }

        public static string genrateotp()
        {
            string resp = string.Empty;
            try
            {
                Random generator = new Random();
                resp = generator.Next(0, 1000000).ToString("D6");
            }
            catch (Exception ex)
            {

                return resp = "-1";
            }
            return resp;
        }
        public static string WebReqPostingData(string sUrl, string sPayload)
        {
            long lStartTime = DateTime.Now.Ticks;
            string strResponse = "";
            StringBuilder strBReqRes = new StringBuilder();
            try
            {
                strBReqRes.Append(string.Format("Url: {0} Payload:{1}", sUrl, sPayload));
                HttpWebRequest httpWebreq = (HttpWebRequest)WebRequest.Create(sUrl);
                httpWebreq.Method = "POST";
                httpWebreq.Timeout = WebReqTimeOut;
                httpWebreq.ContentType = "application/json";
                // httpWebreq.Headers.Add("Key", sServiceKey);
                byte[] bReqBytes = null;
                try
                {
                    ServicePointManager.DefaultConnectionLimit = CONNECTION_LIMIT;
                }
                catch { }

                if (!string.IsNullOrEmpty(sPayload))
                {
                    bReqBytes = Encoding.UTF8.GetBytes(FormatPayLoad(sPayload));
                    httpWebreq.ContentLength = bReqBytes.Length;
                    using (Stream newStream = httpWebreq.GetRequestStream())
                    {
                        newStream.Write(bReqBytes, 0, bReqBytes.Length);
                    }
                }
                else
                {
                    httpWebreq.ContentLength = 0;
                }
                strResponse = ReadWebResponse((HttpWebResponse)httpWebreq.GetResponse());
                strBReqRes.Append(strResponse);
            }
            catch (WebException Ex)
            {
                if (Ex.Response != null)
                {
                    try
                    {
                        using (WebResponse response = Ex.Response)
                        {
                            HttpWebResponse httpResponse = (HttpWebResponse)response;
                            using (Stream data = response.GetResponseStream())
                            {
                                using (var reader = new StreamReader(data))
                                {
                                    strResponse = reader.ReadToEnd();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                         LogData.Write("", "", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException::" + ex.Message);
                    }
                }
                else
                {
                     LogData.Write("", "", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException:" + Ex.Message);
                }

                // get status code 
                if (Ex.Status == WebExceptionStatus.Timeout)
                {
                     LogData.Write("", "", LogMode.Excep, "WebRequestingProcess-WebReqPostingData WebException:Timeout" + Ex.Message);
                }
                else if (Ex.Status == WebExceptionStatus.ConnectFailure || Ex.Status == WebExceptionStatus.ConnectionClosed || Ex.Status == WebExceptionStatus.KeepAliveFailure
                    || Ex.Status == WebExceptionStatus.Pending)
                {
                     LogData.Write("", "", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException:Connection" + Ex.Message);
                }
                else
                {
                     LogData.Write("", "", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException" + Ex.Message);
                }

            }
            catch (Exception ex)
            {
                 LogData.Write("", "", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -Exception" + ex.Message);

            }
            finally
            {
                strBReqRes.Append("TimeTaken:" + (DateTime.Now.Ticks - lStartTime) / 10000);
                 LogData.Write("", "", LogMode.Debug, strBReqRes.ToString());
            }
            return strResponse + "|" + strBReqRes + "|" + ((DateTime.Now.Ticks - lStartTime) / 10000);
        }

        public static string WebReqPost(string sUrl, string sPayload)
        {
            long lStartTime = DateTime.Now.Ticks;
            string strResponse = "";
            StringBuilder strBReqRes = new StringBuilder();
            try
            {
                strBReqRes.Append(string.Format("Url: {0} Payload:{1}", sUrl, sPayload));
                HttpWebRequest httpWebreq = (HttpWebRequest)WebRequest.Create(sUrl);
                httpWebreq.Method = "POST";
                httpWebreq.Timeout = WebReqTimeOut;
                httpWebreq.ContentType = "application/json";
                // httpWebreq.Headers.Add("Key", sServiceKey);
                byte[] bReqBytes = null;
                try
                {
                    ServicePointManager.DefaultConnectionLimit = CONNECTION_LIMIT;
                }
                catch { }

                if (!string.IsNullOrEmpty(sPayload))
                {
                    bReqBytes = Encoding.UTF8.GetBytes(FormatPayLoad(sPayload));
                    httpWebreq.ContentLength = bReqBytes.Length;
                    using (Stream newStream = httpWebreq.GetRequestStream())
                    {
                        newStream.Write(bReqBytes, 0, bReqBytes.Length);
                    }
                }
                else
                {
                    httpWebreq.ContentLength = 0;
                }
                strResponse = ReadWebResponse((HttpWebResponse)httpWebreq.GetResponse());
                strBReqRes.Append(strResponse);
            }
            catch (WebException Ex)
            {
                if (Ex.Response != null)
                {
                    try
                    {
                        using (WebResponse response = Ex.Response)
                        {
                            HttpWebResponse httpResponse = (HttpWebResponse)response;
                            using (Stream data = response.GetResponseStream())
                            {
                                using (var reader = new StreamReader(data))
                                {
                                    strResponse = reader.ReadToEnd();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                         LogData.Write("", "", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException::" + ex.Message);
                    }
                }
                else
                {
                     LogData.Write("", "", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException:" + Ex.Message);
                }

                // get status code 
                if (Ex.Status == WebExceptionStatus.Timeout)
                {
                     LogData.Write("", "", LogMode.Excep, "WebRequestingProcess-WebReqPostingData WebException:Timeout" + Ex.Message);
                }
                else if (Ex.Status == WebExceptionStatus.ConnectFailure || Ex.Status == WebExceptionStatus.ConnectionClosed || Ex.Status == WebExceptionStatus.KeepAliveFailure
                    || Ex.Status == WebExceptionStatus.Pending)
                {
                     LogData.Write("", "", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException:Connection" + Ex.Message);
                }
                else
                {
                     LogData.Write("", "", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException" + Ex.Message);
                }

            }
            catch (Exception ex)
            {
                 LogData.Write("", "", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -Exception" + ex.Message);

            }
            finally
            {
                strBReqRes.Append("TimeTaken:" + (DateTime.Now.Ticks - lStartTime) / 10000);
                 LogData.Write("", "", LogMode.Debug, strBReqRes.ToString());
            }
            return strResponse;
        }






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
        public static string ReadWebResponse(HttpWebResponse httpWebResponse)
        {
            string strResponse = "";
            try
            {
                if (httpWebResponse != null)
                {
                    Encoding encodeRes = Encoding.GetEncoding("utf-8");
                    using (var strmReader = new StreamReader(httpWebResponse.GetResponseStream(), encodeRes))
                    {
                        Char[] read = new Char[256];
                        int count = strmReader.Read(read, 0, 256);
                        StringBuilder sb = new StringBuilder(500);
                        while (count > 0)
                        {
                            String str = new String(read, 0, count);
                            sb.Append(str);
                            count = strmReader.Read(read, 0, 256);
                        }
                        strResponse = sb.ToString();
                    }
                }
            }
            catch (WebException Ex)
            {
                if (Ex.Response != null)
                {
                    try
                    {
                        using (WebResponse response = Ex.Response)
                        {
                            HttpWebResponse httpResponse = (HttpWebResponse)response;
                            using (Stream data = response.GetResponseStream())
                            {
                                using (var reader = new StreamReader(data))
                                {
                                    strResponse = reader.ReadToEnd();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                         LogData.Write("", "", LogMode.Excep, "ReadWebResponse:WebException::" + ex.Message);
                    }
                }
                else
                {
                     LogData.Write("", "", LogMode.Excep, "ReadWebResponse:WebException:" + Ex.Message);
                }

                // get status code 
                if (Ex.Status == WebExceptionStatus.Timeout)
                {
                     LogData.Write("", "", LogMode.Excep, "ReadWebResponse:Timeout" + Ex.Message);
                }
                else if (Ex.Status == WebExceptionStatus.ConnectFailure || Ex.Status == WebExceptionStatus.ConnectionClosed || Ex.Status == WebExceptionStatus.KeepAliveFailure
                    || Ex.Status == WebExceptionStatus.Pending)
                {
                     LogData.Write("", "", LogMode.Excep, "ReadWebResponse:Connection-" + Ex.Message);
                }
                else
                {
                     LogData.Write("", "", LogMode.Excep, "ReadWebResponse:other:" + Ex.Message);
                }

            }
            catch (Exception ex)
            {
                 LogData.Write("", "", LogMode.Excep, "ReadWebResponse:General-" + ex.Message);
            }
            return strResponse;
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
        public static string FormatPayLoad(string sPayload)
        {
            Char[] chrReserved = { '?', '=', '&' };
            StringBuilder strBUrlEncoded = new StringBuilder();
            try
            {
                if (!string.IsNullOrEmpty(sPayload))
                {
                    int i = 0, j;
                    while (i < sPayload.Length)
                    {
                        j = sPayload.IndexOfAny(chrReserved, i);
                        if (j == -1)
                        {
                            strBUrlEncoded.Append(sPayload.Substring(i, sPayload.Length - i));
                            break;
                        }
                        strBUrlEncoded.Append(sPayload.Substring(i, j - i));
                        strBUrlEncoded.Append(sPayload.Substring(j, 1));
                        i = j + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                 LogData.Write("", "", LogMode.Excep, "FormatPayLoad:Exception:" + ex.Message);
            }

            return strBUrlEncoded.ToString();
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

        public static string ConvertOraDateTime(string sVal)
        {
            DateTime dtDate = DateTime.Now;
            if (DateTime.TryParse(sVal, out dtDate))
            {
                return dtDate.ToString("dd MMM yyyy HH:mm:ss");
            }
            else
            {
                return dtDate.ToString("dd MMM yyyy HH:mm:ss");
            }
        }

        public static bool ConvertDateTime(string sVal, out DateTime dtDate)
        {
            dtDate = DateTime.Now;
            if (!DateTime.TryParse(sVal, out dtDate))
                return false;
            return true;
        }
        public static DateTime DateTimeFailedToMin(string sVal)
        {
            DateTime dtDate = DateTime.Now;
            DateTime.TryParse(sVal, out dtDate);
            return dtDate;
        }
        public bool IsBase64String(string str)
        {
            try
            {
                // If not exception is cought, then it is a base64 string
                MemoryStream stream = new MemoryStream(Convert.FromBase64String(str));
                return true;
            }
            catch
            {
                // If exception is cought, then I assumed it is a normal string
                return false;
            }
        }

        public void convertmemorytofile(string filepath, string attachments)
        {
            try
            {
                string filename = string.Empty;
                byte[] sPDFDecoded = Convert.FromBase64String(attachments);
                using (var imageFile = new FileStream(filepath, FileMode.Create))
                {
                    imageFile.Write(sPDFDecoded, 0, sPDFDecoded.Length);
                    imageFile.Flush();
                }
            }
            catch (Exception ex)
            {
                LogData.Write("TwalletIVRS_bal", "clsgeneral", LogMode.Excep, ex, "convertmemorytofile");
            }

        }
        public static string GetCustomHeaderValue(HttpRequestMessage request, string sHeaderName)
        {
            IEnumerable<string> sHeaderValue;
            try
            {
                if (request.Headers.TryGetValues(sHeaderName, out sHeaderValue))
                {
                    return sHeaderValue.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

            }
            return "";

        }

        public static string GetDictionaryKeyValue(Dictionary<string, string> dic, string sKey)
        {

            try
            {
                if (dic != null && dic.ContainsKey(sKey))
                {
                    return dic[sKey] as string;
                }
            }
            catch (Exception ex)
            {

            }
            return "0";

        }

    

        //public static string GetUniqueCode(int MaxLength)
        //{
        //    string _strUniqueCode = string.Format("{0}{1}", DateTime.Now.ToString("MMddHHmm"), clsGeneral.GenerateRandomSessionNo(2));
        //    try
        //    {
        //        string sYear = DateTime.Now.Year.ToString().Substring(2);
        //        sYear = sYear.Substring(1);
        //        _strUniqueCode = string.Format("{0}{1}{2}{3}{4}", sYear, int.Parse(DateTime.Now.Month.ToString()), int.Parse(DateTime.Now.Day.ToString()), int.Parse(DateTime.Now.Hour.ToString()), int.Parse(DateTime.Now.Minute.ToString()));
        //        if (_strUniqueCode.Length < MaxLength)
        //        {
        //            _strUniqueCode = string.Format("{0}{1}", _strUniqueCode, GenerateRandomSessionNo(MaxLength - _strUniqueCode.Length));
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    finally
        //    {
        //        if (_strUniqueCode.Length > MaxLength)
        //            _strUniqueCode = _strUniqueCode.Substring(0, 10);
        //    }
        //    return _strUniqueCode;
        //}
        //public static string GenerateUniqueCode(int MaxLength)
        //{
        //    string _strUniqueCode = string.Format("{0}{1}", DateTime.Now.ToString("MMddHHmm"), clsGeneral.GenerateRandomSessionNo(2));
        //    try
        //    {
        //        string sYear = DateTime.Now.Year.ToString().Substring(2);
        //        sYear = sYear.Substring(1);
        //        _strUniqueCode = string.Format("{0}{1}{2}{3}{4}{5}", sYear, int.Parse(DateTime.Now.Month.ToString()), int.Parse(DateTime.Now.Day.ToString()), int.Parse(DateTime.Now.Hour.ToString()), int.Parse(DateTime.Now.Minute.ToString()), int.Parse(DateTime.Now.Second.ToString()));
        //        if (_strUniqueCode.Length < MaxLength)
        //        {
        //            _strUniqueCode = string.Format("{0}{1}", _strUniqueCode, GenerateRandomSessionNo(MaxLength - _strUniqueCode.Length));
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    finally
        //    {
        //        if (_strUniqueCode.Length > MaxLength)
        //            _strUniqueCode = _strUniqueCode.Substring(0, 10);
        //    }
        //    return _strUniqueCode;
        //}
        //public static string GenerateRandomSessionNo(int length)
        //{
        //    Random random = new Random();
        //    StringBuilder _strbRandom = new StringBuilder();
        //    if (length > 0)
        //    {
        //        for (int i = 0; i < length; i++)
        //        {
        //            _strbRandom.Append(random.Next(10).ToString());
        //        }
        //    }
        //    return _strbRandom.ToString();
        //}

 
        public static DateTime StartOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }

        public static int DeliveryStatus(string sStatus)
        {
            if (string.Compare(sStatus, "D", true) == 0 || string.Compare(sStatus, "Delivered", true) == 0)
                return 3;
            if (string.Compare(sStatus, "F", true) == 0)
                return 4;
            return 2;
        }
        public static string EscapeStringValue(string value)
        {
            const char BACK_SLASH = '\\';
            const char SLASH = '/';
            const char DBL_QUOTE = '"';

            var output = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                switch (c)
                {
                    case SLASH:
                        output.AppendFormat("{0}{1}", BACK_SLASH, SLASH);
                        break;

                    case BACK_SLASH:
                        output.AppendFormat("{0}{0}", BACK_SLASH);
                        break;

                    case DBL_QUOTE:
                        output.AppendFormat("{0}{1}", BACK_SLASH, DBL_QUOTE);
                        break;

                    default:
                        output.Append(c);
                        break;
                }
            }

            return output.ToString();
        }


        public static DateTime formatDate(string _dtDate)
        {
            try
            {
                string[] formats = {"dd/MM/yyyy h:mm:ss tt", "dd/MM/yyyy h:mm tt","dd/MMM/yyyy h:mm tt",
                   "dd/MM/yyyy hh:mm:ss", "dd/MM/yyyy h:mm:ss","dd/MMM/yyyy h:mm A",
                   "dd/MM/yyyy hh:mm tt", "dd/MM/yyyy hh tt",
                   "dd/MM/yyyy h:mm", "dd/MM/yyyy h:mm",
                   "dd/MM/yyyy hh:mm", "dd/MM/yyyy hh:mm","dd/MM/yyyy hh:mm:s","dd/MM/yyyy hh:m:s","dd/MM/yyyy h:m:s","dd/MM/yyyy h:mm:s","dd/MM/yyyy hh:m:ss","yyyy-MM-dd HH:mm:ss","dd-MM-yyyy HH:mm","dd-MM-yyyy hh:mm tt","dd-MM-yyyy","M/d/yyyy h:mm:ss tt"};
                DateTime _ConvertedDate = DateTime.Now;
                if (!DateTime.TryParseExact(_dtDate, "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _ConvertedDate))
                {
                    DateTime.TryParseExact(_dtDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out _ConvertedDate);
                }
                return _ConvertedDate;
            }
            catch (Exception ex)
            {


            }
            return DateTime.Now;
        }
    }
