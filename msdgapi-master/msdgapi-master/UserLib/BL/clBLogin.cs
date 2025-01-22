using System;
using System.Data;
using System.Linq;
using IMI.Logger;
using Newtonsoft.Json;
using User.Cryptography;

namespace User
{
    public class clBLogin
    {
        #region GET

        /// <summary>
        /// To update the password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="opassword"></param>
        /// <param name="ouser"></param>
        /// <returns></returns>
        public string updatePassword(string password, string opassword, UserDetail ouser)
        {
            string response = string.Empty;
            Error error = null;
            try
            {
                if (!password.Contains('<'))
                {
                    if (!password.Contains('>'))
                    {
                        if (new clDLogin().updatePassword(ouser.LoginId, password, opassword) == "1")
                            error = clGeneral.getFailedmsg("0", "Success");
                        else
                            error = clGeneral.getFailedmsg("1", "Please enter valid current password");
                    }
                    else
                        error = clGeneral.getFailedmsg("1", "Password contains invalid characthers");
                }
                else
                    error = clGeneral.getFailedmsg("1", "Password contains invalid characthers");

            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clBLogin", LogMode.Excep, ex, "updatePassword ");
            }
            finally
            {
                if (error == null)
                    error = clGeneral.getFailedmsg("1", "We are unable to process your request");

                response = JsonConvert.SerializeObject(error);
            }
            return response;
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        /// Login functionality
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="ip"></param>
        /// <param name="ispasswordexpired"></param>
        /// <param name="errormessage"></param>
        /// <param name="isaccountlocked"></param>
        /// <returns></returns>
        public UResponse checkUserDetils(string username, string password, string ip, ref bool ispasswordexpired, ref string errormessage, ref bool isaccountlocked)
        {
            string sResp = string.Empty;
            UserDetail objUsers = null;
            UResponse objResp = null;
            string type = string.Empty;
            try
            {
                objResp = getUserDetails(username, password, ip, ref ispasswordexpired, ref errormessage, ref type, ref  isaccountlocked);
                if (type != "LOG_UNIQUE_LOGIN")
                {
                    if (sResp != null)
                    {
                        new clDLogin().saveuserretrycount(username, ip, "RESET");
                    }
                    if (type == "LOG_LOGIN_RETRY")
                    {
                        new clDLogin().saveuserretrycount(username, ip, "COUNT");
                    }
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clBLogin", LogMode.Excep, ex, "checkUserDetils ");
            }
            return objResp;
        }

        /// <summary>
        /// To Save the user session data
        /// </summary>
        /// <param name="username"></param>
        /// <param name="sessionid"></param>
        public void saveuserlogininfo(string username, string sessionid)
        {
            try
            {
                new clDLogin().saveuserlogininfo(username, sessionid);
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clBLogin", LogMode.Excep, ex, "logsessiondata ");
            }
        }

        /// <summary>
        /// To delete the session data
        /// </summary>
        /// <param name="username"></param>
        public void deleteuserlogininfo(string username)
        {
            try
            {
                new clDLogin().deleteuserlogininfo(username);
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clBLogin", LogMode.Excep, ex, "deletesessiondata ");
            }
        }

        private UResponse getUserDetails(string username, string password, string ip, ref bool ispasswordexpired, ref string errormessage, ref string type, ref bool isaccountlocked)
        {
            UResponse objResp = null;
            DataTable dtRecords = null;
            DataTable dtLogeduser = null;
            PwdPolicy objppolicy = null;
            DataTable dtPasswordExp = null;
            int isessiontime = 0;
            int ilockperiod = 0;
            int iRval = 0;
            string sResponse = string.Empty;
            try
            {
                objppolicy = framePasswordpolicy(new clDLogin().getpwdpolicysettings());
                string sAppName = clGeneral.GetConfigValue("APP_NAME") == "" ? "GENSERVICE" : clGeneral.GetConfigValue("APP_NAME").ToString();
                dtRecords = new clDLogin().validateUser(username, sAppName, ref iRval);
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
                            dtLogeduser = new clDLogin().checkuserloggedin(username);

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
                            objResp = clGeneral.getOutputObjResponse("1", "User already logged in");
                            //errormessage = "User is already logged in";
                            type = "LOG_UNIQUE_LOGIN";
                        }
                        else
                        {
                            bool userlocked = false;

                            userlocked = isUserLocked(username, ref ilockperiod, objppolicy, ip);

                            if (userlocked)
                            {
                                objResp = clGeneral.getOutputObjResponse("1", "User locked, Please try after " + ilockperiod + " minutes");
                                //errormessage = "User locked, Please try after " + ilockperiod + "minutes";
                                type = "";
                                isaccountlocked = true;
                            }
                            else
                            {
                                dtPasswordExp = new clDLogin().getpasswordexpritydetails(username);
                                if (dtPasswordExp.Rows.Count > 0)
                                {
                                    if (Convert.ToDateTime(dtPasswordExp.Rows[0]["UpdatedDate"].ToString()).AddDays(objppolicy.ExpireInDays) < DateTime.Now)
                                    {
                                        ispasswordexpired = true;
                                    }
                                }

                                objResp = new UserDetail
                                {
                                    UserId = int.Parse(dtRecords.Rows[0]["UseriD"].ToString() == "1000" ? "0" : dtRecords.Rows[0]["UseriD"].ToString()),
                                    UserType = dtRecords.Rows[0]["usertype"].ToString(), //dtRecords.Rows[0]["displayusertype"].ToString(),
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
                        if (isUserLocked(username, ref ilockperiod, objppolicy, ip))
                        {
                            objResp = clGeneral.getOutputObjResponse("1", "User locked, Please try after " + ilockperiod + " minutes");
                            //errormessage = "User locked, Please try after " + ilockperiod + "minutes";
                            type = "";
                            isaccountlocked = true;
                        }
                        else
                        {
                            objResp = clGeneral.getOutputObjResponse("1", "Invalid Username or Password");
                            //errormessage = "Invalid Username or Password";
                            type = "LOG_LOGIN_RETRY";
                        }
                    }
                }
                else if (iRval == 1)//Not Active user
                {
                    objResp = clGeneral.getOutputObjResponse("1", "User is not Active");
                    //errormessage = "Invalid Username or Password";
                    type = "";
                }
                else if (iRval == 2)//User not present/Invalid credentials.
                {
                    objResp = clGeneral.getOutputObjResponse("1", "Invalid Username or Password");
                    //errormessage = "Invalid Username or Password";
                    type = "";
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clBLogin", LogMode.Excep, ex, "getUserDetails ");
            }
            finally
            {
                dtRecords = null;
                dtLogeduser = null;
                objppolicy = null;

                dtPasswordExp = null;
            }
            return objResp;
        }

        /// <summary>
        /// To Check wether user is locked or not
        /// </summary>
        /// <param name="username"></param>
        /// <param name="ilockperiod"></param>
        /// <param name="objppolicy"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool isUserLocked(string username, ref int ilockperiod, PwdPolicy objppolicy, string ip)
        {
            bool userlocked = false;
            DataTable dtRetryCount = null;
            try
            {
                int.TryParse(clGeneral.GetConfigValue("LOCK_PERIOD"), out ilockperiod);
                ilockperiod = ilockperiod == 0 ? 20 : ilockperiod;

                int retrycount = 0;
                // Checking user locked or not
                dtRetryCount = new clDLogin().getuserretrycount(username);
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
                                new clDLogin().saveuserretrycount(username, ip, "RDATE");
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


        #endregion

        #region OBJECT

        /// <summary>
        /// To frame the password policy object
        /// </summary>
        /// <param name="dtRecords"></param>
        /// <returns></returns>
        public PwdPolicy framePasswordpolicy(DataTable dtRecords)
        {
            PwdPolicy objppolicy = null;
            int iValue = 0;
            try
            {
                if (dtRecords != null && dtRecords.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtRecords.Rows)
                    {
                        objppolicy = new PwdPolicy();
                        int.TryParse(dr["MinLength"].ToString(), out iValue);
                        iValue = iValue == 0 ? 6 : iValue;
                        objppolicy.MinLength = iValue;

                        objppolicy.GrpCaps = dr["grpcaps"].ToString();
                        objppolicy.GrpSmalls = dr["grpsmalls"].ToString();
                        objppolicy.GrpNums = dr["grpnums"].ToString();
                        objppolicy.GrpSplChars = dr["grpsplchars"].ToString();

                        int.TryParse(dr["notinprevmatches"].ToString(), out iValue);
                        iValue = iValue == 0 ? 4 : iValue;
                        objppolicy.NotInPrevMatches = iValue;

                        int.TryParse(dr["expireindays"].ToString(), out iValue);
                        iValue = iValue == 0 ? 120 : iValue;
                        objppolicy.ExpireInDays = iValue;

                        int.TryParse(dr["alertdays"].ToString(), out iValue);
                        iValue = iValue == 0 ? 120 : iValue;
                        objppolicy.AlertDays = iValue;

                        objppolicy.Active = dr["active"].ToString();

                        int.TryParse(dr["matchcharlen"].ToString(), out iValue);
                        iValue = iValue == 0 ? 120 : iValue;
                        objppolicy.MatchCharLen = iValue;

                        break;

                    }
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clBLogin", LogMode.Excep, ex, "framePasswordpolicy ");
            }
            finally
            {

            }
            return objppolicy;
        }

        #endregion


    }
}
