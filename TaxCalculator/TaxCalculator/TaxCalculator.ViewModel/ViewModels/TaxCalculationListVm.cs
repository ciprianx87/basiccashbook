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
using TaxCalculator.ViewModel.ViewModels.Model;
using TaxCalculator.ViewModel.Extensions;


namespace TaxCalculator.ViewModel.ViewModels
{
    public class TaxCalculationListVm : BaseViewModel
    {
        ICompanyRepository companyRepository;
        ITaxCalculationsRepository taxCalculationRepository;
        DispatcherTimer dt;
        private bool isRectifying = false;

        public ICommand SaveCommand { get; set; }
        public ICommand CreateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand EditIndicatorsCommand { get; set; }
        public ICommand ViewCommand { get; set; }
        public ICommand FilterCommand { get; set; }
        public ICommand ClearFiltersCommand { get; set; }

        public TaxCalculationListVm(bool isRectifying)
        {
            this.isRectifying = isRectifying;
            this.Title = "";
            SaveCommand = new DelegateCommand(Save, CanSave);
            CreateCommand = new DelegateCommand(Create, CanCreate);
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
            EditCommand = new DelegateCommand(Edit, CanEdit);
            this.EditIndicatorsCommand = new DelegateCommand(EditIndicators, CanEditIndicators);
            this.ViewCommand = new DelegateCommand(View, CanView);
            this.FilterCommand = new DelegateCommand(Filter, CanFilter);
            this.ClearFiltersCommand = new DelegateCommand(ClearFilters, CanClearFilters); 
            Mediator.Instance.Register(MediatorActionType.RefreshList, RefreshList);

            taxCalculationRepository = new TaxCalculationsRepository();
            companyRepository = new CompanyRepository();
            LoadInitialData();
            //if no company exists, do not continue loading data
            if (SelectedCompany == null)
            {
                return;
            }
            RefreshList(null);

        }

        private void LoadInitialData()
        {
            Years = new ObservableCollection<int>(Constants.AvailableYears);
            SelectedYear = Years[0];
            Months = new ObservableCollection<string>(Constants.AvailableMonths);
            SelectedMonth = Months[0];
            Companies = new ObservableCollection<Company>(companyRepository.GetAll());
            if (Companies.Count > 0)
            {
                SelectedCompany = Companies[0];
            }
            else
            {
                //error message
                WindowHelper.OpenErrorDialog(Messages.Error_NoCompanies);
            }
        }

        #region properties


        private Company selectedCompany;
        public Company SelectedCompany
        {
            get { return selectedCompany; }
            set
            {
                if (selectedCompany != value)
                {
                    selectedCompany = value;
                    this.NotifyPropertyChanged("SelectedCompany");
                }
            }
        }


        private ObservableCollection<Company> companies;
        public ObservableCollection<Company> Companies
        {
            get { return companies; }
            set
            {
                if (companies != value)
                {
                    companies = value;
                    this.NotifyPropertyChanged("Companies");
                }
            }
        }


        private string selectedMonth;
        public string SelectedMonth
        {
            get { return selectedMonth; }
            set
            {
                if (selectedMonth != value)
                {
                    selectedMonth = value;
                    this.NotifyPropertyChanged("SelectedMonth");
                }
            }
        }

        private ObservableCollection<string> months;
        public ObservableCollection<string> Months
        {
            get { return months; }
            set
            {
                if (months != value)
                {
                    months = value;
                    this.NotifyPropertyChanged("Months");
                }
            }
        }

        private ObservableCollection<int> years;
        public ObservableCollection<int> Years
        {
            get { return years; }
            set
            {
                if (years != value)
                {
                    years = value;
                    this.NotifyPropertyChanged("Years");
                }
            }
        }


        private int selectedYear;
        public int SelectedYear
        {
            get { return selectedYear; }
            set
            {
                if (selectedYear != value)
                {
                    selectedYear = value;
                    this.NotifyPropertyChanged("SelectedYear");
                }
            }
        }
        private ObservableCollection<TaxCalculationsViewModel> taxCalculationList;
        public ObservableCollection<TaxCalculationsViewModel> TaxCalculationList
        {
            get { return taxCalculationList; }
            set
            {
                taxCalculationList = value;
                NotifyPropertyChanged("TaxCalculationList");
            }
        }


        #endregion

        #region methods

       

        private bool CanClearFilters(object parameter)
        {
            return true;
        }

        private void ClearFilters(object parameter)
        {
            LoadData(false);

        } 

        private bool CanFilter(object parameter)
        {
            return true;
        }

        private void Filter(object parameter)
        {
            LoadData(true);
        }
        private bool CanView(object parameter)
        {
            return true;
        }

        public void View(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.PrintPreview);
            Mediator.Instance.SendMessage(MediatorActionType.SetReportData, parameter);
        }

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
            LoadData(false);

        }


        private void LoadData(bool useFilters)
        {
            try
            {
                TaxCalculationList = new ObservableCollection<TaxCalculationsViewModel>();
                var existingIndicators = taxCalculationRepository.GetAll().Where(p => p.Rectifying == isRectifying).ToList();
                if (useFilters)
                {
                    ApplyFilter(existingIndicators);
                }
                var existingIndicatorsVm = existingIndicators.ToVmList();
                if (existingIndicatorsVm != null)
                {
                    foreach (var item in existingIndicatorsVm)
                    {
                        TaxCalculationList.Add(item);
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.Error_LoadingData);
            }
            IsBusy = false;
        }

        private void ApplyFilter(List<TaxCalculations> existingIndicators)
        {
            foreach (var item in existingIndicators.ToArray())
            {
                TaxCalculationOtherData otherData = VmUtils.Deserialize<TaxCalculationOtherData>(item.OtherData);
                bool isValid = false;
                isValid = otherData.Month == SelectedMonth && otherData.Year == SelectedYear && item.CompanyId == SelectedCompany.Id;
                if (!isValid)
                {
                    existingIndicators.Remove(item);
                }
            }
        }

        private void ApplyFilter()
        {
            //TaxCalculations t;
            //t.
            //TaxCalculationList=TaxCalculationList.Where(p=>p.Month==SelectedMonth && p.
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
                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.CreateOrEditIndicator);
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
                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.CreateOrEditIndicator);
                Mediator.Instance.SendMessage(MediatorActionType.SetEntityToEdit, param);
            }
            catch (Exception ex)
            {

                Logger.Instance.LogException(ex);
            }
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
                WindowHelper.OpenErrorDialog(Messages.CannotDeleteEntity);
            }
        }

        public bool CanDelete(object param)
        {
            return true;
        }

        private bool CanEditIndicators(object parameter)
        {
            return true;
        }


        public void EditIndicators(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.SetMainContent, ContentTypes.EditIndicators);
            Mediator.Instance.SendMessage(MediatorActionType.SetTaxIndicatorToEditFormula, parameter);

        }

        public override void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.RefreshList, RefreshList);
            base.Dispose();
        }

        #endregion

    }
}
