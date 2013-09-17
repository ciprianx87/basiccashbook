using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxCalculator.ViewModel.ViewModels.Model
{
    public class ElseStatement
    {
        public ElseStatement(string statement)
        {
            Result = new TaxFormula(statement);

        }
        public TaxFormula Result { get; set; }
        public decimal Execute(List<TaxIndicatorViewModel> taxIndicatorList)
        {
            return Result.Execute(taxIndicatorList);
        }
    }

}
