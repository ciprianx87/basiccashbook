using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.ViewModel.Base;
using System.Collections.ObjectModel;
using TaxCalculator.Data.Model;
using TaxCalculator.ViewModel.ViewModels.Model;
using System.Windows;
using TaxCalculator.Data;

namespace TaxCalculator.ViewModel.ViewModels
{
    public class TaxIndicatorListVm : BaseViewModel
    {

        public TaxIndicatorListVm()
        {
            //TaxIndicators = new ObservableCollection<TaxIndicatorViewModel>()
            //{
            //    new TaxIndicatorViewModel(){ NrCrt=1, Description="ind 1", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Normal},
            //    new TaxIndicatorViewModel(){ NrCrt=2, Description="ind 2", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Normal},
            //    new TaxIndicatorViewModel(){ NrCrt=22, Description="ind 3", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Title  },
            //    new TaxIndicatorViewModel(){ NrCrt=3, Description="ind 3", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Normal},
            //    new TaxIndicatorViewModel(){ NrCrt=4, Description="ind 4", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Normal},
            //    new TaxIndicatorViewModel(){ NrCrt=4, Description="ind 4", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Total },
            //    new TaxIndicatorViewModel(){ NrCrt=5, Description="ind 5", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Normal}
            //};

            taxIndicators.Clear();
            var defaultIndicators = DefaultTaxIndicators.GetDefaultIndicators();
            foreach (var item in defaultIndicators)
            {
                taxIndicators.Add(new TaxIndicatorViewModel()
                {
                    Description = item.Description,
                    TypeDescription = item.TypeDescription,
                    IndicatorFormula = item.IndicatorFormula
                });
            }
        }


        private ObservableCollection<TaxIndicatorViewModel> taxIndicators;
        public ObservableCollection<TaxIndicatorViewModel> TaxIndicators
        {
            get { return taxIndicators; }
            set
            {
                if (taxIndicators != value)
                {
                    taxIndicators = value;
                    this.NotifyPropertyChanged("TaxIndicators");
                }
            }
        }

    }
}
