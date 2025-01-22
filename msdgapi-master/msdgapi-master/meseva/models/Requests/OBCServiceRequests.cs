
namespace meseva.models.Requests
{
    public class OBCTransactionNoReq : TransactionNoReq
    {
        public string DocumentRefNumbers = string.Empty;
        public OBCProfile Profile { get; set; }
        public OBCCertificate Certificate { get; set; }
        public OBCCharge Charge { get; set; }
        public OBCDocument Document { get; set; }
    }

    public class OBCProfile : Profile
    {
        public string Relation = string.Empty;
        public string FatherName = string.Empty;
        public string PhoneNo = string.Empty;
    }

    public class OBCCertificate
    {
        public string DeliveryType = string.Empty;
        public string IssuedCasteCertificateInPast = string.Empty;
        public string CasteClaimed = string.Empty;
        public string CasteCategory = string.Empty;
        public string EducationCertificateContainsCaste = string.Empty;
        public string PurposeofCasteCertificate = string.Empty;
        public string Religion = string.Empty;
    }

    public class OBCCharge : Charge
    {
        public string FamilyIncome = string.Empty;
    }

    public class OBCDocument
    {
        public string DocApplicationform = string.Empty;
        public string DocIDProof = string.Empty;
        public string DocProperty = string.Empty;
        public string DocITReturns = string.Empty;
    }
}
