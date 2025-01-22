using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CommonHelper;
using IMI.SqlWrapper;
using System.Xml;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using IMI.Logger;
using System.Collections;

namespace TAppTransactionsList
{
    class GetTransList
    {
        public static void GetTransactionsList()
        {
            try
            {
                String strFromDate = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");
                String strToDate = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");
                if (General.GetConfigVal("ENABLE_CUSTOM_DATE").ToUpper() == "Y" && General.GetConfigVal("CUSTOM_FROM_DATE") != "")
                {
                    strFromDate = Convert.ToDateTime(General.GetConfigVal("CUSTOM_FROM_DATE")).ToString("dd-MM-yyyy");
                    strToDate = Convert.ToDateTime(General.GetConfigVal("CUSTOM_TO_DATE")).ToString("dd-MM-yyyy");
                }
                //strFromDate = strFromDate + " 00:00:00";
                //strToDate = strToDate + " 23:59:59";
                Console.WriteLine("From Date:" + strFromDate);
                Console.WriteLine("To Date:" + strToDate);
                if (Array.IndexOf(General.GetConfigVal("ENABLE_TRANS_LIST_DEPT").ToUpper().Split(','), "ENTROLABS_TEMPLES") > -1)
                {
                    #region EntrolLabs
                    EntroLabsDeptRes objResponse = null;
                    try
                    {
                        string strurl = General.GetConfigVal("ENTROLABS_GET_REVENUE_URL");
                        string[] srvArray = General.GetConfigVal("ENTROLABS_TEMPLES_SERVICES").Split(',');
                        foreach(string srv in srvArray)
                        {
                            EntroLabsDeptReq objReq = new EntroLabsDeptReq()
                            {
                                gettempleTransactionslist = "1",
                                fromdate = strFromDate.Split('-')[2] + "-" + strFromDate.Split('-')[1] + "-" + strFromDate.Split('-')[0],
                                todate = strToDate.Split('-')[2] + "-" + strToDate.Split('-')[1] + "-" + strToDate.Split('-')[0],
                                service = srv,
                                temple_code = "",
                            };
                            var JData = JsonConvert.SerializeObject(objReq);
                            Hashtable objHash = new Hashtable();
                            objHash.Add("ApiKey", General.GetConfigVal("ENTROLABS_API_KEY"));
                            string strResponse = General.DoRequest(strurl, JData, "POST", "application/json", objHash);
                            if (!string.IsNullOrEmpty(strResponse))
                            {
                                objResponse = JsonConvert.DeserializeObject<EntroLabsDeptRes>(strResponse);
                            }
                            if (objResponse != null && objResponse.result.ToStr().Trim().ToLower() == "success" && objResponse.data != null && objResponse.data.Count > 0)
                            {

                                DataTable dtTrans = objResponse.data.ToDataTable<EntroLabs>();
                                if (dtTrans != null && dtTrans.Rows.Count > 0)
                                {
                                    dtTrans.Columns.Add("ID", typeof(decimal)).SetOrdinal(0);
                                    SqlConnection conn = new SqlConnection(General.GetConfigVal("DSN_GOT_TRANS"));
                                    long lTimeTaken1 = -1, lStart1 = DateTime.Now.Ticks;
                                    try
                                    {
                                        conn.Open();
                                        SqlTransaction transaction = conn.BeginTransaction();
                                        SqlBulkCopy copy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction);
                                        copy.DestinationTableName = "TAPP_RECON_ENTROLABS_TRANS";
                                        copy.WriteToServer(dtTrans);
                                        transaction.Commit();
                                    }
                                    catch (Exception ex)
                                    {
                                        General.WriteLog("GET_TRANS_EX", "EXception while bulk inserting in  ENTROLABS_TEMPLES Trans." + ex.ToStr());
                                    }
                                    finally
                                    {
                                        if (lStart1 > 0)
                                            lTimeTaken1 = (DateTime.Now.Ticks - lStart1) / 10000;
                                        General.WriteLog("GET_TRANS", "ENTROLABS bulk insertion time taken:" + lTimeTaken1.ToStr());
                                        conn.Close();
                                        dtTrans = null;
                                    }
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        General.WriteLog("GET_TRANS_EX", "EXception while  ENTROLABS_TEMPLES Trans calling method." + ex.ToStr());
                    }
                    #endregion
                }
                if (Array.IndexOf(General.GetConfigVal("ENABLE_TRANS_LIST_DEPT").ToUpper().Split(','), "RTAFEST") > -1)
                {
                    #region RTAFEST
                    RTAFESTTransInfo objResponse = null;
                    try
                    {
                        string strHashCode = string.Empty;
                        string strSaltKey = string.Empty;
                        GetSecurityHashCode(ref strHashCode, ref strSaltKey);
                        string strurl = General.GetConfigVal("RTAFEST_GET_TRANSLIST_URL");
                        //strurl = strurl.Replace("{hashcode}", strHashCode).Replace("{saltkey}", strSaltKey);
                        string strPostBody = "{\"fromDate\" :\"" + strFromDate.Split('-')[2] + "-" + strFromDate.Split('-')[1] + "-" + strFromDate.Split('-')[0] + "\", \"toDate\" :\"" + strToDate.Split('-')[2] + "-" + strToDate.Split('-')[1] + "-" + strToDate.Split('-')[0] + "\"}";
                        string strResponse = General.DoRequest(strurl, strPostBody, "POST", "application/json");
                        if (!string.IsNullOrEmpty(strResponse))
                        {
                            objResponse = JsonConvert.DeserializeObject<RTAFESTTransInfo>(strResponse);
                        }
                        if (objResponse != null && objResponse.Code.ToStr() == "0" && objResponse.Data.Length > 0)
                        {
                            DataTable dtTrans = objResponse.Data.ToDataTable<DatumL>();
                            if (dtTrans != null && dtTrans.Rows.Count > 0)
                            {
                                dtTrans.Columns.Add("ID", typeof(decimal)).SetOrdinal(0);
                                SqlConnection conn = new SqlConnection(General.GetConfigVal("DSN_GOT_TRANS"));
                                long lTimeTaken1 = -1, lStart1 = DateTime.Now.Ticks;
                                try
                                {
                                    conn.Open();
                                    SqlTransaction transaction = conn.BeginTransaction();
                                    SqlBulkCopy copy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction);
                                    copy.DestinationTableName = "TAPP_RECON_RTAFEST_TRANS";
                                    copy.WriteToServer(dtTrans);
                                    transaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    General.WriteLog("GET_TRANS_EX", "EXception while bulk inserting in  RTAFEST Trans." + ex.ToStr());
                                }
                                finally
                                {
                                    if (lStart1 > 0)
                                        lTimeTaken1 = (DateTime.Now.Ticks - lStart1) / 10000;
                                    General.WriteLog("GET_TRANS", "RTAFEST bulk insertion time taken:" + lTimeTaken1.ToStr());
                                    conn.Close();
                                    dtTrans = null;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    #endregion
                }
                if (Array.IndexOf(General.GetConfigVal("ENABLE_TRANS_LIST_DEPT").ToUpper().Split(','), "IGRS") > -1)
                {
                    #region IGRS
                    IGRSTransInfo objResponse = null;
                    try
                    {
                        string strHashCode = string.Empty;
                        string strSaltKey = string.Empty;
                        GetSecurityHashCode(ref strHashCode, ref strSaltKey);
                        string strurl = General.GetConfigVal("STAMPS_GET_TRANSLIST_URL");
                        strurl = strurl.Replace("{transdate}", strFromDate);
                        strurl = strurl.Replace("{pcode}", "0");//for all services
                        strurl = strurl.Replace("{hashcode}", strHashCode).Replace("{saltkey}", strSaltKey);
                        string strResponse = General.DoRequest(strurl, "", "POST", "");
                        if (!string.IsNullOrEmpty(strResponse))
                        {
                            objResponse = JsonConvert.DeserializeObject<IGRSTransInfo>(strResponse);
                        }
                        if (objResponse != null && objResponse.code.ToStr() == "200" && objResponse.data.Count > 0)
                        {
                            DataTable dtTrans = objResponse.data.ToDataTable<DataTrans>();
                            if (dtTrans != null && dtTrans.Rows.Count > 0)
                            {
                                dtTrans.Columns.Add("ID", typeof(decimal)).SetOrdinal(0);
                                SqlConnection conn = new SqlConnection(General.GetConfigVal("DSN_GOT_TRANS"));
                                long lTimeTaken1 = -1, lStart1 = DateTime.Now.Ticks;
                                try
                                {
                                    conn.Open();
                                    SqlTransaction transaction = conn.BeginTransaction();
                                    SqlBulkCopy copy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction);
                                    copy.DestinationTableName = "TAPP_RECON_IGRS_TRANS";
                                    copy.WriteToServer(dtTrans);
                                    transaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    General.WriteLog("GET_TRANS_EX", "EXception while bulk inserting in  IGRS Trans." + ex.ToStr());
                                }
                                finally
                                {
                                    if (lStart1 > 0)
                                        lTimeTaken1 = (DateTime.Now.Ticks - lStart1) / 10000;
                                    General.WriteLog("GET_TRANS", "IGRS bulk insertion time taken:" + lTimeTaken1.ToStr());
                                    conn.Close();
                                    dtTrans = null;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    #endregion
                }
                if (Array.IndexOf(General.GetConfigVal("ENABLE_TRANS_LIST_DEPT").ToUpper().Split(','), "IGRS_DHARANI") > -1)
                {
                    #region IGRS-DHARANI
                    IGRSTransInfo objResponse = null;
                    try
                    {
                        string strHashCode = string.Empty;
                        string strSaltKey = string.Empty;
                        GetSecurityHashCode(ref strHashCode, ref strSaltKey);
                        string strurl = General.GetConfigVal("STAMPS_DH_GET_TRANSLIST_URL");
                        strurl = strurl.Replace("{transdate}", strFromDate);
                        strurl = strurl.Replace("{pcode}", "0");//for all services
                        strurl = strurl.Replace("{hashcode}", strHashCode).Replace("{saltkey}", strSaltKey);
                        string strResponse = General.DoRequest(strurl, "", "POST", "");
                        General.WriteLog("IGRS_DHARANI_TXNS", "url:" + strurl + Environment.NewLine + "Response:" + strResponse);
                        if (!string.IsNullOrEmpty(strResponse))
                        {
                            objResponse = JsonConvert.DeserializeObject<IGRSTransInfo>(strResponse);
                        }
                        if (objResponse != null && objResponse.statuscode.ToStr() == "200" && objResponse.data.Count > 0)
                        {
                            DataTable dtTrans = objResponse.data.ToDataTable<DataTrans>();
                            if (dtTrans != null && dtTrans.Rows.Count > 0)
                            {
                                dtTrans.Columns.Add("ID", typeof(decimal)).SetOrdinal(0);
                                SqlConnection conn = new SqlConnection(General.GetConfigVal("DSN_GOT_TRANS"));
                                long lTimeTaken1 = -1, lStart1 = DateTime.Now.Ticks;
                                try
                                {
                                    conn.Open();
                                    SqlTransaction transaction = conn.BeginTransaction();
                                    SqlBulkCopy copy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction);
                                    copy.DestinationTableName = "TAPP_RECON_IGRS_TRANS";
                                    copy.WriteToServer(dtTrans);
                                    transaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    General.WriteLog("GET_TRANS_EX", "EXception while bulk inserting in  IGRS Dharani Trans." + ex.ToStr());
                                }
                                finally
                                {
                                    if (lStart1 > 0)
                                        lTimeTaken1 = (DateTime.Now.Ticks - lStart1) / 10000;
                                    General.WriteLog("GET_TRANS", "IGRS dharani bulk insertion time taken:" + lTimeTaken1.ToStr());
                                    conn.Close();
                                    dtTrans = null;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        General.WriteLog("GET_TRANS_EX", "EXception while bulk inserting in  IGRS Dharani Trans(1)." + ex.ToStr());
                    }
                    #endregion
                }
                if (Array.IndexOf(General.GetConfigVal("ENABLE_TRANS_LIST_DEPT").ToUpper().Split(','), "BRMART") > -1)
                {
                    #region BRMART
                    try
                    {
                        String strBrMartUrl = General.GetConfigVal("BRMART_HISTROY_URL");
                        strBrMartUrl = strBrMartUrl.Replace("!FROMDATE!", strFromDate.Split('-')[2] + "-" + strFromDate.Split('-')[1] + "-" + strFromDate.Split('-')[0]);
                        strBrMartUrl = strBrMartUrl.Replace("!TODATE!", strToDate.Split('-')[2] + "-" + strToDate.Split('-')[1] + "-" + strToDate.Split('-')[0]);
                        String strAPIResp = General.DoWebRequest(strBrMartUrl);
                        if (!string.IsNullOrEmpty(strAPIResp))
                        {
                            //XmlDocument xDoc = new XmlDocument();
                            //xDoc.LoadXml(strAPIResp);
                            StringReader theReader = new StringReader(strAPIResp);
                            DataSet dsMArt = new DataSet();
                            dsMArt.ReadXml(theReader);
                            if (dsMArt != null && dsMArt.Tables != null && dsMArt.Tables["recharge"] != null && dsMArt.Tables["recharge"].Rows.Count > 0)
                            {
                                int iRet = -1;
                                foreach (DataRow dr in dsMArt.Tables["recharge"].Rows)
                                {
                                    using (DBFactory objDB = new DBFactory("DSN_GOT_TRANS"))
                                    {
                                        objDB.AddInParam("TID", SqlType.VarChar, dr["TID"].ToStr());
                                        objDB.AddInParam("RECHARGEID", SqlType.VarChar, dr["RechargeID"].ToStr());
                                        objDB.AddInParam("TDATETIME", SqlType.VarChar, dr["TDateTime"].ToStr());
                                        objDB.AddInParam("MOBILENUMBER", SqlType.VarChar, dr["MobileNumber"].ToStr());
                                        objDB.AddInParam("AMOUNT", SqlType.VarChar, dr["Amount"].ToStr());
                                        objDB.AddInParam("OPERATOR", SqlType.VarChar, dr["Operator"].ToStr());
                                        objDB.AddInParam("STATUS", SqlType.VarChar, dr["Status"].ToStr());
                                        objDB.AddInParam("STATUSTEXT", SqlType.VarChar, dr["StatusText"].ToStr());
                                        objDB.AddInParam("ACTUALTID", SqlType.VarChar, dr["ActualTID"].ToStr());
                                        objDB.AddInParam("CLIENT_ID", SqlType.VarChar, dr["Client_ID"].ToStr());
                                        objDB.AddOutParam("RETVALUE", SqlType.Int, 10);
                                        objDB.RunProc("UDP_TAPP__RECON_BRMART_INSERT");
                                        iRet = objDB.GetOutValue("RETVALUE").ToInt();
                                    }
                                }
                            }
                        }


                        String strBrMartUrlPostPaid = General.GetConfigVal("BRMART_HISTROY_POSTPAID_URL");
                        strBrMartUrlPostPaid = strBrMartUrlPostPaid.Replace("!FROMDATE!", strFromDate.Split('-')[2] + "-" + strFromDate.Split('-')[1] + "-" + strFromDate.Split('-')[0]);
                        strBrMartUrlPostPaid = strBrMartUrlPostPaid.Replace("!TODATE!", strToDate.Split('-')[2] + "-" + strToDate.Split('-')[1] + "-" + strToDate.Split('-')[0]);
                        String strAPIRespPostPaid = General.DoWebRequest(strBrMartUrlPostPaid);
                        if (!string.IsNullOrEmpty(strAPIRespPostPaid))
                        {
                            //XmlDocument xDoc = new XmlDocument();
                            //xDoc.LoadXml(strAPIResp);
                            StringReader theReaderPP = new StringReader(strAPIRespPostPaid);
                            DataSet dsMArtPP = new DataSet();
                            //dsMArtPP.ReadXml(@"D:\Applications\GOT\Console Applications\TAppTransactionsList\TAppTransactionsList\brmartpostpaid.xml");
                            dsMArtPP.ReadXml(theReaderPP);
                            if (dsMArtPP != null && dsMArtPP.Tables != null && dsMArtPP.Tables["recharge"] != null && dsMArtPP.Tables["recharge"].Rows.Count > 0)
                            {
                                int iRet = -1;
                                foreach (DataRow dr in dsMArtPP.Tables["recharge"].Rows)
                                {
                                    using (DBFactory objDB = new DBFactory("DSN_GOT_TRANS"))
                                    {
                                        objDB.AddInParam("TID", SqlType.VarChar, dr["TID"].ToStr());
                                        objDB.AddInParam("RECHARGEID", SqlType.VarChar, dr["Recharges_ID"].ToStr());
                                        objDB.AddInParam("TDATETIME", SqlType.VarChar, dr["TDateTime"].ToStr());
                                        objDB.AddInParam("MOBILENUMBER", SqlType.VarChar, dr["MobileNumber"].ToStr());
                                        objDB.AddInParam("AMOUNT", SqlType.VarChar, dr["Amount"].ToStr());
                                        objDB.AddInParam("OPERATOR", SqlType.VarChar, dr["Operator"].ToStr());
                                        objDB.AddInParam("STATUS", SqlType.VarChar, dr["Status"].ToStr());
                                        objDB.AddInParam("STATUSTEXT", SqlType.VarChar, dr["StatusText"].ToStr());
                                        objDB.AddInParam("ACTUALTID", SqlType.VarChar, dr["ActualTID"].ToStr());
                                        objDB.AddInParam("CLIENT_ID", SqlType.VarChar, string.Empty);//dr["Client_ID"].ToStr()--Client ID is not availbe in postpaid response
                                        objDB.AddOutParam("RETVALUE", SqlType.Int, 10);
                                        objDB.RunProc("UDP_TAPP__RECON_BRMART_INSERT");
                                        iRet = objDB.GetOutValue("RETVALUE").ToInt();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        General.WriteLog("GET_TRANS_EX", "EXception while getting history data from BRMART==>" + ex.ToStr());
                    }

                    #endregion
                }
                if (Array.IndexOf(General.GetConfigVal("ENABLE_TRANS_LIST_DEPT").ToUpper().Split(','), "MESEVA") > -1)
                {
                    #region MEE SEVA
                    // tapp.MeesevaMobileWebservice objMeeSevaService = new tapp.MeesevaMobileWebservice();
                    meseva.MeesevaMobileWebserviceSoapClient objMeeSevaService = new meseva.MeesevaMobileWebserviceSoapClient();
                    long lTimeTaken = -1, lStart = DateTime.Now.Ticks;
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    DataSet ds = objMeeSevaService.TappTransactionList(strFromDate, strToDate);
                    if (lStart > 0)
                        lTimeTaken = (DateTime.Now.Ticks - lStart) / 10000;
                    General.WriteLog("GET_TRANS", "MESEVA web service time taken:" + lTimeTaken.ToStr());
                    objMeeSevaService = null;

                    if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && !ds.Tables[0].Columns.Contains("Error"))
                    {
                        General.WriteLog("GET_TRANS", "MESEVA: FROM DATE:" + strFromDate + ",strToDate:" + strToDate + ", Fetched Row Count:" + ds.Tables[0].Rows.Count.ToStr());
                        ds.Tables[0].Columns.Add("ID", typeof(decimal)).SetOrdinal(0);
                        SqlConnection conn = new SqlConnection(General.GetConfigVal("DSN_GOT_TRANS"));
                        long lTimeTaken1 = -1, lStart1 = DateTime.Now.Ticks;
                        try
                        {
                            conn.Open();
                            SqlTransaction transaction = conn.BeginTransaction();
                            SqlBulkCopy copy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction);
                            copy.DestinationTableName = "TAPP_RECON_MESEVA_TRANS";
                            copy.WriteToServer(ds.Tables[0]);
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            General.WriteLog("GET_TRANS_EX", "EXception while bulk inserting in  MEseva Trans." + ex.ToStr());
                        }
                        finally
                        {
                            if (lStart1 > 0)
                                lTimeTaken1 = (DateTime.Now.Ticks - lStart1) / 10000;
                            General.WriteLog("GET_TRANS", "MESEVA bulk insertion time taken:" + lTimeTaken1.ToStr());
                            conn.Close();
                            ds = null;
                            objMeeSevaService = null;
                        }
                        #region commented
                        //foreach (DataRow dr in ds.Tables[0].Rows)
                        //{
                        //    int iRet = TransDataInsert(dr["ApplicationNumber"].ToStr(), dr["Service"].ToStr(), dr["TransactionID"].ToStr(), dr["CreatedBy"].ToStr(), dr["TransactionDate"].ToStr(), dr["TransactionAmount"].ToStr());
                        //    if (iRet <= 0)
                        //    {
                        //        General.WriteLog("INSERT_FAIL", "Insert failed for Application no:" + dr["ApplicationNumber"].ToStr() + ", service:" + dr["Service"].ToStr() + ", Amount:" + dr["TransactionAmount"].ToStr());
                        //    }
                        //} 
                        #endregion
                    }
                    else
                    {
                        General.WriteLog("GET_TRANS", "MESEVA: FROM DATE:" + strFromDate + ",strToDate:" + strToDate + ", No data Returned.");
                    }

                    #endregion
                }
                if (Array.IndexOf(General.GetConfigVal("ENABLE_TRANS_LIST_DEPT").ToUpper().Split(','), "ESEVA") > -1)
                {
                    #region E SEVA
                    eseva.tapp objEsevaService = new eseva.tapp();
                    eseva.MobileReconRequestBean objEsevaServiceReq = new eseva.MobileReconRequestBean();
                    eseva.MobileReconResponceBean objEsevaServiceResp = null;
                    objEsevaServiceReq.userId = General.GetConfigVal("ESEVA_USERID");
                    objEsevaServiceReq.password = General.GetConfigVal("ESEVA_PASSWORD");
                    objEsevaServiceReq.transdate = strFromDate;
                    long lTimeTaken2 = -1, lStart2 = DateTime.Now.Ticks;
                    objEsevaServiceResp = objEsevaService.MsdgRecondata(objEsevaServiceReq);
                    if (lStart2 > 0)
                        lTimeTaken2 = (DateTime.Now.Ticks - lStart2) / 10000;
                    General.WriteLog("GET_TRANS", "ESEVA web service time taken:" + lTimeTaken2.ToStr());
                    String strResp = string.Empty;
                    if (objEsevaServiceResp != null && objEsevaServiceResp.strResponeCode == "000")
                    {
                        strResp = objEsevaServiceResp.strTransdata;
                        XmlDocument xdocResp = new XmlDocument();
                        xdocResp.LoadXml(strResp);
                        XmlReader xmlReader = new XmlNodeReader(xdocResp);
                        DataSet ds = new DataSet();
                        ds.ReadXml(xmlReader);
                        if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            SqlConnection conn = new SqlConnection(General.GetConfigVal("DSN_GOT_TRANS"));
                            long lTimeTaken3 = -1, lStart3 = DateTime.Now.Ticks;
                            try
                            {
                                conn.Open();
                                SqlTransaction transaction = conn.BeginTransaction();
                                ds.Tables[0].Columns.Add("ID", typeof(decimal)).SetOrdinal(0);
                                SqlBulkCopy copy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction);
                                copy.DestinationTableName = "TAPP_RECON_ESEVA_TRANS";
                                copy.WriteToServer(ds.Tables[0]);
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                General.WriteLog("GET_TRANS_EX", "EXception while bulk inserting in  Eseva Trans." + ex.ToStr());
                            }
                            finally
                            {
                                if (lStart3 > 0)
                                    lTimeTaken3 = (DateTime.Now.Ticks - lStart3) / 10000;
                                General.WriteLog("GET_TRANS", "ESEVA bulk insertion time taken:" + lTimeTaken3.ToStr());
                                conn.Close();
                                objEsevaService = null;
                                objEsevaServiceReq = null;
                                ds = null;
                            }
                        }
                        else
                        {
                            General.WriteLog("GET_TRANS", "ESEVA: FROM DATE:" + strFromDate + ",strToDate:" + strToDate + ", No data Returned.");
                        }
                    }
                    else
                    {
                        General.WriteLog("GET_TRANS", "ESEVA: FROM DATE:" + strFromDate + ",strToDate:" + strToDate + ", Error Response from Web Service." + objEsevaServiceResp.strResponeCode + "-" + objEsevaServiceResp.strResponeDesc);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                General.WriteLog("TappTranactionsList", "Exception in Main==>Message:" + ex.Message);
                Console.WriteLine("Exception in Main==>Message:" + ex.Message);
            }
        }

        public static void GetSecurityHashCode(ref string strHashCode, ref string strSaltKey)
        {
            try
            {
                strSaltKey = DateTime.Now.Ticks.ToStr();
                string strSecurityCode = General.GetConfigVal("STAMPS_SECURITY_CODE");
                strHashCode = General.GetSHA1Hash(strSaltKey + strSecurityCode);
            }
            catch (Exception ex)
            {
                General.WriteLog("TappTranactionsList", "Exception in GetSecurityHashCode==>Message:" + ex.Message);
            }
        }

        //private static int TransDataInsert(string ApplicationNumber, string Service, string TransactionID, string CreatedBy, string TransactionDate, string TransactionAmount)
        //{
        //    int iRet = -1;
        //    long lTimeTaken = -1, lStart = DateTime.Now.Ticks;
        //    try
        //    {
        //        using (DBFactory objDB = new DBFactory("DSN_GOT_TRANS"))
        //        {
        //            objDB.AddInParam("APPLICATIONNUMBER", SqlType.VarChar, ApplicationNumber);
        //            objDB.AddInParam("SERVICE", SqlType.VarChar, Service);
        //            objDB.AddInParam("TRANSACTIONID", SqlType.VarChar, TransactionID);
        //            objDB.AddInParam("CREATEDBY", SqlType.VarChar, CreatedBy);
        //            objDB.AddInParam("TRANSACTIONDATE", SqlType.DateTime, DateTime.ParseExact(TransactionDate, "dd/MM/yyyy", null).ToStr());
        //            objDB.AddInParam("TRANSACTIONAMOUNT", SqlType.VarChar, TransactionAmount);
        //            objDB.AddOutParam("RETVALUE", SqlType.Int, 10);
        //            objDB.RunProc("UDP_TAPP_TRANS_CRUD");
        //            iRet = objDB.GetOutValue("RETVALUE").ToInt();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        General.WriteLog("TappTranactionsList", "Exception in TransDataInsert-->Message::" + ex.Message + ", StackTrace::" + ex.StackTrace);
        //    }
        //    finally
        //    {
        //        if (lStart > 0)
        //            lTimeTaken = (DateTime.Now.Ticks - lStart) / 10000;
        //        General.WriteLog("DB_TT", "UDP_TAPP_TRANS_CRUD==>" + lTimeTaken.ToStr());
        //    }
        //    return iRet;
        //}
    }

    public class DataTrans
    {
        public string purposeCode { get; set; }
        public string purposeName { get; set; }
        public string transId { get; set; }
        public string transDate { get; set; }
        public string deptTransId { get; set; }
        public string deptTransDate { get; set; }
        public string amount { get; set; }
        public string remitterMobileNo { get; set; }
    }

    public class IGRSTransInfo
    {
        public string code { get; set; }

        public string statuscode { get; set; }
        public string statusMessage { get; set; }

        public string desc { get; set; }
        public List<DataTrans> data { get; set; }
    }

    public class RTAFESTTransInfo
    {
        public long Code { get; set; }
        public string Description { get; set; }
        public DatumL[] Data { get; set; }
    }

    public class DatumL
    {
        public string SubService { get; set; }
        public string Description { get; set; }
        public string TransId { get; set; }
        public string TransDate { get; set; }
        public string DeptTransId { get; set; }
        public string Amount { get; set; }
        public string ConsumerNumber { get; set; }
    }

    public class EntroLabsDeptReq
    {
        public string gettempleTransactionslist { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public string temple_code { get; set; }
        public string service { get; set; }
    }
    public class EntroLabsDeptRes
    {
        public string result { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public string count { get; set; }
        public string amount { get; set; }
        public List<EntroLabs> data { get; set; }
    }

    public class EntroLabs
    {
        public string temple_code { get; set; }
        public string ticket_no { get; set; }
        public string amount { get; set; }
        public string order_id { get; set; }
        public string transaction_id { get; set; }
        public string booking_date { get; set; }
        public string created_timestamp { get; set; }
    }
}
