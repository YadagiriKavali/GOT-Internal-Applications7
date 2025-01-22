using System;
using System.IO;
using System.Net;
using System.Text;
using IMI.Logger;

namespace eseva.Utilities
{
    public class WebRequestProcess
    {
        public static string DoHttpPost(string url, string postXml, string action)
        {
            var startTime = DateTime.Now.Ticks;
            var response = string.Empty;
            try
            {
                var httpWebReq = (HttpWebRequest)WebRequest.Create(new Uri(url));
                if (httpWebReq != null)
                {
                    httpWebReq.Method = "POST";
                    byte[] postBytes = Encoding.ASCII.GetBytes(postXml.Replace("\r", ""));
                    httpWebReq.Accept = "*/*";
                    httpWebReq.Timeout = 60000;
                    httpWebReq.ContentType = "text/xml;charset=\"UTF-8\"";
                    httpWebReq.Headers.Add("Accept-Encoding", "gzip,deflate");
                    httpWebReq.Headers.Add("SOAPAction", action);
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
                    LogData.Write("ESEVA", "WEBREQUEST-Exception", LogMode.Excep, WebEx, string.Format("WebRequestProcess => DoHttpPost - Timeout Exception:{0}", WebEx.Message));
                else
                    LogData.Write("ESEVA", "WEBREQUEST-Exception", LogMode.Excep, WebEx, string.Format("WebRequestProcess => DoHttpPost - Web Exception:{0}", WebEx.Message));
            }
            catch (Exception ex)
            {
                LogData.Write("ESEVA", "WEBREQUEST-Exception", LogMode.Excep, ex, string.Format("WebRequestProcess => DoHttpPost - Web Exception:{0}", ex.Message));
            }
            finally
            {
                LogData.Write("ESEVA", "WEBREQUEST", LogMode.Info, string.Format("WebRequestProcess => DoHttpPost - Action: {0}, URL: {1}, Request: {2}, Reponse: {3}, TimeTaken: {4}", action, url, postXml, response, (DateTime.Now.Ticks - startTime) / 10000));
            }

            return response;
        }
    }
}
