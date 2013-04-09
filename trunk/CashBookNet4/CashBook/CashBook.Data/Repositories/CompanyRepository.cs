using CashBook.Data.Interfaces;
using CashBook.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;

namespace CashBook.Data.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        public void EditDetails(string name, string cui, string address)
        {
            //only allow one company to exist
            var context = new CashBook.Data.Model.CashBookEntities();// CashBookEntities("name=CashBookEntities");
            var existingCompany = context.Societates.FirstOrDefault();
            if (existingCompany == null)
            {
                Societate soc = new Societate()
                {
                    Adresa = address,
                    Nume = name,
                    CUI = cui
                };
                context.Societates.Add(soc);
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
}
