using AadharAdmin.BAL.Common;
using AadharAdmin.BAL.Operator;
using AadharAdmin.Models;
using IMI.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace AadharAdmin.Controllers
{
    public class OperatorController : BaseController
    {
        private IbalGeneral _General;
        private balOperatorProcess ObjOperProc;
        private UserData objProfile;
        public OperatorController()
        {
            ObjOperProc = new balOperatorProcess();
            _General = new balGeneral();
            objProfile = new UserData();
        }
        // GET: Operator
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string GetTransDetails(string slotdate, string Securitykey)
        {
            string tempSecuritykey = Guid.NewGuid().ToString().Replace("-", "");
            string strData = string.Empty;
            try
            {
                if (Session["Securitykey"] != null && string.Compare(Session["Securitykey"].ToString(), Securitykey, true) == 0)
                {
                    _General.ChangeSecurityKey(tempSecuritykey);

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

                    objProfile = GetProfileInfo();
                    if (objProfile != null)
                    {
                        ReqTransDet model = new ReqTransDet();
                        model.mobileno = objProfile.mobileno;
                        model.centerid = objProfile.centerid;
                        model.centrcode = objProfile.centercode;
                        model.startdate = startDate;
                        model.enddate = endDate;
                        model.usertype = Session["USERTYPE"].ToString().ToUpper();
                        var data = ObjOperProc.GetTransDetails(model);

                        listOperTransDet lisData = JsonConvert.DeserializeObject<listOperTransDet>(data.ToString());
                        if (lisData.resCode == "000")
                        {
                            for (int rw = 0; rw < lisData.TransDet.Count; rw++)
                            {
                                var slotDate = lisData.TransDet[rw].slotdate;
                                DateTime dtSlotDate = new DateTime(1900, 1, 1);
                                DateTime.TryParseExact(slotDate, "dd-MM-yyyy", null, DateTimeStyles.None, out dtSlotDate);
                                DateTime currentDate = new DateTime(1900, 1, 1);
                                DateTime.TryParseExact(DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", null, DateTimeStyles.None, out currentDate);
                                if (dtSlotDate != currentDate)
                                {
                                    lisData.TransDet[rw].ActionStatus = "-1";
                                }
                                else
                                {
                                    if (lisData.TransDet[rw].statusid == "1")
                                    {
                                        lisData.TransDet[rw].ActionStatus = "1";
                                    }
                                }
                            }
                            data = lisData;
                        }


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
                LogData.Write("OperatorCtrl", "GetTransDetails", LogMode.Excep, ex.Message);
            }
            return strData;
        }


        [HttpPost]
        public string UserSlotAuth(string UserPassCode, string mGovId, string slotdate, string Securitykey)
        {
            string tempSecuritykey = Guid.NewGuid().ToString().Replace("-", "");
            string strData = string.Empty;
            try
            {
                if (Session["Securitykey"] != null && string.Compare(Session["Securitykey"].ToString(), Securitykey, true) == 0)
                {
                    _General.ChangeSecurityKey(tempSecuritykey);
                    DateTime dtSlotDate = new DateTime(1900, 1, 1);
                    DateTime.TryParseExact(slotdate, "dd-MM-yyyy", null, DateTimeStyles.None, out dtSlotDate);
                    DateTime currentDate = new DateTime(1900, 1, 1);
                    DateTime.TryParseExact(DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", null, DateTimeStyles.None, out currentDate);
                    if (dtSlotDate == currentDate)
                    {
                        if (_General.ValidatePassCode(UserPassCode) && _General.ValidateMGOVID(mGovId))
                        {
                            SlotPassCodeAuthReq model = new SlotPassCodeAuthReq();
                            objProfile = GetProfileInfo();
                            if (objProfile != null)
                            {
                                model.passcode = UserPassCode;
                                model.centerid = objProfile.centerid;
                                model.mgovid = mGovId;
                                model.mobileno = objProfile.mobileno;
                                var data = ObjOperProc.UserPassCodeAuth(model);
                                strData = JsonConvert.SerializeObject(new { code = "0", Securitykey = tempSecuritykey, desc = "success", rawData = data });
                            }
                            else
                            {
                                strData = JsonConvert.SerializeObject(new { code = "401", Securitykey = tempSecuritykey, desc = "Un Authorized" });
                            }
                        }
                        else
                        {
                            strData = JsonConvert.SerializeObject(new { code = "1", Securitykey = tempSecuritykey, desc = "Invalid Passcode" });
                        }
                    }
                    else
                    {
                        strData = JsonConvert.SerializeObject(new { code = "1", Securitykey = tempSecuritykey, desc = "Not elegible for Authorized" });
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
                LogData.Write("OperatorCtrl", "UserSlotAuth", LogMode.Excep, ex.Message);
            }
            return strData;
        }

    }
}