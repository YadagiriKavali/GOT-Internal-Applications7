using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMI.Helper;
using IMI.Logger;
using System.Text;
using Newtonsoft.Json;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for clGetDocs
/// </summary>
public class clGetDocs
{
    public String GetUploadedDocs(string sAccessToken, string sExpiresTime, string sTokenType, string sState)
    {
        String sResp = string.Empty;
        StringBuilder sbDtls = null;
        try
        {
            sbDtls = new StringBuilder();
            string sGetFileAPI = General.GetConfigVal("API_GETFILES");
            string sGetAuthHeader = General.GetConfigVal("API_GETFILES_AUTHHEADER").Replace("!AUTHTOKEN!", sAccessToken);
            string sAPIResp = clGeneral.DoGETRequest(sGetFileAPI, sGetAuthHeader);
            clEFiles.UploadedFiles objGetFiles = JsonConvert.DeserializeObject<clEFiles.UploadedFiles>(sResp);
            if (objGetFiles != null && objGetFiles.items.Count > 0)
            {
                sbDtls.Append("<table><thead>");
                sbDtls.Append("<tr style=\"color: #1976D2;\"><th id=\"headerName\" class=\"column-name\"><div id=\"headerName-container\"><a class=\"name sort columntitle\" data-sort=\"name\" style=\"color: #1976D2; font-weight: bold;\"><span>Name</span><span class=\"sort-indicator icon-triangle-n\"></span></a><span id=\"selectedActionsList\" class=\"selectedActions hidden\">");
                sbDtls.Append("<a href=\"\" class=\"download\"><img class=\"svg\" alt=\"\" src=\"https://cdntest.digitallocker.gov.in/core/img/actions/download.svg\">Download </a></span></div></th><th id=\"headerSize\" class=\"column-size\"><a class=\"size sort columntitle\" data-sort=\"size\" style=\"color: #1976D2; font-weight: bold;\"><span>Size</span><span class=\"sort-indicator hidden icon-triangle-s\"></span></a></th>");
                sbDtls.Append("<th id=\"headerDate\" class=\"column - mtime\"><a id=\"modified\" class=\"columntitle\" data-sort=\"mtime\" style=\"color: #1976D2; font-weight: bold;\"><span>Updated</span><span class=\"sort-indicator hidden icon-triangle-s\"></span></a><span class=\"selectedActions hidden\"><a href=\"\" class=\"delete-selected\">Delete<img class=\"svg\" alt=\"Delete\" src=\"https://cdntest.digitallocker.gov.in/core/img/actions/delete.svg\"></a></span></th><th></th>");
                sbDtls.Append("</tr>");
                sbDtls.Append("</thead>");
                sbDtls.Append("<tbody>");
                for (int i = objGetFiles.items.Count; i > 0; i--)
                {
                    sbDtls.Append("\n<tr>");
                    sbDtls.Append("<td valign=\"top\" style=\"width:50%\" onclick=\"GetDocuments(" + objGetFiles.items[i - 1].id + ",'" + sAccessToken + "')\">" + objGetFiles.items[i - 1].name + "</td>");
                    long lSize = 0;
                    long.TryParse(objGetFiles.items[i - 1].size, out lSize);
                    long lSizeinKB = lSize / 1024;
                    sbDtls.Append("<td valign=\"top\" style=\"width:25%;text-align: right;\">" + lSizeinKB + " kB</td>");
                    sbDtls.Append("<td valign=\"top\" style=\"width:25%\">" + objGetFiles.items[i - 1].date + "</td>");
                    sbDtls.Append("\n</tr>");
                }
                sbDtls.Append("</tbody>");
                sbDtls.Append("<tfoot><tr class=\"summary\"><td><span class=\"info\"><span class=\"dirinfo\">2 folders</span><span class=\"connector hidden\"> and </span><span class=\"fileinfo hidden\">0 files</span><span class=\"filter hidden\"></span></span></td><td class=\"filesize\">412 kB</td><td class=\"date\"></td></tr></tfoot>");
                sbDtls.Append("</table>");
                sResp = sbDtls.ToString();
            }
            else
            {
                //controlsv.Style.Add("display", "none");
            }
        }
        catch (Exception ex)
        {
            LogData.Write("GOTAPI", "DIGILOCKER", LogMode.Excep, ex, string.Format(" => DigiLockerUpload-GetUploadedDocs()- Ex:{0}", ex.Message));
        }
        return sResp;
    }
}