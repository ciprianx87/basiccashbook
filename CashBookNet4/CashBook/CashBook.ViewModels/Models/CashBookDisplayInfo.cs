using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CashBook.Data.Model;

namespace CashBook.ViewModels.Models
{
    class CashBookDisplayInfo
    {
        public UserCashBook SelectedCashbook { get; set; }
        public DateTime SelectedDate { get; set; }
    }
}
