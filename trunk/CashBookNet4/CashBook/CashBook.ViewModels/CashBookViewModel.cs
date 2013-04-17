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
using System.ComponentModel;

namespace CashBook.ViewModels
{
    public class CashBookViewModel : BaseViewModel
    {
        ICashBookRepository cashBookRepository;
        ICashBookEntryRepository cashBookEntryRepository;

        public ICommand DeleteCommand { get; set; }
        public ICommand SaveCommand { get; set; }


        public CashBookViewModel()
        {
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
            SaveCommand = new DelegateCommand(Save, CanSave);

            this.LegalReglementationsCommand = new DelegateCommand(LegalReglementations, CanLegalReglementations);

            Mediator.Instance.Register(MediatorActionType.RefreshList, RefreshList);
            Mediator.Instance.Register(MediatorActionType.SetSelectedCashBook, SetSelectedCashBook);

            cashBookRepository = new CashBookRepository();
            cashBookEntryRepository = new CashBookEntryRepository();

            SelectedDate = DateTime.Now;
            canSave = true;
            // LoadData();
        }
        private bool canSave = false;
        private void LoadDataForDay(DateTime dateTime)
        {
            CashBookEntries = new ObservableCollection<CashBookEntry>();

            var existingCashBookEntries = cashBookEntryRepository.GetEntriesForDay(dateTime);
            if (existingCashBookEntries != null)
            {
                foreach (var item in existingCashBookEntries)
                {
                    CashBookEntries.Add(item);
                }
            }
            CashBookEntries.Add(new CashBookEntry());

            //AddFakeItems();
            CashBookEntries.CollectionChanged += CashBookEntries_CollectionChanged;
            //CashBookEntries.CollectionChanged += items_CollectionChanged;

        }

        private void AddFakeItems()
        {
            for (int i = CashBookEntries.Count; i < 100; i++)
            {
                CashBookEntries.Add(new CashBookEntry()
                {
                    NrCrt = i
                });
            }
        }

        void items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (INotifyPropertyChanged item in e.OldItems)
                item.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);

            foreach (INotifyPropertyChanged item in e.NewItems)
                item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("CashBookEntries");
        }

        void CashBookEntries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //foreach (INotifyPropertyChanged item in e.OldItems)
            //    item.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);

            //foreach (INotifyPropertyChanged item in e.NewItems)
            //    item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);

            //return;
            UpdateItemState();
            return;
            if (canSave)
            {
                //if all the items are valid, save them in the DB
                SaveData();
            }
        }
        private void UpdateItemState()
        {
            foreach (var item in CashBookEntries)
            {

            }
        }

        #region properties

        private CashBookEntry selectedItem;
        public CashBookEntry SelectedItem
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


        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (selectedIndex != value)
                {
                    selectedIndex = value;
                    this.NotifyPropertyChanged("SelectedIndex");
                }
            }
        }


        private DateTime selectedDate;
        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set
            {
                if (selectedDate != value)
                {
                    selectedDate = value;
                    this.NotifyPropertyChanged("SelectedDate");
                    LoadDataForDay(selectedDate);
                }
            }
        }

        private ObservableCollection<CashBookEntry> cashBookEntries;
        public ObservableCollection<CashBookEntry> CashBookEntries
        {
            get { return cashBookEntries; }
            set
            {
                cashBookEntries = value;
                NotifyPropertyChanged("CashBookEntries");
            }
        }


        private decimal totalBalance;
        public decimal TotalBalance
        {
            get { return totalBalance; }
            set
            {
                if (totalBalance != value)
                {
                    totalBalance = value;
                    this.NotifyPropertyChanged("TotalBalance");
                }
            }
        }


        private string totalBalanceString;
        public string TotalBalanceString
        {
            get { return totalBalanceString; }
            set
            {
                if (totalBalanceString != value)
                {
                    totalBalanceString = value;
                    this.NotifyPropertyChanged("TotalBalanceString");
                }
            }
        }


        #endregion

        #region methods
        public void AddNewItem()
        {
            //navigate to the next item or add a new one
            if (SelectedIndex < CashBookEntries.Count - 1)
            {
                SelectedIndex++;
            }
            else
            {
                CashBookEntries.Insert(CashBookEntries.Count, new CashBookEntry());
                SelectedIndex = CashBookEntries.Count - 1;
            }
        }

        public void RefreshList(object param)
        {
        }

        UserCashBook selectedCashBook;
        public void SetSelectedCashBook(object param)
        {
            selectedCashBook = param as UserCashBook;
        }

        public ICommand LegalReglementationsCommand { get; set; }

        private bool CanLegalReglementations(object parameter)
        {
            return true;
        }

        private void LegalReglementations(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.LegalReglementations);
        }

        public void SaveData()
        {
            try
            {
                var validEntries = ExtractValidItems(CashBookEntries);
                if (validEntries.Count > 0)
                {
                    cashBookEntryRepository.UpdateRepositoryForDay(selectedCashBook.Id, validEntries, SelectedDate);
                    WindowHelper.OpenInformationDialog("Informatia a fost salvata");
                }
            }
            catch (Exception ex)
            {
                WindowHelper.OpenErrorDialog("Eroare la salvarea informatiei");

            }
        }

        private List<CashBookEntry> ExtractValidItems(ObservableCollection<CashBookEntry> entries)
        {
            List<CashBookEntry> validEntries = new List<CashBookEntry>();
            foreach (var item in entries)
            {
                if (IsValid(item))
                {
                    validEntries.Add(item);
                }
            }
            return validEntries;
        }

        private bool IsValid(CashBookEntry item)
        {
            return !string.IsNullOrEmpty(item.Explicatii) && !string.IsNullOrEmpty(item.NrActCasa) && (item.Incasari != 0 || item.Plati != 0);
        }


        public bool CanSave(object param)
        {
            return true;
        }

        public void Save(object param)
        {
            SaveData();
        }

        public bool CanCreate(object param)
        {
            return true;
        }


        public void Delete(object param)
        {
            var itemToDelete = param as CashBookEntry;
            if (itemToDelete != null)
            {
                CashBookEntries.Remove(itemToDelete);
            }
        }

        public bool CanDelete(object param)
        {
            return true;
        }

        public override void Dispose()
        {
            CashBookEntries.CollectionChanged -= CashBookEntries_CollectionChanged;
            CashBookEntries.CollectionChanged -= items_CollectionChanged;
            Mediator.Instance.Unregister(MediatorActionType.RefreshList, RefreshList);
            Mediator.Instance.Unregister(MediatorActionType.SetSelectedCashBook, SetSelectedCashBook);
            base.Dispose();
        }

        #endregion

    }
}
