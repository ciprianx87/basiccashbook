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

        public static string PrepareForConversion(string decimalString)
        {
            if (Count(decimalString, '.') == 1 && Count(decimalString, ',') == 0)
            {
                decimalString = decimalString.Replace(".", ",");
                return decimalString;
            }

            if (Count(decimalString, '.') == 0 && Count(decimalString, ',') ==1)
            {
                return decimalString;
            }
            if (Count(decimalString, '.') >= 1 && Count(decimalString, ',') == 1)
            {
                decimalString=decimalString.Replace(".", "");
                decimalString = decimalString.Replace(".", ",");
                return decimalString;
            }
            return decimalString;

        }
        private static int Count(string target, char ch){
            return target.Count(p => p == ch);
    }
    }
}
