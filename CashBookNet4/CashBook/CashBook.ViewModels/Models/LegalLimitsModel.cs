using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CashBook.ViewModels.Models
{
    public class LegalLimitsModel
    {
        public int DailyCashing { get; set; }
        public int TotalCashing { get; set; }
        public int DailyPayment { get; set; }
        public int TotalPayment { get; set; }
        public int TotalBalance { get; set; }
        public bool DailyCashingActive { get; set; }
        public bool TotalCashingActive { get; set; }
        public bool TotalBalanceActive { get; set; }
    }
}
