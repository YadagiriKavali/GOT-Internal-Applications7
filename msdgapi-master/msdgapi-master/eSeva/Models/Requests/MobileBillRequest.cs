namespace eseva.Models.Requests
{
    public class BillRequest
    {
        public string Tid { get; set; }
        public string MobileNo { get; set; }
        public string Number { get; set; }
        public string Channel { get; set; }
    }
    
    public class CPDCLBillReq : BillRequest
    {
        public string EroCode { get; set; }
    }
}
