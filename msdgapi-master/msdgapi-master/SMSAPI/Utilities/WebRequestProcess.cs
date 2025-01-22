using System;
using System.IO;
using System.Net;
using System.Text;
using IMI.Logger;

namespace SMSAPI.Utilities
{
    public class WebRequestProcess
    {
        public static string EsevaHttpPost(string url, string service, string action, string data)
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
                    LogData.Write("SMSAPI", "Exception", LogMode.Excep, WebEx, string.Format("WebRequestProcess => DoHttpPost - Timeout Exception:{0}", WebEx.Message));
                else
                    LogData.Write("SMSAPI", "Exception", LogMode.Excep, WebEx, string.Format("WebRequestProcess => DoHttpPost - Web Exception:{0}", WebEx.Message));
            }
            catch (Exception ex)
            {
                LogData.Write("SMSAPI", "Exception", LogMode.Excep, ex, string.Format("WebRequestProcess => DoHttpPost - Web Exception:{0}", ex.Message));
            }
            finally
            {
                LogData.Write("SMSAPI", "HttpPost", LogMode.Info, string.Format("WebRequestProcess => DoHttpPost - Action: {0}, URL: {1}, Request: {2}, Reponse: {3}, TimeTaken: {4}", action, url, data, response, (DateTime.Now.Ticks - startTime) / 10000));
            }

            return response;
        }
    }
}