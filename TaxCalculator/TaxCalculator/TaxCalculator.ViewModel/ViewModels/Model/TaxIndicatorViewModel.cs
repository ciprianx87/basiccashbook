using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Data.Model;
using System.Windows;
using TaxCalculator.Common.Mediator;
using TaxCalculator.ViewModel.Base;
using TaxCalculator.Common;

namespace TaxCalculator.ViewModel.ViewModels.Model
{
    public class TaxIndicatorViewModel : NotificationPoperty
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
            set
            {
                try
                {
                    valueField = Utils.PrepareForConversion(value);
                    if (!string.IsNullOrEmpty(valueField))
                    {
                        ValueFieldNumeric = DecimalConvertor.Instance.StringToDecimal(valueField);
                    }
                    valueField = DecimalConvertor.Instance.DecimalToString(ValueFieldNumeric);

                    //ValueFieldNumeric = Convert.ToInt32(value);

                    //execute the function
                    if (Type != TaxIndicatorType.Calculat)
                    {
                        Mediator.Instance.SendMessage(MediatorActionType.ExecuteTaxCalculation, null);
                    }
                    NotifyPropertyChanged("ValueField");
                }
                catch (Exception)
                {

                }
            }
        }


        //private string test;
        //public string Test
        //{
        //    get { return test; }
        //    set
        //    {
        //        if (test != value)
        //        {
        //            test = value;
        //            this.NotifyPropertyChanged("Test");
        //        }
        //    }
        //}


        public decimal ValueFieldNumeric { get; set; }

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
