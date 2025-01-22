using AadharAdmin.BAL.Admin;
using AadharAdmin.BAL.Common;
using AadharAdmin.BAL.Operator;
using AadharAdmin.Models;
using IMI.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AadharAdmin.Controllers
{
    public class AdminController : BaseController
    {
        private IbalGeneral _General;
        private UserData objProfile;
        private balAdminProcess ObjOperProc;
        public AdminController()
        {
            ObjOperProc = new balAdminProcess();
            _General = new balGeneral();
            objProfile = new UserData();
        }
        // GET: Admin
        #region Admin Reports
        public ActionResult Index()
        {
            if (_General.CheckIsAdmin())
            {
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]
        public string GetTransDetails(string centerid, string districtname, string status, string slotdate, string Securitykey)
        {
            string tempSecuritykey = Guid.NewGuid().ToString().Replace("-", "");
            string strData = string.Empty;
            try
            {
                if (Session["Securitykey"] != null && string.Compare(Session["Securitykey"].ToString(), Securitykey, true) == 0)
                {
                    _General.ChangeSecurityKey(tempSecuritykey);

                    if (_General.CheckIsAdmin())
                    {
                        string startDate, endDate = string.Empty;                       

                        if (!string.IsNullOrEmpty(slotdate))
                        {
                            startDate = slotdate.Split('-')[0].Trim().Replace("/", "-");
                            endDate = slotdate.Split('-')[1].Trim().Replace("/", "-");
                        }
                        else
                        {
                            var dt = DateTime.Now;
                            startDate = dt.ToString("dd-MM-yyyy");
                            endDate = dt.ToString("dd-MM-yyyy");
                        }

                        ReqAdminTransDet model = new ReqAdminTransDet();
                        model.mobileno = "9533273244";
                        model.centerid = centerid;
                        model.districtname = districtname;
                        model.startdate = startDate;
                        model.status = status;
                        model.enddate = endDate;

                        var data = ObjOperProc.GetTransDetails(model);

                        strData = JsonConvert.SerializeObject(new { code = "0", Securitykey = tempSecuritykey, desc = "success", rawData = data });
                    }
                    else
                    {
                        strData = JsonConvert.SerializeObject(new { code = "401", Securitykey = tempSecuritykey, desc = "Un Authorized" });
                    }
                }
                else
                {
                    strData = JsonConvert.SerializeObject(new { code = "401", Securitykey = tempSecuritykey, desc = "Un Authorized" });
                }
            }
            catch (Exception ex)
            {
                strData = JsonConvert.SerializeObject(new { code = "1", Securitykey = tempSecuritykey, desc = "Error Occured to your request" });
                LogData.Write("AdminCtrl", "GetTransDetails", LogMode.Excep, ex.Message);
            }
            return strData;
        }
        #endregion

        #region Slot Manager for Admin
        [HttpGet]
        public ActionResult manage()
        {
            return View();
        }

        [HttpPost]
        public string GetDistricts(string Securitykey)
        {
            string tempSecuritykey = Guid.NewGuid().ToString().Replace("-", "");
            string strData = string.Empty;
            try
            {
                if (Session["Securitykey"] != null && string.Compare(Session["Securitykey"].ToString(), Securitykey, true) == 0)
                {
                    _General.ChangeSecurityKey(tempSecuritykey);
                    Request objReq = new Models.Request();
                    if (_General.CheckIsAdmin())
                    {
                        var data = ObjOperProc.GetDistricts(objReq.tid, "1234567890");
                        strData = JsonConvert.SerializeObject(new { code = "0", Securitykey = tempSecuritykey, desc = "success", rawData = data });
                    }
                    else
                    {
                        strData = JsonConvert.SerializeObject(new { code = "401", Securitykey = tempSecuritykey, desc = "Un Authorized" });
                    }
                }
                else
                {
                    strData = JsonConvert.SerializeObject(new { code = "401", Securitykey = tempSecuritykey, desc = "Un Authorized" });
                }
            }
            catch (Exception ex)
            {
                strData = JsonConvert.SerializeObject(new { code = "1", Securitykey = tempSecuritykey, desc = "Error Occured to your request" });
                LogData.Write("AdminCtrl", "GetDistricts", LogMode.Excep, ex.Message);
            }
            return strData;
        }


        [HttpPost]
        public string GetCenters(string districtid, string Securitykey)
        {
            string tempSecuritykey = Guid.NewGuid().ToString().Replace("-", "");
            string strData = string.Empty;
            try
            {
                if (Session["Securitykey"] != null && string.Compare(Session["Securitykey"].ToString(), Securitykey, true) == 0)
                {
                    _General.ChangeSecurityKey(tempSecuritykey);
                    Request objReq = new Models.Request();
                    if (_General.CheckIsAdmin())
                    {
                        var data = ObjOperProc.GetCenters(objReq.tid, "1234567890", districtid);
                        strData = JsonConvert.SerializeObject(new { code = "0", Securitykey = tempSecuritykey, desc = "success", rawData = data });
                    }
                    else
                    {
                        strData = JsonConvert.SerializeObject(new { code = "401", Securitykey = tempSecuritykey, desc = "Un Authorized" });
                    }
                }
                else
                {
                    strData = JsonConvert.SerializeObject(new { code = "401", Securitykey = tempSecuritykey, desc = "Un Authorized" });
                }
            }
            catch (Exception ex)
            {
                strData = JsonConvert.SerializeObject(new { code = "1", Securitykey = tempSecuritykey, desc = "Error Occured to your request" });
                LogData.Write("AdminCtrl", "GetDistricts", LogMode.Excep, ex.Message);
            }
            return strData;
        }

        [HttpPost]
        public string GetSlotDates(string typevalue, string Securitykey)
        {
            string tempSecuritykey = Guid.NewGuid().ToString().Replace("-", "");
            string strData = string.Empty;
            try
            {
                if (Session["Securitykey"] != null && string.Compare(Session["Securitykey"].ToString(), Securitykey, true) == 0)
                {
                    _General.ChangeSecurityKey(tempSecuritykey);
                    Request objReq = new Models.Request();
                    if (_General.CheckIsAdmin())
                    {
                        typevalue = typevalue.ToUpper();
                        var val = "1";
                        if (typevalue == "DISABLE")
                        {
                            val = "0";
                        }
                        var data = ObjOperProc.GetSlotDates(objReq.tid, "1234567890", val);
                        strData = JsonConvert.SerializeObject(new { code = "0", Securitykey = tempSecuritykey, desc = "success", rawData = data });
                    }
                    else
                    {
                        strData = JsonConvert.SerializeObject(new { code = "401", Securitykey = tempSecuritykey, desc = "Un Authorized" });
                    }
                }
                else
                {
                    strData = JsonConvert.SerializeObject(new { code = "401", Securitykey = tempSecuritykey, desc = "Un Authorized" });
                }
            }
            catch (Exception ex)
            {
                strData = JsonConvert.SerializeObject(new { code = "1", Securitykey = tempSecuritykey, desc = "Error Occured to your request" });
                LogData.Write("AdminCtrl", "GetSlotDates", LogMode.Excep, ex.Message);
            }
            return strData;
        }

        [HttpPost]
        public string GetSlotTimes(string centerid, string slotdate, string Securitykey)
        {
            string tempSecuritykey = Guid.NewGuid().ToString().Replace("-", "");
            string strData = string.Empty;
            try
            {
                if (Session["Securitykey"] != null && string.Compare(Session["Securitykey"].ToString(), Securitykey, true) == 0)
                {
                    _General.ChangeSecurityKey(tempSecuritykey);
                    Request objReq = new Models.Request();
                    if (_General.CheckIsAdmin())
                    {
                        var data = ObjOperProc.GetSlotTimings(objReq.tid, "1234567890", centerid, slotdate);
                        strData = JsonConvert.SerializeObject(new { code = "0", Securitykey = tempSecuritykey, desc = "success", rawData = data });
                    }
                    else
                    {
                        strData = JsonConvert.SerializeObject(new { code = "401", Securitykey = tempSecuritykey, desc = "Un Authorized" });
                    }
                }
                else
                {
                    strData = JsonConvert.SerializeObject(new { code = "401", Securitykey = tempSecuritykey, desc = "Un Authorized" });
                }
            }
            catch (Exception ex)
            {
                strData = JsonConvert.SerializeObject(new { code = "1", Securitykey = tempSecuritykey, desc = "Error Occured to your request" });
                LogData.Write("AdminCtrl", "GetSlotTimes", LogMode.Excep, ex.Message);
            }
            return strData;
        }

        [HttpPost]
        public string AdminSubmit(AdminUser model)
        {
            string tempSecuritykey = Guid.NewGuid().ToString().Replace("-", "");
            string strData = string.Empty;
            try
            {
                if (Session["Securitykey"] != null && string.Compare(Session["Securitykey"].ToString(), model.Securitykey, true) == 0)
                {
                    _General.ChangeSecurityKey(tempSecuritykey);
                    if (_General.CheckIsAdmin())
                    {
                        if (string.IsNullOrEmpty(model.centerid)) { model.centerid = ""; }
                        if (string.IsNullOrEmpty(model.hourid)) { model.hourid = ""; }
                        if (string.IsNullOrEmpty(model.slotid)) { model.slotid = ""; }
                        if (string.IsNullOrEmpty(model.slotdate)) { model.slotdate = ""; }
                        model.mobileno = "1234567890";
                        var data = ObjOperProc.ADMIN_OPERATIONS_CRUD(model);
                        strData = JsonConvert.SerializeObject(new { code = "0", Securitykey = tempSecuritykey, desc = "success", rawData = data });
                    }
                    else
                    {
                        strData = JsonConvert.SerializeObject(new { code = "401", Securitykey = tempSecuritykey, desc = "Un Authorized" });
                    }
                }
                else
                {
                    strData = JsonConvert.SerializeObject(new { code = "401", Securitykey = tempSecuritykey, desc = "Un Authorized" });
                }
            }
            catch (Exception ex)
            {
                strData = JsonConvert.SerializeObject(new { code = "1", Securitykey = tempSecuritykey, desc = "Error Occured to your request" });
                LogData.Write("AdminCtrl", "AdminSubmit", LogMode.Excep, ex.Message);
            }
            return strData;
        }

        #endregion

        #region Add new Centers
        public ActionResult AddCenters()
        {
            if (_General.CheckIsAdmin())
            {

                if (TempData["ErrMsg"] != null && TempData["AddCenter"] != null)
                {
                    AddCenter model = TempData["AddCenter"] as AddCenter;
                    AlertNotification(AlertType.Error, TempData["ErrMsg"].ToString());
                    return View(model);
                }
                else if(TempData["succMsg"] != null)
                {
                    AlertNotification(AlertType.Success, TempData["succMsg"].ToString());
                }
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCenters(AddCenter model)
        {
            string strErrorMsg = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (_General.CheckIsAdmin())
                    {
                        model.doneby = "ADMIN";
                        model.mobileno = "0123456789";
                        var data = ObjOperProc.ADMIN_ADD_NEW_CENTER(model);
                        var Response = JsonConvert.DeserializeObject<Response>(data.ToString());
                        if (Response.resCode == "000")
                        {
                            TempData["succMsg"] = "Cener is Created Successfully..";
                            TempData["AddCenter"] = model;
                            return RedirectToAction("AddCenters");
                        }
                        else
                        {
                            strErrorMsg = Response.resDesc;
                        }
                    }
                    else
                    {
                        return RedirectToAction("Logout", "Home");
                    }
                }
                else
                {
                    strErrorMsg = "model validation faild";
                }
            }
            catch (Exception ex)
            {
                strErrorMsg = "Error Occured to your request";
                LogData.Write("AdminCtrl", "AddCenters", LogMode.Excep, ex.Message);
            }
            TempData["AddCenter"] = model;
            TempData["ErrMsg"] = strErrorMsg;
            return RedirectToAction("AddCenters");
        }
        #endregion

        public string BookedSlotsCount(AdminUser model)
        {
            string tempSecuritykey = Guid.NewGuid().ToString().Replace("-", "");
            string strData = string.Empty;
            try
            {
                if (Session["Securitykey"] != null && string.Compare(Session["Securitykey"].ToString(), model.Securitykey, true) == 0)
                {
                    _General.ChangeSecurityKey(tempSecuritykey);
                    if (_General.CheckIsAdmin())
                    {
                        model.mobileno = "0123456789";
                        var data = ObjOperProc.ADMIN_BOOKED_SLOTS_COUNT(model);
                        strData = JsonConvert.SerializeObject(new { code = "0", Securitykey = tempSecuritykey, desc = "success", rawData = data });
                    }
                    else
                    {
                        strData = JsonConvert.SerializeObject(new { code = "401", Securitykey = tempSecuritykey, desc = "Un Authorized" });
                    }
                }
                else
                {
                    strData = JsonConvert.SerializeObject(new { code = "401", Securitykey = tempSecuritykey, desc = "Un Authorized" });
                }
            }
            catch(Exception ex)
            {
                strData = JsonConvert.SerializeObject(new { code = "1", Securitykey = tempSecuritykey, desc = "Error Occured to your request" });
                LogData.Write("AdminCtrl", "GetSlotTimes", LogMode.Excep, ex.Message);
            }
            return strData;
        }
    }
}