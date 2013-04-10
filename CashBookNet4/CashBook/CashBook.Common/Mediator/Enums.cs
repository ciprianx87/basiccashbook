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
        
    }
    public enum PopupType
    {
        NewAction,
        NewTimer,
        DeleteDialog,
        Information
    }
}
