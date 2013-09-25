using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxCalculator.Data.Model
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
        LegalLimitations,
        TaxIndicatorList,
        TaxCalculation,
        TaxCalculationTest,
        CompanyList,
        EditIndicators
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
