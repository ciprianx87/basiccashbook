using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

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
            return date.ToString("dd.MM.yyyy");
        }

      
    }
}
