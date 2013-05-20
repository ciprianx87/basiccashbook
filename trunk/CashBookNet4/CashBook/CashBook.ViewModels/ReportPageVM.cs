using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CashBook.Data.Model;
using System.Windows;
using System.Collections.ObjectModel;
using CashBook.ViewModels.Models;
using CashBook.Common;
using CashBook.Data.Interfaces;

namespace CashBook.ViewModels
{
    public class ReportPageVM : BaseViewModel
    {
        ICashBookRepository cashBookRepository;
        ICashBookEntryRepository cashBookEntryRepository;
        public ReportPageVM()
        {
            //this.Company = company;
            //this.SelectedCashBook = cashBook;
            //this.cashBookEntryRepository = cashBookEntryRepository;
            //this.cashBookRepository = cashBookRepository;
            LoadData();
        }




        #region Properties


        private string reportTitle;
        public string ReportTitle
        {
            get { return reportTitle; }
            set
            {
                if (reportTitle != value)
                {
                    reportTitle = value;
                    this.NotifyPropertyChanged("ReportTitle");
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
                }
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
                        MoneyExchangeRateString = DecimalConvertor.Instance.DecimalToString(MoneyExchangeRate.Value, 4);
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

        #region Methods

        private void LoadData()
        {
        }


        public int MaxEntriesPerPage { get; set; }

        private int currentPage = 1;

        //private void LoadDataForCurrentPage()
        //{
        //    if (currentPage > 0)
        //    {
        //        CurrentPageCashBookEntries = new ObservableCollection<CashBookEntryUI>();
        //        var currentPageItems = CashBookEntries.Skip(MaxEntriesPerPage * (currentPage - 1)).Take(MaxEntriesPerPage).ToList();

        //        foreach (var item in currentPageItems)
        //        {
        //            CurrentPageCashBookEntries.Add(item);
        //        }
        //    }


        //    int totalPages = 0;
        //    int mod = (int)CashBookEntries.Count % MaxEntriesPerPage;
        //    totalPages = (int)CashBookEntries.Count / MaxEntriesPerPage;
        //    if (mod != 0)
        //    {
        //        totalPages++;
        //    }
        //    PageNumber = string.Format("{0}/{1}", currentPage, totalPages);
        //}

        //private ReportPageVM LoadDataForDay(DateTime dateTime)
        //{
        //    ReportPageVM page = new ReportPageVM(Company, SelectedCashBook, cashBookRepository, cashBookEntryRepository);

        //    MoneyExchangeRate = cashBookEntryRepository.GetExchangeRateForDay(SelectedCashBook.Id, dateTime);

        //    CashBookEntries = new ObservableCollection<CashBookEntryUI>();
        //    var tempCashBookEntries = new ObservableCollection<CashBookEntryUI>();

        //    var existingCashBookEntries = cashBookEntryRepository.GetEntriesForDay(SelectedCashBook.Id, dateTime);
        //    if (existingCashBookEntries != null)
        //    {
        //        foreach (var item in existingCashBookEntries)
        //        {
        //            tempCashBookEntries.Add((CashBookEntryUI)item);
        //        }
        //    }

        //    //add incasari
        //    foreach (var item in tempCashBookEntries)
        //    {
        //        if (item.Incasari != 0)
        //        {
        //            CashBookEntries.Add((CashBookEntryUI)item);
        //        }
        //    }

        //    //add payments
        //    foreach (var item in tempCashBookEntries)
        //    {
        //        if (item.Plati != 0)
        //        {
        //            CashBookEntries.Add((CashBookEntryUI)item);
        //        }
        //    }
        //    ReportTitle = SelectedCashBook.IsLei ? "REGISTRU DE CASA in LEI" : "REGISTRU DE CASA in VALUTA";
        //    MoneyExchangeRateVisibility = SelectedCashBook.IsLei ? Visibility.Collapsed : Visibility.Visible;

        //    if (!SelectedCashBook.IsLei && MoneyExchangeRate.HasValue)
        //    {
        //        foreach (var item in CashBookEntries)
        //        {
        //            if (item.Incasari != 0)
        //            {
        //                item.LeiValue = item.Incasari * MoneyExchangeRate.Value;
        //            }
        //            else
        //            {
        //                item.LeiValue = item.Plati * MoneyExchangeRate.Value;
        //            }
        //        }
        //    }

        //    InitialBalanceForDayDecimal = cashBookRepository.GetInitialBalanceForDay(SelectedCashBook.Id, dateTime);



        //    AddExtraDetailsRows();

        //    return page;
        //}


        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            //Mediator.Instance.Unregister(MediatorActionType.SetInformationPopupMessage, SetInformationPopupMessage);
        }

        #endregion
    }
}
