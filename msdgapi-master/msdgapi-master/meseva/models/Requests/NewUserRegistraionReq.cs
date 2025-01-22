namespace meseva.models.Requests
{
    public class NewUserRegistraionReq : MSRequest
    {
        public string Firstname { get; set; }
        public string Latsname { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Pincode { get; set; }
        public string Emailid { get; set; }
        public string LoginId { get; set; }
        public string Loginpassword { get; set; }
        public string SystemIP { get; set; }
        public string Adharno { get; set; } 
    }
}
