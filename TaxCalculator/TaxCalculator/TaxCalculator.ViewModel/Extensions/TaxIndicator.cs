using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.ViewModel.ViewModels.Model;
using TaxCalculator.Data.Model;
using System.Windows;

namespace TaxCalculator.ViewModel.Extensions
{
    public static class Extensions
    {
        public static List<TaxIndicator> ToModelList(this List<TaxIndicatorViewModel> list)
        {
            var taxIndicators = new List<TaxIndicator>();
            taxIndicators.AddRange(list.Select(p => ToVm(p)));
            return taxIndicators;
        }

        public static List<TaxIndicatorViewModel> ToVmList(this List<TaxIndicator> list)
        {
            var taxIndicators = new List<TaxIndicatorViewModel>();
            taxIndicators.AddRange(list.Select(p => ToVm(p)));
            return taxIndicators;
        }

        public static TaxIndicatorViewModel ToVm(this TaxIndicator item)
        {
            var result = new TaxIndicatorViewModel()
            {
                NrCrt = item.NrCrt,
                Description = item.Description,
                TypeDescription = item.TypeDescription,
                IndicatorFormula = item.IndicatorFormula,
                Type = VmUtils.GetIndicatorType(item.TypeDescription),
                Style = VmUtils.GetStyleInfo(VmUtils.GetIndicatorType(item.TypeDescription)), 
            };

            return result;
        }

        public static TaxIndicator ToVm(this TaxIndicatorViewModel item)
        {
            var result = new TaxIndicator()
            {
                NrCrt = item.NrCrt,
                NrCrtString = item.NrCrtString,
                Description = item.Description,
                TypeDescription = item.TypeDescription,
                IndicatorFormula = item.IndicatorFormula,
                Type = VmUtils.GetIndicatorType(item.TypeDescription),
            };

            return result;
        }
    }
}
