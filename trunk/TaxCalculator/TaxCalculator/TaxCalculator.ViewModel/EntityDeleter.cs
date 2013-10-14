using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Data.Model;
using TaxCalculator.Data.Repositories;
using TaxCalculator.Data.Interfaces;
using TaxCalculator.ViewModel.ViewModels.Model;

namespace TaxCalculator.ViewModel
{
    public class EntityDeleter
    {
        public static void DeleteEntity(object entity)
        {
            if (entity is Company)
            {
                DeleteCompany((Company)entity);
            }
            else if (entity is Indicator)
            {
                DeleteIndicator((Indicator)entity);
            }
            else if (entity is TaxCalculationsViewModel)
            {
                DeleteTaxCalculation((TaxCalculationsViewModel)entity);
            }
        }

        private static void DeleteCompany(Company entity)
        {
            ICompanyRepository repository = new CompanyRepository();
            repository.Delete(entity.Id);
        }

        private static void DeleteIndicator(Indicator entity)
        {
            IIndicatorRepository repository = new IndicatorRepository();
            repository.Delete(entity.Id);
        }

        private static void DeleteTaxCalculation(TaxCalculationsViewModel entity)
        {
            ITaxCalculationsRepository repository = new TaxCalculationsRepository();
            repository.Delete(entity.Id);
        }
    }
}
