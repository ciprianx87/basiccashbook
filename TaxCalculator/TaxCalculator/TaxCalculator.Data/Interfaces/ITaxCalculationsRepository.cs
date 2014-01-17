using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Data.Model;

namespace TaxCalculator.Data.Interfaces
{
    public interface ITaxCalculationsRepository
    {
        void Edit(TaxCalculations entity);
        TaxCalculations Get(Int64 id);
        void Create(TaxCalculations entity);
        void Delete(Int64 id);
        List<TaxCalculations> GetAll();

        void UpdateContent(long id, string content, string otherData);
    }
}
