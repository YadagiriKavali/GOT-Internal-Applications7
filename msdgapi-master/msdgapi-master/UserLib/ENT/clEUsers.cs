namespace User
{
    public class clEUsers : clEResp
    {
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
        public string UPDATEDIP { get; set; }
        public string PwdPolicy { get; set; }
    }

    public class EPwdChange
    {
        public string LoginId { get; set; }
        public string OldPwd { get; set; }
        public string NewPwd { get; set; }
        public string ConfirmPwd { get; set; }
    }
}
