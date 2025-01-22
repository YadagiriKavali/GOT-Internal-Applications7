using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMI.Logger;

/// <summary>
/// Summary description for Textlog
/// </summary>

public static class TextLog
{
    /// <summary>
    /// To Write Info  Message in Log file
    /// </summary>
    /// <param name="FileName">Name of the file name</param>
    /// <param name="sMessage">Message to be logged</param>
    public static void Info(string ws, string FileName, string sMessage)
    {
        if (General.GetConfigVal("ERRORLOG_INFO").ToUpper().GetBoolean())
            LogData.Write(ws, FileName, LogMode.Info, sMessage);
    }

    /// <summary>
    /// To Write Info  Exception in Log file
    /// </summary>
    /// <param name="FileName">Name of the file name</param>
    /// <param name="ex">Exception Message to be logged</param>
    public static void Exception(string ws, string FileName, Exception ex)
    {
        LogData.Write(ws, FileName, LogMode.Excep, ex);
    }

    /// <summary>
    /// To Write Error  Message in Log file
    /// </summary>
    /// <param name="FileName">Name of the file name</param>
    /// <param name="sMessage">Message to be logged</param>
    public static void Error(string ws, string FileName, string sMessage)
    {
        if (General.GetConfigVal("ERRORLOG_ERROR").ToUpper().GetBoolean())
            LogData.Write(ws, FileName, LogMode.Error, sMessage);
    }

    /// <summary>
    /// To Write Debug  Message in Log file
    /// </summary>
    /// <param name="FileName">Name of the file name</param>
    /// <param name="sMessage">Message to be logged</param>
    public static void Debug(string ws, string FileName, string sMessage)
    {
        if (General.GetConfigVal("ERRORLOG_DEBUG").ToUpper().GetBoolean())
            LogData.Write(ws, FileName, LogMode.Debug, sMessage);
    }

    /// <summary>
    /// To Write Audit  Message in Log file
    /// </summary>
    /// <param name="FileName">Name of the file name</param>
    /// <param name="sMessage">Message to be logged</param>
    public static void Audit(string ws, string FileName, string sMessage)
    {
        if (General.GetConfigVal("ERRORLOG_AUDIT").ToUpper().GetBoolean())
            LogData.Write(ws, FileName, LogMode.Audit, sMessage);
    }

    /// <summary>
    /// To Write DBError  Message in Log file
    /// </summary>
    /// <param name="FileName">Name of the file name</param>
    /// <param name="sMessage">Message to be logged</param>
    public static void DBError(string ws, string FileName, string sMessage)
    {
        LogData.Write(ws, FileName, LogMode.DBError, sMessage);
    }

    /// <summary>
    /// To Write IOError  Message in Log file
    /// </summary>
    /// <param name="FileName">Name of the file name</param>
    /// <param name="sMessage">Message to be logged</param>
    public static void IOError(string ws, string FileName, string sMessage)
    {
        LogData.Write(ws, FileName, LogMode.IOError, sMessage);
    }
    /// <summary>
    /// To Write Info  Exception in Log file
    /// </summary>
    /// <param name="FileName">Name of the file name</param>
    /// <param name="ex">Exception Message to be logged</param>
    public static void Exception(string ws, string FileName, string sMessage)
    {
        LogData.Write(ws, FileName, LogMode.Excep, sMessage);
    }
}
