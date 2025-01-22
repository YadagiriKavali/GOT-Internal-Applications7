using System;
using System.Collections.Generic;
using System.Data;
using IMI.Logger;
using Newtonsoft.Json;
using User.Cryptography;
using User.DB;
using User.Utilities;
using System.Configuration;
using user.MeesevaServices;

namespace User.BL
{
    public class UserBL
    {
        /// <summary>
        /// To insert or update the user details.
        /// </summary>
        /// <param name="ousers"></param>
        /// <returns></returns>
        public UResponse SaveUserDetail(UserDetail ousers)
        {
            try
            {
                ousers.Pwd = new PasswordFormat().GetPassword(ousers.Pwd);
                int rval = new UsersDataAccess().Saveusers(ousers);
                if (rval == -1)
                    return new UResponse { ResCode = "1", ResDesc = "User already exists" };
                
                if (rval == 0)
                    return new UResponse { ResCode = "1", ResDesc = "We are unable to process your request. Try again later" };

                var mesevaClient = new MeesevaMobileWebserviceSoapClient("MeesevaMobileWebserviceSoap");
                mesevaClient.NewUserRegistration("", "", "", "", "", "", 0, "", "", "", "", ousers.Email, ousers.MobileNo, ousers.Pwd, ousers.IP, "");

                return new UResponse { ResCode = "0", ResDesc = "User Details Saved Successfully" };
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clBUsers", LogMode.Excep, ex, "SaveUserDtls");
                return new UResponse { ResCode = "1", ResDesc = "We are unable to process your request. Try again later" };
            }
        }

        /// <summary>
        /// To Update the user details.
        /// </summary>
        /// <param name="ousers"></param>
        /// <returns></returns>
        public UResponse UserUpdate(UserDetail ousers)
        {
            try
            {
                int rval = new UsersDataAccess().UserUpdate(ousers);
                if (rval == -1)
                    return new UResponse { ResCode = "1", ResDesc = "User does not exists" };

                if (rval == 0)
                    return new UResponse { ResCode = "1", ResDesc = "We are unable to process your request. Try again later" };

                return new UResponse { ResCode = "0", ResDesc = "User Details Updated Successfully" };
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clBUsers", LogMode.Excep, ex, "UserUpdate");
                return new UResponse { ResCode = "1", ResDesc = "We are unable to process your request. Try again later" };
            }
        }

        /// <summary>
        /// TO GET USERSLIST
        /// </summary>
        /// <returns></returns>
        public string GetUserDetail()
        {

            string resp = null;
            DataTable dtuser = null;
            List<UserDetail> userlist = null;
            try
            {
                dtuser = new UsersDataAccess().GetUserDetail();
                if (dtuser.Rows.Count > 0)
                {
                    userlist = new List<UserDetail>();
                    foreach (DataRow dr in dtuser.Rows)
                    {
                        try
                        {
                            string active = string.Empty;
                            userlist.Add(new UserDetail()
                            {
                                UserId = Convert.ToInt32(dr["userid"].ToString()),
                                LoginId = dr["LoginId"].ToString(),
                                Pwd = new PasswordFormat().Decrypt(dr["Pwd"].ToString()),
                                MobileNo = dr["MobileNo"].ToString(),
                                Email = dr["Email"].ToString(),
                                UserType = dr["UserType"].ToString(),
                                CreatedOn = dr["CreatedOn"].ToString() != "" ? Convert.ToDateTime(dr["CreatedOn"].ToString()).ToString("dd/MMM/yyyy hh:mm:ss tt") : "",
                                UpdatedOn = dr["UpdatedOn"].ToString() != "" ? Convert.ToDateTime(dr["UpdatedOn"].ToString()).ToString("dd/MMM/yyyy hh:mm:ss tt") : "",
                                Active = dr["Active"].ToString().Trim(),
                                RegType = dr["RegType"].ToString().Trim(),
                                AppType = dr["AppType"].ToString().Trim(),
                                IP = dr["IP"].ToString().Trim(),
                                UpdatedIP = dr["UPDATEDIP"].ToString().Trim(),
                                PwdPolicy = dr["PwdPolicy"].ToString().Trim()
                            });
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    resp = JsonConvert.SerializeObject(userlist);
                }
                else
                {
                    //resp = clGeneral.getOutputResponse("1", oxml.getUserErrorMsg("NODATA"));
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clBUsers", LogMode.Excep, ex, "getUserDtls()");
            }
            finally
            {
                dtuser = null;
            }
            return resp;
        }

        public List<UserDetail> GetUsersDetail()
        {
            List<UserDetail> userlist = null;
            try
            {
                var dtuser = new UsersDataAccess().GetUserDetail();
                if (dtuser.Rows.Count > 0)
                {
                    userlist = new List<UserDetail>();
                    foreach (DataRow dr in dtuser.Rows)
                    {
                        try
                        {
                            string active = string.Empty;
                            userlist.Add(new UserDetail()
                            {
                                UserId = Convert.ToInt32(dr["userid"].ToString()),
                                LoginId = dr["LoginId"].ToString(),
                                Pwd = new PasswordFormat().Decrypt(dr["Pwd"].ToString()),
                                MobileNo = dr["MobileNo"].ToString(),
                                Email = dr["Email"].ToString(),
                                UserType = dr["UserType"].ToString(),
                                CreatedOn = dr["CreatedOn"].ToString() != "" ? Convert.ToDateTime(dr["CreatedOn"].ToString()).ToString("dd/MMM/yyyy hh:mm:ss tt") : "",
                                UpdatedOn = dr["UpdatedOn"].ToString() != "" ? Convert.ToDateTime(dr["UpdatedOn"].ToString()).ToString("dd/MMM/yyyy hh:mm:ss tt") : "",
                                Active = dr["Active"].ToString().Trim(),
                                RegType = dr["RegType"].ToString().Trim(),
                                AppType = dr["AppType"].ToString().Trim(),
                                IP = dr["IP"].ToString().Trim(),
                                UpdatedIP = dr["UPDATEDIP"].ToString().Trim(),
                                PwdPolicy = dr["PwdPolicy"].ToString().Trim()
                            });
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clBUsers", LogMode.Excep, ex, "GetUsersDetail");
            }

            return userlist;
        }

        public UResponse ChangePassword(PasswordChange pwdChange)
        {
            try
            {
                var dtUser = new UsersDataAccess().getUserDtlsByLoginId(pwdChange.LoginId);
                if (string.IsNullOrEmpty(pwdChange.OldPwd) || string.IsNullOrEmpty(pwdChange.NewPwd) || string.IsNullOrEmpty(pwdChange.ConfirmPwd))
                    return new UResponse { ResCode = "1", ResDesc = "Provide proper values" };

                if (string.Compare(new PasswordFormat().GetPassword(pwdChange.OldPwd), dtUser.Rows[0]["Pwd"].ToString(), false) != 0)
                    return new UResponse { ResCode = "1", ResDesc = "Old password is incorrect" };

                if (string.Compare(pwdChange.NewPwd, pwdChange.ConfirmPwd, false) != 0)
                    return new UResponse { ResCode = "1", ResDesc = "New password and confirm password should be same" };

                pwdChange.NewPwd = new PasswordFormat().GetPassword(pwdChange.NewPwd);
                pwdChange.OldPwd = new PasswordFormat().GetPassword(pwdChange.OldPwd);
                new UsersDataAccess().changePassword(pwdChange);

                return new UResponse { ResCode = "0", ResDesc = "Password changed successfully" };
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clBUsers", LogMode.Excep, ex, "UserUpdate");
                return new UResponse { ResCode = "1", ResDesc = "We are unable to process your request. Try again later" };
            }
        }
    }
}
