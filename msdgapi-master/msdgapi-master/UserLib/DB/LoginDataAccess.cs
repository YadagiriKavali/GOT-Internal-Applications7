using System;
using System.Data;
using IMI.Logger;
using IMI.SqlWrapper;

namespace User.DB
{
    public class LoginDataAccess
    {
        /// <summary>
        /// To Validate the user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public DataTable ValidateUser(string userName, string appName, ref int irval)
        {
            DataTable dtrecords = null;
            try
            {
                using (DBFactory odbfactory = new DBFactory("DSN_MSDG"))
                {
                    odbfactory.AddInParam("LOGINID", SqlType.VarChar, userName);
                    odbfactory.AddInParam("SOURCE", SqlType.VarChar, appName);
                    odbfactory.AddOutParam("RVAL", SqlType.Int, 4);
                    odbfactory.RunProc("UDP_VALIDATE_USER", out dtrecords);
                    int.TryParse(odbfactory.GetOutValue("RVAL").ToString(), out irval);
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
        public DataTable GetPwdPolicySettings()
        {
            DataTable result = null;
            try
            {
                using (DBFactory odbfactory = new DBFactory("DSN_MSDG"))
                    odbfactory.RunProc("UDP_USER_GET_PWD_POLICY", out result);
            }
            catch (Exception ex)
            {
                LogData.Write("User", "LoginDataAccess", LogMode.Excep, ex, "GetPwdPolicySettings (UDP_USER_GET_PWD_POLICY)");
            }

            return result;
        }

        /// <summary>
        /// To Check is is already logged in or not
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public DataTable CheckUserLoggedIn(string userName)
        {
            DataTable dtrecords = null;
            try
            {
                using (DBFactory odbfactory = new DBFactory("DSN_MSDG"))
                {
                    odbfactory.AddInParam("LOGINID", SqlType.VarChar, userName);
                    odbfactory.RunProc("UDP_EAPPS_GET_USER_LOGININFO", out dtrecords);
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clDLogin", LogMode.Excep, ex, "validateUser (UDP_EAPPS_GET_USER_LOGININFO) ");
            }
            finally
            {

            }
            return dtrecords;
        }

        /// <summary>
        /// To get the user retry count
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public DataTable GetUserRetryCount(string userName)
        {
            DataTable dtrecords = null;
            try
            {
                using (DBFactory odbfactory = new DBFactory("DSN_MSDG"))
                {
                    odbfactory.AddInParam("LOGINID", SqlType.VarChar, userName);
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
        /// <param name="userName"></param>
        /// <returns></returns>
        public DataTable GetPwdExprityDetail(string userName)
        {
            DataTable dtrecords = null;
            try
            {
                using (DBFactory odbfactory = new DBFactory("DSN_MSDG"))
                {
                    odbfactory.AddInParam("LOGINID", SqlType.VarChar, userName);
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
        /// <param name="userName"></param>
        /// <returns></returns>
        public void SaveUserRetryCount(string userName, string ip, string type)
        {
            try
            {
                using (DBFactory odbfactory = new DBFactory("DSN_MSDG"))
                {
                    odbfactory.AddInParam("LOGINID", SqlType.VarChar, userName);
                    odbfactory.AddInParam("IP", SqlType.VarChar, ip);
                    odbfactory.AddInParam("RETRYDATE", SqlType.DateTime, DateTime.Now);
                    odbfactory.AddInParam("TYPE", SqlType.VarChar, type);
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
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool SaveUserLoginInfo(string userName, string sessionId)
        {
            try
            {
                using (DBFactory odbfactory = new DBFactory("DSN_MSDG"))
                {
                    odbfactory.AddInParam("LOGINID", SqlType.VarChar, userName);
                    odbfactory.AddInParam("REQUESTTIME", SqlType.DateTime, DateTime.Now);
                    odbfactory.AddInParam("SESSIONID", SqlType.VarChar, sessionId);
                    return odbfactory.RunProc("UDP_USER_SAVE_INFO") > 0;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clDLogin", LogMode.Excep, ex, "saveuserlogininfo (UDP_USER_SAVE_INFO) ");
                return false;
            }
        }

        /// <summary>
        /// To get the user retry count
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public void DeleteUserLoginInfo(string userName)
        {
            try
            {
                using (DBFactory odbfactory = new DBFactory("DSN_MSDG"))
                {
                    odbfactory.AddInParam("LOGINID", SqlType.VarChar, userName);
                    odbfactory.RunProc("UDP_EAPPS_DELETE_USER_INFO");
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clDLogin", LogMode.Excep, ex, "deleteuserlogininfo (UDP_EAPPS_DELETE_USER_INFO) ");
            }
            finally
            {

            }
        }

        /// <summary>
        /// To update the password
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string UpdatePassword(string userName, string password, string opassword)
        {
            string rval = "0";
            try
            {
                using (DBFactory odbfactory = new DBFactory("DSN_MSDG"))
                {
                    odbfactory.AddInParam("LOGINID", SqlType.VarChar, userName);
                    odbfactory.AddInParam("PASSWORD", SqlType.VarChar, password);
                    odbfactory.AddInParam("OPASSWORD", SqlType.VarChar, opassword);
                    odbfactory.AddOutParam("RVAL", SqlType.Int, 4);
                    odbfactory.RunProc("UDP_EAPPS_UPD_PWD");
                    rval = odbfactory.GetOutValue("RVAL").ToString();
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clDLogin", LogMode.Excep, ex, "saveuserlogininfo (UDP_EAPPS_UPD_PWD) ");
            }
            finally
            {

            }
            return rval;
        }
    }
}
