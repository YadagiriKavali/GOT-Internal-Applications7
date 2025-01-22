using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace eSeva.WebRequestProcess
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
                    StreamReader str = new StreamReader(webResponse.GetResponseStream());
                    response = str.ReadToEnd();
                    webResponse.Close();
                    webResponse = null;
                    str = null;
                    httpWebReq = null;
                }
                httpWebReq = null;
            }
            catch (WebException WebEx)
            {
                if (WebExceptionStatus.Timeout == WebEx.Status)
                {
                    //ClsGeneral.WriteLog("WebRequestPrcess-DoHttpPost- Timeout " + WebEx.Message + ":" + sUrl + ":" + strAction, enumLogMode.Err);
                }
                else
                {
                    //ClsGeneral.WriteLog("WebRequestPrcess-DoHttpPost web ex" + WebEx.Message + ":" + sUrl + ":" + strAction, enumLogMode.Err);
                }
            }
            catch (Exception ex)
            {
                //ClsGeneral.WriteLog("WebRequestPrcess-DoHttpPost web ex" + ex.Message + ":" + sUrl + ":" + strAction, enumLogMode.Err);
            }
            finally
            {
               // ClsGeneral.WriteLog("Action:" + strAction + Environment.NewLine + "URL: " + sUrl + Environment.NewLine + "Req:" + sPostXml + Environment.NewLine + "Response: " + strResp + ":tt:" + (DateTime.Now.Ticks - lStartTime) / 10000, enumLogMode.Debug);
            }
            return response;
        }
    }
}
