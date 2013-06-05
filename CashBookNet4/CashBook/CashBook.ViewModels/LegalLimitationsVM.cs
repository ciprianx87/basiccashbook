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
            this.Title = "Detalii despre companie";
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


        #endregion

        #region methods
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
                DailyCashing = limitsModel.DailyCashing;
                DailyPayment = limitsModel.DailyPayment;
                TotalBalance = limitsModel.TotalBalance;
                TotalCashing = limitsModel.TotalCashing;
                TotalPayment = limitsModel.TotalPayment;
                DailyCashingActive = limitsModel.DailyCashingActive;
                TotalBalanceActive = limitsModel.TotalBalanceActive;
                TotalCashingActive = limitsModel.TotalCashingActive;
                //throw new ArgumentNullException("a");
                //var currentCompany = companyRepository.GetCompany();
                //if (currentCompany != null)
                //{
                //    this.CompanyName = currentCompany.Nume;
                //    this.CompanyAddress = currentCompany.Adresa;
                //    this.CompanyCui = currentCompany.CUI;
                //}
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
                    XmlSerializer serializer = new XmlSerializer(typeof(LegalLimitsModel));
                    MemoryStream ms = new MemoryStream();
                    serializer.Serialize(ms, limitsModel);
                    var serializedString = Encoding.ASCII.GetString(ms.GetBuffer());

                    //companyRepository.EditDetails(CompanyName, CompanyCui, CompanyAddress);
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
