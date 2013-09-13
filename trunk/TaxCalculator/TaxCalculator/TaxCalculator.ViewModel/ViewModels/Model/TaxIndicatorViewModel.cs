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

        public TaxIndicatorViewModel()
        {
            Style = new TaxIndicatorStyleInfo();
            Style.FontWeight = FontWeights.Normal;
            Style.FormulaFieldVisibility = Visibility.Visible;
            Style.ValueFieldVisibility = Visibility.Visible;
        }

        public int NrCrt { get; set; }
        public string NrCrtString { get; set; }
        public string Description { get; set; }
        public string TypeDescription { get; set; }
        public string IndicatorFormula { get; set; }
        public TaxIndicatorType Type { get; set; }

        private string valueField;

        public string ValueField
        {
            //get { return valueField; }
            get { return ValueFieldNumeric.ToString(); }
            set {
                valueField = value;
                try
                {
                    ValueFieldNumeric = Convert.ToInt32(value);
                }
                catch (Exception)
                {

                }
            }
        }

        public int ValueFieldNumeric { get; set; }

        public TaxIndicatorStyleInfo Style { get; set; }
        public class TaxIndicatorStyleInfo
        {
            public Visibility FormulaFieldVisibility { get; set; }
            public Visibility ValueFieldVisibility { get; set; }
            public FontWeight FontWeight { get; set; }
        }

        public static TaxIndicatorViewModel FromTaxIndicator(TaxIndicator ti)
        {

            return new TaxIndicatorViewModel() { };
        }
    }

}
