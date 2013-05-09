using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CashBook.Data.Model;

namespace CashBook.ViewModels.Models
{
    class ReportInitialData
    {
        public DateTime SelectedDate { get; set; }

        public UserCashBook CashBook { get; set; }
    }
}
