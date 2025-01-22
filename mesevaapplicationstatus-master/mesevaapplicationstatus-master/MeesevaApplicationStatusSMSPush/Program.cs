using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonHelper;
using System.Xml;
using System.Data;
using IMI.SqlWrapper;

namespace MeesevaApplicationStatusSMSPush
{
    class Program
    {
        static void Main(string[] args)
        {
            String strMailTo = General.GetConfigVal("MAIL_TO");
            String strMailCC = General.GetConfigVal("MAIL_CC");
            String strMailBCC = General.GetConfigVal("MAIL_BCC");
            String strMailSub = General.GetConfigVal("MAIL_SUB");
            String strMailBody = General.GetConfigVal("MAIL_BODY");
            try
            {
                String strUserID = General.GetConfigVal("MEESEVA_USERID");
                String strPassword = General.GetConfigVal("MEESEVA_PASSWORD");
                String strSenderID = General.GetConfigVal("SMS_SENDERID");
                String strFromDate = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");
                String strToDate = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");
                if (General.GetConfigVal("ENABLE_CUSTOM_DATE").ToUpper() == "Y" && General.GetConfigVal("CUSTOM_FROM_DATE") != "")
                {
                    strFromDate = Convert.ToDateTime(General.GetConfigVal("CUSTOM_FROM_DATE")).ToString("dd-MM-yyyy");
                    strToDate = Convert.ToDateTime(General.GetConfigVal("CUSTOM_TO_DATE")).ToString("dd-MM-yyyy");
                }
                strFromDate = strFromDate + " 00:00:00";
                strToDate = strToDate + " 23:59:59";
                Console.WriteLine("From Date:" + strFromDate);
                Console.WriteLine("To Date:" + strToDate);
                LoadConfig.LoadEsevaConfig();
                if (LoadConfig.xDocEsevaConfig != null)
                {
                    if (LoadConfig.xDocEsevaConfig.DocumentElement.SelectSingleNode("SERVICES") != null)
                    {
                        XmlNodeList XServiceNodes = LoadConfig.xDocEsevaConfig.DocumentElement.SelectSingleNode("SERVICES").ChildNodes;
                        if (XServiceNodes != null && XServiceNodes.Count > 0)
                        {

                            GOT.MEESEVA.MeesevaMobileWebserviceSoapClient objService = new GOT.MEESEVA.MeesevaMobileWebserviceSoapClient();
                            foreach (XmlNode XNode in XServiceNodes)
                            {
                                String strSeriveName = XNode.Name;
                                String strServiceID = XNode.InnerText;
                                Console.Title = strSeriveName + "(" + strServiceID + ")";
                                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                                String strResp = objService.GetServiceWiseApplicationStatus(strUserID, strPassword, strFromDate, strToDate, strServiceID);

                                Console.WriteLine("Service Name :" + strSeriveName + "(" + strServiceID + ")" + ", API RESP:" + strResp);
                                General.WriteLog("REQ_RESP", "REQ SERVICE NAME::" + strSeriveName + "(" + strServiceID + ")" + Environment.NewLine + "API RESP: " + strResp);

                                String strApplicationNo = String.Empty;
                                XmlDocument xdocResp = new XmlDocument();
                                xdocResp.LoadXml(strResp);
                                if (xdocResp != null)
                                {
                                    if (xdocResp.GetElementsByTagName("ErrorCode").Count > 0)
                                    {
                                        Console.WriteLine("Error Response from API for Service Name:" + strSeriveName + "(" + strServiceID + ")" + ", Response:" + strResp);
                                        General.WriteLog("ApplicaionStatus", "Error Response from API for Service ID:" + strSeriveName + "(" + strServiceID + ")" + " Response:" + strResp);
                                        continue;
                                    }
                                    else
                                    {
                                        if (xdocResp.DocumentElement.SelectNodes("Table1").Count > 0)
                                        {
                                            foreach (XmlNode Table1 in xdocResp.DocumentElement.SelectNodes("Table1"))
                                            {
                                                strApplicationNo = Table1.SelectSingleNode("application_number").InnerText;
                                                DateTime dtCertDate = new DateTime(1900, 1, 1);
                                                try
                                                {
                                                    dtCertDate = Convert.ToDateTime(Table1.SelectSingleNode("updated_date").InnerText);
                                                }
                                                catch (Exception ex) { General.WriteLog("ApplicaionStatus", "exception in datetime convertion:" + ex.ToStr()); }
                                                DataSet ods = new DataSet();
                                                GetPaymentDetails(strApplicationNo, ref ods);
                                                if (ods != null && ods.Tables.Count > 0 && ods.Tables[0].Rows.Count > 0)
                                                {
                                                    String strMobileNo = String.Empty; String strMessage = String.Empty;
                                                    String strConsumerName = String.Empty; String strPaymentTransID = String.Empty;
                                                    String strCertStatus = "A";
                                                    strMobileNo = ods.Tables[0].Rows[0]["MobileNo"].ToStr();
                                                    strPaymentTransID = ods.Tables[0].Rows[0]["DeptTransId"].ToStr();
                                                    strMessage = General.GetConfigVal("SMS_TEXT").Replace("!TRANSID!", strPaymentTransID).Replace("!APPNO!", strApplicationNo);
                                                    PushSMS(DateTime.Now.Ticks.ToStr(), strMobileNo, "console", "APPNOSTATUS", "", strSenderID, strMessage, "");
                                                    UpdateCertificateStatus(strApplicationNo, strCertStatus, dtCertDate);
                                                }
                                                else
                                                {
                                                    General.WriteLog("ApplicaionStatus", "DB record not found for application no:" + strApplicationNo);
                                                    Console.WriteLine("DB record not found for application no:" + strApplicationNo);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            General.WriteLog("ApplicaionStatus", "Success response from API, But application numbers not found." + strResp);
                                            Console.WriteLine("Success response from API, But application numbers not found." + strResp);
                                        }
                                    }
                                }
                                else
                                {
                                    General.WriteLog("ApplicaionStatus", "Success response from API, But application numbers not found." + strResp);
                                    Console.WriteLine("Success response from API, But application numbers not found." + strResp);
                                }
                            }
                        }
                        else
                        {
                            General.WriteLog("ApplicaionStatus", "Service IDs not found.");
                            Console.WriteLine("Service IDs not found.");
                        }
                    }
                    // Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                General.SendMail(strMailTo, strMailCC, strMailBCC, strMailSub, strMailBody, "");
                General.WriteLog("ApplicaionStatus", "Exception in Main==>Message:" + ex.ToStr());
                Console.WriteLine("Exception in Main==>Message:" + ex.ToStr());
            }
        }


        public static void PushSMS(string strRefID, string strMobileNo, string strChannel, string strService, string strServiceCode, string strSenderID, string strMessage, string strDigitalID)
        {
            if (General.GetConfigVal("PUSH_TYPE") == "Q")
            {
                SMS objBO = new SMS();
                try
                {
                    objBO.RefID = strRefID;
                    objBO.MobileNo = strMobileNo;
                    objBO.Channel = strChannel;
                    objBO.Service = strService;
                    objBO.ServiceCode = strServiceCode;
                    objBO.SenderID = strSenderID;
                    objBO.Message = strMessage;
                    objBO.DigitalID = strDigitalID;
                    objBO.LogMessage();
                    General.WriteLog("SMSSENT", "Mobileno:" + strMobileNo + ", Message:" + strMessage);
                }
                catch (Exception ex)
                {
                    General.WriteLog("ApplicaionStatus", "Exception in PushSMS==>" + ex.ToStr());
                }
                finally
                {
                    objBO = null;
                }
            }
            #region Not required
            //else
            //{
            //    String strUrl = General.GetConfigVal("SENDSMS_URL");
            //    String strPostBody = General.GetConfigVal("SENDSMS_BODY");
            //    if (strUrl.Trim().Length == 0)
            //        return;

            //    String strResponse = String.Empty;
            //    long lTimeTaken = -1, lStart = DateTime.Now.Ticks;
            //    try
            //    {
            //        strPostBody = strPostBody.Replace("{uid}", HttpUtility.UrlEncode(General.GetConfigVal("SENDSMS_UID")));
            //        strPostBody = strPostBody.Replace("{pwd}", HttpUtility.UrlEncode(EncryptedPasswod(General.GetConfigVal("SENDSMS_PWD"))));
            //        strPostBody = strPostBody.Replace("{mobile}", strMobileNo);
            //        strPostBody = strPostBody.Replace("{msg}", HttpUtility.UrlEncode(strMessage));
            //        strPostBody = strPostBody.Replace("{senderid}", strSenderID);
            //        strResponse = General.DoRequest(strUrl, strPostBody, "POST", "application/x-www-form-urlencoded");
            //        if (lStart > 0)
            //            lTimeTaken = (DateTime.Now.Ticks - lStart) / 10000;
            //    }
            //    catch (Exception ex)
            //    {
            //        LogData.Write("GOTAPI", "PushSMS", LogMode.Excep, ex, string.Format("APIHelper => PushSMS- Ex:{0}", ex.Message));
            //    }
            //    finally
            //    {
            //        object retVal = -1;
            //        using (DBFactory objDB = new DBFactory("DSN_TRANS"))
            //        {
            //            objDB.AddInParam("RefId", SqlType.VarChar, strRefID);
            //            objDB.AddInParam("MobileNo", SqlType.VarChar, strMobileNo);
            //            objDB.AddInParam("Channel", SqlType.VarChar, strChannel);
            //            objDB.AddInParam("Service", SqlType.VarChar, strService);
            //            objDB.AddInParam("ServiceCode", SqlType.VarChar, strServiceCode);
            //            objDB.AddInParam("SenderId", SqlType.VarChar, strSenderID);
            //            objDB.AddInParam("Message", SqlType.NVarChar, strMessage);
            //            objDB.AddInParam("GWTID", SqlType.VarChar, strResponse);
            //            objDB.AddInParam("TimeTaken", SqlType.Decimal, lTimeTaken);
            //            objDB.AddInParam("DigitalId", SqlType.VarChar, strDigitalID);
            //            objDB.AddInParam("TYPE", SqlType.VarChar, "C");
            //            objDB.AddOutParam("RETSTATUS", SqlType.VarChar, 10);
            //            objDB.RunProc("SMSPUSH_TRANS_CRUD");
            //            retVal = objDB.GetOutValue("RETSTATUS");
            //        }
            //    }
            //} 
            #endregion
        }

        private static void GetPaymentDetails(String strApplicationNo, ref DataSet oDS)
        {
            try
            {
                object retVal = -1;
                using (DBFactory objDB = new DBFactory("DSN_GOT"))
                {
                    objDB.AddInParam("APPLICATIONNO", SqlType.VarChar, strApplicationNo);
                    objDB.AddOutParam("RETSTATUS", SqlType.VarChar, 10);
                    objDB.RunProc("PAYMENT_TRANS_GET", out oDS, 1);
                    retVal = objDB.GetOutValue("RETSTATUS");
                }
            }
            catch (Exception ex)
            {
                CommonHelper.General.WriteLog("GOT", ex.Message + ": StackTrace " + ex.StackTrace);
                throw;
            }
        }

        private static int UpdateCertificateStatus(String strApplicationNo, String strCertStatus, DateTime dtCertDateTime)
        {
            int retVal = -1;
            try
            {

                using (DBFactory objDB = new DBFactory("DSN_GOT"))
                {
                    objDB.AddInParam("APPLICATIONNO", SqlType.VarChar, strApplicationNo);
                    objDB.AddInParam("CERT_STATUS", SqlType.VarChar, strCertStatus);
                    objDB.AddInParam("CERT_DATE", SqlType.DateTime, dtCertDateTime);
                    objDB.AddOutParam("RETSTATUS", SqlType.VarChar, 10);
                    objDB.RunProc("UDP_UPDATE_CERTIFICATE_STATUS");
                    retVal = objDB.GetOutValue("RETSTATUS").ToInt();
                }
            }
            catch (Exception ex)
            {
                CommonHelper.General.WriteLog("UPDATE_STATUS", "Exception in UpdateCertificateStatus::" + ex.ToStr());
            }
            return retVal;
        }
    }
}
