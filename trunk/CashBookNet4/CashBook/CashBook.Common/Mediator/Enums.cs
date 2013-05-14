using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CashBook.Common.Mediator
{
    public enum MediatorActionType
    {
        OpenWindow,
        CloseWindow,
        SetMainContent,
        SetEntityToDelete,
        SetEntityToEdit,
        RefreshList,
        SetCashBookData,
        SetSelectedCashBook,
        SetInformationPopupMessage,
        SetErrorDialogMessage,
        UpdateBalance,
        SetCashBookListType,
        SetSelectedDate,
        SetPaymentInformationPopupMessage,
        SetTitle,
        SetMessage,
        YesNoPopupResponse,

    }
    public enum PopupType
    {
        NewCashBook,
        DeleteDialog,
        Information,
        CreateOrEditCashBook,
        LegalReglementations,
        ErrorDialog,
        PaymentInformation,
        YesNoDialog
    }
}
