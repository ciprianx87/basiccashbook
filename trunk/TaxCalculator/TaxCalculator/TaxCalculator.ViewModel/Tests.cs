using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.ViewModel.ViewModels.Model;

namespace TaxCalculator.ViewModel
{
    class Tests
    {
        List<ViewModels.Model.TaxIndicatorViewModel> taxIndicators;
        internal void PerformTests(List<ViewModels.Model.TaxIndicatorViewModel> list)
        {
            this.taxIndicators = list;
            Test1();
            Test2();
            Test3();
        }

        private void Test1()
        {
            //taxIndicators.First(p => p.NrCrt == 1).ValueField = "5";
            taxIndicators.First(p => p.NrCrt == 2).ValueField = "100";

            //string formula = "rd.(1-2)";
            string formula = "rd.2*16%";
            TaxFormula taxFormula = new TaxFormula(formula);
            var rez = taxFormula.Execute(taxIndicators);
            if (rez != 16)
            {
                throw new Exception();
            }
        }

        private void Test2()
        {
            //taxIndicators.First(p => p.NrCrt == 1).ValueField = "5";
            taxIndicators.First(p => p.NrCrt == 2).ValueField = "10";

            //string formula = "rd.(1-2)";
            string formula = "rd.2*16%";
            TaxFormula taxFormula = new TaxFormula(formula);
            var rez = taxFormula.Execute(taxIndicators);
            if (rez != (decimal)1.6)
            {
                throw new Exception();
            }
        }

        private void Test3()
        {
            taxIndicators.First(p => p.NrCrt == 1).ValueField = "5";
            taxIndicators.First(p => p.NrCrt == 2).ValueField = "10";

            string formula = "rd.(1-2)";
            TaxFormula taxFormula = new TaxFormula(formula);
            var rez = taxFormula.Execute(taxIndicators);
            if (rez != -5)
            {
                throw new Exception();
            }
        }
    }
}
