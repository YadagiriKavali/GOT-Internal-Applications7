using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

/// <summary>
/// Summary description for ControlAPIWrapper
/// </summary>
public class ControlAPIWrapper
{
    Hashtable htHeaders = new Hashtable();
    Dictionary<String, String> dicResp = new Dictionary<String, String>();
    String strRequest = String.Empty;
    String strResp = String.Empty;
    private static string strAPIURL = General.GetConfigVal("CONTROL_API_URL");
    private static string strHttpMethod = "POST";
    private static string strContentType = "application/json";
    private static int iWebReqTimeOut = General.GetConfigVal("REQ_TIMEOUT").ToInt();
    private string strPGServiceId = String.Empty;
    private static string strWorkSpaceId = "channelinterface";
    private static string strErrorFileName = "controlapiwrapper";

    #region View Bill Methods
    public Dictionary<String, String> HMWSSB_ViewBill(Dictionary<String, String> dicRequest)
    {
        #region HMWSSB VIEW BILL
        try
        {
            if (string.IsNullOrEmpty(dicRequest.GetValue("number")) || (dicRequest.GetValue("number").Trim().Length < General.GetConfigVal("HMWSSB_CANNO_MIN_LENGTH").ToInt()) || (dicRequest.GetValue("number").Trim().Length > General.GetConfigVal("HMWSSB_CANNO_MAX_LENGTH").ToInt()))
            {                
                TextLog.Exception("CHANNELINTERFACE", "HMWSSB_EX", "Invlaid Can length , tid:" + dicRequest.GetValue("tid") + ",strCANNo:" + dicRequest.GetValue("number"));
                dicResp.AddKeyValue("resCode", "3");
            }
            else
            {
                AddHeaders("hmwssb_viewbill", ref htHeaders);
                strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"number\":\"" + dicRequest.GetValue("number") + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\"}";
                strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                if (!string.IsNullOrEmpty(strResp))
                    dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                else
                    dicResp.AddKeyValue("resCode", "1");
            }
        }
        catch (Exception ex)
        {
            TextLog.Exception("CHANNELINTERFACE", "HMWSSB_EX", "Exception in ControlAPIWrapper-->HMWSSBViewBill,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
            dicResp.AddKeyValue("resCode", "2");
        }
        finally
        {
            TextLog.Debug("CHANNELINTERFACE", "HMWSSB", "HMWSSBViewBill, strRequest:" + strRequest + ",strResp:" + strResp);
            htHeaders = null;
            if (dicResp == null || dicResp.Keys.Count == 0)
                dicResp.AddKeyValue("resCode", "1");
        }
        return dicResp;
        #endregion
    }

    public Dictionary<String, String> Airtel_ViewBill(Dictionary<String, String> dicRequest)
    {
        #region AIRTEL VIEW BILL
        try
        {
            if (string.IsNullOrEmpty(dicRequest.GetValue("number")) || dicRequest.GetValue("number").Trim().Length != 10)
            {
                TextLog.Exception("CHANNELINTERFACE", "AIRTEL_EX", "Invlaid Pay Mobileno , tid:" + dicRequest.GetValue("tid") + ",number:" + dicRequest.GetValue("number"));
                dicResp.AddKeyValue("resCode", "3");
            }
            else
            {
                AddHeaders("airtel_viewbill", ref htHeaders);
                strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"number\":\"" + dicRequest.GetValue("number") + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\"}";
                strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                if (!string.IsNullOrEmpty(strResp))
                    dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                else
                    dicResp.AddKeyValue("resCode", "1");
            }
        }
        catch (Exception ex)
        {
            TextLog.Exception("CHANNELINTERFACE", "AIRTEL_EX", "Exception in ControlAPIWrapper-->AIRTELViewBill,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
            dicResp.AddKeyValue("resCode", "2");
        }
        finally
        {
            TextLog.Debug("CHANNELINTERFACE", "AIRTEL", "AIRTELViewBill, strRequest:" + strRequest + ",strResp:" + strResp);
            htHeaders = null;
            if (dicResp == null || dicResp.Keys.Count == 0)
                dicResp.AddKeyValue("resCode", "1");
        }
        return dicResp;
        #endregion
    }

    public Dictionary<String, String> Airtel_Landline_ViewBill(Dictionary<String, String> dicRequest)
    {
        #region AIRTEL LANDLINE VIEW BILL
        try
        {
            if (string.IsNullOrEmpty(dicRequest.GetValue("number")))
            {
                TextLog.Exception("CHANNELINTERFACE", "AIRTEL_EX", "Invlaid Pay Mobileno , tid:" + dicRequest.GetValue("tid") + ",number:" + dicRequest.GetValue("number"));
                dicResp.AddKeyValue("resCode", "3");
            }
            else
            {
                AddHeaders("airtel_landline_viewbill", ref htHeaders);
                strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"number\":\"" + dicRequest.GetValue("number") + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\"}";
                strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                if (!string.IsNullOrEmpty(strResp))
                    dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                else
                    dicResp.AddKeyValue("resCode", "1");
            }
        }
        catch (Exception ex)
        {
            TextLog.Exception("CHANNELINTERFACE", "AIRTEL_EX", "Exception in ControlAPIWrapper-->AIRTEL Landline ViewBill,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
            dicResp.AddKeyValue("resCode", "2");
        }
        finally
        {
            TextLog.Debug("CHANNELINTERFACE", "AIRTEL", "AIRTEL Landline ViewBill, strRequest:" + strRequest + ",strResp:" + strResp);
            htHeaders = null;
            if (dicResp == null || dicResp.Keys.Count == 0)
                dicResp.AddKeyValue("resCode", "1");
        }
        return dicResp;
        #endregion
    }

    public Dictionary<String, String> Idea_ViewBill(Dictionary<String, String> dicRequest)
    {
        #region IDEA VIEW BILL
        try
        {
            if (string.IsNullOrEmpty(dicRequest.GetValue("number")) || dicRequest.GetValue("number").Trim().Length != 10)
            {
                TextLog.Exception("CHANNELINTERFACE", "IDEA_EX", "Invlaid Pay Mobileno , tid:" + dicRequest.GetValue("tid") + ",number:" + dicRequest.GetValue("number"));
                dicResp.AddKeyValue("resCode", "3");
            }
            else
            {
                AddHeaders("idea_viewbill", ref htHeaders);
                strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"number\":\"" + dicRequest.GetValue("number") + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\"}";
                strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                if (!string.IsNullOrEmpty(strResp))
                    dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                else
                    dicResp.AddKeyValue("resCode", "1");
            }
        }
        catch (Exception ex)
        {
            TextLog.Exception("CHANNELINTERFACE", "IDEA_EX", "Exception in ControlAPIWrapper-->IDEA ViewBill,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
            dicResp.AddKeyValue("resCode", "2");
        }
        finally
        {
            TextLog.Debug("CHANNELINTERFACE", "IDEA", "IDEAViewBill, strRequest:" + strRequest + ",strResp:" + strResp);
            htHeaders = null;
            if (dicResp == null || dicResp.Keys.Count == 0)
                dicResp.AddKeyValue("resCode", "1");
        }
        return dicResp;
        #endregion
    }

    public Dictionary<String, String> Act_ViewBill(Dictionary<String, String> dicRequest)
    {
        #region Act VIEW BILL
        try
        {
            if (string.IsNullOrEmpty(dicRequest.GetValue("number")))
            {
                TextLog.Exception("CHANNELINTERFACE", "ACT_EX", "Invlaid Pay Mobileno , tid:" + dicRequest.GetValue("tid") + ",number:" + dicRequest.GetValue("number"));
                dicResp.AddKeyValue("resCode", "3");
            }
            else
            {
                AddHeaders("act_viewbill", ref htHeaders);
                strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"number\":\"" + dicRequest.GetValue("number") + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\"}";
                strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                if (!string.IsNullOrEmpty(strResp))
                    dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                else
                    dicResp.AddKeyValue("resCode", "1");
            }
        }
        catch (Exception ex)
        {
            TextLog.Exception("CHANNELINTERFACE", "ACT_EX", "Exception in ControlAPIWrapper-->ACT ViewBill,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
            dicResp.AddKeyValue("resCode", "2");
        }
        finally
        {
            TextLog.Debug("CHANNELINTERFACE", "ACT", "ACTViewBill, strRequest:" + strRequest + ",strResp:" + strResp);
            htHeaders = null;
            if (dicResp == null || dicResp.Keys.Count == 0)
                dicResp.AddKeyValue("resCode", "1");
        }
        return dicResp;
        #endregion
    }

    public Dictionary<String, String> RTA_ViewBill(Dictionary<String, String> dicRequest)
    {
        #region RTA VIEW BILL
        try
        {
            if (string.IsNullOrEmpty(dicRequest.GetValue("number")))
            {
                TextLog.Exception("CHANNELINTERFACE", "RTA_EX", "Invlaid number , tid:" + dicRequest.GetValue("tid") + ",number:" + dicRequest.GetValue("number"));
                dicResp.AddKeyValue("resCode", "3");
            }
            else
            {
                AddHeaders("rta_viewbill", ref htHeaders);
                strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"number\":\"" + dicRequest.GetValue("number") + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\"}";
                strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                if (!string.IsNullOrEmpty(strResp))
                    dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                else
                    dicResp.AddKeyValue("resCode", "1");
            }
        }
        catch (Exception ex)
        {
            TextLog.Exception("CHANNELINTERFACE", "RTA_EX", "Exception in ControlAPIWrapper-->RTA ViewBill,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
            dicResp.AddKeyValue("resCode", "2");
        }
        finally
        {
            TextLog.Debug("CHANNELINTERFACE", "RTA", "RTAViewBill, strRequest:" + strRequest + ",strResp:" + strResp);
            htHeaders = null;
            if (dicResp == null || dicResp.Keys.Count == 0)
                dicResp.AddKeyValue("resCode", "1");
        }
        return dicResp;
        #endregion
    }

    public Dictionary<String, String> TTL_ViewBill(Dictionary<String, String> dicRequest)
    {
        #region TTL VIEW BILL
        try
        {
            if (string.IsNullOrEmpty(dicRequest.GetValue("number")) || dicRequest.GetValue("number").Trim().Length != 10)
            {
                TextLog.Exception("CHANNELINTERFACE", "TTL_EX", "Invlaid number , tid:" + dicRequest.GetValue("tid") + ",number:" + dicRequest.GetValue("number"));
                dicResp.AddKeyValue("resCode", "3");
            }
            else
            {
                AddHeaders("ttl_viewbill", ref htHeaders);
                strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"number\":\"" + dicRequest.GetValue("number") + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\"}";
                strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                if (!string.IsNullOrEmpty(strResp))
                    dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                else
                    dicResp.AddKeyValue("resCode", "1");
            }
        }
        catch (Exception ex)
        {
            TextLog.Exception("CHANNELINTERFACE", "TTL_EX", "Exception in ControlAPIWrapper-->RTA ViewBill,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
            dicResp.AddKeyValue("resCode", "2");
        }
        finally
        {
            TextLog.Debug("CHANNELINTERFACE", "TTL", "TTLViewBill, strRequest:" + strRequest + ",strResp:" + strResp);
            htHeaders = null;
            if (dicResp == null || dicResp.Keys.Count == 0)
                dicResp.AddKeyValue("resCode", "1");
        }
        return dicResp;
        #endregion
    }

    public Dictionary<String, String> Bsnl_ViewBill(Dictionary<String, String> dicRequest)
    {
        #region BSNL VIEW BILL
        try
        {
            if (string.IsNullOrEmpty(dicRequest.GetValue("number")) || dicRequest.GetValue("number").Trim().Length != 10)
            {
                TextLog.Exception("CHANNELINTERFACE", "AIRTEL_EX", "Invlaid Pay Mobileno , tid:" + dicRequest.GetValue("tid") + ",number:" + dicRequest.GetValue("number"));
                dicResp.AddKeyValue("resCode", "3");
            }
            else
            {
                AddHeaders("bsnl_viewbill", ref htHeaders);
                strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"number\":\"" + dicRequest.GetValue("number") + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\"}";
                strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                if (!string.IsNullOrEmpty(strResp))
                    dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                else
                    dicResp.AddKeyValue("resCode", "1");
            }
        }
        catch (Exception ex)
        {
            TextLog.Exception("CHANNELINTERFACE", "BSNL_EX", "Exception in ControlAPIWrapper-->Bsnl_ViewBill,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
            dicResp.AddKeyValue("resCode", "2");
        }
        finally
        {
            TextLog.Debug("CHANNELINTERFACE", "BSNL", "Bsnl_ViewBill, strRequest:" + strRequest + ",strResp:" + strResp);
            htHeaders = null;
            if (dicResp == null || dicResp.Keys.Count == 0)
                dicResp.AddKeyValue("resCode", "1");
        }
        return dicResp;
        #endregion
    }

    public Dictionary<String, String> RTA_LifeTax(Dictionary<String, String> dicRequest)
    {
        #region RTA Life Tax
        try
        {
            if (string.IsNullOrEmpty(dicRequest.GetValue("number")))
            {
                TextLog.Exception("CHANNELINTERFACE", "RTALT_EX", "Invlaid number , tid:" + dicRequest.GetValue("tid") + ",number:" + dicRequest.GetValue("number"));
                dicResp.AddKeyValue("resCode", "3");
            }
            else
            {
                AddHeaders("rta_lifetax_viewbill", ref htHeaders);
                strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"number\":\"" + dicRequest.GetValue("number").ToUpper() + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\"}";
                strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                if (!string.IsNullOrEmpty(strResp))
                    dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                else
                    dicResp.AddKeyValue("resCode", "1");
            }
        }
        catch (Exception ex)
        {
            TextLog.Exception("CHANNELINTERFACE", "RTALT_EX", "Exception in ControlAPIWrapper-->RTA_LifeTax,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
            dicResp.AddKeyValue("resCode", "2");
        }
        finally
        {
            TextLog.Debug("CHANNELINTERFACE", "RTALT", "RTA_LifeTax, strRequest:" + strRequest + ",strResp:" + strResp);
            htHeaders = null;
            if (dicResp == null || dicResp.Keys.Count == 0)
                dicResp.AddKeyValue("resCode", "1");
        }
        return dicResp;
        #endregion
    }

    public Dictionary<String, String> Get_MEESEVA_Application_Status(Dictionary<String, String> dicRequest)
    {
        #region APPLICATION STATUS
        try
        {
            if (string.IsNullOrEmpty(dicRequest.GetValue("number")) || dicRequest.GetValue("number").Trim().Length > 20)
            {
                TextLog.Exception("CHANNELINTERFACE", "APPLICATION_STATUS_EX", "Invalid application no , tid:" + dicRequest.GetValue("tid") + ",application no:" + dicRequest.GetValue("number"));
                dicResp.AddKeyValue("resCode", "3");
            }
            else if (!(dicRequest.GetValue("number").isAlphaNumeric()))
            {
                TextLog.Exception("CHANNELINTERFACE", "APPLICATION_STATUS_EX", "Invalid application no , tid:" + dicRequest.GetValue("tid") + ",application no:" + dicRequest.GetValue("number"));
                dicResp.AddKeyValue("resCode", "3");
            }
            else
            {
                AddHeaders("application_status", ref htHeaders);
                strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"applicationNo\":\"" + dicRequest.GetValue("number").ToUpper() + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\"}";
                strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                if (!string.IsNullOrEmpty(strResp))
                    dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                else
                    dicResp.AddKeyValue("resCode", "1");
            }
        }
        catch (Exception ex)
        {
            TextLog.Exception("CHANNELINTERFACE", "APPLICATION_STATUS_EX", "Exception in ControlAPIWrapper-->Get_MEESEVA_Application_Status,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
            dicResp.AddKeyValue("resCode", "2");
        }
        finally
        {
            TextLog.Debug("CHANNELINTERFACE", "APPLICATION_STATUS", "Get_MEESEVA_Application_Status, strRequest:" + strRequest + ",strResp:" + strResp);
            htHeaders = null;
            if (dicResp == null || dicResp.Keys.Count == 0)
                dicResp.AddKeyValue("resCode", "1");
        }
        return dicResp;
        #endregion
    }

    public Dictionary<String, String> HathWay_ViewBill(Dictionary<String, String> dicRequest)
    {
        #region HATHWAY VIEW BILL
        try
        {
            if (string.IsNullOrEmpty(dicRequest.GetValue("number")))
            {
                TextLog.Exception("CHANNELINTERFACE", "HATHWAY_EX", "Invlaid Pay Mobileno , tid:" + dicRequest.GetValue("tid") + ",number:" + dicRequest.GetValue("number"));
                dicResp.AddKeyValue("resCode", "3");
            }
            else
            {
                AddHeaders("hathway_viewbill", ref htHeaders);
                strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"accountno\":\"" + dicRequest.GetValue("number") + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\"}";
                strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                if (!string.IsNullOrEmpty(strResp))
                    dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                else
                    dicResp.AddKeyValue("resCode", "1");
            }
        }
        catch (Exception ex)
        {
            TextLog.Exception("CHANNELINTERFACE", "HATHWAY_EX", "Exception in ControlAPIWrapper-->HATHWAY ViewBill,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
            dicResp.AddKeyValue("resCode", "2");
        }
        finally
        {
            TextLog.Debug("CHANNELINTERFACE", "HATHWAY", "HATHWAYViewBill, strRequest:" + strRequest + ",strResp:" + strResp);
            htHeaders = null;
            if (dicResp == null || dicResp.Keys.Count == 0)
                dicResp.AddKeyValue("resCode", "1");
        }
        return dicResp;
        #endregion
    }

    #endregion

    public Dictionary<String, String> User_Update_PreferredLang(Dictionary<String, String> dicRequest)
    {
        #region UPDATE LANGUAGE
        try
        {
            if (string.IsNullOrEmpty(dicRequest.GetValue("language")))
            {
                TextLog.Exception(strWorkSpaceId, "USER_EX", "Invlaid language , tid:" + dicRequest.GetValue("tid") + ",Language:" + dicRequest.GetValue("language"));
                dicResp.AddKeyValue("resCode", "3");
            }
            else
            {
                AddHeaders(dicRequest.GetValue("service"), ref htHeaders);
                strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileno\":\"" + dicRequest.GetValue("mobileno") + "\",\"language\":\"" + dicRequest.GetValue("language") + "\",\"channel\":\"" + dicRequest.GetValue("channel") + "\"}";
                strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
                if (!string.IsNullOrEmpty(strResp))
                    dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                else
                    dicResp.AddKeyValue("resCode", "1");
            }
        }
        catch (Exception ex)
        {
            TextLog.Exception(strWorkSpaceId, "USER_EX", "Exception in ControlAPIWrapper-->User_Update_PreferredLang,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
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

    public Dictionary<String, String> User_Check_Existance(Dictionary<String, String> dicRequest)
    {
        #region CHECK USER EXISTANCE
        try
        {
            AddHeaders(dicRequest.GetValue("service"), ref htHeaders);
            strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileno\":\"" + dicRequest.GetValue("mobileno") + "\",\"channel\":\"" + dicRequest.GetValue("channel") + "\"}";
            strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
            if (!string.IsNullOrEmpty(strResp))
            {
                dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
                if (dicResp.GetValue("resCode") == "2")// if user not exits
                {
                    dicResp = User_Registartion(dicRequest);
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

    private Dictionary<String, String> User_Registartion(Dictionary<String, String> dicRequest)
    {
        #region USER REG
        try
        {
            htHeaders = new Hashtable();
            AddHeaders("user_reg", ref htHeaders);
            strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileno\":\"" + dicRequest.GetValue("mobileno") + "\",\"channel\":\"" + dicRequest.GetValue("channel") + "\"}";
            strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
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

    public Dictionary<String, String> GenerateTwalletOTP(Dictionary<String, String> dicRequest)
    {
        Dictionary<String, String> dicOTPResp = new Dictionary<string, string>();
        String strOTPRequest = String.Empty;
        String strOTPResp = String.Empty;
        try
        {
            if (string.IsNullOrEmpty(dicRequest.GetValue("twalletmobileno")) || dicRequest.GetValue("twalletmobileno").Length != 10)
            {
                dicOTPResp.EditKeyValue("Response_Code", "1");
                TextLog.Exception(strWorkSpaceId, "TWALLET_OTP", "invalid T wallet mobileno:" + dicRequest.GetValue("twalletmobileno"));
            }
            else
            {
                strOTPRequest = General.GetConfigVal("TWALLET_OTP_GENERATE_POSTDATA").Replace("!TWALLETMOBNO!", dicRequest.GetValue("twalletmobileno"));
                //strParams = strParams.Replace("!AMOUNT!", dicRequest.GetValue("amount"));
                //strOTPResp = General.DoPostRequest(General.GetConfigVal("TWALLET_OTP_GENERATE_URL"), strOTPRequest);
                strOTPResp = General.DoWebRequest(General.GetConfigVal("TWALLET_OTP_GENERATE_URL") + "?" + strOTPRequest);
                if (!string.IsNullOrEmpty(strOTPResp) && (!strOTPResp.ToLower().Contains("transaction failed")) && (!strOTPResp.ToLower().Contains("missing parameter")))
                {
                    dicOTPResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strOTPResp);
                }
                else
                    dicOTPResp.EditKeyValue("Response_Code", "1");
            }
        }
        catch (Exception ex)
        {
            TextLog.Exception(strWorkSpaceId, "TWALLET_OTP_EX", "Exception in GenerateTwalletOTP,Message=" + ex.Message + ", stack trace:" + ex.StackTrace);
            dicOTPResp.EditKeyValue("Response_Code", "1");
        }
        finally
        {
            TextLog.Debug(strWorkSpaceId, "TWALLET_OTP", "strRequest:" + strOTPRequest + ",strResp:" + strOTPResp);
            htHeaders = null;
            if (dicOTPResp == null || dicOTPResp.Keys.Count == 0)
                dicOTPResp.EditKeyValue("Response_Code", "1");
        }
        return dicOTPResp;
    }

    private void AddHeaders(string strType, ref Hashtable headers)
    {
        try
        {
            LoadConfig.LoadConfigFile();
            if (LoadConfig.xDocConfig != null)
            {
                XmlNode xNode = LoadConfig.xDocConfig.DocumentElement.SelectSingleNode(String.Format("{0}", strType.ToLower()));
                headers.Add("Service", xNode.GetAttrValue("Service"));
                headers.Add("Action", xNode.GetAttrValue("Action"));
                headers.Add("X-FORWARDIP", General.GetConfigVal("CLIENT_IP"));
            }
        }
        catch (Exception ex)
        {
            TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in AddHeaders, Message:" + ex.Message + ", stack trace:" + ex.StackTrace);
        }
    }

    private AckStatusBO GetAcknowledgementJSON(Dictionary<String, String> dicRequest)
    {
        AckStatusBO objAckStaus = new AckStatusBO();
        Dictionary<String, String> dictionaryResp = new Dictionary<String, String>();
        Dictionary<string, string> dicAck = new Dictionary<string, string>();
        dicAck.AddKeyValue("tid", dicRequest.GetValue("tid"));
        dicAck.AddKeyValue("mobileNo", dicRequest.GetValue("mobileno"));
        dicAck.AddKeyValue("transcode", "!TRANSCODE!");
        dicAck.AddKeyValue("authcode", "!AUTHCODE!");
        dicAck.AddKeyValue("cardtype", "!CARDTYPE!");
        dicAck.AddKeyValue("imitransid", "!IMITRANSID!");
        dicAck.AddKeyValue("ptransid", "!PTRANSID!");
        dicAck.AddKeyValue("channel", dicRequest.GetValue("channel").ToUpper());
        dicAck.AddKeyValue("email", "", true);
        dicAck.AddKeyValue("username", "", true);
        try
        {
            //if (dicRequest.GetValue("service") == "GENERATEORDERID")
            //{
            //    if (strLanguage == "1")
            //        strLanguage = "2";
            //    else if (strLanguage == "2")
            //        strLanguage = "1";
            //}
            switch (dicRequest.GetValue("dept").ToLower())
            {
                case "hmwssb":
                    {
                        #region HMWSSB
                        dictionaryResp = HMWSSB_ViewBill(dicRequest);
                        if (dictionaryResp.GetValue("resCode") == "000")
                        {
                            dicAck.AddKeyValue("number", dicRequest.GetValue("number"));
                            dicAck.AddKeyValue("network", "HMWSSB");
                            dicAck.AddKeyValue("accountNo", dictionaryResp.GetValue("accountNo"));
                            dicAck.AddKeyValue("amount", dicRequest.GetValue("amount"));
                            dicAck.AddKeyValue("billRefNo", dictionaryResp.GetValue("reqId"));
                            objAckStaus.ActualBillAmount = dictionaryResp.GetValue("amount");
                            objAckStaus.RespCode = "0";
                            objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                            objAckStaus.RespDesc = "success";
                            objAckStaus.Service = "eseva";
                            objAckStaus.Action = "8";
                            objAckStaus.ActionDesc = "HMWSSB";
                        }
                        #endregion
                    }
                    break;
                case "act":
                    {
                        #region ACT
                        try
                        {
                            dictionaryResp = Act_ViewBill(dicRequest);
                            if (dictionaryResp.GetValue("resCode") == "000")
                            {
                                dicAck.AddKeyValue("number", dicRequest.GetValue("number"));
                                // dicAck.AddKeyValue("network", "AIRTEL_LANDLINE");
                                dicAck.AddKeyValue("accountNo", dictionaryResp.GetValue("accountNo"));
                                dicAck.AddKeyValue("amount", dicRequest.GetValue("amount"));
                                dicAck.AddKeyValue("billRefNo", dictionaryResp.GetValue("reqId"));
                                objAckStaus.ActualBillAmount = dictionaryResp.GetValue("billAmount");
                                objAckStaus.RespCode = "0";
                                objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                objAckStaus.RespDesc = "success";
                                objAckStaus.Service = "eseva";
                                objAckStaus.Action = "10";
                                objAckStaus.ActionDesc = "ACT";
                            }
                        }
                        catch (Exception ex)
                        {
                            TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + dicRequest.GetValue("dept").ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
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
                            dictionaryResp = Airtel_ViewBill(dicRequest);
                            if (dictionaryResp.GetValue("resCode") == "000")
                            {
                                dicAck.AddKeyValue("number", dicRequest.GetValue("number"));
                                dicAck.AddKeyValue("network", "Airtel");
                                dicAck.AddKeyValue("accountNo", dictionaryResp.GetValue("accountNo"));
                                dicAck.AddKeyValue("amount", dicRequest.GetValue("amount"));
                                dicAck.AddKeyValue("billRefNo", dictionaryResp.GetValue("reqId"));
                                objAckStaus.ActualBillAmount = dictionaryResp.GetValue("billAmount");
                                objAckStaus.RespCode = "0";
                                objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                objAckStaus.RespDesc = "success";
                                objAckStaus.Service = "eseva";
                                objAckStaus.Action = "2";
                                objAckStaus.ActionDesc = "AIRTEL";
                            }
                        }
                        catch (Exception ex)
                        {
                            TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + dicRequest.GetValue("dept").ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                            objAckStaus.RespCode = "1";
                            objAckStaus.RespDesc = "exception";
                        }
                        #endregion
                    }
                    break;
                case "airtel_landline":
                    {
                        #region AIRTEL LAND LINE
                        try
                        {
                            dictionaryResp = Airtel_Landline_ViewBill(dicRequest);
                            if (dictionaryResp.GetValue("resCode") == "000")
                            {
                                dicAck.AddKeyValue("number", dicRequest.GetValue("number"));
                                dicAck.AddKeyValue("network", "AIRTEL_LANDLINE");
                                dicAck.AddKeyValue("accountNo", dictionaryResp.GetValue("accountNo"));
                                dicAck.AddKeyValue("amount", dicRequest.GetValue("amount"));
                                dicAck.AddKeyValue("billRefNo", dictionaryResp.GetValue("reqId"));
                                objAckStaus.ActualBillAmount = dictionaryResp.GetValue("billAmount");
                                objAckStaus.RespCode = "0";
                                objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                objAckStaus.RespDesc = "success";
                                objAckStaus.Service = "eseva";
                                objAckStaus.Action = "4";
                                objAckStaus.ActionDesc = "AIRTELLANDLINE";
                            }
                        }
                        catch (Exception ex)
                        {
                            TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + dicRequest.GetValue("dept").ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
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
                            dictionaryResp = Idea_ViewBill(dicRequest);
                            if (dictionaryResp.GetValue("resCode") == "000")
                            {
                                dicAck.AddKeyValue("number", dicRequest.GetValue("number"));
                                dicAck.AddKeyValue("accountNo", dictionaryResp.GetValue("accountNo"));
                                dicAck.AddKeyValue("amount", dicRequest.GetValue("amount"));
                                dicAck.AddKeyValue("billRefNo", dictionaryResp.GetValue("reqId"));
                                objAckStaus.ActualBillAmount = dictionaryResp.GetValue("billAmount");
                                objAckStaus.RespCode = "0";
                                objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                objAckStaus.RespDesc = "success";
                                objAckStaus.Service = "eseva";
                                objAckStaus.Action = "6";
                                objAckStaus.ActionDesc = "IDEA";
                            }
                        }
                        catch (Exception ex)
                        {
                            TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + dicRequest.GetValue("dept").ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                            objAckStaus.RespCode = "1";
                            objAckStaus.RespDesc = "exception";
                        }
                        #endregion
                    }
                    break;
                case "ttl":
                    {
                        #region TTL
                        try
                        {
                            dictionaryResp = TTL_ViewBill(dicRequest);
                            if (dictionaryResp.GetValue("resCode") == "000")
                            {
                                dicAck.AddKeyValue("number", dicRequest.GetValue("number"));
                                dicAck.AddKeyValue("accountNo", dictionaryResp.GetValue("accountNo"));
                                dicAck.AddKeyValue("amount", dicRequest.GetValue("amount"));
                                dicAck.AddKeyValue("billRefNo", dictionaryResp.GetValue("reqId"));
                                objAckStaus.ActualBillAmount = dictionaryResp.GetValue("billAmount");
                                objAckStaus.RespCode = "0";
                                objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                objAckStaus.RespDesc = "success";
                                objAckStaus.Service = "eseva";
                                objAckStaus.Action = "14";
                                objAckStaus.ActionDesc = "TTL";
                            }
                        }
                        catch (Exception ex)
                        {
                            TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + dicRequest.GetValue("dept").ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                            objAckStaus.RespCode = "1";
                            objAckStaus.RespDesc = "exception";
                        }
                        #endregion
                    }
                    break;
                case "reliance":
                    {
                        #region RELIANCE
                        dicAck.AddKeyValue("operator", "RG");
                        PrepareForPostPaid(dicAck, dicRequest, ref objAckStaus);
                        #endregion
                    }
                    break;
                case "vodafone":
                    {
                        #region VODAFONE
                        dicAck.AddKeyValue("operator", "VO");
                        PrepareForPostPaid(dicAck, dicRequest, ref objAckStaus);
                        #endregion
                    }
                    break;
                case "bsnl":
                    {
                        #region BSNL
                        try
                        {
                            dictionaryResp = Bsnl_ViewBill(dicRequest);
                            if (dictionaryResp.GetValue("resCode") == "000")
                            {
                                dicAck.AddKeyValue("number", dicRequest.GetValue("number"));
                                dicAck.AddKeyValue("amount", dicRequest.GetValue("amount"));
                                dicAck.AddKeyValue("billRefNo", dictionaryResp.GetValue("reqId"));
                                objAckStaus.ActualBillAmount = dictionaryResp.GetValue("billAmount");
                                objAckStaus.RespCode = "0";
                                objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                objAckStaus.RespDesc = "success";
                                objAckStaus.Service = "eseva";
                                objAckStaus.Action = "34";
                                objAckStaus.ActionDesc = "BSNL";
                            }
                        }
                        catch (Exception ex)
                        {
                            TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + dicRequest.GetValue("dept").ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                            objAckStaus.RespCode = "1";
                            objAckStaus.RespDesc = "exception";
                        }
                        #endregion
                    }
                    break;

                #region DTH RECHARGE
                case "dth_airtel":
                    {
                        dicAck.AddKeyValue("operator", "AT");
                        PrepareForRecharge(dicAck, dicRequest, ref objAckStaus);
                    }
                    break;
                case "dth_bigtv":
                    {
                        dicAck.AddKeyValue("operator", "BI");
                        PrepareForRecharge(dicAck, dicRequest, ref objAckStaus);
                    }
                    break;
                case "dth_dishtv":
                    {
                        dicAck.AddKeyValue("operator", "DI");
                        PrepareForRecharge(dicAck, dicRequest, ref objAckStaus);
                    }
                    break;
                case "dth_sundirect":
                    {
                        dicAck.AddKeyValue("operator", "SU");
                        PrepareForRecharge(dicAck, dicRequest, ref objAckStaus);
                    }
                    break;
                case "dth_tatasky":
                    {
                        dicAck.AddKeyValue("operator", "TS");
                        PrepareForRecharge(dicAck, dicRequest, ref objAckStaus);
                    }
                    break;
                case "dth_videocon":
                    {
                        dicAck.AddKeyValue("operator", "VT");
                        PrepareForRecharge(dicAck, dicRequest, ref objAckStaus);
                    }
                    break;
                #endregion

                #region DATACARD RECHARGE
                case "datacard_airtel":
                    {
                        dicAck.AddKeyValue("operator", "AR");
                        PrepareForRecharge(dicAck, dicRequest, ref objAckStaus);
                    }
                    break;
                case "datacard_bsnl":
                    {
                        dicAck.AddKeyValue("operator", "BT");
                        PrepareForRecharge(dicAck, dicRequest, ref objAckStaus);
                    }
                    break;
                case "datacard_mts":
                    {
                        dicAck.AddKeyValue("operator", "MT");
                        PrepareForRecharge(dicAck, dicRequest, ref objAckStaus);
                    }
                    break;
                case "datacard_idea":
                    {
                        dicAck.AddKeyValue("operator", "ID");
                        PrepareForRecharge(dicAck, dicRequest, ref objAckStaus);
                    }
                    break;
                case "datacard_reliance":
                    {
                        dicAck.AddKeyValue("operator", "RC");
                        PrepareForRecharge(dicAck, dicRequest, ref objAckStaus);
                    }
                    break;
                case "datacard_tata":
                    {
                        dicAck.AddKeyValue("operator", "TI");
                        PrepareForRecharge(dicAck, dicRequest, ref objAckStaus);
                    }
                    break;
                case "datacard_vodafone":
                    {
                        dicAck.AddKeyValue("operator", "VO");
                        PrepareForRecharge(dicAck, dicRequest, ref objAckStaus);
                    }
                    break;
                #endregion

                #region RTA Services
                case "rta":
                    {
                        #region RTA Fee Payment [60+ Services]
                        try
                        {
                            dicAck.AddKeyValue("number", dicRequest.GetValue("number").ToUpper());
                            dicAck.AddKeyValue("amount", dicRequest.GetValue("amount"));
                            dicAck.AddKeyValue("billRefNo", dicRequest.GetValue("reqid").ToUpper());
                            objAckStaus.ActualBillAmount = dicRequest.GetValue("amount");
                            objAckStaus.RespCode = "0";
                            objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                            objAckStaus.RespDesc = "success";
                            objAckStaus.Service = "eseva";
                            objAckStaus.Action = "12";
                            objAckStaus.ActionDesc = "RTAFEE";
                        }
                        catch (Exception ex)
                        {
                            TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + dicRequest.GetValue("dept").ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                            objAckStaus.RespCode = "1";
                            objAckStaus.RespDesc = "exception";
                        }
                        #endregion
                    }
                    break;
                case "rta_lifetax":
                    {
                        #region RTA LIFE TAX PAYMENT
                        try
                        {
                            dicAck.AddKeyValue("number", dicRequest.GetValue("number").ToUpper());
                            dicAck.AddKeyValue("amount", dicRequest.GetValue("amount"));
                            dicAck.AddKeyValue("billRefNo", dicRequest.GetValue("reqid").ToUpper());
                            objAckStaus.ActualBillAmount = dicRequest.GetValue("amount");
                            objAckStaus.RespCode = "0";
                            objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                            objAckStaus.RespDesc = "success";
                            objAckStaus.Service = "eseva";
                            objAckStaus.Action = "36";
                        }
                        catch (Exception ex)
                        {
                            TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + dicRequest.GetValue("dept").ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                            objAckStaus.RespCode = "1";
                            objAckStaus.RespDesc = "exception";
                        }
                        #endregion
                    }
                    break;
                #endregion
                case "hathway":
                    {
                        #region HATHWAY
                        try
                        {
                            dictionaryResp = HathWay_ViewBill(dicRequest);
                            if (dictionaryResp.GetValue("resCode") == "000")
                            {
                                dicAck.AddKeyValue("number", dicRequest.GetValue("accountno"));
                                dicAck.AddKeyValue("accountno", dictionaryResp.GetValue("accountno"));
                                dicAck.AddKeyValue("amount", dicRequest.GetValue("amount"));
                                dicAck.AddKeyValue("billrefno", dictionaryResp.GetValue("billRefNo"));
                                dicAck.AddKeyValue("hathwayMobileno", dictionaryResp.GetValue("mobileNo"));

                                objAckStaus.ActualBillAmount = dictionaryResp.GetValue("billamount");
                                objAckStaus.RespCode = "0";
                                objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                                objAckStaus.RespDesc = "success";
                                objAckStaus.Service = "eseva";
                                objAckStaus.Action = "84";
                                objAckStaus.ActionDesc = "HATHWAY";
                            }
                        }
                        catch (Exception ex)
                        {
                            TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + dicRequest.GetValue("dept").ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                            objAckStaus.RespCode = "1";
                            objAckStaus.RespDesc = "exception";
                        }
                        #endregion
                    }
                    break;
                default:
                    {
                        TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception GetAcknowledgementJSON()==> Invalid department:" + dicRequest.GetValue("dept").ToLower());
                        objAckStaus.RespCode = "1";
                        objAckStaus.RespDesc = "keyword not found";
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception GetAcknowledgementJSON(),Message=" + ex.Message + ", stack trace:" + ex.StackTrace);
            objAckStaus.RespCode = "1";
            objAckStaus.RespDesc = "exception";
        }
        finally
        {
            TextLog.Debug(strWorkSpaceId, "Ack_json", "Dept:" + dicRequest.GetValue("dept").ToLower() + ",mobileno:" + dicRequest.GetValue("mobileno") + ",channel:" + dicRequest.GetValue("channel") + ",strAckJson:" + objAckStaus.AckJson.ToStr());
            dictionaryResp = null;
            dicAck = null;
        }
        return objAckStaus;
    }

    private void PrepareForRecharge(Dictionary<String, String> dicAck, Dictionary<String, String> dicRequest, ref AckStatusBO objAckStaus)
    {
        try
        {
            if (string.IsNullOrEmpty(dicRequest.GetValue("number")))
            {
                TextLog.Exception(strWorkSpaceId, strErrorFileName, "PrepareForRecharge==>Invalid number ==>" + dicRequest.GetValue("number") + " for tid:" + dicRequest.GetValue("tid"));
                objAckStaus.RespCode = "1";
                objAckStaus.RespDesc = "invalid number";
                return;
            }
            if (dicRequest.GetValue("dept").ToLower().StartsWith("dth_"))
            {
                if ((dicRequest.GetValue("number").Length < General.GetConfigVal("DTH_NUMBER_MIN_LENGTH").ToInt()) || (dicRequest.GetValue("number").Length > General.GetConfigVal("DTH_NUMBER_MAX_LENGTH").ToInt()))
                {
                    TextLog.Exception(strWorkSpaceId, strErrorFileName, "PrepareForRecharge==>Invalid dth number ==>" + dicRequest.GetValue("number") + " for tid:" + dicRequest.GetValue("tid"));
                    objAckStaus.RespCode = "1";
                    objAckStaus.RespDesc = "invalid number";
                    return;
                }
            }
            else if (dicRequest.GetValue("dept").ToLower().StartsWith("datacard_") && (dicRequest.GetValue("number").Trim().Length != 10))
            {
                TextLog.Exception(strWorkSpaceId, strErrorFileName, "PrepareForRecharge==>Invalid data card number ==>" + dicRequest.GetValue("number") + " for tid:" + dicRequest.GetValue("tid"));
                objAckStaus.RespCode = "1";
                objAckStaus.RespDesc = "invalid number";
                return;
            }
            if (string.IsNullOrEmpty(dicRequest.GetValue("amount")))
            {
                TextLog.Exception(strWorkSpaceId, strErrorFileName, "PrepareForRecharge==> Invalid amount ==>" + dicRequest.GetValue("amount") + " for tid:" + dicRequest.GetValue("tid"));
                objAckStaus.RespCode = "1";
                objAckStaus.RespDesc = "invalid amount";
                return;
            }
            else
            {
                dicAck.AddKeyValue("tid", dicRequest.GetValue("tid"));
                dicAck.AddKeyValue("mobileNo", dicRequest.GetValue("mobileno"));
                dicAck.AddKeyValue("number", dicRequest.GetValue("number"));
                dicAck.AddKeyValue("amount", dicRequest.GetValue("amount"));
                objAckStaus.ActualBillAmount = dicRequest.GetValue("amount");
                objAckStaus.RespCode = "0";
                objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
                objAckStaus.RespDesc = "success";
                objAckStaus.Service = "martconnect";
                objAckStaus.Action = "2";
                objAckStaus.ActionDesc = "RECHARGE";
            }
        }
        catch (Exception ex)
        {
            TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception PrepareForRecharge, Exception=" + ex.ToStr());
            objAckStaus.RespCode = "1";
            objAckStaus.RespDesc = "exception";
        }
    }

    private void PrepareForPostPaid(Dictionary<String, String> dicAck, Dictionary<String, String> dicRequest, ref AckStatusBO objAckStaus)
    {
        try
        {
            if (string.IsNullOrEmpty(dicRequest.GetValue("number")))
            {
                TextLog.Exception(strWorkSpaceId, strErrorFileName, "PrepareForRecharge==>Invalid number ==>" + dicRequest.GetValue("number") + " for tid:" + dicRequest.GetValue("tid"));
                objAckStaus.RespCode = "1";
                objAckStaus.RespDesc = "invalid number";
            }
            else if (dicRequest.GetValue("number").Length != 10)
            {
                TextLog.Exception(strWorkSpaceId, strErrorFileName, "PrepareForRecharge==>Invalid number ==>" + dicRequest.GetValue("number") + " for tid:" + dicRequest.GetValue("tid"));
                objAckStaus.RespCode = "1";
                objAckStaus.RespDesc = "invalid number";
            }
            else if (string.IsNullOrEmpty(dicRequest.GetValue("amount")))
            {
                TextLog.Exception(strWorkSpaceId, strErrorFileName, "PrepareForRecharge==> Invalid amount ==>" + dicRequest.GetValue("amount") + " for tid:" + dicRequest.GetValue("tid"));
                objAckStaus.RespCode = "1";
                objAckStaus.RespDesc = "invalid amount";
            }
            dicAck.AddKeyValue("tid", dicRequest.GetValue("tid"));
            dicAck.AddKeyValue("mobileNo", dicRequest.GetValue("mobileno"));
            dicAck.AddKeyValue("number", dicRequest.GetValue("number"));
            dicAck.AddKeyValue("amount", dicRequest.GetValue("amount"));
            objAckStaus.ActualBillAmount = dicRequest.GetValue("amount");
            objAckStaus.RespCode = "0";
            objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
            objAckStaus.RespDesc = "success";
            objAckStaus.Service = "martconnect";
            objAckStaus.Action = "5";
            objAckStaus.ActionDesc = "POSTPAID";
        }
        catch (Exception ex)
        {
            TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception PrepareForPostPaid, Exception=" + ex.ToStr());
            objAckStaus.RespCode = "1";
            objAckStaus.RespDesc = "exception";
        }
    }

    public TWALLETBO PayThroughTWALLET(Dictionary<String, String> dicRequest)
    {
        TWALLETBO objBO = new TWALLETBO();
        try
        {
            string strParams = General.GetConfigVal("TWALLET_POSTDATA").Replace("!CUSTMOBNO!", dicRequest.GetValue("twalletmobileno"));
            AckStatusBO objAckStatus = GetAcknowledgementJSON(dicRequest);
            if (objAckStatus.RespCode == "0")
            {
                #region Call Twallet url
                #region Min Amount & Partial Amount Checks
                if (objAckStatus.ActualBillAmount.ToDouble() <= 0)
                {
                    objBO.RespCode = "2";
                    objBO.RespDesc = General.GetConfigVal("INVALID_BILL_AMOUNT_MSG").Replace("!AMOUNT!", objAckStatus.ActualBillAmount);
                    return objBO;
                }
                else if (Array.IndexOf(General.GetConfigVal("PARTIAL_PAY_NOTALLOWED_DEPTS").Split(','), dicRequest.GetValue("dept").ToUpper()) > -1)// check for partial amounts
                {
                    if (dicRequest.GetValue("amount").ToDouble()!= objAckStatus.ActualBillAmount.ToDouble())
                    {
                        objBO.RespCode = "3";
                        objBO.RespDesc = General.GetConfigVal("PARTIAL_AMOUNT_ERROR_MSG").Replace("!BILLAMOUNT!", objAckStatus.ActualBillAmount);
                        return objBO;
                    }
                }
                //if (objAckStatus.ActualBillAmount.ToDouble() < Convert.ToDouble(General.GetConfigVal("MIN_ALLOWED_AMOUNT")))
                //{
                //    TextLog.Exception(strWorkSpaceId, "payment_ex", "Invalid amount.:" + objAckStatus.ActualBillAmount.ToStr());
                //    objBO.RespCode = "4";
                //    objBO.RespDesc = General.GetConfigVal("INVALID_AMOUNT");
                //    return objBO;
                //}
                if (objAckStatus.ActualBillAmount.ToDouble() > Convert.ToDouble(General.GetConfigVal("MAX_ALLOWED_AMOUNT")))
                {
                    TextLog.Exception(strWorkSpaceId, "payment_ex", "Invalid amount.:" + objAckStatus.ActualBillAmount.ToStr());
                    objBO.RespCode = "5";
                    objBO.RespDesc = General.GetConfigVal("INVALID_AMOUNT");
                    return objBO;
                }
                #endregion
                strParams = strParams.Replace("!IMIPAYACK!", objAckStatus.AckJson).Replace("!CHANNEL!", dicRequest.GetValue("channel").ToUpper());
                objBO.TWalletAmount = dicRequest.GetValue("amount");

                strParams = strParams.Replace("!CUSTOTP!", dicRequest.GetValue("otp")).Replace("!AMOUNT!", dicRequest.GetValue("amount")).Replace("!TRANSID!", DateTime.Now.Ticks.ToString());
                strParams = strParams.Replace("!ACTION!", objAckStatus.Action.ToStr()).Replace("!SERVICE!", objAckStatus.Service.ToStr()).Replace("!DESC!", objAckStatus.ActionDesc.ToStr());
                String strPaymentResp = General.DoPostRequest(General.GetConfigVal("TWALLET_PAYMENT_URL"), strParams);
                TextLog.Exception(strWorkSpaceId, "twallet_payments", "url:" + General.GetConfigVal("TWALLET_PAYMENT_URL") + Environment.NewLine + "Request:" + strParams + Environment.NewLine + "Response:" + strPaymentResp);
                if (!string.IsNullOrEmpty(strPaymentResp))
                {
                    Dictionary<String, String> dicPaymentResp = new Dictionary<String, String>();
                    dicPaymentResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strPaymentResp);
                    if (dicPaymentResp != null && dicPaymentResp.Count > 0 && dicPaymentResp.GetValue("status") == "0")
                    {
                        #region Payment success
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
                        objBO.RespDesc = General.GetConfigVal("PAYMENT_SUCCESS_MSG").Replace("!AMOUNT!", strTxnAmount).Replace("!SERVICE!", objAckStatus.ActionDesc);
                        #endregion
                    }
                    else
                    {
                        objBO.RespCode = "1";
                        objBO.RespDesc = General.GetConfigVal("PAYMENT_FAIL_MSG");
                    }
                }
                else
                {
                    objBO.RespCode = "1";
                    objBO.RespDesc = General.GetConfigVal("PAYMENT_FAIL_MSG");
                }
                #endregion
            }
            else
            {
                objBO.RespCode = "1";
                objBO.RespDesc = General.GetConfigVal("EXCEPTION_MSG");
            }
        }
        catch (Exception ex)
        {
            TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception PayThroughTWALLET(),Message=" + ex.Message + ", stack trace:" + ex.StackTrace);
            objBO.RespCode = "1";
            objBO.RespDesc = General.GetConfigVal("EXCEPTION_MSG");
        }
        return objBO;
    }

    #region Not using
    #region Not Using
    //case "rta_lltestfee":
    //    {
    //        #region RTA LL TEST FEE
    //        try
    //        {
    //            dicAck.AddKeyValue("number", dicRequest.GetValue("number").ToUpper());
    //            dicAck.AddKeyValue("amount", dicRequest.GetValue("amount"));
    //            dicAck.AddKeyValue("billRefNo", dicRequest.GetValue("reqid").ToUpper());
    //            objAckStaus.ActualBillAmount = dicRequest.GetValue("amount");
    //            objAckStaus.RespCode = "0";
    //            objAckStaus.AckJson = JsonConvert.SerializeObject(dicAck);
    //            objAckStaus.RespDesc = "success";
    //            objAckStaus.Service = "eseva";
    //            objAckStaus.Action = "41";
    //            objAckStaus.ActionDesc = "RTALLRTESTFEE";
    //        }
    //        catch (Exception ex)
    //        {
    //            TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception in GetAcknowledgementJSON==>" + dicRequest.GetValue("dept").ToLower() + "==> Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
    //            objAckStaus.RespCode = "1";
    //            objAckStaus.RespDesc = "exception";
    //        }
    //        #endregion
    //    }
    //    break; 
    #endregion
    //private bool CheckAmount(String strPayAmount, String strBillAmount)
    //{
    //    try
    //    {
    //        if (Convert.ToDouble(strPayAmount) >= Convert.ToDouble(strBillAmount))
    //            return true;
    //        else
    //            return false;
    //    }
    //    catch (Exception ex)
    //    {
    //        return false;
    //    }
    //}
    //public Dictionary<String, String> RTA_LLTestFee(Dictionary<String, String> dicRequest)
    //{
    //    #region RTA LL TEST FEE
    //    try
    //    {
    //        if (string.IsNullOrEmpty(dicRequest.GetValue("number")))
    //        {
    //            TextLog.Exception("CHANNELINTERFACE", "RTA_EX", "Invlaid Pay Mobileno , tid:" + dicRequest.GetValue("tid") + ",number:" + dicRequest.GetValue("number"));
    //            dicResp.AddKeyValue("resCode", "3");
    //        }
    //        else
    //        {
    //            AddHeaders("rta_lltest_viewbill", ref htHeaders);
    //            strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"llrno\":\"" + dicRequest.GetValue("number").ToUpper() + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\"}";
    //            strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
    //            if (!string.IsNullOrEmpty(strResp))
    //                dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
    //            else
    //                dicResp.AddKeyValue("resCode", "1");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        TextLog.Exception("CHANNELINTERFACE", "RTALLTEST_EX", "Exception in ControlAPIWrapper-->RTA_LLTestFee,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
    //        dicResp.AddKeyValue("resCode", "2");
    //    }
    //    finally
    //    {
    //        TextLog.Debug("CHANNELINTERFACE", "RTALLTEST", "RTA_LLTestFee, strRequest:" + strRequest + ",strResp:" + strResp);
    //        htHeaders = null;
    //        if (dicResp == null || dicResp.Keys.Count == 0)
    //            dicResp.AddKeyValue("resCode", "1");
    //    }
    //    return dicResp;
    //    #endregion
    //}
    //public Dictionary<String, String> Prepaid_Recharge(Dictionary<String, String> dicRequest)
    //{
    //    #region Prepaid Recharge
    //    try
    //    {
    //        if (string.IsNullOrEmpty(dicRequest.GetValue("number")) || dicRequest.GetValue("number").Trim().Length < 10 || dicRequest.GetValue("number").Trim().Length > 12)
    //        {
    //            TextLog.Exception("CHANNELINTERFACE", "PREPAID_RECHARGE_EX", "Invlaid number , tid:" + dicRequest.GetValue("tid") + ",number:" + dicRequest.GetValue("number"));
    //            dicResp.AddKeyValue("resCode", "3");
    //        }
    //        else
    //        {
    //            AddHeaders("prepaid_recharge", ref htHeaders);
    //            strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"number\":\"" + dicRequest.GetValue("number") + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\",\"operator\":\"" + dicRequest.GetValue("operator").ToUpper() + "\",\"amount\":\"" + dicRequest.GetValue("amount") + "\"}";
    //            strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
    //            if (!string.IsNullOrEmpty(strResp))
    //                dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
    //            else
    //                dicResp.AddKeyValue("resCode", "1");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        TextLog.Exception("CHANNELINTERFACE", "PREPAID_RECHARGE_EX", "Exception in ControlAPIWrapper-->Prepaid_Recharge,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
    //        dicResp.AddKeyValue("resCode", "2");
    //    }
    //    finally
    //    {
    //        TextLog.Debug("CHANNELINTERFACE", "PREPAID_RECHARGE", "Prepaid_Recharge, strRequest:" + strRequest + ",strResp:" + strResp);
    //        htHeaders = null;
    //        if (dicResp == null || dicResp.Keys.Count == 0)
    //            dicResp.AddKeyValue("resCode", "1");
    //    }
    //    return dicResp;
    //    #endregion
    //}
    //public JObject Prepaid_GetOperators(Dictionary<String, String> dicRequest)
    //{
    //    #region Prepaid Get Operators
    //    JObject oJ = null;
    //    try
    //    {

    //        AddHeaders("prepaid_getoperators", ref htHeaders);
    //        strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\",\"type\":\"" + dicRequest.GetValue("type") + "\"}";
    //        strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
    //        if (!string.IsNullOrEmpty(strResp))
    //        {
    //            oJ = JObject.Parse(strResp);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        TextLog.Exception("CHANNELINTERFACE", "PREPAID_GETOPERATORS_EX", "Exception in ControlAPIWrapper-->Prepaid_GetOperators,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
    //    }
    //    finally
    //    {
    //        TextLog.Debug("CHANNELINTERFACE", "PREPAID_GETOPERATORS", "Prepaid_GetOperators, strRequest:" + strRequest + ",strResp:" + strResp);
    //        htHeaders = null;
    //    }
    //    return oJ;
    //    #endregion
    //}
    //public Dictionary<String, String> Prepaid_RechargeStatus(Dictionary<String, String> dicRequest)
    //{
    //    #region Prepaid Recharge Status
    //    try
    //    {
    //        if (string.IsNullOrEmpty(dicRequest.GetValue("transid")))
    //        {
    //            TextLog.Exception("CHANNELINTERFACE", "PREPAID_RECHARGE_STATUS_EX", "Invalid transid , tid:" + dicRequest.GetValue("tid") + ",transid:" + dicRequest.GetValue("transid"));
    //            dicResp.AddKeyValue("resCode", "3");
    //        }
    //        else
    //        {
    //            AddHeaders("prepaid_rechargestatus", ref htHeaders);
    //            strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"transid\":\"" + dicRequest.GetValue("transid") + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\"}";
    //            strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
    //            if (!string.IsNullOrEmpty(strResp))
    //                dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
    //            else
    //                dicResp.AddKeyValue("resCode", "1");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        TextLog.Exception("CHANNELINTERFACE", "PREPAID_RECHARGE_STATUS_EX", "Exception in ControlAPIWrapper-->Prepaid_Recharge,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
    //        dicResp.AddKeyValue("resCode", "2");
    //    }
    //    finally
    //    {
    //        TextLog.Debug("CHANNELINTERFACE", "PREPAID_RECHARGE_STATUS", "Prepaid_Recharge_Status, strRequest:" + strRequest + ",strResp:" + strResp);
    //        htHeaders = null;
    //        if (dicResp == null || dicResp.Keys.Count == 0)
    //            dicResp.AddKeyValue("resCode", "1");
    //    }
    //    return dicResp;
    //    #endregion
    //}

    //public Dictionary<String, String> Prepaid_ViewBalance(Dictionary<String, String> dicRequest)
    //{
    //    #region Prepaid Recharge Status
    //    try
    //    {
    //        if (string.IsNullOrEmpty(dicRequest.GetValue("mobileno")))
    //        {
    //            TextLog.Exception("CHANNELINTERFACE", "PREPAID_VIEWBALANCE_EX", "Invalid mobileNo , tid:" + dicRequest.GetValue("tid") + ",mobileNo:" + dicRequest.GetValue("mobileNo"));
    //            dicResp.AddKeyValue("resCode", "3");
    //        }
    //        else
    //        {
    //            AddHeaders("prepaid_viewbalance", ref htHeaders);
    //            strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\"}";
    //            strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
    //            if (!string.IsNullOrEmpty(strResp))
    //                dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
    //            else
    //                dicResp.AddKeyValue("resCode", "1");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        TextLog.Exception("CHANNELINTERFACE", "PREPAID_VIEWBALANCE_EX", "Exception in ControlAPIWrapper-->Prepaid_ViewBalance,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
    //        dicResp.AddKeyValue("resCode", "2");
    //    }
    //    finally
    //    {
    //        TextLog.Debug("CHANNELINTERFACE", "PREPAID_VIEWBALANCE", "Prepaid_ViewBalance, strRequest:" + strRequest + ",strResp:" + strResp);
    //        htHeaders = null;
    //        if (dicResp == null || dicResp.Keys.Count == 0)
    //            dicResp.AddKeyValue("resCode", "1");
    //    }
    //    return dicResp;
    //    #endregion
    //} 
    //public JObject TSSPDCL_GetDistricts(Dictionary<String, String> dicRequest)
    //{
    //    #region TSSPDCL_DISTRICTS
    //    JObject oJ = null;
    //    try
    //    {

    //        AddHeaders("tsspdcl_getdistricts", ref htHeaders);
    //        strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\"}";
    //        strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
    //        if (!string.IsNullOrEmpty(strResp))
    //        {
    //            oJ = JObject.Parse(strResp);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        TextLog.Exception("CHANNELINTERFACE", "TSSPDCL_EX", "Exception in ControlAPIWrapper-->Get Districts,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
    //    }
    //    finally
    //    {
    //        TextLog.Debug("CHANNELINTERFACE", "TSSPDCL", "Get Districts, strRequest:" + strRequest + ",strResp:" + strResp);
    //        htHeaders = null;
    //    }
    //    return oJ;
    //    #endregion
    //}

    //public JObject TSSPDCL_GetEROs(Dictionary<String, String> dicRequest)
    //{
    //    #region TSSPDCL_EROs
    //    JObject oJ = null;
    //    try
    //    {
    //        if (string.IsNullOrEmpty(dicRequest.GetValue("number")))
    //        {
    //            TextLog.Exception("CHANNELINTERFACE", "TSSPDCL_EX", "Invlaid District code , tid:" + dicRequest.GetValue("tid") + ",districtcode:" + dicRequest.GetValue("number"));
    //            dicResp.AddKeyValue("resCode", "3");
    //        }
    //        else
    //        {
    //            AddHeaders("tsspdcl_geteros", ref htHeaders);
    //            strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"number\":\"" + dicRequest.GetValue("number") + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\"}";
    //            strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
    //            if (!string.IsNullOrEmpty(strResp))
    //            {
    //                oJ = JObject.Parse(strResp);
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        TextLog.Exception("CHANNELINTERFACE", "TSSPDCL_EX", "Exception in ControlAPIWrapper-->Get Eros,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
    //    }
    //    finally
    //    {
    //        TextLog.Debug("CHANNELINTERFACE", "TSSPDCL", "Get EROs, strRequest:" + strRequest + ",strResp:" + strResp);
    //        htHeaders = null;
    //    }
    //    return oJ;
    //    #endregion
    //}

    //public Dictionary<String, String> TSSPDCL_ViewBill(Dictionary<String, String> dicRequest)
    //{
    //    #region TSSPDCL VIEW BILL
    //    try
    //    {
    //        if (string.IsNullOrEmpty(dicRequest.GetValue("number")))
    //        {
    //            TextLog.Exception("CHANNELINTERFACE", "TSSPDCL_EX", "Invlaid number , tid:" + dicRequest.GetValue("tid") + ",number:" + dicRequest.GetValue("number"));
    //            dicResp.AddKeyValue("resCode", "3");
    //        }
    //        else if (string.IsNullOrEmpty(dicRequest.GetValue("erocode")))
    //        {
    //            TextLog.Exception("CHANNELINTERFACE", "TTL_EX", "Invalid erocode , tid:" + dicRequest.GetValue("tid") + ",erocode:" + dicRequest.GetValue("erocode"));
    //            dicResp.AddKeyValue("resCode", "4");
    //        }
    //        else
    //        {
    //            AddHeaders("tsspdcl_viewbill", ref htHeaders);
    //            strRequest = "{\"tid\":\"" + dicRequest.GetValue("tid") + "\",\"mobileNo\":\"" + dicRequest.GetValue("mobileno") + "\",\"number\":\"" + dicRequest.GetValue("number") + "\",\"eroCode\":\"" + dicRequest.GetValue("erocode") + "\",\"Channel\":\"" + dicRequest.GetValue("channel") + "\"}";
    //            strResp = General.DoHttpWebRequest(strAPIURL, strRequest, strHttpMethod, strContentType, htHeaders, iWebReqTimeOut);
    //            if (!string.IsNullOrEmpty(strResp))
    //                dicResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strResp);
    //            else
    //                dicResp.AddKeyValue("resCode", "1");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        TextLog.Exception("CHANNELINTERFACE", "TSSPDCL_EX", "Exception in ControlAPIWrapper-->RTA ViewBill,strRequest:" + strRequest + ",strResp:" + strResp + ", Message:" + ex.Message);
    //        dicResp.AddKeyValue("resCode", "2");
    //    }
    //    finally
    //    {
    //        TextLog.Debug("CHANNELINTERFACE", "TSSPDCL", "TSSPDCL ViewBill, strRequest:" + strRequest + ",strResp:" + strResp);
    //        htHeaders = null;
    //        if (dicResp == null || dicResp.Keys.Count == 0)
    //            dicResp.AddKeyValue("resCode", "1");
    //    }
    //    return dicResp;
    //    #endregion
    //}


    //public IMPSBO PayThroughIMPS(Dictionary<String, String> dicRequest)
    //{
    //    IMPSBO objBO = new IMPSBO();
    //    try
    //    {
    //        string strParams = General.GetConfigVal("IMPS_POSTDATA").Replace("!CUSTMOBNO!", dicRequest.GetValue("mobileno"));
    //        String strAck = GetAcknowledgementJSON(dicRequest);
    //        if (string.IsNullOrEmpty(strAck))
    //        {
    //            objBO.RespCode = "1";
    //            return objBO;
    //        }
    //        strParams = strParams.Replace("!IMIPAYACK!", strAck).Replace("!CHANNEL!", dicRequest.GetValue("channel"));
    //        String strAmount = dicRequest.GetValue("amount");
    //        objBO.IMPSAmount = strAmount;
    //        strParams = strParams.Replace("!MMID!", dicRequest.GetValue("mmid")).Replace("!CUSTOTP!", dicRequest.GetValue("otp")).Replace("!AMOUNT!", strAmount).Replace("!TRANSID!", DateTime.Now.Ticks.ToString());
    //        String strPaymentResp = General.DoPostRequest(General.GetConfigVal("IMPS_PAYMENT_URL"), strParams);
    //        TextLog.Exception(strWorkSpaceId, "imps_hits", "url:" + General.GetConfigVal("IMPS_PAYMENT_URL") + Environment.NewLine + "Request:" + strParams + Environment.NewLine + "Response:" + strPaymentResp);
    //        if (!string.IsNullOrEmpty(strPaymentResp) && (!strPaymentResp.ToLower().Contains("transaction failed")))
    //        {
    //            /** PayGov Response Format:
    //                 * status=1&PAYMENT_REF_NO=635470847977014238&AdditionalInfo5=NA&paymentMode=IMM&ItemCode=IVRS&AuthStatus=0399&SettlementType=NA&AdditionalInfo6=NA&AdditionalInfo7=NA&BankReferenceNo=426711484456110149608886&TxnAmount=11.0&TxnReferenceNo=MIMM3495724277&CurrencyName=INR&ErrorStatus=NA&BankID=IMM&SecurityType=NA&SecurityID=NA&TxnDate=24-09-2014+11%3A46%3A33&SecurityPassword=NA&MerchantID=XNSKAMONE&ErrorDescription=Error+Occurred&BankMerchantID=NA&CustomerID=2393&AdditionalInfo4=NA&AdditionalInfo3=USSD&TxnType=06&AdditionalInfo2=GOV001&AdditionalInfo1=635470847977014238&tid=MIMM3495724277

    //            Uri myUri = new Uri("http://localhost/Default.aspx?" + strPaymentResp);
    //            String Status = HttpUtility.ParseQueryString(myUri.Query).Get("status");
    //            String strBankTransID = HttpUtility.ParseQueryString(myUri.Query).Get("tid");
    //            String strIMITransID = HttpUtility.ParseQueryString(myUri.Query).Get("AdditionalInfo5");
    //             */
    //            Dictionary<string, string> dicPaymentResp = new Dictionary<string, string>();
    //            NameValueCollection nvcQueryString = HttpUtility.ParseQueryString(strPaymentResp.ToLower());
    //            dicPaymentResp = nvcQueryString.AllKeys.ToDictionary(k => k, k => nvcQueryString[k]);
    //            if (dicPaymentResp != null && dicPaymentResp.Count > 0)
    //            {
    //                String Status = dicPaymentResp.GetValue("status");
    //                String strBankTransID = dicPaymentResp.GetValue("tid");
    //                String strIMITransID = dicPaymentResp.GetValue("AdditionalInfo5");
    //                if (Status == "0")
    //                {
    //                    objBO.TransNo = strBankTransID;
    //                    objBO.AuthCode = strBankTransID;
    //                    objBO.IMITransID = strIMITransID;
    //                    objBO.RespCode = "0";
    //                    return objBO;
    //                }
    //                else
    //                {
    //                    objBO.RespCode = "1";
    //                    return objBO;
    //                }
    //            }
    //            else
    //            {
    //                objBO.RespCode = "1";
    //                return objBO;
    //            }
    //        }
    //        else
    //        {
    //            objBO.RespCode = "1";
    //            return objBO;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception PayThroughIMPS(),Message=" + ex.Message + ", stack trace:" + ex.StackTrace);
    //        objBO.RespCode = "1";
    //        return objBO;
    //    }
    //}

    //public TWALLETBO PayThroughTWALLET(Dictionary<String, String> dicRequest)
    //{
    //    TWALLETBO objBO = new TWALLETBO();
    //    try
    //    {
    //        string strParams = General.GetConfigVal("TWALLET_POSTDATA").Replace("!CUSTMOBNO!", dicRequest.GetValue("twalletmobileno"));
    //        String strAck = GetAcknowledgementJSON(dicRequest);
    //        if (string.IsNullOrEmpty(strAck))
    //        {
    //            objBO.RespCode = "1";
    //            return objBO;
    //        }

    //        strParams = strParams.Replace("!IMIPAYACK!", strAck).Replace("!CHANNEL!", dicRequest.GetValue("channel"));
    //        String strAmount = dicRequest.GetValue("amount");
    //        objBO.TWalletAmount = strAmount;
    //        strParams = strParams.Replace("!CUSTOTP!", dicRequest.GetValue("otp")).Replace("!AMOUNT!", strAmount).Replace("!TRANSID!", DateTime.Now.Ticks.ToString());
    //        String strPaymentResp = General.DoPostRequest(General.GetConfigVal("TWALLET_PAYMENT_URL"), strParams);
    //        TextLog.Exception(strWorkSpaceId, "twallet_hits", "url:" + General.GetConfigVal("TWALLET_PAYMENT_URL") + Environment.NewLine + "Request:" + strParams + Environment.NewLine + "Response:" + strPaymentResp);
    //        if (!string.IsNullOrEmpty(strPaymentResp) && (!strPaymentResp.ToLower().Contains("transaction failed")) && (!strPaymentResp.ToLower().Contains("missing parameter")) && (!strPaymentResp.ToLower().Contains("not found|")))
    //        {
    //            Dictionary<string, string> dicPaymentResp = new Dictionary<string, string>();
    //            if (!string.IsNullOrEmpty(strPaymentResp))
    //            {
    //                dicPaymentResp = JsonConvert.DeserializeObject<Dictionary<String, String>>(strPaymentResp);
    //            }
    //            if (dicPaymentResp != null && dicPaymentResp.Count > 0)
    //            {
    //                String ResponseCode = dicPaymentResp.GetValue("Response_Code");
    //                String strMessage = dicPaymentResp.GetValue("Message");
    //                String strUniqueId = dicPaymentResp.GetValue("UniqueID");
    //                if (ResponseCode == "1980")
    //                {
    //                    objBO.TransNo = strUniqueId;
    //                    objBO.AuthCode = strUniqueId;
    //                    objBO.IMITransID = strUniqueId;
    //                    objBO.RespCode = "0";
    //                    return objBO;
    //                }
    //                else
    //                {
    //                    objBO.RespCode = "1";
    //                    objBO.RespDesc = General.GetConfigVal("PAYMENT_FAIL_MSG");
    //                    return objBO;
    //                }
    //            }
    //            else
    //            {
    //                objBO.RespCode = "1";
    //                return objBO;
    //            }
    //        }
    //        else
    //        {
    //            objBO.RespCode = "1";
    //            return objBO;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        TextLog.Exception(strWorkSpaceId, strErrorFileName, "Exception PayThroughTWALLET(),Message=" + ex.Message + ", stack trace:" + ex.StackTrace);
    //        objBO.RespCode = "1";
    //        return objBO;
    //    }
    //}

    #endregion
}