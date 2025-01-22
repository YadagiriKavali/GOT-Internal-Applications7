using System.Collections.Generic;

namespace meseva.models.Responses
{
    public class ApplicationNoResp : MSResponse
    {
        public string ApplicationNo { get; set; }
        public List<ApplicationDocument> Documents { get; set; }

        public ApplicationNoResp()
        {
            Documents = new List<ApplicationDocument>();
        }
    }

    public class DistrictsResp : MSResponse
    {
        public List<Detail> Districts { get; set; }

        public DistrictsResp()
        {
            Districts = new List<Detail>();
        }
    }

    public class MandalsResp : MSResponse
    {
        public List<Detail> Mandals { get; set; }

        public MandalsResp()
        {
            Mandals = new List<Detail>();
        }
    }

    public class VillagesResp : MSResponse
    {
        public List<Detail> Villages { get; set; }

        public VillagesResp()
        {
            Villages = new List<Detail>();
        }
    }

    public class AppStatusResp : MSResponse
    {
        public string Status { get; set; }
    }

    public class ServiceChargeResp : MSResponse
    {
        public string ServiceId { get; set; }
        public string DeliveryType { get; set; }
        public string DeliveryCharges { get; set; }
        public string ServiceAmount { get; set; }
        public string UserCharges { get; set; }
        public string ChallanAmount { get; set; }
        public string SLA { get; set; }
    }

    public class RelationResp : MSResponse
    {
        public List<Detail> RelationDetail { get; set; }

        public RelationResp()
        {
            RelationDetail = new List<Detail>();
        }
    }

    public class CircleResp : MSResponse
    {
        public List<string> Circles { get; set; }

        public CircleResp()
        {
            Circles = new List<string>();
        }
    }

    public class CasteResp : MSResponse
    {
        public List<Detail> CasteDetail { get; set; }

        public CasteResp()
        {
            CasteDetail = new List<Detail>();
        }
    }

    public class ReligionResp : MSResponse
    {
        public List<Detail> ReligionDetail { get; set; }

        public ReligionResp()
        {
            ReligionDetail = new List<Detail>();
        }
    }

    public class OccupationResp : MSResponse
    {
        public List<Detail> Occupation { get; set; }

        public OccupationResp()
        {
            Occupation = new List<Detail>();
        }
    }

    public class DeathReasonResp : MSResponse
    {
        public List<Detail> DeathReasonDetail { get; set; }

        public DeathReasonResp()
        {
            DeathReasonDetail = new List<Detail>();
        }
    }

    public class CertificatePDFResp : MSResponse
    {
        public string PDFDocument { get; set; }
    }

    public class TransactionNoResp : MSResponse
    {
        public string TransactionNo { get; set; }
    }

    public class ApplicationDocument
    {
        public string DocId { get; set; }
        public string DocName { get; set; }
        public string Mandatory { get; set; }
    }

    public class Detail
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
