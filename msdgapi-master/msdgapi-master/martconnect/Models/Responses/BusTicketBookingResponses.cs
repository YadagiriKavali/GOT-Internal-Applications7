using System.Collections.Generic;

namespace martconnect.Models.Responses
{
    public class BTBCitiesResp : MCResponse
    {
        public List<Detail> Cities { get; set; }

        public BTBCitiesResp()
        {
            Cities = new List<Detail>();
        }
    }

    public class BTBSearchBusesResp : MCResponse
    {
        public string UserTrackId { get; set; }        
        public List<BusDetail> BusDetails { get; set; }

        public BTBSearchBusesResp()
        {
            BusDetails = new List<BusDetail>();
        }
    }

    public class BTBSeatAvailabilityResp : MCResponse
    {
        public string CustomParamRef = string.Empty;
        public SeatMap SeatMap { get; set; }
        public List<BookingTicket> BookingTickets { get; set; }
        public List<string> AvailableSeats { get; set; }
        public List<BordingPoint> BordingPoints { get; set; }
        public List<DropingPoint> DropingPoints { get; set; }

        public BTBSeatAvailabilityResp()
        {
            BookingTickets = new List<BookingTicket>();
            AvailableSeats = new List<string>();
            BordingPoints = new List<BordingPoint>();
            DropingPoints = new List<DropingPoint>();
        }
    }

    public class BookedTicketResp : MCResponse
    {
        public string UserTrackId = string.Empty;
        public string TransactionId = string.Empty;
        public string TransportPNR = string.Empty;
        public string TransportName = string.Empty;
        public string BusName = string.Empty;
        public string CoachName = string.Empty;
        public string OriginName = string.Empty;
        public string DestinationName = string.Empty;
        public string TravelDate = string.Empty;
        public string DepartureTime = string.Empty;
        public string TravelRelatedQueries = string.Empty;
        public TransportDetail TransportDetail { get; set; }
        public BoardingDetail BoardingDetail { get; set; }
        public DroppingDetail DroppingDetail { get; set; }
        public List<PassengerDetail> Passengers { get; set; }

        public BookedTicketResp()
        {
            Passengers = new List<PassengerDetail>();
        }
    }


    public class BusDetail
    {
        public string ScheduleId = string.Empty;
        public string StationId = string.Empty;
        public string BusId = string.Empty;
        public string BusName = string.Empty;
        public string TransportId = string.Empty;
        public string TransportName = string.Empty;
        public string CoachTypeId = string.Empty;
        public string DepartureTime = string.Empty;
        public string ArrivalTime = string.Empty;
        public string SeatsAvailable = string.Empty;
        public string StatusId = string.Empty;
        public string StatusDesc = string.Empty;
        public string Fare = string.Empty;
        public string ServiceTax = string.Empty;
        public string SeatTypeId = string.Empty;
        public string SeatTypeName = string.Empty;
        public string TotalFare = string.Empty;
        public string CustomParamsRef = string.Empty;
        public string ReturnPolicy = string.Empty;
        public string PartialCancellation = string.Empty;
    }

    public class SeatMap
    {
        public List<SeatColumn> SeatColumns { get; set; }

        public SeatMap()
        {
            SeatColumns = new List<SeatColumn>();
        }
    }

    public class SeatColumn
    {
        public List<Seat> Seats { get; set; }

        public SeatColumn()
        {
            Seats = new List<Seat>();
        }
    }

    public class Seat
    {
        public string SeatNo = string.Empty;
        public string SeatTypeId = string.Empty;
        public string SeatTypeName = string.Empty;
    }

    public class BookingTicket
    {
        public string BookedSeatNo = string.Empty;
        public string Gender = string.Empty;
        public string SeatTypeId = string.Empty;
    }

    public class BordingPoint
    {
        public string BoardingId = string.Empty;
        public string BoardingPlace = string.Empty;
        public string BoardingTime = string.Empty;
        public string BoardingAddress = string.Empty;
        public string BoardingLandmark = string.Empty;
        public string BoardingContactNo = string.Empty;
    }

    public class DropingPoint
    {
        public string DropingId = string.Empty;
        public string DropingPlace = string.Empty;
        public string DropingTime = string.Empty;
        public string DropingAddress = string.Empty;
        public string DropingLandmark = string.Empty;
        public string DropingContactNo = string.Empty;
    }

    public class TransportDetail
    {
        public string TransportName = string.Empty;
        public string ReportingTime = string.Empty;
        public string Address1 = string.Empty;
        public string Address2 = string.Empty;
        public string Address3 = string.Empty;
        public string CityNamePinCode = string.Empty;
        public string Phone = string.Empty;
        public string Fax = string.Empty;
        public string Website = string.Empty;
        public string Email = string.Empty;
    }

    public class BoardingDetail
    {
        public string BoardingAt = string.Empty;
        public string BoardingAddress = string.Empty;
        public string BoardingLandmark = string.Empty;
        public string BoardingTime = string.Empty;
        public string BoardingTelephoneno = string.Empty;
    }

    public class DroppingDetail
    {
        public string DroppingAt = string.Empty;
        public string DroppingAddress = string.Empty;
        public string DroppingLandmark = string.Empty;
        public string DroppingTime = string.Empty;
        public string DroppingTelephoneno = string.Empty;
    }

    public class PassengerDetail
    {
        public string PassengerName = string.Empty;
        public string TicketNumber = string.Empty;
        public string SeatType = string.Empty;
        public string SeatNo = string.Empty;
        public string Sex = string.Empty;
        public string Age = string.Empty;
        public string Fare = string.Empty;
        public string IsPrimary = string.Empty;
    }
}
