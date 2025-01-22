using System;
using System.Configuration;
using System.Text;
using System.Xml;
using eseva.Data;
using eseva.Models;
using eseva.Models.Requests;
using eseva.Models.Responses;
using eseva.Utilities;
using IMI.Logger;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace eseva
{
    public class MobileBillRequestProcess
    {
        #region Private Members

        private AuthRequestBean authBean;

        #endregion Private Members

        #region Constructors

        public MobileBillRequestProcess()
        {
            authBean = GetAuthBean();
        }

        #endregion Constructors

        #region Public Methods

        public BillResponse AirtelBill(object data)
        {
            BillRequest reqData = null;

            try
            {
                reqData = JsonConvert.DeserializeObject<BillRequest>(data.ToString());
            }
            catch { }

            try
            {
                if (authBean == null || reqData == null)
                    return new BillResponse { ResCode = "400", ResDesc = CacheConfigXml.GetErrorDescription("400") };
                
                authBean.strdeptcode = CacheConfigXml.GetDeptCode("AIRTEL");
                var response = new Tsmobileapss().AirtelBillDetails(reqData.Number, authBean);
                if (response.strResCode == "000")
                {
                    return new AirtelBillResp
                    {
                        ResCode = response.strResCode,
                        ResDesc = response.strResDesc,
                        AccountNo = response.strAccountno,
                        BillAmount = response.strBillamount,
                        ConsumerName = response.strConsumername,
                        ReqId = response.strReqid
                    };
                }

                var resDesc = CacheConfigXml.GetErrorDescription(response.strResCode);
                return new BillResponse { ResCode = response.strResCode, ResDesc = !string.IsNullOrEmpty(resDesc) ? resDesc : response.strResDesc };
            }
            catch (Exception ex)
            {
                LogData.Write("ESEVA", "ESEVA-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess- ProcessAirtelBillRequest- Ex:{0}", ex.Message));
                return new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        public BillResponse AirtelLandlineBill(object data)
        {
            BillRequest reqData = null;

            try
            {
                reqData = JsonConvert.DeserializeObject<BillRequest>(data.ToString());
            }
            catch { }

            try
            {
                if (authBean == null || reqData == null)
                    return new BillResponse { ResCode = "400", ResDesc = CacheConfigXml.GetErrorDescription("400") };

                authBean.strdeptcode = CacheConfigXml.GetDeptCode("AIRTEL_LANDLINE");
                var response = new Tsmobileapss().AirtelLandLineBillDetails(reqData.Number, authBean);
                if (response.strResCode == "000")
                {
                    return new AirtelLLBillResp
                    {
                        ResCode = response.strResCode,
                        ResDesc = response.strResDesc,
                        AccountNo = response.strAccountno,
                        BillAmount = response.strBillamount,
                        ConsumerName = response.strConsumername,
                        ReqId = response.strReqid
                    };
                }

                var resDesc = CacheConfigXml.GetErrorDescription(response.strResCode);
                return new BillResponse { ResCode = response.strResCode, ResDesc = !string.IsNullOrEmpty(resDesc) ? resDesc : response.strResDesc };
            }
            catch (Exception ex)
            {
                LogData.Write("ESEVA", "ESEVA-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess- ProcessAirtelLandLineBillRequest- Ex:{0}", ex.Message));
                return new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        public BillResponse IdeaBill(object data)
        {
            BillRequest reqData = null;

            try
            {
                reqData = JsonConvert.DeserializeObject<BillRequest>(data.ToString());
            }
            catch { }

            try
            {
                if (authBean == null || reqData == null)
                    return new BillResponse { ResCode = "400", ResDesc = CacheConfigXml.GetErrorDescription("400") };

                authBean.strdeptcode = CacheConfigXml.GetDeptCode("IDEA");
                var resp = new Tsmobileapss().IdeaBillDetails(reqData.Number, authBean);
                if (resp.strResCode == "000")
                {
                    return new IdeaBillResp
                    {
                        ResCode = resp.strResCode,
                        ResDesc = resp.strResDesc,
                        AccountNo = resp.strAccountno,
                        BillAmount = resp.strBillamount,
                        ConsumerName = resp.strConsumername,
                        ReqId = resp.strReqid,
                        Address1 = resp.strAdd1,
                        Address2 = resp.strAdd2,
                        Address3 = resp.strAdd3
                    };
                }

                var resDesc = CacheConfigXml.GetErrorDescription(resp.strResCode);
                return new BillResponse { ResCode = resp.strResCode, ResDesc = !string.IsNullOrEmpty(resDesc) ? resDesc : resp.strResDesc };
            }
            catch (Exception ex)
            {
                LogData.Write("ESEVA", "ESEVA-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess- ProcessIdeaBillRequest- Ex:{0}", ex.Message));
                return new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        public BillResponse WaterBill(object data)
        {
            BillRequest reqData = null;

            try
            {
                reqData = JsonConvert.DeserializeObject<BillRequest>(data.ToString());
            }
            catch { }

            try
            {
                if (authBean == null || reqData == null)
                    return new BillResponse { ResCode = "400", ResDesc = CacheConfigXml.GetErrorDescription("400") };

                authBean.strdeptcode = CacheConfigXml.GetDeptCode("HMWSSB");
                var resp = new Tsmobileapss().HmwssbBillDetails(reqData.Number, authBean);
                if (resp.strResCode == "000")
                {
                    return new WaterBillResp
                    {
                        ResCode = resp.strResCode,
                        ResDesc = resp.strResDesc,
                        ReqId = resp.strReqId,
                        Amount = resp.strAmount,
                        CanNo = resp.strCan,
                        MobileNo = resp.strMobileno,
                        Name = resp.strName,
                        Email = resp.strEmail,
                        Category = resp.strCategory,
                        Address = resp.strAddress
                    };
                }

                var resDesc = CacheConfigXml.GetErrorDescription(resp.strResCode);
                return new BillResponse { ResCode = resp.strResCode, ResDesc = !string.IsNullOrEmpty(resDesc) ? resDesc : resp.strResDesc };
            }
            catch (Exception ex)
            {
                LogData.Write("ESEVA", "ESEVA-Exception", LogMode.Excep, ex, string.Format("WaterBillReqProcess- ProcessHmssbBill- Ex:{0}", ex.Message));
                return new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        public BillResponse ActBillDetails(object data)
        {
            BillRequest reqData = null;

            try
            {
                reqData = JsonConvert.DeserializeObject<BillRequest>(data.ToString());
            }
            catch { }

            try
            {
                if (authBean == null || reqData == null)
                    return new BillResponse { ResCode = "400", ResDesc = CacheConfigXml.GetErrorDescription("400") };

                authBean.strdeptcode = CacheConfigXml.GetDeptCode("ACT");
                return GetActBillDetails(reqData, "ActBillDetails");

            }
            catch (Exception ex)
            {
                LogData.Write("ESEVA", "ESEVA-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess- ActBillDetails- Ex:{0}", ex.Message));
                return new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        public BillResponse RtaFeePaymentDetails(object data)
        {
            BillRequest reqData = null;

            try
            {
                reqData = JsonConvert.DeserializeObject<BillRequest>(data.ToString());
            }
            catch { }

            try
            {
                if (authBean == null || reqData == null)
                    return new BillResponse { ResCode = "400", ResDesc = CacheConfigXml.GetErrorDescription("400") };

                authBean.strdeptcode = CacheConfigXml.GetDeptCode("RTAFEE");
                return GetRTAFeePaymentDetails(reqData, "RtaFeePaymentDetails");
            }
            catch (Exception ex)
            {
                LogData.Write("ESEVA", "ESEVA-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess- RtaFeePaymentDetails- Ex:{0}", ex.Message));
                return new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        public BillResponse TTLBillDetails(object data)
        {
            BillRequest reqData = null;

            try
            {
                reqData = JsonConvert.DeserializeObject<BillRequest>(data.ToString());
            }
            catch { }

            try
            {
                if (authBean == null || reqData == null)
                    return new BillResponse { ResCode = "400", ResDesc = CacheConfigXml.GetErrorDescription("400") };

                authBean.strdeptcode = CacheConfigXml.GetDeptCode("TTL");
                return GetTTLBillDetails(reqData, "TTLBillDetails");
            }
            catch (Exception ex)
            {
                LogData.Write("ESEVA", "ESEVA-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess- TTLBillDetails- Ex:{0}", ex.Message));
                return new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        public BillResponse GetDistList(object data)
        {
            try
            {
                if (authBean == null)
                    return new BillResponse { ResCode = "400", ResDesc = CacheConfigXml.GetErrorDescription("400") };

                authBean.strdeptcode = CacheConfigXml.GetDeptCode("TSSPDCL");
                return GetDistListDetails("GetDistListDetails");
            }
            catch (Exception ex)
            {
                LogData.Write("ESEVA", "ESEVA-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess- GetDistList- Ex:{0}", ex.Message));
                return new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        public BillResponse GetCPDCLEROList(object data)
        {
            BillRequest reqData = null;

            try
            {
                reqData = JsonConvert.DeserializeObject<BillRequest>(data.ToString());
            }
            catch { }

            try
            {
                if (authBean == null || reqData == null)
                    return new BillResponse { ResCode = "400", ResDesc = CacheConfigXml.GetErrorDescription("400") };

                authBean.strdeptcode = CacheConfigXml.GetDeptCode("TSSPDCL");
                return GetEroListDetails(reqData, "getcpdclEroList");
            }
            catch (Exception ex)
            {
                LogData.Write("ESEVA", "ESEVA-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess- GetCPDCLEROList- Ex:{0}", ex.Message));
                return new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        public BillResponse GetCPDCLBill(object data)
        {
            CPDCLBillReq reqData = null;

            try
            {
                reqData = JsonConvert.DeserializeObject<CPDCLBillReq>(data.ToString());
            }
            catch { }

            try
            {
                if (authBean == null || reqData == null)
                    return new BillResponse { ResCode = "400", ResDesc = CacheConfigXml.GetErrorDescription("400") };

                authBean.strdeptcode = CacheConfigXml.GetDeptCode("TSSPDCL");
                return GetCPDCLBillDetails(reqData, "CpdclBillDetails");
            }
            catch (Exception ex)
            {
                LogData.Write("ESEVA", "ESEVA-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess- GetCPDCLBill- Ex:{0}", ex.Message));
                return new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        public PaymentResponse BillPayment(object data)
        {
            MobilePaymentRequest reqData = null;

            try
            {
                reqData = JsonConvert.DeserializeObject<MobilePaymentRequest>(data.ToString());
            }
            catch { }

            try
            {
                if (authBean == null || reqData == null)
                    return new PaymentResponse { ResCode = "400", ResDesc = CacheConfigXml.GetErrorDescription("400") };

                authBean.strdeptcode = CacheConfigXml.GetDeptCode(reqData.Network.ToUpper());
                PaymentResponse response = null;
                switch (reqData.Network.ToUpper())
                {
                    case "IDEA":
                        response = IdeaBillPayment(reqData, "IdeaBillPayment");
                        break;
                    case "AIRTEL":
                        response = AirtelBillPayment(reqData, "AirtelBillPayment");
                        break;
                    case "AIRTEL_LANDLINE":
                        response = AirtelLLBillPayment(reqData, "AirtelLandLineBillPayment");
                        break;
                    case "HMWSSB":
                        response = WaterBillPayment(reqData, "HmwssbBillPayment");
                        break;
                    case "ACT":
                        response = ActBillPayment(reqData, "ActBillPayment");
                        break;
                    case "RTAFEE":
                        response = RTAFeeBillPayment(reqData, "rtaFeePayment");
                        break;
                    case "TTL":
                        response = TTLBillPayment(reqData, "TTLBillPayment");
                        break;
                    case "TSSPDCL":
                        response = CPDCLBillPayment(reqData, "CpdclBillPayment");
                        break;
                    default:
                        response = new PaymentResponse { ResCode = "400", ResDesc = CacheConfigXml.GetErrorDescription("400") };
                        break;
                }

                return response;
            }
            catch (Exception ex)
            {
                LogData.Write("ESEVA", "ESEVA-Exception", LogMode.Excep, ex, string.Format("BillPayment- Ex: {0}", ex.Message));
                return new PaymentResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        public PaymentResponse ReceiptReprint(object data)
        {
            BillRequest reqData = null;

            try
            {
                reqData = JsonConvert.DeserializeObject<BillRequest>(data.ToString());
            }
            catch { }

            try
            {
                if (authBean == null || reqData == null)
                    return new PaymentResponse { ResCode = "400", ResDesc = CacheConfigXml.GetErrorDescription("400") };

                return GetReceiptReprint(reqData, "ReceiptReprint");
            }
            catch (Exception ex)
            {
                LogData.Write("ESEVA", "ESEVA-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess- ReceiptReprint- Ex:{0}", ex.Message));
                return new PaymentResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        #endregion Public Methods

        #region Private Methods

        private AuthRequestBean GetAuthBean()
        {
            AuthenticateMasterData objAuthMasterData = null;
            try
            {
                objAuthMasterData = CacheConfigXml.GetAuthDetails();
                if (objAuthMasterData != null)
                    return new AuthRequestBean { groupid = objAuthMasterData.Groupid, password = objAuthMasterData.Password, serviceid = objAuthMasterData.Serviceid, strCentrecode = objAuthMasterData.Centrecode, strdeptcode = objAuthMasterData.Deptcode, strDistcode = objAuthMasterData.Distcode, strStaffcode = objAuthMasterData.Staffcode, userId = objAuthMasterData.UserId };
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess- getAuthBean- Ex:{0}", ex.Message));
            }
            finally
            {
                objAuthMasterData = null;
            }

            return null;
        }

        private PaymentResponse GetReceiptReprint(BillRequest data, string action)
        {
            #region Request

            var sbXmlReq = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:gat=\"http://gateway.mobile.eseva.com\">");
            sbXmlReq.Append("<soapenv:Header/><soapenv:Body><gat:ReceiptReprint>");
            sbXmlReq.AppendFormat("<gat:strEsevaReqid>{0}</gat:strEsevaReqid><gat:strUserid>{1}</gat:strUserid><gat:strPassword>{2}</gat:strPassword>", data.Number, authBean.userId, authBean.password);
            sbXmlReq.Append("</gat:ReceiptReprint></soapenv:Body></soapenv:Envelope>");

            #endregion Request

            var webResponse = WebRequestProcess.DoHttpPost(ConfigurationManager.AppSettings["ESEVA_MOB_SERVICE_URL"], sbXmlReq.ToString(), action);

            var response = new PaymentResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webResponse);

                var returnNode = xmlDoc.DocumentElement.GetElementsByTagName("ns:return");
                if (returnNode == null || returnNode.Count <= 0)
                    return response;

                var ax21Manager = new XmlNamespaceManager(xmlDoc.NameTable);
                ax21Manager.AddNamespace("ax21", "http://auth.gateway.mobile.eseva.com/xsd");

                var resCode = returnNode[0].SelectSingleNode("ax21:strResCode", ax21Manager).InnerText;
                if (resCode == "000")
                {
                    response = new MobilePaymentResponse
                    {
                        ResCode = resCode,
                        ResDesc = returnNode[0].SelectSingleNode("ax21:strResDesc", ax21Manager).InnerText,
                        RegId = data.Tid,
                        RefNo = data.Number,
                        Receipt = returnNode[0].SelectSingleNode("ax21:strReceipt", ax21Manager).InnerText
                    };
                }
                else
                {
                    response.ResCode = resCode;
                    response.ResDesc = !string.IsNullOrEmpty(CacheConfigXml.GetErrorDescription(resCode)) ? CacheConfigXml.GetErrorDescription(resCode) : returnNode[0].SelectSingleNode("ax21:strResDesc", ax21Manager).InnerText;
                }

                return response;
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess => GetReceiptReprint- Ex:{0}", ex.Message));
                return new PaymentResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        private BillResponse GetActBillDetails(BillRequest data, string action)
        {
            #region Request

            var sbXmlReq = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:gat=\"http://gateway.mobile.eseva.com\" xmlns:xsd=\"http://auth.gateway.mobile.eseva.com/xsd\">");
            sbXmlReq.Append("<soapenv:Header/><soapenv:Body><gat:ActBillDetails>");
            sbXmlReq.AppendFormat("<gat:strMobileno>{0}</gat:strMobileno><gat:RequestBean>", data.Number);
            sbXmlReq.AppendFormat("<xsd:groupid>{0}</xsd:groupid><xsd:password>{1}</xsd:password><xsd:serviceid>{2}</xsd:serviceid><xsd:strCentrecode>{3}</xsd:strCentrecode>", authBean.groupid, authBean.password, authBean.serviceid, authBean.strCentrecode);
            sbXmlReq.AppendFormat("<xsd:strDistcode>{0}</xsd:strDistcode><xsd:strStaffcode>{1}</xsd:strStaffcode><xsd:strdeptcode>{2}</xsd:strdeptcode><xsd:userId>{3}</xsd:userId>", authBean.strDistcode, authBean.strStaffcode, authBean.strdeptcode, authBean.userId);
            sbXmlReq.Append("</gat:RequestBean></gat:ActBillDetails></soapenv:Body></soapenv:Envelope>");

            #endregion Request

            var reqStartTime = DateTime.Now.Ticks;
            var webResponse = WebRequestProcess.DoHttpPost(ConfigurationManager.AppSettings["ESEVA_MOB_SERVICE_URL"], sbXmlReq.ToString(), action);
            var reqEndTime = DateTime.Now.Ticks;

            var response = new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webResponse);

                var returnNode = xmlDoc.DocumentElement.GetElementsByTagName("ns:return");
                if (returnNode == null || returnNode.Count <= 0)
                    return response;

                var ax23Manager = new XmlNamespaceManager(xmlDoc.NameTable);
                ax23Manager.AddNamespace("ax23", "http://act.gateway.mobile.eseva.com/xsd");

                var resCode = returnNode[0].SelectSingleNode("ax23:strResCode", ax23Manager).InnerText;
                if (resCode == "000")
                {
                    var dataNode = returnNode[0];
                    response = new ActBillResp
                    {
                        ResCode = resCode,
                        ResDesc = dataNode.SelectSingleNode("ax23:strResDesc", ax23Manager).InnerText,
                        MobileNo = dataNode.SelectSingleNode("ax23:strMobileNo", ax23Manager).InnerText,
                        ReqId = dataNode.SelectSingleNode("ax23:strReqid", ax23Manager).InnerText,
                        AccountNo = dataNode.SelectSingleNode("ax23:strAccountno", ax23Manager).InnerText,
                        Address1 = dataNode.SelectSingleNode("ax23:strAdd1", ax23Manager).InnerText,
                        Address2 = dataNode.SelectSingleNode("ax23:strAdd2", ax23Manager).InnerText,
                        Address3 = dataNode.SelectSingleNode("ax23:strAdd3", ax23Manager).InnerText,
                        BillNo = dataNode.SelectSingleNode("ax23:strBillNo", ax23Manager).InnerText,
                        BillDate = dataNode.SelectSingleNode("ax23:strBillDate", ax23Manager).InnerText,
                        BillAmount = dataNode.SelectSingleNode("ax23:strBillamount", ax23Manager).InnerText,
                        City = dataNode.SelectSingleNode("ax23:strCity", ax23Manager).InnerText,
                        CustomerName = dataNode.SelectSingleNode("ax23:strConsumername", ax23Manager).InnerText,
                        Country = dataNode.SelectSingleNode("ax23:strCountry", ax23Manager).InnerText,
                        District = dataNode.SelectSingleNode("ax23:strDistrict", ax23Manager).InnerText,
                        ErrCD = dataNode.SelectSingleNode("ax23:strErrcd", ax23Manager).InnerText,
                        Message = dataNode.SelectSingleNode("ax23:strMsg", ax23Manager).InnerText,
                        Period = dataNode.SelectSingleNode("ax23:strPeriod", ax23Manager).InnerText,
                        Region = dataNode.SelectSingleNode("ax23:strRegion", ax23Manager).InnerText,
                        State = dataNode.SelectSingleNode("ax23:strState", ax23Manager).InnerText,
                        TransNo = dataNode.SelectSingleNode("ax23:strTransNo", ax23Manager).InnerText
                    };
                }
                else
                {
                    response.ResCode = resCode;
                    response.ResDesc = !string.IsNullOrEmpty(CacheConfigXml.GetErrorDescription(resCode)) ? CacheConfigXml.GetErrorDescription(resCode) : returnNode[0].SelectSingleNode("ax23:strResDesc", ax23Manager).InnerText;
                }

                return response;
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva-Exception", LogMode.Excep, ex, string.Format("WaterBillReqProcess- GetActBillDetails- Ex:{0}", ex.Message));
                return new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        private BillResponse GetRTAFeePaymentDetails(BillRequest data, string action)
        {
            #region Request

            var sbXmlReq = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:gat=\"http://gateway.mobile.eseva.com\" xmlns:xsd=\"http://auth.gateway.mobile.eseva.com/xsd\">");
            sbXmlReq.Append("<soapenv:Header/><soapenv:Body><gat:ActBillDetails>");
            sbXmlReq.AppendFormat("<gat:strConsno>{0}</gat:strConsno><gat:RequestBean>", data.Number);
            sbXmlReq.AppendFormat("<xsd:groupid>{0}</xsd:groupid><xsd:password>{1}</xsd:password><xsd:serviceid>{2}</xsd:serviceid><xsd:strCentrecode>{3}</xsd:strCentrecode>", authBean.groupid, authBean.password, authBean.serviceid, authBean.strCentrecode);
            sbXmlReq.AppendFormat("<xsd:strDistcode>{0}</xsd:strDistcode><xsd:strStaffcode>{1}</xsd:strStaffcode><xsd:strdeptcode>{2}</xsd:strdeptcode><xsd:userId>{3}</xsd:userId>", authBean.strDistcode, authBean.strStaffcode, authBean.strdeptcode, authBean.userId);
            sbXmlReq.Append("</gat:RequestBean></gat:ActBillDetails></soapenv:Body></soapenv:Envelope>");

            #endregion Request

            var reqStartTime = DateTime.Now.Ticks;
            var webResponse = WebRequestProcess.DoHttpPost(ConfigurationManager.AppSettings["ESEVA_MOB_SERVICE_URL"], sbXmlReq.ToString(), action);
            var reqEndTime = DateTime.Now.Ticks;

            var response = new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webResponse);

                var returnNode = xmlDoc.DocumentElement.GetElementsByTagName("ns:return");
                if (returnNode == null || returnNode.Count <= 0)
                    return response;

                var ax213Manager = new XmlNamespaceManager(xmlDoc.NameTable);
                ax213Manager.AddNamespace("ax213", "http://rtaFeePayment.gateway.mobile.eseva.com/xsd");

                var resCode = returnNode[0].SelectSingleNode("ax213:strResCode", ax213Manager).InnerText;
                if (resCode == "000")
                {
                    var dataNode = returnNode[0];
                    response = new RTAFeePaymentDetail
                    {
                        ResCode = resCode,
                        ResDesc = dataNode.SelectSingleNode("ax213:strResDesc", ax213Manager).InnerText,
                        AppFee = dataNode.SelectSingleNode("ax213:strAppFee", ax213Manager).InnerText,
                        ApplicationNo = dataNode.SelectSingleNode("ax213:strApplicationNo", ax213Manager).InnerText,
                        CardFee = dataNode.SelectSingleNode("ax213:strCardFee", ax213Manager).InnerText,
                        CompFee = dataNode.SelectSingleNode("ax213:strCompFee", ax213Manager).InnerText,
                        DeptTransId = dataNode.SelectSingleNode("ax213:strDeptTransId", ax213Manager).InnerText,
                        DocNo = dataNode.SelectSingleNode("ax213:strDocNo", ax213Manager).InnerText,
                        GreenTax = dataNode.SelectSingleNode("ax213:strGreenTax", ax213Manager).InnerText,
                        LateFee = dataNode.SelectSingleNode("ax213:strLateFee", ax213Manager).InnerText,
                        LifeTax = dataNode.SelectSingleNode("ax213:strLifeTax", ax213Manager).InnerText,
                        MobileNo = dataNode.SelectSingleNode("ax213:strMobileNo", ax213Manager).InnerText,
                        Name = dataNode.SelectSingleNode("ax213:strName", ax213Manager).InnerText,
                        OfficeCd = dataNode.SelectSingleNode("ax213:strOfficeCd", ax213Manager).InnerText,
                        PostFee = dataNode.SelectSingleNode("ax213:strPostFee", ax213Manager).InnerText,
                        Qtax = dataNode.SelectSingleNode("ax213:strQtax", ax213Manager).InnerText,
                        ReqId = dataNode.SelectSingleNode("ax213:strReqId", ax213Manager).InnerText,
                        RtaAmount = dataNode.SelectSingleNode("ax213:strRtaAmount", ax213Manager).InnerText,
                        ServiceId = dataNode.SelectSingleNode("ax213:strServiceId", ax213Manager).InnerText,
                        SlotDate = dataNode.SelectSingleNode("ax213:strSlotDate", ax213Manager).InnerText,
                        SlotTime = dataNode.SelectSingleNode("ax213:strSlotTime", ax213Manager).InnerText,
                        SlotTimeid = dataNode.SelectSingleNode("ax213:strSlotTimeid", ax213Manager).InnerText,
                        SrvcFee = dataNode.SelectSingleNode("ax213:strSrvcFee", ax213Manager).InnerText,
                        ServiceDesc = dataNode.SelectSingleNode("ax213:strServiceDesc", ax213Manager).InnerText,
                        TotAmt = dataNode.SelectSingleNode("ax213:strTotAmt", ax213Manager).InnerText,
                        UserChargs = dataNode.SelectSingleNode("ax213:strUserChargs", ax213Manager).InnerText
                    };
                }
                else
                {
                    response.ResCode = resCode;
                    response.ResDesc = !string.IsNullOrEmpty(CacheConfigXml.GetErrorDescription(resCode)) ? CacheConfigXml.GetErrorDescription(resCode) : returnNode[0].SelectSingleNode("ax213:strResDesc", ax213Manager).InnerText;
                }

                return response;
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva-Exception", LogMode.Excep, ex, string.Format("WaterBillReqProcess- GetActBillDetails- Ex:{0}", ex.Message));
                return new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        private BillResponse GetTTLBillDetails(BillRequest data, string action)
        {
            #region Request

            var sbXmlReq = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:gat=\"http://gateway.mobile.eseva.com\" xmlns:xsd=\"http://auth.gateway.mobile.eseva.com/xsd\">");
            sbXmlReq.Append("<soapenv:Header/><soapenv:Body><gat:TTLBillDetails>");
            sbXmlReq.AppendFormat("<gat:strMobileno>{0}</gat:strMobileno>", data.Number);
            sbXmlReq.AppendFormat("<gat:RequestBean><xsd:groupid>{0}</xsd:groupid><xsd:password>{1}</xsd:password><xsd:serviceid>{2}</xsd:serviceid><xsd:strCentrecode>{3}</xsd:strCentrecode>", authBean.groupid, authBean.password, authBean.serviceid, authBean.strCentrecode);
            sbXmlReq.AppendFormat("<xsd:strDistcode>{0}</xsd:strDistcode><xsd:strStaffcode>{1}</xsd:strStaffcode><xsd:strdeptcode>{2}</xsd:strdeptcode><xsd:userId>{3}</xsd:userId></gat:RequestBean>", authBean.strDistcode, authBean.strStaffcode, authBean.strdeptcode, authBean.userId);
            sbXmlReq.Append("</gat:TTLBillDetails></soapenv:Body></soapenv:Envelope>");

            #endregion Request

            var reqStartTime = DateTime.Now.Ticks;
            var webResponse = WebRequestProcess.DoHttpPost(ConfigurationManager.AppSettings["ESEVA_MOB_SERVICE_URL"], sbXmlReq.ToString(), action);
            var reqEndTime = DateTime.Now.Ticks;

            var response = new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webResponse);

                var returnNode = xmlDoc.DocumentElement.GetElementsByTagName("ns:return");
                if (returnNode == null || returnNode.Count <= 0)
                    return response;

                var ax217Manager = new XmlNamespaceManager(xmlDoc.NameTable);
                ax217Manager.AddNamespace("ax217", "http://ttl.gateway.mobile.eseva.com/xsd");

                var resCode = returnNode[0].SelectSingleNode("ax217:strResCode", ax217Manager).InnerText;
                if (resCode == "000")
                {
                    var dataNode = returnNode[0];
                    response = new TTLBillResp
                    {
                        ResCode = resCode,
                        ResDesc = dataNode.SelectSingleNode("ax217:strResDesc", ax217Manager).InnerText,
                        MobileNo = dataNode.SelectSingleNode("ax217:strMobileNo", ax217Manager).InnerText,
                        ReqId = dataNode.SelectSingleNode("ax217:strReqid", ax217Manager).InnerText,
                        AccountNo = dataNode.SelectSingleNode("ax217:strAccountno", ax217Manager).InnerText,
                        BillAmount = dataNode.SelectSingleNode("ax217:strBillamount", ax217Manager).InnerText,
                        ConsumerName = dataNode.SelectSingleNode("ax217:strConsumername", ax217Manager).InnerText
                    };
                }
                else
                {
                    response.ResCode = resCode;
                    response.ResDesc = !string.IsNullOrEmpty(CacheConfigXml.GetErrorDescription(resCode)) ? CacheConfigXml.GetErrorDescription(resCode) : returnNode[0].SelectSingleNode("ax217:strResDesc", ax217Manager).InnerText;
                }

                return response;
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva-Exception", LogMode.Excep, ex, string.Format("WaterBillReqProcess- GetActBillDetails- Ex:{0}", ex.Message));
                return new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        private BillResponse GetDistListDetails(string action)
        {
            #region Request

            var sbXmlReq = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:gat=\"http://gateway.mobile.eseva.com\" xmlns:xsd=\"http://auth.gateway.mobile.eseva.com/xsd\">");
            sbXmlReq.Append("<soapenv:Header/><soapenv:Body><gat:getdistList><gat:RequestBean>");
            sbXmlReq.AppendFormat("<xsd:groupid>{0}</xsd:groupid><xsd:password>{1}</xsd:password><xsd:serviceid>{2}</xsd:serviceid><xsd:strCentrecode>{3}</xsd:strCentrecode>", authBean.groupid, authBean.password, authBean.serviceid, authBean.strCentrecode);
            sbXmlReq.AppendFormat("<xsd:strDistcode>{0}</xsd:strDistcode><xsd:strStaffcode>{1}</xsd:strStaffcode><xsd:strdeptcode>{2}</xsd:strdeptcode><xsd:userId>{3}</xsd:userId>", authBean.strDistcode, authBean.strStaffcode, authBean.strdeptcode, authBean.userId);
            sbXmlReq.Append("</gat:RequestBean></gat:getdistList></soapenv:Body></soapenv:Envelope>");

            #endregion Request

            var reqStartTime = DateTime.Now.Ticks;
            var webResponse = WebRequestProcess.DoHttpPost(ConfigurationManager.AppSettings["ESEVA_MOB_SERVICE_URL"], sbXmlReq.ToString(), action);
            var reqEndTime = DateTime.Now.Ticks;

            var response = new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webResponse);

                var returnNode = xmlDoc.DocumentElement.GetElementsByTagName("ns:return");
                if (returnNode == null || returnNode.Count <= 0)
                    return response;

                var ax29Manager = new XmlNamespaceManager(xmlDoc.NameTable);
                ax29Manager.AddNamespace("ax29", "http://cpdcl.gateway.mobile.eseva.com/xsd");

                var resCode = returnNode[0].SelectSingleNode("ax29:strResCode", ax29Manager).InnerText;
                if (resCode == "000")
                {
                    var resDesc = returnNode[0].SelectSingleNode("ax29:strResDesc", ax29Manager).InnerText;
                    var strDistList = returnNode[0].SelectSingleNode("ax29:strDistxml", ax29Manager).InnerText;

                    xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(strDistList);

                    var distList = new List<DistData>();
                    
                    var distDataList = xmlDoc.DocumentElement.GetElementsByTagName("Distdata");
                    foreach (XmlNode node in distDataList)
                        distList.Add(new DistData { DistCode = node.SelectSingleNode("DISTCODE").InnerText, DistName = node.SelectSingleNode("DISTNAME").InnerText });

                    response = new DistListDetailResp { ResCode = resCode, ResDesc = resDesc, DistList = distList };
                }
                else
                {
                    response.ResCode = resCode;
                    response.ResDesc = !string.IsNullOrEmpty(CacheConfigXml.GetErrorDescription(resCode)) ? CacheConfigXml.GetErrorDescription(resCode) : returnNode[0].SelectSingleNode("ax29:strResDesc", ax29Manager).InnerText;
                }

                return response;
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva-Exception", LogMode.Excep, ex, string.Format("WaterBillReqProcess- GetDistListDetails- Ex:{0}", ex.Message));
                return new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        private BillResponse GetEroListDetails(BillRequest data, string action)
        {
            #region Request

            var sbXmlReq = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:gat=\"http://gateway.mobile.eseva.com\" xmlns:xsd=\"http://auth.gateway.mobile.eseva.com/xsd\">");
            sbXmlReq.Append("<soapenv:Header/><soapenv:Body><gat:getcpdclEroList>");
            sbXmlReq.AppendFormat("<gat:distcode>{0}</gat:distcode><gat:RequestBean>", data.Number);
            sbXmlReq.AppendFormat("<xsd:groupid>{0}</xsd:groupid><xsd:password>{1}</xsd:password><xsd:serviceid>{2}</xsd:serviceid><xsd:strCentrecode>{3}</xsd:strCentrecode>", authBean.groupid, authBean.password, authBean.serviceid, authBean.strCentrecode);
            sbXmlReq.AppendFormat("<xsd:strDistcode>{0}</xsd:strDistcode><xsd:strStaffcode>{1}</xsd:strStaffcode><xsd:strdeptcode>{2}</xsd:strdeptcode><xsd:userId>{3}</xsd:userId>", authBean.strDistcode, authBean.strStaffcode, authBean.strdeptcode, authBean.userId);
            sbXmlReq.Append("</gat:RequestBean></gat:getcpdclEroList></soapenv:Body></soapenv:Envelope>");

            #endregion Request

            var reqStartTime = DateTime.Now.Ticks;
            var webResponse = WebRequestProcess.DoHttpPost(ConfigurationManager.AppSettings["ESEVA_MOB_SERVICE_URL"], sbXmlReq.ToString(), action);
            var reqEndTime = DateTime.Now.Ticks;

            var response = new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webResponse);

                var returnNode = xmlDoc.DocumentElement.GetElementsByTagName("ns:return");
                if (returnNode == null || returnNode.Count <= 0)
                    return response;

                var ax29Manager = new XmlNamespaceManager(xmlDoc.NameTable);
                ax29Manager.AddNamespace("ax29", "http://cpdcl.gateway.mobile.eseva.com/xsd");

                var resCode = returnNode[0].SelectSingleNode("ax29:strResCode", ax29Manager).InnerText;
                if (resCode == "000")
                {
                    var resDesc = returnNode[0].SelectSingleNode("ax29:strResDesc", ax29Manager).InnerText;
                    var strEroList = returnNode[0].SelectSingleNode("ax29:strEroxml", ax29Manager).InnerText;

                    xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(strEroList);

                    var eroList = new List<EroData>();

                    var eroDataList = xmlDoc.DocumentElement.GetElementsByTagName("Erodata");
                    foreach (XmlNode node in eroDataList)
                        eroList.Add(new EroData { EroCode = node.SelectSingleNode("EROCODE").InnerText, EroName = node.SelectSingleNode("ERONAME").InnerText });

                    response = new CPDCLEROListResp { ResCode = resCode, ResDesc = resDesc, EroList = eroList };
                }
                else
                {
                    response.ResCode = resCode;
                    response.ResDesc = !string.IsNullOrEmpty(CacheConfigXml.GetErrorDescription(resCode)) ? CacheConfigXml.GetErrorDescription(resCode) : returnNode[0].SelectSingleNode("ax29:strResDesc", ax29Manager).InnerText;
                }

                return response;
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva-Exception", LogMode.Excep, ex, string.Format("WaterBillReqProcess- GetDistListDetails- Ex:{0}", ex.Message));
                return new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        private BillResponse GetCPDCLBillDetails(CPDCLBillReq data, string action)
        {
            #region Request

            var sbXmlReq = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:gat=\"http://gateway.mobile.eseva.com\" xmlns:xsd=\"http://auth.gateway.mobile.eseva.com/xsd\"><soapenv:Header/><soapenv:Body>");
            sbXmlReq.AppendFormat("<gat:CpdclBillDetails><gat:strConsno>{0}</gat:strConsno><gat:erocode>{1}</gat:erocode><gat:RequestBean>", data.Number, data.EroCode);
            sbXmlReq.AppendFormat("<xsd:groupid>{0}</xsd:groupid><xsd:password>{1}</xsd:password><xsd:serviceid>{2}</xsd:serviceid><xsd:strCentrecode>{3}</xsd:strCentrecode>", authBean.groupid, authBean.password, authBean.serviceid, authBean.strCentrecode);
            sbXmlReq.AppendFormat("<xsd:strDistcode>{0}</xsd:strDistcode><xsd:strStaffcode>{1}</xsd:strStaffcode><xsd:strdeptcode>{2}</xsd:strdeptcode><xsd:userId>{3}</xsd:userId>", authBean.strDistcode, authBean.strStaffcode, authBean.strdeptcode, authBean.userId);
            sbXmlReq.Append("</gat:RequestBean></gat:CpdclBillDetails></soapenv:Body></soapenv:Envelope>");

            #endregion Request

            var reqStartTime = DateTime.Now.Ticks;
            var webResponse = WebRequestProcess.DoHttpPost(ConfigurationManager.AppSettings["ESEVA_MOB_SERVICE_URL"], sbXmlReq.ToString(), action);
            var reqEndTime = DateTime.Now.Ticks;

            var response = new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webResponse);

                var returnNode = xmlDoc.DocumentElement.GetElementsByTagName("ns:return");
                if (returnNode == null || returnNode.Count <= 0)
                    return response;

                var ax29Manager = new XmlNamespaceManager(xmlDoc.NameTable);
                ax29Manager.AddNamespace("ax29", "http://cpdcl.gateway.mobile.eseva.com/xsd");

                var resCode = returnNode[0].SelectSingleNode("ax29:strResCode", ax29Manager).InnerText;
                if (resCode == "000")
                {
                    response = new CPDCLBillResp 
                    {
                        Address1 = returnNode[0].SelectSingleNode("ax29:strAddress1", ax29Manager).InnerText,
                        Address2 = returnNode[0].SelectSingleNode("ax29:strAddress2", ax29Manager).InnerText,
                        Arrears = returnNode[0].SelectSingleNode("ax29:strarrears", ax29Manager).InnerText,
                        Billdate = returnNode[0].SelectSingleNode("ax29:strbilldate", ax29Manager).InnerText,
                        Billno = returnNode[0].SelectSingleNode("ax29:strbillno", ax29Manager).InnerText,
                        Category = returnNode[0].SelectSingleNode("ax29:strCategory", ax29Manager).InnerText,
                        Consumername = returnNode[0].SelectSingleNode("ax29:strconsumername", ax29Manager).InnerText,
                        Consumerno = returnNode[0].SelectSingleNode("ax29:strconsumerno", ax29Manager).InnerText,
                        Currentdmd = returnNode[0].SelectSingleNode("ax29:strcurrentdmd", ax29Manager).InnerText,
                        Discondate = returnNode[0].SelectSingleNode("ax29:strdiscondate", ax29Manager).InnerText,
                        Duedate = returnNode[0].SelectSingleNode("ax29:strduedate", ax29Manager).InnerText,
                        Mobileno = returnNode[0].SelectSingleNode("ax29:strMobileno", ax29Manager).InnerText,
                        NetAmount = returnNode[0].SelectSingleNode("ax29:strNetAmount", ax29Manager).InnerText,
                        ReqId = returnNode[0].SelectSingleNode("ax29:strReqId", ax29Manager).InnerText,
                        ResCode = resCode,
                        ResDesc = returnNode[0].SelectSingleNode("ax29:strResDesc", ax29Manager).InnerText,
                        Ukscno = returnNode[0].SelectSingleNode("ax29:strukscno", ax29Manager).InnerText,
                        Usercharges = returnNode[0].SelectSingleNode("ax29:strUsercharges", ax29Manager).InnerText
                    };
                }
                else
                {
                    response.ResCode = resCode;
                    response.ResDesc = !string.IsNullOrEmpty(CacheConfigXml.GetErrorDescription(resCode)) ? CacheConfigXml.GetErrorDescription(resCode) : returnNode[0].SelectSingleNode("ax29:strResDesc", ax29Manager).InnerText;
                }

                return response;
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva-Exception", LogMode.Excep, ex, string.Format("WaterBillReqProcess- GetDistListDetails- Ex:{0}", ex.Message));
                return new BillResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };
            }
        }

        private PaymentResponse IdeaBillPayment(MobilePaymentRequest data, string action)
        {
            #region Request

            var sbXmlReq = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:gat=\"http://gateway.mobile.eseva.com\" xmlns:xsd=\"http://auth.gateway.mobile.eseva.com/xsd\">");
            sbXmlReq.Append("<soapenv:Header/><soapenv:Body><gat:IdeaBillPayment>");
            sbXmlReq.Append("<gat:PayReq>");
            sbXmlReq.AppendFormat("<xsd:strEsevaReqid>{0}</xsd:strEsevaReqid>", data.BillRefNo);
            sbXmlReq.AppendFormat("<xsd:strPaidAmt>{0}</xsd:strPaidAmt>", data.Amount);
            sbXmlReq.AppendFormat("<xsd:strVendorRefno>{0}</xsd:strVendorRefno>", data.tid);
            sbXmlReq.Append("</gat:PayReq>");
            sbXmlReq.Append("<gat:RequestBean>");
            sbXmlReq.AppendFormat("<xsd:groupid>{0}</xsd:groupid>", authBean.groupid);
            sbXmlReq.AppendFormat("<xsd:password>{0}</xsd:password>", authBean.password);
            sbXmlReq.AppendFormat("<xsd:serviceid>{0}</xsd:serviceid>", authBean.serviceid);
            sbXmlReq.AppendFormat("<xsd:strCentrecode>{0}</xsd:strCentrecode>", authBean.strCentrecode);
            sbXmlReq.AppendFormat("<xsd:strDistcode>{0}</xsd:strDistcode>", authBean.strDistcode);
            sbXmlReq.AppendFormat("<xsd:strStaffcode>{0}</xsd:strStaffcode>", authBean.strStaffcode);
            sbXmlReq.AppendFormat("<xsd:strdeptcode>{0}</xsd:strdeptcode>", authBean.strdeptcode);
            sbXmlReq.AppendFormat("<xsd:userId>{0}</xsd:userId>", authBean.userId);
            sbXmlReq.Append("</gat:RequestBean>");
            sbXmlReq.Append("</gat:IdeaBillPayment></soapenv:Body></soapenv:Envelope>");

            #endregion Request

            var reqStartTime = DateTime.Now.Ticks;
            var webResponse = WebRequestProcess.DoHttpPost(ConfigurationManager.AppSettings["ESEVA_MOB_SERVICE_URL"], sbXmlReq.ToString(), action);
            var reqEndTime = DateTime.Now.Ticks;

            var response = new PaymentResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };

            try
            {
                var queuePath = ConfigurationManager.AppSettings["MSMQ_QUEUE_URL"].ToString();
                var queueLabel = ConfigurationManager.AppSettings["MSMQ_ESEVA_LABEL"].ToString();
                var queueStr = "<xml><RefId>{0}</RefId><MethodName>{1}</MethodName><DeptName>{2}</DeptName><ServiceCode>{3}</ServiceCode><Request>{4}</Request><Response>{5}</Response><TimeTaken>{6}</TimeTaken><Reqtime>{7}</Reqtime><Restime>{8}</Restime></xml>";
                var queueLog = string.Format(queueStr, data.BillRefNo, action, "ESEVA", authBean.strdeptcode, sbXmlReq.ToString(), webResponse, ((reqEndTime - reqStartTime) / 10000), reqStartTime, reqEndTime);
                MSMQPush.MessagePush(queuePath, queueLabel, queueLog);

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webResponse);

                var returnNode = xmlDoc.DocumentElement.GetElementsByTagName("ns:return");

                var ax21Manager = new XmlNamespaceManager(xmlDoc.NameTable);
                ax21Manager.AddNamespace("ax21", "http://auth.gateway.mobile.eseva.com/xsd");

                if (returnNode == null || returnNode.Count <= 0)
                {
                    var faultNode = xmlDoc.DocumentElement.GetElementsByTagName("soapenv:Fault");
                    if (faultNode.Count > 0)
                    {
                        var faultCode = faultNode[0].SelectSingleNode("faultcode");
                        var faultDesc = faultNode[0].SelectSingleNode("faultstring");
                        LogData.Write("MSDGAPI", "ESeva", LogMode.Error, string.Format("MobileBillRequestProcess :: IdeaBillPayment => Error occurred on idea api side Fault code: {0}, Desc: {1}", faultCode, faultDesc));
                    }

                    return response;
                }

                var resCode = returnNode[0].SelectSingleNode("ax21:strResCode", ax21Manager).InnerText;
                if (resCode == "000")
                {
                    response = new MobilePaymentResponse
                    {
                        ResCode = resCode,
                        ResDesc = returnNode[0].SelectSingleNode("ax21:strResDesc", ax21Manager).InnerText,
                        RegId = data.tid,
                        RefNo = data.BillRefNo,
                        Receipt = returnNode[0].SelectSingleNode("ax21:strReceipt", ax21Manager).InnerText
                    };
                }
                else
                {
                    response.ResCode = resCode;
                    response.ResDesc = !string.IsNullOrEmpty(CacheConfigXml.GetErrorDescription(resCode)) ? CacheConfigXml.GetErrorDescription(resCode) : returnNode[0].SelectSingleNode("ax21:strResDesc", ax21Manager).InnerText;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess :: IdeaBillPayment => Exception: {0}", ex.Message));
            }

            return response;
        }

        private PaymentResponse AirtelBillPayment(MobilePaymentRequest data, string action)
        {
            #region Request

            var sbXmlReq = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:gat=\"http://gateway.mobile.eseva.com\" xmlns:xsd=\"http://auth.gateway.mobile.eseva.com/xsd\">");
            sbXmlReq.Append("<soapenv:Header/><soapenv:Body><gat:AirtelBillPayment>");
            sbXmlReq.Append("<gat:PayReq>");
            sbXmlReq.AppendFormat("<xsd:strEsevaReqid>{0}</xsd:strEsevaReqid>", data.BillRefNo);
            sbXmlReq.AppendFormat("<xsd:strPaidAmt>{0}</xsd:strPaidAmt>", data.Amount);
            sbXmlReq.AppendFormat("<xsd:strVendorRefno>{0}</xsd:strVendorRefno>", data.tid);
            sbXmlReq.Append("</gat:PayReq>");
            sbXmlReq.Append("<gat:RequestBean>");
            sbXmlReq.AppendFormat("<xsd:groupid>{0}</xsd:groupid>", authBean.groupid);
            sbXmlReq.AppendFormat("<xsd:password>{0}</xsd:password>", authBean.password);
            sbXmlReq.AppendFormat("<xsd:serviceid>{0}</xsd:serviceid>", authBean.serviceid);
            sbXmlReq.AppendFormat("<xsd:strCentrecode>{0}</xsd:strCentrecode>", authBean.strCentrecode);
            sbXmlReq.AppendFormat("<xsd:strDistcode>{0}</xsd:strDistcode>", authBean.strDistcode);
            sbXmlReq.AppendFormat("<xsd:strStaffcode>{0}</xsd:strStaffcode>", authBean.strStaffcode);
            sbXmlReq.AppendFormat("<xsd:strdeptcode>{0}</xsd:strdeptcode>", authBean.strdeptcode);
            sbXmlReq.AppendFormat("<xsd:userId>{0}</xsd:userId>", authBean.userId);
            sbXmlReq.Append("</gat:RequestBean>");
            sbXmlReq.Append("</gat:AirtelBillPayment></soapenv:Body></soapenv:Envelope>");

            #endregion Request

            var reqStartTime = DateTime.Now.Ticks;
            var webResponse = WebRequestProcess.DoHttpPost(ConfigurationManager.AppSettings["ESEVA_MOB_SERVICE_URL"], sbXmlReq.ToString(), action);
            var reqEndTime = DateTime.Now.Ticks;

            var response = new PaymentResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };

            try
            {
                var queuePath = ConfigurationManager.AppSettings["MSMQ_QUEUE_URL"].ToString();
                var queueLabel = ConfigurationManager.AppSettings["MSMQ_ESEVA_LABEL"].ToString();
                var queueStr = "<xml><RefId>{0}</RefId><MethodName>{1}</MethodName><DeptName>{2}</DeptName><ServiceCode>{3}</ServiceCode><Request>{4}</Request><Response>{5}</Response><TimeTaken>{6}</TimeTaken><Reqtime>{7}</Reqtime><Restime>{8}</Restime></xml>";
                var queueLog = string.Format(queueStr, data.BillRefNo, action, "ESEVA", authBean.strdeptcode, sbXmlReq.ToString(), webResponse, ((reqEndTime - reqStartTime) / 10000), reqStartTime, reqEndTime);
                MSMQPush.MessagePush(queuePath, queueLabel, queueLog);

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webResponse);

                var returnNode = xmlDoc.DocumentElement.GetElementsByTagName("ns:return");

                var ax21Manager = new XmlNamespaceManager(xmlDoc.NameTable);
                ax21Manager.AddNamespace("ax21", "http://auth.gateway.mobile.eseva.com/xsd");

                if (returnNode == null || returnNode.Count <= 0)
                {
                    var faultNode = xmlDoc.DocumentElement.GetElementsByTagName("soapenv:Fault");
                    if (faultNode.Count > 0)
                    {
                        var faultCode = faultNode[0].SelectSingleNode("faultcode");
                        var faultDesc = faultNode[0].SelectSingleNode("faultstring");
                        LogData.Write("MSDGAPI", "ESeva", LogMode.Error, string.Format("MobileBillRequestProcess :: AirtelBillPayment => Error occurred on api side Fault code: {0}, Desc: {1}", faultCode, faultDesc));
                    }

                    return response;
                }

                var resCode = returnNode[0].SelectSingleNode("ax21:strResCode", ax21Manager).InnerText;
                if (resCode == "000")
                {
                    response = new MobilePaymentResponse
                    {
                        ResCode = resCode,
                        ResDesc = returnNode[0].SelectSingleNode("ax21:strResDesc", ax21Manager).InnerText,
                        RegId = data.tid,
                        RefNo = data.BillRefNo,
                        Receipt = returnNode[0].SelectSingleNode("ax21:strReceipt", ax21Manager).InnerText
                    };
                }
                else
                {
                    response.ResCode = resCode;
                    response.ResDesc = !string.IsNullOrEmpty(CacheConfigXml.GetErrorDescription(resCode)) ? CacheConfigXml.GetErrorDescription(resCode) : returnNode[0].SelectSingleNode("ax21:strResDesc", ax21Manager).InnerText;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess :: AirtelBillPayment => Exception: {0}", ex.Message));
            }

            return response;
        }

        private PaymentResponse AirtelLLBillPayment(MobilePaymentRequest data, string action)
        {
            #region Request

            var sbXmlReq = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:gat=\"http://gateway.mobile.eseva.com\" xmlns:xsd=\"http://auth.gateway.mobile.eseva.com/xsd\">");
            sbXmlReq.Append("<soapenv:Header/><soapenv:Body><gat:AirtelLandLineBillPayment>");
            sbXmlReq.Append("<gat:PayReq>");
            sbXmlReq.AppendFormat("<xsd:strEsevaReqid>{0}</xsd:strEsevaReqid>", data.BillRefNo);
            sbXmlReq.AppendFormat("<xsd:strPaidAmt>{0}</xsd:strPaidAmt>", data.Amount);
            sbXmlReq.AppendFormat("<xsd:strVendorRefno>{0}</xsd:strVendorRefno>", data.tid);
            sbXmlReq.Append("</gat:PayReq>");
            sbXmlReq.Append("<gat:RequestBean>");
            sbXmlReq.AppendFormat("<xsd:groupid>{0}</xsd:groupid>", authBean.groupid);
            sbXmlReq.AppendFormat("<xsd:password>{0}</xsd:password>", authBean.password);
            sbXmlReq.AppendFormat("<xsd:serviceid>{0}</xsd:serviceid>", authBean.serviceid);
            sbXmlReq.AppendFormat("<xsd:strCentrecode>{0}</xsd:strCentrecode>", authBean.strCentrecode);
            sbXmlReq.AppendFormat("<xsd:strDistcode>{0}</xsd:strDistcode>", authBean.strDistcode);
            sbXmlReq.AppendFormat("<xsd:strStaffcode>{0}</xsd:strStaffcode>", authBean.strStaffcode);
            sbXmlReq.AppendFormat("<xsd:strdeptcode>{0}</xsd:strdeptcode>", authBean.strdeptcode);
            sbXmlReq.AppendFormat("<xsd:userId>{0}</xsd:userId>", authBean.userId);
            sbXmlReq.Append("</gat:RequestBean>");
            sbXmlReq.Append("</gat:AirtelLandLineBillPayment></soapenv:Body></soapenv:Envelope>");

            #endregion Request

            var reqStartTime = DateTime.Now.Ticks;
            var webResponse = WebRequestProcess.DoHttpPost(ConfigurationManager.AppSettings["ESEVA_MOB_SERVICE_URL"], sbXmlReq.ToString(), action);
            var reqEndTime = DateTime.Now.Ticks;

            var response = new PaymentResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };

            try
            {
                var queuePath = ConfigurationManager.AppSettings["MSMQ_QUEUE_URL"].ToString();
                var queueLabel = ConfigurationManager.AppSettings["MSMQ_ESEVA_LABEL"].ToString();
                var queueStr = "<xml><RefId>{0}</RefId><MethodName>{1}</MethodName><DeptName>{2}</DeptName><ServiceCode>{3}</ServiceCode><Request>{4}</Request><Response>{5}</Response><TimeTaken>{6}</TimeTaken><Reqtime>{7}</Reqtime><Restime>{8}</Restime></xml>";
                var queueLog = string.Format(queueStr, data.BillRefNo, action, "ESEVA", authBean.strdeptcode, sbXmlReq.ToString(), webResponse, ((reqEndTime - reqStartTime) / 10000), reqStartTime, reqEndTime);
                MSMQPush.MessagePush(queuePath, queueLabel, queueLog);

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webResponse);

                var returnNode = xmlDoc.DocumentElement.GetElementsByTagName("ns:return");

                var ax25Manager = new XmlNamespaceManager(xmlDoc.NameTable);
                ax25Manager.AddNamespace("ax25", "http://airtelLandLine.gateway.mobile.eseva.com/xsd");

                if (returnNode == null || returnNode.Count <= 0)
                {
                    var faultNode = xmlDoc.DocumentElement.GetElementsByTagName("soapenv:Fault");
                    if (faultNode.Count > 0)
                    {
                        var faultCode = faultNode[0].SelectSingleNode("faultcode");
                        var faultDesc = faultNode[0].SelectSingleNode("faultstring");
                        LogData.Write("MSDGAPI", "ESeva", LogMode.Error, string.Format("MobileBillRequestProcess :: AirtelLLBillPayment => Error occurred on api side Fault code: {0}, Desc: {1}", faultCode, faultDesc));
                    }

                    return response;
                }

                var resCode = returnNode[0].SelectSingleNode("ax25:strResCode", ax25Manager).InnerText;
                if (resCode == "000")
                {
                    response = new MobilePaymentResponse
                    {
                        ResCode = resCode,
                        ResDesc = returnNode[0].SelectSingleNode("ax25:strResDesc", ax25Manager).InnerText,
                        RegId = data.tid,
                        RefNo = data.BillRefNo,
                        Receipt = returnNode[0].SelectSingleNode("ax25:strReceipt", ax25Manager).InnerText
                    };
                }
                else
                {
                    response.ResCode = resCode;
                    response.ResDesc = !string.IsNullOrEmpty(CacheConfigXml.GetErrorDescription(resCode)) ? CacheConfigXml.GetErrorDescription(resCode) : returnNode[0].SelectSingleNode("ax25:strResDesc", ax25Manager).InnerText;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess :: AirtelLLBillPayment => Exception: {0}", ex.Message));
            }

            return response;
        }

        private PaymentResponse WaterBillPayment(MobilePaymentRequest data, string action)
        {
            #region Request

            var sbXmlReq = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:gat=\"http://gateway.mobile.eseva.com\" xmlns:xsd=\"http://auth.gateway.mobile.eseva.com/xsd\">");
            sbXmlReq.Append("<soapenv:Header/><soapenv:Body><gat:HmwssbBillPayment>");
            sbXmlReq.Append("<gat:PayReq>");
            sbXmlReq.AppendFormat("<xsd:strEsevaReqid>{0}</xsd:strEsevaReqid>", data.BillRefNo);
            sbXmlReq.AppendFormat("<xsd:strPaidAmt>{0}</xsd:strPaidAmt>", data.Amount);
            sbXmlReq.AppendFormat("<xsd:strVendorRefno>{0}</xsd:strVendorRefno>", data.tid);
            sbXmlReq.Append("</gat:PayReq>");
            sbXmlReq.Append("<gat:RequestBean>");
            sbXmlReq.AppendFormat("<xsd:groupid>{0}</xsd:groupid>", authBean.groupid);
            sbXmlReq.AppendFormat("<xsd:password>{0}</xsd:password>", authBean.password);
            sbXmlReq.AppendFormat("<xsd:serviceid>{0}</xsd:serviceid>", authBean.serviceid);
            sbXmlReq.AppendFormat("<xsd:strCentrecode>{0}</xsd:strCentrecode>", authBean.strCentrecode);
            sbXmlReq.AppendFormat("<xsd:strDistcode>{0}</xsd:strDistcode>", authBean.strDistcode);
            sbXmlReq.AppendFormat("<xsd:strStaffcode>{0}</xsd:strStaffcode>", authBean.strStaffcode);
            sbXmlReq.AppendFormat("<xsd:strdeptcode>{0}</xsd:strdeptcode>", authBean.strdeptcode);
            sbXmlReq.AppendFormat("<xsd:userId>{0}</xsd:userId>", authBean.userId);
            sbXmlReq.Append("</gat:RequestBean>");
            sbXmlReq.Append("</gat:HmwssbBillPayment></soapenv:Body></soapenv:Envelope>");

            #endregion Request

            var reqStartTime = DateTime.Now.Ticks;
            var webResponse = WebRequestProcess.DoHttpPost(ConfigurationManager.AppSettings["ESEVA_MOB_SERVICE_URL"], sbXmlReq.ToString(), action);
            var reqEndTime = DateTime.Now.Ticks;

            var response = new PaymentResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };

            try
            {
                var queuePath = ConfigurationManager.AppSettings["MSMQ_QUEUE_URL"].ToString();
                var queueLabel = ConfigurationManager.AppSettings["MSMQ_ESEVA_LABEL"].ToString();
                var queueStr = "<xml><RefId>{0}</RefId><MethodName>{1}</MethodName><DeptName>{2}</DeptName><ServiceCode>{3}</ServiceCode><Request>{4}</Request><Response>{5}</Response><TimeTaken>{6}</TimeTaken><Reqtime>{7}</Reqtime><Restime>{8}</Restime></xml>";
                var queueLog = string.Format(queueStr, data.BillRefNo, action, "ESEVA", authBean.strdeptcode, sbXmlReq.ToString(), webResponse, ((reqEndTime - reqStartTime) / 10000), reqStartTime, reqEndTime);
                MSMQPush.MessagePush(queuePath, queueLabel, queueLog);

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webResponse);

                var returnNode = xmlDoc.DocumentElement.GetElementsByTagName("ns:return");

                var ax27Manager = new XmlNamespaceManager(xmlDoc.NameTable);
                ax27Manager.AddNamespace("ax27", "http://hmwssb.gateway.mobile.eseva.com/xsd");

                if (returnNode == null || returnNode.Count <= 0)
                {
                    var faultNode = xmlDoc.DocumentElement.GetElementsByTagName("soapenv:Fault");
                    if (faultNode.Count > 0)
                    {
                        var faultCode = faultNode[0].SelectSingleNode("faultcode");
                        var faultDesc = faultNode[0].SelectSingleNode("faultstring");
                        LogData.Write("MSDGAPI", "ESeva", LogMode.Error, string.Format("MobileBillRequestProcess :: WaterBillPayment => Error occurred on api side Fault code: {0}, Desc: {1}", faultCode, faultDesc));
                    }

                    return response;
                }

                var resCode = returnNode[0].SelectSingleNode("ax27:strResCode", ax27Manager).InnerText;
                if (resCode == "000")
                {
                    response = new MobilePaymentResponse
                    {
                        ResCode = resCode,
                        ResDesc = returnNode[0].SelectSingleNode("ax27:strResDesc", ax27Manager).InnerText,
                        RegId = data.tid,
                        RefNo = data.BillRefNo,
                        Receipt = returnNode[0].SelectSingleNode("ax27:strReceipt", ax27Manager).InnerText
                    };
                }
                else
                {
                    response.ResCode = resCode;
                    response.ResDesc = !string.IsNullOrEmpty(CacheConfigXml.GetErrorDescription(resCode)) ? CacheConfigXml.GetErrorDescription(resCode) : returnNode[0].SelectSingleNode("ax27:strResDesc", ax27Manager).InnerText;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess :: WaterBillPayment => Exception: {0}", ex.Message));
            }

            return response;
        }

        private PaymentResponse ActBillPayment(MobilePaymentRequest data, string action)
        {
            #region Request

            var sbXmlReq = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:gat=\"http://gateway.mobile.eseva.com\" xmlns:xsd=\"http://auth.gateway.mobile.eseva.com/xsd\">");
            sbXmlReq.Append("<soapenv:Header/><soapenv:Body><gat:ActBillPayment>");
            sbXmlReq.AppendFormat("<gat:PayReq><xsd:strEsevaReqid>{0}</xsd:strEsevaReqid><xsd:strPaidAmt>{1}</xsd:strPaidAmt><xsd:strVendorRefno>{2}</xsd:strVendorRefno></gat:PayReq>", data.BillRefNo, data.Amount, data.tid);
            sbXmlReq.AppendFormat("<gat:RequestBean><xsd:groupid>{0}</xsd:groupid><xsd:password>{1}</xsd:password><xsd:serviceid>{2}</xsd:serviceid><xsd:strCentrecode>{3}</xsd:strCentrecode>", authBean.groupid, authBean.password, authBean.serviceid, authBean.strCentrecode);
            sbXmlReq.AppendFormat("<xsd:strDistcode>{0}</xsd:strDistcode><xsd:strStaffcode>{1}</xsd:strStaffcode><xsd:strdeptcode>{2}</xsd:strdeptcode><xsd:userId>{3}</xsd:userId></gat:RequestBean>", authBean.strDistcode, authBean.strStaffcode, authBean.strdeptcode, authBean.userId);
            sbXmlReq.Append("</gat:ActBillPayment></soapenv:Body></soapenv:Envelope>");

            #endregion Request

            var reqStartTime = DateTime.Now.Ticks;
            var webResponse = WebRequestProcess.DoHttpPost(ConfigurationManager.AppSettings["ESEVA_MOB_SERVICE_URL"], sbXmlReq.ToString(), action);
            var reqEndTime = DateTime.Now.Ticks;

            var response = new PaymentResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };

            try
            {
                var queuePath = ConfigurationManager.AppSettings["MSMQ_QUEUE_URL"].ToString();
                var queueLabel = ConfigurationManager.AppSettings["MSMQ_ESEVA_LABEL"].ToString();
                var queueStr = "<xml><RefId>{0}</RefId><MethodName>{1}</MethodName><DeptName>{2}</DeptName><ServiceCode>{3}</ServiceCode><Request>{4}</Request><Response>{5}</Response><TimeTaken>{6}</TimeTaken><Reqtime>{7}</Reqtime><Restime>{8}</Restime></xml>";
                var queueLog = string.Format(queueStr, data.BillRefNo, action, "ESEVA", authBean.strdeptcode, sbXmlReq.ToString(), webResponse, ((reqEndTime - reqStartTime) / 10000), reqStartTime, reqEndTime);
                MSMQPush.MessagePush(queuePath, queueLabel, queueLog);

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webResponse);

                var returnNode = xmlDoc.DocumentElement.GetElementsByTagName("ns:return");

                var ax21Manager = new XmlNamespaceManager(xmlDoc.NameTable);
                ax21Manager.AddNamespace("ax21", "http://auth.gateway.mobile.eseva.com/xsd");

                if (returnNode == null || returnNode.Count <= 0)
                    return response;

                var resCode = returnNode[0].SelectSingleNode("ax21:strResCode", ax21Manager).InnerText;
                if (resCode == "000")
                {
                    response = new MobilePaymentResponse
                    {
                        ResCode = resCode,
                        ResDesc = returnNode[0].SelectSingleNode("ax21:strResDesc", ax21Manager).InnerText,
                        RegId = data.tid,
                        RefNo = data.BillRefNo,
                        Receipt = returnNode[0].SelectSingleNode("ax21:strReceipt", ax21Manager).InnerText
                    };
                }
                else
                {
                    response.ResCode = resCode;
                    response.ResDesc = !string.IsNullOrEmpty(CacheConfigXml.GetErrorDescription(resCode)) ? CacheConfigXml.GetErrorDescription(resCode) : returnNode[0].SelectSingleNode("ax21:strResDesc", ax21Manager).InnerText;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess :: ActBillPayment => Exception: {0}", ex.Message));
            }

            return response;
        }

        private PaymentResponse RTAFeeBillPayment(MobilePaymentRequest data, string action)
        {
            #region Request

            var sbXmlReq = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:gat=\"http://gateway.mobile.eseva.com\" xmlns:xsd=\"http://auth.gateway.mobile.eseva.com/xsd\">");
            sbXmlReq.Append("<soapenv:Header/><soapenv:Body><gat:IdeaBillPayment>");
            sbXmlReq.Append("<gat:PayReq>");
            sbXmlReq.AppendFormat("<xsd:strEsevaReqid>{0}</xsd:strEsevaReqid>", data.BillRefNo);
            sbXmlReq.AppendFormat("<xsd:strPaidAmt>{0}</xsd:strPaidAmt>", data.Amount);
            sbXmlReq.AppendFormat("<xsd:strMobileno>{0}</xsd:strMobileno>", data.Number);
            sbXmlReq.AppendFormat("<xsd:strVendorRefno>{0}</xsd:strVendorRefno>", data.tid);
            sbXmlReq.Append("</gat:PayReq>");
            sbXmlReq.Append("<gat:RequestBean>");
            sbXmlReq.AppendFormat("<xsd:groupid>{0}</xsd:groupid>", authBean.groupid);
            sbXmlReq.AppendFormat("<xsd:password>{0}</xsd:password>", authBean.password);
            sbXmlReq.AppendFormat("<xsd:serviceid>{0}</xsd:serviceid>", authBean.serviceid);
            sbXmlReq.AppendFormat("<xsd:strCentrecode>{0}</xsd:strCentrecode>", authBean.strCentrecode);
            sbXmlReq.AppendFormat("<xsd:strDistcode>{0}</xsd:strDistcode>", authBean.strDistcode);
            sbXmlReq.AppendFormat("<xsd:strStaffcode>{0}</xsd:strStaffcode>", authBean.strStaffcode);
            sbXmlReq.AppendFormat("<xsd:strdeptcode>{0}</xsd:strdeptcode>", authBean.strdeptcode);
            sbXmlReq.AppendFormat("<xsd:userId>{0}</xsd:userId>", authBean.userId);
            sbXmlReq.Append("</gat:RequestBean>");
            sbXmlReq.Append("</gat:IdeaBillPayment></soapenv:Body></soapenv:Envelope>");

            #endregion Request

            var reqStartTime = DateTime.Now.Ticks;
            var webResponse = WebRequestProcess.DoHttpPost(ConfigurationManager.AppSettings["ESEVA_MOB_SERVICE_URL"], sbXmlReq.ToString(), action);
            var reqEndTime = DateTime.Now.Ticks;

            var response = new PaymentResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };

            try
            {
                var queuePath = ConfigurationManager.AppSettings["MSMQ_QUEUE_URL"].ToString();
                var queueLabel = ConfigurationManager.AppSettings["MSMQ_ESEVA_LABEL"].ToString();
                var queueStr = "<xml><RefId>{0}</RefId><MethodName>{1}</MethodName><DeptName>{2}</DeptName><ServiceCode>{3}</ServiceCode><Request>{4}</Request><Response>{5}</Response><TimeTaken>{6}</TimeTaken><Reqtime>{7}</Reqtime><Restime>{8}</Restime></xml>";
                var queueLog = string.Format(queueStr, data.BillRefNo, action, "ESEVA", authBean.strdeptcode, sbXmlReq.ToString(), webResponse, ((reqEndTime - reqStartTime) / 10000), reqStartTime, reqEndTime);
                MSMQPush.MessagePush(queuePath, queueLabel, queueLog);

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webResponse);

                var returnNode = xmlDoc.DocumentElement.GetElementsByTagName("ns:return");

                var ax213Manager = new XmlNamespaceManager(xmlDoc.NameTable);
                ax213Manager.AddNamespace("ax213", "http://rtaFeePayment.gateway.mobile.eseva.com/xsd");

                if (returnNode == null || returnNode.Count <= 0)
                    return response;

                var resCode = returnNode[0].SelectSingleNode("ax213:strResCode", ax213Manager).InnerText;
                if (resCode == "000")
                {
                    response = new MobilePaymentResponse
                    {
                        ResCode = resCode,
                        ResDesc = returnNode[0].SelectSingleNode("ax213:strResDesc", ax213Manager).InnerText,
                        RegId = data.tid,
                        RefNo = data.BillRefNo,
                        Receipt = returnNode[0].SelectSingleNode("ax213:strReceipt", ax213Manager).InnerText
                    };
                }
                else
                {
                    response.ResCode = resCode;
                    response.ResDesc = returnNode[0].SelectSingleNode("ax213:strResDesc", ax213Manager).InnerText;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess :: RTAFeeBillPayment => Exception: {0}", ex.Message));
            }

            return response;
        }

        private PaymentResponse TTLBillPayment(MobilePaymentRequest data, string action)
        {
            #region Request

            var sbXmlReq = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:gat=\"http://gateway.mobile.eseva.com\" xmlns:xsd=\"http://auth.gateway.mobile.eseva.com/xsd\">");
            sbXmlReq.Append("<soapenv:Header/><soapenv:Body><gat:TTLBillPayment><gat:PayReq>");
            sbXmlReq.AppendFormat("<xsd:strEsevaReqid>{0}</xsd:strEsevaReqid><xsd:strMobileno>{1}</xsd:strMobileno><xsd:strPaidAmt>{2}</xsd:strPaidAmt><xsd:strVendorRefno>{3}</xsd:strVendorRefno>", data.BillRefNo, data.Number, data.Amount, data.tid);
            sbXmlReq.Append("</gat:PayReq><gat:RequestBean>");
            sbXmlReq.AppendFormat("<xsd:groupid>{0}</xsd:groupid><xsd:password>{1}</xsd:password><xsd:serviceid>{2}</xsd:serviceid><xsd:strCentrecode>{3}</xsd:strCentrecode>", authBean.groupid, authBean.password, authBean.serviceid, authBean.strCentrecode);
            sbXmlReq.AppendFormat("<xsd:strDistcode>{0}</xsd:strDistcode><xsd:strStaffcode>{1}</xsd:strStaffcode><xsd:strdeptcode>{2}</xsd:strdeptcode><xsd:userId>{3}</xsd:userId>", authBean.strDistcode, authBean.strStaffcode, authBean.strdeptcode, authBean.userId);
            sbXmlReq.Append("</gat:RequestBean></gat:TTLBillPayment></soapenv:Body></soapenv:Envelope>");

            #endregion Request

            var reqStartTime = DateTime.Now.Ticks;
            var webResponse = WebRequestProcess.DoHttpPost(ConfigurationManager.AppSettings["ESEVA_MOB_SERVICE_URL"], sbXmlReq.ToString(), action);
            var reqEndTime = DateTime.Now.Ticks;

            var response = new PaymentResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };

            try
            {
                var queuePath = ConfigurationManager.AppSettings["MSMQ_QUEUE_URL"].ToString();
                var queueLabel = ConfigurationManager.AppSettings["MSMQ_ESEVA_LABEL"].ToString();
                var queueStr = "<xml><RefId>{0}</RefId><MethodName>{1}</MethodName><DeptName>{2}</DeptName><ServiceCode>{3}</ServiceCode><Request>{4}</Request><Response>{5}</Response><TimeTaken>{6}</TimeTaken><Reqtime>{7}</Reqtime><Restime>{8}</Restime></xml>";
                var queueLog = string.Format(queueStr, data.BillRefNo, action, "ESEVA", authBean.strdeptcode, sbXmlReq.ToString(), webResponse, ((reqEndTime - reqStartTime) / 10000), reqStartTime, reqEndTime);
                MSMQPush.MessagePush(queuePath, queueLabel, queueLog);

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webResponse);

                var returnNode = xmlDoc.DocumentElement.GetElementsByTagName("ns:return");

                var ax21Manager = new XmlNamespaceManager(xmlDoc.NameTable);
                ax21Manager.AddNamespace("ax21", "http://auth.gateway.mobile.eseva.com/xsd");

                if (returnNode == null || returnNode.Count <= 0)
                    return response;

                var resCode = returnNode[0].SelectSingleNode("ax21:strResCode", ax21Manager).InnerText;
                if (resCode == "000")
                {
                    response = new MobilePaymentResponse
                    {
                        ResCode = resCode,
                        ResDesc = returnNode[0].SelectSingleNode("ax21:strResDesc", ax21Manager).InnerText,
                        RegId = data.tid,
                        RefNo = data.BillRefNo,
                        Receipt = returnNode[0].SelectSingleNode("ax21:strReceipt", ax21Manager).InnerText
                    };
                }
                else
                {
                    response.ResCode = resCode;
                    response.ResDesc = !string.IsNullOrEmpty(CacheConfigXml.GetErrorDescription(resCode)) ? CacheConfigXml.GetErrorDescription(resCode) : returnNode[0].SelectSingleNode("ax21:strResDesc", ax21Manager).InnerText;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess :: TTLBillPayment => Exception: {0}", ex.Message));
            }

            return response;
        }

        private PaymentResponse CPDCLBillPayment(MobilePaymentRequest data, string action)
        {
            #region Request

            var sbXmlReq = new StringBuilder("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:gat=\"http://gateway.mobile.eseva.com\" xmlns:xsd=\"http://auth.gateway.mobile.eseva.com/xsd\">");
            sbXmlReq.Append("<soapenv:Header/><soapenv:Body><gat:CpdclBillPayment><gat:PayReq>");
            sbXmlReq.AppendFormat("<xsd:strEsevaReqid>{0}</xsd:strEsevaReqid><xsd:strMobileno>{1}</xsd:strMobileno><xsd:strPaidAmt>{2}</xsd:strPaidAmt><xsd:strVendorRefno>{3}</xsd:strVendorRefno>", data.BillRefNo, data.Number, data.Amount, data.tid);
            sbXmlReq.Append("</gat:PayReq><gat:RequestBean>");
            sbXmlReq.AppendFormat("<xsd:groupid>{0}</xsd:groupid><xsd:password>{1}</xsd:password><xsd:serviceid>{2}</xsd:serviceid><xsd:strCentrecode>{3}</xsd:strCentrecode>", authBean.groupid, authBean.password, authBean.serviceid, authBean.strCentrecode);
            sbXmlReq.AppendFormat("<xsd:strDistcode>{0}</xsd:strDistcode><xsd:strStaffcode>{1}</xsd:strStaffcode><xsd:strdeptcode>{2}</xsd:strdeptcode><xsd:userId>{3}</xsd:userId>", authBean.strDistcode, authBean.strStaffcode, authBean.strdeptcode, authBean.userId);
            sbXmlReq.Append("</gat:RequestBean></gat:CpdclBillPayment></soapenv:Body></soapenv:Envelope>");

            #endregion Request

            var reqStartTime = DateTime.Now.Ticks;
            var webResponse = WebRequestProcess.DoHttpPost(ConfigurationManager.AppSettings["ESEVA_MOB_SERVICE_URL"], sbXmlReq.ToString(), action);
            var reqEndTime = DateTime.Now.Ticks;

            var response = new PaymentResponse { ResCode = "300", ResDesc = CacheConfigXml.GetErrorDescription("300") };

            try
            {
                var queuePath = ConfigurationManager.AppSettings["MSMQ_QUEUE_URL"].ToString();
                var queueLabel = ConfigurationManager.AppSettings["MSMQ_ESEVA_LABEL"].ToString();
                var queueStr = "<xml><RefId>{0}</RefId><MethodName>{1}</MethodName><DeptName>{2}</DeptName><ServiceCode>{3}</ServiceCode><Request>{4}</Request><Response>{5}</Response><TimeTaken>{6}</TimeTaken><Reqtime>{7}</Reqtime><Restime>{8}</Restime></xml>";
                var queueLog = string.Format(queueStr, data.BillRefNo, action, "ESEVA", authBean.strdeptcode, sbXmlReq.ToString(), webResponse, ((reqEndTime - reqStartTime) / 10000), reqStartTime, reqEndTime);
                MSMQPush.MessagePush(queuePath, queueLabel, queueLog);

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webResponse);

                var returnNode = xmlDoc.DocumentElement.GetElementsByTagName("ns:return");

                var ax29Manager = new XmlNamespaceManager(xmlDoc.NameTable);
                ax29Manager.AddNamespace("ax29", "http://cpdcl.gateway.mobile.eseva.com/xsd");

                if (returnNode == null || returnNode.Count <= 0)
                    return response;

                var resCode = returnNode[0].SelectSingleNode("ax29:strResCode", ax29Manager).InnerText;
                if (resCode == "000")
                {
                    response = new MobilePaymentResponse
                    {
                        ResCode = resCode,
                        ResDesc = returnNode[0].SelectSingleNode("ax29:strResDesc", ax29Manager).InnerText,
                        RegId = data.tid,
                        RefNo = data.BillRefNo,
                        Receipt = returnNode[0].SelectSingleNode("ax29:strReceipt", ax29Manager).InnerText
                    };
                }
                else
                {
                    response.ResCode = resCode;
                    response.ResDesc = !string.IsNullOrEmpty(CacheConfigXml.GetErrorDescription(resCode)) ? CacheConfigXml.GetErrorDescription(resCode) : returnNode[0].SelectSingleNode("ax29:strResDesc", ax29Manager).InnerText;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("MSDGAPI", "ESeva-Exception", LogMode.Excep, ex, string.Format("MobileBillRequestProcess :: CPDCLBillPayment => Exception: {0}", ex.Message));
            }

            return response;
        }

        #endregion Private Methods

        #region Destructor

        ~MobileBillRequestProcess()
        {
            authBean = null;
        }

        #endregion Destructor
    }
}
