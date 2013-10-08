using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Data.Interfaces;
using TaxCalculator.Data.Model;
using TaxCalculator.Common.Exceptions;
//using System.Data.en

namespace TaxCalculator.Data.Repositories
{
    public class SettingsRepository : BaseRepository, ISettingsRepository
    {
        public string GetSetting(string key)
        {
            using (var context = GetContext())
            {
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
        }

        public void AddOrUpdateSetting(string key, string value)
        {
            using (var context = GetContext())
            {
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
}
