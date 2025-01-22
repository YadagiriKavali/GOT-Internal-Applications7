using System.Collections.Generic;
namespace eseva.Models.Responses
{
    public class BillResponse
    {
        public string ResCode { get; set; }
        public string ResDesc { get; set; }
    }

    public class AirtelBillResp : BillResponse
    {
        public string AccountNo { get; set; }
        public string BillAmount { get; set; }
        public string ConsumerName { get; set; }
        public string ReqId { get; set; }
    }

    public class AirtelLLBillResp : BillResponse
    {
        public string AccountNo { get; set; }
        public string BillAmount { get; set; }
        public string ConsumerName { get; set; }
        public string ReqId { get; set; }
    }

    public class IdeaBillResp : BillResponse
    {
        public string AccountNo { get; set; }
        public string BillAmount { get; set; }
        public string ConsumerName { get; set; }
        public string ReqId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
    }

    public class WaterBillResp : BillResponse
    {
        public string Amount { get; set; }
        public string Address { get; set; }
        public string CanNo { get; set; }
        public string Category { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string Name { get; set; }
        public string ReqId { get; set; }
    }

    public class ActBillResp : BillResponse
    {
        public string AccountNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string BillNo { get; set; }
        public string BillDate { get; set; }
        public string BillAmount { get; set; }
        public string City { get; set; }
        public string CustomerName { get; set; }
        public string Country { get; set; }
        public string District { get; set; }
        public string ErrCD { get; set; }
        public string MobileNo { get; set; }
        public string Message { get; set; }
        public string Period { get; set; }
        public string Region { get; set; }
        public string ReqId { get; set; }
        public string State { get; set; }
        public string TransNo { get; set; }
    }

    public class RTAFeePaymentDetail : BillResponse
    {
        public string AppFee { get; set; }
        public string ApplicationNo { get; set; }
        public string CardFee { get; set; }
        public string CompFee { get; set; }
        public string DeptTransId { get; set; }
        public string DocNo { get; set; }
        public string GreenTax { get; set; }
        public string LateFee { get; set; }
        public string LifeTax { get; set; }
        public string MobileNo { get; set; }
        public string Name { get; set; }
        public string OfficeCd { get; set; }
        public string PostFee { get; set; }
        public string Qtax { get; set; }
        public string ReqId { get; set; }
        public string RtaAmount { get; set; }
        public string ServiceId { get; set; }
        public string SlotDate { get; set; }
        public string SlotTime { get; set; }
        public string SlotTimeid { get; set; }
        public string SrvcFee { get; set; }
        public string ServiceDesc { get; set; }
        public string TotAmt { get; set; }
        public string UserChargs { get; set; }
    }

    public class TTLBillResp : BillResponse
    {
        public string AccountNo { get; set; }
        public string BillAmount { get; set; }
        public string ConsumerName { get; set; }
        public string ReqId { get; set; }
        public string MobileNo { get; set; }
    }

    public class DistListDetailResp : BillResponse
    {
        public List<DistData> DistList { get; set; }
    }

    public class CPDCLEROListResp : BillResponse
    {
        public List<EroData> EroList { get; set; }
    }

    public class CPDCLBillResp : BillResponse
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Category { get; set; }
        public string Mobileno { get; set; }
        public string NetAmount { get; set; }
        public string ReqId { get; set; }
        public string Usercharges { get; set; }
        public string Arrears { get; set; }
        public string Billdate { get; set; }
        public string Billno { get; set; }
        public string Consumername { get; set; }
        public string Consumerno { get; set; }
        public string Currentdmd { get; set; }
        public string Discondate { get; set; }
        public string Duedate { get; set; }
        public string Ukscno { get; set; }
    }

    public class DistData
    {
        public string DistCode { get; set; }
        public string DistName { get; set; }
    }

    public class EroData
    {
        public string EroCode { get; set; }
        public string EroName { get; set; }
    }
}
