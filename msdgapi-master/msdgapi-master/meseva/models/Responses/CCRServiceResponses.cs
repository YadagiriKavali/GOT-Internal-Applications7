using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace meseva.models.Responses
{
    public class CCRDocYearsResp : MSResponse
    {
        public List<string> DocYears = new List<string>();
    }

    public class CCRDocListResp : MSResponse
    {
        public List<string> DocumentNumbers = new List<string>();
    }
}
