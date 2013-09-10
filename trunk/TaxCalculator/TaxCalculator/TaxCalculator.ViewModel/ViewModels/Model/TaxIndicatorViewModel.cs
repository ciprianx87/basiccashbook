using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Data.Model;
using System.Windows;

namespace TaxCalculator.ViewModel.ViewModels.Model
{
    public class TaxIndicatorViewModel
    {
        public int NrCrt { get; set; }
        public string NrCrtString { get; set; }
        public string Description { get; set; }
        public string TypeDescription { get; set; }
        public string IndicatorFormula { get; set; }
        public TaxIndicatorType Type { get; set; }

        public FontWeight FontWeight { get; set; }
        //public Style Style { get; set; }
        //public class Style
        //{
            
        //}

        public static TaxIndicatorViewModel FromTaxIndicator(TaxIndicator ti)
        {
            
            return new TaxIndicatorViewModel() { };
        }
    }
}
