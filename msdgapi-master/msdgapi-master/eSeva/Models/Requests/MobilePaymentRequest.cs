namespace eseva.Models.Requests
{
    public class MobilePaymentRequest
    {
        public string tid { get; set; }
        public string MobileNo { get; set; }
        public string Number { get; set; }
        public string Channel { get; set; }
        public string Network { get; set; }
        public string AccountNo { get; set; }
        public string Amount { get; set; }
        public string BillRefNo { get; set; }
    }
}
