﻿using System;
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

    }
    public enum PopupType
    {
        NewCashBook,
        DeleteDialog,
        Information,
        CreateOrEditCashBook,
        LegalReglementations
    }
}
