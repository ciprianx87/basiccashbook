﻿using CashBook.Data.Interfaces;
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

        public List<CashBookEntry> GetEntriesForDay(DateTime dateTime)
        {
            using (var context = GetContext())
            {
                var dateOnly = Utils.DateTimeToDay(dateTime);
                var registryForDay = context.DailyCashBooks.FirstOrDefault(p => p.Data == dateOnly);
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

        public void UpdateRepositoryForDay(Int64 selectedCashBookId, List<CashBookEntry> list, DateTime dateTime)
        {
            using (var context = GetContext())
            {
                var dateOnly = Utils.DateTimeToDay(dateTime);
                var registryForDay = context.DailyCashBooks.FirstOrDefault(p => p.Data == dateOnly);
                if (registryForDay == null)
                {
                    //create a new one
                    context.DailyCashBooks.AddObject(new DailyCashBook()
                    {
                        Id = DbIdHelper.GetNextID(),
                        RegistruCasaId = selectedCashBookId,
                        Data = dateOnly
                    });
                    Commit(context);
                }

                registryForDay = context.DailyCashBooks.FirstOrDefault(p => p.Data == dateOnly);

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
                        GridId = item.GridId,
                        Id = item.Id == 0 ? DbIdHelper.GetNextID() : item.Id,
                        Incasari = item.Incasari,
                        IsOk = item.IsOk,
                        NrActCasa = item.NrActCasa,
                        NrAnexe = item.NrAnexe,
                        NrCrt = item.NrCrt,
                        Plati = item.Plati,
                        //RegistruCasaZi = item.RegistruCasaZi,
                        RegistruCasaZiId = registryForDay.Id,
                        //RegistruCasaZiReference = item.RegistruCasaZiReference,
                    });
                }
                foreach (var item in newItems)
                {
                    context.CashBookEntries.AddObject(item);

                }

                Commit(context);
            }
        }


        private CashBookEntry GetCashBookEntry(long id, CashBookContainer context)
        {
            return context.CashBookEntries.FirstOrDefault(p => p.Id == id);
        }
    }
}
