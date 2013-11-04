
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
    public class ExistingTaxCalculationCompletionVm : BaseViewModel
    {
        public ICommand BackCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        IIndicatorRepository indicatorRepository;
        ITaxCalculationsRepository taxCalculationRepository;
        private TaxCalculationSetupModel setupModel;
        private byte nrDecimals = 0;
        public ExistingTaxCalculationCompletionVm()
        {
            Mediator.Instance.Register(MediatorActionType.ExecuteTaxCalculation, ExecuteTaxCalculation);
            Mediator.Instance.Register(MediatorActionType.SetSetupModel, SetSetupModel);
            this.SaveCommand = new DelegateCommand(Save, CanSave);
            this.BackCommand = new DelegateCommand(Back, CanBack);
            indicatorRepository = new IndicatorRepository();
            taxCalculationRepository = new TaxCalculationsRepository();
            Rectifying = Visibility.Collapsed;
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
                item.SetError("eroare la interpretarea formulei");
            }
            try
            {

                var newValueString = DecimalConvertor.Instance.DecimalToString(taxFormula.Execute(TaxIndicators.ToList()));
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

        private bool CanBack(object parameter)
        {
            return true;
        }

        private void Back(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.TaxCalculationSetup);
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
                        var taxIndicatorModelList = TaxIndicators.ToList().ToCompletedList();
                        //multiply with the exchange rate
                        //taxIndicatorModelList.ForEach(p =>
                        //{
                        //    if (!string.IsNullOrEmpty(p.Value))
                        //    {
                        //        var valueDec = DecimalConvertor.Instance.StringToDecimal(p.Value);
                        //        p.Value = DecimalConvertor.Instance.DecimalToString(valueDec * initialOtherData.ExchangeRate, initialOtherData.NrOfDecimals);
                        //    }
                        //}
                        //);
                        CompletedIndicatorDbModel contentModel = new CompletedIndicatorDbModel()
                           {
                               CompletedIndicators = taxIndicatorModelList,
                               PreviousIndicatorId = null
                           };
                        var content = VmUtils.SerializeEntity(contentModel);

                        taxCalculationRepository.UpdateContent(selectedTaxCalculationVm.Id, content);
                        WindowHelper.OpenInformationDialog(Messages.InfoWasSaved);
                        //ask the user if he wants version 2, if needed
                        //bool row57Completed = false;
                        //var row57 = GetRowByInnerId(TaxIndicators.ToList(), 57);
                        //if (row57 != null)
                        //{
                        //    row57Completed = DecimalConvertor.Instance.StringToDecimal(row57.ValueField) != 0;
                        //}
                        //if (row57Completed)
                        //{
                        //    //ask the user and wait for the response
                        //    Mediator.Instance.Register(MediatorActionType.YesNoPopupResponse, YesNoPopupResponseCallback);
                        //    Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.YesNoDialog);
                        //    Mediator.Instance.SendMessage(MediatorActionType.SetMessage, Messages.GenerateReportType2);
                        //}
                        //else
                        //{
                        //    //save with the selected name
                        //    isSecondTypeReport = false;
                        //    PerformSave();
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
            Action<string> saveAsCallBackAction = new Action<string>(SaveAsCallBack);
            Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.ChooseTaxCompletionName);
            Mediator.Instance.SendMessage(MediatorActionType.SetSaveAsCallBackAction, saveAsCallBackAction);
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

                    if (setupModel.ExchangeRate != 0 && setupModel.CoinType != Constants.CoinTypeLei)
                    {
                        SaveTaxCalculationCompletion(chosenName, setupModel.CoinType, setupModel.ExchangeRate, setupModel.NrOfDecimals);
                        SaveTaxCalculationCompletion(chosenName, Constants.CoinTypeLei, 1, setupModel.NrOfDecimals);
                    }
                    else
                    {
                        SaveTaxCalculationCompletion(chosenName, setupModel.CoinType, 1, setupModel.NrOfDecimals);
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

        private void SaveTaxCalculationCompletion(string chosenName, string coinType, decimal exchangeRate, byte nrOfDecimals)
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
                NrOfDecimals = setupModel.NrOfDecimals,
                Name = chosenName + " - " + coinType,
                Year = setupModel.Year,
                SecondTypeReport = isSecondTypeReport
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
        TaxCalculationsViewModel selectedTaxCalculationVm;
        TaxCalculationOtherData initialOtherData;
        public void SetSetupModel(object param)
        {
            try
            {
                selectedTaxCalculationVm = param as TaxCalculationsViewModel;
                var calculation = taxCalculationRepository.Get(selectedTaxCalculationVm.Id);
                var completedIndicatorDbModel = VmUtils.Deserialize<CompletedIndicatorDbModel>(calculation.Content);
                List<CompletedIndicatorVm> savedEntities = completedIndicatorDbModel.CompletedIndicators;
                initialOtherData = VmUtils.Deserialize<TaxCalculationOtherData>(calculation.OtherData);

                DecimalConvertor.Instance.SetNumberOfDecimals(initialOtherData.NrOfDecimals);
                var vmList = savedEntities.ToVmList();
                //load the previous values
                vmList.ForEach(p =>
                {
                    if (p.Type == TaxIndicatorType.Numeric)
                    {
                        p.InitialValueField = p.ValueField;
                        var valueDec = DecimalConvertor.Instance.StringToDecimal(p.ValueField);
                        p.ValueField = DecimalConvertor.Instance.DecimalToString(valueDec * initialOtherData.ExchangeRate, initialOtherData.NrOfDecimals);
                    }
                });
                TaxIndicators = new ObservableCollection<TaxIndicatorViewModel>(vmList);
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
            Mediator.Instance.Unregister(MediatorActionType.YesNoPopupResponse, YesNoPopupResponseCallback);
        }

    }
}

