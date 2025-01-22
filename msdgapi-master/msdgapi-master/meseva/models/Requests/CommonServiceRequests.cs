
namespace meseva.models.Requests
{
    public class ServiceBasedReq : MSRequest
    {
        public string Service = string.Empty;
    }

    public class ApplicationNoBasedReq : MSRequest
    {
        public string ApplicationNo = string.Empty;
    }

    public class ServiceTypeBasedReq : MSRequest
    {
        public string ServiceType = string.Empty;
    }

    public class MandalReq : MSRequest
    {
        public string DistrictId = string.Empty;
    }

    public class VillageReq : MandalReq
    {
        public string MandalId = string.Empty;
    }

    public class ReceiptBasedOnTransNoReq : ServiceBasedReq
    {
        public string TransactionNo = string.Empty;
    }

    public class ServiceChargeReq : ServiceBasedReq
    {
        public string DeliveryType = string.Empty;
    }
}
