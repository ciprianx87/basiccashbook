using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Data.Model;

namespace TaxCalculator.ViewModel.ViewModels.Model
{
   public class CompletedIndicatorVm
    {
        public string NrCrt { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public TaxIndicatorType Type { get; set; }

    }
   public class CompletedIndicatorDbModel
   {
       public List<CompletedIndicatorVm> CompletedIndicators { get; set; }
       public Int64? PreviousIndicatorId { get; set; }
   }
}
