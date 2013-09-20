using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Data.Model;

namespace TaxCalculator.Data.Interfaces
{
    public interface ICompanyRepository
    {
        void Edit(Company entity);
        Company GetCompany(Int64 id);
        void Create(Company entity);
        void Delete(Int64 id);
        List<Company> GetAll();
    }
}
