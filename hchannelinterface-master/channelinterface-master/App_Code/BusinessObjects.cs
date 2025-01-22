using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for BusinessObjects
/// </summary>
public class IMPSBO
{
    public string RespCode { get; set; }
    public string RespDesc { get; set; }
    public string IMPSAmount { get; set; }
    public string TransNo { get; set; }
    public string AuthCode { get; set; }

    public string IMITransID { get; set; }

}

public class TWALLETBO
{
    public string RespCode { get; set; }
    public string RespDesc { get; set; }
    public string TWalletAmount { get; set; }
    public string TransNo { get; set; }
    public string AuthCode { get; set; }
    public string TransDate { get; set; }
    public string IMITransID { get; set; }

}

public class AckStatusBO
{
    public string AckJson { get; set; }
    public string ActualBillAmount { get; set; }
    public string RespCode { get; set; }
    public string RespDesc { get; set; }
    public string Service { get; set; }
    public string Action { get; set; }
    public string ActionDesc { get; set; }

}