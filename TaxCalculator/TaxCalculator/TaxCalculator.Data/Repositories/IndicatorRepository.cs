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
    public class IndicatorRepository : BaseRepository, IIndicatorRepository
    {

        public Indicator Get(Int64 id)
        {
            using (var context = GetContext())
            {
                var existingCompany = context.Indicators.FirstOrDefault(p => p.Id == id);               
                return existingCompany;
            }
        }

        public void Edit(Indicator entity)
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
                    existingIndicator.Name = entity.Name;
                    existingIndicator.Content = entity.Content;
                    existingIndicator.IsDefault = entity.IsDefault;
                    existingIndicator.CreatedTimestamp = entity.CreatedTimestamp;

                    base.Commit(context);
                }
            }
        }

        public void Create(Indicator entity)
        {
            using (var context = GetContext())
            {
                var existingEntity = context.Indicators.FirstOrDefault(p => p.Name == entity.Name);
                if (existingEntity != null)
                {
                    throw new DuplicateEntityNameException();
                }
                Indicator soc = new Indicator()
                {
                    Id = DbIdHelper.GetNextID(),                  
                    Name = entity.Name,
                    Content=entity.Content,
                    CreatedTimestamp=entity.CreatedTimestamp,
                    IsDefault=entity.IsDefault
                };
                context.Indicators.AddObject(soc);

                base.Commit(context);
            }
        }

        public void Delete(long id)
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
                    context.DeleteObject(entity);
                    base.Commit(context);
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
    }
}
