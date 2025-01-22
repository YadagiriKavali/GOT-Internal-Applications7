namespace meseva.models.Requests
{
    public class ResidenceTransactionNoReq : TransactionNoReq
    {
        public string DocumentRefNumbers = string.Empty;
        public ResidenceProfile Profile { get; set; }
        public Charge Charge { get; set; }
        public ResidenceDocument Document { get; set; }
    }

    public class ResidenceProfile : Profile
    {
        public string FatherName = string.Empty;
        public string PhoneNo = string.Empty;
        public string DeliveryType = string.Empty;
        public string ResidingSinceinYears = string.Empty;
        public string Purpose = string.Empty;
    }

    public class ResidenceDocument
    {
        public string DocApplicationform = string.Empty;
        public string DocIDProof = string.Empty;
        public string DocHouseProof = string.Empty;
        public string DocPhoto = string.Empty;
    }
}
