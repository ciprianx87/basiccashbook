using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CashBook.ViewModels;
using CashBook.Common.Mediator;
using CashBook.Data.Model;
using CashBook.Data;

namespace CashBook.ViewModels
{
    public class YesNoDialogVM : BaseViewModel, IDisposable
    {
        #region Fields
        private UserCashBook entityToDelete;
        #endregion

        #region Constructor

        public YesNoDialogVM()
        {
            this.Title = "Stergere";
           
            InitializeCommands();
            Mediator.Instance.Register(MediatorActionType.SetMessage, SetMessage);
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
            Mediator.Instance.SendMessage(MediatorActionType.YesNoPopupResponse, YesNoPopupResponse.Yes);

            Mediator.Instance.SendMessage(MediatorActionType.CloseWindow, this.Guid);
        }

        public ICommand CancelCommand { get; set; }

        private bool CanCancel(object parameter)
        {
            return true;
        }

        private void Cancel(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.YesNoPopupResponse, YesNoPopupResponse.No);
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

        public void SetMessage(object param)
        {
            if (param is string)
            {
                this.Message = param.ToString();
            }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.SetMessage, SetMessage);
        }

        #endregion

    }
}
