using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Data.Model;

namespace TaxCalculator.ViewModel.ViewModels.Model
{
    public class TaxCalculationSetupModel
    {
        public TaxCalculationSetupModel()
        {
            BackToReports = false;
        }
        public Company SelectedCompany { get; set; }
        public Indicator SelectedIndicatorList { get; set; }
        public decimal ExchangeRate { get; set; }
        public string Month { get; set; }
        public bool Rectifying { get; set; }
        public string CoinType { get; set; }
        public byte NrOfDecimals { get; set; }
        public string VerifiedBy { get; set; }
        public string CreatedBy { get; set; }
        public int Year { get; set; }
        public TaxCalculationSelection SelectedTaxCalculation { get; set; }
        public Int64 CompletedTaxIndicatorId { get; set; }
        public bool BackToReports { get; set; }
    }
}
