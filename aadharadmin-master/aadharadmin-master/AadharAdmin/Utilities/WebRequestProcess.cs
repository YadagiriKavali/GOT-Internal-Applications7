using AadharAdmin.BAL.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using IMI.Helper;

namespace AadharAdmin.Utilities
{
    public class WebRequestProcess
    {
        private IbalGeneral _General;
        private readonly string ControlApiUrl = General.GetConfigVal("CONTROL_API");
        public WebRequestProcess()
        {
            _General = new balGeneral();
        }

        public string HttpJsonPost(string service, string action, string data)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var response = string.Empty;
            string imiInit = DateTime.Now.Ticks.ToString();             
            string CurrentLanguage = string.Empty;             
            try
            {
                string imiSignature = EncryptionProcess.GenerateSignature(data, imiInit);
                var httpWebReq = (HttpWebRequest)WebRequest.Create(new Uri(ControlApiUrl));                 

                if (httpWebReq != null)
                {
                    httpWebReq.Method = "POST";                    
                    byte[] postBytes = Encoding.UTF8.GetBytes(data);
                    httpWebReq.Accept = "*/*";
                    httpWebReq.Timeout = 60000;
                    httpWebReq.ContentType = "application/json;charset=\"UTF-8\"";
                   // httpWebReq.Headers.Add("Accept-Encoding", "gzip,deflate");
                    httpWebReq.Headers.Add("Service", service);
                    httpWebReq.Headers.Add("Action", action);
                    httpWebReq.Headers.Add("X-FORWARDIP", "203.199.178.211");// _General.GetClentIP())

                    httpWebReq.Headers.Add("X-IMI-REQINIT", imiInit);
                    httpWebReq.Headers.Add("X-IMI-SIGNATURE", imiSignature);
                    httpWebReq.Headers.Add("X-IMI-LANG", CurrentLanguage);
                    ServicePointManager.DefaultConnectionLimit = 5;
                    ServicePointManager.Expect100Continue = false;                    
                    httpWebReq.KeepAlive = true;
                    httpWebReq.ContentLength = postBytes.Length;
                    httpWebReq.UserAgent = string.Empty;
                    Stream requestStream = httpWebReq.GetRequestStream();
                    requestStream.Write(postBytes, 0, postBytes.Length);
                    requestStream.Close();
                    HttpWebResponse webResponse = (HttpWebResponse)httpWebReq.GetResponse();
                    StreamReader streamReader = new StreamReader(webResponse.GetResponseStream());
                    response = streamReader.ReadToEnd();
                    webResponse.Close();
                    webResponse = null;
                    streamReader = null;                     
                }
                httpWebReq = null;
            }
            catch (WebException WebEx)
            {
                string Msg = WebEx.Message;
                response = string.Format("{{\"resCode\":\"900\",\"resDesc\":\"Dear User, currently we are unable to process your request. Please try after sometime.\"}}");
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
            finally
            {
                
            }

            return response;
        }
    }
}