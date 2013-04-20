using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CashBook.Common
{
    public static class Utils
    {
        public static DateTime DateTimeToDay(DateTime date)
        {
            return date.Date;
        }

        public static string DateTimeToString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd hh:mm:ss");
        }

        public static string DateTimeToStringDateOnly(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static string DecimalToString(decimal number, int nrDecimals)
        {
            if (number == 0)
            {
                return "0";
            }
            string format = "0.";
            format = format.PadRight(nrDecimals + 2, '0');
            return number.ToString(format);
        }
    }
}
