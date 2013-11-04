
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

namespace TaxCalculator.ViewModel.ViewModels
{
    public class TaxCalculationVm : BaseViewModel
    {

        public ICommand AddBeforeCommand { get; set; }
        public ICommand SaveAsCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand BackCommand { get; set; }

        IIndicatorRepository indicatorRepository;

        public TaxCalculationVm()
        {
            this.AddBeforeCommand = new DelegateCommand(AddBefore, CanAddBefore);
            this.SaveCommand = new DelegateCommand(Save, CanSave);
            this.SaveAsCommand = new DelegateCommand(SaveAs, CanSaveAs);
            this.BackCommand = new DelegateCommand(Back, CanBack);
            indicatorRepository = new IndicatorRepository();

            Mediator.Instance.Register(MediatorActionType.ExecuteTaxCalculation, ExecuteTaxCalculation);
            Mediator.Instance.Register(MediatorActionType.SetTaxIndicatorToEditFormula, SetTaxIndicatorToEditFormula);

            TaxIndicators = new ObservableCollection<TaxIndicatorViewModel>();

            //Tests();

            ExecuteTaxCalculation(null);
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
            newItem.Style = VmUtils.GetStyleInfo(newItem.Type);
            return newItem;
        }

        private bool CanSave(object parameter)
        {
            return true;
        }

        private void Save(object parameter)
        {
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

        }

        public override void Dispose()
        {
            base.Dispose();
            Mediator.Instance.Unregister(MediatorActionType.ExecuteTaxCalculation, ExecuteTaxCalculation);
            Mediator.Instance.Unregister(MediatorActionType.SetTaxIndicatorToEditFormula, SetTaxIndicatorToEditFormula);


        }
        #endregion

    }
}

