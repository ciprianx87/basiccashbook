using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Data.Model;
using System.Windows;

namespace TaxCalculator.ViewModel.ViewModels.Model
{
    public class PrintDataModel
    {
        public List<PrintPage> Pages { get; set; }
    }

    public class PrintPage
    {
        public bool FirstPage { get; set; }
        public int PageNumber { get; set; }
        public FirstPageData FirstPageData { get; set; }
        public List<PrintRow> Rows { get; set; }
        public Visibility RectifyingVisibility { get; set; }

    }
    public class PrintRow
    {
        public string Description { get; set; }
        public string NrCrt { get; set; }
        public string Value { get; set; }
        public TaxIndicatorType Type { get; set; }

    }
    public class FirstPageData
    {
        public string Company { get; set; }
        public string Address { get; set; }
        public string Cui { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string CoinType { get; set; }
        public string MonthYear
        {
            get
            {
                return string.Format("{0} {1}", Month, Year);
            }
        }
    }
}
