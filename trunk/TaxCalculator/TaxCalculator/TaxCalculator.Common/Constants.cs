using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxCalculator.Common
{
    public static class Constants
    {
        public static char[] AllowedRdOperations = new char[] { '-', '+' };
        public static string DateTimeFormat = "yyyy-MM-dd hh:mm:ss";
        public static string DateTimeFormatValability = "yyyy-MM-dd HH:mm:ss";
        public static string CoinTypesKey = "CoinTypes";
        public static List<string> AvailableMonths = new List<string>() { "Ianuarie", "Feburarie", "Martie", "Aprilie", "Mai", "Iunie", "Iulie", "August", "Septembrie", "Octombrie", "Noiembrie", "Decembrie" };
        public static List<int> AvailableYears = new List<int>() { 2013, 2014, 2015, 2016, 2017, 2018, 2019, 2020 };
        public static string CoinTypeLei = "LEI";
        public static int InfiniteLoopThreshold = 700;

        public static string LastSetupValueKey = "LastSetupValue";

    }
}
