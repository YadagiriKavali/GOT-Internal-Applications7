using System.Collections.Generic;

namespace meseva.models.Responses
{
    public class SearchResponse : MSResponse
    {
        public string CircleNo = string.Empty;
        public string AckNo = string.Empty;
        public string Sex = string.Empty;
        public string HospitalName = string.Empty;        
        public string RegistrationNo = string.Empty;
        public string WardNo = string.Empty;
        public string RegistrationDate = string.Empty;
        public string SignedBy = string.Empty;
        public string SignedDate = string.Empty;
    }

    public class GHMCBirthDeathDataByAckNoResp : SearchResponse
    {
        public string DOB = string.Empty;
        public string ChildName = string.Empty;
        public string MotherName = string.Empty;
        public string FatherHusbandName = string.Empty;
        public string TypeOfHospital = string.Empty;
        public string ResidenceAddress = string.Empty;
        public string Locality = string.Empty;
    }

    public class GHMCBirthOrDeathSearchRecordResp : GHMCBirthDeathDataByAckNoResp
    {
        public string HMExists = string.Empty;
    }

    public class GHMCServiceChargeResp : ServiceChargeResp
    {
        public string TotalAmount = string.Empty;
    }

    public class GHMCSearchBirthRecordResp : MSResponse
    {
        public List<GHMCBirthDeathDataByAckNoResp> BirthRecords { get; set; }

        public GHMCSearchBirthRecordResp()
        {
            BirthRecords = new List<GHMCBirthDeathDataByAckNoResp>();
        }
    }

    public class GHMCSearchDeathRecordResp : MSResponse
    {
        public List<GHMCSearchDeathRecord> DeathRecords { get; set; }

        public GHMCSearchDeathRecordResp()
        {
            DeathRecords = new List<GHMCSearchDeathRecord>();
        }
    }

    public class GHMCSearchDeathRecord : SearchResponse
    {
        public string DOD = string.Empty;
        public string DeceasedName = string.Empty;
        public string MotherName = string.Empty;
        public string FatherHusbandName = string.Empty;
        public string DeathCause = string.Empty;
        public string PresentAddress = string.Empty;
        public string PermanentAddress = string.Empty;
        public string DeathLocation = string.Empty;
    }
}
