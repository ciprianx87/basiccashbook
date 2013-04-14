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

        public ICommand SaveCommand { get; set; }
        public ICommand CreateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand SelectCommand { get; set; }

       
        public CashBookViewModel()
        {
            SaveCommand = new DelegateCommand(Save, CanSave);
            CreateCommand = new DelegateCommand(Create, CanCreate);
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
            EditCommand = new DelegateCommand(Edit, CanEdit);
            SelectCommand = new DelegateCommand(Select, CanSelect);

            this.LegalReglementationsCommand = new DelegateCommand(LegalReglementations, CanLegalReglementations); 

            Mediator.Instance.Register(MediatorActionType.RefreshList, RefreshList);

            cashBookRepository = new CashBookRepository();

            LoadData();
            SelectedDate = DateTime.Now;
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
       
        #endregion

        #region methods
        public void RefreshList(object param)
        {
            LoadData();
        }
        private void LoadData()
        {
            try
            {
                CashBooks = new ObservableCollection<UserCashBook>();
                var existingCashBooks = cashBookRepository.GetAll();
                if (existingCashBooks != null)
                {
                    foreach (var item in existingCashBooks)
                    {
                        CashBooks.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
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

        public void Save(object param)
        {
            try
            {
                //settingsRepository.AddOrUpdateSetting(Constants.LegalRelementationsKey, LegalReglementationsText);
            }
            catch (Exception ex)
            {

            }
        }

        public bool CanSave(object param)
        {
            return true;
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
            Mediator.Instance.Unregister(MediatorActionType.RefreshList, RefreshList);
            base.Dispose();
        }

        #endregion

    }
}
