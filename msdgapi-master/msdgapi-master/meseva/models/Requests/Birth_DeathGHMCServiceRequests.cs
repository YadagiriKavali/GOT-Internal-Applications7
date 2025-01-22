
namespace meseva.models.Requests
{
    public class GHMCBirthOrDeathSearchRecordReq : ServiceBasedReq
    {
        public string ServiceType = string.Empty;
        public string DateOfBirth = string.Empty;
        public string RegistrationNo = string.Empty;
        public string CircleNo = string.Empty;
        public string Gender = string.Empty;
        public string MotherName = string.Empty;
        public string FatherHusbandName = string.Empty;
        public string DeceasedName = string.Empty;
    }

    public class GHMCBirthDeathDataByAckNoReq : ServiceBasedReq
    {
        public string ServiceType = string.Empty;
        public string AckNo = string.Empty;
    }

    public class GHMCServiceChargeReq : ServiceBasedReq
    {
        public string ServiceType = string.Empty;
        public string DeliveryType = string.Empty;
        public string NoOfCopies = string.Empty;
    }

    public class GHMCBirthDeathTransactionNo : TransactionNoReq
    {
        public string ServiceType = string.Empty;
        public string AcknowledgementNo = string.Empty;
        public string RegistrationNumber = string.Empty;
        public string EventName = string.Empty;
        public string EventDate = string.Empty;
        public string Gender = string.Empty;
        public string Circle = string.Empty;
        public string Ward = string.Empty;
        public string Locality = string.Empty;
        public string FatherHusbandName = string.Empty;
        public string MotherName = string.Empty;
        public string ResidenceAddress = string.Empty;
        public string PermanentAddress = string.Empty;
        public string RegDate = string.Empty;
        public string DeathCause = string.Empty;
        public string PlaceOfEvent = string.Empty;
        public string Remarks = string.Empty;
        public string InformantName = string.Empty;
        public string InformantRelation = string.Empty;
        public string InformantAddress = string.Empty;
        public string InformantPhoneNo = string.Empty;
        public string AadhaarNo = string.Empty;
        public string RationCardNo = string.Empty;
        public string InformantEmailId = string.Empty;
        public string InformantRemarks = string.Empty;
        public string InformantPinCode = string.Empty;
        public string DeliveryType = string.Empty;
        public string NumberOfCopies = string.Empty;
        public string Purpose = string.Empty;
        public string postalDoorNo = string.Empty;
        public string postalDistrict = string.Empty;
        public string postalMandalId = string.Empty;
        public string postalVillageId = string.Empty;
        public string postalPinCode = string.Empty;
        public string CreatedBy = string.Empty;
        public string DocApplicationform = string.Empty;
        public Charge Charge { get; set; }
    }

    public class GHMCBDCertificatePDFReq : ApplicationNoBasedReq
    {
        public string Service = string.Empty;
        public string ServiceType = string.Empty;
    }

    public class GHMCCorrectionBirthDeathTransactionNo : TransactionNoReq
    {
        public string ServiceType = string.Empty;
        public string AcknowledgementNo = string.Empty;
        public string RegistrationNumber = string.Empty;
        public string Gender = string.Empty;
        public string ChangedGender = string.Empty;
        public string EventName = string.Empty;
        public string ChangedEventName = string.Empty;
        public string FatherName = string.Empty;
        public string ChangedFatherName = string.Empty;
        public string MotherName = string.Empty;
        public string ChangedMotherName = string.Empty;
        public string DateOfEvent = string.Empty;
        public string ChangedDateOfEvent = string.Empty;
        public string PlaceOfEvent = string.Empty;
        public string ChangedPlaceOfEvent = string.Empty;
        public string Circle = string.Empty;
        public string Changedcircle = string.Empty;
        public string WardNo = string.Empty;
        public string Locality = string.Empty;
        public string ResidenceAddress = string.Empty;
        public string ChangedResidenceAddress = string.Empty;
        public string ReasonForDeath = string.Empty;
        public string ChangedReasonForDeath = string.Empty;
        public string PermanentAddress = string.Empty;
        public string Remarks = string.Empty;
        public string Relation = string.Empty;
        public string RationCardNo = string.Empty;
        public string AadhaarCardNo = string.Empty;
        public string ApplicantAddress = string.Empty;
        public string PinCode = string.Empty;
        public string PhoneNo = string.Empty;
        public string PostalDoorNo = string.Empty;
        public string PostalLocality = string.Empty;
        public string PostalDistrict = string.Empty;
        public string PostalMandalId = string.Empty;
        public string PostalVillageId = string.Empty;
        public string PostalPinCode = string.Empty;
        public string PostalMobileNo = string.Empty;
        public string PostalEmailId = string.Empty;
        public string DeliveryType = string.Empty;
        public string NumberOfCopies = string.Empty;
        public string Purpose = string.Empty;        
        public string CreatedBy = string.Empty;
        public string PhysicalDocument = string.Empty;
        public string Certificate = string.Empty;
        public string ParentsDeclaration = string.Empty;
        public string NotaryAffidavit = string.Empty;
        public string AvailableDocuments = string.Empty;
        public string HospitalLetter = string.Empty;
        public string MedicoLegalCase = string.Empty;
        public Charge Charge { get; set; }
    }

    public class GHMCCNInclusionTransactionNoReq : TransactionNoReq
    {
        public string ServiceType = string.Empty;
        public string AcknowledgementNo = string.Empty;
        public string RegistrationNumber = string.Empty;
        public string ActualchildName = string.Empty;
        public string ChangedchildName = string.Empty;
        public string FatherName = string.Empty;
        public string MotherName = string.Empty;
        public string Gender = string.Empty;
        public string DateOfBirth = string.Empty;
        public string Circle = string.Empty;
        public string WardNo = string.Empty;
        public string Locality = string.Empty;
        public string PlaceOfEvent = string.Empty;
        public string ApplicantAddress = string.Empty;
        public string PermanentAddress = string.Empty;
        public string Remarks = string.Empty;
        public string InformantName = string.Empty;
        public string InformantRelation = string.Empty;
        public string RationCardNo = string.Empty;
        public string AadhaarNo = string.Empty;
        public string InformantAddress = string.Empty;
        public string InformantPinCode = string.Empty;
        public string InformantPhoneNo = string.Empty;
        public string DeliveryType = string.Empty;
        public string PostalDoorNo = string.Empty;
        public string PostalLocality = string.Empty;
        public string PostalDistrict = string.Empty;
        public string PostalMandalId = string.Empty;
        public string PostalVillageId = string.Empty;
        public string PostalPinCode = string.Empty;
        public string PostalMobileNo = string.Empty;
        public string PostalEmailId = string.Empty;
        public string NumberOfCopies = string.Empty;
        public string Purpose = string.Empty;
        public string CreatedBy = string.Empty;
        public string DocApplicationform = string.Empty;
        public string DocAffidavit = string.Empty;
        public Charge Charge { get; set; }
    }

    public class GetGHMCNABDTransactionNoReq : TransactionNoReq
    {
        public string ServiceType = string.Empty;
        public string AadhaarNo = string.Empty;
        public string ApplicantFatherName = string.Empty;
        public string Age = string.Empty;
        public string Address = string.Empty;
        public string PhoneNo = string.Empty;
        public string EventRelation = string.Empty;
        public string RationCardNumber = string.Empty;
        public string EmailID = string.Empty;
        public string EventName = string.Empty;
        public string DateofEvent = string.Empty;
        public string PlaceofEvent = string.Empty;
        public string DeathCause = string.Empty;
        public string ResidenceAddress = string.Empty;
        public string EventGender = string.Empty;
        public string MotherName = string.Empty;
        public string FatherName = string.Empty;
        public string Circle = string.Empty;
        public string Ward = string.Empty;
        public string DeliveryType = string.Empty;
        public string PostalDoorNo = string.Empty;
        public string PostalDistrict = string.Empty;
        public string PostalMandal = string.Empty;
        public string PostalVillage = string.Empty;
        public string PostalPincode = string.Empty;
        public string NumberOfCopies = string.Empty;
        public string Purpose = string.Empty;
        public string CreatedBy = string.Empty;
        public string PhysicalDocument = string.Empty;
        public string RationOtherResidenceProof = string.Empty;
        public string SchoolBonafied = string.Empty;
        public string SSCMark = string.Empty;
        public string Deathnotarized = string.Empty;
        public string Otherevidence = string.Empty;
        public string Affidavit = string.Empty;
        public string OtherDoc = string.Empty;
        public string Medicallegal = string.Empty;
        public Charge Charge { get; set; }
    }
}
