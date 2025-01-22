using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AadharAdmin.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult NotFound()
        {
            FormsAuthentication.SignOut();
            Response.StatusCode = 404;
            return View();
        }

        [AllowAnonymous]
        public ActionResult BadRequest()
        {
            FormsAuthentication.SignOut();
            Response.StatusCode = 403;
            return View();
        }
    }
}