using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonHelper;
using System.Xml;
using System.Data;
using IMI.SqlWrapper;
using System.Data.SqlClient;

namespace TAppTransactionsList
{
    class Program
    {
        static void Main(string[] args)
        {
            if (General.GetConfigVal("REPORT_TYPE") == "GET_TRANSLIST")
            {
                GetTransList.GetTransactionsList();
            }
            else if (General.GetConfigVal("REPORT_TYPE") == "COMPARE_TRANS")
            {
                CompareTrans.CompareTransList();
            }
        }
    }
}
