using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Data.Interfaces;
using TaxCalculator.ViewModel.Base;
using System.ComponentModel;
using TaxCalculator.Data.Repositories;
using TaxCalculator.Common;
using TaxCalculator.Data.Model;
using TaxCalculator.Common.Mediator;
using System.Windows.Input;

namespace TaxCalculator.ViewModel.ViewModels
{

    public class CreateEditCompanyVm : BaseViewModel, IDataErrorInfo
    {
        ICompanyRepository companyRepository;
        Company currentEntity;

        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public CreateEditCompanyVm()
        {
            Mediator.Instance.Register(MediatorActionType.SetEntityToEdit, SetEntityToEdit);
            SaveCommand = new DelegateCommand(Save, CanSave);
            this.CancelCommand = new DelegateCommand(Cancel, CanCancel);
            companyRepository = new CompanyRepository();

            LoadData();

        }

        #region properties
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                NotifyPropertyChanged("Address");
            }
        }

        private string cui;
        public string Cui
        {
            get { return cui; }
            set
            {
                cui = value;
                NotifyPropertyChanged("Cui");
            }
        }

        #endregion

        #region methods
      
        private bool CanCancel(object parameter)
        {
            return true;
        }

        private void Cancel(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.CloseWindow, this.Guid);
        }

        public void SetEntityToEdit(object param)
        {
            if (param is Company)
            {
                this.Title = "Editare Societate";
                currentEntity = param as Company;
                Name = currentEntity.Nume;
                Cui = currentEntity.CUI;
                Address = currentEntity.Adresa;
            }
            else
            {
                this.Title = "Creare Societate";
                Name = "";
                Cui = "";
                Address = "";
            }
        }


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
                    case "Name":
                        if (string.IsNullOrEmpty(Name))
                        {
                            result = "Camp obligatoriu";
                        }
                        break;
                    case "Address":
                        if (string.IsNullOrEmpty(Address))
                        {
                            result = "Camp obligatoriu";
                        }
                        break;
                    case "Cui":
                        if (string.IsNullOrEmpty(Cui))
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
            //try
            //{
            //    //throw new ArgumentNullException("a");
            //    var currentCompany = companyRepository.GetCompany();
            //    if (currentCompany != null)
            //    {
            //        this.CompanyName = currentCompany.Nume;
            //        this.Address = currentCompany.Adresa;
            //        this.Cui = currentCompany.CUI;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Logger.Instance.LogException(ex);
            //    WindowHelper.OpenErrorDialog("Eroare la preluarea informatiei");
            //}
        }

        public void Save(object param)
        {
            try
            {
                if (IsValid())
                {
                    //update fields
                    if (currentEntity == null)
                    {
                        //create
                        currentEntity = new Company();
                        UpdateFields();
                        companyRepository.Create(currentEntity);
                    }
                    else
                    {
                        //edit
                        UpdateFields();
                        companyRepository.Edit(currentEntity);
                    }       
                    WindowHelper.OpenInformationDialog("Informatia a fost salvata");
                    Mediator.Instance.SendMessage(MediatorActionType.CloseWindow, this.Guid);
                    Mediator.Instance.SendMessage(MediatorActionType.RefreshList, this.Guid);
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

        private void UpdateFields()
        {
            currentEntity.Nume = Name;
            currentEntity.Adresa = Address;
            currentEntity.CUI = Cui;
        }
        private bool IsValid()
        {
            return !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(Cui) && !string.IsNullOrEmpty(Name);
        }

        public bool CanSave(object param)
        {
            return true;
        }

        public override void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.SetEntityToEdit, SetEntityToEdit);
            base.Dispose();
        }

        #endregion
    }
}
