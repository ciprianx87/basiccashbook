﻿using CashBook.Data.Interfaces;
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

namespace CashBook.ViewModels
{
    public class CreateOrEditCashBookViewModel : BaseViewModel, IDataErrorInfo

    {
        ICashBookRepository cashBookRepository;

        public ICommand SaveCommand { get; set; }
        public CreateOrEditCashBookViewModel()
        {
            Mediator.Instance.Register(MediatorActionType.SetEntityToEdit, SetEntityToEdit);
            SaveCommand = new DelegateCommand(Save, CanSave);
            this.CancelCommand = new DelegateCommand(Cancel, CanCancel);
            cashBookRepository = new CashBookRepository();

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

        public  string this[string columnName]
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

        private decimal initialBalance;
        public decimal InitialBalance
        {
            get { return initialBalance; }
            set
            {
                initialBalance = value;
                NotifyPropertyChanged("InitialBalance");
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

        private string coinType;
        public string CoinType
        {
            get { return coinType; }
            set
            {
                coinType = value;
                NotifyPropertyChanged("CoinType");
            }
        }


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

        #endregion

        #region methods
        UserCashBook currentEntity;
        public void SetEntityToEdit(object param)
        {
            if (param is UserCashBook)
            {
                this.Title = "Editare registru de casa";
                currentEntity = param as UserCashBook;
                Account = currentEntity.Account;
                CashierName = currentEntity.CashierName;
                Location = currentEntity.Location;
                CoinType = currentEntity.CoinType;
                //CoinDecimals = currentEntity.CoinDecimals;
                Name = currentEntity.Name;
                InitialBalance = currentEntity.InitialBalance;
                InitialBalanceDate = currentEntity.InitialBalanceDate;

                SelectedDecimal = currentEntity.CoinDecimals;
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
        }

        private void UpdateFields()
        {

            currentEntity.Account = Account;
            currentEntity.CashierName = CashierName;
            currentEntity.Location = Location;
            currentEntity.CoinType = CoinType;
            currentEntity.CoinDecimals = SelectedDecimal;
            currentEntity.Name = Name;
            currentEntity.InitialBalance = InitialBalance;
            currentEntity.InitialBalanceDate = InitialBalanceDate;
        }

        public void Save(object param)
        {
            try
            {
                //create
                if (currentEntity == null)
                {
                    UserCashBook cashBook = new UserCashBook()
                    {
                        Account = Account != null ? Account : "",
                        CashierName = CashierName != null ? CashierName : "",
                        Location = Location != null ? Location : "",
                        CoinType = CoinType,
                        CoinDecimals = SelectedDecimal,
                        Name = Name != null ? Name : "",
                        InitialBalance = InitialBalance,
                        InitialBalanceDate = InitialBalanceDate
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
            catch (CompanyNotFoundException ce)
            {
                WindowHelper.OpenErrorDialog("Va rugam completati datele despre companie");
            }
            catch (Exception ex)
            {
                WindowHelper.OpenErrorDialog("Eroare la salvarea informatiei");
            }
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
            return Name.Length <= AppSettings.CashRegistryNameCharacterLimit;
        }


        public override void Dispose()
        {
            base.Dispose();
            Mediator.Instance.Unregister(MediatorActionType.SetEntityToEdit, SetEntityToEdit);

        }

        #endregion
    }
}
