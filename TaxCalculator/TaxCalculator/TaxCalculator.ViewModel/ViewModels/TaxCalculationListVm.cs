﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.ViewModel.Base;
using System.Windows.Input;
using TaxCalculator.Common.Mediator;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using TaxCalculator.Common;
using TaxCalculator.Data.Model;
using TaxCalculator.Data.Repositories;
using TaxCalculator.Data.Interfaces;
using TaxCalculator.ViewModel.ViewModels.Model;
using TaxCalculator.ViewModel.Extensions;


namespace TaxCalculator.ViewModel.ViewModels
{
    public class TaxCalculationListVm : BaseViewModel
    {
        const string ALL = "Toate";
        ICompanyRepository companyRepository;
        ITaxCalculationsRepository taxCalculationRepository;
        DispatcherTimer dt;
        private bool isRectifying = false;

        public ICommand SaveCommand { get; set; }
        public ICommand CreateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand EditIndicatorsCommand { get; set; }
        public ICommand ViewCommand { get; set; }
        public ICommand FilterCommand { get; set; }
        public ICommand ClearFiltersCommand { get; set; }

        public ICommand ModifyCommand { get; set; }

        public ICommand ChangeReportNameCommand { get; set; }



        public TaxCalculationListVm(bool isRectifying)
        {
            this.isRectifying = isRectifying;
            this.Title = "";
            SaveCommand = new DelegateCommand(Save, CanSave);
            CreateCommand = new DelegateCommand(Create, CanCreate);
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
            EditCommand = new DelegateCommand(Edit, CanEdit);
            this.ModifyCommand = new DelegateCommand(Modify, CanModify);
            this.EditIndicatorsCommand = new DelegateCommand(EditIndicators, CanEditIndicators);
            this.ViewCommand = new DelegateCommand(View, CanView);
            this.FilterCommand = new DelegateCommand(Filter, CanFilter);
            this.ClearFiltersCommand = new DelegateCommand(ClearFilters, CanClearFilters);
            this.ChangeReportNameCommand = new DelegateCommand(ChangeReportName, CanChangeReportName);
            Mediator.Instance.Register(MediatorActionType.RefreshList, RefreshList);
            Mediator.Instance.Register(MediatorActionType.SetRemainingDays, SetRemainingDays);

            taxCalculationRepository = new TaxCalculationsRepository();
            companyRepository = new CompanyRepository();
            LoadInitialData();
            //if no company exists, do not continue loading data
            if (SelectedCompany == null)
            {
                return;
            }
            RefreshList(null);
            RemainingDays = CurrentRemainingDaysMessage;

        }

        private string remainingDays;
        public string RemainingDays
        {
            get { return remainingDays; }
            set
            {
                if (remainingDays != value)
                {
                    remainingDays = value;
                    this.NotifyPropertyChanged("RemainingDays");
                }
            }
        }

        public static string CurrentRemainingDaysMessage = "";
        public void SetRemainingDays(object param)
        {
            RemainingDays = "Zile ramase: " + param;
            CurrentRemainingDaysMessage = RemainingDays;
        }

        private void LoadInitialData()
        {
            Years = new ObservableCollection<string>(Constants.AvailableYears.Select(p => p.ToString()));
            Years.Insert(0, ALL);
            SelectedYear = Years[0];
            Months = new ObservableCollection<string>(Constants.AvailableMonths);
            Months.Insert(0, ALL);
            SelectedMonth = Months[0];
            Companies = new ObservableCollection<Company>(companyRepository.GetAll());
            if (Companies.Count > 0)
            {
                Companies.Insert(0, new Company() { Name = ALL });
                SelectedCompany = Companies[0];
            }
            else
            {
                //error message
                WindowHelper.OpenErrorDialog(Messages.Error_NoCompanies);
            }
        }

        #region properties


        private Company selectedCompany;
        public Company SelectedCompany
        {
            get { return selectedCompany; }
            set
            {
                if (selectedCompany != value)
                {
                    selectedCompany = value;
                    this.NotifyPropertyChanged("SelectedCompany");
                }
            }
        }


        private ObservableCollection<Company> companies;
        public ObservableCollection<Company> Companies
        {
            get { return companies; }
            set
            {
                if (companies != value)
                {
                    companies = value;
                    this.NotifyPropertyChanged("Companies");
                }
            }
        }


        private string selectedMonth;
        public string SelectedMonth
        {
            get { return selectedMonth; }
            set
            {
                if (selectedMonth != value)
                {
                    selectedMonth = value;
                    this.NotifyPropertyChanged("SelectedMonth");
                }
            }
        }

        private ObservableCollection<string> months;
        public ObservableCollection<string> Months
        {
            get { return months; }
            set
            {
                if (months != value)
                {
                    months = value;
                    this.NotifyPropertyChanged("Months");
                }
            }
        }

        private ObservableCollection<string> years;
        public ObservableCollection<string> Years
        {
            get { return years; }
            set
            {
                if (years != value)
                {
                    years = value;
                    this.NotifyPropertyChanged("Years");
                }
            }
        }


        private string selectedYear;
        public string SelectedYear
        {
            get { return selectedYear; }
            set
            {
                if (selectedYear != value)
                {
                    selectedYear = value;
                    this.NotifyPropertyChanged("SelectedYear");
                }
            }
        }
        private ObservableCollection<TaxCalculationsViewModel> taxCalculationList;
        public ObservableCollection<TaxCalculationsViewModel> TaxCalculationList
        {
            get { return taxCalculationList; }
            set
            {
                taxCalculationList = value;
                NotifyPropertyChanged("TaxCalculationList");
            }
        }


        private TaxCalculationsViewModel selectedTaxCalculation;
        public TaxCalculationsViewModel SelectedTaxCalculation
        {
            get { return selectedTaxCalculation; }
            set
            {
                if (selectedTaxCalculation != value)
                {
                    selectedTaxCalculation = value;
                    this.NotifyPropertyChanged("SelectedTaxCalculation");
                    ChooseNameEnabled = value != null ? true : false;
                }
            }
        }

        private bool chooseNameEnabled;
        public bool ChooseNameEnabled
        {
            get { return chooseNameEnabled; }
            set
            {
                if (chooseNameEnabled != value)
                {
                    chooseNameEnabled = value;
                    this.NotifyPropertyChanged("ChooseNameEnabled");
                }
            }
        }



        #endregion

        #region methods



        private bool CanClearFilters(object parameter)
        {
            return true;
        }

        private void ClearFilters(object parameter)
        {
            LoadData(false);

        }

        private bool CanFilter(object parameter)
        {
            return true;
        }

        private void Filter(object parameter)
        {
            LoadData(true);
        }
        private bool CanView(object parameter)
        {
            return true;
        }

        public void View(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.PrintPreview);
            Mediator.Instance.SendMessage(MediatorActionType.SetReportData, parameter);
        }

        private bool CanChangeReportName(object parameter)
        {
            return true;
        }
        TaxCalculationsViewModel selTaxCalc = null;
        private void ChangeReportName(object parameter)
        {
            if (SelectedTaxCalculation != null)
            {
                selTaxCalc = SelectedTaxCalculation;
                Action<string> saveAsCallBackAction = new Action<string>(SaveAsCallBack);
                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.ChooseTaxCompletionName);
                Mediator.Instance.SendMessage(MediatorActionType.SetSaveAsCallBackAction, saveAsCallBackAction);
                Mediator.Instance.SendMessage(MediatorActionType.NameChangeSetDefaultName, SelectedTaxCalculation.Name);
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
                    var currentTaxCalc = existingIndicators.FirstOrDefault(p => p.Id == selTaxCalc.Id);
                    TaxCalculationOtherData otherData = VmUtils.Deserialize<TaxCalculationOtherData>(currentTaxCalc.OtherData);
                    otherData.Name = chosenName;
                    var serializedOtherData = VmUtils.SerializeEntity(otherData);
                    taxCalculationRepository.UpdateOtherData(currentTaxCalc.Id, serializedOtherData);

                    RefreshList(null);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.ErrorSavingInfo);
            }
        }

        public void RefreshList(object param)
        {
            IsBusy = true;
            Dispatcher.CurrentDispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
            dt = new DispatcherTimer();
            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = TimeSpan.FromMilliseconds(500);
            dt.Start();
        }

        void dt_Tick(object sender, EventArgs e)
        {
            dt.Tick -= new EventHandler(dt_Tick);
            dt.Stop();
            LoadData(false);

        }

        List<TaxCalculations> existingIndicators = null;
        private void LoadData(bool useFilters)
        {
            try
            {
                TaxCalculationList = new ObservableCollection<TaxCalculationsViewModel>();
                existingIndicators = new List<TaxCalculations>();
                if (isRectifying)
                {
                    existingIndicators = taxCalculationRepository.GetAll().Where(p => p.Rectifying == isRectifying).ToList();
                }
                else
                {
                    existingIndicators = taxCalculationRepository.GetAll().ToList();
                }

                if (useFilters)
                {
                    ApplyFilter(existingIndicators);
                    if (existingIndicators == null || existingIndicators.Count == 0)
                    {
                        //inform the user that no reports exist
                        WindowHelper.OpenInformationDialog(Messages.Error_NoReportsForFilters);
                        return;
                    }
                }
                var existingIndicatorsVm = existingIndicators.ToVmList();
                if (existingIndicatorsVm != null)
                {
                    foreach (var item in existingIndicatorsVm)
                    {
                        TaxCalculationList.Add(item);
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.Error_LoadingData);
            }
            IsBusy = false;
        }

        private void ApplyFilter(List<TaxCalculations> existingIndicators)
        {
            foreach (var item in existingIndicators.ToArray())
            {
                TaxCalculationOtherData otherData = VmUtils.Deserialize<TaxCalculationOtherData>(item.OtherData);
                bool isValid = false;
                isValid = (otherData.Month == SelectedMonth || SelectedMonth == ALL) &&
                    (otherData.Year.ToString() == SelectedYear || SelectedYear == ALL) &&
                    (item.CompanyId == SelectedCompany.Id || SelectedCompany.Name == ALL);
                if (!isValid)
                {
                    existingIndicators.Remove(item);
                }
            }
        }

        private void ApplyFilter()
        {
            //TaxCalculations t;
            //t.
            //TaxCalculationList=TaxCalculationList.Where(p=>p.Month==SelectedMonth && p.
        }

        public void Save(object param)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);

            }
        }

        public bool CanSave(object param)
        {
            return true;
        }

        public void Create(object param)
        {
            try
            {
                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.CreateOrEditIndicator);
                Mediator.Instance.SendMessage(MediatorActionType.SetEntityToEdit, null);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }

        public bool CanEdit(object param)
        {
            return true;
        }
        public void Edit(object param)
        {
            try
            {
                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.CreateOrEditIndicator);
                Mediator.Instance.SendMessage(MediatorActionType.SetEntityToEdit, param);
            }
            catch (Exception ex)
            {

                Logger.Instance.LogException(ex);
            }
        }

        private bool CanModify(object parameter)
        {
            return true;
        }

        public void Modify(object parameter)
        {
            //Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.ExistingTaxCalculationCompletion);
            //Mediator.Instance.SendMessage(MediatorActionType.SetSetupModel, parameter);
            var paramCalculation = parameter as TaxCalculationsViewModel;
            var selectedCalculation = taxCalculationRepository.Get(paramCalculation.Id);
            TaxCalculationOtherData otherData = VmUtils.Deserialize<TaxCalculationOtherData>(selectedCalculation.OtherData);
            TaxCalculationSetupModel setupModel = new TaxCalculationSetupModel()
            {
                CoinType = otherData.CoinType,
                ExchangeRate = otherData.ExchangeRate,
                Month = otherData.Month,
                NrOfDecimals = (byte)otherData.NrOfDecimals,
                //Rectifying = selectedCalculation.Rectifying,
                //always treat this as rectifying=false when editing it
                Rectifying = false,
                SelectedCompany = selectedCalculation.Company,
                SelectedIndicatorList = selectedCalculation.Indicator,
                VerifiedBy = otherData.VerifiedBy,
                CreatedBy = otherData.CreatedBy,
                Year = otherData.Year,
                SelectedTaxCalculation = new TaxCalculationSelection() { Id = selectedCalculation.Id },
                CompletedTaxIndicatorId = selectedCalculation.Id,
                BackToReports = true
            };

            Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.TaxCalculationCompletion);
            Mediator.Instance.SendMessage(MediatorActionType.SetSetupModel, setupModel);
        }

        public bool CanCreate(object param)
        {
            return true;
        }


        public void Delete(object param)
        {
            try
            {
                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.DeleteDialog);
                Mediator.Instance.SendMessage(MediatorActionType.SetEntityToDelete, param);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.CannotDeleteEntity);
            }
        }

        public bool CanDelete(object param)
        {
            return true;
        }

        private bool CanEditIndicators(object parameter)
        {
            return true;
        }


        public void EditIndicators(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.EditIndicators);
            Mediator.Instance.SendMessage(MediatorActionType.SetTaxIndicatorToEditFormula, parameter);

        }

        public override void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.RefreshList, RefreshList);
            Mediator.Instance.Unregister(MediatorActionType.SetRemainingDays, SetRemainingDays);
            base.Dispose();
        }

        #endregion

    }
}
