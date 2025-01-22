using System;
using System.Data;
using IMI.Logger;
using IMI.SqlWrapper;

namespace User
{
    public class clDLogin
    {
        #region GET
        /// <summary>
        /// To Validate the user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public DataTable validateUser (string username,string appname,ref int irval)
        {
            DataTable dtrecords = null;
            try
            {
                using (DBFactory odbfactory = new DBFactory ( "DSN_MSDG" ))
                {
                    odbfactory.AddInParam ( "LOGINID", SqlType.VarChar, username );
                    odbfactory.AddInParam("SOURCE", SqlType.VarChar, appname);
                    odbfactory.AddOutParam("RVAL", SqlType.Int, 4);
                    odbfactory.RunProc("UDP_VALIDATE_USER", out dtrecords);
                    int.TryParse(odbfactory.GetOutValue("RVAL").ToString(),out irval);
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clDLogin", LogMode.Excep, ex, "validateUser (UDP_VALIDATE_USER) ");
            }
            finally
            {

            }
            return dtrecords;
        }

        /// <summary>
        /// To Get the password policy settings
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public DataTable getpwdpolicysettings ()
        {
            DataTable dtrecords = null;
            try
            {
                using (DBFactory odbfactory = new DBFactory ( "DSN_MSDG" ))
                {
                    odbfactory.RunProc("UDP_USER_GET_PWD_POLICY", out dtrecords);
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib","clDLogin", LogMode.Excep, ex, "getpwdpolicysettings (UDP_USER_GET_PWD_POLICY) ");
            }
            finally
            {

            }
            return dtrecords;
        }


        /// <summary>
        /// To Check is is already logged in or not
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public DataTable checkuserloggedin (string username)
        {
            DataTable dtrecords = null;
            try
            {
                using (DBFactory odbfactory = new DBFactory ( "DSN_MSDG" ))
                {
                    odbfactory.AddInParam ( "LOGINID", SqlType.VarChar, username );
                    odbfactory.RunProc ( "UDP_EAPPS_GET_USER_LOGININFO", out dtrecords );
                }
            }
            catch (Exception ex)
            {
                LogData.Write ( "UserLib","clDLogin", LogMode.Excep, ex, "validateUser (UDP_EAPPS_GET_USER_LOGININFO) " );
            }
            finally
            {

            }
            return dtrecords;
        }

        /// <summary>
        /// To get the user retry count
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public DataTable getuserretrycount (string username)
        {
            DataTable dtrecords = null;
            try
            {
                using (DBFactory odbfactory = new DBFactory ( "DSN_MSDG" ))
                {
                    odbfactory.AddInParam ( "LOGINID", SqlType.VarChar, username );
                    odbfactory.RunProc("UDP_USER_GET_LOGIN_RETRY", out dtrecords);
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clDLogin", LogMode.Excep, ex, "getuserretrycount (UDP_USER_GET_LOGIN_RETRY) ");
            }
            finally
            {

            }
            return dtrecords;
        }


        /// <summary>
        /// To get the user retry count
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public DataTable getpasswordexpritydetails (string username)
        {
            DataTable dtrecords = null;
            try
            {
                using (DBFactory odbfactory = new DBFactory ( "DSN_MSDG" ))
                {
                    odbfactory.AddInParam ( "LOGINID", SqlType.VarChar, username );
                    odbfactory.RunProc("UDP_USER_GET_PWD_EXPIRY", out dtrecords);
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clDLogin", LogMode.Excep, ex, "getpasswordexpritydetails (UDP_USER_GET_PWD_EXPIRY) ");
            }
            finally
            {

            }
            return dtrecords;
        }

        /// <summary>
        /// To get the user retry count
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public void saveuserretrycount (string username, string ip, string type)
        {
            try
            {
                using (DBFactory odbfactory = new DBFactory ( "DSN_MSDG" ))
                {
                    odbfactory.AddInParam ( "LOGINID", SqlType.VarChar, username );
                    odbfactory.AddInParam ( "IP", SqlType.VarChar, ip );
                    odbfactory.AddInParam ( "RETRYDATE", SqlType.DateTime, DateTime.Now );
                    odbfactory.AddInParam ( "TYPE", SqlType.VarChar, type );
                    odbfactory.RunProc("UDP_USER_SAVE_LOGIN_RETRY");
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clDLogin", LogMode.Excep, ex, "getuserretrycount (UDP_USER_SAVE_LOGIN_RETRY) ");
            }
            finally
            {

            }
        }

        /// <summary>
        /// To get the user retry count
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public void saveuserlogininfo (string username, string sessionid)
        {
            try
            {
                using (DBFactory odbfactory = new DBFactory ( "DSN_MSDG" ))
                {
                    odbfactory.AddInParam ( "LOGINID", SqlType.VarChar, username );
                    odbfactory.AddInParam ( "REQUESTTIME", SqlType.DateTime, DateTime.Now );
                    odbfactory.AddInParam ( "SESSIONID", SqlType.VarChar, sessionid );
                    odbfactory.RunProc("UDP_USER_SAVE_INFO");
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clDLogin", LogMode.Excep, ex, "saveuserlogininfo (UDP_USER_SAVE_INFO) ");
            }
            finally
            {

            }
        }

        /// <summary>
        /// To get the user retry count
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public void deleteuserlogininfo (string username)
        {
            try
            {
                using (DBFactory odbfactory = new DBFactory ( "DSN_MSDG" ))
                {
                    odbfactory.AddInParam ( "LOGINID", SqlType.VarChar, username );
                    odbfactory.RunProc ( "UDP_EAPPS_DELETE_USER_INFO" );
                }
            }
            catch (Exception ex)
            {
                LogData.Write ( "UserLib","clDLogin", LogMode.Excep, ex, "deleteuserlogininfo (UDP_EAPPS_DELETE_USER_INFO) " );
            }
            finally
            {

            }
        }


        /// <summary>
        /// To update the password
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string updatePassword (string username, string password, string opassword)
        {
            string rval = "0";
            try
            {
                using (DBFactory odbfactory = new DBFactory ( "DSN_MSDG" ))
                {
                    odbfactory.AddInParam ( "LOGINID", SqlType.VarChar, username );
                    odbfactory.AddInParam ( "PASSWORD", SqlType.VarChar, password );
                    odbfactory.AddInParam ( "OPASSWORD", SqlType.VarChar, opassword );
                    odbfactory.AddOutParam ( "RVAL", SqlType.Int, 4 );
                    odbfactory.RunProc ( "UDP_EAPPS_UPD_PWD" );
                    rval = odbfactory.GetOutValue ( "RVAL" ).ToString ();
                }
            }
            catch (Exception ex)
            {
                LogData.Write ( "UserLib","clDLogin", LogMode.Excep, ex, "saveuserlogininfo (UDP_EAPPS_UPD_PWD) " );
            }
            finally
            {

            }
            return rval;
        }


        #endregion
    }
}
