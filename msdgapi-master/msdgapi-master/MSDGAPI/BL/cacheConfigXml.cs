using System;
using System.Configuration;
using System.Web.Caching;
using System.Xml;
using IMI.Logger;
using MSDGAPI.Models;

namespace MSDGAPI.Eseva.BL
{
    public class CacheConfigXml
    {
        #region Private Members

        private static CacheDependency assemplyCD;
        private static XmlDocument xmlDocAssemblyConfig;
        private static string ASSEMBLY_CONFIG_XML_PATH;

        #endregion Private Members

        #region Public Methods

        public static AssemblyInfo GetAssembDetails(string service, string action)
        {
            AssemblyInfo assemplyInfo = null;
            try
            {
                LoadAssemblyConfigSettings();
                var xmlAssemblyInfo = xmlDocAssemblyConfig.DocumentElement.SelectSingleNode(service.ToUpper() + "/ASSEMBLYINFO");
                if (xmlAssemblyInfo != null)
                {
                    assemplyInfo = new AssemblyInfo
                    {
                        AssemblyPath = xmlAssemblyInfo.Attributes["AssemblyPath"] != null ? xmlAssemblyInfo.Attributes["AssemblyPath"].Value : string.Empty,
                        ClassName = xmlAssemblyInfo.Attributes["ClassName"] != null ? xmlAssemblyInfo.Attributes["ClassName"].Value : string.Empty
                    };

                    var methodNode = xmlAssemblyInfo.SelectSingleNode("BEHAVIOUR/" + action.ToUpper());
                    if (methodNode != null)
                        assemplyInfo.MethodName = methodNode.InnerText;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "MSDGAPI", LogMode.Excep, ex, string.Format("cacheConfigXml- getAuthDetails- Ex:{0}", ex.Message));
            }
            return assemplyInfo;
        }

        #endregion Public Methods

        #region Private Methods

        private static void LoadAssemblyConfigSettings()
        {
            try
            {
                ASSEMBLY_CONFIG_XML_PATH = ConfigurationManager.AppSettings["ASSEMBLY_CONFIG_XML_PATH"];
                if (!string.IsNullOrEmpty(ASSEMBLY_CONFIG_XML_PATH))
                {
                    if (assemplyCD == null || assemplyCD.HasChanged || xmlDocAssemblyConfig == null || xmlDocAssemblyConfig.DocumentElement == null)
                    {
                        assemplyCD = new CacheDependency(ASSEMBLY_CONFIG_XML_PATH, DateTime.Now);
                        xmlDocAssemblyConfig = new XmlDocument();
                        var reader = XmlReader.Create(ASSEMBLY_CONFIG_XML_PATH);
                        xmlDocAssemblyConfig.Load(reader);
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "MSDGAPI", LogMode.Excep, ex, string.Format("cacheConfigXml- LoadConfigSettings- Ex:{0}", ex.Message));
            }
        }

        #endregion Private Methods
    }
}