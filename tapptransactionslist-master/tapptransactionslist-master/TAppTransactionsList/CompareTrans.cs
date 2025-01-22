using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonHelper;
using System.Data;
using IMI.SqlWrapper;
using System.Xml;
using System.IO;
using Newtonsoft.Json;
using System.Collections;

namespace TAppTransactionsList
{
    class CompareTrans
    {
        static String strMailTo = General.GetConfigVal("MAIL_TO");
        static String strMailCC = General.GetConfigVal("MAIL_CC");
        static String strMailBCC = General.GetConfigVal("MAIL_BCC");
        public static void CompareTransList()
        {
            try
            {


                String strFromDate = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");
                String strToDate = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");
                String strDate = DateTime.Now.AddDays(-1).ToString("dd-MMM-yyyy");
                if (General.GetConfigVal("ENABLE_CUSTOM_DATE").ToUpper() == "Y" && General.GetConfigVal("CUSTOM_FROM_DATE") != "")
                {
                    strFromDate = Convert.ToDateTime(General.GetConfigVal("CUSTOM_FROM_DATE")).ToString("dd-MM-yyyy");
                    strToDate = Convert.ToDateTime(General.GetConfigVal("CUSTOM_TO_DATE")).ToString("dd-MM-yyyy");
                    strDate = Convert.ToDateTime(General.GetConfigVal("CUSTOM_FROM_DATE")).ToString("dd-MMM-yyyy");
                }
                Console.WriteLine("From Date:" + strFromDate);
                Console.WriteLine("To Date:" + strToDate);
                String strMailBody = string.Empty;
                if (Array.IndexOf(General.GetConfigVal("ENABLE_COMPARE_TRANS_DEPT").ToUpper().Split(','), "ENTROLABS_TEMPLES") > -1)
                {
                    ResponseBO objStamps = GetEntrolabsRevenue(strFromDate, strToDate);
                    General.WriteLog("COMPARE_TRANS", " Entrolabs: FROM DATE:" + strFromDate + ", strToDate:" + strToDate + ",TransCount From TAPP:" + objStamps.EntrolabsTappCount.ToStr() + ",  Entrolabs From Dept:" + objStamps.EntrolabsCount.ToStr() + ", Entrolabs Amount:" + objStamps.EntrolabsAmount.ToStr() + ", Entrolabs Tapp amount:" + objStamps.EntrolabsTappAmount.ToStr());
                    if (objStamps != null && ((objStamps.EntrolabsCount != objStamps.EntrolabsTappCount) || objStamps.EntrolabsAmount != objStamps.EntrolabsTappAmount))
                    {
                        Console.WriteLine("differenrce found in ENTROLABS_TEMPLES");
                        strMailBody += PrepareEntrolabsMailBody(objStamps.EntrolabsCount,objStamps.EntrolabsTappCount,objStamps.EntrolabsAmount,objStamps.EntrolabsTappAmount);
                    }
                }
                if (Array.IndexOf(General.GetConfigVal("ENABLE_COMPARE_TRANS_DEPT").ToUpper().Split(','), "RTAFEST") > -1)
                {
                    ResponseBO objStamps = GetRTAFESTTransactions(strFromDate, strToDate, strDate);
                    General.WriteLog("COMPARE_TRANS", " RTAFEST: FROM DATE:" + (strFromDate.Split('-')[2] + "-" + strFromDate.Split('-')[1] + "-" + strFromDate.Split('-')[0]) + ", strToDate:" + (strToDate.Split('-')[2] + "-" + strToDate.Split('-')[1] + "-" + strToDate.Split('-')[0]) + ",TransCount From TAPP:" + objStamps.RTAFESTTappTransCount.ToStr() + ", From Dept:" + objStamps.RTAFESTTransCount.ToStr() + ", Amount:" + objStamps.RTAFESTAmount.ToStr() + ", Tapp amount:" + objStamps.RTAFESTappAmount.ToStr());
                    if (objStamps != null && ((objStamps.RTAFESTTransCount != objStamps.RTAFESTTappTransCount) || objStamps.RTAFESTAmount != objStamps.RTAFESTappAmount))
                    {
                        Console.WriteLine("differenrce found in RTAFEST");
                        strMailBody += PrepareRTAFESTMailBody(objStamps.RTAFESTTransCount, objStamps.RTAFESTTappTransCount, objStamps.RTAFESTAmount, objStamps.RTAFESTappAmount);
                    }
                }
                if (Array.IndexOf(General.GetConfigVal("ENABLE_COMPARE_TRANS_DEPT").ToUpper().Split(','), "IGRS") > -1)
                {
                    ResponseBO objStamps = GetStampsTransactions(strFromDate, strToDate, strDate);
                    General.WriteLog("COMPARE_TRANS", " IGRS: FROM DATE:" + strFromDate + ", strToDate:" + strToDate + ",TransCount From TAPP:" + objStamps.IGRSTappTransCount.ToStr() + ", From Dept:" + objStamps.IGRSTransCount.ToStr() + ", Amount:" + objStamps.IGRSAmount.ToStr() + ", Tapp amount:" + objStamps.IGRSTappAmount.ToStr());
                    if (objStamps != null && ((objStamps.IGRSTransCount != objStamps.IGRSTappTransCount) || objStamps.IGRSAmount != objStamps.IGRSTappAmount))
                    {
                        Console.WriteLine("differenrce found in IGRS");
                        strMailBody += PrepareIGRSMailBody(objStamps.IGRSTransCount, objStamps.IGRSTappTransCount, objStamps.IGRSAmount, objStamps.IGRSTappAmount);
                    }
                }
                if (Array.IndexOf(General.GetConfigVal("ENABLE_COMPARE_TRANS_DEPT").ToUpper().Split(','), "IGRS_DHARANI") > -1)
                {
                    ResponseBO objStamps = GetStampsDharaniTransactions(strFromDate, strToDate, strDate);
                    General.WriteLog("COMPARE_TRANS", " IGRS_DHARANI: FROM DATE:" + strFromDate + ", strToDate:" + strToDate + ",TransCount From TAPP:" + objStamps.DharniTappTransCount.ToStr() + ", From Dept:" + objStamps.DharniTransCount.ToStr() + ", Amount:" + objStamps.DharniAmount.ToStr() + ", Tapp amount:" + objStamps.DharniTappAmount.ToStr());
                    if (objStamps != null && ((objStamps.DharniTransCount != objStamps.DharniTappTransCount) || objStamps.DharniAmount != objStamps.DharniTappAmount))
                    {
                        Console.WriteLine("differenrce found in IGRS Dharani");
                        strMailBody += PrepareIGRSDharaniMailBody(objStamps.DharniTransCount, objStamps.DharniTappTransCount, objStamps.DharniAmount, objStamps.DharniTappAmount);
                    }
                }
                if (Array.IndexOf(General.GetConfigVal("ENABLE_COMPARE_TRANS_DEPT").ToUpper().Split(','), "MESEVA") > -1)
                {
                    ResponseBO objMeseva = GetMeeSevaTransactions(strFromDate, strToDate, strDate);
                    General.WriteLog("COMPARE_TRANS", " MESEVA: FROM DATE:" + strFromDate + ", strToDate:" + strToDate + ",TransCount From TAPP:" + objMeseva.MeeSevaTappTransCount.ToStr() + ", From Dept:" + objMeseva.MeeSevaTransCount.ToStr() + ", Amount:" + objMeseva.MeeSevaAmount.ToStr() + ", Tapp amount:" + objMeseva.MeeSevaTappAmount.ToStr());
                    if (objMeseva != null && ((objMeseva.MeeSevaTransCount != objMeseva.MeeSevaTappTransCount) || objMeseva.MeeSevaAmount != objMeseva.MeeSevaTappAmount))
                    {
                        strMailBody += PrepareMeesevaMailBody(objMeseva.MeeSevaTransCount, objMeseva.MeeSevaTappTransCount, objMeseva.MeeSevaAmount, objMeseva.MeeSevaTappAmount);
                    }
                }
                if (Array.IndexOf(General.GetConfigVal("ENABLE_COMPARE_TRANS_DEPT").ToUpper().Split(','), "ESEVA") > -1)
                {
                    ResponseBO objEseva = GetESevaTransactions(strFromDate, strToDate, strDate);
                    General.WriteLog("COMPARE_TRANS", "ESEVA:FROM DATE:" + strFromDate + ", strToDate:" + strToDate + ", TransCount From TAPP:" + objEseva.EsevaTappTransCount.ToStr() + ", From Dept:" + objEseva.EsevaTransCount.ToStr() + ", Amount:" + objEseva.ESevaAmount.ToStr() + ", Tapp amount:" + objEseva.ESevaTappAmount.ToStr());

                    if (objEseva != null && ((objEseva.EsevaTransCount != objEseva.EsevaTappTransCount) || objEseva.ESevaAmount != objEseva.ESevaTappAmount))
                    {
                        strMailBody += PrepareEsevaMailBody(objEseva.EsevaTransCount, objEseva.EsevaTappTransCount, objEseva.ESevaAmount, objEseva.ESevaTappAmount);
                    }
                }
                if (Array.IndexOf(General.GetConfigVal("ENABLE_COMPARE_TRANS_DEPT").ToUpper().Split(','), "BRMART") > -1)
                {
                    ResponseBO objBRMart = GetBRMARTTransactions(strFromDate, strToDate, strDate);
                    General.WriteLog("COMPARE_TRANS", "BRMART:FROM DATE:" + strFromDate + ", strToDate:" + strToDate + ", TransCount From TAPP:" + objBRMart.BRMartTappTransCount.ToStr() + ", From Dept:" + objBRMart.BRMartTransCount.ToStr() + ", Amount:" + objBRMart.BRMartAmount.ToStr() + ", Tapp amount:" + objBRMart.BRMartTappAmount.ToStr());

                    if (objBRMart != null && ((objBRMart.BRMartTransCount != objBRMart.BRMartTappTransCount) || objBRMart.BRMartAmount != objBRMart.BRMartTappAmount))
                    {
                        strMailBody += PrepareBRMartMailBody(objBRMart.BRMartTransCount, objBRMart.BRMartTappTransCount, objBRMart.BRMartAmount, objBRMart.BRMartTappAmount);
                    }
                }
                if (strMailBody.Length > 0)
                {
                    Console.WriteLine("strMailBody found in IGRS:" + strMailBody);
                    Boolean bln = General.SendMail(strMailTo, strMailCC, strMailBCC, General.GetConfigVal("MAIL_BODY").Replace("!DATE!", strDate) + strMailBody, General.GetConfigVal("MAIL_SUB"), "");
                    Console.WriteLine("Mail sending:" + bln.ToStr());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in CompareTransList==>" + ex.ToStr());
                General.WriteLog("Exception in CompareTransList==>" + ex.ToStr());
            }
        }

        private static ResponseBO GetMeeSevaTransactions(string strFromDate, string strToDate, string strDate)
        {
            ResponseBO objRespBo = new ResponseBO();
            try
            {
                //tapp.MeesevaMobileWebservice objServiceMeeseva = new tapp.MeesevaMobileWebservice();
                meseva.MeesevaMobileWebserviceSoapClient objServiceMeeseva = new meseva.MeesevaMobileWebserviceSoapClient();
                // string strFromDateDB = Convert.ToDateTime(strFromDate).ToString("dd-MMM-yyyy");
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                DataSet dsService = objServiceMeeseva.TappTransactionCount(strFromDate, strToDate);
                objServiceMeeseva = null;
                if (dsService != null && dsService.Tables != null && dsService.Tables.Count > 0 && dsService.Tables[0].Rows.Count > 0 && !dsService.Tables[0].Columns.Contains("Error"))
                {
                    objRespBo.MeeSevaTransCount = dsService.Tables[0].Rows[0]["TransactionCount"].ToStr().ToInt();
                    objRespBo.MeeSevaAmount = dsService.Tables[0].Rows[0]["TransactionAmount"].ToStr().ToDecimal();
                }
                DataTable dtDB = GetPaymentTransactions(strFromDate, strToDate, "MESEVA", "COUNT");
                if (dtDB != null && dtDB.Rows.Count > 0)
                {
                    string countstr = dtDB.Rows[0]["COUNT"].ToString();
                    string stramount = dtDB.Rows[0]["TOTALAMOUNT"].ToString();
                    int Tappcount = 0;
                    int.TryParse(countstr, out Tappcount);
                    decimal damt = 0;
                    decimal.TryParse(stramount, out damt);
                    objRespBo.MeeSevaTappAmount = damt;
                    objRespBo.MeeSevaTappTransCount = Tappcount;
                }
                dsService = null;
            }
            catch (Exception ex)
            {
                General.WriteLog("COMPARE_TRANS", "Exception in GetMeeSevaTransactions==>" + ex.ToStr());
            }
            return objRespBo;
        }

        private static ResponseBO GetESevaTransactions(string strFromDate, string strToDate, string strDate)
        {
            ResponseBO objRespBo = new ResponseBO();
            eseva.MobileReconRequestBean objEsevaServiceReq = new eseva.MobileReconRequestBean();
            eseva.tapp objEsevaService = new eseva.tapp();
            eseva.MobileReconResponceBean objEsevaServiceResp = null;
            try
            {
                objEsevaServiceReq.userId = General.GetConfigVal("ESEVA_USERID");
                objEsevaServiceReq.password = General.GetConfigVal("ESEVA_PASSWORD");
                objEsevaServiceReq.transdate = strFromDate;
                objEsevaServiceResp = objEsevaService.TAppTransactionSummary(objEsevaServiceReq);
                String strResp = string.Empty;
                if (objEsevaServiceResp != null && objEsevaServiceResp.strResponeCode == "000")
                {
                    strResp = objEsevaServiceResp.strTransdata;
                }

                //String strResp = "<TappTransactionSummary><TranSummary><TransactionCount>1</TransactionCount><TransactionAmount>10</TransactionAmount><TransactionUserCharges>0</TransactionUserCharges><TotalAmount>10</TotalAmount></TranSummary></TappTransactionSummary>";
                if (!string.IsNullOrEmpty(strResp))
                {
                    XmlDocument xdocResp = new XmlDocument();
                    xdocResp.LoadXml(strResp);
                    if (xdocResp.DocumentElement.SelectSingleNode("TranSummary/TransactionCount") != null)
                    {
                        objRespBo.EsevaTransCount = xdocResp.DocumentElement.SelectSingleNode("TranSummary/TransactionCount").InnerText.ToInt();
                        objRespBo.ESevaAmount = Convert.ToDecimal(xdocResp.DocumentElement.SelectSingleNode("TranSummary/TotalAmount").InnerText);
                    }
                    DataTable dtDB = GetPaymentTransactions(strFromDate, strToDate, "ESEVA", "COUNT");
                    if (dtDB != null && dtDB.Rows.Count > 0)
                    {
                        string countstr = dtDB.Rows[0]["COUNT"].ToString();
                        string stramount = dtDB.Rows[0]["TOTALAMOUNT"].ToString();
                        int Tappcount = 0;
                        int.TryParse(countstr, out Tappcount);
                        decimal damt = 0;
                        decimal.TryParse(stramount, out damt);
                        objRespBo.ESevaTappAmount = damt;
                        objRespBo.EsevaTappTransCount = Tappcount;
                    }
                    dtDB = null;
                }
            }
            catch (Exception ex)
            {
                General.WriteLog("COMPARE_TRANS", "Exception in GetESevaTransactions==>" + ex.ToStr());
            }
            finally
            {
                objEsevaServiceReq = null;
                objEsevaService = null;
                objEsevaServiceResp = null;
            }
            return objRespBo;
        }

        private static ResponseBO GetBRMARTTransactions(string strFromDate, string strToDate, string strDate)
        {
            ResponseBO objRespBo = new ResponseBO();

            try
            {
                String strBrMartUrl = General.GetConfigVal("BRMART_HISTROY_URL");
                strBrMartUrl = strBrMartUrl.Replace("!FROMDATE!", strFromDate.Split('-')[2] + "-" + strFromDate.Split('-')[1] + "-" + strFromDate.Split('-')[0]);
                strBrMartUrl = strBrMartUrl.Replace("!TODATE!", strToDate.Split('-')[2] + "-" + strToDate.Split('-')[1] + "-" + strToDate.Split('-')[0]);
                String strAPIResp = General.DoWebRequest(strBrMartUrl);
                General.WriteLog("COMPARE_TRANS", "BRMART_PREPAID RESPONSE:" + strAPIResp);
                DataSet dsMArt_Prepaid = new DataSet();
                DataSet dsMArt_Postpaid = new DataSet();
                decimal prepaid_result = 0;
                decimal postpaid_result = 0;
                int prepaid_cnt = 0;
                int postpaid_cnt = 0;
                //strAPIResp = "<response><errorcode>0</errorcode><errortext>Sucess</errortext><Recharges></Recharges></response>";
                if (!string.IsNullOrEmpty(strAPIResp))
                {
                    StringReader theReader = new StringReader(strAPIResp);
                    // dsMArt_Prepaid.ReadXml(@"D:\Applications\GOT\Console Applications\TAppTransactionsList\TAppTransactionsList\brmart.xml");
                    dsMArt_Prepaid.ReadXml(theReader);
                    if (dsMArt_Prepaid != null && dsMArt_Prepaid.Tables != null && dsMArt_Prepaid.Tables["recharge"] != null && dsMArt_Prepaid.Tables["recharge"].Rows.Count > 0)
                    {
                        prepaid_cnt = dsMArt_Prepaid.Tables["recharge"].Select("Status='0'").Count();
                        prepaid_result = dsMArt_Prepaid.Tables["recharge"].AsEnumerable().Where(r => r.Field<string>("Status") == "0").Sum(x => Convert.ToDecimal(x["Amount"]));
                        //objRespBo.BRMartAmount = prepaid_result.ToDouble();
                    }

                    String strBrMartUrl_Postpaid = General.GetConfigVal("BRMART_HISTROY_POSTPAID_URL");
                    strBrMartUrl_Postpaid = strBrMartUrl_Postpaid.Replace("!FROMDATE!", strFromDate.Split('-')[2] + "-" + strFromDate.Split('-')[1] + "-" + strFromDate.Split('-')[0]);
                    strBrMartUrl_Postpaid = strBrMartUrl_Postpaid.Replace("!TODATE!", strToDate.Split('-')[2] + "-" + strToDate.Split('-')[1] + "-" + strToDate.Split('-')[0]);
                    String strAPIResp_PP = General.DoWebRequest(strBrMartUrl_Postpaid);
                    General.WriteLog("COMPARE_TRANS", "BRMART_POSTPAID RESPONSE:" + strAPIResp_PP);
                    if (!string.IsNullOrEmpty(strAPIResp_PP))
                    {
                        StringReader theReader_PP = new StringReader(strAPIResp_PP);
                        // dsMArt_Postpaid.ReadXml(@"D:\Applications\GOT\Console Applications\TAppTransactionsList\TAppTransactionsList\brmartpostpaid.xml");
                        dsMArt_Postpaid.ReadXml(theReader_PP);
                        if (dsMArt_Postpaid != null && dsMArt_Postpaid.Tables != null && dsMArt_Postpaid.Tables["recharge"] != null && dsMArt_Postpaid.Tables["recharge"].Rows.Count > 0)
                        {
                            postpaid_cnt = dsMArt_Postpaid.Tables["recharge"].Select("Status='0'").Count();
                            postpaid_result = dsMArt_Postpaid.Tables["recharge"].AsEnumerable().Where(r => r.Field<string>("Status") == "0").Sum(x => Convert.ToDecimal(x["Amount"]));

                        }
                    }
                    objRespBo.BRMartTransCount = prepaid_cnt + postpaid_cnt;
                    objRespBo.BRMartAmount = prepaid_result + postpaid_result;


                    DataTable dtDB = GetPaymentTransactions(strFromDate, strToDate, "MARTCONNECT", "COUNT");
                    if (dtDB != null && dtDB.Rows.Count > 0)
                    {
                        string countstr = dtDB.Rows[0]["COUNT"].ToString();
                        string stramount = dtDB.Rows[0]["TOTALAMOUNT"].ToString();
                        int Tappcount = 0;
                        int.TryParse(countstr, out Tappcount);
                        decimal damt = 0;
                        decimal.TryParse(stramount, out damt);
                        objRespBo.BRMartTappTransCount = Tappcount;
                        objRespBo.BRMartTappAmount = damt;
                    }
                    dtDB = null;
                }
            }
            catch (Exception ex)
            {
                General.WriteLog("COMPARE_TRANS", "Exception in GetBRMARTTransactions==>" + ex.ToStr());
            }
            finally
            {

            }
            return objRespBo;
        }

        private static ResponseBO GetStampsTransactions(string strFromDate, string strToDate, string strDate)
        {
            ResponseBO objRespBo = new ResponseBO();
            try
            {
                String strStampsUrl = General.GetConfigVal("STAMPS_GET_SUMMARY_URL");
                string strHashCode = string.Empty;
                string strSaltKey = string.Empty;
                GetTransList.GetSecurityHashCode(ref strHashCode, ref strSaltKey);
                strStampsUrl = strStampsUrl.Replace("{transdate}", strFromDate);
                strStampsUrl = strStampsUrl.Replace("{hashcode}", strHashCode).Replace("{saltkey}", strSaltKey);
                string strResponse = General.DoRequest(strStampsUrl, "", "POST", "");
                General.WriteLog("COMPARE_TRANS", "STAMPS RESPONSE:" + strResponse);
                StampsSummaryInfo objResponse = null;
                if (!string.IsNullOrEmpty(strResponse))
                {
                    objResponse = JsonConvert.DeserializeObject<StampsSummaryInfo>(strResponse);
                    if (objResponse != null && objResponse.code == "200" && objResponse.data.Count > 0)
                    {
                        var summary = objResponse.data.Where(x => x.purposeCode == "0").FirstOrDefault();
                        objRespBo.IGRSAmount = Convert.ToDecimal(summary.amount);
                        objRespBo.IGRSTransCount = summary.count.ToInt();
                    }
                }
                IGRSTransListReq objReq = new IGRSTransListReq();
                objReq.FromDate = strFromDate.Split('-')[2] + "-" + strFromDate.Split('-')[1] + "-" + strFromDate.Split('-')[0];
                objReq.ToDate = strFromDate.Split('-')[2] + "-" + strFromDate.Split('-')[1] + "-" + strFromDate.Split('-')[0];
                objReq.Type = "";

                DataTable dtDB = GetIGRSTransactions(objReq, "COUNT");
                Console.WriteLine("IGRS ROW CONT:" + dtDB.Rows.Count.ToStr());
                if (dtDB != null && dtDB.Rows.Count > 0)
                {
                    Console.WriteLine("countstr:" + dtDB.Rows[0]["COUNT"].ToString() + ",stramount:" + dtDB.Rows[0]["TOTALAMOUNT"].ToString());
                    string countstr = dtDB.Rows[0]["COUNT"].ToString();
                    string stramount = dtDB.Rows[0]["TOTALAMOUNT"].ToString();
                    int Tappcount = 0;
                    int.TryParse(countstr, out Tappcount);
                    decimal damt = 0;
                    decimal.TryParse(stramount, out damt);
                    objRespBo.IGRSTappTransCount = Tappcount;
                    objRespBo.IGRSTappAmount = damt;
                }
                dtDB = null;
            }
            catch (Exception ex)
            {
                General.WriteLog("COMPARE_TRANS", "Exception in GetStampsTransactions==>" + ex.ToStr());
            }
            finally
            {

            }
            return objRespBo;
        }

        private static ResponseBO GetStampsDharaniTransactions(string strFromDate, string strToDate, string strDate)
        {
            ResponseBO objRespBo = new ResponseBO();
            try
            {
                String strStampsUrl = General.GetConfigVal("STAMPS_DH_GET_SUMMARY_URL");
                string strHashCode = string.Empty;
                string strSaltKey = string.Empty;
                GetTransList.GetSecurityHashCode(ref strHashCode, ref strSaltKey);
                strStampsUrl = strStampsUrl.Replace("{transdate}", strFromDate);
                strStampsUrl = strStampsUrl.Replace("{hashcode}", strHashCode).Replace("{saltkey}", strSaltKey);
                string strResponse = General.DoRequest(strStampsUrl, "", "POST", "");
                General.WriteLog("COMPARE_TRANS", "STAMPS DHARANI URL:" + strStampsUrl + Environment.NewLine + " RESPONSE:" + strResponse);
                StampsSummaryInfo objResponse = null;
                if (!string.IsNullOrEmpty(strResponse))
                {
                    objResponse = JsonConvert.DeserializeObject<StampsSummaryInfo>(strResponse);
                    if (objResponse != null && objResponse.statuscode == "200" && objResponse.data.Count > 0)
                    {
                        // var summary = objResponse.data.Where(x => x.purposeCode == "0").FirstOrDefault();

                        objRespBo.DharniAmount = objResponse.data.Sum(x => Convert.ToDecimal(x.amount));// Convert.ToDecimal(summary.amount);
                        objRespBo.DharniTransCount = objResponse.data.Sum(x => Convert.ToInt32(x.count));//summary.count.ToInt();
                    }
                }
                IGRSTransListReq objReq = new IGRSTransListReq();
                objReq.FromDate = strFromDate.Split('-')[2] + "-" + strFromDate.Split('-')[1] + "-" + strFromDate.Split('-')[0];
                objReq.ToDate = strFromDate.Split('-')[2] + "-" + strFromDate.Split('-')[1] + "-" + strFromDate.Split('-')[0];
                objReq.Type = "";

                DataTable dtDB = GetIGRSDharaniTransactions(objReq, "COUNT");
                Console.WriteLine("IGRS DHARANI ROW CONT:" + dtDB.Rows.Count.ToStr());
                if (dtDB != null && dtDB.Rows.Count > 0)
                {
                    Console.WriteLine("countstr:" + dtDB.Rows[0]["COUNT"].ToString() + ",stramount:" + dtDB.Rows[0]["TOTALAMOUNT"].ToString());
                    string countstr = dtDB.Rows[0]["COUNT"].ToString();
                    string stramount = dtDB.Rows[0]["TOTALAMOUNT"].ToString();
                    int Tappcount = 0;
                    int.TryParse(countstr, out Tappcount);
                    decimal damt = 0;
                    decimal.TryParse(stramount, out damt);
                    objRespBo.DharniTappTransCount = Tappcount;
                    objRespBo.DharniTappAmount = damt;
                }
                dtDB = null;
            }
            catch (Exception ex)
            {
                General.WriteLog("COMPARE_TRANS", "Exception in GetStampsDharaniTransactions==>" + ex.ToStr());
            }
            finally
            {

            }
            return objRespBo;
        }

        private static ResponseBO GetRTAFESTTransactions(string strFromDate, string strToDate, string strDate)
        {
            ResponseBO objRespBo = new ResponseBO();
            try
            {
                IGRSTransListReq objReq = new IGRSTransListReq();
                objReq.FromDate = strFromDate.Split('-')[2] + "-" + strFromDate.Split('-')[1] + "-" + strFromDate.Split('-')[0];
                objReq.ToDate = strToDate.Split('-')[2] + "-" + strToDate.Split('-')[1] + "-" + strToDate.Split('-')[0];
                objReq.Type = "";
                String strStampsUrl = General.GetConfigVal("RTAFEST_GET_SUMMARY_URL");
                string strHashCode = string.Empty;
                string strSaltKey = string.Empty;
                // GetTransList.GetSecurityHashCode(ref strHashCode, ref strSaltKey);
                string strPostBody = "{\"fromDate\" :\"" + objReq.FromDate + "\", \"toDate\" :\"" + objReq.ToDate + "\"}";
                string strResponse = General.DoRequest(strStampsUrl, strPostBody, "POST", "application/json");
                General.WriteLog("COMPARE_TRANS", "RTAFEST RESPONSE:" + strResponse);
                RTAFESTSummaryInfo objResponse = null;
                if (!string.IsNullOrEmpty(strResponse))
                {
                    objResponse = JsonConvert.DeserializeObject<RTAFESTSummaryInfo>(strResponse);
                    if (objResponse != null && objResponse.Code == "0")
                    {
                        objRespBo.RTAFESTAmount = Convert.ToDecimal(objResponse.Amount);
                        objRespBo.RTAFESTTransCount = objResponse.Count.ToInt();
                    }
                }


                DataTable dtDB = GetRTAFESTTransactions(objReq, "COUNT");
                Console.WriteLine("RTAFEST ROW CONT:" + dtDB.Rows.Count.ToStr());
                if (dtDB != null && dtDB.Rows.Count > 0)
                {
                    Console.WriteLine("countstr:" + dtDB.Rows[0]["COUNT"].ToString() + ",stramount:" + dtDB.Rows[0]["TOTALAMOUNT"].ToString());
                    string countstr = dtDB.Rows[0]["COUNT"].ToString();
                    string stramount = dtDB.Rows[0]["TOTALAMOUNT"].ToString();
                    int Tappcount = 0;
                    int.TryParse(countstr, out Tappcount);
                    decimal damt = 0;
                    decimal.TryParse(stramount, out damt);
                    objRespBo.RTAFESTTappTransCount = Tappcount;
                    objRespBo.RTAFESTappAmount = damt;
                }
                dtDB = null;
            }
            catch (Exception ex)
            {
                General.WriteLog("COMPARE_TRANS", "Exception in GetRTAFESTTransactions==>" + ex.ToStr());
            }
            finally
            {

            }
            return objRespBo;
        }

        private static ResponseBO GetEntrolabsRevenue(string strFromDate, string strToDate)
        {
            ResponseBO objRespBo = new ResponseBO();
            try
            {
                IGRSTransListReq objReq = new IGRSTransListReq();
                objReq.FromDate = strFromDate.Split('-')[2] + "-" + strFromDate.Split('-')[1] + "-" + strFromDate.Split('-')[0];
                objReq.ToDate = strToDate.Split('-')[2] + "-" + strToDate.Split('-')[1] + "-" + strToDate.Split('-')[0];
                objReq.Type = "";
                String strStampsUrl = General.GetConfigVal("ENTROLABS_GET_REVENUE_URL");
                EntrolabDeptReq objDeptReq = new EntrolabDeptReq()
                {
                    fromdate = objReq.FromDate,
                    todate = objReq.ToDate
                };

                Hashtable objHash = new Hashtable();
                objHash.Add("ApiKey", General.GetConfigVal("ENTROLABS_API_KEY"));
                
                string strPostBody = JsonConvert.SerializeObject(objDeptReq);
                string strResponse = General.DoRequest(strStampsUrl, strPostBody, "POST", "application/json", objHash);
                EntrolabsDeptRes objResponse = null;
                if (!string.IsNullOrEmpty(strResponse))
                {
                    objResponse = JsonConvert.DeserializeObject<EntrolabsDeptRes>(strResponse);
                    if (objResponse != null && objResponse.result.Trim().ToUpper() == "SUCCESS")
                    {
                        objRespBo.EntrolabsAmount = Convert.ToDecimal(objResponse.amount);
                        objRespBo.EntrolabsCount = objResponse.count.ToInt();
                    }
                }
                IGRSTransListReq objReqNew = new IGRSTransListReq();
                objReqNew.FromDate = strFromDate.Split('-')[2] + "-" + strFromDate.Split('-')[1] + "-" + strFromDate.Split('-')[0];
                objReqNew.ToDate = strFromDate.Split('-')[2] + "-" + strFromDate.Split('-')[1] + "-" + strFromDate.Split('-')[0];
                objReqNew.Type = "";
                DataTable dtDB = GetEntrolabsTransactions(objReq, "COUNT");
                Console.WriteLine("Entrolabs ROW CONT:" + dtDB.Rows.Count.ToStr());
                if (dtDB != null && dtDB.Rows.Count > 0)
                {
                    Console.WriteLine("countstr:" + dtDB.Rows[0]["COUNT"].ToString() + ",stramount:" + dtDB.Rows[0]["TOTALAMOUNT"].ToString());
                    string countstr = dtDB.Rows[0]["COUNT"].ToString();
                    string stramount = dtDB.Rows[0]["TOTALAMOUNT"].ToString();
                    int Tappcount = 0;
                    int.TryParse(countstr, out Tappcount);
                    decimal damt = 0;
                    decimal.TryParse(stramount, out damt);
                    objRespBo.EntrolabsTappCount = Tappcount;
                    objRespBo.EntrolabsTappAmount = damt;
                }
                dtDB = null;
            }
            catch (Exception ex)
            {
                General.WriteLog("COMPARE_TRANS", "Exception in GetEntrolabsRevenue==>" + ex.ToStr());
            }
            return objRespBo;
        }

        private static string PrepareMeesevaMailBody(int iMeseva, int iTapp, decimal dAmtMeseva, decimal dAmtMesevaTapp)
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("</br></br>").Append("MESEVA").Append("<table border='1'>");
            sbHtml.Append("<tr>");
            sbHtml.Append("<th>");
            sbHtml.Append("MESEVA");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("T-APP");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("MESEVA AMOUNT");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("TAPP AMOUNT");
            sbHtml.Append("</th>");
            sbHtml.Append("</tr>");

            //sbHtml.Append("</tr>");
            sbHtml.Append("<tr>");
            sbHtml.Append("<td>");
            sbHtml.Append(iMeseva.ToStr());
            sbHtml.Append("</td>");
            sbHtml.Append("<td>");
            sbHtml.Append(iTapp.ToStr());
            sbHtml.Append("</td>");
            sbHtml.Append("<td>");
            sbHtml.Append(dAmtMeseva.ToStr());
            sbHtml.Append("</td>");
            sbHtml.Append("<td>");
            sbHtml.Append(dAmtMesevaTapp.ToStr());
            sbHtml.Append("</td>");
            sbHtml.Append("</tr>");
            sbHtml.Append("</table>");
            return sbHtml.ToStr();
        }

        private static string PrepareEsevaMailBody(int iEseva, int iTapp, decimal dAmteseva, decimal dAmtesevaTapp)
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("</br></br>").Append("ESEVA").Append("<table border='1'>");
            sbHtml.Append("<tr>");
            sbHtml.Append("<th>");
            sbHtml.Append("ESEVA");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("T-APP");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("ESEVA AMOUNT");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("TAPP AMOUNT");
            sbHtml.Append("</th>");
            sbHtml.Append("</tr>");

            sbHtml.Append("<tr>");
            sbHtml.Append("<td>");
            sbHtml.Append(iEseva.ToStr());
            sbHtml.Append("</td>");
            sbHtml.Append("<td>");
            sbHtml.Append(iTapp.ToStr());
            sbHtml.Append("</td>");
            sbHtml.Append("<td>");
            sbHtml.Append(dAmteseva.ToStr());
            sbHtml.Append("</td>");
            sbHtml.Append("<td>");
            sbHtml.Append(dAmtesevaTapp.ToStr());
            sbHtml.Append("</td>");
            sbHtml.Append("</tr>");
            sbHtml.Append("</table>");
            return sbHtml.ToStr();
        }

        private static string PrepareBRMartMailBody(int iBrmart, int iTapp, decimal dAmtbrmart, decimal dAmtbrmartTapp)
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("</br></br>").Append("BRMART").Append("<table border='1'>");
            sbHtml.Append("<tr>");
            sbHtml.Append("<th>");
            sbHtml.Append("BRMART");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("T-APP");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("BRMART AMOUNT");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("TAPP AMOUNT");
            sbHtml.Append("</th>");
            sbHtml.Append("</tr>");
            sbHtml.Append("<tr>");

            sbHtml.Append("<td>");
            sbHtml.Append(iBrmart.ToStr());
            sbHtml.Append("</td>");

            sbHtml.Append("<td>");
            sbHtml.Append(iTapp.ToStr());
            sbHtml.Append("</td>");

            sbHtml.Append("<td>");
            sbHtml.Append(dAmtbrmart.ToStr());
            sbHtml.Append("</td>");
            sbHtml.Append("<td>");
            sbHtml.Append(dAmtbrmartTapp.ToStr());
            sbHtml.Append("</td>");
            sbHtml.Append("</tr>");
            sbHtml.Append("</table>");
            return sbHtml.ToStr();
        }

        private static string PrepareIGRSMailBody(int iIGRS, int iTapp, decimal dAmtIGRS, decimal dAmtIGRSTapp)
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("</br></br>").Append("IGRS").Append("<table border='1'>");
            sbHtml.Append("<tr>");
            sbHtml.Append("<th>");
            sbHtml.Append("IGRS");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("T-APP");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("IGRS AMOUNT");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("TAPP AMOUNT");
            sbHtml.Append("</th>");
            sbHtml.Append("</tr>");
            sbHtml.Append("<tr>");

            sbHtml.Append("<td>");
            sbHtml.Append(iIGRS.ToStr());
            sbHtml.Append("</td>");

            sbHtml.Append("<td>");
            sbHtml.Append(iTapp.ToStr());
            sbHtml.Append("</td>");

            sbHtml.Append("<td>");
            sbHtml.Append(dAmtIGRS.ToStr());
            sbHtml.Append("</td>");
            sbHtml.Append("<td>");
            sbHtml.Append(dAmtIGRSTapp.ToStr());
            sbHtml.Append("</td>");
            sbHtml.Append("</tr>");
            sbHtml.Append("</table>");
            return sbHtml.ToStr();
        }

        private static string PrepareIGRSDharaniMailBody(int iIGRS, int iTapp, decimal dAmtIGRS, decimal dAmtIGRSTapp)
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("</br></br>").Append("DHARANI").Append("<table border='1'>");
            sbHtml.Append("<tr>");
            sbHtml.Append("<th>");
            sbHtml.Append("DHARANI");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("T-APP");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("DHARANI AMOUNT");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("TAPP AMOUNT");
            sbHtml.Append("</th>");
            sbHtml.Append("</tr>");
            sbHtml.Append("<tr>");

            sbHtml.Append("<td>");
            sbHtml.Append(iIGRS.ToStr());
            sbHtml.Append("</td>");

            sbHtml.Append("<td>");
            sbHtml.Append(iTapp.ToStr());
            sbHtml.Append("</td>");

            sbHtml.Append("<td>");
            sbHtml.Append(dAmtIGRS.ToStr());
            sbHtml.Append("</td>");
            sbHtml.Append("<td>");
            sbHtml.Append(dAmtIGRSTapp.ToStr());
            sbHtml.Append("</td>");
            sbHtml.Append("</tr>");
            sbHtml.Append("</table>");
            return sbHtml.ToStr();
        }

        private static string PrepareRTAFESTMailBody(int iRTAFEST, int iTapp, decimal dAmtRTAFST, decimal dAmtRTAFSTTapp)
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("</br></br>").Append("IGRS").Append("<table border='1'>");
            sbHtml.Append("<tr>");
            sbHtml.Append("<th>");
            sbHtml.Append("RTAFEST");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("T-APP");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("POSIDEX AMOUNT");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("TAPP AMOUNT");
            sbHtml.Append("</th>");
            sbHtml.Append("</tr>");
            sbHtml.Append("<tr>");

            sbHtml.Append("<td>");
            sbHtml.Append(iRTAFEST.ToStr());
            sbHtml.Append("</td>");

            sbHtml.Append("<td>");
            sbHtml.Append(iTapp.ToStr());
            sbHtml.Append("</td>");

            sbHtml.Append("<td>");
            sbHtml.Append(dAmtRTAFST.ToStr());
            sbHtml.Append("</td>");
            sbHtml.Append("<td>");
            sbHtml.Append(dAmtRTAFSTTapp.ToStr());
            sbHtml.Append("</td>");
            sbHtml.Append("</tr>");
            sbHtml.Append("</table>");
            return sbHtml.ToStr();
        }

        private static string PrepareEntrolabsMailBody(int iEntroLabs, int iTapp, decimal dAmtEntrolabs, decimal dAmtEntroLabsTapp)
        {
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.Append("</br></br>").Append("Entrolabs Temples").Append("<table border='1'>");
            sbHtml.Append("<tr>");
            sbHtml.Append("<th>");
            sbHtml.Append("ENTROLABS");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("T-APP");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("ENTROLABS AMOUNT");
            sbHtml.Append("</th>");
            sbHtml.Append("<th>");
            sbHtml.Append("TAPP AMOUNT");
            sbHtml.Append("</th>");
            sbHtml.Append("</tr>");
            sbHtml.Append("<tr>");

            sbHtml.Append("<td>");
            sbHtml.Append(iEntroLabs.ToStr());
            sbHtml.Append("</td>");

            sbHtml.Append("<td>");
            sbHtml.Append(iTapp.ToStr());
            sbHtml.Append("</td>");

            sbHtml.Append("<td>");
            sbHtml.Append(dAmtEntrolabs.ToStr());
            sbHtml.Append("</td>");
            sbHtml.Append("<td>");
            sbHtml.Append(dAmtEntroLabsTapp.ToStr());
            sbHtml.Append("</td>");
            sbHtml.Append("</tr>");
            sbHtml.Append("</table>");
            return sbHtml.ToStr();
        }

        private static DataTable GetPaymentTransactions(string strFromDate, string strToDate, string strType, string strMode)
        {
            DataTable dt = new DataTable();
            long lTimeTaken = -1, lStart = DateTime.Now.Ticks;
            try
            {
                using (DBFactory objDB = new DBFactory("DSN_PAYMENT_TRANS"))
                {
                    objDB.AddInParam("FROMDATE", SqlType.VarChar, DateTime.ParseExact(strFromDate, "dd-MM-yyyy", null).ToString("dd-MMM-yyyy"));
                    objDB.AddInParam("TODATE", SqlType.VarChar, DateTime.ParseExact(strFromDate, "dd-MM-yyyy", null).ToString("dd-MMM-yyyy"));
                    objDB.AddInParam("SERVICE", SqlType.VarChar, strType);
                    objDB.AddInParam("MODE", SqlType.VarChar, strMode);
                    objDB.RunProc("PAYMENT_TRANS_BY_TRANSDATEnSERVICE_GET", out dt);
                }
            }
            catch (Exception ex)
            {
                General.WriteLog("COMPARE_TRANS", "Exception in GetPaymentTransactions==>" + ex.ToStr());
            }
            finally
            {
                if (lStart > 0)
                    lTimeTaken = (DateTime.Now.Ticks - lStart) / 10000;
                General.WriteLog("DB_TT", "PAYMENT_TRANS_BY_TRANSDATEnSERVICE_GET==>" + lTimeTaken.ToStr());
            }
            return dt;
        }

        private static DataTable GetIGRSTransactions(IGRSTransListReq obj, string strMode)
        {
            DataTable dt = new DataTable();
            long lTimeTaken = -1, lStart = DateTime.Now.Ticks;
            try
            {
                using (DBFactory objDB = new DBFactory("DSN_PAYMENT_TRANS"))
                {
                    objDB.AddInParam("FROMDATE", SqlType.VarChar, obj.FromDate.ToStr());
                    objDB.AddInParam("TODATE", SqlType.VarChar, obj.ToDate.ToStr());
                    objDB.AddInParam("SUBSERVICE", SqlType.VarChar, obj.Type.ToStr());
                    objDB.AddInParam("MODE", SqlType.VarChar, strMode.ToStr());
                    objDB.RunProc("PAYMENT_TRANS_IGRS_GET", out dt);
                }
            }
            catch (Exception ex)
            {
                General.WriteLog("COMPARE_TRANS", "Exception in GetIGRSTransactions==>" + ex.ToStr());
            }
            finally
            {
                if (lStart > 0)
                    lTimeTaken = (DateTime.Now.Ticks - lStart) / 10000;
                General.WriteLog("DB_TT", "PAYMENT_TRANS_IGRS_GET==>" + lTimeTaken.ToStr());
            }
            return dt;
        }
        private static DataTable GetIGRSDharaniTransactions(IGRSTransListReq obj, string strMode)
        {
            DataTable dt = new DataTable();
            long lTimeTaken = -1, lStart = DateTime.Now.Ticks;
            try
            {
                using (DBFactory objDB = new DBFactory("DSN_PAYMENT_TRANS"))
                {
                    objDB.AddInParam("FROMDATE", SqlType.VarChar, obj.FromDate.ToStr());
                    objDB.AddInParam("TODATE", SqlType.VarChar, obj.ToDate.ToStr());
                    objDB.AddInParam("SUBSERVICE", SqlType.VarChar, obj.Type.ToStr());
                    objDB.AddInParam("MODE", SqlType.VarChar, strMode.ToStr());
                    objDB.RunProc("PAYMENT_TRANS_IGRS_DHARANI_GET", out dt);
                }
            }
            catch (Exception ex)
            {
                General.WriteLog("COMPARE_TRANS", "Exception in GetIGRSTransactions==>" + ex.ToStr());
            }
            finally
            {
                if (lStart > 0)
                    lTimeTaken = (DateTime.Now.Ticks - lStart) / 10000;
                General.WriteLog("DB_TT", "PAYMENT_TRANS_IGRS_GET==>" + lTimeTaken.ToStr());
            }
            return dt;
        }
        private static DataTable GetRTAFESTTransactions(IGRSTransListReq obj, string strMode)
        {
            DataTable dt = new DataTable();
            long lTimeTaken = -1, lStart = DateTime.Now.Ticks;
            try
            {
                using (DBFactory objDB = new DBFactory("DSN_PAYMENT_TRANS"))
                {
                    objDB.AddInParam("FROMDATE", SqlType.VarChar, obj.FromDate.ToStr());
                    objDB.AddInParam("TODATE", SqlType.VarChar, obj.ToDate.ToStr());
                    objDB.AddInParam("SUBSERVICE", SqlType.VarChar, obj.Type.ToStr());
                    objDB.AddInParam("MODE", SqlType.VarChar, strMode.ToStr());
                    objDB.RunProc("PAYMENT_TRANS_RECON_RTAFEST_GET", out dt);
                }
            }
            catch (Exception ex)
            {
                General.WriteLog("COMPARE_TRANS", "Exception in GetRTAFESTTransactions==>" + ex.ToStr());
            }
            finally
            {
                if (lStart > 0)
                    lTimeTaken = (DateTime.Now.Ticks - lStart) / 10000;
                General.WriteLog("DB_TT", "PAYMENT_TRANS_RTAFEST_GET==>" + lTimeTaken.ToStr());
            }
            return dt;
        }

        private static DataTable GetEntrolabsTransactions(IGRSTransListReq obj,string strMode)
        {
            DataTable dt = new DataTable();
            long lTimeTaken = -1, lStart = DateTime.Now.Ticks;
            try
            {
                using (DBFactory objDB = new DBFactory("DSN_PAYMENT_TRANS"))
                {
                    objDB.AddInParam("FROMDATE", SqlType.VarChar, obj.FromDate.ToStr());
                    objDB.AddInParam("TODATE", SqlType.VarChar, obj.ToDate.ToStr());
                    objDB.AddInParam("SUBSERVICE", SqlType.VarChar, obj.Type.ToStr());
                    objDB.AddInParam("MODE", SqlType.VarChar, strMode.ToStr());
                    objDB.RunProc("PAYMENT_TRANS_ENTROLABS_GET", out dt);
                }
            }
            catch(Exception ex)
            {
                General.WriteLog("COMPARE_TRANS", "Exception in GetEntrolabsTransactions==>" + ex.ToStr());
            }
            finally
            {
                if (lStart > 0)
                    lTimeTaken = (DateTime.Now.Ticks - lStart) / 10000;
                General.WriteLog("DB_TT", "PAYMENT_TRANS_ENTROLABS_GET==>" + lTimeTaken.ToStr());
            }
            return dt;
        }
    }
    public class ResponseBO
    {
        public int MeeSevaTransCount { get; set; }
        public int MeeSevaTappTransCount { get; set; }
        public decimal MeeSevaAmount { get; set; }
        public decimal MeeSevaTappAmount { get; set; }
        public int EsevaTransCount { get; set; }
        public int EsevaTappTransCount { get; set; }
        public decimal ESevaAmount { get; set; }
        public decimal ESevaTappAmount { get; set; }
        public int BRMartTransCount { get; set; }
        public int BRMartTappTransCount { get; set; }
        public decimal BRMartAmount { get; set; }
        public decimal BRMartTappAmount { get; set; }
        public int IGRSTransCount { get; set; }
        public int IGRSTappTransCount { get; set; }
        public decimal IGRSAmount { get; set; }
        public decimal IGRSTappAmount { get; set; }

        public int RTAFESTTransCount { get; set; }
        public int RTAFESTTappTransCount { get; set; }
        public decimal RTAFESTAmount { get; set; }
        public decimal RTAFESTappAmount { get; set; }

        public int DharniTransCount { get; set; }
        public int DharniTappTransCount { get; set; }
        public decimal DharniAmount { get; set; }
        public decimal DharniTappAmount { get; set; }

        public int EntrolabsCount { get; set; }
        public decimal EntrolabsAmount { get; set; }
        public int EntrolabsTappCount { get; set; }
        public decimal EntrolabsTappAmount { get; set; }
    }

    public class Datum
    {
        public string purposeCode { get; set; }
        public string purposeName { get; set; }
        public string count { get; set; }
        public string amount { get; set; }
    }

    public class StampsSummaryInfo
    {
        public string code { get; set; }
        public string desc { get; set; }

        public string statuscode { get; set; }
        public string statusMessage { get; set; }

        public List<Datum> data { get; set; }
    }

    public class IGRSTransListReq
    {
        public string Type { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }

    public class RTAFESTSummaryInfo
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Count { get; set; }
        public string Amount { get; set; }
    }

    public class EntrolabDeptReq
    {
        public string gettotals = "1";
        public string fromdate { get; set; }
        public string todate { get; set; }
    }

    public class EntrolabsDeptRes
    {
        public string result { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public string count { get; set; }
        public string amount { get; set; }
    }
}
