using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CashBook.ViewModels;
using CashBook.Common.Mediator;
using CashBook.Data.Model;

namespace CashBook.ViewModels
{
    public class DeleteDialogVM : BaseViewModel, IDisposable
    {
        #region Fields
        private RegistruCasa actionToDelete;
        #endregion

        #region Constructor

        public DeleteDialogVM()
        {
            InitializeCommands();
            Mediator.Instance.Register(MediatorActionType.SetActionToDelete, SetActionToDelete);
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
            //ModelHelper.Instance.DeleteUserAction(actionToDelete);
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
        public void SetActionToDelete(object param)
        {
            if (param is RegistruCasa)
            {
                actionToDelete = param as RegistruCasa;
                this.Message = "Sunteti sigur ca doriti sa stergeti " + actionToDelete.Name + " ?";
            }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

    }
}
