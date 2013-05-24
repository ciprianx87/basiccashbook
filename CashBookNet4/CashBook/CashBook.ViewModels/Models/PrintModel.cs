using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CashBook.Data.Model;

namespace CashBook.ViewModels.Models
{
  public  class PrintModel
    {
        public bool CurrentMonthSelected { get; set; }
        public bool OtherPeriodSelected { get; set; }
        public bool CurrentDaySelected { get; set; }
        public UserCashBook SelectedCashBook { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
