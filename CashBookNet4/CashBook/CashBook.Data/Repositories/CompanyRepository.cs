using CashBook.Data.Interfaces;
using CashBook.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashBook.Data.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        public void EditDetails(string name, string cui, string address)
        {
            //only allow one company to exist
            var context = new CashBookContainer();
            var existingCompany = context.Societates.FirstOrDefault();
            if (existingCompany == null)
            {              
                Societate soc = new Societate()
                {
                    Id=DbIdHelper.GetNextID(),
                    Adresa = address,
                    Nume = name,
                    CUI = cui
                };
                context.Societates.AddObject(soc);
            }
            else
            {
                existingCompany.Adresa = address;
                existingCompany.Nume = name;
                existingCompany.CUI = cui;
            }
            context.SaveChanges();
        }


        public Societate GetCompany()
        {
            var context = new CashBookContainer();
            var existingCompany = context.Societates.FirstOrDefault();
            return existingCompany;
        }
    }
}
