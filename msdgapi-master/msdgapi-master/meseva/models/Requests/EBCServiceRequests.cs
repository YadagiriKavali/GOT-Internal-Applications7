namespace meseva.models.Requests
{
    public class EBCTransactionNoReq : TransactionNoReq
    {
        public string DocumentRefNumbers = string.Empty;
        public EBCProfile Profile { get; set; }
        public EBCCertificate Certificate { get; set; }
        public EBCCharge Charge { get; set; }
        public EBCDocument Document { get; set; }
    }

    public class EBCProfile : Profile
    {
        public string Relation = string.Empty;
        public string FatherName = string.Empty;
        public string PhoneNo = string.Empty;
    }

    public class EBCCertificate
    {
        public string DeliveryType = string.Empty;
        public string IssuedCasteCertificateInPast = string.Empty;
        public string CasteClaimed = string.Empty;
        public string CasteCategory = string.Empty;
        public string Religion = string.Empty;
        public string PurposeofCasteCertificate = string.Empty;
    }

    public class EBCCharge : Charge
    {
        public string FamilyIncome = string.Empty;
    }

    public class EBCDocument
    {
        public string DocApplicationform = string.Empty;
        public string DocIDProof = string.Empty;
    }
}
