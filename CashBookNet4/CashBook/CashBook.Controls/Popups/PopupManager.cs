﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using CashBook.Common.Mediator;
using CashBook.ViewModels;

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
                    case PopupType.Information:
                        popup = new InformationPopup();
                        break;
                    case PopupType.CreateOrEditCashBook:
                        popup = new CreateOrEditCashBook();
                        break;
                    default:
                        break;
                }
                if (popup != null)
                {
                    popup.Loaded += Popup_Loaded;
                    popup.Tag = popupType;
                    popup.Show();
                }
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
