//$Author: Anilkumar.ko $
//$Date: 4/03/18 6:55p $
//$Header: /Mobileapps/Operator/Davinci/TS/FailedTransUpdate/FailedTransUpdate/CommonHelper/APIHelper.cs 1     4/03/18 6:55p Anilkumar.ko $
//$Modtime: 9/28/16 4:28p $
//$Revision: 1 $
//Copyright @2012 IMImobile Pvt. Ltd.

using System;
using System.Collections;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using IMI.SqlWrapper;

namespace CommonHelper
{
    public class APIHelper
    {
        public static XmlDocument xDocMessageCodes;
        public static CacheDependency objCacheMesssageCodes;

        public static void LoadMessageCodes()
        {
            String strPath = String.Empty;
            try
            {
                if (xDocMessageCodes == null || objCacheMesssageCodes == null || objCacheMesssageCodes.HasChanged)
                {
                    strPath = CommonHelper.General.GetConfigVal("SERVICE_XML_PATH") + "messagecodes.xml";
                    if (!System.IO.File.Exists(strPath))
                        return;

                    xDocMessageCodes = null;
                    objCacheMesssageCodes = null;
                    xDocMessageCodes = new XmlDocument();
                    try
                    {
                        xDocMessageCodes.Load(strPath);
                        objCacheMesssageCodes = new CacheDependency(strPath, DateTime.Now);
                    }
                    catch (Exception ex)
                    {
                        General.WriteLog("Exception in LoadErrorCodes xml() - Error" + ex.Message + ",Source:" + ex.Source + ",StackTrace:" + ex.StackTrace + ",InnerException:" + ex.InnerException);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.General.WriteLog("Exception LoadErrorCodes() - Error" + ex.Message + ",Source:" + ex.Source + ",StackTrace:" + ex.StackTrace + ",InnerException:" + ex.InnerException);
            }
        }

        public static String GetMessage(String strCode)
        {
            String strResult = String.Empty;
            XDocument xDoc;
            try
            {
                LoadMessageCodes();
                XmlNode xNode = xDocMessageCodes.DocumentElement.SelectSingleNode("//code[@id='" + strCode.ToUpper() + "']");

                if (xNode != null)
                {
                    xDoc = new XDocument(new XElement("root",
                                            new XElement("code", General.GetConfigVal("SNMP_MODULE_ID") + strCode.ToUpper()),
                                            new XElement("desc", xNode.InnerText)
                                            ));
                    strResult = General.GetConfigVal("SNMP_MODULE_ID") + strCode.ToUpper() + "$" + xDoc.ToString();
                }
                else
                {
                    strResult = General.GetConfigVal("ERROR_DEF_MSG");
                }
            }
            catch
            {
                strResult = General.GetConfigVal("ERROR_DEF_MSG");
            }
            finally
            {
                xDoc = null;
            }
            return strResult;
        }

        public static String GetMessage(String strCode, String strErrorMsg)
        {
            String strResult = String.Empty;
            XDocument xDoc;
            try
            {
                xDoc = new XDocument(new XElement("root",
                                        new XElement("code", strCode),
                                        new XElement("desc", strErrorMsg)
                                        ));
                strResult = General.GetConfigVal("SNMP_MODULE_ID") + strCode.ToUpper() + "$" + xDoc.ToString();
            }
            catch
            {
                strResult = General.GetConfigVal("ERROR_DEF_MSG");
            }
            finally
            {
                xDoc = null;
            }
            return strResult;
        }

        public static String GetMessageText(String strCode)
        {
            String strResult = String.Empty;
            try
            {
                LoadMessageCodes();
                XmlNode xNode = xDocMessageCodes.DocumentElement.SelectSingleNode("//code[@id='" + strCode.ToUpper() + "']");

                if (xNode != null)
                    strResult = General.GetConfigVal("SNMP_MODULE_ID") + strCode.ToUpper() + "$" + xNode.InnerText;
                else
                    strResult = General.GetConfigVal("ERROR_DEF_MSG");
            }
            catch
            {
                strResult = General.GetConfigVal("ERROR_DEF_MSG");
            }
            return strResult;
        }

        public static String GetMessagePlainText(String strCode)
        {
            String strResult = String.Empty;
            try
            {
                LoadMessageCodes();
                XmlNode xNode = xDocMessageCodes.DocumentElement.SelectSingleNode("//code[@id='" + strCode.ToUpper() + "']");

                if (xNode != null)
                    strResult = xNode.InnerText;
                else
                    strResult = General.GetConfigVal("ERROR_DEF_MSG");
            }
            catch
            {
                strResult = General.GetConfigVal("ERROR_DEF_MSG");
            }
            return strResult;
        }

        public static XmlDocument xDocPayment;
        public static CacheDependency objCachePayment;

        public static void LoadPaymentConfigFile()
        {
            String strPath = String.Empty;
            try
            {
                if (xDocPayment == null || objCachePayment == null || objCachePayment.HasChanged)
                {
                    strPath = CommonHelper.General.GetConfigVal("SERVICE_XML_PATH") + "payment.xml";
                    if (!System.IO.File.Exists(strPath))
                        return;

                    xDocPayment = null;
                    objCachePayment = null;
                    xDocPayment = new XmlDocument();
                    try
                    {
                        xDocPayment.Load(strPath);
                        objCachePayment = new CacheDependency(strPath, DateTime.Now);
                    }
                    catch (Exception ex)
                    {
                        General.WriteLog("Exception in Loading xml() - Error" + ex.Message + ",Source:" + ex.Source + ",StackTrace:" + ex.StackTrace + ",InnerException:" + ex.InnerException);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.General.WriteLog("Exception LoadConfigData() - Error" + ex.Message + ",Source:" + ex.Source + ",StackTrace:" + ex.StackTrace + ",InnerException:" + ex.InnerException);
            }
        }

        public static String GetPaymentMessage(String strDept, ref String strSenderID)
        {
            String strResult = String.Empty;
            try
            {
                LoadPaymentConfigFile();
                if (xDocPayment != null)
                {
                    XmlNode xNode = xDocPayment.DocumentElement.SelectSingleNode(strDept.ToLower());
                    if (xNode != null)
                    {
                        strResult = xNode.SelectSingleNode("message") != null ? xNode.SelectSingleNode("message").InnerText.ToString() : String.Empty;
                        strSenderID = xNode.SelectSingleNode("senderid") != null ? xNode.SelectSingleNode("senderid").InnerText.ToString() : String.Empty;
                    }
                }
            }
            catch
            {
            }
            return strResult;
        }

        public static String GetPaymentMessage(String strDept, ref String strSenderID, ref String strDesc)
        {
            String strResult = String.Empty;
            try
            {
                LoadPaymentConfigFile();
                if (xDocPayment != null)
                {
                    XmlNode xNode = xDocPayment.DocumentElement.SelectSingleNode(strDept.ToLower());
                    if (xNode != null)
                    {
                        strResult = xNode.SelectSingleNode("message") != null ? xNode.SelectSingleNode("message").InnerText.ToString() : String.Empty;
                        strSenderID = xNode.SelectSingleNode("senderid") != null ? xNode.SelectSingleNode("senderid").InnerText.ToString() : String.Empty;
                        strDesc = xNode.SelectSingleNode("desc") != null ? xNode.SelectSingleNode("desc").InnerText : String.Empty;
                    }
                }
            }
            catch
            {
            }
            return strResult;
        }

        public static void PushMessage(String strMessage, String strMobileNo, String strSenderID)
        {
            SMSBO objBO = new SMSBO();
            try
            {
                objBO.MobileNo = strMobileNo;
                objBO.Channel = String.Empty;
                objBO.Department = String.Empty;
                objBO.SenderID = strSenderID;
                objBO.Message = strMessage;
                objBO.TransDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                objBO.LogMessage();
            }
            catch (Exception ex)
            {
                General.WriteLog("EMAIL_QUEUE_FAIL", "Error in SendMail(), Error-" + ex.Message.ToString());
            }
            finally
            {
                objBO = null;
            }
            #region Commented code
            //String strSendSMSUrl = General.GetConfigVal("SENDSMS_URL");
            //String strServiceKey = General.GetConfigVal("SENDSMS_KEY");

            //if (strSendSMSUrl.Trim().Length == 0)
            //    return;

            //String strResponse = String.Empty;
            //String strMainContent = String.Empty;
            //try
            //{
            //    strSendSMSUrl = strSendSMSUrl.Replace("{senderAddress}", HttpUtility.UrlEncode(strSenderID));
            //    strMainContent = "{\"outboundSMSMessageRequest\":{\"address\":[\"tel:!address!\"],\"senderAddress\":\"tel:!sendername!\",\"outboundSMSTextMessage\":{\"message\":\"!message!\"},\"clientCorrelator\":\"\",\"receiptRequest\": {\"notifyURL\":\"\",\"callbackData\":\"$(callbackData)\"} ,\"messageType\":\"\",\"senderName\":\"\"}}";
            //    strMainContent = strMainContent.Replace("!address!", strMobileNo);
            //    strMainContent = strMainContent.Replace("!sendername!", HttpUtility.UrlEncode(strSenderID));
            //    strMainContent = strMainContent.Replace("!key!", strServiceKey);
            //    strMainContent = strMainContent.Replace("!message!", strMessage);

            //    Hashtable htHeaders = new Hashtable();
            //    htHeaders.Add("key", strServiceKey);
            //    strResponse = General.DoRequest(strSendSMSUrl, strMainContent, "POST", "application/json", htHeaders);
            //}
            //catch (Exception ex)
            //{
            //    General.WriteLog("Exception:- " + ex.Message + "stacktrace" + ex.StackTrace);
            //}
            //finally
            //{
            //    String strMobileNumber = String.Empty;
            //    if (strMobileNo.Length == 10)
            //        strMobileNumber = "91" + strMobileNo;
            //    dynamic objLog = new ExpandoObject();
            //    objLog.mobieno = strMobileNumber;
            //    objLog.senderid = strSenderID;
            //    objLog.message = strMessage;
            //    objLog.transdate = DateTime.Now;
            //    objLog.postbody = strMainContent;
            //    objLog.response = strResponse;
            //    LogServer.Log ObjLogging = new LogServer.Log();
            //    ObjLogging.WriteToMongoDB(objLog, "gok_smspush");
            //    objLog = null;
            //}
            #endregion
        }

        public static void PushMessage(String strMessage, String strMobileNo, String strSenderID, String strChannel, String strDept)
        {
            SMSBO objBO = new SMSBO();
            try
            {
                objBO.MobileNo = strMobileNo;
                objBO.Channel = strChannel;
                objBO.Department = strDept;
                objBO.SenderID = strSenderID;
                objBO.Message = strMessage;
                objBO.TransDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                objBO.LogMessage();
            }
            catch (Exception ex)
            {
                General.WriteLog("EMAIL_QUEUE_FAIL", "Error in SendMail(), Error-" + ex.Message.ToString());
            }
            finally
            {
                objBO = null;
            }

            #region Commented code
            //String strSendSMSUrl = General.GetConfigVal("SENDSMS_URL");
            //String strServiceKey = General.GetConfigVal("SENDSMS_KEY");

            //if (strSendSMSUrl.Trim().Length == 0)
            //    return;

            //String strResponse = String.Empty;
            //String strMainContent = String.Empty;
            //try
            //{
            //    strSendSMSUrl = strSendSMSUrl.Replace("{senderAddress}", HttpUtility.UrlEncode(strSenderID));
            //    strMainContent = "{\"outboundSMSMessageRequest\":{\"address\":[\"tel:!address!\"],\"senderAddress\":\"tel:!sendername!\",\"outboundSMSTextMessage\":{\"message\":\"!message!\"},\"clientCorrelator\":\"\",\"receiptRequest\": {\"notifyURL\":\"\",\"callbackData\":\"$(callbackData)\"} ,\"messageType\":\"\",\"senderName\":\"\"}}";
            //    strMainContent = strMainContent.Replace("!address!", strMobileNo);
            //    strMainContent = strMainContent.Replace("!sendername!", HttpUtility.UrlEncode(strSenderID));
            //    strMainContent = strMainContent.Replace("!key!", strServiceKey);
            //    strMainContent = strMainContent.Replace("!message!", strMessage);

            //    Hashtable htHeaders = new Hashtable();
            //    htHeaders.Add("key", strServiceKey);
            //    strResponse = General.DoRequest(strSendSMSUrl, strMainContent, "POST", "application/json", htHeaders);
            //}
            //catch (Exception ex)
            //{
            //    General.WriteLog("Exception:- " + ex.Message + "stacktrace" + ex.StackTrace);
            //}
            //finally
            //{
            //    String strMobileNumber = String.Empty;
            //    if (strMobileNo.Length == 10)
            //        strMobileNumber = "91" + strMobileNo;
            //    dynamic objLog = new ExpandoObject();
            //    objLog.mobieno = strMobileNumber;
            //    objLog.senderid = strSenderID;
            //    objLog.message = strMessage;
            //    objLog.transdate = DateTime.Now;
            //    objLog.postbody = strMainContent;
            //    objLog.response = strResponse;
            //    LogServer.Log ObjLogging = new LogServer.Log();
            //    ObjLogging.WriteToMongoDB(objLog, "gok_smspush");
            //    objLog = null;
            //}
            #endregion
        }

        public static String CreateRandomString(int iLength)
        {
            if (iLength == 0)
                iLength = 6;
            string allowedChars = "0123456789";
            char[] chars = new char[iLength];
            Random rd = new Random();

            for (int i = 0; i < iLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        public static String CreateNumericString(int iLength)
        {
            if (iLength == 0)
                iLength = 6;
            string allowedChars = "0123456789";
            char[] chars = new char[iLength];
            Random rd = new Random();

            for (int i = 0; i < iLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        public static String CreateAlphaNumericString(int iLength)
        {
            if (iLength == 0)
                iLength = 6;
            string allowedChars = "ABCDEFGHJKLMNPQRSTUVWXYZ123456789abcdefghjklmnpqrstuvwxyz";
            char[] chars = new char[iLength];
            Random rd = new Random();

            for (int i = 0; i < iLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }

        public static String GetMD5Hash(string strInput)
        {
            if (strInput.Trim().Length == 0)
                return String.Empty;
            StringBuilder sbMD5Hash = new StringBuilder();
            MD5CryptoServiceProvider md5 = null;
            try
            {
                md5 = new MD5CryptoServiceProvider();
                byte[] data = System.Text.Encoding.ASCII.GetBytes(strInput);
                data = md5.ComputeHash(data);
                for (int i = 0; i < data.Length; i++)
                    sbMD5Hash.Append(data[i].ToString("x2"));
            }
            catch (CryptographicException Cex)
            {
                General.WriteLog("MD5", "Cryptographic Error:" + Cex.Message + ", InnerException:" + Cex.InnerException);
            }
            catch (Exception ex)
            {
                General.WriteLog("MD5", "Error:" + ex.Message + ", InnerException:" + ex.InnerException);
            }
            finally
            {
                md5.Clear();
            }
            return sbMD5Hash.ToString();
        }

        #region Serialization
        /// <summary>
        /// Convert class object into xml file
        /// </summary>
        /// <param name="_obj">class object</param>
        /// <returns></returns>
        public static string xmlSerialization(object _obj)
        {
            try
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                XmlDocument xDoc = new XmlDocument();

                XmlSerializer serializer = new XmlSerializer(_obj.GetType());
                using (MemoryStream ms = new MemoryStream())
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    // settings.Encoding = Encoding.GetEncoding("ISO-8859-1");
                    settings.Encoding = Encoding.GetEncoding("UTF-8");
                    ms.Seek(0, 0);
                    using (XmlWriter writer = XmlWriter.Create(ms, settings))
                    {
                        serializer.Serialize(writer, _obj, ns);
                    }
                    return settings.Encoding.GetString(ms.ToArray());

                }
            }
            catch //(Exception ex)
            {
                // APIGeneral.WriteErrorLog(ex.Message);
                //writeErrorLoginQueue("General", LogType.EXCEPTION, "xmlSerialization", "Exception " + ex.Message + ":Trace Log " + ex.StackTrace, DateTime.Now.ToString(), "General", "xmlSerialization", TAG.CS);
            }
            return "";
        }

        public static string xmlSerialization(object _obj, string DocTypeName, string DocTypePubId, string DocTypeSysId)
        {
            try
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                XmlDocument xDoc = new XmlDocument();

                XmlSerializer serializer = new XmlSerializer(_obj.GetType());
                using (MemoryStream ms = new MemoryStream())
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    // settings.Encoding = Encoding.GetEncoding("ISO-8859-1");
                    settings.Encoding = Encoding.GetEncoding("UTF-8");
                    ms.Seek(0, 0);
                    using (XmlWriter writer = XmlWriter.Create(ms, settings))
                    {
                        if (!string.IsNullOrEmpty(DocTypeName.Trim()))
                        {
                            writer.WriteDocType(DocTypeName, DocTypePubId, DocTypeSysId, null);
                            //writer.WriteDocType("applicants", "//National Informatics Center/", "../../files_uc09/llform.dtd", null);
                        }
                        serializer.Serialize(writer, _obj, ns);
                    }
                    return settings.Encoding.GetString(ms.ToArray());
                }
            }
            catch //(Exception ex)
            {
                // APIGeneral.WriteErrorLog(ex.Message);
                //writeErrorLoginQueue("General", LogType.EXCEPTION, "xmlSerialization", "Exception " + ex.Message + ":Trace Log " + ex.StackTrace, DateTime.Now.ToString(), "General", "xmlSerialization", TAG.CS);
            }

            return "";
        }
        #endregion

        #region DeSerialization
        /// <summary>
        /// This is used to De serialize the Data
        /// </summary>
        /// <param name="strXml">input xml string</param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object xmlDeSerialization(string strXml, Object obj)
        {
            try
            {
                XmlSerializer Deserializer = new XmlSerializer(obj.GetType());
                obj = Deserializer.Deserialize(new StringReader(strXml));
                Deserializer = null;

            }
            catch //(Exception ex)
            {
                //writeErrorLoginQueue("General", LogType.EXCEPTION, "xmlDeSerialization~" + strXml, "Exception " + ex.Message + ":Trace Log " + ex.StackTrace, DateTime.Now.ToString(), "General", "xmlDeSerialization", TAG.CS);
            }
            return obj;
        }
        #endregion

        #region My Transactions
        public bool InsertPaymentTrans(String strMobileNo, String strTransID, String strDept, String strDeptTransID, String strConsumerNo, String strConsumerName,
            String strService, String strPayMode, String strCardType, String strAuthCode, String strBatchNo, String strCCTransNo, String strBankReceiptNo,
            double dPaidAmount, double dCCCharges, double dTotalAmount, String strChannel, int iStatus)
        {
            bool bResult = false;
            try
            {
                if (strMobileNo.Length == 10)
                    strMobileNo = "91" + strMobileNo;

                object retVal = -1;
                using (DBFactory objDB = new DBFactory("DSN_PAYMENT"))
                {
                    objDB.AddInParam("MOBILENO", SqlType.VarChar, strMobileNo);
                    objDB.AddInParam("TRANSID", SqlType.VarChar, strTransID);
                    objDB.AddInParam("DEPARTMENT", SqlType.VarChar, strDept);
                    objDB.AddInParam("DEPARTMENTTRANSID", SqlType.VarChar, strDeptTransID);
                    objDB.AddInParam("CONSUMERNUMBER", SqlType.VarChar, strConsumerNo);
                    objDB.AddInParam("CONSUMERNAME", SqlType.NVarChar, strConsumerName);
                    objDB.AddInParam("SERVICE", SqlType.NVarChar, strService);
                    objDB.AddInParam("PAYMODE", SqlType.VarChar, strPayMode);
                    objDB.AddInParam("CARDTYPE", SqlType.VarChar, strCardType);
                    objDB.AddInParam("AUTHCODE", SqlType.VarChar, strAuthCode);
                    objDB.AddInParam("BATCHNUMBER", SqlType.VarChar, strBatchNo);
                    objDB.AddInParam("CCTRANSNO", SqlType.VarChar, strCCTransNo);
                    objDB.AddInParam("BANKRECEIPTNO", SqlType.VarChar, strBankReceiptNo);
                    objDB.AddInParam("PAIDAMOUNT", SqlType.Decimal, dPaidAmount);
                    objDB.AddInParam("CCCHARGES", SqlType.Decimal, dCCCharges);
                    objDB.AddInParam("TOTALAMOUNT", SqlType.Decimal, dTotalAmount);
                    objDB.AddInParam("CHANNEL", SqlType.VarChar, strChannel);
                    objDB.AddInParam("STATUS", SqlType.Bit, iStatus);
                    objDB.AddInParam("TYPE", SqlType.VarChar, "C");
                    objDB.AddOutParam("RETSTATUS", SqlType.VarChar, 10);
                    objDB.RunProc("PAYMENT_TRANS_GOK_CURD");
                    retVal = objDB.GetOutValue("RETSTATUS");
                }

                if (retVal.ToString() == "0")
                    bResult = true;
            }
            catch (Exception ex)
            {
                General.WriteLog("PAYMENT", "Exp:" + ex.Message + ", InnerExp" + ex.InnerException);
                General.WriteLog("PAYMENT_FAIL", "'" + strMobileNo + "','" + strTransID + "','" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "','" + strDept + "','" + strDeptTransID + "','" + strConsumerNo + "','" + strConsumerName + "','" + strService + "','" + strPayMode + "','" + strCardType + "','" + strAuthCode + "','" + strBatchNo + "','" + strCCTransNo + "','" + strBankReceiptNo + "'," + dPaidAmount + "," + dCCCharges + "," + dTotalAmount);
            }
            return bResult;
        }

        public DataTable GetRecipts(String strMobileNo)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DBFactory objDB = new DBFactory("DSN_PAYMENT"))
                {
                    objDB.AddInParam("MOBILENO", SqlType.VarChar, strMobileNo);
                    objDB.AddInParam("TYPE", SqlType.VarChar, "ALL");
                    objDB.AddOutParam("RETSTATUS", SqlType.VarChar, 10);
                    objDB.RunProc("PAYMENT_TRANS_GOK_CURD", out dt);
                }
            }
            catch (Exception ex)
            {
                General.WriteLog("PAYMENT", "Exp:" + ex.Message + ", InnerExp" + ex.InnerException);
                throw;
            }
            return dt;
        }

        public DataTable GetReciptData(String strMobileNo, String strTransID, String strDept)
        {
            DataTable dt = new DataTable();
            try
            {
                using (DBFactory objDB = new DBFactory("DSN_PAYMENT"))
                {
                    objDB.AddInParam("MOBILENO", SqlType.VarChar, strMobileNo);
                    objDB.AddInParam("TRANSID", SqlType.VarChar, strTransID);
                    objDB.AddInParam("DEPARTMENT", SqlType.VarChar, strDept);
                    objDB.AddInParam("TYPE", SqlType.VarChar, "R");
                    objDB.AddOutParam("RETSTATUS", SqlType.VarChar, 10);
                    objDB.RunProc("PAYMENT_TRANS_GOK_CURD", out dt);
                }
            }
            catch (Exception ex)
            {
                General.WriteLog("PAYMENT", "Exp:" + ex.Message + ", InnerExp" + ex.InnerException);
                throw;
            }
            return dt;
        }
        #endregion

        #region Payment Acknowledgement
        public bool InsertAcknowledmentTrans(String strMobileNo, String strChannel, String strTransID, String strDept, String strPayMode, String strCardType,
            String strAuthCode, String strTransCode, String strMcheckUniqueno, String strIMITransid, double dTotalAmount, int iStatus, String strAPIError)
        {
            bool bResult = false;
            try
            {
                if (strMobileNo.Length == 10)
                    strMobileNo = "91" + strMobileNo;

                object retVal = -1;
                using (DBFactory objDB = new DBFactory("DSN_PAYMENT"))
                {
                    objDB.AddInParam("MOBILENO", SqlType.VarChar, strMobileNo);
                    objDB.AddInParam("CHANNEL", SqlType.VarChar, strChannel);
                    objDB.AddInParam("TRANSID", SqlType.VarChar, strTransID);
                    objDB.AddInParam("DEPARTMENT", SqlType.VarChar, strDept);
                    objDB.AddInParam("PAYMODE", SqlType.VarChar, strPayMode);
                    objDB.AddInParam("CARDTYPE", SqlType.VarChar, strCardType);
                    objDB.AddInParam("AUTHCODE", SqlType.VarChar, strAuthCode);
                    objDB.AddInParam("TRANSCODE", SqlType.VarChar, strTransCode);
                    objDB.AddInParam("MCHECKNO", SqlType.VarChar, strMcheckUniqueno);
                    objDB.AddInParam("IMITRANSID", SqlType.VarChar, strIMITransid);
                    objDB.AddInParam("AMOUNT", SqlType.VarChar, dTotalAmount);
                    objDB.AddInParam("STATUS", SqlType.Bit, iStatus);
                    objDB.AddInParam("APIERROR", SqlType.NVarChar, strAPIError);
                    objDB.AddInParam("TYPE", SqlType.VarChar, "C");
                    objDB.AddOutParam("RETSTATUS", SqlType.VarChar, 10);
                    objDB.RunProc("PAYMENT_ACK_TRANS_GOK_CURD");
                    retVal = objDB.GetOutValue("RETSTATUS");
                }

                if (retVal.ToString() == "0")
                    bResult = true;
            }
            catch (Exception ex)
            {
                General.WriteLog("PAYMENT_ACK", "Exp:" + ex.Message + ", InnerExp" + ex.InnerException);
                General.WriteLog("PAYMENT_ACK_FAIL", "'" + strMobileNo + "','" + strChannel + "','" + strTransID + "','" + strDept + "','" + strPayMode + "','" + strCardType + "','" + strAuthCode + "','" + strMcheckUniqueno + "','" + strIMITransid + "'," + dTotalAmount + "," + iStatus + ",'" + strAPIError + "'");
            }
            return bResult;
        }

        public bool InsertAcknowledmentTrans(String strMobileNo, String strChannel, String strTransID, String strDept, String strPayMode, String strCardType,
            String strAuthCode, String strTransCode, String strMcheckUniqueno, String strIMITransid, double dTotalAmount, int iStatus, String strAPIError, String strAckResponse)
        {
            bool bResult = false;
            try
            {
                if (strMobileNo.Length == 10)
                    strMobileNo = "91" + strMobileNo;

                object retVal = -1;
                using (DBFactory objDB = new DBFactory("DSN_PAYMENT"))
                {
                    objDB.AddInParam("MOBILENO", SqlType.VarChar, strMobileNo);
                    objDB.AddInParam("CHANNEL", SqlType.VarChar, strChannel);
                    objDB.AddInParam("TRANSID", SqlType.VarChar, strTransID);
                    objDB.AddInParam("DEPARTMENT", SqlType.VarChar, strDept);
                    objDB.AddInParam("PAYMODE", SqlType.VarChar, strPayMode);
                    objDB.AddInParam("CARDTYPE", SqlType.VarChar, strCardType);
                    objDB.AddInParam("AUTHCODE", SqlType.VarChar, strAuthCode);
                    objDB.AddInParam("TRANSCODE", SqlType.VarChar, strTransCode);
                    objDB.AddInParam("MCHECKNO", SqlType.VarChar, strMcheckUniqueno);
                    objDB.AddInParam("IMITRANSID", SqlType.VarChar, strIMITransid);
                    objDB.AddInParam("AMOUNT", SqlType.VarChar, dTotalAmount);
                    objDB.AddInParam("STATUS", SqlType.Bit, iStatus);
                    objDB.AddInParam("APIERROR", SqlType.NVarChar, strAPIError);
                    objDB.AddInParam("ACKRESP", SqlType.NVarChar, strAckResponse);
                    objDB.AddInParam("TYPE", SqlType.VarChar, "C");
                    objDB.AddOutParam("RETSTATUS", SqlType.VarChar, 10);
                    objDB.RunProc("PAYMENT_ACK_TRANS_GOK_CURD");
                    retVal = objDB.GetOutValue("RETSTATUS");
                }

                if (retVal.ToString() == "0")
                    bResult = true;
            }
            catch (Exception ex)
            {
                General.WriteLog("PAYMENT_ACK", "Exp:" + ex.Message + ", InnerExp" + ex.InnerException);
                General.WriteLog("PAYMENT_ACK_FAIL", "'" + strMobileNo + "','" + strChannel + "','" + strTransID + "','" + strDept + "','" + strPayMode + "','" + strCardType + "','" + strAuthCode + "','" + strMcheckUniqueno + "','" + strIMITransid + "'," + dTotalAmount + "," + iStatus + ",'" + strAPIError + "'");
            }
            return bResult;
        }

        public bool CancelTrans(String strMobileNo, String strChannel, String strRefNo, String strDept, String strPNR, double dPaidAmount,
            double dRefundAmount, int iStatus, String strAPIError)
        {
            bool bResult = false;
            try
            {
                if (strMobileNo.Length == 10)
                    strMobileNo = "91" + strMobileNo;

                object retVal = -1;
                using (DBFactory objDB = new DBFactory("DSN_PAYMENT"))
                {
                    objDB.AddInParam("MOBILENO", SqlType.VarChar, strMobileNo);
                    objDB.AddInParam("CHANNEL", SqlType.VarChar, strChannel);
                    objDB.AddInParam("REFERENCENO", SqlType.VarChar, strRefNo);
                    objDB.AddInParam("PNR", SqlType.VarChar, strPNR);
                    objDB.AddInParam("PAIDAMOUNT", SqlType.Decimal, dPaidAmount);
                    objDB.AddInParam("REFUNDAMOUNT", SqlType.Decimal, dRefundAmount);
                    objDB.AddInParam("STATUS", SqlType.Bit, iStatus);
                    objDB.AddInParam("APIERROR", SqlType.NVarChar, strAPIError);
                    objDB.AddInParam("DEPARTMENT", SqlType.VarChar, strDept);
                    objDB.AddInParam("TYPE", SqlType.VarChar, "C");
                    objDB.AddOutParam("RETSTATUS", SqlType.VarChar, 10);
                    objDB.RunProc("PAYMENT_CANCEL_TRANS_GOK_CURD");
                    retVal = objDB.GetOutValue("RETSTATUS");
                }

                if (retVal.ToString() == "0")
                    bResult = true;
            }
            catch (Exception ex)
            {
                General.WriteLog("PAYMENT_CANCEL", "Exp:" + ex.Message + ", InnerExp" + ex.InnerException);
                General.WriteLog("PAYMENT_CANCEL_FAIL", "'" + strMobileNo + "','" + strChannel + "','" + strRefNo + "','" + strDept + "','" + strPNR + "'," + dPaidAmount + "," + dRefundAmount + "," + iStatus + ",'" + strAPIError + "','" + strDept + "'");
            }
            return bResult;
        }
        #endregion
    }
}
