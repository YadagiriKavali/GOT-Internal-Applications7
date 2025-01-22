using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;

namespace meseva.Utilities
{
    public class WebRequestProcess
    {
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
                    ServicePointManager.DefaultConnectionLimit = Convert.ToInt32(GetConfigVal("DEFAULT_CONNECTION_LIMIT"));
                else
                    ServicePointManager.DefaultConnectionLimit = 5;
                //ServicePointManager.UseNagleAlgorithm = false;
                objReq = WebRequest.Create(strURL);
                //objReq.ConnectionGroupName = Guid.NewGuid().ToString();
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
                //using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                //    WriteLog("Url: " + strURL + ", Error: " + reader.ReadToEnd());
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
                    ServicePointManager.DefaultConnectionLimit = Convert.ToInt32(GetConfigVal("DEFAULT_CONNECTION_LIMIT"));
                else
                    ServicePointManager.DefaultConnectionLimit = 5;
                //ServicePointManager.UseNagleAlgorithm = false;
                objReq = WebRequest.Create(strURL);
                //objReq.ConnectionGroupName = Guid.NewGuid().ToString();
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
                //using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                //    WriteLog("Url: " + strURL + ", Error: " + reader.ReadToEnd());
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
                    ServicePointManager.DefaultConnectionLimit = Convert.ToInt32(GetConfigVal("DEFAULT_CONNECTION_LIMIT"));
                else
                    ServicePointManager.DefaultConnectionLimit = 5;
                objReq = WebRequest.Create(strURL);
                //ServicePointManager.UseNagleAlgorithm = false;
                //objReq.ConnectionGroupName = Guid.NewGuid().ToString();                
                if (strProxyIP.Trim().Length > 0)
                {
                    WebProxy objWebProxy = null;
                    if (strProxyPort.Trim().Length > 0)
                        objWebProxy = new WebProxy(strProxyIP, Convert.ToInt32(strProxyPort));
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
                //using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                //    WriteLog("Url: " + strURL + ", Error: " + reader.ReadToEnd());
                return "Dear User, currently we are unable to process your request. Please try after sometime.";
            }
            finally
            {
                objReq = null;
                objRes = null;
                objSr = null;
            }
        }

        private static string GetConfigVal(string key)
        {
            return !string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]) ? ConfigurationManager.AppSettings[key] : string.Empty;
        }
    }
}
