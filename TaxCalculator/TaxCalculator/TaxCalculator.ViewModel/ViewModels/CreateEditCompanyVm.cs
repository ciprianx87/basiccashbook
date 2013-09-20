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
using TaxCalculator.Common.Exceptions;
using TaxCalculator.ViewModel.ViewModels.Model;

namespace TaxCalculator.ViewModel.ViewModels
{

    public class CreateEditCompanyVm : BaseViewModel
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

        private CompanyViewModel companyViewModel;
        public CompanyViewModel CompanyViewModel
        {
            get { return companyViewModel; }
            set
            {
                if (companyViewModel != value)
                {
                    companyViewModel = value;
                    this.NotifyPropertyChanged("CompanyViewModel");
                }
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
            CompanyViewModel = new CompanyViewModel();
            if (param is Company)
            {
                this.Title = "Editare Societate";
                currentEntity = param as Company;
                CompanyViewModel.Name = currentEntity.Nume;
                CompanyViewModel.Cui = currentEntity.CUI;
                CompanyViewModel.Address = currentEntity.Adresa;
            }
            else
            {
                this.Title = "Creare Societate";
                CompanyViewModel.Name = "";
                CompanyViewModel.Cui = "";
                CompanyViewModel.Address = "";
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
                    if (currentEntity == null || currentEntity.Id == 0)
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
            catch (DuplicateCompanyNameException dcne)
            {
                WindowHelper.OpenErrorDialog("Exista deja o societate cu acelasi nume!");
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog("Eroare la salvarea informatiei");
            }
        }

        private void UpdateFields()
        {
            currentEntity.Nume = CompanyViewModel.Name;
            currentEntity.Adresa = CompanyViewModel.Address;
            currentEntity.CUI = CompanyViewModel.Cui;
        }
        private bool IsValid()
        {
            return !string.IsNullOrEmpty(CompanyViewModel.Address) && !string.IsNullOrEmpty(CompanyViewModel.Cui) && !string.IsNullOrEmpty(CompanyViewModel.Name);
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
