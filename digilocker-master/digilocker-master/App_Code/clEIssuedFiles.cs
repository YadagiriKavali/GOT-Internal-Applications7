using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for clEIssuedFiles
/// </summary>
public class clEIssuedFiles
{
    public class Item
    {
        public string name { get; set; }
        public string type { get; set; }
        public string size { get; set; }
        public string date { get; set; }
        public string parent { get; set; }
        public List<string> mime { get; set; }
        public string uri { get; set; }
        public string description { get; set; }
        public string issuer { get; set; }
        public string doctype { get; set; }
        public string issuerid { get; set; }
    }

    public class IssuedFiles
    {
        public List<Item> items { get; set; }
        public string resource { get; set; }
    }
}