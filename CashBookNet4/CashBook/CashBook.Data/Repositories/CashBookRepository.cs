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
        public UserCashBook Get(long id)
        {
            var context = GetContext();
            var existingEntity = context.UserCashBooks.FirstOrDefault(p => p.Id == id);
            return existingEntity;
        }

        public List<UserCashBook> GetAll()
        {
            var context = GetContext();
            return context.UserCashBooks.ToList();
        }

        public void Create(UserCashBook item)
        {
            using (var context = GetContext())
            {
                var existingCompany = context.Companies.First();
                item.Id = DbIdHelper.GetNextID();
                item.RegistruCasaZis = new System.Data.Objects.DataClasses.EntityCollection<DailyCashBook>();
                item.Societate = existingCompany;
                context.UserCashBooks.AddObject(item);
                Commit(context);
            }
        }

        public void Edit(long id, UserCashBook item)
        {
            using (var context = GetContext())
            {
                var existingEntity = GetCashBook(id, context);
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
                //the same context needs to load this entity
                var existingEntity = GetCashBook(id, context);
                if (existingEntity != null)
                {
                    context.UserCashBooks.DeleteObject(existingEntity);
                }
                Commit(context);
            }
        }

        private UserCashBook GetCashBook(long id, CashBookContainer context)
        {
            return context.UserCashBooks.FirstOrDefault(p => p.Id == id);
        }
    }
}
