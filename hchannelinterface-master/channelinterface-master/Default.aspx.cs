using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using Newtonsoft.Json;
using System.Xml;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Cryptography;
using IMI.Logger;

public partial class _Default : System.Web.UI.Page
{
    // 0-success,1-fail,2-exception or timeouts
    String strRequest = String.Empty, strMobileNo = String.Empty, strChannel = String.Empty;
    String strService = String.Empty, strNoticeSeq = String.Empty;
    String strPager = String.Empty, strLanguage = String.Empty;
    String strDept = String.Empty;
    String strActualBillAmount = String.Empty;
    String strWorkSpaceId = "channelinterface";
    private const String CHANNEL = "channel";
    private const String MOBILENO = "mobileno";
    protected void Page_Load(object sender, EventArgs e)
    {
        String strRespCode = String.Empty;
        ControlAPIWrapper oWrapper = new ControlAPIWrapper();
        Dictionary<String, String> varDictionary = new Dictionary<String, String>();
        Dictionary<String, String> dicResp = new Dictionary<String, String>();
        string hostName = Dns.GetHostName();
        string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
        RijndaelManaged sessionKey = new RijndaelManaged();
        string client_url = General.GetConfigVal("CLIENT_URL");
        try
        {
            strRequest = Request.QueryString.ToString();
            NameValueCollection nvcQueryString = HttpUtility.ParseQueryString(Request.QueryString.ToString().ToLower());
            varDictionary = nvcQueryString.AllKeys.ToDictionary(k => k, k => nvcQueryString[k]);
            if (!varDictionary.ContainsKey(MOBILENO))
            {
                TextLog.Exception(strWorkSpaceId, "Default_ex", "Mobile no is empty.");
                strRespCode = "1";
                return;
            }
            else if (!varDictionary.ContainsKey(CHANNEL))
            {
                TextLog.Exception(strWorkSpaceId, "Default_ex", "channel is empty.");
                strRespCode = "1";
                return;
            }
            varDictionary.TryGetValue(CHANNEL, out strChannel);
            varDictionary.TryGetValue(MOBILENO, out strMobileNo);
            if (strMobileNo.Length > 10)
            {
                strMobileNo = strMobileNo.Substring(strMobileNo.Length - 10, 10);
                varDictionary.EditKeyValue(MOBILENO, strMobileNo);
            }

            strService = Request["service"] != null ? Request["service"].ToString() : "";
            strDept = Request["dept"] != null ? Request["dept"].ToString() : "";
            strActualBillAmount = Request["billamount"] != null ? Request["billamount"].ToString() : "";
            #region commented
            // strNoticeSeq = Request["noticeseq"] != null ? Request["noticeseq"].ToString() : "";
            // strPager = Request["pager"] != null ? Request["pager"].ToString() : "0";

            //            //{
            //  "operators": [
            //    {
            //      "code": "AT",
            //      "name": "AIRTEL TV",
            //      "length": "10",
            //    },
            //    {
            //      "code": "BI",
            //      "name": "BIG TV",
            //      "length": "12",
            //    },
            //    {
            //      "code": "DI",
            //      "name": "DISH TV",
            //      "length": "10|11",
            //    },
            //    {
            //      "code": "SU",
            //      "name": "SUN DIRECT",
            //      "length": "11",
            //    },
            //    {
            //      "code": "TS",
            //      "name": "TATASKY TV",
            //      "length": "10",
            //    },
            //    {
            //      "code": "VT",
            //      "name": "VIDEOCOND2H",
            //      "length": "",
            //    }
            //  ],
            //  "resCode": "000",
            //  "resDesc": "Success"
            //}

            //            {
            //  "operators": [
            //    {
            //      "code": "AR",
            //      "name": "Airtel",
            //      "length": "",
            //    },
            //    {
            //      "code": "BT",
            //      "name": "BSNL",
            //      "length": "",
            //    },
            //    {
            //      "code": "ID",
            //      "name": "Idea",
            //      "length": "",
            //    },
            //    {
            //      "code": "MT",
            //      "name": "MTS",
            //      "length": "",
            //    },
            //    {
            //      "code": "RC",
            //      "name": "Reliance",
            //      "length": "",
            //    },
            //    {
            //      "code": "TI",
            //      "name": "Tata",
            //      "length": "",
            //    },
            //    {
            //      "code": "VO",
            //      "name": "Vodafone",
            //      "length": "",
            //    }
            //  ],
            //  "resCode": "000",
            //  "resDesc": "Success"
            //} 
            #endregion
            if (string.IsNullOrEmpty(varDictionary.GetValue("tid")))
                varDictionary.EditKeyValue("tid", DateTime.Now.Ticks.ToStr());

            #region IVR
            if (strChannel.ToUpper().Trim() == "IVR")
            {
                Response.ContentType = "text/dtmf";
                Response.Charset = "UTF-8";
                switch (strService.ToLower().Trim())
                {
                    case "user_check":
                        {
                            #region CHECK USER EXISTANCE
                            try
                            {
                                dicResp = oWrapper.User_Check_Existance(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    strRespCode = "0";
                                    string lang = dicResp.GetValue("language");
                                    if (string.IsNullOrEmpty(lang))
                                        lang = "english";
                                    else if (lang.ToLower() == "en")
                                        lang = "english";
                                    else if (lang.ToLower() == "te")
                                        lang = "telugu";
                                    Response.AddHeader("X-IMI-IVRS-LANG", lang);// if new user regitered by lang will be english
                                }
                                else if (dicResp.GetValue("resCode") == "3")
                                {
                                    strRespCode = "3";
                                    Response.AddHeader("X-IMI-IVRS-LANG", "english");
                                }
                                else
                                    strRespCode = "1";
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in user_check, Message:" + ex.Message + ", stack trace:" + ex.StackTrace);
                                strRespCode = "1";
                            }
                            #endregion
                        }
                        break;
                    case "user_update_lang":
                        {
                            #region UPDATE PREFERRED LANGUAGE
                            try
                            {
                                dicResp = oWrapper.User_Update_PreferredLang(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                    strRespCode = "0";
                                else
                                    strRespCode = "1";
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in user_update_lang, Message:" + ex.Message + ", stack trace:" + ex.StackTrace);
                                strRespCode = "1";
                            }
                            #endregion
                        }
                        break;
                    case "airtel_viewbill":
                        {
                            #region AIRTEL VIEW BILL
                            try
                            {
                                dicResp = oWrapper.Airtel_ViewBill(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    //dicResp.EditKeyValue("billAmount", "98.45");// need to delete after testing
                                    Response.AddHeader("X-IMI-IVRS-ACCNO", dicResp.GetValue("accountNo"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMT", dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMTINRUPEES", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[0] : dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMTINPAISE", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[1] : String.Empty);
                                    Response.AddHeader("X-IMI-IVRS-NAME", dicResp.GetValue("consumerName"));
                                    Response.AddHeader("X-IMI-IVRS-REQID", dicResp.GetValue("reqId"));
                                    Response.AddHeader("X-IMI-IVRS-RESPCODE", dicResp.GetValue("resCode"));
                                    if (dicResp.GetValue("billAmount").ToDouble() <= 0)// if amount <=0
                                        strRespCode = "4";
                                    else
                                        strRespCode = "0";
                                }
                                else
                                    strRespCode = "1";// dicResp.GetValue("resCode");
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "2";
                            }
                            #endregion
                        }
                        break;
                    case "airtel_landline_viewbill":
                        {
                            #region AIRTEL LANDLINE VIEW BILL
                            try
                            {
                                dicResp = oWrapper.Airtel_Landline_ViewBill(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    Response.AddHeader("X-IMI-IVRS-ACCNO", dicResp.GetValue("accountNo"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMT", dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMTINRUPEES", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[0] : dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMTINPAISE", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[1] : String.Empty);
                                    Response.AddHeader("X-IMI-IVRS-NAME", dicResp.GetValue("consumerName"));
                                    Response.AddHeader("X-IMI-IVRS-REQID", dicResp.GetValue("reqId"));
                                    Response.AddHeader("X-IMI-IVRS-RESPCODE", dicResp.GetValue("resCode"));
                                    if (dicResp.GetValue("billAmount").ToInt() <= 0)// if amount <=0
                                        strRespCode = "4";
                                    else
                                        strRespCode = "0";
                                }
                                else
                                    strRespCode = "1";// dicResp.GetValue("resCode");
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "2";
                            }
                            #endregion
                        }
                        break;
                    case "idea_viewbill":
                        {
                            #region IDEA VIEW BILL
                            try
                            {
                                dicResp = oWrapper.Idea_ViewBill(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    //dicResp.EditKeyValue("billAmount", "10.00");// need to delete after testing
                                    Response.AddHeader("X-IMI-IVRS-ACCNO", dicResp.GetValue("accountNo"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMT", dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMTINRUPEES", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[0] : dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMTINPAISE", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[1] : String.Empty);
                                    Response.AddHeader("X-IMI-IVRS-NAME", dicResp.GetValue("consumerName"));
                                    Response.AddHeader("X-IMI-IVRS-REQID", dicResp.GetValue("reqId"));
                                    //Response.AddHeader("X-IMI-IVRS-ADDRESS1", dicResp.GetValue("address1"));
                                    //Response.AddHeader("X-IMI-IVRS-ADDRESS2", dicResp.GetValue("address2"));
                                    //Response.AddHeader("X-IMI-IVRS-ADDRESS3", dicResp.GetValue("address3"));
                                    Response.AddHeader("X-IMI-IVRS-RESPCODE", dicResp.GetValue("resCode"));
                                    if (dicResp.GetValue("billAmount").ToDouble() <= 0)// if amount <=0
                                        strRespCode = "4";
                                    else
                                        strRespCode = "0";
                                }
                                else
                                    strRespCode = "1";// dicResp.GetValue("resCode");
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "1";
                            }
                            #endregion
                        }
                        break;
                    case "ttl_viewbill":
                        {
                            #region TTL VIEW BILL
                            try
                            {
                                dicResp = oWrapper.TTL_ViewBill(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    // dicResp.EditKeyValue("billAmount", "10.00");// need to delete after testing
                                    Response.AddHeader("X-IMI-IVRS-ACCNO", dicResp.GetValue("accountNo"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMT", dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-IVRS-NAME", dicResp.GetValue("consumerName"));
                                    Response.AddHeader("X-IMI-IVRS-REQID", dicResp.GetValue("reqId"));
                                    Response.AddHeader("X-IMI-IVRS-MOBILENO", dicResp.GetValue("mobileNo"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMTINRUPEES", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[0] : dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMTINPAISE", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[1] : String.Empty);
                                    Response.AddHeader("X-IMI-IVRS-RESDESC", dicResp.GetValue("resDesc"));
                                    Response.AddHeader("X-IMI-IVRS-RESPCODE", dicResp.GetValue("resCode"));
                                    if (dicResp.GetValue("billAmount").ToDouble() <= 0)// if amount <=0
                                        strRespCode = "4";
                                    else
                                        strRespCode = "0";
                                }
                                else
                                    strRespCode = "1";// dicResp.GetValue("resCode");
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "2";
                            }
                            #endregion
                        }
                        break;
                    case "bsnl_viewbill":
                        {
                            #region BSNL POSTPAID & LANDLINE VIEW BILL
                            try
                            {
                                dicResp = oWrapper.Bsnl_ViewBill(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    //dicResp.EditKeyValue("billAmount", "98.45");// need to delete after testing
                                    Response.AddHeader("X-IMI-IVRS-ACCNO", dicResp.GetValue("accountNo"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMT", dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMTINRUPEES", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[0] : dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMTINPAISE", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[1] : String.Empty);
                                    Response.AddHeader("X-IMI-IVRS-NAME", dicResp.GetValue("consumerName"));
                                    Response.AddHeader("X-IMI-IVRS-REQID", dicResp.GetValue("reqId"));
                                    Response.AddHeader("X-IMI-IVRS-RESPCODE", dicResp.GetValue("resCode"));
                                    if (dicResp.GetValue("billAmount").ToDouble() <= 0)// if amount <=0
                                        strRespCode = "4";
                                    else
                                        strRespCode = "0";
                                }
                                else
                                    strRespCode = "1";// dicResp.GetValue("resCode");
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "2";
                            }
                            #endregion
                        }
                        break;
                    case "act_viewbill":
                        {
                            #region ACT VIEW BILL
                            try
                            {
                                dicResp = oWrapper.Act_ViewBill(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    //dicResp.EditKeyValue("billAmount", "403.34");// need to delete after testing
                                    Response.AddHeader("X-IMI-IVRS-ACCNO", dicResp.GetValue("accountNo"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMT", dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMTINRUPEES", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[0] : dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMTINPAISE", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[1] : String.Empty);
                                    Response.AddHeader("X-IMI-IVRS-BILLNO", dicResp.GetValue("billNo"));
                                    Response.AddHeader("X-IMI-IVRS-BILLDATE", dicResp.GetValue("billDate"));
                                    Response.AddHeader("X-IMI-IVRS-DISTRICT", dicResp.GetValue("district"));
                                    Response.AddHeader("X-IMI-IVRS-ERRORCD", dicResp.GetValue("errCD"));
                                    Response.AddHeader("X-IMI-IVRS-PERIOD", dicResp.GetValue("period"));
                                    Response.AddHeader("X-IMI-IVRS-REGION", dicResp.GetValue("region"));
                                    Response.AddHeader("X-IMI-IVRS-STATE", dicResp.GetValue("state"));
                                    Response.AddHeader("X-IMI-IVRS-TRANSNO", dicResp.GetValue("transNo"));
                                    Response.AddHeader("X-IMI-IVRS-NAME", dicResp.GetValue("customerName"));
                                    Response.AddHeader("X-IMI-IVRS-REQID", dicResp.GetValue("reqId"));
                                    Response.AddHeader("X-IMI-IVRS-ADDRESS1", dicResp.GetValue("address1"));
                                    Response.AddHeader("X-IMI-IVRS-ADDRESS2", dicResp.GetValue("address2"));
                                    Response.AddHeader("X-IMI-IVRS-ADDRESS3", dicResp.GetValue("address3"));
                                    Response.AddHeader("X-IMI-IVRS-RESPCODE", dicResp.GetValue("resCode"));
                                    if (dicResp.GetValue("billAmount").ToDouble() <= 0)// if amount <=0
                                        strRespCode = "4";
                                    else
                                        strRespCode = "0";
                                }
                                else
                                    strRespCode = "1";
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "2";
                            }
                            #endregion
                        }
                        break;
                    case "hmwssb_viewbill":
                        {
                            #region HMWSSB VIEW BILL
                            try
                            {
                                dicResp = oWrapper.HMWSSB_ViewBill(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    Response.AddHeader("X-IMI-IVRS-CANNO", dicResp.GetValue("canNo"));
                                    Response.AddHeader("X-IMI-IVRS-ADDRESS", dicResp.GetValue("address"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMT", dicResp.GetValue("amount"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMTINRUPEES", dicResp.GetValue("amount").Contains(".") ? dicResp.GetValue("amount").Split('.')[0] : dicResp.GetValue("amount"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMTINPAISE", dicResp.GetValue("amount").Contains(".") ? dicResp.GetValue("amount").Split('.')[1] : String.Empty);
                                    Response.AddHeader("X-IMI-IVRS-NAME", dicResp.GetValue("name"));
                                    Response.AddHeader("X-IMI-IVRS-REQID", dicResp.GetValue("reqId"));
                                    Response.AddHeader("X-IMI-IVRS-RESPCODE", dicResp.GetValue("resCode"));
                                    if (dicResp.GetValue("amount").ToDouble() <= 0)// if amount <=0
                                        strRespCode = "4";
                                    else
                                        strRespCode = "0";
                                }
                                else
                                    strRespCode = "1";// dicResp.GetValue("resCode");
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in hmwssb_viewbill, Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "2";
                            }
                            #endregion
                        }
                        break;
                    case "hathway_viewbill":
                        {
                            #region HATH VIEW BILL
                            try
                            {
                                dicResp = oWrapper.HathWay_ViewBill(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    //dicResp.EditKeyValue("billAmount", "403.34");// need to delete after testing
                                    Response.AddHeader("X-IMI-IVRS-ACCNO", dicResp.GetValue("accountno"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMT", dicResp.GetValue("billamount"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMTINRUPEES", dicResp.GetValue("billamount").Contains(".") ? dicResp.GetValue("billamount").Split('.')[0] : dicResp.GetValue("billamount"));
                                    Response.AddHeader("X-IMI-IVRS-BILLAMTINPAISE", dicResp.GetValue("billamount").Contains(".") ? dicResp.GetValue("billamount").Split('.')[1] : String.Empty);
                                    Response.AddHeader("X-IMI-IVRS-BILLNO", dicResp.GetValue("billNo"));
                                    Response.AddHeader("X-IMI-IVRS-BILLDATE", dicResp.GetValue("billDate"));
                                    Response.AddHeader("X-IMI-IVRS-PERIOD", dicResp.GetValue("period"));
                                    Response.AddHeader("X-IMI-IVRS-NAME", dicResp.GetValue("consumername"));
                                    Response.AddHeader("X-IMI-IVRS-REQID", dicResp.GetValue("billRefNo"));
                                    Response.AddHeader("X-IMI-IVRS-HWMOBNO", dicResp.GetValue("mobileNo"));
                                    Response.AddHeader("X-IMI-IVRS-ADDRESS", dicResp.GetValue("address"));
                                    Response.AddHeader("X-IMI-IVRS-RESPCODE", dicResp.GetValue("resCode"));
                                    if (dicResp.GetValue("billamount").ToDouble() <= 0)// if amount <=0
                                        strRespCode = "4";
                                    else
                                        strRespCode = "0";
                                }
                                else
                                    strRespCode = "1";
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "2";
                            }
                            #endregion
                        }
                        break;
                    #region Payment related

                    case "check_dthnumber":
                        {
                            #region CHECK DTH SUBSCIPTION NUMBER LENGTH
                            if ((varDictionary.GetValue("number").Length < General.GetConfigVal("DTH_NUMBER_MIN_LENGTH").ToInt()) || (varDictionary.GetValue("number").Length > General.GetConfigVal("DTH_NUMBER_MAX_LENGTH").ToInt()))
                            {
                                TextLog.Exception(strWorkSpaceId, "dth_number", "Invalid dth number" + varDictionary.GetValue("number") + " for tid:" + varDictionary.GetValue("tid"));
                                strRespCode = "1";
                            }
                            else
                                strRespCode = "0";
                            #endregion
                        }
                        break;
                    case "validateamount"://validate amount is partial
                        {
                            #region VALIDATE AMOUNT
                            try
                            {
                                if (Convert.ToDouble(varDictionary.GetValue("amount")) >= Convert.ToDouble(varDictionary.GetValue("billamount")))
                                    strRespCode = "0";
                                else
                                    strRespCode = "1";
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "1";
                            }
                            #endregion
                        }
                        break;
                    case "validateminamount":
                        {
                            #region Validate Min & Max Amount For Post paid bills & utility bills
                            try
                            {
                                if (Convert.ToDouble(varDictionary.GetValue("amount")) < Convert.ToDouble(General.GetConfigVal("MIN_ALLOWED_AMOUNT")))
                                    strRespCode = "1";
                                else if (Convert.ToDouble(varDictionary.GetValue("amount")) > Convert.ToDouble(General.GetConfigVal("MAX_ALLOWED_AMOUNT")))
                                    strRespCode = "2";
                                else
                                    strRespCode = "0";
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "1";
                            }
                            #endregion
                        }
                        break;
                    case "validateminmaxamount":
                        {
                            #region Validate Min & Max Amount for prepaid recharges
                            try
                            {
                                if (Convert.ToDouble(varDictionary.GetValue("amount")) < Convert.ToDouble(General.GetConfigVal("MIN_AMOUNT_RECHARGE")))
                                    strRespCode = "1";
                                else if (Convert.ToDouble(varDictionary.GetValue("amount")) > Convert.ToDouble(General.GetConfigVal("MAX_ALLOWED_AMOUNT")))
                                    strRespCode = "2";
                                else
                                    strRespCode = "0";
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "1";
                            }
                            #endregion
                        }
                        break;
                    case "twalletgenotp":
                        {
                            #region GENERATE OTP
                            Dictionary<string, string> dicOTPResp = oWrapper.GenerateTwalletOTP(varDictionary);
                            if (dicOTPResp != null && dicOTPResp.GetValue("Response_Code") == "1680")
                                strRespCode = "0";
                            else
                                strRespCode = "1";
                            #endregion
                        }
                        break;
                    case "twalletpayment":
                        {
                            #region T-WALLET
                            try
                            {
                                if (Array.IndexOf(General.GetConfigVal("MIN_AMOUNT_1_DEPTS").ToLower().Split(','), varDictionary.GetValue("dept").ToLower()) > -1)
                                {
                                    if (Convert.ToDouble(varDictionary.GetValue("amount")) < Convert.ToDouble(General.GetConfigVal("MIN_AMOUNT_RECHARGE")))
                                    {
                                        TextLog.Exception(strWorkSpaceId, "payment_ex", "Invalid amount found: channel:" + strChannel + ", Mobileno:" + strMobileNo + "Amount:" + varDictionary.GetValue("amount"));
                                        strRespCode = "4";
                                        return;
                                    }
                                }
                                else if (Array.IndexOf(General.GetConfigVal("MIN_AMOUNT_10_DEPTS").ToLower().Split(','), varDictionary.GetValue("dept").ToLower()) > -1)
                                {
                                    if (Convert.ToDouble(varDictionary.GetValue("amount")) < Convert.ToDouble(General.GetConfigVal("MIN_AMOUNT_BILLPAYMENTS")))
                                    {
                                        TextLog.Exception(strWorkSpaceId, "payment_ex", "Invalid amount found: channel:" + strChannel + ", Mobileno:" + strMobileNo + "Amount:" + varDictionary.GetValue("amount"));
                                        strRespCode = "4";
                                        return;
                                    }
                                }
                                if (Convert.ToDouble(varDictionary.GetValue("amount")) > Convert.ToDouble(General.GetConfigVal("MAX_ALLOWED_AMOUNT")))
                                {
                                    TextLog.Exception(strWorkSpaceId, "payment_ex", "Invalid amount found: channel:" + strChannel + ", Mobileno:" + strMobileNo + "Amount:" + varDictionary.GetValue("amount"));
                                    strRespCode = "5";
                                    return;
                                }
                                TWALLETBO objBO = oWrapper.PayThroughTWALLET(varDictionary);
                                if (objBO.RespCode == "0")
                                {
                                    Response.AddHeader("X-IMI-IVRS-TRANSNO", objBO.TransNo.ToStr());
                                    Response.AddHeader("X-IMI-IVRS-AUTHCODE", objBO.AuthCode.ToStr());
                                    Response.AddHeader("X-IMI-IVRS-IMITRANSID", objBO.IMITransID.ToStr());
                                    Response.AddHeader("X-IMI-IVRS-IMPSAMOUNT", objBO.TWalletAmount.ToStr());
                                    Response.AddHeader("X-IMI-IVRS-RESPDESC", objBO.RespDesc.ToStr());
                                    strRespCode = "0";
                                }
                                else
                                {
                                    strRespCode = objBO.RespCode.ToStr();
                                }

                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                            }
                            #endregion
                        }
                        break;
                    #endregion

                    #region Not using
                    //case "validatemmmid":
                    //case "validatemmid":
                    //    {
                    //        #region Validate MMID or MMMID
                    //        if (varDictionary.GetValue("mmid").StartsWith("9") && varDictionary.GetValue("mmid").Length == 7)
                    //            strRespCode = "0";
                    //        else
                    //            strRespCode = "1";
                    //        #endregion
                    //    }
                    //    break;
                    //case "imps":
                    //    {
                    //        #region IMPS
                    //        try
                    //        {
                    //            IMPSBO objBO = oWrapper.PayThroughIMPS(varDictionary);
                    //            if (objBO.RespCode == "0")
                    //            {
                    //                Response.AddHeader("X-IMI-IVRS-TRANSNO", objBO.TransNo.ToStr());
                    //                Response.AddHeader("X-IMI-IVRS-AUTHCODE", objBO.AuthCode.ToStr());
                    //                Response.AddHeader("X-IMI-IVRS-IMITRANSID", objBO.IMITransID.ToStr());
                    //                Response.AddHeader("X-IMI-IVRS-IMPSAMOUNT", objBO.IMPSAmount.ToStr());
                    //                strRespCode = "0";
                    //            }
                    //            else
                    //            {
                    //                strRespCode = objBO.RespCode.ToStr();
                    //            }
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                    //        }
                    //        #endregion
                    //    }
                    //    break;


                    //case "rta_viewbill":
                    //    {
                    //        #region RTA VIEW BILL
                    //        try
                    //        {
                    //            dicResp = oWrapper.RTA_ViewBill(varDictionary);
                    //            if (dicResp.GetValue("resCode") == "000")
                    //            {
                    //                Response.AddHeader("X-IMI-IVRS-APPFEE", dicResp.GetValue("appFee"));
                    //                Response.AddHeader("X-IMI-IVRS-APPLICATIONNO", dicResp.GetValue("applicationNo"));
                    //                Response.AddHeader("X-IMI-IVRS-CARDFEE", dicResp.GetValue("cardFee"));
                    //                Response.AddHeader("X-IMI-IVRS-DEPTTRANSID", dicResp.GetValue("deptTransId"));
                    //                Response.AddHeader("X-IMI-IVRS-DOCNO", dicResp.GetValue("docNo"));
                    //                Response.AddHeader("X-IMI-IVRS-NAME", dicResp.GetValue("name"));
                    //                Response.AddHeader("X-IMI-IVRS-OFFICECD", dicResp.GetValue("officeCd"));
                    //                Response.AddHeader("X-IMI-IVRS-REQID", dicResp.GetValue("reqId"));
                    //                Response.AddHeader("X-IMI-IVRS-RTAAMOUNT", dicResp.GetValue("rtaAmount"));
                    //                Response.AddHeader("X-IMI-IVRS-SLOTDATE", dicResp.GetValue("slotDate"));
                    //                Response.AddHeader("X-IMI-IVRS-SLOTTIME", dicResp.GetValue("slotTime"));
                    //                Response.AddHeader("X-IMI-IVRS-SERVICEDESC", dicResp.GetValue("serviceDesc"));
                    //                Response.AddHeader("X-IMI-IVRS-TOTAMT", dicResp.GetValue("totAmt"));
                    //                Response.AddHeader("X-IMI-IVRS-TOTAMTINRUPEES", dicResp.GetValue("totAmt").Contains(".") ? dicResp.GetValue("totAmt").Split('.')[0] : dicResp.GetValue("totAmt"));
                    //                Response.AddHeader("X-IMI-IVRS-TOTAMTINPAISE", dicResp.GetValue("totAmt").Contains(".") ? dicResp.GetValue("totAmt").Split('.')[1] : String.Empty);
                    //                Response.AddHeader("X-IMI-IVRS-USERCHARGS", dicResp.GetValue("userChargs"));
                    //                Response.AddHeader("X-IMI-IVRS-RESDESC", dicResp.GetValue("resDesc"));
                    //                Response.AddHeader("X-IMI-IVRS-RESPCODE", dicResp.GetValue("resCode"));
                    //                if (dicResp.GetValue("totAmt").ToInt() <= 0)// if amount <=0
                    //                    strRespCode = "4";
                    //                else
                    //                    strRespCode = "0";
                    //            }
                    //            else
                    //                strRespCode = "1";// dicResp.GetValue("resCode");
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                    //            strRespCode = "2";
                    //        }
                    //        #endregion
                    //    }
                    //    break;
                    //case "prepaid_getoperators":
                    //    {
                    //        #region PREPAID GET OPERATORS
                    //        try
                    //        {
                    //            JObject jObj = oWrapper.Prepaid_GetOperators(varDictionary);
                    //            if (jObj != null && jObj["resCode"].ToStr() == "000")
                    //            {
                    //                JArray objOperatorsArray = (JArray)jObj["operators"];
                    //                Response.AddHeader("X-IMI-IVRS-RESDESC", dicResp.GetValue("resDesc"));
                    //                Response.AddHeader("X-IMI-IVRS-RESPCODE", dicResp.GetValue("resCode"));
                    //                strRespCode = "0";
                    //            }
                    //            else
                    //                strRespCode = "1";
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                    //            strRespCode = "2";
                    //        }
                    //        #endregion
                    //    }
                    //    break;
                    //case "dth_airtel":
                    //    {
                    //        varDictionary.EditKeyValue("type", "DTH");
                    //        varDictionary.EditKeyValue("operator", "AT");
                    //        dicResp = oWrapper.Prepaid_Recharge(varDictionary);
                    //        if (dicResp.GetValue("resCode") == "000")
                    //        {
                    //            Response.AddHeader("X-IMI-IVRS-TID", dicResp.GetValue("TID"));
                    //            Response.AddHeader("X-IMI-IVRS-TSTATUS", dicResp.GetValue("TStatus"));
                    //            Response.AddHeader("X-IMI-IVRS-OPERATORTRANSID", dicResp.GetValue("OperatorTransID"));
                    //            Response.AddHeader("X-IMI-IVRS-RESDESC", dicResp.GetValue("resDesc"));
                    //            Response.AddHeader("X-IMI-IVRS-RESPCODE", dicResp.GetValue("resCode"));
                    //            strRespCode = "0";
                    //        }
                    //        else
                    //            strRespCode = "1";// dicResp.GetValue("resCode");
                    //    }
                    //    break;
                    //case "prepaid_recharge":
                    //    {
                    //        #region PREPAID RECHARGE
                    //        try
                    //        {
                    //            dicResp = oWrapper.Prepaid_Recharge(varDictionary);
                    //            if (dicResp.GetValue("resCode") == "000")
                    //            {
                    //                Response.AddHeader("X-IMI-IVRS-TID", dicResp.GetValue("TID"));
                    //                Response.AddHeader("X-IMI-IVRS-TSTATUS", dicResp.GetValue("TStatus"));
                    //                Response.AddHeader("X-IMI-IVRS-OPERATORTRANSID", dicResp.GetValue("OperatorTransID"));
                    //                Response.AddHeader("X-IMI-IVRS-RESDESC", dicResp.GetValue("resDesc"));
                    //                Response.AddHeader("X-IMI-IVRS-RESPCODE", dicResp.GetValue("resCode"));
                    //                strRespCode = "0";
                    //            }
                    //            else
                    //                strRespCode = "1";// dicResp.GetValue("resCode");
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                    //            strRespCode = "2";
                    //        }
                    //        #endregion
                    //    }
                    //    break;
                    //case "tsspdcl_getdistricts":
                    //    {
                    //        #region TSSPDCL GET DISTRICTS
                    //        try
                    //        {
                    //            JObject jObj = oWrapper.TSSPDCL_GetDistricts(varDictionary);
                    //            if (jObj != null && jObj["resCode"].ToStr() == "000")
                    //            {
                    //                JArray objDistArray = (JArray)jObj["distList"];
                    //                Response.AddHeader("X-IMI-IVRS-RESDESC", dicResp.GetValue("resDesc"));
                    //                Response.AddHeader("X-IMI-IVRS-RESPCODE", dicResp.GetValue("resCode"));
                    //                strRespCode = "0";
                    //            }
                    //            else
                    //                strRespCode = "1";
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                    //            strRespCode = "2";
                    //        }
                    //        #endregion
                    //    }
                    //    break;
                    //case "tsspdcl_geteros":
                    //    {
                    //        #region TSSPDCL GET EROS
                    //        try
                    //        {
                    //            JObject jObj = oWrapper.TSSPDCL_GetEROs(varDictionary);
                    //            if (jObj != null && jObj["resCode"].ToStr() == "000")
                    //            {
                    //                JArray objDistArray = (JArray)jObj["eroList"];
                    //                Response.AddHeader("X-IMI-IVRS-RESDESC", dicResp.GetValue("resDesc"));
                    //                Response.AddHeader("X-IMI-IVRS-RESPCODE", dicResp.GetValue("resCode"));
                    //                strRespCode = "0";
                    //            }
                    //            else
                    //                strRespCode = "1";
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                    //            strRespCode = "2";
                    //        }
                    //        #endregion
                    //    }
                    //    break;
                    //case "tsspdcl_viewbill":
                    //    {
                    //        #region TSSPDCL VIEW BILL
                    //        try
                    //        {
                    //            dicResp = oWrapper.TSSPDCL_ViewBill(varDictionary);
                    //            if (dicResp.GetValue("resCode") == "000")
                    //            {
                    //                Response.AddHeader("X-IMI-IVRS-ADDRESS1", dicResp.GetValue("address1"));
                    //                Response.AddHeader("X-IMI-IVRS-ADDRESS2", dicResp.GetValue("address2"));
                    //                Response.AddHeader("X-IMI-IVRS-CATEGORY", dicResp.GetValue("category"));
                    //                Response.AddHeader("X-IMI-IVRS-MOBILENO", dicResp.GetValue("mobileno"));
                    //                Response.AddHeader("X-IMI-IVRS-NETAMOUNT", dicResp.GetValue("netAmount"));
                    //                Response.AddHeader("X-IMI-IVRS-REQID", dicResp.GetValue("reqId"));
                    //                Response.AddHeader("X-IMI-IVRS-USERCHARGES", dicResp.GetValue("usercharges"));
                    //                Response.AddHeader("X-IMI-IVRS-ARREARS", dicResp.GetValue("arrears"));
                    //                Response.AddHeader("X-IMI-IVRS-BILLDATE", dicResp.GetValue("billdate"));
                    //                Response.AddHeader("X-IMI-IVRS-BILLNO", dicResp.GetValue("billno"));
                    //                Response.AddHeader("X-IMI-IVRS-CONSUMERNAME", dicResp.GetValue("consumername"));
                    //                Response.AddHeader("X-IMI-IVRS-CONSUMERNO", dicResp.GetValue("consumerno"));
                    //                Response.AddHeader("X-IMI-IVRS-CURRENTDMD", dicResp.GetValue("currentdmd"));
                    //                Response.AddHeader("X-IMI-IVRS-DISCONDATE", dicResp.GetValue("discondate"));
                    //                Response.AddHeader("X-IMI-IVRS-DUEDATE", dicResp.GetValue("duedate"));
                    //                Response.AddHeader("X-IMI-IVRS-UKSCNO", dicResp.GetValue("ukscno"));
                    //                Response.AddHeader("X-IMI-IVRS-RESDESC", dicResp.GetValue("resDesc"));
                    //                Response.AddHeader("X-IMI-IVRS-RESPCODE", dicResp.GetValue("resCode"));
                    //                if (dicResp.GetValue("netAmount").ToInt() <= 0)// if amount <=0
                    //                    strRespCode = "4";
                    //                else
                    //                    strRespCode = "0";
                    //            }
                    //            else
                    //                strRespCode = "1";// dicResp.GetValue("resCode");
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                    //            strRespCode = "2";
                    //        }
                    //        #endregion
                    //    }
                    //    break; 
                    //case "prepaid_recharge_status":
                    //    {
                    //        #region PREPAID RECHARGE STATUS
                    //        try
                    //        {
                    //            dicResp = oWrapper.Prepaid_RechargeStatus(varDictionary);
                    //            if (dicResp.GetValue("resCode") == "000")
                    //            {
                    //                Response.AddHeader("X-IMI-IVRS-TID", dicResp.GetValue("tid"));
                    //                Response.AddHeader("X-IMI-IVRS-TRANSSTATUSCODE", dicResp.GetValue("transStatusCode"));
                    //                Response.AddHeader("X-IMI-IVRS-TRANSSTATUS", dicResp.GetValue("transStatus"));
                    //                Response.AddHeader("X-IMI-IVRS-TRANSDATETIME", dicResp.GetValue("transDateTime"));
                    //                Response.AddHeader("X-IMI-IVRS-MOBILENO", dicResp.GetValue("mobileNo"));
                    //                Response.AddHeader("X-IMI-IVRS-AMOUNT", dicResp.GetValue("amount"));
                    //                Response.AddHeader("X-IMI-IVRS-RESDESC", dicResp.GetValue("resDesc"));
                    //                Response.AddHeader("X-IMI-IVRS-RESPCODE", dicResp.GetValue("resCode"));
                    //                strRespCode = "0";
                    //            }
                    //            else
                    //                strRespCode = dicResp.GetValue("resCode");
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                    //            strRespCode = "2";
                    //        }
                    //        #endregion
                    //    }
                    //    break;

                    //case "prepaid_viewbalance":
                    //    {
                    //        #region PREPAID VIEW BALANCE
                    //        try
                    //        {
                    //            dicResp = oWrapper.Prepaid_ViewBalance(varDictionary);
                    //            if (dicResp.GetValue("resCode") == "000")
                    //            {
                    //                Response.AddHeader("X-IMI-IVRS-TOTALACCOUNTBALANCE", dicResp.GetValue("totalAccountBalance"));
                    //                Response.AddHeader("X-IMI-IVRS-RESDESC", dicResp.GetValue("resDesc"));
                    //                Response.AddHeader("X-IMI-IVRS-RESPCODE", dicResp.GetValue("resCode"));
                    //                strRespCode = "0";
                    //            }
                    //            else
                    //                strRespCode = dicResp.GetValue("resCode");
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                    //            strRespCode = "2";
                    //        }
                    //        #endregion
                    //    }
                    //    break; 

                    //case "generateorderid"://credit card payment
                    //    {
                    //        #region Credit card payment
                    //        //String strAck = oWrapper.GetAcknowledgeXML(varDictionary);
                    //        //if (!string.IsNullOrEmpty(strAck))
                    //        //{
                    //        //    string strPGPostBody = General.GetConfigVal("PAYGOV_POSTBODY");
                    //        //    strPGPostBody = strPGPostBody.Replace("!TRANSID!", Guid.NewGuid().ToString());
                    //        //    strPGPostBody = strPGPostBody.Replace("!LANGUAGE!", varDictionary.GetValue("language"));
                    //        //    strPGPostBody = strPGPostBody.Replace("!CARDTYPE!", varDictionary.GetValue("cardtype"));
                    //        //    strPGPostBody = strPGPostBody.Replace("!AMOUNT!", varDictionary.GetValue("amount"));
                    //        //    strPGPostBody = strPGPostBody.Replace("!ACKXML!", strAck);
                    //        //    strResp = General.DoPostRequest(General.GetConfigVal("PGW_URL"), strPGPostBody);
                    //        //    if (strResp.Length > 0)
                    //        //    {
                    //        //        Response.AddHeader("X-IMI-IVRS-ORDERID", strResp);
                    //        //        Response.Write("0");
                    //        //    }
                    //        //    else
                    //        //        Response.Write("1");
                    //        //}
                    //        //else
                    //        //    Response.Write("2");

                    //        #endregion
                    //    }
                    //    break;
                    #endregion
                    #region Twallet IVRS
                    case "twallet_checkno":
                        {
                            #region CHECK number is available with Twallet or not
                            //string Plain_request = "<? xml version = \"1.0\" encoding = \"UTF-8\" ?>< Request type = \"IVRSCustCheck\" Terminal_Number = \"" + clgeneral.GetConfigVal("TERMINAL_NUMBER") + "\" Terminal_Name = \"" + clgeneral.GetConfigVal("TERMINAL_NAME") + "\">< Machine_Id >" + myIP + "</ Machine_Id >< Mobile_num >" + msisdn + "</ Mobile_num ></ Request >";
                            string Plain_request = new CreateXMl().getxmlstring_msisdn(strMobileNo, myIP);
                            string _Randomkey = DateTime.Now.Ticks.ToString();
                            string Rawdata = Plain_request;
                            string skey_encryption = Encryption.GetEncryptedText(Convert.ToBase64String(sessionKey.Key), Decryption.Decrypt_usingpassword(General.GetConfigVal("TAPublic_Key").ToString()));
                            string encrypted_data = Encryption.AESEncryption(Rawdata, Convert.ToBase64String(sessionKey.Key), false);
                            string final_data_req = new CreateXMl().getxmlstring(encrypted_data, skey_encryption);
                            string web_resp = new General().DoHttpPost_v1(client_url, "", final_data_req);
                            JObject jsonObj = JObject.Parse(web_resp);
                            if (!string.IsNullOrEmpty(jsonObj["Response_Code"].ToString()))
                            {
                                if (jsonObj["Response_Code"].ToString().Length == 4)
                                {
                                    strRespCode = "0";
                                }
                                else
                                {
                                    strRespCode = "1";
                                }
                            }
                            else
                            {
                                strRespCode = "1";
                            }
                            #endregion
                        }
                        break;

                    case "twallet_checkbal":
                        {
                            #region CHECK twallet balance
                            try
                            {
                                string Plain_request = new CreateXMl().getxmlstring_bal(strMobileNo, myIP);
                                string _Randomkey = DateTime.Now.Ticks.ToString();
                                string Rawdata = Plain_request;
                                string skey_encryption = Encryption.GetEncryptedText(Convert.ToBase64String(sessionKey.Key), Decryption.Decrypt_usingpassword(General.GetConfigVal("TAPublic_Key").ToString()));
                                string encrypted_data = Encryption.AESEncryption(Rawdata, Convert.ToBase64String(sessionKey.Key), false);
                                string final_data_req = new CreateXMl().getxmlstring(encrypted_data, skey_encryption);
                                string web_resp = new General().DoHttpPost_v1(client_url, "", final_data_req);
                                JObject jsonObj = JObject.Parse(web_resp);
                                if (!string.IsNullOrEmpty(jsonObj["Response_Code"].ToString()))
                                {
                                    if (jsonObj["Response_Code"].ToString().Length == 4)
                                    {
                                        Response.AddHeader("X-IMI-IVRS-AMOUNT", jsonObj["Balance"].ToString());
                                        strRespCode = "0";
                                    }
                                    else
                                    {
                                        strRespCode = "1";
                                    }
                                }
                                else
                                {
                                    strRespCode = "1";
                                }

                            }
                            catch (Exception ex)
                            {
                                LogData.Write("TwalletIVRS_bal", "twallet_checkbal", LogMode.Excep, ex.Message);
                                strRespCode = "1";
                            }
                            #endregion
                        }
                        break;
                    case "twallet_checkotherno":
                        {
                            #region CHECK twallet other number
                            try
                            {

                                if (!string.IsNullOrEmpty(varDictionary.GetValue("othertwalletnumber")))
                                {
                                    if (!string.IsNullOrEmpty(varDictionary.GetValue("othertwalletnumber")) || varDictionary.GetValue("othertwalletnumber").Trim().Length == 10)
                                    {

                                        string Plain_request = new CreateXMl().getxmlstring_Othermsisdn(strMobileNo, varDictionary.GetValue("othertwalletnumber"), myIP);
                                        string _Randomkey = DateTime.Now.Ticks.ToString();
                                        string Rawdata = Plain_request;
                                        string skey_encryption = Encryption.GetEncryptedText(Convert.ToBase64String(sessionKey.Key), Decryption.Decrypt_usingpassword(General.GetConfigVal("TAPublic_Key").ToString()));
                                        string encrypted_data = Encryption.AESEncryption(Rawdata, Convert.ToBase64String(sessionKey.Key), false);
                                        string final_data_req = new CreateXMl().getxmlstring(encrypted_data, skey_encryption);
                                        string web_resp = new General().DoHttpPost_v1(client_url, "", final_data_req);
                                        JObject jsonObj = JObject.Parse(web_resp);
                                        if (!string.IsNullOrEmpty(jsonObj["Response_Code"].ToString()))
                                        {
                                            if (jsonObj["Response_Code"].ToString().Length == 4)
                                            {
                                                strRespCode = "0";
                                            }
                                            else
                                            {
                                                strRespCode = "1";
                                            }
                                        }
                                        else
                                        {
                                            strRespCode = "1";
                                        }
                                    }
                                    else
                                    {
                                        strRespCode = "1";
                                    }
                                }
                                else
                                {
                                    strRespCode = "1";
                                }
                            }
                            catch (Exception ex)
                            {
                                strRespCode = "1";
                                LogData.Write("TwalletIVRS_bal", "twallet_checkotherno", LogMode.Excep, ex.Message);
                            }

                            #endregion
                        }
                        break;
                    case "twallet_checkotherotp":
                        {
                            #region CHECK twallet other number
                            try
                            {
                                if (!string.IsNullOrEmpty(varDictionary.GetValue("otp")))
                                {
                                    if (!string.IsNullOrEmpty(varDictionary.GetValue("otp")) || varDictionary.GetValue("otp").Trim().Length == 6)
                                    {
                                        string Plain_request = new CreateXMl().getxmlstring_Othermsisdn_bal(strMobileNo, varDictionary.GetValue("othertwalletnumber"), myIP, varDictionary.GetValue("otp"));
                                        string _Randomkey = DateTime.Now.Ticks.ToString();
                                        string Rawdata = Plain_request;
                                        string skey_encryption = Encryption.GetEncryptedText(Convert.ToBase64String(sessionKey.Key), Decryption.Decrypt_usingpassword(General.GetConfigVal("TAPublic_Key").ToString()));
                                        string encrypted_data = Encryption.AESEncryption(Rawdata, Convert.ToBase64String(sessionKey.Key), false);
                                        string final_data_req = new CreateXMl().getxmlstring(encrypted_data, skey_encryption);
                                        string web_resp = new General().DoHttpPost_v1(client_url, "", final_data_req);
                                        JObject jsonObj = JObject.Parse(web_resp);
                                        if (!string.IsNullOrEmpty(jsonObj["Response_Code"].ToString()))
                                        {
                                            if (jsonObj["Response_Code"].ToString().Length == 4)
                                            {
                                                Response.AddHeader("X-IMI-IVRS-AMOUNT", jsonObj["Balance"].ToString());
                                                strRespCode = "0";
                                                //strRespCode = jsonObj["Balance"].ToString();
                                            }
                                            else
                                            {
                                                strRespCode = "1";
                                            }
                                        }
                                        else
                                        {
                                            strRespCode = "1";
                                        }
                                    }
                                    else
                                    {
                                        strRespCode = "1";
                                    }
                                }
                                else
                                {
                                    strRespCode = "1";
                                }
                            }
                            catch (Exception ex)
                            {
                                LogData.Write("TwalletIVRS_bal", "twallet_checkotherotp", LogMode.Excep, ex.Message);
                                strRespCode = "1";
                            }

                            #endregion
                        }
                        break;
                    #endregion
                    default:
                        {
                            strRespCode = "-1";
                            TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Invalid Service:" + strService);
                        }
                        break;
                }
            }
            #endregion

            #region USSD
            if (strChannel.ToUpper().Trim() == "USSD")
            {
                Response.ContentType = "text/plain";
                //Response.Charset = "UTF-8";
                switch (strService.ToLower().Trim())
                {
                    case "user_check":
                        {
                            #region CHECK USER EXISTANCE
                            try
                            {
                                dicResp = oWrapper.User_Check_Existance(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    strRespCode = "0";
                                    strRespCode = "0";
                                    string lang = dicResp.GetValue("language");
                                    if (string.IsNullOrEmpty(lang))
                                        lang = "english";
                                    else if (lang.ToLower() == "en")
                                        lang = "english";
                                    else if (lang.ToLower() == "te")
                                        lang = "telugu";
                                    Response.AddHeader("x-imi-ussd-lang", lang);// if new user regitered by lang will be english
                                }
                                else if (dicResp.GetValue("resCode") == "3")
                                {
                                    strRespCode = "3";
                                    Response.AddHeader("x-imi-ussd-lang", "english");
                                }
                                else
                                    strRespCode = "1";
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in user_check, Message:" + ex.Message + ", stack trace:" + ex.StackTrace);
                                strRespCode = "1";
                            }
                            #endregion
                        }
                        break;
                    case "user_update_lang":
                        {
                            #region UPDATE PREFERRED LANGUAGE
                            try
                            {
                                dicResp = oWrapper.User_Update_PreferredLang(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                    strRespCode = "0";
                                else
                                    strRespCode = "1";
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in user_update_lang, Message:" + ex.Message + ", stack trace:" + ex.StackTrace);
                                strRespCode = "1";
                            }
                            #endregion
                        }
                        break;
                    case "airtel_viewbill":
                        {
                            #region AIRTEL VIEW BILL
                            try
                            {
                                dicResp = oWrapper.Airtel_ViewBill(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    // dicResp.EditKeyValue("billAmount", "98.45");// need to delete after testing
                                    Response.AddHeader("X-IMI-USSD-ACCNO", dicResp.GetValue("accountNo"));
                                    Response.AddHeader("X-IMI-USSD-BILLAMT", dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-USSD-BILLAMTINRUPEES", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[0] : dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-USSD-BILLAMTINPAISE", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[1] : String.Empty);
                                    Response.AddHeader("X-IMI-USSD-NAME", dicResp.GetValue("consumerName"));
                                    Response.AddHeader("X-IMI-USSD-REQID", dicResp.GetValue("reqId"));
                                    Response.AddHeader("X-IMI-USSD-RESPCODE", dicResp.GetValue("resCode"));
                                    if (dicResp.GetValue("billAmount").ToDouble() <= 0)// if amount <=0
                                        strRespCode = "4";
                                    else
                                        strRespCode = "0";
                                }
                                else
                                    strRespCode = "1";// dicResp.GetValue("resCode");
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "2";
                            }
                            #endregion
                        }
                        break;
                    case "airtel_landline_viewbill":
                        {
                            #region AIRTEL LANDLINE VIEW BILL
                            try
                            {
                                dicResp = oWrapper.Airtel_Landline_ViewBill(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    Response.AddHeader("X-IMI-USSD-ACCNO", dicResp.GetValue("accountNo"));
                                    Response.AddHeader("X-IMI-USSD-BILLAMT", dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-USSD-BILLAMTINRUPEES", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[0] : dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-USSD-BILLAMTINPAISE", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[1] : String.Empty);
                                    Response.AddHeader("X-IMI-USSD-NAME", dicResp.GetValue("consumerName"));
                                    Response.AddHeader("X-IMI-USSD-REQID", dicResp.GetValue("reqId"));
                                    Response.AddHeader("X-IMI-USSD-RESPCODE", dicResp.GetValue("resCode"));
                                    if (dicResp.GetValue("billAmount").ToInt() <= 0)// if amount <=0
                                        strRespCode = "4";
                                    else
                                        strRespCode = "0";
                                }
                                else
                                    strRespCode = "1";// dicResp.GetValue("resCode");
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "2";
                            }
                            #endregion
                        }
                        break;
                    case "idea_viewbill":
                        {
                            #region IDEA VIEW BILL
                            try
                            {
                                dicResp = oWrapper.Idea_ViewBill(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    // dicResp.EditKeyValue("billAmount", "10.00");// need to delete after testing
                                    Response.AddHeader("X-IMI-USSD-ACCNO", dicResp.GetValue("accountNo"));
                                    Response.AddHeader("X-IMI-USSD-BILLAMT", dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-USSD-BILLAMTINRUPEES", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[0] : dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-USSD-BILLAMTINPAISE", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[1] : String.Empty);
                                    Response.AddHeader("X-IMI-USSD-NAME", dicResp.GetValue("consumerName"));
                                    Response.AddHeader("X-IMI-USSD-REQID", dicResp.GetValue("reqId"));
                                    //Response.AddHeader("X-IMI-USSD-ADDRESS1", dicResp.GetValue("address1"));
                                    //Response.AddHeader("X-IMI-USSD-ADDRESS2", dicResp.GetValue("address2"));
                                    //Response.AddHeader("X-IMI-USSD-ADDRESS3", dicResp.GetValue("address3"));
                                    Response.AddHeader("X-IMI-USSD-RESPCODE", dicResp.GetValue("resCode"));
                                    if (dicResp.GetValue("billAmount").ToDouble() <= 0)// if amount <=0
                                        strRespCode = "4";
                                    else
                                        strRespCode = "0";
                                }
                                else
                                    strRespCode = "1";// dicResp.GetValue("resCode");
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "2";
                            }
                            #endregion
                        }
                        break;
                    case "ttl_viewbill":
                        {
                            #region TTL VIEW BILL
                            try
                            {
                                dicResp = oWrapper.TTL_ViewBill(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    //dicResp.EditKeyValue("billAmount", "10.00");// need to delete after testing
                                    Response.AddHeader("X-IMI-USSD-ACCNO", dicResp.GetValue("accountNo"));
                                    Response.AddHeader("X-IMI-USSD-BILLAMT", dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-USSD-NAME", dicResp.GetValue("consumerName"));
                                    Response.AddHeader("X-IMI-USSD-REQID", dicResp.GetValue("reqId"));
                                    Response.AddHeader("X-IMI-USSD-MOBILENO", dicResp.GetValue("mobileNo"));
                                    Response.AddHeader("X-IMI-USSD-BILLAMTINRUPEES", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[0] : dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-USSD-BILLAMTINPAISE", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[1] : String.Empty);
                                    Response.AddHeader("X-IMI-USSD-RESDESC", dicResp.GetValue("resDesc"));
                                    Response.AddHeader("X-IMI-USSD-RESPCODE", dicResp.GetValue("resCode"));
                                    if (dicResp.GetValue("billAmount").ToDouble() <= 0)// if amount <=0
                                        strRespCode = "4";
                                    else
                                        strRespCode = "0";
                                }
                                else
                                    strRespCode = "1";// dicResp.GetValue("resCode");
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "2";
                            }
                            #endregion
                        }
                        break;
                    case "bsnl_viewbill":
                        {
                            #region BSNL POSTPAID & LANDLINE VIEW BILL
                            try
                            {
                                dicResp = oWrapper.Bsnl_ViewBill(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    //dicResp.EditKeyValue("billAmount", "98.45");// need to delete after testing
                                    Response.AddHeader("X-IMI-USSD-ACCNO", dicResp.GetValue("accountNo"));
                                    Response.AddHeader("X-IMI-USSD-BILLAMT", dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-USSD-BILLAMTINRUPEES", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[0] : dicResp.GetValue("billAmount"));
                                    Response.AddHeader("X-IMI-USSD-BILLAMTINPAISE", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[1] : String.Empty);
                                    Response.AddHeader("X-IMI-USSD-NAME", dicResp.GetValue("consumerName"));
                                    Response.AddHeader("X-IMI-USSD-REQID", dicResp.GetValue("reqId"));
                                    Response.AddHeader("X-IMI-USSD-RESPCODE", dicResp.GetValue("resCode"));
                                    if (dicResp.GetValue("billAmount").ToDouble() <= 0)// if amount <=0
                                        strRespCode = "4";
                                    else
                                        strRespCode = "0";
                                }
                                else
                                    strRespCode = "1";// dicResp.GetValue("resCode");
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "2";
                            }
                            #endregion
                        }
                        break;
                    case "act_viewbill":
                        {
                            #region ACT VIEW BILL
                            try
                            {
                                dicResp = oWrapper.Act_ViewBill(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    //dicResp.EditKeyValue("billAmount", "403.34");// need to delete after testing
                                    Response.AddHeader("x-imi-ussd-accno", dicResp.GetValue("accountNo"));
                                    Response.AddHeader("x-imi-ussd-billamt", dicResp.GetValue("billAmount"));
                                    //Response.AddHeader("X-IMI-USSD-BILLAMTINRUPEES", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[0] : dicResp.GetValue("billAmount"));
                                    //Response.AddHeader("X-IMI-USSD-BILLAMTINPAISE", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[1] : String.Empty);
                                    //Response.AddHeader("X-IMI-USSD-BILLNO", dicResp.GetValue("billNo"));
                                    //Response.AddHeader("X-IMI-USSD-BILLDATE", dicResp.GetValue("billDate"));
                                    //Response.AddHeader("X-IMI-USSD-DISTRICT", dicResp.GetValue("district"));
                                    //Response.AddHeader("X-IMI-USSD-ERRORCD", dicResp.GetValue("errCD"));
                                    // Response.AddHeader("X-IMI-USSD-PERIOD", dicResp.GetValue("period"));
                                    // Response.AddHeader("X-IMI-USSD-REGION", dicResp.GetValue("region"));
                                    // Response.AddHeader("X-IMI-USSD-STATE", dicResp.GetValue("state"));
                                    //  Response.AddHeader("X-IMI-USSD-TRANSNO", dicResp.GetValue("transNo"));
                                    //  Response.AddHeader("X-IMI-USSD-NAME", dicResp.GetValue("customerName"));
                                    Response.AddHeader("x-imi-ussd-reqid", dicResp.GetValue("reqId"));
                                    // Response.AddHeader("X-IMI-USSD-ADDRESS1", dicResp.GetValue("address1"));
                                    // Response.AddHeader("X-IMI-USSD-ADDRESS2", dicResp.GetValue("address2"));
                                    // Response.AddHeader("X-IMI-USSD-ADDRESS3", dicResp.GetValue("address3"));
                                    Response.AddHeader("x-imi-ussd-respcode", dicResp.GetValue("resCode"));
                                    if (dicResp.GetValue("billAmount").ToDouble() <= 0)// if amount <=0
                                        strRespCode = "4";
                                    else
                                        strRespCode = "0";
                                }
                                else
                                    strRespCode = "1";
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "2";
                            }
                            #endregion
                        }
                        break;
                    case "rta_viewbill":
                        {
                            #region RTA VIEW BILL
                            try
                            {
                                dicResp = oWrapper.RTA_ViewBill(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    Response.AddHeader("x-imi-ussd-depttransid", dicResp.GetValue("deptTransId"));
                                    Response.AddHeader("x-imi-ussd-name", dicResp.GetValue("name"));
                                    Response.AddHeader("x-imi-ussd-slotdate", dicResp.GetValue("slotDate"));
                                    Response.AddHeader("x-imi-ussd-slottime", dicResp.GetValue("slotTime"));
                                    Response.AddHeader("x-imi-ussd-servicedesc", dicResp.GetValue("serviceDesc"));
                                    Response.AddHeader("x-imi-ussd-totalamount", dicResp.GetValue("totAmt"));
                                    Response.AddHeader("x-imi-ussd-reqid", dicResp.GetValue("reqId"));
                                    Response.AddHeader("x-imi-ussd-respcode", dicResp.GetValue("resCode"));
                                    if (dicResp.GetValue("totAmt").ToDouble() <= 0)// if amount <=0
                                        strRespCode = "4";
                                    else
                                        strRespCode = "0";
                                }
                                else
                                    strRespCode = "1";// dicResp.GetValue("resCode");
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "2";
                            }
                            #endregion
                        }
                        break;
                    case "rta_lifetax_viewbill":
                        {
                            #region RTA Life Tax View Bill
                            try
                            {

                                dicResp = oWrapper.RTA_LifeTax(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    Response.AddHeader("x-imi-ussd-llrno", dicResp.GetValue("llrNo"));
                                    Response.AddHeader("x-imi-ussd-totalamount", dicResp.GetValue("totAmt"));
                                    Response.AddHeader("x-imi-ussd-invdate", dicResp.GetValue("invoceDt"));
                                    Response.AddHeader("x-imi-ussd-name", dicResp.GetValue("custName"));
                                    Response.AddHeader("x-imi-ussd-reqid", dicResp.GetValue("reqId"));
                                    Response.AddHeader("x-imi-ussd-respcode", dicResp.GetValue("resCode"));
                                    strRespCode = "0";
                                }
                                else
                                {
                                    strRespCode = "1";
                                }

                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                            }
                            #endregion
                        }
                        break;
                    case "application_status":
                        {
                            #region Meeseva application status
                            dicResp = oWrapper.Get_MEESEVA_Application_Status(varDictionary);
                            if (dicResp.GetValue("resCode") == "000")
                            {
                                Response.AddHeader("x-imi-ussd-appstatus", dicResp.GetValue("status"));
                                Response.AddHeader("x-imi-ussd-resdesc", dicResp.GetValue("resDesc"));
                                Response.AddHeader("x-imi-ussd-respcode", dicResp.GetValue("resCode"));
                                strRespCode = "0";
                            }
                            else if (dicResp.GetValue("resCode") == "3")
                            {
                                strRespCode = "3";
                            }
                            else
                            {
                                strRespCode = "1";
                                Response.AddHeader("x-imi-ussd-appstatus", dicResp.GetValue("resDesc"));
                            }
                            #endregion
                        }
                        break;
                    case "hmwssb_viewbill":
                        {
                            #region HMWSSB VIEW BILL
                            try
                            {
                                dicResp = oWrapper.HMWSSB_ViewBill(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    Response.AddHeader("x-imi-ussd-canno", dicResp.GetValue("canNo"));
                                    Response.AddHeader("x-imi-ussd-address", dicResp.GetValue("address"));
                                    Response.AddHeader("x-imi-ussd-billamt", dicResp.GetValue("amount"));
                                    Response.AddHeader("x-imi-ussd-billamtinrupees", dicResp.GetValue("amount").Contains(".") ? dicResp.GetValue("amount").Split('.')[0] : dicResp.GetValue("amount"));
                                    Response.AddHeader("x-imi-ussd-billamtinpaise", dicResp.GetValue("amount").Contains(".") ? dicResp.GetValue("amount").Split('.')[1] : String.Empty);
                                    Response.AddHeader("x-imi-ussd-name", dicResp.GetValue("name"));
                                    Response.AddHeader("x-imi-ussd-reqid", dicResp.GetValue("reqId"));
                                    Response.AddHeader("x-imi-ussd-respcode", dicResp.GetValue("resCode"));
                                    if (dicResp.GetValue("amount").ToDouble() <= 0)// if amount <=0
                                        strRespCode = "4";
                                    else
                                        strRespCode = "0";
                                }
                                else
                                    strRespCode = "1";// dicResp.GetValue("resCode");
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in hmwssb_viewbill, Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "2";
                            }
                            #endregion
                        }
                        break;
                    case "hathway_viewbill":
                        {
                            #region HATHWAY VIEW BILL
                            try
                            {
                                dicResp = oWrapper.HathWay_ViewBill(varDictionary);
                                if (dicResp.GetValue("resCode") == "000")
                                {
                                    //dicResp.EditKeyValue("billAmount", "403.34");// need to delete after testing
                                    Response.AddHeader("x-imi-ussd-accno", dicResp.GetValue("accountno"));
                                    Response.AddHeader("x-imi-ussd-billamt", dicResp.GetValue("billamount"));
                                    //Response.AddHeader("X-IMI-USSD-BILLAMTINRUPEES", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[0] : dicResp.GetValue("billAmount"));
                                    //Response.AddHeader("X-IMI-USSD-BILLAMTINPAISE", dicResp.GetValue("billAmount").Contains(".") ? dicResp.GetValue("billAmount").Split('.')[1] : String.Empty);
                                    //Response.AddHeader("X-IMI-USSD-BILLNO", dicResp.GetValue("billNo"));
                                    //Response.AddHeader("X-IMI-USSD-BILLDATE", dicResp.GetValue("billDate"));
                                    //Response.AddHeader("X-IMI-USSD-DISTRICT", dicResp.GetValue("district"));
                                    //Response.AddHeader("X-IMI-USSD-ERRORCD", dicResp.GetValue("errCD"));
                                    // Response.AddHeader("X-IMI-USSD-PERIOD", dicResp.GetValue("period"));
                                    // Response.AddHeader("X-IMI-USSD-REGION", dicResp.GetValue("region"));
                                    // Response.AddHeader("X-IMI-USSD-STATE", dicResp.GetValue("state"));
                                    //  Response.AddHeader("X-IMI-USSD-TRANSNO", dicResp.GetValue("transNo"));
                                    //  Response.AddHeader("X-IMI-USSD-NAME", dicResp.GetValue("customerName"));
                                    Response.AddHeader("x-imi-ussd-reqid", dicResp.GetValue("billRefNo"));
                                    Response.AddHeader("x-imi-ussd-hwmobno", dicResp.GetValue("mobileNo"));
                                    Response.AddHeader("x-imi-ussd-address", dicResp.GetValue("address"));
                                    Response.AddHeader("x-imi-ussd-respcode", dicResp.GetValue("resCode"));
                                    if (dicResp.GetValue("billAmount").ToDouble() <= 0)// if amount <=0
                                        strRespCode = "4";
                                    else
                                        strRespCode = "0";
                                }
                                else
                                    strRespCode = "1";
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "2";
                            }
                            #endregion
                        }
                        break;
                    #region Payment related
                    case "check_dthnumber":
                        {
                            #region CHECK DTH SUBSCIPTION NUMBER LENGTH
                            if ((varDictionary.GetValue("number").Length < General.GetConfigVal("DTH_NUMBER_MIN_LENGTH").ToInt()) || (varDictionary.GetValue("number").Length > General.GetConfigVal("DTH_NUMBER_MAX_LENGTH").ToInt()))
                            {
                                TextLog.Exception(strWorkSpaceId, "dth_number", "Invalid dth number" + varDictionary.GetValue("number") + " for tid:" + varDictionary.GetValue("tid"));
                                strRespCode = "1";
                            }
                            else
                                strRespCode = "0";
                            #endregion
                        }
                        break;
                    case "validateamount"://validate amount is partial
                        {
                            #region VALIDATE AMOUNT
                            try
                            {
                                if (Convert.ToDouble(varDictionary.GetValue("amount")) >= Convert.ToDouble(varDictionary.GetValue("billamount")))
                                    strRespCode = "0";
                                else
                                    strRespCode = "1";
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "1";
                            }
                            #endregion
                        }
                        break;
                    case "validateminamount":
                        {
                            #region Validate Min & Max Amount
                            try
                            {
                                if (Convert.ToDouble(varDictionary.GetValue("amount")) < Convert.ToDouble(General.GetConfigVal("MIN_ALLOWED_AMOUNT")))
                                    strRespCode = "1";
                                else if (Convert.ToDouble(varDictionary.GetValue("amount")) > Convert.ToDouble(General.GetConfigVal("MAX_ALLOWED_AMOUNT")))
                                    strRespCode = "2";
                                else
                                    strRespCode = "0";
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "1";
                            }
                            #endregion
                        }
                        break;
                    case "validateminmaxamount":
                        {
                            #region Validate Min & Max Amount for prepaid recharges
                            try
                            {
                                if (Convert.ToDouble(varDictionary.GetValue("amount")) < Convert.ToDouble(General.GetConfigVal("MIN_AMOUNT_RECHARGE")))
                                    strRespCode = "1";
                                else if (Convert.ToDouble(varDictionary.GetValue("amount")) > Convert.ToDouble(General.GetConfigVal("MAX_ALLOWED_AMOUNT")))
                                    strRespCode = "2";
                                else
                                    strRespCode = "0";
                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                                strRespCode = "1";
                            }
                            #endregion
                        }
                        break;
                    case "twalletgenotp":
                        {
                            #region GENERATE OTP
                            Dictionary<string, string> dicOTPResp = oWrapper.GenerateTwalletOTP(varDictionary);
                            if (dicOTPResp != null && dicOTPResp.GetValue("Response_Code") == "1680")
                                strRespCode = "0";
                            else
                                strRespCode = "1";
                            #endregion
                        }
                        break;
                    case "twalletpayment":
                        {
                            #region T-WALLET
                            try
                            {
                                //if (varDictionary.GetValue("amount") != "" && Convert.ToDouble(varDictionary.GetValue("amount")) < Convert.ToDouble(General.GetConfigVal("MIN_ALLOWED_AMOUNT")))
                                //{
                                //    TextLog.Exception(strWorkSpaceId, "payment_ex", "Invalid amount found: channel:" + strChannel + ", Mobileno:" + strMobileNo + "Amount:" + varDictionary.GetValue("amount"));
                                //    strRespCode = "4";
                                //}
                                //else if (varDictionary.GetValue("amount") != "" && Convert.ToDouble(varDictionary.GetValue("amount")) > Convert.ToDouble(General.GetConfigVal("MAX_ALLOWED_AMOUNT")))
                                //{
                                //    TextLog.Exception(strWorkSpaceId, "payment_ex", "Invalid amount found: channel:" + strChannel + ", Mobileno:" + strMobileNo + "Amount:" + varDictionary.GetValue("amount"));
                                //    strRespCode = "5";
                                //}

                                if (Array.IndexOf(General.GetConfigVal("MIN_AMOUNT_1_DEPTS").ToLower().Split(','), varDictionary.GetValue("dept").ToLower()) > -1)
                                {
                                    if (Convert.ToDouble(varDictionary.GetValue("amount")) < Convert.ToDouble(General.GetConfigVal("MIN_AMOUNT_RECHARGE")))
                                    {
                                        TextLog.Exception(strWorkSpaceId, "payment_ex", "Invalid amount found: channel:" + strChannel + ", Mobileno:" + strMobileNo + "Amount:" + varDictionary.GetValue("amount"));
                                        strRespCode = "4";
                                        return;
                                    }
                                }
                                else if (Array.IndexOf(General.GetConfigVal("MIN_AMOUNT_10_DEPTS").ToLower().Split(','), varDictionary.GetValue("dept").ToLower()) > -1)
                                {
                                    if (Convert.ToDouble(varDictionary.GetValue("amount")) < Convert.ToDouble(General.GetConfigVal("MIN_AMOUNT_BILLPAYMENTS")))
                                    {
                                        TextLog.Exception(strWorkSpaceId, "payment_ex", "Invalid amount found: channel:" + strChannel + ", Mobileno:" + strMobileNo + "Amount:" + varDictionary.GetValue("amount"));
                                        strRespCode = "4";
                                        return;
                                    }
                                }
                                if (Convert.ToDouble(varDictionary.GetValue("amount")) > Convert.ToDouble(General.GetConfigVal("MAX_ALLOWED_AMOUNT")))
                                {
                                    TextLog.Exception(strWorkSpaceId, "payment_ex", "Invalid amount found: channel:" + strChannel + ", Mobileno:" + strMobileNo + "Amount:" + varDictionary.GetValue("amount"));
                                    strRespCode = "5";
                                    return;
                                }
                                if (varDictionary.GetValue("twalletmobileno").Length != 10)
                                {
                                    TextLog.Exception(strWorkSpaceId, "payment_ex", "Invalid twalletmobileno found: channel:" + strChannel + ", Mobileno:" + strMobileNo + "twalletmobileno:" + varDictionary.GetValue("twalletmobileno"));
                                    strRespCode = "6";
                                    return;
                                }
                                TWALLETBO objBO = oWrapper.PayThroughTWALLET(varDictionary);
                                if (objBO.RespCode == "0")
                                {
                                    Response.AddHeader("x-imi-ussd-transno", objBO.TransNo.ToStr());
                                    Response.AddHeader("x-imi-ussd-authcode", objBO.AuthCode.ToStr());
                                    Response.AddHeader("x-imi-ussd-imitransid", objBO.IMITransID.ToStr());
                                    Response.AddHeader("x-imi-ussd-impsamount", objBO.TWalletAmount.ToStr());
                                    Response.AddHeader("x-imi-ussd-respdesc", objBO.RespDesc.ToStr());
                                    strRespCode = "0";
                                }
                                else
                                {
                                    strRespCode = objBO.RespCode.ToStr();
                                }

                            }
                            catch (Exception ex)
                            {
                                TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                            }
                            #endregion
                        }
                        break;
                    #endregion

                    #region Not using

                    //case "validatemmmid":
                    //case "validatemmid":
                    //    {
                    //        #region Validate MMID or MMMID
                    //        if (varDictionary.GetValue("mmid").StartsWith("9") && varDictionary.GetValue("mmid").Length == 7)
                    //            strRespCode = "0";
                    //        else
                    //            strRespCode = "1";
                    //        #endregion
                    //    }
                    //    break;
                    //case "imps":
                    //    {
                    //        #region IMPS
                    //        try
                    //        {
                    //            IMPSBO objBO = oWrapper.PayThroughIMPS(varDictionary);
                    //            if (objBO.RespCode == "0")
                    //            {
                    //                Response.AddHeader("X-IMI-USSD-TRANSNO", objBO.TransNo.ToStr());
                    //                Response.AddHeader("X-IMI-USSD-AUTHCODE", objBO.AuthCode.ToStr());
                    //                Response.AddHeader("X-IMI-USSD-IMITRANSID", objBO.IMITransID.ToStr());
                    //                Response.AddHeader("X-IMI-USSD-IMPSAMOUNT", objBO.IMPSAmount.ToStr());
                    //                strRespCode = "0";
                    //            }
                    //            else
                    //            {
                    //                strRespCode = objBO.RespCode.ToStr();
                    //            }
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                    //        }
                    //        #endregion
                    //    }
                    //    break;                   
                    //case "tsspdcl_getdistricts":
                    //    {
                    //        #region TSSPDCL GET DISTRICTS
                    //        try
                    //        {
                    //            JObject jObj = oWrapper.TSSPDCL_GetDistricts(varDictionary);
                    //            if (jObj != null && jObj["resCode"].ToStr() == "000")
                    //            {
                    //                JArray objDistArray = (JArray)jObj["distList"];
                    //                Response.AddHeader("X-IMI-USSD-RESDESC", dicResp.GetValue("resDesc"));
                    //                Response.AddHeader("X-IMI-USSD-RESPCODE", dicResp.GetValue("resCode"));
                    //                strRespCode = "0";
                    //            }
                    //            else
                    //                strRespCode = "1";
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                    //            strRespCode = "2";
                    //        }
                    //        #endregion
                    //    }
                    //    break;
                    //case "tsspdcl_geteros":
                    //    {
                    //        #region TSSPDCL GET EROS
                    //        try
                    //        {
                    //            JObject jObj = oWrapper.TSSPDCL_GetEROs(varDictionary);
                    //            if (jObj != null && jObj["resCode"].ToStr() == "000")
                    //            {
                    //                JArray objDistArray = (JArray)jObj["eroList"];
                    //                Response.AddHeader("X-IMI-USSD-RESDESC", dicResp.GetValue("resDesc"));
                    //                Response.AddHeader("X-IMI-USSD-RESPCODE", dicResp.GetValue("resCode"));
                    //                strRespCode = "0";
                    //            }
                    //            else
                    //                strRespCode = "1";
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                    //            strRespCode = "2";
                    //        }
                    //        #endregion
                    //    }
                    //    break;
                    //case "tsspdcl_viewbill":
                    //    {
                    //        #region TSSPDCL VIEW BILL
                    //        try
                    //        {
                    //            dicResp = oWrapper.TSSPDCL_ViewBill(varDictionary);
                    //            if (dicResp.GetValue("resCode") == "000")
                    //            {
                    //                Response.AddHeader("X-IMI-USSD-ADDRESS1", dicResp.GetValue("address1"));
                    //                Response.AddHeader("X-IMI-USSD-ADDRESS2", dicResp.GetValue("address2"));
                    //                Response.AddHeader("X-IMI-USSD-CATEGORY", dicResp.GetValue("category"));
                    //                Response.AddHeader("X-IMI-USSD-MOBILENO", dicResp.GetValue("mobileno"));
                    //                Response.AddHeader("X-IMI-USSD-NETAMOUNT", dicResp.GetValue("netAmount"));
                    //                Response.AddHeader("X-IMI-USSD-REQID", dicResp.GetValue("reqId"));
                    //                Response.AddHeader("X-IMI-USSD-USERCHARGES", dicResp.GetValue("usercharges"));
                    //                Response.AddHeader("X-IMI-USSD-ARREARS", dicResp.GetValue("arrears"));
                    //                Response.AddHeader("X-IMI-USSD-BILLDATE", dicResp.GetValue("billdate"));
                    //                Response.AddHeader("X-IMI-USSD-BILLNO", dicResp.GetValue("billno"));
                    //                Response.AddHeader("X-IMI-USSD-CONSUMERNAME", dicResp.GetValue("consumername"));
                    //                Response.AddHeader("X-IMI-USSD-CONSUMERNO", dicResp.GetValue("consumerno"));
                    //                Response.AddHeader("X-IMI-USSD-CURRENTDMD", dicResp.GetValue("currentdmd"));
                    //                Response.AddHeader("X-IMI-USSD-DISCONDATE", dicResp.GetValue("discondate"));
                    //                Response.AddHeader("X-IMI-USSD-DUEDATE", dicResp.GetValue("duedate"));
                    //                Response.AddHeader("X-IMI-USSD-UKSCNO", dicResp.GetValue("ukscno"));
                    //                Response.AddHeader("X-IMI-USSD-RESDESC", dicResp.GetValue("resDesc"));
                    //                Response.AddHeader("X-IMI-USSD-RESPCODE", dicResp.GetValue("resCode"));
                    //                if (dicResp.GetValue("netAmount").ToInt() <= 0)// if amount <=0
                    //                    strRespCode = "4";
                    //                else
                    //                    strRespCode = "0";
                    //            }
                    //            else
                    //                strRespCode = "1";// dicResp.GetValue("resCode");
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                    //            strRespCode = "2";
                    //        }
                    //        #endregion
                    //    }
                    //    break;
                    //case "prepaid_getoperators":
                    //    {
                    //        #region PREPAID GET OPERATORS
                    //        try
                    //        {
                    //            JObject jObj = oWrapper.Prepaid_GetOperators(varDictionary);
                    //            if (jObj != null && jObj["resCode"].ToStr() == "000")
                    //            {
                    //                JArray objOperatorsArray = (JArray)jObj["operators"];
                    //                Response.AddHeader("X-IMI-USSD-RESDESC", dicResp.GetValue("resDesc"));
                    //                Response.AddHeader("X-IMI-USSD-RESPCODE", dicResp.GetValue("resCode"));
                    //                strRespCode = "0";
                    //            }
                    //            else
                    //                strRespCode = "1";
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                    //            strRespCode = "2";
                    //        }
                    //        #endregion
                    //    }
                    //    break;                
                    //case "prepaid_recharge":
                    //    {
                    //        #region PREPAID RECHARGE
                    //        try
                    //        {
                    //            dicResp = oWrapper.Prepaid_Recharge(varDictionary);
                    //            if (dicResp.GetValue("resCode") == "000")
                    //            {
                    //                Response.AddHeader("X-IMI-USSD-TID", dicResp.GetValue("TID"));
                    //                Response.AddHeader("X-IMI-USSD-TSTATUS", dicResp.GetValue("TStatus"));
                    //                Response.AddHeader("X-IMI-USSD-OPERATORTRANSID", dicResp.GetValue("OperatorTransID"));
                    //                Response.AddHeader("X-IMI-USSD-RESDESC", dicResp.GetValue("resDesc"));
                    //                Response.AddHeader("X-IMI-USSD-RESPCODE", dicResp.GetValue("resCode"));
                    //                strRespCode = "0";
                    //            }
                    //            else
                    //                strRespCode = "1";// dicResp.GetValue("resCode");
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                    //            strRespCode = "2";
                    //        }
                    //        #endregion
                    //    }
                    //    break;
                    //case "rta_lltest_viewbill":
                    //    {
                    //        #region RTA LL Test fee View Bill
                    //        try
                    //        {
                    //            dicResp = oWrapper.RTA_LLTestFee(varDictionary);
                    //            if (dicResp.GetValue("resCode") == "000")
                    //            {
                    //                //Response.AddHeader("X-IMI-USSD-LLRNO", dicResp.GetValue("llrNo"));
                    //                //Response.AddHeader("X-IMI-USSD-TOTALAMOUNT", dicResp.GetValue("testTotalAmount"));
                    //                //Response.AddHeader("X-IMI-USSD-SLOTDATE", dicResp.GetValue("sbDate"));
                    //                // Response.AddHeader("X-IMI-USSD-SLOTTIME", dicResp.GetValue("stTime"));
                    //                Response.AddHeader("x-imi-ussd-name", dicResp.GetValue("appName"));
                    //                //Response.AddHeader("X-IMI-USSD-REQID", dicResp.GetValue("reqId"));
                    //                //Response.AddHeader("X-IMI-USSD-RESPCODE", dicResp.GetValue("resCode"));
                    //                // Response.AddHeader("X-IMI-USSD-STATUS", "0");
                    //                strRespCode = "0";
                    //            }
                    //            else
                    //                strRespCode = "1";
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                    //        }
                    //        #endregion
                    //    }
                    //    break; 
                    //case "prepaid_recharge_status":
                    //    {
                    //        #region PREPAID RECHARGE STATUS
                    //        try
                    //        {
                    //            dicResp = oWrapper.Prepaid_RechargeStatus(varDictionary);
                    //            if (dicResp.GetValue("resCode") == "000")
                    //            {
                    //                Response.AddHeader("X-IMI-USSD-TID", dicResp.GetValue("tid"));
                    //                Response.AddHeader("X-IMI-USSD-TRANSSTATUSCODE", dicResp.GetValue("transStatusCode"));
                    //                Response.AddHeader("X-IMI-USSD-TRANSSTATUS", dicResp.GetValue("transStatus"));
                    //                Response.AddHeader("X-IMI-USSD-TRANSDATETIME", dicResp.GetValue("transDateTime"));
                    //                Response.AddHeader("X-IMI-USSD-MOBILENO", dicResp.GetValue("mobileNo"));
                    //                Response.AddHeader("X-IMI-USSD-AMOUNT", dicResp.GetValue("amount"));
                    //                Response.AddHeader("X-IMI-USSD-RESDESC", dicResp.GetValue("resDesc"));
                    //                Response.AddHeader("X-IMI-USSD-RESPCODE", dicResp.GetValue("resCode"));
                    //                strRespCode = "0";
                    //            }
                    //            else
                    //                strRespCode = dicResp.GetValue("resCode");
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                    //            strRespCode = "2";
                    //        }
                    //        #endregion
                    //    }
                    //    break;

                    //case "prepaid_viewbalance":
                    //    {
                    //        #region PREPAID VIEW BALANCE
                    //        try
                    //        {
                    //            dicResp = oWrapper.Prepaid_ViewBalance(varDictionary);
                    //            if (dicResp.GetValue("resCode") == "000")
                    //            {
                    //                Response.AddHeader("X-IMI-USSD-TOTALACCOUNTBALANCE", dicResp.GetValue("totalAccountBalance"));
                    //                Response.AddHeader("X-IMI-USSD-RESDESC", dicResp.GetValue("resDesc"));
                    //                Response.AddHeader("X-IMI-USSD-RESPCODE", dicResp.GetValue("resCode"));
                    //                strRespCode = "0";
                    //            }
                    //            else
                    //                strRespCode = dicResp.GetValue("resCode");
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            TextLog.Exception(strWorkSpaceId, "Default_" + CHANNEL + "_ex", "Exception in " + strService.ToLower() + ", Message:" + ex.Message + ", Stack trace:" + ex.StackTrace);
                    //            strRespCode = "2";
                    //        }
                    //        #endregion
                    //    }
                    //    break; 
                    //case "generateorderid"://credit card payment
                    //    {
                    //        #region Credit card payment
                    //        //String strAck = oWrapper.GetAcknowledgeXML(varDictionary);
                    //        //if (!string.IsNullOrEmpty(strAck))
                    //        //{
                    //        //    string strPGPostBody = General.GetConfigVal("PAYGOV_POSTBODY");
                    //        //    strPGPostBody = strPGPostBody.Replace("!TRANSID!", Guid.NewGuid().ToString());
                    //        //    strPGPostBody = strPGPostBody.Replace("!LANGUAGE!", varDictionary.GetValue("language"));
                    //        //    strPGPostBody = strPGPostBody.Replace("!CARDTYPE!", varDictionary.GetValue("cardtype"));
                    //        //    strPGPostBody = strPGPostBody.Replace("!AMOUNT!", varDictionary.GetValue("amount"));
                    //        //    strPGPostBody = strPGPostBody.Replace("!ACKXML!", strAck);
                    //        //    strResp = General.DoPostRequest(General.GetConfigVal("PGW_URL"), strPGPostBody);
                    //        //    if (strResp.Length > 0)
                    //        //    {
                    //        //        Response.AddHeader("X-IMI-USSD-ORDERID", strResp);
                    //        //        Response.Write("0");
                    //        //    }
                    //        //    else
                    //        //        Response.Write("1");
                    //        //}
                    //        //else
                    //        //    Response.Write("2");

                    //        #endregion
                    //    }
                    //    break;
                    #endregion
                    default:
                        {
                            strRespCode = "-1";
                            TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Invalid Service:" + strService);
                        }
                        break;
                }
            }
            #endregion

        }
        catch (Exception ex)
        {
            TextLog.Exception(strWorkSpaceId, "Default_" + strChannel + "_ex", "Exception in page_load, Message:" + ex.Message + ", stack trace:" + ex.StackTrace);
            strRespCode = "1";
        }
        finally
        {
            varDictionary = null;
            oWrapper = null;
            dicResp = null;
            Response.Write(strRespCode);
            TextLog.Debug(strWorkSpaceId, "Default", string.Format("Landed Request:{1}{0} Response:{2}{0}", Environment.NewLine, Request.Url.ToStr(), strRespCode));
        }
    }






}