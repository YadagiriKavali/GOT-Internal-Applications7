using System;
using System.Collections.Generic;
using IMI.Logger;
using Newtonsoft.Json;
using User.BL;

namespace User
{
    public class Users
    {
        #region [ USER LOGIN METHODS ]

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
        public UResponse CheckUserDetail(object data)
        {
            UserDetail userDetail = null;

            try
            {
                userDetail = JsonConvert.DeserializeObject<UserDetail>(data.ToString());
            }
            catch { }

            if (userDetail == null)
                return new UResponse { ResCode = "1", ResDesc = "Invalis request" };

            return new LoginBL().CheckUserDetail(userDetail.LoginId, userDetail.Pwd, userDetail.IP);
        }

        /// <summary>
        /// To Save the user session data
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="sessionid"></param>
        /// <returns></returns>
        public UResponse SaveUserLoginInfo(string userName, string sessionid)
        {
            if (new LoginBL().SaveUserLoginInfo(userName, sessionid))
                return new UResponse { ResCode = "0", ResDesc = "User information is saved successfully" };
            else
                return new UResponse { ResCode = "1", ResDesc = "Error occurred while saving user information" };
        }

        /// <summary>
        /// To delete the session data
        /// </summary>
        /// <param name="userName"></param>
        public void DeleteUserLoginInfo(string userName)
        {
            try
            {
                new LoginBL().DeleteUserLoginInfo(userName);
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "Users", LogMode.Excep, ex, "deletesessiondata-deleteuserlogininfo ");
            }
        }

        /// <summary>
        /// To update the password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="opassword"></param>
        /// <param name="ouser"></param>
        /// <returns></returns>
        public UResponse UpdatePassword(string password, string opassword, UserDetail ouser)
        {
            return new LoginBL().UpdatePassword(password, opassword, ouser);
        }

        #endregion [ USER LOGIN METHODS ]

        #region [ USER RELATED DETAIL METHODS ]

        /// <summary>
        /// To insert the user details.
        /// </summary>
        /// <param name="ousers"></param>
        /// <returns></returns>
        public UResponse UserRegistration(object data)
        {
            UserDetail userDetail = null;
            try
            {
                userDetail = JsonConvert.DeserializeObject<UserDetail>(data.ToString());
            }
            catch { }

            if (userDetail == null)
                return new UResponse { ResCode = "1", ResDesc = "Invalis request" };

            return new UserBL().SaveUserDetail(userDetail);
        }

        /// <summary>
        /// To update the user details.
        /// </summary>
        /// <param name="ousers"></param>
        /// <returns></returns>
        public UResponse UserUpdate(object data)
        {
            UserDetail userDetail = null;
            try
            {
                userDetail = JsonConvert.DeserializeObject<UserDetail>(data.ToString());
            }
            catch { }

            if (userDetail == null)
                return new UResponse { ResCode = "1", ResDesc = "Invalis request" };

            return new UserBL().UserUpdate(userDetail);
        }

        /// <summary>
        /// TO GET USERSLIST
        /// </summary>
        /// <returns></returns>
        public UResponse GetUsersDetail(object data)
        {
            var users = new UserBL().GetUsersDetail();
            if(users == null || users.Count <= 0)
                return new UResponse { ResCode = "1", ResDesc = "No data found" };

            return new UsersDetail { ResCode = "0", ResDesc = "Success", Users = users };
        }

        /// <summary>
        /// To get userslist based on Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetUserDetailById(int id)
        {
            string resp = null;
            try
            {
                resp = new UserBL().GetUserDetail();
            }
            catch (Exception ex)
            {
                LogData.Write("UserLib", "Users", LogMode.Excep, ex, "GetUserDetailById");
            }
            return resp;
        }

        /// <summary>
        /// To change the Password based on LoginId
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public UResponse ChangePassword(object data)
        {
            PasswordChange userpwd  = null;
            try
            {
                userpwd = JsonConvert.DeserializeObject<PasswordChange>(data.ToString());
            }
            catch { }

            if (userpwd == null)
                return new UResponse { ResCode = "1", ResDesc = "Invalis request" };

            return new UserBL().ChangePassword(userpwd); ;
        }

        #endregion [ USER RELATED DETAIL METHODS ]
    }
}
