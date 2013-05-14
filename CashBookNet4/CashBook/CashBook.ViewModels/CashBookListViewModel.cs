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
    public class CashBookListViewModel : BaseViewModel
    {
        ICashBookRepository cashBookRepository;

        public ICommand SaveCommand { get; set; }
        public ICommand CreateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand SelectCommand { get; set; }
        public CashBookListViewModel()
        {
            this.Title = "Registre de casa";
            SaveCommand = new DelegateCommand(Save, CanSave);
            CreateCommand = new DelegateCommand(Create, CanCreate);
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
            EditCommand = new DelegateCommand(Edit, CanEdit);
            SelectCommand = new DelegateCommand(Select, CanSelect);

            Mediator.Instance.Register(MediatorActionType.SetCashBookListType, SetCashBookListType);
            Mediator.Instance.Register(MediatorActionType.RefreshList, RefreshList);

            cashBookRepository = new CashBookRepository();
        }

        #region properties

        private ObservableCollection<UserCashBook> cashBooks;
        public ObservableCollection<UserCashBook> CashBooks
        {
            get { return cashBooks; }
            set
            {
                cashBooks = value;
                NotifyPropertyChanged("CashBooks");
            }
        }

        #endregion

        #region methods
        CashBookListType cashBookListType;
        public void SetCashBookListType(object param)
        {
            if (param is CashBookListType)
            {
                cashBookListType = (CashBookListType)param;
                LoadData();
            }
        }

        public void RefreshList(object param)
        {
            LoadData();
        }


        private void LoadData()
        {
            try
            {
                CashBooks = new ObservableCollection<UserCashBook>();
                var existingCashBooks = cashBookRepository.GetAll(cashBookListType);
                if (existingCashBooks != null)
                {
                    foreach (var item in existingCashBooks)
                    {
                        item.InitialBalanceDateString = item.InitialBalanceDate.HasValue ? Utils.DateTimeToStringDateOnly(item.InitialBalanceDate.Value) : "";
                        item.InitialBalanceString = DecimalConvertor.Instance.DecimalToString(item.InitialBalance);
                        CashBooks.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void Save(object param)
        {
            try
            {
            }
            catch (Exception ex)
            {

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
                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.CreateOrEditCashBook);
                Mediator.Instance.SendMessage(MediatorActionType.SetEntityToEdit, null);
            }
            catch (Exception ex)
            {

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
                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.CreateOrEditCashBook);
                Mediator.Instance.SendMessage(MediatorActionType.SetEntityToEdit, param);
            }
            catch (Exception ex)
            {

            }
        }

        public bool CanSelect(object param)
        {
            return true;
        }

        public void Select(object param)
        {
            Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.CashBook);
            Mediator.Instance.SendMessage(MediatorActionType.SetSelectedCashBook, param);
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
                WindowHelper.OpenErrorDialog("Registrul nu a putut fi sters");
            }
        }

        public bool CanDelete(object param)
        {
            return true;
        }

        public override void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.RefreshList, RefreshList);
            Mediator.Instance.Unregister(MediatorActionType.SetCashBookListType, SetCashBookListType);
            base.Dispose();
        }

        #endregion

    }
}
