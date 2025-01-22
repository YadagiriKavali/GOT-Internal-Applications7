//$Author: Anilkumar.ko $
//$Date: 4/03/18 6:55p $
//$Header: /Mobileapps/Operator/Davinci/TS/FailedTransUpdate/FailedTransUpdate/CommonHelper/DemoGeneral.cs 1     4/03/18 6:55p Anilkumar.ko $
//$Modtime: 8/27/15 10:29a $
//$Revision: 1 $
//Copyright @2012 IMImobile Pvt. Ltd.

using System;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Xml.Linq;
using System.Configuration;

namespace CommonHelper
{
    public class DemoGeneral
    {
        public static XmlDocument xDocDemo;
        public static CacheDependency objCacheXDocDemo;

        public static String GetDemoData(String strDept, String strNode)
        {
            string strMsg = string.Empty;
            try
            {
                LoadDemoData();
                XmlNode xNode = xDocDemo.DocumentElement.SelectSingleNode(strDept + "/" + strNode);
                strMsg = xNode.InnerText;
            }
            catch
            {
            }
            return strMsg;
        }

        public static void LoadDemoData()
        {
            String strPath = String.Empty;
            try
            {
                if (xDocDemo == null || objCacheXDocDemo == null || objCacheXDocDemo.HasChanged)
                {
                    if (ConfigurationManager.AppSettings["SERVICE_XML_PATH"] != null && ConfigurationManager.AppSettings["SERVICE_XML_PATH"].ToString().Length > 0)
                        strPath = ConfigurationManager.AppSettings["SERVICE_XML_PATH"].ToString() + "/Demo.xml";

                    if (!System.IO.File.Exists(strPath))
                        return;
                    xDocDemo = null;
                    objCacheXDocDemo = null;
                    xDocDemo = new XmlDocument();
                    xDocDemo.Load(strPath);
                    objCacheXDocDemo = new CacheDependency(strPath, DateTime.Now);
                }
            }
            catch { }
        }
    }
}
