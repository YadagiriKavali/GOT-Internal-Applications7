using System;
using System.Configuration;
using System.Xml;
using IMI.Logger;
using martconnect.Models.Requests;
using martconnect.Models.Responses;
using meseva.Utilities;
using Newtonsoft.Json;

namespace martconnect
{
    public class MartConnectProcess
    {
        #region Public Methods

        public MCResponse GetBTBSourceCities(object reqData)
        {
            MCRequest request = null;

            try
            {
                request = JsonConvert.DeserializeObject<MCRequest>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MCResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var reqUrl = CacheConfig.GetServiceUrl(request.Service);
                var clientResp = WebRequestProcess.DoRequest(reqUrl, string.Empty, "GET", "application/x-www-form-urlencoded");

                LogData.Write("MARTCONNECT", "MARTCONNECT", LogMode.Info, string.Format("GetBTBSourceCities => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MCResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("errorcode");
                if (errorCodeNode != null)
                {
                    if (errorCodeNode.InnerText.Trim() == "0")
                    {
                        var dataNodes = root.SelectNodes("cities/city");
                        if (dataNodes != null && dataNodes.Count > 0)
                        {
                            var response = new BTBCitiesResp { ResCode = "000", ResDesc = "Success" };

                            foreach (XmlNode node in dataNodes)
                            {
                                response.Cities.Add(new Detail
                                {
                                    Code = node["citycode"] != null ? node["citycode"].InnerText.Trim() : string.Empty,
                                    Description = node["cityname"] != null ? node["cityname"].InnerText.Trim() : string.Empty
                                });
                            }

                            return response;
                        }
                    }
                    else
                    {
                        var errorCode = errorCodeNode.InnerText;
                        var errordesc = root.SelectSingleNode("errortext") != null ? root.SelectSingleNode("errortext").InnerText : string.Empty;

                        return new MCResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                    }
                }

                return new MCResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MARTCONNECT", "MARTCONNECT", LogMode.Excep, ex, string.Format("GetBTBSourceCities: Ex:{0}", ex.Message));
                return new MCResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MCResponse GetBTBDestinationCities(object reqData)
        {
            BTBDestinationCityReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<BTBDestinationCityReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MCResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var reqUrl = CacheConfig.GetServiceUrl(request.Service);
                if (string.IsNullOrEmpty(reqUrl))
                    return new MCResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                reqUrl = reqUrl.Replace("{CITYID}", request.CityId);

                var clientResp = WebRequestProcess.DoRequest(reqUrl, string.Empty, "GET", "application/x-www-form-urlencoded");

                LogData.Write("MARTCONNECT", "MARTCONNECT", LogMode.Info, string.Format("GetBTBDestinationCities => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MCResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("errorcode");
                if (errorCodeNode != null)
                {
                    if (errorCodeNode.InnerText.Trim() == "0")
                    {
                        var dataNodes = root.SelectNodes("cities/city");
                        if (dataNodes != null && dataNodes.Count > 0)
                        {
                            var response = new BTBCitiesResp { ResCode = "000", ResDesc = "Success" };

                            foreach (XmlNode node in dataNodes)
                            {
                                response.Cities.Add(new Detail
                                {
                                    Code = node["citycode"] != null ? node["citycode"].InnerText.Trim() : string.Empty,
                                    Description = node["cityname"] != null ? node["cityname"].InnerText.Trim() : string.Empty
                                });
                            }

                            return response;
                        }
                    }
                    else
                    {
                        var errorCode = errorCodeNode.InnerText;
                        var errordesc = root.SelectSingleNode("errortext") != null ? root.SelectSingleNode("errortext").InnerText : string.Empty;

                        return new MCResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                    }
                }

                return new MCResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MARTCONNECT", "MARTCONNECT", LogMode.Excep, ex, string.Format("GetBTBDestinationCities: Ex:{0}", ex.Message));
                return new MCResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MCResponse BTBSearchBuses(object reqData)
        {
            BTBSearchBusesReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<BTBSearchBusesReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MCResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var reqUrl = CacheConfig.GetServiceUrl(request.Service);
                if (string.IsNullOrEmpty(reqUrl))
                    return new MCResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                reqUrl = reqUrl.Replace("{SOURCECITYID}", request.SourceCityId);
                reqUrl = reqUrl.Replace("{DESTCITYID}", request.DestinationCityId);
                reqUrl = reqUrl.Replace("{DEPARTDATE}", request.DepartDate);

                var clientResp = WebRequestProcess.DoRequest(reqUrl, string.Empty, "GET", "application/x-www-form-urlencoded");

                LogData.Write("MARTCONNECT", "MARTCONNECT", LogMode.Info, string.Format("BTBSearchBuses => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MCResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("errorcode");
                if (errorCodeNode != null)
                {
                    if (errorCodeNode.InnerText.Trim() == "0")
                    {
                        var dataNodes = root.SelectNodes("BusDetails/BusDetail");
                        if (dataNodes != null && dataNodes.Count > 0)
                        {
                            var userTrackId = root.SelectSingleNode("BusDetails/UserTrackId") != null ? root.SelectSingleNode("BusDetails/UserTrackId").InnerText : string.Empty;
                            var response = new BTBSearchBusesResp { ResCode = "000", ResDesc = "Success", UserTrackId = userTrackId };

                            foreach (XmlNode node in dataNodes)
                            {
                                response.BusDetails.Add(new BusDetail
                                {
                                    ScheduleId = node["ScheduleId"] != null ? node["ScheduleId"].InnerText.Trim() : string.Empty,
                                    StationId = node["StationId"] != null ? node["StationId"].InnerText.Trim() : string.Empty,
                                    BusId = node["BusId"] != null ? node["BusId"].InnerText.Trim() : string.Empty,
                                    BusName = node["BusName"] != null ? node["BusName"].InnerText.Trim() : string.Empty,
                                    TransportId = node["TransportId"] != null ? node["TransportId"].InnerText.Trim() : string.Empty,
                                    TransportName = node["TransportName"] != null ? node["TransportName"].InnerText.Trim() : string.Empty,
                                    CoachTypeId = node["CoachTypeId"] != null ? node["CoachTypeId"].InnerText.Trim() : string.Empty,
                                    DepartureTime = node["DepartureTime"] != null ? node["DepartureTime"].InnerText.Trim() : string.Empty,
                                    ArrivalTime = node["ArrivalTime"] != null ? node["ArrivalTime"].InnerText.Trim() : string.Empty,
                                    SeatsAvailable = node["SeatsAvailable"] != null ? node["SeatsAvailable"].InnerText.Trim() : string.Empty,
                                    StatusId = node["StatusId"] != null ? node["StatusId"].InnerText.Trim() : string.Empty,
                                    StatusDesc = node["StatusDesc"] != null ? node["StatusDesc"].InnerText.Trim() : string.Empty,
                                    Fare = node["Fare"] != null ? node["Fare"].InnerText.Trim() : string.Empty,
                                    ServiceTax = node["ServiceTax"] != null ? node["ServiceTax"].InnerText.Trim() : string.Empty,
                                    SeatTypeId = node["SeatTypeId"] != null ? node["SeatTypeId"].InnerText.Trim() : string.Empty,
                                    SeatTypeName = node["SeatTypeName"] != null ? node["SeatTypeName"].InnerText.Trim() : string.Empty,
                                    TotalFare = node["TotalFare"] != null ? node["TotalFare"].InnerText.Trim() : string.Empty,
                                    CustomParamsRef = node["CustomParamsRef"] != null ? node["CustomParamsRef"].InnerText.Trim() : string.Empty,
                                    ReturnPolicy = node["ReturnPolicy"] != null ? node["ReturnPolicy"].InnerText.Trim() : string.Empty,
                                    PartialCancellation = node["PartialCancellation"] != null ? node["PartialCancellation"].InnerText.Trim() : string.Empty
                                });
                            }

                            return response;
                        }
                    }
                    else
                    {
                        var errorCode = errorCodeNode.InnerText;
                        var errordesc = root.SelectSingleNode("errortext") != null ? root.SelectSingleNode("errortext").InnerText : string.Empty;

                        return new MCResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                    }
                }

                return new MCResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MARTCONNECT", "MARTCONNECT", LogMode.Excep, ex, string.Format("BTBSearchBuses: Ex:{0}", ex.Message));
                return new MCResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MCResponse GetBTBSeatAvailabilities(object reqData)
        {
            BTBSeatAvailabilityReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<BTBSeatAvailabilityReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MCResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var reqUrl = CacheConfig.GetServiceUrl(request.Service);
                if (string.IsNullOrEmpty(reqUrl))
                    return new MCResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                reqUrl = reqUrl.Replace("{USERTRACKID}", request.UserTrackId);
                reqUrl = reqUrl.Replace("{SOURCECITY}", request.SourceCityId);
                reqUrl = reqUrl.Replace("{DESTINATIONCITY}", request.DestinationCityId);
                reqUrl = reqUrl.Replace("{DEPARTDATE}", request.DepartDate);
                reqUrl = reqUrl.Replace("{RETURNDATE}", request.ReturnDate);
                reqUrl = reqUrl.Replace("{SCHEDULEID}", request.ScheduleId);
                reqUrl = reqUrl.Replace("{STATIONID}", request.StationId);
                reqUrl = reqUrl.Replace("{TRANSPORTID}", request.TransportId);
                reqUrl = reqUrl.Replace("{TRANSPORTNAME}", request.TransportName);
                reqUrl = reqUrl.Replace("{SEATTYPEID}", request.SeatTypeId);
                reqUrl = reqUrl.Replace("{TRAVELDATE}", request.TravelDate);
                reqUrl = reqUrl.Replace("{CUSTOMPARAMSREF}", request.CustomParamsRef);

                var clientResp = WebRequestProcess.DoRequest(reqUrl, string.Empty, "GET", "application/x-www-form-urlencoded");

                LogData.Write("MARTCONNECT", "MARTCONNECT", LogMode.Info, string.Format("GetBTBSeatAvailabilities => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MCResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("errorcode");
                if (errorCodeNode != null)
                {
                    if (errorCodeNode.InnerText.Trim() == "0")
                    {
                        var response = new BTBSeatAvailabilityResp { ResCode = "000", ResDesc = "Success" };

                        var seatMapNodes = root.SelectNodes("SeatMap/Column");
                        if (seatMapNodes != null && seatMapNodes.Count > 0)
                        {
                            var seatMap = new SeatMap();
                            foreach (XmlNode columnNode in seatMapNodes)
                            {
                                var seatColumn = new SeatColumn();
                                foreach (XmlNode item in columnNode.SelectNodes("item"))
                                {
                                    if (item["SeatNo"] != null && string.IsNullOrEmpty(item["SeatNo"].InnerText))
                                        continue;

                                    seatColumn.Seats.Add(new Seat
                                    {
                                        SeatNo = item["SeatNo"] != null ? item["SeatNo"].InnerText.Trim() : string.Empty,
                                        SeatTypeId = item["SeatTypeId"] != null ? item["SeatTypeId"].InnerText.Trim() : string.Empty,
                                        SeatTypeName = item["SeatTypeName"] != null ? item["SeatTypeName"].InnerText.Trim() : string.Empty
                                    });
                                }

                                seatMap.SeatColumns.Add(seatColumn);
                            }

                            response.SeatMap = seatMap;
                        }

                        var bookedTicketNodes = root.SelectNodes("BookedTickets/item");
                        if (bookedTicketNodes != null && bookedTicketNodes.Count > 0)
                        {
                            foreach (XmlNode item in bookedTicketNodes)
                            {
                                if (item["BookedSeatNo"] != null && string.IsNullOrEmpty(item["BookedSeatNo"].InnerText))
                                    continue;

                                response.BookingTickets.Add(new BookingTicket
                                {
                                    BookedSeatNo = item["BookedSeatNo"] != null ? item["BookedSeatNo"].InnerText.Trim() : string.Empty,
                                    SeatTypeId = item["SeatTypeId"] != null ? item["SeatTypeId"].InnerText.Trim() : string.Empty,
                                    Gender = item["Gender"] != null ? item["Gender"].InnerText.Trim() : string.Empty
                                });
                            }
                        }

                        var availableTicketsNodes = root.SelectNodes("AvailableTickets/AvailbleSeatNo");
                        if (availableTicketsNodes != null && availableTicketsNodes.Count > 0)
                        {
                            foreach (XmlNode item in availableTicketsNodes)
                            {
                                if (item != null && string.IsNullOrEmpty(item.InnerText))
                                    continue;

                                response.AvailableSeats.Add(item.InnerText);
                            }
                        }

                        var boardingPointNodes = root.SelectNodes("BoardingPoints/Point");
                        if (boardingPointNodes != null && boardingPointNodes.Count > 0)
                        {
                            var seatColumn = new SeatColumn();
                            foreach (XmlNode item in boardingPointNodes)
                            {
                                if (item["BordingId"] != null && string.IsNullOrEmpty(item["BordingId"].InnerText))
                                    continue;

                                response.BordingPoints.Add(new BordingPoint
                                {
                                    BoardingId = item["BoardingId"] != null ? item["BoardingId"].InnerText.Trim() : string.Empty,
                                    BoardingPlace = item["BoardingPlace"] != null ? item["BoardingPlace"].InnerText.Trim() : string.Empty,
                                    BoardingTime = item["BoardingTime"] != null ? item["BoardingTime"].InnerText.Trim() : string.Empty,
                                    BoardingAddress = item["BoardingAddress"] != null ? item["BoardingAddress"].InnerText.Trim() : string.Empty,
                                    BoardingLandmark = item["BoardingLandmark"] != null ? item["BoardingLandmark"].InnerText.Trim() : string.Empty,
                                    BoardingContactNo = item["BoardingContactNo"] != null ? item["BoardingContactNo"].InnerText.Trim() : string.Empty
                                });
                            }
                        }

                        var dropingPointNodes = root.SelectNodes("DropingPoints/Point");
                        if (dropingPointNodes != null && dropingPointNodes.Count > 0)
                        {
                            var seatColumn = new SeatColumn();
                            foreach (XmlNode item in dropingPointNodes)
                            {
                                if (item["DropingId"] != null && string.IsNullOrEmpty(item["DropingId"].InnerText))
                                    continue;

                                response.DropingPoints.Add(new DropingPoint
                                {
                                    DropingId = item["DropingId"] != null ? item["DropingId"].InnerText.Trim() : string.Empty,
                                    DropingPlace = item["DropingPlace"] != null ? item["DropingPlace"].InnerText.Trim() : string.Empty,
                                    DropingTime = item["DropingTime"] != null ? item["DropingTime"].InnerText.Trim() : string.Empty,
                                    DropingAddress = item["DropingAddress"] != null ? item["DropingAddress"].InnerText.Trim() : string.Empty,
                                    DropingLandmark = item["DropingLandmark"] != null ? item["DropingLandmark"].InnerText.Trim() : string.Empty,
                                    DropingContactNo = item["DropingContactNo"] != null ? item["DropingContactNo"].InnerText.Trim() : string.Empty
                                });
                            }
                        }

                        response.CustomParamRef = root.SelectSingleNode("CustomParamsRef").InnerText;

                        return response;
                    }
                    else
                    {
                        var errorCode = errorCodeNode.InnerText;
                        var errordesc = root.SelectSingleNode("errortext") != null ? root.SelectSingleNode("errortext").InnerText : string.Empty;

                        return new MCResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                    }
                }

                return new MCResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MARTCONNECT", "MARTCONNECT", LogMode.Excep, ex, string.Format("GetBTBSeatAvailabilities: Ex:{0}", ex.Message));
                return new MCResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MCResponse BTBBookTicket(object reqData)
        {
            BTBookTicketReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<BTBookTicketReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MCResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var ticketCount = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["MAX_TICKET_COUNT"]) ? Convert.ToInt32(ConfigurationManager.AppSettings["MAX_TICKET_COUNT"]) : 6;
                if (request.Passengers.Count > ticketCount)
                    return new MCResponse { ResCode = "401", ResDesc = CacheConfig.GetErrorMessage("401") };

                var reqUrl = CacheConfig.GetServiceUrl(request.Service);
                if (string.IsNullOrEmpty(reqUrl))
                    return new MCResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                reqUrl = reqUrl.Replace("{USERTRACKID}", request.UserTrackId);
                reqUrl = reqUrl.Replace("{SCHEDULEID}", request.ScheduleId);
                reqUrl = reqUrl.Replace("{STATIONID}", request.StationId);
                reqUrl = reqUrl.Replace("{TRANSPORTID}", request.TransportId);
                reqUrl = reqUrl.Replace("{TRANSPORTNAME}", request.TransportName);
                reqUrl = reqUrl.Replace("{BUSID}", request.BusId);
                reqUrl = reqUrl.Replace("{BUSNAME}", request.BusName);
                reqUrl = reqUrl.Replace("{TRAVELDATE}", request.TravelDate);
                reqUrl = reqUrl.Replace("{ARRIVALTIME}", request.ArrivalTime);
                reqUrl = reqUrl.Replace("{DEPARTURETIME}", request.DepartureTime);
                reqUrl = reqUrl.Replace("{SOURCECITY}", request.SourceCity);
                reqUrl = reqUrl.Replace("{DESTINATIONCITY}", request.DestinationCity);
                reqUrl = reqUrl.Replace("{SOURCECITYNAME}", request.SourceCityName);
                reqUrl = reqUrl.Replace("{DESTINATIONCITYNAME}", request.DestinationCityName);
                reqUrl = reqUrl.Replace("{ADDRESS}", request.Address);
                reqUrl = reqUrl.Replace("{CITY}", request.City);
                reqUrl = reqUrl.Replace("{PINCODE}", request.Pincode);
                reqUrl = reqUrl.Replace("{COUNTRY}", request.Country);
                reqUrl = reqUrl.Replace("{MOBILE}", request.MobileNo);
                reqUrl = reqUrl.Replace("{EMAIL}", request.EmailId);
                reqUrl = reqUrl.Replace("{IDPROOFTYPE}", request.IDProofType);
                reqUrl = reqUrl.Replace("{IDPROOFNUMBER}", request.IDProofNumber);
                reqUrl = reqUrl.Replace("{BOARDINGPOINT}", request.BoardingPoint);
                reqUrl = reqUrl.Replace("{BOARDINGPOINTNAME}", request.BoardingPointName);
                reqUrl = reqUrl.Replace("{DROPPOINT}", request.DropPoint);
                reqUrl = reqUrl.Replace("{DROPPOINTNAME}", request.DropPointName);
                reqUrl = reqUrl.Replace("{FARE}", request.Fare);
                reqUrl = reqUrl.Replace("{COACHTYPEID}", request.CoachTypeId);
                reqUrl = reqUrl.Replace("{SEATTYPEID}", request.SeatTypeId);
                reqUrl = reqUrl.Replace("{SEATTYPENAME}", request.SeatTypeName);
                reqUrl = reqUrl.Replace("{CUSTOMPARAMSREF}", request.CustomParamsRef);
                reqUrl = reqUrl.Replace("{RETRYBOOKING}", request.RetryBooking);

                var passengerStr = "&PassengerSeatTypeId_{0}={1}&PassengerSeatNo_{0}={2}&PassengerTitle_{0}={3}&PassengerName_{0}={4}&PassengerAge_{0}={5}&PassengerPrimary_{0}={6}";
                for (int i = 0; i < request.Passengers.Count; i++)
                {
                    var passenger = request.Passengers[i];
                    reqUrl += string.Format(passengerStr, i, passenger.SeatTypeId, passenger.SeatNo, passenger.Title, passenger.Name, passenger.Age, passenger.Primary);
                }

                var clientResp = WebRequestProcess.DoRequest(reqUrl, string.Empty, "GET", "application/x-www-form-urlencoded");

                LogData.Write("MARTCONNECT", "MARTCONNECT", LogMode.Info, string.Format("BTBBookTicket => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MCResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("errorcode");
                if (errorCodeNode != null)
                {
                    if (errorCodeNode.InnerText.Trim() == "0")
                    {
                        var response = new BookedTicketResp
                        {
                            ResCode = "000",
                            ResDesc = "Success",
                            UserTrackId = root.SelectSingleNode("UserTrackId") != null ? root.SelectSingleNode("UserTrackId").InnerText.Trim() : string.Empty,
                            TransactionId = root.SelectSingleNode("Transactionid") != null ? root.SelectSingleNode("Transactionid").InnerText.Trim() : string.Empty,
                            TransportPNR = root.SelectSingleNode("TransportPNR") != null ? root.SelectSingleNode("TransportPNR").InnerText.Trim() : string.Empty,
                            TransportName = root.SelectSingleNode("Transportname") != null ? root.SelectSingleNode("Transportname").InnerText.Trim() : string.Empty,
                            BusName = root.SelectSingleNode("Busname") != null ? root.SelectSingleNode("Busname").InnerText.Trim() : string.Empty,
                            CoachName = root.SelectSingleNode("Coachname") != null ? root.SelectSingleNode("Coachname").InnerText.Trim() : string.Empty,
                            OriginName = root.SelectSingleNode("originame") != null ? root.SelectSingleNode("originame").InnerText.Trim() : string.Empty,
                            DestinationName = root.SelectSingleNode("Destinationname") != null ? root.SelectSingleNode("Destinationname").InnerText.Trim() : string.Empty,
                            TravelDate = root.SelectSingleNode("Traveldate") != null ? root.SelectSingleNode("Traveldate").InnerText.Trim() : string.Empty,
                            DepartureTime = root.SelectSingleNode("Departuretime") != null ? root.SelectSingleNode("Departuretime").InnerText.Trim() : string.Empty,
                            TravelRelatedQueries = root.SelectSingleNode("TravelRelatedQueries") != null ? root.SelectSingleNode("TravelRelatedQueries").InnerText.Trim() : string.Empty
                        };

                        response.TransportDetail = new TransportDetail
                        {
                            TransportName = root.SelectSingleNode("TransportDetails/Transportname") != null ? root.SelectSingleNode("TransportDetails/Transportname").InnerText.Trim() : string.Empty,
                            ReportingTime = root.SelectSingleNode("TransportDetails/Reportingtime") != null ? root.SelectSingleNode("TransportDetails/Reportingtime").InnerText.Trim() : string.Empty,
                            Address1 = root.SelectSingleNode("TransportDetails/Address1") != null ? root.SelectSingleNode("TransportDetails/Address1").InnerText.Trim() : string.Empty,
                            Address2 = root.SelectSingleNode("TransportDetails/Address2") != null ? root.SelectSingleNode("TransportDetails/Address2").InnerText.Trim() : string.Empty,
                            Address3 = root.SelectSingleNode("TransportDetails/Address3") != null ? root.SelectSingleNode("TransportDetails/Address3").InnerText.Trim() : string.Empty,
                            CityNamePinCode = root.SelectSingleNode("TransportDetails/CityNamePinCode") != null ? root.SelectSingleNode("TransportDetails/CityNamePinCode").InnerText.Trim() : string.Empty,
                            Phone = root.SelectSingleNode("TransportDetails/Phone") != null ? root.SelectSingleNode("TransportDetails/Phone").InnerText.Trim() : string.Empty,
                            Fax = root.SelectSingleNode("TransportDetails/Fax") != null ? root.SelectSingleNode("TransportDetails/Fax").InnerText.Trim() : string.Empty,
                            Website = root.SelectSingleNode("TransportDetails/Website") != null ? root.SelectSingleNode("TransportDetails/Website").InnerText.Trim() : string.Empty,
                            Email = root.SelectSingleNode("TransportDetails/Email") != null ? root.SelectSingleNode("TransportDetails/Email").InnerText.Trim() : string.Empty
                        };

                        response.BoardingDetail = new BoardingDetail
                        {
                            BoardingAt = root.SelectSingleNode("BoardingDetails/Boardingat") != null ? root.SelectSingleNode("BoardingDetails/Boardingat").InnerText.Trim() : string.Empty,
                            BoardingAddress = root.SelectSingleNode("BoardingDetails/Boardingaddress") != null ? root.SelectSingleNode("BoardingDetails/Boardingaddress").InnerText.Trim() : string.Empty,
                            BoardingLandmark = root.SelectSingleNode("BoardingDetails/BoardingLandmark") != null ? root.SelectSingleNode("BoardingDetails/BoardingLandmark").InnerText.Trim() : string.Empty,
                            BoardingTime = root.SelectSingleNode("BoardingDetails/BoardingTime") != null ? root.SelectSingleNode("BoardingDetails/BoardingTime").InnerText.Trim() : string.Empty,
                            BoardingTelephoneno = root.SelectSingleNode("BoardingDetails/BoardingTelephoneno") != null ? root.SelectSingleNode("BoardingDetails/BoardingTelephoneno").InnerText.Trim() : string.Empty
                        };

                        response.DroppingDetail = new DroppingDetail
                        {
                            DroppingAt = root.SelectSingleNode("DropingDetails/Droppingat") != null ? root.SelectSingleNode("DropingDetails/Droppingat").InnerText.Trim() : string.Empty,
                            DroppingAddress = root.SelectSingleNode("DropingDetails/Droppingaddress") != null ? root.SelectSingleNode("DropingDetails/Droppingaddress").InnerText.Trim() : string.Empty,
                            DroppingLandmark = root.SelectSingleNode("DropingDetails/DroppingLandmark") != null ? root.SelectSingleNode("DropingDetails/DroppingLandmark").InnerText.Trim() : string.Empty,
                            DroppingTime = root.SelectSingleNode("DropingDetails/DroppingTime") != null ? root.SelectSingleNode("DropingDetails/DroppingTime").InnerText.Trim() : string.Empty,
                            DroppingTelephoneno = root.SelectSingleNode("DropingDetails/DroppingTelephoneno") != null ? root.SelectSingleNode("DropingDetails/DroppingTelephoneno").InnerText.Trim() : string.Empty
                        };

                        var passengers = root.SelectNodes("Passengers/Passenger");
                        foreach (XmlNode passenger in passengers)
                        {
                            response.Passengers.Add(new PassengerDetail
                            {
                                PassengerName = passenger["PassengerName"] != null ? passenger["PassengerName"].InnerText.Trim() : string.Empty,
                                TicketNumber = passenger["TicketNumber"] != null ? passenger["TicketNumber"].InnerText.Trim() : string.Empty,
                                SeatType = passenger["SeatType"] != null ? passenger["SeatType"].InnerText.Trim() : string.Empty,
                                SeatNo = passenger["SeatNo"] != null ? passenger["SeatNo"].InnerText.Trim() : string.Empty,
                                Sex = passenger["Sex"] != null ? passenger["Sex"].InnerText.Trim() : string.Empty,
                                Age = passenger["Age"] != null ? passenger["Age"].InnerText.Trim() : string.Empty,
                                Fare = passenger["Fare"] != null ? passenger["Fare"].InnerText.Trim() : string.Empty,
                                IsPrimary = passenger["isPrimary"] != null ? passenger["isPrimary"].InnerText.Trim() : string.Empty
                            });
                        }

                        return response;
                    }
                    else
                    {
                        var errorCode = errorCodeNode.InnerText;
                        var errordesc = root.SelectSingleNode("errortext") != null ? root.SelectSingleNode("errortext").InnerText : string.Empty;

                        return new MCResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                    }
                }

                return new MCResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MARTCONNECT", "MARTCONNECT", LogMode.Excep, ex, string.Format("BTBBookTicket: Ex:{0}", ex.Message));
                return new MCResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MCResponse BTBCancelBookedTicket(object reqData)
        {
            BTCancelBookedTicketReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<BTCancelBookedTicketReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MCResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var reqUrl = CacheConfig.GetServiceUrl(request.Service);
                if (string.IsNullOrEmpty(reqUrl))
                    return new MCResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                reqUrl = reqUrl.Replace("{TICKETBOOKINGID}", request.TicketBookingId);
                reqUrl = reqUrl.Replace("{TICKETNUMBER}", request.TicketNumber);
                reqUrl = reqUrl.Replace("{BENIFSCCODE}", request.IFSCCode);

                var clientResp = WebRequestProcess.DoRequest(reqUrl, string.Empty, "GET", "application/x-www-form-urlencoded");
                LogData.Write("MARTCONNECT", "MARTCONNECT", LogMode.Info, string.Format("BTBCancelBookedTicket => Response: {0}", clientResp));

                if (string.IsNullOrEmpty(clientResp))
                    return new MCResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("errorcode");
                if (errorCodeNode != null)
                {
                    if (errorCodeNode.InnerText.Trim() == "0")
                        return new BTBCitiesResp { ResCode = "000", ResDesc = "Success" };

                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("errortext") != null ? root.SelectSingleNode("errortext").InnerText : string.Empty;

                    return new MCResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MCResponse { ResCode = "302", ResDesc = CacheConfig.GetErrorMessage("302") };
            }
            catch (Exception ex)
            {
                LogData.Write("MARTCONNECT", "MARTCONNECT", LogMode.Excep, ex, string.Format("BTBCancelBookedTicket: Ex:{0}", ex.Message));
                return new MCResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion Public Methods
    }
}
