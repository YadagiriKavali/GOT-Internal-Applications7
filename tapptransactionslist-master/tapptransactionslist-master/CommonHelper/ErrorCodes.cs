//$Author: Anilkumar.ko $
//$Date: 12/05/17 6:27p $
//$Header: /Mobileapps/Operator/Davinci/TS/TAppTransactionsList/TAppTransactionsList/CommonHelper/ErrorCodes.cs 1     12/05/17 6:27p Anilkumar.ko $
//$Modtime: 8/27/15 10:29a $
//$Revision: 1 $
//Copyright @2012 IMImobile Pvt. Ltd.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonHelper
{
    public class ErrorCodes
    {
        public enum Departments
        {
            AIRTEL,
            BESCOM,
            BSNL,
            BTP,
            BWSSB,
            CELLONE,
            HDMC,
            HESCOM,
            HRMS,
            INGPOLICY,
            KSRTC,
            KWB,
            SAKALA,
            VODAFONE,
            PAYMENT
        }

        public class BESCOM
        {
            public const String ACCOUNT_NUMBER_EMPTY_CODE = "1000";
            public const String ACCOUNT_NUMBER_EMPTY_MSG = "Account Number should not be null";
        }
    }
}
