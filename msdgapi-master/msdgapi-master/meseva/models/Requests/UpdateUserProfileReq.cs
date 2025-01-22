namespace meseva.models.Requests
{
    public class UpdateUserProfileReq : MSRequest
    {
        public string LoginId { get; set; }
        public string LoginPassword { get; set; }
        public string UpdatedPassword { get; set; }
        public string Emailid { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string RoleID { get; set; }
    }
}
