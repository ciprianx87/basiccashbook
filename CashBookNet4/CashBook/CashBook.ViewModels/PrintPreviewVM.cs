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
    public class PrintPreviewVM : BaseViewModel
    {
        private const int EXTRA_REPORT_ENTRIES = 2;
        ICashBookRepository cashBookRepository;
        ICompanyRepository companyRepository;
        ICashBookEntryRepository cashBookEntryRepository;

        public ICommand SaveCommand { get; set; }

        public ICommand NextPageCommand { get; set; }
        public ICommand PreviousPageCommand { get; set; }

        public ICommand BackCommand { get; set; }



        public PrintPreviewVM()
        {
            this.Title = "Previzualizare Rapoarte";
            Mediator.Instance.Register(MediatorActionType.SetReportsToPreview, SetReportsToPreview);
            Mediator.Instance.Register(MediatorActionType.SetPrintPreviewPrintModel, SetPrintPreviewPrintModel);


            cashBookRepository = new CashBookRepository();
            cashBookEntryRepository = new CashBookEntryRepository();
            companyRepository = new CompanyRepository();

            this.NextPageCommand = new DelegateCommand(NextPage, CanNextPage);
            this.PreviousPageCommand = new DelegateCommand(PreviousPage, CanPreviousPage);
            this.BackCommand = new DelegateCommand(Back, CanBack);
        }

        #region properties
        public List<ReportPageVM> Reports { get; set; }


        private ReportPageVM reportToPreview;
        public ReportPageVM ReportToPreview
        {
            get { return reportToPreview; }
            set
            {
                if (reportToPreview != value)
                {
                    reportToPreview = value;
                    this.NotifyPropertyChanged("ReportToPreview");
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
        public PrintModel PrintModel { get; set; }
        public void SetPrintPreviewPrintModel(object param)
        {
            PrintModel = param as PrintModel;
        }

        public void SetReportsToPreview(object param)
        {
            Reports = param as List<ReportPageVM>;
            LoadDataForCurrentPage();
        }

        private bool CanBack(object parameter)
        {
            return true;
        }

        private void Back(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.PrintControl);
            Mediator.Instance.SendMessage(MediatorActionType.SetPrintControlPrintModel, PrintModel);
            //Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.CashBook);
            //Mediator.Instance.SendMessage(MediatorActionType.SetSelectedCashBook, new CashBookDisplayInfo() { SelectedCashbook = SelectedCashBook, SelectedDate = SelectedDate });
        }

        private int currentPage = 1;
        private bool CanNextPage(object parameter)
        {
            NextPageEnabled = Reports != null && Reports.Count > currentPage;
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
        private int nrCrt = 0;
        private void LoadDataForCurrentPage()
        {
            if (currentPage > 0)
            {
                ReportToPreview = Reports[currentPage - 1];
            }
            PreviousPageCommand.CanExecute(null);
            NextPageCommand.CanExecute(null);

            PageNumber = string.Format("{0}/{1}", currentPage, Reports.Count);
        }

        public override void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.SetReportsToPreview, SetReportsToPreview);
            Mediator.Instance.Unregister(MediatorActionType.SetPrintPreviewPrintModel, SetPrintPreviewPrintModel);
            base.Dispose();
        }

        #endregion
    }
}
