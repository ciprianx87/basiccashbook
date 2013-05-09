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
using System.Windows;

namespace CashBook.ViewModels
{
    public class CashBookViewModel : BaseViewModel
    {
        ICashBookRepository cashBookRepository;
        ICashBookEntryRepository cashBookEntryRepository;

        public ICommand DeleteCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand ViewReportsCommand { get; set; }


        public CashBookViewModel()
        {
            this.Title = "Registru de casa";
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
            SaveCommand = new DelegateCommand(Save, CanSave);

            this.LegalReglementationsCommand = new DelegateCommand(LegalReglementations, CanLegalReglementations);
            this.ViewReportsCommand = new DelegateCommand(ViewReports, CanViewReports);

            Mediator.Instance.Register(MediatorActionType.RefreshList, RefreshList);
            Mediator.Instance.Register(MediatorActionType.SetSelectedCashBook, SetSelectedCashBook);
            Mediator.Instance.Register(MediatorActionType.UpdateBalance, UpdateBalance);

            cashBookRepository = new CashBookRepository();
            cashBookEntryRepository = new CashBookEntryRepository();

            canSave = true;
        }



        private bool canSave = false;
        private void LoadDataForDay(DateTime dateTime)
        {
            CashBookEntries = new ObservableCollection<CashBookEntryUI>();

            var existingCashBookEntries = cashBookEntryRepository.GetEntriesForDay(SelectedCashBook.Id, dateTime);
            if (existingCashBookEntries != null)
            {
                foreach (var item in existingCashBookEntries)
                {
                    CashBookEntries.Add((CashBookEntryUI)item);
                }
            }
            CashBookEntries.Add(new CashBookEntryUI());
            //CashBookEntries.CollectionChanged+=new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CashBookEntries_CollectionChanged);
            MoneyExchangeRate = cashBookEntryRepository.GetExchangeRateForDay(SelectedCashBook.Id, dateTime);
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

        private string currentBalanceInString;
        public string CurrentBalanceInString
        {
            get { return currentBalanceInString; }
            set
            {
                if (currentBalanceInString != value)
                {
                    currentBalanceInString = value;
                    this.NotifyPropertyChanged("CurrentBalanceInString");
                }
            }
        }

        private decimal currentBalanceIn;
        public decimal CurrentBalanceIn
        {
            get { return currentBalanceIn; }
            set
            {
                //if (currentBalanceIn != value)
                {
                    currentBalanceIn = value;
                    this.NotifyPropertyChanged("CurrentBalanceIn");
                }
            }
        }



        private string currentBalanceOutString;
        public string CurrentBalanceOutString
        {
            get { return currentBalanceOutString; }
            set
            {
                if (currentBalanceOutString != value)
                {
                    currentBalanceOutString = value;
                    this.NotifyPropertyChanged("CurrentBalanceOutString");
                }
            }
        }


        private decimal currentBalanceOut;
        public decimal CurrentBalanceOut
        {
            get { return currentBalanceOut; }
            set
            {
                //if (currentBalanceOut != value)
                {
                    currentBalanceOut = value;
                    this.NotifyPropertyChanged("CurrentBalanceOut");
                }
            }
        }



        private CashBookEntryUI selectedItem;
        public CashBookEntryUI SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    //this.NotifyPropertyChanged("SelectedItem");
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
                    //this.NotifyPropertyChanged("SelectedIndex");
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
                    SelectedDateString = Utils.DateTimeToStringDateOnly(selectedDate);
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


        private string selectedDateString;
        public string SelectedDateString
        {
            get { return selectedDateString; }
            set
            {
                if (selectedDateString != value)
                {
                    selectedDateString = value;
                    this.NotifyPropertyChanged("SelectedDateString");
                }
            }
        }


        private string moneyExchangeRateString;
        public string MoneyExchangeRateString
        {
            get { return moneyExchangeRateString; }
            set
            {
                if (moneyExchangeRateString != value)
                {
                    moneyExchangeRateString = value;
                    this.NotifyPropertyChanged("MoneyExchangeRateString");
                }
            }
        }


        private decimal? moneyExchangeRate;
        public decimal? MoneyExchangeRate
        {
            get { return moneyExchangeRate; }
            set
            {
                //if (moneyExchangeRate != value)
                {
                    moneyExchangeRate = value;
                    this.NotifyPropertyChanged("MoneyExchangeRate");
                    // UpdateStringValues();
                }
            }
        }


        private Visibility moneyExchangeRateVisibility;
        public Visibility MoneyExchangeRateVisibility
        {
            get { return moneyExchangeRateVisibility; }
            set
            {
                if (moneyExchangeRateVisibility != value)
                {
                    moneyExchangeRateVisibility = value;
                    this.NotifyPropertyChanged("MoneyExchangeRateVisibility");
                }
            }
        }




        #endregion

        #region methods

        private void UpdateStringValues()
        {
            TotalBalanceString = DecimalConvertor.Instance.DecimalToString(TotalBalance);
            CurrentBalanceInString = DecimalConvertor.Instance.DecimalToString(CurrentBalanceIn);
            CurrentBalanceOutString = DecimalConvertor.Instance.DecimalToString(CurrentBalanceOut);
            if (MoneyExchangeRate.HasValue)
            {
                MoneyExchangeRateString = DecimalConvertor.Instance.DecimalToString(MoneyExchangeRate.Value);
            }
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
            CurrentBalanceOut = 0;
            CurrentBalanceIn = 0;
            CashBookEntries.ToList().ForEach(p => CurrentBalanceIn += p.Incasari);
            CashBookEntries.ToList().ForEach(p => CurrentBalanceOut += p.Plati);

            totalSum = CurrentBalanceIn - CurrentBalanceOut;

            if (SelectedCashBook != null)
            {
                var initialBalanceForDayDecimal = cashBookRepository.GetInitialBalanceForDay(SelectedCashBook.Id, SelectedDate);
                InitialBalanceForDayString = DecimalConvertor.Instance.DecimalToString(initialBalanceForDayDecimal);
                totalSum += initialBalanceForDayDecimal;
            }
            TotalBalance = totalSum;

            if (CurrentBalanceOut > AppSettings.TotalPaymentLimit)
            {
                WindowHelper.OpenPaymentInformationDialog("Va rugam sa cititi Reglementarile legale legate de valoarea platilor. \r\nDoriti sa le cititi acum?");
            }
        }

        public void SetSelectedCashBook(object param)
        {
            SelectedCashBook = param as UserCashBook;
            this.Title = "Editare Registru de casa (" + SelectedCashBook.Name + ")";
            DecimalConvertor.Instance.SetNumberOfDecimals(SelectedCashBook.CoinDecimals);

            SelectedDate = DateTime.Now;
            UpdateBalance(null);

            MoneyExchangeRateVisibility = SelectedCashBook.IsLei ? Visibility.Collapsed : Visibility.Visible;
            if (SelectedCashBook.IsLei)
            {
                MoneyExchangeRate = 0;
            }
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
                if (IsFormValid())
                {
                    var validEntries = ExtractValidItems(CashBookEntries);
                    if (validEntries.Count > 0)
                    {
                        cashBookEntryRepository.UpdateRepositoryForDay(SelectedCashBook.Id, validEntries, SelectedDate, MoneyExchangeRate);
                        WindowHelper.OpenInformationDialog("Informatia a fost salvata");
                    }
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

        private bool IsFormValid()
        {
            if (TotalBalance < 0)
            {
                WindowHelper.OpenInformationDialog("Atentie! Sold final negativ");
                return false;
            }
            if (CurrentBalanceOut > AppSettings.SinglePaymentLimit)
            {
                WindowHelper.OpenPaymentInformationDialog("Va rugam sa cititi Reglementarile legale legate de valoarea platilor. \r\nDoriti sa le cititi acum?");
                return false;
            }
            if (CashBookEntries.Any(p => p.Plati > AppSettings.SinglePaymentLimit))
            {
                WindowHelper.OpenPaymentInformationDialog("Va rugam sa cititi Reglementarile legale legate de valoarea platilor. \r\nDoriti sa le cititi acum?");

                return false;
            }


            foreach (var item in CashBookEntries)
            {
                if (item.IsEmpty() && CashBookEntries.IndexOf(item) == CashBookEntries.Count - 1)
                {
                    continue;
                }
                //do not take the last one (empty) into consideration
                if (!item.IsFormValid())
                {
                    WindowHelper.OpenInformationDialog("Atentie! Exista erori pe pagina");
                    return false;
                }
            }
            return true;
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

        private bool CanViewReports(object parameter)
        {
            return true;
        }

        private void ViewReports(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.Reports);
            Mediator.Instance.SendMessage(MediatorActionType.SetSelectedDate, new ReportInitialData()
            {
                SelectedDate = SelectedDate,
                SelectedCashBookId = SelectedCashBook.Id
            });

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


        public void SetCurrentSelection(object p)
        {
            SelectedItem = p as CashBookEntryUI;
            SelectedIndex = CashBookEntries.IndexOf(SelectedItem);
        }
    }
}
