using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AadharAdmin.Models
{
    #region Looged User
    public class loginUser : Request
    {
        [Required(ErrorMessage = "Please enter username")]
        public string username { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        public string password { get; set; }
    }
    public class loggedUser : Response
    {
        public UserData items { get; set; }
    }
    public class UserData
    {
        public string district { get; set; }
        public string centerid { get; set; }
        public string centercode { get; set; }
        public string centername { get; set; }
        public string operatorname { get; set; }
        public string mobileno { get; set; }
        public string email { get; set; }

    }

    public class UserDetail : Response
    {
        //public string Tid { get; set; }
        //public string Channel { get; set; }
        //public int UserId { get; set; }
        //public string MobileNo { get; set; }
        //public string LoginId { get; set; }
        //public string Pwd { get; set; }
        //public string UserType { get; set; }
        //public string Email { get; set; }
        //public string Active { get; set; }
        //public string CreatedOn { get; set; }
        //public string UpdatedOn { get; set; }
        //public string RegType { get; set; }
        //public string AppType { get; set; }
        //public string IP { get; set; }
        //public string UpdatedIP { get; set; }
        //public string PwdPolicy { get; set; }
        public string mobileno { get; set; }
        public string email { get; set; }
        public string language { get; set; }
        public string firstname { get; set; }
        public string aadhar { get; set; }
        public string temple_type { get; set; }
        public string appkey { get; set; }
        public string os { get; set; }
        public string aadhar_login_type { get; set; }
    }
    public class loginUserNew : Request
    {
        [Required(ErrorMessage ="Please select login type")]
        public string logintype { get; set; }
        [Required(ErrorMessage = "Please enter mobile number")]
        [RegularExpression("^[0-9]{10,10}$",ErrorMessage ="Please enter valid mobile number")]
        public string mobileno { get; set; }
        [Required(ErrorMessage = "Please enter otp")]
        [RegularExpression("^[0-9]{6,6}$", ErrorMessage = "Please enter valid otp")]
        public string otpVal { get; set; }
        public string Securitykey { get; set; }
    }
    #endregion

    #region Trans Details
    public class ReqAdminTransDet : Request
    {
        public string mobileno { get; set; }
        public string centerid { get; set; }
        public string districtname { get; set; }
        public string startdate { get; set; }
        public string enddate { get; set; }
        public string status { get; set; }
    }

    public class ReqTransDet : Request
    {
        public string mobileno { get; set; }
        public string centerid { get; set; }
        public string centrcode { get; set; }
        public string startdate { get; set; }
        public string enddate { get; set; }
        public string usertype { get; set; }
    }
    #endregion

    #region Pass Auth Req
    public class SlotPassCodeAuthReq : Request
    {
        public string passcode { get; set; }
        public string centerid { get; set; }
        public string mgovid { get; set; }
        public string mobileno { get; set; }
    }
    #endregion

    public class AdminUser : Request
    {
        public string mobileno { get; set; }
        public string actiontype { get; set; }
        public string actionvalue { get; set; }
        public string districts { get; set; }
        public string centerid { get; set; }
        public string slotdate { get; set; }
        public string hourid { get; set; }
        public string slotid { get; set; }
        public string Securitykey { get; set; }
    }

    public class AddCenter : Request
    {
        public string mobileno { get; set; }
        [Required(ErrorMessage = "Please select district")]
        public string distrctname { get; set; }
        [Required(ErrorMessage = "Please enter center name")]
        [RegularExpression("[a-zA-Z\\s]+", ErrorMessage = "Please enter valid center name")]
        public string centername { get; set; }
        [Required(ErrorMessage = "Please enter center code")]
        [RegularExpression("[a-zA-Z\\s]+", ErrorMessage = "Please enter valid center code")]
        public string centercode { get; set; }
        [Required(ErrorMessage = "Please enter operator name")]
        [RegularExpression("[a-zA-Z\\s]+", ErrorMessage = "Please enter valid operator name")]
        public string operatorname { get; set; }
        [Required(ErrorMessage = "Please enter mobile number")]
        [RegularExpression("^[0-9]{10,10}", ErrorMessage = "Please enter valid mobile number")]
        public string phoneno { get; set; }
        [Required(ErrorMessage = "Please enter email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter valid email")]
        public string email { get; set; }
        [Required(ErrorMessage = "Please enter user name")]
        [RegularExpression("[a-zA-Z\\s]+", ErrorMessage = "Please enter user name")]
        public string username { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        //[RegularExpression("", ErrorMessage = "Please enter center name")]
        public string password { get; set; }
        public string doneby { get; set; }

    }

    public class listOperTransDet : Response
    {
        public List<OperTransDet> TransDet { get; set; }
        public listOperTransDet()
        {
            TransDet = new List<OperTransDet>();
        }
    }

    public class OperTransDet
    {
        public string mgovid { get; set; }
        public string slotdate { get; set; }
        public string slottime { get; set; }
        public string district { get; set; }
        public string center { get; set; }
        public string name { get; set; }
        public string statusid { get; set; }
        public string status { get; set; }
        public string ActionStatus { get; set; }

    }
}