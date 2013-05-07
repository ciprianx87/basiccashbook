using CashBook.Data.Interfaces;
using CashBook.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CashBook.Common;
using System.Collections.ObjectModel;
using CashBook.ViewModels.Models;
using CashBook.Common.Mediator;

namespace CashBook.ViewModels
{
    public class ReportsViewModel : BaseViewModel
    {
        ICashBookRepository cashBookRepository;
        ICashBookEntryRepository cashBookEntryRepository;

        public ICommand SaveCommand { get; set; }
        public ReportsViewModel()
        {
            this.Title = "Rapoarte";
            Mediator.Instance.Register(MediatorActionType.SetSelectedDate, SetSelectedDate);


            cashBookRepository = new CashBookRepository();
            cashBookEntryRepository = new CashBookEntryRepository();


        }

        #region properties

        private DateTime selectedDate;
        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set
            {
                if (selectedDate != value)
                {
                    selectedDate = value;
                    this.NotifyPropertyChanged("SelectedDate");
                }
            }
        }

        private ObservableCollection<CashBookEntryUI> cashBookEntries;
        public ObservableCollection<CashBookEntryUI> CashBookEntries
        {
            get { return cashBookEntries; }
            set
            {
                cashBookEntries = value;
                NotifyPropertyChanged("CashBookEntries");
            }
        }

        public long SelectedCashBookId { get; set; }
        #endregion

        #region methods
        public void SetSelectedDate(object param)
        {
            ReportInitialData initialData = param as ReportInitialData;
            SelectedDate = initialData.SelectedDate;
            SelectedCashBookId = initialData.SelectedCashBookId;
            this.Title = "Vizualizare Raport";
            LoadDataForDay(SelectedDate);

        }
        private void LoadDataForDay(DateTime dateTime)
        {
            CashBookEntries = new ObservableCollection<CashBookEntryUI>();

            var existingCashBookEntries = cashBookEntryRepository.GetEntriesForDay(SelectedCashBookId, dateTime);
            if (existingCashBookEntries != null)
            {
                foreach (var item in existingCashBookEntries)
                {
                    CashBookEntries.Add((CashBookEntryUI)item);
                }
            }
        }


        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion
    }
}
