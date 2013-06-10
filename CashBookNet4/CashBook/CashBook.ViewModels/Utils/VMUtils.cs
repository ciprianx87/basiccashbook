using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using CashBook.Data.Interfaces;
using CashBook.Common;
using CashBook.ViewModels.Models;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace CashBook.ViewModels
{
    public static class VMUtils
    {
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

        public static List<DateTime> CompletedDates;
        public static LegalLimitsModel LegalLimits { get; set; }

        public static LegalLimitsModel GetLegalLimits(ISettingsRepository settingsRepository)
        {
            MemoryStream ms = null;
            StreamWriter sw = null;
            try
            {
                string legaLimits = settingsRepository.GetSetting(Constants.LegalLimitsKey);
                if (string.IsNullOrEmpty(legaLimits))
                {
                    //add the default values
                    LegalLimitsModel limitsModel = new LegalLimitsModel()
                    {
                        DailyCashing = 0,
                        TotalBalance = 0,
                        TotalCashing = 0,
                        TotalPayment = 10000,
                        DailyPayment = 5000,
                        DailyCashingActive = false,
                        TotalBalanceActive = false,
                        TotalCashingActive = false
                    };
                    SaveLegalLimits(settingsRepository, limitsModel);
                    legaLimits = settingsRepository.GetSetting(Constants.LegalLimitsKey);
                }
                XmlSerializer serializer = new XmlSerializer(typeof(LegalLimitsModel));
                ms = new MemoryStream();
                sw = new StreamWriter(ms);
                sw.Write(legaLimits);
                sw.Flush();
                ms.Flush();
                StringReader sr = new StringReader(legaLimits);
                var result = (LegalLimitsModel)serializer.Deserialize(sr);
                return result;
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                throw ex;
            }
            finally
            {
                try
                {
                    sw.Close();
                }
                catch { }
                try
                {
                    ms.Close();
                }
                catch { }
            }
        }

        public static void SaveLegalLimits(ISettingsRepository settingsRepository, LegalLimitsModel limitsModel)
        {

            MemoryStream ms = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(LegalLimitsModel));
                ms = new MemoryStream();
                serializer.Serialize(ms, limitsModel);
                var serializedString = Encoding.ASCII.GetString(ms.GetBuffer());
                settingsRepository.AddOrUpdateSetting(Constants.LegalLimitsKey, serializedString);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                throw ex;
            }
            finally
            {
                try
                {
                    ms.Close();
                }
                catch { }
            }
        }
    }
}
