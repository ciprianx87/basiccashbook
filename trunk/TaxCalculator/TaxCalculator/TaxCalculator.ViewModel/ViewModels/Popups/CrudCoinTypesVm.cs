using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using TaxCalculator.ViewModel.Base;
using TaxCalculator.Data.Interfaces;
using System.Collections.ObjectModel;
using TaxCalculator.Common;
using TaxCalculator.Common.Mediator;
using TaxCalculator.Data.Repositories;

namespace TaxCalculator.ViewModel.ViewModels.Popups
{
    public class CrudCoinTypesVm : BaseViewModel
    {
        ISettingsRepository settingsRepository;
        public ICommand CreateCoinTypeCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand AddCommand { get; set; }


        public CrudCoinTypesVm()
        {
            SaveCommand = new DelegateCommand(Save, CanSave);
            this.CancelCommand = new DelegateCommand(Cancel, CanCancel);
            this.CreateCoinTypeCommand = new DelegateCommand(CreateCoinType, CanCreateCoinType);
            this.AddCommand = new DelegateCommand(Add, CanAdd);

            settingsRepository = new SettingsRepository();

            this.Title = "Tipuri de monezi";
            LoadData();

        }

        #region properties

        private CoinType selectedCoinType;
        public CoinType SelectedCoinType
        {
            get { return selectedCoinType; }
            set
            {
                if (selectedCoinType != value)
                {
                    selectedCoinType = value;
                    this.NotifyPropertyChanged("SelectedCoinType");
                }
            }
        }

        private ObservableCollection<CoinType> coinTypes;
        public ObservableCollection<CoinType> CoinTypes
        {
            get { return coinTypes; }
            set
            {
                if (coinTypes != value)
                {
                    coinTypes = value;
                    this.NotifyPropertyChanged("CoinTypes");
                }
            }
        }

        #endregion

        #region methods

        private bool CanCreateCoinType(object parameter)
        {
            return true;
        }

        private void CreateCoinType(object parameter)
        {

        }

        private void LoadData()
        {
            try
            {
                CoinTypes = new ObservableCollection<CoinType>();
                var coinTypesString = new ObservableCollection<string>();
                VmUtils.ExtractCoinTypes(settingsRepository, coinTypesString);
                if (coinTypesString != null && coinTypesString.Count > 0)
                {
                    int counter = 1;
                    foreach (var item in coinTypesString)
                    {
                        CoinTypes.Add(new CoinType()
                        {
                            CoinTypeName = item,
                            Id = counter++
                        });
                    }
                }
                if (CoinTypes.Count > 0)
                {
                    SelectedCoinType = CoinTypes[0];
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }


        private bool CanAdd(object parameter)
        {
            return true;
        }

        private void Add(object parameter)
        {
            if (CoinTypes == null)
            {
                CoinTypes = new ObservableCollection<CoinType>();
            }
            var currentMaxId = 0;
            if (CoinTypes.Count > 0)
            {
                currentMaxId = CoinTypes.Max(p => p.Id);
            }
            CoinTypes.Add(new CoinType()
            {
                CoinTypeName = "",
                Id = currentMaxId + 1
            });
        }


        public void DeleteItem(object param)
        {
            if (CoinTypes != null)
            {
                int id = (int)param;
                var existingItem = CoinTypes.FirstOrDefault(p => p.Id == id);
                if (existingItem != null)
                {
                    CoinTypes.Remove(existingItem);
                }

            }
        }

        public void Save(object param)
        {
            try
            {
                if (IsFormValid())
                {
                    VmUtils.SaveCoinTypes(settingsRepository, CoinTypes.Select(p => p.CoinTypeName).ToList());
                    WindowHelper.OpenInformationDialog("Informatia a fost salvata");
                    Mediator.Instance.SendMessage(MediatorActionType.RefreshCoinTypes, null);
                }
                else
                {
                    //WindowHelper.OpenErrorDialog("Va rugam corectati erorile");

                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
                WindowHelper.OpenErrorDialog("Eroare la salvarea informatiei");
            }
        }

        private bool IsFormValid()
        {
            if (CoinTypes == null || CoinTypes.Count == 0)
            {
                WindowHelper.OpenErrorDialog("Va rugam adaugati cel putin un tip de moneda");
                return false;
            }
            foreach (var item in CoinTypes)
            {
                if (string.IsNullOrEmpty(item.CoinTypeName))
                {
                    WindowHelper.OpenErrorDialog("Introduceti nume moneda!");
                    return false;
                }
            }
            List<String> duplicates = CoinTypes.GroupBy(x => x.CoinTypeName.ToLower())
                             .Where(g => g.Count() > 1)
                             .Select(g => g.Key)
                             .ToList();
            if (duplicates.Count > 0)
            {
                WindowHelper.OpenErrorDialog("Exista moneda cu acelasi nume!");
                return false;
            }

            return true;
        }
        public bool CanSave(object param)
        {
            return true;
        }

        public ICommand CancelCommand { get; set; }

        private bool CanCancel(object parameter)
        {
            return true;
        }

        private void Cancel(object parameter)
        {
            Mediator.Instance.SendMessage(MediatorActionType.CloseWindow, this.Guid);
        }

        private bool IsValid()
        {
            bool isValid = true;

            return isValid;
        }

        public override void Dispose()
        {
            base.Dispose();

        }

        #endregion
    }

    public class CoinType
    {
        public string CoinTypeName { get; set; }
        public int Id { get; set; }


    }
}
