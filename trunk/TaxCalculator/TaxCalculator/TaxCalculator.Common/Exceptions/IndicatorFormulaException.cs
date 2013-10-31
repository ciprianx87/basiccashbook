using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxCalculator.Common.Exceptions
{
    public class IndicatorFormulaException:Exception
    {
        public IndicatorFormulaException(string error)
        {
            this.Error = error;
        }
        public string Error { get; set; }
    }
}
