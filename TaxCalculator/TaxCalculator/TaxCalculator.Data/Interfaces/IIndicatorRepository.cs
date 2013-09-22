using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Data.Model;

namespace TaxCalculator.Data.Interfaces
{
    public interface IIndicatorRepository
    {
        void Edit(Indicator entity);
        Indicator Get(Int64 id);
        void Create(Indicator entity);
        void Delete(Int64 id);
        List<Indicator> GetAll();
    }
}
