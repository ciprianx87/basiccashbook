﻿using System;
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

        List<CashBookEntry> GetEntriesForDay(Int64 selectedCashBookId,DateTime dateTime);

        void UpdateRepositoryForDay(Int64 selectedCashBookId, List<CashBookEntry> list, DateTime dateTime, decimal? moneyExchangeRate);
        decimal? GetExchangeRateForDay(Int64 selectedCashBookId, DateTime dateTime);
        List<DateTime> GetCompletedDays(long selectedCashBookId);


        CashBookEntry GetDuplicateEntryByNumber(List<string> currentEntries, long currentCashBookId, DateTime currentDay,
            out DateTime? date, out string registryName);
    }
}
