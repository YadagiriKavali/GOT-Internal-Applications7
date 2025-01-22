using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace meseva.models.Responses
{
    public class LicenceTypeResp : MSResponse
    {
        public List<Detail> LicenceTypes { get; set; }

        public LicenceTypeResp()
        {
            LicenceTypes = new List<Detail>();
        }
    }
}
