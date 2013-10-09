
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
using TaxCalculator.ViewModel.Extensions;
using TaxCalculator.Data.Repositories;
using TaxCalculator.Data.Interfaces;
using TaxCalculator.Common;

namespace TaxCalculator.ViewModel.ViewModels
{
    public class TaxCalculationCompletionVm : BaseViewModel
    {
        IIndicatorRepository indicatorRepository;
         
        private TaxCalculationSetupModel setupModel;
        public TaxCalculationCompletionVm()
        {
            Mediator.Instance.Register(MediatorActionType.ExecuteTaxCalculation, ExecuteTaxCalculation);
            Mediator.Instance.Register(MediatorActionType.SetSetupModel, SetSetupModel);
            this.SaveCommand = new DelegateCommand(Save, CanSave);

            indicatorRepository = new IndicatorRepository();
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
        public void SetSetupModel(object param)
        {
            try
            {
                setupModel = param as TaxCalculationSetupModel;
                var selectedIndicatorList = indicatorRepository.Get(setupModel.SelectedIndicatorList.Id);
                var dbIndicators = VmUtils.Deserialize<List<TaxIndicator>>(selectedIndicatorList.Content);
                TaxIndicators = new ObservableCollection<TaxIndicatorViewModel>(dbIndicators.ToVmList());
                ExecuteTaxCalculation(null);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log.Error(ex);
                WindowHelper.OpenErrorDialog(Messages.Error_LoadingData);
            }
        }
        public override void Dispose()
        {
            base.Dispose();
            Mediator.Instance.Unregister(MediatorActionType.ExecuteTaxCalculation, ExecuteTaxCalculation);
            Mediator.Instance.Unregister(MediatorActionType.SetSetupModel, SetSetupModel);
        }

    }
}

