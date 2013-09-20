using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.ViewModel.Base;
using TaxCalculator.Common.Mediator;
using System.Windows.Input;

namespace TaxCalculator.ViewModel.ViewModels.Popups
{
    public class ErrorDialogVm : BaseViewModel, IDisposable
    {
        #region Fields
        #endregion

        #region Constructor

        public ErrorDialogVm()
        {
            InitializeCommands();
            Mediator.Instance.Register(MediatorActionType.SetErrorDialogMessage, SetErrorDialogMessage);
            this.Title = "Eroare";
        }

        private void InitializeCommands()
        {
            this.OkCommand = new DelegateCommand(OK, CanOK);
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
            Mediator.Instance.SendMessage(MediatorActionType.CloseWindow, this.Guid);
        }

        #endregion

        #region Methods


        public void SetErrorDialogMessage(object param)
        {
            this.Message = param.ToString();
        }
        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.SetErrorDialogMessage, SetErrorDialogMessage);
        }

        #endregion

    }
}
