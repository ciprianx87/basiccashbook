using CashBook.Data.Interfaces;
using CashBook.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashBook.Data.Repositories
{
    public class CashBookRepository : BaseRepository, ICashBookRepository
    {
        public Societate GetCompany()
        {
            var context = new CashBookContainer();
            var existingCompany = context.Societates.FirstOrDefault();
            return existingCompany;
        }


        public RegistruCasa Get(long id)
        {
            var context = GetContext();
            var existingEntity = context.RegistruCasas.FirstOrDefault(p => p.Id == id);
            return existingEntity;
        }

        public List<RegistruCasa> GetAll()
        {
            var context = GetContext();
            return context.RegistruCasas.ToList();
        }

        public void Create(RegistruCasa item)
        {
            var context = GetContext();
            item.Id = DbIdHelper.GetNextID();
            context.RegistruCasas.AddObject(item);
        }

        public void Edit(long id, RegistruCasa item)
        {
            using (var context = GetContext())
            {
                var existingEntity = Get(id);
                if (existingEntity != null)
                {
                   // existingEntity.Name
                }
                Commit(context);
            }
        }

        public void Delete(long id)
        {
            var context = GetContext();
            var existingEntity = Get(id);
            if (existingEntity != null)
            {
                context.RegistruCasas.DeleteObject(existingEntity);
            }
        }
    }
}
