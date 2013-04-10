using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using CashBook.Common.Mediator;
using CashBook.ViewModels;

namespace CashBook.Controls.Popups
{
    class PopupManager
    {
        Dictionary<Guid, Window> openedPopups;
       
        #region singleton
        private static PopupManager instance;
        public static PopupManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PopupManager();
                }
                return instance;
            }
        }
        #endregion

        public void Init()
        {
            Mediator.Instance.Register(MediatorActionType.OpenWindow, OpenWindow);
            Mediator.Instance.Register(MediatorActionType.CloseWindow, CloseWindow);
            openedPopups = new Dictionary<Guid, Window>();
        }

        public void OpenWindow(object param)
        {
            if (param is PopupType)
            {
                switch ((PopupType)param)
                {                  
                    case PopupType.DeleteDialog:
                        DeleteDialog delete = new DeleteDialog();
                        delete.Loaded += Popup_Loaded;
                        delete.Show();
                        break;                   
                    case PopupType.Information:
                        InformationPopup information = new InformationPopup();
                        information.Loaded += Popup_Loaded;
                        information.Show();
                        break;
                    default:
                        break;
                }
            }
        }

        void Popup_Loaded(object sender, RoutedEventArgs e)
        {
            if ((sender as Window).DataContext is BaseViewModel)
            {
                Guid guid = ((sender as Window).DataContext as BaseViewModel).Guid;
                openedPopups[guid] = sender as Window;
            }
        }
        public void CloseWindow(object param)
        {
            if (param is Guid)
            {
                Guid guid =  (Guid)param;               
                if (openedPopups.ContainsKey(guid))
                {
                    //(openedPopups[guid] as IDisposable).Dispose();
                    (openedPopups[guid].DataContext as IDisposable).Dispose();
                    openedPopups[guid].Close();
                    openedPopups.Remove(guid);
                }
            }
        }
    }
}
