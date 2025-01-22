using IMI.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetApplicationStatus
{
    public class cBLprocess
    {   
        public readonly string CONTROL_API_URL = clgeneral.GetConfigVal("CONTROL_API");
        public readonly string LOGIN_ENCRYPT_KEY = clgeneral.GetConfigVal("LOGIN_ENCRYPT_KEY");
        public cBLprocess()
        {

            try
            {
                DataTable dt = new cDLGetstatus().Getcertstatus();
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            try
                            {
                                getstatus objgetstatus = new getstatus()
                                {

                                    tid = DateTime.Now.Ticks.ToString(),
                                    applicationNo = dr["ConsumerNumber"].ToString(),
                                    channel = "SMS",
                                    mobileNo = dr["MobileNo"].ToString()
                                };
                                string requestbody = JsonConvert.SerializeObject(objgetstatus);
                                HttpRequestParamModel objHttpRequest = null;
                                HttpRequestHeaderModel objRequestHeaders = new HttpRequestHeaderModel { Action = "8", SaltKey = objgetstatus.tid, Service = "meseva", Signature = EncryptionProcess.GenerateSignature(objgetstatus.applicationNo, objgetstatus.tid) };
                                objHttpRequest = new HttpRequestParamModel { ContentType = "application/json", PostBody = requestbody, URL = CONTROL_API_URL, httpRequestHeader = objRequestHeaders };
                                objRequestHeaders = null;
                                HttpStatusModel objHttpStatus = new clgeneral().DoHttpWebPost(objHttpRequest);
                                if (objHttpStatus != null)
                                {
                                    if (objHttpStatus.response != "")
                                    {
                                        getstatus_resp objgetstatus_resp = JsonConvert.DeserializeObject<getstatus_resp>(objHttpStatus.response);
                                       if(objgetstatus_resp.resCode =="000") {
                                            //update the data
                                            string status = "";
                                            if (string.Compare(objgetstatus_resp.status.ToUpper(),"NOT VIEWED",true)==0)
                                            {
                                                status = clgeneral.GetConfig("STATUS_NOT_VIEWED");
                                            }
                                            else if(string.Compare(objgetstatus_resp.status.ToUpper(), "APPROVED", true) == 0)
                                            {
                                                status = clgeneral.GetConfig("STATUS_APPROVED");
                                            }
                                            else if (string.Compare(objgetstatus_resp.status.ToUpper(), "REJECTED", true) == 0)
                                            {
                                                status = clgeneral.GetConfig("STATUS_REJECTED");
                                            }
                                            if (!string.IsNullOrEmpty(status))
                                            {
                                                int resp = new cDLGetstatus().updatecertstatus(dr["ConsumerNumber"].ToString(), dr["DeptTransId"].ToString(), status, DateTime.Now);
                                            }
                                        }

                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                                LogData.Write("GetApplicationStatus", "cBLprocess--foreach loop", LogMode.Excep, ex, "Getcertstatus (UDP_GET_CERTIFICATE_STATUS) ");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogData.Write("GetApplicationStatus", "cBLprocess", LogMode.Excep, ex, "Getcertstatus (UDP_GET_CERTIFICATE_STATUS) ");
            }


        }

    }
}
