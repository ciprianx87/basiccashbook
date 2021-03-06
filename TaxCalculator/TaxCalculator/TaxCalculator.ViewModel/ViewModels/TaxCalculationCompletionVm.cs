﻿
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
        public ICommand BackCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        IIndicatorRepository indicatorRepository;
        ITaxCalculationsRepository taxCalculationRepository;



        private byte nrDecimals = 0;
        public TaxCalculationCompletionVm()
        {
            Mediator.Instance.Register(MediatorActionType.ExecuteTaxCalculation, ExecuteTaxCalculation);
            Mediator.Instance.Register(MediatorActionType.SetSetupModel, SetSetupModel);
            this.SaveCommand = new DelegateCommand(Save, CanSave);
            this.BackCommand = new DelegateCommand(Back, CanBack);
            indicatorRepository = new IndicatorRepository();
            taxCalculationRepository = new TaxCalculationsRepository();
            Rectifying = Visibility.Collapsed;
            ExchangeRateVisibility = Visibility.Collapsed;
        }


        private bool IsValid()
        {
            bool isValid = true;
            //return;
            //execute this until all the values remain the same
            foreach (var item in TaxIndicators)
            {
                bool currentItemValid = false;
                try
                {
                    DecimalConvertor.Instance.StringToDecimal(item.ValueField);
                }
                catch
                {
                    isValid = false;
                    break;
                }
            }
            return isValid;
        }

        public void ExecuteTaxCalculation(object param)
        {
            if (TaxIndicators == null)
            {
                return;
            }

            //return;
            //execute this until all the values remain the same
            int executionCounter = 0;
            bool hasChanged = true;
            while (hasChanged)
            {
                executionCounter++;
                if (executionCounter > Constants.InfiniteLoopThreshold)
                {
                    //probably got in an infinite loop
                    WindowHelper.OpenErrorDialog(Messages.Error_InfiniteLoopDetected);
                    break;
                }
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
                //item.SetError(Constants.RulesText);
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
                newValueString = newValueString.Replace(".", string.Empty);
                item.ValueField = newValueString;
            }
            catch (Exception ex)
            {
                item.SetError("formula invalida");
            }
            return hasChanged;
        }
        private TaxCalculationSetupModel setupModel;
        public TaxCalculationSetupModel SetupModel
        {
            get { return setupModel; }
            set
            {
                if (setupModel != value)
                {
                    setupModel = value;
                    this.NotifyPropertyChanged("SetupModel");
                }
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


        private Visibility rectifying;
        public Visibility Rectifying
        {
            get { return rectifying; }
            set
            {
                //if (rectifying != value)
                {
                    rectifying = value;
                    this.NotifyPropertyChanged("Rectifying");
                }
            }
        }


        private Visibility exchangeRateVisibility;
        public Visibility ExchangeRateVisibility
        {
            get { return exchangeRateVisibility; }
            set
            {
                if (exchangeRateVisibility != value)
                {
                    exchangeRateVisibility = value;
                    this.NotifyPropertyChanged("ExchangeRateVisibility");
                }
            }
        }

        private bool CanBack(object parameter)
        {
            return true;
        }

        private void Back(object parameter)
        {
            if (setupModel.BackToReports)
            {
                Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.TaxCalculationList);
            }
            else
            {
                Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.TaxCalculationSetup);
            }
        }

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
                    if (IsValid())
                    {
                        //always test for v2, in edit and create mode
                        //if (savedTaxCalculation == null)
                        {
                            //ask the user if he wants version 2, if needed
                            bool row69Completed = false;
                            var row69 = GetRowByInnerId(TaxIndicators.ToList(), 69);
                            if (row69 != null)
                            {
                                row69Completed = DecimalConvertor.Instance.StringToDecimal(row69.ValueField) != 0;
                            }
                            if (row69Completed)
                            {
                                //ask the user and wait for the response
                                Mediator.Instance.Register(MediatorActionType.YesNoPopupResponse, YesNoPopupResponseCallback);
                                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.YesNoDialog);
                                Mediator.Instance.SendMessage(MediatorActionType.SetMessage, Messages.GenerateReportType2);
                            }
                            else
                            {

                                //save with the selected name
                                isSecondTypeReport = false;
                                PerformSave();
                            }
                        }
                        //else
                        //{
                        //    //just update

                        //    SaveAsCallBack(savedChosenName);
                        //    //var taxIndicatorModelList = TaxIndicators.ToList().ToCompletedList();

                        //    //CompletedIndicatorDbModel contentModel = new CompletedIndicatorDbModel()
                        //    //{
                        //    //    CompletedIndicators = taxIndicatorModelList,
                        //    //    PreviousIndicatorId = null
                        //    //};
                        //    //var content = VmUtils.SerializeEntity(contentModel);

                        //    //taxCalculationRepository.UpdateContent(savedTaxCalculation.Id, content);
                        //    //WindowHelper.OpenInformationDialog(Messages.InfoWasSaved);
                        //}
                    }
                    else
                    {
                        WindowHelper.OpenErrorDialog(Messages.Error_InvalidFields);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.ErrorSavingInfo);
            }
        }

        private void PerformSave()
        {
            //if the current calculation is null,ask for the new name, else just save over the other
            if (savedTaxCalculation == null)
            {
                Action<string> saveAsCallBackAction = new Action<string>(SaveAsCallBack);
                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.ChooseTaxCompletionName);
                Mediator.Instance.SendMessage(MediatorActionType.SetSaveAsCallBackAction, saveAsCallBackAction);
            }
            else
            {
                TaxCalculationOtherData otherData = VmUtils.Deserialize<TaxCalculationOtherData>(savedTaxCalculation.OtherData);
                SaveAsCallBack(otherData.Name);
            }
        }
        private bool isSecondTypeReport = false;
        public void YesNoPopupResponseCallback(object param)
        {
            try
            {
                Mediator.Instance.Unregister(MediatorActionType.YesNoPopupResponse, YesNoPopupResponseCallback);
                switch ((YesNoPopupResponse)param)
                {
                    case YesNoPopupResponse.Yes:
                        //save with the selected name
                        //save as version2
                        isSecondTypeReport = true;
                        PerformSave();
                        break;
                    case YesNoPopupResponse.No:
                        //save only as version1
                        isSecondTypeReport = false;
                        PerformSave();
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.GenericError);
            }
        }

        private static TaxIndicatorViewModel GetRowByInnerId(List<TaxIndicatorViewModel> savedEntities, int rowInnerId)
        {
            var existingRow = savedEntities.FirstOrDefault(p => p.InnerId == rowInnerId);
            return existingRow;
        }
        string savedChosenName;
        private void SaveAsCallBack(string chosenName)
        {
            try
            {
                savedChosenName = chosenName;
                //canceled was pressed
                if (string.IsNullOrEmpty(savedChosenName))
                {

                }
                else
                {
                    //savedChosenName = savedChosenName + " - " + coinType;
                    if (setupModel.ExchangeRate != 0 && setupModel.CoinType != Constants.CoinTypeLei)
                    {
                        //SaveTaxCalculationCompletion(savedChosenName, setupModel.CoinType, setupModel.ExchangeRate, setupModel.NrOfDecimals);
                        SaveTaxCalculationCompletion(savedChosenName, Constants.CoinTypeLei, 1, nrDecimals);
                    }
                    else
                    {
                        SaveTaxCalculationCompletion(savedChosenName, setupModel.CoinType, 1, nrDecimals);
                    }

                    //add the tax calculation for the selected coin also if an exchange rate is provided
                    WindowHelper.OpenInformationDialog(Messages.InfoWasSaved);

                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.ErrorSavingInfo);
            }
        }

        private void SaveTaxCalculationCompletion(string chosenName, string coinType, decimal exchangeRate, int nrOfDecimals)
        {
            var taxIndicatorModelList = TaxIndicators.ToList().ToCompletedList();
            //multiply with the exchange rate
            taxIndicatorModelList.ForEach(p =>
            {
                if (!string.IsNullOrEmpty(p.Value))
                {
                    var valueDec = DecimalConvertor.Instance.StringToDecimal(p.Value);
                    p.Value = DecimalConvertor.Instance.DecimalToString(valueDec * exchangeRate, nrOfDecimals);
                }
            }
            );
            CompletedIndicatorDbModel contentModel = new CompletedIndicatorDbModel()
            {
                CompletedIndicators = taxIndicatorModelList,
                PreviousIndicatorId = null
            };
            if (setupModel.Rectifying)
            {
                contentModel.PreviousIndicatorId = setupModel.SelectedTaxCalculation.Id;
            }
            var content = VmUtils.SerializeEntity(contentModel);
            TaxCalculationOtherData otherData = new TaxCalculationOtherData()
            {
                VerifiedBy = setupModel.VerifiedBy,
                CreatedBy = setupModel.CreatedBy,
                CoinType = coinType,
                ExchangeRate = exchangeRate,
                Month = setupModel.Month,
                NrOfDecimals = nrOfDecimals,
                Name = chosenName,
                Year = setupModel.Year,
                SecondTypeReport = isSecondTypeReport,
                LastModifiedDate = DateTime.Now,
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
            if (savedTaxCalculation == null)
            {
                taxCalculationRepository.Create(tc);
                savedTaxCalculation = tc;
            }
            else
            {
                taxCalculationRepository.UpdateContent(savedTaxCalculation.Id, content, tc.OtherData);
            }
        }

        TaxCalculations savedTaxCalculation;
        public void SetSetupModel(object param)
        {
            try
            {
                SetupModel = param as TaxCalculationSetupModel;
                ExchangeRateVisibility = SetupModel.CoinType == "LEI" ? Visibility.Collapsed : Visibility.Visible;
                nrDecimals = setupModel.NrOfDecimals;
                DecimalConvertor.Instance.SetNumberOfDecimals(nrDecimals);
                Rectifying = setupModel.Rectifying ? Visibility.Visible : Visibility.Collapsed;
                //setupModel.Rectifying = false;
                if (setupModel.Rectifying)
                {
                    //load the old nr of decimals

                    //load the selected calculation
                    var selectedTaxCalculation = taxCalculationRepository.Get(setupModel.SelectedTaxCalculation.Id);
                    var otherData = VmUtils.Deserialize<TaxCalculationOtherData>(selectedTaxCalculation.OtherData);
                    nrDecimals = (byte)otherData.NrOfDecimals;
                    DecimalConvertor.Instance.SetNumberOfDecimals(nrDecimals);

                    var completeIndicatorDbModel = VmUtils.Deserialize<CompletedIndicatorDbModel>(selectedTaxCalculation.Content);
                    List<CompletedIndicatorVm> completedIndicatorList = completeIndicatorDbModel.CompletedIndicators;

                    var vmList = completedIndicatorList.ToVmList();
                    //load the previous values
                    vmList.ForEach(p =>
                    {
                        if (p.Type == TaxIndicatorType.Numeric)
                        {
                            //var initValue = DecimalConvertor.Instance.DecimalToString(DecimalConvertor.Instance.StringToDecimal(p.ValueField));
                            //initValue = initValue.Replace(".", string.Empty);
                            //p.InitialValueField = initValue;
                            p.InitialValueField = p.ValueField;
                            //p.ValueField = p.ValueField;
                        }
                        else if (p.Type == TaxIndicatorType.Calculat)
                        {
                            //var initValue = DecimalConvertor.Instance.DecimalToString(DecimalConvertor.Instance.StringToDecimal(p.ValueField));
                            //initValue = initValue.Replace(".", string.Empty);
                            //p.InitialValueField = initValue;
                            p.InitialValueField = p.ValueField;
                            //p.ValueField = p.ValueField;
                        }
                    });

                    TaxIndicators = new ObservableCollection<TaxIndicatorViewModel>(vmList);

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
                                //existingVmIndicator.InitialValueField = indicator.Value;
                            }
                        }
                        //also load other data (nr decimals etc)
                        ExecuteTaxCalculation(null);
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
                    //if a load was performed, load all the data
                    if (setupModel.CompletedTaxIndicatorId != 0)
                    {
                        var completed = taxCalculationRepository.Get(setupModel.CompletedTaxIndicatorId);
                        savedTaxCalculation = completed;
                        TaxCalculationOtherData otherData = VmUtils.Deserialize<TaxCalculationOtherData>(completed.OtherData);
                        savedChosenName = otherData.Name;
                        var completeIndicatorDbModel = VmUtils.Deserialize<CompletedIndicatorDbModel>(completed.Content);
                        vmList.ForEach(p =>
                        {
                            if (p.NrCrt.HasValue)
                            {
                                var existing = completeIndicatorDbModel.CompletedIndicators.First(i => i.NrCrt == p.NrCrt.Value.ToString());
                                if (existing != null)
                                {
                                    //existing.Value = existing.Value.Replace(",", string.Empty);
                                    existing.Value = existing.Value.Replace(".", string.Empty);
                                    p.ValueField = existing.Value;
                                }
                            }
                        });
                    }
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
            Mediator.Instance.Unregister(MediatorActionType.YesNoPopupResponse, YesNoPopupResponseCallback);
        }

    }
}

