using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashBook.Data.Model;

namespace CashBook.Data.Interfaces
{
    public interface ICashBookEntryRepository
    {
        CashBookEntry Get(Int64 id);
        List<CashBookEntry> GetAll();
        void Create(CashBookEntry item);
        void Edit(Int64 id, CashBookEntry item);
        void Delete(Int64 id);

        List<CashBookEntry> GetEntriesForDay(DateTime dateTime);

        void UpdateRepositoryForDay(Int64 selectedCashBookId, List<CashBookEntry> list, DateTime dateTime);
    }
}
