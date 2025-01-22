using System;
using System.Configuration;
using System.Messaging;
using IMI.Logger;

namespace meseva.Utilities
{
    public class MSMQPush
    {
        #region Private Members

        private static string queuePath = ConfigurationManager.AppSettings["MSMQ_QUEUE_URL"];

        #endregion Private Members

        #region Public Methods

        public static void MessagePush(string queuename, string label, string cdrrecord)
        {
            var msgQueue = new MessageQueue();
            try
            {
                msgQueue.Path = queuename;
                var message = new Message
                {
                    Label = label,
                    Formatter = new XmlMessageFormatter(new Type[] { typeof(string) }),
                    Body = cdrrecord
                };
                msgQueue.Send(message);
                message = null;
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MSMQ", LogMode.Excep, ex, string.Format("MSMQPush => MessagePush - Exception: {0}", ex.Message));
                msgQueue = null;
            }
        }

        public static void DepartmentMessagePush(DepartmentRecord depRecord)
        {
            if (depRecord == null)
                return;

            string strRecord = "<xml><RefId>{0}</RefId><MethodName>{1}</MethodName><DeptName>{2}</DeptName><ServiceCode>{3}</ServiceCode><Request>{4}</Request><Response>{5}</Response><TimeTaken>{6}</TimeTaken><Reqtime>{7}</Reqtime><Restime>{8}</Restime></xml>";
            string msgRecord = string.Format(strRecord, depRecord.ReferenceId, depRecord.MethodName, depRecord.DepartmentName, depRecord.ServiceCode, depRecord.Request, depRecord.Response, depRecord.TimeTaken, depRecord.RequestTime, depRecord.ResponseTime);

            try
            {
                MSMQMsgPush(ConfigurationManager.AppSettings["MSMQ_DEPARTMENT_LABEL"], msgRecord);
            }
            catch (Exception ex)
            {
                LogData.Write("MSMQPush", "Department", LogMode.Excep, ex, string.Format("DepartmentMessagePush => Exception: {0}", ex.Message));
            }
        }

        public static void TransactionMessagePush(TransactionRecord transRecord)
        {
            if (transRecord == null)
                return;

            string strRecord = "<xml><RefId>{0}</RefId><MethodName>{1}</MethodName><DeptName>{2}</DeptName><ServiceCode>{3}</ServiceCode><Request>{4}</Request><Response>{5}</Response><TimeTaken>{6}</TimeTaken><Reqtime>{7}</Reqtime><Restime>{8}</Restime></xml>";
            string msgRecord = string.Format(strRecord, transRecord.ReferenceId, transRecord.MethodName, transRecord.DepartmentName, transRecord.ServiceCode, transRecord.Request, transRecord.Response, transRecord.TimeTaken, transRecord.RequestTime, transRecord.ResponseTime);

            try
            {
                MSMQMsgPush(ConfigurationManager.AppSettings["MSMQ_Transaction_LABEL"], msgRecord);
            }
            catch (Exception ex)
            {
                LogData.Write("MSMQPush", "Transaction", LogMode.Excep, ex, string.Format("TransactionMessagePush => Exception: {0}", ex.Message));
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static void MSMQMsgPush(string label, string record)
        {
            if (string.IsNullOrEmpty(queuePath) || string.IsNullOrEmpty(label))
                throw new Exception("MSMQ path or label not found");

            using (var msgQueue = new MessageQueue())
            {
                msgQueue.Path = queuePath;
                Message message = new Message
                {
                    Label = label,
                    Formatter = new XmlMessageFormatter(new Type[] { typeof(string) }),
                    Body = record
                };

                msgQueue.Send(message);
                message = null;
            }
        }

        #endregion Private Methods
    }

    #region POCOs

    public class DepartmentRecord
    {
        public string ReferenceId { get; set; }
        public string MethodName { get; set; }
        public string DepartmentName { get; set; }
        public string ServiceCode { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string RequestTime { get; set; }
        public string ResponseTime { get; set; }
        public string TimeTaken { get; set; }
    }

    public class TransactionRecord
    {
        public string ReferenceId { get; set; }
        public string MethodName { get; set; }
        public string DepartmentName { get; set; }
        public string ServiceCode { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string RequestTime { get; set; }
        public string ResponseTime { get; set; }
        public string TimeTaken { get; set; }
    }

    #endregion POCOs
}
