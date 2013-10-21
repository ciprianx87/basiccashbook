using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxCalculator.ViewModel
{
    public static class Messages
    {
        public static string CannotDeleteEntity = "Entitatea nu a putut fi stearsa!";
        public static string CannotDeleteCashBookEntry = "Intrarea nu a putut fi stearsa!";
        public static string GenericError = "Ne pare rau, a intervenit o eroare";
        public static string LegalReglementationsNotifications = "Ati depasit plafonul de plati! Doriti sa cititi reglementarile legale?";
        public static string LegalReglementationsNotificationsCashingDaily = "Ati depasit plafonul pentru incasare zilnica!";
        public static string LegalReglementationsNotificationsCashingTotal = "Ati depasit plafonul pentru incasari totale zilnice!";
        public static string LegalReglementationsNotificationsTotalBalance = "Ati depasit plafonul pentru soldul total!";
        public static string ErrorSavingInfo = "Eroare la salvarea informatiei";
        public static string InfoWasSaved = "Informatia a fost salvata";
        public static string Error_NoCompanies = "Nu exista nici o companie!";
        public static string Error_NoTaxIndicatorList = "Nu exista nici o lista de formule!";
        public static string Error_LoadingData = "Eroare la incarcarea informatiei";
        public static string Error_NoTaxCalculationsSelection = "Nu exista nici un document de calcul impozit pentru datele curente (companie si lista indicatori)!";
        public static string Error_FormulaModifiedForRectification = "Lista de indicatori a fost modificata de cand s-a facut calculul initial!";

    }
}
