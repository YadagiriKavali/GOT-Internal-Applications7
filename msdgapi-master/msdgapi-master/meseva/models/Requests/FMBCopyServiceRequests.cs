namespace meseva.models.Requests
{
    public class FMBCopyDetailReq : MSRequest
    {
        public string DistrictId { get; set; }
        public string MandalId { get; set; }
        public string VillageId { get; set; }
        public string SurveyNo { get; set; }
        public string ApplicationNo { get; set; }
    }

    public class FMBCopyTransactionNoReq : TransactionNoReq
    {
        public string FMBDistrict = string.Empty;
        public string FMBMandal = string.Empty;
        public string FMBVillage = string.Empty;
        public string FMBSurveyNo = string.Empty;
        public string DeliveryType = string.Empty;
        public FMBCopyProfile Profile { get; set; }
        public Charge Charge { get; set; }
        public FMBCopyDocument Document { get; set; } 
    }

    public class FMBCopyProfile : Profile
    {
        public string FatherName = string.Empty;
    }

    public class FMBCopyDocument
    {
        public string DocApplicationform = string.Empty;
    }
}
