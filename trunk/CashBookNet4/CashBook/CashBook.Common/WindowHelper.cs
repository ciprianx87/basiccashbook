using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CashBook.Common
{

    public static class WindowHelper
    {
        public static void OpenErrorDialog(string message)
        {
            Mediator.Mediator.Instance.SendMessage(Mediator.MediatorActionType.OpenWindow, Mediator.PopupType.ErrorDialog);
            Mediator.Mediator.Instance.SendMessage(Mediator.MediatorActionType.SetErrorDialogMessage, message);
        }

        public static void OpenInformationDialog(string message)
        {
            Mediator.Mediator.Instance.SendMessage(Mediator.MediatorActionType.OpenWindow, Mediator.PopupType.Information);
            Mediator.Mediator.Instance.SendMessage(Mediator.MediatorActionType.SetInformationPopupMessage, message);
        }
    }

}
