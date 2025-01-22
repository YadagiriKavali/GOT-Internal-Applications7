using AadharAdmin.BAL.Common;
using AadharAdmin.Models;
using AadharAdmin.Utilities;
using IMI.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AadharAdmin.BAL.Admin
{
    public class balAdminProcess
    {
        private WebRequestProcess WebReqProc;
        private readonly string serviceName = "AADHAR";
        public balAdminProcess()
        {
            WebReqProc = new WebRequestProcess();
        }
        internal object GetTransDetails(ReqAdminTransDet model)
        {
            try
            {
                var reqData = JsonConvert.SerializeObject(model);
                var webResponse = WebReqProc.HttpJsonPost(serviceName, "14", reqData);
                return JsonConvert.DeserializeObject(webResponse);
            }
            catch (Exception ex)
            {
                LogData.Write("balAdminProcess", "GetTransDetails", LogMode.Excep, "ex " + ex.Message + "Tid:" + model.tid + ",Center id :" + model.centerid);
            }
            return null;
        }

        internal object GetDistricts(string tid, string mobileNo)
        {
            try
            {
                var reqData = string.Format("{{\"tid\":\"{0}\",\"mobileNo\":\"{1}\",\"channel\":\"WEB\"}}", tid, mobileNo);
                var webResponse = WebReqProc.HttpJsonPost(serviceName, "16", reqData);
                return JsonConvert.DeserializeObject(webResponse);
            }
            catch (Exception ex)
            {
                LogData.Write("balAdminProcess", "GetDistricts", LogMode.Excep, "ex " + ex.Message);
            }
            return null;
        }

        internal object GetCenters(string tid, string mobileNo, string districtid)
        {
            try
            {
                var reqData = string.Format("{{\"tid\":\"{0}\",\"mobileNo\":\"{1}\",\"channel\":\"WEB\",\"districtid\":\"{2}\"}}", tid, mobileNo, districtid); ;
                var webResponse = WebReqProc.HttpJsonPost(serviceName, "17", reqData);
                return JsonConvert.DeserializeObject(webResponse);
            }
            catch (Exception ex)
            {
                LogData.Write("balAdminProcess", "GetDistricts", LogMode.Excep, "ex " + ex.Message);
            }
            return null;
        }

        internal object GetSlotDates(string tid, string mobileNo, string type)
        {
            try
            {
                var reqData = string.Format("{{\"tid\":\"{0}\",\"mobileNo\":\"{1}\",\"channel\":\"WEB\",\"type\":\"{2}\"}}", tid, mobileNo, type);
                var webResponse = WebReqProc.HttpJsonPost(serviceName, "15", reqData);
                return JsonConvert.DeserializeObject(webResponse);
            }
            catch (Exception ex)
            {
                LogData.Write("balAdminProcess", "GetSlotDates", LogMode.Excep, "ex " + ex.Message);
            }
            return null;
        }

        internal object GetSlotTimings(string tid, string mobileNo, string centerid, string slotdate)
        {
            try
            {
                var reqData = string.Format("{{\"tid\":\"{0}\",\"mobileNo\":\"{1}\",\"channel\":\"WEB\",\"centerid\":\"{2}\",\"slotdate\":\"{3}\"}}", tid, mobileNo, centerid, slotdate);
                var webResponse = WebReqProc.HttpJsonPost(serviceName, "4", reqData);
                return JsonConvert.DeserializeObject(webResponse);
            }
            catch (Exception ex)
            {
                LogData.Write("balAdminProcess", "GetSlotTimings", LogMode.Excep, "ex " + ex.Message);
            }
            return null;
        }

        internal object ADMIN_OPERATIONS_CRUD(AdminUser model)
        {
            try
            {
                var reqData = JsonConvert.SerializeObject(model);
                var webResponse = WebReqProc.HttpJsonPost(serviceName, "18", reqData);
                return JsonConvert.DeserializeObject(webResponse);
            }
            catch (Exception ex)
            {
                LogData.Write("balAdminProcess", "ADMIN_OPERATIONS_CRUD", LogMode.Excep, "ex " + ex.Message);
            }
            return null;
        }

        internal object ADMIN_ADD_NEW_CENTER(AddCenter model)
        {
            try
            {

                model.password = new RC4(model.password).Encrypt();
                var reqData = JsonConvert.SerializeObject(model);
                var webResponse = WebReqProc.HttpJsonPost(serviceName, "19", reqData);
                return JsonConvert.DeserializeObject(webResponse);
            }
            catch (Exception ex)
            {
                LogData.Write("balAdminProcess", "ADMIN_ADD_NEW_CENTER", LogMode.Excep, "ex " + ex.Message);
            }
            return null;
        }


        internal object ADMIN_BOOKED_SLOTS_COUNT(AdminUser model)
        {
            try
            {
                var reqData = JsonConvert.SerializeObject(model);
                var webResponse = WebReqProc.HttpJsonPost(serviceName, "20", reqData);
                return JsonConvert.DeserializeObject(webResponse);
            }
            catch (Exception ex)
            {
                LogData.Write("balAdminProcess", "ADMIN_BOOKED_SLOTS_COUNT", LogMode.Excep, "ex " + ex.Message);
            }
            return null;
        }
    }
}