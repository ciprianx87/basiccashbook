using CashBook.Data.Interfaces;
using CashBook.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CashBook.Common;
using CashBook.Data.Model;
using System.Collections.ObjectModel;

namespace CashBook.ViewModels
{
    public class CashBookListViewModel : BaseViewModel
    {
        ISettingsRepository settingsRepository;

        public ICommand SaveCommand { get; set; }
        public CashBookListViewModel()
        {
            SaveCommand = new DelegateCommand(Save, CanSave);
            settingsRepository = new SettingsRepository();

            LoadData();
            
        }

        #region properties

        private ObservableCollection<RegistruCasa> cashBooks;
        public ObservableCollection<RegistruCasa> CashBooks
        {
            get { return cashBooks; }
            set
            {
                cashBooks = value;
                NotifyPropertyChanged("CashBooks");
            }
        }


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
            }
            catch (Exception ex)
            {

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
