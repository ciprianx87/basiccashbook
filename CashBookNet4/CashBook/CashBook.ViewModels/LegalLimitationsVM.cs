using CashBook.Data.Interfaces;
using CashBook.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CashBook.Common.Mediator;
using CashBook.Common;
using CashBook.Common.Exceptions;
using System.ComponentModel;
using CashBook.ViewModels.Models;
using System.Xml.Serialization;
using System.IO;

namespace CashBook.ViewModels
{
    public class LegalLimitationsVM : BaseViewModel, IDataErrorInfo
    {
        ISettingsRepository settingsRepository;

        public ICommand SaveCommand { get; set; }
        public LegalLimitationsVM()
        {
            this.Title = "Limitari legislative";
            SaveCommand = new DelegateCommand(Save, CanSave);
            settingsRepository = new SettingsRepository();

            LoadData();

        }

        #region properties

        private int dailyCashing;
        public int DailyCashing
        {
            get { return dailyCashing; }
            set
            {
                if (dailyCashing != value)
                {
                    dailyCashing = value;
                    this.NotifyPropertyChanged("DailyCashing");
                }
            }
        }



        private int totalCashing;
        public int TotalCashing
        {
            get { return totalCashing; }
            set
            {
                if (totalCashing != value)
                {
                    totalCashing = value;
                    this.NotifyPropertyChanged("TotalCashing");
                }
            }
        }


        private int totalPayment;
        public int TotalPayment
        {
            get { return totalPayment; }
            set
            {
                if (totalPayment != value)
                {
                    totalPayment = value;
                    this.NotifyPropertyChanged("TotalPayment");
                }
            }
        }


        private int dailyPayment;
        public int DailyPayment
        {
            get { return dailyPayment; }
            set
            {
                if (dailyPayment != value)
                {
                    dailyPayment = value;
                    this.NotifyPropertyChanged("DailyPayment");
                }
            }
        }

        private int totalBalance;
        public int TotalBalance
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


        private bool dailyCashingActive;
        public bool DailyCashingActive
        {
            get { return dailyCashingActive; }
            set
            {
                if (dailyCashingActive != value)
                {
                    dailyCashingActive = value;
                    this.NotifyPropertyChanged("DailyCashingActive");
                }
            }
        }


        private bool totalCashingActive;
        public bool TotalCashingActive
        {
            get { return totalCashingActive; }
            set
            {
                if (totalCashingActive != value)
                {
                    totalCashingActive = value;
                    this.NotifyPropertyChanged("TotalCashingActive");
                }
            }
        }


        private bool totalBalanceActive;
        public bool TotalBalanceActive
        {
            get { return totalBalanceActive; }
            set
            {
                if (totalBalanceActive != value)
                {
                    totalBalanceActive = value;
                    this.NotifyPropertyChanged("TotalBalanceActive");
                }
            }
        }

        #region string versions

        private int dailyCashingString;
        public int DailyCashingString
        {
            get { return dailyCashingString; }
            set
            {
                if (dailyCashingString != value)
                {
                    dailyCashingString = value;
                    this.NotifyPropertyChanged("DailyCashingString");
                }
            }
        }



        private int totalCashingString;
        public int TotalCashingString
        {
            get { return totalCashingString; }
            set
            {
                if (totalCashingString != value)
                {
                    totalCashingString = value;
                    this.NotifyPropertyChanged("TotalCashingString");
                }
            }
        }


        private int totalPaymentString;
        public int TotalPaymentString
        {
            get { return totalPaymentString; }
            set
            {
                if (totalPaymentString != value)
                {
                    totalPaymentString = value;
                    this.NotifyPropertyChanged("TotalPaymentString");
                }
            }
        }


        private int dailyPaymentString;
        public int DailyPaymentString
        {
            get { return dailyPaymentString; }
            set
            {
                if (dailyPaymentString != value)
                {
                    dailyPaymentString = value;
                    this.NotifyPropertyChanged("DailyPaymentString");
                }
            }
        }

        private int totalBalanceString;
        public int TotalBalanceString
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


        private bool dailyCashingActiveString;
        public bool DailyCashingActiveString
        {
            get { return dailyCashingActiveString; }
            set
            {
                if (dailyCashingActiveString != value)
                {
                    dailyCashingActiveString = value;
                    this.NotifyPropertyChanged("DailyCashingActiveString");
                }
            }
        }


        private bool totalCashingActiveString;
        public bool TotalCashingActiveString
        {
            get { return totalCashingActiveString; }
            set
            {
                if (totalCashingActiveString != value)
                {
                    totalCashingActiveString = value;
                    this.NotifyPropertyChanged("TotalCashingActiveString");
                }
            }
        }


        private bool totalBalanceActiveString;
        public bool TotalBalanceActiveString
        {
            get { return totalBalanceActiveString; }
            set
            {
                if (totalBalanceActiveString != value)
                {
                    totalBalanceActiveString = value;
                    this.NotifyPropertyChanged("TotalBalanceActiveString");
                }
            }
        }

        #endregion
        #endregion

        #region methods
        private void UpdateStringFields()
        {
            //PlatiString = DecimalConvertor.Instance.DecimalToString(Plati);
        }
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get
            {
                string result = GetError(columnName);
                return result;
            }
        }
        private string GetError(string columnName)
        {
            string result = null;
            switch (columnName)
            {
                case "DailyPayment":
                    if (DailyPayment < 0)
                    {
                        result = "Campul trebuie sa fie pozitiv";
                    }
                    break;
                case "TotalPayment":
                    if (TotalPayment < 0)
                    {
                        result = "Campul trebuie sa fie pozitiv";
                    }
                    break;
                case "DailyCashing":
                    if (DailyCashing < 0 && DailyCashingActive)
                    {
                        result = "Campul trebuie sa fie pozitiv";
                    }
                    break;
                case "TotalBalance":
                    if (TotalBalance < 0 && TotalBalanceActive)
                    {
                        result = "Campul trebuie sa fie pozitiv";
                    }
                    break;
                case "TotalCashing":
                    if (TotalCashing < 0 && TotalCashingActive)
                    {
                        result = "Campul trebuie sa fie pozitiv";
                    }
                    break;
            }

            return result;
        }
        private void LoadData()
        {
            try
            {
                LegalLimitsModel limitsModel = VMUtils.GetLegalLimits(settingsRepository);

                DailyCashing = limitsModel.DailyCashing;
                DailyPayment = limitsModel.DailyPayment;
                TotalBalance = limitsModel.TotalBalance;
                TotalCashing = limitsModel.TotalCashing;
                TotalPayment = limitsModel.TotalPayment;
                DailyCashingActive = limitsModel.DailyCashingActive;
                TotalBalanceActive = limitsModel.TotalBalanceActive;
                TotalCashingActive = limitsModel.TotalCashingActive;
            }
            catch (CompanyNotFoundException ce)
            {
                //do not show the error when loading data
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog("Eroare la preluarea informatiei");
            }
        }

        public void Save(object param)
        {
            try
            {
                if (IsValid())
                {
                    LegalLimitsModel limitsModel = new LegalLimitsModel()
                    {
                        DailyCashing = DailyCashing,
                        DailyPayment = DailyPayment,
                        TotalBalance = TotalBalance,
                        TotalCashing = TotalCashing,
                        TotalPayment = TotalPayment,
                        DailyCashingActive = DailyCashingActive,
                        TotalBalanceActive = TotalBalanceActive,
                        TotalCashingActive = TotalCashingActive
                    };
                    VMUtils.SaveLegalLimits(settingsRepository, limitsModel);
                    VMUtils.LegalLimits = VMUtils.GetLegalLimits(settingsRepository);
                    WindowHelper.OpenInformationDialog("Informatia a fost salvata");
                }
                else
                {
                    WindowHelper.OpenErrorDialog("Va rugam completati toate campurile");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog("Eroare la salvarea informatiei");
            }
        }
        private bool IsValid()
        {
            return string.IsNullOrEmpty(GetError("DailyPayment") + GetError("TotalPayment") + GetError("DailyCashing") + GetError("TotalBalance") + GetError("TotalCashing"));
            return true;
            //return !string.IsNullOrEmpty(CompanyAddress) && !string.IsNullOrEmpty(CompanyCui) && !string.IsNullOrEmpty(CompanyName);
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
