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

            Mediator.Instance.Register(MediatorActionType.RefreshCoinTypes, RefreshCoinTypes);
            this.CreateCoinTypeCommand = new DelegateCommand(CreateCoinType, CanCreateCoinType);
            this.AddValuesCommand = new DelegateCommand(AddValues, CanAddValues);
            settingsRepository = new SettingsRepository();
            companyRepository = new CompanyRepository();
            indicatorRepository = new IndicatorRepository();
            taxCalculationsRepository = new TaxCalculationsRepository();
            CreatedBy = "";
            VerifiedBy = "";
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
                if (selectedCompany != value)
                {
                    selectedCompany = value;
                    this.NotifyPropertyChanged("SelectedCompany");
                    LoadTaxCalculations();
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

        private ObservableCollection<string> existingCoinTypes;
        public ObservableCollection<string> ExistingCoinTypes
        {
            get { return existingCoinTypes; }
            set
            {
                if (existingCoinTypes != value)
                {
                    existingCoinTypes = value;
                    this.NotifyPropertyChanged("ExistingCoinTypes");
                }
            }
        }


        private string selectedCoinType;
        public string SelectedCoinType
        {
            get { return selectedCoinType; }
            set
            {
                if (selectedCoinType != value)
                {
                    selectedCoinType = value;
                    this.NotifyPropertyChanged("SelectedCoinType");
                }
            }
        }


        private string exchangeRateString;
        public string ExchangeRateString
        {
            get { return exchangeRateString; }
            set
            {
                if (exchangeRateString != value)
                {
                    exchangeRateString = value;

                    exchangeRateString = Utils.PrepareForConversion(value);
                    if (!string.IsNullOrEmpty(exchangeRateString))
                    {
                        ExchangeRate = DecimalConvertor.Instance.StringToDecimal(exchangeRateString);
                    }
                    exchangeRateString = DecimalConvertor.Instance.DecimalToString(ExchangeRate, exchangeRateDefaultNrOfDecimals);
                    this.NotifyPropertyChanged("ExchangeRateString");
                }
            }
        }

        private decimal exchangeRate;
        public decimal ExchangeRate
        {
            get { return exchangeRate; }
            set
            {
                if (value == 0)
                {
                    exchangeRate = 0;
                    ExchangeRateString = "0";
                }
                else if (exchangeRate != value)
                {
                    exchangeRate = value;
                    this.NotifyPropertyChanged("ExchangeRate");
                    ExchangeRateString = DecimalConvertor.Instance.DecimalToString(ExchangeRate, exchangeRateDefaultNrOfDecimals);
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

        public void RefreshCoinTypes(object param)
        {
            string previouslySelectedCointType = SelectedCoinType;
            ExistingCoinTypes = new ObservableCollection<string>();
            VmUtils.ExtractCoinTypes(settingsRepository, ExistingCoinTypes);
            if (ExistingCoinTypes.Count > 0)
            {
                if (ExistingCoinTypes.Contains(previouslySelectedCointType))
                {
                    SelectedCoinType = previouslySelectedCointType;
                }
                else
                {
                    SelectedCoinType = ExistingCoinTypes[0];
                }
            }
        }

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
            ExchangeRateString = "0";
            try
            {
                ExistingCoinTypes = new ObservableCollection<string>();
                VmUtils.ExtractCoinTypes(settingsRepository, ExistingCoinTypes);
                if (ExistingCoinTypes.Count > 0)
                {
                    SelectedCoinType = ExistingCoinTypes[0];
                }
                //load companies
                Companies = new ObservableCollection<Company>(companyRepository.GetAll().OrderBy(p => p.Name));
                if (Companies.Count > 0)
                {
                    SelectedCompany = Companies[0];
                }
                else
                {
                    //error message
                    WindowHelper.OpenErrorDialog(Messages.Error_NoCompanies);
                }

                //load indicators list
                TaxIndicatorLists = new ObservableCollection<Indicator>(indicatorRepository.GetAll());
                if (TaxIndicatorLists.Count > 0)
                {
                    SelectedIndicatorList = TaxIndicatorLists.FirstOrDefault(p => p.IsDefault);
                    if (SelectedIndicatorList == null)
                    {
                        SelectedIndicatorList = TaxIndicatorLists[0];
                    }
                }
                else
                {
                    //error message
                    WindowHelper.OpenErrorDialog(Messages.Error_NoTaxIndicatorList);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.Error_LoadingData);
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
                    List<TaxCalculations> existingTaxCalculations = taxCalculationsRepository.GetAll().Where(p => p.CompanyId == SelectedCompany.Id && p.IndicatorId == SelectedIndicatorList.Id && !p.Rectifying).ToList();
                    if (existingTaxCalculations != null && existingTaxCalculations.Count > 0)
                    {
                        foreach (var item in existingTaxCalculations)
                        {
                            TaxCalculationOtherData otherData = VmUtils.Deserialize<TaxCalculationOtherData>(item.OtherData);
                            TaxCalculationList.Add(new TaxCalculationSelection()
                            {
                                Id = item.Id,
                                Name = otherData.Name
                            });
                        }
                        SelectedTaxCalculation = TaxCalculationList[0];
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

        private bool CanAddValues(object parameter)
        {
            return true;
        }
        private bool IsValid()
        {
            return !string.IsNullOrEmpty(CreatedBy) && !string.IsNullOrEmpty(VerifiedBy);
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

            var existingCalculations = taxCalculationsRepository.GetAll().Where(p => p.CompanyId == SelectedCompany.Id && p.IndicatorId == SelectedIndicatorList.Id).ToList();
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
           
            //load the selected data and navigate to the fill-in screen
            TaxCalculationSetupModel setupModel = new TaxCalculationSetupModel()
            {
                CoinType = SelectedCoinType,
                ExchangeRate = ExchangeRate,
                Month = SelectedMonth,
                NrOfDecimals = SelectedNrOfDecimals,
                Rectifying = Rectifying,
                SelectedCompany = SelectedCompany,
                SelectedIndicatorList = SelectedIndicatorList,
                VerifiedBy = VerifiedBy,
                CreatedBy = CreatedBy,
                Year = SelectedYear,
                SelectedTaxCalculation = SelectedTaxCalculation
            };

            Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.TaxCalculationCompletion);
            Mediator.Instance.SendMessage(MediatorActionType.SetSetupModel, setupModel);
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
                case "CreatedBy":
                    if (string.IsNullOrWhiteSpace(CreatedBy))
                    {
                        result = "Camp obligatoriu";
                    }
                    break;
                case "VerifiedBy":
                    if (string.IsNullOrEmpty(VerifiedBy))
                    {
                        result = "Camp obligatoriu";
                    }
                    break;
                //case "TotalPayment":
                //    if (TotalPayment < 0)
                //    {
                //        result = "Campul trebuie sa fie pozitiv";
                //    }
                //    break;
                //case "DailyCashing":
                //    if (DailyCashing < 0 && DailyCashingActive)
                //    {
                //        result = "Campul trebuie sa fie pozitiv";
                //    }
                //    break;
                //case "TotalBalance":
                //    if (TotalBalance < 0 && TotalBalanceActive)
                //    {
                //        result = "Campul trebuie sa fie pozitiv";
                //    }
                //    break;
                //case "TotalCashing":
                //    if (TotalCashing < 0 && TotalCashingActive)
                //    {
                //        result = "Campul trebuie sa fie pozitiv";
                //    }
                //    break;
            }

            return result;
        }
    }
}
