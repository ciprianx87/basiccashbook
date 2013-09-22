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
    public class TaxIndicatorsListVm : BaseViewModel
    {
        IIndicatorRepository indicatorRepository;
        DispatcherTimer dt;


        public ICommand SaveCommand { get; set; }
        public ICommand CreateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }

        public ICommand SetAsDefaultCommand { get; set; }

        public TaxIndicatorsListVm()
        {
            this.Title = "Indicatori";
            SaveCommand = new DelegateCommand(Save, CanSave);
            CreateCommand = new DelegateCommand(Create, CanCreate);
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
            EditCommand = new DelegateCommand(Edit, CanEdit);
            this.SetAsDefaultCommand = new DelegateCommand(SetAsDefault, CanSetAsDefault);

            Mediator.Instance.Register(MediatorActionType.RefreshList, RefreshList);

            indicatorRepository = new IndicatorRepository();

            RefreshList(null);
        }

        #region properties

        private ObservableCollection<Indicator> taxIndicatorList;
        public ObservableCollection<Indicator> TaxIndicatorList
        {
            get { return taxIndicatorList; }
            set
            {
                taxIndicatorList = value;
                NotifyPropertyChanged("TaxIndicatorList");
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
            try
            {
                TaxIndicatorList = new ObservableCollection<Indicator>();
                var existingIndicators = indicatorRepository.GetAll();
                if (existingIndicators != null)
                {
                    foreach (var item in existingIndicators)
                    {
                        TaxIndicatorList.Add(item);
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

        private bool CanSetAsDefault(object parameter)
        {
            return true;
        }

        public void SetAsDefault(object parameter)
        {
            try
            {
                var indicator = parameter as Indicator;
                indicator.IsDefault = true;
                indicatorRepository.Edit(indicator);
                Mediator.Instance.SendMessage (MediatorActionType.RefreshList, null);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog(Messages.GenericError);
            }
        }

        public override void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.RefreshList, RefreshList);
            base.Dispose();
        }

        #endregion

    }
}
