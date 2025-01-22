using System;
using System.Messaging;
using IMI.Logger;

namespace eseva.Utilities
{
    public class MSMQPush
    {
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
                LogData.Write("ESEVA", "MSMQ-Exception", LogMode.Excep, ex, string.Format("MSMQPush => MessagePush - Exception: {0}", ex.Message));
                msgQueue = null;
            }
        }
    }
}
