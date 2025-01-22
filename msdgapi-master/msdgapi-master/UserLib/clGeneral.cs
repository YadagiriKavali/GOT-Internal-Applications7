using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Caching;
using System.Xml;
using IMI.Logger;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using User.Cryptography;

namespace User
{
    public static class clGeneral
    {
        public static int WebReqTimeOut = 30000;
        public static int CONNECTION_LIMIT = 10;

        public static XmlDocument xmsgDocAPI;
        public static CacheDependency objmsgCacheAPI;
        public static XmlDocument xDocAPI;
        public static CacheDependency objCacheAPI;
        /// <summary>
        /// Get value from configuration file.
        /// </summary>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string GetConfigValue(string sKey)
        {
            return ConfigurationManager.AppSettings[sKey] ?? "";
        }
        public static int ConvertoInt(string sValue)
        {
            int iIntValue = 0;
            int.TryParse(sValue, out iIntValue);
            return iIntValue;
        }
        public static string GetMessage(string strCode)
        {

            string strMessage = string.Empty;

            try
            {
                string strPath = GetConfigValue("ERRORMSGXML_PATH") + "Messages.xml";
                if (xmsgDocAPI == null || objmsgCacheAPI == null || objmsgCacheAPI.HasChanged)
                {
                    xmsgDocAPI = new XmlDocument();
                    xmsgDocAPI.Load(strPath);
                    objmsgCacheAPI = new CacheDependency(strPath, DateTime.Now);

                }
                if (xmsgDocAPI != null)
                {
                    XmlNode xNode = xmsgDocAPI.DocumentElement.SelectSingleNode("MSG_" + strCode);
                    if (xNode != null)
                        strMessage = xNode.InnerText;
                }

            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clGeneral", LogMode.Excep, ex, string.Format("GetMessage-->Ex:{0}", ex.Message));
            }
            return strMessage;
        }
        /// <summary>
        /// Method to get the email templates
        /// </summary>
        /// <param name="strId"></param>
        /// <returns></returns>
        public static XmlNode GetEmailTemplateMessage(string strId)
        {

            XmlNode xNode = null;
            string strMessage = string.Empty;
            try
            {
                string strPath = GetConfigValue("ERRORMSGXML_PATH") + "EmailTemplate.xml";
                if (xDocAPI == null || objCacheAPI == null || objCacheAPI.HasChanged)
                {
                    xDocAPI = new XmlDocument();
                    xDocAPI.Load(strPath);
                    objCacheAPI = new CacheDependency(strPath, DateTime.Now);

                }
                if (xDocAPI != null)
                {

                    xNode = xDocAPI.DocumentElement.SelectSingleNode("TEMPLATE[@ID='" + strId + "']");
                }

                //if (File.Exists(GetConfigValue("ERRORMSGXML_PATH") + "EmailTemplate.xml"))
                //{
                //    xDoc.Load(GetConfigValue("ERRORMSGXML_PATH") + "EmailTemplate.xml");
                //    if (xDoc != null)
                //    {
                //        xNode = xDoc.DocumentElement.SelectSingleNode("TEMPLATE[@ID='" + strId + "']");

                //    }
                //}
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clGeneral", LogMode.Excep, ex, string.Format("GetEmailTemplateMessage-->Ex:{0}", ex.Message));
            }
            return xNode;
        }

        /// <summary>
        /// Method to get sms template message for templateid
        /// </summary>
        /// <param name="strId"></param>
        /// <returns></returns>
        public static string GetSMSTemplateMessage(string strId)
        {
            XmlDocument xDoc = new XmlDocument();
            XmlNode xNode = null;
            string strMessage = string.Empty;
            try
            {
                string strpath = GetConfigValue("ERRORMSGXML_PATH") + "SMSAlerts.xml";
                // LogData.Write("UserLib", "clGeneral", LogMode.Debug, "smstemplatemsssage:" + strpath,0);
                if (File.Exists(strpath))
                {
                    xDoc.Load(strpath);
                    if (xDoc != null)
                    {
                        xNode = xDoc.DocumentElement.SelectSingleNode("TEMPLATE[@ID='" + strId.Trim() + "']");
                        if (xNode != null)
                            strMessage = xNode.SelectSingleNode("MESSAGE").InnerText;
                        // LogData.Write("UserLib", "clGeneral", LogMode.Debug, "smstemplatemsssage:" + strId+ xDoc.InnerXml, 0);
                    }
                }
                // LogData.Write("UserLib", "clGeneral", LogMode.Debug, "smstemplatemsssage:" + strMessage, 0);
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clGeneral", LogMode.Excep, ex, string.Format("GetSMSTemplateMessage-->Ex:{0}", ex.Message));
            }
            return strMessage;
        }

        /// <summary>
        /// Creating excel file
        /// </summary>
        /// <param name="sPath"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string CreateExcel(string sPath, DataTable dt, string reportname)
        {
            string sFile = "";
            try
            {
                if (!Directory.Exists(sPath))
                    Directory.CreateDirectory(sPath);

                //string reportname = "report";
                sFile = reportname + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
                FileInfo newFile = new FileInfo(sPath + @"\" + sFile);
                if (newFile.Exists)
                {
                    newFile.Delete();  // ensures we create a new workbook
                    newFile = new FileInfo(sPath + @"\sample1.xlsx");
                }
                string sheetName = "";
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    sheetName = "sheet1";
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);
                    DataTable objData = dt;
                    for (int intC = 1; intC <= objData.Rows.Count + 1; intC++)
                    {
                        if (intC == 1)
                        {
                            for (int col = 1; col <= objData.Columns.Count; col++)
                            {
                                worksheet.Cells[1, col].Value = objData.Columns[col - 1].ColumnName;
                            }

                            using (var range = worksheet.Cells[1, 1, 1, objData.Columns.Count])
                            {
                                range.Style.Font.Bold = true;
                                range.Style.Fill.PatternType = ExcelFillStyle.Solid;

                                range.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#017dbb"));
                                range.Style.Font.Color.SetColor(Color.White);
                            }
                        }
                        else
                        {
                            for (int col = 1; col <= objData.Columns.Count; col++)
                            {
                                worksheet.Cells[intC, col].Value = objData.Rows[intC - 2][col - 1].ToString();
                                //worksheet.Cells[charArray[col] + col.ToString()].Value = objData.Rows[intC - 2][col - 1].ToString();
                                //if (readOnlyCells.ToLower().IndexOf("," + objData.Columns[col - 1].ColumnName.ToLower() + ",") == -1)
                                //    cell.Locked = false;
                            }
                        }
                    }
                    //worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells
                    //worksheet.Cells.Style.WrapText = true;

                    // worksheet.Cells.Style.ShrinkToFit = true;
                    // lets set the header text 
                    worksheet.HeaderFooter.OddHeader.CenteredText = "&24&U&\"Arial,Regular Bold\" Inventory";
                    // add the page number to the footer plus the total number of pages
                    worksheet.HeaderFooter.OddFooter.RightAlignedText =
                        string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
                    // add the sheet name to the footer
                    worksheet.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
                    // add the file path to the footer
                    worksheet.HeaderFooter.OddFooter.LeftAlignedText = ExcelHeaderFooter.FilePath + ExcelHeaderFooter.FileName;
                    // set some document properties
                    //package.Workbook.Properties.Title = "";
                    //package.Workbook.Properties.Author = " ";
                    //package.Workbook.Properties.Comments = "";

                    // set some extended property values
                    package.Workbook.Properties.Company = "";

                    // set some custom property values
                    //package.Workbook.Properties.SetCustomPropertyValue("Checked by", "Jan Källman");
                    //package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");
                    // save our new workbook and we are done!
                    package.Save();
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clGeneral", LogMode.Excep, ex, string.Format("GetSMSTemplateMessage-->Ex:{0}", ex.Message));
                sFile = "";
            }
            return sFile;
        }

        #region CONVERTION(S)
        public static DateTime ConvertDateTime(string sdtTime)
        {
            DateTime dtTime = DateTime.Now;
            if (!string.IsNullOrEmpty(sdtTime))
            {
                DateTime.TryParse(sdtTime, out dtTime);
            }
            return dtTime;
        }
        public static int ConvertInt(string strVal)
        {
            int intValue;
            int.TryParse(strVal, out intValue);
            return intValue;
        }

        public static long ConvertLong(string sVal)
        {
            long lValue;
            long.TryParse(sVal, out lValue);
            return lValue;
        }
        public static DateTime formatDate(string _dtDate)
        {
            try
            {
                string[] formats = {"dd/MM/yyyy h:mm:ss tt", "dd/MM/yyyy h:mm tt","dd/MMM/yyyy h:mm tt",  
                   "dd/MM/yyyy hh:mm:ss", "dd/MM/yyyy h:mm:ss","dd/MMM/yyyy h:mm A",
                   "dd/MM/yyyy hh:mm tt", "dd/MM/yyyy hh tt", 
                   "dd/MM/yyyy h:mm", "dd/MM/yyyy h:mm", 
                   "dd/MM/yyyy hh:mm", "dd/MM/yyyy hh:mm","dd/MM/yyyy hh:mm:s","dd/MM/yyyy hh:m:s","dd/MM/yyyy h:m:s","dd/MM/yyyy h:mm:s","dd/MM/yyyy hh:m:ss","yyyy-MM-dd HH:mm:ss","dd-MM-yyyy HH:mm","dd-MM-yyyy hh:mm tt","dd-MM-yyyy"};
                DateTime _ConvertedDate = DateTime.Now;
                if (!DateTime.TryParseExact(_dtDate, "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _ConvertedDate))
                {
                    DateTime.TryParseExact(_dtDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out _ConvertedDate);
                }
                return _ConvertedDate;
            }
            catch (Exception ex)
            {


            }
            return DateTime.Now;
        }
        #endregion

        /// <summary>
        /// This method is to get the output response
        /// </summary>
        /// <param name="msisdn"></param>
        /// <returns></returns>
        public static string getOutputResponse(string errorcode, string message)
        {
            UResponse oresp = null;
            string response = string.Empty;
            try
            {
                oresp = new UResponse
                {
                    ResCode = errorcode,
                    ResDesc = message
                };
                response = JsonConvert.SerializeObject(oresp);
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clGeneral", LogMode.Excep, ex, "clGeneral- getOutputResponse errorcode" + errorcode + " message " + message);
            }
            finally
            {

            }
            return response;
        }

        /// <summary>
        /// This method is to get the output response
        /// </summary>
        /// <param name="msisdn"></param>
        /// <returns></returns>
        public static UResponse getOutputObjResponse(string errorcode, string message)
        {
            UResponse oresp = null;
            string response = string.Empty;
            try
            {
                oresp = new UResponse
                {
                    ResCode = errorcode,
                    ResDesc = message
                };
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clGeneral", LogMode.Excep, ex, "clGeneral- getOutputObjResponse errorcode" + errorcode + " message " + message);
            }
            finally
            {

            }
            return oresp;
        }

        ///// <summary>
        ///// This method is to get the output response
        ///// </summary>
        ///// <param name="msisdn"></param>
        ///// <returns></returns>
        //public static clEResp getOutputObjResponse(string errorcode, clEUsers objUser)
        //{
        //    clEResp oresp = null;
        //    string response = string.Empty;
        //    try
        //    {
        //        oresp = new clEResp
        //        {
        //            ResCode = errorcode,
        //            ResDesc = objUser
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        LogData.Write("UserLib", "clGeneral", LogMode.Excep, ex, "clGeneral- getOutputObjResponse errorcode" + errorcode + " message " + objUser.ToString());
        //    }
        //    finally
        //    {

        //    }
        //    return oresp;
        //}

        /// <summary>
        ///  Purpose: Common http web request method.
        ///  Author :Raghava Reddy.K
        ///  
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="strAction"></param>
        public static string DoHttpGet(string strUrl, string strAction)
        {
            string _strResponse = "";
            if (!string.IsNullOrEmpty(strUrl))
            {
                long lStartTime = DateTime.Now.Ticks;
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strUrl));
                    request.Timeout = WebReqTimeOut;
                    ServicePointManager.Expect100Continue = false;
                    ServicePointManager.DefaultConnectionLimit = CONNECTION_LIMIT;
                    ServicePointManager.UseNagleAlgorithm = false;
                    using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
                    {
                        _strResponse = reader.ReadToEnd().ToString();
                    }

                }
                catch (WebException WebEx)
                {
                    if (WebEx.Status == WebExceptionStatus.Timeout)
                    {
                        LogData.Write("UserLib", "clGeneral", LogMode.Error, string.Format("WebRequestHandler-WebReqGetMethod->WebEx:{0}", WebEx.Message));
                    }
                    else
                    {
                        LogData.Write("UserLib", "clGeneral", LogMode.Error, string.Format("WebRequestHandler-WebReqGetMethod->WebEx:{0}", WebEx.Message));
                    }

                }
                catch (Exception ex)
                {
                    LogData.Write("UserLib", "clGeneral", LogMode.Error, string.Format("WebRequestHandler-WebReqGetMethod->Ex:{0}", ex.Message));

                }
                finally
                {
                    LogData.Write("UserLib", "clGeneral", LogMode.Debug, string.Format("WebRequestHandler-WebReqGetMethod->Req:{0} | Res:{1} | tt: {2} ", strUrl, _strResponse, (DateTime.Now.Ticks - lStartTime) / 10000));
                }
            }
            else
            {
                // url should not blank
            }

            return _strResponse;
        }

        /// <summary>
        /// Purpose: Common HTTP POST method.
        /// Author:  Raghava Reddy.K
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="strAction"></param>
        public static string DoHttpPost(string strUrl, string strAction, string sPayload)
        {
            long lStartTime = DateTime.Now.Ticks;
            WebResponse result = null;
            string _strResponse = "";
            try
            {
                WebRequest req = WebRequest.Create(strUrl);
                req.Timeout = WebReqTimeOut;
                req.Method = "POST";
                if (strAction == "json")
                    req.ContentType = "application/json";
                else
                    req.ContentType = "application/xml";
                StringBuilder UrlEncoded = new StringBuilder();
                Char[] reserved = { '?', '=', '&' };
                byte[] SomeBytes = null;

                try
                {
                    ServicePointManager.DefaultConnectionLimit = CONNECTION_LIMIT;
                }
                catch { }

                if (!string.IsNullOrEmpty(sPayload))
                {
                    int i = 0, j;
                    while (i < sPayload.Length)
                    {
                        j = sPayload.IndexOfAny(reserved, i);
                        if (j == -1)
                        {
                            UrlEncoded.Append(sPayload.Substring(i, sPayload.Length - i));
                            break;
                        }
                        UrlEncoded.Append(sPayload.Substring(i, j - i));
                        UrlEncoded.Append(sPayload.Substring(j, 1));
                        i = j + 1;
                    }
                    SomeBytes = Encoding.UTF8.GetBytes(UrlEncoded.ToString());
                    req.ContentLength = SomeBytes.Length;
                    using (Stream newStream = req.GetRequestStream())
                    {
                        newStream.Write(SomeBytes, 0, SomeBytes.Length);
                    }
                }
                else
                {
                    req.ContentLength = 0;
                }
                result = req.GetResponse();
                using (Stream ReceiveStream = result.GetResponseStream())
                {
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    using (StreamReader sr = new StreamReader(ReceiveStream, encode))
                    {
                        Char[] read = new Char[256];
                        int count = sr.Read(read, 0, 256);
                        StringBuilder sb = new StringBuilder();
                        while (count > 0)
                        {
                            String str = new String(read, 0, count);
                            sb.Append(str);
                            count = sr.Read(read, 0, 256);
                        }
                        _strResponse = sb.ToString();
                    }
                }
                if (result != null)
                {
                    result.Close();
                }
            }
            catch (WebException Ex)
            {
                LogData.Write("UserLib", "clGeneral", LogMode.Excep, "WebException URL:" + strUrl + "Payload: " + sPayload + "Exception " + Ex);

            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clGeneral", LogMode.Excep, ex, "Ex URL:" + strUrl + "Payload: " + sPayload);

            }
            finally
            {
                LogData.Write("UserLib", "clGeneral", LogMode.Debug, "Ex URL:" + strUrl + "Payload: " + sPayload + ":Res:" + _strResponse + ":tt:" + (DateTime.Now.Ticks - lStartTime) / 10000);
            }
            return _strResponse;
        }

        /// <summary>
        /// Purpose: Common HTTP POST method.
        /// Author:  Raghava Reddy.K
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="strAction"></param>
        public static string DoHttpPost(ExternalURLData objexternal)
        {
            string strUrl = objexternal.url.ToString();
            string strAction = "json";
            string sPayload = objexternal.payload.ToString();
            long lStartTime = DateTime.Now.Ticks;
            WebResponse result = null;
            string _strResponse = "";
            try
            {
                WebRequest req = WebRequest.Create(strUrl);
                req.Timeout = WebReqTimeOut;
                req.Method = "POST";
                if (strAction == "json")
                    req.ContentType = "application/json";
                else
                    req.ContentType = "application/xml";
                StringBuilder UrlEncoded = new StringBuilder();
                Char[] reserved = { '?', '=', '&' };
                byte[] SomeBytes = null;

                try
                {
                    ServicePointManager.DefaultConnectionLimit = CONNECTION_LIMIT;
                }
                catch { }

                if (!string.IsNullOrEmpty(sPayload))
                {
                    int i = 0, j;
                    while (i < sPayload.Length)
                    {
                        j = sPayload.IndexOfAny(reserved, i);
                        if (j == -1)
                        {
                            UrlEncoded.Append(sPayload.Substring(i, sPayload.Length - i));
                            break;
                        }
                        UrlEncoded.Append(sPayload.Substring(i, j - i));
                        UrlEncoded.Append(sPayload.Substring(j, 1));
                        i = j + 1;
                    }
                    SomeBytes = Encoding.UTF8.GetBytes(UrlEncoded.ToString());
                    req.ContentLength = SomeBytes.Length;
                    using (Stream newStream = req.GetRequestStream())
                    {
                        newStream.Write(SomeBytes, 0, SomeBytes.Length);
                    }
                }
                else
                {
                    req.ContentLength = 0;
                }
                result = req.GetResponse();
                using (Stream ReceiveStream = result.GetResponseStream())
                {
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    using (StreamReader sr = new StreamReader(ReceiveStream, encode))
                    {
                        Char[] read = new Char[256];
                        int count = sr.Read(read, 0, 256);
                        StringBuilder sb = new StringBuilder();
                        while (count > 0)
                        {
                            String str = new String(read, 0, count);
                            sb.Append(str);
                            count = sr.Read(read, 0, 256);
                        }
                        _strResponse = sb.ToString();
                    }
                }
                if (result != null)
                {
                    result.Close();
                }
            }
            catch (WebException Ex)
            {
                LogData.Write("UserLib", "clGeneral", LogMode.Excep, "WebException URL:" + strUrl + "Payload: " + sPayload + "Exception " + Ex);

            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clGeneral", LogMode.Excep, ex, "Ex URL:" + strUrl + "Payload: " + sPayload);

            }
            finally
            {
                LogData.Write("UserLib", "clGeneral", LogMode.Debug, "Ex URL:" + strUrl + "Payload: " + sPayload + ":Res:" + _strResponse + ":tt:" + (DateTime.Now.Ticks - lStartTime) / 10000);
            }
            return _strResponse;
        }

        /// <summary>
        /// This method is to get the formmated msisdn
        /// </summary>
        /// <param name="msisdn"></param>
        /// <returns></returns>
        public static string getFormattedMsisdn(string msisdn)
        {
            try
            {
                if (msisdn.Length > 10)
                    msisdn = msisdn.Substring(msisdn.Length - 10, 10);
                if (msisdn.Length == 10)
                    msisdn = string.Format("{0}{1}", "91", msisdn);
                else if (msisdn.Length == 11)
                    msisdn = string.Format("{0}{1}", "91", msisdn.Substring(1));
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clGeneral", LogMode.Excep, ex, "GenLib - clGeneral- getFormattedMsisdn " + msisdn);
            }
            finally
            {

            }
            return msisdn;
        }

        /// <summary>
        /// This method is to get the default output response
        /// </summary>
        /// <param name="msisdn"></param>
        /// <returns></returns>
        public static string getDefaultResponse()
        {
            UResponse oresp = null;
            string response = string.Empty;
            try
            {
                oresp = new UResponse
                {
                    ResCode = "0",
                    ResDesc = "We are unable to process your request. Try again later"
                };
                response = JsonConvert.SerializeObject(oresp);
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clGeneral", LogMode.Excep, ex, "CallPatchUI - clGeneral- getDefaultResponse ");
            }
            finally
            {

            }
            return response;
        }

        /// <summary>
        /// This method is to get the default output response
        /// </summary>
        /// <param name="msisdn"></param>
        /// <returns></returns>
        public static UResponse getDefaultObjResponse()
        {
            UResponse oresp = null;
            string response = string.Empty;
            try
            {
                oresp = new UResponse
                {
                    ResCode = "0",
                    ResDesc = "We are unable to process your request. Try again later"
                };
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clGeneral", LogMode.Excep, ex, "CallPatchUI - clGeneral- getDefaultObjResponse ");
            }
            finally
            {

            }
            return oresp;
        }

        /// <summary>
        /// COmmon SUccess method
        /// </summary>
        /// <param name="errormsg"></param>
        /// <returns></returns>
        public static Error getFailedmsg(string errorcode, string errormsg)
        {
            Error oerror = null;
            try
            {
                oerror = new Error()
                {
                    ErrorCode = errorcode,
                    ErrorMessge = errormsg
                };
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clGeneral", LogMode.Excep, ex, "getFailedmsg");
            }
            return oerror;
        }

        /// <summary>
        /// Method to ecrypt val using RC4
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static string Encrypt(string strValue)
        {
            string strEncrypted = string.Empty;
            try
            {
                strEncrypted = new RC4(strValue).EnDeCrypt();
                strEncrypted = RC4.StrToHexStr(strEncrypted);
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clGeneral", LogMode.Excep, ex, "Encrypt");
            }
            return strEncrypted;

        }
    }
}
