using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for clEFiles
/// </summary>
public class clEFiles
{
    public class Item
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string size { get; set; }
        public string date { get; set; }
        public string parent { get; set; }
        public string mime { get; set; }
        public string uri { get; set; }
        public string description { get; set; }
        public string issuer { get; set; }
    }

    public class UploadedFiles
    {
        public string directory { get; set; }
        public List<Item> items { get; set; }
    }
    
}

public class TokenResDept : ErrorDetails
{
    public string access_token { get; set; }
    public string expires_in { get; set; }
    public string token_type { get; set; }
    public string scope { get; set; }
    public string refresh_token { get; set; }
    public string digilockerid { get; set; }
    public string name { get; set; }
    public string dob { get; set; }
    public string gender { get; set; }
    public string eaadhaar { get; set; }
    public string reference_key { get; set; }
    public string new_account { get; set; }
}

public class ErrorDetails
{
    public string error { get; set; }
    public string error_description { get; set; }
    public string state { get; set; }
}