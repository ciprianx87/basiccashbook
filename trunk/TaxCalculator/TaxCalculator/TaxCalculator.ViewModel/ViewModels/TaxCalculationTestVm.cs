
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
using System.Windows.Input;

namespace TaxCalculator.ViewModel.ViewModels
{
    public class TaxCalculationTestVm : BaseViewModel
    {

        public TaxCalculationTestVm()
        {
            Mediator.Instance.Register(MediatorActionType.ExecuteTaxCalculation, ExecuteTaxCalculation);
            this.SaveCommand = new DelegateCommand(Save, CanSave); 
            
            List<TaxIndicator> testTaxIndicators = new List<TaxIndicator>()
            {
                  new TaxIndicator(){ NrCrtString="1", Description="ind 1", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Numeric},
                new TaxIndicator(){ NrCrtString="2", Description="ind 2", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Numeric},
                //new TaxIndicatorViewModel(){ NrCrt=22, Description="ind 3", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Title },
                new TaxIndicator(){ NrCrtString="3", Description="ind 3", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Numeric},
                new TaxIndicator(){ NrCrtString="4", Description="ind 4", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Numeric},
                //new TaxIndicatorViewModel(){ NrCrt=4, Description="ind 4", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Total},
                new TaxIndicator(){ NrCrtString="5",Description="ind 5", TypeDescription="Numeric", IndicatorFormula="..", Type= TaxIndicatorType.Numeric},
                new TaxIndicator(){NrCrtString="6", Description="FORMULA", TypeDescription="Calculat", IndicatorFormula="rd.2", Type= TaxIndicatorType.Calculat}
         
            };

            TaxIndicators = new ObservableCollection<TaxIndicatorViewModel>();
            var defaultIndicators = DefaultTaxIndicators.GetDefaultIndicators();
            foreach (var item in testTaxIndicators)
            {
                taxIndicators.Add(new TaxIndicatorViewModel()
                {
                    NrCrt = item.NrCrt,
                    Description = item.Description,
                    TypeDescription = item.TypeDescription,
                    IndicatorFormula = item.IndicatorFormula,
                    Type = GetIndicatorType(item.TypeDescription),
                    Style = GetStyleInfo(GetIndicatorType(item.TypeDescription)),
                    //ValueFieldNumeric=2
                });
            }
            //Tests();

            ExecuteTaxCalculation(null);
        }
        private void Tests()
        {
            Tests t = new Tests();
            t.PerformTests(TaxIndicators.ToList());
        }


        public void ExecuteTaxCalculation(object param)
        {
            //return;
            //execute this until all the values remain the same
            bool hasChanged = true;
            while (hasChanged)
            {
                hasChanged = false;
                var calculatedTaxIndicators = TaxIndicators.Where(p => p.Type == TaxIndicatorType.Calculat);
                foreach (var item in calculatedTaxIndicators)
                {
                    hasChanged = ExecuteTaxCalculation(hasChanged, item);
                }
            }
        }

        private bool ExecuteTaxCalculation(bool hasChanged, TaxIndicatorViewModel item)
        {
            TaxFormula taxFormula = null;
            try
            {
                if (string.IsNullOrEmpty(item.IndicatorFormula))
                {
                    item.SetError("formula goala");
                }
                taxFormula = new TaxFormula(item.IndicatorFormula);
            }
            catch (Exception ex)
            {
                item.SetError("eroare la interpretarea formulei");
            }
            try
            {

                var newValue = taxFormula.Execute(TaxIndicators.ToList()).ToString();
                if (item.ValueField != newValue)
                {
                    hasChanged = true;
                }
                item.ValueField = newValue;
            }
            catch (Exception ex)
            {
                item.SetError("formula invalida");
            }
            return hasChanged;
        }
        private TaxIndicatorViewModel.TaxIndicatorStyleInfo GetStyleInfo(TaxIndicatorType taxIndicatorType)
        {
            TaxIndicatorViewModel.TaxIndicatorStyleInfo styleInfo = new TaxIndicatorViewModel.TaxIndicatorStyleInfo();

            switch (taxIndicatorType)
            {

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


        public ICommand SaveCommand { get; set; }

        private bool CanSave(object parameter)
        {
            return true;
        }

        private void Save(object parameter)
        {
            if (TaxIndicators != null)
            {
                var calculatedTaxIndicators = TaxIndicators.Where(p => p.Type == TaxIndicatorType.Calculat);

                foreach (var item in calculatedTaxIndicators)
                {
                    ExecuteTaxCalculation(true, item);

                    //item.IsValid();
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

