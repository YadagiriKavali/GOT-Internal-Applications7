namespace meseva.models.Requests
{
    public class IncomeTransactionNoReq : TransactionNoReq
    {
        public string DocumentRefNumbers = string.Empty;
        public IncomeProfile Profile { get; set; }
        public IncomeCertificate Income { get; set; }
        public Charge Charge { get; set; }
        public IncomeDocument Document { get; set; }
    }

    public class IncomeProfile : Profile
    {
        public string FatherName { get; set; }
    }

    public class IncomeCertificate
    {
        public string DeliveryType = string.Empty;
        public string LandIncome = string.Empty;
        public string BusinessIncome = string.Empty;
        public string BuildingIncome = string.Empty;
        public string LabourIncome = string.Empty;
        public string EmpSal = string.Empty;
        public string OtherIncome = string.Empty;
        public string TotalIncome = string.Empty;
        public string Purpose = string.Empty;
    }

    public class IncomeDocument
    {
        public string DocApplicationform = string.Empty;
        public string DocIDProof = string.Empty;
        public string DocIncomeProof = string.Empty;
    }
}
