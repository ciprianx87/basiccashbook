using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TaxCalculator.Data
{
    public class RulesHelper
    {
        public static string GetRulesText()
        {
            try
            {
                var allLines = File.ReadAllLines("Rules.txt");
                var fullText = string.Join(Environment.NewLine, allLines);
                return fullText;
            }
            catch
            {
            }
            return string.Empty;
        }
    }
}
