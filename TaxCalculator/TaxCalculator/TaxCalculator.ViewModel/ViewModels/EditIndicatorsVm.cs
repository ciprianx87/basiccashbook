
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
using TaxCalculator.Data.Interfaces;
using TaxCalculator.Data.Repositories;
using TaxCalculator.ViewModel.Extensions;
using TaxCalculator.Common.Exceptions;

namespace TaxCalculator.ViewModel.ViewModels
{
    public class EditIndicatorsVm : BaseViewModel
    {

        public ICommand ValidateCommand { get; set; }
        public ICommand AddAfterCommand { get; set; }
        public ICommand RemoveRowCommand { get; set; }
        public ICommand AddBeforeCommand { get; set; }
        public ICommand SaveAsCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand BackCommand { get; set; }

        IIndicatorRepository indicatorRepository;

        public EditIndicatorsVm()
        {
            this.AddBeforeCommand = new DelegateCommand(AddBefore, CanAddBefore);
            this.AddAfterCommand = new DelegateCommand(AddAfter, CanAddAfter);
            this.RemoveRowCommand = new DelegateCommand(RemoveRow, CanRemoveRow);
            this.ValidateCommand = new DelegateCommand(Validate, CanValidate);
            this.SaveCommand = new DelegateCommand(Save, CanSave);
            this.SaveAsCommand = new DelegateCommand(SaveAs, CanSaveAs);
            this.BackCommand = new DelegateCommand(Back, CanBack);
            indicatorRepository = new IndicatorRepository();

            Mediator.Instance.Register(MediatorActionType.SetTaxIndicatorToEditFormula, SetTaxIndicatorToEditFormula);

            TaxIndicators = new ObservableCollection<TaxIndicatorViewModel>();

            LoadInitialData();
            //Tests();
        }

        private void AddDefaultIndicators()
        {
            var defaultIndicators = DefaultTaxIndicators.GetDefaultIndicators();
            TaxIndicators = new ObservableCollection<TaxIndicatorViewModel>(defaultIndicators.ToVmList());
        }

        private void Tests()
        {
            Tests t = new Tests();
            t.PerformTests(TaxIndicators.ToList());
        }



        private ObservableCollection<TaxIndicatorType> availableIndicatorTypes;
        public ObservableCollection<TaxIndicatorType> AvailableIndicatorTypes
        {
            get { return availableIndicatorTypes; }
            set
            {
                if (availableIndicatorTypes != value)
                {
                    availableIndicatorTypes = value;
                    this.NotifyPropertyChanged("AvailableIndicatorTypes");
                }
            }
        }


        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    this.NotifyPropertyChanged("Name");
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

        #region methods

        private bool CanRemoveRow(object parameter)
        {
            return true;
        }

        private void RemoveRow(object parameter)
        {
            if (SelectedItem != null)
            {
                var indexOf = TaxIndicators.IndexOf(SelectedItem);
                var itemsToUpdate = TaxIndicators.Skip(indexOf).ToList();

                TaxIndicators.Remove(SelectedItem);
                foreach (var item in itemsToUpdate)
                {
                    if (item.NrCrt.HasValue)
                    {
                        item.NrCrt--;
                    }
                }
            }
        }

        private bool CanAddAfter(object parameter)
        {
            return true;
        }

        private void AddAfter(object parameter)
        {
            if (SelectedItem != null)
            {
                var indexOf = TaxIndicators.IndexOf(SelectedItem);
                int maxNrCrt = GetMaxNrCrt();
                var itemsToUpdate = TaxIndicators.Skip(indexOf + 1).ToList();

                InsertAndUpdateRows(indexOf + 1, itemsToUpdate, maxNrCrt);
            }
        }

        public int GetMaxNrCrt()
        {
            if (SelectedItem == null)
            {
                return 0;
            }
            var indexOf = TaxIndicators.IndexOf(SelectedItem);
            var previousItems = TaxIndicators.Take(indexOf + 1);
            int maxNrCrt = 1;
            maxNrCrt = GetMaxNrCrt(previousItems, maxNrCrt);
            return maxNrCrt;
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
                int maxNrCrt = GetMaxNrCrt() - 1;
                var itemsToUpdate = TaxIndicators.Skip(indexOf).ToList();

                InsertAndUpdateRows(indexOf, itemsToUpdate, maxNrCrt);
            }
        }

        private void InsertAndUpdateRows(int indexOf, List<TaxIndicatorViewModel> itemsToUpdate, int maxNrCrt)
        {
            TaxIndicators.Insert(indexOf, GetEmptyRow(maxNrCrt));
            foreach (var item in itemsToUpdate)
            {
                if (item.NrCrt.HasValue)
                {
                    item.NrCrt++;
                }
            }
        }

        private static int GetMaxNrCrt(IEnumerable<TaxIndicatorViewModel> previousItems, int maxNrCrt)
        {
            foreach (var item in previousItems.Reverse())
            {
                if (item.NrCrt.HasValue)
                {
                    maxNrCrt = item.NrCrt.Value + 1;
                    break;
                }
            }
            return maxNrCrt;
        }

        private TaxIndicatorViewModel GetEmptyRow(int? id)
        {
            TaxIndicatorViewModel newItem = new TaxIndicatorViewModel();
            newItem.NrCrt = id;
            newItem.Type = TaxIndicatorType.Numeric;
            newItem.TypeDescription = newItem.Type.ToString();
            newItem.Description = "";
            newItem.IndicatorFormula = "";
            newItem.ValueFieldNumeric = 0;
            newItem.Style = VmUtils.GetStyleInfo(newItem.Type);
            return newItem;
        }

        private bool CanSave(object parameter)
        {
            return true;
        }

        private void Save(object parameter)
        {
            if (IsValid())
            {
                WindowHelper.OpenErrorDialog(Messages.Error_InvalidIndicatorsCannotSave);
                return;
            }
            try
            {
                var taxIndicatorModelList = TaxIndicators.ToList().ToModelList();
                currentTaxIndicator.Content = VmUtils.SerializeEntity(taxIndicatorModelList);
                indicatorRepository.Edit(currentTaxIndicator);
                WindowHelper.OpenInformationDialog(Messages.InfoWasSaved);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.ErrorSavingInfo);
            }
        }

        private bool IsValid()
        {
            bool isValid = ExecuteTaxCalculation(null);
            if (isValid)
            {
                bool hasErrors = TaxIndicators.Any(p => !string.IsNullOrEmpty(p.ErrorMessage));

                if (!hasErrors)
                {
                    WindowHelper.OpenInformationDialog(Messages.ValidFormulas);
                    return false;
                }
                else
                {
                    WindowHelper.OpenErrorDialog(Messages.Error_InvalidIndicatorsCannotSave);
                }
            }
            return isValid;
        }

        public void SaveAsCallBackAction(Indicator newIndicator)
        {
            try
            {
                //the new formula was created - callback
                //replace the current indicator with the new one
                currentTaxIndicator = newIndicator;
                //save the formula data
                var taxIndicatorModelList = TaxIndicators.ToList().ToModelList();
                currentTaxIndicator.Content = VmUtils.SerializeEntity(taxIndicatorModelList);
                indicatorRepository.Edit(currentTaxIndicator);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.ErrorSavingInfo);
            }
        }

        private bool CanSaveAs(object parameter)
        {
            return true;
        }

        private void SaveAs(object parameter)
        {
            if (IsValid())
            {
                WindowHelper.OpenErrorDialog(Messages.Error_InvalidIndicatorsCannotSave);
                return;
            }
            try
            {
                Action<Indicator> saveAsCallBackAction = new Action<Indicator>(SaveAsCallBackAction);
                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.CreateOrEditIndicator);
                Mediator.Instance.SendMessage(MediatorActionType.SetEntityToEdit, null);
                Mediator.Instance.SendMessage(MediatorActionType.SetSaveAsCallBackAction, saveAsCallBackAction);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }


        private bool CanValidate(object parameter)
        {
            return true;
        }

        private void Validate(object parameter)
        {
            IsValid();
            //var calculatedIndicators = TaxIndicators.Where(p => p.Type == TaxIndicatorType.Calculat).ToList();
            //foreach (var item in calculatedIndicators)
            //{

            //}
        }

        public bool ExecuteTaxCalculation(object param)
        {
            bool isValid = true;
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
                    isValid = false;
                    break;
                }
                hasChanged = false;
                var calculatedTaxIndicators = TaxIndicators.Where(p => p.Type == TaxIndicatorType.Calculat);
                foreach (var item in calculatedTaxIndicators)
                {
                    try
                    {
                        hasChanged = ExecuteTaxCalculation(hasChanged, item);
                    }
                    catch (IndicatorFormulaException ife)
                    {
                        item.SetError(ife.Error);
                        isValid = false;
                        break;
                    }
                    catch (Exception ex)
                    {
                        item.SetError(Messages.Error_ParsingFormula);
                        isValid = false;
                        break;
                    }
                }
            }
            return isValid;
        }


        private bool ExecuteTaxCalculation(bool hasChanged, TaxIndicatorViewModel item)
        {
            hasChanged = false;
            TaxFormula taxFormula = null;
            item.IsIndicatorValid();
            //try
            //{
            //if (item.Type == TaxIndicatorType.Calculat && string.IsNullOrEmpty(item.IndicatorFormula))
            //{
            //    item.SetError(Messages.Error_EmptyFormula);
            //    return hasChanged;
            //}
            //if (string.IsNullOrEmpty(item.Description))
            //{
            //    item.SetError(Messages.Error_EmptyIndicatorName);
            //    return hasChanged;
            //}
            taxFormula = new TaxFormula(item.IndicatorFormula);
            //}
            //catch (IndicatorFormulaException ife)
            //{
            //    item.SetError(ife.Error);
            //}
            //catch (Exception ex)
            //{
            //    item.SetError(Messages.Error_ParsingFormula);
            //}
            //try
            //{

            var newValueString = DecimalConvertor.Instance.DecimalToString(taxFormula.Execute(TaxIndicators.ToList()), 0);
            if (item.ValueField != newValueString)
            {
                hasChanged = true;
            }
            item.ValueField = newValueString;
            //}
            //catch (IndicatorFormulaException ife)
            //{
            //    item.SetError(ife.Error);
            //}
            //catch (Exception ex)
            //{
            //    item.SetError("formula invalida");
            //}
            return hasChanged;
        }
        private bool CanBack(object parameter)
        {
            return true;
        }

        private void Back(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.TaxIndicatorList);

        }

        Indicator currentTaxIndicator;
        public void SetTaxIndicatorToEditFormula(object param)
        {
            currentTaxIndicator = param as Indicator;
            if (currentTaxIndicator != null)
            {
                Name = currentTaxIndicator.Name;
                //CS: do not add the default ones, show error
                if (currentTaxIndicator.Content == null)
                {
                    WindowHelper.OpenErrorDialog(Messages.Error_LoadingData);
                    //add the default indicators if none is present
                    //AddDefaultIndicators();
                }
                else
                {
                    var dbIndicators = VmUtils.Deserialize<List<TaxIndicator>>(currentTaxIndicator.Content);
                    TaxIndicators = new ObservableCollection<TaxIndicatorViewModel>(dbIndicators.ToVmList());
                }
            }
        }



        private void LoadInitialData()
        {
            var availableIndicators = Enum.GetValues(typeof(TaxIndicatorType)).Cast<TaxIndicatorType>().ToList();
            AvailableIndicatorTypes = new ObservableCollection<TaxIndicatorType>(availableIndicators);
        }

        public override void Dispose()
        {
            base.Dispose();
            Mediator.Instance.Unregister(MediatorActionType.SetTaxIndicatorToEditFormula, SetTaxIndicatorToEditFormula);


        }
        #endregion

    }
}

