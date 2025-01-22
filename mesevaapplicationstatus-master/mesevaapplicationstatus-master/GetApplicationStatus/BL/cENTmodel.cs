using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetApplicationStatus
{
   public class cENTmodel
    {
    }
    public class HttpStatusModel
    {
        public int statusCode { set; get; }
        public int statusDesc { set; get; }
        public string response { set; get; }
        public long timeTaken { set; get; }
        public string url { set; get; }
        public string requestBody { set; get; }
        public string headerInfo { set; get; }
        public int status { get; set; }
        public override string ToString()
        {
            return string.Format("URL:{0} resp:{1} tt:{2} code:{3} body:{4} headers:{5}", url, response, timeTaken, statusCode, requestBody, headerInfo);
        }
    }

    public class HttpRequestParamModel
    {
        public HttpRequestHeaderModel httpRequestHeader { get; set; }
        public string URL { get; set; }
        public string PostBody { get; set; }
        public string ContentType { get; set; }
    }
    public class HttpRequestHeaderModel
    {
        public string Service { get; set; }
        public string Action { get; set; }
        public string IP { get; set; }
        public string SaltKey { get; set; }
        public string Signature { get; set; }
        public override string ToString()
        {
            return string.Format("service:{0} action:{1} ip:{2} salkey:{3} sig:{4}", Service, Action, IP, SaltKey, Signature);
        }
    }

    public class getstatus
    {
        public string tid { get; set; }
        public string mobileNo { get; set;}
        public string channel { get; set; }
        public string applicationNo { get; set; }
    }
    public class getstatus_resp
    {
        public string status{ get; set; }
        public string resCode { get; set; }
        public string resDesc { get; set; }     
    }
}
