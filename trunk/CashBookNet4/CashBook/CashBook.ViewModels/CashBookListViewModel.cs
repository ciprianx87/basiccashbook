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
        public CashBookListViewModel()
        {
            SaveCommand = new DelegateCommand(Save, CanSave);
            CreateCommand = new DelegateCommand(Create, CanCreate);
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
                Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.CreateCashBook);
            }
            catch (Exception ex)
            {

            }
        }

        public bool CanCreate(object param)
        {
            return true;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion
    }
}
