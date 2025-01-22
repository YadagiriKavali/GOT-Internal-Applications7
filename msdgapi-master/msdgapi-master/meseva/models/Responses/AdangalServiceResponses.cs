using System.Collections.Generic;

namespace meseva.models.Responses
{
    public class RevenueDocYearResp : MSResponse
    {
        public List<RevenueDocYear> DocYear { get; set; }

        public RevenueDocYearResp()
        {
            DocYear = new List<RevenueDocYear>();
        }
    }

    public class AdangaldetailResp : MSResponse
    {
        public string SerialNo { get; set; }
        public string SurveyNo { get; set; }
        public string TotalExtent { get; set; }
        public string UncultivatedLand { get; set; }
        public string CultivatableLand { get; set; }
        public string LandNature { get; set; }
        public string Tax { get; set; }
        public string LandClassification { get; set; }
        public string WaterResource { get; set; }
        public string AyakatExtent { get; set; }
        public string KhataNumber { get; set; }
        public string PattadarName { get; set; }
        public string OccupantName { get; set; }
        public string OccupantExtent { get; set; }
        public string EnjoymentNature { get; set; }
        public string OccupantFatherName { get; set; }
        public string PattadarFatherName { get; set; }
        public string VillageName { get; set; }
        public string VillageCode { get; set; }
        public string DeleteFlag { get; set; }
        public string MutatedDate { get; set; }
        public string BaseSurveyNo { get; set; }
        public string Signature { get; set; }
        public string SignatureChecked { get; set; }
        public string VerifiedBy { get; set; }
        public string LandExtentUnits { get; set; }
        public string PassbookNumber { get; set; }
        public string Fasaliyear { get; set; }
        public string Pahaniyear { get; set; }
        public string TarhaCode { get; set; }
        //public string TotalExtent { get; set; }
        public string CR_TR_I_EXT { get; set; }
        public string PCRSeason { get; set; }
        public string MonthName { get; set; }
        public string CRPNAME { get; set; }
        public string CRSowType { get; set; }
        public string WaterSourceDesc { get; set; }
        public string CR1stExt { get; set; }
        public string CR2nd3rdExt { get; set; }
        public string CRYield { get; set; }
        public string CRVaoRiRem { get; set; }
        public string CRMroRem { get; set; }
        public string Remarks { get; set; }
        public string SingleJoint { get; set; }
        public string MeesevaAppliNo { get; set; }
    }

    public class RevenueDocYear
    {
        public string DocYear { get; set; }
        public string DocYearDesc { get; set; }
    }
}
