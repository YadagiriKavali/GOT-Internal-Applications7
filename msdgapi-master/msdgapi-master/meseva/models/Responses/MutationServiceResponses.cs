using System.Collections.Generic;

namespace meseva.models.Responses
{
    public class MutationTypeResp : MSResponse
    {
        public List<Detail> MutationTypes { get; set; }

        public MutationTypeResp()
        {
            MutationTypes = new List<Detail>();
        }
    }

    public class MutationCasteResp : MSResponse
    {
        public List<Detail> MutationCastes { get; set; }

        public MutationCasteResp()
        {
            MutationCastes = new List<Detail>();
        }
    }

    public class DivisionResp : MSResponse
    {
        public List<Detail> Divisions { get; set; }

        public DivisionResp()
        {
            Divisions = new List<Detail>();
        }
    }
}
