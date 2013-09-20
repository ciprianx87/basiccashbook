using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.ViewModel.Base;
using TaxCalculator.Common.Mediator;
using System.Windows.Input;
using TaxCalculator.Data.Model;

namespace TaxCalculator.ViewModel.ViewModels.Popups
{
    public class DeleteDialogVM : BaseViewModel, IDisposable
    {
        #region Fields
        private object entityToDelete;
        #endregion

        #region Constructor

        public DeleteDialogVM()
        {
            this.Title = "Stergere";

            InitializeCommands();
            Mediator.Instance.Register(MediatorActionType.SetEntityToDelete, SetEntityToDelete);
        }

        private void InitializeCommands()
        {
            this.OKCommand = new DelegateCommand(OK, CanOK);
            this.CancelCommand = new DelegateCommand(Cancel, CanCancel);
        }

        #endregion

        #region Properties

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                if (message != value)
                {
                    message = value;
                    this.NotifyPropertyChanged("Message");
                }
            }
        }

        public ICommand OKCommand { get; set; }

        private bool CanOK(object parameter)
        {
            return true;
        }

        private void OK(object parameter)
        {
            DeleteEntity();
            Mediator.Instance.SendMessage(MediatorActionType.CloseWindow, this.Guid);
        }

        public ICommand CancelCommand { get; set; }

        private bool CanCancel(object parameter)
        {
            return true;
        }

        private void Cancel(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.CloseWindow, this.Guid);
        }
        #endregion

        #region Methods

        private void DeleteEntity()
        {
            EntityDeleter.DeleteEntity(entityToDelete);
            RefreshList();
        }

        private void RefreshList()
        {
            Mediator.Instance.SendMessage(MediatorActionType.RefreshList, null);

        }

        public void SetEntityToDelete(object param)
        {
            entityToDelete = param;
            if (param is Company)
            {
                var currentEntity = param as Company;
                this.Message = "Sunteti sigur ca doriti sa stergeti " + currentEntity.Nume + " ?".Replace(Environment.NewLine, "");
            }
        }
        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.SetEntityToDelete, SetEntityToDelete);
        }

        #endregion

    }
}
