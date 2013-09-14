﻿using System;
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
            Test4();
            Test5();
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

        private void Test4()
        {
            //taxIndicators.First(p => p.NrCrt == 1).ValueField = "5";
            taxIndicators.First(p => p.NrCrt == 2).ValueField = "10";

            //string formula = "rd.(1-2)";
            string formula = "rd.2*3/1000";
            TaxFormula taxFormula = new TaxFormula(formula);
            var rez = taxFormula.Execute(taxIndicators);
            if (rez != (decimal)0.03)
            {
                throw new Exception();
            }
        }

        private void Test5()
        {
            taxIndicators.First(p => p.NrCrt == 1).ValueField = "5";
            taxIndicators.First(p => p.NrCrt == 2).ValueField = "1";
            taxIndicators.First(p => p.NrCrt == 3).ValueField = "2";
            taxIndicators.First(p => p.NrCrt == 4).ValueField = "3";
            taxIndicators.First(p => p.NrCrt == 5).ValueField = "4";

            //string formula = "rd.(1-2)";//4 -3=1
            string formula = "rd.(1-2) -rd.(3-4+5)";
            TaxFormula taxFormula = new TaxFormula(formula);
            var rez = taxFormula.Execute(taxIndicators);
            if (rez != 1)
            {
                throw new Exception();
            }
        }
    }
}
