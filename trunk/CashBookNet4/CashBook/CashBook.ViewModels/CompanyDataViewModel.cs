using CashBook.Data.Interfaces;
using CashBook.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CashBook.ViewModels
{
    public class CompanyDataViewModel : BaseViewModel
    {
        ICompanyRepository companyRepository;
        
        public ICommand SaveCommand { get; set; }
        public CompanyDataViewModel()
        {
            SaveCommand = new DelegateCommand(Save, CanSave);
            companyRepository = new CompanyRepository();
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

        public void Save(object param)
        {
            try
            {
                companyRepository.EditDetails(CompanyName, CompanyCui, CompanyAddress);
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
    }
}
