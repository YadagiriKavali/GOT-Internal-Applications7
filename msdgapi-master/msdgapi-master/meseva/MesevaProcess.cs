using System;
using System.Linq;
using System.Xml;
using IMI.Logger;
using meseva.MesevaService;
using meseva.models.Requests;
using meseva.models.Responses;
using Newtonsoft.Json;

namespace meseva
{
    public class MesevaProcess
    {
        #region Private Members

        private MeesevaMobileWebserviceSoapClient mesevaClient;

        #endregion Private Members

        #region Constructors

        public MesevaProcess()
        {
            CacheConfig.LoadAuthDetails();
            mesevaClient = new MeesevaMobileWebserviceSoapClient("MeesevaMobileWebserviceSoap");
        }

        #endregion Constructors

        #region Public Methods

        #region [ COMMON SERVICES ]

        public MSResponse GenerateApplicationNo(object reqData)
        {
            ServiceBasedReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<ServiceBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GenerateApplicationNo(CacheConfig.UserName, CacheConfig.Password, request.MobileNo, CacheConfig.GetServiceId(request.Service));
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GenerateApplicationNo => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var response = new ApplicationNoResp { ResCode = "000", ResDesc = "Success", ApplicationNo = root.SelectSingleNode("AppNo/ApplicationNo").InnerText };
                var dataNodes = root.SelectNodes("Documents");
                foreach (XmlNode node in dataNodes)
                {
                    response.Documents.Add(new ApplicationDocument
                    {
                        DocId = node["DocId"] != null ? node["DocId"].InnerText.Trim() : string.Empty,
                        DocName = node["DocName"] != null ? node["DocName"].InnerText.Trim() : string.Empty,
                        Mandatory = node["Mandatory"] != null ? node["Mandatory"].InnerText.Trim() : string.Empty
                    });
                }

                return response;
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GenerateApplicationNo: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateDistrict(object reqData)
        {
            try
            {
                var clientResp = mesevaClient.PopulateDistrict(CacheConfig.UserName, CacheConfig.Password);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateDistrict => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("dtDistrict");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new DistrictsResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.Districts.Add(new Detail
                        {
                            Code = node["district_id"] != null ? node["district_id"].InnerText.Trim() : string.Empty,
                            Description = node["district_description"] != null ? node["district_description"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateDistrict: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateMandal(object reqData)
        {
            MandalReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<MandalReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.PopulateMandal(request.DistrictId, CacheConfig.UserName, CacheConfig.Password);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateMandal => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };



                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("dtMandal");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new MandalsResp { ResCode = "000", ResDesc = "Success" };
                    foreach (XmlNode node in dataNodes)
                    {
                        response.Mandals.Add(new Detail
                        {
                            Code = node["mandal_id"] != null ? node["mandal_id"].InnerText.Trim() : string.Empty,
                            Description = node["mandal_description"] != null ? node["mandal_description"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateMandal: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateVillage(object reqData)
        {
            VillageReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<VillageReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.PopulateVillage(request.DistrictId, request.MandalId, CacheConfig.UserName, CacheConfig.Password);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateVillage => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("dtVillage");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new VillagesResp { ResCode = "000", ResDesc = "Success" };
                    foreach (XmlNode node in dataNodes)
                    {
                        response.Villages.Add(new Detail
                        {
                            Code = node["village_id"] != null ? node["village_id"].InnerText.Trim() : string.Empty,
                            Description = node["village_name"] != null ? node["village_name"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateVillage: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateRevenueDistrict(object reqData)
        {
            try
            {
                var clientResp = mesevaClient.PopulateRevenueDistrict(CacheConfig.UserName, CacheConfig.Password);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateRevenueDistrict => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("dtDistrict");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new DistrictsResp { ResCode = "000", ResDesc = "Success" };
                    foreach (XmlNode node in dataNodes)
                    {
                        response.Districts.Add(new Detail
                        {
                            Code = node["dist_code"] != null ? node["dist_code"].InnerText.Trim() : string.Empty,
                            Description = node["dist_name"] != null ? node["dist_name"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateRevenueDistrict: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateRevenueMandal(object reqData)
        {
            MandalReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<MandalReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.PopulateRevenueMandal(CacheConfig.UserName, CacheConfig.Password, request.DistrictId);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateRevenueMandal => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };



                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("dtMandal");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new MandalsResp { ResCode = "000", ResDesc = "Success" };
                    foreach (XmlNode node in dataNodes)
                    {
                        response.Mandals.Add(new Detail
                        {
                            Code = node["mandal_id"] != null ? node["mandal_id"].InnerText.Trim() : string.Empty,
                            Description = node["mandal_description"] != null ? node["mandal_description"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateRevenueMandal: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateRevenueVillage(object reqData)
        {
            VillageReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<VillageReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.PopulateRevenueVillage(CacheConfig.UserName, CacheConfig.Password, request.DistrictId, request.MandalId);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateRevenueVillage => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("dtVillage");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new VillagesResp { ResCode = "000", ResDesc = "Success" };
                    foreach (XmlNode node in dataNodes)
                    {
                        response.Villages.Add(new Detail
                        {
                            Code = node["Village_ID"] != null ? node["Village_ID"].InnerText.Trim() : string.Empty,
                            Description = node["village_name"] != null ? node["village_name"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateRevenueVillage: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetReceiptBasedOnTransNo(object reqData)
        {
            ReceiptBasedOnTransNoReq request = null;
            try
            {
                request = JsonConvert.DeserializeObject<ReceiptBasedOnTransNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetReceiptBasedOnTransNo(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service), request.TransactionNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetReceiptBasedOnTransNo => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                MSResponse response;
                switch (CacheConfig.GetServiceId(request.Service))
                {
                    case "801":
                    case "806":
                    case "813":
                    case "821":
                    case "822":
                    case "823":
                    case "1500":
                        response = new IncomeReceiptBasedOnTransNoResp
                        {
                            ResCode = "000",
                            ResDesc = "Success",
                            TransID = root.SelectSingleNode("Table/TransID") != null ? root.SelectSingleNode("Table/TransID").InnerText.Trim() : string.Empty,
                            ApplicationNumber = root.SelectSingleNode("Table/ApplicationNumber") != null ? root.SelectSingleNode("Table/ApplicationNumber").InnerText.Trim() : string.Empty,
                            TransDate = root.SelectSingleNode("Table/TransDate") != null ? root.SelectSingleNode("Table/TransDate").InnerText.Trim() : string.Empty,
                            ApplicantName = root.SelectSingleNode("Table/ApplicantName") != null ? root.SelectSingleNode("Table/ApplicantName").InnerText.Trim() : string.Empty,
                            DeliveryType = root.SelectSingleNode("Table/DeliveryType") != null ? root.SelectSingleNode("Table/DeliveryType").InnerText.Trim() : string.Empty,
                            FatherName = root.SelectSingleNode("Table/FatherName") != null ? root.SelectSingleNode("Table/FatherName").InnerText.Trim() : string.Empty,
                            ServiceName = root.SelectSingleNode("Table/ServiceName") != null ? root.SelectSingleNode("Table/ServiceName").InnerText.Trim() : string.Empty,
                            LoginID = root.SelectSingleNode("Table/LoginID") != null ? root.SelectSingleNode("Table/LoginID").InnerText.Trim() : string.Empty,
                            SLA = root.SelectSingleNode("Table/SLA") != null ? root.SelectSingleNode("Table/SLA").InnerText.Trim() : string.Empty
                        };
                        break;
                    case "802":
                        response = new AdangalReceiptBasedOnTransNoResp
                        {
                            ResCode = "000",
                            ResDesc = "Success",
                            TransID = root.SelectSingleNode("Table/TransID") != null ? root.SelectSingleNode("Table/TransID").InnerText.Trim() : string.Empty,
                            ApplicationNumber = root.SelectSingleNode("Table/ApplicationNumber") != null ? root.SelectSingleNode("Table/ApplicationNumber").InnerText.Trim() : string.Empty,
                            TransDate = root.SelectSingleNode("Table/TransDate") != null ? root.SelectSingleNode("Table/TransDate").InnerText.Trim() : string.Empty,
                            ApplicantName = root.SelectSingleNode("Table/ApplicantName") != null ? root.SelectSingleNode("Table/ApplicantName").InnerText.Trim() : string.Empty,
                            DocumentSurveyNo = root.SelectSingleNode("Table/DocumentSurveyNo") != null ? root.SelectSingleNode("Table/DocumentSurveyNo").InnerText.Trim() : string.Empty,
                            DocumentDistrict = root.SelectSingleNode("Table/DocumentDistrict") != null ? root.SelectSingleNode("Table/DocumentDistrict").InnerText.Trim() : string.Empty,
                            DocumentMandal = root.SelectSingleNode("Table/DocumentMandal") != null ? root.SelectSingleNode("Table/DocumentMandal").InnerText.Trim() : string.Empty,
                            DocumentVillage = root.SelectSingleNode("Table/DocumentVillage") != null ? root.SelectSingleNode("Table/DocumentVillage").InnerText.Trim() : string.Empty,
                            ServiceName = root.SelectSingleNode("Table/ServiceName") != null ? root.SelectSingleNode("Table/ServiceName").InnerText.Trim() : string.Empty,
                            FasliYear = root.SelectSingleNode("Table/FasliYear") != null ? root.SelectSingleNode("Table/FasliYear").InnerText.Trim() : string.Empty,
                            LoginID = root.SelectSingleNode("Table/LoginID") != null ? root.SelectSingleNode("Table/LoginID").InnerText.Trim() : string.Empty,
                            SLA = root.SelectSingleNode("Table/SLA") != null ? root.SelectSingleNode("Table/SLA").InnerText.Trim() : string.Empty
                        };
                        break;
                    case "827":
                        response = new EBCReceiptBasedOnTransNoResp
                        {
                            ResCode = "000",
                            ResDesc = "Success",
                            TransID = root.SelectSingleNode("Table/TransID") != null ? root.SelectSingleNode("Table/TransID").InnerText.Trim() : string.Empty,
                            ApplicationNumber = root.SelectSingleNode("Table/ApplicationNumber") != null ? root.SelectSingleNode("Table/ApplicationNumber").InnerText.Trim() : string.Empty,
                            TransDate = root.SelectSingleNode("Table/TransDate") != null ? root.SelectSingleNode("Table/TransDate").InnerText.Trim() : string.Empty,
                            ApplicantName = root.SelectSingleNode("Table/ApplicantName") != null ? root.SelectSingleNode("Table/ApplicantName").InnerText.Trim() : string.Empty,
                            CasteClaimed = root.SelectSingleNode("Table/CasteClaimed") != null ? root.SelectSingleNode("Table/CasteClaimed").InnerText.Trim() : string.Empty,
                            ServiceName = root.SelectSingleNode("Table/ServiceName") != null ? root.SelectSingleNode("Table/ServiceName").InnerText.Trim() : string.Empty,
                            LoginID = root.SelectSingleNode("Table/LoginID") != null ? root.SelectSingleNode("Table/LoginID").InnerText.Trim() : string.Empty,
                            SLA = root.SelectSingleNode("Table/SLA") != null ? root.SelectSingleNode("Table/SLA").InnerText.Trim() : string.Empty
                        };
                        break;
                    case "828":
                        response = new OBCReceiptBasedOnTransNoResp
                        {
                            ResCode = "000",
                            ResDesc = "Success",
                            TransID = root.SelectSingleNode("Table/TransID") != null ? root.SelectSingleNode("Table/TransID").InnerText.Trim() : string.Empty,
                            ApplicationNumber = root.SelectSingleNode("Table/ApplicationNumber") != null ? root.SelectSingleNode("Table/ApplicationNumber").InnerText.Trim() : string.Empty,
                            TransDate = root.SelectSingleNode("Table/TransDate") != null ? root.SelectSingleNode("Table/TransDate").InnerText.Trim() : string.Empty,
                            ApplicantName = root.SelectSingleNode("Table/ApplicantName") != null ? root.SelectSingleNode("Table/ApplicantName").InnerText.Trim() : string.Empty,
                            DeliveryType = root.SelectSingleNode("Table/DeliveryType") != null ? root.SelectSingleNode("Table/DeliveryType").InnerText.Trim() : string.Empty,
                            FatherName = root.SelectSingleNode("Table/FatherName") != null ? root.SelectSingleNode("Table/FatherName").InnerText.Trim() : string.Empty,
                            CasteClaimed = root.SelectSingleNode("Table/CasteClaimed") != null ? root.SelectSingleNode("Table/CasteClaimed").InnerText.Trim() : string.Empty,
                            ServiceName = root.SelectSingleNode("Table/ServiceName") != null ? root.SelectSingleNode("Table/ServiceName").InnerText.Trim() : string.Empty,
                            LoginID = root.SelectSingleNode("Table/LoginID") != null ? root.SelectSingleNode("Table/LoginID").InnerText.Trim() : string.Empty,
                            SLA = root.SelectSingleNode("Table/SLA") != null ? root.SelectSingleNode("Table/SLA").InnerText.Trim() : string.Empty
                        };
                        break;
                    case "805":
                        response = new ResidenceReceiptBasedOnTransNoResp
                        {
                            ResCode = "000",
                            ResDesc = "Success",
                            TransID = root.SelectSingleNode("Table/TransID") != null ? root.SelectSingleNode("Table/TransID").InnerText.Trim() : string.Empty,
                            ApplicationNumber = root.SelectSingleNode("Table/ApplicationNumber") != null ? root.SelectSingleNode("Table/ApplicationNumber").InnerText.Trim() : string.Empty,
                            TransDate = root.SelectSingleNode("Table/TransDate") != null ? root.SelectSingleNode("Table/TransDate").InnerText.Trim() : string.Empty,
                            ApplicantName = root.SelectSingleNode("Table/ApplicantName") != null ? root.SelectSingleNode("Table/ApplicantName").InnerText.Trim() : string.Empty,
                            DeliveryType = root.SelectSingleNode("Table/DeliveryType") != null ? root.SelectSingleNode("Table/DeliveryType").InnerText.Trim() : string.Empty,
                            FatherName = root.SelectSingleNode("Table/FatherName") != null ? root.SelectSingleNode("Table/FatherName").InnerText.Trim() : string.Empty,
                            ServiceName = root.SelectSingleNode("Table/ServiceName") != null ? root.SelectSingleNode("Table/ServiceName").InnerText.Trim() : string.Empty,
                            LoginID = root.SelectSingleNode("Table/LoginID") != null ? root.SelectSingleNode("Table/LoginID").InnerText.Trim() : string.Empty,
                            SLA = root.SelectSingleNode("Table/SLA") != null ? root.SelectSingleNode("Table/SLA").InnerText.Trim() : string.Empty
                        };
                        break;
                    case "800":
                    case "803":
                        response = new FMBCopyReceiptBasedOnTransNoResp
                        {
                            ResCode = "000",
                            ResDesc = "Success",
                            TransID = root.SelectSingleNode("Table/TransID") != null ? root.SelectSingleNode("Table/TransID").InnerText.Trim() : string.Empty,
                            ApplicationNumber = root.SelectSingleNode("Table/ApplicationNumber") != null ? root.SelectSingleNode("Table/ApplicationNumber").InnerText.Trim() : string.Empty,
                            TransDate = root.SelectSingleNode("Table/TransDate") != null ? root.SelectSingleNode("Table/TransDate").InnerText.Trim() : string.Empty,
                            ApplicantName = root.SelectSingleNode("Table/ApplicantName") != null ? root.SelectSingleNode("Table/ApplicantName").InnerText.Trim() : string.Empty,
                            DeliveryType = root.SelectSingleNode("Table/DeliveryType") != null ? root.SelectSingleNode("Table/DeliveryType").InnerText.Trim() : string.Empty,
                            FatherName = root.SelectSingleNode("Table/FatherName") != null ? root.SelectSingleNode("Table/FatherName").InnerText.Trim() : string.Empty,
                            CasteClaimed = root.SelectSingleNode("Table/CasteClaimed") != null ? root.SelectSingleNode("Table/CasteClaimed").InnerText.Trim() : string.Empty,
                            ServiceName = root.SelectSingleNode("Table/ServiceName") != null ? root.SelectSingleNode("Table/ServiceName").InnerText.Trim() : string.Empty,
                            LoginID = root.SelectSingleNode("Table/LoginID") != null ? root.SelectSingleNode("Table/LoginID").InnerText.Trim() : string.Empty,
                            SLA = root.SelectSingleNode("Table/SLA") != null ? root.SelectSingleNode("Table/SLA").InnerText.Trim() : string.Empty
                        };
                        break;
                    case "830":
                    case "930":
                    case "936":
                        response = new FMReceiptBasedOnTransNoResp
                        {
                            ResCode = "000",
                            ResDesc = "Success",
                            TransID = root.SelectSingleNode("Table/TransID") != null ? root.SelectSingleNode("Table/TransID").InnerText.Trim() : string.Empty,
                            ApplicationNumber = root.SelectSingleNode("Table/ApplicationNumber") != null ? root.SelectSingleNode("Table/ApplicationNumber").InnerText.Trim() : string.Empty,
                            TransDate = root.SelectSingleNode("Table/TransDate") != null ? root.SelectSingleNode("Table/TransDate").InnerText.Trim() : string.Empty,
                            ApplicantName = root.SelectSingleNode("Table/ApplicantName") != null ? root.SelectSingleNode("Table/ApplicantName").InnerText.Trim() : string.Empty,
                            DeliveryType = root.SelectSingleNode("Table/DeliveryType") != null ? root.SelectSingleNode("Table/DeliveryType").InnerText.Trim() : string.Empty,
                            FatherName = root.SelectSingleNode("Table/FatherName") != null ? root.SelectSingleNode("Table/FatherName").InnerText.Trim() : string.Empty,
                            ServiceName = root.SelectSingleNode("Table/ServiceName") != null ? root.SelectSingleNode("Table/ServiceName").InnerText.Trim() : string.Empty,
                            LoginID = root.SelectSingleNode("Table/LoginID") != null ? root.SelectSingleNode("Table/LoginID").InnerText.Trim() : string.Empty,
                            SLA = root.SelectSingleNode("Table/SLA") != null ? root.SelectSingleNode("Table/SLA").InnerText.Trim() : string.Empty
                        };
                        break;
                    case "709":
                    case "717":
                    case "790":
                        response = new TempleReceiptBasedOnTransNoResp
                        {
                            ResCode = "000",
                            ResDesc = "Success",
                            TransID = root.SelectSingleNode("Table/TransID") != null ? root.SelectSingleNode("Table/TransID").InnerText.Trim() : string.Empty,
                            ApplicationNumber = root.SelectSingleNode("Table/ApplicationNumber") != null ? root.SelectSingleNode("Table/ApplicationNumber").InnerText.Trim() : string.Empty,
                            TransDate = root.SelectSingleNode("Table/TransDate") != null ? root.SelectSingleNode("Table/TransDate").InnerText.Trim() : string.Empty,
                            SevaName = root.SelectSingleNode("Table/SevaName") != null ? root.SelectSingleNode("Table/SevaName").InnerText.Trim() : string.Empty,
                            SevaType = root.SelectSingleNode("Table/SevaType") != null ? root.SelectSingleNode("Table/SevaType").InnerText.Trim() : string.Empty,
                            SevaDate = root.SelectSingleNode("Table/Seva_Date") != null ? root.SelectSingleNode("Table/Seva_Date").InnerText.Trim() : string.Empty,
                            ReportingTime = root.SelectSingleNode("Table/ReportingTime") != null ? root.SelectSingleNode("Table/ReportingTime").InnerText.Trim() : string.Empty,
                            DevoteeName = root.SelectSingleNode("Table/DevoteeName") != null ? root.SelectSingleNode("Table/DevoteeName").InnerText.Trim() : string.Empty,
                            InformantName = root.SelectSingleNode("Table/InformantName") != null ? root.SelectSingleNode("Table/InformantName").InnerText.Trim() : string.Empty,
                            SevaAmount = root.SelectSingleNode("Table/SevaAmount") != null ? root.SelectSingleNode("Table/SevaAmount").InnerText.Trim() : string.Empty,
                            UserCharge = root.SelectSingleNode("Table/UserCharges") != null ? root.SelectSingleNode("Table/UserCharges").InnerText.Trim() : string.Empty,
                            TotalAmount = root.SelectSingleNode("Table/TotalAmount") != null ? root.SelectSingleNode("Table/TotalAmount").InnerText.Trim() : string.Empty,
                            LoginID = root.SelectSingleNode("Table/LoginID") != null ? root.SelectSingleNode("Table/LoginID").InnerText.Trim() : string.Empty
                        };
                        break;
                    case "719":
                    case "3011":
                        response = new RoomBookingReceiptBasedOnTransNoResp
                        {
                            ResCode = "000",
                            ResDesc = "Success",
                            TransID = root.SelectSingleNode("Table/TransID") != null ? root.SelectSingleNode("Table/TransID").InnerText.Trim() : string.Empty,
                            ApplicationNumber = root.SelectSingleNode("Table/ApplicationNumber") != null ? root.SelectSingleNode("Table/ApplicationNumber").InnerText.Trim() : string.Empty,
                            TransDate = root.SelectSingleNode("Table/TransDate") != null ? root.SelectSingleNode("Table/TransDate").InnerText.Trim() : string.Empty,
                            RoomName = root.SelectSingleNode("Table/RoomName") != null ? root.SelectSingleNode("Table/RoomName").InnerText.Trim() : string.Empty,
                            CheckinDate = root.SelectSingleNode("Table/CheckinDate") != null ? root.SelectSingleNode("Table/CheckinDate").InnerText.Trim() : string.Empty,
                            NoofDays = root.SelectSingleNode("Table/NoofDays") != null ? root.SelectSingleNode("Table/NoofDays").InnerText.Trim() : string.Empty,
                            DevoteeName = root.SelectSingleNode("Table/DevoteeName") != null ? root.SelectSingleNode("Table/DevoteeName").InnerText.Trim() : string.Empty,
                            InformantName = root.SelectSingleNode("Table/InformantName") != null ? root.SelectSingleNode("Table/InformantName").InnerText.Trim() : string.Empty,
                            RoomAmount = root.SelectSingleNode("Table/RoomAmount") != null ? root.SelectSingleNode("Table/RoomAmount").InnerText.Trim() : string.Empty,
                            UserCharge = root.SelectSingleNode("Table/UserCharges") != null ? root.SelectSingleNode("Table/UserCharges").InnerText.Trim() : string.Empty,
                            AmountPaid = root.SelectSingleNode("Table/AmountPaid") != null ? root.SelectSingleNode("Table/AmountPaid").InnerText.Trim() : string.Empty,
                            LoginID = root.SelectSingleNode("Table/LoginID") != null ? root.SelectSingleNode("Table/LoginID").InnerText.Trim() : string.Empty
                        };
                        break;
                    case "815":
                        response = new ECReceiptBasedOnTransNoResp
                        {
                            ResCode = "000",
                            ResDesc = "Success",
                            TransactionID = root.SelectSingleNode("Table/TransactionID") != null ? root.SelectSingleNode("Table/TransactionID").InnerText.Trim() : string.Empty,
                            ApplicationNo = root.SelectSingleNode("Table/ApplicationNo") != null ? root.SelectSingleNode("Table/ApplicationNo").InnerText.Trim() : string.Empty,
                            AuthorizedAgentName = root.SelectSingleNode("Table/AuthorizedAgentName") != null ? root.SelectSingleNode("Table/AuthorizedAgentName").InnerText.Trim() : string.Empty,
                            DateofPayment = root.SelectSingleNode("Table/DateofPayment") != null ? root.SelectSingleNode("Table/DateofPayment").InnerText.Trim() : string.Empty,
                            ApplicantName = root.SelectSingleNode("Table/ApplicantName") != null ? root.SelectSingleNode("Table/ApplicantName").InnerText.Trim() : string.Empty,
                            PropertyOwnerName = root.SelectSingleNode("Table/PropertyOwnerName") != null ? root.SelectSingleNode("Table/PropertyOwnerName").InnerText.Trim() : string.Empty,
                            DocumentDistrict = root.SelectSingleNode("Table/DocumentDistrict") != null ? root.SelectSingleNode("Table/DocumentDistrict").InnerText.Trim() : string.Empty,
                            DocumentID = root.SelectSingleNode("Table/DocumentID") != null ? root.SelectSingleNode("Table/DocumentID").InnerText.Trim() : string.Empty,
                            DocumentYear = root.SelectSingleNode("Table/DocumentYear") != null ? root.SelectSingleNode("Table/DocumentYear").InnerText.Trim() : string.Empty,
                            SelectedSRO = root.SelectSingleNode("Table/SelectedSRO") != null ? root.SelectSingleNode("Table/SelectedSRO").InnerText.Trim() : string.Empty,
                            AmountPaidinRs = root.SelectSingleNode("Table/AmountPaidinRs.") != null ? root.SelectSingleNode("Table/AmountPaidinRs.").InnerText.Trim() : string.Empty,
                            DeliveryType = root.SelectSingleNode("Table/DeliveryType") != null ? root.SelectSingleNode("Table/DeliveryType").InnerText.Trim() : string.Empty,
                            DeliveredIn = root.SelectSingleNode("Table/Deliveredin") != null ? root.SelectSingleNode("Table/Deliveredin").InnerText.Trim() : string.Empty
                        };
                        break;
                    case "816":
                        response = new CCRReceiptBasedOnTransNoResp
                        {
                            ResCode = "000",
                            ResDesc = "Success",
                            TransactionID = root.SelectSingleNode("Table/TransactionID") != null ? root.SelectSingleNode("Table/TransactionID").InnerText.Trim() : string.Empty,
                            ApplicationNo = root.SelectSingleNode("Table/ApplicationNo") != null ? root.SelectSingleNode("Table/ApplicationNo").InnerText.Trim() : string.Empty,
                            TransDate = root.SelectSingleNode("Table/TransDate") != null ? root.SelectSingleNode("Table/TransDate").InnerText.Trim() : string.Empty,
                            ApplicantName = root.SelectSingleNode("Table/ApplicantName") != null ? root.SelectSingleNode("Table/ApplicantName").InnerText.Trim() : string.Empty,
                            ApplicantVillage = root.SelectSingleNode("Table/ApplicantVillage") != null ? root.SelectSingleNode("Table/ApplicantVillage").InnerText.Trim() : string.Empty,
                            DocumentDistrict = root.SelectSingleNode("Table/DocumentDistric") != null ? root.SelectSingleNode("Table/DocumentDistric").InnerText.Trim() : string.Empty,
                            SRO = root.SelectSingleNode("Table/SRO") != null ? root.SelectSingleNode("Table/SRO").InnerText.Trim() : string.Empty,
                            YearofRegistration = root.SelectSingleNode("Table/YearofRegistration") != null ? root.SelectSingleNode("Table/YearofRegistration").InnerText.Trim() : string.Empty,
                            DocumentID = root.SelectSingleNode("Table/DocumentID") != null ? root.SelectSingleNode("Table/DocumentID").InnerText.Trim() : string.Empty,
                            NumberofPages = root.SelectSingleNode("Table/NumberofPages") != null ? root.SelectSingleNode("Table/NumberofPages").InnerText.Trim() : string.Empty,
                            ChallanAmount = root.SelectSingleNode("Table/ChallanAmount.") != null ? root.SelectSingleNode("Table/ChallanAmount.").InnerText.Trim() : string.Empty,
                            UserCharges = root.SelectSingleNode("Table/UserCharges") != null ? root.SelectSingleNode("Table/UserCharges").InnerText.Trim() : string.Empty,
                            TotalAmountPaid = root.SelectSingleNode("Table/TotalAmountPaid") != null ? root.SelectSingleNode("Table/TotalAmountPaid").InnerText.Trim() : string.Empty,
                            TotalAmount = root.SelectSingleNode("Table/TotalAmount") != null ? root.SelectSingleNode("Table/TotalAmount").InnerText.Trim() : string.Empty,
                            CertificateDeliveryDate = root.SelectSingleNode("Table/CertificateDeliveryDate") != null ? root.SelectSingleNode("Table/CertificateDeliveryDate").InnerText.Trim() : string.Empty,
                            DeliveryType = root.SelectSingleNode("Table/DeliveryType") != null ? root.SelectSingleNode("Table/DeliveryType").InnerText.Trim() : string.Empty,
                            ReceiptHeading = root.SelectSingleNode("Table/ReceiptHeading") != null ? root.SelectSingleNode("Table/ReceiptHeading").InnerText.Trim() : string.Empty,
                            AuthorizedAgentName = root.SelectSingleNode("Table/AuthorizedAgentName") != null ? root.SelectSingleNode("Table/AuthorizedAgentName").InnerText.Trim() : string.Empty,
                            AuthorizedAgentName1 = root.SelectSingleNode("Table/AuthorizedAgentName1") != null ? root.SelectSingleNode("Table/AuthorizedAgentName1").InnerText.Trim() : string.Empty,
                            CertificateDeliveryWithIn = root.SelectSingleNode("Table/CertificateDeliveryWithIn") != null ? root.SelectSingleNode("Table/CertificateDeliveryWithIn").InnerText.Trim() : string.Empty
                        };
                        break;
                    case "845":
                        response = new CDMABDReceiptBasedOnTransNoResp
                        {
                            ResCode = "000",
                            ResDesc = "Success",
                            TransactionID = root.SelectSingleNode("Table/TransactionID") != null ? root.SelectSingleNode("Table/TransactionID").InnerText.Trim() : string.Empty,
                            ApplicationNumber = root.SelectSingleNode("Table/ApplicationNo") != null ? root.SelectSingleNode("Table/ApplicationNo").InnerText.Trim() : string.Empty,
                            AuthorizedAgent = root.SelectSingleNode("Table/AuthorizedAgent") != null ? root.SelectSingleNode("Table/AuthorizedAgent").InnerText.Trim() : string.Empty,
                            DateofPayment = root.SelectSingleNode("Table/DateofPayment") != null ? root.SelectSingleNode("Table/DateofPayment").InnerText.Trim() : string.Empty,
                            InformantName = root.SelectSingleNode("Table/InformantName") != null ? root.SelectSingleNode("Table/InformantName").InnerText.Trim() : string.Empty,
                            District = root.SelectSingleNode("Table/District") != null ? root.SelectSingleNode("Table/District").InnerText.Trim() : string.Empty,
                            DeliveryType = root.SelectSingleNode("Table/DeliveryType") != null ? root.SelectSingleNode("Table/DeliveryType").InnerText.Trim() : string.Empty,
                            NoOfCopies = root.SelectSingleNode("Table/NoOfCopies") != null ? root.SelectSingleNode("Table/NoOfCopies").InnerText.Trim() : string.Empty,
                            AmountPaidInRs = root.SelectSingleNode("Table/AmountPaidinRs") != null ? root.SelectSingleNode("Table/AmountPaidinRs").InnerText.Trim() : string.Empty,
                            TransactionSlipNo = root.SelectSingleNode("Table/TransactionSlipNo") != null ? root.SelectSingleNode("Table/TransactionSlipNo").InnerText.Trim() : string.Empty,
                            ApproverCodevarchar = root.SelectSingleNode("Table/ApproverCodevarchar") != null ? root.SelectSingleNode("Table/ApproverCodevarchar").InnerText.Trim() : string.Empty,
                            DeliveredIn = root.SelectSingleNode("Table/Deliveredin") != null ? root.SelectSingleNode("Table/Deliveredin").InnerText.Trim() : string.Empty
                        };
                        break;
                    default:
                        response = new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
                        break;
                }

                return response;
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetReceiptBasedOnTransNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetAppStatus(object reqData)
        {
            ApplicationNoBasedReq request = null;
            try
            {
                request = JsonConvert.DeserializeObject<ApplicationNoBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetAppStatus(request.ApplicationNo, CacheConfig.UserName, CacheConfig.Password);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetAppStatus => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectSingleNode("dtGetApp");
                if (dataNodes != null)
                {
                    return new AppStatusResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        Status = dataNodes.SelectSingleNode("Status_Desc") != null ? dataNodes.SelectSingleNode("Status_Desc").InnerText : string.Empty
                    };
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetAppStatus: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetServiceCharges(object reqData)
        {
            ServiceChargeReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<ServiceChargeReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetServiceCharges(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service), request.DeliveryType);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetServiceCharges => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNode = root.SelectSingleNode("dtcharges");
                if (dataNode != null)
                {
                    return new ServiceChargeResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        ServiceId = dataNode["service_id"] != null ? dataNode["service_id"].InnerText.Trim() : string.Empty,
                        ServiceAmount = dataNode["ServiceAmount"] != null ? dataNode["ServiceAmount"].InnerText.Trim() : (dataNode["Service_Amount"] != null ? dataNode["Service_Amount"].InnerText.Trim() : string.Empty),
                        DeliveryType = dataNode["delivery_type"] != null ? dataNode["delivery_type"].InnerText.Trim() : string.Empty,
                        DeliveryCharges = dataNode["Delivery_Charges"] != null ? dataNode["Delivery_Charges"].InnerText.Trim() : string.Empty,
                        ChallanAmount = dataNode["Challan_Amount"] != null ? dataNode["Challan_Amount"].InnerText.Trim() : string.Empty,
                        SLA = dataNode["SLA"] != null ? dataNode["SLA"].InnerText.Trim() : string.Empty,
                        UserCharges = dataNode["User_Charges"] != null ? dataNode["User_Charges"].InnerText.Trim() : string.Empty
                    };
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetServiceCharges: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateRelation(object reqData)
        {
            try
            {
                var clientResp = mesevaClient.PopulateRelation(CacheConfig.UserName, CacheConfig.Password);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateRelation => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new RelationResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.RelationDetail.Add(new Detail
                        {
                            Code = node["Relation_Id"] != null ? node["Relation_Id"].InnerText.Trim() : string.Empty,
                            Description = node["Relation_Name"] != null ? node["Relation_Name"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateRelation: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateRelationShip(object reqData)
        {
            try
            {
                var clientResp = mesevaClient.PopulateRelationShip(CacheConfig.UserName, CacheConfig.Password);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateRelationShip => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new RelationResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.RelationDetail.Add(new Detail
                        {
                            Code = node["Relation_Id"] != null ? node["Relation_Id"].InnerText.Trim() : string.Empty,
                            Description = node["Relation_Name"] != null ? node["Relation_Name"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateRelationShip: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateCaste(object reqData)
        {
            ServiceBasedReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<ServiceBasedReq>(reqData.ToString());
            }
            catch { }


            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.PopulateCaste(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service));
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateCaste => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new CasteResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.CasteDetail.Add(new Detail
                        {
                            Code = node["Caste_Name"] != null ? node["Caste_Name"].InnerText.Trim() : string.Empty,
                            Description = node["Caste_Category_x0020_AlphaCode"] != null ? node["Caste_Category_x0020_AlphaCode"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateCaste: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateReligion(object reqData)
        {
            try
            {
                var clientResp = mesevaClient.PopulateReligion(CacheConfig.UserName, CacheConfig.Password);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateReligion => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new ReligionResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.ReligionDetail.Add(new Detail
                        {
                            Code = node["Religion_Id"] != null ? node["Religion_Id"].InnerText.Trim() : string.Empty,
                            Description = node["Religion_desc"] != null ? node["Religion_desc"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateReligion: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateOccupation(object reqData)
        {
            ServiceTypeBasedReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<ServiceTypeBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.PopulateOccupation(CacheConfig.UserName, CacheConfig.Password, request.ServiceType);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateOccupation => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new OccupationResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.Occupation.Add(new Detail
                        {
                            Code = node["Occupation_id"] != null ? node["Occupation_id"].InnerText.Trim() : string.Empty,
                            Description = node["Occupation_desc"] != null ? node["Occupation_desc"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateOccupation: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateDeathReason(object reqData)
        {
            ServiceTypeBasedReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<ServiceTypeBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.PopulateDeathReason(CacheConfig.UserName, CacheConfig.Password, request.ServiceType);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateDeathReason => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new DeathReasonResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.DeathReasonDetail.Add(new Detail
                        {
                            Code = node["Death_Id"] != null ? node["Death_Id"].InnerText.Trim() : string.Empty,
                            Description = node["Death_Description"] != null ? node["Death_Description"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateDeathReason: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateCircle(object reqData)
        {
            try
            {
                var clientResp = mesevaClient.PopulateCircle(CacheConfig.UserName, CacheConfig.Password);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateCircle => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new CircleResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                        response.Circles.Add(node["Circle_No"] != null ? node["Circle_No"].InnerText.Trim() : string.Empty);

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateCircle: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ COMMON SERVICES ]

        #region [ INCOME SERVICES ]

        public MSResponse GetIncomeTransactionNo(object reqData)
        {
            IncomeTransactionNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<IncomeTransactionNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                if ("y,t".IndexOf(request.AddressFlag.ToLower()) >= 0)
                {
                    request.Profile.PostalDoorNo = request.Profile.PermanentDoorNo;
                    request.Profile.PostalLocality = request.Profile.PermanentLocality;
                    request.Profile.PostalDistrict = request.Profile.PermanentDistrict;
                    request.Profile.PostalMandal = request.Profile.PermanentMandal;
                    request.Profile.PostalVillage = request.Profile.PermanentVillage;
                    request.Profile.PostalPincode = request.Profile.PermanentPincode;
                }

                if ((request.Income.BuildingIncome == "0" && request.Income.BusinessIncome == "0" && request.Income.EmpSal == "0" && request.Income.LabourIncome == "0" && request.Income.LandIncome == "0" && request.Income.OtherIncome == "0")
                    || (request.Income.BuildingIncome == "" && request.Income.BusinessIncome == "" && request.Income.EmpSal == "" && request.Income.LabourIncome == "" && request.Income.LandIncome == "" && request.Income.OtherIncome == ""))
                {
                    return new MSResponse { ResCode = "201", ResDesc = CacheConfig.GetErrorMessage("201") };
                }

                var clientResp = mesevaClient.GetIncomeTransactionNo(CacheConfig.UserName, CacheConfig.Password, request.MobileNo, CacheConfig.GetServiceId(request.Service),
                                    request.DocumentRefNumbers, request.AddressFlag, request.ApplicationNo, request.ApplicantName, request.Profile.FatherName,
                                    request.Profile.Gender, request.Profile.DateOfBirth, request.Profile.PermanentDoorNo, request.Profile.PermanentLocality, request.Profile.PermanentDistrict,
                                    request.Profile.PermanentMandal, request.Profile.PermanentVillage, request.Profile.PermanentPincode, request.Profile.PostalDoorNo, request.Profile.PostalLocality,
                                    request.Profile.StateId, request.Profile.PostalDistrict, request.Profile.PostalMandal, request.Profile.PostalVillage, request.Profile.PostalPincode, request.MobileNo,
                                    request.MobileNo, request.Profile.EmailID, request.Profile.Remarks, request.Profile.RationCardNo, request.Profile.AadhaarNo, request.Income.DeliveryType,
                                    request.Income.LandIncome, request.Income.BusinessIncome, request.Income.BuildingIncome, request.Income.LabourIncome, request.Income.EmpSal, request.Income.OtherIncome,
                                    request.Income.TotalIncome, request.Income.Purpose, request.Charge.ServiceCharge, request.Charge.UserCharge, request.Charge.PostalCharge, request.Charge.TotalAmount,
                                    request.Document.DocApplicationform, request.Document.DocIDProof, request.Document.DocIncomeProof);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetIncomeTransactionNo => Response: {0}", clientResp));
                if (clientResp == null)
                    new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode == null || respNode.InnerText.Trim().Trim() != "100")
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText.Trim() };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetIncomeTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetIncomeCertificatePDF(object reqData)
        {
            ApplicationNoBasedReq request = null;
            try
            {
                request = JsonConvert.DeserializeObject<ApplicationNoBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetIncomeCertificatePDF(CacheConfig.UserName, CacheConfig.Password, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetIncomeCertificatePDF => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                if (root.SelectSingleNode("Table1/Data_Available") != null && root.SelectSingleNode("Table1/Data_Available").InnerText.Trim().ToUpper() == "Y"
                    && root.SelectSingleNode("Table/IS_PDF") != null && root.SelectSingleNode("Table/IS_PDF").InnerText.Trim().Trim() == "Y"
                    && root.SelectSingleNode("Table2/PDF") != null)
                {
                    return new CertificatePDFResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        PDFDocument = root.SelectSingleNode("Table2/PDF").InnerText.Trim()
                    };
                }

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetIncomeCertificatePDF: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ INCOME SERVICES ]

        #region [ RESIDENCE SERVICES ]

        public MSResponse GetResidanceTransactionNo(object reqData)
        {
            ResidenceTransactionNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<ResidenceTransactionNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                if ("y,t".IndexOf(request.AddressFlag.ToLower()) >= 0)
                {
                    request.Profile.PostalDoorNo = request.Profile.PermanentDoorNo;
                    request.Profile.PostalLocality = request.Profile.PermanentLocality;
                    request.Profile.PostalDistrict = request.Profile.PermanentDistrict;
                    request.Profile.PostalMandal = request.Profile.PermanentMandal;
                    request.Profile.PostalVillage = request.Profile.PermanentVillage;
                    request.Profile.PostalPincode = request.Profile.PermanentPincode;
                }

                var clientResp = mesevaClient.GetResidanceTransactionNo(CacheConfig.UserName, CacheConfig.Password, request.MobileNo, CacheConfig.GetServiceId(request.Service),
                    request.DocumentRefNumbers, request.AddressFlag, request.ApplicationNo, request.Profile.AadhaarNo, request.ApplicantName, request.Profile.FatherName,
                    request.Profile.Gender, request.Profile.DateOfBirth, request.Profile.PermanentDoorNo, request.Profile.PermanentLocality, request.Profile.PermanentDistrict,
                    request.Profile.PermanentMandal, request.Profile.PermanentVillage, request.Profile.PermanentPincode, request.Profile.PostalDoorNo, request.Profile.PostalLocality,
                    request.Profile.StateId, request.Profile.PostalDistrict, request.Profile.PostalMandal, request.Profile.PostalVillage, request.Profile.PostalPincode,
                    request.MobileNo, request.Profile.PhoneNo, request.Profile.EmailID, request.Profile.Remarks, request.Profile.RationCardNo, request.Profile.DeliveryType,
                    request.Profile.ResidingSinceinYears, request.Profile.Purpose, request.Charge.ServiceCharge, request.Charge.UserCharge, request.Charge.PostalCharge,
                    request.Charge.TotalAmount, request.Document.DocApplicationform, request.Document.DocIDProof, request.Document.DocHouseProof, request.Document.DocPhoto);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetResidanceTransactionNo => Response: {0}", clientResp));
                if (clientResp == null)
                    new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode == null || respNode.InnerText.Trim().Trim() != "100")
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText.Trim() };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetResidanceTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetResidenceCertificatePDF(object reqData)
        {
            ApplicationNoBasedReq request = null;
            try
            {
                request = JsonConvert.DeserializeObject<ApplicationNoBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetResidenceCertificatePDF(CacheConfig.UserName, CacheConfig.Password, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetResidenceCertificatePDF => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                if (root.SelectSingleNode("Table1/Data_Available") != null && root.SelectSingleNode("Table1/Data_Available").InnerText.Trim().ToUpper() == "Y"
                    && root.SelectSingleNode("Table/IS_PDF") != null && root.SelectSingleNode("Table/IS_PDF").InnerText.Trim().Trim() == "Y"
                    && root.SelectSingleNode("Table2/PDF") != null)
                {
                    return new CertificatePDFResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        PDFDocument = root.SelectSingleNode("Table2/PDF").InnerText.Trim()
                    };
                }

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetResidenceCertificatePDF: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ RESIDENCE SERVICES ]

        #region [ ADANGAL SERVICES ]

        public MSResponse PopulateRevenueDocYear(object reqData)
        {
            try
            {
                var clientResp = mesevaClient.PopulateRevenueDocYear(CacheConfig.UserName, CacheConfig.Password);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateRevenueDocYear => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("dtVillage");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new RevenueDocYearResp { ResCode = "000", ResDesc = "Success" };
                    foreach (XmlNode node in dataNodes)
                    {
                        response.DocYear.Add(new RevenueDocYear
                        {
                            DocYear = node["DocYear"] != null ? node["DocYear"].InnerText.Trim() : string.Empty,
                            DocYearDesc = node["DocYear_Description"] != null ? node["DocYear_Description"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateRevenueDocYear: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateSurveyNumber(object reqData)
        {
            SurveyNumberReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<SurveyNumberReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.PopulateSurveyNumber(CacheConfig.UserName, CacheConfig.Password, request.DistrictId, request.MandalId, request.VillageId, request.DocYear);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateSurveyNumber => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var surveyNoNodes = root.SelectNodes("Table1/lSurveyNos");
                if (surveyNoNodes.Count > 0)
                {
                    var surveyNos = surveyNoNodes.OfType<XmlNode>().Select(n => n.InnerText.Trim()).ToList();
                    return new SurveyNumberResp { ResCode = "000", ResDesc = "Success", SurveyNumbers = surveyNos };
                }

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateSurveyNumber: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateAdangaldetails(object reqData)
        {
            AdangaldetailReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<AdangaldetailReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.PopulateAdangaldetails(CacheConfig.UserName, CacheConfig.Password, request.ApplicationNo, request.DistrictId, request.MandalId, request.VillageId, request.DocYear, request.SurveyNo, request.IsCropDetails);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateAdangaldetails => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNode = root.SelectSingleNode("dtSurveyDetails");
                if (dataNode != null)
                {
                    return new AdangaldetailResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        SerialNo = dataNode.SelectSingleNode("pSerialNo") != null ? dataNode.SelectSingleNode("pSerialNo").InnerText.Trim() : string.Empty,
                        SurveyNo = dataNode.SelectSingleNode("pSurvey_no") != null ? dataNode.SelectSingleNode("pSurvey_no").InnerText.Trim() : string.Empty,
                        TotalExtent = dataNode.SelectSingleNode("pTotal_Extent") != null ? dataNode.SelectSingleNode("pTotal_Extent").InnerText.Trim() : string.Empty,
                        UncultivatedLand = dataNode.SelectSingleNode("pUncultivated_Land") != null ? dataNode.SelectSingleNode("pUncultivated_Land").InnerText.Trim() : string.Empty,
                        CultivatableLand = dataNode.SelectSingleNode("pCultivatable_Land") != null ? dataNode.SelectSingleNode("pCultivatable_Land").InnerText.Trim() : string.Empty,
                        LandNature = dataNode.SelectSingleNode("pLand_Nature") != null ? dataNode.SelectSingleNode("pLand_Nature").InnerText.Trim() : string.Empty,
                        Tax = dataNode.SelectSingleNode("pTax") != null ? dataNode.SelectSingleNode("pTax").InnerText.Trim() : string.Empty,
                        LandClassification = dataNode.SelectSingleNode("pLand_Classification") != null ? dataNode.SelectSingleNode("pLand_Classification").InnerText.Trim() : string.Empty,
                        WaterResource = dataNode.SelectSingleNode("pWater_Resource") != null ? dataNode.SelectSingleNode("pWater_Resource").InnerText.Trim() : string.Empty,
                        AyakatExtent = dataNode.SelectSingleNode("pAyakat_Extent") != null ? dataNode.SelectSingleNode("pAyakat_Extent").InnerText.Trim() : string.Empty,
                        KhataNumber = dataNode.SelectSingleNode("pKhata_Number") != null ? dataNode.SelectSingleNode("pKhata_Number").InnerText.Trim() : string.Empty,
                        PattadarName = dataNode.SelectSingleNode("pPattadar_Name") != null ? dataNode.SelectSingleNode("pPattadar_Name").InnerText.Trim() : string.Empty,
                        OccupantName = dataNode.SelectSingleNode("pOccupant_Name") != null ? dataNode.SelectSingleNode("pOccupant_Name").InnerText.Trim() : string.Empty,
                        OccupantExtent = dataNode.SelectSingleNode("pOccupant_Extent") != null ? dataNode.SelectSingleNode("pOccupant_Extent").InnerText.Trim() : string.Empty,
                        EnjoymentNature = dataNode.SelectSingleNode("pEnjoyment_Nature") != null ? dataNode.SelectSingleNode("pEnjoyment_Nature").InnerText.Trim() : string.Empty,
                        OccupantFatherName = dataNode.SelectSingleNode("pOccupant_Father_Name") != null ? dataNode.SelectSingleNode("pOccupant_Father_Name").InnerText.Trim() : string.Empty,
                        PattadarFatherName = dataNode.SelectSingleNode("pPattadar_Father_Name") != null ? dataNode.SelectSingleNode("pPattadar_Father_Name").InnerText.Trim() : string.Empty,
                        VillageName = dataNode.SelectSingleNode("pVillage_Name") != null ? dataNode.SelectSingleNode("pVillage_Name").InnerText.Trim() : string.Empty,
                        VillageCode = dataNode.SelectSingleNode("pVillage_Code") != null ? dataNode.SelectSingleNode("pVillage_Code").InnerText.Trim() : string.Empty,
                        DeleteFlag = dataNode.SelectSingleNode("pDelete_Flag") != null ? dataNode.SelectSingleNode("pDelete_Flag").InnerText.Trim() : string.Empty,
                        MutatedDate = dataNode.SelectSingleNode("pMutated_Date") != null ? dataNode.SelectSingleNode("pMutated_Date").InnerText.Trim() : string.Empty,
                        BaseSurveyNo = dataNode.SelectSingleNode("pBase_survey_No") != null ? dataNode.SelectSingleNode("pBase_survey_No").InnerText.Trim() : string.Empty,
                        Signature = dataNode.SelectSingleNode("pSignature") != null ? dataNode.SelectSingleNode("pSignature").InnerText.Trim() : string.Empty,
                        SignatureChecked = dataNode.SelectSingleNode("pSignatureChecked") != null ? dataNode.SelectSingleNode("pSignatureChecked").InnerText.Trim() : string.Empty,
                        VerifiedBy = dataNode.SelectSingleNode("verifiedBy") != null ? dataNode.SelectSingleNode("verifiedBy").InnerText.Trim() : string.Empty,
                        LandExtentUnits = dataNode.SelectSingleNode("pLand_Extent_Units") != null ? dataNode.SelectSingleNode("pLand_Extent_Units").InnerText.Trim() : string.Empty,
                        PassbookNumber = dataNode.SelectSingleNode("pPassbook_Number") != null ? dataNode.SelectSingleNode("pPassbook_Number").InnerText.Trim() : string.Empty,
                        Fasaliyear = dataNode.SelectSingleNode("fasali_year") != null ? dataNode.SelectSingleNode("fasali_year").InnerText.Trim() : string.Empty,
                        Pahaniyear = dataNode.SelectSingleNode("pahani_year") != null ? dataNode.SelectSingleNode("pahani_year").InnerText.Trim() : string.Empty,
                        TarhaCode = dataNode.SelectSingleNode("pTARHACODE") != null ? dataNode.SelectSingleNode("pTARHACODE").InnerText.Trim() : string.Empty,
                        CR_TR_I_EXT = dataNode.SelectSingleNode("pCR_TR_I_EXT") != null ? dataNode.SelectSingleNode("pCR_TR_I_EXT").InnerText.Trim() : string.Empty,
                        PCRSeason = dataNode.SelectSingleNode("pcr_season") != null ? dataNode.SelectSingleNode("pcr_season").InnerText.Trim() : string.Empty,
                        MonthName = dataNode.SelectSingleNode("pMonthName") != null ? dataNode.SelectSingleNode("pMonthName").InnerText.Trim() : string.Empty,
                        CRPNAME = dataNode.SelectSingleNode("pCRPNAME") != null ? dataNode.SelectSingleNode("pCRPNAME").InnerText.Trim() : string.Empty,
                        CRSowType = dataNode.SelectSingleNode("pcr_sow_type") != null ? dataNode.SelectSingleNode("pcr_sow_type").InnerText.Trim() : string.Empty,
                        WaterSourceDesc = dataNode.SelectSingleNode("pwater_source_desc") != null ? dataNode.SelectSingleNode("pwater_source_desc").InnerText.Trim() : string.Empty,
                        CR1stExt = dataNode.SelectSingleNode("pcr_1st_ext") != null ? dataNode.SelectSingleNode("pcr_1st_ext").InnerText.Trim() : string.Empty,
                        CR2nd3rdExt = dataNode.SelectSingleNode("pcr_2nd_3rd_ext") != null ? dataNode.SelectSingleNode("pcr_2nd_3rd_ext").InnerText.Trim() : string.Empty,
                        CRYield = dataNode.SelectSingleNode("pcr_yield") != null ? dataNode.SelectSingleNode("pcr_yield").InnerText.Trim() : string.Empty,
                        CRVaoRiRem = dataNode.SelectSingleNode("pcr_vao_ri_rem") != null ? dataNode.SelectSingleNode("pcr_vao_ri_rem").InnerText.Trim() : string.Empty,
                        CRMroRem = dataNode.SelectSingleNode("pcr_mro_rem") != null ? dataNode.SelectSingleNode("pcr_mro_rem").InnerText.Trim() : string.Empty,
                        Remarks = dataNode.SelectSingleNode("premarks") != null ? dataNode.SelectSingleNode("premarks").InnerText.Trim() : string.Empty,
                        SingleJoint = dataNode.SelectSingleNode("psingle_joint") != null ? dataNode.SelectSingleNode("psingle_joint").InnerText.Trim() : string.Empty,
                        MeesevaAppliNo = dataNode.SelectSingleNode("MeesevaAppliNo") != null ? dataNode.SelectSingleNode("MeesevaAppliNo").InnerText.Trim() : string.Empty
                    };
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateAdangaldetails: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetAdangalTransactionNo(object reqData)
        {
            AdangalTransactionNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<AdangalTransactionNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetAdangalTransactionNo(CacheConfig.UserName, CacheConfig.Password, request.MobileNo, request.ApplicationNo,
                    request.DocumentRefNumbers, request.Document.DocDistrictId, request.Document.DocMandalId, request.Document.DocVillageId, request.Document.DocYear,
                    request.Document.DocSurveyNo, request.CropFlag, request.ApplicantName, request.Profile.Gender, request.Profile.FatherName, request.Profile.Perstate,
                    request.ApplicantDistrict, request.ApplicantMandal, request.ApplicantVillage, request.MobileNo, request.Profile.AadhaarNo, request.DeliveryType,
                    request.Profile.EmailID, request.Profile.PermanentDoorNo, request.Profile.PermanentLocality, request.Profile.PermanentPincode, request.Profile.PhoneNo,
                    request.Profile.PostalDistrict, request.Profile.PostalMandal, request.Profile.PostalVillage, request.Profile.PostalLocality, request.Profile.PostalDoorNo,
                    request.Profile.PostalPincode, request.Profile.PostalState, request.Profile.RationCardNo, request.Profile.DateOfBirth, request.Charge.ServiceCharge,
                    request.Charge.PostalCharge, request.Charge.UserCharge, request.Charge.TotalAmount, request.Document.DocApplicationform);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetAdangalTransactionNo => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode != null && respNode.InnerText.Trim() == "100")
                    return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText };

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetAdangalTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetAdangalCertificatePDF(object reqData)
        {
            ApplicationNoBasedReq request = null;
            try
            {
                request = JsonConvert.DeserializeObject<ApplicationNoBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetAdangalCertificatePDF(CacheConfig.UserName, CacheConfig.Password, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetAdangalCertificatePDF => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                if (root.SelectSingleNode("Table1/Data_Available") != null && root.SelectSingleNode("Table1/Data_Available").InnerText.Trim().ToUpper() == "Y"
                    && root.SelectSingleNode("Table/IS_PDF") != null && root.SelectSingleNode("Table/IS_PDF").InnerText.Trim().Trim() == "Y"
                    && root.SelectSingleNode("Table2/PDF") != null)
                {
                    return new CertificatePDFResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        PDFDocument = root.SelectSingleNode("Table2/PDF").InnerText.Trim()
                    };
                }

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetAdangalCertificatePDF: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ ADANGAL SERVICES ]

        #region [ EBC SERVICES ]

        public MSResponse GetEBCTransactionNo(object reqData)
        {
            EBCTransactionNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<EBCTransactionNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                if ("y,t".IndexOf(request.AddressFlag.ToLower()) >= 0)
                {
                    request.Profile.PostalDoorNo = request.Profile.PermanentDoorNo;
                    request.Profile.PostalLocality = request.Profile.PermanentLocality;
                    request.Profile.PostalDistrict = request.Profile.PermanentDistrict;
                    request.Profile.PostalMandal = request.Profile.PermanentMandal;
                    request.Profile.PostalVillage = request.Profile.PermanentVillage;
                    request.Profile.PostalPincode = request.Profile.PermanentPincode;
                }

                var clientResp = mesevaClient.GetEBCTransactionNo(CacheConfig.UserName, CacheConfig.Password, request.MobileNo, CacheConfig.GetServiceId(request.Service),
                    request.DocumentRefNumbers, request.AddressFlag, request.ApplicationNo, request.Profile.AadhaarNo, request.ApplicantName, request.Profile.Relation,
                    request.Profile.FatherName, request.Profile.Gender, request.Profile.DateOfBirth, request.Profile.PermanentDoorNo, request.Profile.PermanentLocality,
                    request.Profile.PermanentDistrict, request.Profile.PermanentMandal, request.Profile.PermanentVillage, request.Profile.PermanentPincode, request.Profile.PostalDoorNo,
                    request.Profile.PostalLocality, request.Profile.StateId, request.Profile.PostalDistrict, request.Profile.PostalMandal, request.Profile.PostalVillage,
                    request.Profile.PostalPincode, request.MobileNo, request.Profile.PhoneNo, request.Profile.EmailID, request.Profile.Remarks, request.Profile.RationCardNo,
                    request.Certificate.DeliveryType, request.Certificate.IssuedCasteCertificateInPast, request.Certificate.CasteClaimed, request.Certificate.CasteCategory,
                    request.Certificate.Religion, request.Certificate.PurposeofCasteCertificate, request.Charge.FamilyIncome, request.Charge.ServiceCharge, request.Charge.UserCharge,
                    request.Charge.PostalCharge, request.Charge.TotalAmount, request.Document.DocApplicationform, request.Document.DocIDProof);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetEBCTransactionNo => Response: {0}", clientResp));
                if (clientResp == null)
                    new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode == null || respNode.InnerText.Trim().Trim() != "100")
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText.Trim() };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetEBCTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetEBCCertificatePDF(object reqData)
        {
            ApplicationNoBasedReq request = null;
            try
            {
                request = JsonConvert.DeserializeObject<ApplicationNoBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetEBCCertificatePDF(CacheConfig.UserName, CacheConfig.Password, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetEBCCertificatePDF => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                if (root.SelectSingleNode("Table1/Data_Available") != null && root.SelectSingleNode("Table1/Data_Available").InnerText.Trim().ToUpper() == "Y"
                    && root.SelectSingleNode("Table/IS_PDF") != null && root.SelectSingleNode("Table/IS_PDF").InnerText.Trim().Trim() == "Y"
                    && root.SelectSingleNode("Table2/PDF") != null)
                {
                    return new CertificatePDFResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        PDFDocument = root.SelectSingleNode("Table2/PDF").InnerText.Trim()
                    };
                }

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetEBCCertificatePDF: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ EBC SERVICES ]

        #region [ OBC SERVICES ]

        public MSResponse GetOBCTransactionNo(object reqData)
        {
            OBCTransactionNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<OBCTransactionNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                if ("y,t".IndexOf(request.AddressFlag.ToLower()) >= 0)
                {
                    request.Profile.PostalDoorNo = request.Profile.PermanentDoorNo;
                    request.Profile.PostalLocality = request.Profile.PermanentLocality;
                    request.Profile.PostalDistrict = request.Profile.PermanentDistrict;
                    request.Profile.PostalMandal = request.Profile.PermanentMandal;
                    request.Profile.PostalVillage = request.Profile.PermanentVillage;
                    request.Profile.PostalPincode = request.Profile.PermanentPincode;
                }

                var clientResp = mesevaClient.GetOBCTransactionNo(CacheConfig.UserName, CacheConfig.Password, request.MobileNo, CacheConfig.GetServiceId(request.Service),
                    request.DocumentRefNumbers, request.AddressFlag, request.ApplicationNo, request.Profile.AadhaarNo, request.ApplicantName, request.Profile.Relation, request.Profile.FatherName,
                    request.Profile.Gender, request.Profile.DateOfBirth, request.Profile.PermanentDoorNo, request.Profile.PermanentLocality, request.Profile.PermanentDistrict,
                    request.Profile.PermanentMandal, request.Profile.PermanentVillage, request.Profile.PermanentPincode, request.Profile.PostalDoorNo, request.Profile.PostalLocality,
                    request.Profile.StateId, request.Profile.PostalDistrict, request.Profile.PostalMandal, request.Profile.PostalVillage, request.Profile.PostalPincode, request.MobileNo,
                    request.Profile.PhoneNo, request.Profile.EmailID, request.Profile.Remarks, request.Profile.RationCardNo, request.Certificate.DeliveryType, request.Certificate.IssuedCasteCertificateInPast,
                    request.Certificate.CasteClaimed, request.Certificate.CasteCategory, request.Certificate.EducationCertificateContainsCaste, request.Certificate.PurposeofCasteCertificate,
                    request.Certificate.Religion, request.Charge.FamilyIncome, request.Charge.ServiceCharge, request.Charge.UserCharge, request.Charge.PostalCharge, request.Charge.TotalAmount,
                    request.Document.DocApplicationform, request.Document.DocIDProof, request.Document.DocProperty, request.Document.DocITReturns);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetOBCTransactionNo => Response: {0}", clientResp));
                if (clientResp == null)
                    new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode == null || respNode.InnerText.Trim() != "100")
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText.Trim() };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetOBCTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetOBCCertificatePDF(object reqData)
        {
            ApplicationNoBasedReq request = null;
            try
            {
                request = JsonConvert.DeserializeObject<ApplicationNoBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetOBCCertificatePDF(CacheConfig.UserName, CacheConfig.Password, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetEBCCertificatePDF => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                if (root.SelectSingleNode("Table1/Data_Available") != null && root.SelectSingleNode("Table1/Data_Available").InnerText.Trim().ToUpper() == "Y"
                    && root.SelectSingleNode("Table/IS_PDF") != null && root.SelectSingleNode("Table/IS_PDF").InnerText.Trim() == "Y"
                    && root.SelectSingleNode("Table2/PDF") != null)
                {
                    return new CertificatePDFResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        PDFDocument = root.SelectSingleNode("Table2/PDF").InnerText.Trim()
                    };
                }

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetEBCCertificatePDF: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ OBC SERVICES ]

        #region [ FM SERVICES ]

        public MSResponse PopulateFMServiceTypes(object reqData)
        {
            try
            {
                var clientResp = mesevaClient.PopulateFamilyMembershipServiceTypes(CacheConfig.UserName, CacheConfig.Password);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateFamilyMembershipServiceTypes => Response: {0}", clientResp));
                if (clientResp == null)
                    new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new FMServiceTypeResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.FMServiceTypes.Add(new Detail
                        {
                            Code = node["service_id"] != null ? node["service_id"].InnerText.Trim() : string.Empty,
                            Description = node["service_desc"] != null ? node["service_desc"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateFamilyMembershipServiceTypes: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateReasonforCertificate(object reqData)
        {
            ServiceTypeBasedReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<ServiceTypeBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.PopulateReasonforCertificate(CacheConfig.UserName, CacheConfig.Password, request.ServiceType);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateFamilyMembershipServiceTypes => Response: {0}", clientResp));
                if (clientResp == null)
                    new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new ReasonCertificateResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.Purposes.Add(new Detail
                        {
                            Code = node["Purpose_id"] != null ? node["Purpose_id"].InnerText.Trim() : string.Empty,
                            Description = node["Purpose_desc"] != null ? node["Purpose_desc"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateFamilyMembershipServiceTypes: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetFMTransactionNo(object reqData)
        {
            FMTransactionNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<FMTransactionNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                if ("y,t".IndexOf(request.AddressFlag.ToLower()) >= 0)
                {
                    request.Profile.PostalDoorNo = request.Profile.PermanentDoorNo;
                    request.Profile.PostalLocality = request.Profile.PermanentLocality;
                    request.Profile.PostalDistrict = request.Profile.PermanentDistrict;
                    request.Profile.PostalMandal = request.Profile.PermanentMandal;
                    request.Profile.PostalVillage = request.Profile.PermanentVillage;
                    request.Profile.PostalPincode = request.Profile.PermanentPincode;
                }

                var clientResp = mesevaClient.GetFamilyMembershipTransactionNo(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service), request.MobileNo,
                request.ServiceType, request.AddressFlag, request.ApplicationNo, request.ApplicantName, request.Profile.Gender, request.Profile.Relation, request.Profile.RelationName,
                request.Profile.DateOfBirth, request.Profile.PermanentDistrict, request.Profile.PermanentMandal, request.Profile.PermanentVillage, request.Profile.PermanentDoorNo,
                request.Profile.PermanentLocality, request.Profile.PermanentPincode, request.Profile.StateId, request.Profile.PostalDistrict, request.Profile.PostalMandal,
                request.Profile.PostalVillage, request.MobileNo, request.Profile.PostalDoorNo, request.Profile.PostalLocality, request.Profile.PostalPincode, request.DeliveryType,
                request.Profile.RationCardNo, request.DeceaseInfo.DeceasedName, request.DeceaseInfo.DeceasedFName, request.DeceaseInfo.DateofDeath, request.DeceaseInfo.ReasonforDeath,
                request.DeceaseInfo.Occupation, request.DeceaseInfo.ReasonforCertificate, request.DeceaseInfo.DeathPlace, request.DeceaseInfo.GridFamilyMemberDetails,
                request.Charge.ServiceCharge, request.Charge.UserCharge, request.Charge.PostalCharge, request.Charge.TotalAmount, request.Document.DocApplicationform, request.Document.DocIDProof, request.Document.DocDeathProof);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetFMTransactionNo => Response: {0}", clientResp));
                if (clientResp == null)
                    new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode == null || respNode.InnerText.Trim() != "100")
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText.Trim() };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetFMTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetFMCertificatePDF(object reqData)
        {
            ApplicationNoBasedReq request = null;
            try
            {
                request = JsonConvert.DeserializeObject<ApplicationNoBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetFamilyMembershipCertificatePDF(CacheConfig.UserName, CacheConfig.Password, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetFMCertificatePDF => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                if (root.SelectSingleNode("Table1/Data_Available") != null && root.SelectSingleNode("Table1/Data_Available").InnerText.Trim().ToUpper() == "Y"
                    && root.SelectSingleNode("Table/IS_PDF") != null && root.SelectSingleNode("Table/IS_PDF").InnerText.Trim().Trim() == "Y"
                    && root.SelectSingleNode("Table2/PDF") != null)
                {
                    return new CertificatePDFResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        PDFDocument = root.SelectSingleNode("Table2/PDF").InnerText.Trim()
                    };
                }

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetFMCertificatePDF: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ FM SERVICES ]

        #region [ FMB COPY SERVICES ]

        public MSResponse GetFMBCopySurveyNumbers(object reqData)
        {
            SurveyNumberReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<SurveyNumberReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetFMBCopySurveyNumbers(CacheConfig.UserName, CacheConfig.Password, request.DistrictId, request.MandalId, request.VillageId);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetFMBCopySurveyNumbers => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var surveyNoNodes = root.SelectNodes("Table1/SurveyNos");
                if (surveyNoNodes.Count > 0)
                {
                    var surveyNos = surveyNoNodes.OfType<XmlNode>().Select(n => n.InnerText.Trim()).ToList();
                    return new SurveyNumberResp { ResCode = "000", ResDesc = "Success", SurveyNumbers = surveyNos };
                }

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetFMBCopySurveyNumbers: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetFMBCopyDetails(object reqData)
        {
            FMBCopyDetailReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<FMBCopyDetailReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetFMBCopyDetails(CacheConfig.UserName, CacheConfig.Password, request.DistrictId, request.MandalId, request.VillageId, request.SurveyNo, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetFMBCopyDetails => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode != null && respNode.InnerText.Trim() == "100")
                    return new MSResponse { ResCode = "000", ResDesc = "Success" };

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetFMBCopyDetails: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse FMBCopyGetTransactionNo(object reqData)
        {
            FMBCopyTransactionNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<FMBCopyTransactionNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.FMBCopyGetTransactionID(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service),
                    request.MobileNo, request.FMBDistrict, request.FMBMandal, request.FMBVillage, request.FMBSurveyNo, request.ApplicationNo, request.ApplicantName,
                    request.Profile.FatherName, request.Profile.AadhaarNo, request.DeliveryType, request.Profile.PermanentDistrict, request.Profile.PermanentMandal,
                    request.Profile.PermanentVillage, request.Profile.PermanentDoorNo, request.Profile.PermanentPincode, request.MobileNo, request.Profile.EmailID,
                    request.Charge.ServiceCharge, request.Charge.UserCharge, request.Charge.PostalCharge, request.Charge.TotalAmount, request.Document.DocApplicationform);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("FMBCopyGetTransactionNo => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode != null && respNode.InnerText.Trim() == "100")
                    return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText };

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => FMBCopyGetTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetFMBCopyCertificatePDF(object reqData)
        {
            ApplicationNoBasedReq request = null;
            try
            {
                request = JsonConvert.DeserializeObject<ApplicationNoBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetFMBCopyCertificatePDF(CacheConfig.UserName, CacheConfig.Password, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetFMBCopyCertificatePDF => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                if (root.SelectSingleNode("Table1/Data_Available") != null && root.SelectSingleNode("Table1/Data_Available").InnerText.Trim().ToUpper() == "Y"
                    && root.SelectSingleNode("Table/IS_PDF") != null && root.SelectSingleNode("Table/IS_PDF").InnerText.Trim().Trim() == "Y"
                    && root.SelectSingleNode("Table2/PDF") != null)
                {
                    return new CertificatePDFResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        PDFDocument = root.SelectSingleNode("Table2/PDF").InnerText.Trim()
                    };
                }

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetFMBCopyCertificatePDF: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ FMB COPY SERVICES ]

        #region [ TEMPLE SEVA BOOKING SERVICES ]

        public MSResponse PopulateTemples(object reqData)
        {
            ServiceBasedReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<ServiceBasedReq>(reqData.ToString());
            }
            catch { }


            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.PopulateTemples(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service));
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateTemples => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    return new TempleDetailResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        TempleId = dataNodes[0]["TEMPLEID"] != null ? dataNodes[0]["TEMPLEID"].InnerText.Trim() : (dataNodes[0]["temple_id"] != null ? dataNodes[0]["temple_id"].InnerText.Trim() : string.Empty),
                        TempleName = dataNodes[0]["TEMPLENAME"] != null ? dataNodes[0]["TEMPLENAME"].InnerText.Trim() : (dataNodes[0]["temple_desc"] != null ? dataNodes[0]["temple_desc"].InnerText.Trim() : string.Empty),
                        TempleDesc = dataNodes[0]["TEMPLEDESCRIPTION"] != null ? dataNodes[0]["TEMPLEDESCRIPTION"].InnerText.Trim() : string.Empty,
                        TempleLocation = dataNodes[0]["TEMPLELOCATION"] != null ? dataNodes[0]["TEMPLELOCATION"].InnerText.Trim() : string.Empty
                    };
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateTemples: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateSevas(object reqData)
        {
            TempleSevasReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<TempleSevasReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.PopulateSevas(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service), request.TempleId);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateSevas => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new TempleSevasResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.Sevas.Add(new Detail
                        {
                            Code = node["seva_id"] != null ? node["seva_id"].InnerText.Trim() : string.Empty,
                            Description = node["seva_desc"] != null ? node["seva_desc"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateSevas: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateBatches(object reqData)
        {
            TempleBatchesReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<TempleBatchesReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.PopulateBatches(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service), request.TempleId, request.SevaId);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateBatches => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new TempleBatchesResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.Batches.Add(new BatcheDetail
                        {
                            BatchID = node["BatchID"] != null ? node["BatchID"].InnerText.Trim() : string.Empty,
                            BatchName = node["Batch_Name"] != null ? node["Batch_Name"].InnerText.Trim() : string.Empty,
                            BatchType = node["BatchType"] != null ? node["BatchType"].InnerText.Trim() : string.Empty,
                            BatchAvailDays = node["BatchAvailDays"] != null ? node["BatchAvailDays"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateBatches: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateProofDocuments(object reqData)
        {
            try
            {
                var clientResp = mesevaClient.PopulateProofDocuments(CacheConfig.UserName, CacheConfig.Password);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateProofDocuments => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new ProofDocumentsResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.ProofOfDocuments.Add(new Detail
                        {
                            Code = node["doc_id"] != null ? node["doc_id"].InnerText.Trim() : string.Empty,
                            Description = node["doc_desc"] != null ? node["doc_desc"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateProofDocuments: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetSevaDetails(object reqData)
        {
            TempleSevaDetailReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<TempleSevaDetailReq>(reqData.ToString());
            }
            catch { }


            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetSevaDetails(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service), request.TempleId, request.SevaId, request.BatchId);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetSevaDetails => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new TempleSevaDetailResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.SevaDetails.Add(new SevaDetail
                        {
                            SevaId = node["sevaid"] != null ? node["sevaid"].InnerText.Trim() : string.Empty,
                            SevaName = node["Seva_name"] != null ? node["Seva_name"].InnerText.Trim() : string.Empty,
                            SevaAmount = node["SevaAmount"] != null ? node["SevaAmount"].InnerText.Trim() : string.Empty,
                            BatchName = node["Batch_Name"] != null ? node["Batch_Name"].InnerText.Trim() : string.Empty,
                            UserCharge = node["Usercharges"] != null ? node["Usercharges"].InnerText.Trim() : string.Empty,
                            SevaDescription = node["SevaDescription"] != null ? node["SevaDescription"].InnerText.Trim() : string.Empty,
                            ReportingTime = node["ReportingTime"] != null ? node["ReportingTime"].InnerText.Trim() : string.Empty,
                            NoOfPersons = node["No_of_Persons"] != null ? node["No_of_Persons"].InnerText.Trim() : string.Empty,
                            SevaType = node["SevaType"] != null ? node["SevaType"].InnerText.Trim() : string.Empty,
                            MaxNoOfTickesAllowed = node["MaxNoOFTickesAllowed"] != null ? node["MaxNoOFTickesAllowed"].InnerText.Trim() : string.Empty,
                            IsActive = node["IsActive"] != null ? node["IsActive"].InnerText.Trim() : string.Empty,
                            PrasadamRequire = node["PrasadamRequire"] != null ? node["PrasadamRequire"].InnerText.Trim() : string.Empty,
                            SevaCategoryId = node["sevacategoryid"] != null ? node["sevacategoryid"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetSevaDetails: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetSLNSevaDetails(object reqData)
        {
            TempleSevaDetailReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<TempleSevaDetailReq>(reqData.ToString());
            }
            catch { }


            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetSevaDetails(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service), request.TempleId, request.SevaId, request.BatchId);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetSevaDetails => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new TempleSLNSevaDetailResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.SevaDetails.Add(new SLMSevaDetail
                        {
                            SevaId = node["sevaid"] != null ? node["sevaid"].InnerText.Trim() : string.Empty,
                            SevaName = node["Seva_name"] != null ? node["Seva_name"].InnerText.Trim() : string.Empty,
                            SevaAmount = node["SevaAmount"] != null ? node["SevaAmount"].InnerText.Trim() : string.Empty,
                            UserCharge = node["Usercharges"] != null ? node["Usercharges"].InnerText.Trim() : string.Empty,
                            SevaDescription = node["SevaDescription"] != null ? node["SevaDescription"].InnerText.Trim() : string.Empty,
                            ReportingTime = node["ReportingTime"] != null ? node["ReportingTime"].InnerText.Trim() : string.Empty,
                            NoOfPersons = node["No_of_Persons"] != null ? node["No_of_Persons"].InnerText.Trim() : string.Empty,
                            SevaType = node["SevaType"] != null ? node["SevaType"].InnerText.Trim() : string.Empty,
                            MaxNoOfTickesAllowed = node["MaxNoOFTickesAllowed"] != null ? node["MaxNoOFTickesAllowed"].InnerText.Trim() : string.Empty,
                            Laddu = node["Laddu"] != null ? node["Laddu"].InnerText.Trim() : string.Empty,
                            BigLaddu = node["BigLaddu"] != null ? node["BigLaddu"].InnerText.Trim() : string.Empty,
                            Pulihora = node["pulihora"] != null ? node["pulihora"].InnerText.Trim() : string.Empty,
                            Other = node["Other"] != null ? node["Other"].InnerText.Trim() : string.Empty,
                            IsActive = node["IsActive"] != null ? node["IsActive"].InnerText.Trim() : string.Empty,
                            PrasadamRequire = node["PrasadamRequire"] != null ? node["PrasadamRequire"].InnerText.Trim() : string.Empty,
                            SevaCategoryId = node["sevacategoryid"] != null ? node["sevacategoryid"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetSevaDetails: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #region [ ROOM BOOKING ]

        public MSResponse PopulateRoomType(object reqData)
        {
            try
            {
                var clientResp = mesevaClient.PopulateRoomType(CacheConfig.UserName, CacheConfig.Password);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateRoomType => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new RoomTypeResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.RoomTypes.Add(new Detail
                        {
                            Code = node["RoomTypeId"] != null ? node["RoomTypeId"].InnerText.Trim() : string.Empty,
                            Description = node["RoomTypeName"] != null ? node["RoomTypeName"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateRoomType: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateTempleRooms(object reqData)
        {
            TempleRoomReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<TempleRoomReq>(reqData.ToString());
            }
            catch { }


            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.PopulateRooms(CacheConfig.UserName, CacheConfig.Password, request.RoomTypeId, request.TempleId);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateTempleRooms => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new TempleRoomDetailResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.RoomsDetail.Add(new TempleRoomDetail
                        {
                            ID = node["ID"] != null ? node["ID"].InnerText.Trim() : string.Empty,
                            Name = node["Name"] != null ? node["Name"].InnerText.Trim() : string.Empty,
                            Persons = node["Persons"] != null ? node["Persons"].InnerText.Trim() : string.Empty,
                            Rent = node["Rent"] != null ? node["Rent"].InnerText.Trim() : string.Empty,
                            Rooms = node["Rooms"] != null ? node["Rooms"].InnerText.Trim() : string.Empty,
                            Category = node["Category"] != null ? node["Category"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateTempleRooms: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetAvailableRoomsDate(object reqData)
        {
            AvailableRoomsDateReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<AvailableRoomsDateReq>(reqData.ToString());
            }
            catch { }


            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetAvailableRoomsDate(CacheConfig.UserName, CacheConfig.Password, request.TempleId, request.RoomId, request.CheckinDate);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetAvailableRoomsDate => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new AvailableRoomsDateResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlElement item in dataNodes[0].ChildNodes)
                    {
                        response.RoomsDates.Add(new AvailableRoomsDate
                        {
                            Date = item.Name.Substring(item.Name.LastIndexOf("_") - 1).Replace("_", "").Replace("-", "/"),
                            NoOfRooms = item.InnerText
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetAvailableRoomsDate: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetEndowmentRoomAmount(object reqData)
        {
            EndowmentRoomAmountReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<EndowmentRoomAmountReq>(reqData.ToString());
            }
            catch { }


            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetEndowmentRoomAmount(CacheConfig.UserName, CacheConfig.Password, request.TempleId, request.RoomId);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetEndowmentRoomAmount => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectSingleNode("Table1");
                if (dataNodes != null)
                {
                    return new EndowmentRoomAmount
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        TempleId = dataNodes["TempleID"] != null ? dataNodes["TempleID"].InnerText.Trim() : string.Empty,
                        RoomId = dataNodes["RoomID"] != null ? dataNodes["RoomID"].InnerText.Trim() : string.Empty,
                        UserCharge = dataNodes["User_Charges"] != null ? dataNodes["User_Charges"].InnerText.Trim() : string.Empty,
                        RoomAmount = dataNodes["RoomAmount"] != null ? dataNodes["RoomAmount"].InnerText.Trim() : string.Empty
                    };
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetEndowmentRoomAmount: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetRoomBookingTransactionNo(object reqData)
        {
            RoomBookingTransactionNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<RoomBookingTransactionNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetRoomBookingTransactionNo(CacheConfig.UserName, CacheConfig.Password, request.MobileNo, CacheConfig.GetServiceId(request.Service),
                    request.TempleId, request.RoomType, request.RoomID, request.RoombookingDate, request.ApplicationNo, request.Profile.AadhaarNo, request.Profile.DevoteeName,
                    request.Document.ProofDocumentID, request.Document.ProofDocumentNumber, request.Document.ProofDocumentName, request.Profile.Gothram, request.Profile.Nakshatram,
                    request.Profile.Gender, request.Profile.Age, request.MobileNo, request.Profile.EmailId, request.Profile.HouseNo, request.Profile.Location, request.Profile.CountryId,
                    request.Profile.StateId, request.Profile.District, request.Profile.Mandal, request.Profile.Village, request.Profile.Pincode, request.Profile.StateName,
                    request.Profile.DistrictName, request.Profile.MandalName, request.Profile.VillageName, request.ApplicantName, request.ApplicantRelation, request.Charge.RoomAmount,
                    request.Charge.Usercharges, request.Charge.TotalAmount, request.Document.DocApplicationform, request.Document.DocIDProof);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetRoomBookingTransactionNo => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode != null && respNode.InnerText.Trim() == "100")
                    return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText };

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetRoomBookingTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ ROOM BOOKING ]

        public MSResponse GetSevaBookingTransactionNo(object reqData)
        {
            SevaBookingTransactionNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<SevaBookingTransactionNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetSevaBookingTransactionNo(CacheConfig.UserName, CacheConfig.Password, request.MobileNo, CacheConfig.GetServiceId(request.Service),
                    request.TempleId, request.SevaId, request.BatchId, request.SevaBookingDate, request.ApplicationNo, request.Profile.AadhaarNo,
                    request.Profile.DevoteeName, request.Profile.Nakshatram, request.Profile.Gothram, request.Profile.Gender, request.Profile.Age, request.Profile.HouseNo,
                    request.Profile.Location, request.MobileNo, request.Profile.EmailId, request.Profile.CountryId, request.Profile.StateId, request.Profile.District,
                    request.Profile.Mandal, request.Profile.Village, request.Profile.Pincode, request.Profile.StateName, request.Profile.DistrictName, request.Profile.MandalName,
                    request.Profile.VillageName, request.Document.ProofDocumentID, request.Document.ProofDocumentNumber, request.Document.ProofDocumentName, request.DeliveryType,
                    request.Charge.SevaAmount, request.Charge.Usercharges, request.Charge.TotalAmount, request.ApplicantName, request.Profile.RelationId, request.Document.DocApplicationform,
                    request.Document.DocIDProof, request.Document.DocPhoto);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetSevaBookingTransactionNo => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode != null && respNode.InnerText.Trim() == "100")
                    return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText };

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetSevaBookingTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetSLNSDSevaBookingTransactionNo(object reqData)
        {
            SevaBookingTransactionNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<SevaBookingTransactionNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetSriLaxmiNarasimhaSwamiwariDevastanamSevaBookingTransactionNo(CacheConfig.UserName, CacheConfig.Password, request.MobileNo, CacheConfig.GetServiceId(request.Service),
                    request.TempleId, request.SevaId, request.BatchId, request.SevaBookingDate, request.ApplicationNo, request.Profile.AadhaarNo,
                    request.Profile.DevoteeName, request.Profile.Nakshatram, request.Profile.Gothram, request.Profile.Gender, request.Profile.Age, request.Profile.HouseNo,
                    request.Profile.Location, request.MobileNo, request.Profile.EmailId, request.Profile.CountryId, request.Profile.StateId, request.Profile.District,
                    request.Profile.Mandal, request.Profile.Village, request.Profile.Pincode, request.Profile.StateName, request.Profile.DistrictName, request.Profile.MandalName,
                    request.Profile.VillageName, request.Document.ProofDocumentID, request.Document.ProofDocumentNumber, request.Document.ProofDocumentName, request.DeliveryType,
                    request.Charge.SevaAmount, request.Charge.Usercharges, request.Charge.TotalAmount, request.ApplicantName, request.Profile.RelationId, request.Document.DocApplicationform,
                    request.Document.DocIDProof);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetSLNSDSevaBookingTransactionNo => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode != null && respNode.InnerText.Trim() == "100")
                    return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText };

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetSLNSDSevaBookingTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ TEMPLE SEVA BOOKING SERVICES ]

        #region [ ROR CERTIFICATE SERVICES ]

        public MSResponse PopulateRORDetails(object reqData)
        {
            RORDetailReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<RORDetailReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.PopulateRORDetails(CacheConfig.UserName, CacheConfig.Password, request.ApplicationNo, request.DistrictId, request.MandalId, request.VillageId, request.KathaNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateRORDetails => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("dtRORDetails");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    return new RORDetail
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        SerialNo = dataNodes[0]["pSerialNo"] != null ? dataNodes[0]["pSerialNo"].InnerText.Trim() : string.Empty,
                        Surveyno = dataNodes[0]["pSurvey_no"] != null ? dataNodes[0]["pSurvey_no"].InnerText.Trim() : string.Empty,
                        TotalExtent = dataNodes[0]["pTotal_Extent"] != null ? dataNodes[0]["pTotal_Extent"].InnerText.Trim() : string.Empty,
                        UncultivatedLand = dataNodes[0]["pUncultivated_Land"] != null ? dataNodes[0]["pUncultivated_Land"].InnerText.Trim() : string.Empty,
                        CultivatableLand = dataNodes[0]["pCultivatable_Land"] != null ? dataNodes[0]["pCultivatable_Land"].InnerText.Trim() : string.Empty,
                        LandNature = dataNodes[0]["pLand_Nature"] != null ? dataNodes[0]["pLand_Nature"].InnerText.Trim() : string.Empty,
                        Tax = dataNodes[0]["village_id"] != null ? dataNodes[0]["village_id"].InnerText.Trim() : string.Empty,
                        LandClassification = dataNodes[0]["pLand_Classification"] != null ? dataNodes[0]["pLand_Classification"].InnerText.Trim() : string.Empty,
                        WaterResource = dataNodes[0]["pWater_Resource"] != null ? dataNodes[0]["pWater_Resource"].InnerText.Trim() : string.Empty,
                        AyakatExtent = dataNodes[0]["pAyakat_Extent"] != null ? dataNodes[0]["pAyakat_Extent"].InnerText.Trim() : string.Empty,
                        KhataNumber = dataNodes[0]["pKhata_Number"] != null ? dataNodes[0]["pKhata_Number"].InnerText.Trim() : string.Empty,
                        PattadarName = dataNodes[0]["pPattadar_Name"] != null ? dataNodes[0]["pPattadar_Name"].InnerText.Trim() : string.Empty,
                        OccupantName = dataNodes[0]["pOccupant_Name"] != null ? dataNodes[0]["pOccupant_Name"].InnerText.Trim() : string.Empty,
                        OccupantExtent = dataNodes[0]["pOccupant_Extent"] != null ? dataNodes[0]["pOccupant_Extent"].InnerText.Trim() : string.Empty,
                        EnjoymentNature = dataNodes[0]["pEnjoyment_Nature"] != null ? dataNodes[0]["pEnjoyment_Nature"].InnerText.Trim() : string.Empty,
                        OccupantFatherName = dataNodes[0]["pOccupant_Father_Name"] != null ? dataNodes[0]["pOccupant_Father_Name"].InnerText.Trim() : string.Empty,
                        PattadarFatherName = dataNodes[0]["pPattadar_Father_Name"] != null ? dataNodes[0]["pPattadar_Father_Name"].InnerText.Trim() : string.Empty,
                        VillageName = dataNodes[0]["pVillage_Name"] != null ? dataNodes[0]["pVillage_Name"].InnerText.Trim() : string.Empty,
                        VillageCode = dataNodes[0]["pVillage_Code"] != null ? dataNodes[0]["pVillage_Code"].InnerText.Trim() : string.Empty,
                        DeleteFlag = dataNodes[0]["pDelete_Flag"] != null ? dataNodes[0]["pDelete_Flag"].InnerText.Trim() : string.Empty,
                        MutatedDate = dataNodes[0]["pMutated_Date"] != null ? dataNodes[0]["pMutated_Date"].InnerText.Trim() : string.Empty,
                        BasesurveyNo = dataNodes[0]["pBase_survey_No"] != null ? dataNodes[0]["pBase_survey_No"].InnerText.Trim() : string.Empty,
                        Signature = dataNodes[0]["pSignature"] != null ? dataNodes[0]["pSignature"].InnerText.Trim() : string.Empty,
                        SignatureChecked = dataNodes[0]["pSignatureChecked"] != null ? dataNodes[0]["pSignatureChecked"].InnerText.Trim() : string.Empty,
                        AcquiredReason = dataNodes[0]["pAcquiredReason"] != null ? dataNodes[0]["pAcquiredReason"].InnerText.Trim() : string.Empty,
                        VerifiedBy = dataNodes[0]["verifiedBy"] != null ? dataNodes[0]["verifiedBy"].InnerText.Trim() : string.Empty,
                        MeesevaAppliNo = dataNodes[0]["MeesevaAppliNo"] != null ? dataNodes[0]["MeesevaAppliNo"].InnerText.Trim() : string.Empty
                    };
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateRORDetails: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetRORTransactionNo(object reqData)
        {
            RORTransactionNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<RORTransactionNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetROR1BTransactionNo(CacheConfig.UserName, CacheConfig.Password, request.MobileNo, CacheConfig.GetServiceId(request.Service),
                    request.AddressFlag, request.Document.DocDistrictId, request.Document.DocMandalId, request.Document.DocVillageId,
                    request.Document.DocKhataNo, request.ApplicationNo, request.ApplicantName, request.Profile.ApplicantFatherName, request.Profile.ApplicantDoorNo,
                    request.Profile.Gender, request.Profile.ApplicationStreetname, request.Profile.ApplicantState, request.Profile.ApplicantDistrict, request.Profile.ApplicantMandal,
                    request.Profile.ApplicantVillage, request.Profile.PinCode, request.MobileNo, request.DeliveryType, request.Charge.ServiceCharge,
                    request.Charge.PostalCharge, request.Charge.UserCharge, request.Charge.TotalAmount, request.Profile.PostalState, request.Profile.PostalDistrct,
                    request.Profile.PostalMandal, request.Profile.PostalVillage, request.Profile.PostalPincode, request.Profile.PostalLocation, request.Profile.PostalDoorNo,
                    request.Profile.RationcardNo, request.Profile.AadhaarNo, request.Profile.EmailId);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetRORTransactionNo => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode != null && respNode.InnerText.Trim() == "100")
                    return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText };

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetRORTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetRORCertificatePDF(object reqData)
        {
            ApplicationNoBasedReq request = null;
            try
            {
                request = JsonConvert.DeserializeObject<ApplicationNoBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetROR1BCertificatePDF(CacheConfig.UserName, CacheConfig.Password, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetRORCertificatePDF => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                if (root.SelectSingleNode("Table1/PDF") != null)
                {
                    return new CertificatePDFResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        PDFDocument = root.SelectSingleNode("Table1/PDF").InnerText.Trim()
                    };
                }

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetRORCertificatePDF: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ ROR CERTIFICATE SERVICES ]

        #region [ NOC PETROLEUM SERVICES ]

        public MSResponse PopulateLicenceType(object reqData)
        {
            try
            {
                var clientResp = mesevaClient.PopulateLicenceType(CacheConfig.UserName, CacheConfig.Password);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateLicenceType => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new LicenceTypeResp { ResCode = "000", ResDesc = "Success" };
                    foreach (XmlNode node in dataNodes)
                    {
                        response.LicenceTypes.Add(new Detail
                        {
                            Code = node["LicenceTypeID"] != null ? node["LicenceTypeID"].InnerText.Trim() : string.Empty,
                            Description = node["LicenceTypeDescription"] != null ? node["LicenceTypeDescription"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateLicenceType: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetPetroleumNOCTransactionNo(object reqData)
        {
            PetroleumNOCTransactionNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<PetroleumNOCTransactionNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetIssueOfNoForStoringOfPetroleumProductsTransactionNo(CacheConfig.UserName, CacheConfig.Password, request.MobileNo,
                    CacheConfig.GetServiceId(request.Service), request.AddressFlag, request.ApplicationNo, request.ApplicantName, request.ApplicantCalling,
                    request.FatherOrHusbandName, request.AadhaarNo, request.PermanentDistrict, request.PermanentMandal, request.PermanentVillage, request.PermanentLocality,
                    request.PermanentDoorNo, request.PermanentPincode, request.DocDistrict, request.DocMandal, request.DocVillage, request.DOCPincode, request.NearestPoliceStation,
                    request.NearestRailwayStation, request.SurveyNumber, request.LicenceNumber, request.LicenceType, request.NoOfLicence, request.ImpAIBQuantityInlitres,
                    request.ImpANIB, request.ImpATotal, request.ImpBIB, request.ImpBNIB, request.ImpBTotal, request.ImpCIB, request.ImpCNIB, request.ImpCTotal, request.ImpTotal,
                    request.StoredAIBQuantityInlitres, request.StoredANIB, request.StoredATotal, request.StoredBIB, request.StoredBNIB, request.StoredBTotal, request.StoredCIB,
                    request.StoredCNIB, request.StoredCTotal, request.StoredTotal, request.InfName, request.InfRelation, request.InfEmail, request.InfMobileNo, request.DeliveryType,
                    request.StateId, request.PostalDistrict, request.PostalMandal, request.PostalVillage, request.PostalMobile, request.PostalLocality, request.PostalDoorNo,
                    request.PostalPincode, request.Phone, request.Charge.UserCharge, request.Charge.PostalCharge, request.Charge.ServiceCharge, request.Charge.TotalAmount, request.DocApplicationform,
                    request.DocSitePlan, request.DocCopyofPassbookOrtitledeedOrsaledeed, request.DocLeaseAgreement, request.DocLetterofoilcompany, request.DocExtractPahani);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetPetroleumNOCTransactionNo => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode != null && respNode.InnerText.Trim() == "100")
                    return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText };

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetPetroleumNOCTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetPetroleumProductsCertificatePDF(object reqData)
        {
            ApplicationNoBasedReq request = null;
            try
            {
                request = JsonConvert.DeserializeObject<ApplicationNoBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetPetroleumProductsCertificatePDF(CacheConfig.UserName, CacheConfig.Password, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetPetroleumProductsCertificatePDF => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                if (root.SelectSingleNode("Table1/PDF") != null)
                {
                    return new CertificatePDFResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        PDFDocument = root.SelectSingleNode("Table1/PDF").InnerText.Trim()
                    };
                }

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetPetroleumProductsCertificatePDF: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ NOC PETROLEUM SERVICES ]

        #region [ INAM LAND SERVICES ]

        public MSResponse GetINAMLANDSTransactionNo(object reqData)
        {
            INAMLANDSTransactionNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<INAMLANDSTransactionNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetIssueOfOccupancyRightScertForINAMLANDSTransactionNo(CacheConfig.UserName, CacheConfig.Password, request.MobileNo,
                    CacheConfig.GetServiceId(request.Service), request.ApplicationNo, request.AadharNo, request.ApplicantName,
                    request.FatherName, request.State, request.ApplicantDistrict, request.ApplicantMandal, request.ApplicantVillage, request.ApplicantDoorNo, request.ApplicantLocality,
                    request.ApplicantPincode, request.ApplicantInamdarYesNo, request.RelationShipInamdar, request.NatureOfInterests, request.RevenueAmountPaid,
                    request.LandDistrict, request.LandMandal, request.GridINAMLandsDetails, request.InformantName, request.RelationshipApp, request.InformantEmail,
                    request.InformantMobileNo, request.DeliveryType, request.ServiceCharge, request.ChallanAmount, request.UserCharge, request.PostalCharge, request.TotalAmount,
                    request.DocApplicationform, request.Doc1954_1955KasaraPahani, request.Doc73_74LatestPahaniCopies, request.DocFamilyTree, request.DocRelationshipWithInamdhar);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetINAMLANDSTransactionNo => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode != null && respNode.InnerText.Trim() == "100")
                    return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText };

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetINAMLANDSTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetINAMLANDSCertificatePDF(object reqData)
        {
            ApplicationNoBasedReq request = null;
            try
            {
                request = JsonConvert.DeserializeObject<ApplicationNoBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetINAMLANDSCertificatePDF(CacheConfig.UserName, CacheConfig.Password, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetINAMLANDSCertificatePDF => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                if (root.SelectSingleNode("Table1/Data_Available") != null && root.SelectSingleNode("Table1/Data_Available").InnerText.Trim().ToUpper() == "Y"
                    && root.SelectSingleNode("Table/IS_PDF") != null && root.SelectSingleNode("Table/IS_PDF").InnerText.Trim().Trim() == "Y"
                    && root.SelectSingleNode("Table2/PDF") != null)
                {
                    return new CertificatePDFResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        PDFDocument = root.SelectSingleNode("Table2/PDF").InnerText.Trim()
                    };
                }

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetINAMLANDSCertificatePDF: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ INAM LAND SERVICES ]

        #region [ BIRTH/DEATH GHMC SERVICES ]

        public MSResponse GHMCBirthOrDeathRecord(object reqData)
        {
            GHMCBirthOrDeathSearchRecordReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<GHMCBirthOrDeathSearchRecordReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GHMCBirthOrDeathSearchRecord(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service),
                    request.ServiceType, request.DateOfBirth, request.RegistrationNo, request.CircleNo, request.Gender, request.MotherName, request.FatherHusbandName, request.DeceasedName);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GHMCBirthOrDeathSearchRecord => Response: {0}", clientResp));
                if (clientResp == null)
                    new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectSingleNode("Table");
                if (dataNodes != null)
                {
                    var response = new GHMCBirthOrDeathSearchRecordResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        CircleNo = dataNodes["CIRCLE_NO"] != null ? dataNodes["CIRCLE_NO"].InnerText.Trim() : string.Empty,
                        AckNo = dataNodes["ACK_NO"] != null ? dataNodes["ACK_NO"].InnerText.Trim() : string.Empty,
                        ChildName = dataNodes["CHILD_NAME"] != null ? dataNodes["CHILD_NAME"].InnerText.Trim() : string.Empty,
                        DOB = dataNodes["DOB"] != null ? dataNodes["DOB"].InnerText.Trim() : string.Empty,
                        Sex = dataNodes["SEX"] != null ? dataNodes["SEX"].InnerText.Trim() : string.Empty,
                        MotherName = dataNodes["MOTHER_NAME"] != null ? dataNodes["MOTHER_NAME"].InnerText.Trim() : string.Empty,
                        FatherHusbandName = dataNodes["FATHER_NAME"] != null ? dataNodes["FATHER_NAME"].InnerText.Trim() : string.Empty,
                        HospitalName = dataNodes["HOSPITAL_NAME"] != null ? dataNodes["HOSPITAL_NAME"].InnerText.Trim() : string.Empty,
                        ResidenceAddress = dataNodes["RESIDENCE_ADDRESS"] != null ? dataNodes["RESIDENCE_ADDRESS"].InnerText.Trim() : string.Empty,
                        Locality = dataNodes["LOCALITY"] != null ? dataNodes["LOCALITY"].InnerText.Trim() : string.Empty,
                        TypeOfHospital = dataNodes["TYPE_OF_HOSPITAL"] != null ? dataNodes["TYPE_OF_HOSPITAL"].InnerText.Trim() : string.Empty,
                        RegistrationNo = dataNodes["REG_NO"] != null ? dataNodes["REG_NO"].InnerText.Trim() : string.Empty,
                        WardNo = dataNodes["WARD_NO"] != null ? dataNodes["WARD_NO"].InnerText.Trim() : string.Empty,
                        RegistrationDate = dataNodes["REG_DATE"] != null ? dataNodes["REG_DATE"].InnerText.Trim() : string.Empty,
                        SignedBy = dataNodes["SIGNED_BY"] != null ? dataNodes["SIGNED_BY"].InnerText.Trim() : string.Empty,
                        SignedDate = dataNodes["SIGNED_DATE"] != null ? dataNodes["SIGNED_DATE"].InnerText.Trim() : string.Empty,
                        HMExists = dataNodes["HMExists"] != null ? dataNodes["HMExists"].InnerText.Trim() : string.Empty
                    };

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GHMCBirthOrDeathSearchRecord: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GHMCBirthDeathDataByAckNo(object reqData)
        {
            GHMCBirthDeathDataByAckNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<GHMCBirthDeathDataByAckNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GHMCBirthDeathDataByAckNo(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service), request.ServiceType, request.AckNo);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GHMCBirthDeathDataByAckNo => Response: {0}", clientResp));
                if (clientResp == null)
                    new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectSingleNode("Table");
                if (dataNodes != null)
                {
                    var response = new GHMCBirthOrDeathSearchRecordResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        CircleNo = dataNodes["CIRCLE_NO"] != null ? dataNodes["CIRCLE_NO"].InnerText.Trim() : string.Empty,
                        AckNo = dataNodes["ACK_NO"] != null ? dataNodes["ACK_NO"].InnerText.Trim() : string.Empty,
                        ChildName = dataNodes["CHILD_NAME"] != null ? dataNodes["CHILD_NAME"].InnerText.Trim() : string.Empty,
                        DOB = dataNodes["DOB"] != null ? dataNodes["DOB"].InnerText.Trim() : string.Empty,
                        Sex = dataNodes["SEX"] != null ? dataNodes["SEX"].InnerText.Trim() : string.Empty,
                        MotherName = dataNodes["MOTHER_NAME"] != null ? dataNodes["MOTHER_NAME"].InnerText.Trim() : string.Empty,
                        FatherHusbandName = dataNodes["FATHER_NAME"] != null ? dataNodes["FATHER_NAME"].InnerText.Trim() : string.Empty,
                        HospitalName = dataNodes["HOSPITAL_NAME"] != null ? dataNodes["HOSPITAL_NAME"].InnerText.Trim() : string.Empty,
                        ResidenceAddress = dataNodes["RESIDENCE_ADDRESS"] != null ? dataNodes["RESIDENCE_ADDRESS"].InnerText.Trim() : string.Empty,
                        Locality = dataNodes["LOCALITY"] != null ? dataNodes["LOCALITY"].InnerText.Trim() : string.Empty,
                        TypeOfHospital = dataNodes["TYPE_OF_HOSPITAL"] != null ? dataNodes["TYPE_OF_HOSPITAL"].InnerText.Trim() : string.Empty,
                        RegistrationNo = dataNodes["REG_NO"] != null ? dataNodes["REG_NO"].InnerText.Trim() : string.Empty,
                        WardNo = dataNodes["WARD_NO"] != null ? dataNodes["WARD_NO"].InnerText.Trim() : string.Empty,
                        RegistrationDate = dataNodes["REG_DATE"] != null ? dataNodes["REG_DATE"].InnerText.Trim() : string.Empty,
                        SignedBy = dataNodes["SIGNED_BY"] != null ? dataNodes["SIGNED_BY"].InnerText.Trim() : string.Empty,
                        SignedDate = dataNodes["SIGNED_DATE"] != null ? dataNodes["SIGNED_DATE"].InnerText.Trim() : string.Empty
                    };

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GHMCBirthDeathDataByAckNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetGHMCServiceCharges(object reqData)
        {
            GHMCServiceChargeReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<GHMCServiceChargeReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetServiceCharges_GHMC(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service), request.ServiceType, request.DeliveryType, request.NoOfCopies);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetGHMCServiceCharges => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNode = root.SelectSingleNode("dtcharges");
                if (dataNode != null)
                {
                    return new GHMCServiceChargeResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        ServiceId = dataNode["service_id"] != null ? dataNode["service_id"].InnerText.Trim() : string.Empty,
                        ServiceAmount = dataNode["ServiceAmount"] != null ? dataNode["ServiceAmount"].InnerText.Trim() : string.Empty,
                        DeliveryType = dataNode["delivery_type"] != null ? dataNode["delivery_type"].InnerText.Trim() : string.Empty,
                        DeliveryCharges = dataNode["Delivery_Charges"] != null ? dataNode["Delivery_Charges"].InnerText.Trim() : string.Empty,
                        ChallanAmount = dataNode["challan_amount"] != null ? dataNode["challan_amount"].InnerText.Trim() : string.Empty,
                        SLA = dataNode["SLA"] != null ? dataNode["SLA"].InnerText.Trim() : string.Empty,
                        UserCharges = dataNode["User_Charges"] != null ? dataNode["User_Charges"].InnerText.Trim() : string.Empty,
                        TotalAmount = dataNode["TotalAmount"] != null ? dataNode["TotalAmount"].InnerText.Trim() : string.Empty
                    };
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetGHMCServiceCharges: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetGHMCBirthDeathTransactionNo(object reqData)
        {
            GHMCBirthDeathTransactionNo request = null;

            try
            {
                request = JsonConvert.DeserializeObject<GHMCBirthDeathTransactionNo>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetGHMCBirthDeathTransactionNo(CacheConfig.UserName, CacheConfig.Password, request.MobileNo, CacheConfig.GetServiceId(request.Service),
                    request.ServiceType, request.ApplicationNo, request.AcknowledgementNo, request.RegistrationNumber, request.EventName,
                    request.EventDate, request.Gender, request.Circle, request.Ward, request.Locality, request.FatherHusbandName, request.MotherName, request.ResidenceAddress,
                    request.PermanentAddress, request.RegDate, request.DeathCause, request.PlaceOfEvent, request.Remarks, request.InformantName, request.InformantRelation,
                    request.InformantAddress, request.InformantPhoneNo, request.AadhaarNo, request.RationCardNo, request.InformantEmailId, request.InformantRemarks,
                    request.InformantPinCode, request.DeliveryType, request.NumberOfCopies, request.Purpose, request.postalDoorNo, request.postalDistrict, request.postalMandalId,
                    request.postalVillageId, request.postalPinCode, request.Charge.ServiceCharge, request.Charge.PostalCharge, request.Charge.UserCharge, request.Charge.TotalAmount,
                    request.DocApplicationform);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetGHMCBirthDeathTransactionNo => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode != null && respNode.InnerText.Trim() == "100")
                    return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText };

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetGHMCBirthDeathTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetGHMCBDCertificatePDF(object reqData)
        {
            GHMCBDCertificatePDFReq request = null;
            try
            {
                request = JsonConvert.DeserializeObject<GHMCBDCertificatePDFReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetGHMCBirthDeathCertificatePDF(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service), request.ServiceType, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetGHMCBDCertificatePDF => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                if (root.SelectSingleNode("Table1/PDF") != null)
                {
                    return new CertificatePDFResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        PDFDocument = root.SelectSingleNode("Table1/PDF").InnerText.Trim()
                    };
                }

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetGHMCBDCertificatePDF: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ BIRTH/DEATH GHMC SERVICES ]

        #region [ BIRTH/DEATH CORRECTION GHMC SERVICES ]

        public MSResponse GHMCSearchBirthRecord(object reqData)
        {
            GHMCBirthOrDeathSearchRecordReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<GHMCBirthOrDeathSearchRecordReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GHMCSearchBirthRecord(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service),
                    request.ServiceType, request.DateOfBirth, request.RegistrationNo, request.CircleNo, request.Gender, request.MotherName, request.FatherHusbandName);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GHMCSearchBirthRecord => Response: {0}", clientResp));
                if (clientResp == null)
                    new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectSingleNode("Table");
                if (dataNodes != null)
                {
                    var response = new GHMCSearchBirthRecordResp { ResCode = "000", ResDesc = "Success" };
                    response.BirthRecords.Add(new GHMCBirthDeathDataByAckNoResp
                    {
                        CircleNo = dataNodes["CIRCLE_NO"] != null ? dataNodes["CIRCLE_NO"].InnerText.Trim() : string.Empty,
                        AckNo = dataNodes["ACK_NO"] != null ? dataNodes["ACK_NO"].InnerText.Trim() : string.Empty,
                        ChildName = dataNodes["CHILD_NAME"] != null ? dataNodes["CHILD_NAME"].InnerText.Trim() : string.Empty,
                        DOB = dataNodes["DOB"] != null ? dataNodes["DOB"].InnerText.Trim() : string.Empty,
                        Sex = dataNodes["SEX"] != null ? dataNodes["SEX"].InnerText.Trim() : string.Empty,
                        MotherName = dataNodes["MOTHER_NAME"] != null ? dataNodes["MOTHER_NAME"].InnerText.Trim() : string.Empty,
                        FatherHusbandName = dataNodes["FATHER_NAME"] != null ? dataNodes["FATHER_NAME"].InnerText.Trim() : string.Empty,
                        HospitalName = dataNodes["HOSPITAL_NAME"] != null ? dataNodes["HOSPITAL_NAME"].InnerText.Trim() : string.Empty,
                        ResidenceAddress = dataNodes["RESIDENCE_ADDRESS"] != null ? dataNodes["RESIDENCE_ADDRESS"].InnerText.Trim() : string.Empty,
                        Locality = dataNodes["LOCALITY"] != null ? dataNodes["LOCALITY"].InnerText.Trim() : string.Empty,
                        TypeOfHospital = dataNodes["TYPE_OF_HOSPITAL"] != null ? dataNodes["TYPE_OF_HOSPITAL"].InnerText.Trim() : string.Empty,
                        RegistrationNo = dataNodes["REG_NO"] != null ? dataNodes["REG_NO"].InnerText.Trim() : string.Empty,
                        WardNo = dataNodes["WARD_NO"] != null ? dataNodes["WARD_NO"].InnerText.Trim() : string.Empty,
                        RegistrationDate = dataNodes["REG_DATE"] != null ? dataNodes["REG_DATE"].InnerText.Trim() : string.Empty,
                        SignedBy = dataNodes["SIGNED_BY"] != null ? dataNodes["SIGNED_BY"].InnerText.Trim() : string.Empty,
                        SignedDate = dataNodes["SIGNED_DATE"] != null ? dataNodes["SIGNED_DATE"].InnerText.Trim() : string.Empty
                    });

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GHMCSearchBirthRecord: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GHMCSearchDeathRecord(object reqData)
        {
            GHMCBirthOrDeathSearchRecordReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<GHMCBirthOrDeathSearchRecordReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GHMCSearchDeathRecord(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service),
                    request.ServiceType, request.DateOfBirth, request.RegistrationNo, request.CircleNo, request.Gender, request.DeceasedName, request.FatherHusbandName);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GHMCSearchDeathRecord => Response: {0}", clientResp));
                if (clientResp == null)
                    new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectSingleNode("Table");
                if (dataNodes != null)
                {
                    var response = new GHMCSearchDeathRecordResp { ResCode = "000", ResDesc = "Success" };
                    response.DeathRecords.Add(new GHMCSearchDeathRecord
                    {
                        CircleNo = dataNodes["CIRCLE_NO"] != null ? dataNodes["CIRCLE_NO"].InnerText.Trim() : string.Empty,
                        AckNo = dataNodes["ACK_NO"] != null ? dataNodes["ACK_NO"].InnerText.Trim() : string.Empty,
                        DeceasedName = dataNodes["DECEASED_NAME"] != null ? dataNodes["DECEASED_NAME"].InnerText.Trim() : string.Empty,
                        DOD = dataNodes["DOD"] != null ? dataNodes["DOD"].InnerText.Trim() : string.Empty,
                        Sex = dataNodes["SEX"] != null ? dataNodes["SEX"].InnerText.Trim() : string.Empty,
                        MotherName = dataNodes["MOTHER_NAME"] != null ? dataNodes["MOTHER_NAME"].InnerText.Trim() : string.Empty,
                        FatherHusbandName = dataNodes["FATHER_NAME"] != null ? dataNodes["FATHER_NAME"].InnerText.Trim() : string.Empty,
                        HospitalName = dataNodes["HOSPITAL_NAME"] != null ? dataNodes["HOSPITAL_NAME"].InnerText.Trim() : string.Empty,
                        PresentAddress = dataNodes["PRESENT_RES_ADDRESS"] != null ? dataNodes["PRESENT_RES_ADDRESS"].InnerText.Trim() : string.Empty,
                        PermanentAddress = dataNodes["PERMANENT_ADDRESS"] != null ? dataNodes["PERMANENT_ADDRESS"].InnerText.Trim() : string.Empty,
                        DeathLocation = dataNodes["DEATH_LOCATION"] != null ? dataNodes["DEATH_LOCATION"].InnerText.Trim() : string.Empty,
                        DeathCause = dataNodes["DEATH_CAUSE"] != null ? dataNodes["DEATH_CAUSE"].InnerText.Trim() : string.Empty,
                        RegistrationNo = dataNodes["REG_NO"] != null ? dataNodes["REG_NO"].InnerText.Trim() : string.Empty,
                        WardNo = dataNodes["WARD_NO"] != null ? dataNodes["WARD_NO"].InnerText.Trim() : string.Empty,
                        RegistrationDate = dataNodes["REG_DATE"] != null ? dataNodes["REG_DATE"].InnerText.Trim() : string.Empty,
                        SignedBy = dataNodes["SIGNED_BY"] != null ? dataNodes["SIGNED_BY"].InnerText.Trim() : string.Empty,
                        SignedDate = dataNodes["SIGNED_DATE"] != null ? dataNodes["SIGNED_DATE"].InnerText.Trim() : string.Empty
                    });

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GHMCSearchDeathRecord: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GHMCACNInclusionByAckNo(object reqData)
        {
            GHMCBirthDeathDataByAckNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<GHMCBirthDeathDataByAckNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GHMCorrectionAndChidlNameInclusionDataByAckNo(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service), request.ServiceType, request.AckNo);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GHMCorrectionAndChidlNameInclusionDataByAckNo => Response: {0}", clientResp));
                if (clientResp == null)
                    new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectSingleNode("Table");
                if (dataNodes != null)
                {
                    if (request.ServiceType.Trim() == "03" || request.ServiceType.Trim() == "07")
                    {
                        return new GHMCBirthDeathDataByAckNoResp
                        {
                            ResCode = "000",
                            ResDesc = "Success",
                            CircleNo = dataNodes["CIRCLE_NO"] != null ? dataNodes["CIRCLE_NO"].InnerText.Trim() : string.Empty,
                            AckNo = dataNodes["ACK_NO"] != null ? dataNodes["ACK_NO"].InnerText.Trim() : string.Empty,
                            ChildName = dataNodes["CHILD_NAME"] != null ? dataNodes["CHILD_NAME"].InnerText.Trim() : string.Empty,
                            DOB = dataNodes["DOB"] != null ? dataNodes["DOB"].InnerText.Trim() : string.Empty,
                            Sex = dataNodes["SEX"] != null ? dataNodes["SEX"].InnerText.Trim() : string.Empty,
                            MotherName = dataNodes["MOTHER_NAME"] != null ? dataNodes["MOTHER_NAME"].InnerText.Trim() : string.Empty,
                            FatherHusbandName = dataNodes["FATHER_OR_HUS_NAME"] != null ? dataNodes["FATHER_OR_HUS_NAME"].InnerText.Trim() : (dataNodes["FATHER_NAME"] != null ? dataNodes["FATHER_NAME"].InnerText.Trim() : string.Empty),
                            HospitalName = dataNodes["HOSPITAL_NAME"] != null ? dataNodes["HOSPITAL_NAME"].InnerText.Trim() : string.Empty,
                            TypeOfHospital = dataNodes["TYPE_OF_HOSPITAL"] != null ? dataNodes["TYPE_OF_HOSPITAL"].InnerText.Trim() : string.Empty,
                            ResidenceAddress = dataNodes["RESIDENCE_ADDRESS"] != null ? dataNodes["RESIDENCE_ADDRESS"].InnerText.Trim() : string.Empty,
                            Locality = dataNodes["LOCALITY"] != null ? dataNodes["LOCALITY"].InnerText.Trim() : string.Empty,
                            RegistrationNo = dataNodes["REG_NO"] != null ? dataNodes["REG_NO"].InnerText.Trim() : string.Empty,
                            WardNo = dataNodes["WARD_NO"] != null ? dataNodes["WARD_NO"].InnerText.Trim() : string.Empty,
                            RegistrationDate = dataNodes["REG_DATE"] != null ? dataNodes["REG_DATE"].InnerText.Trim() : string.Empty,
                            SignedBy = dataNodes["SIGNED_BY"] != null ? dataNodes["SIGNED_BY"].InnerText.Trim() : string.Empty,
                            SignedDate = dataNodes["SIGNED_DATE"] != null ? dataNodes["SIGNED_DATE"].InnerText.Trim() : string.Empty
                        };
                    }

                    if (request.ServiceType.Trim() == "04")
                    {
                        return new GHMCSearchDeathRecord
                        {
                            ResCode = "000",
                            ResDesc = "Success",
                            CircleNo = dataNodes["CIRCLE_NO"] != null ? dataNodes["CIRCLE_NO"].InnerText.Trim() : string.Empty,
                            AckNo = dataNodes["ACK_NO"] != null ? dataNodes["ACK_NO"].InnerText.Trim() : string.Empty,
                            DeceasedName = dataNodes["DECEASED_NAME"] != null ? dataNodes["DECEASED_NAME"].InnerText.Trim() : string.Empty,
                            DOD = dataNodes["DOD"] != null ? dataNodes["DOD"].InnerText.Trim() : string.Empty,
                            Sex = dataNodes["SEX"] != null ? dataNodes["SEX"].InnerText.Trim() : string.Empty,
                            MotherName = dataNodes["MOTHER_NAME"] != null ? dataNodes["MOTHER_NAME"].InnerText.Trim() : string.Empty,
                            FatherHusbandName = dataNodes["FATHER_OR_HUS_NAME"] != null ? dataNodes["FATHER_OR_HUS_NAME"].InnerText.Trim() : string.Empty,
                            HospitalName = dataNodes["HOSPITAL_NAME"] != null ? dataNodes["HOSPITAL_NAME"].InnerText.Trim() : string.Empty,
                            PresentAddress = dataNodes["PRESENT_RES_ADDRESS"] != null ? dataNodes["PRESENT_RES_ADDRESS"].InnerText.Trim() : string.Empty,
                            PermanentAddress = dataNodes["PERMANENT_ADDRESS"] != null ? dataNodes["PERMANENT_ADDRESS"].InnerText.Trim() : string.Empty,
                            DeathLocation = dataNodes["DEATH_LOCATION"] != null ? dataNodes["DEATH_LOCATION"].InnerText.Trim() : string.Empty,
                            DeathCause = dataNodes["DEATH_CAUSE"] != null ? dataNodes["DEATH_CAUSE"].InnerText.Trim() : string.Empty,
                            RegistrationNo = dataNodes["REG_NO"] != null ? dataNodes["REG_NO"].InnerText.Trim() : string.Empty,
                            WardNo = dataNodes["WARD_NO"] != null ? dataNodes["WARD_NO"].InnerText.Trim() : string.Empty,
                            RegistrationDate = dataNodes["REG_DATE"] != null ? dataNodes["REG_DATE"].InnerText.Trim() : string.Empty,
                            SignedBy = dataNodes["SIGNED_BY"] != null ? dataNodes["SIGNED_BY"].InnerText.Trim() : string.Empty,
                            SignedDate = dataNodes["SIGNED_DATE"] != null ? dataNodes["SIGNED_DATE"].InnerText.Trim() : string.Empty
                        };
                    }
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GHMCorrectionAndChidlNameInclusionDataByAckNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetGHMCCorrectionBirthDeathTransactionNo(object reqData)
        {
            GHMCCorrectionBirthDeathTransactionNo request = null;

            try
            {
                request = JsonConvert.DeserializeObject<GHMCCorrectionBirthDeathTransactionNo>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetGHMCCorrectionBirthDeathTransactionNo(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service),
                    request.ServiceType, request.AcknowledgementNo, request.ApplicationNo, request.RegistrationNumber, request.Gender,
                    request.ChangedGender, request.EventName, request.ChangedEventName, request.FatherName, request.ChangedFatherName, request.MotherName, request.ChangedMotherName,
                    request.DateOfEvent, request.ChangedDateOfEvent, request.PlaceOfEvent, request.ChangedPlaceOfEvent, request.Circle, request.Changedcircle, request.WardNo,
                    request.Locality, request.ResidenceAddress, request.ChangedResidenceAddress, request.ReasonForDeath, request.ChangedReasonForDeath, request.PermanentAddress,
                    request.Remarks, request.ApplicantName, request.Relation, request.RationCardNo, request.AadhaarCardNo, request.ApplicantAddress, request.PinCode,
                    request.PhoneNo, request.PostalDoorNo, request.PostalLocality, request.PostalDistrict, request.PostalMandalId, request.PostalVillageId, request.PostalPinCode,
                    request.PostalMobileNo, request.PostalEmailId, request.DeliveryType, request.NumberOfCopies, request.Purpose, request.Charge.ServiceCharge, request.Charge.PostalCharge,
                    request.Charge.UserCharge, request.Charge.TotalAmount, request.CreatedBy, request.PhysicalDocument, request.Certificate, request.ParentsDeclaration, request.NotaryAffidavit,
                    request.AvailableDocuments, request.HospitalLetter, request.MedicoLegalCase);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetGHMCBirthDeathTransactionNo => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode != null && respNode.InnerText.Trim() == "100")
                    return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText };

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetGHMCBirthDeathTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ BIRTH/DEATH CORRECTION GHMC SERVICES ]

        #region [ CHILD NAME INCLUSION GHMC SERVICE ]

        public MSResponse GetGHMCChildNameInclusionTransactionNo(object reqData)
        {
            GHMCCNInclusionTransactionNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<GHMCCNInclusionTransactionNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetGHMCChilNameInclusionTransactionNo(CacheConfig.UserName, CacheConfig.Password, request.MobileNo, CacheConfig.GetServiceId(request.Service),
                    request.ServiceType, request.ApplicationNo, request.AcknowledgementNo, request.RegistrationNumber, request.ActualchildName, request.ChangedchildName,
                    request.FatherName, request.MotherName, request.Gender, request.DateOfBirth, request.Circle, request.WardNo, request.Locality, request.PlaceOfEvent,
                    request.ApplicantAddress, request.PermanentAddress, request.Remarks, request.InformantName, request.InformantRelation,
                    request.RationCardNo, request.AadhaarNo, request.InformantAddress, request.InformantPinCode, request.InformantPhoneNo, request.DeliveryType, request.PostalDoorNo,
                    request.PostalLocality, request.PostalDistrict, request.PostalMandalId, request.PostalVillageId, request.PostalPinCode, request.PostalMobileNo,
                    request.PostalEmailId, request.NumberOfCopies, request.Purpose, request.Charge.ServiceCharge, request.Charge.PostalCharge, request.Charge.UserCharge,
                    request.Charge.TotalAmount, request.DocApplicationform, request.DocAffidavit);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetGHMCChildNameInclusionTransactionNo => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode != null && respNode.InnerText.Trim() == "100")
                    return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText };

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetGHMCChildNameInclusionTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetCertificatePDF(object reqData)
        {
            GHMCBDCertificatePDFReq request = null;
            try
            {
                request = JsonConvert.DeserializeObject<GHMCBDCertificatePDFReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetGHMCBirthDeathCertificatePDF(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service), request.ServiceType, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetGHMCBDCertificatePDF => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                if (root.SelectSingleNode("Table1/PDF") != null)
                {
                    return new CertificatePDFResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        PDFDocument = root.SelectSingleNode("Table1/PDF").InnerText.Trim()
                    };
                }

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetGHMCBDCertificatePDF: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ CHILD NAME INCLUSION GHMC SERVICE ]

        #region [ NON AVAILABILITY GHMC SERVICES ]

        public MSResponse GetGHMCNABDTransactionNo(object reqData)
        {
            GetGHMCNABDTransactionNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<GetGHMCNABDTransactionNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetGHMCNonAailabilityBirthDeathTransactionNo(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service),
                    request.ServiceType, request.ApplicationNo, request.AadhaarNo, request.ApplicantName, request.ApplicantFatherName, request.Age,
                    request.Address, request.PhoneNo, request.EventRelation, request.RationCardNumber, request.EmailID, request.EventName, request.DateofEvent, request.PlaceofEvent,
                    request.DeathCause, request.ResidenceAddress, request.EventGender, request.MotherName, request.FatherName, request.Circle, request.Ward, request.DeliveryType,
                    request.PostalDoorNo, request.PostalDistrict, request.PostalMandal, request.PostalVillage, request.PostalPincode, request.NumberOfCopies, request.Purpose,
                    request.Charge.ServiceCharge, request.Charge.PostalCharge, request.Charge.UserCharge, request.Charge.TotalAmount, request.CreatedBy, request.PhysicalDocument,
                    request.RationOtherResidenceProof, request.SchoolBonafied, request.SSCMark, request.Deathnotarized, request.Otherevidence, request.Affidavit, request.OtherDoc, request.Medicallegal);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetGHMCNABDTransactionNo => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode != null && respNode.InnerText.Trim() == "100")
                    return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText };

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetGHMCNABDTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetGHMCNABDCertificatePDF(object reqData)
        {
            GHMCBDCertificatePDFReq request = null;
            try
            {
                request = JsonConvert.DeserializeObject<GHMCBDCertificatePDFReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetGHMCNonAailabilityBirthDeathCertificatePDF(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service), request.ServiceType, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetGHMCNABDCertificatePDF => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                if (clientResp.Contains("<Table"))
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(clientResp);
                    var root = xmlDoc.DocumentElement;

                    //if (root.SelectSingleNode("Table1/PDF") != null)
                    //{
                    //    return new CertificatePDFResp
                    //    {
                    //        ResCode = "000",
                    //        ResDesc = "Success",
                    //        PDFDocument = root.SelectSingleNode("Table1/PDF").InnerText.Trim()
                    //    };
                    //}

                    var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                    if (errorCodeNode != null)
                    {
                        var errorCode = errorCodeNode.InnerText;
                        var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                        return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                    }
                }

                if (!string.IsNullOrEmpty(clientResp))
                {
                    return new CertificatePDFResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        PDFDocument = clientResp
                    };
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetGHMCNABDCertificatePDF: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ NON AVAILABILITY GHMC SERVICES ]

        #region [ MUTATION PATTAR SERVICES ]

        public MSResponse PopulateMutationType(object reqData)
        {
            try
            {
                var clientResp = mesevaClient.PopulateMutationType(CacheConfig.UserName, CacheConfig.Password);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateMutationType => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new MutationTypeResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.MutationTypes.Add(new Detail
                        {
                            Code = node["Mutation_Type"] != null ? node["Mutation_Type"].InnerText.Trim() : string.Empty,
                            Description = node["Mutation_Desc"] != null ? node["Mutation_Desc"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateMutationType: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateDivision(object reqData)
        {
            DivisionReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<DivisionReq>(reqData.ToString());
            }
            catch { }

            try
            {
                var clientResp = mesevaClient.PopulateDivision(CacheConfig.UserName, CacheConfig.Password, request.DistrictId, request.MandalId);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateDivision => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new DivisionResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.Divisions.Add(new Detail
                        {
                            Code = node["Division_ID"] != null ? node["Division_ID"].InnerText.Trim() : string.Empty,
                            Description = node["Division_name"] != null ? node["Division_name"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateDivision: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateMutationCaste(object reqData)
        {
            try
            {
                var clientResp = mesevaClient.PopulateMutationCaste(CacheConfig.UserName, CacheConfig.Password);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateMutationCaste => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new MutationCasteResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.MutationCastes.Add(new Detail
                        {
                            Code = node["Code"] != null ? node["Code"].InnerText.Trim() : string.Empty,
                            Description = node["CASTE_CAT"] != null ? node["CASTE_CAT"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateMutationCaste: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetMutationAppTransactionNo(object reqData)
        {
            MutationAppTransactionNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<MutationAppTransactionNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                if ("y,t".IndexOf(request.AddressFlag.ToLower()) >= 0)
                {
                    request.PostalDoorNo = request.PermanentDoorNo;
                    request.PostalLocality = request.PermanentLocality;
                    request.PostalDistrict = request.PermanentDistrict;
                    request.PostalMandal = request.PermanentMandal;
                    request.PostalVillage = request.PermanentVillage;
                    request.PostalPinCode = request.PermanentPinCode;
                }

                var clientResp = mesevaClient.GetMutationApplicationTransactionNo(CacheConfig.UserName, CacheConfig.Password, request.MobileNo, CacheConfig.GetServiceId(request.Service),
                    request.AddressFlag, request.ApplicationNo, request.AadhaarNo, request.BuyerName, request.FatherOrHusbandName,
                    request.Gender, request.DateOfBirth, request.BuyerKathaNo, request.PermanentState, request.PermanentDoorNo, request.PermanentLocality,
                    request.PermanentDistrict, request.PermanentMandal, request.PermanentVillage, request.PermanentPinCode, request.PostalState, request.PostalDoorNo,
                    request.PostalLocality, request.PostalDistrict, request.PostalMandal, request.PostalVillage, request.PostalPinCode, request.MobileNo, request.Phone,
                    request.EmailId, request.Remarks, request.RationCardNo, request.DeliveryType, request.docdistrict, request.docmandal, request.docvillage, request.Division,
                    request.Caste, request.MutationType, request.GridMutationDetails, request.UserCharge, request.ServiceCharge, request.PostalCharge, request.TotalAmount,
                    request.DocApplicationform, request.DocRegisteredDocumentCopies, request.DocOldPattadarPassbook, request.DocTaxReceipts, request.DocPassportSizephoto, request.DocSignature);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetMutationAppTransactionNo => Response: {0}", clientResp));
                if (clientResp == null)
                    new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode == null || respNode.InnerText.Trim().Trim() != "100")
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText.Trim() };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetMutationAppTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ MUTATION PATTAR SERVICES ]

        #region [ COMMUNITY DOB SERVICES ]

        public MSResponse PopulateCNDOBServiceType(object reqData)
        {
            try
            {
                var clientResp = mesevaClient.PopulateCommunityDOBServiceTypeList(CacheConfig.UserName, CacheConfig.Password);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateCNDOBServiceType => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new CNDOBServiceTypeResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.CNDOBServiceTypes.Add(new Detail
                        {
                            Code = node["Service_Type_Id"] != null ? node["Service_Type_Id"].InnerText.Trim() : string.Empty,
                            Description = node["service_desc"] != null ? node["service_desc"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateCNDOBServiceType: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateCNCastes(object reqData)
        {
            CNCasteReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<CNCasteReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                string clientResp = string.Empty;
                if (request.Whom == "1")
                    clientResp = mesevaClient.PopulateCommunityOftheApplicantCaste(CacheConfig.UserName, CacheConfig.Password, request.ServiceType, request.DistrictId);
                else if (request.Whom == "2")
                    clientResp = mesevaClient.PopulateCommunityOftheFatherCaste(CacheConfig.UserName, CacheConfig.Password, request.ServiceType, request.DistrictId);
                else if (request.Whom == "3")
                    clientResp = mesevaClient.PopulateCommunityOftheMotherCaste(CacheConfig.UserName, CacheConfig.Password, request.DistrictId);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateCNCastes => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new CNCasteResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.CNCastes.Add(new Detail
                        {
                            Code = node["Caste_SLNo"] != null ? node["Caste_SLNo"].InnerText.Trim() : string.Empty,
                            Description = node["Caste_Name"] != null ? node["Caste_Name"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateCNCastes: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateCNSubtribe(object reqData)
        {
            CNSubtribeReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<CNSubtribeReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                string clientResp = string.Empty;
                if (request.Whom == "1")
                    clientResp = mesevaClient.PopulateSubtribeApplicant(CacheConfig.UserName, CacheConfig.Password, request.DistrictId, request.Caste, request.ServiceType);
                else if (request.Whom == "2")
                    clientResp = mesevaClient.PopulateSubtribeFather(CacheConfig.UserName, CacheConfig.Password, request.DistrictId, request.Caste, request.ServiceType);
                else if (request.Whom == "3")
                    clientResp = mesevaClient.PopulateSubtribeMother(CacheConfig.UserName, CacheConfig.Password, request.DistrictId, request.Caste);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateCNSubtribe => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectSingleNode("Table1");
                if (dataNodes != null)
                {
                    return new CNSubtribeResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        CasteCode = dataNodes["SlCaste"] != null ? dataNodes["SlCaste"].InnerText.Trim() : string.Empty,
                        CasteName = dataNodes["Caste_Name"] != null ? dataNodes["Caste_Name"].InnerText.Trim() : string.Empty,
                        SubCaste = dataNodes["subcaste"] != null ? dataNodes["subcaste"].InnerText.Trim() : string.Empty
                    };
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateCNSubtribe: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetServiceChargesOnServiceType(object reqData)
        {
            GHMCServiceChargeReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<GHMCServiceChargeReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetServiceChargesBasedOnServiceTpye(CacheConfig.GetServiceId(request.Service), request.ServiceType, request.DeliveryType, CacheConfig.UserName, CacheConfig.Password);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetServiceChargesOnServiceType => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNode = root.SelectSingleNode("dtcharges");
                if (dataNode != null)
                {
                    return new ServiceChargeResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        ServiceId = dataNode["service_id"] != null ? dataNode["service_id"].InnerText.Trim() : string.Empty,
                        ServiceAmount = dataNode["ServiceAmount"] != null ? dataNode["ServiceAmount"].InnerText.Trim() : string.Empty,
                        DeliveryType = dataNode["delivery_type"] != null ? dataNode["delivery_type"].InnerText.Trim() : string.Empty,
                        DeliveryCharges = dataNode["Delivery_Charges"] != null ? dataNode["Delivery_Charges"].InnerText.Trim() : string.Empty,
                        ChallanAmount = dataNode["challan_amount"] != null ? dataNode["challan_amount"].InnerText.Trim() : string.Empty,
                        SLA = dataNode["SLA"] != null ? dataNode["SLA"].InnerText.Trim() : string.Empty,
                        UserCharges = dataNode["User_Charges"] != null ? dataNode["User_Charges"].InnerText.Trim() : string.Empty
                    };
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetServiceChargesOnServiceType: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetCNDOBTransactionNo(object reqData)
        {
            CNDOBTransactionNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<CNDOBTransactionNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetCommunityDateofBirthTransactionNo(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service), request.MobileNo,
                    request.ServiceType, request.ApplicationNo, request.AadhaarNo, request.ApplicantName, request.Gender, request.FatherOrHusbandName,
                    request.MotherName, request.DateofBirth, request.Age, request.PresentDoorNo, request.PresentLocality, request.PresentDistrict, request.PresentMandal,
                    request.PresentVillage, request.PresentPinCode, request.PermanentDoorNo, request.PermanentLocality, request.PermanentDistrict, request.PermanentMandal,
                    request.PermanentVillage, request.PermanentPinCode, request.ResidenceDoorNo, request.ResidencetLocality, request.ResidenceDistrict, request.ResidenceMandal,
                    request.ResidenceVillage, request.ResidencetPinCode, request.POBDoorNo, request.POBLocality, request.POBState, request.POBDistrict, request.POBMandal,
                    request.POBVillage, request.POBPinCode, request.CommunityCertificatePastYesNo, request.ApplicantCommunity, request.SubtribeOrSubgroupofApplicant, request.FatherCommunity,
                    request.SubtribeFather, request.MotherCommunity, request.SubtribeMother, request.ApplicantReligion, request.FatherReligion, request.MotherReligion,
                    request.NaturalBornOrAdoptedBaby, request.HouseholdSurveyNo, request.MobileNo, request.EmailId, request.DeliveryType, request.ServiceCharge, request.PostalCharge,
                    request.UserCharge, request.TotalAmount, request.DocApplicationform, request.DocIDProof, request.DocDOBCert, request.DocSSCMarksMemo, request.DocImmovableProperties, request.DocStudyCert);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetCNDOBTransactionNo => Response: {0}", clientResp));
                if (clientResp == null)
                    new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode == null || respNode.InnerText.Trim().Trim() != "100")
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText.Trim() };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetCNDOBTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse GetCNDOBCertificatePDF(object reqData)
        {
            ApplicationNoBasedReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<ApplicationNoBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetIncomeCertificatePDF(CacheConfig.UserName, CacheConfig.Password, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetCNDOBCertificatePDF => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                if (root.SelectSingleNode("Table/IS_PDF") != null && root.SelectSingleNode("Table/IS_PDF").InnerText.Trim().Trim() == "Y" && root.SelectSingleNode("Table2/PDF") != null)
                {
                    return new CertificatePDFResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        PDFDocument = root.SelectSingleNode("Table2/PDF").InnerText.Trim()
                    };
                }

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetCNDOBCertificatePDF: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ COMMUNITY DOB SERVICES ]

        #region [ BIRTH/DEATH CDMA SERVICES ]

        public MSResponse CDMARUIDDetails(object reqData)
        {
            CDMARUIDDetailReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<CDMARUIDDetailReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.PopulateRUIDDetails(CacheConfig.UserName, CacheConfig.Password, request.LocationType, request.DistrictId);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("CDMARUIDDetails => Response: {0}", clientResp));
                if (clientResp == null)
                    new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("dtRUIDList");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new CDMARUIDDetailResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode item in dataNodes)
                    {
                        response.RUIDDetail.Add(new Detail
                        {
                            Code = item["OFFICEID"] != null ? item["OFFICEID"].InnerText.Trim() : string.Empty,
                            Description = item["OFFICE_TITTLE"] != null ? item["OFFICE_TITTLE"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => CDMARUIDDetails: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse CDMABirthDeathDetail(object reqData)
        {
            CDMABirthDeathDetailReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<CDMABirthDeathDetailReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                string clientResp = string.Empty;
                if (request.ServiceType == "01")
                    clientResp = mesevaClient.PopulateCDMABirthDeathDetails(request.ApplicationNo, CacheConfig.UserName, CacheConfig.Password, request.ServiceType, request.RegUnitId, request.RegYear, request.RegNo, request.TestId);
                else if (request.ServiceType == "02")
                    clientResp = mesevaClient.PopulateCDMADeathDetails(CacheConfig.UserName, CacheConfig.Password, request.ServiceType, request.RegUnitId, request.RegYear, request.RegNo, request.TestId);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("CDMABirthDeathDetail => Response: {0}", clientResp));
                if (clientResp == null)
                    new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectSingleNode("dtDetails");
                if (dataNodes != null)
                {
                    if (request.ServiceType == "01")
                    {
                        return new CDMABirthDetailResp
                        {
                            ResCode = "000",
                            ResDesc = "Success",
                            BirthDetail = new CDMABirthDetail
                            {
                                AddressAtBirth1 = dataNodes["ADDRESS_ATBIRTH1"] != null ? dataNodes["ADDRESS_ATBIRTH1"].InnerText.Trim() : string.Empty,
                                AddressAtBirth2 = dataNodes["ADDRESS_ATBIRTH2"] != null ? dataNodes["ADDRESS_ATBIRTH2"].InnerText.Trim() : string.Empty,
                                AddressAtBirth3 = dataNodes["ADDRESS_ATBIRTH3"] != null ? dataNodes["ADDRESS_ATBIRTH3"].InnerText.Trim() : string.Empty,
                                AddressPrem1 = dataNodes["ADDRESS_PERM1"] != null ? dataNodes["ADDRESS_PERM1"].InnerText.Trim() : string.Empty,
                                AddressPrem2 = dataNodes["ADDRESS_PERM2"] != null ? dataNodes["ADDRESS_PERM2"].InnerText.Trim() : string.Empty,
                                AddressPrem3 = dataNodes["ADDRESS_PERM3"] != null ? dataNodes["ADDRESS_PERM3"].InnerText.Trim() : string.Empty,
                                SyNo = dataNodes["SyNo"] != null ? dataNodes["SyNo"].InnerText.Trim() : string.Empty,
                                DateOfBirth = dataNodes["CDOB"] != null ? dataNodes["CDOB"].InnerText.Trim() : string.Empty,
                                CertTitle = dataNodes["CERTTITLE"] != null ? dataNodes["CERTTITLE"].InnerText.Trim() : string.Empty,
                                ChildName = dataNodes["CHILDNAME"] != null ? dataNodes["CHILDNAME"].InnerText.Trim() : string.Empty,
                                ChildSURName = dataNodes["CHILDSURNAME"] != null ? dataNodes["CHILDSURNAME"].InnerText.Trim() : string.Empty,
                                DigitalSIG = dataNodes["DIGITAL_SIG"] != null ? dataNodes["DIGITAL_SIG"].InnerText.Trim() : string.Empty,
                                DistrictName = dataNodes["DISTNAME"] != null ? dataNodes["DISTNAME"].InnerText.Trim() : string.Empty,
                                DTIssue = dataNodes["DTISSUE"] != null ? dataNodes["DTISSUE"].InnerText.Trim() : string.Empty,
                                FatherName = dataNodes["FATHERNAME"] != null ? dataNodes["FATHERNAME"].InnerText.Trim() : string.Empty,
                                MotherName = dataNodes["MOTHERNAME"] != null ? dataNodes["MOTHERNAME"].InnerText.Trim() : string.Empty,
                                MotherSURName = dataNodes["MOTHERSURNAME"] != null ? dataNodes["MOTHERSURNAME"].InnerText.Trim() : string.Empty,
                                OfficeTitle = dataNodes["OFFICE_TITLE"] != null ? dataNodes["OFFICE_TITLE"].InnerText.Trim() : string.Empty,
                                OfficeId = dataNodes["OFFICEID"] != null ? dataNodes["OFFICEID"].InnerText.Trim() : string.Empty,
                                Pin = dataNodes["PIN"] != null ? dataNodes["PIN"].InnerText.Trim() : string.Empty,
                                RHAddress1 = dataNodes["r_H_ADDRESS1"] != null ? dataNodes["r_H_ADDRESS1"].InnerText.Trim() : string.Empty,
                                RHAddress2 = dataNodes["r_H_ADDRESS2"] != null ? dataNodes["r_H_ADDRESS2"].InnerText.Trim() : string.Empty,
                                RHAddress3 = dataNodes["r_H_ADDRESS3"] != null ? dataNodes["r_H_ADDRESS3"].InnerText.Trim() : string.Empty,
                                RegDate = dataNodes["REGDATE"] != null ? dataNodes["REGDATE"].InnerText.Trim() : string.Empty,
                                RegistrarOffice = dataNodes["REGISTRAROFFICE"] != null ? dataNodes["REGISTRAROFFICE"].InnerText.Trim() : string.Empty,
                                RegLocation = dataNodes["REGLOCATION"] != null ? dataNodes["REGLOCATION"].InnerText.Trim() : string.Empty,
                                RegNo = dataNodes["REGNO"] != null ? dataNodes["REGNO"].InnerText.Trim() : string.Empty,
                                RegYear = dataNodes["REGYEAR"] != null ? dataNodes["REGYEAR"].InnerText.Trim() : string.Empty,
                                Res_Code = dataNodes["RESCODE"] != null ? dataNodes["RESCODE"].InnerText.Trim() : string.Empty,
                                SealText1 = dataNodes["SEALTEXT1"] != null ? dataNodes["SEALTEXT1"].InnerText.Trim() : string.Empty,
                                SealText2 = dataNodes["SEALTEXT2"] != null ? dataNodes["SEALTEXT2"].InnerText.Trim() : string.Empty,
                                Sex = dataNodes["SEX"] != null ? dataNodes["SEX"].InnerText.Trim() : string.Empty,
                                StateName = dataNodes["STATENAME"] != null ? dataNodes["STATENAME"].InnerText.Trim() : string.Empty,
                                CertificateId = dataNodes["CertificateID"] != null ? dataNodes["CertificateID"].InnerText.Trim() : string.Empty,
                                DistrictLocal = dataNodes["DistrictLocal"] != null ? dataNodes["DistrictLocal"].InnerText.Trim() : string.Empty,
                                MandalLocal = dataNodes["MandalLocal"] != null ? dataNodes["MandalLocal"].InnerText.Trim() : string.Empty,
                                OfficeTitleLocal = dataNodes["OfficeTitleLocal"] != null ? dataNodes["OfficeTitleLocal"].InnerText.Trim() : string.Empty,
                                Hash = dataNodes["Hash"] != null ? dataNodes["Hash"].InnerText.Trim() : string.Empty,
                                MeesevaAppliNo = dataNodes["MeesevaAppliNo"] != null ? dataNodes["MeesevaAppliNo"].InnerText.Trim() : string.Empty,
                                RLBTypeId = dataNodes["RLBTYPEID"] != null ? dataNodes["RLBTYPEID"].InnerText.Trim() : string.Empty
                            }
                        };
                    }
                    else if (request.ServiceType == "02")
                    {
                        return new CDMADeathDetailResp
                        {
                            ResCode = "000",
                            ResDesc = "Success",
                            DeathDetail = new CDMADeathDetail
                            {
                                AddressAtDeath1 = dataNodes["ADDRESS_ATDEATH1"] != null ? dataNodes["ADDRESS_ATDEATH1"].InnerText.Trim() : string.Empty,
                                AddressAtDeath2 = dataNodes["ADDRESS_ATDEATH2"] != null ? dataNodes["ADDRESS_ATDEATH2"].InnerText.Trim() : string.Empty,
                                AddressAtDeath3 = dataNodes["ADDRESS_ATDEATH3"] != null ? dataNodes["ADDRESS_ATDEATH3"].InnerText.Trim() : string.Empty,
                                AddressPrem1 = dataNodes["ADDRESS_PERM1"] != null ? dataNodes["ADDRESS_PERM1"].InnerText.Trim() : string.Empty,
                                AddressPrem2 = dataNodes["ADDRESS_PERM2"] != null ? dataNodes["ADDRESS_PERM2"].InnerText.Trim() : string.Empty,
                                AddressPrem3 = dataNodes["ADDRESS_PERM3"] != null ? dataNodes["ADDRESS_PERM3"].InnerText.Trim() : string.Empty,
                                Age = dataNodes["AGE"] != null ? dataNodes["AGE"].InnerText.Trim() : string.Empty,
                                DateOfDeath = dataNodes["DTOFDEATH"] != null ? dataNodes["DTOFDEATH"].InnerText.Trim() : string.Empty,
                                AgeIn = dataNodes["AGE_IN"] != null ? dataNodes["AGE_IN"].InnerText.Trim() : string.Empty,
                                DeathPlace = dataNodes["DEATH_PLACE"] != null ? dataNodes["DEATH_PLACE"].InnerText.Trim() : string.Empty,
                                FMH = dataNodes["f_m_h"] != null ? dataNodes["f_m_h"].InnerText.Trim() : string.Empty,
                                DigitalSIG = dataNodes["DIGITAL_SIG"] != null ? dataNodes["DIGITAL_SIG"].InnerText.Trim() : string.Empty,
                                DistrictName = dataNodes["DISTNAME"] != null ? dataNodes["DISTNAME"].InnerText.Trim() : string.Empty,
                                DTIssue = dataNodes["DTISSUE"] != null ? dataNodes["DTISSUE"].InnerText.Trim() : string.Empty,
                                FatherName = dataNodes["FATHERNAME"] != null ? dataNodes["FATHERNAME"].InnerText.Trim() : string.Empty,
                                MotherName = dataNodes["MOTHERNAME"] != null ? dataNodes["MOTHERNAME"].InnerText.Trim() : string.Empty,
                                OfficeTitle = dataNodes["OFFICE_TITLE"] != null ? dataNodes["OFFICE_TITLE"].InnerText.Trim() : string.Empty,
                                OfficeId = dataNodes["OFFICEID"] != null ? dataNodes["OFFICEID"].InnerText.Trim() : string.Empty,
                                Pin = dataNodes["PIN"] != null ? dataNodes["PIN"].InnerText.Trim() : string.Empty,
                                RHAddress1 = dataNodes["r_H_ADDRESS1"] != null ? dataNodes["r_H_ADDRESS1"].InnerText.Trim() : string.Empty,
                                RHAddress2 = dataNodes["r_H_ADDRESS2"] != null ? dataNodes["r_H_ADDRESS2"].InnerText.Trim() : string.Empty,
                                RHAddress3 = dataNodes["r_H_ADDRESS3"] != null ? dataNodes["r_H_ADDRESS3"].InnerText.Trim() : string.Empty,
                                RegDate = dataNodes["REGDATE"] != null ? dataNodes["REGDATE"].InnerText.Trim() : string.Empty,
                                RegistrarOffice = dataNodes["REGISTRAROFFICE"] != null ? dataNodes["REGISTRAROFFICE"].InnerText.Trim() : string.Empty,
                                RegLocation = dataNodes["REGLOCATION"] != null ? dataNodes["REGLOCATION"].InnerText.Trim() : string.Empty,
                                RegNo = dataNodes["REGNO"] != null ? dataNodes["REGNO"].InnerText.Trim() : string.Empty,
                                RegYear = dataNodes["REGYEAR"] != null ? dataNodes["REGYEAR"].InnerText.Trim() : string.Empty,
                                LogId = dataNodes["LOGID"] != null ? dataNodes["LOGID"].InnerText.Trim() : string.Empty,
                                MandName = dataNodes["MANDNAME"] != null ? dataNodes["MANDNAME"].InnerText.Trim() : string.Empty,
                                Name = dataNodes["NAME"] != null ? dataNodes["NAME"].InnerText.Trim() : string.Empty,
                                SealText1 = dataNodes["SEALTEXT1"] != null ? dataNodes["SEALTEXT1"].InnerText.Trim() : string.Empty,
                                SealText2 = dataNodes["SEALTEXT2"] != null ? dataNodes["SEALTEXT2"].InnerText.Trim() : string.Empty,
                                Sex = dataNodes["SEX"] != null ? dataNodes["SEX"].InnerText.Trim() : string.Empty,
                                StateName = dataNodes["STATENAME"] != null ? dataNodes["STATENAME"].InnerText.Trim() : string.Empty,
                                CertificateId = dataNodes["CertificateID"] != null ? dataNodes["CertificateID"].InnerText.Trim() : string.Empty,
                                DistrictLocal = dataNodes["DistrictLocal"] != null ? dataNodes["DistrictLocal"].InnerText.Trim() : string.Empty,
                                MandalLocal = dataNodes["MandalLocal"] != null ? dataNodes["MandalLocal"].InnerText.Trim() : string.Empty,
                                OfficeTitleLocal = dataNodes["OfficeTitleLocal"] != null ? dataNodes["OfficeTitleLocal"].InnerText.Trim() : string.Empty,
                                Hash = dataNodes["Hash"] != null ? dataNodes["Hash"].InnerText.Trim() : string.Empty,
                                MeesevaAppliNo = dataNodes["MeesevaAppliNo"] != null ? dataNodes["MeesevaAppliNo"].InnerText.Trim() : string.Empty,
                                RLBTypeId = dataNodes["RLBTYPEID"] != null ? dataNodes["RLBTYPEID"].InnerText.Trim() : string.Empty
                            }
                        };
                    }
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => CDMABirthDeathDetail: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse CDMABirthDeathDetailSearch(object reqData)
        {
            CDMABirthDeathSearchReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<CDMABirthDeathSearchReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };
                string clientResp = string.Empty;
                if (request.ServiceType == "01")
                    clientResp = mesevaClient.SearchCDMABirthDetails(CacheConfig.UserName, CacheConfig.Password, request.ServiceType, request.RegUnitId, request.RegYear, request.RegNo, request.Gender);
                else if (request.ServiceType == "02")
                    clientResp = mesevaClient.SearchCDMADeathDetails(CacheConfig.UserName, CacheConfig.Password, request.ServiceType, request.RegUnitId, request.RegYear, request.RegNo, request.Gender);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("CDMABirthDeathDetailSearch => Response: {0}", clientResp));
                if (clientResp == null)
                    new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("dtDetails");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    if (request.ServiceType == "01")
                    {
                        var response = new CDMABirthSearchResp { ResCode = "000", ResDesc = "Success" };

                        foreach (XmlNode item in dataNodes)
                        {
                            if (item["RESCODE"] != null && item["RESCODE"].InnerText.Trim() == "1")
                            {
                                response.BirthSearch.Add(new CDMABirthSearch
                                {
                                    AddressAtBirth1 = item["ADDRESS_ATBIRTH1"] != null ? item["ADDRESS_ATBIRTH1"].InnerText.Trim() : string.Empty,
                                    AddressAtBirth2 = item["ADDRESS_ATBIRTH2"] != null ? item["ADDRESS_ATBIRTH2"].InnerText.Trim() : string.Empty,
                                    AddressAtBirth3 = item["ADDRESS_ATBIRTH3"] != null ? item["ADDRESS_ATBIRTH3"].InnerText.Trim() : string.Empty,
                                    AddressPrem1 = item["ADDRESS_PERM1"] != null ? item["ADDRESS_PERM1"].InnerText.Trim() : string.Empty,
                                    AddressPrem2 = item["ADDRESS_PERM2"] != null ? item["ADDRESS_PERM2"].InnerText.Trim() : string.Empty,
                                    AddressPrem3 = item["ADDRESS_PERM3"] != null ? item["ADDRESS_PERM3"].InnerText.Trim() : string.Empty,
                                    DateOfBirth = item["CDOB"] != null ? item["CDOB"].InnerText.Trim() : string.Empty,
                                    FatherName = item["FATHERNAME"] != null ? item["FATHERNAME"].InnerText.Trim() : string.Empty,
                                    MotherName = item["MOTHERNAME"] != null ? item["MOTHERNAME"].InnerText.Trim() : string.Empty,
                                    OfficeId = item["OFFICEID"] != null ? item["OFFICEID"].InnerText.Trim() : string.Empty,
                                    RHAddress1 = item["r_H_ADDRESS1"] != null ? item["r_H_ADDRESS1"].InnerText.Trim() : string.Empty,
                                    RHAddress2 = item["r_H_ADDRESS2"] != null ? item["r_H_ADDRESS2"].InnerText.Trim() : string.Empty,
                                    RHAddress3 = item["r_H_ADDRESS3"] != null ? item["r_H_ADDRESS3"].InnerText.Trim() : string.Empty,
                                    BirthPlace = item["BIRTH_PLACE"] != null ? item["BIRTH_PLACE"].InnerText.Trim() : string.Empty,
                                    HospitalName = item["HOSNAME"] != null ? item["HOSNAME"].InnerText.Trim() : string.Empty,
                                    LocationName = item["LOCNAME"] != null ? item["LOCNAME"].InnerText.Trim() : string.Empty,
                                    RegDate = item["REGDATE"] != null ? item["REGDATE"].InnerText.Trim() : string.Empty,
                                    RegNo = item["REGNO"] != null ? item["REGNO"].InnerText.Trim() : string.Empty,
                                    RegYear = item["REGYEAR"] != null ? item["REGYEAR"].InnerText.Trim() : string.Empty,
                                    Res_Code = item["RESCODE"] != null ? item["RESCODE"].InnerText.Trim() : string.Empty,
                                    Sex = item["SEX"] != null ? item["SEX"].InnerText.Trim() : string.Empty
                                });
                            }
                        }

                        if (response.BirthSearch.Count > 0)
                            return response;
                    }
                    else if (request.ServiceType == "02")
                    {
                        var response = new CDMADeathSearchResp { ResCode = "000", ResDesc = "Success" };

                        foreach (XmlNode item in dataNodes)
                        {
                            if (item["RESCODE"] != null && item["RESCODE"].InnerText.Trim() == "1")
                            {
                                response.DeathSearch.Add(new CDMADeathSearch
                                {
                                    AddressAtBirth1 = item["ADDRESS_ATBIRTH1"] != null ? item["ADDRESS_ATBIRTH1"].InnerText.Trim() : string.Empty,
                                    AddressAtBirth2 = item["ADDRESS_ATBIRTH2"] != null ? item["ADDRESS_ATBIRTH2"].InnerText.Trim() : string.Empty,
                                    AddressAtBirth3 = item["ADDRESS_ATBIRTH3"] != null ? item["ADDRESS_ATBIRTH3"].InnerText.Trim() : string.Empty,
                                    AddressPrem1 = item["ADDRESS_PERM1"] != null ? item["ADDRESS_PERM1"].InnerText.Trim() : string.Empty,
                                    AddressPrem2 = item["ADDRESS_PERM2"] != null ? item["ADDRESS_PERM2"].InnerText.Trim() : string.Empty,
                                    AddressPrem3 = item["ADDRESS_PERM3"] != null ? item["ADDRESS_PERM3"].InnerText.Trim() : string.Empty,
                                    DateOfDeath = item["DTOFDEATH"] != null ? item["DTOFDEATH"].InnerText.Trim() : string.Empty,
                                    Name = item["NAME"] != null ? item["NAME"].InnerText.Trim() : string.Empty,
                                    FatherName = item["FATHERNAME"] != null ? item["FATHERNAME"].InnerText.Trim() : string.Empty,
                                    MotherName = item["MOTHERNAME"] != null ? item["MOTHERNAME"].InnerText.Trim() : string.Empty,
                                    OfficeId = item["OFFICEID"] != null ? item["OFFICEID"].InnerText.Trim() : string.Empty,
                                    RHAddress1 = item["r_H_ADDRESS1"] != null ? item["r_H_ADDRESS1"].InnerText.Trim() : string.Empty,
                                    RHAddress2 = item["r_H_ADDRESS2"] != null ? item["r_H_ADDRESS2"].InnerText.Trim() : string.Empty,
                                    RHAddress3 = item["r_H_ADDRESS3"] != null ? item["r_H_ADDRESS3"].InnerText.Trim() : string.Empty,
                                    BirthPlace = item["BIRTH_PLACE"] != null ? item["BIRTH_PLACE"].InnerText.Trim() : string.Empty,
                                    HospitalName = item["HOSNAME"] != null ? item["HOSNAME"].InnerText.Trim() : string.Empty,
                                    LocationName = item["LOCNAME"] != null ? item["LOCNAME"].InnerText.Trim() : string.Empty,
                                    RegDate = item["REGDATE"] != null ? item["REGDATE"].InnerText.Trim() : string.Empty,
                                    RegNo = item["REGNO"] != null ? item["REGNO"].InnerText.Trim() : string.Empty,
                                    RegYear = item["REGYEAR"] != null ? item["REGYEAR"].InnerText.Trim() : string.Empty,
                                    Res_Code = item["RESCODE"] != null ? item["RESCODE"].InnerText.Trim() : string.Empty,
                                    Sex = item["SEX"] != null ? item["SEX"].InnerText.Trim() : string.Empty
                                });
                            }
                        }

                        if (response.DeathSearch.Count > 0)
                            return response;
                    }
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => CDMABirthDeathDetailSearch: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse CDMAServiceCharges(object reqData)
        {
            CDMAServiceChargeReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<CDMAServiceChargeReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetServiceCharges_CDMA(CacheConfig.UserName, CacheConfig.Password, CacheConfig.GetServiceId(request.Service), request.ServiceType, request.DeliveryType, request.RLBType, request.NoOfCopies);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetCDMAServiceCharges => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNode = root.SelectSingleNode("dtcharges");
                if (dataNode != null)
                {
                    return new CDMAServiceChargeResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        ServiceId = dataNode["service_id"] != null ? dataNode["service_id"].InnerText.Trim() : string.Empty,
                        ServiceType = dataNode["service_type"] != null ? dataNode["service_type"].InnerText.Trim() : string.Empty,
                        ServiceAmount = dataNode["Service_Amount"] != null ? dataNode["Service_Amount"].InnerText.Trim() : string.Empty,
                        DeliveryType = dataNode["delivery_type"] != null ? dataNode["delivery_type"].InnerText.Trim() : string.Empty,
                        CourierCharge = dataNode["Courier_Charges"] != null ? dataNode["Courier_Charges"].InnerText.Trim() : string.Empty,
                        ChallanAmount = dataNode["ChallanAmount"] != null ? dataNode["ChallanAmount"].InnerText.Trim() : string.Empty,
                        SLA = dataNode["SLA"] != null ? dataNode["SLA"].InnerText.Trim() : string.Empty,
                        UserCharge = dataNode["User_Charges"] != null ? dataNode["User_Charges"].InnerText.Trim() : string.Empty,
                        StatuaryCharge = dataNode["Statuary_Charges"] != null ? dataNode["Statuary_Charges"].InnerText.Trim() : string.Empty,
                        TotalAmount = dataNode["TotalAmount"] != null ? dataNode["TotalAmount"].InnerText.Trim() : string.Empty
                    };
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetCDMAServiceCharges: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse CDMABDTransactionNo(object reqData)
        {
            CDMATransactionNo request = null;

            try
            {
                request = JsonConvert.DeserializeObject<CDMATransactionNo>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetCDMATransactionNo(CacheConfig.UserName, CacheConfig.Password, request.MobileNo, CacheConfig.GetServiceId(request.Service),
                    request.ServiceType, request.RegUnitId, request.RegNo, request.RegYear, request.ApplicationNo, request.RegDate, request.EventName, request.EventSurName,
                    request.DateofBirthOrDeath, request.Gender, request.FatherOrHusbandName, request.FatherOrHusbandSurName, request.MotherName, request.MotherSurName,
                    request.BirthorDeathPlace, request.LocationName, request.HospitalName, request.MobileNo, request.HospitalAddress1, request.HospitalAddress2, request.HospitalAddress3,
                    request.StateName, request.DistrictName, request.PinCode, request.InformantName, request.InformantRelation, request.InformantAddress1, request.InformantAddress2,
                    request.InformantAddress3, request.InformantPhoneNo, request.AadharCardNo, request.RationCardNo, request.InformantEmailId, request.InformantRemarks,
                    request.InformantPinCode, request.DeliveryType, request.NoOfCopies, request.Purpose, request.PostalDoorNo, request.PostalLocality, request.PostalState,
                    request.PostalDistrict, request.PostalMandalId, request.PostalVillageId, request.PostalPinCode, request.RLBType, request.ServiceCharge, request.PostalCharge,
                    request.UserCharge, request.StationaryCharges, request.TotalAmount, request.DocApplicationform);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("GetCDMATransactionNo => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode != null && respNode.InnerText.Trim() == "100")
                    return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText };

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => GetCDMATransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse CDMABDCertificatePDF(object reqData)
        {
            ApplicationNoBasedReq request = null;
            try
            {
                request = JsonConvert.DeserializeObject<ApplicationNoBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetCDMABirthDeathCertificatePDF(CacheConfig.UserName, CacheConfig.Password, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("CDMABDCertificatePDF => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                if (root.SelectSingleNode("Table1/PDF") != null)
                {
                    return new CertificatePDFResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        PDFDocument = root.SelectSingleNode("Table1/PDF").InnerText.Trim()
                    };
                }

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => CDMABDCertificatePDF: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ BIRTH/DEATH CDMA SERVICES ]

        #region [ ENCUMBRANCE SERVICES ]

        public MSResponse PopulateECDistrict(object reqData)
        {
            try
            {
                var clientResp = mesevaClient.PopulateECDistrict(CacheConfig.UserName, CacheConfig.Password);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateECDistrict => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new DistrictsResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes)
                    {
                        response.Districts.Add(new Detail
                        {
                            Code = node["district_id"] != null ? node["district_id"].InnerText.Trim() : string.Empty,
                            Description = node["district_description"] != null ? node["district_description"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateECDistrict: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse PopulateSRO(object reqData)
        {
            SROReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<SROReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.PopulateSRO(CacheConfig.UserName, CacheConfig.Password, request.DistrictId);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("PopulateSRO => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectNodes("Table1");
                if (dataNodes != null && dataNodes.Count > 0)
                {
                    var response = new SROResp { ResCode = "000", ResDesc = "Success" };
                    foreach (XmlNode node in dataNodes)
                    {
                        response.SROs.Add(new Detail
                        {
                            Code = node["SRO_CODE"] != null ? node["SRO_CODE"].InnerText.Trim() : string.Empty,
                            Description = node["SRO_NAME"] != null ? node["SRO_NAME"].InnerText.Trim() : string.Empty
                        });
                    }

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => PopulateSRO: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse ECDocuments(object reqData)
        {
            ECDocumentReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<ECDocumentReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetECDocuments(CacheConfig.UserName, CacheConfig.Password, request.DistrictId, request.SRO1, request.RegYear, request.DocumentNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("ECDocuments => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };



                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectSingleNode("Table1");
                if (dataNodes != null)
                {
                    var response = new ECDocumentResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        SRO = dataNodes["SRO_CODE"] != null ? dataNodes["SRO_CODE"].InnerText.Trim() : string.Empty,
                        BVillage = dataNodes["BVillage"] != null ? dataNodes["BVillage"].InnerText.Trim() : string.Empty,
                        BVillAlias = dataNodes["BVillAlias"] != null ? dataNodes["BVillAlias"].InnerText.Trim() : string.Empty,
                        Colony = dataNodes["Colony"] != null ? dataNodes["Colony"].InnerText.Trim() : string.Empty,
                        Apartment = dataNodes["Apartment"] != null ? dataNodes["Apartment"].InnerText.Trim() : string.Empty,
                        FlatNo = dataNodes["FlatNo"] != null ? dataNodes["FlatNo"].InnerText.Trim() : string.Empty,
                        HouseNo = dataNodes["HouseNo"] != null ? dataNodes["HouseNo"].InnerText.Trim() : string.Empty,
                        SyNo = dataNodes["SyNo"] != null ? dataNodes["SyNo"].InnerText.Trim() : string.Empty,
                        PlotNo = dataNodes["PlotNo"] != null ? dataNodes["PlotNo"].InnerText.Trim() : string.Empty,
                        FromDate = dataNodes["FromDate"] != null ? dataNodes["FromDate"].InnerText.Trim() : string.Empty,
                        ToDate = dataNodes["ToDate"] != null ? dataNodes["ToDate"].InnerText.Trim() : string.Empty,
                        Ward = dataNodes["Ward"] != null ? dataNodes["Ward"].InnerText.Trim() : string.Empty,
                        Block = dataNodes["Block"] != null ? dataNodes["Block"].InnerText.Trim() : string.Empty,
                        East = dataNodes["East"] != null ? dataNodes["East"].InnerText.Trim() : string.Empty,
                        West = dataNodes["West"] != null ? dataNodes["West"].InnerText.Trim() : string.Empty,
                        South = dataNodes["South"] != null ? dataNodes["South"].InnerText.Trim() : string.Empty,
                        North = dataNodes["North"] != null ? dataNodes["North"].InnerText.Trim() : string.Empty,
                        Extent = dataNodes["Extent"] != null ? dataNodes["Extent"].InnerText.Trim() : string.Empty,
                        Built = dataNodes["Built"] != null ? dataNodes["Built"].InnerText.Trim() : string.Empty,
                        SroJdn = dataNodes["SroJdn"] != null ? dataNodes["SroJdn"].InnerText.Trim() : string.Empty,
                        SLNo = dataNodes["SL_No"] != null ? dataNodes["SL_No"].InnerText.Trim() : string.Empty,
                    };

                    return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => ECDocuments: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse ECTransactionId(object reqData)
        {
            ECtransactionIdReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<ECtransactionIdReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                DateTime fromDate = DateTime.MinValue, toDate = DateTime.MinValue;
                if (!DateTime.TryParse(request.FromDate, out fromDate) || !DateTime.TryParse(request.ToDate, out toDate))
                    return new MSResponse { ResCode = "202", ResDesc = CacheConfig.GetErrorMessage("202") };

                if (fromDate.CompareTo(Convert.ToDateTime("01/01/1983")) < 0)
                    return new MSResponse { ResCode = "203", ResDesc = CacheConfig.GetErrorMessage("203") };

                if (toDate.CompareTo(fromDate) <= 0)
                    return new MSResponse { ResCode = "204", ResDesc = CacheConfig.GetErrorMessage("204") };

                if ("y,t".IndexOf(request.AddressFlag.ToLower()) >= 0)
                {
                    request.PostalDoorNo = request.PerDoorNo;
                    request.PostalDistrict = request.PerDistrict;
                    request.PostalMandal = request.PerMandal;
                    request.PostalVillage = request.PerVillage;
                    request.PostalPincode = request.PerPincode;
                }

                var clientResp = mesevaClient.GetECtransactionID(CacheConfig.UserName, CacheConfig.Password, request.MobileNo, CacheConfig.GetServiceId(request.Service),
                    request.ApplicationNo, request.DocDistrict, request.SROId, request.DocNo, request.DocYear, request.FromDate, request.ToDate, request.BLDGFlatNo,
                    request.BLDGOldHouseNo, request.BLDGAprtment, request.BLDGWard, request.BLDGBlock, request.BLDGVillageID, request.BLDGAliasVillage, request.AGRLPlotNo,
                    request.AGRLSurveyNo, request.AGRLVillageID, request.AGRLAliasVillage, request.BNDREAST, request.BNDRWEST, request.BNDRNORTH, request.BNDRSOUTH,
                    request.BNDRExtent, request.BNDRBUILTUP, request.ECSlno, request.SroJdn, request.AadharNo, request.ApplicantName, request.OwnerName, request.PerDoorNo,
                    request.PerDistrict, request.PerMandal, request.PerVillage, request.PerPincode, request.AddressFlag, request.PostalDoorNo, request.PostalLocality,
                    request.PostalState, request.PostalDistrict, request.PostalMandal, request.PostalVillage, request.PostalPincode, request.MobileNo, request.EmailId,
                    request.DeliveryType, request.ChalanAmount, request.PostalCharge, request.UserCharge, request.TotalAmount);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("ECtransactionId => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode != null && respNode.InnerText.Trim() == "100")
                    return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText };

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => ECtransactionId: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse ECCertificatePDF(object reqData)
        {
            ApplicationNoBasedReq request = null;
            try
            {
                request = JsonConvert.DeserializeObject<ApplicationNoBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetECCertificatePDF(CacheConfig.UserName, CacheConfig.Password, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("ECCertificatePDF => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                if (root.SelectSingleNode("Table1/PDF") != null)
                {
                    return new CertificatePDFResp
                    {
                        ResCode = "000",
                        ResDesc = "Success",
                        PDFDocument = root.SelectSingleNode("Table1/PDF").InnerText.Trim()
                    };
                }

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => ECCertificatePDF: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ ENCUMBRANCE SERVICES ]

        #region [ CCR SERVICES ]

        public MSResponse CCRDocDistrict(object reqData)
        {
            try
            {
                var clientResp = mesevaClient.GetCCDocDistrict(CacheConfig.UserName, CacheConfig.Password);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("CCRDocDistrict => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectSingleNode("Table1");
                if (dataNodes != null && dataNodes.ChildNodes != null && dataNodes.ChildNodes.Count > 0)
                {
                    var response = new DistrictsResp { ResCode = "000", ResDesc = "Success" };

                    foreach (XmlNode node in dataNodes.ChildNodes)
                    {
                        var districtDetail = node.InnerText.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        if (districtDetail.Length == 2)
                        {
                            response.Districts.Add(new Detail
                            {
                                Code = districtDetail[0],
                                Description = districtDetail[1]
                            });
                        }
                    }

                    if (response.Districts.Count > 0)
                        return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => CCRDocDistrict: Ex:{0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse CCRDocSRO(object reqData)
        {
            SROReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<SROReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetCCGetDocSRO(CacheConfig.UserName, CacheConfig.Password, request.DistrictId);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("CCRDocSRO => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectSingleNode("Table1");
                if (dataNodes != null && dataNodes.ChildNodes != null && dataNodes.ChildNodes.Count > 0)
                {
                    var response = new SROResp { ResCode = "000", ResDesc = "Success" };
                    foreach (XmlNode node in dataNodes.ChildNodes)
                    {
                        var sroDetail = node.InnerText.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        response.SROs.Add(new Detail
                        {
                            Code = sroDetail[0],
                            Description = sroDetail[1]
                        });
                    }

                    if (response.SROs.Count > 0)
                        return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => CCRDocSRO: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse CCRDocYear(object reqData)
        {
            CCRDocYearReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<CCRDocYearReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetCCGetDocYear(CacheConfig.UserName, CacheConfig.Password, request.SROCode);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("CCRDocYear => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectSingleNode("Table1");
                if (dataNodes != null && dataNodes.ChildNodes != null && dataNodes.ChildNodes.Count > 0)
                {
                    var response = new CCRDocYearsResp { ResCode = "000", ResDesc = "Success" };
                    foreach (XmlNode node in dataNodes.ChildNodes)
                        response.DocYears.Add(node.InnerText.Trim());

                    if (response.DocYears.Count > 0)
                        return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => CCRDocYear: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse CCRDocList(object reqData)
        {
            CCRDocListReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<CCRDocListReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetCCGetDocList(CacheConfig.UserName, CacheConfig.Password, request.SROCode, request.SROYear);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("CCRDocList => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNodes = root.SelectSingleNode("Table1");
                if (dataNodes != null && dataNodes.ChildNodes != null && dataNodes.ChildNodes.Count > 0)
                {
                    var response = new CCRDocListResp { ResCode = "000", ResDesc = "Success" };
                    foreach (XmlNode node in dataNodes.ChildNodes)
                        response.DocumentNumbers.Add(node.InnerText.Trim());

                    if (response.DocumentNumbers.Count > 0)
                        return response;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => CCRDocList: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse CCRCopyDetails(object reqData)
        {
            CCRCopyDetailReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<CCRCopyDetailReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetCertifiedCopyDetails(CacheConfig.UserName, CacheConfig.Password, request.DistrictId, request.SROCode, request.DocYear, request.DocumentNumber, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("CCRCopyDetails => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                var dataNode = root.SelectSingleNode("Table2");
                if (dataNode != null)
                {
                    return new CertificatePDFResp { ResCode = "000", ResDesc = "Success", PDFDocument = dataNode.InnerText };
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => CCRCopyDetails: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse CCRDocTransactionNo(object reqData)
        {
            CCRDocTransactionNoReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<CCRDocTransactionNoReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetCertifiedCopyOfRegistrationDocumentTransactionNo(CacheConfig.UserName, CacheConfig.Password, request.MobileNo,
                    CacheConfig.GetServiceId(request.Service), request.DocDistrict, request.SROCode, request.YearofRegistration, request.DocumentId,
                    request.ApplicationNo, request.ApplicantName, request.DoorNo, request.PermanentDistrict, request.PermanentMandal, request.PermanentVillage,
                    request.PinCode, request.MobileNo, request.EmailId, request.ChalanAmount, request.UserCharge, request.CourierCharge, request.TotalAmount);

                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("CCRDocTransactionNo => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var respNode = root.SelectSingleNode("Table1/ResponseCode");
                if (respNode != null && respNode.InnerText.Trim() == "100")
                    return new TransactionNoResp { ResCode = "000", ResDesc = "Success", TransactionNo = root.SelectSingleNode("Table1/ResponseDesc").InnerText };

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => CCRDocTransactionNo: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse CCRCertificatePDF(object reqData)
        {
            ApplicationNoBasedReq request = null;
            try
            {
                request = JsonConvert.DeserializeObject<ApplicationNoBasedReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = mesevaClient.GetECCertificatePDF(CacheConfig.UserName, CacheConfig.Password, request.ApplicationNo);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("CCRCertificatePDF => Response: {0}", clientResp));
                if (string.IsNullOrEmpty(clientResp))
                    return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };

                if (clientResp.Contains("<Table"))
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(clientResp);
                    var root = xmlDoc.DocumentElement;

                    var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                    if (errorCodeNode != null)
                    {
                        var errorCode = errorCodeNode.InnerText;
                        var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                        return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                    }
                }

                return new CertificatePDFResp
                {
                    ResCode = "000",
                    ResDesc = "Success",
                    PDFDocument = clientResp.Trim()
                };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => CCRCertificatePDF: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ CCR SERVICES ]

        #region [ REGISTRATION  SERVICES ]

        public MSResponse NewUserRegistration(object reqData)
        {
            NewUserRegistraionReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<NewUserRegistraionReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                int state = 0;
                int.TryParse(request.State, out state);
                var clientResp = mesevaClient.NewUserRegistration(CacheConfig.UserName, CacheConfig.Password, request.Firstname, request.Latsname, request.Gender,
                                        request.DOB, state, request.City, request.AddressLine1, request.AddressLine2, request.Pincode, request.Emailid,
                                        request.LoginId, request.Loginpassword, request.SystemIP, request.Adharno);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("NewUserRegistration => Response: {0}", clientResp));
                if (clientResp == "100")
                    return new MSResponse { ResCode = "000", ResDesc = "Success" };

                if (clientResp.StartsWith("ERROR"))
                {
                    var errorDetail = clientResp.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (errorDetail.Length > 0)
                    {
                        var errorCode = errorDetail[0].Replace("ERROR_", "");
                        var errorDesc = CacheConfig.GetErrorMessage(errorCode);
                        return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(errorDesc) ? errorDesc : errorDetail[1] };
                    }
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => NewUserRegistration: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        public MSResponse UpdateUserProfile(object reqData)
        {
            //TODO
            UpdateUserProfileReq request = null;

            try
            {
                request = JsonConvert.DeserializeObject<UpdateUserProfileReq>(reqData.ToString());
            }
            catch { }

            try
            {
                if (request == null)
                    return new MSResponse { ResCode = "400", ResDesc = CacheConfig.GetErrorMessage("400") };

                var clientResp = "";// mesevaClient.UpdateUserProfile(CacheConfig.UserName, CacheConfig.Password, request.LoginId, request.LoginPassword, request.UpdatedPassword,
                //   request.Emailid, request.MobileNo, request.Address1, request.Address2, request.City, request.State, request.RoleID);
                LogData.Write("MEESEVA", "MEESEVA-RESPONSE", LogMode.Info, string.Format("UpdateUserProfile => Response: {0}", clientResp));
                if (clientResp == "100")
                    return new MSResponse { ResCode = "000", ResDesc = "Success" };

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(clientResp);
                var root = xmlDoc.DocumentElement;

                var errorCodeNode = root.SelectSingleNode("Table1/ErrorCode");
                if (errorCodeNode != null)
                {
                    var errorCode = errorCodeNode.InnerText;
                    var errordesc = root.SelectSingleNode("Table1/ErrorDesc") != null ? root.SelectSingleNode("Table1/ErrorDesc").InnerText : string.Empty;

                    return new MSResponse { ResCode = errorCode, ResDesc = !string.IsNullOrEmpty(CacheConfig.GetErrorMessage(errorCode)) ? CacheConfig.GetErrorMessage(errorCode) : errordesc }; ;
                }

                return new MSResponse { ResCode = "301", ResDesc = CacheConfig.GetErrorMessage("301") };
            }
            catch (Exception ex)
            {
                LogData.Write("MEESEVA", "MEESEVA-Exception", LogMode.Excep, ex, string.Format("MesevaProcess => UpdateUserProfile: Ex: {0}", ex.Message));
                return new MSResponse { ResCode = "300", ResDesc = CacheConfig.GetErrorMessage("300") };
            }
        }

        #endregion [ REGISTRATION  SERVICES ]

        #endregion Public Methods
    }
}
