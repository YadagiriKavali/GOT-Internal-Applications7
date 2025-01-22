using System.Collections.Generic;

namespace meseva.models.Responses
{
    public class FMServiceTypeResp : MSResponse
    {
        public List<Detail> FMServiceTypes { get; set; }

        public FMServiceTypeResp()
        {
            FMServiceTypes = new List<Detail>();
        }
    }

    public class ReasonCertificateResp : MSResponse
    {
        public List<Detail> Purposes { get; set; }

        public ReasonCertificateResp()
        {
            Purposes = new List<Detail>();
        }
    }

    public class SurveyNumberResp : MSResponse
    {
        public List<string> SurveyNumbers { get; set; }
    }
}
