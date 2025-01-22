using IMI.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace GetApplicationStatus
{
    public class clgeneral
    {
        HttpRequestParamModel objHttpRequestParams = null;
        public  int WebReqTimeOut = 60000;
        public  int CONNECTION_LIMIT = 10;
        public  string Accept_Encoding = "gzip,deflate";

        public static string GetConfigVal(string strTag)
        {
            try
            {
                if (!string.IsNullOrEmpty(strTag))
                {
                    return ConfigurationManager.AppSettings[strTag] != null ? ConfigurationManager.AppSettings[strTag].ToString().Trim() : "";
                }
            }
            catch (Exception ex)
            {
                LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, ex, string.Format("GetConfigVal- Ex:{0}", ex.Message));
            }
            return "";
        }

        public static string GetConfig(string strTag)
        {
            try
            {
                return ConfigurationManager.AppSettings[strTag] ?? null;
            }
            catch (Exception ex)
            {
                LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, ex, string.Format("GetConfig- Ex:{0}", ex.Message));
                return null;
            }
        }

        public static int ConvertInt(string strVal)
        {
            int intValue;
            int.TryParse(strVal, out intValue);
            return intValue;
        }

        public static float ConvertFloat(string strVal)
        {
            float fValue;
            float.TryParse(strVal, out fValue);
            return fValue;
        }
        public static double ConvertDouble(string strVal)
        {
            double dValue;
            double.TryParse(strVal, out dValue);
            return dValue;
        }
        public static long ConvertLong(string sVal)
        {
            long lValue;
            long.TryParse(sVal, out lValue);
            return lValue;
        }
        public static string getMsisdn(string msisdn)
        {
            try
            {
                msisdn = msisdn.Length > 10 ? msisdn.Substring(msisdn.Length - 10, 10) : msisdn;
                msisdn = string.Format("91{0}", msisdn);
            }
            catch (Exception ex)
            {
                LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, ex, "DHFLBotApp - clGeneral- getMsisdn " + msisdn);
            }
            finally
            {

            }
            return msisdn;
        }
        public static DateTime ConvertDateTime(string sVal)
        {
            DateTime dtCurDate = DateTime.Now;
            DateTime.TryParse(sVal, out dtCurDate);
            return dtCurDate;
        }
        public static DateTime DateParseExact(string sVal, string sFormat)
        {
            DateTime dtDateTime = DateTime.ParseExact(sVal, sFormat, CultureInfo.InvariantCulture);

            return dtDateTime;
        }
        public static bool isValidName(string sName)
        {
            bool isValid = false;
            try
            {
                if (!string.IsNullOrEmpty(sName))
                {
                    if (Regex.IsMatch(sName, @"^[a-zA-Z ]+$"))
                    {
                        if (sName.Length < ConvertInt(GetConfigVal("NAME_MAX_LENGTH")))
                        {
                            if (sName.Split(' ').Length >= 2)
                            {
                                isValid = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, ex, string.Format("DomainMapper- Ex:{0}", ex.Message));
            }
            return isValid;
        }
        public static string getDate(string sDOB)
        {
            try
            {
                if (sDOB.Split('-').Length == 3)
                {
                    DateTime dtDOB = new DateTime(ConvertInt(sDOB.Split('-')[2]), ConvertInt(sDOB.Split('-')[1]), ConvertInt(sDOB.Split('-')[0]));
                    return dtDOB.ToString("dd-MM-yyyy");
                }
            }
            catch (Exception ex)
            {

            }
            return "";
        }
        public static string ConvertQueryStringToJson(string sQueryString)
        {
            try
            {
                var dicOrderPayData = HttpUtility.ParseQueryString(sQueryString);
                if (dicOrderPayData != null && dicOrderPayData.Count > 0)
                {
                    var jsonOrder = new JavaScriptSerializer().Serialize(
                                        dicOrderPayData.AllKeys.ToDictionary(k => k, k => dicOrderPayData[k]));
                    return jsonOrder;

                }
            }
            catch (Exception ex)
            {
                LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, ex, string.Format("orderNotifyProcess-ConvertQueryStringToJson Ex:{0}", ex.Message));

            }
            return "";
        }
 
        public static string GenerateRandomCode()
        {
            Random r = new Random();
            string s = "";
            for (int j = 0; j < 5; j++)
            {
                int i = r.Next(1, 3);
                int ch;
                switch (i)
                {
                    case 1:
                        ch = r.Next(1, 9);
                        s = s + ch.ToString();
                        break;
                    case 2:
                        ch = r.Next(65, 90);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                    case 3:
                        ch = r.Next(65, 90);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                    default:
                        ch = r.Next(1, 9);
                        s = s + Convert.ToChar(ch).ToString();
                        break;
                }
                r.NextDouble();
                r.Next(100, 1999);
            }
            return s;
        }
        public HttpStatusModel DoHttpWebPost(HttpRequestParamModel objHttpRequestParams)
        {
            this.objHttpRequestParams = objHttpRequestParams;
            if (!int.TryParse(clgeneral.GetConfigVal("WebReqTimeout"), out WebReqTimeOut) || WebReqTimeOut == 0)
            {
                WebReqTimeOut = 60000;
            }
            if (!int.TryParse(clgeneral.GetConfigVal("CONN_LIMIT"), out CONNECTION_LIMIT) || CONNECTION_LIMIT == 0)
                CONNECTION_LIMIT = 10;
            HttpStatusModel objHttpStatus = new HttpStatusModel();
            if (objHttpRequestParams != null && objHttpRequestParams.httpRequestHeader != null)
            {
                objHttpStatus = new HttpStatusModel { url = objHttpRequestParams.URL, statusCode = 200, requestBody = objHttpRequestParams.PostBody };
                long lStartTime = DateTime.Now.Ticks;
                try
                {
                    objHttpStatus.headerInfo = (objHttpRequestParams != null ? (objHttpRequestParams.httpRequestHeader != null ? objHttpRequestParams.httpRequestHeader.ToString() : "") : "");
                    HttpWebRequest httpWebReq = (HttpWebRequest)WebRequest.Create(objHttpRequestParams.URL);
                    httpWebReq.Method = "POST";
                    httpWebReq.Timeout = WebReqTimeOut;
                    httpWebReq.KeepAlive = false;
                    httpWebReq.ContentType = "application/json; charset=utf-8";
                    httpWebReq.ProtocolVersion = HttpVersion.Version10;
                    httpWebReq.Headers.Add("Accept-Encoding", Accept_Encoding);
                    httpWebReq.Headers.Add("Service", objHttpRequestParams.httpRequestHeader.Service);
                    httpWebReq.Headers.Add("Action", objHttpRequestParams.httpRequestHeader.Action);
                    httpWebReq.Headers.Add("X-FORWARDIP", objHttpRequestParams.httpRequestHeader.IP);
                    httpWebReq.Headers.Add("X-IMI-REQINIT", objHttpRequestParams.httpRequestHeader.SaltKey);
                    httpWebReq.Headers.Add("X-IMI-SIGNATURE", objHttpRequestParams.httpRequestHeader.Signature);

                    byte[] bReqBytes = null;
                    try
                    {
                        ServicePointManager.DefaultConnectionLimit = CONNECTION_LIMIT;
                        ServicePointManager.Expect100Continue = false;
                    }
                    catch { }

                    if (!string.IsNullOrEmpty(objHttpRequestParams.PostBody))
                    {
                        bReqBytes = Encoding.UTF8.GetBytes(FormatPayLoad(objHttpRequestParams.PostBody));
                        httpWebReq.ContentLength = bReqBytes.Length;

                        try
                        {
                            using (Stream newStream = httpWebReq.GetRequestStream())
                            {
                                newStream.Write(bReqBytes, 0, bReqBytes.Length);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, ex, "WebRequestingProcess-WebReqPostingData-GetRequestStream -WebException::" + ex.Message);
                            return objHttpStatus;
                        }
                    }
                    else
                    {
                        httpWebReq.ContentLength = 0;
                    }
                    using (var webRes = (HttpWebResponse)httpWebReq.GetResponse())
                    {
                        objHttpStatus.response = ReadWebResponse(webRes);
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
                                        objHttpStatus.response = reader.ReadToEnd();
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, ex, "WebRequestingProcess-WebReqPostingData -WebException::" + ex.Message + ":Payload:" + objHttpRequestParams.PostBody);
                        }
                    }
                    else
                    {
                        LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, Ex, "WebRequestingProcess-WebReqPostingData -WebException:" + Ex.Message + ":Payload:" + objHttpRequestParams.PostBody);
                    }
                    // get status code 
                    if (Ex.Status == WebExceptionStatus.Timeout)
                    {
                        LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, Ex, "WebRequestingProcess-WebReqPostingData WebException:Timeout" + Ex.Message + ":Payload:" + objHttpRequestParams.PostBody);
                    }
                    else if (Ex.Status == WebExceptionStatus.ConnectFailure || Ex.Status == WebExceptionStatus.ConnectionClosed || Ex.Status == WebExceptionStatus.KeepAliveFailure
                        || Ex.Status == WebExceptionStatus.Pending)
                    {
                        LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, Ex,
                            "WebRequestingProcess-WebReqPostingData -WebException:Connection" + Ex.Message + ":URL:" + objHttpRequestParams.PostBody);
                    }
                    else
                    {
                        LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, Ex, "WebRequestingProcess-WebReqPostingData -WebException" + Ex.Message + ":URL:" + objHttpRequestParams.PostBody);
                    }

                }
                catch (Exception ex)
                {
                    LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, ex, "WebRequestingProcess-WebReqPostingData -Exception" + ex.Message + ":URL:" + objHttpRequestParams.PostBody);

                }
                finally
                {

                    objHttpStatus.timeTaken = (DateTime.Now.Ticks - lStartTime) / 10000;
                    LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Debug, (objHttpStatus != null ? objHttpStatus.ToString() : ""));
                }
            }
            return objHttpStatus;
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
                LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, ex, "FormatPayLoad:Exception:" + ex.Message + ":pl:" + sPayload);
            }

            return strBUrlEncoded.ToString();
        }

        /// <summary>
        /// Read web Response 
        /// </summary>
        /// <param name="httpWebResponse"></param>
        /// <returns></returns>
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
                        LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, ex, "ReadWebResponse:WebException::" + ex.Message);
                    }
                }
                else
                {
                    LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, Ex, "ReadWebResponse:WebException:" + Ex.Message);
                }

                // get status code 
                if (Ex.Status == WebExceptionStatus.Timeout)
                {
                    LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, Ex, "ReadWebResponse:Timeout" + Ex.Message);
                }
                else if (Ex.Status == WebExceptionStatus.ConnectFailure || Ex.Status == WebExceptionStatus.ConnectionClosed || Ex.Status == WebExceptionStatus.KeepAliveFailure
                    || Ex.Status == WebExceptionStatus.Pending)
                {
                    LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, Ex,
                        "ReadWebResponse:Connection-" + Ex.Message);
                }
                else
                {
                    LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, Ex, "ReadWebResponse:other:" + Ex.Message);
                }

            }
            catch (Exception ex)
            {
                LogData.Write("GetApplicationStatus", "GetApplicationStatus", LogMode.Excep, ex, "ReadWebResponse:General-" + ex.Message);
            }
            return strResponse;
        }
    }
}
