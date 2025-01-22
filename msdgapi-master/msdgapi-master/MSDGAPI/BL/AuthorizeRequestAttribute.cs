using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Xml;
using IMI.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace MSDGAPI.BL
{
    public class AuthorizeRequestAttribute : AuthorizeAttribute
    {
        #region Overridden Methods

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                var clientIP = string.Empty;
                var signature = string.Empty;
                var saltText = string.Empty;
                var requestBody = string.Empty;
                var request = actionContext.Request;
                
                IEnumerable<string> headerValues;
                if (request.Headers.TryGetValues("X-FORWARDIP", out headerValues))
                    clientIP = headerValues.FirstOrDefault();

                if (request.Headers.TryGetValues("X-IMI-SIGNATURE", out headerValues))
                    signature = headerValues.FirstOrDefault();

                if (request.Headers.TryGetValues("X-IMI-REQINIT", out headerValues))
                    saltText = headerValues.FirstOrDefault();

                request.Headers.Add("CustomMediaType", request.Content.Headers.ContentType != null ? request.Content.Headers.ContentType.MediaType : "NOMEDIATYPE");

                if (request.Content.Headers.ContentType != null && request.Content.Headers.ContentType.MediaType.ToLower() == "application/xml")
                {
                    try
                    {
                        using (var reqStream = new StreamReader(actionContext.Request.Content.ReadAsStreamAsync().Result))
                            requestBody = reqStream.ReadToEnd();

                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(requestBody);
                        requestBody = JsonConvert.SerializeXmlNode(doc);
                        var json = JObject.Parse(requestBody);

                        requestBody = json.GetValue("xml").ToString();

                        actionContext.Request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    }
                    catch { }
                }
                else
                {
                    var dictionary = actionContext.Request.GetQueryNameValuePairs().ToDictionary(qk => qk.Key, qv => qv.Value);
                    if (dictionary.Count > 0)
                    {
                        var json = JsonConvert.SerializeObject(dictionary, Newtonsoft.Json.Formatting.Indented);
                        actionContext.Request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    }
                }

                var configIPs = ConfigurationManager.AppSettings["AUTHORIZED_NOT_REQUIRED_FOR_IP"];
                if (!string.IsNullOrEmpty(configIPs) && configIPs.IndexOf("[" + clientIP + "]") >= 0)
                    return;

                using (var reqStream = new StreamReader(actionContext.Request.Content.ReadAsStreamAsync().Result))
                    requestBody = reqStream.ReadToEnd();

                if (EncryptionProcess.IsValidSignature(signature, requestBody, saltText))
                {
                    actionContext.Request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    return;
                }

                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
                actionContext.Response.ReasonPhrase = "Unauthorized";
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "AUTHORIZATION-Exception", LogMode.Excep, ex, string.Format("AuthorizeRequestAttribute => OnAuthorization - Ex:{0}", ex.Message));
            }
        }

        #endregion Overridden Methods
    }
}