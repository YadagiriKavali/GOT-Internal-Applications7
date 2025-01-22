using System;
using System.Data;
using System.Linq;
using IMI.Logger;
using Newtonsoft.Json;
using User.Cryptography;
using User.DB;

namespace User.BL
{
    public class LoginBL
    {
        #region Public Methods

        /// <summary>
        /// To update the password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="opassword"></param>
        /// <param name="ouser"></param>
        /// <returns></returns>
        public UResponse UpdatePassword(string password, string opassword, UserDetail ouser)
        {
            try
            {
                if (password.Contains('<') || password.Contains('>'))
                    return new UResponse { ResCode = "1", ResDesc = "Password contains invalid characthers" };

                if (new LoginDataAccess().UpdatePassword(ouser.LoginId, password, opassword) == "1")
                    return new UResponse { ResCode = "0", ResDesc = "Success" };
                else
                    return new UResponse { ResCode = "1", ResDesc = "Please enter valid current password" };
            }
            catch (Exception ex)
            {
                LogData.Write("User", "LoginBL", LogMode.Excep, ex, "updatePassword ");
                return new UResponse { ResCode = "1", ResDesc = "We are unable to process your request" };
            }
        }

        /// <summary>
        /// Login functionality
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="ip"></param>
        /// <param name="isPwdExpired"></param>
        /// <param name="errorMessage"></param>
        /// <param name="isAccountLocked"></param>
        /// <returns></returns>
        public UResponse CheckUserDetail(string username, string password, string ip)//, ref bool isPwdExpired, ref string errorMessage, ref bool isAccountLocked)
        {
            //string sResp = string.Empty;
            UResponse response = null;
            string type = string.Empty;

            try
            {
                response = GetUserDetail(username, password, ip, ref type);//, ref isPwdExpired, ref errorMessage, ref type, ref  isAccountLocked);
                if (type != "LOG_UNIQUE_LOGIN")
                {
                    //if (sResp != null)
                    //    new LoginDataAccess().SaveUserRetryCount(username, ip, "RESET");

                    if (type == "LOG_LOGIN_RETRY")
                        new LoginDataAccess().SaveUserRetryCount(username, ip, "COUNT");
                }
            }
            catch (Exception ex)
            {
                LogData.Write("User", "LoginBL", LogMode.Excep, ex, "checkUserDetils ");
                response = new UResponse { ResCode = "1", ResDesc = "We are unable to process your request" };
            }

            return response;
        }

        /// <summary>
        /// To Save the user session data
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="sessionid"></param>
        public bool SaveUserLoginInfo(string userName, string sessionid)
        {
            return new LoginDataAccess().SaveUserLoginInfo(userName, sessionid);
        }

        /// <summary>
        /// To delete the session data
        /// </summary>
        /// <param name="userName"></param>
        public void DeleteUserLoginInfo(string userName)
        {
            try
            {
                new LoginDataAccess().DeleteUserLoginInfo(userName);
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clBLogin", LogMode.Excep, ex, "deletesessiondata ");
            }
        }

        public UResponse GetUserDetail(string userName, string password, string ip, ref string type)//, ref bool isPwdExpired, ref string errorMessage, ref string type, ref bool isAccountLocked)
        {
            UResponse response = null;
            DataTable dtRecords = null;
            DataTable dtLogeduser = null;
            DataTable dtPasswordExp = null;
            int isessiontime = 0;
            int ilockperiod = 0;
            int iRval = 0;
            type = string.Empty;

            try
            {
                var pwdpolicy = FramePasswordPolicy(new LoginDataAccess().GetPwdPolicySettings());
                string appName = clGeneral.GetConfigValue("APP_NAME") == "" ? "GENSERVICE" : clGeneral.GetConfigValue("APP_NAME").ToString();
                dtRecords = new LoginDataAccess().ValidateUser(userName, appName, ref iRval);
                if (dtRecords != null && dtRecords.Rows.Count > 0)
                {
                    string sPwd = new PasswordFormat().GetPassword(password);
                    // Checking User given correct password or not
                    if (dtRecords.Rows[0]["pwd"].ToString() == sPwd)
                    {
                        bool userlogged = false;
                        if (clGeneral.GetConfigValue("CHECK_UNIQUE_LOGIN") == "Y")
                        {
                            // Checking unique user functionality
                            dtLogeduser = new LoginDataAccess().CheckUserLoggedIn(userName);

                            if (dtLogeduser != null && dtLogeduser.Rows.Count > 0)
                            {
                                DateTime reqDatetime = Convert.ToDateTime(dtLogeduser.Rows[0]["REQUESTTIME"]);
                                TimeSpan totalSpan = DateTime.Now.Subtract(reqDatetime);
                                if (totalSpan.TotalMinutes < isessiontime)
                                    userlogged = true;
                            }
                        }

                        if (userlogged)
                        {
                            response = new UResponse { ResCode = "1", ResDesc = "User already logged in" };
                            type = "LOG_UNIQUE_LOGIN";
                        }
                        else
                        {
                            bool userlocked = isUserLocked(userName, pwdpolicy, ip, ref ilockperiod);
                            if (userlocked)
                            {
                                response = new UResponse { ResCode = "1", ResDesc = "User locked, Please try after " + ilockperiod + " minutes" };
                            }
                            else
                            {
                                dtPasswordExp = new LoginDataAccess().GetPwdExprityDetail(userName);
                                //isPwdExpired = dtPasswordExp.Rows.Count > 0 && Convert.ToDateTime(dtPasswordExp.Rows[0]["UpdatedDate"].ToString()).AddDays(pwdpolicy.ExpireInDays) < DateTime.Now;

                                response = new UserDetail
                                {
                                    UserId = int.Parse(dtRecords.Rows[0]["UseriD"].ToString() == "1000" ? "0" : dtRecords.Rows[0]["UseriD"].ToString()),
                                    UserType = dtRecords.Rows[0]["usertype"].ToString(),
                                    LoginId = dtRecords.Rows[0]["LoginId"].ToString(),
                                    Email = dtRecords.Rows[0]["Email"].ToString(),
                                    Pwd = "",
                                    MobileNo = dtRecords.Rows[0]["MobileNo"].ToString(),
                                    CreatedOn = dtRecords.Rows[0]["CreatedOn"].ToString() != "" ? Convert.ToDateTime(dtRecords.Rows[0]["CreatedOn"].ToString()).ToString("dd/MMM/yyyy hh:mm:ss tt") : "",
                                    UpdatedOn = dtRecords.Rows[0]["UpdatedOn"].ToString() != "" ? Convert.ToDateTime(dtRecords.Rows[0]["UpdatedOn"].ToString()).ToString("dd/MMM/yyyy hh:mm:ss tt") : "",
                                    Active = dtRecords.Rows[0]["Active"].ToString().Trim(),
                                    RegType = dtRecords.Rows[0]["RegType"].ToString().Trim(),
                                    AppType = dtRecords.Rows[0]["AppType"].ToString().Trim(),
                                    IP = dtRecords.Rows[0]["IP"].ToString().Trim(),
                                    UpdatedIP = dtRecords.Rows[0]["UPDATEDIP"].ToString().Trim(),
                                    PwdPolicy = dtRecords.Rows[0]["PwdPolicy"].ToString().Trim(),
                                    ResCode = "0",
                                    ResDesc = "Success"
                                };
                            }
                        }
                    }
                    else
                    {
                        if (isUserLocked(userName, pwdpolicy, ip, ref ilockperiod))
                        {
                            response = new UResponse { ResCode = "1", ResDesc = "User locked, Please try after " + ilockperiod + " minutes" };
                        }
                        else
                        {
                            response = new UResponse { ResCode = "1", ResDesc = "Invalid Username or Password" };
                            type = "LOG_LOGIN_RETRY";
                        }
                    }
                }
                else if (iRval == 1)//Not Active user
                {
                    response = new UResponse { ResCode = "1", ResDesc = "User is not Active" };
                }
                else if (iRval == 2)//User not present/Invalid credentials.
                {
                    response = new UResponse { ResCode = "1", ResDesc = "Invalid Username or Password" };
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clBLogin", LogMode.Excep, ex, "getUserDetails ");
                response = new UResponse { ResCode = "1", ResDesc = "We are unable to process your request" };
            }

            return response;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// To Check wether user is locked or not
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="ilockperiod"></param>
        /// <param name="objppolicy"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        private bool isUserLocked(string userName, PwdPolicy objppolicy, string ip, ref int ilockperiod)
        {
            bool userlocked = false;
            DataTable dtRetryCount = null;
            try
            {
                int.TryParse(clGeneral.GetConfigValue("LOCK_PERIOD"), out ilockperiod);
                ilockperiod = ilockperiod == 0 ? 20 : ilockperiod;

                // Checking user locked or not
                dtRetryCount = new LoginDataAccess().GetUserRetryCount(userName);
                if (dtRetryCount != null && dtRetryCount.Rows.Count > 0)
                {
                    if (Convert.ToDateTime(dtRetryCount.Rows[0]["RetryDate"].ToString()).ToShortDateString() == DateTime.Now.ToShortDateString())
                    {
                        if (Convert.ToInt32(dtRetryCount.Rows[0]["Count"].ToString()) >= objppolicy.NotInPrevMatches)
                        {
                            DateTime reqDatetime = Convert.ToDateTime(dtRetryCount.Rows[0]["RetryDate"].ToString());
                            TimeSpan totalSpan = DateTime.Now.Subtract(reqDatetime);
                            if (totalSpan.Minutes < ilockperiod)
                            {
                                new LoginDataAccess().SaveUserRetryCount(userName, ip, "RDATE");
                                userlocked = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clBLogin", LogMode.Excep, ex, "isUserLocked ");
            }
            finally
            {
                dtRetryCount = null;
            }

            return userlocked;
        }

        /// <summary>
        /// To frame the password policy object
        /// </summary>
        /// <param name="dtRecords"></param>
        /// <returns></returns>
        private PwdPolicy FramePasswordPolicy(DataTable dtRecords)
        {
            PwdPolicy pwdpolicy = null;


            try
            {
                if (dtRecords == null || dtRecords.Rows.Count <= 0)
                    return pwdpolicy;

                DataRow row = dtRecords.Rows[0];
                int iValue = 0;

                pwdpolicy = new PwdPolicy();

                int.TryParse(row["MinLength"].ToString(), out iValue);
                pwdpolicy.MinLength = iValue == 0 ? 6 : iValue;

                pwdpolicy.GrpCaps = row["grpcaps"].ToString();
                pwdpolicy.GrpSmalls = row["grpsmalls"].ToString();
                pwdpolicy.GrpNums = row["grpnums"].ToString();
                pwdpolicy.GrpSplChars = row["grpsplchars"].ToString();

                int.TryParse(row["notinprevmatches"].ToString(), out iValue);
                pwdpolicy.NotInPrevMatches = iValue == 0 ? 4 : iValue;

                int.TryParse(row["expireindays"].ToString(), out iValue);
                pwdpolicy.ExpireInDays = iValue == 0 ? 120 : iValue;

                int.TryParse(row["alertdays"].ToString(), out iValue);
                pwdpolicy.AlertDays = iValue == 0 ? 120 : iValue;

                pwdpolicy.Active = row["active"].ToString();

                int.TryParse(row["matchcharlen"].ToString(), out iValue);
                pwdpolicy.MatchCharLen = iValue == 0 ? 120 : iValue;
            }
            catch (Exception ex)
            {
                LogData.Write("User", "LoginBL", LogMode.Excep, ex, "FramePasswordPolicy ");
            }

            return pwdpolicy;
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        #endregion Private Methods
    }
}
