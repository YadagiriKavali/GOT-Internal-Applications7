using System;
using System.Configuration;
using System.IO;
using System.Web.Caching;
using System.Xml;
using IMI.Logger;

namespace martconnect
{
    public class CacheConfig
    {
        #region Private Members

        private static CacheDependency cacheMConfig;
        private static XmlDocument XmlDoc;
        private static string MARTCONNECT_CONFIG_XML_PATH;

        #endregion Private Members
        
        #region Public Methods

        public static string GetServiceUrl(string service)
        {
            var serviceUrl = string.Empty;
            try
            {
                LoadConfigSettings();
                var urlInfo = XmlDoc.DocumentElement.SelectSingleNode("AUTHENTICATION/URL");
                if (urlInfo != null)
                    serviceUrl = urlInfo.InnerText;

                var serviceInfo = XmlDoc.DocumentElement.SelectSingleNode("SERVICES/" + service.ToUpper());
                if (!string.IsNullOrEmpty(serviceUrl))
                    serviceUrl += serviceInfo.InnerText;
            }
            catch (Exception ex)
            {
                LogData.Write("MARTCONNECT", "MARTCONNECT", LogMode.Excep, ex, string.Format("CacheConfig => LoadAuthDetails- Ex:{0}", ex.Message));
            }

            return serviceUrl;
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
                LogData.Write("MARTCONNECT", "MARTCONNECT", LogMode.Excep, ex, string.Format("CacheConfig => GetErrorMessage- Ex:{0}", ex.Message));
                return string.Empty;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static void LoadConfigSettings()
        {
            MARTCONNECT_CONFIG_XML_PATH = ConfigurationManager.AppSettings["MARTCONNECT_CONFIG_XML_PATH"] ?? "";
            try
            {
                if (!string.IsNullOrEmpty(MARTCONNECT_CONFIG_XML_PATH) && File.Exists(MARTCONNECT_CONFIG_XML_PATH))
                {
                    if (cacheMConfig == null || cacheMConfig.HasChanged || XmlDoc == null || XmlDoc.DocumentElement == null)
                    {
                        cacheMConfig = new CacheDependency(MARTCONNECT_CONFIG_XML_PATH, DateTime.Now);
                        XmlDoc = new XmlDocument();
                        var reader = XmlReader.Create(MARTCONNECT_CONFIG_XML_PATH);
                        XmlDoc.Load(reader);
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogData.Write("MARTCONNECT", "MARTCONNECT", LogMode.Excep, ex, string.Format("CacheConfig => LoadConfigSettings: Ex: {0}", ex.Message));
            }
        }

        #endregion Private Methods
    }
}
