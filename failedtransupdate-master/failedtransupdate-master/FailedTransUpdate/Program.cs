using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonHelper;
using Newtonsoft.Json;
using IMI.SqlWrapper;

namespace FailedTransUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                String strFromDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");//ToString("yyyy-MM-dd");
                String strToDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");//ToString("yyyy-MM-dd");
                String strURL = General.GetConfigVal("PAYMENT_TRANS_DATA_FETCH_URL");
                if (General.GetConfigVal("ENABLE_CUSTOM_DATE").ToUpper() == "Y" && General.GetConfigVal("CUSTOM_FROM_DATE") != "")
                {
                    strFromDate = Convert.ToDateTime(General.GetConfigVal("CUSTOM_FROM_DATE")).ToString("yyyy-MM-dd");
                    strToDate = Convert.ToDateTime(General.GetConfigVal("CUSTOM_TO_DATE")).ToString("yyyy-MM-dd");
                }
                strFromDate = strFromDate + " 00:00:00";
                strToDate = strToDate + " 23:59:59";
                Console.WriteLine("From Date:" + strFromDate);
                Console.WriteLine("To Date:" + strToDate);
                strURL = strURL.Replace("!STARTDATE!", strFromDate).Replace("!ENDDATE!", strToDate);
                String strAPiResp = General.DoRequest(strURL, "", "GET", "application/json");
                General.WriteLog("APILOGS", "URL:" + strURL + Environment.NewLine + "Response:" + strAPiResp);
                if (!string.IsNullOrEmpty(strAPiResp) && !strAPiResp.ToUpper().Contains("NO DATA AVAILABLE"))
                {
                    List<PaymentBO> lstBO = JsonConvert.DeserializeObject<List<PaymentBO>>(strAPiResp);
                    if (lstBO != null && lstBO.Count > 0)
                    {
                        foreach (var item in lstBO)
                        {
                            int iRet = UpdatePaymentDetails(item);
                            if (iRet <= 0)
                                General.WriteLog("STATUS_UPDATE_FAIL", "Status update failed for:" + item.transId.ToStr());
                        }
                    }
                }
                else
                {
                    General.WriteLog("FAILED_TRANS_UPDATE", "No data found for date:" + strFromDate + ", and " + strToDate);
                }
            }
            catch (Exception ex)
            {
                General.WriteLog("FAILED_TRANS_UPDATE", "Exception in Main:" + ex.ToStr());
            }
        }
        private static int UpdatePaymentDetails(PaymentBO objBO)
        {
            int retVal = -1;
            try
            {

                using (DBFactory objDB = new DBFactory("DSN_GOT"))
                {
                    objDB.AddInParam("MGOVID", SqlType.VarChar, objBO.transId.ToStr());
                    objDB.AddInParam("TID", SqlType.VarChar, objBO.tid.ToStr());
                    objDB.AddInParam("PGWSTATUS", SqlType.Int, objBO.status.ToInt());
                    objDB.AddOutParam("RETSTATUS", SqlType.Int, 10);
                    objDB.RunProc("UDP_FAILED_PAYMENT_TRANS_UPDATE");
                    retVal = objDB.GetOutValue("RETSTATUS").ToInt();
                }
            }
            catch (Exception ex)
            {
                General.WriteLog("FAILED_TRANS_UPDATE", "Exception in UpdatePaymentDetails:" + ex.Message + ": StackTrace " + ex.StackTrace);
            }
            return retVal;
        }
    }
    public class PaymentBO
    {
        public string datetime { get; set; }
        public string transId { get; set; }
        public string tid { get; set; }
        public string status { get; set; }
    }
}
