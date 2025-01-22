namespace User
{
    public class PwdPolicy
    {
        public int MinLength { get; set; }
        public string GrpCaps { get; set; }
        public string GrpSmalls { get; set; }
        public string GrpNums { get; set; }
        public string GrpSplChars { get; set; }
        public string UpdatedBy { get; set; }
        public int NotInPrevMatches { get; set; }
        public int ExpireInDays { get; set; }
        public int AlertDays { get; set; }
        public string Active { get; set; }
        public int MatchCharLen { get; set; }
    }
}
