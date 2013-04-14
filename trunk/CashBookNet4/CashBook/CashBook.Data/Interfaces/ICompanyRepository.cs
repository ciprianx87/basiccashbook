using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashBook.Data.Model;

namespace CashBook.Data.Interfaces
{
    public interface ICompanyRepository
    {
        void EditDetails(string name, string cui, string address);
        Company GetCompany();
    }
}
