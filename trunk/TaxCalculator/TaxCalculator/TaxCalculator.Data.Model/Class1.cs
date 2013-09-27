using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxCalculator.Data.Model
{
    public class Class1
    {
    }

    public partial class TaxIndicator
    {
        public int? NrCrt { get; set; }
        public string NrCrtString { get; set; }
        public string Description { get; set; }
        public string TypeDescription { get; set; }
        public string IndicatorFormula { get; set; }
        public TaxIndicatorType Type { get; set; }

       
    }

    public enum TaxIndicatorType
    { 
        Numeric,
        Text,
        Calculat
    }
}
