using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using CashBook.Data.Interfaces;
using CashBook.Common;

namespace CashBook.ViewModels
{
    static class VMUtils
    {
        public static void ExtractCoinTypes(ISettingsRepository settingsRepository, ObservableCollection<string> coinTypes)
        {
            //coinTypes = new ObservableCollection<string>();
            string allCoinTypes = settingsRepository.GetSetting(Constants.CoinTypesKey);
            if (allCoinTypes == null)
            {
                settingsRepository.AddOrUpdateSetting(Constants.CoinTypesKey, "LEI;EURO;DOLARI");
            }
            allCoinTypes = settingsRepository.GetSetting(Constants.CoinTypesKey);
            if (allCoinTypes != null)
            {
                var allCoinTypesArray = allCoinTypes.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (allCoinTypesArray != null)
                {
                    foreach (var item in allCoinTypesArray)
                    {
                        coinTypes.Add(item);
                    }
                }
            }
            else
            {
            }
        }

        public static void SaveCoinTypes(ISettingsRepository settingsRepository, List<string> coinTypes)
        {
            string coinTypesString = "";
            foreach (var item in coinTypes)
            {
                coinTypesString += item + ";";
            }
            settingsRepository.AddOrUpdateSetting(Constants.CoinTypesKey, coinTypesString);
        }
    }
}
