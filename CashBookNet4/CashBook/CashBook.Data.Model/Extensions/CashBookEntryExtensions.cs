using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CashBook.Data.Model
{
    public partial class CashBookEntry
    {
        public CashBookEntry()
        {

        }
    }
    public partial class UserCashBook
    {

        public string InitialBalanceDateString { get; set; }
        public string InitialBalanceString { get; set; }

        public string LastDateTimeWithEntriesString { get; set; }
        public string CurrentBalanceString { get; set; }
        public UserCashBook()
        {
            
        }

        private bool isLei;
        public bool IsLei
        {
            get { return CoinType.ToLower() == "lei" || CoinType.ToLower() == "ron"; }
           
        }

    }


}
