﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashBook.Data.Model
{
    public enum ContentTypes
    {
        CashBook,
        MainContent,
        CompanyDetails,
        LegalReglementations,
        CashBookList,
        Reports,
        PrintControl,
        PrintPreview,
        LegalLimitations
    }

    public enum CashBookListType
    {
        Lei,
        Other,
        Any
    }

    public enum YesNoPopupResponse
    {
        Yes,
        No
    }
}
