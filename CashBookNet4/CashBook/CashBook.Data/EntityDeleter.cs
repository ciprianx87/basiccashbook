using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CashBook.Data.Model;
using CashBook.Data.Repositories;
using CashBook.Data.Interfaces;

namespace CashBook.Data
{
    public class EntityDeleter
    {
        public static void DeleteEntity(object entity)
        {
            DeleteCashBook(entity);
        }

        private static void DeleteCashBook(object entity)
        {
            UserCashBook ent = entity as UserCashBook;
            if (ent != null)
            {
                ICashBookRepository repository= new CashBookRepository();
                repository.Delete(ent.Id);
            }
        }
    }
}
