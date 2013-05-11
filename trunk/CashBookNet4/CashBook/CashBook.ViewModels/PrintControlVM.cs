using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CashBook.ViewModels;
using CashBook.Common.Mediator;
using CashBook.Data.Model;
using CashBook.Data;
using System.Timers;
using System.Windows.Threading;
using CashBook.Common;

namespace CashBook.ViewModels
{
    public class PrintControlVM : BaseViewModel, IDisposable
    {
        #region Fields
        
        #endregion
       
        #region Constructor

        public PrintControlVM()
        {
            InitializeCommands();
            Mediator.Instance.Register(MediatorActionType.SetPaymentInformationPopupMessage, SetPaymentInformationPopupMessage);
            this.Title = "Informatie";          
        }


        private void InitializeCommands()
        {
            this.OkCommand = new DelegateCommand(OK, CanOK);
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

        public ICommand OkCommand { get; set; }

        private bool CanOK(object parameter)
        {
            return true;
        }

        private void OK(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, PopupType.LegalReglementations);
            Mediator.Instance.SendMessage(MediatorActionType.CloseWindow, this.Guid);
        }

        #endregion

        #region Methods


        public void SetPaymentInformationPopupMessage(object param)
        {
            this.Message = param.ToString();
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

        #region IDisposable Members

        public void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.SetPaymentInformationPopupMessage, SetPaymentInformationPopupMessage);
        }

        #endregion

    }
}
