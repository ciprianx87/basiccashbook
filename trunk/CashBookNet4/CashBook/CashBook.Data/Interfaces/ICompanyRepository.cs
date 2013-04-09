using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashBook.Data.Interfaces
{
    public interface ICompanyRepository
    {
        void EditDetails(string name, string cui, string address);
    }
}
