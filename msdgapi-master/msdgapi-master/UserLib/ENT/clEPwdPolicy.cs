namespace User
{
    public class clEPwdPolicy
    {
        public int minlength { get; set; }
        public string grpcaps { get; set; }
        public string grpsmalls { get; set; }
        public string grpnums { get; set; }
        public string grpsplchars { get; set; }
        public string updatedby { get; set; }
        public int notinprevmatches { get; set; }
        public int expireindays { get; set; }
        public int alertdays { get; set; }
        public string active { get; set; }
        public int matchcharlen { get; set; }
    }
}
