using AadharAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMI.Logger;
using AadharAdmin.BAL.Common;
using System.Web.Security;
using AadharAdmin.BAL.Operator;
using Newtonsoft.Json;
using IMI.Helper;

namespace AadharAdmin.Controllers
{
    public class HomeController : BaseController
    {
        private IbalGeneral _General;
        private balOperatorProcess ObjOperProc;

        public HomeController()
        {
            _General = new balGeneral();
            ObjOperProc = new balOperatorProcess();
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            string strErrorMsg = TempData["ErrMsg"] != null ? TempData["ErrMsg"].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(strErrorMsg)) { AlertNotification(AlertType.Error, strErrorMsg); }
            FormsAuthentication.SignOut();
            if (Session["USERTYPE"] != null || Session["UserData"] != null)
            {
                Session["USERTYPE"] = null; Session["UserData"] = null;
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(loginUserNew model)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    var data = new balOperatorProcess().ValidateOtp(model.tid, model.otpVal, model.mobileno);
                    Response result = JsonConvert.DeserializeObject<Response>(data.ToString());
                    if (result.resCode == "000")
                    {
                        if (model.logintype.ToUpper() == "ADMIN")
                        {
                            Session["USERTYPE"] = "A";
                            FormsAuthentication.SetAuthCookie(model.mobileno, false);
                            return RedirectToAction("manage", "Admin");
                        }
                        else
                        {
                            #region Operator Login Functional flow
                            Session["USERTYPE"] = "O";
                            FormsAuthentication.SetAuthCookie(model.mobileno, false);
                            return RedirectToAction("Index", "Operator");
                            #endregion
                        }
                    }
                    else
                    {
                        strErrorMsg = result.resDesc;
                        ViewBag.InValidOtp = "true";
                        AlertNotification(AlertType.Error, result.resDesc);
                        return View(model);
                    }
                }
                else
                {
                    strErrorMsg = "Model validations falid.";
                }
            }
            catch (Exception ex)
            {
                LogData.Write("HomeCtrl", "Login_Post", LogMode.Excep, ex.Message);
            }
            TempData["ErrMsg"] = strErrorMsg;
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            Session.RemoveAll();
            FormsAuthentication.SignOut();
            //Removing ASP.NET_SessionId Cookie
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-10);
            }

            if (Request.Cookies["AuthenticationToken"] != null)
            {
                Response.Cookies["AuthenticationToken"].Value = string.Empty;
                Response.Cookies["AuthenticationToken"].Expires = DateTime.Now.AddMonths(-10);
            }
            return RedirectToAction("Login");
        }

        public bool IsAdmin(string usrName, string pwd)
        {
            bool isAdmin = false;
            try
            {
                string strAdminCredientls = General.GetConfigVal("ADMIN_USERNAME_PASSWORD");

                string finalUsrName = string.Empty; string finalUsrPwd = string.Empty;
                if (strAdminCredientls.IndexOf(',') > 0)
                {//For Multiple Admins Coding
                    string[] arrayUsers = strAdminCredientls.Split(',');

                    for (var i = 0; i < arrayUsers.Length; i++)
                    {
                        string[] loopedArray = arrayUsers[i].Split('-');
                        finalUsrName = loopedArray[0];
                        finalUsrPwd = loopedArray[1];
                        if ((!string.IsNullOrEmpty(finalUsrName) && string.Compare(finalUsrName.Trim().ToUpper(), usrName.Trim().ToUpper(), true) == 0) && (!string.IsNullOrEmpty(finalUsrPwd) && string.Compare(finalUsrPwd.Trim().ToUpper(), pwd.Trim().ToUpper(), true) == 0))
                        {
                            isAdmin = true;
                        }
                    }
                }
                else
                {
                    string[] arrayUsers = strAdminCredientls.Split('-');

                    finalUsrName = arrayUsers[0];
                    finalUsrPwd = arrayUsers[1];

                    if ((!string.IsNullOrEmpty(finalUsrName) && string.Compare(finalUsrName.Trim().ToUpper(), usrName.Trim().ToUpper(), true) == 0) && (!string.IsNullOrEmpty(finalUsrPwd) && string.Compare(finalUsrPwd.Trim().ToUpper(), pwd.Trim().ToUpper(), true) == 0))
                    {
                        isAdmin = true;
                    }
                }

            }
            catch (Exception ex)
            {
                LogData.Write("HomeCtrl", "IsAdmin", LogMode.Excep, ex.Message);
            }
            return isAdmin;
        }

        [HttpPost]
        [AllowAnonymous]
        public string CheckLoginFlow(loginUserNew model)
        {
            string tempSecuritykey = Guid.NewGuid().ToString().Replace("-", "");
            string strData = string.Empty;
            Response response = new Models.Response() { resCode = "1", resDesc = "No data found" };
           
            try
            {
                if (ModelState.IsValid && model.otpVal == "123456")
                {
                    if (Session["Securitykey"] != null && string.Compare(Session["Securitykey"].ToString(), model.Securitykey, true) == 0)
                    {
                        _General.ChangeSecurityKey(tempSecuritykey);
                        var profiledata = new balOperatorProcess().getProfileData(model.tid, model.mobileno);
                        UserDetail objUserdetail = JsonConvert.DeserializeObject<UserDetail>(profiledata.ToString());
                        if (objUserdetail != null && objUserdetail.resCode.ToInt() == 0)
                        {
                            if (objUserdetail.aadhar_login_type != null && objUserdetail.aadhar_login_type.ToStr().ToUpper() == model.logintype.ToUpper())
                            {
                                var data = new balOperatorProcess().GenerateOtp(model.tid, model.logintype, model.mobileno);
                                loggedUser resultData = JsonConvert.DeserializeObject<loggedUser>(data.ToString());
                                if (model.logintype.ToUpper() == "OPERATOR")
                                {
                                    if (resultData != null && resultData.resCode == "000")
                                    {
                                        response.resCode = resultData.resCode;
                                        response.resDesc = resultData.resDesc;
                                        Session["UserData"] = resultData.items;
                                        strData = JsonConvert.SerializeObject(new { code = "0", Securitykey = tempSecuritykey, desc = "success", rawData = response });

                                    }
                                    else
                                    {
                                        response.resCode = resultData.resCode;
                                        response.resDesc = resultData.resDesc;
                                        strData = JsonConvert.SerializeObject(new { code = "0", Securitykey = tempSecuritykey, desc = "success", rawData = response });
                                    }
                                }
                                else
                                {
                                    response.resCode = resultData.resCode;
                                    response.resDesc = resultData.resDesc;
                                    strData = JsonConvert.SerializeObject(new { code = "0", Securitykey = tempSecuritykey, desc = "success", rawData = response });
                                }
                            }
                            else
                            {
                                response.resCode = "401";
                                response.resDesc = "Un Authorized";
                                strData = JsonConvert.SerializeObject(new { code = "401", Securitykey = tempSecuritykey, desc = "Un Authorized", rawData = response });
                            }
                        }
                        strData = JsonConvert.SerializeObject(new { code = "1", Securitykey = tempSecuritykey, desc = "Error Occured to your request", rawData = response });
                    }
                    else
                    {
                        strData = JsonConvert.SerializeObject(new { code = "401", Securitykey = tempSecuritykey, desc = "Un Authorized", rawData = response });
                    }
                }
                else
                {
                    strData = JsonConvert.SerializeObject(new { code = "1", Securitykey = tempSecuritykey, desc = "Validation falid", rawData = response });
                }
            }
            catch (Exception ex)
            {
                strData = JsonConvert.SerializeObject(new { code = "1", Securitykey = tempSecuritykey, desc = "Error Occured to your request", rawData = response });
                LogData.Write("AdminCtrl", "CheckLoginFlow", LogMode.Excep, ex.Message);
            }
            return strData;
        }


        #region Old Login Flow don't delete
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult Login(loginUser model)
        //{
        //    string strErrorMsg = string.Empty;
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (IsAdmin(model.username, model.password))
        //            {
        //                Session["USERTYPE"] = "A";
        //                FormsAuthentication.SetAuthCookie(model.username, false);
        //                return RedirectToAction("Index", "Admin");
        //            }
        //            else
        //            {
        //                #region Operator Login Functional flow
        //                var Reslult = ObjOperProc.loginCheck(model);
        //                loggedUser resultData = JsonConvert.DeserializeObject<loggedUser>(Reslult.ToString());
        //                if (resultData != null && resultData.resCode == "000")
        //                {
        //                    Session["USERTYPE"] = "O";
        //                    Session["UserData"] = resultData.items;
        //                    FormsAuthentication.SetAuthCookie(model.username, false);
        //                    return RedirectToAction("Index", "Operator");
        //                }
        //                else
        //                {
        //                    strErrorMsg = resultData.resDesc;
        //                }
        //                #endregion
        //            }
        //        }
        //        else
        //        {
        //            strErrorMsg = "Model validations falid.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogData.Write("HomeCtrl", "Login_Post", LogMode.Excep, ex.Message);
        //    }
        //    TempData["ErrMsg"] = strErrorMsg;
        //    return RedirectToAction("Login");
        //}
        #endregion
    }
}
