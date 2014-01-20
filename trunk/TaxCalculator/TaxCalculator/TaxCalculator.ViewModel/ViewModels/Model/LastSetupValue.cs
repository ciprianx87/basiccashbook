using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxCalculator.ViewModel.ViewModels.Model
{
    class LastSetupValue
    {
        public string CreatedBy { get; set; }
        public string VerifiedBy { get; set; }

        public long IndicatorListId { get; set; }
    }

    class LastSetupValueEntireScreen
    {
        public string CreatedBy { get; set; }
        public string VerifiedBy { get; set; }

        public long IndicatorListId { get; set; }
        public long CompanyId { get; set; }
        public byte NrDecimals { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        
    }
}
