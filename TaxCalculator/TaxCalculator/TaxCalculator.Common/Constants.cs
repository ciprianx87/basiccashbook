﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxCalculator.Common
{
    public static class Constants
    {
        static Constants()
        {
            AvailableYears = new List<int>();
            for (int i = 2014; i <= 2050; i++)
            {
                AvailableYears.Add(i);
            }
        }
        public static char[] AllowedRdOperations = new char[] { '-', '+' };
        public static string DateTimeFormat = "yyyy-MM-dd hh:mm:ss";
        public static string DateFormat = "dd-MM-yyyy";
        public static string DateTimeFormatValability = "yyyy-MM-dd HH:mm:ss";
        public static string CoinTypesKey = "CoinTypes";
        public static List<string> AvailableMonths = new List<string>() { "Ianuarie", "Feburarie", "Martie", "Aprilie", "Mai", "Iunie", "Iulie", "August", "Septembrie", "Octombrie", "Noiembrie", "Decembrie" };
        public static List<int> AvailableYears;
        public static string CoinTypeLei = "LEI";
        public static int InfiniteLoopThreshold = 700;

        public static string LastSetupValueKey = "LastSetupValue";
        public static string LastSetupValueEntireScreenKey = "LastSetupValueEntireScreen";
        public static string IndicatorVisibilityKey = "IndicatorVisibility";
        public static string IndicatorRelationshipsKey = "IndicatorRelationships";

        public static string RulesText = "";

        public static int ValabilityDays = 30;

        public static int RemainingDays { get; set; }
        public static string KeyFilename = "C86D883A-7B72-4F49-9871-4EBA439036F9.tc";
        public static string SerialKeyFilename = "C86D883A-7B72-4F49-9871-4EBA439036F8.tc";
        public static string RulesFileName = "";
        public static string DocumentationFileName = "";
    }
}
