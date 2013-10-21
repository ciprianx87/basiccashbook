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
        ITaxCalculationsRepository taxCalculationRepository;
        DispatcherTimer dt;
        private bool isRectifying = false;

        public ICommand SaveCommand { get; set; }
        public ICommand CreateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand EditIndicatorsCommand { get; set; }
        public ICommand ViewCommand { get; set; }


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

            Mediator.Instance.Register(MediatorActionType.RefreshList, RefreshList);

            taxCalculationRepository = new TaxCalculationsRepository();

            RefreshList(null);
        }

        #region properties

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
            LoadData();

        }


        private void LoadData()
        {
            try
            {
                TaxCalculationList = new ObservableCollection<TaxCalculationsViewModel>();
                var existingIndicators = taxCalculationRepository.GetAll().Where(p => p.Rectifying == isRectifying).ToList().ToVmList();
                if (existingIndicators != null)
                {
                    foreach (var item in existingIndicators)
                    {
                        //TaxCalculationOtherData otherData = VmUtils.Deserialize<TaxCalculationOtherData>(item.ot);
                        TaxCalculationList.Add(item);
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
