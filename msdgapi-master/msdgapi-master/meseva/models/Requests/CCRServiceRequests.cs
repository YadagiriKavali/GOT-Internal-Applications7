using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace meseva.models.Requests
{
    public class CCRDocYearReq : MSRequest
    {
        public int SROCode { get; set; }
    }

    public class CCRDocListReq : CCRDocYearReq
    {
        public int SROYear { get; set; }
    }

    public class CCRCopyDetailReq : MSRequest
    {
        public string DistrictId = string.Empty;
        public string SROCode = string.Empty;
        public string DocYear = string.Empty;
        public string DocumentNumber = string.Empty;
        public string ApplicationNo = string.Empty;
    }

    public class CCRDocTransactionNoReq : TransactionNoReq
    {
        public string DocDistrict = string.Empty;
        public string SROCode = string.Empty;
        public int YearofRegistration;
        public string DocumentId = string.Empty;
        public string DoorNo = string.Empty;
        public string PermanentDistrict = string.Empty;
        public string PermanentMandal = string.Empty;
        public string PermanentVillage = string.Empty;
        public string PinCode = string.Empty;
        public string EmailId = string.Empty;
        public string ChalanAmount = string.Empty;
        public string UserCharge = string.Empty;
        public string CourierCharge = string.Empty;
        public string TotalAmount = string.Empty;
    }
}
