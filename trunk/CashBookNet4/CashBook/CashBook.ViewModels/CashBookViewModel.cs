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

namespace CashBook.ViewModels
{
    public class CashBookViewModel : BaseViewModel
    {
        ICashBookRepository cashBookRepository;
        ICashBookEntryRepository cashBookEntryRepository;

        public ICommand CreateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand SelectCommand { get; set; }


        public CashBookViewModel()
        {
            CreateCommand = new DelegateCommand(Create, CanCreate);
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
            EditCommand = new DelegateCommand(Edit, CanEdit);
            SelectCommand = new DelegateCommand(Select, CanSelect);

            this.LegalReglementationsCommand = new DelegateCommand(LegalReglementations, CanLegalReglementations);

            Mediator.Instance.Register(MediatorActionType.RefreshList, RefreshList);
            Mediator.Instance.Register(MediatorActionType.SetSelectedCashBook, SetSelectedCashBook);

            cashBookRepository = new CashBookRepository();
            cashBookEntryRepository = new CashBookEntryRepository();

            SelectedDate = DateTime.Now;
            // LoadData();
        }

        private void LoadDataForDay(DateTime dateTime)
        {
            CashBookEntries = new ObservableCollection<CashBookEntry>();
            CashBookEntries.CollectionChanged += CashBookEntries_CollectionChanged;

            var existingCashBookEntries = cashBookEntryRepository.GetEntriesForDay(dateTime);
            if (existingCashBookEntries != null)
            {
                foreach (var item in existingCashBookEntries)
                {
                    CashBookEntries.Add(item);
                }
            }
        }

        void CashBookEntries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //if all the items are valid, save them in the DB
            SaveData();
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
            CashBookEntries.Add(new CashBookEntry());
        }

        public void RefreshList(object param)
        {
            // LoadData();
        }
        UserCashBook selectedCashBook;
        public void SetSelectedCashBook(object param)
        {
            selectedCashBook = param as UserCashBook;
            // LoadData();
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
                cashBookEntryRepository.UpdateRepositoryForDay(selectedCashBook.Id, CashBookEntries.ToList(), SelectedDate);
            }
            catch (Exception ex)
            {

            }
        }

        public void Create(object param)
        {
            try
            {
                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.CreateOrEditCashBook);
                Mediator.Instance.SendMessage(MediatorActionType.SetEntityToEdit, null);
            }
            catch (Exception ex)
            {

            }
        }

        public bool CanEdit(object param)
        {
            return true;
        }
        public void Edit(object param)
        {
            try
            {
                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.CreateOrEditCashBook);
                Mediator.Instance.SendMessage(MediatorActionType.SetEntityToEdit, param);
            }
            catch (Exception ex)
            {

            }
        }

        public bool CanSelect(object param)
        {
            return true;
        }

        public void Select(object param)
        {
            Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.CashBook);
            Mediator.Instance.SendMessage(MediatorActionType.SetCashBookData, param);
        }

        public bool CanCreate(object param)
        {
            return true;
        }


        public void Delete(object param)
        {
            Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.DeleteDialog);
            Mediator.Instance.SendMessage(MediatorActionType.SetEntityToDelete, param);
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
            base.Dispose();
        }

        #endregion

    }
}
