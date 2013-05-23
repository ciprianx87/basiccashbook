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

namespace CashBook.ViewModels
{
    public class CompanyDataViewModel : BaseViewModel, IDataErrorInfo
    {
        ICompanyRepository companyRepository;

        public ICommand SaveCommand { get; set; }
        public CompanyDataViewModel()
        {
            this.Title = "Detalii despre companie";
            SaveCommand = new DelegateCommand(Save, CanSave);
            companyRepository = new CompanyRepository();

            LoadData();

        }

        #region properties
        private string companyName;
        public string CompanyName
        {
            get { return companyName; }
            set
            {
                companyName = value;
                NotifyPropertyChanged("CompanyName");
            }
        }

        private string companyAddress;
        public string CompanyAddress
        {
            get { return companyAddress; }
            set
            {
                companyAddress = value;
                NotifyPropertyChanged("CompanyAddress");
            }
        }

        private string companyCui;
        public string CompanyCui
        {
            get { return companyCui; }
            set
            {
                companyCui = value;
                NotifyPropertyChanged("CompanyCui");
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
                string result = null;
                switch (columnName)
                {
                    case "CompanyName":
                        if (string.IsNullOrEmpty(CompanyName))
                        {
                            result = "Camp obligatoriu";
                        }
                        break;
                    case "CompanyAddress":
                        if (string.IsNullOrEmpty(CompanyAddress))
                        {
                            result = "Camp obligatoriu";
                        }
                        break;
                    case "CompanyCui":
                        if (string.IsNullOrEmpty(CompanyCui))
                        {
                            result = "Camp obligatoriu";
                        }
                        break;
                }

                return result;
            }
        }

        private void LoadData()
        {
            try
            {
                //throw new ArgumentNullException("a");
                var currentCompany = companyRepository.GetCompany();
                if (currentCompany != null)
                {
                    this.CompanyName = currentCompany.Nume;
                    this.CompanyAddress = currentCompany.Adresa;
                    this.CompanyCui = currentCompany.CUI;
                }
            }
            catch (CompanyNotFoundException ce)
            {
                //do not show the error when loading data
                //WindowHelper.OpenErrorDialog("Va rugam completati datele despre companie");
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
                    companyRepository.EditDetails(CompanyName, CompanyCui, CompanyAddress);
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
            return !string.IsNullOrEmpty(CompanyAddress) && !string.IsNullOrEmpty(CompanyCui) && !string.IsNullOrEmpty(CompanyName);
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
