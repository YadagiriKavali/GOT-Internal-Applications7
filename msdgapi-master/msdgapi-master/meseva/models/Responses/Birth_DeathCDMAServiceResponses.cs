using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace meseva.models.Responses
{
    public class CDMARUIDDetailResp : MSResponse
    {
        public List<Detail> RUIDDetail { get; set; }

        public CDMARUIDDetailResp()
        {
            RUIDDetail = new List<Detail>();
        }
    }

    public class CDMABirthDetailResp : MSResponse
    {
        public CDMABirthDetail BirthDetail { get; set; }
    }

    public class CDMADeathDetailResp : MSResponse
    {
        public CDMADeathDetail DeathDetail { get; set; }
    }

    public class CDMABirthSearchResp : MSResponse
    {
        public List<CDMABirthSearch> BirthSearch { get; set; }

        public CDMABirthSearchResp()
        {
            BirthSearch = new List<CDMABirthSearch>();
        }
    }

    public class CDMADeathSearchResp : MSResponse
    {
        public List<CDMADeathSearch> DeathSearch { get; set; }

        public CDMADeathSearchResp()
        {
            DeathSearch = new List<CDMADeathSearch>();
        }
    }

    public class CDMAServiceChargeResp : MSResponse
    {
        public string ServiceId = string.Empty;
        public string ServiceType = string.Empty;
        public string DeliveryType = string.Empty;
        public string CourierCharge = string.Empty;
        public string ServiceAmount = string.Empty;
        public string UserCharge = string.Empty;
        public string ChallanAmount = string.Empty;
        public string SLA = string.Empty;
        public string StatuaryCharge = string.Empty;
        public string TotalAmount = string.Empty;
    }


    public class CDMABirthDeathDetail
    {
        public string AddressPrem1 = string.Empty;
        public string AddressPrem2 = string.Empty;
        public string AddressPrem3 = string.Empty;        
        public string DistrictName = string.Empty;
        public string DTIssue = string.Empty;
        public string FatherName = string.Empty;
        public string MotherName = string.Empty;
        public string DigitalSIG = string.Empty;
        public string OfficeTitle = string.Empty;
        public string OfficeId = string.Empty;
        public string RegDate = string.Empty;
        public string RegistrarOffice = string.Empty;
        public string RegLocation = string.Empty;
        public string RegNo = string.Empty;
        public string RegYear = string.Empty;
        public string RHAddress1 = string.Empty;
        public string RHAddress2 = string.Empty;
        public string RHAddress3 = string.Empty;
        public string SealText1 = string.Empty;
        public string SealText2 = string.Empty;
        public string Sex = string.Empty;
        public string StateName = string.Empty;
        public string Pin = string.Empty;
        public string CertificateId = string.Empty;
        public string DistrictLocal = string.Empty;
        public string MandalLocal = string.Empty;
        public string OfficeTitleLocal = string.Empty;
        public string Hash = string.Empty;
        public string MeesevaAppliNo = string.Empty;
        public string RLBTypeId = string.Empty;
    }

    public class CDMABirthDetail : CDMABirthDeathDetail
    {
        public string AddressAtBirth1 = string.Empty;
        public string AddressAtBirth2 = string.Empty;
        public string AddressAtBirth3 = string.Empty;
        public string SyNo = string.Empty;
        public string DateOfBirth = string.Empty;
        public string CertTitle = string.Empty;
        public string ChildName = string.Empty;
        public string ChildSURName = string.Empty;        
        public string MotherSURName = string.Empty;
        public string Res_Code = string.Empty;
    }

    public class CDMADeathDetail : CDMABirthDeathDetail
    {
        public string AddressAtDeath1 = string.Empty;
        public string AddressAtDeath2 = string.Empty;
        public string AddressAtDeath3 = string.Empty;
        public string Age = string.Empty;
        public string AgeIn = string.Empty;
        public string DeathPlace = string.Empty;
        public string DateOfDeath = string.Empty;
        public string LogId = string.Empty;
        public string MandName = string.Empty;
        public string Name = string.Empty;
        public string FMH = string.Empty;
    }

    public class CDMASearch
    {
        public string AddressAtBirth1 = string.Empty;
        public string AddressAtBirth2 = string.Empty;
        public string AddressAtBirth3 = string.Empty;
        public string AddressPrem1 = string.Empty;
        public string AddressPrem2 = string.Empty;
        public string AddressPrem3 = string.Empty;        
        public string FatherName = string.Empty;
        public string MotherName = string.Empty;
        public string OfficeId = string.Empty;
        public string RegDate = string.Empty;
        public string RegNo = string.Empty;
        public string RegYear = string.Empty;
        public string Res_Code = string.Empty;
        public string Sex = string.Empty;
        public string RHAddress1 = string.Empty;
        public string RHAddress2 = string.Empty;
        public string RHAddress3 = string.Empty;
        public string BirthPlace = string.Empty;
        public string HospitalName = string.Empty;
        public string LocationName = string.Empty;
    }

    public class CDMABirthSearch : CDMASearch
    {
        public string DateOfBirth = string.Empty;
    }

    public class CDMADeathSearch : CDMASearch
    {
        public string Name = string.Empty;
        public string DateOfDeath = string.Empty;
    }
}
