using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace meseva.models.Responses
{
    public class CNDOBServiceTypeResp : MSResponse
    {
        public List<Detail> CNDOBServiceTypes { get; set; }

        public CNDOBServiceTypeResp()
        {
            CNDOBServiceTypes = new List<Detail>();
        }
    }

    public class CNCasteResp : MSResponse
    {
        public List<Detail> CNCastes { get; set; }

        public CNCasteResp()
        {
            CNCastes = new List<Detail>();
        }
    }

    public class CNSubtribeResp : MSResponse
    {
        public string CasteCode = string.Empty;
        public string CasteName = string.Empty;
        public string SubCaste = string.Empty;
    }
}
