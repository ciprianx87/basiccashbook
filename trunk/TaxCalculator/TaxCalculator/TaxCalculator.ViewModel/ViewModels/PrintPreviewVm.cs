using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.ViewModel.Base;
using TaxCalculator.Common.Mediator;
using TaxCalculator.ViewModel.ViewModels.Model;
using TaxCalculator.Data.Repositories;
using TaxCalculator.Data.Interfaces;
using TaxCalculator.Data.Model;
using TaxCalculator.Common;
using TaxCalculator.ViewModel.Extensions;
using System.Windows.Input;
using System.Windows;


namespace TaxCalculator.ViewModel.ViewModels
{
    public class PrintPreviewVm : BaseViewModel, IDisposable
    {
        #region Fields
        IIndicatorRepository indicatorRepository;
        ITaxCalculationsRepository taxCalculationRepository;

        public ICommand NextPageCommand { get; set; }
        public ICommand PreviousPageCommand { get; set; }

        public ICommand PrintCommand { get; set; }

        #endregion

        #region Constructor

        public PrintPreviewVm()
        {
            InitializeCommands();
            Mediator.Instance.Register(MediatorActionType.SetReportData, SetReportData);

            indicatorRepository = new IndicatorRepository();
            taxCalculationRepository = new TaxCalculationsRepository();


            PrintData = new PrintDataModel();
            PrintData.Pages = new List<PrintPage>();
            PrintData.Pages.Add(new PrintPage());
            PrintData.Pages[0].Rows = new List<PrintRow>();
            //LoadData();
        }

        private void InitializeCommands()
        {
            this.NextPageCommand = new DelegateCommand(NextPage, CanNextPage);
            this.PreviousPageCommand = new DelegateCommand(PreviousPage, CanPreviousPage);
            this.PrintCommand = new DelegateCommand(Print, CanPrint);
        }

        private void LoadData()
        {
            for (int i = 0; i < 10; i++)
            {
                PrintData.Pages[0].Rows.Add(new PrintRow()
                 {
                     Description = "afsafsfsa",
                     NrCrt = i.ToString(),
                     Value = (i * 20).ToString()
                 });

            }
            CurrentPage = PrintData.Pages[0];
            //Test = "A";
        }

        #endregion

        #region Properties

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


        private PrintPage currentPage;
        public PrintPage CurrentPage
        {
            get { return currentPage; }
            set
            {
                if (currentPage != value)
                {
                    currentPage = value;
                    this.NotifyPropertyChanged("CurrentPage");
                }
            }
        }

        private PrintDataModel printData;
        public PrintDataModel PrintData
        {
            get { return printData; }
            set
            {
                if (printData != value)
                {
                    printData = value;
                    this.NotifyPropertyChanged("PrintData");
                }
            }
        }


        private string test;
        public string Test
        {
            get { return test; }
            set
            {
                if (test != value)
                {
                    test = value;
                    this.NotifyPropertyChanged("Test");
                }
            }
        }

        #endregion

        #region Methods

        private bool CanPrint(object parameter)
        {
            return true;
        }

        private void Print(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.StartPrinting, PrintData.Pages);
        }

        private int currentPageNr = 1;
        private bool CanNextPage(object parameter)
        {
            NextPageEnabled = PrintData.Pages != null && PrintData.Pages.Count > currentPageNr;
            return true;
        }

        private void NextPage(object parameter)
        {
            currentPageNr++;
            LoadDataForCurrentPage();
        }


        private bool CanPreviousPage(object parameter)
        {
            PreviousPageEnabled = currentPageNr > 1;
            return true;
        }

        private void PreviousPage(object parameter)
        {
            currentPageNr--;
            LoadDataForCurrentPage();

        }

        private void LoadDataForCurrentPage()
        {
            if (currentPageNr > 0)
            {
                CurrentPage = PrintData.Pages[currentPageNr - 1];
            }
            PreviousPageCommand.CanExecute(null);
            NextPageCommand.CanExecute(null);

            PageNumber = string.Format("{0}/{1}", currentPageNr, PrintData.Pages.Count);
        }
        TaxCalculationsViewModel selectedVm;
        public void SetReportData(object param)
        {
            selectedVm = param as TaxCalculationsViewModel;
            LoadInitialData();
            LoadDataForCurrentPage();

        }
        private void LoadInitialData()
        {
            try
            {
                var calculation = taxCalculationRepository.Get(selectedVm.Id);
                List<CompletedIndicatorVm> savedEntities = VmUtils.Deserialize<List<CompletedIndicatorVm>>(calculation.Content);
                TaxCalculationOtherData otherData = VmUtils.Deserialize<TaxCalculationOtherData>(calculation.OtherData);

                var printRowList = savedEntities.ToPrintRowList();
                AddExtraRows(printRowList, selectedVm.VerifiedBy, selectedVm.CreatedBy);
                PrintData.Pages = BuildPages(printRowList);

                //add first page details
                if (PrintData.Pages.Count > 0)
                {
                    var firstPage = PrintData.Pages[0];
                    firstPage.FirstPageData = new FirstPageData()
                    {
                        Address = calculation.Company.Address,
                        Company = calculation.Company.Name,
                        Cui = calculation.Company.CUI,
                        Month = selectedVm.Month,
                        Year = otherData.Year.ToString(),
                        CoinType = otherData.CoinType
                    };
                    firstPage.FirstPage = true;
                }
                PrintData.Pages.ForEach(p => p.RectifyingVisibility = calculation.Rectifying ? Visibility.Visible : Visibility.Collapsed);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log.Error(ex);
                WindowHelper.OpenErrorDialog(Messages.Error_LoadingData);
            }
        }
        private void AddEmptyRows(List<PrintRow> printRowList)
        {
            //add an empty row before each Text row
            for (int i = printRowList.Count - 1; i > 0; i--)
            {

                if (printRowList[i].Type == TaxIndicatorType.Text)
                {
                    AddEmptyRow(printRowList, i);
                }
            }
        }

        private void AddEmptyRow(List<PrintRow> printRowList, int index)
        {
            try
            {
                printRowList.Insert(index, new PrintRow());
            }
            catch (Exception ex)
            {
                Logger.Instance.Log.Error(ex);
            }
        }

        private void AddExtraRows(List<PrintRow> printRowList, string verifiedBy, string createdBy)
        {
            AddEmptyRows(printRowList);
            AddEmptyRow(printRowList);
            //add created by
            printRowList.Add(new PrintRow()
            {
                Value = "INTOCMIT,"
            });
            printRowList.Add(new PrintRow()
            {
                Value = createdBy
            });
            //add empty row
            AddEmptyRow(printRowList);

            //add verified by
            printRowList.Add(new PrintRow()
            {
                Description = "VERIFICAT, " + verifiedBy
            });
        }

        private static void AddEmptyRow(List<PrintRow> printRowList)
        {
            printRowList.Add(new PrintRow());
        }

        int maxItemsPerPage = 47;
        int maxItemsForFirstPage = 38;
        private List<PrintPage> BuildPages(List<PrintRow> allEntities)
        {
            List<PrintPage> pages = new List<PrintPage>();
            int pageCount = 0;
            while (allEntities.Count > 0)
            {
                pageCount++;
                int itemsPerPage = maxItemsPerPage;
                if (pageCount == 1)
                {
                    itemsPerPage = maxItemsForFirstPage;
                }
                var newGroup = allEntities.Take(itemsPerPage);
                var newPrintPage = new PrintPage() { Rows = new List<PrintRow>() };
                foreach (var item in newGroup)
                {
                    newPrintPage.Rows.Add(new PrintRow()
                    {
                        Description = item.Description,
                        NrCrt = item.NrCrt,
                        Value = item.Value,
                        Type = item.Type
                    });
                }
                pages.Add(newPrintPage);

                allEntities = allEntities.Skip(itemsPerPage).ToList();
            }

            return pages;

        }
        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.SetReportData, SetReportData);
            base.Dispose();
        }


        #endregion

    }
}
