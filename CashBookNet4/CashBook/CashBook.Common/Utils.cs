using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CashBook.Common
{
    public static class Utils
    {
        public static DateTime DateTimeToDay(DateTime date)
        {
            return date.Date;
        }
    }
}
