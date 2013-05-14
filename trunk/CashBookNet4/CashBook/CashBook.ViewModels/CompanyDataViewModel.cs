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

namespace CashBook.ViewModels
{
    public class CompanyDataViewModel : BaseViewModel
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
                companyRepository.EditDetails(CompanyName, CompanyCui, CompanyAddress);
                WindowHelper.OpenInformationDialog("Informatia a fost salvata");
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog("Eroare la salvarea informatiei");
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
