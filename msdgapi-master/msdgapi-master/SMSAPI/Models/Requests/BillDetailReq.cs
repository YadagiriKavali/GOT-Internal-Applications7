namespace SMSAPI.Models.Requests
{
    public class BillDetailReq
    {
        public string logtid { get; set; }
        public string msisdn { get; set; }
        public string SMS { get; set; }
        public string SRC { get; set; }
        public string CMD { get; set; }
        public string carrier { get; set; }
    }
}