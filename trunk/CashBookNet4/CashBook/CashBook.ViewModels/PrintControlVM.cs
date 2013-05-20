using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CashBook.ViewModels;
using CashBook.Common.Mediator;
using CashBook.Data.Model;
using CashBook.Data;
using System.Timers;
using System.Windows.Threading;
using CashBook.Common;
using CashBook.Data.Interfaces;
using CashBook.Data.Repositories;
using System.Collections.ObjectModel;
using CashBook.ViewModels.Models;
using System.Windows;

namespace CashBook.ViewModels
{
    public class PrintControlVM : BaseViewModel, IDisposable
    {

        #region Fields
        public ICommand OkCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        ICashBookRepository cashBookRepository;
        ICashBookEntryRepository cashBookEntryRepository;
        ICompanyRepository companyRepository;
        #endregion

        #region Constructor

        public PrintControlVM()
        {
            InitializeCommands();
            Mediator.Instance.Register(MediatorActionType.SetPaymentInformationPopupMessage, SetPaymentInformationPopupMessage);
            this.Title = "Informatie";

            cashBookRepository = new CashBookRepository();
            cashBookEntryRepository = new CashBookEntryRepository();
            companyRepository = new CompanyRepository();

            LoadData();
        }

        private void LoadData()
        {
            SelectedStartDate = DateTime.Now;
            SelectedEndDate = DateTime.Now;
            CurrentDaySelected = true;

            //load cash books

            try
            {
                Company = companyRepository.GetCompany();

                CashBooks = new ObservableCollection<UserCashBook>();
                var existingCashBooks = cashBookRepository.GetAll(CashBookListType.Any);
                if (existingCashBooks != null)
                {
                    foreach (var item in existingCashBooks)
                    {
                        CashBooks.Add(item);
                    }
                }
                if (CashBooks.Count > 0)
                {
                    SelectedCashBook = CashBooks[0];
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }


        private void InitializeCommands()
        {
            this.OkCommand = new DelegateCommand(OK, CanOK);
            this.CancelCommand = new DelegateCommand(Cancel, CanCancel);
            this.PrintCommand = new DelegateCommand(Print, CanPrint);
        }

        #endregion

        #region Properties


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

        private bool currentDaySelected;
        public bool CurrentDaySelected
        {
            get { return currentDaySelected; }
            set
            {
                if (currentDaySelected != value)
                {
                    currentDaySelected = value;
                    this.NotifyPropertyChanged("CurrentDaySelected");
                }
            }
        }



        private bool currentMonthSelected;
        public bool CurrentMonthSelected
        {
            get { return currentMonthSelected; }
            set
            {
                if (currentMonthSelected != value)
                {
                    currentMonthSelected = value;
                    this.NotifyPropertyChanged("CurrentMonthSelected");
                }
            }
        }



        private bool otherPeriodSelected;
        public bool OtherPeriodSelected
        {
            get { return otherPeriodSelected; }
            set
            {
                if (otherPeriodSelected != value)
                {
                    otherPeriodSelected = value;
                    this.NotifyPropertyChanged("OtherPeriodSelected");
                }
            }
        }


        private bool currentDateSelected;
        public bool CurrentDateSelected
        {
            get { return currentDateSelected; }
            set
            {
                if (currentDateSelected != value)
                {
                    currentDateSelected = value;
                    this.NotifyPropertyChanged("CurrentDateSelected");
                }
            }
        }

        private DateTime selectedStartDate;
        public DateTime SelectedStartDate
        {
            get { return selectedStartDate; }
            set
            {
                if (selectedStartDate != value)
                {
                    selectedStartDate = value;
                    this.NotifyPropertyChanged("SelectedStartDate");
                    SelectedStartDateString = Utils.DateTimeToStringDateOnly(selectedStartDate);
                }
            }
        }


        private DateTime selectedEndDate;
        public DateTime SelectedEndDate
        {
            get { return selectedEndDate; }
            set
            {
                if (selectedEndDate != value)
                {
                    selectedEndDate = value;
                    this.NotifyPropertyChanged("SelectedEndDate");
                    SelectedEndDateString = Utils.DateTimeToStringDateOnly(selectedEndDate);
                }
            }
        }


        private string selectedStartDateString;
        public string SelectedStartDateString
        {
            get { return selectedStartDateString; }
            set
            {
                if (selectedStartDateString != value)
                {
                    selectedStartDateString = value;
                    this.NotifyPropertyChanged("SelectedStartDateString");
                }
            }
        }


        private string selectedEndDateString;
        public string SelectedEndDateString
        {
            get { return selectedEndDateString; }
            set
            {
                if (selectedEndDateString != value)
                {
                    selectedEndDateString = value;
                    this.NotifyPropertyChanged("SelectedEndDateString");
                }
            }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                if (message != value)
                {
                    message = value;
                    this.NotifyPropertyChanged("Message");
                }
            }
        }

        public Company Company { get; set; }

        #endregion

        #region Methods

        #region printing methods

        #endregion

        private bool CanPrint(object parameter)
        {
            return true;
        }

        private void Print(object parameter)
        {
            if (IsValid())
            {
                List<DateTime> datesToPrint = new List<DateTime>();
                if (CurrentDaySelected)
                {
                    datesToPrint.Add(DateTime.Now);
                }
                else if (CurrentMonthSelected)
                {
                    var currentDate = DateTime.Now;
                    var currentDay = currentDate.Day;
                    for (int i = 1; i <= currentDay; i++)
                    {
                        datesToPrint.Add(new DateTime(currentDate.Year, currentDate.Month, i));
                    }
                }
                else if (OtherPeriodSelected)
                {
                    var currentCounterDate = Utils.DateTimeToDay(SelectedStartDate);
                    while (currentCounterDate <= Utils.DateTimeToDay(SelectedEndDate))
                    {
                        datesToPrint.Add(currentCounterDate);
                        currentCounterDate = currentCounterDate.AddDays(1);
                    }
                }
                //gather the data that needs to be printed
                List<ReportPageVM> pagesToPrint = new List<ReportPageVM>();
                foreach (var date in datesToPrint)
                {

                    ReportPagesRetriever pagesRetriever = new ReportPagesRetriever();
                    var returnedPages = pagesRetriever.GetPages(Company, SelectedCashBook, date, cashBookRepository, cashBookEntryRepository, MaxEntriesPerPage);
                    if (returnedPages != null && returnedPages.Count > 0)
                    {
                        pagesToPrint.AddRange(returnedPages);
                    }
                }
                if (pagesToPrint.Count == 0)
                {
                    WindowHelper.OpenInformationDialog("Nu exista rapoarte de listat");
                }
                else
                {
                    Mediator.Instance.SendMessage(MediatorActionType.StartPrinting, pagesToPrint);
                }
            }
        }

        private bool IsValid()
        {
            if (OtherPeriodSelected)
            {
                if (SelectedStartDate > SelectedEndDate)
                {
                    WindowHelper.OpenErrorDialog("Data de sfarsit trebuie sa fie mai mare decat cea de inceput");
                    return false;
                }
            }
            return true;
        }

        public int MaxEntriesPerPage { get; set; }

        public void SetMaxEntriesPerPage(int maxEntries)
        {
            MaxEntriesPerPage = maxEntries;
            //LoadDataForCurrentPage();

        }

        private bool CanOK(object parameter)
        {
            return true;
        }

        private void OK(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.LegalReglementations);
            Mediator.Instance.SendMessage(MediatorActionType.CloseWindow, this.Guid);
        }
        public void SetPaymentInformationPopupMessage(object param)
        {
            this.Message = param.ToString();
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

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.SetPaymentInformationPopupMessage, SetPaymentInformationPopupMessage);
        }

        #endregion

    }
}
