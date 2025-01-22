using AadharAdmin.BAL.Common;
using AadharAdmin.Models;
using AadharAdmin.Utilities;
using IMI.Logger;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AadharAdmin.BAL.Operator
{
    public class balOperatorProcess
    {
        private WebRequestProcess WebReqProc;
        private readonly string serviceName = "AADHAR";
        public balOperatorProcess()
        {
            WebReqProc = new WebRequestProcess();
        }

        internal object loginCheck(loginUser model)
        {
            try
            {
                model.password = new RC4(model.password).Encrypt();
                var reqData = JsonConvert.SerializeObject(model);
                var webResponse = WebReqProc.HttpJsonPost(serviceName, "11", reqData);
                return JsonConvert.DeserializeObject(webResponse);
            }
            catch (Exception ex)
            {
                LogData.Write("balUsers", "loginCheck", LogMode.Excep, "ex " + ex.Message + "Tid:" + model.tid + ",username :" + model.username);
            }
            return null;
        }

        internal object GetTransDetails(ReqTransDet model)
        {
            try
            {
                var reqData = JsonConvert.SerializeObject(model);
                var webResponse = WebReqProc.HttpJsonPost(serviceName, "12", reqData);
                return JsonConvert.DeserializeObject(webResponse);
            }
            catch (Exception ex)
            {
                LogData.Write("balOperatorProcess", "GetTransDetails", LogMode.Excep, "ex " + ex.Message + "Tid:" + model.tid + ",Center Code :" + model.centrcode);
            }
            return null;
        }

        internal object UserPassCodeAuth(SlotPassCodeAuthReq model)
        {
            try
            {
                var reqData = JsonConvert.SerializeObject(model);
                var webResponse = WebReqProc.HttpJsonPost(serviceName, "13", reqData);
                return JsonConvert.DeserializeObject(webResponse);
            }
            catch (Exception ex)
            {
                LogData.Write("balOperatorProcess", "UserPassCodeAuth", LogMode.Excep, "ex " + ex.Message + "Tid:" + model.tid + ",mgovid :" + model.mgovid);
            }
            return null;
        }

        internal object getProfileData(string tid, string mobileno)
        {
            try
            {
                var reqData = string.Format("{{\"tid\":\"{0}\",\"channel\":\"Web\",\"mobileno\":\"{1}\"}}", tid, mobileno);
                var webResponse = WebReqProc.HttpJsonPost("user", "31", reqData);
                return JsonConvert.DeserializeObject(webResponse);
            }
            catch (Exception ex)
            {
                LogData.Write("balUsers", "getProfileData", LogMode.Excep, "ex " + ex.Message + "Tid:" + tid + ",mobileno :" + mobileno);
            }
            return null;
        }

        internal object GenerateOtp(string tid, string logintype, string mobileno)
        {
            try
            {
                var reqData = string.Format("{{\"tid\":\"{0}\",\"channel\":\"Web\",\"mobileno\":\"{1}\",\"logintype\":\"{2}\"}}", tid, mobileno, logintype);
                var webResponse = WebReqProc.HttpJsonPost(serviceName, "21", reqData);
                return JsonConvert.DeserializeObject(webResponse);
            }
            catch (Exception ex)
            {
                LogData.Write("balUsers", "GenerateOtp", LogMode.Excep, "ex " + ex.Message + "Tid:" + tid + ",mobileno :" + mobileno);
            }
            return null;
        }

        internal object ValidateOtp(string tid, string otp, string mobileno)
        {
            try
            {
                var reqData = string.Format("{{\"tid\":\"{0}\",\"channel\":\"Web\",\"mobileno\":\"{1}\",\"otp\":\"{2}\"}}", tid, mobileno, otp);
                var webResponse = WebReqProc.HttpJsonPost(serviceName, "22", reqData);
                return JsonConvert.DeserializeObject(webResponse);
            }
            catch (Exception ex)
            {
                LogData.Write("balUsers", "ValidateOtp", LogMode.Excep, "ex " + ex.Message + "Tid:" + tid + ",mobileno :" + mobileno);
            }
            return null;
        }
    }
}