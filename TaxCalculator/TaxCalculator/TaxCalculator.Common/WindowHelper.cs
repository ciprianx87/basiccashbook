using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Common.Mediator;

namespace TaxCalculator.Common
{
    public static class WindowHelper
    {
        public static void OpenErrorDialog(string message)
        {
            Mediator.Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, Mediator.PopupType.ErrorDialog);
            Mediator.Mediator.Instance.SendMessage(MediatorActionType.SetErrorDialogMessage, message);
        }

        public static void OpenInformationDialog(string message)
        {
            Mediator.Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, Mediator.PopupType.Information);
            Mediator.Mediator.Instance.SendMessage(MediatorActionType.SetInformationPopupMessage, message);
        }

        public static void OpenPaymentInformationDialog(string message)
        {
            Mediator.Mediator.Instance.SendMessage(MediatorActionType.OpenWindow, Mediator.PopupType.PaymentInformation);
            Mediator.Mediator.Instance.SendMessage(MediatorActionType.SetPaymentInformationPopupMessage, message);
        }
    }
}
