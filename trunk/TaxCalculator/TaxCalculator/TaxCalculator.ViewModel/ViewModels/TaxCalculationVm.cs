
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
using TaxCalculator.Common;
using System.Windows.Input;

namespace TaxCalculator.ViewModel.ViewModels
{
    public class TaxCalculationVm : BaseViewModel
    {

        public ICommand AddBeforeCommand { get; set; }

        public TaxCalculationVm()
        {

            this.AddBeforeCommand = new DelegateCommand(AddBefore, CanAddBefore);
            Mediator.Instance.Register(MediatorActionType.ExecuteTaxCalculation, ExecuteTaxCalculation);
            TaxIndicators = new ObservableCollection<TaxIndicatorViewModel>();
            var defaultIndicators = DefaultTaxIndicators.GetDefaultIndicators();
            foreach (var item in defaultIndicators)
            {
                taxIndicators.Add(new TaxIndicatorViewModel()
                {
                    NrCrt = item.NrCrt,
                    Description = item.Description,
                    TypeDescription = item.TypeDescription,
                    IndicatorFormula = item.IndicatorFormula,
                    Type = GetIndicatorType(item.TypeDescription),
                    Style = GetStyleInfo(GetIndicatorType(item.TypeDescription)),
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
                if (item.Type == TaxIndicatorType.Calculat && string.IsNullOrEmpty(item.IndicatorFormula))
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

                //var newValue = taxFormula.Execute(TaxIndicators.ToList());//.ToString();
                var newValue = taxFormula.Execute(TaxIndicators.ToList()).ToString();
                //var newValueConverted = DecimalConvertor.Instance.DecimalToString(newValue);
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
                case "": return TaxIndicatorType.Calculat; break;
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


        private TaxIndicatorViewModel selectedItem;
        public TaxIndicatorViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    this.NotifyPropertyChanged("SelectedItem");
                }
            }
        }

        private bool CanAddBefore(object parameter)
        {
            return true;
        }

        private void AddBefore(object parameter)
        {
            //test for items with no number
            //test for corner cases , first, last position

            if (SelectedItem != null)
            {
                var indexOf = TaxIndicators.IndexOf(SelectedItem);

                var previousItems = TaxIndicators.Take(indexOf);
                var itemsToUpdate = TaxIndicators.Skip(indexOf).ToList();

                int maxNrCrt = 1;
                foreach (var item in previousItems.Reverse())
                {
                    if (item.NrCrt.HasValue)
                    {
                        maxNrCrt = item.NrCrt.Value + 1;
                        break;
                    }
                }

                TaxIndicators.Insert(indexOf, GetEmptyRow(maxNrCrt));
                foreach (var item in itemsToUpdate)
                {
                    if (item.NrCrt.HasValue)
                    {
                        item.NrCrt++;
                    }
                }
            }
        }

        private TaxIndicatorViewModel GetEmptyRow(int? id)
        {
            TaxIndicatorViewModel newItem = new TaxIndicatorViewModel();
            newItem.NrCrt = id;
            newItem.Type = TaxIndicatorType.Numeric;
            newItem.Description = "";
            newItem.IndicatorFormula = "";
            newItem.ValueFieldNumeric = 0;
            newItem.Style = GetStyleInfo(newItem.Type);
            return newItem;
        }

        public override void Dispose()
        {
            base.Dispose();
            Mediator.Instance.Unregister(MediatorActionType.ExecuteTaxCalculation, ExecuteTaxCalculation);

        }

    }
}

