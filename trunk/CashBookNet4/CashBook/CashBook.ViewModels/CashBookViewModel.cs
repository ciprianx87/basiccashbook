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
using System.ComponentModel;
using CashBook.ViewModels.Models;

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
            this.Title = "Registru de casa";
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
            SaveCommand = new DelegateCommand(Save, CanSave);

            this.LegalReglementationsCommand = new DelegateCommand(LegalReglementations, CanLegalReglementations);

            Mediator.Instance.Register(MediatorActionType.RefreshList, RefreshList);
            Mediator.Instance.Register(MediatorActionType.SetSelectedCashBook, SetSelectedCashBook);
            Mediator.Instance.Register(MediatorActionType.UpdateBalance, UpdateBalance);

            cashBookRepository = new CashBookRepository();
            cashBookEntryRepository = new CashBookEntryRepository();

            SelectedDate = DateTime.Now;
            canSave = true;
        }
        private bool canSave = false;
        private void LoadDataForDay(DateTime dateTime)
        {
            CashBookEntries = new ObservableCollection<CashBookEntryUI>();

            var existingCashBookEntries = cashBookEntryRepository.GetEntriesForDay(dateTime);
            if (existingCashBookEntries != null)
            {
                foreach (var item in existingCashBookEntries)
                {
                    CashBookEntries.Add((CashBookEntryUI)item);
                }
            }
            CashBookEntries.Add(new CashBookEntryUI());
            //CashBookEntries.CollectionChanged+=new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CashBookEntries_CollectionChanged);

            UpdateBalance(null);
            //AddFakeItems();
            UpdateItemState();

        }

        private void AddFakeItems()
        {
            for (int i = CashBookEntries.Count; i < 100; i++)
            {
                CashBookEntries.Add(new CashBookEntryUI()
                {
                    NrCrt = i
                });
            }
        }      

        void CashBookEntries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {           
            UpdateItemState();        
        }

        private void UpdateItemState()
        {
            var index = 0;
            foreach (var item in CashBookEntries)
            {
                item.NrCrt = ++index;
            }
        }

        #region properties

        private CashBookEntryUI selectedItem;
        public CashBookEntryUI SelectedItem
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

        private ObservableCollection<CashBookEntryUI> cashBookEntries;
        public ObservableCollection<CashBookEntryUI> CashBookEntries
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
                //if (totalBalance != value)
                {
                    totalBalance = value;
                    //this.NotifyPropertyChanged("TotalBalance");
                    UpdateStringValues();
                }
            }
        }


        private string totalBalanceString;
        public string TotalBalanceString
        {
            get { return totalBalanceString; }
            set
            {
                //if (totalBalanceString != value)
                {
                    totalBalanceString = value;
                    this.NotifyPropertyChanged("TotalBalanceString");
                }
            }
        }


        private string initialBalanceForDayString;
        public string InitialBalanceForDayString
        {
            get { return initialBalanceForDayString; }
            set
            {
                if (initialBalanceForDayString != value)
                {
                    initialBalanceForDayString = value;
                    this.NotifyPropertyChanged("InitialBalanceForDayString");
                }
            }
        }


        private UserCashBook selectedCashBook;
        public UserCashBook SelectedCashBook
        {
            get { return selectedCashBook; }
            set
            {
                if (selectedCashBook != value)
                {
                    selectedCashBook = value;
                    this.NotifyPropertyChanged("SelectedCashBook");
                }
            }
        }



        #endregion

        #region methods

        private void UpdateStringValues()
        {
            TotalBalanceString = TotalBalance.ToString("0.00");
        }

        public void AddNewItem()
        {
            //navigate to the next item or add a new one
            if (SelectedIndex < CashBookEntries.Count - 1)
            {
                SelectedIndex++;
            }
            else
            {
                CashBookEntries.Insert(CashBookEntries.Count, new CashBookEntryUI());
                SelectedIndex = CashBookEntries.Count - 1;
            }
        }

        public void RefreshList(object param)
        {
        }

        public void UpdateBalance(object param)
        {
            decimal totalSum = 0;
            CashBookEntries.ToList().ForEach(p => totalSum += p.Incasari);
            CashBookEntries.ToList().ForEach(p => totalSum -= p.Plati);

            if (SelectedCashBook != null)
            {
                var initialBalanceForDayDecimal = cashBookRepository.GetInitialBalanceForDay(SelectedCashBook.Id, SelectedDate);
                InitialBalanceForDayString = Utils.DecimalToString(initialBalanceForDayDecimal, 2);
                totalSum += initialBalanceForDayDecimal;
            }
            TotalBalance = totalSum;
        }

        public void SetSelectedCashBook(object param)
        {
            SelectedCashBook = param as UserCashBook;
            this.Title = "Editare Registru de casa (" + SelectedCashBook.Name + ")";

            UpdateBalance(null);
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
                    cashBookEntryRepository.UpdateRepositoryForDay(SelectedCashBook.Id, validEntries, SelectedDate);
                    WindowHelper.OpenInformationDialog("Informatia a fost salvata");
                }
            }
            catch (Exception ex)
            {
                WindowHelper.OpenErrorDialog("Eroare la salvarea informatiei");

            }
        }

        private List<CashBookEntry> ExtractValidItems(ObservableCollection<CashBookEntryUI> entries)
        {
            List<CashBookEntry> validEntries = new List<CashBookEntry>();
            foreach (var item in entries)
            {
                if (IsValid(item))
                {
                    validEntries.Add((CashBookEntry)item);
                }
            }
            return validEntries;
        }

        private bool IsValid(CashBookEntryUI item)
        {
            return !string.IsNullOrEmpty(item.Explicatii) && !string.IsNullOrEmpty(item.NrActCasa) && (item.Incasari != 0 && item.Plati == 0 || item.Incasari == 0 && item.Plati != 0);
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
            var itemToDelete = param as CashBookEntryUI;
            if (itemToDelete != null)
            {
                CashBookEntries.Remove(itemToDelete);
                if (CashBookEntries.Count == 0)
                {
                    AddNewItem();
                }
            }
            UpdateBalance(null);
            UpdateItemState();
            
        }

        public bool CanDelete(object param)
        {
            return true;
        }

        public override void Dispose()
        {
            CashBookEntries.CollectionChanged -= CashBookEntries_CollectionChanged;
            Mediator.Instance.Unregister(MediatorActionType.RefreshList, RefreshList);
            Mediator.Instance.Unregister(MediatorActionType.SetSelectedCashBook, SetSelectedCashBook);
            Mediator.Instance.Unregister(MediatorActionType.UpdateBalance, UpdateBalance);
            base.Dispose();
        }

        #endregion

    }
}
