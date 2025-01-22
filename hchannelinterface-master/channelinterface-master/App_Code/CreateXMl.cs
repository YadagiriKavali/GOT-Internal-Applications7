using IMI.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;


    public class CreateXMl
    {

        public string getxmlstring(string data, string key)
        {
            try
            {
                XElement xml = new XElement("TA",
             new XElement("Data", data),
             new XElement("S_KEY", key)
         );
                return xml.ToString();
            }
            catch (Exception ex)
            {
                LogData.Write("TwalletIVRS_bal", "getxmlstring", LogMode.Excep, ex.Message);

                return "";
            }

        }

        public string getxmlstring_msisdn(string msisdn, string myip)
        {
            try
            {
                XElement xml = new XElement("Request",
             new XAttribute("type", "IVRSCustCheck"),
               new XAttribute("Terminal_Number", General.GetConfigVal("TERMINAL_NUMBER")),
                  new XAttribute("Terminal_Name", General.GetConfigVal("TERMINAL_NAME")),

                     new XElement("Machine_Id", myip),
                               new XElement("Mobile_num", msisdn)
         );
                return xml.ToString();
            }
            catch (Exception ex)
            {
                LogData.Write("TwalletIVRS_bal", "getxmlstring_msisdn", LogMode.Excep, ex.Message);

                return "";
            }

        }


        public string getxmlstring_bal(string msisdn, string myip)
        {
            try
            {
                XElement xml = new XElement("Request",
             new XAttribute("type", "IVRSCustBalEnq"),
               new XAttribute("Terminal_Number", General.GetConfigVal("TERMINAL_NUMBER")),
                  new XAttribute("Terminal_Name", General.GetConfigVal("TERMINAL_NAME")),

                     new XElement("Machine_Id", myip),
                               new XElement("Mobile_num", msisdn)
         );
                return xml.ToString();
            }
            catch (Exception ex)
            {
                LogData.Write("TwalletIVRS_bal", "getxmlstring_bal", LogMode.Excep, ex.Message);

                return "";
            }

        }

        public string getxmlstring_Othermsisdn(string msisdn,string othermsisdn, string myip)
        {
            try
            {
                XElement xml = new XElement("Request",
             new XAttribute("type", "IVRSOtherCustBalEnq"),
               new XAttribute("Terminal_Number", General.GetConfigVal("TERMINAL_NUMBER")),
                  new XAttribute("Terminal_Name", General.GetConfigVal("TERMINAL_NAME")),

                     new XElement("Machine_Id", myip),
                               new XElement("Mobile_num", msisdn),
                                      new XElement("Other_Mobile_num", othermsisdn)
         );
                return xml.ToString();
            }
            catch (Exception ex)
            {
                LogData.Write("TwalletIVRS_bal", "getxmlstring_Othermsisdn", LogMode.Excep, ex.Message);
                return "";
            }

        }

        public string getxmlstring_Othermsisdn_bal(string msisdn, string othermsisdn, string myip,string otp)
        {
            try
            {
                XElement xml = new XElement("Request",
             new XAttribute("type", "IVRSOtherCustBalEnqOTP"),
               new XAttribute("Terminal_Number", General.GetConfigVal("TERMINAL_NUMBER")),
                  new XAttribute("Terminal_Name", General.GetConfigVal("TERMINAL_NAME")),

                     new XElement("Machine_Id", myip),
                               new XElement("Mobile_num", msisdn),
                                      new XElement("Other_Mobile_num", othermsisdn),
                                        new XElement("OTP", otp)
         );
                return xml.ToString();
            }
            catch (Exception ex)
            {
                LogData.Write("TwalletIVRS_bal", "getxmlstring_Othermsisdn_bal", LogMode.Excep,ex.Message);
                return "";
            }

        }
    }
