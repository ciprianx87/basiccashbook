using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxCalculator.Data.Model
{

    public partial class TaxIndicator
    {
        public int? NrCrt { get; set; }
        public string NrCrtString { get; set; }
        public string Description { get; set; }
        public string TypeDescription { get; set; }
        public string IndicatorFormula { get; set; }
        public TaxIndicatorType Type { get; set; }
        public string Value { get; set; }
    }

    public enum TaxIndicatorType
    {
        Numeric,
        Text,
        Calculat
    }

    public partial class TaxCalculationDbModel
    {
        public string NrCrtString { get; set; }
        public string Description { get; set; }
        public string TypeDescription { get; set; }
        public string IndicatorFormula { get; set; }
        public TaxIndicatorType Type { get; set; }
    }

    public class TaxCalculationOtherData
    {
        public string Name { get; set; }
        public string CoinType { get; set; }
        public decimal ExchangeRate { get; set; }
        public string Month { get; set; }
        public int NrOfDecimals { get; set; }
        public string CreatedBy { get; set; }
        public string VerifiedBy { get; set; }
        public int Year { get; set; }
    }
    public class TaxCalculationSelection
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
    }
   

    public enum ReportTaxIndicatorType
    {
        Numeric,
        Text,
        Calculat,

    }
}
