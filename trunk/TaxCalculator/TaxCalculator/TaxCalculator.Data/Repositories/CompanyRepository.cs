using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Data.Interfaces;
using TaxCalculator.Data.Model;
//using System.Data.en

namespace TaxCalculator.Data.Repositories
{
    public class CompanyRepository : BaseRepository, ICompanyRepository
    {

        public Company GetCompany(Int64 id)
        {
            using (var context = GetContext())
            {
                var existingCompany = context.Companies.FirstOrDefault(p => p.Id == id);
                //if (existingCompany == null)
                //{
                //    throw new CompanyNotFoundException();
                //}
                return existingCompany;
            }
        }

        public void Edit(Company entity)
        {
            using (var context = GetContext())
            {
                var existingCompany = context.Companies.FirstOrDefault(p => p.Id == entity.Id);
                if (existingCompany == null)
                {
                    throw new Exception("invalid company id: " + entity.Id);
                }
                else
                {
                    existingCompany.Adresa = entity.Adresa;
                    existingCompany.Nume = entity.Nume;
                    existingCompany.CUI = entity.CUI;
                    base.Commit(context);
                }
            }
        }

        public void Create(Company entity)
        {
            using (var context = GetContext())
            {
                Company soc = new Company()
                {
                    Id = DbIdHelper.GetNextID(),
                    Adresa = entity.Adresa,
                    Nume = entity.Nume,
                    CUI = entity.CUI
                };
                context.Companies.AddObject(soc);

                base.Commit(context);
            }
        }

        public void Delete(long id)
        {
            using (var context = GetContext())
            {
                var entity = context.Companies.FirstOrDefault(p => p.Id == id);
                if (entity == null)
                {
                    throw new Exception("invalid company id: " + entity.Id);
                }
                else
                {
                    context.DeleteObject(entity);
                    base.Commit(context);
                }
            }
        }

        public List<Company> GetAll()
        {
            using (var context = GetContext())
            {
                var list = context.Companies.ToList();               
                return list;
            }
        }
    }
}
