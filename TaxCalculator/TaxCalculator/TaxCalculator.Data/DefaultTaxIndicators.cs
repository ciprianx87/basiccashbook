using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TaxCalculator.Data.Model;

namespace TaxCalculator.Data
{

    public class DefaultTaxIndicators
    {
        public static List<TaxIndicator> GetDefaultIndicators()
        {
            try
            {
                List<TaxIndicator> taxIndicators = new List<TaxIndicator>();
                //read from file
                var allLines = File.ReadAllLines("DefaultTaxIndicators.txt");
                foreach (var curLine in allLines)
                {
                    if (string.IsNullOrEmpty(curLine))
                    {
                        //do not add any item if the line is empty
                    }
                    else
                    {
                        string line = curLine;
                        var quote1 = line.IndexOf('"');
                        if (quote1 >= 0)
                        {
                            var quote2 = line.IndexOf('"', quote1 + 1);
                            var strToReplace = line.Substring(quote1, quote2 - quote1 + 1);
                            var replaceValue = strToReplace.Replace(",", "@");
                            replaceValue = replaceValue.Trim();
                            replaceValue = replaceValue.Trim(new char[] { '"' });

                            line = line.Replace(strToReplace, replaceValue);

                        }
                        var parts = line.Split(new char[] { ',' });
                        var nr = parts[0];
                        var description = parts[1];
                        var type = parts[2];
                        var value = parts[3];
                        if (value == "..") { value = ""; }

                        int innerId = 0;
                        if (parts.Length == 5)
                        {
                            innerId = Convert.ToInt32(parts[4]);
                        }
                        int? nrCrt = null;
                        int normalNr = 0;
                        if (int.TryParse(nr, out normalNr))
                        {
                            nrCrt = normalNr;
                        }

                        description = description.Replace('@', ',');
                        TaxIndicator ti = new TaxIndicator()
                        {
                            NrCrt = nrCrt,
                            Description = description,
                            TypeDescription = type,
                            IndicatorFormula = value,
                            InnerId = innerId
                        };
                        taxIndicators.Add(ti);
                    }
                }
                return taxIndicators;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
