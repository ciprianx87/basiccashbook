using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Data.Interfaces;
using TaxCalculator.Data.Model;
using TaxCalculator.Common.Exceptions;
//using System.Data.en

namespace TaxCalculator.Data.Repositories
{
    public class TaxCalculationsRepository : BaseRepository, ITaxCalculationsRepository
    {
        private static readonly object syncObj = new object();
        public TaxCalculations Get(Int64 id)
        {
            using (var context = GetContext())
            {
                var existingCompany = context.TaxCalculations.FirstOrDefault(p => p.Id == id);
                return existingCompany;
            }
        }

        public void Edit(TaxCalculations entity)
        {
            lock (syncObj)
            {
                //using (var context = GetContext())
                //{
                //    var existingIndicator = context.TaxCalculations.FirstOrDefault(p => p.Id == entity.Id);
                //    if (existingIndicator == null)
                //    {
                //        throw new Exception("invalid Indicator id: " + entity.Id);
                //    }
                //    else
                //    {
                //        //get companies with the same name but different id
                //        var existingEntity = context.TaxCalculations.FirstOrDefault(p => p.Name == entity.Name && p.Id != entity.Id);
                //        if (existingEntity != null)
                //        {
                //            throw new DuplicateEntityNameException();
                //        }
                //        SetAllIndicatorsDefaultValue(context, entity);

                //        existingIndicator.Name = entity.Name;
                //        existingIndicator.Content = entity.Content;
                //        existingIndicator.IsDefault = entity.IsDefault;
                //        existingIndicator.CreatedTimestamp = entity.CreatedTimestamp;

                //        base.Commit(context);
                //    }
                //}
            }
        }

        //private static void SetAllIndicatorsDefaultValue(TaxCalculatorModelContainer context, TaxCalculations entity)
        //{
        //    if (entity.IsDefault)
        //    {
        //        var allIntems = context.TaxCalculations.ToList();
        //        //set everything as notDefault
        //        allIntems.ForEach(p => p.IsDefault = false);
        //    }
        //}

        public void Create(TaxCalculations entity)
        {
            lock (syncObj)
            {
                try
                {
                    using (var context = GetContext())
                    {

                        //SetAllIndicatorsDefaultValue(context, entity);
                        //TaxCalculations soc = new TaxCalculations()
                        //{
                        //    Id = DbIdHelper.GetNextID(),

                        //};]
                        entity.Id = DbIdHelper.GetNextID();
                        context.TaxCalculations.AddObject(entity);

                        base.Commit(context);
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        public void Delete(long id)
        {
            lock (syncObj)
            {
                using (var context = GetContext())
                {
                    var entity = context.TaxCalculations.FirstOrDefault(p => p.Id == id);
                    if (entity == null)
                    {
                        throw new Exception("invalid Indicator id: " + entity.Id);
                    }
                    else
                    {
                        context.DeleteObject(entity);
                        base.Commit(context);
                    }
                }
            }
        }

        public List<TaxCalculations> GetAll()
        {
            using (var context = GetContext())
            {
                var list = context.TaxCalculations.ToList();
                return list;
            }
        }
    }
}
