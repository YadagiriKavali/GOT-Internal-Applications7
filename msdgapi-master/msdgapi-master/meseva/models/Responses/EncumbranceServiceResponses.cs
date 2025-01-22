using System.Collections.Generic;

namespace meseva.models.Responses
{
    public class SROResp : MSResponse
    {
        public List<Detail> SROs { get; set; }

        public SROResp()
        {
            SROs = new List<Detail>();
        }
    }

    public class ECDocumentResp : MSResponse
    {
        public string SRO = string.Empty;
        public string BVillage = string.Empty;
        public string BVillAlias = string.Empty;
        public string Colony = string.Empty;
        public string Apartment = string.Empty;
        public string FlatNo = string.Empty;
        public string HouseNo = string.Empty;
        public string SyNo = string.Empty;
        public string PlotNo = string.Empty;
        public string FromDate = string.Empty;
        public string ToDate = string.Empty;
        public string Ward = string.Empty;
        public string Block = string.Empty;
        public string East = string.Empty;
        public string West = string.Empty;
        public string South = string.Empty;
        public string North = string.Empty;
        public string Extent = string.Empty;
        public string Built = string.Empty;
        public string SroJdn = string.Empty;
        public string SLNo = string.Empty;
    }
}
