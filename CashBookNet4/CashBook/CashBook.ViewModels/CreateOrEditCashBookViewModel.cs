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

namespace CashBook.ViewModels
{
    public class CreateOrEditCashBookViewModel : BaseViewModel
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

        #endregion

        #region methods
        RegistruCasa currentEntity;
        public void SetEntityToEdit(object param)
        {
            if (param is RegistruCasa)
            {
                currentEntity = param as RegistruCasa;
                Account = currentEntity.Account;
                CashierName = currentEntity.CashierName;
                Location = currentEntity.Location;
                CoinType = currentEntity.CoinType;
                CoinDecimals = currentEntity.CoinDecimals;
                Name = currentEntity.Name;
                InitialBalance = currentEntity.InitialBalance;
            }
        }

        private void LoadData()
        {

        }

        private void UpdateFields()
        {

            currentEntity.Account = Account;
            currentEntity.CashierName = CashierName;
            currentEntity.Location = Location;
            currentEntity.CoinType = CoinType;
            currentEntity.CoinDecimals = CoinDecimals;
            currentEntity.Name = Name;
            currentEntity.InitialBalance = InitialBalance;
        }

        public void Save(object param)
        {
            try
            {
                //create
                if (currentEntity == null)
                {
                    RegistruCasa cashBook = new RegistruCasa()
                    {
                        Account = Account != null ? Account : "",
                        CashierName = CashierName != null ? CashierName : "",
                        Location = Location != null ? Location : "",
                        CoinType = CoinType,
                        CoinDecimals = CoinDecimals,
                        Name = Name != null ? Name : "",
                        InitialBalance = InitialBalance,
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
            catch (Exception ex)
            {

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

        public override void Dispose()
        {
            base.Dispose();
            Mediator.Instance.Unregister(MediatorActionType.SetEntityToEdit, SetEntityToEdit);

        }

        #endregion
    }
}