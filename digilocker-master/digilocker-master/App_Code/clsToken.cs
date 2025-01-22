using IMI.Helper;
using IMI.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for clsToken
/// </summary>
public static class clsToken
{

    public static string FinalJosnWebReq(string strUrl, string Payload)
    {
        string Result = string.Empty;
        try
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | 
                SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | (SecurityProtocolType)3072;
            //LogData.Write("clsToken", "DoRequestForAccessToken", LogMode.Debug, "FinalJosnWebReq Request Started");
            //WebRequest request = WebRequest.Create(strUrl);
            var request = (HttpWebRequest)WebRequest.Create(new Uri(strUrl));
            string transid = DateTime.Now.Ticks.ToString();
            // Set the Method property of the request to POST.
            request.Method = "POST";
            request.Timeout = General.GetConfigVal("PageTimeout") == "" ? 600000 : Convert.ToInt32(General.GetConfigVal("PageTimeout"));

            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(General.GetConfigVal("CLIENTID") + ":" + General.GetConfigVal("CLIENTSECRET")));

            //request.Headers.Add("Authorization", General.GetConfigVal("AUTHORIZATION_VALUE"));
            request.Headers.Add("Authorization", "Basic " + svcCredentials);

            //LogData.Write("clsToken", "DoRequestForAccessToken", LogMode.Debug, "FinalJosnWebReq: Authorization Hdeader added to the WebRequest.");

            byte[] byteArray = Encoding.UTF8.GetBytes(Payload);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            //LogData.Write("clsToken", "DoRequestForAccessToken", LogMode.Debug, "FinalJosnWebReq: Set the ContentType property of the WebRequest.");
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            //LogData.Write("clsToken", "DoRequestForAccessToken", LogMode.Debug, "FinalJosnWebReq: Set the ContentLength property of the WebRequest.");
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            //LogData.Write("clsToken", "DoRequestForAccessToken", LogMode.Debug, "FinalJosnWebReq: Get the request stream");
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            //LogData.Write("clsToken", "DoRequestForAccessToken", LogMode.Debug, "FinalJosnWebReq: Write the data to the request stream");
            // Close the Stream object.
            dataStream.Close();
            //LogData.Write("clsToken", "DoRequestForAccessToken", LogMode.Debug, "FinalJosnWebReq: dataStream closed");
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            //Console.WriteLine(responseFromServer);
            Result = responseFromServer;
            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();
            //LogData.Write("clsToken", "DoRequestForAccessToken", LogMode.Debug, "FinalJosnWebReq response closed");
        }
        catch (WebException WebEx)
        {
            using (var reader = new StreamReader(WebEx.Response.GetResponseStream()))
                Result = "Web Exception:" + reader.ReadToEnd();
            LogData.Write("GOTAPI", "DIGILOCKER", LogMode.Excep, WebEx.Message);
        }
        catch (Exception ex)
        {
            Result = "Exception:" + ex.Message;
            LogData.Write("GOTAPI", "DIGILOCKER", LogMode.Excep, ex.Message);
        }
        finally
        {
            LogData.Write("GOTAPI", "DIGILOCKER", LogMode.Audit, string.Format("Url:{0},Req:{1},Res:{2}", strUrl, Payload, Result));
        }
        return Result;
    }

}