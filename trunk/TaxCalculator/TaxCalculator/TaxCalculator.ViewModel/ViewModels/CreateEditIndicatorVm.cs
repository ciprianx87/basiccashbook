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

    public class CreateEditIndicatorVm : BaseViewModel
    {
        IIndicatorRepository indicatorRepository;
        Indicator currentEntity;

        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public CreateEditIndicatorVm()
        {
            Mediator.Instance.Register(MediatorActionType.SetEntityToEdit, SetEntityToEdit);
            SaveCommand = new DelegateCommand(Save, CanSave);
            this.CancelCommand = new DelegateCommand(Cancel, CanCancel);
            indicatorRepository = new IndicatorRepository();

        }

        #region properties

        private IndicatorViewModel indicatorViewModel;
        public IndicatorViewModel IndicatorViewModel
        {
            get { return indicatorViewModel; }
            set
            {
                if (indicatorViewModel != value)
                {
                    indicatorViewModel = value;
                    this.NotifyPropertyChanged("IndicatorViewModel");
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
            IndicatorViewModel = new IndicatorViewModel();
            if (param is Indicator)
            {
                this.Title = "Editare Indicator";
                currentEntity = param as Indicator;
                IndicatorViewModel.Name = currentEntity.Name;
                IndicatorViewModel.IsDefault = currentEntity.IsDefault;
                IndicatorViewModel.IsDefaultEnabled = false;
            }
            else
            {
                this.Title = "Creare Indicator";
                IndicatorViewModel.Name = "";
                IndicatorViewModel.IsDefault = false;
                IndicatorViewModel.IsDefaultEnabled = true;
            }
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
                        currentEntity = new Indicator();
                        UpdateFields();
                        var allItems = indicatorRepository.GetAll();
                        if (allItems.Count == 0)
                        {
                            //force IsDefault;
                            currentEntity.IsDefault = true;
                        }
                        indicatorRepository.Create(currentEntity);
                    }
                    else
                    {
                        //edit
                        UpdateFields();
                        indicatorRepository.Edit(currentEntity);
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
            catch (DuplicateEntityNameException dcne)
            {
                WindowHelper.OpenErrorDialog("Exista deja un indicator cu acelasi nume!");
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog("Eroare la salvarea informatiei");
            }
        }

        private void UpdateFields()
        {
            currentEntity.Name = IndicatorViewModel.Name;
            currentEntity.IsDefault = IndicatorViewModel.IsDefault;
            currentEntity.Content = "";
            currentEntity.CreatedTimestamp = DateTime.Now;
        }
        private bool IsValid()
        {
            return !string.IsNullOrEmpty(IndicatorViewModel.Name);
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
