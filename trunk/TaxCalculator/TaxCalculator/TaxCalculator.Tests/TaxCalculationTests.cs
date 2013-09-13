using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaxCalculator.ViewModel.ViewModels.Model;

namespace TaxCalculator.Tests
{
    [TestClass]
    public class TaxCalculationTests
    {
        [TestMethod]
        public void FormulaBuilderTests()
        {
            string formula = "rd1.(1-2)";
            TaxFormula taxFormula = new TaxFormula(formula);

        }
    }
}
