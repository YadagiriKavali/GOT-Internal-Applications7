using System;
using System.Collections.Generic;
using System.Data;
using IMI.Logger;
using Newtonsoft.Json;
using User.Cryptography;

namespace User
{
    public class clBUsers
    {
        clReadXML oxml = null;
        public clBUsers()
        {
            //
            // TODO: Add constructor logic here
            //
            oxml = new clReadXML();
        }

        /// <summary>
        /// To insert or update the user details.
        /// </summary>
        /// <param name="ousers"></param>
        /// <returns></returns>
        public UResponse SaveUserDtls(UserDetail ousers)
        {
            UResponse objResp = null;
            if (oxml == null) oxml = new clReadXML();
            try
            {
                ousers.Pwd = new PasswordFormat().GetPassword(ousers.Pwd);
                int rval = new clDUsers().Saveusers(ousers);
                if (rval == -1)
                    objResp = clGeneral.getOutputObjResponse("1", oxml.getUserErrorMsg("USERID_EXISTS"));
                else if (rval == 0)
                    objResp = clGeneral.getDefaultObjResponse();
                else
                {
                    objResp = clGeneral.getOutputObjResponse("0", oxml.getUserSuccessMsg("INSERT"));
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clBUsers", LogMode.Excep, ex, "SaveUserDtls");
            }
            finally
            {
                if (objResp == null)
                    objResp = clGeneral.getDefaultObjResponse();
            }
            return objResp;
        }


        /// <summary>
        /// To Update the user details.
        /// </summary>
        /// <param name="ousers"></param>
        /// <returns></returns>
        public UResponse UserUpdate(UserDetail ousers)
        {
            //string response = string.Empty;
            UResponse objResp = null;
            if (oxml == null) oxml = new clReadXML();

            try
            {
                int rval = new clDUsers().UserUpdate(ousers);
                if (rval == -1)
                    objResp = clGeneral.getOutputObjResponse("1", oxml.getUserErrorMsg("USERID_NOTEXISTS"));
                else if (rval == 0)
                    objResp = clGeneral.getDefaultObjResponse();
                else
                {
                    objResp = clGeneral.getOutputObjResponse("0", oxml.getUserSuccessMsg("UPDATE"));
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clBUsers", LogMode.Excep, ex, "UserUpdate");
            }
            finally
            {
                if (objResp == null)
                    objResp = clGeneral.getDefaultObjResponse();
            }
            return objResp;
        }

        /// <summary>
        /// TO GET USERSLIST
        /// </summary>
        /// <returns></returns>
        public string getUserDtls()
        {

            string resp = null;
            DataTable dtuser = null;
            List<UserDetail> userlist = null;
            try
            {
                dtuser = new clDUsers().getUserDtls();
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
                    resp = clGeneral.getOutputResponse("1", oxml.getUserErrorMsg("NODATA"));
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

        public List<UserDetail> getUserDtlsObj()
        {
            string resp = null;
            DataTable dtuser = null;
            List<UserDetail> userlist = null;
            try
            {
                dtuser = new clDUsers().getUserDtls();
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
                    resp = clGeneral.getOutputResponse("1", oxml.getUserErrorMsg("NODATA"));
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
            return userlist;
        }

        public DataTable getUserDtlsByLoginId(string sLoginid)
        {
            DataTable dtuser = null;
            try
            {
                dtuser = new clDUsers().getUserDtlsByLoginId(sLoginid);
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clBUsers", LogMode.Excep, ex, "getUserDtlsById()");
            }
            finally
            {
            }
            return dtuser;
        }

        public UResponse changePassword(PasswordChange pwdChange)
        {
            UResponse objResp = null;
            if (oxml == null) oxml = new clReadXML();
            DataTable dtUser = null;
            try
            {
                dtUser = getUserDtlsByLoginId(pwdChange.LoginId);
                if (!string.IsNullOrEmpty(pwdChange.OldPwd) && !string.IsNullOrEmpty(pwdChange.NewPwd) && !string.IsNullOrEmpty(pwdChange.ConfirmPwd))
                {
                    if (string.Compare(new PasswordFormat().GetPassword(pwdChange.OldPwd), dtUser.Rows[0]["Pwd"].ToString(), false) == 0)
                    {
                        if (string.Compare(pwdChange.NewPwd, pwdChange.ConfirmPwd, false) == 0)
                        {
                            pwdChange.NewPwd = new PasswordFormat().GetPassword(pwdChange.NewPwd);
                            pwdChange.OldPwd = new PasswordFormat().GetPassword(pwdChange.OldPwd);
                            new clDUsers().changePassword(pwdChange);
                            objResp = clGeneral.getOutputObjResponse("1", "Password changed successfully.");
                        }
                        else
                        {
                            // new pasword mismatch
                            objResp = clGeneral.getOutputObjResponse("0", "New password and confirm password should be same.");
                        }
                    }
                    else
                    {
                        // old password mismatch
                        objResp = clGeneral.getOutputObjResponse("0", "Old password is incorrect.");
                    }
                }
                else
                {
                    objResp = clGeneral.getOutputObjResponse("1", "Provide proper values");
                }
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "clBUsers", LogMode.Excep, ex, "UserUpdate");
            }
            finally
            {
                if (objResp==null)
                    objResp = clGeneral.getDefaultObjResponse();
            }
            return objResp;
        }
    }
}
