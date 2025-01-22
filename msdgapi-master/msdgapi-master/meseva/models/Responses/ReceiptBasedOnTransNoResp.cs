namespace meseva.models.Responses
{
    public class ReceiptBasedOnTransNoResp : MSResponse
    {
        public string TransID { get; set; }
        public string TransDate { get; set; }
        public string ApplicationNumber { get; set; }
        public string ApplicantName { get; set; }
        public string ServiceName { get; set; }
        public string LoginID { get; set; }
        public string SLA { get; set; }
    }

    public class AdangalReceiptBasedOnTransNoResp : ReceiptBasedOnTransNoResp
    {
        public string DocumentSurveyNo { get; set; }
        public string DocumentDistrict { get; set; }
        public string DocumentMandal { get; set; }
        public string DocumentVillage { get; set; }
        public string FasliYear { get; set; }
    }

    public class IncomeReceiptBasedOnTransNoResp : ReceiptBasedOnTransNoResp
    {
        public string FatherName { get; set; }
        public string DeliveryType { get; set; }
    }

    public class ResidenceReceiptBasedOnTransNoResp : ReceiptBasedOnTransNoResp
    {
        public string FatherName { get; set; }
        public string DeliveryType { get; set; }
    }

    public class EBCReceiptBasedOnTransNoResp : ReceiptBasedOnTransNoResp
    {
        public string CasteClaimed { get; set; }
    }

    public class OBCReceiptBasedOnTransNoResp : ReceiptBasedOnTransNoResp
    {
        public string FatherName { get; set; }
        public string DeliveryType { get; set; }
        public string CasteClaimed { get; set; }
    }

    public class FMReceiptBasedOnTransNoResp : ReceiptBasedOnTransNoResp
    {
        public string FatherName { get; set; }
        public string DeliveryType { get; set; }
    }

    public class FMBCopyReceiptBasedOnTransNoResp : ReceiptBasedOnTransNoResp
    {
        public string FatherName { get; set; }
        public string DeliveryType { get; set; }
        public string CasteClaimed { get; set; }
    }

    public class RORReceiptBasedOnTransNoResp : FMBCopyReceiptBasedOnTransNoResp
    {
    }

    public class TempleReceiptBasedOnTransNoResp : MSResponse
    {
        public string TransID { get; set; }
        public string TransDate { get; set; }
        public string ApplicationNumber { get; set; }
        public string SevaName { get; set; }
        public string SevaType { get; set; }
        public string SevaDate { get; set; }
        public string ReportingTime { get; set; }
        public string DevoteeName { get; set; }
        public string InformantName { get; set; }
        public string SevaAmount { get; set; }
        public string UserCharge { get; set; }
        public string TotalAmount { get; set; }
        public string LoginID { get; set; }
    }

    public class RoomBookingReceiptBasedOnTransNoResp : MSResponse
    {
        public string TransID { get; set; }
        public string TransDate { get; set; }
        public string ApplicationNumber { get; set; }
        public string RoomName { get; set; }
        public string CheckinDate { get; set; }
        public string NoofDays { get; set; }
        public string DevoteeName { get; set; }
        public string InformantName { get; set; }
        public string RoomAmount { get; set; }        
        public string UserCharge { get; set; }
        public string AmountPaid { get; set; }
        public string LoginID { get; set; }
    }

    public class CDMABDReceiptBasedOnTransNoResp : MSResponse
    {
        public string TransactionID { get; set; }
        public string AuthorizedAgent { get; set; }
        public string ApplicationNumber { get; set; }
        public string DateofPayment { get; set; }
        public string InformantName { get; set; }
        public string District { get; set; }
        public string DeliveryType { get; set; }
        public string NoOfCopies { get; set; }
        public string AmountPaidInRs { get; set; }
        public string TransactionSlipNo { get; set; }
        public string ApproverCodevarchar { get; set; }
        public string DeliveredIn { get; set; }
    }

    public class ECReceiptBasedOnTransNoResp : MSResponse
    {
        public string DateofPayment = string.Empty;
        public string AuthorizedAgentName = string.Empty;
        public string ApplicationNo = string.Empty;
        public string TransactionID = string.Empty;
        public string ApplicantName = string.Empty;
        public string PropertyOwnerName = string.Empty;
        public string DocumentDistrict = string.Empty;
        public string DocumentID = string.Empty;
        public string DocumentYear = string.Empty;
        public string SelectedSRO = string.Empty;
        public string AmountPaidinRs = string.Empty;
        public string DeliveryType = string.Empty;
        public string DeliveredIn = string.Empty;
    }

    public class CCRReceiptBasedOnTransNoResp : MSResponse
    {
        public string TransactionID = string.Empty;
        public string ApplicationNo = string.Empty;
        public string TransDate = string.Empty;
        public string ApplicantName = string.Empty;
        public string ApplicantVillage = string.Empty;
        public string DocumentDistrict = string.Empty;
        public string SRO = string.Empty;
        public string YearofRegistration = string.Empty;
        public string DocumentID = string.Empty;
        public string NumberofPages = string.Empty;
        public string ChallanAmount = string.Empty;
        public string UserCharges = string.Empty;
        public string TotalAmountPaid = string.Empty;
        public string TotalAmount = string.Empty;
        public string CertificateDeliveryDate = string.Empty;
        public string DeliveryType = string.Empty;
        public string ReceiptHeading = string.Empty;
        public string AuthorizedAgentName = string.Empty;
        public string AuthorizedAgentName1 = string.Empty;
        public string CertificateDeliveryWithIn = string.Empty;
    }
}
