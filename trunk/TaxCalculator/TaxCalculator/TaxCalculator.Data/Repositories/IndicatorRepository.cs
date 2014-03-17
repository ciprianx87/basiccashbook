using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Data.Interfaces;
using TaxCalculator.Data.Model;
using TaxCalculator.Common.Exceptions;
using System.Data.Entity;
using TaxCalculator.Common;

namespace TaxCalculator.Data.Repositories
{
    public class IndicatorRepository : BaseRepository, IIndicatorRepository
    {
        private static readonly object syncObj = new object();
        public Indicator Get(Int64 id)
        {
            using (var context = GetContext())
            {
                var existingEntity = context.Indicators.FirstOrDefault(p => p.Id == id);
                return existingEntity;
            }
        }

        public void Edit(Indicator entity)
        {
            lock (syncObj)
            {
                using (var context = GetContext())
                {
                    var existingIndicator = context.Indicators.FirstOrDefault(p => p.Id == entity.Id);
                    if (existingIndicator == null)
                    {
                        throw new Exception("invalid Indicator id: " + entity.Id);
                    }
                    else
                    {
                        //get companies with the same name but different id
                        var existingEntity = context.Indicators.FirstOrDefault(p => p.Name == entity.Name && p.Id != entity.Id);
                        if (existingEntity != null)
                        {
                            throw new DuplicateEntityNameException();
                        }
                        SetAllIndicatorsDefaultValue(context, entity);

                        existingIndicator.Name = entity.Name;
                        existingIndicator.Content = entity.Content;
                        existingIndicator.IsDefault = entity.IsDefault;
                        existingIndicator.CreatedTimestamp = entity.CreatedTimestamp;

                        base.Commit(context);
                    }
                }
            }
        }

        private static void SetAllIndicatorsDefaultValue(TaxCalculatorModelContainer context, Indicator entity)
        {
            if (entity.IsDefault)
            {
                var allIntems = context.Indicators.ToList();
                //set everything as notDefault
                allIntems.ForEach(p => p.IsDefault = false);
            }
        }

        public void Create(Indicator entity)
        {
            lock (syncObj)
            {
                using (var context = GetContext())
                {
                    var existingEntity = context.Indicators.FirstOrDefault(p => p.Name == entity.Name);
                    if (existingEntity != null)
                    {
                        throw new DuplicateEntityNameException();
                    }
                    SetAllIndicatorsDefaultValue(context, entity);
                    Indicator soc = new Indicator()
                    {
                        Id = DbIdHelper.GetNextID(),
                        Name = entity.Name,
                        Content = entity.Content,
                        CreatedTimestamp = entity.CreatedTimestamp,
                        IsDefault = entity.IsDefault
                    };
                    context.Indicators.AddObject(soc);

                    base.Commit(context);
                    entity.Id = soc.Id;
                }
            }
        }

        public void Delete(long id)
        {
            lock (syncObj)
            {
                using (var context = GetContext())
                {
                    var entity = context.Indicators.FirstOrDefault(p => p.Id == id);
                    if (entity == null)
                    {
                        throw new Exception("invalid Indicator id: " + entity.Id);
                    }
                    else
                    {
                        //if the deleted entity is the default one then make the first one default
                        if (entity.IsDefault)
                        {
                            var allItems = context.Indicators.Where(p => p.Id != entity.Id).ToList();
                            if (allItems.Count > 0)
                            {
                                allItems.First().IsDefault = true;
                            }
                        }
                        context.DeleteObject(entity);
                        base.Commit(context);
                    }
                }
            }
        }

        public List<Indicator> GetAll()
        {
            using (var context = GetContext())
            {
                var list = context.Indicators.ToList();
                return list;
            }
        }


        public void EditWithHide(Indicator entity)
        {
            lock (syncObj)
            {
                using (var context = GetContext())
                {

                    var existingIndicator = context.Indicators.FirstOrDefault(p => p.Id == entity.Id);
                    if (existingIndicator == null)
                    {
                        throw new Exception("invalid Indicator id: " + entity.Id);
                    }
                    else
                    {
                        //set the values for the new entity as the current one
                        //create a new entity

                        Indicator newEntity = new Indicator()
                        {
                            Id = DbIdHelper.GetNextID(),
                            Name = entity.Name,
                            Content = entity.Content,
                            CreatedTimestamp = entity.CreatedTimestamp,
                            IsDefault = entity.IsDefault
                        };
                        context.Indicators.AddObject(newEntity);
                        //SetAllIndicatorsDefaultValue(context, entity);
                        //hide the current entity
                        //add an entry in the settings file


                        base.Commit(context);
                    }
                }
            }
        }
    }
}
