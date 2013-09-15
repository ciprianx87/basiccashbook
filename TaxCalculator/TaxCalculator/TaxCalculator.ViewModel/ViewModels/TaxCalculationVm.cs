
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
using TaxCalculator.Common.Mediator;

namespace TaxCalculator.ViewModel.ViewModels
{
    public class TaxCalculationVm : BaseViewModel
    {

        public TaxCalculationVm()
        {
            Mediator.Instance.Register(MediatorActionType.ExecuteTaxCalculation, ExecuteTaxCalculation);
            TaxIndicators = new ObservableCollection<TaxIndicatorViewModel>()
            {
                new TaxIndicatorViewModel(){ NrCrt=1, Description="ind 1", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Normal},
                new TaxIndicatorViewModel(){ NrCrt=2, Description="ind 2", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Normal},
                new TaxIndicatorViewModel(){ NrCrt=22, Description="ind 3", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Title },
                new TaxIndicatorViewModel(){ NrCrt=3, Description="ind 3", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Normal},
                new TaxIndicatorViewModel(){ NrCrt=4, Description="ind 4", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Normal},
                new TaxIndicatorViewModel(){ NrCrt=4, Description="ind 4", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Total},
                new TaxIndicatorViewModel(){ NrCrt=5, Description="ind 5", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Normal}
            };

            taxIndicators.Clear();
            var defaultIndicators = DefaultTaxIndicators.GetDefaultIndicators();
            foreach (var item in defaultIndicators)
            {
                taxIndicators.Add(new TaxIndicatorViewModel()
                {
                    NrCrtString = item.NrCrtString,
                    NrCrt = string.IsNullOrEmpty(item.NrCrtString) ? 0 : Convert.ToInt32(item.NrCrtString),
                    Description = item.Description,
                    TypeDescription = item.TypeDescription,
                    IndicatorFormula = item.IndicatorFormula,
                    Type = GetIndicatorType(item.TypeDescription),
                    Style = GetStyleInfo(GetIndicatorType(item.TypeDescription)),
                    //ValueFieldNumeric=2
                });
            }
            Tests();
            
            ExecuteTaxCalculation(null);
        }
        private void Tests()
        {
            Tests t = new Tests();
            t.PerformTests(TaxIndicators.ToList());
        }

      
        public void ExecuteTaxCalculation(object param)
        {
            return;
            //execute this until all the values remain the same
            bool hasChanged = true;
            while (hasChanged)
            {
                hasChanged = false;
                var calculatedTaxIndicators = TaxIndicators.Where(p => p.Type == TaxIndicatorType.Calculat);
                foreach (var item in calculatedTaxIndicators)
                {
                    TaxFormula taxFormula = new TaxFormula(item.IndicatorFormula);
                    var newValue = taxFormula.Execute(TaxIndicators.ToList()).ToString();
                    if (item.ValueField != newValue)
                    {
                        hasChanged = true;
                    }
                    item.ValueField = newValue;
                }
            }
        }
        private TaxIndicatorViewModel.TaxIndicatorStyleInfo GetStyleInfo(TaxIndicatorType taxIndicatorType)
        {
            TaxIndicatorViewModel.TaxIndicatorStyleInfo styleInfo = new TaxIndicatorViewModel.TaxIndicatorStyleInfo();

            switch (taxIndicatorType)
            {
                case TaxIndicatorType.Normal:
                    break;
                case TaxIndicatorType.Total:
                    break;
                case TaxIndicatorType.Title:
                    break;
                case TaxIndicatorType.Numeric:
                    styleInfo.FontWeight = FontWeights.Normal;
                    styleInfo.FormulaFieldVisibility = Visibility.Collapsed;
                    styleInfo.ValueFieldVisibility = Visibility.Visible;
                    break;
                case TaxIndicatorType.Text:
                    styleInfo.FontWeight = FontWeights.Bold;
                    styleInfo.FormulaFieldVisibility = Visibility.Collapsed;
                    styleInfo.ValueFieldVisibility = Visibility.Collapsed;
                    break;
                case TaxIndicatorType.Calculat:
                    styleInfo.FontWeight = FontWeights.Bold;
                    styleInfo.FormulaFieldVisibility = Visibility.Visible;
                    styleInfo.ValueFieldVisibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
            return styleInfo;
        }

        private TaxIndicatorType GetIndicatorType(string type)
        {
            switch (type.ToLower())
            {
                case "numeric": return TaxIndicatorType.Numeric; break;
                case "text": return TaxIndicatorType.Text; break;
                case "calculat": return TaxIndicatorType.Calculat; break;
                default: throw new ArgumentException(type);
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
        public override void Dispose()
        {
            base.Dispose();
            Mediator.Instance.Unregister(MediatorActionType.ExecuteTaxCalculation, ExecuteTaxCalculation);

        }

    }
}

