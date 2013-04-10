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
            using (var context = GetContext())
            {
                var existingCompany=context.Societates.First();
                item.Id = DbIdHelper.GetNextID();
                item.RegistruCasaZis = new System.Data.Objects.DataClasses.EntityCollection<RegistruCasaZi>();
                item.Societate = existingCompany;
                context.RegistruCasas.AddObject(item);
                Commit(context);
            }
        }

        public void Edit(long id, RegistruCasa item)
        {
            using (var context = GetContext())
            {
                var existingEntity = Get(id);
                if (existingEntity != null)
                {
                    existingEntity.Name = item.Name;
                    existingEntity.Account = item.Account;
                    existingEntity.CashierName = item.CashierName;
                    existingEntity.CoinDecimals = item.CoinDecimals;
                    existingEntity.CoinType = item.CoinType;
                    existingEntity.InitialBalance = item.InitialBalance;
                    existingEntity.Location = item.Location;
                }
                Commit(context);
            }
        }

        public void Delete(long id)
        {
            using (var context = GetContext())
            {
                var existingEntity = Get(id);
                if (existingEntity != null)
                {
                    context.RegistruCasas.DeleteObject(existingEntity);
                }
                Commit(context);
            }
        }
    }
}
