namespace meseva.models.Requests
{
    public class SurveyNumberReq : MSRequest
    {
        public string DistrictId { get; set; }
        public string MandalId { get; set; }
        public string VillageId { get; set; }
        public string DocYear { get; set; }
    }

    public class AdangaldetailReq : MSRequest
    {
        public string DistrictId { get; set; }
        public string MandalId { get; set; }
        public string VillageId { get; set; }
        public string SurveyNo { get; set; }
        public string IsCropDetails { get; set; }
        public string ApplicationNo { get; set; }
        public string DocYear { get; set; }
    }

    public class AdangalTransactionNoReq : MSRequest
    {
        public string DocumentRefNumbers = string.Empty;
        public string ApplicationNo = string.Empty;
        public string ApplicantName = string.Empty;
        public string ApplicantDistrict = string.Empty;
        public string ApplicantMandal = string.Empty;
        public string ApplicantVillage = string.Empty;
        public string CropFlag = string.Empty;
        public string DeliveryType = string.Empty;
        public AdangalProfile Profile { get; set; }
        public Charge Charge { get; set; }
        public AdangalDocument Document { get; set; }
    }

    public class AdangalProfile : Profile
    {
        public string FatherName = string.Empty;
        public string Perstate = string.Empty;
        public string PhoneNo = string.Empty;
    }
    
    public class AdangalDocument
    {
        public string DocYear = string.Empty;
        public string DocDistrictId = string.Empty;
        public string DocMandalId = string.Empty;
        public string DocVillageId = string.Empty;
        public string DocSurveyNo = string.Empty;
        public string DocApplicationform = string.Empty;
    }
}
