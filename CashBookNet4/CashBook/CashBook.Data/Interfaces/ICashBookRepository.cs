using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashBook.Data.Model;

namespace CashBook.Data.Interfaces
{
    public interface ICashBookRepository
    {
        UserCashBook Get(Int64 id);
        List<UserCashBook> GetAll(CashBookListType cashBookListType);
        void Create(UserCashBook item);
        void Edit(Int64 id, UserCashBook item);
        void Delete(Int64 id);
        decimal GetInitialBalanceForDay(long selectedCashBookId, DateTime SelectedDate);
        //decimal GetBalanceInfoForDay(long selectedCashBookId, DateTime SelectedDate, out decimal initial);
    }
}
