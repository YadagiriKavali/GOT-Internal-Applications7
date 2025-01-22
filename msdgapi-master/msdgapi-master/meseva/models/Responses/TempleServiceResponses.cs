using System.Collections.Generic;

namespace meseva.models.Responses
{
    public class TempleDetailResp : MSResponse
    {
        public string TempleId { get; set; }
        public string TempleName { get; set; }
        public string TempleDesc { get; set; }
        public string TempleLocation { get; set; }
    }

    public class TempleBatchesResp : MSResponse
    {
        public List<BatcheDetail> Batches { get; set; }

        public TempleBatchesResp()
        {
            Batches = new List<BatcheDetail>();
        }
    }

    public class TempleSevasResp : MSResponse
    {
        public List<Detail> Sevas { get; set; }

        public TempleSevasResp()
        {
            Sevas = new List<Detail>();
        }
    }

    public class ProofDocumentsResp : MSResponse
    {
        public List<Detail> ProofOfDocuments { get; set; }

        public ProofDocumentsResp()
        {
            ProofOfDocuments = new List<Detail>();
        }
    }

    public class TempleSevaDetailResp : MSResponse
    {
        public List<SevaDetail> SevaDetails { get; set; }

        public TempleSevaDetailResp()
        {
            SevaDetails = new List<SevaDetail>();
        }
    }

    public class TempleSLNSevaDetailResp : MSResponse
    {
        public List<SLMSevaDetail> SevaDetails { get; set; }

        public TempleSLNSevaDetailResp()
        {
            SevaDetails = new List<SLMSevaDetail>();
        }
    }

    public class RoomTypeResp : MSResponse
    {
        public List<Detail> RoomTypes { get; set; }

        public RoomTypeResp()
        {
            RoomTypes = new List<Detail>();
        }
    }

    public class TempleRoomDetailResp : MSResponse
    {
        public List<TempleRoomDetail> RoomsDetail { get; set; }

        public TempleRoomDetailResp()
        {
            RoomsDetail = new List<TempleRoomDetail>();
        }
    }

    public class EndowmentRoomAmount : MSResponse
    {
        public string TempleId { get; set; }
        public string RoomId { get; set; }
        public string UserCharge { get; set; }
        public string RoomAmount { get; set; }
    }

    public class AvailableRoomsDateResp : MSResponse
    {
        public List<AvailableRoomsDate> RoomsDates { get; set; }

        public AvailableRoomsDateResp()
        {
            RoomsDates = new List<AvailableRoomsDate>();
        }
    }

    public class BatcheDetail
    {
        public string BatchID { get; set; }
        public string BatchName { get; set; }
        public string BatchType { get; set; }
        public string BatchAvailDays { get; set; }
    }

    public class AvailableRoomsDate
    {
        public string Date { get; set; }
        public string NoOfRooms { get; set; }
    }

    public class SevaDetail
    {
        public string SevaId { get; set; }
        public string SevaName { get; set; }
        public string SevaAmount { get; set; }
        public string BatchName { get; set; }
        public string UserCharge { get; set; }
        public string SevaDescription { get; set; }
        public string ReportingTime { get; set; }
        public string NoOfPersons { get; set; }
        public string SevaType { get; set; }
        public string MaxNoOfTickesAllowed { get; set; }
        public string IsActive { get; set; }
        public string PrasadamRequire { get; set; }
        public string SevaCategoryId { get; set; }
    }

    public class SLMSevaDetail
    {
        public string SevaId { get; set; }
        public string SevaName { get; set; }
        public string SevaAmount { get; set; }
        public string UserCharge { get; set; }
        public string SevaDescription { get; set; }
        public string ReportingTime { get; set; }
        public string NoOfPersons { get; set; }
        public string SevaType { get; set; }
        public string MaxNoOfTickesAllowed { get; set; }
        public string Laddu { get; set; }
        public string BigLaddu { get; set; }
        public string Pulihora { get; set; }
        public string Other { get; set; }
        public string IsActive { get; set; }
        public string PrasadamRequire { get; set; }
        public string SevaCategoryId { get; set; }
    }

    public class TempleRoomDetail
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Persons { get; set; }
        public string Rooms { get; set; }
        public string Rent { get; set; }
        public string Category { get; set; }
    }
}
