
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
using System.Diagnostics;
using Microsoft.Win32;
using Newtonsoft.Json;

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

        public ICommand RulesCommand { get; set; }


        IIndicatorRepository indicatorRepository;
        ISettingsRepository settingsRepository;

        public EditIndicatorsVm()
        {
            this.AddBeforeCommand = new DelegateCommand(AddBefore, CanAddBefore);
            this.AddAfterCommand = new DelegateCommand(AddAfter, CanAddAfter);
            this.RemoveRowCommand = new DelegateCommand(RemoveRow, CanRemoveRow);
            this.ValidateCommand = new DelegateCommand(Validate, CanValidate);
            this.SaveCommand = new DelegateCommand(Save, CanSave);
            this.SaveAsCommand = new DelegateCommand(SaveAs, CanSaveAs);
            this.BackCommand = new DelegateCommand(Back, CanBack);
            this.RulesCommand = new DelegateCommand(Rules, CanRules);
            indicatorRepository = new IndicatorRepository();
            settingsRepository = new SettingsRepository();

            EditEnabled = true;
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

        private bool editEnabled;
        public bool EditEnabled
        {
            get { return editEnabled; }
            set
            {
                if (editEnabled != value)
                {
                    editEnabled = value;
                    this.NotifyPropertyChanged("EditEnabled");
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
                int maxNrCrt = 0;
                //maxNrCrt = GetMaxNrCrt(itemsToUpdate, maxNrCrt);
                var firstItem = itemsToUpdate.FirstOrDefault(p => p.NrCrt.HasValue);
                if (firstItem != null)
                {
                    maxNrCrt = firstItem.NrCrt.Value;
                    // UpdateItemsInFormulaDelete(maxNrCrt, -1);
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
                // UpdateItemsInFormula(maxNrCrt, 1);

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
                // UpdateItemsInFormula(maxNrCrt, 1);
            }
        }
        private void UpdateItemsInFormula(int startingNumber, int valueDif)
        {
            var calculatedIndicators = TaxIndicators.Where(p => p.Type == TaxIndicatorType.Calculat);
            int maxIntem = calculatedIndicators.Max(p => p.NrCrt).Value;
            int start = maxIntem;
            int end = startingNumber;
            int increment = -1;
            for (int i = start; i >= end; i = i + increment)
            {
                int nextNr = i;

                var itemToReplace = "rd." + nextNr;
                string nextItem = "rd." + (nextNr + valueDif);
                foreach (var item in calculatedIndicators)
                {
                    try
                    {
                        //get all the matches
                        //replace with new values
                        var formulaLower = item.IndicatorFormula.ToLower();
                        if (formulaLower.Contains(itemToReplace))
                        {
                            Debug.WriteLine(string.Format("replacing {0} with {1}", itemToReplace, nextItem));
                            item.IndicatorFormula = formulaLower.Replace(itemToReplace, nextItem);
                        }
                        else if (formulaLower.Contains(nextNr.ToString()))
                        {
                            string nextNrString = nextNr.ToString();

                            var noSpaces = formulaLower.Trim().Replace(" ", string.Empty);
                            int indexOf = noSpaces.IndexOf(nextNrString);
                            bool isValidNumber = true;
                            List<char> invalidChars = new List<char>() { '*', '%' };
                            if (indexOf > 0)
                            {
                                char prevChar = noSpaces[indexOf - 1];
                                if (invalidChars.Contains(prevChar) || char.IsNumber(prevChar))
                                {
                                    isValidNumber = false;
                                }
                            }
                            if (indexOf < noSpaces.Length - nextNrString.Length)
                            {
                                char nextChar = noSpaces[indexOf + nextNrString.Length];
                                if (invalidChars.Contains(nextChar) || char.IsNumber(nextChar))
                                {
                                    isValidNumber = false;
                                }
                            }
                            if (isValidNumber)
                            {
                                Debug.WriteLine(string.Format("replacing {0} with {1}", nextNr, (nextNr + valueDif)));

                                item.IndicatorFormula = formulaLower.Replace(nextNr.ToString(), (nextNr + valueDif).ToString());
                            }
                        }
                        //TaxFormula tf = new TaxFormula(item.IndicatorFormula);
                        //if (tf.FormulaType == FormulaType.Value)
                        //{
                        //    foreach (var param in tf.Params)
                        //    {
                        //        if (param.ParamType == ParamType.RowData)
                        //        {
                        //            var rowData = (param.ParamData as RowData);
                        //            if (rowData.NrCrt > startingNumber)
                        //            {
                        //                rowData.NrCrt++;
                        //            }
                        //        }
                        //    }
                        //}
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void UpdateItemsInFormulaDelete(int startingNumber, int valueDif)
        {
            var calculatedIndicators = TaxIndicators.Where(p => p.Type == TaxIndicatorType.Calculat);
            int maxIntem = calculatedIndicators.Max(p => p.NrCrt).Value;
            int start = maxIntem;
            int end = startingNumber;
            int increment = +1;
            for (int i = startingNumber; i < maxIntem; i = i + increment)
            {
                int nextNr = i;

                var itemToReplace = "rd." + nextNr;
                string nextItem = "rd." + (nextNr + valueDif);
                foreach (var item in calculatedIndicators)
                {
                    try
                    {
                        //get all the matches
                        //replace with new values
                        var formulaLower = item.IndicatorFormula.ToLower();
                        if (formulaLower.Contains(itemToReplace))
                        {
                            Debug.WriteLine(string.Format("replacing {0} with {1}", itemToReplace, nextItem));
                            item.IndicatorFormula = formulaLower.Replace(itemToReplace, nextItem);
                        }
                        else if (formulaLower.Contains(nextNr.ToString()))
                        {
                            string nextNrString = nextNr.ToString();

                            var noSpaces = formulaLower.Trim().Replace(" ", string.Empty);
                            int indexOf = noSpaces.IndexOf(nextNrString);
                            bool isValidNumber = true;
                            List<char> invalidChars = new List<char>() { '*', '%' };
                            if (indexOf > 0)
                            {
                                char prevChar = noSpaces[indexOf - 1];
                                if (invalidChars.Contains(prevChar) || char.IsNumber(prevChar))
                                {
                                    isValidNumber = false;
                                }
                            }
                            if (indexOf < noSpaces.Length - nextNrString.Length)
                            {
                                char nextChar = noSpaces[indexOf + nextNrString.Length];
                                if (invalidChars.Contains(nextChar) || char.IsNumber(nextChar))
                                {
                                    isValidNumber = false;
                                }
                            }
                            if (isValidNumber)
                            {
                                Debug.WriteLine(string.Format("replacing {0} with {1}", nextNr, (nextNr + valueDif)));
                                formulaLower = string.Format("{0}@{1}@{2}", formulaLower.Substring(0, indexOf), nextNrString, formulaLower.Substring(indexOf + nextNrString.Length, formulaLower.Length - indexOf + nextNrString.Length - 1));

                                //item.IndicatorFormula = formulaLower.Replace(nextNr.ToString(), (nextNr + valueDif).ToString());
                                item.IndicatorFormula = formulaLower.Replace("@" + nextNr.ToString() + "@", (nextNr + valueDif).ToString());
                            }
                        }
                        //TaxFormula tf = new TaxFormula(item.IndicatorFormula);
                        //if (tf.FormulaType == FormulaType.Value)
                        //{
                        //    foreach (var param in tf.Params)
                        //    {
                        //        if (param.ParamType == ParamType.RowData)
                        //        {
                        //            var rowData = (param.ParamData as RowData);
                        //            if (rowData.NrCrt > startingNumber)
                        //            {
                        //                rowData.NrCrt++;
                        //            }
                        //        }
                        //    }
                        //}
                    }
                    catch
                    {
                    }
                }
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
            if (!IsValid())
            {
                WindowHelper.OpenErrorDialog(Messages.Error_InvalidIndicatorsCannotSave);
                return;
            }
            try
            {
                PerformSave();
                WindowHelper.OpenInformationDialog(Messages.InfoWasSaved);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.ErrorSavingInfo);
            }
        }

        private void PerformSave()
        {
            var taxIndicatorModelList = TaxIndicators.ToList().ToModelList();
            currentTaxIndicator.Content = VmUtils.SerializeEntity(taxIndicatorModelList);
            long newId = indicatorRepository.EditWithHide(currentTaxIndicator);

            //update the setting with the visibility information
            var existingVisibilityInfoString = settingsRepository.GetSetting(Constants.IndicatorVisibilityKey);
            List<IndicatorVisibilityModel> existingData = new List<IndicatorVisibilityModel>();
            if (!string.IsNullOrEmpty(existingVisibilityInfoString))
            {
                existingData = JsonConvert.DeserializeObject<List<IndicatorVisibilityModel>>(existingVisibilityInfoString);
                var existingEntry = existingData.FirstOrDefault(p => p.IndicatorId == currentTaxIndicator.Id);
                if (existingEntry == null)
                {
                    existingData.Add(new IndicatorVisibilityModel() { IndicatorId = currentTaxIndicator.Id, Hidden = true });
                }
            }
            else
            {
                existingData.Add(new IndicatorVisibilityModel() { IndicatorId = currentTaxIndicator.Id, Hidden = true });
            }
            var serializedData = JsonConvert.SerializeObject(existingData);
            settingsRepository.AddOrUpdateSetting(Constants.IndicatorVisibilityKey, serializedData);

            UpdateSettings(currentTaxIndicator.Id, newId);

        }

        private static void UpdateSettings(long initialIndicatorId, long newIndicatorId)
        {
            ISettingsRepository settingsRepository = new SettingsRepository();
            var allSettings = settingsRepository.GetAll().ToList();
            var setupValueSettings = allSettings.Where(p => p.Key.StartsWith(Constants.LastSetupValueKey) && p.Key != Constants.LastSetupValueEntireScreenKey).ToList();
            foreach (var item in setupValueSettings)
            {
                LastSetupValue setupValue = VmUtils.Deserialize<LastSetupValue>(item.Value);
                if (setupValue.IndicatorListId == initialIndicatorId)
                {
                    setupValue.IndicatorListId = newIndicatorId;
                    string serializedSetupValue = VmUtils.SerializeEntity(setupValue);
                    settingsRepository.AddOrUpdateSetting(item.Key, serializedSetupValue);
                }
            }
        }

        private bool IsValid()
        {
            bool isValid = ExecuteTaxCalculation(null);
            if (isValid)
            {
                List<int> allowedIds = new List<int>() { 18, 19, 20, 34, 35, 36, 37, 38, 39 };
                bool hasErrors = TaxIndicators.Any(p => !string.IsNullOrEmpty(p.ErrorMessage) && (p.NrCrt.HasValue && !allowedIds.Contains(p.NrCrt.Value)));


                if (!hasErrors)
                {
                    //WindowHelper.OpenInformationDialog(Messages.ValidFormulas);
                    return true;
                }
                else
                {
                    return false;
                    //WindowHelper.OpenErrorDialog(Messages.Error_InvalidIndicatorsCannotSave);
                }
            }
            else
            {
                return false;
                //WindowHelper.OpenErrorDialog(Messages.Error_InvalidIndicatorsCannotSave);
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
            if (!IsValid())
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
            bool isValid = IsValid();
            if (isValid)
            {
                WindowHelper.OpenInformationDialog(Messages.ValidFormulas);
            }
            else
            {
                WindowHelper.OpenErrorDialog(Messages.Error_InvalidIndicatorsCannotSave);
            }
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
            taxFormula = new TaxFormula(item.IndicatorFormula);
            var newValueString = DecimalConvertor.Instance.DecimalToString(taxFormula.Execute(TaxIndicators.ToList()), 0);
            if (item.ValueField != newValueString)
            {
                hasChanged = true;
            }
            item.ValueField = newValueString;

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

        private bool CanRules(object parameter)
        {
            return true;
        }

        private void Rules(object parameter)
        {
            OpenWordDocument(Constants.RulesFileName);
        }

        private static void OpenWordDocument(string fileName)
        {
            try
            {
                using (var regWord = Registry.ClassesRoot.OpenSubKey("Word.Application"))
                {
                    if (regWord == null)
                    {
                        MessageBox.Show("Trebuie sa aveti instalat Office Word pentru a putea deschide acest document");
                    }
                    else
                    {
                        Process.Start(fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nu s-a putut deschide documentul");
                Logger.Instance.LogException(ex);
            }
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
                if (currentTaxIndicator.IsDefault)
                {
                    EditEnabled = false;
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

