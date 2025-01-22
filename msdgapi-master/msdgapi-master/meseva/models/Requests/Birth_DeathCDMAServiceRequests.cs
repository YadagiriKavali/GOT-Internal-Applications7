using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace meseva.models.Requests
{
    public class CDMARUIDDetailReq : MSRequest
    {
        public string LocationType = string.Empty;
        public string DistrictId = string.Empty;
    }

    public class CDMABirthDeathDetailReq : MSRequest
    {
        public string ServiceType = string.Empty;
        public string ApplicationNo = string.Empty;
        public string RegUnitId = string.Empty;
        public string RegYear = string.Empty;
        public string RegNo = string.Empty;
        public string TestId = string.Empty;
    }

    public class CDMABirthDeathSearchReq : MSRequest
    {
        public string ServiceType = string.Empty;
        public string ApplicationNo = string.Empty;
        public string RegUnitId = string.Empty;
        public string RegYear = string.Empty;
        public string RegNo = string.Empty;
        public string Gender = string.Empty;
    }

    public class CDMAServiceChargeReq : ServiceBasedReq
    {
        public string ServiceType = string.Empty;
        public string DeliveryType = string.Empty;
        public string RLBType = string.Empty;
        public string NoOfCopies = string.Empty;
    }

    public class CDMATransactionNo : MSRequest
    {
        public string Service = string.Empty;
        public string ServiceType = string.Empty;
        public string RegUnitId = string.Empty;
        public string RegNo = string.Empty;
        public string RegYear = string.Empty;
        public string ApplicationNo = string.Empty;
        public string RegDate = string.Empty;
        public string EventName = string.Empty;
        public string EventSurName = string.Empty;
        public string DateofBirthOrDeath = string.Empty;
        public string Gender = string.Empty;
        public string FatherOrHusbandName = string.Empty;
        public string FatherOrHusbandSurName = string.Empty;
        public string MotherName = string.Empty;
        public string MotherSurName = string.Empty;
        public string BirthorDeathPlace = string.Empty;
        public string LocationName = string.Empty;
        public string HospitalName = string.Empty;
        public string HospitalAddress1 = string.Empty;
        public string HospitalAddress2 = string.Empty;
        public string HospitalAddress3 = string.Empty;
        public string StateName = string.Empty;
        public string DistrictName = string.Empty;
        public string PinCode = string.Empty;
        public string InformantName = string.Empty;
        public string InformantRelation = string.Empty;
        public string InformantAddress1 = string.Empty;
        public string InformantAddress2 = string.Empty;
        public string InformantAddress3 = string.Empty;
        public string InformantPhoneNo = string.Empty;
        public string AadharCardNo = string.Empty;
        public string RationCardNo = string.Empty;
        public string InformantEmailId = string.Empty;
        public string InformantRemarks = string.Empty;
        public string InformantPinCode = string.Empty;
        public string DeliveryType = string.Empty;
        public string NoOfCopies = string.Empty;
        public string Purpose = string.Empty;
        public string PostalDoorNo = string.Empty;
        public string PostalLocality = string.Empty;
        public string PostalState = string.Empty;
        public string PostalDistrict = string.Empty;
        public string PostalMandalId = string.Empty;
        public string PostalVillageId = string.Empty;
        public string PostalPinCode = string.Empty;
        public string RLBType = string.Empty;
        public string ServiceCharge = string.Empty;
        public string PostalCharge = string.Empty;
        public string UserCharge = string.Empty;
        public string StationaryCharges = string.Empty;
        public string TotalAmount = string.Empty;
        public string DocApplicationform = string.Empty;
    }
}
