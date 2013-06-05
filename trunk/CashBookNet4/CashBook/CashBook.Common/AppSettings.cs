using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CashBook.Common.Model;

namespace CashBook.Common
{
    public static class AppSettings
    {
        public static int InformationPopupCloseInterval { get; set; }
        public static int CashRegistryNameCharacterLimit { get; set; }
        public static int SinglePaymentLimit { get; set; }
        public static int TotalPaymentLimit { get; set; }

    }
}
