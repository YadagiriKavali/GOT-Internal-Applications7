namespace meseva.models.Requests
{
    public class TempleSevasReq : ServiceBasedReq
    {
        public string TempleId = string.Empty;
    }

    public class TempleBatchesReq : TempleSevasReq
    {
        public string SevaId = string.Empty;
    }

    public class TempleSevaDetailReq : TempleBatchesReq
    {
        public string BatchId = string.Empty;
    }

    public class TempleRoomReq : MSRequest
    {
        public string RoomTypeId = string.Empty;
        public string TempleId = string.Empty;
    }

    public class EndowmentRoomAmountReq : MSRequest
    {
        public string RoomId = string.Empty;
        public string TempleId = string.Empty;
    }

    public class AvailableRoomsDateReq : EndowmentRoomAmountReq
    {
        public string CheckinDate = string.Empty;
    }

    public class SevaBookingTransactionNoReq : TempleSevaDetailReq
    {
        public string SevaBookingDate = string.Empty;
        public string ApplicationNo = string.Empty;
        public string ApplicantName = string.Empty;
        public string DeliveryType = string.Empty;
        public SevaBookingProfile Profile { get; set; }
        public SevaBookingCharge Charge { get; set; }
        public SevaBookingDocument Document { get; set; }
    }

    public class RoomBookingTransactionNoReq : TempleSevasReq
    {
        public string DocumentRefNumbers = string.Empty;
        public string RoomType = string.Empty;
        public string RoomID = string.Empty;
        public string RoombookingDate = string.Empty;
        public string ApplicationNo = string.Empty;
        public string ApplicantName = string.Empty;
        public string ApplicantRelation = string.Empty;

        public RoomBookingProfile Profile { get; set; }
        public RoomBookingCharge Charge { get; set; }
        public RoomBookingDocument Document { get; set; }
    }


    public class SevaBookingProfile
    {
        public string AadhaarNo = string.Empty;
        public string DevoteeName = string.Empty;
        public string Nakshatram = string.Empty;
        public string Gothram = string.Empty;
        public string Gender = string.Empty;
        public string Age = string.Empty;
        public string HouseNo = string.Empty;
        public string Location = string.Empty;
        public string EmailId = string.Empty;
        public string CountryId = string.Empty;
        public string StateId = string.Empty;
        public string District = string.Empty;
        public string Mandal = string.Empty;
        public string Village = string.Empty;
        public string Pincode = string.Empty;
        public string StateName = string.Empty;
        public string DistrictName = string.Empty;
        public string MandalName = string.Empty;
        public string VillageName = string.Empty;
        public string RelationId = string.Empty;
    }

    public class SevaBookingCharge
    {
        public string SevaAmount = string.Empty;
        public string Usercharges = string.Empty;
        public string TotalAmount = string.Empty;
    }

    public class SevaBookingDocument
    {
        public string ProofDocumentID = string.Empty;
        public string ProofDocumentNumber = string.Empty;
        public string ProofDocumentName = string.Empty;
        public string DocApplicationform = string.Empty;
        public string DocIDProof = string.Empty;
        public string DocPhoto = string.Empty;
    }

    public class RoomBookingProfile
    {
        public string AadhaarNo = string.Empty;
        public string DevoteeName = string.Empty;
        public string Gothram = string.Empty;
        public string Nakshatram = string.Empty;
        public string Gender = string.Empty;
        public string Age = string.Empty;
        public string EmailId = string.Empty;
        public string HouseNo = string.Empty;
        public string Location = string.Empty;
        public string CountryId = string.Empty;
        public string StateId = string.Empty;
        public string District = string.Empty;
        public string Mandal = string.Empty;
        public string Village = string.Empty;
        public string Pincode = string.Empty;
        public string StateName = string.Empty;
        public string DistrictName = string.Empty;
        public string MandalName = string.Empty;
        public string VillageName = string.Empty; 
    }

    public class RoomBookingCharge
    {
        public string RoomAmount = string.Empty;
        public string Usercharges = string.Empty;
        public string TotalAmount = string.Empty;
    }

    public class RoomBookingDocument
    {
        public string ProofDocumentID = string.Empty;
        public string ProofDocumentNumber = string.Empty;
        public string ProofDocumentName = string.Empty;
        public string DocApplicationform = string.Empty;
        public string DocIDProof = string.Empty;
    }
}
