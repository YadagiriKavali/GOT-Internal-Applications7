using System;
using System.Messaging;
using System.Xml;
using CommonHelper;

namespace MeesevaApplicationStatusSMSPush
{
    public class SMS
    {
        private const string CONST_REFID = "REFID";
        private const string CONST_MOBILE = "MOBILE";
        private const string CONST_CHANNEL = "CHANNEL";
        private const string CONST_SERVICE = "SERVICE";
        private const string CONST_SERVICECODE = "SERVICECODE";
        private const string CONST_SENDERID = "SENDERID";
        private const string CONST_MESSAGE = "MESSAGE";
        private const string CONST_GWTID = "GWTID";
        private const string CONST_DLVSTATUS = "DLVSTATUS";
        private const string CONST_DLVTIME = "DLVTIME";
        private const string CONST_TIMETAKEN = "TIMETAKEN";
        private const string CONST_DIGITALID = "DIGITALID";

        public string RefID { get; set; }
        public string MobileNo { get; set; }
        public string Channel { get; set; }
        public string Service { get; set; }
        public string ServiceCode { get; set; }
        public string SenderID { get; set; }
        public string Message { get; set; }
        public string GWTID { get; set; }
        public string DeliveryStatus { get; set; }
        public string DeliveryTime { get; set; }
        public string TimeTaken { get; set; }
        public string DigitalID { get; set; }

        private XmlDocument m_xmlDocCdr = new XmlDocument();

        private String GetNodeValue(string strNodeName)
        {
            try
            {
                XmlNode xmlnd = m_xmlDocCdr.SelectSingleNode("LOG/" + strNodeName);
                if (xmlnd == null)
                    return String.Empty;

                return xmlnd.InnerText.ToString();
            }
            catch
            {
            }
            return String.Empty;
        }

        private void LoadCDRElements()
        {
            RefID = GetNodeValue(CONST_REFID);
            MobileNo = GetNodeValue(CONST_MOBILE);
            Channel = GetNodeValue(CONST_CHANNEL);
            Service = GetNodeValue(CONST_SERVICE);
            ServiceCode = GetNodeValue(CONST_SERVICECODE);
            SenderID = GetNodeValue(CONST_SENDERID);
            Message = GetNodeValue(CONST_MESSAGE);
            GWTID = GetNodeValue(CONST_GWTID);
            DeliveryStatus = GetNodeValue(CONST_DLVSTATUS);
            DeliveryTime = GetNodeValue(CONST_DLVTIME);
            TimeTaken = GetNodeValue(CONST_TIMETAKEN);
            DigitalID = GetNodeValue(CONST_DIGITALID);
        }

        public bool LoadCDRRecord(string _cdrrecord)
        {
            try
            {
                m_xmlDocCdr.LoadXml(_cdrrecord);
                LoadCDRElements();
            }
            catch //(Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                return false;
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
            CreatePropNode(CONST_REFID, RefID, ref xmlMainNode);
            CreatePropNode(CONST_MOBILE, MobileNo, ref xmlMainNode);
            CreatePropNode(CONST_CHANNEL, Channel, ref xmlMainNode);
            CreatePropNode(CONST_SERVICE, Service, ref xmlMainNode);
            CreatePropNode(CONST_SERVICECODE, ServiceCode, ref xmlMainNode);
            CreatePropNode(CONST_SENDERID, SenderID, ref xmlMainNode);
            CreatePropNode(CONST_MESSAGE, Message, ref xmlMainNode);
            CreatePropNode(CONST_GWTID, GWTID, ref xmlMainNode);
            CreatePropNode(CONST_DLVSTATUS, DeliveryStatus, ref xmlMainNode);
            CreatePropNode(CONST_DLVTIME, DeliveryTime, ref xmlMainNode);
            CreatePropNode(CONST_TIMETAKEN, TimeTaken, ref xmlMainNode);
            CreatePropNode(CONST_DIGITALID, DigitalID, ref xmlMainNode);
            m_xmlDocCdr.AppendChild(xmlMainNode);
            xmlMainNode = null;
            //Console.WriteLine(m_xmlDocCdr.OuterXml.ToString());
            return true;
        }

        public bool LogMessage()
        {
            try
            {
                if (!GenerateXmlCDR())
                    return false;

                string strQueueName = @".\private$\emailsmspush";
                if (General.GetConfigVal("QUEUE_PUSH").Trim().Length > 0)
                    strQueueName = General.GetConfigVal("QUEUE_PUSH").Trim();
                return MSMQPush(strQueueName, "SMSPUSH", m_xmlDocCdr.OuterXml.ToString());
            }
            catch (Exception ex)
            {
                General.WriteLog("LOG_FAIL_ERROR", RefID + ", Error in LogMessage(), Error:" + ex.Message + "," + ex.StackTrace.Replace("\n", " ").Replace("\r\n", " "));
            }
            return true;
        }

        private bool MSMQPush(string _queuename, string _label, string _cdrrecord)
        {
            MessageQueue objMSGQ = new MessageQueue();
            System.Messaging.Message objMsg = null;
            try
            {
                objMSGQ.Path = _queuename;
                objMsg = new System.Messaging.Message();
                objMsg.Label = _label;
                //objMsg.Recoverable = true;
                objMsg.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                objMsg.Body = _cdrrecord;
                if (General.GetConfigVal("QUEUE_ISTRANSACTIONAL").Trim().ToUpper() == "Y")
                    objMSGQ.Send(objMsg, MessageQueueTransactionType.Single);
                else
                    objMSGQ.Send(objMsg);
                objMsg = null;
            }
            catch (Exception ex)
            {
                General.WriteLog("LOG_FAIL", _cdrrecord);
                General.WriteLog("LOG_FAIL_ERROR", RefID + ", Error in MSMQPush(), Error:" + ex.Message + ",StackTrace:" + ex.StackTrace.Replace("\r\n", " ").Replace("\n", " "));
                objMSGQ = null;
                objMsg = null;
                return false;
            }
            return true;
        }
    }
}
