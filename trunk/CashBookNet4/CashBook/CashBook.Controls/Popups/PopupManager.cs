using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using CashBook.Common.Mediator;
using CashBook.ViewModels;
using CashBook.Common;

namespace CashBook.Controls.Popups
{
    public class PopupManager
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
        Dictionary<Guid, PopupType> existingPopups = new Dictionary<Guid, PopupType>();
        public void OpenWindow(object param)
        {
            if (param is PopupType)
            {
                var popupType = (PopupType)param;
                if (existingPopups.Values.Any(p => p == popupType))
                {
                    //we already have a popup of this type opened
                    return;
                }
                Window popup = null;
                switch (popupType)
                {
                    case PopupType.DeleteDialog:
                        popup = new DeleteDialog();
                        break;
                    case PopupType.InformationModal:
                        popup = new InformationPopup(true);
                        break;
                    case PopupType.Information:
                        popup = new InformationPopup(false);
                        break;
                    case PopupType.ErrorDialog:
                        popup = new ErrorDialog();
                        break;
                    case PopupType.CreateOrEditCashBook:
                        popup = new CreateOrEditCashBook();
                        break;
                    case PopupType.LegalReglementations:
                        popup = new LegalReglementationsPopup();
                        break;
                    case PopupType.PaymentInformation:
                        popup = new PaymentInformationPopup();
                        break;
                    case PopupType.YesNoDialog:
                        popup = new YesNoDialog();
                        break;
                    case PopupType.CrudCoinTypes:
                        popup = new CrudCoinTypes();
                        break;
                    default:
                        break;
                }
                if (popup != null)
                {
                    popup.Loaded += Popup_Loaded;
                    popup.Closed += popup_Closed;
                    popup.Tag = popupType;

                    CenterPopup(popup);
                    popup.Show();
                    //popup.Focus();
                }
            }
        }
        public void CenterPopup(Window popup)
        {
            double primScreenHeight = System.Windows.SystemParameters.FullPrimaryScreenHeight;
            double primScreenWidth = System.Windows.SystemParameters.FullPrimaryScreenWidth;
            popup.Top = (primScreenHeight - popup.Height) / 2;
            popup.Left = (primScreenWidth - popup.Width) / 2;
        }

        void popup_Closed(object sender, EventArgs e)
        {
            try
            {
                var type = (PopupType)(sender as Window).Tag;
                if (existingPopups.ContainsValue(type))
                {
                    var existingPopup = existingPopups.FirstOrDefault(p => p.Value == type);

                    existingPopups.Remove(existingPopup.Key);

                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }


        void Popup_Loaded(object sender, RoutedEventArgs e)
        {
            if ((sender as Window).DataContext is BaseViewModel)
            {
                Guid guid = ((sender as Window).DataContext as BaseViewModel).Guid;
                openedPopups[guid] = sender as Window;
                existingPopups[guid] = (PopupType)(sender as Window).Tag;
            }
        }
        public void CloseWindow(object param)
        {
            if (param is Guid)
            {
                Guid guid = (Guid)param;
                if (openedPopups.ContainsKey(guid))
                {
                    //(openedPopups[guid] as IDisposable).Dispose();
                    (openedPopups[guid].DataContext as IDisposable).Dispose();
                    openedPopups[guid].Close();
                    openedPopups.Remove(guid);
                    existingPopups.Remove(guid);
                }
            }
        }
    }
}
