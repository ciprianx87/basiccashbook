using CashBook.Data.Interfaces;
using CashBook.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashBook.Common.Exceptions;

namespace CashBook.Data.Repositories
{
    public class CompanyRepository :BaseRepository, ICompanyRepository
    {
        public void EditDetails(string name, string cui, string address)
        {
            //only allow one company to exist
            using (var context = GetContext())
            {
                var existingCompany = context.Companies.FirstOrDefault();
                if (existingCompany == null)
                {
                    Company soc = new Company()
                    {
                        Id = DbIdHelper.GetNextID(),
                        Adresa = address,
                        Nume = name,
                        CUI = cui
                    };
                    context.Companies.AddObject(soc);
                }
                else
                {
                    existingCompany.Adresa = address;
                    existingCompany.Nume = name;
                    existingCompany.CUI = cui;
                }
                context.SaveChanges();
            }
        }


        public Company GetCompany()
        {
            using (var context = GetContext())
            {
                var existingCompany = context.Companies.FirstOrDefault();
                if (existingCompany == null)
                {
                    throw new CompanyNotFoundException();
                }
                return existingCompany;
            }
        }
    }
}
