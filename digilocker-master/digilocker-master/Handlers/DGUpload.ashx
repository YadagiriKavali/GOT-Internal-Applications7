<%@ WebHandler Language="C#" Class="DGUpload" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMI.Logger;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
//using XmpCore;
using System.Xml;
using System.Windows;
using RestSharp;

public class DGUpload : IHttpHandler
{
    public class Sim
    {
        public string MSISDN { get; set; }
        public string friendlyName { get; set; }
        public string MEID { get; set; }
        public string ICCID { get; set; }
        public string IMSI { get; set; }
        public int DataUsage { get; set; }
        public string ipAddress { get; set; }
        public string group { get; set; }
        public string status { get; set; }
        public string alert { get; set; }
        public string controls { get; set; }
        public string dataMSISDN { get; set; }
        public string localMSISDN { get; set; }
        public string mappedMSISDN { get; set; }
    }

    public class Status
    {
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string status { get; set; }
    }

    public class RootObject
    {
        public int totRecs { get; set; }
        public List<Sim> sims { get; set; }
        public Status status { get; set; }
    }

    public void ProcessRequest(HttpContext context)
    {
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        string responseMsg = "";
        string sRespType = "text/plain";

        try
        {
            switch (clGeneral.QueryString("action").ToLower())
            {
                case "getdocfiles":
                    {
                        string docid = clGeneral.QueryString("docid");
                        string accesstoken = clGeneral.QueryString("accesstoken");
                        string sGetFileAPI = clGeneral.GetConfigVal("API_GETFILES");
                        if (!string.IsNullOrEmpty(docid))
                            sGetFileAPI = clGeneral.GetConfigVal("API_GETFILES") + "/" + docid;
                        string sGetAuthHeader = clGeneral.GetConfigVal("API_GETFILES_AUTHHEADER").Replace("!AUTHTOKEN!", accesstoken);
                        string sResp = clGeneral.DoGETRequest(sGetFileAPI, sGetAuthHeader);
                        clEFiles.UploadedFiles objGetFiles = JsonConvert.DeserializeObject<clEFiles.UploadedFiles>(sResp);
                        if (objGetFiles != null && objGetFiles.items.Count > 0)
                        {
                            responseMsg = JsonConvert.SerializeObject(objGetFiles.items);
                        }
                        sRespType = "text/json";
                    }
                    break;
                case "getissueddocfiles":
                    {
                        string docid = clGeneral.QueryString("docid");
                        string accesstoken = clGeneral.QueryString("accesstoken");
                        string sGetFileAPI = clGeneral.GetConfigVal("API_GETISSUEDFILES");
                        if (!string.IsNullOrEmpty(docid))
                            sGetFileAPI = clGeneral.GetConfigVal("API_GETFILES") + "/" + docid;
                        string sGetAuthHeader = clGeneral.GetConfigVal("API_GETFILES_AUTHHEADER").Replace("!AUTHTOKEN!", accesstoken);
                        string sResp = clGeneral.DoGETRequest(sGetFileAPI, sGetAuthHeader);
                        clEIssuedFiles.IssuedFiles objGetFiles = JsonConvert.DeserializeObject<clEIssuedFiles.IssuedFiles>(sResp);
                        if (objGetFiles != null && objGetFiles.items.Count > 0)
                        {
                            responseMsg = JsonConvert.SerializeObject(objGetFiles.items);
                        }
                        sRespType = "text/json";
                    }
                    break;
                case "getpdffiles":
                    {
                        string docid = clGeneral.QueryString("docid");
                        string accesstoken = clGeneral.QueryString("accesstoken");
                        string sGetFileAPI = clGeneral.GetConfigVal("API_GETFILES");
                        if (!string.IsNullOrEmpty(docid))
                            sGetFileAPI = clGeneral.GetConfigVal("API_GETFILES") + "/" + docid;
                        string sGetAuthHeader = clGeneral.GetConfigVal("API_GETFILES_AUTHHEADER").Replace("!AUTHTOKEN!", accesstoken);
                        string sResp = clGeneral.DoGETRequest(sGetFileAPI, sGetAuthHeader);
                        clEFiles.UploadedFiles objGetFiles = JsonConvert.DeserializeObject<clEFiles.UploadedFiles>(sResp);
                        if (objGetFiles != null && objGetFiles.items.Count > 0)
                        {
                            responseMsg = JsonConvert.SerializeObject(objGetFiles.items);
                        }
                        sRespType = "text/json";
                    }
                    break;
                case "downloadpdffiles":
                    {
                        string pdfuri = clGeneral.QueryString("uri");
                        string accesstoken = clGeneral.QueryString("accesstoken");
                        string filename = clGeneral.QueryString("name");
                        string sGetFileAPI = clGeneral.GetConfigVal("API_GETFILES");
                        string mimeType = clGeneral.QueryString("mimetype");
                        string issuer = clGeneral.QueryString("issuer");
                        if (!string.IsNullOrEmpty(pdfuri))
                        {
                            sGetFileAPI = clGeneral.GetConfigVal("API_GETPDFFILES") + "/" + pdfuri;
                            string sGetAuthHeader = clGeneral.GetConfigVal("API_GETFILES_AUTHHEADER").Replace("!AUTHTOKEN!", accesstoken);
                            string sResp = clGeneral.DoGETRequestAndDownload(sGetFileAPI, sGetAuthHeader, filename);
                            //string sGetResp = clGeneral.DoRequest(sGetFileAPI, "", "POST", "", sGetAuthHeader);// clGeneral.DoGETRequest(sGetFileAPI, sGetAuthHeader);
                            LogData.Write("DGUpload_Handler", "DIGILOCKER_download_files", LogMode.Debug, string.Format("DGUpload Handler downloadpdffiles() with sGetFileAPI::{0}, sGetAuthHeader::{1}, filename::{2}, sResp::{3}", sGetFileAPI, sGetAuthHeader, filename, sResp));
                            //responseMsg = "respuri=" + sResp + "&accesstoken=" + accesstoken;// "{\"respuri\":\"" + sResp + "\",\"accesstoken\":\"Bearer " + accesstoken + "\"}";
                            //                                                                 //sRespType = mimeType;
                            if (!string.IsNullOrEmpty(issuer))
                                filename = filename.Replace(" ", "_") + GetMimeTypeExtension(mimeType);
                            LogData.Write("DGUpload_Handler", "DIGILOCKER_download_files", LogMode.Audit, "file Name:" + filename);
                            DownloadFile(sResp, filename, mimeType, accesstoken, context);
                            responseMsg = filename;
                            LogData.Write("DGUpload_Handler", "DIGILOCKER_download_files", LogMode.Audit, "responseMsg:" + responseMsg);
                        }
                    }
                    break;
                case "refresh":
                    {
                        #region token Refresh
                        string Result = string.Empty;
                        string refreshtoken = clGeneral.QueryString("refreshtoken");
                        string strReq = "grant_type=refresh_token&refresh_token=" + refreshtoken;
                        //LogData.Write("DGUpload_Handler", "ProcessRequest_Refersh_Token", LogMode.Debug, "PayLoad:" + strReq);
                        Result = clsToken.FinalJosnWebReq(clGeneral.GetConfigVal("API_GET_ACCESS_TOKEN"), strReq);
                        //LogData.Write("DGUpload_Handler", "ProcessRequest_Refersh_Token", LogMode.Debug, Result);
                        if (!string.IsNullOrEmpty(Result))
                        {
                            TokenResDept Res = JsonConvert.DeserializeObject<TokenResDept>(Result);
                            if (string.IsNullOrEmpty(Res.error))
                            {
                                responseMsg = JsonConvert.SerializeObject(Res);
                                //LogData.Write("DGUpload_Handler", "ProcessRequest_Refersh_Token", LogMode.Debug, responseMsg);
                            }
                        }
                        #endregion
                        sRespType = "text/json";
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            LogData.Write("GOTAPI", "DIGILOCKER", LogMode.Excep, "Exception in DGUploadHandler==>" + ex.ToString());
        }
        finally
        {
            context.Response.ContentType = sRespType;
            context.Response.Write(responseMsg);
        }
    }

    public string DownloadFile(string strRespURI, string strFileName, string strcontentType, string accesstoken, HttpContext context)
    {
        string strDownloadPath = (clGeneral.GetConfigVal("DIGILOCKER_DOWNLOAD_PATH").Length == 0) ? @"c:\uploadfiles\" : clGeneral.GetConfigVal("DIGILOCKER_DOWNLOAD_PATH");
        string fileName = strDownloadPath + "//" + strFileName;
        try
        {
            LogData.Write("GOTAPI", "DIGILOCKER_DownloadFile", LogMode.Audit, "DownloadFile_1: Landed strRespURI:" + strRespURI);
            var client = new RestClient(strRespURI);
            var request = new RestRequest(Method.POST);
            request.AddHeader("postman-token", "11e18e96-e120-9540-5d21-730bba028022");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", string.Format("Bearer {0}", accesstoken));
            request.Timeout = clGeneral.GetConfigVal("CONN_TIME_OUT") != "" ? int.Parse(clGeneral.GetConfigVal("CONN_TIME_OUT")) : 60000;
            //IRestResponse response =  client.Execute(request);
            byte[] tmpArr = client.DownloadData(request);
            LogData.Write("GOTAPI", "DIGILOCKER_DownloadFile", LogMode.Audit, "DownloadFile_2: dwonload Request");
            FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
            fs.Write(tmpArr, 0, tmpArr.Length);
            fs.Close();
            fs.Dispose();
            LogData.Write("GOTAPI", "DIGILOCKER_DownloadFile", LogMode.Audit, "DownloadFile_2: dwonload Completed.");
        }
        catch (WebException WebEx)
        {
            using (var reader = new StreamReader(WebEx.Response.GetResponseStream()))
                LogData.Write("GOTAPI", "DIGILOCKER_DownloadFile", LogMode.Excep, string.Format("WebException:{0},WebException:{1}", reader.ReadToEnd(), WebEx.Message));
        }
        catch (Exception ex)
        {
            LogData.Write("GOTAPI", "DIGILOCKER_DownloadFile", LogMode.Excep, "Exception in DownloadFile==>" + ex.ToString());
        }

        return fileName;
        //Base64Decode(response.Content, fileName);
    }

    private string Base64Decode(string data, string fileName)
    {
        FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
        int sizeOfChunk = 4;
        int startPosition = 0;
        while (startPosition < data.Length)
        {
            string tmp = data.Substring(startPosition, sizeOfChunk);
            startPosition = startPosition + sizeOfChunk;
            byte[] tmpArr = Convert.FromBase64String(tmp);
            fs.Write(tmpArr, 0, tmpArr.Length);
        }
        fs.Close();
        fs.Dispose();
        return fileName;
    }
    /*
string strDownloadPath = (clGeneral.GetConfigVal("DIGILOCKER_DOWNLOAD_PATH").Length == 0) ? @"c:\uploadfiles\" : clGeneral.GetConfigVal("DIGILOCKER_DOWNLOAD_PATH");
string fileName = strDownloadPath + strFileName;
LogData.Write("GOTAPI", "docDownload", LogMode.Debug, string.Format(" => DGUpload-for- fileName:{0}", fileName));
LogData.Write("GOTAPI", "docDownload", LogMode.Debug, string.Format(" => DGUpload-for- fileName1:{0}", (strFileName)));

var start = asString.IndexOf("<xmpGImg:image>");
var end = asString.IndexOf("</xmpGImg:image>") + 16;
//if (start == -1 || end == -1)
//    return null;

var justTheMeta = asString.Substring(start, end - start).Replace("<xmpGImg:image>", "").Replace("</xmpGImg:image>", "");
File.WriteAllBytes(fileName, Convert.FromBase64String(justTheMeta));
context.Response.Clear();
context.Response.ContentType = strcontentType;
context.Response.AddHeader("content-disposition", "attachment;filename=" + (strFileName));
//   Response.WriteFile(fileName);
//     Response.BinaryWrite(Convert.FromBase64String(content));
context.Response.TransmitFile(fileName);
context.Response.Flush();
context.Response.End();
//var returnVal = new XmlDocument();
//returnVal.LoadXml(justTheMeta);
//XmlNode node = returnVal.SelectSingleNode("//xmpGImg:image");
//return returnVal;
*/
    public static string GetFileFromBase64String(string strMobileNo, string strBase64String)
    {
        string strVirtualPath = string.Empty;
        string strDownloadPath = clGeneral.GetConfigVal("DIGILOCKER_DOWNLOAD_PATH").Length == 0 ? @"c:\downloadfiles\" : clGeneral.GetConfigVal("DIGILOCKER_DOWNLOAD_PATH");
        try
        {
            string strDestFilePath = String.Empty;
            string strExt = GetFileExtension(strBase64String);
            string strFileName = System.Guid.NewGuid().ToString("N") + strExt;
            foreach (String strVal in strDownloadPath.Split(','))
            {
                strDestFilePath = strVal.TrimEnd('\\') + @"\" + DateTime.Now.ToString("yyyy-MM-dd");// + @"\" + strMobileNo;
                if (!Directory.Exists(strDestFilePath))
                    Directory.CreateDirectory(strDestFilePath);

                File.WriteAllBytes(strDestFilePath + @"\" + strFileName, Convert.FromBase64String(strBase64String));
            }

            if (strExt.ToLower() == ".tiff")
            {
                //PdfDocument doc = new PdfDocument();
                //XSize size = new XSize(0, 0);
                //PdfPage oPage = new PdfPage();
                //doc.Pages.Add(oPage);
                //size = new XSize(oPage.Width, oPage.Height);
                //oPage.Height = size.Height;
                //oPage.Width = size.Width;
                //using (var xgr = XGraphics.FromPdfPage(oPage))
                //{
                //    var bm = doc.Pages[0];
                //    using (var img = XImage.FromFile(strDestFilePath + @"\" + strFileName))
                //        xgr.DrawImage(img, 0, 0, size.Width - 5, size.Height - 5);
                //}
                //doc.Save(strDestFilePath + @"\" + strFileName.Replace(".tiff", ".pdf"));
                //doc.Close();
                //strVirtualPath = clGeneral.GetConfigVal("DOWNLOAD_PATH_VIRTUAL").Length > 0 ? clGeneral.GetConfigVal("DOWNLOAD_PATH_VIRTUAL") : string.Empty;
                //strVirtualPath = strVirtualPath + DateTime.Now.ToString("yyyy-MM-dd") + @"/" + strMobileNo + @"/" + strFileName.Replace(".tiff", ".pdf");
            }
            else
            {
                strVirtualPath = clGeneral.GetConfigVal("DOWNLOAD_PATH_VIRTUAL").Length > 0 ? clGeneral.GetConfigVal("DOWNLOAD_PATH_VIRTUAL") : string.Empty;
                strVirtualPath = strVirtualPath + DateTime.Now.ToString("yyyy-MM-dd") + @"/" + strMobileNo + @"/" + strFileName;
            }
        }
        catch (Exception ex)
        {
            LogData.Write("GOTAPI", "APIHelper-File", LogMode.Excep, ex, string.Format("APIHelper => GetFileFromBase64String- Ex:{0}", ex.Message));
        }
        return strVirtualPath;
    }
    private static string GetFileExtension(string base64String)
    {
        var data = base64String.Substring(0, 5);

        switch (data.ToUpper())
        {
            case "IVBOR":
                return ".png";
            case "/9J/4":
                return ".jpg";
            case "AAAAF":
                return ".mp4";
            case "JVBER":
                return ".pdf";
            case "AAABA":
                return ".ico";
            case "UMFYI":
                return ".rar";
            case "E1XYD":
                return ".rtf";
            case "U1PKC":
                return ".txt";
            case "MQOWM":
            case "77U/M":
                return ".srt";
            case "SUKQA":
                return ".tiff";
            default:
                return string.Empty;
        }
    }

    private static string GetMimeTypeExtension(string sMimeType)
    {
        try
        {
            switch (sMimeType.Replace("\\", "").ToLower())
            {
                case "application/pdf":
                    return ".pdf";
            }
        }
        catch (Exception ex)
        {

        }
        return "";
    }
    //private static string GetFileExtension(string base64String)
    //{
    //    try
    //    {
    //        if (!string.IsNullOrEmpty(base64String) && base64String.Length > 5)
    //        {
    //            var data = base64String.Substring(0, 5);

    //            switch (data.ToUpper())
    //            {
    //                case "IVBOR":
    //                    return ".png";
    //                case "/9J/4":
    //                    return ".jpg";
    //                case "AAAAF":
    //                    return ".mp4";
    //                case "JVBER":
    //                    return ".pdf";
    //                case "AAABA":
    //                    return ".ico";
    //                case "UMFYI":
    //                    return ".rar";
    //                case "E1XYD":
    //                    return ".rtf";
    //                case "U1PKC":
    //                    return ".txt";
    //                case "MQOWM":
    //                case "77U/M":
    //                    return ".srt";
    //                case "SUKQA":
    //                    return ".tiff";
    //                default:
    //                    return string.Empty;
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {

    //    }
    //    return "";
    //}
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}