using CashBook.Data.Interfaces;
using CashBook.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CashBook.Common;
using System.Collections.ObjectModel;
using CashBook.ViewModels.Models;
using CashBook.Common.Mediator;
using CashBook.Data.Model;
using System.Windows;

namespace CashBook.ViewModels
{
    public class ReportsViewModel : BaseViewModel
    {
        private const int EXTRA_REPORT_ENTRIES = 2;
        ICashBookRepository cashBookRepository;
        ICompanyRepository companyRepository;
        ICashBookEntryRepository cashBookEntryRepository;

        public ICommand SaveCommand { get; set; }

        public ICommand NextPageCommand { get; set; }
        public ICommand PreviousPageCommand { get; set; }


        public ReportsViewModel()
        {
            this.Title = "Rapoarte";
            Mediator.Instance.Register(MediatorActionType.SetSelectedDate, SetSelectedDate);


            cashBookRepository = new CashBookRepository();
            cashBookEntryRepository = new CashBookEntryRepository();
            companyRepository = new CompanyRepository();

            this.NextPageCommand = new DelegateCommand(NextPage, CanNextPage);
            this.PreviousPageCommand = new DelegateCommand(PreviousPage, CanPreviousPage);
        }

        #region properties

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


        private ObservableCollection<CashBookEntryUI> currentPageCashBookEntries;
        public ObservableCollection<CashBookEntryUI> CurrentPageCashBookEntries
        {
            get { return currentPageCashBookEntries; }
            set
            {
                if (currentPageCashBookEntries != value)
                {
                    currentPageCashBookEntries = value;
                    this.NotifyPropertyChanged("CurrentPageCashBookEntries");
                }
            }
        }



        private Company company;
        public Company Company
        {
            get { return company; }
            set
            {
                if (company != value)
                {
                    company = value;
                    this.NotifyPropertyChanged("Company");
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

        private decimal? moneyExchangeRate;
        public decimal? MoneyExchangeRate
        {
            get { return moneyExchangeRate; }
            set
            {
                //if (moneyExchangeRate != value)
                {
                    moneyExchangeRate = value;

                    if (MoneyExchangeRate.HasValue)
                    {
                        MoneyExchangeRateString = DecimalConvertor.Instance.DecimalToString(MoneyExchangeRate.Value);
                    }
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

        private decimal initialBalanceForDayDecimal;
        public decimal InitialBalanceForDayDecimal
        {
            get { return initialBalanceForDayDecimal; }
            set
            {
                //if (initialBalanceForDayDecimal != value)
                {
                    initialBalanceForDayDecimal = value;
                    InitialBalanceForDayString = DecimalConvertor.Instance.DecimalToString(InitialBalanceForDayDecimal);

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


        private bool nextPageEnabled;
        public bool NextPageEnabled
        {
            get { return nextPageEnabled; }
            set
            {
                if (nextPageEnabled != value)
                {
                    nextPageEnabled = value;
                    this.NotifyPropertyChanged("NextPageEnabled");
                }
            }
        }


        private bool previousPageEnabled;
        public bool PreviousPageEnabled
        {
            get { return previousPageEnabled; }
            set
            {
                if (previousPageEnabled != value)
                {
                    previousPageEnabled = value;
                    this.NotifyPropertyChanged("PreviousPageEnabled");
                }
            }
        }


        private string pageNumber;
        public string PageNumber
        {
            get { return pageNumber; }
            set
            {
                if (pageNumber != value)
                {
                    pageNumber = value;
                    this.NotifyPropertyChanged("PageNumber");
                }
            }
        }


        #endregion

        #region methods
        public void SetSelectedDate(object param)
        {
            ReportInitialData initialData = param as ReportInitialData;
            SelectedDate = initialData.SelectedDate;
            SelectedCashBook = initialData.CashBook;
            this.Title = "Vizualizare Raport";
            LoadDataForDay(SelectedDate);

            LoadCompanyData();
        }



        private void LoadCompanyData()
        {
            Company = companyRepository.GetCompany();
        }

        private void LoadDataForDay(DateTime dateTime)
        {
            MoneyExchangeRate = cashBookEntryRepository.GetExchangeRateForDay(SelectedCashBook.Id, dateTime);

            CashBookEntries = new ObservableCollection<CashBookEntryUI>();

            var existingCashBookEntries = cashBookEntryRepository.GetEntriesForDay(SelectedCashBook.Id, dateTime);
            if (existingCashBookEntries != null)
            {
                foreach (var item in existingCashBookEntries)
                {
                    CashBookEntries.Add((CashBookEntryUI)item);
                }
            }
            AddExtraDetailsRows();

            MoneyExchangeRateVisibility = SelectedCashBook.IsLei ? Visibility.Collapsed : Visibility.Visible;

            if (!SelectedCashBook.IsLei && MoneyExchangeRate.HasValue)
            {
                foreach (var item in CashBookEntries)
                {
                    if (item.Incasari != 0)
                    {
                        item.LeiValue = item.Incasari * MoneyExchangeRate.Value;
                    }
                    else
                    {
                        item.LeiValue = item.Plati * MoneyExchangeRate.Value;
                    }
                }
            }

            InitialBalanceForDayDecimal = cashBookRepository.GetInitialBalanceForDay(SelectedCashBook.Id, SelectedDate);

        }

        private int currentPage = 1;
        private bool CanNextPage(object parameter)
        {
            NextPageEnabled = CashBookEntries != null && CashBookEntries.Count > MaxEntriesPerPage * currentPage;
            return true;
        }

        private void NextPage(object parameter)
        {
            currentPage++;
            LoadDataForCurrentPage();
        }


        private bool CanPreviousPage(object parameter)
        {
            PreviousPageEnabled = currentPage > 1;
            return true;
        }

        private void PreviousPage(object parameter)
        {
            currentPage--;
            LoadDataForCurrentPage();

        }

        public int MaxEntriesPerPage { get; set; }
        public void SetMaxEntriesPerPage(int maxEntries)
        {
            MaxEntriesPerPage = maxEntries;
            LoadDataForCurrentPage();

        }

        private void LoadDataForCurrentPage()
        {
            if (currentPage > 0)
            {
                CurrentPageCashBookEntries = new ObservableCollection<CashBookEntryUI>();
                var currentPageItems = CashBookEntries.Skip(MaxEntriesPerPage * (currentPage - 1)).Take(MaxEntriesPerPage).ToList();
                foreach (var item in currentPageItems)
                {
                    CurrentPageCashBookEntries.Add(item);
                }
            }
            PreviousPageCommand.CanExecute(null);
            NextPageCommand.CanExecute(null);

            int totalPages = 0;
            int mod = (int)CashBookEntries.Count % MaxEntriesPerPage;
            totalPages = (int)CashBookEntries.Count / MaxEntriesPerPage;
            if (mod != 0)
            {
                totalPages++;
            }
            PageNumber = string.Format("{0}/{1}", currentPage, totalPages);
        }

        private void AddExtraDetailsRows()
        {
            CashBookEntryUI currentBalance = new CashBookEntryUI()
            {
                Plati = 32.2M,
                Incasari = 311.3M,
                Explicatii = "Rulaj curent",
                IsExtraDetail = true
            };

            CashBookEntryUI finalBalance = new CashBookEntryUI()
            {
                Incasari = 1232.2M,
                Plati = 0,
                PlatiString = "",
                Explicatii = "Sold final",
                IsExtraDetail = true
            };

            CashBookEntries.Add(currentBalance);
            CashBookEntries.Add(finalBalance);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion
    }
}
