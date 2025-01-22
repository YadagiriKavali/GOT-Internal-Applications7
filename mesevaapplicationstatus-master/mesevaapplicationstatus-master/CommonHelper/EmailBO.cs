//$Author: Anilkumar.ko $
//$Date: 12/05/17 6:33p $
//$Header: /Mobileapps/Operator/Davinci/TS/MeesevaApplicationStatusSMSPush/MeesevaApplicationStatusSMSPush/CommonHelper/EmailBO.cs 1     12/05/17 6:33p Anilkumar.ko $
//$Modtime: 8/27/15 10:29a $
//$Revision: 1 $
//Copyright @2012 IMImobile Pvt. Ltd.

using System;
using System.Configuration;
using System.Messaging;
using System.Xml;

namespace CommonHelper
{
    public class EmailBO
    {
        private const string CONST_MOBILENO = "MOBILENO";
        private const string CONST_CHANNEL = "CHANNEL";
        private const string CONST_DEPARTMENT = "DEPARTMENT";
        private const string CONST_MAILTO = "MAILTO";
        private const string CONST_MAILCC = "MAILCC";
        private const string CONST_MAILBCC = "MAILBCC";
        private const string CONST_SUBJECT = "SUBJECT";
        private const string CONST_BODY = "BODY";
        private const string CONST_ATTACHMENT = "ATTACHMENT";
        private const string CONST_TRANSDATE = "TRANSDATE";

        private string m_strMobileNo = String.Empty;
        private string m_strChannel = String.Empty;
        private string m_strDepartment = String.Empty;
        private string m_strMailTo = String.Empty;
        private string m_strMailCC = String.Empty;
        private string m_strMailBCC = String.Empty;
        private string m_strSubject = String.Empty;
        private string m_strBody = String.Empty;
        private string m_strAttachment = String.Empty;
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

        public string MailTo
        {
            set
            {
                m_strMailTo = value;
            }
            get
            {
                return m_strMailTo;
            }
        }

        public string MailCC
        {
            set
            {
                m_strMailCC = value;
            }
            get
            {
                return m_strMailCC;
            }
        }

        public string MailBCC
        {
            set
            {
                m_strMailBCC = value;
            }
            get
            {
                return m_strMailBCC;
            }
        }

        public string Subject
        {
            set
            {
                m_strSubject = value;
            }
            get
            {
                return m_strSubject;
            }
        }

        public string Body
        {
            set
            {
                m_strBody = value;
            }
            get
            {
                return m_strBody;
            }
        }

        public string Attachment
        {
            set
            {
                m_strAttachment = value;
            }
            get
            {
                return m_strAttachment;
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
            m_strMailTo = GetNodeValue(CONST_MAILTO);
            m_strMailCC = GetNodeValue(CONST_MAILCC);
            m_strMailBCC = GetNodeValue(CONST_MAILBCC);
            m_strSubject = GetNodeValue(CONST_SUBJECT);
            m_strBody = GetNodeValue(CONST_BODY);
            m_strAttachment = GetNodeValue(CONST_ATTACHMENT);
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

                string strQueueName = @".\private$\emailsmspush";

                if (General.GetConfigVal("QUEUE_EMAILSMS_PUSH").Trim().Length > 0)
                    strQueueName = General.GetConfigVal("QUEUE_EMAILSMS_PUSH").Trim();
                return MSMQPush(strQueueName, "EMAILPUSH", m_xmlDocCdr.OuterXml.ToString());
            }
            catch (Exception ex)
            {
                General.WriteLog("EMAIL_QUEUE_ERROR", m_strMobileNo + ",Error in LogSMSMessage(), Error:" + ex.Message + "," + ex.StackTrace.Replace("\n", " ").Replace("\r\n", " "));
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
            CreatePropNode(CONST_MAILTO, m_strMailTo, ref xmlMainNode);
            CreatePropNode(CONST_MAILCC, m_strMailCC, ref xmlMainNode);
            CreatePropNode(CONST_MAILBCC, m_strMailBCC, ref xmlMainNode);
            CreatePropNode(CONST_SUBJECT, m_strSubject, ref xmlMainNode);
            CreatePropNode(CONST_BODY, m_strBody, ref xmlMainNode);
            CreatePropNode(CONST_ATTACHMENT, m_strAttachment, ref xmlMainNode);
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
                //General.WriteLog("EMAILPUSH_SUCCESS", _cdrrecord);
            }
            catch (Exception ex)
            {
                General.WriteLog("EMAIL_PUSH_FAIL", _cdrrecord);
                General.WriteLog("EMAIL_PUSH_ERROR", m_strMobileNo + ", Error in MSMQPush(), Error:" + ex.Message + ",StackTrace:" + ex.StackTrace.Replace("\r\n", " ").Replace("\n", " "));
                objMSGQ = null;
                objMsg = null;
                return false;
            }
            return true;
        }
    }
}
