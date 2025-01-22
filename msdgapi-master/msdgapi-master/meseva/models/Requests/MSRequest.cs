namespace meseva.models.Requests
{
    public class MSRequest
    {
        public string Tid = string.Empty;
        public string MobileNo = string.Empty;
        public string Channel = string.Empty;
    }

    public class TransactionNoReq : MSRequest
    {
        public string Service = string.Empty;
        public string AddressFlag = string.Empty;
        public string ApplicationNo = string.Empty;
        public string ApplicantName = string.Empty;
    }

    public class Profile
    {
        public string Gender = string.Empty;
        public string DateOfBirth = string.Empty;
        public string PermanentDoorNo = string.Empty;
        public string PermanentLocality = string.Empty;
        public string PermanentDistrict = string.Empty;
        public string PermanentMandal = string.Empty;
        public string PermanentVillage = string.Empty;
        public string PermanentPincode = string.Empty;
        public string PostalDoorNo = string.Empty;
        public string PostalLocality = string.Empty;
        public string StateId = string.Empty;
        public string PostalDistrict = string.Empty;
        public string PostalMandal = string.Empty;
        public string PostalVillage = string.Empty;
        public string PostalPincode = string.Empty;
        public string PostalState = string.Empty;
        public string EmailID = string.Empty;
        public string Remarks = string.Empty;
        public string RationCardNo = string.Empty;
        public string AadhaarNo = string.Empty;
    }

    public class Charge
    {
        public string ServiceCharge = string.Empty;
        public string PostalCharge = string.Empty;
        public string UserCharge = string.Empty;
        public string TotalAmount = string.Empty;
    }
}
