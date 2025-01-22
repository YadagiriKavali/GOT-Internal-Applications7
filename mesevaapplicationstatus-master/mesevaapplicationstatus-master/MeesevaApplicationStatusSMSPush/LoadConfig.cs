using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CommonHelper;

namespace MeesevaApplicationStatusSMSPush
{
    public class LoadConfig
    {
        public static XmlDocument xDocEsevaConfig;
       // public static MemoryCache objCache = MemoryCache.Default;

        public static void LoadEsevaConfig()
        {
            try
            {
                if (xDocEsevaConfig == null || xDocEsevaConfig.DocumentElement == null)
                {
                    String strPath = General.GetConfigVal("SERVICE_XML_PATH") + "\\config.xml";
                    if (!System.IO.File.Exists(strPath))
                        return;

                    xDocEsevaConfig = null;
                    xDocEsevaConfig = new XmlDocument();
                    try
                    {
                        xDocEsevaConfig.Load(strPath);
                       // CacheItemPolicy policy = new CacheItemPolicy();
                        List<string> filePaths = new List<string>();
                        filePaths.Add(strPath);
                    }
                    catch (System.Exception ex)
                    {
                        General.WriteLog("GOT", "Exception in LoadEsevaConfig==>" + ex.Message);
                    }
                }
            }
            catch (System.Exception ex)
            {
               // LogData.Write("GOTAPI", "ESEVA", LogMode.Excep, ex, string.Format("LoadConfig => Process- Ex:{0}", "Error" + ex.Message + ",Source:" + ex.Source + ",StackTrace:" + ex.StackTrace + ",InnerException:" + ex.InnerException));
            }
        }

        //public static AuthRequestBean GetAuthDetails()
        //{
        //    AuthRequestBean objAuthRequestBean = null;
        //    try
        //    {
        //        LoadEsevaConfig();
        //        var xmlEsevaInfo = xDocEsevaConfig.DocumentElement.SelectSingleNode("ESEVA/AUTHENTICATION");
        //        if (xmlEsevaInfo != null)
        //        {
        //            objAuthRequestBean = new AuthRequestBean
        //            {
        //                groupid = xmlEsevaInfo.Attributes["Groupid"] != null ? xmlEsevaInfo.Attributes["Groupid"].Value : string.Empty,
        //                userId = xmlEsevaInfo.Attributes["UserId"] != null ? xmlEsevaInfo.Attributes["UserId"].Value : string.Empty,
        //                password = xmlEsevaInfo.Attributes["Password"] != null ? xmlEsevaInfo.Attributes["Password"].Value : string.Empty,
        //                serviceid = xmlEsevaInfo.Attributes["Serviceid"] != null ? xmlEsevaInfo.Attributes["Serviceid"].Value : string.Empty,
        //                strStaffcode = xmlEsevaInfo.Attributes["Staffcode"] != null ? xmlEsevaInfo.Attributes["Staffcode"].Value : string.Empty,
        //                strCentrecode = xmlEsevaInfo.Attributes["Centrecode"] != null ? xmlEsevaInfo.Attributes["Centrecode"].Value : string.Empty,
        //                strdeptcode = xmlEsevaInfo.Attributes["Deptcode"] != null ? xmlEsevaInfo.Attributes["Deptcode"].Value : string.Empty,
        //                strDistcode = xmlEsevaInfo.Attributes["Distcode"] != null ? xmlEsevaInfo.Attributes["Distcode"].Value : string.Empty
        //            };
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        LogData.Write("GOTAPI", "ESEVA", LogMode.Excep, ex, string.Format("LoadConfig- GetAuthDetails- Ex:{0}", ex.Message));
        //    }
        //    return objAuthRequestBean;
        //}

        //public static string GetDeptCode(string dept)
        //{
        //    try
        //    {
        //        LoadEsevaConfig();
        //        var xmlEsevaInfo = xDocEsevaConfig.DocumentElement.SelectSingleNode("ESEVA/DEPTS/" + dept);
        //        return xmlEsevaInfo != null && xmlEsevaInfo.Attributes["DeptCode"] != null ? xmlEsevaInfo.Attributes["DeptCode"].Value : string.Empty;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        LogData.Write("GOTAPI", "ESEVA", LogMode.Excep, ex, string.Format("LoadConfig- GetDeptCode- Ex:{0}", ex.Message));
        //    }

        //    return string.Empty;
        //}

        //public static string GetErrorDescription(string errorCode)
        //{
        //    try
        //    {
        //        LoadEsevaConfig();
        //        var xmlEsevaInfo = xDocEsevaConfig.DocumentElement.SelectSingleNode("ESEVA/ERRORS/CODE_" + errorCode);
        //        return xmlEsevaInfo != null ? xmlEsevaInfo.InnerText.Trim() : string.Empty;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        LogData.Write("GOTAPI", "ESEVA", LogMode.Excep, ex, string.Format("LoadConfig- GetErrorDescription- Ex:{0}", ex.Message));
        //    }
        //    return string.Empty;
        //}
    }
}
