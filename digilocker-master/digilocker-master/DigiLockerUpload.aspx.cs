using IMI.Helper;
using IMI.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;

public partial class DigiLockerUpload : System.Web.UI.Page
{
    public static string appid = "";
    public static string strDGURL = "";
    public static string strDocuments = "";
    public static string strMyCertificates = "";

    public static string AccessToken = "";
    public static string AuthCode = "";
    public static string reFreshToken = "";
    public static string expireTime = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string code = Request.QueryString["code"].ToStr();
            LogData.Write("DigiLockerUpload", "Page_Load", LogMode.Debug, "Auth Code:" + code);
            string strURL = General.GetConfigVal("API_GETAUTHTOKEN_TOKEN");             
            #region Commented
            //if (Request.Params != null && Request.Params[0].Contains("respuri"))
            //{
            //    Response.Redirect("testupload.aspx?" + HttpUtility.UrlDecode(Request.Params[0]));
            //}
            //else if (Request.QueryString.ToString().Length > 0)
            //{
            //    string sQuerystrings = string.Empty;

            //    foreach (String key in Request.QueryString.AllKeys)
            //    {
            //        sQuerystrings = sQuerystrings + "\r\n" + key + ":" + Request[key].ToString();
            //    }
            //    LogData.Write("GOTAPI", "DIGILOCKER", LogMode.Debug, "=> DigiLockerUpload-Pager_Load() :: QueryStrings->" + sQuerystrings);
            //    if (sQuerystrings.Contains("access_token"))//For response_type = TOKEN
            //    {
            //        string sAccessToken = Request["access_token"].ToString();
            //        string sExpiresTime = Request["expires_in"].ToString();
            //        string sTokenType = Request["token_type"].ToString();
            //        string sState = Request["state"].ToString();

            //        HtmlGenericControl div = new HtmlGenericControl("tViewDtls");
            //        String sUploadedDocs = new clGetDocs().GetUploadedDocs(sAccessToken, sExpiresTime, sTokenType, sState);
            //        tViewDtls.InnerHtml = sUploadedDocs;
            //    }
            //    else if (sQuerystrings.Contains("code"))//For response_type = CODE
            //    {

            //    }
            //}
            //else
            //{
            //    string strURL = General.GetConfigVal("API_GETAUTHTOKEN_TOKEN");
            //    string clientID = General.GetConfigVal("CLIENTID");
            //    strDGURL = strURL.Replace("!CLIENTID!", clientID);
            //}
            #endregion

            AuthCode = code;
            string Result = string.Empty;            
            var reqData = "grant_type=authorization_code&client_id="+ General.GetConfigVal("CLIENTID") + "&client_secret="+ General.GetConfigVal("CLIENTSECRET") + "&code="+ AuthCode + "&redirect_uri="+ General.GetConfigVal("TOKEN_REDIRECT_URI") + "";
            //dataVal = HttpUtility.UrlEncode(dataVal);
            //LogData.Write("DigiLockerUpload", "Page_Load_Ack_Data", LogMode.Debug, "PayLoad:" + reqData);
            Result = clsToken.FinalJosnWebReq(General.GetConfigVal("API_GET_ACCESS_TOKEN"), reqData);
            //LogData.Write("DigiLockerUpload", "Page_Load_Ack_Data", LogMode.Debug, Result);

            //Need to deserilize and check
            if (!string.IsNullOrEmpty(Result))
            {
                TokenResDept Res = JsonConvert.DeserializeObject<TokenResDept>(Result);
                if (string.IsNullOrEmpty(Res.error))
                {
                    AccessToken = Res.access_token.ToStr();
                    reFreshToken = Res.refresh_token.ToStr();
                    expireTime = Res.expires_in.ToStr();
                    LogData.Write("DigiLockerUpload", "Page_Load_Ack_Data", LogMode.Debug, Result);
                }
                else
                    LogData.Write("DigiLockerUpload", "Page_Load_Ack_Data", LogMode.Debug, Result);
            }
            else
            {
                LogData.Write("DigiLockerUpload", "Page_Load_Ack_Data", LogMode.Debug, "Empty Result is coming");
            }
        }
        catch (Exception ex)
        {
            LogData.Write("GOTAPI", "DIGILOCKER", LogMode.Excep, ex, string.Format(" => DigiLockerUpload-Pager_Load()- Ex:{0}", ex.Message));
        }
    }
}