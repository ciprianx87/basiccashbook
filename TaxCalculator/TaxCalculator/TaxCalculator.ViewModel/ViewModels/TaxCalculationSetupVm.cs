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

namespace TaxCalculator.ViewModel.ViewModels
{
    public class TaxCalculationSetupVm : BaseViewModel, IDataErrorInfo
    {
        ISettingsRepository settingsRepository;
        ICompanyRepository companyRepository;
        IIndicatorRepository indicatorRepository;
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

            CreatedBy = "";
            VerifiedBy = "";
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

        private ObservableCollection<int> availableNrOfDecimals;
        public ObservableCollection<int> AvailableNrOfDecimals
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


        private int selectedNrOfDecimals;
        public int SelectedNrOfDecimals
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
                if (rectifying != value)
                {
                    rectifying = value;
                    this.NotifyPropertyChanged("Rectifying");
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
            AvailableNrOfDecimals = new ObservableCollection<int>() { 0, 1, 2 };
            Months = new ObservableCollection<string>() { "Ianuarie", "Feburarie", "Martie", "Aprilie", "Mai", "Iunie", "Iulie", "August", "Septembrie", "Octombrie", "Noiembrie", "Decembrie" };
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
                Companies = new ObservableCollection<Company>(companyRepository.GetAll());
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
            }
        }

        private bool CanAddValues(object parameter)
        {
            return true;
        }

        private void AddValues(object parameter)
        {
            //load the selected data and navigate to the fill-in screen
            TaxCalculationSetupModel setupModel = new TaxCalculationSetupModel()
            {
                CoinType = SelectedCoinType,
                ExchangeRate = ExchangeRate,
                Month = SelectedMonth,
                NrOfDecimals = SelectedNrOfDecimals,
                Rectifying = Rectifying,
                SelectedCompany = SelectedCompany,
                SelectedIndicatorList = SelectedIndicatorList
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
