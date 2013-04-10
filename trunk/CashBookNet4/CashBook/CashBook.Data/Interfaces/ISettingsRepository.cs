using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashBook.Data.Model;

namespace CashBook.Data.Interfaces
{
    public interface ISettingsRepository
    {
        string GetSetting(string key);
        void AddOrUpdateSetting(string key,string value);
    }
}
