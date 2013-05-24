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
    public class InformationDialogVM : BaseViewModel, IDisposable
    {
        #region Fields
        DispatcherTimer dt;
        #endregion
       
        #region Constructor

        public InformationDialogVM()
        {
            InitializeCommands();
            Mediator.Instance.Register(MediatorActionType.SetInformationPopupMessage, SetInformationPopupMessage);
            this.Title = "Informatie";
          
            dt = new DispatcherTimer(DispatcherPriority.Normal);
            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = TimeSpan.FromMilliseconds(AppSettings.InformationPopupCloseInterval);
            dt.Start();
        }

        void dt_Tick(object sender, EventArgs e)
        {
            dt.Tick -= new EventHandler(dt_Tick);
            dt.Stop();
            OK(null);
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


        public void SetInformationPopupMessage(object param)
        {
            this.Message = param.ToString();
        }
        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.SetInformationPopupMessage, SetInformationPopupMessage);
        }

        #endregion

    }
}
