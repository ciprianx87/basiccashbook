using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Windows;
using TaxCalculator.ViewModel.ViewModels.Model;
using TaxCalculator.Data.Model;
using System.Collections.ObjectModel;
using TaxCalculator.Data.Interfaces;
using TaxCalculator.Common;
using TaxCalculator.Data.Repositories;

namespace TaxCalculator.ViewModel
{
    static class VmUtils
    {
        public static string SerializeEntity(object entity)
        {
            return JsonConvert.SerializeObject(entity, Formatting.Indented);
        }

        public static T Deserialize<T>(string serializedData)
        {
            return JsonConvert.DeserializeObject<T>(serializedData);
        }

        public static byte[] SerializeEntityToBinary(object entity)
        {
            return StringToByteArray(SerializeEntity(entity));
        }

        public static T DeserializeFromBinary<T>(byte[] serializedData)
        {
            return JsonConvert.DeserializeObject<T>(ByteArrayToString(serializedData));
        }

        static string ByteArrayToString(byte[] arr)
        {
            char[] charArray = arr.Select(b => (char)b).ToArray();
            return new string(charArray);
        }

        static byte[] StringToByteArray(string s)
        {
            //this method maps each char in the string to a single output byte;
            //all chars should be in the range 0 to 255.  The checked 
            //conversion will catch any data that violates this requirement.

            return s.Select(c => checked((byte)c)).ToArray();
        }
        public static TaxIndicatorStyleInfo GetStyleInfo(TaxIndicatorType taxIndicatorType)
        {
            TaxIndicatorStyleInfo styleInfo = new TaxIndicatorStyleInfo();
            styleInfo.NrCrtVisibility = Visibility.Visible;
            switch (taxIndicatorType)
            {

                case TaxIndicatorType.Numeric:
                    styleInfo.FontWeight = FontWeights.Normal;
                    styleInfo.FormulaFieldVisibility = Visibility.Collapsed;
                    styleInfo.ValueFieldVisibility = Visibility.Visible;
                    break;
                case TaxIndicatorType.Text:
                    styleInfo.FontWeight = FontWeights.Bold;
                    styleInfo.FormulaFieldVisibility = Visibility.Collapsed;
                    styleInfo.ValueFieldVisibility = Visibility.Collapsed;
                    styleInfo.NrCrtVisibility = Visibility.Collapsed;
                    break;
                case TaxIndicatorType.Calculat:
                    styleInfo.FontWeight = FontWeights.Bold;
                    styleInfo.FormulaFieldVisibility = Visibility.Visible;
                    styleInfo.ValueFieldVisibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
            return styleInfo;
        }

        public static TaxIndicatorType GetIndicatorType(string type)
        {
            switch (type.ToLower())
            {
                case "numeric": return TaxIndicatorType.Numeric; break;
                case "text": return TaxIndicatorType.Text; break;
                case "calculat": return TaxIndicatorType.Calculat; break;
                case "": return TaxIndicatorType.Calculat; break;
                default: throw new ArgumentException(type);
            }
        }


        public static void ExtractCoinTypes(ISettingsRepository settingsRepository, ObservableCollection<string> coinTypes)
        {
            string allCoinTypes = settingsRepository.GetSetting(Constants.CoinTypesKey);
            //fix for old version of the coin types
            if (allCoinTypes == null || allCoinTypes == "LEI;EURO;DOLARI")
            {
                settingsRepository.AddOrUpdateSetting(Constants.CoinTypesKey, "LEI;EUR;USD");
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
        public static void RemoveHiddenIndicators(List<Indicator> existingIndicators)
        {
            List<IndicatorVisibilityModel> existingData = new List<IndicatorVisibilityModel>();

            var existingVisibilityInfoString = new SettingsRepository().GetSetting(Constants.IndicatorVisibilityKey);
            if (!string.IsNullOrEmpty(existingVisibilityInfoString))
            {
                existingData = JsonConvert.DeserializeObject<List<IndicatorVisibilityModel>>(existingVisibilityInfoString);
                foreach (var item in existingData)
                {
                    var existing = existingIndicators.FirstOrDefault(p => p.Id == item.IndicatorId);
                    if (existing != null)
                    {
                        existingIndicators.Remove(existing);
                    }
                }
            }
        }
    }
}
