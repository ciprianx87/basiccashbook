using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxCalculator.Common.Mediator
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
        RefreshCoinTypes,
        StartPrinting,
        SetRemainingDays,
        SetReportsToPreview,
        SetPrintPreviewPrintModel,
        SetPrintControlPrintModel,
        CloseInformationPopup,
        SetCompletedDays,
        ExecuteTaxCalculation,
        SetSaveAsCallBackAction,
        SetTaxIndicatorToEditFormula,
        SetSetupModel,
        SetReportData,

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
        YesNoDialog,
        CrudCoinTypes,
        InformationModal,
        CreateOrEditCompany,
        CreateOrEditIndicator,
        ChooseTaxCompletionName
    }
}
