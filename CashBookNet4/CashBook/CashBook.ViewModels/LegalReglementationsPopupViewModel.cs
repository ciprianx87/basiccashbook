using CashBook.Data.Interfaces;
using CashBook.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CashBook.Common;
using CashBook.Common.Mediator;

namespace CashBook.ViewModels
{
    public class LegalReglementationsPopupViewModel : BaseViewModel
    {
        ISettingsRepository settingsRepository;

        public ICommand OkCommand { get; set; }
        public LegalReglementationsPopupViewModel()
        {

            OkCommand = new DelegateCommand(Ok, CanOk);
            settingsRepository = new SettingsRepository();

            this.Title = "Reglementari legale";
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

        public void Ok(object param)
        {
            Mediator.Instance.SendMessage(MediatorActionType.CloseWindow, this.Guid);
        }

        public bool CanOk(object param)
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
