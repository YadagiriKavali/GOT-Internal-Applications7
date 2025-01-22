using AadharAdmin.Models;
using IMI.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AadharAdmin.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public UserData GetProfileInfo()
        {
            UserData profile = new UserData();
            try
            {
                if (Session["UserData"] != null)
                {
                    profile = Session["UserData"] as UserData;
                }
                else
                {
                    profile = null;
                }
                
            }
            catch(Exception ex)
            {
                LogData.Write("BaseCtrl", "GetProfileInfo", LogMode.Excep, ex.Message);
            }
            return profile;
        }
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //This method is Once user loggedout then user can't got Home page if user clicks browser back button.
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            GenerateNotifications();
            base.OnActionExecuted(filterContext);
        }

        #region Alert Notification Coding
        public void GenerateNotifications()
        {
            if (this.TempData.ContainsKey("alert"))
            {
                ViewBag.Notifications = this.TempData["alert"];
                this.TempData.Remove("alert");
            }
        }

        public void AlertNotification(AlertType type, string message)
        {
            Alert toast = new Alert();
            toast.Type = type;
            toast.Message = message;

            List<Alert> alerts = new List<Alert>();

            if (this.TempData.ContainsKey("alert"))
            {
                alerts = JsonConvert.DeserializeObject<List<Alert>>(this.TempData["alert"].ToString());
                this.TempData.Remove("alert");
            }

            alerts.Add(toast);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            string alertJson = JsonConvert.SerializeObject(alerts, settings);

            this.TempData.Add("alert", alertJson);
        }
        #endregion       

        #region Http404 handling
        protected override void HandleUnknownAction(string actionName)
        {
            // If controller is ErrorController dont 'nest' exceptions
            if (this.GetType() != typeof(ErrorController))
                this.InvokeHttp404(HttpContext);
        }

        public ActionResult InvokeHttp404(HttpContextBase httpContext)
        {
            IController errorController = new ErrorController();
            var errorRoute = new RouteData();
            errorRoute.Values.Add("controller", "Error");
            errorRoute.Values.Add("action", "NotFound");
            errorRoute.Values.Add("url", httpContext.Request.Url.OriginalString);
            errorController.Execute(new RequestContext(
                 httpContext, errorRoute));

            return new EmptyResult();
        }

        #endregion  
    }

    #region Supporting Class of Alert Notifications
    public class Alert
    {
        public AlertType Type { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
    }

    public enum AlertType
    {
        Info,
        Success,
        Warning,
        Error
    }
    #endregion
}