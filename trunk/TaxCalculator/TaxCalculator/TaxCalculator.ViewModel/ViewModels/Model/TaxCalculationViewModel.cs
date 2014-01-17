using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxCalculator.ViewModel.ViewModels.Model
{
   public class TaxCalculationsViewModel
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string CoinType { get; set; }
        public decimal ExchangeRate { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public int NrOfDecimals { get; set; }
        public string CreatedBy { get; set; }
        public string VerifiedBy { get; set; }
        public bool Rectifying { get; set; }
        public string LastModifiedDate { get; set; }
    }
}
