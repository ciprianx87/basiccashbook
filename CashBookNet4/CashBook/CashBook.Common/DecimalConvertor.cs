using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace CashBook.Common
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
            return DecToString(number, NumberOfDecimals);
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
            return number.ToString(format, ci);
        }
    }
}
