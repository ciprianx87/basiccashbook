using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.ViewModel.Base;
using System.Windows.Input;
using TaxCalculator.Common.Mediator;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using TaxCalculator.Common;
using TaxCalculator.Data.Model;
using TaxCalculator.Data.Repositories;
using TaxCalculator.Data.Interfaces;

namespace TaxCalculator.ViewModel.ViewModels
{
    public class CompanyListVm : BaseViewModel
    {
        ICompanyRepository companyRepository;
        DispatcherTimer dt;


        public ICommand SaveCommand { get; set; }
        public ICommand CreateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand SelectCommand { get; set; }
        public CompanyListVm()
        {
            this.Title = "Societati";
            SaveCommand = new DelegateCommand(Save, CanSave);
            CreateCommand = new DelegateCommand(Create, CanCreate);
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
            EditCommand = new DelegateCommand(Edit, CanEdit);
            SelectCommand = new DelegateCommand(Select, CanSelect);

            //Mediator.Instance.Register(MediatorActionType.SetCashBookListType, SetCashBookListType);
            Mediator.Instance.Register(MediatorActionType.RefreshList, RefreshList);

            companyRepository = new CompanyRepository();

            RefreshList(null);
        }

        #region properties

        private ObservableCollection<Company> companies;
        public ObservableCollection<Company> Companies
        {
            get { return companies; }
            set
            {
                companies = value;
                NotifyPropertyChanged("Companies");
            }
        }


        #endregion

        #region methods

        public void RefreshList(object param)
        {
            IsBusy = true;
            Dispatcher.CurrentDispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
            dt = new DispatcherTimer();
            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = TimeSpan.FromMilliseconds(500);
            dt.Start();
        }

        void dt_Tick(object sender, EventArgs e)
        {
            dt.Tick -= new EventHandler(dt_Tick);
            dt.Stop();
            LoadData();

        }


        private void LoadData()
        {
            //IsBusy = true;
            //Thread.Sleep(3000);
            try
            {
                Companies = new ObservableCollection<Company>();
                var existingCompanies = companyRepository.GetAll();
                if (existingCompanies != null)
                {
                    foreach (var item in existingCompanies)
                    {
                        Companies.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog("Nu s-au putut citi datele");
            }
            IsBusy = false;
        }

        public void Save(object param)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);

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
                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.CreateOrEditCompany);
                Mediator.Instance.SendMessage(MediatorActionType.SetEntityToEdit, null);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
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
                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.CreateOrEditCompany);
                Mediator.Instance.SendMessage(MediatorActionType.SetEntityToEdit, param);
            }
            catch (Exception ex)
            {

                Logger.Instance.LogException(ex);
            }
        }

        public bool CanSelect(object param)
        {
            return true;
        }

        public void Select(object param)
        {
            Logger.Instance.Log.Debug(string.Format("param != null {0}", param != null));
            Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.CashBook);
            Mediator.Instance.SendMessage(MediatorActionType.SetSelectedCashBook, param);
        }

        public bool CanCreate(object param)
        {
            return true;
        }


        public void Delete(object param)
        {
            try
            {
                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.DeleteDialog);
                Mediator.Instance.SendMessage(MediatorActionType.SetEntityToDelete, param);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.CannotDeleteCashBook);
            }
        }

        public bool CanDelete(object param)
        {
            return true;
        }

        public override void Dispose()
        {
            //Mediator.Instance.Unregister(MediatorActionType.RefreshList, RefreshList);
            //Mediator.Instance.Unregister(MediatorActionType.SetCashBookListType, SetCashBookListType);
            base.Dispose();
        }

        #endregion

    }
}
