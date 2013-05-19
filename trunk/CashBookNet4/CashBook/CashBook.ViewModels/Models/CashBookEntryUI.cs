using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using CashBook.Data.Model;
using CashBook.Common.Mediator;
using CashBook.Common;
using System.Windows;

namespace CashBook.ViewModels.Models
{
    public class CashBookEntryUI : INotifyPropertyChanged, IDataErrorInfo
    {
        public static implicit operator CashBookEntryUI(CashBookEntry entry)
        {
            return new CashBookEntryUI()
            {
                Incasari = entry.Incasari,
                NrAnexe = entry.NrAnexe,
                Plati = entry.Plati,
                NrActCasa = entry.NrActCasa,
                Explicatii = entry.Explicatii,
                NrCrt = entry.NrCrt
            };
        }

        public static implicit operator CashBookEntry(CashBookEntryUI entry)
        {
            return new CashBookEntry()
            {
                Incasari = entry.Incasari,
                NrAnexe = entry.NrAnexe,
                Plati = entry.Plati,
                NrActCasa = entry.NrActCasa,
                Explicatii = entry.Explicatii,
                NrCrt = entry.NrCrt
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                switch (columnName)
                {
                    case "NrActCasa":
                        return null;
                        if (string.IsNullOrEmpty(NrActCasa))
                        {
                            result = "Camp obligatoriu";
                        }
                        break;
                    case "NrAnexe":
                        //if (string.IsNullOrEmpty(NrAnexe))
                        //{
                        //    result = "Camp obligatoriu";
                        //}
                        break;
                    case "Explicatii":
                        if (string.IsNullOrEmpty(Explicatii))
                        {
                            result = "Camp obligatoriu";
                        }
                        break;
                    case "Incasari":
                        return null;
                        if (Incasari < 0)
                        {
                            return "Valoarea trebuie sa fie mai mare sau egala cu 0";
                        }
                        result = AddErrorIfNeeded();
                        break;
                    case "Plati":
                        if (Plati < 0)
                        {
                            return "Valoarea trebuie sa fie mai mare sau egala cu 0";
                        }
                        result = AddErrorIfNeeded();

                        break;
                }

                return result;
            }
        }

        private string AddErrorIfNeeded()
        {
            string result = null;
            if (Incasari == 0 && Plati == 0)
            {
                result = "Unul din campurile Incasari sau Plati trebuie completat";

            } if (Incasari != 0 && Plati != 0)
            {
                result = "Doar unul din campurile Incasari sau Plati trebuie completat";
            }
            return result;
        }

        private int nrAnexe;
        public int NrAnexe
        {
            get { return nrAnexe; }
            set
            {
                if (nrAnexe != value)
                {
                    nrAnexe = value;
                    this.NotifyPropertyChanged("NrAnexe");
                    UpdateStringFields();
                }
            }
        }



        private decimal incasari;
        public decimal Incasari
        {
            get { return incasari; }
            set
            {
                if (incasari != value)
                {
                    incasari = value;
                    this.NotifyPropertyChanged("Incasari");
                    this.NotifyPropertyChanged("Plati");
                    NotifyChangedBalance(incasari);
                    UpdateStringFields();
                    IsValid();
                }
            }
        }



        private decimal plati;
        public decimal Plati
        {
            get { return plati; }
            set
            {
                if (plati != value)
                {
                    plati = value;
                    this.NotifyPropertyChanged("Plati");
                    this.NotifyPropertyChanged("Incasari");
                    NotifyChangedBalance(-plati);
                    IsFormValid();
                    UpdateStringFields();
                    IsValid();
                }
            }
        }

        private void NotifyChangedBalance(decimal value)
        {
            Mediator.Instance.SendMessage(MediatorActionType.UpdateBalance, value);
        }


        private int nrCrt;
        public int NrCrt
        {
            get { return nrCrt; }
            set
            {
                if (nrCrt != value)
                {
                    nrCrt = value;
                    this.NotifyPropertyChanged("NrCrt");
                    UpdateStringFields();
                }
            }
        }



        private string explicatii;
        public string Explicatii
        {
            get { return explicatii; }
            set
            {
                if (explicatii != value)
                {
                    explicatii = value;
                    this.NotifyPropertyChanged("Explicatii");
                    IsValid();
                }
            }
        }

        private string nrActCasa;
        public string NrActCasa
        {
            get { return nrActCasa; }
            set
            {
                if (nrActCasa != value)
                {
                    nrActCasa = value;
                    this.NotifyPropertyChanged("NrActCasa");
                    IsValid();
                }
            }
        }


        private decimal leiValue;
        public decimal LeiValue
        {
            get { return leiValue; }
            set
            {
                //if (leiValue != value)
                {
                    leiValue = value;

                    LeiValueString = DecimalConvertor.Instance.DecimalToString(LeiValue);

                }
            }
        }


        private string leiValueString;
        public string LeiValueString
        {
            get { return leiValueString; }
            set
            {
                if (leiValueString != value)
                {
                    leiValueString = value;
                    this.NotifyPropertyChanged("LeiValueString");
                }
            }
        }


        private FontWeight fontWeight;
        public FontWeight FontWeight
        {
            get { return fontWeight; }
            set
            {
                if (fontWeight != value)
                {
                    fontWeight = value;
                    this.NotifyPropertyChanged("FontWeight");
                }
            }
        }


        private bool isExtraDetail;
        public bool IsExtraDetail
        {
            get { return isExtraDetail; }
            set
            {
                if (isExtraDetail != value)
                {
                    isExtraDetail = value;
                    if (isExtraDetail)
                    {
                        FontWeight = FontWeights.Bold;
                        NrCrtString = "";
                        NrAnexeString = "";
                    }
                }
            }
        }


        private string incasariString;
        public string IncasariString
        {
            get { return incasariString; }
            set
            {
                if (incasariString != value)
                {
                    incasariString = Utils.PrepareForConversion(value);
                    if (!string.IsNullOrEmpty(incasariString))
                    {
                        Incasari = DecimalConvertor.Instance.StringToDecimal(incasariString);
                    }
                    incasariString = DecimalConvertor.Instance.DecimalToString(Incasari);
                    this.NotifyPropertyChanged("IncasariString");
                }
            }
        }


        private string platiString;
        public string PlatiString
        {
            get { return platiString; }
            set
            {
                if (platiString != value)
                {
                    platiString = Utils.PrepareForConversion(value);
                    if (!string.IsNullOrEmpty(platiString))
                    {
                        Plati = DecimalConvertor.Instance.StringToDecimal(platiString);
                    }
                    platiString = DecimalConvertor.Instance.DecimalToString(Plati);
                    this.NotifyPropertyChanged("PlatiString");
                }
            }
        }


        private string nrCrtString;
        public string NrCrtString
        {
            get { return nrCrtString; }
            set
            {
                if (nrCrtString != value)
                {
                    nrCrtString = value;
                    this.NotifyPropertyChanged("NrCrtString");
                }
            }
        }


        private string nrActCasaString;
        public string NrActCasaString
        {
            get { return nrActCasaString; }
            set
            {
                if (nrActCasaString != value)
                {
                    nrActCasaString = value;
                    this.NotifyPropertyChanged("NrActCasaString");
                }
            }
        }

        private string nrAnexeString;
        public string NrAnexeString
        {
            get { return nrAnexeString; }
            set
            {
                if (nrAnexeString != value)
                {
                    nrAnexeString = value;
                    this.NotifyPropertyChanged("NrAnexeString");
                    IsValid();
                }
            }
        }


        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                if (errorMessage != value)
                {
                    errorMessage = value;
                    this.NotifyPropertyChanged("ErrorMessage");
                    ErrorVisible = string.IsNullOrEmpty(errorMessage) ? Visibility.Hidden : Visibility.Visible;
                }
            }
        }

        private Visibility errorVisible;
        public Visibility ErrorVisible
        {
            get { return errorVisible; }
            set
            {
                if (errorVisible != value)
                {
                    errorVisible = value;
                    this.NotifyPropertyChanged("ErrorVisible");
                }
            }
        }


        public bool IsFormValid()
        {
            if (Plati > AppSettings.SinglePaymentLimit)
            {
                WindowHelper.OpenPaymentInformationDialog("Va rugam sa cititi Reglementarile legale legate de valoarea platilor. \r\nDoriti sa le cititi acum?");
                return false;
            }
            return IsValid();
            //return !(IsEmpty(NrActCasa, Explicatii) || ((Plati == 0 && Incasari == 0) || (Plati != 0 && Incasari != 0)));
        }

        public bool IsValid()
        {
            bool result = true;
            string message = "";
            if (IsEmpty(NrActCasa))
            {
                message += "NrActCasa este obligatoriu" + Environment.NewLine;
            }
            if (NrAnexe < 0)
            {
                message += "NrAnexe nu poate fi negativ" + Environment.NewLine;
            }
            if (IsEmpty(Explicatii))
            {
                message += "Explicatii este obligatoriu" + Environment.NewLine;
            }
            if (Plati != 0 && Incasari != 0)
            {
                message += "Doar unul din campurile Incasari sau Plati trebuie completat " + Environment.NewLine;
            }
            if (Plati == 0 && Incasari == 0)
            {
                message += "Unul din campurile Incasari sau Plati trebuie completat" + Environment.NewLine;
            }

            ErrorMessage = message;
            result = string.IsNullOrEmpty(message);
            if (!result)
            {
                // WindowHelper.OpenErrorDialog(message);
            }
            return result;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(NrActCasa) && string.IsNullOrEmpty(Explicatii) && Plati == 0 && Incasari == 0;
        }

        private bool IsEmpty(params string[] str)
        {
            foreach (var item in str)
            {
                if (string.IsNullOrEmpty(item))
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateStringFields()
        {
            //NrActCasaString = DecimalConvertor.Instance.DecimalToString(NrActCasa);
            PlatiString = DecimalConvertor.Instance.DecimalToString(Plati);
            IncasariString = DecimalConvertor.Instance.DecimalToString(Incasari);
            if (!IsExtraDetail)
            {
                NrAnexeString = NrAnexe.ToString();
                NrCrtString = NrCrt.ToString();
            }
        }
    }
}
