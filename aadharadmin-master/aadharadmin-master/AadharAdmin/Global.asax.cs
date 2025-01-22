using IMI.Helper;
using IMI.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AadharAdmin
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();            
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MvcHandler.DisableMvcResponseHeader = true;
        }

        protected void Application_BeginRequest()
        {
            try
            {
                //Cache
                HttpContext.Current.Response.Cache.SetAllowResponseInBrowserHistory(false);
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Current.Response.Cache.SetNoStore();
                Response.Cache.SetExpires(DateTime.Now);
                Response.Cache.SetValidUntilExpires(true);

                //Security
                string strReqVal = string.Empty;
                string strRegex = General.GetConfigVal("SECURITY_REGEX");
                List<string> strBlockKeys = General.GetConfigVal("SECURITY_WORDS").Split(',').ToList<string>().Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                string[] ArrayList = HttpUtility.UrlDecode(Request.Form.ToString()).Split('&');
                for (int i = 0; i < ArrayList.Length; i++)
                {
                    if (ArrayList[i] != "")
                    {
                        //To prevent Angular exparssion injection
                        strReqVal = ArrayList[i].Split('=').GetValue(1).ToString();

                        if (strReqVal.Contains("--"))
                            strReqVal = strReqVal.Replace("--", "");

                        if (strReqVal.Contains("{{") || strReqVal.Contains("}}"))
                        {
                            LogData.Write("GlobalASAXFile", "AngularExparssion", LogMode.Debug, strReqVal + "@@@" + ArrayList[i]);
                            HttpContext.Current.Response.AppendToLog("&ReqExp=failed");
                            HttpContext.Current.Response.StatusCode = 404;

                            HttpContextBase httpContext = new HttpContextWrapper(HttpContext.Current);
                            UrlHelper urlHelper = new UrlHelper(new RequestContext(httpContext, new RouteData()));
                            string redirectUrl = urlHelper.Action("NotFound", "Error");

                            httpContext.Response.Redirect(redirectUrl);
                            return;
                        }

                        //XSS
                        //bool a = strBlockKeys.Any(strReqVal.Contains);                     
                        //bool b = strBlockKeys.Any(s => strReqVal.Contains(s));                        
                        if (strBlockKeys.Any(strReqVal.Contains))
                        {
                            var item = strBlockKeys.FindAll(strReqVal.Contains)[0];
                            LogData.Write("GlobalASAXFile", "Blocked_SecurityKey", LogMode.Debug, item + "@@@" + ArrayList[i]);
                            HttpContext.Current.Response.AppendToLog("&ReqExp=failed");
                            HttpContext.Current.Response.StatusCode = 404;

                            HttpContextBase httpContext = new HttpContextWrapper(HttpContext.Current);
                            UrlHelper urlHelper = new UrlHelper(new RequestContext(httpContext, new RouteData()));
                            string redirectUrl = urlHelper.Action("NotFound", "Error");

                            httpContext.Response.Redirect(redirectUrl);
                            return;
                        }
                        else
                        {
                            if (ValidateURLParameters(strReqVal, strRegex))
                            {
                                LogData.Write("GlobalASAXFile", "ValidateURLParameters", LogMode.Debug, strReqVal + "@@@" + ArrayList[i]);
                                //WriteLog("ValidateURLParameters- Invalid Parameter :" + strReqVal);
                                HttpContext.Current.Response.AppendToLog("&ReqExp=failed");
                                HttpContext.Current.Response.StatusCode = 404;
                                HttpContextBase httpContext = new HttpContextWrapper(HttpContext.Current);
                                UrlHelper urlHelper = new UrlHelper(new RequestContext(httpContext, new RouteData()));
                                string redirectUrl = urlHelper.Action("NotFound", "Error");

                                httpContext.Response.Redirect(redirectUrl);

                                Response.Flush();
                                Response.End();
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogData.Write("GlobalAsaxFile", "Application_BeginRequest", LogMode.Excep, ex.Message);
            }
        }

        protected void Session_Start()
        {
            //Cookie Secure tag
            if (Request.IsSecureConnection == true)
            {
                Response.Cookies["ASP.NET_SessionID"].Secure = true;
                Response.Cookies[".ASPXAUTH"].Secure = true;
            }


            if (HttpContext.Current.Session != null)
            {
                if (HttpContext.Current.Session.IsNewSession)
                {
                    string sCookieHeader = HttpContext.Current.Request.Headers["Cookie"];
                    if ((null != sCookieHeader) && (sCookieHeader.IndexOf("ASP.NET_SessionId") >= 0))
                    {
                        HttpContextBase httpContext = new HttpContextWrapper(HttpContext.Current);
                        UrlHelper urlHelper = new UrlHelper(new RequestContext(httpContext, new RouteData()));
                        string redirectUrl = urlHelper.Action("Logout", "Home");

                        httpContext.Response.Redirect(redirectUrl);

                        Response.Flush();
                        Response.End();
                    }
                }
            }
        }

        protected void Session_End()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                    Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
                }
            }
        }

        protected void Application_PreSendRequestHeaders()
        {
            //XSS
            Response.AddHeader("X-Frame-Options", "SAMEORIGIN");
            //Remove Server Header
            Response.Headers.Remove("Server");
            //Remove X-AspNet-Version Header
            Response.Headers.Remove("X-AspNet-Version");

        }

        private void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();

            if (ex is HttpAntiForgeryException)
            {
                HttpContextBase httpContext = new HttpContextWrapper(HttpContext.Current);
                UrlHelper urlHelper = new UrlHelper(new RequestContext(httpContext, new RouteData()));                

                httpContext.Response.RedirectToRoute("Logout");

                Response.Flush();
                Response.End();
            }
        }

        public bool ValidateURLParameters(string sVal, string sPattern)
        {
            bool bFlag = false;
            try
            {
                sVal = sVal.Replace(" ", "").Replace("\"", "");
                Regex objrex = new Regex(sPattern, RegexOptions.IgnoreCase);
                bFlag = objrex.IsMatch(sVal);
            }
            catch
            {
            }
            return bFlag;
        }
    }
}
