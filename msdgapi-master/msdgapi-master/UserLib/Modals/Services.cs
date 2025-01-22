namespace User
{
    public class SMS
    {
        public string SID { get; set; }      
        public string Type { get; set; }
        public int CheckDuration { get; set; }
        public string DisplayCheckDuration { get; set; }
        public string Validate357 { get; set; }
        public string ValidateSMS { get; set; }
        public string Description { get; set; }
        public SMSSetting SMSSetting {get;set; }
        public Message Message{get;set; }
        public OBD OBDSetting { get; set; }
        public URL UrlSetting { get; set; }
    }

    public class SMSSetting
    {
        public string SMSUrl { get; set; }
        public string ProviderId { get; set; }
        public string AppName { get; set; }
        public string TypeId { get; set; }
        public Message Message { get; set; }
        public OBD OBDSetting { get; set; }
        
     
    }

    public class Message
    {
        public string SMSMessage { get; set; }
        public string RepeatSMS { get; set; }
        public string DayLimitExceed { get; set; }
        public string WeekLimitExceed { get; set; }
        public string TotalLimitExceed { get; set; }
    }

    public class OBD
    {
        public string OBDUrl { get; set; }
        public string ProviderId { get; set; }
        public string CallFlowId { get; set; }
        public string MenuFile { get; set; }
        public string CallType { get; set; }
    }

    public class URL
    {
        public string UrlSetting { get; set; }
    }
}
