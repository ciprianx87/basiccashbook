using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
