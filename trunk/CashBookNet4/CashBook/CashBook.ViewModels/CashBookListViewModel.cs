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
    public class CashBookListViewModel : BaseViewModel
    {
        ICashBookRepository cashBookRepository;

        public ICommand SaveCommand { get; set; }
        public ICommand CreateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand SelectCommand { get; set; }
        public CashBookListViewModel()
        {
            SaveCommand = new DelegateCommand(Save, CanSave);
            CreateCommand = new DelegateCommand(Create, CanCreate);
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
            EditCommand = new DelegateCommand(Edit, CanEdit);
            SelectCommand = new DelegateCommand(Select, CanSelect);

            Mediator.Instance.Register(MediatorActionType.RefreshList, RefreshList);

            cashBookRepository = new CashBookRepository();

            LoadData();

        }

        #region properties

        private ObservableCollection<RegistruCasa> cashBooks;
        public ObservableCollection<RegistruCasa> CashBooks
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
                CashBooks = new ObservableCollection<RegistruCasa>();
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
