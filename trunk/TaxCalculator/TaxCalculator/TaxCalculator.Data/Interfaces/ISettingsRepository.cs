using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Data.Model;

namespace TaxCalculator.Data.Interfaces
{
    public interface ISettingsRepository
    {

        string GetSetting(string key);

        void AddOrUpdateSetting(string key, string value);
    }
}
