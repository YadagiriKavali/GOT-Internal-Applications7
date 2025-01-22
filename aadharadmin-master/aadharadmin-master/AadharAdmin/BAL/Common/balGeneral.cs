using IMI.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace AadharAdmin.BAL.Common
{
    public interface IbalGeneral
    {
        string GetClentIP();
        void ChangeSecurityKey(string temSecurity);
        bool ValidatePassCode(string passCode);
        bool ValidateMGOVID(string mgovid);
        bool CheckIsAdmin();
    }
    public class balGeneral : IbalGeneral
    {
        public string GetClentIP()
        {
            string hostName = Dns.GetHostName();
            return Dns.GetHostByName(hostName).AddressList[0].ToString();
        }

        public void ChangeSecurityKey(string temSecurity)
        {
            HttpContext.Current.Session["Securitykey"] = temSecurity;
        }

        public bool ValidatePassCode(string passCode)
        {
            bool flagIsValid = false;
            try
            {
                //validate Mobile No format
                string passCodeRegex = @"^[0-9]{3,6}$";
                Regex re = new Regex(passCodeRegex);
                if (re.IsMatch(passCode))
                {
                    flagIsValid = true;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("balGeneral", "ValidatePassCode", LogMode.Excep, ex.Message);
            }
            return flagIsValid;
        }

        public bool ValidateMGOVID(string mgovid)
        {
            bool flagIsValid = false;
            try
            {
                //validate Mobile No format
                string mgovRegex = @"^[a-zA-Z0-9]{10,50}$";
                Regex re = new Regex(mgovRegex);
                if (re.IsMatch(mgovid))
                {
                    flagIsValid = true;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("balGeneral", "ValidateMGOVID", LogMode.Excep, ex.Message);
            }
            return flagIsValid;
        }

        public bool CheckIsAdmin()
        {
            bool isAdmin = false;
            try
            {
                if (HttpContext.Current.Session["USERTYPE"] !=null && HttpContext.Current.Session["USERTYPE"].ToString().ToUpper() == "A")
                {
                    isAdmin = true;
                }
            }
            catch (Exception ex)
            {
                LogData.Write("balGeneral", "CheckIsAdmin", LogMode.Excep, ex.Message);
            }
            return isAdmin;
        }
    }
}