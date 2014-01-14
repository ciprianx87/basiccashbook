using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace TaxCalculator.Common
{
    public sealed class DecimalConvertor
    {
        private static readonly object syncObj = new object();

        private static DecimalConvertor instance;
        public static DecimalConvertor Instance
        {
            get
            {
                lock (syncObj)
                {
                    if (instance == null)
                    {
                        instance = new DecimalConvertor();
                    }
                    return instance;
                }
            }
        }

        public int NumberOfDecimals { get; private set; }

        private DecimalConvertor()
        {

        }


        public void SetNumberOfDecimals(int nrDec)
        {
            NumberOfDecimals = nrDec;
        }

        public string DecimalToString(decimal number)
        {
            return DecToString(number, NumberOfDecimals);
        }

        public string DecimalToString(decimal number, int nrDecimals)
        {
            return DecToString(number, nrDecimals);
        }

        private static string DecToString(decimal number, int nrDecimals)
        {
            if (number == 0)
            {
                return "0";
            }
            string format = "0.";
            format = format.PadRight(nrDecimals + 2, '0');

            CultureInfo ci = new CultureInfo("ro-RO");
            format = "N" + nrDecimals;
            //CS removed this to allow formatting numbers with 0 decimals with thousands separator
            if (nrDecimals == 0)
            {
                format = "N1";
                //hack to allow 0 values for decimals to be used
                //round so that a number like this 21.9, will be turned into 22, and then the decimal part will be removed
                number = Math.Round(number, 0);
                var conv = number.ToString(format, ci);
                if (conv.Contains(','))
                {
                    conv = conv.Remove(conv.Length - 2, 2);
                }
                return conv;
                //var conv = string.Format(ci, "{0:n0}", number);
                //return conv;
                //format = "{0:0.0}";
            }
            var converted = number.ToString(format, ci);
            return converted;

            var index = converted.IndexOf('.');
            var previous = converted.Substring(0, index).ToList(); ;
            previous.Reverse();
            string result = "";
            while (previous.Count > 3)
            {
                var piece = previous.Take(3);
                result += string.Join("", piece.ToList() + ",");
            }
            result += converted.Substring(index, converted.Length - index);
            return result;
            return number.ToString(format, ci);
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public decimal StringToDecimal(string str)
        {
            CultureInfo ci = new CultureInfo("ro-RO");
            decimal dec = Convert.ToDecimal(str, ci);
            return dec;
        }
    }
}
