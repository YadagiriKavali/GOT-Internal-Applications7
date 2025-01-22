
namespace meseva.models.Requests
{
    public class SROReq : MSRequest
    {
        public string DistrictId = string.Empty;
    }

    public class ECDocumentReq : MSRequest
    {
        public string DistrictId = string.Empty;
        public string SRO1 = string.Empty;
        public string RegYear = string.Empty;
        public string DocumentNo = string.Empty;
    }

    public class ECtransactionIdReq : TransactionNoReq
    {
        public string DocDistrict = string.Empty;
        public string SROId = string.Empty;
        public string DocNo = string.Empty;
        public string DocYear = string.Empty;
        public string FromDate = string.Empty;
        public string ToDate = string.Empty;
        public string BLDGFlatNo = string.Empty;
        public string BLDGOldHouseNo = string.Empty;
        public string BLDGAprtment = string.Empty;
        public string BLDGWard = string.Empty;
        public string BLDGBlock = string.Empty;
        public string BLDGVillageID = string.Empty;
        public string BLDGAliasVillage = string.Empty;
        public string AGRLPlotNo = string.Empty;
        public string AGRLSurveyNo = string.Empty;
        public string AGRLVillageID = string.Empty;
        public string AGRLAliasVillage = string.Empty;
        public string BNDREAST = string.Empty;
        public string BNDRWEST = string.Empty;
        public string BNDRNORTH = string.Empty;
        public string BNDRSOUTH = string.Empty;
        public string BNDRExtent = string.Empty;
        public string BNDRBUILTUP = string.Empty;
        public string ECSlno = string.Empty;
        public string SroJdn = string.Empty;
        public string AadharNo = string.Empty;
        public string OwnerName = string.Empty;
        public string PerDoorNo = string.Empty;
        public string PerDistrict = string.Empty;
        public string PerMandal = string.Empty;
        public string PerVillage = string.Empty;
        public string PerPincode = string.Empty;
        public string PostalDoorNo = string.Empty;
        public string PostalLocality = string.Empty;
        public string PostalState = string.Empty;
        public string PostalDistrict = string.Empty;
        public string PostalMandal = string.Empty;
        public string PostalVillage = string.Empty;
        public string PostalPincode = string.Empty;
        public string EmailId = string.Empty;
        public string DeliveryType = string.Empty;
        public string ChalanAmount = string.Empty;
        public string PostalCharge = string.Empty;
        public string UserCharge = string.Empty;
        public string TotalAmount = string.Empty;
    }
}
