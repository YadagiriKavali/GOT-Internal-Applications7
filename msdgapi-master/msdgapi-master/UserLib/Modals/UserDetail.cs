using System.Collections.Generic;
namespace User
{
    public class UserDetail : UResponse
    {
        public string Tid { get; set; }
        public string Channel { get; set; }
        public int UserId { get; set; }
        public string MobileNo { get; set; }
        public string LoginId { get; set; }
        public string Pwd { get; set; }
        public string UserType { get; set; }
        public string Email { get; set; }
        public string Active { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
        public string RegType { get; set; }
        public string AppType { get; set; }
        public string IP { get; set; }
        public string UpdatedIP { get; set; }
        public string PwdPolicy { get; set; }        
    }

    public class UsersDetail : UResponse
    {
        public List<UserDetail> Users { get; set; }

        public UsersDetail()
        {
            Users = new List<UserDetail>();
        }
    }

    public class PasswordChange
    {
        public string LoginId { get; set; }
        public string OldPwd { get; set; }
        public string NewPwd { get; set; }
        public string ConfirmPwd { get; set; }
    }
}
