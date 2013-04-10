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
    public class CreateCashBookViewModel : BaseViewModel
    {
        ICashBookRepository cashBookRepository;

        public ICommand SaveCommand { get; set; }
        public CreateCashBookViewModel()
        {
            SaveCommand = new DelegateCommand(Save, CanSave);
            cashBookRepository = new CashBookRepository();

            CoinDecimals = 2;
            LoadData();

        }

        #region properties


        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private decimal initialBalance;
        public decimal InitialBalance
        {
            get { return initialBalance; }
            set
            {
                initialBalance = value;
                NotifyPropertyChanged("InitialBalance");
            }
        }

        private string cashierName;
        public string CashierName
        {
            get { return cashierName; }
            set
            {
                cashierName = value;
                NotifyPropertyChanged("CashierName");
            }
        }

        private string location;
        public string Location
        {
            get { return location; }
            set
            {
                location = value;
                NotifyPropertyChanged("Location");
            }
        }

        private string account;
        public string Account
        {
            get { return account; }
            set
            {
                account = value;
                NotifyPropertyChanged("Account");
            }
        }

        private byte coinDecimals;
        public byte CoinDecimals
        {
            get { return coinDecimals; }
            set
            {
                coinDecimals = value;
                NotifyPropertyChanged("CoinDecimals");
            }
        }

        private string coinType;
        public string CoinType
        {
            get { return coinType; }
            set
            {
                coinType = value;
                NotifyPropertyChanged("CoinType");
            }
        }

        #endregion

        #region methods
        private void LoadData()
        {

        }

        public void Save(object param)
        {
            try
            {

                RegistruCasa cashBook = new RegistruCasa()
                {
                    Account = Account != null ? Account : "",
                    CashierName = CashierName != null ? CashierName : "",
                    Location = Location != null ? Location : "",
                    CoinType = CoinType,
                    CoinDecimals = CoinDecimals,
                    Name = Name != null ? Name : "",
                    InitialBalance = InitialBalance,
                };
                cashBookRepository.Create(cashBook);

                Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.CashBookList);

            }
            catch (Exception ex)
            {

            }
        }

        public bool CanSave(object param)
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
