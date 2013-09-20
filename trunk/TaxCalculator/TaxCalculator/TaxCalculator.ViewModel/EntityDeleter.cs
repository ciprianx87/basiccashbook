using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Data.Model;
using TaxCalculator.Data.Repositories;
using TaxCalculator.Data.Interfaces;

namespace TaxCalculator.ViewModel
{
    public class EntityDeleter
    {
        public static void DeleteEntity(object entity)
        {
            DeleteCompany(entity);
        }

        private static void DeleteCompany(object entity)
        {
            Company ent = entity as Company;
            if (ent != null)
            {
                ICompanyRepository repository = new CompanyRepository();
                repository.Delete(ent.Id);
            }
        }
    }
}
