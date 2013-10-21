
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
        private byte nrDecimals = 0;
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
            if (TaxIndicators == null)
            {
                return;
            }
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

                var newValueString = DecimalConvertor.Instance.DecimalToString(taxFormula.Execute(TaxIndicators.ToList()), nrDecimals);
                //var newValue = taxFormula.Execute(TaxIndicators.ToList()).ToString();
                if (item.ValueField != newValueString)
                {
                    hasChanged = true;
                }
                item.ValueField = newValueString;
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
                    var taxIndicatorModelList = TaxIndicators.ToList().ToCompletedList();
                    var content = VmUtils.SerializeEntity(taxIndicatorModelList);
                    TaxCalculationOtherData otherData = new TaxCalculationOtherData()
                    {
                        VerifiedBy = setupModel.VerifiedBy,
                        CreatedBy = setupModel.CreatedBy,
                        CoinType = setupModel.CoinType,
                        ExchangeRate = setupModel.ExchangeRate,
                        Month = setupModel.Month,
                        NrOfDecimals = setupModel.NrOfDecimals,
                        Name = chosenName,
                        Year = setupModel.Year
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
                    WindowHelper.OpenInformationDialog(Messages.InfoWasSaved);

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
                nrDecimals = setupModel.NrOfDecimals;
                DecimalConvertor.Instance.SetNumberOfDecimals(nrDecimals);
                if (setupModel.Rectifying)
                {
                    if (setupModel.SelectedTaxCalculation != null)
                    {
                        //load the selected calculation
                        var selectedTaxCalculation = taxCalculationRepository.Get(setupModel.SelectedTaxCalculation.Id);
                        List<CompletedIndicatorVm> completedIndicatorList = VmUtils.Deserialize<List<CompletedIndicatorVm>>(selectedTaxCalculation.Content);
                        TaxIndicators = new ObservableCollection<TaxIndicatorViewModel>(completedIndicatorList.ToVmList());

                        //load the formulas from the indicators
                        var selectedIndicator = indicatorRepository.Get(selectedTaxCalculation.IndicatorId);
                        var dbIndicators = VmUtils.Deserialize<List<TaxIndicator>>(selectedIndicator.Content);
                        if (dbIndicators.Count != TaxIndicators.Count)
                        {
                            WindowHelper.OpenErrorDialog(Messages.Error_FormulaModifiedForRectification);
                        }
                        else
                        {
                            foreach (var indicator in dbIndicators)
                            {
                                //if (indicator.NrCrt != null)
                                {
                                    //var existingVmIndicator = TaxIndicators.FirstOrDefault(p => p.NrCrt == indicator.NrCrt);
                                    var existingVmIndicator = TaxIndicators[dbIndicators.IndexOf(indicator)];
                                    existingVmIndicator.IndicatorFormula = indicator.IndicatorFormula;
                                    existingVmIndicator.TypeDescription = indicator.TypeDescription;
                                    existingVmIndicator.Type = VmUtils.GetIndicatorType(indicator.TypeDescription);
                                    existingVmIndicator.Style = VmUtils.GetStyleInfo(VmUtils.GetIndicatorType(indicator.TypeDescription));
                                }
                            }
                            //also load other data (nr decimals etc)
                            ExecuteTaxCalculation(null);
                        }
                    }
                }
                else
                {
                    var selectedIndicatorList = indicatorRepository.Get(setupModel.SelectedIndicatorList.Id);
                    var dbIndicators = VmUtils.Deserialize<List<TaxIndicator>>(selectedIndicatorList.Content);
                    var vmList = dbIndicators.ToVmList();
                    vmList.ForEach(p =>
                    {
                        if (p.Type == TaxIndicatorType.Numeric)
                        {
                            p.ValueField = "0";
                        }
                    });
                    TaxIndicators = new ObservableCollection<TaxIndicatorViewModel>(vmList);
                    ExecuteTaxCalculation(null);
                }
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

