using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashBook.Data.Model;

namespace CashBook.Data.Interfaces
{
    public interface ICashBookRepository
    {
        RegistruCasa Get(Int64 id);
        List<RegistruCasa> GetAll();
        void Create(RegistruCasa item);
        void Edit(Int64 id, RegistruCasa item);
        void Delete(Int64 id);
    }
}
