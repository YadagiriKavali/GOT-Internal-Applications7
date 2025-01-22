using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using IMI.Logger;

namespace User.Utilities
{
    public class WebReqProcess
    {
        private int WebReqTimeOut = 60000;
        private int CONNECTION_LIMIT = 10;
        private readonly string CONTENT_TYPE = "";
        private readonly string SERVICE_KEY = "";
        public WebReqProcess()
        {
            CommonSettings();
            SERVICE_KEY = clGeneral.GetConfigValue("SERVICE_KEY");
        }
        public WebReqProcess(string sContentType, string sServiceKey)
        {
            CommonSettings();
            SERVICE_KEY = sServiceKey;
            CONTENT_TYPE = sContentType;

        }

        private void CommonSettings()
        {
            if (!int.TryParse(clGeneral.GetConfigValue("WEB_REQ_TIMEOUT"), out WebReqTimeOut))
                WebReqTimeOut = 60000;

            if (!int.TryParse(clGeneral.GetConfigValue("CONNECTION_LIMIT"), out CONNECTION_LIMIT))
                CONNECTION_LIMIT = 10;
        }
        
        /// <summary>
        ///  for sms
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sPayload"></param>
        /// <param name="sServiceKey"></param>
        /// <returns></returns>
        public string WebReqPostingJsonDataData(string sUrl, string sPayload, string sServiceKey)
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
                httpWebreq.ContentType = "application/json; charset=utf-8";
                if (!string.IsNullOrEmpty(sServiceKey))
                    httpWebreq.Headers.Add("Key", sServiceKey);
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
                LogData.Write("UserLib", "WEBREQUEST", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException::" + Ex.Message + ":URL:" + sUrl + ":Payload:" + sPayload);

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

                        LogData.Write("UserLib", "WEBREQUEST", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException::" + ex.Message + ":URL:" + sUrl + ":Payload:" + sPayload);
                    }
                }
                else
                {
                    LogData.Write("UserLib", "WEBREQUEST", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException:" + Ex.Message + ":URL:" + sUrl + ":Payload:" + sPayload);
                }

                // get status code 
                if (Ex.Status == WebExceptionStatus.Timeout)
                {
                    LogData.Write("UserLib", "WEBREQUEST", LogMode.Excep, "WebRequestingProcess-WebReqPostingData WebException:Timeout" + Ex.Message + ":URL:" + sUrl + ":Payload:" + sPayload);
                }
                else if (Ex.Status == WebExceptionStatus.ConnectFailure || Ex.Status == WebExceptionStatus.ConnectionClosed || Ex.Status == WebExceptionStatus.KeepAliveFailure
                    || Ex.Status == WebExceptionStatus.Pending)
                {
                    LogData.Write("UserLib", "WEBREQUEST", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException:Connection" + Ex.Message + ":URL:" + sUrl + ":Payload:" + sPayload);
                }
                else
                {
                    LogData.Write("UserLib", "WEBREQUEST", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException" + Ex.Message + ":URL:" + sUrl + ":Payload:" + sPayload);
                }

            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "WEBREQUEST", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -Exception" + ex.Message + ":URL:" + sUrl + ":Payload:" + sPayload);

            }
            finally
            {
                strBReqRes.Append("TimeTaken:" + (DateTime.Now.Ticks - lStartTime) / 10000);
                LogData.Write("", "", LogMode.Debug, strBReqRes.ToString());
            }
            return strResponse;
        }
        
        public string DoChargePostRequest(string sUrl, string sPayload)
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
                httpWebreq.ContentType = "application/x-www-form-urlencoded";
                byte[] bReqBytes = null;
                try
                {
                    ServicePointManager.DefaultConnectionLimit = CONNECTION_LIMIT;
                }
                catch { }

                if (!string.IsNullOrEmpty(sPayload))
                {
                    bReqBytes = Encoding.ASCII.GetBytes(sPayload);
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
                LogData.Write("UserLib", "WEBREQUEST", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException::" + Ex.Message + ":URL:" + sUrl + ":Payload:" + sPayload);
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

                        LogData.Write("UserLib", "WEBREQUEST", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException::" + ex.Message + ":URL:" + sUrl + ":Payload:" + sPayload);
                    }
                }
                else
                {
                    LogData.Write("UserLib", "WEBREQUEST", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException:" + Ex.Message + ":URL:" + sUrl + ":Payload:" + sPayload);
                }

                // get status code 
                if (Ex.Status == WebExceptionStatus.Timeout)
                {
                    LogData.Write("UserLib", "WEBREQUEST", LogMode.Excep, "WebRequestingProcess-WebReqPostingData WebException:Timeout" + Ex.Message + ":URL:" + sUrl + ":Payload:" + sPayload);
                }
                else if (Ex.Status == WebExceptionStatus.ConnectFailure || Ex.Status == WebExceptionStatus.ConnectionClosed || Ex.Status == WebExceptionStatus.KeepAliveFailure
                    || Ex.Status == WebExceptionStatus.Pending)
                {
                    LogData.Write("UserLib", "WEBREQUEST", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException:Connection" + Ex.Message + ":URL:" + sUrl + ":Payload:" + sPayload);
                }
                else
                {
                    LogData.Write("UserLib", "WEBREQUEST", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException" + Ex.Message + ":URL:" + sUrl + ":Payload:" + sPayload);
                }

            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "WEBREQUEST", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -Exception" + ex.Message + ":URL:" + sUrl + ":Payload:" + sPayload);

            }
            finally
            {
                strBReqRes.Append("TimeTaken:" + (DateTime.Now.Ticks - lStartTime) / 10000);
                LogData.Write("", "", LogMode.Debug, strBReqRes.ToString());
            }
            return strResponse;
        }
        
        public string WebCryptoAPIRequest(string sUrl, string sPayload, Dictionary<string, string> dicHeaders)
        {
            long lStartTime = DateTime.Now.Ticks;
            string strResponse = "";
            StringBuilder strBReqRes = new StringBuilder();
            try
            {
                strBReqRes.Append(string.Format("Url: {0} Payload:{1} key:{2} hearderCnt:{3}", sUrl, sPayload, SERVICE_KEY, (dicHeaders != null ? dicHeaders.Count : 0)));
                HttpWebRequest httpWebreq = (HttpWebRequest)WebRequest.Create(sUrl);
                httpWebreq.Method = "POST";
                httpWebreq.Timeout = WebReqTimeOut;
                httpWebreq.ContentType = string.IsNullOrEmpty(CONTENT_TYPE) ? "application/x-www-form-urlencoded" : CONTENT_TYPE;

                httpWebreq.Headers.Add("X-CyberPlat-Proto", "SHA1RSA");

                if (dicHeaders != null && dicHeaders.Count > 0)
                {
                    foreach (KeyValuePair<string, string> keyPair in dicHeaders)
                    {
                        // httpWebreq.Headers.Add("X-CyberPlat-Proto", keyPair.Value);
                    }
                }
                byte[] bReqBytes = null;
                try
                {
                    ServicePointManager.DefaultConnectionLimit = CONNECTION_LIMIT;
                }
                catch { }

                if (!string.IsNullOrEmpty(sPayload))
                {
                    bReqBytes = Encoding.ASCII.GetBytes(FormatPayLoad(sPayload));
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
                    LogData.Write("", "", LogMode.Excep, "WebRequestingProcess-WebReqPostingData WebException:Timeout:" + Ex.Message);
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
        /// <summary>
        /// format Payload
        /// </summary>
        /// <param name="sPayload"></param>
        /// <returns></returns>
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
                    //  raise an alert
                    LogData.Write("UserLib", "WEBREQUEST", LogMode.Excep, "DoWebRequest:webException:" + WebEx.Message + ":URL:" + strUrl);
                    if (WebEx.Response != null)
                    {
                        try
                        {
                            using (WebResponse response = WebEx.Response)
                            {
                                HttpWebResponse httpResponse = (HttpWebResponse)response;
                                using (Stream data = response.GetResponseStream())
                                {
                                    using (var reader = new StreamReader(data))
                                    {
                                        _strResponse = reader.ReadToEnd();
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogData.Write("RECHARGE", "WEBREQUEST", LogMode.Excep, "WebRequestingProcess-WebReqPostingData -WebException::" + ex.Message + ":URL:" + strUrl);
                        }
                    }
                    if (WebEx.Status == WebExceptionStatus.Timeout)
                    {
                    }
                    else
                    {
                    }

                }
                catch (Exception ex)
                {
                    LogData.Write("RECHARGE", "WEBREQUEST", LogMode.Excep, "DoWebRequest:Exception:" + ex.Message + ":URL:" + strUrl);
                }
                finally
                {
                    LogData.Write("RECHARGE", "WEBREQUEST", LogMode.Debug, string.Format("WebRequestHandler-WebReqGetMethod->Req:{0} | Res:{1} | tt: {2} ", strUrl, _strResponse, (DateTime.Now.Ticks - lStartTime) / 10000));
                }
            }
            else
            {
                // url should not blank
            }

            return "";
        }


        public static string DoHttpPost(string url, string service, string action, string data)
        {
            var startTime = DateTime.Now.Ticks;
            var response = string.Empty;
            try
            {
                var httpWebReq = (HttpWebRequest)WebRequest.Create(new Uri(url));
                if (httpWebReq != null)
                {
                    httpWebReq.Method = "POST";
                    byte[] postBytes = Encoding.ASCII.GetBytes(data);
                    httpWebReq.Accept = "*/*";
                    httpWebReq.Timeout = 60000;
                    httpWebReq.ContentType = "application/json;charset=\"UTF-8\"";
                    httpWebReq.Headers.Add("Accept-Encoding", "gzip,deflate");
                    httpWebReq.Headers.Add("Service", service);
                    httpWebReq.Headers.Add("Action", action);
                    ServicePointManager.DefaultConnectionLimit = 5;
                    ServicePointManager.Expect100Continue = false;
                    httpWebReq.KeepAlive = true;
                    httpWebReq.ContentLength = postBytes.Length;
                    httpWebReq.UserAgent = "Apache-HttpClient/4.1.1 (java 1.5)";
                    Stream requestStream = httpWebReq.GetRequestStream();
                    requestStream.Write(postBytes, 0, postBytes.Length);
                    requestStream.Close();
                    HttpWebResponse webResponse = (HttpWebResponse)httpWebReq.GetResponse();
                    StreamReader streamReader = new StreamReader(webResponse.GetResponseStream());
                    response = streamReader.ReadToEnd();
                    webResponse.Close();
                    webResponse = null;
                    streamReader = null;
                    httpWebReq = null;
                }
                httpWebReq = null;
            }
            catch (WebException WebEx)
            {
                if (WebExceptionStatus.Timeout == WebEx.Status)
                    LogData.Write("User", "HttpPost", LogMode.Excep, WebEx, string.Format("WebRequestProcess => DoHttpPost - Timeout Exception:{0}", WebEx.Message));
                else
                    LogData.Write("User", "HttpPost", LogMode.Excep, WebEx, string.Format("WebRequestProcess => DoHttpPost - Web Exception:{0}", WebEx.Message));
            }
            catch (Exception ex)
            {
                LogData.Write("User", "HttpPost", LogMode.Excep, ex, string.Format("WebRequestProcess => DoHttpPost - Web Exception:{0}", ex.Message));
            }
            finally
            {
                LogData.Write("User", "HttpPost", LogMode.Info, string.Format("WebRequestProcess => DoHttpPost - Action: {0}, URL: {1}, Request: {2}, Reponse: {3}, TimeTaken: {4}", action, url, data, response, (DateTime.Now.Ticks - startTime) / 10000));
            }

            return response;
        }
    }
}
