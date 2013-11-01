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
        public static string Error_MissingFields = "Nu sunt completate toate campurile!";
        public static string Error_EmptyFormula = "Formula goala!";
        public static string Error_EmptyIndicatorName = "Denumire indicator goala!";
        public static string Error_ParsingFormula = "Eroare la interpretarea formulei!";
        public static string Error_InvalidFormula = "Formula invalida!";
        public static string Error_InvalidIfCondition = "Conditie IF invalida. Trebuie sa contina 3 parti separate prin \";\"";
        public static string Error_UnknownFormulaType = "Tip invalid de formula";
        public static string Error_InvalidIndicatorsCannotSave = "Lista contine indicatori eronati. Nu se poate efectua salvarea!";
        public static string Error_InfiniteLoopDetected = "Bucla infinita detectata";
        public static string Error_InvalidFields = "Exista campuri invalide!";
        public static string GenerateReportType2 = "Generati calcul de impozit dupa inregistrarea impozitului?";
        public static string ValidFormulas = "Formulele sunt valide";

    }
}
