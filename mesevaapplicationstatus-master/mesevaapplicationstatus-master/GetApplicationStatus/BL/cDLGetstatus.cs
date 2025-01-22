using System;
using IMI.Logger;
using IMI.SqlWrapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace GetApplicationStatus
{

    public class cDLGetstatus
    {

        public DataTable Getcertstatus()
        {
            DataTable dt = new DataTable();
            try
            {
                using (DBFactory odbfactory = new DBFactory("DB_GOT"))
                {

                    odbfactory.RunProc("UDP_GET_CERTIFICATE_STATUS", out dt);
                }
                return dt;
            }
            catch (Exception ex)
            {
                LogData.Write("GetApplicationStatus", "cDLGetstatus", LogMode.Excep, ex, "Getcertstatus (UDP_GET_CERTIFICATE_STATUS) ");
            }
            finally
            {


            }
            return dt;

        }


        public int updatecertstatus(string APPLICATIONNO,string Depttransid,string CERT_STATUS,DateTime CERT_DATE)
        {
            int resp=0;
            try
            {
                using (DBFactory odbfactory = new DBFactory("DB_GOT"))
                {
                    odbfactory.AddInParam("APPLICATIONNO", SqlType.VarChar, APPLICATIONNO);
                    odbfactory.AddInParam("Depttransid", SqlType.VarChar, Depttransid);
                    odbfactory.AddInParam("CERT_STATUS", SqlType.VarChar,CERT_STATUS);
                    odbfactory.AddInParam("CERT_DATE", SqlType.DateTime, CERT_DATE);
                    odbfactory.AddOutParam("RETSTATUS", SqlType.VarChar,4);
                    odbfactory.RunProc("UDP_UPDATE_CERT_STATUS");
                    int.TryParse(odbfactory.GetOutValue("RETSTATUS").ToString(), out resp);
                }
                return resp;
            }
            catch (Exception ex)
            {
                LogData.Write("GetApplicationStatus", "cDLGetstatus", LogMode.Excep, ex, "Getcertstatus (UDP_GET_CERTIFICATE_STATUS) ");
            }
            finally
            {


            }
            return resp;

        }

    }
}
