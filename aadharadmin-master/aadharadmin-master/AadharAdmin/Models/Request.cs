using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AadharAdmin.Models
{
    public class Request
    {
        public string tid = DateTime.Now.Ticks.ToString();
        public string channel = "WEB";
    }
}