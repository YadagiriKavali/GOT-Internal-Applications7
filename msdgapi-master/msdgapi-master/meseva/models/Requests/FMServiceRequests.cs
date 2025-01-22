namespace meseva.models.Requests
{
    public class FMTransactionNoReq : TransactionNoReq
    {
        public string ServiceType = string.Empty;
        public string DeliveryType = string.Empty;
        public FMProfile Profile { get; set; }
        public FMDeceasedInfo DeceaseInfo { get; set; }
        public Charge Charge { get; set; }
        public FMDocument Document { get; set; }
    }

    public class FMProfile : Profile
    {
        public string Relation = string.Empty;
        public string RelationName = string.Empty;
    }

    public class FMDeceasedInfo
    {
        public string DeceasedName = string.Empty;
        public string DeceasedFName = string.Empty;
        public string DateofDeath = string.Empty;
        public string ReasonforDeath = string.Empty;
        public string Occupation = string.Empty;
        public string ReasonforCertificate = string.Empty;
        public string DeathPlace = string.Empty;
        public string GridFamilyMemberDetails = string.Empty;
    }

    public class FMDocument
    {
        public string DocApplicationform = string.Empty;
        public string DocIDProof = string.Empty;
        public string DocDeathProof = string.Empty;
    }
}
