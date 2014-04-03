using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.ViewModel.Base;
using System.Collections.ObjectModel;
using TaxCalculator.Data.Interfaces;
using TaxCalculator.Common;
using System.ComponentModel;
using TaxCalculator.Data.Repositories;
using System.Windows.Input;
using TaxCalculator.Common.Mediator;
using TaxCalculator.Data.Model;
using TaxCalculator.ViewModel.ViewModels.Model;
using System.Windows;
using Newtonsoft.Json;

namespace TaxCalculator.ViewModel.ViewModels
{
    public class TaxCalculationSetupVm : BaseViewModel, IDataErrorInfo
    {
        ISettingsRepository settingsRepository;
        ICompanyRepository companyRepository;
        IIndicatorRepository indicatorRepository;
        ITaxCalculationsRepository taxCalculationsRepository;
        public ICommand AddValuesCommand { get; set; }
        public ICommand CreateCoinTypeCommand { get; set; }

        private int exchangeRateDefaultNrOfDecimals = 2;
        public TaxCalculationSetupVm()
        {
            this.CreateCoinTypeCommand = new DelegateCommand(CreateCoinType, CanCreateCoinType);
            this.AddValuesCommand = new DelegateCommand(AddValues, CanAddValues);
            settingsRepository = new SettingsRepository();
            companyRepository = new CompanyRepository();
            indicatorRepository = new IndicatorRepository();
            taxCalculationsRepository = new TaxCalculationsRepository();

            Rectifying = false;
            LoadData();
        }

        #region properties


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

        private ObservableCollection<int> years;
        public ObservableCollection<int> Years
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


        private int selectedYear;
        public int SelectedYear
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



        private ObservableCollection<Indicator> taxIndicatorLists;
        public ObservableCollection<Indicator> TaxIndicatorLists
        {
            get { return taxIndicatorLists; }
            set
            {
                if (taxIndicatorLists != value)
                {
                    taxIndicatorLists = value;
                    this.NotifyPropertyChanged("TaxIndicatorLists");
                }
            }
        }


        private Indicator selectedIndicatorList;
        public Indicator SelectedIndicatorList
        {
            get { return selectedIndicatorList; }
            set
            {
                if (selectedIndicatorList != value)
                {
                    selectedIndicatorList = value;
                    this.NotifyPropertyChanged("SelectedIndicatorList");
                    LoadTaxCalculations();
                }
            }
        }


        private Company selectedCompany;
        public Company SelectedCompany
        {
            get { return selectedCompany; }
            set
            {
                //if (selectedCompany != value)
                {
                    selectedCompany = value;
                    this.NotifyPropertyChanged("SelectedCompany");
                    LoadTaxCalculations();
                    ExtractLastSetupValues();
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

        private ObservableCollection<byte> availableNrOfDecimals;
        public ObservableCollection<byte> AvailableNrOfDecimals
        {
            get { return availableNrOfDecimals; }
            set
            {
                if (availableNrOfDecimals != value)
                {
                    availableNrOfDecimals = value;
                    this.NotifyPropertyChanged("AvailableNrOfDecimals");
                }
            }
        }


        private byte selectedNrOfDecimals;
        public byte SelectedNrOfDecimals
        {
            get { return selectedNrOfDecimals; }
            set
            {
                if (selectedNrOfDecimals != value)
                {
                    selectedNrOfDecimals = value;
                    this.NotifyPropertyChanged("SelectedNrOfDecimals");
                }
            }
        }



        private string createdBy;
        public string CreatedBy
        {
            get { return createdBy; }
            set
            {
                if (createdBy != value)
                {
                    createdBy = value;
                    this.NotifyPropertyChanged("CreatedBy");
                }
            }
        }


        private string verifiedBy;
        public string VerifiedBy
        {
            get { return verifiedBy; }
            set
            {
                if (verifiedBy != value)
                {
                    verifiedBy = value;
                    this.NotifyPropertyChanged("VerifiedBy");
                }
            }
        }


        private bool rectifying;
        public bool Rectifying
        {
            get { return rectifying; }
            set
            {
                //if (rectifying != value)
                {
                    rectifying = value;
                    this.NotifyPropertyChanged("Rectifying");
                    TaxCalculationSelectionVisible = value ? Visibility.Visible : Visibility.Collapsed;
                    LoadTaxCalculations();
                }
            }
        }


        private ObservableCollection<TaxCalculationSelection> taxCalculationlist;
        public ObservableCollection<TaxCalculationSelection> TaxCalculationList
        {
            get { return taxCalculationlist; }
            set
            {
                if (taxCalculationlist != value)
                {
                    taxCalculationlist = value;
                    this.NotifyPropertyChanged("TaxCalculationList");
                }
            }
        }


        private TaxCalculationSelection selectedTaxCalculation;
        public TaxCalculationSelection SelectedTaxCalculation
        {
            get { return selectedTaxCalculation; }
            set
            {
                if (selectedTaxCalculation != value)
                {
                    selectedTaxCalculation = value;
                    this.NotifyPropertyChanged("SelectedTaxCalculation");
                }
            }
        }


        private Visibility taxCalculationSelectionVisible;
        public Visibility TaxCalculationSelectionVisible
        {
            get { return taxCalculationSelectionVisible; }
            set
            {
                if (taxCalculationSelectionVisible != value)
                {
                    taxCalculationSelectionVisible = value;
                    this.NotifyPropertyChanged("TaxCalculationSelectionVisible");
                }
            }
        }

        #endregion


        #region methods

        private bool CanCreateCoinType(object parameter)
        {
            return true;
        }

        private void CreateCoinType(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.CrudCoinTypes);

        }

        private void LoadData()
        {
            Years = new ObservableCollection<int>(Constants.AvailableYears);
            SelectedYear = Years[0];
            AvailableNrOfDecimals = new ObservableCollection<byte>() { 0, 1, 2 };
            Months = new ObservableCollection<string>(Constants.AvailableMonths);
            SelectedMonth = Months[0];
            try
            {
                var existingIndicators = indicatorRepository.GetAll();
                //do not show the hidden ones
                VmUtils.RemoveHiddenIndicators(existingIndicators);
                //load indicators list
                TaxIndicatorLists = new ObservableCollection<Indicator>(existingIndicators);
                if (TaxIndicatorLists.Count > 0)
                {
                    SelectIndicator();
                }
                else
                {
                    //error message
                    WindowHelper.OpenErrorDialog(Messages.Error_NoTaxIndicatorList);
                }

                //load companies
                Companies = new ObservableCollection<Company>(companyRepository.GetAll().OrderBy(p => p.Name));
                if (Companies.Count > 0)
                {
                    SelectedCompany = Companies[0];
                    ExtractLastSetupValuesEntireScreen();
                }
                else
                {
                    //error message
                    WindowHelper.OpenErrorDialog(Messages.Error_NoCompanies);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.Error_LoadingData);
            }
        }

        private void SelectIndicator()
        {
            SelectedIndicatorList = TaxIndicatorLists.FirstOrDefault(p => p.IsDefault);
            if (SelectedIndicatorList == null)
            {
                SelectedIndicatorList = TaxIndicatorLists[0];
            }
        }

        private void ExtractLastSetupValues()
        {
            try
            {
                if (selectedCompany != null)
                {
                    //automatically select the indicator
                    SelectIndicator();

                    CreatedBy = "";
                    VerifiedBy = "";

                    string settingKey = Constants.LastSetupValueKey + selectedCompany.Id;
                    //load created and verified by for company id
                    var setting = settingsRepository.GetSetting(settingKey);
                    if (setting != null)
                    {
                        LastSetupValue setupValue = VmUtils.Deserialize<LastSetupValue>(setting);
                        CreatedBy = setupValue.CreatedBy;
                        VerifiedBy = setupValue.VerifiedBy;
                        if (setupValue.IndicatorListId != 0)
                        {
                            var newSelectedIndicatorList = TaxIndicatorLists.FirstOrDefault(p => p.Id == setupValue.IndicatorListId);
                            if (newSelectedIndicatorList != null)
                            {
                                SelectedIndicatorList = newSelectedIndicatorList;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }


        private void ExtractLastSetupValuesEntireScreen()
        {
            try
            {
                if (selectedCompany != null)
                {
                    //automatically select the indicator
                    SelectIndicator();



                    string settingKey = Constants.LastSetupValueEntireScreenKey;
                    //load created and verified by for company id
                    var setting = settingsRepository.GetSetting(settingKey);
                    if (setting != null)
                    {
                        CreatedBy = "";
                        VerifiedBy = "";
                        SelectedMonth = "";
                        LastSetupValueEntireScreen setupValue = VmUtils.Deserialize<LastSetupValueEntireScreen>(setting);
                        CreatedBy = setupValue.CreatedBy;
                        VerifiedBy = setupValue.VerifiedBy;
                        SelectedMonth = setupValue.Month;
                        SelectedYear = setupValue.Year;
                        SelectedNrOfDecimals = setupValue.NrDecimals;

                        if (setupValue.CompanyId != 0)
                        {
                            SelectedCompany = Companies.FirstOrDefault(p => p.Id == setupValue.CompanyId);
                        }
                        if (setupValue.IndicatorListId != 0)
                        {
                            var newSelectedIndicatorList = TaxIndicatorLists.FirstOrDefault(p => p.Id == setupValue.IndicatorListId);
                            if (newSelectedIndicatorList != null)
                            {
                                SelectedIndicatorList = newSelectedIndicatorList;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }


        private void LoadTaxCalculations()
        {
            if (!Rectifying)
            {
                return;
            }
            try
            {
                TaxCalculationList = new ObservableCollection<TaxCalculationSelection>();
                //load existing tax calculations
                if (SelectedCompany != null && SelectedIndicatorList != null)
                {
                    //get the old versions of the indicator
                    var validIndicatorList = GetValidIndicatorList();

                    //List<TaxCalculations> existingTaxCalculations = taxCalculationsRepository.GetAll().Where(p => p.CompanyId == SelectedCompany.Id && p.IndicatorId == SelectedIndicatorList.Id).ToList();
                    List<TaxCalculations> existingTaxCalculations = taxCalculationsRepository.GetAll().Where(p => p.CompanyId == SelectedCompany.Id && validIndicatorList.Contains(p.IndicatorId)).ToList();
                    if (existingTaxCalculations != null && existingTaxCalculations.Count > 0)
                    {
                        foreach (var item in existingTaxCalculations)
                        {
                            TaxCalculationOtherData otherData = VmUtils.Deserialize<TaxCalculationOtherData>(item.OtherData);
                            if (otherData.Month == SelectedMonth && otherData.Year == SelectedYear)
                            {
                                TaxCalculationList.Add(new TaxCalculationSelection()
                                {
                                    Id = item.Id,
                                    Name = otherData.Name,
                                    InitialIndicatorId = item.IndicatorId
                                });
                            }
                        }
                        if (TaxCalculationList != null && TaxCalculationList.Count > 0)
                        {
                            SelectedTaxCalculation = TaxCalculationList[0];
                        }
                        else
                        {
                            WindowHelper.OpenErrorDialog(Messages.Error_NoTaxCalculationsSelection);
                        }
                    }
                    else
                    {
                        WindowHelper.OpenErrorDialog(Messages.Error_NoTaxCalculationsSelection);
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.Error_LoadingData);
            }
        }

        private List<long> GetValidIndicatorList()
        {
            List<long> validIndicatorList = new List<long>() { SelectedIndicatorList.Id };
            var existingInfoString = settingsRepository.GetSetting(Constants.IndicatorRelationshipsKey);
            if (!string.IsNullOrEmpty(existingInfoString))
            {
                IndicatorRelationships existingData = JsonConvert.DeserializeObject<IndicatorRelationships>(existingInfoString);
                var existingEntry =
                    existingData.IndicatorStructureRelationships.FirstOrDefault(p => p.Contains(SelectedIndicatorList.Id));
                if (existingEntry != null)
                {
                    validIndicatorList = existingEntry;
                }
            }
            return validIndicatorList;
        }

        private bool CanAddValues(object parameter)
        {
            return true;
        }

        private bool IsValid()
        {
            //CS: removed created and verified by mandatory field check
            return true;
        }

        private void AddValues(object parameter)
        {
            if (!IsValid())
            {
                WindowHelper.OpenErrorDialog(Messages.Error_MissingFields);
                return;
            }
            //verify if the company/indicatorl list /month/year combination already exist
            //get all the calculations for this company and indicator list
            if (!Rectifying)
            {
                var validIndicatorList = GetValidIndicatorList();
                //var existingCalculations = taxCalculationsRepository.GetAll().Where(p => p.CompanyId == SelectedCompany.Id && p.IndicatorId == SelectedIndicatorList.Id && !p.Rectifying).ToList();
                var existingCalculations = taxCalculationsRepository.GetAll().Where(p => p.CompanyId == SelectedCompany.Id && validIndicatorList.Contains(p.IndicatorId) && !p.Rectifying).ToList();
                if (existingCalculations != null && existingCalculations.Count > 0)
                {
                    foreach (var item in existingCalculations)
                    {
                        TaxCalculationOtherData otherData = VmUtils.Deserialize<TaxCalculationOtherData>(item.OtherData);
                        if (otherData.Month == SelectedMonth && otherData.Year == SelectedYear)
                        {
                            WindowHelper.OpenErrorDialog(Messages.Error_ExistingCalculationForSetup);
                            return;
                        }
                    }
                }
            }

            //save the last filled in values
            SaveLastSetupValues();

            //save all the last values
            SaveLastSetupValuesEntireScreen();
            Indicator oldIndicator = null;
            if (SelectedTaxCalculation != null)
            {
                oldIndicator = indicatorRepository.Get(SelectedTaxCalculation.InitialIndicatorId);
            }
            else
            {
                oldIndicator = SelectedIndicatorList;
            }
            //load the selected data and navigate to the fill-in screen
            TaxCalculationSetupModel setupModel = new TaxCalculationSetupModel()
            {
                //CS: removed coin type and exchange rate
                CoinType = "LEI",
                ExchangeRate = 0,
                Month = SelectedMonth,
                NrOfDecimals = SelectedNrOfDecimals,
                Rectifying = Rectifying,
                SelectedCompany = SelectedCompany,
                //SelectedIndicatorList = SelectedIndicatorList,
                SelectedIndicatorList = oldIndicator,
                VerifiedBy = VerifiedBy,
                CreatedBy = CreatedBy,
                Year = SelectedYear,
                SelectedTaxCalculation = SelectedTaxCalculation
            };

            Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.TaxCalculationCompletion);
            Mediator.Instance.SendMessage(MediatorActionType.SetSetupModel, setupModel);
        }

        private void SaveLastSetupValues()
        {
            try
            {
                if (selectedCompany != null)
                {
                    LastSetupValue setupValue = new LastSetupValue()
                    {
                        CreatedBy = CreatedBy,
                        VerifiedBy = VerifiedBy,
                        IndicatorListId = SelectedIndicatorList != null ? SelectedIndicatorList.Id : 0
                    };
                    string serializedSetupValue = VmUtils.SerializeEntity(setupValue);
                    string settingKey = Constants.LastSetupValueKey + selectedCompany.Id;
                    //save created and verified by for company id
                    settingsRepository.AddOrUpdateSetting(settingKey, serializedSetupValue);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }

        private void SaveLastSetupValuesEntireScreen()
        {
            try
            {

                if (selectedCompany != null)
                {
                    LastSetupValueEntireScreen setupValue = new LastSetupValueEntireScreen()
                    {
                        CreatedBy = CreatedBy,
                        VerifiedBy = VerifiedBy,
                        IndicatorListId = SelectedIndicatorList != null ? SelectedIndicatorList.Id : 0,
                        CompanyId = selectedCompany.Id,
                        Month = SelectedMonth,
                        Year = SelectedYear,
                        NrDecimals = SelectedNrOfDecimals,
                    };
                    string serializedSetupValue = VmUtils.SerializeEntity(setupValue);
                    string settingKey = Constants.LastSetupValueEntireScreenKey;
                    settingsRepository.AddOrUpdateSetting(settingKey, serializedSetupValue);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }
        #endregion

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get
            {
                string result = GetError(columnName);
                return result;
            }
        }
        private string GetError(string columnName)
        {
            string result = null;
            switch (columnName)
            {
                default:
                    break;
            }

            return result;
        }
    }
}
