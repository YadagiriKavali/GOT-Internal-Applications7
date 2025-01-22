using eseva.Data;
using IMI.Logger;
using System;
using System.Configuration;
using System.IO;
using System.Web.Caching;
using System.Xml;

namespace eseva.Models
{
    public class CacheConfigXml
    {
        #region Private Members

        private static CacheDependency cacheDepConfig;
        private static XmlDocument XmlDoc;
        private static string ESEVA_CONFIG_XML_PATH;

        #endregion Private Members

        #region Public Methods

        public static AuthenticateMasterData GetAuthDetails()
        {
            AuthenticateMasterData authData = null;
            try
            {
                LoadConfigSettings();
                var xmlEsevaInfo = XmlDoc.DocumentElement.SelectSingleNode("ESEVA/AUTHENTICATION");
                if (xmlEsevaInfo != null)
                {
                    authData = new AuthenticateMasterData
                    {
                        Groupid = xmlEsevaInfo.Attributes["Groupid"] != null ? xmlEsevaInfo.Attributes["Groupid"].Value : string.Empty,
                        UserId = xmlEsevaInfo.Attributes["UserId"] != null ? xmlEsevaInfo.Attributes["UserId"].Value : string.Empty,
                        Password = xmlEsevaInfo.Attributes["Password"] != null ? xmlEsevaInfo.Attributes["Password"].Value : string.Empty,
                        Serviceid = xmlEsevaInfo.Attributes["Serviceid"] != null ? xmlEsevaInfo.Attributes["Serviceid"].Value : string.Empty,
                        Staffcode = xmlEsevaInfo.Attributes["Staffcode"] != null ? xmlEsevaInfo.Attributes["Staffcode"].Value : string.Empty,
                        Centrecode = xmlEsevaInfo.Attributes["Centrecode"] != null ? xmlEsevaInfo.Attributes["Centrecode"].Value : string.Empty,
                        Deptcode = xmlEsevaInfo.Attributes["Deptcode"] != null ? xmlEsevaInfo.Attributes["Deptcode"].Value : string.Empty,
                        Distcode = xmlEsevaInfo.Attributes["Distcode"] != null ? xmlEsevaInfo.Attributes["Distcode"].Value : string.Empty
                    };
                }
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva", LogMode.Excep, ex, string.Format("cacheConfigXml- getAuthDetails- Ex:{0}", ex.Message));
            }

            return authData;
        }

        public static string GetDeptCode(string dept)
        {
            try
            {
                LoadConfigSettings();
                var xmlEsevaInfo = XmlDoc.DocumentElement.SelectSingleNode("ESEVA/DEPTS/" + dept);
                return xmlEsevaInfo != null && xmlEsevaInfo.Attributes["DeptCode"] != null ? xmlEsevaInfo.Attributes["DeptCode"].Value : string.Empty;
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva", LogMode.Excep, ex, string.Format("cacheConfigXml- getDeptCode- Ex:{0}", ex.Message));
            }

            return string.Empty;
        }

        public static string GetErrorDescription(string errorCode)
        {
            try
            {
                LoadConfigSettings();
                var xmlEsevaInfo = XmlDoc.DocumentElement.SelectSingleNode("ESEVA/ERRORS/CODE_" + errorCode);
                return xmlEsevaInfo != null ? xmlEsevaInfo.InnerText.Trim() : string.Empty;
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva", LogMode.Excep, ex, string.Format("cacheConfigXml- getAuthenticationDetails- Ex:{0}", ex.Message));
            }
            return string.Empty;
        }

        #endregion Public Methods

        #region Private Methods

        private static void LoadConfigSettings()
        {
            ESEVA_CONFIG_XML_PATH = ConfigurationManager.AppSettings["ESEVA_CONFIG_XML_PATH"] ?? "";
            try
            {
                if (!string.IsNullOrEmpty(ESEVA_CONFIG_XML_PATH) && File.Exists(ESEVA_CONFIG_XML_PATH))
                {
                    if (cacheDepConfig == null || cacheDepConfig.HasChanged || XmlDoc == null || XmlDoc.DocumentElement == null)
                    {
                        cacheDepConfig = new CacheDependency(ESEVA_CONFIG_XML_PATH, DateTime.Now);
                        XmlDoc = new XmlDocument();
                        var reader = XmlReader.Create(ESEVA_CONFIG_XML_PATH);
                        XmlDoc.Load(reader);
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva", LogMode.Excep, ex, string.Format("cacheConfigXml- LoadConfigSettings- Ex:{0}", ex.Message));
            }
        }

        #endregion Private Methods
    }
}