using IMI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CodeCheck : System.Web.UI.Page
{
    public static string strDGURL = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        string strURL = General.GetConfigVal("API_GETAUTHTOKEN_CODE");
        string clientID = General.GetConfigVal("CLIENTID");
        strDGURL = strURL.Replace("!CLIENTID!", clientID);
    }
}