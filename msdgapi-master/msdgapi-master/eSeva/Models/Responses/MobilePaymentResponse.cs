using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eseva.Models.Responses
{
    public class PaymentResponse
    {
        public string ResCode { get; set; }
        public string ResDesc { get; set; }
    }

    public class MobilePaymentResponse : PaymentResponse
    {
        public string RegId { get; set; }
        public string RefNo { get; set; }
        public string Receipt { get; set; }
    }
}
