using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashBook.Common
{
    public static class Constants
    {
        public static string LegalRelementationsKey = "LegalRelementations";
        public static string CoinTypesKey = "CoinTypes";
        public static string LegalLimitsKey = "LegalLimits";
        public static string KeyFilename = "C86D883A-7B72-4F49-9871-4EBA439036F9.cb";
        public static string SerialKeyFilename = "C86D883A-7B72-4F49-9871-4EBA439036F8.cb";

        public static string DateTimeFormat = "yyyy-MM-dd hh:mm:ss";
        public static string DateTimeFormatValability = "yyyy-MM-dd HH:mm:ss";

        public static int ValabilityDays = 30;

        public static int RemainingDays { get; set; }

    }
}
