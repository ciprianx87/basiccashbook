﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CashBook.Data.Model;
using CashBook.Data.Interfaces;
using System.Windows;
using System.Collections.ObjectModel;
using CashBook.ViewModels.Models;
using CashBook.Common;

namespace CashBook.ViewModels
{
    public class ReportPagesRetriever
    {
        private int currentPage = 1;
        public int MaxEntriesPerPage { get; set; }
        public ObservableCollection<CashBookEntryUI> CashBookEntries { get; set; }
        public ObservableCollection<CashBookEntryUI> CurrentPageCashBookEntries { get; set; }
        public string PageNumber { get; set; }

        public List<ReportPageVM> GetPages(Company company, UserCashBook cashBook, DateTime dateTime, ICashBookRepository cashBookRepository, ICashBookEntryRepository cashBookEntryRepository, int maxEntriesPerPage)
        {
            //maxEntriesPerPage = 13;
            List<ReportPageVM> resultPages = new List<ReportPageVM>();
            try
            {

                this.MaxEntriesPerPage = maxEntriesPerPage;

                var MoneyExchangeRate = cashBookEntryRepository.GetExchangeRateForDay(cashBook.Id, dateTime);

                CashBookEntries = new ObservableCollection<CashBookEntryUI>();
                var tempCashBookEntries = new ObservableCollection<CashBookEntryUI>();

                var existingCashBookEntries = cashBookEntryRepository.GetEntriesForDay(cashBook.Id, dateTime);
                if (existingCashBookEntries != null)
                {
                    foreach (var item in existingCashBookEntries)
                    {
                        tempCashBookEntries.Add((CashBookEntryUI)item);
                    }
                }
                if (tempCashBookEntries.Count == 0)
                {
                    //no report to print for this day, return empty;
                    return resultPages;
                }

                //add incasari
                foreach (var item in tempCashBookEntries)
                {
                    if (item.Incasari != 0)
                    {
                        CashBookEntries.Add((CashBookEntryUI)item);
                    }
                }

                //add payments
                foreach (var item in tempCashBookEntries)
                {
                    if (item.Plati != 0)
                    {
                        CashBookEntries.Add((CashBookEntryUI)item);
                    }
                }
                var ReportTitle = cashBook.IsLei ? "REGISTRU DE CASA in LEI" : "REGISTRU DE CASA in VALUTA";
                var MoneyExchangeRateVisibility = cashBook.IsLei ? Visibility.Collapsed : Visibility.Visible;

                if (!cashBook.IsLei && MoneyExchangeRate.HasValue)
                {
                    foreach (var item in CashBookEntries)
                    {
                        if (item.Incasari != 0)
                        {
                            item.LeiValue = item.Incasari * MoneyExchangeRate.Value;
                        }
                        else
                        {
                            item.LeiValue = item.Plati * MoneyExchangeRate.Value;
                        }
                    }
                }

                var InitialBalanceForDayDecimal = cashBookRepository.GetInitialBalanceForDay(cashBook.Id, dateTime);



                AddExtraDetailsRows(InitialBalanceForDayDecimal, cashBook, MoneyExchangeRate);
                currentPage = 0;
                nrCrt = 1;
                while (HasNextPage())
                {
                    currentPage++;
                    LoadDataForCurrentPage();

                    ReportPageVM newPage = new ReportPageVM()
                    {
                        Company = company,
                        CurrentPageCashBookEntries = CurrentPageCashBookEntries,
                        InitialBalanceForDayDecimal = InitialBalanceForDayDecimal,
                        MoneyExchangeRate = MoneyExchangeRate,
                        MoneyExchangeRateVisibility = MoneyExchangeRateVisibility,
                        PageNumber = PageNumber,
                        ReportTitle = ReportTitle,
                        SelectedCashBook = cashBook,
                        SelectedDate = dateTime,
                    };
                    resultPages.Add(newPage);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Log.Error(ex);
            }
            return resultPages;
        }
        private int nrCrt = 0;
        private bool HasNextPage()
        {
            return CashBookEntries != null && CashBookEntries.Count > MaxEntriesPerPage * currentPage;
        }

        private void LoadDataForCurrentPage()
        {
            if (currentPage > 0)
            {
                CurrentPageCashBookEntries = new ObservableCollection<CashBookEntryUI>();
                var currentPageItems = CashBookEntries.Skip(MaxEntriesPerPage * (currentPage - 1)).Take(MaxEntriesPerPage).ToList();

                foreach (var item in currentPageItems)
                {
                    item.NrCrt = nrCrt;
                    nrCrt++;
                    CurrentPageCashBookEntries.Add(item);
                }
            }


            int totalPages = 0;
            int mod = (int)CashBookEntries.Count % MaxEntriesPerPage;
            totalPages = (int)CashBookEntries.Count / MaxEntriesPerPage;
            if (mod != 0)
            {
                totalPages++;
            }
            PageNumber = string.Format("{0}/{1}", currentPage, totalPages);
        }

        private void AddExtraDetailsRows(decimal InitialBalanceForDayDecimal, UserCashBook cashBook, decimal? MoneyExchangeRate)
        {
            //calculate the current and final balance
            decimal currentBalanceIn = 0;
            decimal currentBalanceOut = 0;
            CashBookEntries.ToList().ForEach(p => currentBalanceIn += p.Incasari);
            CashBookEntries.ToList().ForEach(p => currentBalanceOut += p.Plati);
            var currentBalanceDecimal = currentBalanceIn - currentBalanceOut;
            var totalBalanceDecimal = currentBalanceDecimal + InitialBalanceForDayDecimal;

            CashBookEntryUI currentBalance = new CashBookEntryUI()
            {
                Plati = currentBalanceOut,
                Incasari = currentBalanceIn,
                Explicatii = "Rulaj curent",
                IsExtraDetail = true
            };

            CashBookEntryUI finalBalance = new CashBookEntryUI()
            {
                Incasari = totalBalanceDecimal,
                Plati = 0,
                PlatiString = "",
                Explicatii = "Sold final",
                IsExtraDetail = true
            };
            if (cashBook.IsLei)
            {
                finalBalance.LeiValue = finalBalance.Incasari * MoneyExchangeRate.Value;
            }

            CashBookEntries.Add(currentBalance);
            CashBookEntries.Add(finalBalance);
        }
    }
}
