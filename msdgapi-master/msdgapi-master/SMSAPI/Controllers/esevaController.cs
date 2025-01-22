using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using SMSAPI.BL;
using Newtonsoft.Json;
namespace SMSAPI.Controllers
{
    public class esevaController : ApiController
    {
        public string service_url = string.Empty;
        
        [HttpGet]
        public HttpResponseMessage WaterBill(string msisdn,string sms,string src, string carrier,string dcs)
        {
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
            //service_url = clsGeneral.GetConfigVal("ESEVA_WATER_SERVICES");
            //clsGeneral objGen = new clsGeneral();
            //string sPayload = string.Empty;
            //var sRes = string.Empty;
            //clsHMWRes objhmwres = new clsHMWRes();
            //
            //string smsText = "";
            //try
            //{

            //    sPayload = "{\"CANNo\":\""+ sms.ToUpper().Replace("HMWSS","").Trim() +"\"}";

            //    if (!string.IsNullOrEmpty(service_url))
            //    {
            //        sRes =  objGen.DoHttpPost(service_url, "", sPayload, "");
            //        objhmwres = JsonConvert.DeserializeObject<clsHMWRes>(sRes);
            //        if (objhmwres.strResCodeField == "000")
            //        {
            //            smsText = "Name:" + objhmwres.strNameField + Environment.NewLine + "CanNO:" + objhmwres.strCanField + Environment.NewLine + "Address:" + objhmwres.strAddressField + Environment.NewLine + "Amount:" + objhmwres.strAmountField;
            //        }
            //        else
            //        {
            //            smsText = objhmwres.strResDescField;
            //        }
                    
                   
            //    }
            //    else
            //    {
            //       //return Request.CreateResponse(HttpStatusCode.NotFound, "service url not configured");
            //    }
                
            //}
            //catch (Exception ex)
            //{
            //    httpResponse = Request.CreateResponse(HttpStatusCode.ExpectationFailed, "Unable to process your request.");
                 
            //}
            //finally
            //{
            //    objGen = null;
            //    httpResponse.Content = new StringContent(smsText, System.Text.Encoding.UTF8, "text/plain");
            //}
            return httpResponse;
        }

        [HttpGet]
        public HttpResponseMessage AirtelBill(string msisdn,string sms,string src,string carrier,string dcs)
        {
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
            //service_url = clsGeneral.GetConfigVal("ESEVA_AIRTEL_SERVICES");
            //clsGeneral objGen = new clsGeneral();
            //string sPayload = string.Empty;
            //var sRes = string.Empty;
            //ClsAirtelRes objAirtelRes = new ClsAirtelRes();
            
            //string smsText = "";
            //try
            //{
                                    
            //    sPayload = "{\"msisdn\":\""+ sms.ToUpper().Replace("BILL","").Trim() +"\"}";

            //    if (!string.IsNullOrEmpty(service_url))
            //    {
            //        sRes = objGen.DoHttpPost(service_url, "", sPayload, "");

            //        if (!string.IsNullOrEmpty(sRes))
            //        {
            //            objAirtelRes = JsonConvert.DeserializeObject<ClsAirtelRes>(sRes);

            //            if (objAirtelRes.strResCodeField == "000")
            //            {
            //                smsText = "Account :" + objAirtelRes.strAccountnoField + Environment.NewLine + "Name:" + objAirtelRes.strConsumernameField + Environment.NewLine + "Bill Amount: " + objAirtelRes.strBillamountField;
            //            }
            //            else
            //            {
            //                smsText = objAirtelRes.strResDescField;
            //            }

            //        }
            //        else
            //        {
            //            smsText = "unable to process your request";
            //        }

            //    }
            //    else
            //    {
            //        //return Request.CreateResponse(HttpStatusCode.NotFound, "service url not configured");
            //    }

            //}
            //catch (Exception ex)
            //{

            //    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Exception ");
            //}
            //finally
            //{
            //    objGen = null;
            //    httpResponse.Content = new StringContent(smsText, System.Text.Encoding.UTF8, "text/plain");
            //}
            return httpResponse;

        }

        [HttpGet]
        public HttpResponseMessage TSGOV(string msisdn, string sms, string src, string carrier, string dcs)
        {
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
            string strMsg = "";
            try
            {
                if (ConfigurationManager.AppSettings[sms.ToUpper().Replace(" ", "_")] != null)
                {
                    strMsg = ConfigurationManager.AppSettings[sms.ToUpper().Replace(" ", "_")].ToString();
                }

            }
            catch
            {

            }
            httpResponse.Content = new StringContent(strMsg, System.Text.Encoding.UTF8, "text/plain");
            return httpResponse;
        }
        
        public string gettest()
        {
            return "test";
        }
    }
}
