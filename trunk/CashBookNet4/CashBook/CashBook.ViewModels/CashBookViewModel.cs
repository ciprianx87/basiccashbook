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
        ICompanyRepository companyRepository;

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
            companyRepository = new CompanyRepository();

            canSave = true;

        }

        List<CashBookEntryUI> InitialCashBookEntries;
        decimal InitialMoneyExchangeRate = 0;

        private bool canSave = false;
        private void LoadDataForDay(DateTime dateTime)
        {
            CashBookEntries = new ObservableCollection<CashBookEntryUI>();

            var existingCashBookEntries = cashBookEntryRepository.GetEntriesForDay(SelectedCashBook.Id, dateTime);
            if (existingCashBookEntries != null)
            {
                foreach (var item in existingCashBookEntries)
                {
                    var newItem = (CashBookEntryUI)item;
                    newItem.IsModified = false;
                    CashBookEntries.Add(newItem);
                }
            }
            CashBookEntries.Add(new CashBookEntryUI() { IsModified = false });

            //CashBookEntries.CollectionChanged+=new System.Collections.Specialized.NotifyCollectionChangedEventHandler(CashBookEntries_CollectionChanged);
            var dbExchangeRate = cashBookEntryRepository.GetExchangeRateForDay(SelectedCashBook.Id, dateTime);
            MoneyExchangeRate = dbExchangeRate.HasValue ? dbExchangeRate.Value : 0;
            UpdateBalance(null);
            //AddFakeItems();
            UpdateItemState();

            CashBookEntries.ToList().ForEach(p => p.IsFormValid());
            UpdateInitialDataForComparison();

            UpdateCompletedDays();
        }

        private void UpdateCompletedDays()
        {
            var completedDays = cashBookEntryRepository.GetCompletedDays(SelectedCashBook.Id);
            Mediator.Instance.SendMessage(MediatorActionType.SetCompletedDays, completedDays);
        }

        private void UpdateInitialDataForComparison()
        {
            InitialMoneyExchangeRate = MoneyExchangeRate;
            InitialCashBookEntries = CloneEntries(CashBookEntries.ToList());
        }
        private List<CashBookEntryUI> CloneEntries(List<CashBookEntryUI> original)
        {
            var newList = new List<CashBookEntryUI>();
            foreach (var item in original)
            {
                newList.Add(item.Clone());
            }
            return newList;

        }

        private void AddFakeItems()
        {
            for (int i = CashBookEntries.Count; i < 100; i++)
            {
                CashBookEntries.Add(new CashBookEntryUI()
                {
                    NrCrt = i,
                    IsModified = false
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
                    moneyExchangeRateString = Utils.PrepareForConversion(value);

                    // MoneyExchangeRate = DecimalConvertor.Instance.StringToDecimal(moneyExchangeRateString);
                    if (!string.IsNullOrEmpty(moneyExchangeRateString))
                    {
                        MoneyExchangeRate = DecimalConvertor.Instance.StringToDecimal(moneyExchangeRateString);
                    }
                    moneyExchangeRateString = DecimalConvertor.Instance.DecimalToString(MoneyExchangeRate, 4);
                    this.NotifyPropertyChanged("MoneyExchangeRateString");
                }
            }
        }


        private decimal moneyExchangeRate;
        public decimal MoneyExchangeRate
        {
            get { return moneyExchangeRate; }
            set
            {
                if (moneyExchangeRate != value)
                {
                    moneyExchangeRate = value;
                    this.NotifyPropertyChanged("MoneyExchangeRate");
                    UpdateStringValues();
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


        private DateTime startDate;
        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                if (startDate != value)
                {
                    startDate = value;
                    this.NotifyPropertyChanged("StartDate");
                }
            }
        }


        private DateTime endDate;
        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                if (endDate != value)
                {
                    endDate = value;
                    this.NotifyPropertyChanged("EndDate");
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
            MoneyExchangeRateString = DecimalConvertor.Instance.DecimalToString(MoneyExchangeRate, 4);
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
                UpdateItemState();
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
        }

        public void SetSelectedCashBook(object param)
        {
            if (param is UserCashBook)
            {
                LoadDataForCashBook(param as UserCashBook, DateTime.Now);
            }
            else if (param is CashBookDisplayInfo)
            {
                CashBookDisplayInfo info = param as CashBookDisplayInfo;
                LoadDataForCashBook(info.SelectedCashbook, info.SelectedDate);
            }
        }

        private void LoadDataForCashBook(UserCashBook cashBook, DateTime date)
        {
            SelectedCashBook = cashBook;
            this.Title = "Editare Registru de casa (" + SelectedCashBook.Name + ")";
            DecimalConvertor.Instance.SetNumberOfDecimals(SelectedCashBook.CoinDecimals);

            SelectedDate = date;
            UpdateBalance(null);

            MoneyExchangeRateVisibility = SelectedCashBook.IsLei ? Visibility.Collapsed : Visibility.Visible;
            if (SelectedCashBook.IsLei)
            {
                MoneyExchangeRate = 0;
            }
            Company = companyRepository.GetCompany();
            EndDate = DateTime.Now;
            if (SelectedCashBook.InitialBalanceDate.HasValue)
            {
                StartDate = SelectedCashBook.InitialBalanceDate.Value;
            }
            //do not let the user enter values in a day before the registry start date
            if (SelectedDate < StartDate)
            {
                SelectedDate = StartDate;
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
                    Logger.Instance.Log.InfoFormat("SaveData initial count: {0}", CashBookEntries.Count);
                    var validEntries = ExtractValidItems(CashBookEntries);
                    Logger.Instance.Log.InfoFormat("SaveData validEntries count: {0}", validEntries.Count);
                    //save even if there are 0 valid entries (it means all the entries were deleted)
                    //if (validEntries.Count > 0)
                    {
                        cashBookEntryRepository.UpdateRepositoryForDay(SelectedCashBook.Id, validEntries, SelectedDate, MoneyExchangeRate);
                        CashBookEntries.ToList().ForEach(p => p.IsModified = false);
                        UpdateInitialDataForComparison();
                        UpdateCompletedDays();
                        WindowHelper.OpenInformationDialog("Informatia a fost salvata");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
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
            string errorMessage = "";
            if (TotalBalance < 0)
            {
                errorMessage = "Atentie! Sold final negativ" + Environment.NewLine;
                //WindowHelper.OpenErrorDialog("Atentie! Sold final negativ");
                //return false;
            }
            //only apply the legal limits to LEI coin type
            if (SelectedCashBook.IsLei)
            {
                //payment
                if (CurrentBalanceOut > VMUtils.LegalLimits.TotalPayment)
                {
                    Logger.Instance.Log.Debug(string.Format("CurrentBalanceOut  {0} > {1}", CurrentBalanceOut, VMUtils.LegalLimits.TotalPayment));
                    //errorMessage += Messages.LegalReglementationsNotifications + Environment.NewLine;
                    WindowHelper.OpenPaymentInformationDialog(Messages.LegalReglementationsNotifications);
                }
                if (CashBookEntries.Any(p => p.Plati > VMUtils.LegalLimits.DailyPayment))
                {
                    var count = CashBookEntries.Count(p => p.Plati > VMUtils.LegalLimits.DailyPayment);
                    Logger.Instance.Log.Debug(string.Format("CashBookEntries contains {0} entries with Plati > {1}", count, VMUtils.LegalLimits.DailyPayment));
                    //if (string.IsNullOrEmpty(errorMessage))
                    //{
                    //    errorMessage += Messages.LegalReglementationsNotifications + Environment.NewLine;
                    //}
                    WindowHelper.OpenPaymentInformationDialog(Messages.LegalReglementationsNotifications);
                }

                //cashing
                if (VMUtils.LegalLimits.TotalCashingActive && CurrentBalanceIn > VMUtils.LegalLimits.TotalCashing)
                {
                    Logger.Instance.Log.Debug(string.Format("CurrentBalanceIn  {0} > {1}", CurrentBalanceIn, VMUtils.LegalLimits.TotalCashing));
                    errorMessage += Messages.LegalReglementationsNotificationsCashingTotal + Environment.NewLine;
                }

                if (VMUtils.LegalLimits.DailyCashingActive && CashBookEntries.Any(p => p.Incasari > VMUtils.LegalLimits.DailyCashing))
                {
                    var count = CashBookEntries.Count(p => p.Plati > VMUtils.LegalLimits.DailyCashing);
                    Logger.Instance.Log.Debug(string.Format("CashBookEntries contains {0} entries with Incasari > {1}", count, VMUtils.LegalLimits.DailyCashing));
                    errorMessage += Messages.LegalReglementationsNotificationsCashingDaily + Environment.NewLine;
                }

                if (VMUtils.LegalLimits.TotalBalanceActive && TotalBalance > VMUtils.LegalLimits.TotalBalance)
                {
                    Logger.Instance.Log.Debug(string.Format("TotalBalance  {0} > {1}", TotalBalance, VMUtils.LegalLimits.TotalBalance));
                    errorMessage += Messages.LegalReglementationsNotificationsTotalBalance + Environment.NewLine;
                }

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    WindowHelper.OpenErrorDialog(errorMessage);
                }
            }

            bool moneyExchangeRateOk = true;
            try
            {
                var testMoneyExchangeRate = DecimalConvertor.Instance.StringToDecimal(MoneyExchangeRateString);
            }
            catch (Exception)
            {
                moneyExchangeRateOk = false;
            }

            if (!moneyExchangeRateOk)
            {
                WindowHelper.OpenInformationDialog("Cursul valutar este invalid!");
                return false;
            }

            //force completion of money exchange rate
            if (!SelectedCashBook.IsLei)
            {
                if (MoneyExchangeRate == 0)
                {
                    WindowHelper.OpenErrorDialog("Atentie! Cursul valutar trebuie completat");
                    return false;
                }
                if (MoneyExchangeRate < 0)
                {
                    WindowHelper.OpenErrorDialog("Cursul valutar nu poate fi negativ");
                    return false;
                }

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

            //check if there are duplicate NrActCasa for Receivables
            List<String> duplicates = CashBookEntries.Where(p => p.Incasari != 0).GroupBy(x => x.NrActCasa.ToLower())
                            .Where(g => g.Count() > 1)
                            .Select(g => g.Key)
                            .ToList();
            if (duplicates.Count > 0)
            {
                WindowHelper.OpenErrorDialog(string.Format("Nu se poate sa aveti doua acte de casa cu acelasi numar pentru incasari ({0})", duplicates[0]));
                return false;
            }
            //also check for duplicates for all the current year
            //DateTime? duplicateDate = null;
            //string duplicateCashBook = "";
            //var currentEntries = CashBookEntries.Select(p => p.NrActCasa).ToList();
            //CashBookEntry existingDuplicate = cashBookEntryRepository.GetDuplicateEntryByNumber(currentEntries, SelectedCashBook.Id, Utils.DateTimeToDay(SelectedDate), out duplicateDate, out duplicateCashBook);
            //if (existingDuplicate != null && duplicateDate.HasValue)
            //{
            //    //var cashBookRepository.Get(existingDuplicate.RegistruCasaZiId);

            //    WindowHelper.OpenErrorDialog(string.Format("Nu se poate sa aveti doua acte de casa cu acelasi numar pentru incasari ({0}, in data de {1}, registrul {2})", existingDuplicate.NrActCasa, Utils.DateTimeToStringDateOnly(duplicateDate.Value), duplicateCashBook));
            //    return false;
            //}
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

        CashBookEntryUI itemToDelete = null;
        public void Delete(object param)
        {
            itemToDelete = param as CashBookEntryUI;
            Mediator.Instance.Register(MediatorActionType.YesNoPopupResponse, YesNoPopupResponseCallback);
            Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.YesNoDialog);
            Mediator.Instance.SendMessage(MediatorActionType.SetMessage, "Doriti sa stergeti intrarea?");
        }

        public bool CanDelete(object param)
        {
            return true;
        }

        public void YesNoPopupResponseCallback(object param)
        {
            try
            {
                Mediator.Instance.Unregister(MediatorActionType.YesNoPopupResponse, YesNoPopupResponseCallback);
                switch ((YesNoPopupResponse)param)
                {
                    case YesNoPopupResponse.Yes:
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

                        break;
                    case YesNoPopupResponse.No:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.CannotDeleteCashBook);
            }
        }

        private bool CanViewReports(object parameter)
        {
            return true;
        }

        private void ViewReports(object parameter)
        {
            if (IsModified())
            {
                Mediator.Instance.Register(MediatorActionType.YesNoPopupResponse, YesNoPopupResponseCallbackModifications);
                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.YesNoDialog);
                Mediator.Instance.SendMessage(MediatorActionType.SetMessage, "Aveti modificari pe care nu le-ati salvat.\r\n Doriti sa parasiti ecranul?");
            }
            else
            {
                ViewReportsAction();
            }
        }

        private void ViewReportsAction()
        {
            Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.Reports);
            Mediator.Instance.SendMessage(MediatorActionType.SetSelectedDate, new ReportInitialData()
            {
                SelectedDate = SelectedDate,
                CashBook = SelectedCashBook
            });

        }

        public void YesNoPopupResponseCallbackModifications(object param)
        {
            try
            {
                Mediator.Instance.Unregister(MediatorActionType.YesNoPopupResponse, YesNoPopupResponseCallbackModifications);
                switch ((YesNoPopupResponse)param)
                {
                    case YesNoPopupResponse.Yes:
                        ViewReportsAction();
                        break;
                    case YesNoPopupResponse.No:

                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.GenericError);
            }
        }

        private bool IsModified()
        {
            if (CashBookEntries != null && CashBookEntries.Count > 0)
            {
                if (InitialCashBookEntries != null && InitialCashBookEntries.Count > 0)
                {
                    if (CashBookEntries.Count != InitialCashBookEntries.Count)
                    {
                        return true;
                    }

                    for (int i = 0; i < CashBookEntries.Count; i++)
                    {
                        var old = InitialCashBookEntries[i];
                        var newItem = CashBookEntries[i];
                        if (old.Explicatii != newItem.Explicatii || old.Incasari != newItem.Incasari || old.NrActCasa != newItem.NrActCasa || old.NrAnexe != newItem.NrAnexe ||
                            old.Plati != newItem.Plati)
                        // old.NrCrt != newItem.NrCrt ||
                        {
                            return true;
                        }
                    }
                }

            }
            if (InitialMoneyExchangeRate != MoneyExchangeRate)
            {
                return true;
            }

            return false;
        }

        public override void Dispose()
        {
            CashBookEntries.CollectionChanged -= CashBookEntries_CollectionChanged;
            Mediator.Instance.Unregister(MediatorActionType.RefreshList, RefreshList);
            Mediator.Instance.Unregister(MediatorActionType.SetSelectedCashBook, SetSelectedCashBook);
            Mediator.Instance.Unregister(MediatorActionType.UpdateBalance, UpdateBalance);
            Mediator.Instance.Unregister(MediatorActionType.YesNoPopupResponse, YesNoPopupResponseCallback);
            Mediator.Instance.Unregister(MediatorActionType.YesNoPopupResponse, YesNoPopupResponseCallbackModifications);
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
