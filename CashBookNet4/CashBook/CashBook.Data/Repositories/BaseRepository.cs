using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CashBook.Data.Model;

namespace CashBook.Data.Repositories
{
    public class BaseRepository
    {
        protected CashBookContainer GetContext()
        {
            return new CashBookContainer();
        }

        protected void Commit(CashBookContainer context)
        {
            context.SaveChanges();
        }
    }
}
