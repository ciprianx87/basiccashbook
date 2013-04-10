using CashBook.Data.Interfaces;
using CashBook.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashBook.Data.Repositories
{
    public class SettingsRepository : ISettingsRepository
    {


        public Societate GetCompany()
        {
            var context = new CashBookContainer();
            var existingCompany = context.Societates.FirstOrDefault();
            return existingCompany;
        }

        public string GetSetting(string key)
        {
            var context = new CashBookContainer();
            var setting = context.Settings.FirstOrDefault(p => p.Key == key);
            if (setting == null)
            {
                return null;
            }
            else
            {
                return setting.Value;
            }
        }

        public void AddOrUpdateSetting(string key, string value)
        {
            var context = new CashBookContainer();
            var setting = context.Settings.FirstOrDefault(p => p.Key == key);
            if (setting == null)
            {
                setting = new Settings()
                {
                    Id = DbIdHelper.GetNextID(),
                    Key = key,
                    Value = value
                };
                context.Settings.AddObject(setting);
            }
            else
            {
                setting.Value = value;
            }
            context.SaveChanges();
        }
    }
}
