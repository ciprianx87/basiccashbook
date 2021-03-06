﻿using CashBook.Data.Interfaces;
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
using System.Threading;
using System.Windows.Threading;
using System.IO;

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
            this.Title = "Registre de casa";
            SaveCommand = new DelegateCommand(Save, CanSave);
            CreateCommand = new DelegateCommand(Create, CanCreate);
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
            EditCommand = new DelegateCommand(Edit, CanEdit);
            SelectCommand = new DelegateCommand(Select, CanSelect);

            Mediator.Instance.Register(MediatorActionType.SetCashBookListType, SetCashBookListType);
            Mediator.Instance.Register(MediatorActionType.RefreshList, RefreshList);
            Mediator.Instance.Register(MediatorActionType.SetRemainingDays, SetRemainingDays);

            cashBookRepository = new CashBookRepository();
            //set the message to the previously existing one (if any)
            RemainingDays = CurrentRemainingDaysMessage;

            //SetRemainingDays(Constants.RemainingDays);
        }

        #region properties

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


        private string remainingDays;
        public string RemainingDays
        {
            get { return remainingDays; }
            set
            {
                if (remainingDays != value)
                {
                    remainingDays = value;
                    this.NotifyPropertyChanged("RemainingDays");
                }
            }
        }

        #endregion

        #region methods
        CashBookListType cashBookListType;
        public void SetCashBookListType(object param)
        {
            if (param is CashBookListType)
            {
                cashBookListType = (CashBookListType)param;
                RefreshList(null);
                //LoadData();
            }
        }
        public static string CurrentRemainingDaysMessage = "";
        public void SetRemainingDays(object param)
        {
            RemainingDays = "Zile ramase: " + param;
            CurrentRemainingDaysMessage = RemainingDays;
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
        private void UpdateLegalReglementations()
        {
            var settingsRepository = new SettingsRepository();
            var currentSetting = settingsRepository.GetSetting(Constants.LegalRelementationsKey);
            if (currentSetting == null)
            {
                var sr = new StreamReader(File.Open("DefaultLegalReglementations.txt", FileMode.Open, FileAccess.Read));
                string legalReglementations = sr.ReadToEnd();
                settingsRepository.AddOrUpdateSetting(Constants.LegalRelementationsKey, legalReglementations);
                try
                {
                    sr.Close();
                }
                catch
                {
                }
            }
        }

        DispatcherTimer dt;
        private void LoadData()
        {

            //IsBusy = true;
            //Thread.Sleep(3000);
            try
            {
                var settingsRepository = new SettingsRepository();
                VMUtils.LegalLimits = VMUtils.GetLegalLimits(settingsRepository);

                UpdateLegalReglementations();

                var tempCashBooks = new List<UserCashBook>();
                CashBooks = new ObservableCollection<UserCashBook>();
                var existingCashBooks = cashBookRepository.GetAll(cashBookListType);
                if (existingCashBooks != null)
                {
                    foreach (var item in existingCashBooks)
                    {
                        item.InitialBalanceDateString = item.InitialBalanceDate.HasValue ? Utils.DateTimeToStringDateOnly(item.InitialBalanceDate.Value) : "";
                        item.InitialBalanceString = DecimalConvertor.Instance.DecimalToString(item.InitialBalance);
                        DateTime? lastDateWithEntries = null;
                        decimal currentBalanceForDay = cashBookRepository.GetCurrentBalanceForDay(item.Id, DateTime.Now, out lastDateWithEntries);
                        item.CurrentBalanceString = DecimalConvertor.Instance.DecimalToString(currentBalanceForDay, item.CoinDecimals);
                        if (lastDateWithEntries.HasValue)
                        {
                            item.LastDateTimeWithEntriesString = Utils.DateTimeToStringDateOnly(lastDateWithEntries.Value);
                        }
                        else
                        {
                            if (item.InitialBalanceDate.HasValue)
                            {
                                item.LastDateTimeWithEntriesString = Utils.DateTimeToStringDateOnly(item.InitialBalanceDate.Value);
                                //item.LastDateTimeWithEntriesString = "";
                            }
                        }
                        CashBooks.Add(item);
                    }

                    //Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                    //{
                    //    foreach (var item in tempCashBooks)
                    //    {
                    //        CashBooks.Add(item);
                    //    }
                    //}));
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
                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.CreateOrEditCashBook);
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
                Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.CreateOrEditCashBook);
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
            Mediator.Instance.Unregister(MediatorActionType.RefreshList, RefreshList);
            Mediator.Instance.Unregister(MediatorActionType.SetCashBookListType, SetCashBookListType);
            Mediator.Instance.Unregister(MediatorActionType.SetRemainingDays, SetRemainingDays);
            base.Dispose();
        }

        #endregion

    }
}
