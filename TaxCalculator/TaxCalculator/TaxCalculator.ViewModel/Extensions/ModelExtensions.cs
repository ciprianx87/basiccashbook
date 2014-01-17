using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.ViewModel.ViewModels.Model;
using TaxCalculator.Data.Model;
using System.Windows;
using TaxCalculator.Common;

namespace TaxCalculator.ViewModel.Extensions
{
    public static class Extensions
    {
        public static List<TaxIndicator> ToModelList(this List<TaxIndicatorViewModel> list)
        {
            var taxIndicators = new List<TaxIndicator>();
            taxIndicators.AddRange(list.Select(p => ToModel(p)));
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
                InnerId = item.InnerId,
                InitialNrCrt = item.InitialNrCrt,
                InitialType = item.InitialType
            };
            result.UpdateEnabledState();
            return result;
        }

        public static TaxIndicator ToModel(this TaxIndicatorViewModel item)
        {
            var result = new TaxIndicator()
            {
                NrCrt = item.NrCrt,
                NrCrtString = item.NrCrtString,
                Description = item.Description,
                TypeDescription = item.TypeDescription,
                IndicatorFormula = item.IndicatorFormula,
                //Type = VmUtils.GetIndicatorType(item.TypeDescription),
                Type = item.Type,
                InnerId = item.InnerId,
                InitialNrCrt = item.InitialNrCrt,
                InitialType = item.InitialType
                // Value = item.ValueField
            };


            return result;
        }

        public static List<TaxCalculationsViewModel> ToVmList(this List<TaxCalculations> list)
        {
            var taxIndicators = new List<TaxCalculationsViewModel>();
            taxIndicators.AddRange(list.Select(p => ToVm(p)));
            return taxIndicators;
        }

        public static TaxCalculationsViewModel ToVm(this TaxCalculations item)
        {
            TaxCalculationOtherData otherData = VmUtils.Deserialize<TaxCalculationOtherData>(item.OtherData);

            var result = new TaxCalculationsViewModel()
            {
                Id = item.Id,
                CoinType = otherData.CoinType,
                CreatedBy = otherData.CreatedBy,
                ExchangeRate = otherData.ExchangeRate,
                Month = otherData.Month,
                Name = otherData.Name,
                NrOfDecimals = otherData.NrOfDecimals,
                Rectifying = item.Rectifying,
                VerifiedBy = otherData.VerifiedBy,
                Year = otherData.Year,
                LastModifiedDate = otherData.LastModifiedDate == DateTime.MinValue ? "" : Utils.DateTimeToString(otherData.LastModifiedDate)
            };

            return result;
        }


        public static List<CompletedIndicatorVm> ToCompletedList(this List<TaxIndicatorViewModel> list)
        {
            var taxIndicators = new List<CompletedIndicatorVm>();
            taxIndicators.AddRange(list.Select(p => ToCompleted(p)));
            return taxIndicators;
        }

        public static CompletedIndicatorVm ToCompleted(this TaxIndicatorViewModel item)
        {
            var result = new CompletedIndicatorVm()
            {
                NrCrt = item.NrCrtString,
                Description = item.Description,
                Value = item.ValueField,
                Type = item.Type,
                InnerId = item.InnerId,
                IndicatorFormula = item.IndicatorFormula
            };

            return result;
        }

        public static List<TaxIndicatorViewModel> ToVmList(this List<CompletedIndicatorVm> list)
        {
            var taxIndicators = new List<TaxIndicatorViewModel>();
            taxIndicators.AddRange(list.Select(p => ToVm(p)));
            return taxIndicators;
        }

        public static TaxIndicatorViewModel ToVm(this CompletedIndicatorVm item)
        {
            if (item.Value != null) item.Value = item.Value.Replace(".", string.Empty);
            var result = new TaxIndicatorViewModel()
            {
                NrCrt = string.IsNullOrEmpty(item.NrCrt) ? new Nullable<int>() : Convert.ToInt32(item.NrCrt),
                Description = item.Description,
                ValueField = item.Value,
                Type = item.Type,
                InnerId = item.InnerId,
                Style = VmUtils.GetStyleInfo(item.Type),
                IndicatorFormula = item.IndicatorFormula
            };
            //result.ValueField=
            return result;
        }

        public static PrintRow ToPrintRow(this CompletedIndicatorVm item)
        {
            var result = new PrintRow()
                {
                    Description = item.Description,
                    NrCrt = item.NrCrt,
                    Value = item.Value,
                    Type = item.Type
                };

            return result;
        }

        public static List<PrintRow> ToPrintRowList(this List<CompletedIndicatorVm> list)
        {
            var resultedList = new List<PrintRow>();
            resultedList.AddRange(list.Select(p => p.ToPrintRow()));
            return resultedList;
        }
    }
}
