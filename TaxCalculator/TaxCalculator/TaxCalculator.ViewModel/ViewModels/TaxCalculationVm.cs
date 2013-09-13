
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
    public class TaxCalculationVm : BaseViewModel
    {

        public TaxCalculationVm()
        {
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
                    Description = item.Description,
                    TypeDescription = item.TypeDescription,
                    IndicatorFormula = item.IndicatorFormula,
                    Type = GetIndicatorType(item.TypeDescription),
                    Style = GetStyleInfo(GetIndicatorType(item.TypeDescription))
                });
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

    }
}

