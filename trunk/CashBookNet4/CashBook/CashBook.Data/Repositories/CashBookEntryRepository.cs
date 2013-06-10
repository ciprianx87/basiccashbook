using CashBook.Data.Interfaces;
using CashBook.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashBook.Common;

namespace CashBook.Data.Repositories
{
    public class CashBookEntryRepository : BaseRepository, ICashBookEntryRepository
    {
        public CashBookEntry Get(long id)
        {
            using (var context = GetContext())
            {
                var existingEntity = context.CashBookEntries.FirstOrDefault(p => p.Id == id);
                return existingEntity;
            }
        }

        public List<CashBookEntry> GetAll()
        {
            using (var context = GetContext())
            {
                return context.CashBookEntries.ToList();
            }
        }

        public void Create(CashBookEntry item)
        {
            using (var context = GetContext())
            {
                item.Id = DbIdHelper.GetNextID();
                context.CashBookEntries.AddObject(item);
                Commit(context);
            }
        }

        public void Edit(long id, CashBookEntry item)
        {
            using (var context = GetContext())
            {
                var existingEntity = GetCashBookEntry(id, context);
                if (existingEntity != null)
                {
                    existingEntity.Explicatii = item.Explicatii;
                    existingEntity.Incasari = item.Incasari;
                    existingEntity.NrActCasa = item.NrActCasa;
                    existingEntity.NrAnexe = item.NrAnexe;
                    existingEntity.NrCrt = item.NrCrt;
                    existingEntity.Plati = item.Plati;
                }
                Commit(context);
            }
        }

        public void Delete(long id)
        {
            using (var context = GetContext())
            {
                //the same context needs to load this entity
                var existingEntity = GetCashBookEntry(id, context);
                if (existingEntity != null)
                {
                    context.CashBookEntries.DeleteObject(existingEntity);
                }
                Commit(context);
            }
        }

        public List<CashBookEntry> GetEntriesForDay(Int64 selectedCashBookId, DateTime dateTime)
        {
            using (var context = GetContext())
            {
                var dateOnly = Utils.DateTimeToDay(dateTime);
                var registryForDay = context.DailyCashBooks.FirstOrDefault(p => p.Data == dateOnly && p.RegistruCasaId == selectedCashBookId);
                if (registryForDay != null)
                {
                    return context.CashBookEntries.Where(p => p.RegistruCasaZiId == registryForDay.Id).ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        public void UpdateRepositoryForDay(Int64 selectedCashBookId, List<CashBookEntry> list, DateTime dateTime, decimal? moneyExchangeRate)
        {
            using (var context = GetContext())
            {
                var dateOnly = Utils.DateTimeToDay(dateTime);
                var registryForDay = context.DailyCashBooks.FirstOrDefault(p => p.Data == dateOnly && p.RegistruCasaId == selectedCashBookId);
                if (registryForDay == null)
                {
                    //create a new one
                    context.DailyCashBooks.AddObject(new DailyCashBook()
                    {
                        Id = DbIdHelper.GetNextID(),
                        RegistruCasaId = selectedCashBookId,
                        Data = dateOnly,
                        MoneyExchangeRate = moneyExchangeRate
                    });
                    Commit(context);
                }

                registryForDay = context.DailyCashBooks.FirstOrDefault(p => p.Data == dateOnly && p.RegistruCasaId == selectedCashBookId);

                var allEntries = context.CashBookEntries.Where(p => p.RegistruCasaZiId == registryForDay.Id).ToList();
                //delete all the entries
                foreach (var item in allEntries)
                {
                    context.CashBookEntries.DeleteObject(item);
                }
                Commit(context);

                List<CashBookEntry> newItems = new List<CashBookEntry>();
                //add all the new entries
                foreach (var item in list)
                {
                    newItems.Add(new CashBookEntry()
                    {
                        Explicatii = item.Explicatii,
                        Id = item.Id == 0 ? DbIdHelper.GetNextID() : item.Id,
                        Incasari = item.Incasari,
                        NrActCasa = item.NrActCasa,
                        NrAnexe = item.NrAnexe,
                        NrCrt = item.NrCrt,
                        Plati = item.Plati,
                        RegistruCasaZiId = registryForDay.Id,
                    });
                }
                foreach (var item in newItems)
                {
                    context.CashBookEntries.AddObject(item);
                }

                //update the current balance
                decimal totalSum = 0;
                newItems.ForEach(p => totalSum += p.Incasari);
                newItems.ForEach(p => totalSum -= p.Plati);
                registryForDay.DeltaBalance = totalSum;
                registryForDay.TotalBalance = GetPreviousBalance(context, Utils.DateTimeToDay(dateTime), selectedCashBookId);
                registryForDay.MoneyExchangeRate = moneyExchangeRate;
                Commit(context);
            }
        }

        private decimal GetPreviousBalance(CashBookContainer context, DateTime currentDate, long selectedCashBookId)
        {
            //get all the previous registers
            var previousRegisters = context.DailyCashBooks.Where(p => p.Data < currentDate && p.RegistruCasaId == selectedCashBookId).ToList();
            decimal totalSum = 0;
            previousRegisters.ForEach(p => totalSum += p.DeltaBalance);

            //take the initial balance of the cashbook into account
            var currentCashBook = context.UserCashBooks.FirstOrDefault(p => p.Id == selectedCashBookId);
            if (currentCashBook.InitialBalanceDate < currentDate)
            {
                totalSum += currentCashBook.InitialBalance;
            }
            return totalSum;
        }


        private CashBookEntry GetCashBookEntry(long id, CashBookContainer context)
        {
            return context.CashBookEntries.FirstOrDefault(p => p.Id == id);
        }


        public decimal? GetExchangeRateForDay(long selectedCashBookId, DateTime dateTime)
        {
            using (var context = GetContext())
            {
                var dateOnly = Utils.DateTimeToDay(dateTime);
                var registryForDay = context.DailyCashBooks.FirstOrDefault(p => p.Data == dateOnly && p.RegistruCasaId == selectedCashBookId);
                if (registryForDay != null)
                {
                    return registryForDay.MoneyExchangeRate;
                }
                return null;
            }
        }

        public List<DateTime> GetCompletedDays(long selectedCashBookId)
        {
            List<DateTime> result = new List<DateTime>();
            using (var context = GetContext())
            {
                var registries = context.DailyCashBooks.Where(p => p.RegistruCasaId == selectedCashBookId);
                foreach (var item in registries)
                {
                    if (item.RegistruCasaIntrares.Count > 0)
                    {
                        if (!result.Contains(item.Data))
                        {
                            result.Add(item.Data);
                        }
                    }
                }
            }
            return result;
        }


        public CashBookEntry GetDuplicateEntryByNumber(List<string> currentEntries, long currentCashBookId, DateTime currentDay,
            out DateTime? date, out string registryName)
        {
            date = null;
            registryName = "";
            CashBookEntry result = null;
            using (var context = GetContext())
            {
                int currentYear = currentDay.Year;
                result = context.CashBookEntries.FirstOrDefault(p => currentEntries.Contains(p.NrActCasa)
                    && ((currentCashBookId == p.RegistruCasaZi.RegistruCasaId && p.RegistruCasaZi.Data != currentDay)
                    || (currentCashBookId != p.RegistruCasaZi.RegistruCasaId)
                  ) && p.Incasari != 0
                    && p.RegistruCasaZi.Data.Year == currentYear);
                if (result != null)
                {
                    var dailyCashBook = context.DailyCashBooks.FirstOrDefault(p => p.Id == result.RegistruCasaZiId);
                    if (dailyCashBook != null)
                    {
                        date = dailyCashBook.Data;
                        registryName = dailyCashBook.RegistruCasa.NameAndLocation;
                    }
                }
            }
            return result;
        }
    }
}
