namespace User
{
    public class clEService
    {

    }
    public class sms
    {
        public string sid { get; set; }      
        public string type { get; set; }
        public int checkduration { get; set; }
        public string displaycheckduration { get; set; }
        public string validate357 { get; set; }
        public string validatesms { get; set; }
        public string Description { get; set; }
        public smssetting smssetting {get;set; }
        public message message{get;set; }
        public obd obdsetting { get; set; }
        public url urlsetting { get; set; }
    }
    public class smssetting
    {
        public string smsurl { get; set; }
        public string Providerid { get; set; }
        public string appname { get; set; }
        public string typeid { get; set; }
        public message message { get; set; }
        public obd obdsetting { get; set; }
        
     
    }
    public class message
    {
        public string smsmessage { get; set; }
        public string repeatsms { get; set; }
        public string daylimitexceed { get; set; }
        public string weeklimitexceed { get; set; }
        public string totallimitexceed { get; set; }
    }
    public class obd
    {
        public string obdurl { get; set; }
        public string providerid { get; set; }
        public string callflowid { get; set; }
        public string menufile { get; set; }
        public string calltype { get; set; }
    }
    public class url
    {
        public string urlsetting { get; set; }
    }
}
