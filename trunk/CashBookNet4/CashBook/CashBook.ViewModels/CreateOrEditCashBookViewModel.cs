using CashBook.Data.Interfaces;
using CashBook.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CashBook.Common;
using CashBook.Data.Model;
using System.Collections.ObjectModel;
using CashBook.Common.Mediator;
using CashBook.Common.Exceptions;
using System.ComponentModel;
using CashBook.ViewModels.Models;

namespace CashBook.ViewModels
{
    public class CreateOrEditCashBookViewModel : BaseViewModel, IDataErrorInfo
    {
        ICashBookRepository cashBookRepository;
        ISettingsRepository settingsRepository;
        UserCashBook currentEntity;
        public ICommand CreateCoinTypeCommand { get; set; }
        public ICommand SaveCommand { get; set; }


        public CreateOrEditCashBookViewModel()
        {
            Mediator.Instance.Register(MediatorActionType.SetEntityToEdit, SetEntityToEdit);
            Mediator.Instance.Register(MediatorActionType.RefreshCoinTypes, RefreshCoinTypes);
            SaveCommand = new DelegateCommand(Save, CanSave);
            this.CancelCommand = new DelegateCommand(Cancel, CanCancel);
            this.CreateCoinTypeCommand = new DelegateCommand(CreateCoinType, CanCreateCoinType);

            cashBookRepository = new CashBookRepository();
            settingsRepository = new SettingsRepository();

            CoinDecimals = 2;
            LoadData();

        }

        #region properties

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        //public virtual string this[string columnName]
        //{
        //    get { throw new NotImplementedException(); }
        //}

        public string this[string columnName]
        {
            get
            {
                //if (columnName == "Name")
                //{
                //    if (string.IsNullOrEmpty(Name))
                //    {
                //        return "Camp obligatoriu!";
                //    }
                //    if (!IsNameLengthValid())
                //    {
                //        return "Lungimea numelui trebuie sa fie mai mica de " + AppSettings.CashRegistryNameCharacterLimit + " caractere!";
                //    }
                //}
                return null;

            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }


        private DateTime? initialBalanceDate;
        public DateTime? InitialBalanceDate
        {
            get { return initialBalanceDate; }
            set
            {
                if (initialBalanceDate != value)
                {
                    initialBalanceDate = value;
                    this.NotifyPropertyChanged("InitialBalanceDate");
                    if (initialBalanceDate.HasValue)
                    {
                        InitialBalanceDateString = Utils.DateTimeToStringDateOnly(initialBalanceDate.Value);
                    }
                }
            }
        }

        private string initialBalanceDateString;
        public string InitialBalanceDateString
        {
            get { return initialBalanceDateString; }
            set
            {
                if (initialBalanceDateString != value)
                {
                    initialBalanceDateString = value;
                    this.NotifyPropertyChanged("InitialBalanceDateString");
                }
            }
        }


        private string cashierName;
        public string CashierName
        {
            get { return cashierName; }
            set
            {
                cashierName = value;
                NotifyPropertyChanged("CashierName");
            }
        }

        private string location;
        public string Location
        {
            get { return location; }
            set
            {
                location = value;
                NotifyPropertyChanged("Location");
            }
        }

        private string account;
        public string Account
        {
            get { return account; }
            set
            {
                account = value;
                NotifyPropertyChanged("Account");
            }
        }

        private byte coinDecimals;
        public byte CoinDecimals
        {
            get { return coinDecimals; }
            set
            {
                coinDecimals = value;
                NotifyPropertyChanged("CoinDecimals");
            }
        }

        //private string coinType;
        //public string CoinType
        //{
        //    get { return coinType; }
        //    set
        //    {
        //        coinType = value;
        //        NotifyPropertyChanged("CoinType");
        //    }
        //}


        private byte selectedDecimal;
        public byte SelectedDecimal
        {
            get { return selectedDecimal; }
            set
            {
                if (selectedDecimal != value)
                {
                    selectedDecimal = value;
                    this.NotifyPropertyChanged("SelectedDecimal");
                }
            }
        }


        private ObservableCollection<byte> allowedDecimals;
        public ObservableCollection<byte> AllowedDecimals
        {
            get { return allowedDecimals; }
            set
            {
                if (allowedDecimals != value)
                {
                    allowedDecimals = value;
                    this.NotifyPropertyChanged("AllowedDecimals");
                }
            }
        }


        //private CashBookEntryUI cashBookEntry;
        //public CashBookEntryUI CashBookEntry
        //{
        //    get { return cashBookEntry; }
        //    set
        //    {
        //        if (cashBookEntry != value)
        //        {
        //            cashBookEntry = value;
        //            this.NotifyPropertyChanged("CashBookEntry");
        //        }
        //    }
        //}

        private string initialBalanceString;
        public string InitialBalanceString
        {
            get { return initialBalanceString; }
            set
            {
                if (initialBalanceString != value)
                {
                    initialBalanceString = value;

                    initialBalanceString = Utils.PrepareForConversion(value);
                    if (!string.IsNullOrEmpty(initialBalanceString))
                    {
                        InitialBalance = DecimalConvertor.Instance.StringToDecimal(initialBalanceString);
                    }
                    initialBalanceString = DecimalConvertor.Instance.DecimalToString(InitialBalance, SelectedDecimal);
                    this.NotifyPropertyChanged("InitialBalanceString");
                }
            }
        }

        private decimal initialBalance;
        public decimal InitialBalance
        {
            get { return initialBalance; }
            set
            {
                if (initialBalance != value)
                {
                    initialBalance = value;
                    this.NotifyPropertyChanged("InitialBalance");
                    InitialBalanceString = DecimalConvertor.Instance.DecimalToString(InitialBalance, SelectedDecimal);
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

        public void RefreshCoinTypes(object param)
        {
            string previouslySelectedCointType = SelectedCoinType;
            ExistingCoinTypes = new ObservableCollection<string>();
            VMUtils.ExtractCoinTypes(settingsRepository, ExistingCoinTypes);
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

        public void SetEntityToEdit(object param)
        {
            if (param is UserCashBook)
            {
                this.Title = "Editare registru de casa";
                currentEntity = param as UserCashBook;
                Account = currentEntity.Account;
                CashierName = currentEntity.CashierName;
                Location = currentEntity.Location;
                SelectedCoinType = currentEntity.CoinType;
                //CoinDecimals = currentEntity.CoinDecimals;
                Name = currentEntity.Name;
                SelectedDecimal = currentEntity.CoinDecimals;
                InitialBalance = currentEntity.InitialBalance;
                InitialBalanceDate = currentEntity.InitialBalanceDate;
                //SelectedCoinType = CoinType;
            }
            else
            {
                this.Title = "Creare registru de casa";
            }
        }

        private void LoadData()
        {
            AllowedDecimals = new ObservableCollection<byte>() { 2, 3, 4 };
            SelectedDecimal = AllowedDecimals[0];
            InitialBalanceDate = DateTime.Now;

            try
            {
                ExistingCoinTypes = new ObservableCollection<string>();
                VMUtils.ExtractCoinTypes(settingsRepository, ExistingCoinTypes);
                if (ExistingCoinTypes.Count > 0)
                {
                    SelectedCoinType = ExistingCoinTypes[0];
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }

        private void UpdateFields()
        {

            currentEntity.Account = Account;
            currentEntity.CashierName = CashierName;
            currentEntity.Location = Location;
            currentEntity.CoinType = SelectedCoinType;
            // currentEntity.CoinType = CoinType;
            currentEntity.CoinDecimals = SelectedDecimal;
            currentEntity.Name = Name;
            currentEntity.InitialBalance = InitialBalance;
            currentEntity.InitialBalanceDate = Utils.DateTimeToDay(InitialBalanceDate);
        }

        public void Save(object param)
        {
            try
            {
                if (IsFormValid())
                {
                    //create
                    if (currentEntity == null)
                    {
                        UserCashBook cashBook = new UserCashBook()
                        {
                            Account = Account != null ? Account : "",
                            CashierName = CashierName != null ? CashierName : "",
                            Location = Location != null ? Location : "",
                            CoinType = SelectedCoinType,
                            CoinDecimals = SelectedDecimal,
                            Name = Name != null ? Name : "",
                            InitialBalance = InitialBalance,
                            InitialBalanceDate = Utils.DateTimeToDay(InitialBalanceDate)
                        };
                        cashBookRepository.Create(cashBook);
                    }
                    //edit
                    else
                    {
                        UpdateFields();
                        cashBookRepository.Edit(currentEntity.Id, currentEntity);
                    }
                    Mediator.Instance.SendMessage(MediatorActionType.CloseWindow, this.Guid);
                    Mediator.Instance.SendMessage(MediatorActionType.RefreshList, this.Guid);
                }
                else
                {
                    //WindowHelper.OpenErrorDialog("Va rugam corectati erorile");

                }
            }
            catch (CompanyNotFoundException ce)
            {
                WindowHelper.OpenErrorDialog("Va rugam completati datele despre companie");
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog("Eroare la salvarea informatiei");
            }
        }

        private bool IsFormValid()
        {
            if (string.IsNullOrEmpty(Name))
            {
                WindowHelper.OpenInformationDialog("Numele este obligatoriu!");
                return false;
            }
            if (string.IsNullOrEmpty(SelectedCoinType))
            {
                WindowHelper.OpenInformationDialog("Moneda este camp obligatoriu!");
                return false;
            }
            decimal convertedBalance = 0;
            bool balanceOk = true;
            try
            {
                InitialBalance = DecimalConvertor.Instance.StringToDecimal(InitialBalanceString);
            }
            catch (Exception)
            {
                balanceOk = false;
            }

            if (!balanceOk)
            {
                WindowHelper.OpenInformationDialog("Soldul initial este invalid!");
                return false;
            }
            else if (InitialBalance < 0)
            {
                WindowHelper.OpenInformationDialog("Soldul initial nu poate fi negativ!");
                return false;
            }
            else
            {
                InitialBalance = InitialBalance;
            }
            if (!IsNameLengthValid())
            {
                WindowHelper.OpenErrorDialog(string.Format("Numarul de caractere al Numelui poate fi cel mult {0}", AppSettings.CashRegistryNameCharacterLimit));
                return false;
            }
            return true;
        }
        public bool CanSave(object param)
        {
            return true;
        }

        public ICommand CancelCommand { get; set; }

        private bool CanCancel(object parameter)
        {
            return true;
        }

        private void Cancel(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.CloseWindow, this.Guid);
        }

        private bool IsValid()
        {
            bool isValid = true;
            if (!IsNameLengthValid())
            {
                isValid = false;
            }
            return isValid;
        }

        private bool IsNameLengthValid()
        {
            return Name != null && Name.Length <= AppSettings.CashRegistryNameCharacterLimit;
        }


        public override void Dispose()
        {
            base.Dispose();
            Mediator.Instance.Unregister(MediatorActionType.SetEntityToEdit, SetEntityToEdit);
            Mediator.Instance.Unregister(MediatorActionType.RefreshCoinTypes, RefreshCoinTypes);

        }

        #endregion
    }
}
