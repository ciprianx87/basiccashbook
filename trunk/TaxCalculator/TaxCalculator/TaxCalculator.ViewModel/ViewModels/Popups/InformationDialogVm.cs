using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.ViewModel.Base;
using TaxCalculator.Common.Mediator;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TaxCalculator.Common;

namespace TaxCalculator.ViewModel.ViewModels.Popups
{
    public class InformationDialogVm : BaseViewModel, IDisposable
    {
        #region Fields
        DispatcherTimer dt;
        #endregion

        #region Constructor

        public InformationDialogVm(bool modal)
        {
            InitializeCommands();
            Mediator.Instance.Register(MediatorActionType.SetInformationPopupMessage, SetInformationPopupMessage);
            Mediator.Instance.Register(MediatorActionType.CloseInformationPopup, CloseInformationPopup);
            this.Title = "Informatie";
            OkButtonVisibility = modal ? Visibility.Visible : Visibility.Collapsed;
            if (!modal)
            {
                dt = new DispatcherTimer(DispatcherPriority.Normal);
                dt.Tick += new EventHandler(dt_Tick);
                dt.Interval = TimeSpan.FromMilliseconds(AppSettings.InformationPopupCloseInterval);
                dt.Start();
            }
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

        private Visibility okButtonVisibility;
        public Visibility OkButtonVisibility
        {
            get { return okButtonVisibility; }
            set
            {
                if (okButtonVisibility != value)
                {
                    okButtonVisibility = value;
                    this.NotifyPropertyChanged("OkButtonVisibility");
                }
            }
        }

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


        public void CloseInformationPopup(object param)
        {
            OK(null);
        }
        public void SetInformationPopupMessage(object param)
        {
            this.Message = param.ToString();
        }

        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            Mediator.Instance.Unregister(MediatorActionType.SetInformationPopupMessage, SetInformationPopupMessage);
            Mediator.Instance.Unregister(MediatorActionType.CloseInformationPopup, CloseInformationPopup);
        }

        #endregion

    }
}
