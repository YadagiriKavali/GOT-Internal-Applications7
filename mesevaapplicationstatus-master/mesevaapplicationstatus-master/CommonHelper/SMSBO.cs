//$Author: Anilkumar.ko $
//$Date: 12/05/17 6:33p $
//$Header: /Mobileapps/Operator/Davinci/TS/MeesevaApplicationStatusSMSPush/MeesevaApplicationStatusSMSPush/CommonHelper/SMSBO.cs 1     12/05/17 6:33p Anilkumar.ko $
//$Modtime: 8/27/15 10:29a $
//$Revision: 1 $
//Copyright @2012 IMImobile Pvt. Ltd.

using System;
using System.Configuration;
using System.IO;
using System.Messaging;
using System.Xml;

namespace CommonHelper
{
    public class SMSBO
    {
        private const string CONST_MOBILENO = "MOBILENO";
        private const string CONST_CHANNEL = "CHANNEL";
        private const string CONST_DEPARTMENT = "DEPARTMENT";
        private const string CONST_SENDERID = "SENDERID";
        private const string CONST_MESSAGE = "MESSAGE";
        private const string CONST_TRANSDATE = "TRANSDATE";

        private string m_strMobileNo = String.Empty;
        private string m_strChannel = String.Empty;
        private string m_strDepartment = String.Empty;
        private string m_strSenderID = String.Empty;
        private string m_strMessage = String.Empty;
        private string m_strTransDate = String.Empty;

        private XmlDocument m_xmlDocCdr = new XmlDocument();

        public string MobileNo
        {
            set
            {
                m_strMobileNo = value;
            }
            get
            {
                return m_strMobileNo;
            }
        }

        public string Channel
        {
            set
            {
                m_strChannel = value;
            }
            get
            {
                return m_strChannel;
            }
        }

        public string Department
        {
            set
            {
                m_strDepartment = value;
            }
            get
            {
                return m_strDepartment;
            }
        }

        public string SenderID
        {
            set
            {
                m_strSenderID = value;
            }
            get
            {
                return m_strSenderID;
            }
        }

        public string Message
        {
            set
            {
                m_strMessage = value;
            }
            get
            {
                return m_strMessage;
            }
        }

        public string TransDate
        {
            set
            {
                m_strTransDate = value;
            }
            get
            {
                return m_strTransDate;
            }
        }

        private string GetNodeValue(string strNodeName)
        {
            try
            {
                XmlNode xmlnd = m_xmlDocCdr.SelectSingleNode("LOG/" + strNodeName);
                if (xmlnd == null)
                    return "";

                return xmlnd.InnerText.ToString();
            }
            catch
            {
            }
            return "";
        }

        private void LoadCDRElements()
        {
            m_strMobileNo = GetNodeValue(CONST_MOBILENO);
            m_strChannel = GetNodeValue(CONST_CHANNEL);
            m_strDepartment = GetNodeValue(CONST_DEPARTMENT);
            m_strSenderID = GetNodeValue(CONST_SENDERID);
            m_strMessage = GetNodeValue(CONST_MESSAGE);
            m_strTransDate = GetNodeValue(CONST_TRANSDATE);
        }

        public bool LoadCDRRecord(string _cdrrecord)
        {
            try
            {
                m_xmlDocCdr.LoadXml(_cdrrecord);
                LoadCDRElements();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        public bool LogMessage()
        {
            try
            {
                if (!GenerateXmlCDR())
                    return false;

                string strQueueName = @".\private$\gokemailsmspush";
                if (General.GetConfigVal("QUEUE_EMAILSMS_PUSH").Trim().Length > 0)
                    strQueueName = General.GetConfigVal("QUEUE_EMAILSMS_PUSH").Trim();

                return MSMQPush(strQueueName, "SMSPUSH", m_xmlDocCdr.OuterXml.ToString());
            }
            catch (Exception ex)
            {
                General.WriteLog("SMS_QUEUE_ERROR", m_strMobileNo + ",Error in LogSMSMessage(), Error:" + ex.Message + "," + ex.StackTrace.Replace("\n", " ").Replace("\r\n", " "));
            }
            return true;
        }

        private bool CreatePropNode(string _name, string _value, ref XmlNode _node)
        {
            XmlNode xmlnd = m_xmlDocCdr.CreateNode(XmlNodeType.Element, _name, "");
            xmlnd.InnerText = _value;
            _node.AppendChild(xmlnd);
            xmlnd = null;
            return true;
        }

        private bool GenerateXmlCDR()
        {
            XmlNode xmlMainNode = m_xmlDocCdr.CreateNode(XmlNodeType.Element, "LOG", "");

            CreatePropNode(CONST_MOBILENO, m_strMobileNo, ref xmlMainNode);
            CreatePropNode(CONST_CHANNEL, m_strChannel, ref xmlMainNode);
            CreatePropNode(CONST_DEPARTMENT, m_strDepartment, ref xmlMainNode);
            CreatePropNode(CONST_SENDERID, m_strSenderID, ref xmlMainNode);
            CreatePropNode(CONST_MESSAGE, m_strMessage, ref xmlMainNode);
            CreatePropNode(CONST_TRANSDATE, m_strTransDate, ref xmlMainNode);

            m_xmlDocCdr.AppendChild(xmlMainNode);
            xmlMainNode = null;

            Console.WriteLine(m_xmlDocCdr.OuterXml.ToString());
            return true;
        }

        private bool MSMQPush(string _queuename, string _label, string _cdrrecord)
        {
            System.Messaging.MessageQueue objMSGQ = new System.Messaging.MessageQueue();
            System.Messaging.Message objMsg = null;
            try
            {
                objMSGQ.Path = _queuename;
                objMsg = new System.Messaging.Message();
                objMsg.Label = _label;
                //objMsg.Recoverable = true;
                objMsg.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                objMsg.Body = _cdrrecord;
                objMSGQ.Send(objMsg);
                objMsg = null;
                //General.WriteLog("SMS_PUSH_SUCCESS", _cdrrecord);
            }
            catch (Exception ex)
            {
                General.WriteLog("SMS_PUSH_FAIL", _cdrrecord);
                General.WriteLog("SMS_PUSH_ERROR", m_strMobileNo + ", Error in MSMQPush(), Error:" + ex.Message + ",StackTrace:" + ex.StackTrace.Replace("\r\n", " ").Replace("\n", " "));
                objMSGQ = null;
                objMsg = null;
                return false;
            }
            return true;
        }
    }
}
