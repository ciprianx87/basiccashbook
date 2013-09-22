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
    }
}
