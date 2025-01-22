using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Web.Caching;

/// <summary>
/// Summary description for LoadConfig
/// </summary>
public class LoadConfig
{
    public static XmlDocument xDocConfig;
    public static CacheDependency objCacheConfig;

    public static void LoadConfigFile()
    {
        String strPath = String.Empty;
        try
        {
            if (xDocConfig == null || objCacheConfig == null || objCacheConfig.HasChanged)
            {
                strPath = General.GetConfigVal("SERVICE_XML_PATH") + "config.xml";
                if (!System.IO.File.Exists(strPath))
                    return;

                xDocConfig = null;
                objCacheConfig = null;
                xDocConfig = new XmlDocument();
                try
                {
                    xDocConfig.Load(strPath);
                    objCacheConfig = new CacheDependency(strPath, DateTime.Now);
                }
                catch (Exception ex)
                {
                    General.WriteLog("Exception in Loading xml() - Error" + ex.Message + ",Source:" + ex.Source + ",StackTrace:" + ex.StackTrace + ",InnerException:" + ex.InnerException);
                }
            }
        }
        catch (Exception ex)
        {
            General.WriteLog("Exception LoadConfigData() - Error" + ex.Message + ",Source:" + ex.Source + ",StackTrace:" + ex.StackTrace + ",InnerException:" + ex.InnerException);
        }
    }
}