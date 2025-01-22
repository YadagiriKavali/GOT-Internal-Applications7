using System;
using System.Configuration;
using IMI.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SMSAPI.Models.Requests;
using SMSAPI.Utilities;
using IMI.SqlWrapper;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Linq;
namespace SMSAPI.BL
{
    public class ProcessBillDetailReq
    {
        #region Private Members

        public string strAPIURL = clsGeneral.GetConfigVal("CONTROL_API_URL");
        Hashtable htHeaders = new Hashtable();
        public string strWorkSpaceId = "smsapi";
        public string strErrorFileName = "smsapi_methods";
        String strRequest = String.Empty;
        String strResp = String.Empty;
        public string strHttpMethod = "POST";
        public string strContentType = "application/json";
        public int iWebReqTimeOut = clsGeneral.GetConfigVal("REQ_TIMEOUT").ToInt();
        private string strPGServiceId = String.Empty;
        #endregion Private Members

        #region Public Methods

        public string ProcessRequest(BillDetailReq data)
        {
            var message = string.Empty;

            if (data != null && !string.IsNullOrEmpty(data.SMS) && !string.IsNullOrEmpty(data.CMD))
            {
                
                var keywords = data.SMS.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (data.msisdn.Length > 10)
                    data.msisdn = data.msisdn.Substring(data.msisdn.Length - 10, 10);

                Dictionary<String, String> dicUser = User_Check_Existance(data.logtid, data.msisdn);
                if (dicUser.GetValue("resCode") != "000")
                {
                    TextLog.Exception(strWorkSpaceId, "USER_EX", "User checking failed. Mobile no:" + data.msisdn + ",tid:" + data.logtid);
                    message = clsGeneral.GetConfigVal("EXCEPTION_MSG");
                    return message;
                }
                dicUser = null;
                switch (data.CMD.ToLower())
                {
                    case "tapp":
                    case "menu":
                    case "help":
                        message = clsGeneral.GetConfigVal("GENERAL_HELP_MSG");
                        break;
                    case "twallet":
                        message = GenerateTwalletOTP(data.logtid, data.msisdn, keywords);
                        break;
                    case "airtel":
                        message = HandleAirtelServiceRequests(data.logtid, data.msisdn, keywords);
                        break;
                    case "airtell":
                        message = HandleAirtelLandlineServiceRequests(data.logtid, data.msisdn, keywords);
                        break;
                    case "idea":
                        message = HandleIdeaServiceRequests(data.logtid, data.msisdn, keywords);
                        break;
                    case "ttl":
                        message = HandleTTLServiceRequests(data.logtid, data.msisdn, keywords);
                        break;
                    case "ttll":
                        message = HandleTTLLandLineServiceRequests(data.logtid, data.msisdn, keywords);
                        break;
                    case "bsnlm":
                        message = HandleBSNLServiceRequests(data.logtid, data.msisdn, keywords);
                        break;
                    case "bsnll":
                        message = HandleBSNLLandlineServiceRequests(data.logtid, data.msisdn, keywords);
                        break;
                    case "vodf":
                        message = HandleVodafoneServiceRequests(data.logtid, data.msisdn, keywords);
                        break;
                    case "relcm":
                        message = HandleRelianceServiceRequests(data.logtid, data.msisdn, keywords);
                        break;
                    case "hmwssb":
                        message = HandleHMWSSBServiceRequests(data.logtid, data.msisdn, keywords);
                        break;
                    case "act":
                        message = HandleACTServiceRequests(data.logtid, data.msisdn, keywords);
                        break;
                    case "rta":
                        message = clsGeneral.GetConfigVal("RTA_HELP_MSG");
                        break;
                    case "rtapay":
                        message = HandleRTAServiceRequests(data.logtid, data.msisdn, keywords);
                        break;
                    case "rtalt":
                        message = HandleRTALifeTaxServiceRequests(data.logtid, data.msisdn, keywords);
                        break;
                    //case "rtall":
                    //    message = HandleRTALLTestFeeServiceRequests(data.logtid, data.msisdn, keywords);
                    //    break;
                    case "status":
                        message = HandleApplicationStatusRequests(data.logtid, data.msisdn, keywords);
                        break;
                    case "hathway":
                        message = HandleHathwayServiceRequests(data.logtid, data.msisdn, keywords);
                        break;

                }
            }

            if (string.IsNullOrEmpty(message))
                message = clsGeneral.GetConfigVal("EXCEPTION_MSG");
            TextLog.Debug(strWorkSpaceId, "landedrequests", "msisdn:" + data.msisdn.ToStr() + ", cmd:" + data.CMD.ToStr() + ", sms:" + data.SMS.ToStr() + ", tid:" + data.logtid.ToStr() + Environment.NewLine + ",Response:" + message);
            return message;
        }

        #endregion Public Methods

        #region Private Methods
        #region Service landing Methods
        private string HandleAirtelServiceRequests(string transId, string strMobileNo, string[] strArrData)
        {
            String strRes = String.Empty;
            if (strArrData.Length == 1)
            {
                strRes = clsGeneral.GetConfigVal("AIRTEL_POSTPAID_HELP_MSG");
            }
            //else if (strArrData.Length == 2)
            //{
            //    strRes = GetAirtelBillDetail(transId, strMobileNo, strArrData);
            //}
            else if (strArrData.Length == 5)
            {
                strRes = AirtelBillPayment(transId, strMobileNo, strArrData);
            }
            else
            {
                strRes = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
            }
            return strRes;
        }

        private string HandleHMWSSBServiceRequests(string transId, string strMobileNo, string[] strArrData)
        {
            String strRes = String.Empty;
            if (strArrData.Length == 1)
            {
                strRes = clsGeneral.GetConfigVal("HMWSSB_HELP_MSG");
            }
            else if (strArrData.Length == 2)
            {
                strRes = GetWaterBillDetail(transId, strMobileNo, strArrData);
            }
            else if (strArrData.Length == 4)
            {
                strRes = HMWSSBBillPayment(transId, strMobileNo, strArrData);
            }
            else
            {
                strRes = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
            }
            return strRes;
        }

        private string HandleAirtelLandlineServiceRequests(string transId, string strMobileNo, string[] strArrData)
        {
            String strRes = String.Empty;
            if (strArrData.Length == 1)
            {
                strRes = clsGeneral.GetConfigVal("AIRTEL_LANDLINE_HELP_MSG");
            }
            //if (strArrData.Length == 2)
            //{
            //    strRes = GetAirtelLLBillDetail(transId, strMobileNo, strArrData);
            //}
            else if (strArrData.Length == 5)
            {
                strRes = AirtelLandlineBillPayment(transId, strMobileNo, strArrData);
            }
            else
            {
                strRes = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
            }
            return strRes;
        }

        private string HandleIdeaServiceRequests(string transId, string strMobileNo, string[] strArrData)
        {
            String strRes = String.Empty;
            if (strArrData.Length == 1)
            {
                strRes = clsGeneral.GetConfigVal("IDEA_POSTPAID_HELP_MSG");
            }
            //else if (strArrData.Length == 2)
            //{
            //    strRes = GetIdeaBillDetail(transId, strMobileNo, strArrData);
            //}
            else if (strArrData.Length == 5)
            {
                strRes = IDEABillPayment(transId, strMobileNo, strArrData);
            }
            else
            {
                strRes = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
            }
            return strRes;
        }

        private string HandleACTServiceRequests(string transId, string strMobileNo, string[] strArrData)
        {
            String strRes = String.Empty;
            if (strArrData.Length == 1)// help message
            {
                strRes = clsGeneral.GetConfigVal("ACT_HELP_MSG");
            }
            else if (strArrData.Length == 2)//view bill
            {
                strRes = GetACTBillDetail(transId, strMobileNo, strArrData);
            }
            else if (strArrData.Length == 4)//pay bill
            {
                strRes = ACTBillPayment(transId, strMobileNo, strArrData);
            }
            else
            {
                strRes = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
            }
            return strRes;
        }

        private string HandleRTAServiceRequests(string transId, string strMobileNo, string[] strArrData)
        {
            String strRes = String.Empty;
            if (strArrData.Length == 2)
            {
                strRes = GetRTAFeeDetail(transId, strMobileNo, strArrData);
            }
            else if (strArrData.Length == 4)
            {
                strRes = RTABillPayment(transId, strMobileNo, strArrData);
            }
            else
            {
                strRes = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
            }
            return strRes;
        }

        private string HandleTTLServiceRequests(string transId, string strMobileNo, string[] strArrData)
        {
            String strRes = String.Empty;
            if (strArrData.Length == 1)
            {
                strRes = clsGeneral.GetConfigVal("TATA_POSTPAID_HELP_MSG");
            }
            //else if (strArrData.Length == 2)
            //{
            //    strRes = GetTataBillDetail(transId, strMobileNo, strArrData);
            //}
            else if (strArrData.Length == 5)
            {
                strRes = TTLBillPayment(transId, strMobileNo, strArrData);
            }
            else
            {
                strRes = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
            }
            return strRes;
        }

        private string HandleTTLLandLineServiceRequests(string transId, string strMobileNo, string[] strArrData)
        {
            String strRes = String.Empty;
            if (strArrData.Length == 1)
            {
                strRes = clsGeneral.GetConfigVal("TATA_LANDLINE_HELP_MSG");
            }
            //else if (strArrData.Length == 2)
            //{
            //    strRes = GetTataBillDetail(transId, strMobileNo, strArrData);
            //}
            else if (strArrData.Length == 5)
            {
                strRes = TTLLandlineBillPayment(transId, strMobileNo, strArrData);
            }
            else
            {
                strRes = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
            }
            return strRes;
        }

        private string HandleBSNLServiceRequests(string transId, string strMobileNo, string[] strArrData)
        {
            String strRes = String.Empty;
            if (strArrData.Length == 1)
            {
                strRes = clsGeneral.GetConfigVal("BSNL_POSTAPID_HELP_MSG");
            }
            //else if (strArrData.Length == 2)
            //{
            //    strRes = GetBsnlBillDetail(transId, strMobileNo, strArrData);
            //}
            else if (strArrData.Length == 5)
            {
                strRes = BsnlBillPayment(transId, strMobileNo, strArrData);
            }
            else
            {
                strRes = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
            }
            return strRes;
        }

        private string HandleBSNLLandlineServiceRequests(string transId, string strMobileNo, string[] strArrData)
        {
            String strRes = String.Empty;
            if (strArrData.Length == 1)
            {
                strRes = clsGeneral.GetConfigVal("BSNL_LANDLINE_HELP_MSG");
            }
            //else if (strArrData.Length == 2)
            //{
            //    strRes = GetBsnlLandlineBillDetail(transId, strMobileNo, strArrData);
            //}
            else if (strArrData.Length == 5)
            {
                strRes = BsnlLandlineBillPayment(transId, strMobileNo, strArrData);
            }
            else
            {
                strRes = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
            }
            return strRes;
        }

        private string HandleRTALifeTaxServiceRequests(string transId, string strMobileNo, string[] strArrData)
        {
            String strRes = String.Empty;
            if (strArrData.Length == 2)
            {
                strRes = GetRTALifeTaxBillDetail(transId, strMobileNo, strArrData);
            }
            else if (strArrData.Length == 4)
            {
                strRes = RTALifeTaxBillPayment(transId, strMobileNo, strArrData);
            }
            else
            {
                strRes = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
            }
            return strRes;
        }

        //private string HandleRTALLTestFeeServiceRequests(string transId, string strMobileNo, string[] strArrData)
        //{
        //    String strRes = String.Empty;
        //    if (strArrData.Length == 2)
        //    {
        //        strRes = GetRTALLFeeDetail(transId, strMobileNo, strArrData);
        //    }
        //    else if (strArrData.Length == 4)
        //    {
        //        strRes = RTALLTestFeePayment(transId, strMobileNo, strArrData);
        //    }
        //    else
        //    {
        //        strRes = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
        //    }
        //    return strRes;
        //}

        private string HandleVodafoneServiceRequests(string transId, string strMobileNo, string[] strArrData)
        {
            String strRes = String.Empty;
            if (strArrData.Length == 1)
            {
                strRes = clsGeneral.GetConfigVal("VODAFONE_HELP_MSG");//GetVodafoneBillDetail(transId, strMobileNo, strArrData);
            }
            else if (strArrData.Length == 5)
            {
                strRes = VodafoneBillPayment(transId, strMobileNo, strArrData);
            }
            else
            {
                strRes = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
            }
            return strRes;
        }

        private string HandleRelianceServiceRequests(string transId, string strMobileNo, string[] strArrData)
        {
            String strRes = String.Empty;
            if (strArrData.Length == 1)
            {
                strRes = clsGeneral.GetConfigVal("RELIANCE_HELP_MSG");//GetVodafoneBillDetail(transId, strMobileNo, strArrData);
            }
            else if (strArrData.Length == 5)
            {
                strRes = RelianceBillPayment(transId, strMobileNo, strArrData);
            }
            else
            {
                strRes = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
            }
            return strRes;
        }

        private string HandleApplicationStatusRequests(string transId, string strMobileNo, string[] strArrData)
        {
            String strRes = String.Empty;
            if (strArrData.Length == 1)//help msg
            {
                strRes = clsGeneral.GetConfigVal("APPLICATIONSTATUS_HELP_MSG");
            }
            else if (strArrData.Length == 2)//view status
            {
                strRes = GetMeesevaApplicationStatus(transId, strMobileNo, strArrData);
            }
            else
            {
                strRes = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
            }
            return strRes;
        }

        private string HandleHathwayServiceRequests(string transId, string strMobileNo, string[] strArrData)
        {
            String strRes = String.Empty;
            if (strArrData.Length == 1)// help message
            {
                strRes = clsGeneral.GetConfigVal("HATHWAY_HELP_MSG");
            }
            else if (strArrData.Length == 2)//view bill
            {
                strRes = GetHATHWAYBillDetail(transId, strMobileNo, strArrData);
            }
            else if (strArrData.Length == 4)//pay bill
            {
                strRes = HATHWAYBillPayment(transId, strMobileNo, strArrData);
            }
            else
            {
                strRes = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
            }
            return strRes;
        }
        #endregion

        #region View Bill Methods
        #region not using
        //private string GetAirtelBillDetail(string transId, string strMobileNo, string[] strArrData)
        //{
        //    String resMsg = string.Empty;
        //    String message = String.Empty;
        //    String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(number) || number.Trim().Length != 10)
        //        {
        //            TextLog.Exception("SMSAPI", "AIRTEL_EX", "Invalid number , tid:" + transId + ",number:" + number);
        //            message = clsGeneral.GetConfigVal("INVALID_NUMBER");
        //        }
        //        else
        //        {
        //            AddHeaders("airtel_viewbill", ref htHeaders);
        //            strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + strArrData[1] + "\",\"Channel\":\"SMS\"}";
        //            strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
        //            if (!string.IsNullOrEmpty(strResp))
        //            {
        //                //object obj = JsonConvert.DeserializeObject(strResp);
        //                //dynamic data = obj;
        //                // JObject Json = data;
        //                JObject Json = JObject.Parse(strResp);
        //                message = clsGeneral.GetConfigVal("AIRTEL_SMS_MSG");
        //                if (Json.GetValue("resCode").ToString() == "000")
        //                {
        //                    message = message.Replace("!CONSUMERNAME!", Json.GetValue("consumerName") != null ? Json.GetValue("consumerName").ToString() : string.Empty);
        //                    message = message.Replace("!AMOUNT!", Json.GetValue("billAmount") != null ? Json.GetValue("billAmount").ToString() : string.Empty);
        //                    message = message.Replace("!ACOUNTNO!", Json.GetValue("accountNo") != null ? Json.GetValue("accountNo").ToString() : string.Empty);
        //                    message = message.Replace("!NUMBER!", number);
        //                }
        //                else
        //                    message = Json.GetValue("resDesc").ToString();
        //            }
        //        }
        //        resMsg = message;
        //    }
        //    catch (Exception ex)
        //    {
        //        TextLog.Exception(strWorkSpaceId, "AIRTEL_EX", string.Format("ProcessBillDetailReq- GetAirtelBillDetail- Ex:{0}", ex.Message));
        //    }
        //    finally
        //    {
        //        TextLog.Debug(strWorkSpaceId, "AIRTEL", "GetAirtelBillDetail, strRequest:" + strRequest + ",strResp:" + strResp);
        //        htHeaders = null;
        //    }
        //    return resMsg;
        //}

        //private string GetAirtelLLBillDetail(string transId, string strMobileNo, string[] strArrData)
        //{
        //    String resMsg = string.Empty;
        //    String message = String.Empty;
        //    String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;

        //    try
        //    {
        //        if (string.IsNullOrEmpty(number))
        //        {
        //            TextLog.Exception(strWorkSpaceId, "AIRTEL_LL", "Invalid number , transId:" + transId + ",number:" + number);
        //            message = clsGeneral.GetConfigVal("INVALID_NUMBER");
        //        }
        //        else
        //        {
        //            AddHeaders("airtel_landline_viewbill", ref htHeaders);
        //            strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + number + "\",\"Channel\":\"SMS\"}";
        //            strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
        //            if (!string.IsNullOrEmpty(strResp))
        //            {
        //                JObject Json = JObject.Parse(strResp);
        //                message = clsGeneral.GetConfigVal("AIRTEL_LL_SMS_MSG");
        //                if (Json.GetValue("resCode").ToString() == "000")
        //                {
        //                    message = message.Replace("!CONSUMERNAME!", Json.GetValue("consumerName") != null ? Json.GetValue("consumerName").ToString() : string.Empty);
        //                    message = message.Replace("!AMOUNT!", Json.GetValue("billAmount") != null ? Json.GetValue("billAmount").ToString() : string.Empty);
        //                    message = message.Replace("!ACOUNTNO!", Json.GetValue("accountNo") != null ? Json.GetValue("accountNo").ToString() : string.Empty);
        //                    message = message.Replace("!NUMBER!", number);
        //                }
        //                else
        //                {
        //                    message = Json.GetValue("resDesc").ToString();
        //                }
        //            }
        //        }

        //        resMsg = message;
        //    }
        //    catch (Exception ex)
        //    {
        //        TextLog.Exception(strWorkSpaceId, "AIRTEL_LL_EX", string.Format("ProcessBillDetailReq- GetAirtelLLBillDetail- Ex:{0}", ex.Message));
        //    }
        //    finally
        //    {
        //        TextLog.Debug(strWorkSpaceId, "AIRTEL_LL", "GetAirtelLLBillDetail, strRequest:" + strRequest + ",strResp:" + strResp);
        //        htHeaders = null;
        //    }
        //    return resMsg;
        //}

        //private string GetIdeaBillDetail(string transId, string strMobileNo, string[] strArrData)
        //{
        //    String resMsg = string.Empty;
        //    String message = String.Empty;
        //    String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(number))
        //        {
        //            TextLog.Exception(strWorkSpaceId, "IDEA_EX", "Invalid number , transId:" + transId + ",number:" + number);
        //            message = clsGeneral.GetConfigVal("INVALID_NUMBER");
        //        }
        //        else
        //        {
        //            AddHeaders("idea_viewbill", ref htHeaders);
        //            strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + number + "\",\"Channel\":\"SMS\"}";
        //            strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
        //            if (!string.IsNullOrEmpty(strResp))
        //            {
        //                JObject Json = JObject.Parse(strResp);
        //                message = clsGeneral.GetConfigVal("IDEA_SMS_MSG");
        //                if (Json.GetValue("resCode").ToString() == "000")
        //                {
        //                    message = message.Replace("!CONSUMERNAME!", Json.GetValue("consumerName") != null ? Json.GetValue("consumerName").ToString() : string.Empty);
        //                    message = message.Replace("!AMOUNT!", Json.GetValue("billAmount") != null ? Json.GetValue("billAmount").ToString() : string.Empty);
        //                    message = message.Replace("!ACOUNTNO!", Json.GetValue("accountNo") != null ? Json.GetValue("accountNo").ToString() : string.Empty);
        //                    message = message.Replace("!ADDRESS1!", Json.GetValue("address1") != null ? Json.GetValue("address1").ToString() : string.Empty);
        //                    message = message.Replace("!ADDRESS2!", Json.GetValue("address2") != null ? Json.GetValue("address2").ToString() : string.Empty);
        //                    message = message.Replace("!ADDRESS3!", Json.GetValue("address3") != null ? Json.GetValue("address3").ToString() : string.Empty);
        //                    message = message.Replace("!NUMBER!", number);
        //                }
        //                else
        //                {
        //                    message = Json.GetValue("resDesc").ToString();
        //                }
        //            }
        //        }
        //        resMsg = message;
        //    }
        //    catch (Exception ex)
        //    {
        //        TextLog.Exception(strWorkSpaceId, "IDEA_EX", string.Format("ProcessBillDetailReq- GetIdeaBillDetail- Ex:{0}", ex.Message));
        //    }
        //    finally
        //    {
        //        TextLog.Debug(strWorkSpaceId, "IDEA", "GetIdeaBillDetail, strRequest:" + strRequest + ",strResp:" + strResp);
        //        htHeaders = null;
        //    }
        //    return resMsg;
        //}

        //private string GetBsnlBillDetail(string transId, string strMobileNo, string[] strArrData)
        //{
        //    String resMsg = string.Empty;
        //    String message = String.Empty;
        //    String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(number) || number.Trim().Length != 10)
        //        {
        //            TextLog.Exception("SMSAPI", "BSNL_EX", "Invalid number , tid:" + transId + ",number:" + number);
        //            message = clsGeneral.GetConfigVal("INVALID_NUMBER");
        //        }
        //        else
        //        {
        //            AddHeaders("bsnl_viewbill", ref htHeaders);
        //            strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + strArrData[1] + "\",\"Channel\":\"SMS\"}";
        //            strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
        //            if (!string.IsNullOrEmpty(strResp))
        //            {
        //                JObject Json = JObject.Parse(strResp);
        //                message = clsGeneral.GetConfigVal("BSNL_SMS_MSG");
        //                if (Json.GetValue("resCode").ToString() == "000")
        //                {
        //                    message = message.Replace("!CONSUMERNAME!", Json.GetValue("consumerName") != null ? Json.GetValue("consumerName").ToString() : string.Empty);
        //                    message = message.Replace("!AMOUNT!", Json.GetValue("billAmount") != null ? Json.GetValue("billAmount").ToString() : string.Empty);
        //                    message = message.Replace("!ACOUNTNO!", Json.GetValue("accountNo") != null ? Json.GetValue("accountNo").ToString() : string.Empty);
        //                    message = message.Replace("!NUMBER!", number);
        //                }
        //                else
        //                    message = Json.GetValue("resDesc").ToString();
        //            }
        //        }
        //        resMsg = message;
        //    }
        //    catch (Exception ex)
        //    {
        //        TextLog.Exception(strWorkSpaceId, "BSNL_EX", string.Format("ProcessBillDetailReq- GetBsnlBillDetail- Ex:{0}", ex.Message));
        //    }
        //    finally
        //    {
        //        TextLog.Debug(strWorkSpaceId, "BSNL", "GetBsnlBillDetail, strRequest:" + strRequest + ",strResp:" + strResp);
        //        htHeaders = null;
        //    }
        //    return resMsg;
        //}

        //private string GetBsnlLandlineBillDetail(string transId, string strMobileNo, string[] strArrData)
        //{
        //    String resMsg = string.Empty;
        //    String message = String.Empty;
        //    String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(number) || number.Trim().Length != 10)
        //        {
        //            TextLog.Exception("SMSAPI", "BSNLLL_EX", "Invalid number , tid:" + transId + ",number:" + number);
        //            message = clsGeneral.GetConfigVal("INVALID_NUMBER");
        //        }
        //        else
        //        {
        //            AddHeaders("bsnl_landline_viewbill", ref htHeaders);
        //            strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + strArrData[1] + "\",\"Channel\":\"SMS\"}";
        //            strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
        //            if (!string.IsNullOrEmpty(strResp))
        //            {
        //                JObject Json = JObject.Parse(strResp);
        //                message = clsGeneral.GetConfigVal("BSNL_LL_SMS_MSG");
        //                if (Json.GetValue("resCode").ToString() == "000")
        //                {
        //                    message = message.Replace("!CONSUMERNAME!", Json.GetValue("consumerName") != null ? Json.GetValue("consumerName").ToString() : string.Empty);
        //                    message = message.Replace("!AMOUNT!", Json.GetValue("billAmount") != null ? Json.GetValue("billAmount").ToString() : string.Empty);
        //                    message = message.Replace("!ACOUNTNO!", Json.GetValue("accountNo") != null ? Json.GetValue("accountNo").ToString() : string.Empty);
        //                    message = message.Replace("!NUMBER!", number);
        //                }
        //                else
        //                    message = Json.GetValue("resDesc").ToString();
        //            }
        //        }
        //        resMsg = message;
        //    }
        //    catch (Exception ex)
        //    {
        //        TextLog.Exception(strWorkSpaceId, "BSNLLL_EX", string.Format("ProcessBillDetailReq- GetBsnlLandlineBillDetail- Ex:{0}", ex.Message));
        //    }
        //    finally
        //    {
        //        TextLog.Debug(strWorkSpaceId, "BSNLLL", "GetBsnlLandlineBillDetail, strRequest:" + strRequest + ",strResp:" + strResp);
        //        htHeaders = null;
        //    }
        //    return resMsg;
        //}

        //private string GetTataBillDetail(string transId, string strMobileNo, string[] strArrData)
        //{
        //    String resMsg = string.Empty;
        //    String message = String.Empty;
        //    String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(number) || number.Trim().Length != 10)
        //        {
        //            TextLog.Exception(strWorkSpaceId, "TTL_EX", "Invalid number , transId:" + transId + ",number:" + number);
        //            message = clsGeneral.GetConfigVal("INVALID_NUMBER");
        //        }
        //        else
        //        {
        //            AddHeaders("ttl_viewbill", ref htHeaders);
        //            strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + number + "\",\"Channel\":\"SMS\"}";
        //            strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
        //            if (!string.IsNullOrEmpty(strResp))
        //            {
        //                JObject Json = JObject.Parse(strResp);
        //                message = clsGeneral.GetConfigVal("TTL_SMS_MSG");
        //                if (Json.GetValue("resCode").ToString() == "000")
        //                {
        //                    message = message.Replace("!CONSUMERNAME!", Json.GetValue("consumerName") != null ? Json.GetValue("consumerName").ToString() : string.Empty);
        //                    message = message.Replace("!AMOUNT!", Json.GetValue("billAmount") != null ? Json.GetValue("billAmount").ToString() : string.Empty);
        //                    message = message.Replace("!ACOUNTNO!", Json.GetValue("accountNo") != null ? Json.GetValue("accountNo").ToString() : string.Empty);
        //                    message = message.Replace("!NUMBER!", Json.GetValue("mobileNo") != null ? Json.GetValue("mobileNo").ToString() : string.Empty);
        //                }
        //                else
        //                {
        //                    message = Json.GetValue("resDesc").ToString();
        //                }
        //            }
        //        }
        //        resMsg = message;
        //    }
        //    catch (Exception ex)
        //    {
        //        TextLog.Exception(strWorkSpaceId, "TTL_EX", string.Format("ProcessBillDetailReq- GetTataBillDetail- Ex:{0}", ex.Message));
        //    }
        //    finally
        //    {
        //        TextLog.Debug(strWorkSpaceId, "TTL", "GetTataBillDetail, strRequest:" + strRequest + ",strResp:" + strResp);
        //        htHeaders = null;
        //    }
        //    return resMsg;
        //}
        #endregion
        private string GetWaterBillDetail(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String message = String.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
            try
            {
                if (string.IsNullOrEmpty(number) || (number.Trim().Length < clsGeneral.GetConfigVal("HMWSSB_CANNO_MIN_LENGTH").ToInt()) || (number.Trim().Length > clsGeneral.GetConfigVal("HMWSSB_CANNO_MAX_LENGTH").ToInt()))
                {
                    TextLog.Exception(strWorkSpaceId, "HMWSSB_EX", "Invalid Can length , transId:" + transId + ",strCANNo:" + number);
                    message = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    AddHeaders("hmwssb_viewbill", ref htHeaders);
                    strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + number + "\",\"Channel\":\"SMS\"}";
                    strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                    if (!string.IsNullOrEmpty(strResp))
                    {
                        message = clsGeneral.GetConfigVal("HMWSSB_SMS_MSG");
                        JObject Json = JObject.Parse(strResp);
                        if (Json.GetValue("resCode").ToString() == "000")
                        {
                            //message = message.Replace("!CONSUMERNAME!", Json.GetValue("name") != null ? Json.GetValue("name").ToString() : string.Empty);
                            message = message.Replace("!AMOUNT!", Json.GetValue("amount") != null ? Json.GetValue("amount").ToString() : string.Empty);
                            // message = message.Replace("!ADDRESS!", Json.GetValue("address") != null ? Json.GetValue("address").ToString() : string.Empty);
                            //message = message.Replace("!MOBILENO!", Json.GetValue("mobileNo") != null ? Json.GetValue("mobileNo").ToString() : string.Empty);
                            //message = message.Replace("!EMAIL!", Json.GetValue("email") != null ? Json.GetValue("email").ToString() : string.Empty);
                            message = message.Replace("!CANNUMBER!", number);
                        }
                        else
                        {
                            message = Json.GetValue("resDesc").ToString();
                        }
                    }
                }
                resMsg = message;
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "HMWSSB_EX", string.Format("ProcessBillDetailReq- GetWaterBillDetail- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "HMWSSB", "GetWaterBillDetail, strRequest:" + strRequest + ",strResp:" + strResp);
                htHeaders = null;
            }
            return resMsg;
        }

        private string GetACTBillDetail(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String message = String.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;

            try
            {
                if (string.IsNullOrEmpty(number))
                {
                    TextLog.Exception(strWorkSpaceId, "ACT_EX", "Invalid Number , transId:" + transId + ",number:" + number);
                    resMsg = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    AddHeaders("act_viewbill", ref htHeaders);
                    strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + number + "\",\"Channel\":\"SMS\"}";
                    strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                    if (!string.IsNullOrEmpty(strResp))
                    {
                        message = clsGeneral.GetConfigVal("ACT_SMS_MSG");
                        JObject Json = JObject.Parse(strResp);
                        if (Json.GetValue("resCode").ToString() == "000")
                        {
                            // message = message.Replace("!CONSUMERNAME!", Json.GetValue("customerName") != null ? Json.GetValue("customerName").ToString() : string.Empty);
                            message = message.Replace("!AMOUNT!", Json.GetValue("billAmount") != null ? Json.GetValue("billAmount").ToString() : string.Empty);
                            // message = message.Replace("!ACOUNTNO!", Json.GetValue("accountNo") != null ? Json.GetValue("accountNo").ToString() : string.Empty);
                            // message = message.Replace("!ADDRESS1!", Json.GetValue("address1") != null ? Json.GetValue("address1").ToString() : string.Empty);
                            //message = message.Replace("!ADDRESS2!", Json.GetValue("address2") != null ? Json.GetValue("address2").ToString() : string.Empty);
                            // message = message.Replace("!ADDRESS3!", Json.GetValue("address3") != null ? Json.GetValue("address3").ToString() : string.Empty);
                            //message = message.Replace("!NUMBER!", number);
                        }
                        else
                        {
                            message = Json.GetValue("resDesc").ToString();
                        }
                    }
                }
                resMsg = message;
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "ACT_EX", string.Format("ProcessBillDetailReq- GetACTBillDetail- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "ACT", "GetACTBillDetail, strRequest:" + strRequest + ",strResp:" + strResp);
                htHeaders = null;
            }
            return resMsg;
        }

        private string GetRTAFeeDetail(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String message = String.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;

            try
            {
                if (string.IsNullOrEmpty(number))
                {
                    TextLog.Exception(strWorkSpaceId, "RTA_EX", "Invlaid number , transId:" + transId + ",number:" + number);
                    message = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    AddHeaders("rta_viewbill", ref htHeaders);
                    strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + number + "\",\"Channel\":\"SMS\"}";
                    strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                    if (!string.IsNullOrEmpty(strResp))
                    {
                        message = clsGeneral.GetConfigVal("RTAPAY_SMS_MSG");
                        JObject Json = JObject.Parse(strResp);
                        if (Json.GetValue("resCode").ToString() == "000")
                        {
                            message = message.Replace("!NAME!", Json.GetValue("name") != null ? Json.GetValue("name").ToString() : string.Empty);
                            message = message.Replace("!TOTALAMOUNT!", Json.GetValue("totAmt") != null ? Json.GetValue("totAmt").ToString() : string.Empty);
                            message = message.Replace("!DEPTTRANSNO!", Json.GetValue("deptTransId") != null ? Json.GetValue("deptTransId").ToString() : string.Empty);
                            message = message.Replace("!SLOTDATE!", Json.GetValue("slotDate") != null ? Json.GetValue("slotDate").ToString() : string.Empty);
                            message = message.Replace("!SLOTTIME!", Json.GetValue("slotTime") != null ? Json.GetValue("slotTime").ToString() : string.Empty);
                            // message = message.Replace("!NUMBER!", number);
                        }
                        else
                        {
                            message = Json.GetValue("resDesc").ToString();
                        }
                    }
                }
                resMsg = message;
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "RTA_EX", string.Format("ProcessBillDetailReq- GetRTAFeeDetail- Ex:{0}", ex.Message));
            }
            finally
            {
                htHeaders = null;
                TextLog.Debug(strWorkSpaceId, "RTA", "GetRTAFeeDetail, strRequest:" + strRequest + ",strResp:" + strResp);
            }
            return resMsg;
        }

        private string GetRTALifeTaxBillDetail(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String message = String.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
            try
            {
                if (string.IsNullOrEmpty(number))
                {
                    TextLog.Exception("SMSAPI", "RTA_LifeTax", "Invalid number , tid:" + transId + ",number:" + number);
                    message = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    AddHeaders("rta_lifetax_viewbill", ref htHeaders);
                    strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + strArrData[1] + "\",\"Channel\":\"SMS\"}";
                    strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                    if (!string.IsNullOrEmpty(strResp))
                    {
                        JObject Json = JObject.Parse(strResp);
                        message = clsGeneral.GetConfigVal("RTALT_SMS_MSG");
                        if (Json.GetValue("resCode").ToString() == "000")
                        {
                            message = message.Replace("!NAME!", Json.GetValue("custName") != null ? Json.GetValue("custName").ToString() : string.Empty);
                            message = message.Replace("!INVDATE!", Json.GetValue("invoceDt") != null ? Json.GetValue("invoceDt").ToString() : string.Empty);
                            //message = message.Replace("!TRANSDATE!", Json.GetValue("tranDate") != null ? Json.GetValue("tranDate").ToString() : string.Empty);
                            //message = message.Replace("!CHASISNO!", Json.GetValue("chasisNo") != null ? Json.GetValue("chasisNo").ToString() : string.Empty);
                            //message = message.Replace("!REGAT!", Json.GetValue("regAt") != null ? Json.GetValue("regAt").ToString() : string.Empty);
                            //message = message.Replace("!TAXAMOUNT!", Json.GetValue("taxAmt") != null ? Json.GetValue("taxAmt").ToString() : string.Empty);
                            //message = message.Replace("!HPAFEE!", Json.GetValue("hpaFee") != null ? Json.GetValue("hpaFee").ToString() : string.Empty);
                            //message = message.Replace("!TRFEE!", Json.GetValue("trFee") != null ? Json.GetValue("trFee").ToString() : string.Empty);
                            //message = message.Replace("!SERVICEFEE!", Json.GetValue("serviceFee") != null ? Json.GetValue("serviceFee").ToString() : string.Empty);
                            //message = message.Replace("!REGFEE!", Json.GetValue("regFee") != null ? Json.GetValue("regFee").ToString() : string.Empty);
                            //message = message.Replace("!POSTALFEE!", Json.GetValue("postalFee") != null ? Json.GetValue("postalFee").ToString() : string.Empty);
                            message = message.Replace("!TOTALAMOUNT!", Json.GetValue("totAmt") != null ? Json.GetValue("totAmt").ToString() : string.Empty);
                        }
                        else
                            message = Json.GetValue("resDesc").ToString();
                    }
                }
                resMsg = message;
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "RTA_LifeTax_EX", string.Format("ProcessBillDetailReq- GetRTALifeTaxBillDetail- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "RTA_LifeTax", "GetRTALifeTaxBillDetail, strRequest:" + strRequest + ",strResp:" + strResp);
                htHeaders = null;
            }
            return resMsg;
        }

        //private string GetRTALLFeeDetail(string transId, string strMobileNo, string[] strArrData)
        //{
        //    String resMsg = string.Empty;
        //    String message = String.Empty;
        //    String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;

        //    try
        //    {
        //        if (string.IsNullOrEmpty(number))
        //        {
        //            TextLog.Exception(strWorkSpaceId, "RTALL_EX", "Invalid number , transId:" + transId + ",number:" + number);
        //            message = clsGeneral.GetConfigVal("INVALID_NUMBER");
        //        }
        //        else
        //        {
        //            AddHeaders("rta_lltest_viewbill", ref htHeaders);
        //            strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"llrno\":\"" + number + "\",\"Channel\":\"SMS\"}";
        //            strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
        //            if (!string.IsNullOrEmpty(strResp))
        //            {
        //                message = clsGeneral.GetConfigVal("RTALL_SMS_MSG");
        //                JObject Json = JObject.Parse(strResp);
        //                if (Json.GetValue("resCode").ToString() == "000")
        //                {
        //                    message = message.Replace("!NAME!", Json.GetValue("appName") != null ? Json.GetValue("appName").ToString() : string.Empty);
        //                    message = message.Replace("!SLOTTIME!", Json.GetValue("stTime") != null ? Json.GetValue("stTime").ToString() : string.Empty);
        //                    message = message.Replace("!SLOTDATE!", Json.GetValue("sbDate") != null ? Json.GetValue("sbDate").ToString() : string.Empty);
        //                    message = message.Replace("!TOTALAMOUNT!", Json.GetValue("testTotalAmount") != null ? Json.GetValue("testTotalAmount").ToString() : string.Empty);
        //                }
        //                else
        //                {
        //                    message = Json.GetValue("resDesc").ToString();
        //                }
        //            }
        //        }
        //        resMsg = message;
        //    }
        //    catch (Exception ex)
        //    {
        //        TextLog.Exception(strWorkSpaceId, "RTA_EX", string.Format("ProcessBillDetailReq- GetRTALLFeeDetail- Ex:{0}", ex.Message));
        //    }
        //    finally
        //    {
        //        htHeaders = null;
        //        TextLog.Debug(strWorkSpaceId, "RTA", "GetRTALLFeeDetail, strRequest:" + strRequest + ",strResp:" + strResp);
        //    }
        //    return resMsg;
        //}

        private string GetMeesevaApplicationStatus(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String message = String.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
            try
            {
                if (string.IsNullOrEmpty(number))
                {
                    TextLog.Exception(strWorkSpaceId, "APPLICATIONSTATUS_EX", "Invalid application no , transId:" + transId + ",strapplication no:" + number);
                    message = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    AddHeaders("application_status", ref htHeaders);
                    strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"applicationNo\":\"" + number.ToUpper() + "\",\"Channel\":\"SMS\"}";
                    strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                    if (!string.IsNullOrEmpty(strResp))
                    {
                        message = clsGeneral.GetConfigVal("APPLICATIONSTATUS_SMS_MSG");
                        JObject Json = JObject.Parse(strResp);
                        if (Json.GetValue("resCode").ToString() == "000")
                        {
                            message = message.Replace("!APPSTATUS!", Json.GetValue("status") != null ? Json.GetValue("status").ToString() : string.Empty);
                        }
                        else
                        {
                            message = message.Replace("!APPSTATUS!", Json.GetValue("resDesc") != null ? Json.GetValue("resDesc").ToString() : string.Empty);
                        }
                    }
                }
                resMsg = message;
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "APPLICATIONSTATUS_EX", string.Format("ProcessBillDetailReq- GetMeesevaApplicationStatus- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "APPLICATIONSTATUS", "GetMeesevaApplicationStatus, strRequest:" + strRequest + ",strResp:" + strResp);
                htHeaders = null;
            }
            return resMsg;
        }

        private string GetHATHWAYBillDetail(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String message = String.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;

            try
            {
                if (string.IsNullOrEmpty(number))
                {
                    TextLog.Exception(strWorkSpaceId, "HATHWAY_EX", "Invalid Number , transId:" + transId + ",number:" + number);
                    resMsg = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    AddHeaders("hathway_viewbill", ref htHeaders);
                    strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"accountno\":\"" + number + "\",\"Channel\":\"SMS\"}";
                    strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                    if (!string.IsNullOrEmpty(strResp))
                    {
                        message = clsGeneral.GetConfigVal("HATHWAY_SMS_MSG");
                        JObject Json = JObject.Parse(strResp);
                        if (Json.GetValue("resCode").ToString() == "000")
                        {
                            // message = message.Replace("!CONSUMERNAME!", Json.GetValue("customerName") != null ? Json.GetValue("customerName").ToString() : string.Empty);
                            message = message.Replace("!AMOUNT!", Json.GetValue("billamount") != null ? Json.GetValue("billamount").ToString() : string.Empty);
                            // message = message.Replace("!ACOUNTNO!", Json.GetValue("accountNo") != null ? Json.GetValue("accountNo").ToString() : string.Empty);
                            // message = message.Replace("!ADDRESS1!", Json.GetValue("address1") != null ? Json.GetValue("address1").ToString() : string.Empty);
                            //message = message.Replace("!ADDRESS2!", Json.GetValue("address2") != null ? Json.GetValue("address2").ToString() : string.Empty);
                            // message = message.Replace("!ADDRESS3!", Json.GetValue("address3") != null ? Json.GetValue("address3").ToString() : string.Empty);
                            //message = message.Replace("!NUMBER!", number);
                        }
                        else
                        {
                            message = Json.GetValue("resDesc").ToString();
                        }
                    }
                }
                resMsg = message;
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "HATHWAY_EX", string.Format("ProcessBillDetailReq- GetHATHWAYBillDetail- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "HATHWAY", "GetHATHWAYBillDetail, strRequest:" + strRequest + ",strResp:" + strResp);
                htHeaders = null;
            }
            return resMsg;
        }
        #endregion

        #region Pay Bill Methods
        private string AirtelBillPayment(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
            try
            {
                if (string.IsNullOrEmpty(number) || number.Trim().Length != 10)
                {
                    TextLog.Exception("SMSAPI", "AIRTEL_EX", "Invalid number , tid:" + transId + ",number:" + number);
                    resMsg = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    TWALLETBO objIMPS = PayThroughTWALLET(transId, strMobileNo, strArrData);
                    if (objIMPS != null && objIMPS.RespCode == "0")
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_SUCCESS_MSG").Replace("!AMOUNT!", objIMPS.TWalletAmount).Replace("!SERVICE!", "Airtel postpaid");
                    else if (objIMPS != null && !string.IsNullOrEmpty(objIMPS.RespDesc))
                        resMsg = objIMPS.RespDesc;
                    else
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
                }
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "AIRTEL_EX", string.Format("ProcessBillDetailReq- AirtelBillPayment- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "AIRTEL", "AirtelBillPayment, strRequest:" + strRequest + ",strResp:" + strResp);
            }
            return resMsg;
        }

        private string AirtelLandlineBillPayment(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
            try
            {
                if (string.IsNullOrEmpty(number) || number.Trim().Length != 10)
                {
                    TextLog.Exception("SMSAPI", "AIRTELLL_EX", "Invalid number , tid:" + transId + ",number:" + number);
                    resMsg = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    TWALLETBO objIMPS = PayThroughTWALLET(transId, strMobileNo, strArrData);
                    if (objIMPS != null && objIMPS.RespCode == "0")
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_SUCCESS_MSG").Replace("!AMOUNT!", objIMPS.TWalletAmount).Replace("!SERVICE!", "Airtel landline");
                    else if (objIMPS != null && !string.IsNullOrEmpty(objIMPS.RespDesc))
                        resMsg = objIMPS.RespDesc;
                    else
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
                }
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "AIRTELLL_EX", string.Format("ProcessBillDetailReq- AirtelLandlineBillPayment- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "AIRTEL", "AirtelLandlineBillPayment, strRequest:" + strRequest + ",strResp:" + strResp);
            }
            return resMsg;
        }

        private string HMWSSBBillPayment(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
            try
            {
                if (string.IsNullOrEmpty(number) || (number.Trim().Length < clsGeneral.GetConfigVal("HMWSSB_CANNO_MIN_LENGTH").ToInt()) || (number.Trim().Length > clsGeneral.GetConfigVal("HMWSSB_CANNO_MAX_LENGTH").ToInt()))
                {
                    TextLog.Exception(strWorkSpaceId, "HMWSSB_EX", "Invalid Can length , transId:" + transId + ",strCANNo:" + number);
                    resMsg = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    TWALLETBO objIMPS = PayThroughTWALLET(transId, strMobileNo, strArrData);
                    if (objIMPS != null && objIMPS.RespCode == "0")
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_SUCCESS_MSG").Replace("!AMOUNT!", objIMPS.TWalletAmount).Replace("!SERVICE!", "HMWSSB");
                    else if (objIMPS != null && !string.IsNullOrEmpty(objIMPS.RespDesc))
                        resMsg = objIMPS.RespDesc;
                    else
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
                }
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "HMWSSB_EX", string.Format("ProcessBillDetailReq- HMWSSBBillPayment- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "HMWSSB", "HMWSSBBillPayment, strRequest:" + strRequest + ",strResp:" + strResp);
            }
            return resMsg;
        }

        private string IDEABillPayment(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
            try
            {
                if (string.IsNullOrEmpty(number) || number.Trim().Length != 10)
                {
                    TextLog.Exception(strWorkSpaceId, "IDEA_EX", "Invalid number , transId:" + transId + ",number:" + number);
                    resMsg = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    TWALLETBO objIMPS = PayThroughTWALLET(transId, strMobileNo, strArrData);
                    if (objIMPS != null && objIMPS.RespCode == "0")
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_SUCCESS_MSG").Replace("!AMOUNT!", objIMPS.TWalletAmount).Replace("!SERVICE!", "Idea postpaid");
                    else if (objIMPS != null && !string.IsNullOrEmpty(objIMPS.RespDesc))
                        resMsg = objIMPS.RespDesc;
                    else
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
                }
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "IDEA_EX", string.Format("ProcessBillDetailReq- IDEABillPayment- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "IDEA", "IDEABillPayment, strRequest:" + strRequest + ",strResp:" + strResp);
            }
            return resMsg;
        }

        private string ACTBillPayment(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
            try
            {
                if (string.IsNullOrEmpty(number) || number.Trim().Length != 10)
                {
                    TextLog.Exception(strWorkSpaceId, "ACT_EX", "Invalid Number , transId:" + transId + ",number:" + number);
                    resMsg = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    TWALLETBO objIMPS = PayThroughTWALLET(transId, strMobileNo, strArrData);
                    if (objIMPS != null && objIMPS.RespCode == "0")
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_SUCCESS_MSG").Replace("!AMOUNT!", objIMPS.TWalletAmount).Replace("!SERVICE!", "ACT");
                    else if (objIMPS != null && !string.IsNullOrEmpty(objIMPS.RespDesc))
                        resMsg = objIMPS.RespDesc;
                    else
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
                }
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "ACT_EX", string.Format("ProcessBillDetailReq- ACTBillPayment- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "ACT", "ACTBillPayment, strRequest:" + strRequest + ",strResp:" + strResp);
            }
            return resMsg;
        }

        private string RTABillPayment(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;

            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
            try
            {
                if (string.IsNullOrEmpty(number))
                {
                    TextLog.Exception(strWorkSpaceId, "RTA_EX", "Invlaid number , transId:" + transId + ",number:" + number);
                    resMsg = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    TWALLETBO objIMPS = PayThroughTWALLET(transId, strMobileNo, strArrData);
                    if (objIMPS != null && objIMPS.RespCode == "0")
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_SUCCESS_MSG").Replace("!AMOUNT!", objIMPS.TWalletAmount).Replace("!SERVICE!", "RTA");
                    else if (objIMPS != null && !string.IsNullOrEmpty(objIMPS.RespDesc))
                        resMsg = objIMPS.RespDesc;
                    else
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
                }
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "RTA_EX", string.Format("ProcessBillDetailReq- RTABillPayment- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "RTA", "RTABillPayment, strRequest:" + strRequest + ",strResp:" + strResp);
            }
            return resMsg;
        }

        private string TTLBillPayment(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
            try
            {
                if (string.IsNullOrEmpty(number) || number.Trim().Length != 10)
                {
                    TextLog.Exception(strWorkSpaceId, "TTL_EX", "Invalid number , transId:" + transId + ",number:" + number);
                    resMsg = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    TWALLETBO objIMPS = PayThroughTWALLET(transId, strMobileNo, strArrData);
                    if (objIMPS != null && objIMPS.RespCode == "0")
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_SUCCESS_MSG").Replace("!AMOUNT!", objIMPS.TWalletAmount).Replace("!SERVICE!", "Tata postpaid");
                    else if (objIMPS != null && !string.IsNullOrEmpty(objIMPS.RespDesc))
                        resMsg = objIMPS.RespDesc;
                    else
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
                }
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "TTL_EX", string.Format("ProcessBillDetailReq- TTLBillPayment- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "TTL", "TTLBillPayment, strRequest:" + strRequest + ",strResp:" + strResp);
            }
            return resMsg;
        }

        private string TTLLandlineBillPayment(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
            try
            {
                if (string.IsNullOrEmpty(number) || number.Trim().Length != 10)
                {
                    TextLog.Exception(strWorkSpaceId, "TTLLANDLINE_EX", "Invalid number , transId:" + transId + ",number:" + number);
                    resMsg = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    TWALLETBO objIMPS = PayThroughTWALLET(transId, strMobileNo, strArrData);
                    if (objIMPS != null && objIMPS.RespCode == "0")
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_SUCCESS_MSG").Replace("!AMOUNT!", objIMPS.TWalletAmount).Replace("!SERVICE!", "Tata postpaid");
                    else if (objIMPS != null && !string.IsNullOrEmpty(objIMPS.RespDesc))
                        resMsg = objIMPS.RespDesc;
                    else
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
                }
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "TTL_EX", string.Format("ProcessBillDetailReq- TTLLandlineBillPayment- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "TTL", "TTLLandlineBillPayment, strRequest:" + strRequest + ",strResp:" + strResp);
            }
            return resMsg;
        }

        private string BsnlBillPayment(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
            try
            {
                if (string.IsNullOrEmpty(number) || number.Trim().Length != 10)
                {
                    TextLog.Exception("SMSAPI", "BSNL_EX", "Invalid number , tid:" + transId + ",number:" + number);
                    resMsg = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    TWALLETBO objIMPS = PayThroughTWALLET(transId, strMobileNo, strArrData);
                    if (objIMPS != null && objIMPS.RespCode == "0")
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_SUCCESS_MSG").Replace("!AMOUNT!", objIMPS.TWalletAmount).Replace("!SERVICE!", "BSNL postpaid");
                    else if (objIMPS != null && !string.IsNullOrEmpty(objIMPS.RespDesc))
                        resMsg = objIMPS.RespDesc;
                    else
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
                }
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "BSNL_EX", string.Format("ProcessBillDetailReq- BsnlBillPayment- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "BSNL", "BsnlBillPayment, strRequest:" + strRequest + ",strResp:" + strResp);
            }
            return resMsg;
        }

        private string BsnlLandlineBillPayment(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
            try
            {
                if (string.IsNullOrEmpty(number) || number.Trim().Length != 10)
                {
                    TextLog.Exception("SMSAPI", "BSNLLL_EX", "Invalid number , tid:" + transId + ",number:" + number);
                    resMsg = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    TWALLETBO objIMPS = PayThroughTWALLET(transId, strMobileNo, strArrData);
                    if (objIMPS != null && objIMPS.RespCode == "0")
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_SUCCESS_MSG").Replace("!AMOUNT!", objIMPS.TWalletAmount).Replace("!SERVICE!", "BSNL landline");
                    else if (objIMPS != null && !string.IsNullOrEmpty(objIMPS.RespDesc))
                        resMsg = objIMPS.RespDesc;
                    else
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
                }
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "BSNLLL_EX", string.Format("ProcessBillDetailReq- BsnlLandlineBillPayment- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "BSNLLL", "BsnlLandlineBillPayment, strRequest:" + strRequest + ",strResp:" + strResp);
            }
            return resMsg;
        }

        private string RTALifeTaxBillPayment(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
            try
            {
                if (string.IsNullOrEmpty(number))
                {
                    TextLog.Exception("SMSAPI", "RTALifeTax_EX", "Invalid number , tid:" + transId + ",number:" + number);
                    resMsg = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    TWALLETBO objIMPS = PayThroughTWALLET(transId, strMobileNo, strArrData);
                    if (objIMPS != null && objIMPS.RespCode == "0")
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_SUCCESS_MSG").Replace("!AMOUNT!", objIMPS.TWalletAmount).Replace("!SERVICE!", "RTA LIFETAX");
                    else if (objIMPS != null && !string.IsNullOrEmpty(objIMPS.RespDesc))
                        resMsg = objIMPS.RespDesc;
                    else
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
                }
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "RTALifeTax_EX", string.Format("ProcessBillDetailReq- RTALifeTaxBillPayment- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "RTALifeTax", "RTALifeTaxBillPayment, strRequest:" + strRequest + ",strResp:" + strResp);
            }
            return resMsg;
        }

        private string VodafoneBillPayment(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
            try
            {
                if (string.IsNullOrEmpty(number) || number.Trim().Length != 10)
                {
                    TextLog.Exception(strWorkSpaceId, "VODAFONE_EX", "Invalid number , transId:" + transId + ",number:" + number);
                    resMsg = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    TWALLETBO objIMPS = PayThroughTWALLET(transId, strMobileNo, strArrData);
                    if (objIMPS != null && objIMPS.RespCode == "0")
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_SUCCESS_MSG").Replace("!AMOUNT!", objIMPS.TWalletAmount).Replace("!SERVICE!", "Vodafone postpaid");
                    else if (objIMPS != null && !string.IsNullOrEmpty(objIMPS.RespDesc))
                        resMsg = objIMPS.RespDesc;
                    else
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
                }
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "VODAFONE_EX", string.Format("ProcessBillDetailReq- VodafoneBillPayment- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "VODAFONE", "VodafoneBillPayment, strRequest:" + strRequest + ",strResp:" + strResp);
            }
            return resMsg;
        }

        private string RelianceBillPayment(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
            try
            {
                if (string.IsNullOrEmpty(number) || number.Trim().Length != 10)
                {
                    TextLog.Exception(strWorkSpaceId, "RELIANCE_EX", "Invalid number , transId:" + transId + ",number:" + number);
                    resMsg = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    TWALLETBO objIMPS = PayThroughTWALLET(transId, strMobileNo, strArrData);
                    if (objIMPS != null && objIMPS.RespCode == "0")
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_SUCCESS_MSG").Replace("!AMOUNT!", objIMPS.TWalletAmount).Replace("!SERVICE!", "Reliance postpaid");
                    else if (objIMPS != null && !string.IsNullOrEmpty(objIMPS.RespDesc))
                        resMsg = objIMPS.RespDesc;
                    else
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
                }
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "RELIANCE_EX", string.Format("ProcessBillDetailReq- RelianceBillPayment- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "RELIANCE_EX", "RelianceBillPayment, strRequest:" + strRequest + ",strResp:" + strResp);
            }
            return resMsg;
        }

        private string HATHWAYBillPayment(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
            try
            {
                if (string.IsNullOrEmpty(number) || number.Trim().Length != 10)
                {
                    TextLog.Exception(strWorkSpaceId, "HATHWAY_EX", "Invalid Number , transId:" + transId + ",number:" + number);
                    resMsg = clsGeneral.GetConfigVal("INVALID_NUMBER");
                }
                else
                {
                    TWALLETBO objIMPS = PayThroughTWALLET(transId, strMobileNo, strArrData);
                    if (objIMPS != null && objIMPS.RespCode == "0")
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_SUCCESS_MSG").Replace("!AMOUNT!", objIMPS.TWalletAmount).Replace("!SERVICE!", "HATHWAY");
                    else if (objIMPS != null && !string.IsNullOrEmpty(objIMPS.RespDesc))
                        resMsg = objIMPS.RespDesc;
                    else
                        resMsg = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
                }
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "HATHWAY_EX", string.Format("ProcessBillDetailReq- HATHWAYBillPayment- Ex:{0}", ex.Message));
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "HATHWAY", "HATHWAYBillPayment, strRequest:" + strRequest + ",strResp:" + strResp);
            }
            return resMsg;
        }

        //private string RTALLTestFeePayment(string transId, string strMobileNo, string[] strArrData)
        //{
        //    String resMsg = string.Empty;
        //    String number = strArrData.Length > 1 ? strArrData[1] : string.Empty;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(number))
        //        {
        //            TextLog.Exception("SMSAPI", "RTALLTestFee_EX", "Invalid number , tid:" + transId + ",number:" + number);
        //            resMsg = clsGeneral.GetConfigVal("INVALID_NUMBER");
        //        }
        //        else
        //        {
        //            TWALLETBO objIMPS = PayThroughTWALLET(transId, strMobileNo, strArrData);
        //            if (objIMPS != null && objIMPS.RespCode == "0")
        //                resMsg = clsGeneral.GetConfigVal("PAYMENT_SUCCESS_MSG").Replace("!AMOUNT!", objIMPS.TWalletAmount).Replace("!SERVICE!", "BSNL landline");
        //            else if (objIMPS != null && !string.IsNullOrEmpty(objIMPS.RespDesc))
        //                resMsg = objIMPS.RespDesc;
        //            else
        //                resMsg = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TextLog.Exception(strWorkSpaceId, "BSNLLL_EX", string.Format("ProcessBillDetailReq- BsnlLandlineBillPayment- Ex:{0}", ex.Message));
        //    }
        //    finally
        //    {
        //        TextLog.Debug(strWorkSpaceId, "BSNLLL", "BsnlLandlineBillPayment, strRequest:" + strRequest + ",strResp:" + strResp);
        //    }
        //    return resMsg;
        //}
        #endregion

        private string GenerateTwalletOTP(string transId, string strMobileNo, string[] strArrData)
        {
            String resMsg = string.Empty;
            String message = String.Empty;
            String strOTPResp = String.Empty;
            String strParams = String.Empty;
            if (strArrData.Length != 3)
            {
                resMsg = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
            }
            else if (strArrData[1].ToUpper().Trim() != "OTP")
            {
                resMsg = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
            }
            else
            {
                String TwalletMobNo = strArrData.Length > 2 ? strArrData[2] : string.Empty;
                try
                {
                    if (string.IsNullOrEmpty(TwalletMobNo) || TwalletMobNo.Trim().Length != 10)
                    {
                        TextLog.Exception("SMSAPI", "GenerateTwalletOTP_EX", "Invalid Twallet mobile number , tid:" + transId + ",number:" + TwalletMobNo);
                        message = clsGeneral.GetConfigVal("INVALID_NUMBER");
                    }
                    else
                    {
                        strParams = clsGeneral.GetConfigVal("TWALLET_OTP_GENERATE_POSTDATA").Replace("!TWALLETMOBNO!", TwalletMobNo);
                        // strParams = strParams.Replace("!AMOUNT!", "");
                        strOTPResp = clsGeneral.DoPostRequest(clsGeneral.GetConfigVal("TWALLET_OTP_GENERATE_URL"), strParams);
                        if (!string.IsNullOrEmpty(strOTPResp) && (!strOTPResp.ToLower().Contains("transaction failed")))
                        {
                            JObject Json = JObject.Parse(strOTPResp);
                            if (Json.GetValue("Response_Code").ToString() == "1680")
                            {
                                message = clsGeneral.GetConfigVal("OTP_GENERATION_SUCCESS");
                            }
                            else
                                message = Json.GetValue("Message").ToString();
                        }
                        else
                            message = clsGeneral.GetConfigVal("OTP_GENERATION_FAIL");
                    }
                    resMsg = message;
                }
                catch (Exception ex)
                {
                    TextLog.Exception(strWorkSpaceId, "OTPGENERATION_EX", string.Format("GenerateTwalletOTP- GenerateTwalletOTP- Ex:{0}", ex.Message));
                }
                finally
                {
                    TextLog.Debug(strWorkSpaceId, "OTPGENERATION", "GenerateTwalletOTP, strRequest:" + strParams + ",strResp:" + strOTPResp);
                }
            }
            return resMsg;
        }

        public AckStatusBO GetAcknowledgementJSON(string transId, string strMobileNo, string[] strArrData, PaymentBO payBO)
        {
            AckStatusBO objAckStaus = new AckStatusBO();
            String strAckJson = String.Empty;
            Dictionary<String, String> dictionaryResp = new Dictionary<String, String>();
            Dictionary<string, string> dicAck = new Dictionary<string, string>();
            dicAck.AddKeyValue("tid", transId);
            dicAck.AddKeyValue("mobileNo", strMobileNo);
            dicAck.AddKeyValue("transcode", "!TRANSCODE!");
            dicAck.AddKeyValue("authcode", "!AUTHCODE!");
            dicAck.AddKeyValue("cardtype", "!CARDTYPE!");
            dicAck.AddKeyValue("imitransid", "!IMITRANSID!");
            dicAck.AddKeyValue("ptransid", "!PTRANSID!");
            dicAck.AddKeyValue("channel", "SMS");
            dicAck.AddKeyValue("email", "", true);
            dicAck.AddKeyValue("username", "", true);

            try
            {
                switch (strArrData[0].ToLower())
                {
                    case "hmwssb":
                        {
                            #region HMWSSB
                            try
                            {
                                AddHeaders("hmwssb_viewbill", ref htHeaders);
                                strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + strArrData[1] + "\",\"Channel\":\"SMS\"}";
                                strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                                if (!string.IsNullOrEmpty(strResp))
                                {
                                    dictionaryResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                                    if (dictionaryResp.GetValue("resCode") == "000")
                                    {
                                        dicAck.AddKeyValue("number", strArrData[1]);
                                        dicAck.AddKeyValue("network", "HMWSSB");
                                        dicAck.AddKeyValue("accountNo", dictionaryResp.GetValue("accountNo"));
                                        dicAck.AddKeyValue("amount", payBO.Amount.ToStr());
                                        dicAck.AddKeyValue("billRefNo", dictionaryResp.GetValue("reqId"));
                                        objAckStaus.ActualBillAmount = dictionaryResp.GetValue("amount");
                                        strAckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespCode = "0";
                                        objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespDesc = "success";
                                        objAckStaus.Service = "eseva";
                                        objAckStaus.Action = "8";
                                        objAckStaus.ActionDesc = "HMWSSB";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + strArrData[0].ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                objAckStaus.RespCode = "1";
                                objAckStaus.RespDesc = "exception";
                            }
                            #endregion
                        }
                        break;
                    case "airtel":
                        {
                            #region AIRTEL
                            try
                            {
                                AddHeaders("airtel_viewbill", ref htHeaders);
                                strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + strArrData[1] + "\",\"Channel\":\"SMS\"}";
                                strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                                if (!string.IsNullOrEmpty(strResp))
                                {
                                    dictionaryResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                                    if (dictionaryResp.GetValue("resCode") == "000")
                                    {
                                        dicAck.AddKeyValue("number", strArrData[1]);
                                        dicAck.AddKeyValue("network", "Airtel");
                                        dicAck.AddKeyValue("accountNo", dictionaryResp.GetValue("accountNo"));
                                        dicAck.AddKeyValue("amount", payBO.Amount.ToStr());
                                        dicAck.AddKeyValue("billRefNo", dictionaryResp.GetValue("reqId"));
                                        objAckStaus.ActualBillAmount = dictionaryResp.GetValue("billAmount");
                                        strAckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespCode = "0";
                                        objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespDesc = "success";
                                        objAckStaus.Service = "eseva";
                                        objAckStaus.Action = "2";
                                        objAckStaus.ActionDesc = "AIRTEL";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + strArrData[0].ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                objAckStaus.RespCode = "1";
                                objAckStaus.RespDesc = "exception";
                            }
                            #endregion
                        }
                        break;
                    case "airtell":
                        {
                            #region AIRTEL LAND LINE
                            try
                            {
                                AddHeaders("airtel_landline_viewbill", ref htHeaders);
                                strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + strArrData[1] + "\",\"Channel\":\"SMS\"}";
                                strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                                if (!string.IsNullOrEmpty(strResp))
                                {
                                    dictionaryResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                                    if (dictionaryResp.GetValue("resCode") == "000")
                                    {
                                        dicAck.AddKeyValue("number", strArrData[1]);
                                        dicAck.AddKeyValue("network", "AIRTEL_LANDLINE");
                                        dicAck.AddKeyValue("accountNo", dictionaryResp.GetValue("accountNo"));
                                        dicAck.AddKeyValue("amount", payBO.Amount.ToStr());
                                        dicAck.AddKeyValue("billRefNo", dictionaryResp.GetValue("reqId"));
                                        objAckStaus.ActualBillAmount = dictionaryResp.GetValue("billAmount");
                                        strAckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespCode = "0";
                                        objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespDesc = "success";
                                        objAckStaus.Service = "eseva";
                                        objAckStaus.Action = "4";
                                        objAckStaus.ActionDesc = "AIRTELLANDLINE";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + strArrData[0].ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                objAckStaus.RespCode = "1";
                                objAckStaus.RespDesc = "exception";
                            }
                            #endregion
                        }
                        break;
                    case "idea":
                        {
                            #region IDEA
                            try
                            {
                                AddHeaders("idea_viewbill", ref htHeaders);
                                strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + strArrData[1] + "\",\"Channel\":\"SMS\"}";
                                strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                                if (!string.IsNullOrEmpty(strResp))
                                {
                                    dictionaryResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                                    if (dictionaryResp.GetValue("resCode") == "000")
                                    {
                                        dicAck.AddKeyValue("number", strArrData[1]);
                                        dicAck.AddKeyValue("accountNo", dictionaryResp.GetValue("accountNo"));
                                        dicAck.AddKeyValue("amount", payBO.Amount.ToStr());
                                        dicAck.AddKeyValue("billRefNo", dictionaryResp.GetValue("reqId"));
                                        objAckStaus.ActualBillAmount = dictionaryResp.GetValue("billAmount");
                                        strAckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespCode = "0";
                                        objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespDesc = "success";
                                        objAckStaus.Service = "eseva";
                                        objAckStaus.Action = "6";
                                        objAckStaus.ActionDesc = "IDEA";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + strArrData[0].ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                objAckStaus.RespCode = "1";
                                objAckStaus.RespDesc = "exception";
                            }
                            #endregion
                        }
                        break;
                    case "act":
                        {
                            #region ACT
                            try
                            {
                                AddHeaders("act_viewbill", ref htHeaders);
                                strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + strArrData[1] + "\",\"Channel\":\"SMS\"}";
                                strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                                if (!string.IsNullOrEmpty(strResp))
                                {
                                    dictionaryResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                                    if (dictionaryResp.GetValue("resCode") == "000")
                                    {
                                        dicAck.AddKeyValue("number", strArrData[1]);
                                        dicAck.AddKeyValue("accountNo", dictionaryResp.GetValue("accountNo"));
                                        dicAck.AddKeyValue("amount", dictionaryResp.GetValue("billAmount"));
                                        dicAck.AddKeyValue("billRefNo", dictionaryResp.GetValue("reqId"));
                                        objAckStaus.ActualBillAmount = dictionaryResp.GetValue("billAmount");
                                        strAckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespCode = "0";
                                        objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespDesc = "success";
                                        objAckStaus.Service = "eseva";
                                        objAckStaus.Action = "10";
                                        objAckStaus.ActionDesc = "ACT";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + strArrData[0].ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                objAckStaus.RespCode = "1";
                                objAckStaus.RespDesc = "exception";
                            }
                            #endregion
                        }
                        break;
                    case "rtapay":
                        {
                            #region RTA
                            try
                            {
                                AddHeaders("rta_viewbill", ref htHeaders);
                                strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + strArrData[1] + "\",\"Channel\":\"SMS\"}";
                                strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                                if (!string.IsNullOrEmpty(strResp))
                                {
                                    dictionaryResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                                    if (dictionaryResp.GetValue("resCode") == "000")
                                    {
                                        dicAck.AddKeyValue("number", strArrData[1]);
                                        dicAck.AddKeyValue("accountNo", "", true);
                                        dicAck.AddKeyValue("amount", dictionaryResp.GetValue("totAmt"));
                                        dicAck.AddKeyValue("billRefNo", dictionaryResp.GetValue("reqId"));
                                        objAckStaus.ActualBillAmount = dictionaryResp.GetValue("totAmt");
                                        strAckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespCode = "0";
                                        objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespDesc = "success";
                                        objAckStaus.Service = "eseva";
                                        objAckStaus.Action = "12";
                                        objAckStaus.ActionDesc = "RTAFEE";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + strArrData[0].ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                objAckStaus.RespCode = "1";
                                objAckStaus.RespDesc = "exception";
                            }
                            #endregion
                        }
                        break;
                    case "ttll":
                    case "ttl":
                        {
                            #region TATA Postpaid & Landline
                            try
                            {
                                AddHeaders("ttl_viewbill", ref htHeaders);
                                strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + strArrData[1] + "\",\"Channel\":\"SMS\"}";
                                strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                                if (!string.IsNullOrEmpty(strResp))
                                {
                                    dictionaryResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                                    if (dictionaryResp.GetValue("resCode") == "000")
                                    {
                                        dicAck.AddKeyValue("number", strArrData[1]);
                                        dicAck.AddKeyValue("accountNo", dictionaryResp.GetValue("accountNo"));
                                        dicAck.AddKeyValue("amount", payBO.Amount.ToStr());
                                        dicAck.AddKeyValue("billRefNo", dictionaryResp.GetValue("reqId"));
                                        objAckStaus.ActualBillAmount = dictionaryResp.GetValue("billAmount");
                                        strAckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespCode = "0";
                                        objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespDesc = "success";
                                        objAckStaus.Service = "eseva";
                                        objAckStaus.Action = "14";
                                        objAckStaus.ActionDesc = "TTL";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + strArrData[0].ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                objAckStaus.RespCode = "1";
                                objAckStaus.RespDesc = "exception";
                            }
                            #endregion
                        }
                        break;
                    case "bsnll":
                    case "bsnlm":
                        {
                            #region BSNL Postpaid & Landline
                            try
                            {
                                AddHeaders("bsnl_viewbill", ref htHeaders);
                                strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + strArrData[1] + "\",\"Channel\":\"SMS\"}";
                                strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                                if (!string.IsNullOrEmpty(strResp))
                                {
                                    dictionaryResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                                    if (dictionaryResp.GetValue("resCode") == "000")
                                    {
                                        dicAck.AddKeyValue("number", strArrData[1]);
                                        dicAck.AddKeyValue("amount", payBO.Amount.ToStr());
                                        dicAck.AddKeyValue("billRefNo", dictionaryResp.GetValue("reqId"));
                                        objAckStaus.ActualBillAmount = dictionaryResp.GetValue("billAmount");
                                        strAckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespCode = "0";
                                        objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespDesc = "success";
                                        objAckStaus.Service = "eseva";
                                        objAckStaus.Action = "34";
                                        objAckStaus.ActionDesc = "BSNL";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + strArrData[0].ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                objAckStaus.RespCode = "1";
                                objAckStaus.RespDesc = "exception";
                            }
                            #endregion
                        }
                        break;
                    case "rtalt":
                        {
                            #region RTA LifeTax
                            try
                            {
                                AddHeaders("rta_lifetax_viewbill", ref htHeaders);
                                strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"number\":\"" + strArrData[1] + "\",\"Channel\":\"SMS\"}";
                                strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                                if (!string.IsNullOrEmpty(strResp))
                                {
                                    dictionaryResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                                    if (dictionaryResp.GetValue("resCode") == "000")
                                    {
                                        dicAck.AddKeyValue("number", strArrData[1]);
                                        dicAck.AddKeyValue("amount", dictionaryResp.GetValue("totAmt"));
                                        dicAck.AddKeyValue("billRefNo", dictionaryResp.GetValue("reqId"));
                                        objAckStaus.ActualBillAmount = dictionaryResp.GetValue("totAmt");
                                        strAckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespCode = "0";
                                        objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespDesc = "success";
                                        objAckStaus.Service = "eseva";
                                        objAckStaus.Action = "36";
                                        objAckStaus.ActionDesc = "RTALIFETAX";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + strArrData[0].ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                objAckStaus.RespCode = "1";
                                objAckStaus.RespDesc = "exception";
                            }
                            #endregion
                        }
                        break;
                    case "vodf":
                        {
                            #region VODAFONE POSTPAID BILL PAYMENT
                            try
                            {
                                dicAck.AddKeyValue("number", strArrData[1]);
                                dicAck.AddKeyValue("operator", "VO");
                                dicAck.AddKeyValue("amount", payBO.Amount.ToStr());
                                objAckStaus.ActualBillAmount = payBO.Amount.ToStr();
                                strAckJson = JsonConvert.SerializeObject(dicAck);
                                objAckStaus.RespCode = "0";
                                objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                objAckStaus.RespDesc = "success";
                                objAckStaus.Service = "martconnect";
                                objAckStaus.Action = "5";
                                objAckStaus.ActionDesc = "POSTPAID";
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + strArrData[0].ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                objAckStaus.RespCode = "1";
                                objAckStaus.RespDesc = "exception";
                            }
                            #endregion
                        }
                        break;
                    case "relcm":
                        {
                            #region RELIANCE POSTPAID BILL PAYMENT
                            try
                            {
                                dicAck.AddKeyValue("number", strArrData[1]);
                                dicAck.AddKeyValue("operator", "RG");
                                dicAck.AddKeyValue("amount", payBO.Amount.ToStr());
                                objAckStaus.ActualBillAmount = payBO.Amount.ToStr();
                                strAckJson = JsonConvert.SerializeObject(dicAck);
                                objAckStaus.RespCode = "0";
                                objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                objAckStaus.RespDesc = "success";
                                objAckStaus.Service = "martconnect";
                                objAckStaus.Action = "5";
                                objAckStaus.ActionDesc = "POSTPAID";
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + strArrData[0].ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                objAckStaus.RespCode = "1";
                                objAckStaus.RespDesc = "exception";
                            }
                            #endregion
                        }
                        break;
                    case "rtall":
                        {
                            #region RTA LL Test fee
                            try
                            {
                                AddHeaders("rta_lltest_viewbill", ref htHeaders);
                                strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"llrno\":\"" + strArrData[1] + "\",\"Channel\":\"SMS\"}";
                                strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                                if (!string.IsNullOrEmpty(strResp))
                                {
                                    dictionaryResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                                    if (dictionaryResp.GetValue("resCode") == "000")
                                    {

                                        dicAck.AddKeyValue("number", strArrData[1]);
                                        dicAck.AddKeyValue("amount", dictionaryResp.GetValue("testTotalAmount"));
                                        dicAck.AddKeyValue("billRefNo", dictionaryResp.GetValue("reqId"));
                                        strAckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.ActualBillAmount = dictionaryResp.GetValue("testTotalAmount");
                                        objAckStaus.RespCode = "0";
                                        objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespDesc = "success";
                                        objAckStaus.Service = "eseva";
                                        objAckStaus.Action = "41";
                                        objAckStaus.ActionDesc = "RTALLRTESTFEE";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + strArrData[0].ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                objAckStaus.RespCode = "1";
                                objAckStaus.RespDesc = "exception";
                            }
                            #endregion
                        }
                        break;
                    case "hathway":
                        {
                            #region HATHWAY
                            try
                            {
                                AddHeaders("hathway_viewbill", ref htHeaders);
                                strRequest = "{\"tid\":\"" + transId + "\",\"mobileNo\":\"" + strMobileNo + "\",\"accountno\":\"" + strArrData[1] + "\",\"Channel\":\"SMS\"}";
                                strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                                if (!string.IsNullOrEmpty(strResp))
                                {
                                    dictionaryResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                                    if (dictionaryResp.GetValue("resCode") == "000")
                                    {
                                        dicAck.AddKeyValue("number", strArrData[1]);
                                        dicAck.AddKeyValue("accountNo", dictionaryResp.GetValue("accountno"));
                                        dicAck.AddKeyValue("amount", dictionaryResp.GetValue("billamount"));
                                        dicAck.AddKeyValue("billRefNo", dictionaryResp.GetValue("billRefNo"));
                                        dicAck.AddKeyValue("hathwayMobileno", dictionaryResp.GetValue("mobileNo"));
                                        objAckStaus.ActualBillAmount = dictionaryResp.GetValue("billamount");
                                        strAckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespCode = "0";
                                        objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                        objAckStaus.RespDesc = "success";
                                        objAckStaus.Service = "eseva";
                                        objAckStaus.Action = "84";
                                        objAckStaus.ActionDesc = "HATHWAY";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + strArrData[0].ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                objAckStaus.RespCode = "1";
                                objAckStaus.RespDesc = "exception";
                            }
                            #endregion
                        }
                        break;
                    default:
                        {
                            TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception GetAcknowledgementJSON()==> Invalid department:" + strArrData[0].ToLower());
                            objAckStaus.RespCode = "1";
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception GetAcknowledgementJSON(),Message=" + ex.Message + ", stack trace:" + ex.StackTrace);
                objAckStaus.RespCode = "1";
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "Ack_json", "Dept:" + strArrData[0].ToLower() + ",mobileno:" + strMobileNo + ",strAckJson:" + strAckJson);
                dictionaryResp = null;
                dicAck = null;
                htHeaders = null;
            }
            return objAckStaus;
        }

        private void AddHeaders(string strType, ref Hashtable headers)
        {
            try
            {
                if (headers == null)
                    headers = new Hashtable();
                LoadConfig.LoadConfigFile();
                if (LoadConfig.xDocConfig != null)
                {
                    XmlNode xNode = LoadConfig.xDocConfig.DocumentElement.SelectSingleNode(String.Format("{0}", strType.ToLower()));
                    headers.Add("Service", xNode.GetAttrValue("Service"));
                    headers.Add("Action", xNode.GetAttrValue("Action"));
                    headers.Add("X-FORWARDIP", clsGeneral.GetConfigVal("CLIENT_IP"));
                }
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in AddHeaders, Message:" + ex.Message + ", stack trace:" + ex.StackTrace);
            }
        }

        //public IMPSBO PayThroughIMPS(string transId, string strMobileNo, string[] strArrData)
        //{
        //    IMPSBO objBO = new IMPSBO();
        //    PaymentBO objPaymentBO = DoValidations(strArrData);
        //    if (objPaymentBO != null && objPaymentBO.RespCode != "0")
        //    {
        //        objBO.RespCode = objPaymentBO.RespCode;
        //        objBO.RespDesc = objPaymentBO.RespDesc;
        //        return objBO;
        //    }

        //    try
        //    {
        //        string strParams = clsGeneral.GetConfigVal("IMPS_POSTDATA").Replace("!CUSTMOBNO!", strMobileNo);
        //        String strActualBillAmount = String.Empty;
        //        AckStatusBO objAckStatus = GetAcknowledgementJSON(transId, strMobileNo, strArrData);
        //        if (objAckStatus.RespCode == "0")
        //        {
        //            #region Call IMPS url

        //            #region Min Amount & Partial Amount Checks
        //            if (objAckStatus.ActualBillAmount.ToDouble() <= 0)
        //            {
        //                objBO.RespCode = "2";
        //                objBO.RespDesc = clsGeneral.GetConfigVal("INVALID_BILL_AMOUNT_MSG").Replace("!AMOUNT!", objAckStatus.ActualBillAmount);
        //                return objBO;
        //            }
        //            else if (Array.IndexOf(clsGeneral.GetConfigVal("PARTIAL_PAY_NOTALLOWED_DEPTS").Split(','), strArrData[0].ToUpper()) > -1)// check for partial amounts
        //            {
        //                if (objPaymentBO.Amount.ToDouble() < objAckStatus.ActualBillAmount.ToDouble())
        //                {
        //                    objBO.RespCode = "3";
        //                    objBO.RespDesc = clsGeneral.GetConfigVal("PARTIAL_AMOUNT_ERROR_MSG").Replace("!BILLAMOUNT!", objAckStatus.ActualBillAmount);
        //                    return objBO;
        //                }
        //            }
        //            #endregion
        //            strParams = strParams.Replace("!IMIPAYACK!", objAckStatus.AckJson);
        //            objBO.IMPSAmount = objPaymentBO.Amount;
        //            strParams = strParams.Replace("!MMID!", objPaymentBO.MMID).Replace("!CUSTOTP!", objPaymentBO.OTP).Replace("!AMOUNT!", objPaymentBO.Amount).Replace("!TRANSID!", DateTime.Now.Ticks.ToString());
        //            String strPaymentResp = clsGeneral.DoPostRequest(clsGeneral.GetConfigVal("IMPS_PAYMENT_URL"), strParams);
        //            TextLog.Exception(strWorkSpaceId, "imps_hits", "url:" + clsGeneral.GetConfigVal("IMPS_PAYMENT_URL") + Environment.NewLine + "Request:" + strParams + Environment.NewLine + "Response:" + strPaymentResp);
        //            if (!string.IsNullOrEmpty(strPaymentResp) && (!strPaymentResp.ToLower().Contains("transaction failed")))
        //            {
        //                #region commented
        //                /** PayGov Response Format:
        //                     * status=1&PAYMENT_REF_NO=635470847977014238&AdditionalInfo5=NA&paymentMode=IMM&ItemCode=IVRS&AuthStatus=0399&SettlementType=NA&AdditionalInfo6=NA&AdditionalInfo7=NA&BankReferenceNo=426711484456110149608886&TxnAmount=11.0&TxnReferenceNo=MIMM3495724277&CurrencyName=INR&ErrorStatus=NA&BankID=IMM&SecurityType=NA&SecurityID=NA&TxnDate=24-09-2014+11%3A46%3A33&SecurityPassword=NA&MerchantID=XNSKAMONE&ErrorDescription=Error+Occurred&BankMerchantID=NA&CustomerID=2393&AdditionalInfo4=NA&AdditionalInfo3=USSD&TxnType=06&AdditionalInfo2=GOV001&AdditionalInfo1=635470847977014238&tid=MIMM3495724277

        //                Uri myUri = new Uri("http://localhost/Default.aspx?" + strPaymentResp);
        //                String Status = HttpUtility.ParseQueryString(myUri.Query).Get("status");
        //                String strBankTransID = HttpUtility.ParseQueryString(myUri.Query).Get("tid");
        //                String strIMITransID = HttpUtility.ParseQueryString(myUri.Query).Get("AdditionalInfo5");
        //                 */

        //                #endregion
        //                Dictionary<string, string> dicPaymentResp = new Dictionary<string, string>();
        //                NameValueCollection nvcQueryString = HttpUtility.ParseQueryString(strPaymentResp.ToLower());
        //                dicPaymentResp = nvcQueryString.AllKeys.ToDictionary(k => k, k => nvcQueryString[k]);
        //                if (dicPaymentResp != null && dicPaymentResp.Count > 0)
        //                {
        //                    String Status = dicPaymentResp.GetValue("status");
        //                    String strBankTransID = dicPaymentResp.GetValue("tid");
        //                    String strIMITransID = dicPaymentResp.GetValue("AdditionalInfo5");
        //                    if (Status == "0")
        //                    {
        //                        objBO.TransNo = strBankTransID;
        //                        objBO.AuthCode = strBankTransID;
        //                        objBO.IMITransID = strIMITransID;
        //                        objBO.RespCode = "0";
        //                        return objBO;
        //                    }
        //                    else
        //                    {
        //                        objBO.RespCode = "1";
        //                        objBO.RespDesc = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
        //                        return objBO;
        //                    }
        //                }
        //                else
        //                {
        //                    objBO.RespCode = "1";
        //                    objBO.RespDesc = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
        //                    return objBO;
        //                }
        //            }
        //            else
        //            {
        //                objBO.RespCode = "1";
        //                objBO.RespDesc = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
        //                return objBO;
        //            }
        //            #endregion
        //        }
        //        else
        //        {
        //            objBO.RespCode = "1";
        //            objBO.RespDesc = clsGeneral.GetConfigVal("EXCEPTION_MSG");
        //            return objBO;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception PayThroughIMPS(),Message=" + ex.Message + ", stack trace:" + ex.StackTrace);
        //        objBO.RespCode = "1";
        //        objBO.RespDesc = clsGeneral.GetConfigVal("EXCEPTION_MSG");
        //        return objBO;
        //    }
        //}

        public TWALLETBO PayThroughTWALLET(string transId, string strMobileNo, string[] strArrData)
        {
            TWALLETBO objBO = new TWALLETBO();
            PaymentBO objPaymentBO = null;
            if (strArrData.Length == 5)
            {
                objPaymentBO = DoValidations(strArrData);
                if (objPaymentBO != null && objPaymentBO.RespCode != "0")
                {
                    objBO.RespCode = objPaymentBO.RespCode;
                    objBO.RespDesc = objPaymentBO.RespDesc;
                    return objBO;
                }
            }
            else
            {
                objPaymentBO = new PaymentBO();
                objPaymentBO.TwalletMobNo = strArrData.Length > 2 ? strArrData[2] : string.Empty;
                objPaymentBO.OTP = strArrData.Length > 3 ? strArrData[3] : string.Empty;
                if (string.IsNullOrEmpty(objPaymentBO.TwalletMobNo) || string.IsNullOrEmpty(objPaymentBO.OTP))
                {
                    objBO.RespCode = "1";
                    objBO.RespDesc = "mandatory params missing.";
                    return objBO;
                }
            }
            try
            {
                string strParams = clsGeneral.GetConfigVal("TWALLET_POSTDATA").Replace("!CUSTMOBNO!", objPaymentBO.TwalletMobNo);
                AckStatusBO objAckStatus = GetAcknowledgementJSON(transId, strMobileNo, strArrData, objPaymentBO);
                if (objAckStatus.RespCode == "0")
                {
                    #region Call Twallet url

                    #region Min Amount & Partial Amount Checks
                    if (strArrData.Length < 5)
                        objPaymentBO.Amount = objAckStatus.ActualBillAmount;
                    if (objAckStatus.ActualBillAmount.ToDouble() <= 0)
                    {
                        objBO.RespCode = "2";
                        objBO.RespDesc = clsGeneral.GetConfigVal("INVALID_BILL_AMOUNT_MSG").Replace("!AMOUNT!", objAckStatus.ActualBillAmount);
                        return objBO;
                    }
                    else if (Convert.ToDouble(objPaymentBO.Amount) < Convert.ToDouble(clsGeneral.GetConfigVal("MIN_ALLOWED_AMOUNT")))
                    {
                        objBO.RespCode = "1";
                        objBO.RespDesc = clsGeneral.GetConfigVal("INVALID_MIN_AMOUNT");
                        return objBO;
                    }
                    else if (Convert.ToDouble(objPaymentBO.Amount) > Convert.ToDouble(clsGeneral.GetConfigVal("MAX_ALLOWED_AMOUNT")))
                    {
                        objBO.RespCode = "1";
                        objBO.RespDesc = clsGeneral.GetConfigVal("INVALID_MAX_AMOUNT"); 
                        return objBO;
                    }
                    else if (Array.IndexOf(clsGeneral.GetConfigVal("PARTIAL_PAY_NOTALLOWED_DEPTS").Split(','), strArrData[0].ToUpper()) > -1)// check for partial amounts
                    {
                        if (objPaymentBO.Amount.ToDouble() < objAckStatus.ActualBillAmount.ToDouble())
                        {
                            objBO.RespCode = "3";
                            objBO.RespDesc = clsGeneral.GetConfigVal("PARTIAL_AMOUNT_ERROR_MSG").Replace("!BILLAMOUNT!", objAckStatus.ActualBillAmount);
                            return objBO;
                        }
                    }



                    #endregion
                    strParams = strParams.Replace("!IMIPAYACK!", objAckStatus.AckJson);
                    objBO.TWalletAmount = objPaymentBO.Amount;

                    strParams = strParams.Replace("!CUSTOTP!", objPaymentBO.OTP).Replace("!AMOUNT!", objPaymentBO.Amount).Replace("!TRANSID!", DateTime.Now.Ticks.ToString());
                    strParams = strParams.Replace("!ACTION!", objAckStatus.Action).Replace("!SERVICE!", objAckStatus.Service).Replace("!DESC!", objAckStatus.ActionDesc);
                    String strPaymentResp = clsGeneral.DoPostRequest(clsGeneral.GetConfigVal("TWALLET_PAYMENT_URL"), strParams);
                    //strPaymentResp = "{\"status\": \"0\",   \"TxnDate\": \"2017-09-01 18:21:34\",   \"BankReferenceNo\": \"724416222237\",   \"imitransid\": \"MGOV05181851989\",   \"TxnAmount\": \"1.00\",   \"vpc_MerchTxnRef\": \"636385916290610179\"}";

                    TextLog.Exception(strWorkSpaceId, "twallet_payment", "url:" + clsGeneral.GetConfigVal("TWALLET_PAYMENT_URL") + Environment.NewLine + "Request:" + strParams + Environment.NewLine + "Response:" + strPaymentResp);
                    if (!string.IsNullOrEmpty(strPaymentResp))
                    {
                        Dictionary<String, String> dicPaymentResp = new Dictionary<String, String>();
                        dicPaymentResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strPaymentResp);
                        if (dicPaymentResp != null && dicPaymentResp.Count > 0 && dicPaymentResp.GetValue("status") == "0")
                        {
                            String strTxnDate = dicPaymentResp.GetValue("TxnDate");
                            String strBankReferenceNo = dicPaymentResp.GetValue("BankReferenceNo");
                            String strIMITransId = dicPaymentResp.GetValue("imitransid");
                            String strTxnAmount = dicPaymentResp.GetValue("TxnAmount");
                            String strMer_Txn_RefNo = dicPaymentResp.GetValue("vpc_MerchTxnRef");

                            objBO.TransNo = strMer_Txn_RefNo;
                            objBO.AuthCode = strBankReferenceNo;
                            objBO.IMITransID = strIMITransId;
                            objBO.TransDate = strTxnDate;
                            objBO.TWalletAmount = strTxnAmount;
                            objBO.RespCode = "0";
                            return objBO;
                        }
                        else
                        {
                            objBO.RespCode = "1";
                            objBO.RespDesc = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
                            return objBO;
                        }
                    }
                    else
                    {
                        objBO.RespCode = "1";
                        objBO.RespDesc = clsGeneral.GetConfigVal("PAYMENT_FAIL_MSG");
                        return objBO;
                    }
                    #endregion
                }
                else
                {
                    objBO.RespCode = "1";
                    objBO.RespDesc = clsGeneral.GetConfigVal("EXCEPTION_MSG");
                    return objBO;
                }
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception PayThroughTWALLET(),Message=" + ex.Message + ", stack trace:" + ex.StackTrace);
                objBO.RespCode = "1";
                objBO.RespDesc = clsGeneral.GetConfigVal("EXCEPTION_MSG");
                return objBO;
            }
        }
        #region For IMPS -Not using

        //private PaymentBO DoValidations(String[] strArrData)
        //{
        //    //AIRTEL<space>your mobile no<space>amount<space>your MMID<space>OTP
        //    PaymentBO objBo = new PaymentBO();
        //    String strAmount = String.Empty;
        //    String strMMID = String.Empty;
        //    String strOTP = String.Empty;
        //    double dAmt;
        //    try
        //    {
        //        strAmount = strArrData.Length > 2 ? strArrData[2] : String.Empty;
        //        strMMID = strArrData.Length > 3 ? strArrData[3] : String.Empty;
        //        strOTP = strArrData.Length > 4 ? strArrData[4] : String.Empty;
        //        if (string.IsNullOrEmpty(strAmount))
        //        {
        //            objBo.RespCode = "1";
        //            objBo.RespDesc = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
        //        }
        //        else if (double.TryParse(strAmount, out dAmt) == false)
        //        {
        //            objBo.RespCode = "1";
        //            objBo.RespDesc = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
        //        }
        //        else if (Convert.ToDouble(strAmount) < Convert.ToDouble(clsGeneral.GetConfigVal("MIN_ALLOWED_AMOUNT")))
        //        {
        //            objBo.RespCode = "1";
        //            objBo.RespDesc = "Amount should not be less than " + clsGeneral.GetConfigVal("MIN_ALLOWED_AMOUNT") + ".";
        //        }
        //        else if (string.IsNullOrEmpty(strMMID))
        //        {
        //            objBo.RespCode = "1";
        //            objBo.RespDesc = clsGeneral.GetConfigVal("INVALID_MMID_OTP");
        //        }
        //        else if ((!strMMID.StartsWith("9")) || strMMID.Length != 7)
        //        {
        //            objBo.RespCode = "1";
        //            objBo.RespDesc = clsGeneral.GetConfigVal("INVALID_MMID_OTP");
        //        }
        //        else if (string.IsNullOrEmpty(strOTP))
        //        {
        //            objBo.RespCode = "1";
        //            objBo.RespDesc = clsGeneral.GetConfigVal("INVALID_MMID_OTP");
        //        }
        //        else
        //        {
        //            objBo.RespCode = "0";
        //            objBo.RespDesc = "success";
        //            objBo.Amount = strAmount;
        //            objBo.MMID = strMMID;
        //            objBo.OTP = strOTP;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TextLog.Exception(strWorkSpaceId, "Validations", "Exception in DoValidations==>" + ex.Message);
        //        objBo.RespCode = "1";
        //        objBo.RespDesc = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
        //    }
        //    return objBo;
        //} 
        #endregion

        private PaymentBO DoValidations(String[] strArrData)
        {
            //AIRTEL<space>your mobile no<space>amount<space>your MMID<space>OTP
            PaymentBO objBo = new PaymentBO();
            String strAmount = String.Empty;
            String strTwalletMobNo = String.Empty;
            String strOTP = String.Empty;
            double dAmt;
            try
            {
                strAmount = strArrData.Length > 2 ? strArrData[2] : String.Empty;
                strTwalletMobNo = strArrData.Length > 3 ? strArrData[3] : String.Empty;
                strOTP = strArrData.Length > 4 ? strArrData[4] : String.Empty;
                if (string.IsNullOrEmpty(strAmount))
                {
                    objBo.RespCode = "1";
                    objBo.RespDesc = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
                }
                else if (double.TryParse(strAmount, out dAmt) == false)
                {
                    objBo.RespCode = "1";
                    objBo.RespDesc = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
                }
                else if (Convert.ToDouble(strAmount) < Convert.ToDouble(clsGeneral.GetConfigVal("MIN_ALLOWED_AMOUNT")))
                {
                    objBo.RespCode = "1";
                    objBo.RespDesc = "Amount should not be less than " + clsGeneral.GetConfigVal("MIN_ALLOWED_AMOUNT") + ".";
                }
                else if (Convert.ToDouble(strAmount) > Convert.ToDouble(clsGeneral.GetConfigVal("MAX_ALLOWED_AMOUNT")))
                {
                    objBo.RespCode = "1";
                    objBo.RespDesc = "Amount should not be greater than " + clsGeneral.GetConfigVal("MAX_ALLOWED_AMOUNT") + ".";
                }
                else if (string.IsNullOrEmpty(strTwalletMobNo))
                {
                    objBo.RespCode = "1";
                    objBo.RespDesc = clsGeneral.GetConfigVal("INVALID_TWALLET_MOBNO");
                }
                else if (strTwalletMobNo.Length != 10)
                {
                    objBo.RespCode = "1";
                    objBo.RespDesc = clsGeneral.GetConfigVal("INVALID_TWALLET_MOBNO");
                }
                else if (string.IsNullOrEmpty(strOTP))
                {
                    objBo.RespCode = "1";
                    objBo.RespDesc = clsGeneral.GetConfigVal("INVALID_MMID_OTP");
                }
                else
                {
                    objBo.RespCode = "0";
                    objBo.RespDesc = "success";
                    objBo.Amount = strAmount;
                    objBo.TwalletMobNo = strTwalletMobNo;
                    objBo.OTP = strOTP;
                }
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "Validations", "Exception in DoValidations==>" + ex.Message);
                objBo.RespCode = "1";
                objBo.RespDesc = clsGeneral.GetConfigVal("INCORRECT_SYNTAX_MSG");
            }
            return objBo;
        }


        public Dictionary<String, String> User_Check_Existance(String tid, String strMobileNo)
        {
            #region CHECK USER EXISTANCE
            Dictionary<String, String> dicResp = new Dictionary<string, string>();
            try
            {
                AddHeaders("user_check", ref htHeaders);
                strRequest = "{\"tid\":\"" + tid + "\",\"mobileno\":\"" + strMobileNo + "\",\"channel\":\"SMS\"}";
                strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                if (!string.IsNullOrEmpty(strResp))
                {
                    dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                    if (dicResp.GetValue("resCode") == "2")// if user not exits
                    {
                        dicResp = User_Registartion(tid, strMobileNo);
                        if (dicResp.GetValue("resCode") == "000")
                            dicResp.EditKeyValue("resCode", "3");
                        else
                            dicResp.EditKeyValue("resCode", "1");
                    }
                }
                else
                    dicResp.AddKeyValue("resCode", "1");
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "USER_EX", "Exception in ControlAPIWrapper-->User_Check_Existance,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
                dicResp.AddKeyValue("resCode", "1");
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "USER", "strRequest:" + strRequest + ",strResp:" + strResp);
                htHeaders = null;
                if (dicResp == null || dicResp.Keys.Count == 0)
                    dicResp.AddKeyValue("resCode", "1");
            }
            return dicResp;
            #endregion
        }

        public Dictionary<String, String> User_Registartion(String tid, String strMobileNo)
        {
            #region USER REG
            Dictionary<String, String> dicResp = new Dictionary<String, String>();
            try
            {
                htHeaders = new Hashtable();
                AddHeaders("user_reg", ref htHeaders);
                strRequest = "{\"tid\":\"" + tid + "\",\"mobileno\":\"" + strMobileNo + "\",\"channel\":\"SMS\"}";
                strResp = clsGeneral.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                if (!string.IsNullOrEmpty(strResp))
                {
                    dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                }
                else
                    dicResp.AddKeyValue("resCode", "1");
            }
            catch (Exception ex)
            {
                TextLog.Exception(strWorkSpaceId, "USER_EX", "Exception in ControlAPIWrapper-->User_Registartion,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
                dicResp.AddKeyValue("resCode", "1");
            }
            finally
            {
                TextLog.Debug(strWorkSpaceId, "USER", "USER REG:strRequest:" + strRequest + ",strResp:" + strResp);
                htHeaders = null;
                if (dicResp == null || dicResp.Keys.Count == 0)
                    dicResp.AddKeyValue("resCode", "1");
            }
            return dicResp;
            #endregion
        }

        //public static bool IsValidSignature(string receivedHash, string PostText, string saltText)
        //{
        //    try
        //    {
        //        string sAccessToken = Authentication.GetOdd(saltText);
        //        string sBody = string.Format("REQBODY={0}&SALT={1}", PostText, sAccessToken);
        //        string sSignatureData = Authentication.GetSHA512Hash(sBody);
        //        //LogData.Write("GOTAPI", "EncrptedData", LogMode.Debug, "PostText: " + PostText + "\nSignature Data: " + sSignatureData + "\n\n");
        //        if (string.Compare(receivedHash.ToLower(), sSignatureData.ToLower(), true) == 0)
        //            return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogData.Write("GOTAPI", "EncrptedData", LogMode.Excep, "Exception Mag: " + ex.Message + "\nStack: " + ex.StackTrace + "\n\n");
        //    }
        //    return false;
        //}

        #endregion Private Methods
    }

    #region BO classes
    public class PaymentBO
    {
        public string RespCode { get; set; }
        public string RespDesc { get; set; }
        public string Amount { get; set; }
        // public string MMID { get; set; }
        public string OTP { get; set; }
        public string TwalletMobNo { get; set; }
    }

    public class IMPSBO
    {
        public string RespCode { get; set; }
        public string RespDesc { get; set; }
        public string IMPSAmount { get; set; }
        public string TransNo { get; set; }
        public string AuthCode { get; set; }
        public string IMITransID { get; set; }
    }

    public class AckStatusBO
    {
        public string AckJson { get; set; }
        public string ActualBillAmount { get; set; }
        public string RespCode { get; set; }
        public string RespDesc { get; set; }
        public string Service { get; set; }
        public string Action { get; set; }
        public string ActionDesc { get; set; }
    }

    public class TWALLETBO
    {
        public string RespCode { get; set; }
        public string RespDesc { get; set; }
        public string TWalletAmount { get; set; }
        public string TransNo { get; set; }
        public string AuthCode { get; set; }
        public string IMITransID { get; set; }
        public string TransDate { get; set; }
    }
    #endregion
}