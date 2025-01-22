namespace meseva.models.Requests
{
    public class PopulateSurveyNumberReq : MSRequest
    {
        public string DistrictId { get; set; }
        public string MandalId { get; set; }
        public string VillageId { get; set; }
        public string DocYear { get; set; }        
    }
}
