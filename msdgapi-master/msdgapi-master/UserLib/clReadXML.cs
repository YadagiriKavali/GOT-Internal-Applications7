using System;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Xml.Linq;
using IMI.Logger;
using Newtonsoft.Json;

namespace User
{
    public class clReadXML
    {

        private static XDocument xdoc = null;
        static CacheDependency cacheDep = null;
        XElement xDocE = null;
        Error error = null;
        string guid = string.Empty;
        public clReadXML ()
        {
            // Loading XML
            Fillxml();
        }
        private static bool Fillxml ()
        {
            try
            {
                string strXMLPath = clGeneral.GetConfigValue("XML_FOLDER");//Complete XML folder path along with .xml file name.
                if (strXMLPath == "")
                    return false;
                if (strXMLPath != string.Empty && (cacheDep == null || cacheDep.HasChanged || xdoc == null))
                {
                    cacheDep = null;
                    cacheDep = new CacheDependency(strXMLPath, DateTime.Now);
                    xdoc = XDocument.Load(strXMLPath);
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "ReadXML", LogMode.Excep, ex, "clReadXML - Fillxml");
            }
            return true;
        }

        /// <summary>
        /// TO Get the service details
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="type"></param>
        /// <param name="validation"></param>
        /// <returns></returns>
        public string getDetails (string serviceId, ref  string validation357, ref string validatesms)
        {
            string action = string.Empty;
            try
            {
                XElement ele = xdoc.Root.Elements("MISSEDCALL").Elements("SERVICE").First(x => (string)x.Attribute("SID") == serviceId);
                if (ele != null)
                {
                    action = ele.Attribute("TYPE") != null ? ele.Attribute("TYPE").Value : "";
                    validation357 = ele.Attribute("VALIDATE357") != null ? ele.Attribute("VALIDATE357").Value : "F";
                    validatesms = ele.Attribute("VALIDATESMS") != null ? ele.Attribute("VALIDATESMS").Value : "F";
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "ReadXML", LogMode.Excep, ex, "getSmsMsg");
            }
            finally
            {

            }
            return action;
        }

        /// <summary>
        /// To get the sms string
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public string getSMSString (string serviceId, string node)
        {
            string message = string.Empty;
            try
            {
                XElement ele = xdoc.Root.Elements("MISSEDCALL").Elements("SERVICE").First(x => (string)x.Attribute("SID") == serviceId);
                if (ele != null)
                {
                    if (ele.Descendants("SMSSETTINGS").Descendants("MESSAGES").Descendants(node).Any())
                        message = ele.Descendants("SMSSETTINGS").Descendants("MESSAGES").Descendants(node).FirstOrDefault().Value;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "ReadXML", LogMode.Excep, ex, "getSmsMsg");
            }
            finally
            {

            }
            return message;
        }

        /// <summary>
        /// To get the URL of type U
        /// </summary>
        /// <param name="serviceid"></param>
        /// <returns></returns>
        public string getURL (string serviceId)
        {
            string url = string.Empty;
            try
            {
                XElement ele = xdoc.Root.Elements("MISSEDCALL").Elements("SERVICE").First(x => (string)x.Attribute("SID") == serviceId);
                if (ele != null)
                {
                    if (ele.Descendants("URL").Any())
                        url = ele.Descendants("URL").FirstOrDefault().Value;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "ReadXML", LogMode.Excep, ex, "getURL");
            }
            finally
            {

            }
            return url;
        }

        /// <summary>
        /// To get SMS Settings
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="url"></param>
        /// <param name="provideIid"></param>
        /// <param name="appName"></param>
        /// <param name="typeId"></param>
        public void getsmsSettings (string serviceId, ref string url, ref string provideIid, ref string appName, ref string typeId)
        {
            try
            {
                XElement ele = xdoc.Root.Elements("MISSEDCALL").Elements("SERVICE").First(x => (string)x.Attribute("SID") == serviceId);
                if (ele != null)
                {
                    if (ele.Descendants("SMSSETTINGS").Descendants("SMSURL").Any())
                        url = ele.Descendants("SMSSETTINGS").Descendants("SMSURL").FirstOrDefault().Value;
                    if (ele.Descendants("SMSSETTINGS").Descendants("PROVIDERID").Any())
                        provideIid = ele.Descendants("SMSSETTINGS").Descendants("PROVIDERID").FirstOrDefault().Value;
                    if (ele.Descendants("SMSSETTINGS").Descendants("APPNAME").Any())
                        appName = ele.Descendants("SMSSETTINGS").Descendants("APPNAME").FirstOrDefault().Value;
                    if (ele.Descendants("SMSSETTINGS").Descendants("TYPEID").Any())
                        typeId = ele.Descendants("SMSSETTINGS").Descendants("TYPEID").FirstOrDefault().Value;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("SlumLib", "BReadXML", LogMode.Excep, ex, "getmaislSmtpSettings");
            }
            finally
            {
            }
        }

        /// <summary>
        /// To get SMS Settings
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="url"></param>
        /// <param name="provideIid"></param>
        /// <param name="appName"></param>
        /// <param name="typeId"></param>
        public void getobdSettings (string serviceId, ref string url, ref string provideIid, ref string callflowid, ref string menufile, ref string calltype)
        {
            try
            {
                XElement ele = xdoc.Root.Elements("MISSEDCALL").Elements("SERVICE").First(x => (string)x.Attribute("SID") == serviceId);
                if (ele != null)
                {
                    if (ele.Descendants("OBDSETTINGS").Descendants("OBDURL").Any())
                        url = ele.Descendants("OBDSETTINGS").Descendants("OBDURL").FirstOrDefault().Value;
                    if (ele.Descendants("OBDSETTINGS").Descendants("PROVIDERID").Any())
                        provideIid = ele.Descendants("OBDSETTINGS").Descendants("PROVIDERID").FirstOrDefault().Value;
                    if (ele.Descendants("OBDSETTINGS").Descendants("CALLFLOWID").Any())
                        callflowid = ele.Descendants("OBDSETTINGS").Descendants("CALLFLOWID").FirstOrDefault().Value;
                    if (ele.Descendants("OBDSETTINGS").Descendants("MENUFILE").Any())
                        menufile = ele.Descendants("OBDSETTINGS").Descendants("MENUFILE").FirstOrDefault().Value;
                    if (ele.Descendants("OBDSETTINGS").Descendants("CALLTYPE").Any())
                        calltype = ele.Descendants("OBDSETTINGS").Descendants("CALLTYPE").FirstOrDefault().Value;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("SlumLib", "BReadXML", LogMode.Excep, ex, "getmaislSmtpSettings");
            }
            finally
            {
            }
        }

        public string getallsms ()
        {
            string response = string.Empty;
            try
            {
                var PageResxData = (from ele in xdoc.Descendants("SERVICE").Where(i => (string)i.Attribute("TYPE").Value == "S")
                                    select new SMS
                                    {
                                        SID = "<a id=\"aEditAgent\" style=\"cursor:pointer;\" href=\"#myDetails\" onclick=\"$.fn.editAgent('" + ele.Attribute("SID").Value + "')\">" + ele.Attribute("SID").Value + "</a>",
                                        Type = ele.Attribute("TYPE") != null ? ele.Attribute("TYPE").Value : "",
                                        Validate357 = ele.Attribute("VALIDATE357") != null ? (ele.Attribute("VALIDATE357").Value == "T" ? "True" : "False") : "",
                                        ValidateSMS = ele.Attribute("VALIDATESMS") != null ? (ele.Attribute("VALIDATESMS").Value == "T" ? "True" : "False") : "",
                                        Description = ele.Attribute("DESCRIPTION") != null ? ele.Attribute("DESCRIPTION").Value : "",
                                        DisplayCheckDuration = ele.Attribute("CHECKDURATION") != null ? (ele.Attribute("CHECKDURATION").Value == "0" ? "No" : ("YES (" + ele.Attribute("CHECKDURATION").Value + ")")) : "",
                                        SMSSetting = new SMSSetting
                                        {
                                            SMSUrl = ele.Descendants("SMSURL").Any() == true ? ele.Descendants("SMSURL").First().Value : "",
                                            ProviderId = ele.Descendants("PROVIDERID").Any() == true ? ele.Descendants("PROVIDERID").First().Value : "",
                                            AppName = ele.Descendants("APPNAME").Any() == true ? ele.Descendants("APPNAME").First().Value : "",
                                            TypeId = ele.Descendants("TYPEID").Any() == true ? ele.Descendants("TYPEID").First().Value : "",
                                            Message = new Message
                                            {
                                                SMSMessage = ele.Descendants("SMS").Any() == true ? ele.Descendants("SMS").First().Value : "",
                                                RepeatSMS = ele.Descendants("REPEAT_SMS").Any() == true ? ele.Descendants("REPEAT_SMS").First().Value : "",
                                                DayLimitExceed = ele.Descendants("DAYLIMIT_EXCEEDS").Any() == true ? ele.Descendants("DAYLIMIT_EXCEEDS").First().Value : "",
                                                WeekLimitExceed = ele.Descendants("WEEKLIMIT_EXCEEDS").Any() == true ? ele.Descendants("WEEKLIMIT_EXCEEDS").First().Value : "",
                                                TotalLimitExceed = ele.Descendants("TOTAL_EXCEEDS").Any() == true ? ele.Descendants("TOTAL_EXCEEDS").First().Value : ""
                                            }
                                        }
                                    }).ToList();



                return response = JsonConvert.SerializeObject(PageResxData);


            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "ReadXML", LogMode.Excep, ex, "getallsms");
            }
            finally
            {

            }
            return response;
        }

        public string getallsms (string id)
        {
            string response = string.Empty;
            try
            {
                var PageResxData = (from ele in xdoc.Descendants("SERVICE").Where(i => (string)i.Attribute("TYPE").Value == "S" && (string)i.Attribute("SID").Value == id)
                                    select new SMS
                                    {
                                        SID = ele.Attribute("SID").Value,
                                        Type = ele.Attribute("TYPE") != null ? ele.Attribute("TYPE").Value : "",
                                        Validate357 = ele.Attribute("VALIDATE357") != null ? (ele.Attribute("VALIDATE357").Value == "T" ? "True" : "False") : "T",
                                        ValidateSMS = ele.Attribute("VALIDATESMS") != null ? (ele.Attribute("VALIDATESMS").Value == "T" ? "True" : "False") : "T",
                                        Description = ele.Attribute("DESCRIPTION") != null ? ele.Attribute("DESCRIPTION").Value : "",
                                        CheckDuration = int.Parse(ele.Attribute("CHECKDURATION") != null ? ele.Attribute("CHECKDURATION").Value : "0"),
                                        SMSSetting = new SMSSetting
                                        {
                                            SMSUrl = ele.Descendants("SMSURL").Any() == true ? ele.Descendants("SMSURL").First().Value : "",
                                            ProviderId = ele.Descendants("PROVIDERID").Any() == true ? ele.Descendants("PROVIDERID").First().Value : "",
                                            AppName = ele.Descendants("APPNAME").Any() == true ? ele.Descendants("APPNAME").First().Value : "",
                                            TypeId = ele.Descendants("TYPEID").Any() == true ? ele.Descendants("TYPEID").First().Value : "",
                                            Message = new Message
                                            {
                                                SMSMessage = ele.Descendants("SMS").Any() == true ? ele.Descendants("SMS").First().Value : "",
                                                RepeatSMS = ele.Descendants("REPEAT_SMS").Any() == true ? ele.Descendants("REPEAT_SMS").First().Value : "",
                                                DayLimitExceed = ele.Descendants("DAYLIMIT_EXCEEDS").Any() == true ? ele.Descendants("DAYLIMIT_EXCEEDS").First().Value : "",
                                                WeekLimitExceed = ele.Descendants("WEEKLIMIT_EXCEEDS").Any() == true ? ele.Descendants("WEEKLIMIT_EXCEEDS").First().Value : "",
                                                TotalLimitExceed = ele.Descendants("TOTAL_EXCEEDS").Any() == true ? ele.Descendants("TOTAL_EXCEEDS").First().Value : ""
                                            }
                                        }
                                    }).ToList();



                return response = JsonConvert.SerializeObject(PageResxData);


            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "ReadXML", LogMode.Excep, ex, "getallsms(" + id + ")");
            }
            finally
            {

            }
            return response;
        }

        public string savesmssetting (SMS objsms)
        {
            string srep = string.Empty;
            try
            {

                string strXMLPath = clGeneral.GetConfigValue("XML_FOLDER");
                if (!xdoc.Root.Elements("MISSEDCALL").Elements("SERVICE").Any(x => (string)x.Attribute("SID") == objsms.SID))
                {
                    XElement missedcall = xdoc.Root.Element("MISSEDCALL");
                    //XElement xRoot = new XElement("SERVICE");
                    missedcall.Add(new XElement("SERVICE", new XAttribute("SID", objsms.SID.ToString() != null ? objsms.SID.ToString() : "")
                                                                           , new XAttribute("TYPE", objsms.Type.ToString())
                                                                           , new XAttribute("VALIDATE357", objsms.Validate357.ToString() != null ? objsms.Validate357.ToString() : "")
                                                                           , new XAttribute("VALIDATESMS", objsms.ValidateSMS.ToString() != null ? objsms.ValidateSMS.ToString() : "")
                                                                             , new XAttribute("DESCRIPTION", objsms.Description.ToString() != null ? objsms.Description.ToString() : "")
                                                                           , new XAttribute("CHECKDURATION", objsms.CheckDuration.ToString())
                                                                          , new XElement("SMSSETTINGS", new XElement("SMSURL", objsms.SMSSetting.SMSUrl)
                                                                          , new XElement("PROVIDERID", objsms.SMSSetting.ProviderId)
                                                                          , new XElement("APPNAME", objsms.SMSSetting.AppName)
                                                                           , new XElement("TYPEID", objsms.SMSSetting.TypeId)
                                                                            , new XElement("MESSAGES", new XElement("SMS", objsms.Message.SMSMessage)
                                                                          , new XElement("REPEAT_SMS", objsms.Message.RepeatSMS)
                                                                          , new XElement("DAYLIMIT_EXCEEDS", objsms.Message.DayLimitExceed)
                                                                           , new XElement("WEEKLIMIT_EXCEEDS", objsms.Message.WeekLimitExceed)
                                                                            , new XElement("TOTAL_EXCEEDS", objsms.Message.TotalLimitExceed)))
                                    ));
                    xdoc.Save(clGeneral.GetConfigValue("XML_FOLDER") + "SMS.xml");
                    guid = Guid.NewGuid().ToString().Replace("-", "");
                    error = new Error()
                    {
                        ErrorCode = "0",
                        ErrorMessge = "successfully  saved",
                        Id = guid
                    };
                    HttpContext.Current.Session["UID"] = guid;
                    return srep = JsonConvert.SerializeObject(error);
                }
                else
                {
                    // Edit Mode
                    XElement ele = xdoc.Root.Elements("MISSEDCALL").Elements("SERVICE").First(x => (string)x.Attribute("SID") == objsms.SID);
                    if (ele.Attribute("TYPE") != null)
                        ele.Attribute("TYPE").Value = objsms.Type;
                    else
                        ele.Add(new XAttribute("TYPE", objsms.Type.ToString()));

                    if (ele.Attribute("VALIDATE357") != null)
                        ele.Attribute("VALIDATE357").Value = objsms.Validate357;
                    else
                        ele.Add(new XAttribute("VALIDATE357", objsms.Validate357.ToString()));

                    if (ele.Attribute("VALIDATESMS") != null)
                        ele.Attribute("VALIDATESMS").Value = objsms.ValidateSMS;
                    else
                        ele.Add(new XAttribute("VALIDATESMS", objsms.ValidateSMS.ToString()));

                    if (ele.Attribute("DESCRIPTION") != null)
                        ele.Attribute("DESCRIPTION").Value = objsms.Description;
                    else
                        ele.Add(new XAttribute("DESCRIPTION", objsms.Description.ToString()));

                    if (ele.Attribute("CHECKDURATION") != null)
                        ele.Attribute("CHECKDURATION").Value = objsms.CheckDuration.ToString();
                    else
                        ele.Add(new XAttribute("CHECKDURATION", objsms.Description.ToString()));

                    if (ele.Descendants("SMSURL").Any())
                        ele.Descendants("SMSURL").FirstOrDefault().Value = objsms.SMSSetting.SMSUrl;
                    else
                    {
                        XElement nele = ele.Elements("SMSSETTINGS").Elements("SMSSETTINGS").FirstOrDefault();
                        nele.Add(new XElement("SMSURL", objsms.SMSSetting.SMSUrl));
                    }

                    if (ele.Descendants("PROVIDERID").Any())
                        ele.Descendants("PROVIDERID").FirstOrDefault().Value = objsms.SMSSetting.ProviderId;
                    else
                    {
                        XElement nele = ele.Elements("SMSSETTINGS").Elements("SMSSETTINGS").FirstOrDefault();
                        nele.Add(new XElement("PROVIDERID", objsms.SMSSetting.ProviderId));
                    }

                    if (ele.Descendants("APPNAME").Any())
                        ele.Descendants("APPNAME").FirstOrDefault().Value = objsms.SMSSetting.AppName;
                    else
                    {
                        XElement nele = ele.Elements("SMSSETTINGS").Elements("SMSSETTINGS").FirstOrDefault();
                        nele.Add(new XElement("APPNAME", objsms.SMSSetting.AppName));
                    }

                    if (ele.Descendants("TYPEID").Any())
                        ele.Descendants("TYPEID").FirstOrDefault().Value = objsms.SMSSetting.TypeId;
                    else
                    {
                        XElement nele = ele.Elements("SMSSETTINGS").Elements("SMSSETTINGS").FirstOrDefault();
                        nele.Add(new XElement("TYPEID", objsms.SMSSetting.TypeId));
                    }

                    if (ele.Descendants("SMS").Any())
                        ele.Descendants("SMS").FirstOrDefault().Value = objsms.Message.SMSMessage;
                    else
                    {
                        XElement nele = ele.Elements("SMSSETTINGS").Elements("MESSAGES").FirstOrDefault();
                        nele.Add(new XElement("SMS", objsms.Message.SMSMessage));
                    }

                    if (ele.Descendants("REPEAT_SMS").Any())
                        ele.Descendants("REPEAT_SMS").FirstOrDefault().Value = objsms.Message.RepeatSMS;
                    else
                    {
                        XElement nele = ele.Elements("SMSSETTINGS").Elements("MESSAGES").FirstOrDefault();
                        nele.Add(new XElement("REPEAT_SMS", objsms.Message.RepeatSMS));
                    }

                    if (ele.Descendants("DAYLIMIT_EXCEEDS").Any())
                        ele.Descendants("DAYLIMIT_EXCEEDS").FirstOrDefault().Value = objsms.Message.DayLimitExceed;
                    else
                    {
                        XElement nele = ele.Elements("SMSSETTINGS").Elements("MESSAGES").FirstOrDefault();
                        nele.Add(new XElement("DAYLIMIT_EXCEEDS", objsms.Message.DayLimitExceed));
                    }

                    if (ele.Descendants("WEEKLIMIT_EXCEEDS").Any())
                        ele.Descendants("WEEKLIMIT_EXCEEDS").FirstOrDefault().Value = objsms.Message.WeekLimitExceed;
                    else
                    {
                        XElement nele = ele.Elements("SMSSETTINGS").Elements("MESSAGES").FirstOrDefault();
                        nele.Add(new XElement("WEEKLIMIT_EXCEEDS", objsms.Message.WeekLimitExceed));
                    }

                    if (ele.Descendants("TOTAL_EXCEEDS").Any())
                        ele.Descendants("TOTAL_EXCEEDS").FirstOrDefault().Value = objsms.Message.TotalLimitExceed;
                    else
                    {
                        XElement nele = ele.Elements("SMSSETTINGS").Elements("MESSAGES").FirstOrDefault();
                        nele.Add(new XElement("TOTAL_EXCEEDS", objsms.Message.TotalLimitExceed));
                    }
                    xdoc.Save(clGeneral.GetConfigValue("XML_FOLDER") + "SMS.xml");    
                    guid = Guid.NewGuid().ToString().Replace("-", "");
                    error = new Error()
                    {
                        ErrorCode = "0",
                        ErrorMessge = "successfully saved",
                        Id = guid
                    };
                    HttpContext.Current.Session["UID"] = guid;
                    return srep = JsonConvert.SerializeObject(error);
                }
               
                  
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "ReadXML", LogMode.Excep, ex, "savesmssetting");
            }
            finally
            {
            }
            guid = Guid.NewGuid().ToString().Replace("-", "");
            error = new Error()
            {
                ErrorCode = "2",
                ErrorMessge = "Failed",
                Id = guid
            };
            HttpContext.Current.Session["UID"] = guid;
            return srep = JsonConvert.SerializeObject(error);   

        }
        public string ckecksms (string id)
        {
            string response = string.Empty;
            try
            {
                if (!xdoc.Root.Elements("MISSEDCALL").Elements("SERVICE").Any(x => (string)x.Attribute("SID") == id))
                {
                    return response = "0";
                }
                return response = "1";

            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "ReadXML", LogMode.Excep, ex, "check sid");
            }
            finally
            {

            }
            return response;
        }
        public string getobd ()
        {
            string response = string.Empty;
            try
            {
                var PageResxData = (from ele in xdoc.Descendants("SERVICE").Where(i => (string)i.Attribute("TYPE").Value == "O")
                                    select new SMS
                                    {                                       
                                        SID = "<a id=\"aEditAgent\" style=\"cursor:pointer;\" href=\"#myDetails\" onclick=\"$.fn.editobd('" + ele.Attribute("SID").Value + "')\">" + ele.Attribute("SID").Value + "</a>",
                                        Type = ele.Attribute("TYPE") != null ? ele.Attribute("TYPE").Value : "",
                                        Validate357 = ele.Attribute("VALIDATE357") != null ? (ele.Attribute("VALIDATE357").Value == "T" ? "True" : "False") : "",
                                        ValidateSMS = ele.Attribute("VALIDATESMS") != null ? (ele.Attribute("VALIDATESMS").Value == "T" ? "True" : "False") : "",
                                        Description = ele.Attribute("DESCRIPTION") != null ? ele.Attribute("DESCRIPTION").Value : "",
                                        DisplayCheckDuration = ele.Attribute("CHECKDURATION") != null ? (ele.Attribute("CHECKDURATION").Value == "0" ? "No" : ("YES (" + ele.Attribute("CHECKDURATION").Value + ")")) : "",
                                        OBDSetting = new OBD
                                        {
                                            OBDUrl = ele.Descendants("OBDURL").Any() == true ? ele.Descendants("OBDURL").First().Value : "",
                                            ProviderId = ele.Descendants("PROVIDERID").Any() == true ? ele.Descendants("PROVIDERID").First().Value : "",
                                            CallFlowId = ele.Descendants("callflowid").Any() == true ? ele.Descendants("callflowid").First().Value : "",
                                            MenuFile = ele.Descendants("menufile").Any() == true ? ele.Descendants("menufile").First().Value : "",
                                            CallType = ele.Descendants("CALLTYPE").Any() == true ? ele.Descendants("CALLTYPE").First().Value : ""                                      
                                        }
                                    }).ToList();



                return response = JsonConvert.SerializeObject(PageResxData);



            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "ReadXML", LogMode.Excep, ex, "getobd");
            }
            finally
            {

            }
            return response;
        }
        public string getobd (string Id)
        {
            string response = string.Empty;
            try
            {
                var PageResxData = (from ele in xdoc.Descendants("SERVICE").Where(i => (string)i.Attribute("TYPE").Value == "O" && (string)i.Attribute("SID").Value == Id)
                                    select new SMS
                                    {
                                        SID = ele.Attribute("SID").Value,
                                        Type = ele.Attribute("TYPE") != null ? ele.Attribute("TYPE").Value : "",
                                        Validate357 = ele.Attribute("VALIDATE357") != null ? (ele.Attribute("VALIDATE357").Value == "T" ? "True" : "False") : "T",
                                        ValidateSMS = ele.Attribute("VALIDATESMS") != null ? (ele.Attribute("VALIDATESMS").Value == "T" ? "True" : "False") : "T",
                                        Description = ele.Attribute("DESCRIPTION") != null ? ele.Attribute("DESCRIPTION").Value : "",
                                        CheckDuration = int.Parse(ele.Attribute("CHECKDURATION") != null ? ele.Attribute("CHECKDURATION").Value : "0"), 
                                        OBDSetting = new OBD
                                        {
                                            OBDUrl = ele.Descendants("OBDURL").Any() == true ? ele.Descendants("OBDURL").First().Value : "",
                                            ProviderId = ele.Descendants("PROVIDERID").Any() == true ? ele.Descendants("PROVIDERID").First().Value : "",
                                            CallFlowId = ele.Descendants("CALLFLOWID").Any() == true ? ele.Descendants("CALLFLOWID").First().Value : "",
                                            MenuFile = ele.Descendants("MENUFILE").Any() == true ? ele.Descendants("MENUFILE").First().Value : "",
                                            CallType = ele.Descendants("CALLTYPE").Any() == true ? ele.Descendants("CALLTYPE").First().Value : ""       
                                        }
                                    }).ToList();



                return response = JsonConvert.SerializeObject(PageResxData);



            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "ReadXML", LogMode.Excep, ex, "getobd(" + Id + ")");
            }
            finally
            {

            }
            return response;
        }
        public string saveOBDsetting (SMS objsms)
        {
            string srep = string.Empty;
            try
            {
                string strXMLPath = clGeneral.GetConfigValue("XML_FOLDER");
                if (!xdoc.Root.Elements("MISSEDCALL").Elements("SERVICE").Any(x => (string)x.Attribute("SID") == objsms.SID))
                {
                    XElement missedcall = xdoc.Root.Element("MISSEDCALL");
                    //XElement xRoot = new XElement("SERVICE");
                    missedcall.Add(new XElement("SERVICE", new XAttribute("SID", objsms.SID.ToString() != null ? objsms.SID.ToString() : "")
                                                                           , new XAttribute("TYPE", objsms.Type.ToString())
                                                                           , new XAttribute("VALIDATE357", objsms.Validate357.ToString() != null ? objsms.Validate357.ToString() : "")
                                                                           , new XAttribute("VALIDATESMS", objsms.ValidateSMS.ToString() != null ? objsms.ValidateSMS.ToString() : "")
                                                                             , new XAttribute("DESCRIPTION", objsms.Description.ToString() != null ? objsms.Description.ToString() : "")
                                                                           , new XAttribute("CHECKDURATION", objsms.CheckDuration.ToString())
                                                                          , new XElement("OBDSETTINGS", new XElement("OBDURL", objsms.OBDSetting.OBDUrl)
                                                                          , new XElement("PROVIDERID", objsms.OBDSetting.ProviderId)
                                                                          , new XElement("CALLFLOWID", objsms.OBDSetting.CallFlowId)
                                                                           , new XElement("MENUFILE", objsms.OBDSetting.MenuFile)
                                                                             , new XElement("CALLTYPE", objsms.OBDSetting.CallType))
                                    ));
                    //   missedcall.Add(xRoot);


                    xdoc.Save(clGeneral.GetConfigValue("XML_FOLDER") + "SMS.xml");
                    guid = Guid.NewGuid().ToString().Replace("-", "");
                    error = new Error()
                    {
                        ErrorCode = "0",
                        ErrorMessge = "successfully saved",
                        Id = guid
                    };
                    HttpContext.Current.Session["UID"] = guid;
                    return srep = JsonConvert.SerializeObject(error);
                }
                else
                {
                    // Edit Mode
                    XElement ele = xdoc.Root.Elements("MISSEDCALL").Elements("SERVICE").First(x => (string)x.Attribute("SID") == objsms.SID);
                    if (ele.Attribute("TYPE") != null)
                        ele.Attribute("TYPE").Value = objsms.Type;
                    else
                        ele.Add(new XAttribute("TYPE", objsms.Type.ToString()));
                    if (ele.Attribute("VALIDATE357") != null)
                        ele.Attribute("VALIDATE357").Value = objsms.Validate357;
                    else
                        ele.Add(new XAttribute("VALIDATE357", objsms.Validate357.ToString()));

                    if (ele.Attribute("VALIDATESMS") != null)
                        ele.Attribute("VALIDATESMS").Value = objsms.ValidateSMS;
                    else
                        ele.Add(new XAttribute("VALIDATESMS", objsms.ValidateSMS.ToString()));

                    if (ele.Attribute("DESCRIPTION") != null)
                        ele.Attribute("DESCRIPTION").Value = objsms.Description;
                    else
                        ele.Add(new XAttribute("DESCRIPTION", objsms.Description.ToString()));

                    if (ele.Attribute("CHECKDURATION") != null)
                        ele.Attribute("CHECKDURATION").Value = objsms.CheckDuration.ToString();
                    else
                        ele.Add(new XAttribute("CHECKDURATION", objsms.Description.ToString()));

                    if (ele.Descendants("OBDURL").Any())
                        ele.Descendants("OBDURL").FirstOrDefault().Value = objsms.OBDSetting.OBDUrl;
                    else
                    {
                        XElement nele = ele.Elements("OBDSETTINGS").FirstOrDefault();
                        nele.Add(new XElement("OBDURL", objsms.OBDSetting.OBDUrl));
                    }
                    if (ele.Descendants("PROVIDERID").Any())
                        ele.Descendants("PROVIDERID").FirstOrDefault().Value = objsms.OBDSetting.ProviderId;
                    else
                    {
                        XElement nele = ele.Elements("OBDSETTINGS").FirstOrDefault();
                        nele.Add(new XElement("PROVIDERID", objsms.OBDSetting.ProviderId));
                    }
                    if (ele.Descendants("CALLFLOWID").Any())
                        ele.Descendants("CALLFLOWID").FirstOrDefault().Value = objsms.OBDSetting.CallFlowId;
                    else
                    {
                        XElement nele = ele.Elements("OBDSETTINGS").FirstOrDefault();
                        nele.Add(new XElement("CALLFLOWID", objsms.OBDSetting.CallFlowId));
                    }
                    if (ele.Descendants("MENUFILE").Any())
                        ele.Descendants("MENUFILE").FirstOrDefault().Value = objsms.OBDSetting.MenuFile;
                    else
                    {
                        XElement nele = ele.Elements("OBDSETTINGS").FirstOrDefault();
                        nele.Add(new XElement("MENUFILE", objsms.OBDSetting.MenuFile));
                    }
                    if (ele.Descendants("CALLTYPE").Any())
                        ele.Descendants("CALLTYPE").FirstOrDefault().Value = objsms.OBDSetting.CallType;
                    else
                    {
                        XElement nele = ele.Elements("OBDSETTINGS").FirstOrDefault();
                        nele.Add(new XElement("CALLTYPE", objsms.OBDSetting.CallType));
                    }
                    xdoc.Save(clGeneral.GetConfigValue("XML_FOLDER") + "SMS.xml");
                    guid = Guid.NewGuid().ToString().Replace("-", "");
                    error = new Error()
                    {
                        ErrorCode = "0",
                        ErrorMessge = "successfully saved",
                        Id = guid
                    };
                    HttpContext.Current.Session["UID"] = guid;
                    return srep = JsonConvert.SerializeObject(error);
                }
            }
            catch (Exception ex)
            {

            }
            guid = Guid.NewGuid().ToString().Replace("-", "");
            error = new Error()
            {
                ErrorCode = "2",
                ErrorMessge = "Failed",
                Id = guid
            };
            HttpContext.Current.Session["UID"] = guid;
            return srep = JsonConvert.SerializeObject(error);   

        }
        public string geturl ()
        {
            string response = string.Empty;
            try
            {
                var PageResxData = (from ele in xdoc.Descendants("SERVICE").Where(i => (string)i.Attribute("TYPE").Value == "U")
                                    select new SMS
                                    {
                                        SID = "<a id=\"aEditAgent\" style=\"cursor:pointer;\" href=\"#myDetails\" onclick=\"$.fn.editurl('" + ele.Attribute("SID").Value + "')\">" + ele.Attribute("SID").Value + "</a>",
                                        Type = ele.Attribute("TYPE") != null ? ele.Attribute("TYPE").Value : "",
                                        Validate357 = ele.Attribute("VALIDATE357") != null ? (ele.Attribute("VALIDATE357").Value == "T" ? "True" : "False") : "",
                                        ValidateSMS = ele.Attribute("VALIDATESMS") != null ? (ele.Attribute("VALIDATESMS").Value == "T" ? "True" : "False") : "",
                                        Description = ele.Attribute("DESCRIPTION") != null ? ele.Attribute("DESCRIPTION").Value : "",
                                        DisplayCheckDuration = ele.Attribute("CHECKDURATION") != null ? (ele.Attribute("CHECKDURATION").Value == "0" ? "No" : ("YES (" + ele.Attribute("CHECKDURATION").Value + ")")) : "",
                                        UrlSetting = new URL
                                         {
                                             UrlSetting = ele.Descendants("URL").Any() == true ? ele.Descendants("URL").First().Value : ""
                                           
                                         }
                                    }).ToList();

                return response = JsonConvert.SerializeObject(PageResxData);
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "ReadXML", LogMode.Excep, ex, "geturl");
            }
            finally
            {

            }
            return response;
        }
        public string geturl (string Id)
        {
            string response = string.Empty;
            try
            {
                var PageResxData = (from ele in xdoc.Descendants("SERVICE").Where(i => (string)i.Attribute("TYPE").Value == "U" && (string)i.Attribute("SID").Value == Id)
                                    select new SMS
                                    {
                                        SID = ele.Attribute("SID").Value,
                                        Type = ele.Attribute("TYPE") != null ? ele.Attribute("TYPE").Value : "",
                                        Validate357 = ele.Attribute("VALIDATE357") != null ? (ele.Attribute("VALIDATE357").Value == "T" ? "True" : "False") : "T",
                                        ValidateSMS = ele.Attribute("VALIDATESMS") != null ? (ele.Attribute("VALIDATESMS").Value == "T" ? "True" : "False") : "T",
                                        Description = ele.Attribute("DESCRIPTION") != null ? ele.Attribute("DESCRIPTION").Value : "",
                                        CheckDuration = int.Parse(ele.Attribute("CHECKDURATION") != null ? ele.Attribute("CHECKDURATION").Value : "0"), 
                                        UrlSetting = new URL
                                          {
                                              UrlSetting = ele.Descendants("URL").Any() == true ? ele.Descendants("URL").First().Value : ""
                                          }
                                    }).ToList();



                return response = JsonConvert.SerializeObject(PageResxData);



            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "ReadXML", LogMode.Excep, ex, "geturl(" + Id + ")");
            }
            finally
            {

            }
            return response;
        }
        public string saveurlsetting (SMS objsms)
        {
            string srep = string.Empty;
            try
            {
                string strXMLPath = clGeneral.GetConfigValue("XML_FOLDER");
                if (!xdoc.Root.Elements("MISSEDCALL").Elements("SERVICE").Any(x => (string)x.Attribute("SID") == objsms.SID))
                {
                    XElement missedcall = xdoc.Root.Element("MISSEDCALL");
                    //XElement xRoot = new XElement("SERVICE");
                    missedcall.Add(new XElement("SERVICE", new XAttribute("SID", objsms.SID.ToString() != null ? objsms.SID.ToString() : "")
                                                                           , new XAttribute("TYPE", objsms.Type.ToString())
                                                                           , new XAttribute("VALIDATE357", objsms.Validate357.ToString() != null ? objsms.Validate357.ToString() : "")
                                                                           , new XAttribute("VALIDATESMS", objsms.ValidateSMS.ToString() != null ? objsms.ValidateSMS.ToString() : "")
                                                                             , new XAttribute("DESCRIPTION", objsms.Description.ToString() != null ? objsms.Description.ToString() : "")
                                                                           , new XAttribute("CHECKDURATION", objsms.CheckDuration.ToString())
                                                                          , new XElement("URL", objsms.UrlSetting.UrlSetting)
                                    ));
                    xdoc.Save(clGeneral.GetConfigValue("XML_FOLDER") + "SMS.xml");
                    guid = Guid.NewGuid().ToString().Replace("-", "");
                    error = new Error()
                    {
                        ErrorCode = "0",
                        ErrorMessge = "successfully saved",
                        Id = guid
                    };
                    HttpContext.Current.Session["UID"] = guid;
                    return srep = JsonConvert.SerializeObject(error);
                }
                else
                {
                    // Edit Mode
                    XElement ele = xdoc.Root.Elements("MISSEDCALL").Elements("SERVICE").First(x => (string)x.Attribute("SID") == objsms.SID);
                    if (ele.Attribute("TYPE") != null)
                        ele.Attribute("TYPE").Value = objsms.Type;
                    else
                        ele.Add(new XAttribute("TYPE", objsms.Type.ToString()));

                    if (ele.Attribute("VALIDATE357") != null)
                        ele.Attribute("VALIDATE357").Value = objsms.Validate357;
                    else
                        ele.Add(new XAttribute("VALIDATE357", objsms.Validate357.ToString()));

                    if (ele.Attribute("VALIDATESMS") != null)
                        ele.Attribute("VALIDATESMS").Value = objsms.ValidateSMS;
                    else
                        ele.Add(new XAttribute("VALIDATESMS", objsms.ValidateSMS.ToString()));

                    if (ele.Attribute("DESCRIPTION") != null)
                        ele.Attribute("DESCRIPTION").Value = objsms.Description;
                    else
                        ele.Add(new XAttribute("DESCRIPTION", objsms.Description.ToString()));

                    if (ele.Attribute("CHECKDURATION") != null)
                        ele.Attribute("CHECKDURATION").Value = objsms.CheckDuration.ToString();
                    else
                        ele.Add(new XAttribute("CHECKDURATION", objsms.Description.ToString()));
                    if (ele.Descendants("URL").Any())
                        ele.Descendants("URL").FirstOrDefault().Value = objsms.UrlSetting.UrlSetting;
                    else
                    {
                        XElement nele = ele.Elements("URL").FirstOrDefault();
                        nele.Add(new XElement("URL", objsms.UrlSetting.UrlSetting));
                    }
                    // ele.Descendants("URL").FirstOrDefault().Value = objsms.urlsetting.urlsetting;
                    xdoc.Save(clGeneral.GetConfigValue("XML_FOLDER") + "SMS.xml");
                    guid = Guid.NewGuid().ToString().Replace("-", "");
                    error = new Error()
                    {
                        ErrorCode = "0",
                        ErrorMessge = "successfully saved",
                        Id = guid
                    };
                    HttpContext.Current.Session["UID"] = guid;
                    return srep = JsonConvert.SerializeObject(error);
                }
            }
            catch (Exception ex)
            {

                
            }
            guid = Guid.NewGuid().ToString().Replace("-", "");
            error = new Error()
            {
                ErrorCode = "2",
                ErrorMessge = "Failed",
                Id = guid
            };
            HttpContext.Current.Session["UID"] = guid;
            return srep = JsonConvert.SerializeObject(error);   
        }
        public string getdual ()
        {
            string response = string.Empty;
            try
            {
                var PageResxData = (from ele in xdoc.Descendants("SERVICE").Where(i => (string)i.Attribute("TYPE").Value == "D")
                                    select new SMS
                                    {
                                        SID = "<a id=\"aEditAgent\" style=\"cursor:pointer;\" href=\"#myDetails\" onclick=\"$.fn.editDual('" + ele.Attribute("SID").Value + "')\">" + ele.Attribute("SID").Value + "</a>",
                                        Type = ele.Attribute("TYPE") != null ? ele.Attribute("TYPE").Value : "",
                                        Validate357 = ele.Attribute("VALIDATE357") != null ? (ele.Attribute("VALIDATE357").Value == "T" ? "true" : "false") : "",
                                        ValidateSMS = ele.Attribute("VALIDATESMS") != null ? (ele.Attribute("VALIDATESMS").Value == "T" ? "true" : "false") : "",
                                        Description = ele.Attribute("DESCRIPTION") != null ? ele.Attribute("DESCRIPTION").Value : "",
                                        DisplayCheckDuration = ele.Attribute("CHECKDURATION") != null ? (ele.Attribute("CHECKDURATION").Value == "0" ? "No" : ("YES (" + ele.Attribute("CHECKDURATION").Value + ")")) : "",
                                        SMSSetting = new SMSSetting
                                        {
                                            SMSUrl = ele.Descendants("SMSURL").Any() == true ? ele.Descendants("SMSURL").First().Value : "",
                                            ProviderId = ele.Descendants("PROVIDERID").Any() == true ? ele.Descendants("PROVIDERID").First().Value : "",
                                            AppName = ele.Descendants("APPNAME").Any() == true ? ele.Descendants("APPNAME").First().Value : "",
                                            TypeId = ele.Descendants("TYPEID").Any() == true ? ele.Descendants("TYPEID").First().Value : "",
                                            Message = new Message
                                            {
                                                SMSMessage = ele.Descendants("SMS").Any() == true ? ele.Descendants("SMS").First().Value : "",
                                                RepeatSMS = ele.Descendants("REPEAT_SMS").Any() == true ? ele.Descendants("REPEAT_SMS").First().Value : "",
                                                DayLimitExceed = ele.Descendants("DAYLIMIT_EXCEEDS").Any() == true ? ele.Descendants("DAYLIMIT_EXCEEDS").First().Value : "",
                                                WeekLimitExceed = ele.Descendants("WEEKLIMIT_EXCEEDS").Any() == true ? ele.Descendants("WEEKLIMIT_EXCEEDS").First().Value : "",
                                                TotalLimitExceed = ele.Descendants("TOTAL_EXCEEDS").Any() == true ? ele.Descendants("TOTAL_EXCEEDS").First().Value : ""
                                            }
                                        },
                                        OBDSetting = new OBD
                                        {
                                            OBDUrl = ele.Descendants("OBDURL").Any() == true ? ele.Descendants("OBDURL").First().Value : "",
                                            ProviderId = ele.Descendants("PROVIDERID").Any() == true ? ele.Descendants("PROVIDERID").First().Value : "",
                                            CallFlowId = ele.Descendants("callflowid").Any() == true ? ele.Descendants("callflowid").First().Value : "",
                                            MenuFile = ele.Descendants("menufile").Any() == true ? ele.Descendants("menufile").First().Value : "",
                                            CallType = ele.Descendants("CALLTYPE").Any() == true ? ele.Descendants("CALLTYPE").First().Value : ""      
                                        }

                                    }).ToList();



                return response = JsonConvert.SerializeObject(PageResxData);


            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "ReadXML", LogMode.Excep, ex, "getallsms");
            }
            finally
            {

            }
            return response;
        }

        public string getdual (string id)
        {
            string response = string.Empty;
            try
            {
                var PageResxData = (from ele in xdoc.Descendants("SERVICE").Where(i => (string)i.Attribute("TYPE").Value == "D" && (string)i.Attribute("SID").Value == id)
                                    select new SMS
                                    {
                                        SID = ele.Attribute("SID").Value != null ? ele.Attribute("SID").Value : "",
                                        Type = ele.Attribute("TYPE") != null ? ele.Attribute("TYPE").Value : "",
                                        Validate357 = ele.Attribute("VALIDATE357") != null ? (ele.Attribute("VALIDATE357").Value == "T" ? "True" : "False") : "",
                                        ValidateSMS = ele.Attribute("VALIDATESMS") != null ? (ele.Attribute("VALIDATESMS").Value == "T" ? "True" : "False") : "",
                                        Description = ele.Attribute("DESCRIPTION") != null ? ele.Attribute("DESCRIPTION").Value : "",
                                        DisplayCheckDuration = ele.Attribute("CHECKDURATION") != null ? (ele.Attribute("CHECKDURATION").Value == "0" ? "No" : ("YES (" + ele.Attribute("CHECKDURATION").Value + ")")) : "",
                                        SMSSetting = new SMSSetting
                                        {
                                            SMSUrl = ele.Descendants("SMSURL").Any() == true ? ele.Descendants("SMSURL").First().Value : "",
                                            ProviderId = ele.Descendants("PROVIDERID").Any() == true ? ele.Descendants("PROVIDERID").First().Value : "",
                                            AppName = ele.Descendants("APPNAME").Any() == true ? ele.Descendants("APPNAME").First().Value : "",
                                            TypeId = ele.Descendants("TYPEID").Any() == true ? ele.Descendants("TYPEID").First().Value : "",
                                            Message = new Message
                                            {
                                                SMSMessage = ele.Descendants("SMS").Any() == true ? ele.Descendants("SMS").First().Value : "",
                                                RepeatSMS = ele.Descendants("REPEAT_SMS").Any() == true ? ele.Descendants("REPEAT_SMS").First().Value : "",
                                                DayLimitExceed = ele.Descendants("DAYLIMIT_EXCEEDS").Any() == true ? ele.Descendants("DAYLIMIT_EXCEEDS").First().Value : "",
                                                WeekLimitExceed = ele.Descendants("WEEKLIMIT_EXCEEDS").Any() == true ? ele.Descendants("WEEKLIMIT_EXCEEDS").First().Value : "",
                                                TotalLimitExceed = ele.Descendants("TOTAL_EXCEEDS").Any() == true ? ele.Descendants("TOTAL_EXCEEDS").First().Value : ""
                                            }
                                        },
                                        OBDSetting = new OBD
                                        {
                                            OBDUrl = ele.Descendants("OBDURL").Any() == true ? ele.Descendants("OBDURL").First().Value : "",
                                            ProviderId = ele.Descendants("PROVIDERID").Any() == true ? ele.Descendants("PROVIDERID").First().Value : "",
                                            CallFlowId = ele.Descendants("CALLFLOWID").Any() == true ? ele.Descendants("CALLFLOWID").First().Value : "",
                                            MenuFile = ele.Descendants("MENUFILE").Any() == true ? ele.Descendants("MENUFILE").First().Value : "",
                                            CallType = ele.Descendants("CALLTYPE").Any() == true ? ele.Descendants("CALLTYPE").First().Value : ""      
                                        }
                                    }).ToList();



                return response = JsonConvert.SerializeObject(PageResxData);


            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "ReadXML", LogMode.Excep, ex, "getallsms(" + id + ")");
            }
            finally
            {

            }
            return response;
        }

        public string savedualsetting (SMS objsms)
        {
            string srep = string.Empty;
            try
            {                
                string strXMLPath = clGeneral.GetConfigValue("XML_FOLDER");
                if (!xdoc.Root.Elements("MISSEDCALL").Elements("SERVICE").Any(x => (string)x.Attribute("SID") == objsms.SID))
                {
                    XElement missedcall = xdoc.Root.Element("MISSEDCALL");
                    //XElement xRoot = new XElement("SERVICE");
                    missedcall.Add(new XElement("SERVICE", new XAttribute("SID", objsms.SID.ToString() != null ? objsms.SID.ToString() : "")
                                                                           , new XAttribute("TYPE", objsms.Type.ToString())
                                                                           , new XAttribute("VALIDATE357", objsms.Validate357.ToString() != null ? objsms.Validate357.ToString() : "")
                                                                           , new XAttribute("VALIDATESMS", objsms.ValidateSMS.ToString() != null ? objsms.ValidateSMS.ToString() : "")
                                                                             , new XAttribute("DESCRIPTION", objsms.Description.ToString() != null ? objsms.Description.ToString() : "")
                                                                           , new XAttribute("CHECKDURATION", objsms.CheckDuration.ToString())
                                                                          , new XElement("SMSSETTINGS", new XElement("SMSURL", objsms.SMSSetting.SMSUrl)
                                                                          , new XElement("PROVIDERID", objsms.SMSSetting.ProviderId)
                                                                          , new XElement("APPNAME", objsms.SMSSetting.AppName)
                                                                           , new XElement("TYPEID", objsms.SMSSetting.TypeId)
                                                                            , new XElement("MESSAGES", new XElement("SMS", objsms.Message.SMSMessage)
                                                                          , new XElement("REPEAT_SMS", objsms.Message.RepeatSMS)
                                                                          , new XElement("DAYLIMIT_EXCEEDS", objsms.Message.DayLimitExceed)
                                                                           , new XElement("WEEKLIMIT_EXCEEDS", objsms.Message.WeekLimitExceed)
                                                                            , new XElement("TOTAL_EXCEEDS", objsms.Message.TotalLimitExceed)))
                                                                             , new XElement("OBDSETTINGS", new XElement("OBDURL", objsms.OBDSetting.OBDUrl)
                                                                              , new XElement("PROVIDERID", objsms.OBDSetting.ProviderId)
                                                                          , new XElement("CALLFLOWID", objsms.OBDSetting.CallFlowId)
                                                                           , new XElement("MENUFILE", objsms.OBDSetting.MenuFile)
                                                                            , new XElement("CALLTYPE", objsms.OBDSetting.CallType))
                                    ));
                    //   missedcall.Add(xRoot);


                    xdoc.Save(clGeneral.GetConfigValue("XML_FOLDER") + "SMS.xml");                
                     guid = Guid.NewGuid().ToString().Replace("-", "");
                    error = new Error()
                    {
                        ErrorCode = "0",
                        ErrorMessge = "successfully saved",
                        Id = guid
                    };
                    HttpContext.Current.Session["UID"] = guid;
                   return srep = JsonConvert.SerializeObject(error);
                }
                else
                {
                    // Edit Mode
                    XElement ele = xdoc.Root.Elements("MISSEDCALL").Elements("SERVICE").First(x => (string)x.Attribute("SID") == objsms.SID);

                    if (ele.Attribute("TYPE") != null)
                        ele.Attribute("TYPE").Value = objsms.Type;
                    else
                        ele.Add(new XAttribute("TYPE", objsms.Type.ToString()));

                    if (ele.Attribute("VALIDATE357") != null)
                        ele.Attribute("VALIDATE357").Value = objsms.Validate357;
                    else
                        ele.Add(new XAttribute("VALIDATE357", objsms.Validate357.ToString()));

                    if (ele.Attribute("VALIDATESMS") != null)
                        ele.Attribute("VALIDATESMS").Value = objsms.ValidateSMS;
                    else
                        ele.Add(new XAttribute("VALIDATESMS", objsms.ValidateSMS.ToString()));

                    if (ele.Attribute("DESCRIPTION") != null)
                        ele.Attribute("DESCRIPTION").Value = objsms.Description;
                    else
                        ele.Add(new XAttribute("DESCRIPTION", objsms.Description.ToString()));

                    if (ele.Attribute("CHECKDURATION") != null)
                        ele.Attribute("CHECKDURATION").Value = objsms.CheckDuration.ToString();
                    else
                        ele.Add(new XAttribute("CHECKDURATION", objsms.Description.ToString()));

                    if (ele.Descendants("SMSURL").Any())
                        ele.Descendants("SMSURL").FirstOrDefault().Value = objsms.SMSSetting.SMSUrl;
                    else
                    {
                        XElement nele = ele.Elements("SMSSETTINGS").FirstOrDefault();
                        nele.Add(new XElement("SMSURL", objsms.SMSSetting.SMSUrl));
                    }

                    if (ele.Descendants("PROVIDERID").Any())
                        ele.Descendants("PROVIDERID").FirstOrDefault().Value = objsms.SMSSetting.ProviderId;
                    else
                    {
                        XElement nele = ele.Elements("SMSSETTINGS").FirstOrDefault();
                        nele.Add(new XElement("PROVIDERID", objsms.SMSSetting.ProviderId));
                    }

                    if (ele.Descendants("APPNAME").Any())
                        ele.Descendants("APPNAME").FirstOrDefault().Value = objsms.SMSSetting.AppName;
                    else
                    {
                        XElement nele = ele.Elements("SMSSETTINGS").FirstOrDefault();
                        nele.Add(new XElement("APPNAME", objsms.SMSSetting.AppName));
                    }

                    if (ele.Descendants("TYPEID").Any())
                        ele.Descendants("TYPEID").FirstOrDefault().Value = objsms.SMSSetting.TypeId;
                    else
                    {
                        XElement nele = ele.Elements("SMSSETTINGS").FirstOrDefault();
                        nele.Add(new XElement("TYPEID", objsms.SMSSetting.TypeId));
                    }

                    if (ele.Descendants("SMS").Any())
                        ele.Descendants("SMS").FirstOrDefault().Value = objsms.Message.SMSMessage;
                    else
                    {
                        XElement nele = ele.Elements("SMSSETTINGS").Elements("MESSAGES").FirstOrDefault();
                        nele.Add(new XElement("SMS", objsms.Message.SMSMessage));
                    }

                    if (ele.Descendants("REPEAT_SMS").Any())
                        ele.Descendants("REPEAT_SMS").FirstOrDefault().Value = objsms.Message.RepeatSMS;
                    else
                    {
                        XElement nele = ele.Elements("SMSSETTINGS").Elements("MESSAGES").FirstOrDefault();
                        nele.Add(new XElement("REPEAT_SMS", objsms.Message.RepeatSMS));
                    }

                    if (ele.Descendants("DAYLIMIT_EXCEEDS").Any())
                        ele.Descendants("DAYLIMIT_EXCEEDS").FirstOrDefault().Value = objsms.Message.DayLimitExceed;
                    else
                    {
                        XElement nele = ele.Elements("SMSSETTINGS").Elements("MESSAGES").FirstOrDefault();
                        nele.Add(new XElement("DAYLIMIT_EXCEEDS", objsms.Message.DayLimitExceed));
                    }

                    if (ele.Descendants("WEEKLIMIT_EXCEEDS").Any())
                        ele.Descendants("WEEKLIMIT_EXCEEDS").FirstOrDefault().Value = objsms.Message.WeekLimitExceed;
                    else
                    {
                        XElement nele = ele.Elements("SMSSETTINGS").Elements("MESSAGES").FirstOrDefault();
                        nele.Add(new XElement("WEEKLIMIT_EXCEEDS", objsms.Message.WeekLimitExceed));
                    }

                    if (ele.Descendants("TOTAL_EXCEEDS").Any())
                        ele.Descendants("TOTAL_EXCEEDS").FirstOrDefault().Value = objsms.Message.TotalLimitExceed;
                    else
                    {
                        XElement nele = ele.Elements("SMSSETTINGS").Elements("MESSAGES").FirstOrDefault();
                        nele.Add(new XElement("TOTAL_EXCEEDS", objsms.Message.TotalLimitExceed));
                    }
                    if (ele.Descendants("OBDURL").Any())
                        ele.Descendants("OBDURL").FirstOrDefault().Value = objsms.OBDSetting.OBDUrl;
                    else
                    {
                        XElement nele = ele.Elements("OBDSETTINGS").FirstOrDefault();
                        nele.Add(new XElement("OBDURL", objsms.OBDSetting.OBDUrl));
                    }
                    if (ele.Descendants("PROVIDERID").Any())
                        ele.Descendants("PROVIDERID").FirstOrDefault().Value = objsms.OBDSetting.ProviderId;
                    else
                    {
                        XElement nele = ele.Elements("OBDSETTINGS").FirstOrDefault();
                        nele.Add(new XElement("PROVIDERID", objsms.OBDSetting.ProviderId));
                    }
                    if (ele.Descendants("CALLFLOWID").Any())
                        ele.Descendants("CALLFLOWID").FirstOrDefault().Value = objsms.OBDSetting.CallFlowId;
                    else
                    {
                        XElement nele = ele.Elements("OBDSETTINGS").FirstOrDefault();
                        nele.Add(new XElement("CALLFLOWID", objsms.OBDSetting.CallFlowId));
                    }
                    if (ele.Descendants("MENUFILE").Any())
                        ele.Descendants("MENUFILE").FirstOrDefault().Value = objsms.OBDSetting.MenuFile;
                    else
                    {
                        XElement nele = ele.Elements("OBDSETTINGS").FirstOrDefault();
                        nele.Add(new XElement("MENUFILE", objsms.OBDSetting.MenuFile));
                    }
                    if (ele.Descendants("CALLTYPE").Any())
                        ele.Descendants("CALLTYPE").FirstOrDefault().Value = objsms.OBDSetting.CallType;
                    else
                    {
                        XElement nele = ele.Elements("OBDSETTINGS").FirstOrDefault();
                        nele.Add(new XElement("CALLTYPE", objsms.OBDSetting.CallType));
                    }
                    xdoc.Save(clGeneral.GetConfigValue("XML_FOLDER") + "SMS.xml");
                     guid = Guid.NewGuid().ToString().Replace("-", "");
                    error = new Error()
                    {
                        ErrorCode = "0",
                        ErrorMessge = "successfully saved",
                        Id = guid
                    };
                    HttpContext.Current.Session["UID"] = guid;
                   return srep = JsonConvert.SerializeObject(error);
                  
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "readxml", LogMode.Excep, ex, "readxml - Dual");
            }
            guid = Guid.NewGuid().ToString().Replace("-", "");
            error = new Error()
            {
                ErrorCode = "2",
                ErrorMessge = "Failed",
                Id = guid
            };
            HttpContext.Current.Session["UID"] = guid;
            return srep = JsonConvert.SerializeObject(error);         
        }

        /// <summary>
        /// This method is to get the Users
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string getUserSuccessMsg(string name)
        {
            string successMsg = string.Empty;
            try
            {
                if (xdoc.Descendants("USERS").Descendants("SUCCESS").Descendants(name).Any())
                    successMsg = xdoc.Descendants("USERS").Descendants("SUCCESS").Descendants(name).FirstOrDefault().Value.ToString();
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "ReadXML", LogMode.Excep, ex, "clReadXML- getUserSuccessMsg");
            }

            return successMsg;
        }

        /// <summary>
        /// This method is to get the user success message
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string getUserErrorMsg(string name)
        {
            string errorsMsg = string.Empty;
            try
            {
                if (xdoc.Descendants("USERS").Descendants("ERROR").Descendants(name).Any())
                    errorsMsg = xdoc.Descendants("USERS").Descendants("ERROR").Descendants(name).FirstOrDefault().Value.ToString();
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "ReadXML", LogMode.Excep, ex, "clReadXML- getUserErrorMsg");
            }

            return errorsMsg;
        }
    }
}
