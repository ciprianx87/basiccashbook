﻿using System;
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
using TaxCalculator.ViewModel.Helper;
using Microsoft.Win32;
using System.Globalization;


namespace TaxCalculator.ViewModel.ViewModels
{
    public class PrintPreviewVm : BaseViewModel, IDisposable
    {
        #region Fields
        IIndicatorRepository indicatorRepository;
        ITaxCalculationsRepository taxCalculationRepository;

        public ICommand NextPageCommand { get; set; }
        public ICommand PreviousPageCommand { get; set; }
        public ICommand BackCommand { get; set; }

        public ICommand PrintCommand { get; set; }

        public ICommand ExportCommand { get; set; }


        #endregion

        #region Constructor

        public PrintPreviewVm()
        {
            InitializeCommands();
            Mediator.Instance.Register(MediatorActionType.SetReportData, SetReportData);
            Mediator.Instance.Register(MediatorActionType.ExecuteTaxCalculation, ExecuteTaxCalculation);

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
            this.BackCommand = new DelegateCommand(Back, CanBack);
            this.ExportCommand = new DelegateCommand(Export, CanExport);
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
        private bool CanExport(object parameter)
        {
            return true;
        }

        private void Export(object parameter)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.AddExtension = true;
                //sfd.DefaultExt = ".xls";
                //get excel version
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                double tVersion = 0;
                if (double.TryParse(excel.Version, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out tVersion))
                {
                    string filter = "";
                    // 12 is the first version with .xlsx extension
                    //http://stackoverflow.com/questions/2914643/determine-excel-version-culture-via-microsoft-office-interop-excel
                    if (tVersion > 11.5)
                    {
                        filter = "Excel File (2007)|*.xlsx";
                    }
                    else
                    {
                        filter = "Excel File (2003)|*.xls";
                    }

                    sfd.Filter = filter;
                    sfd.Title = "Save an Excel File";

                    sfd.OverwritePrompt = true;
                    if (sfd.ShowDialog() == true)
                    {
                        ExcelExport.ExportToExcel(PrintData, sfd.FileName);
                    }
                }
                else
                {
                    //excel is not installed
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Log.Error(ex);
                WindowHelper.OpenErrorDialog(Messages.GenericError);
            }
        }

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


        private bool CanBack(object parameter)
        {
            return true;
        }

        private void Back(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.TaxCalculationList);
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
            Logger.Instance.Log.Debug("set report data");
            selectedVm = param as TaxCalculationsViewModel;
            LoadInitialData();
            LoadDataForCurrentPage();

        }
        private void LoadInitialData()
        {
            try
            {
                var calculation = taxCalculationRepository.Get(selectedVm.Id);
                calculation.Rectifying = false;
                var completedIndicatorDbModel = VmUtils.Deserialize<CompletedIndicatorDbModel>(calculation.Content);
                List<CompletedIndicatorVm> savedEntities = completedIndicatorDbModel.CompletedIndicators;
                TaxCalculationOtherData otherData = VmUtils.Deserialize<TaxCalculationOtherData>(calculation.OtherData);

                TaxCalculationOtherData initialOtherData = null;
                List<CompletedIndicatorVm> initialSavedEntities = null;
                List<PrintRow> initialPrintRowList = null;
                bool row69Completed = false;
                var row69 = GetRowByInnerId(savedEntities, 69);
                if (row69 != null)
                {
                    row69Completed = DecimalConvertor.Instance.StringToDecimal(row69.Value) != 0;
                }
                if (row69Completed && otherData.SecondTypeReport)
                {
                    var initialCompletedIndicatorDbModel = VmUtils.Deserialize<CompletedIndicatorDbModel>(calculation.Content);
                    initialSavedEntities = initialCompletedIndicatorDbModel.CompletedIndicators;

                    var row10 = GetRowByInnerId(initialSavedEntities, 10);
                    var row33 = GetRowByInnerId(initialSavedEntities, 33);
                    var row69Value = DecimalConvertor.Instance.StringToDecimal(row69.Value);
                    row10.Value = DecimalConvertor.Instance.DecimalToString(DecimalConvertor.Instance.StringToDecimal(row10.Value) + row69Value, otherData.NrOfDecimals);
                    row33.Value = DecimalConvertor.Instance.DecimalToString(DecimalConvertor.Instance.StringToDecimal(row33.Value) + row69Value, otherData.NrOfDecimals);

                    //execute the formulas after the values have been updated

                    //convert to taxIndicatorViewModel
                    nrDecimals = otherData.NrOfDecimals;
                    DecimalConvertor.Instance.SetNumberOfDecimals(nrDecimals);
                    //remove the dots "." from the value field because they are converted into commans and the number is interpreted as a lower number
                    //ValueField from TaxIndicatorViewModel
                    initialSavedEntities.ForEach(p =>
                    {
                        if (!string.IsNullOrEmpty(p.Value))
                        {
                            p.Value = p.Value.Replace(".", string.Empty);
                        }
                    }
                        );
                    CompletedIndicators = initialSavedEntities.ToTaxIndicatorViewModel();
                    ExecuteTaxCalculation(null);
                    initialSavedEntities = CompletedIndicators.ToCompletedIndicatorVm();

                    initialPrintRowList = initialSavedEntities.ToPrintRowList();
                    //AddExtraRows(initialPrintRowList, selectedVm.VerifiedBy, selectedVm.CreatedBy);
                    AddEmptyRows(initialPrintRowList);
                }

                var printRowList = savedEntities.ToPrintRowList();
                //AddExtraRows(printRowList, selectedVm.VerifiedBy, selectedVm.CreatedBy);
                AddEmptyRows(printRowList);
                if (row69Completed && otherData.SecondTypeReport)
                {
                    PrintData.Pages = BuildPages(printRowList, initialPrintRowList);
                }
                else
                {
                    PrintData.Pages = BuildPages(printRowList);
                }
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
                        CoinType = otherData.CoinType,
                        SecondTypeReport = otherData.SecondTypeReport,
                        Rectifying = selectedVm.Rectifying
                    };
                    firstPage.FirstPage = true;

                    PrintData.Pages.ForEach(p =>
                    {
                        p.Version2Visibility = row69Completed && otherData.SecondTypeReport ? Visibility.Visible : Visibility.Collapsed;
                        p.LastPageVisibility = Visibility.Collapsed;
                    }
                    );

                    PrintData.Pages.Last().LastPageVisibility = Visibility.Visible;
                    PrintData.Pages.Last().LastPageData = new LastPageData()
                    {
                        CreatedBy = otherData.CreatedBy,
                        VerifiedBy = otherData.VerifiedBy
                    };

                    if (row69Completed)
                    {
                        //firstPage.FirstPageData.InitialMonth = initialOtherData.Month;
                        //firstPage.FirstPageData.InitialYear = initialOtherData.Year.ToString();

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Log.Error(ex);
                WindowHelper.OpenErrorDialog(Messages.Error_LoadingData);
            }
        }

        #region execute calculation

        private List<TaxIndicatorViewModel> CompletedIndicators;
        private int nrDecimals = 0;
        public void ExecuteTaxCalculation(object param)
        {
            if (CompletedIndicators == null)
            {
                return;
            }

            //return;
            //execute this until all the values remain the same
            int executionCounter = 0;
            bool hasChanged = true;
            while (hasChanged)
            {
                executionCounter++;
                if (executionCounter > Constants.InfiniteLoopThreshold)
                {
                    //probably got in an infinite loop
                    WindowHelper.OpenErrorDialog(Messages.Error_InfiniteLoopDetected);
                    break;
                }
                hasChanged = false;
                var calculatedTaxIndicators = CompletedIndicators.Where(p => p.Type == TaxIndicatorType.Calculat);
                foreach (var item in calculatedTaxIndicators)
                {
                    hasChanged = ExecuteTaxCalculation(hasChanged, item);
                }
            }
        }

        private bool ExecuteTaxCalculation(bool hasChanged, TaxIndicatorViewModel item)
        {
            TaxFormula taxFormula = null;
            try
            {
                if (string.IsNullOrEmpty(item.IndicatorFormula))
                {
                    //item.SetError("formula goala");
                }
                taxFormula = new TaxFormula(item.IndicatorFormula);
            }
            catch (Exception ex)
            {
                //item.SetError(Constants.RulesText);
                // item.SetError("eroare la interpretarea formulei");
            }
            try
            {

                var newValueString = DecimalConvertor.Instance.DecimalToString(taxFormula.Execute(CompletedIndicators.ToList()), nrDecimals);
                //var newValue = taxFormula.Execute(TaxIndicators.ToList()).ToString();
                if (item.ValueField != newValueString)
                {
                    hasChanged = true;
                }
                newValueString = newValueString.Replace(".", string.Empty);
                item.ValueField = newValueString;
            }
            catch (Exception ex)
            {
                item.SetError("formula invalida");
            }
            return hasChanged;
        }

        #endregion

        private static CompletedIndicatorVm GetRowByInnerId(List<CompletedIndicatorVm> savedEntities, int rowInnerId)
        {
            var existingRow = savedEntities.FirstOrDefault(p => p.InnerId == rowInnerId);
            return existingRow;
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
            return;
            AddEmptyRows(printRowList);
            AddEmptyRow(printRowList);
            AddEmptyRow(printRowList);
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
                var newPrintPage = new PrintPage() { Rows = new List<PrintRow>(), LastPageVisibility = Visibility.Collapsed };
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

        private List<PrintPage> BuildPages(List<PrintRow> allEntities, List<PrintRow> initialEntities)
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
                var initialNewGroup = initialEntities.Take(itemsPerPage);
                var newPrintPage = new PrintPage() { Rows = new List<PrintRow>() };
                foreach (var item in newGroup)
                {
                    int indexOf = newGroup.ToList().IndexOf(item);
                    var newRow = new PrintRow()
                    {
                        Description = item.Description,
                        NrCrt = item.NrCrt,
                        Value = item.Value,
                        Type = item.Type,
                    };
                    if (initialEntities.Count > indexOf)
                    {
                        newRow.InitialValue = initialEntities[indexOf].Value;
                    }
                    newPrintPage.Rows.Add(newRow);
                }
                pages.Add(newPrintPage);

                allEntities = allEntities.Skip(itemsPerPage).ToList();
                initialEntities = initialEntities.Skip(itemsPerPage).ToList();
            }

            return pages;

        }
        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.SetReportData, SetReportData);
            Mediator.Instance.Unregister(MediatorActionType.ExecuteTaxCalculation, ExecuteTaxCalculation);
            base.Dispose();
        }


        #endregion

    }
}
