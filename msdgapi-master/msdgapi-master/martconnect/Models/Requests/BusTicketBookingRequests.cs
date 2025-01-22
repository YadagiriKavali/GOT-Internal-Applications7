using System.Collections.Generic;

namespace martconnect.Models.Requests
{
    public class BTBDestinationCityReq : MCRequest
    {
        public string CityId = string.Empty;
    }

    public class BTBSearchBusesReq : MCRequest
    {
        public string SourceCityId = string.Empty;
        public string DestinationCityId = string.Empty;
        public string DepartDate = string.Empty;
    }

    public class BTBSeatAvailabilityReq : MCRequest
    {
        public string UserTrackId = string.Empty;
        public string SourceCityId = string.Empty;
        public string DestinationCityId = string.Empty;
        public string DepartDate = string.Empty;
        public string ReturnDate = string.Empty;
        public string ScheduleId = string.Empty;
        public string StationId = string.Empty;
        public string TransportId = string.Empty;
        public string TransportName = string.Empty;
        public string SeatTypeId = string.Empty;
        public string TravelDate = string.Empty;
        public string CustomParamsRef = string.Empty;
    }

    public class BTBookTicketReq : MCRequest
    {
        public string UserTrackId = string.Empty;
        public string ScheduleId = string.Empty;
        public string StationId = string.Empty;
        public string TransportId = string.Empty;
        public string TransportName = string.Empty;
        public string BusId = string.Empty;
        public string BusName = string.Empty;
        public string TravelDate = string.Empty;
        public string ArrivalTime = string.Empty;
        public string DepartureTime = string.Empty;
        public string SourceCity = string.Empty;
        public string DestinationCity = string.Empty;
        public string SourceCityName = string.Empty;
        public string DestinationCityName = string.Empty;
        public string Address = string.Empty;
        public string City = string.Empty;
        public string Pincode = string.Empty;
        public string Country = string.Empty;
        public string EmailId = string.Empty;
        public string IDProofType = string.Empty;
        public string IDProofNumber = string.Empty;
        public string BoardingPoint = string.Empty;
        public string BoardingPointName = string.Empty;
        public string DropPoint = string.Empty;
        public string DropPointName = string.Empty;
        public string Fare = string.Empty;
        public string CoachTypeId = string.Empty;
        public string SeatTypeId = string.Empty;
        public string SeatTypeName = string.Empty;        
        public string CustomParamsRef = string.Empty;
        public string RetryBooking = string.Empty;
        public List<Passenger> Passengers { get; set; }
    }

    public class BTCancelBookedTicketReq : MCRequest
    {
        public string TicketBookingId = string.Empty;
        public string TicketNumber = string.Empty;
        public string IFSCCode = string.Empty;
    }

    public class Passenger
    {
        public string SeatTypeId = string.Empty;
        public string SeatNo = string.Empty;
        public string Title = string.Empty;
        public string Name = string.Empty;
        public string Age = string.Empty;
        public string Primary = string.Empty;
    }
}
