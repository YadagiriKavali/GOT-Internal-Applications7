using System;
using System.Data;
using IMI.Logger;
using IMI.SqlWrapper;

namespace User
{
    public class clDUsers
    {
        public readonly string DSN_CONN = "DSN_MSDG";

        /// <summary>
        /// To insert or update the user details.
        /// </summary>
        /// <param name="objuser"></param>
        /// <returns></returns>
        public int Saveusers(UserDetail objuser)
        {
            int irval = 0;
            try
            {
                using (DBFactory odbfactory = new DBFactory(DSN_CONN))
                {
                    odbfactory.AddInParam("MOBILENO", SqlType.VarChar, objuser.MobileNo);
                    odbfactory.AddInParam("LOGINID", SqlType.VarChar, objuser.LoginId);
                    odbfactory.AddInParam("PWD", SqlType.VarChar, objuser.Pwd);
                    odbfactory.AddInParam("USERTYPE", SqlType.VarChar, objuser.UserType);
                    odbfactory.AddInParam("EMAIL", SqlType.VarChar, objuser.Email);
                    odbfactory.AddInParam("ACTIVE", SqlType.VarChar, objuser.Active);
                    odbfactory.AddInParam("CREATEDON", SqlType.VarChar, objuser.CreatedOn);
                    odbfactory.AddInParam("UPDATEDON", SqlType.VarChar, objuser.UpdatedOn);
                    odbfactory.AddInParam("REGTYPE", SqlType.VarChar, objuser.RegType);
                    odbfactory.AddInParam("APPTYPE", SqlType.VarChar, objuser.AppType);
                    odbfactory.AddInParam("IP", SqlType.VarChar, objuser.IP);
                    odbfactory.AddInParam("UPDATEDIP", SqlType.VarChar, objuser.UpdatedIP);
                    odbfactory.AddInParam("PWDPOLICY", SqlType.VarChar, objuser.PwdPolicy);
                    odbfactory.AddOutParam("RVAL", SqlType.Int, 4);
                    odbfactory.RunProc("UDP_INSERT_USER_DTLS");
                    int.TryParse(odbfactory.GetOutValue("RVAL").ToString(), out irval);
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clDUsers", LogMode.Excep, ex, "Saveusers (UDP_INSERT_USER_DTLS) ");
            }

            return irval;
        }

        /// <summary>
        /// To Update the user details.
        /// </summary>
        /// <param name="objuser"></param>
        /// <returns></returns>
        public int UserUpdate(UserDetail objuser)
        {
            int irval = 0;
            try
            {
                using (DBFactory odbfactory = new DBFactory(DSN_CONN))
                {
                    odbfactory.AddInParam("MOBILENO", SqlType.VarChar, objuser.MobileNo);
                    odbfactory.AddInParam("LOGINID", SqlType.VarChar, objuser.LoginId);
                    odbfactory.AddInParam("PWD", SqlType.VarChar, objuser.Pwd);
                    odbfactory.AddInParam("USERTYPE", SqlType.VarChar, objuser.UserType);
                    odbfactory.AddInParam("EMAIL", SqlType.VarChar, objuser.Email);
                    odbfactory.AddInParam("ACTIVE", SqlType.VarChar, objuser.Active);
                    odbfactory.AddInParam("UPDATEDON", SqlType.VarChar, objuser.UpdatedOn);
                    odbfactory.AddInParam("REGTYPE", SqlType.VarChar, objuser.RegType);
                    odbfactory.AddInParam("APPTYPE", SqlType.VarChar, objuser.AppType);
                    odbfactory.AddInParam("IP", SqlType.VarChar, objuser.IP);
                    odbfactory.AddInParam("UPDATEDIP", SqlType.VarChar, objuser.UpdatedIP);
                    odbfactory.AddInParam("PWDPOLICY", SqlType.VarChar, objuser.PwdPolicy);
                    odbfactory.AddOutParam("RVAL", SqlType.Int, 4);
                    odbfactory.RunProc("UDP_UPDATE_USER_DTLS");
                    int.TryParse(odbfactory.GetOutValue("RVAL").ToString(), out irval);
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clDUsers", LogMode.Excep, ex, "UserUpdate (UDP_UPDATE_USER_DTLS) ");
            }

            return irval;
        }

        /// <summary>
        /// TO GET USERSLIST
        /// </summary>
        /// <returns></returns>
        public DataTable getUserDtls()
        {
            DataTable dtservice = null;

            try
            {
                using (DBFactory odbfactory = new DBFactory(DSN_CONN))
                {
                    odbfactory.RunProc("UDP_USER_GET_USERLIST", out dtservice);
                    return dtservice;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clDUsers", LogMode.Excep, ex, "getUserDtls(UDP_USER_GET_USERLIST)");
            }

            return dtservice;
        }

        public DataTable getUserDtlsById(int id)
        {
            DataTable dtservice = null;

            try
            {
                using (DBFactory odbfactory = new DBFactory(DSN_CONN))
                {
                    odbfactory.AddInParam("id", SqlType.Int, id);
                    odbfactory.RunProc("UDP_USER_GET_USERLISTBYID", out dtservice);
                    return dtservice;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clDUsers", LogMode.Excep, ex, "getUserDtlsById(UDP_USER_GET_USERLISTBYID)");
            }

            return dtservice;
        }

        public DataTable getUserDtlsByLoginId(string sLoginid)
        {
            DataTable dtservice = null;

            try
            {
                using (DBFactory odbfactory = new DBFactory(DSN_CONN))
                {
                    odbfactory.AddInParam("LOGINID", SqlType.VarChar, sLoginid);
                    odbfactory.RunProc("UDP_USER_GET_USERLISTBYLOGINID", out dtservice);
                    return dtservice;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clDUsers", LogMode.Excep, ex, "getUserDtlsByLoginId(UDP_USER_GET_USERLISTBYLOGINID)");
            }

            return dtservice;
        }

        public int changePassword(PasswordChange objUserPwd)
        {
            int iResp = 0;
            try
            {
                using (DBFactory odbfactory = new DBFactory(DSN_CONN))
                {
                    odbfactory.AddInParam("LOGINID", SqlType.VarChar, objUserPwd.LoginId);
                    odbfactory.AddInParam("PASSWORD", SqlType.VarChar, objUserPwd.NewPwd);
                    odbfactory.AddInParam("OPASSWORD", SqlType.VarChar, objUserPwd.OldPwd);
                    odbfactory.AddOutParam("RVAL", SqlType.Int, 4);
                    odbfactory.RunProc("UDP_USER_UPD_PWD");
                    int.TryParse(odbfactory.GetOutValue("RVAL").ToString(), out iResp);
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clDUsers", LogMode.Excep, ex, "changePassword(UDP_USER_UPD_PWD)");
                iResp = -1;
            }
            return iResp;
        }
    }
}
