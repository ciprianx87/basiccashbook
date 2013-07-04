using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;

namespace CashBook.Common
{
    public static class Utils
    {
        public static int GeneratePairedKey(string initialKey)
        {
            string constantKey = "CashRegister2013";
            int hash = 23;
            hash = hash * 2 + initialKey.GetHashCode();
            hash = hash * 3 + constantKey.GetHashCode();
            hash = Math.Abs(hash);
            Debug.WriteLine("generated paired key: " + hash);
            return hash;

        }

        public static DateTime DateTimeToDay(DateTime date)
        {
            return date.Date;
        }
        public static DateTime? DateTimeToDay(DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.Date;
            }
            else
            {
                return null;
            }
        }

        public static string DateTimeToString(DateTime date)
        {
            return date.ToString(Constants.DateTimeFormat);
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

            if (Count(decimalString, '.') == 0 && Count(decimalString, ',') == 1)
            {
                return decimalString;
            }
            if (Count(decimalString, '.') >= 1 && Count(decimalString, ',') == 1)
            {
                decimalString = decimalString.Replace(".", "");
                decimalString = decimalString.Replace(".", ",");
                return decimalString;
            }
            return decimalString;

        }
        private static int Count(string target, char ch)
        {
            return target.Count(p => p == ch);
        }
    }
}
