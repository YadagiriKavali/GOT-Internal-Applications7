namespace meseva.models.Requests
{
    public class RORDetailReq : ApplicationNoBasedReq
    {
        public string DistrictId = string.Empty;
        public string MandalId = string.Empty;
        public string VillageId = string.Empty;
        public string KathaNo = string.Empty;
    }

    public class RORTransactionNoReq : TransactionNoReq
    {
        public string DeliveryType = string.Empty;
        public string CreatedBy = string.Empty;
        public RORProfile Profile { get; set; }
        public Charge Charge { get; set; }
        public RORDocument Document { get; set; }
    }

    public class RORProfile
    {
        public string Gender = string.Empty;
        public string ApplicantFatherName = string.Empty;
        public string ApplicantDoorNo = string.Empty;
        public string ApplicationStreetname = string.Empty;
        public string ApplicantState = string.Empty;
        public string ApplicantDistrict = string.Empty;
        public string ApplicantMandal = string.Empty;
        public string ApplicantVillage = string.Empty;
        public string PinCode = string.Empty;
        public string PostalState = string.Empty;
        public string PostalDistrct = string.Empty;
        public string PostalMandal = string.Empty;
        public string PostalVillage = string.Empty;
        public string PostalPincode = string.Empty;
        public string PostalLocation = string.Empty;
        public string PostalDoorNo = string.Empty;
        public string RationcardNo = string.Empty;
        public string AadhaarNo = string.Empty;
        public string EmailId = string.Empty;
    }

    public class RORDocument
    {
        public string DocDistrictId = string.Empty;
        public string DocMandalId = string.Empty;
        public string DocVillageId = string.Empty;
        public string DocKhataNo = string.Empty;
    }
}
