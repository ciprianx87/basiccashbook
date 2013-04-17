using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CashBook.Data.Model
{
    public partial class CashBookEntry : IDataErrorInfo
    {
        public CashBookEntry()
        {
        
        }

        public bool IsOk { get; set; }

        public int GridId { get; set; }

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
                        if (Incasari == 0 && Plati == 0)
                        {
                            result = "Unul din campurile Incasari sau Plati trebuie completat";
                        }
                        break;
                    case "Plati":
                        if (Incasari == 0 && Plati == 0)
                        {
                            result = "Unul din campurile Incasari sau Plati trebuie completat";
                        }
                        break;
                }

                return result;
            }
        }
    }
}
