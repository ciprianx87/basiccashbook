using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using CashBook.Data.Model;
using CashBook.Common.Mediator;

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
                }
            }
        }


    }
}
