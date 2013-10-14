
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
        ITaxCalculationsRepository taxCalculationRepository;
        private TaxCalculationSetupModel setupModel;
        public TaxCalculationCompletionVm()
        {
            Mediator.Instance.Register(MediatorActionType.ExecuteTaxCalculation, ExecuteTaxCalculation);
            Mediator.Instance.Register(MediatorActionType.SetSetupModel, SetSetupModel);
            this.SaveCommand = new DelegateCommand(Save, CanSave);

            indicatorRepository = new IndicatorRepository();
            taxCalculationRepository = new TaxCalculationsRepository();
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
            try
            {
                if (TaxIndicators != null)
                {
                    //perform validation

                    //save with the selected name

                    Action<string> saveAsCallBackAction = new Action<string>(SaveAsCallBack);
                    Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.ChooseTaxCompletionName);
                    Mediator.Instance.SendMessage(MediatorActionType.SetSaveAsCallBackAction, saveAsCallBackAction);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.ErrorSavingInfo);
            }
        }
        private void SaveAsCallBack(string chosenName)
        {
            try
            {
                //canceled was pressed
                if (string.IsNullOrEmpty(chosenName))
                {

                }
                else
                {
                    var taxIndicatorModelList = TaxIndicators.ToList().ToModelList();
                    var content = VmUtils.SerializeEntity(taxIndicatorModelList);
                    TaxCalculationOtherData otherData = new TaxCalculationOtherData()
                    {
                        VerifiedBy = setupModel.VerifiedBy,
                        CreatedBy = setupModel.CreatedBy,
                        CoinType = setupModel.CoinType,
                        ExchangeRate = setupModel.ExchangeRate,
                        Month = setupModel.Month,
                        NrOfDecimals = setupModel.NrOfDecimals,
                        Name = chosenName
                    };
                    //save the data in the DB
                    TaxCalculations tc = new TaxCalculations()
                    {
                        CompanyId = setupModel.SelectedCompany.Id,
                        IndicatorId = setupModel.SelectedIndicatorList.Id,
                        Rectifying = setupModel.Rectifying,
                        Content = content,
                        OtherData = VmUtils.SerializeEntity(otherData)
                    };
                    taxCalculationRepository.Create(tc);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.ErrorSavingInfo);
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

