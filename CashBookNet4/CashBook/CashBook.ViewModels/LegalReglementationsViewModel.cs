using CashBook.Data.Interfaces;
using CashBook.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CashBook.Common;

namespace CashBook.ViewModels
{
    public class LegalReglementationsViewModel : BaseViewModel
    {
        ISettingsRepository settingsRepository;

        public ICommand SaveCommand { get; set; }
        public LegalReglementationsViewModel()
        {
            this.Title = "Reglementari legale";
            SaveCommand = new DelegateCommand(Save, CanSave);
            settingsRepository = new SettingsRepository();

            LoadData();

        }

        #region properties
        private string legalReglementationsText;
        public string LegalReglementationsText
        {
            get { return legalReglementationsText; }
            set
            {
                legalReglementationsText = value;
                NotifyPropertyChanged("LegalReglementationsText");
            }
        }

        #endregion

        #region methods
        private void LoadData()
        {
            var currentSetting = settingsRepository.GetSetting(Constants.LegalRelementationsKey);
            if (currentSetting != null)
            {
                this.LegalReglementationsText = currentSetting;
            }
        }

        public void Save(object param)
        {
            try
            {
                settingsRepository.AddOrUpdateSetting(Constants.LegalRelementationsKey, LegalReglementationsText);
                WindowHelper.OpenInformationDialog("Informatia a fost salvata");
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog("Eroare la salvarea informatiei");
            }
        }

        public bool CanSave(object param)
        {
            return true;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion
    }
}
