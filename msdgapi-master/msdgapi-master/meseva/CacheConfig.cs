using System;
using System.Configuration;
using System.IO;
using System.Web.Caching;
using System.Xml;
using IMI.Logger;

namespace meseva
{
    public class CacheConfig
    {
        #region Private Members

        private static CacheDependency cacheMesevaConfig;
        private static XmlDocument XmlDoc;
        private static string MEESEVA_CONFIG_XML_PATH;

        #endregion Private Members

        #region Public Members

        public static string UserName;
        public static string Password;

        #endregion Public Members

        #region Public Methods

        public static void LoadAuthDetails()
        {
            try
            {
                LoadConfigSettings();
                var xmlMesevaInfo = XmlDoc.DocumentElement.SelectSingleNode("AUTHENTICATION");
                if (xmlMesevaInfo != null)
                {
                    UserName = xmlMesevaInfo.Attributes["UserName"] != null ? xmlMesevaInfo.Attributes["UserName"].Value : string.Empty;
                    Password = xmlMesevaInfo.Attributes["Password"] != null ? xmlMesevaInfo.Attributes["Password"].Value : string.Empty;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("CacheConfig => LoadAuthDetails- Ex:{0}", ex.Message));
            }
        }

        public static string GetServiceId(string serviceName)
        {
            try
            {
                LoadConfigSettings();
                var xmlMesevaInfo = XmlDoc.DocumentElement.SelectSingleNode("SERVICES/" + serviceName);
                return xmlMesevaInfo != null ? xmlMesevaInfo.InnerText : string.Empty;
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("CacheConfig => GetServiceId- Ex:{0}", ex.Message));
                return string.Empty;
            }
        }

        public static string GetErrorMessage(string errorCode)
        {
            try
            {
                LoadConfigSettings();
                var xmlEsevaInfo = XmlDoc.DocumentElement.SelectSingleNode("ERRORS/CODE_" + errorCode);
                return xmlEsevaInfo != null ? xmlEsevaInfo.InnerText.Trim() : string.Empty;
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("CacheConfig => GetErrorMessage- Ex:{0}", ex.Message));
                return string.Empty;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static void LoadConfigSettings()
        {
            MEESEVA_CONFIG_XML_PATH = ConfigurationManager.AppSettings["MEESEVA_CONFIG_XML_PATH"] ?? "";
            try
            {
                if (!string.IsNullOrEmpty(MEESEVA_CONFIG_XML_PATH) && File.Exists(MEESEVA_CONFIG_XML_PATH))
                {
                    if (cacheMesevaConfig == null || cacheMesevaConfig.HasChanged || XmlDoc == null || XmlDoc.DocumentElement == null)
                    {
                        cacheMesevaConfig = new CacheDependency(MEESEVA_CONFIG_XML_PATH, DateTime.Now);
                        XmlDoc = new XmlDocument();
                        var reader = XmlReader.Create(MEESEVA_CONFIG_XML_PATH);
                        XmlDoc.Load(reader);
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("CacheConfig => LoadConfigSettings: Ex: {0}", ex.Message));
            }
        }

        #endregion Private Methods
    }
}
