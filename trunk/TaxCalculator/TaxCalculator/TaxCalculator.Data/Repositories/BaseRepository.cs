using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Data.Model;

namespace TaxCalculator.Data.Repositories
{
    public class BaseRepository
    {
        protected TaxCalculatorModelContainer GetContext()
        {
            return new TaxCalculatorModelContainer();
        }

        protected void Commit(TaxCalculatorModelContainer context)
        {
            context.SaveChanges();
        }
    }
}
