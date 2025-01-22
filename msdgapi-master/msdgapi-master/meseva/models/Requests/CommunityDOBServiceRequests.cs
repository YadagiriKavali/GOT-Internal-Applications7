using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace meseva.models.Requests
{
    public class CNCasteReq : MSRequest
    {
        public string DistrictId = string.Empty;
        public string ServiceType = string.Empty;
        public string Whom = string.Empty;
    }

    public class CNSubtribeReq : CNCasteReq
    {
        public string Caste = string.Empty;
    }

    public class CNDOBTransactionNoReq : TransactionNoReq
    {
        public string ServiceType = string.Empty;
        public string AadhaarNo = string.Empty;
        public string Gender = string.Empty;
        public string FatherOrHusbandName = string.Empty;
        public string MotherName = string.Empty;
        public string DateofBirth = string.Empty;
        public string Age = string.Empty;
        public string PresentDoorNo = string.Empty;
        public string PresentLocality = string.Empty;
        public string PresentDistrict = string.Empty;
        public string PresentMandal = string.Empty;
        public string PresentVillage = string.Empty;
        public string PresentPinCode = string.Empty;
        public string PermanentDoorNo = string.Empty;
        public string PermanentLocality = string.Empty;
        public string PermanentDistrict = string.Empty;
        public string PermanentMandal = string.Empty;
        public string PermanentVillage = string.Empty;
        public string PermanentPinCode = string.Empty;
        public string ResidenceDoorNo = string.Empty;
        public string ResidencetLocality = string.Empty;
        public string ResidenceDistrict = string.Empty;
        public string ResidenceMandal = string.Empty;
        public string ResidenceVillage = string.Empty;
        public string ResidencetPinCode = string.Empty;
        public string POBDoorNo = string.Empty;
        public string POBLocality = string.Empty;
        public string POBState = string.Empty;
        public string POBDistrict = string.Empty;
        public string POBMandal = string.Empty;
        public string POBVillage = string.Empty;
        public string POBPinCode = string.Empty;
        public string CommunityCertificatePastYesNo = string.Empty;
        public string ApplicantCommunity = string.Empty;
        public string SubtribeOrSubgroupofApplicant = string.Empty;
        public string FatherCommunity = string.Empty;
        public string SubtribeFather = string.Empty;
        public string MotherCommunity = string.Empty;
        public string SubtribeMother = string.Empty;
        public string ApplicantReligion = string.Empty;
        public string FatherReligion = string.Empty;
        public string MotherReligion = string.Empty;
        public string NaturalBornOrAdoptedBaby = string.Empty;
        public string HouseholdSurveyNo = string.Empty;
        public string EmailId = string.Empty;
        public string DeliveryType = string.Empty;
        public string ServiceCharge = string.Empty;
        public string PostalCharge = string.Empty;
        public string UserCharge = string.Empty;
        public string TotalAmount = string.Empty;
        public string DocApplicationform = string.Empty;
        public string DocIDProof = string.Empty;
        public string DocDOBCert = string.Empty;
        public string DocSSCMarksMemo = string.Empty;
        public string DocImmovableProperties = string.Empty;
        public string DocStudyCert = string.Empty;
    }
}
