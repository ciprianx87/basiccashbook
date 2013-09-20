using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.ViewModel.Base;
using System.ComponentModel;

namespace TaxCalculator.ViewModel.ViewModels.Model
{
    public class CompanyViewModel : NotificationPoperty, IDataErrorInfo
    {
        public Int64 Id { get; set; }
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                NotifyPropertyChanged("Address");
            }
        }

        private string cui;
        public string Cui
        {
            get { return cui; }
            set
            {
                cui = value;
                NotifyPropertyChanged("Cui");
            }
        }


        public string Error
        {
            get
            {
                return (this as IDataErrorInfo).Error;
                //    throw new NotImplementedException(); 
            }
        }

        public string this[string propertyName]
        {
            get
            {
                string result = "";
                switch (propertyName)
                {
                    case "Name":
                        if (string.IsNullOrEmpty(Name))
                        {
                            result = "Camp obligatoriu";
                        }
                        break;
                    case "Address":
                        if (string.IsNullOrEmpty(Address))
                        {
                            result = "Camp obligatoriu";
                        }
                        break;
                    case "Cui":
                        if (string.IsNullOrEmpty(Cui))
                        {
                            result = "Camp obligatoriu";
                        }
                        break;
                }

                return result;
            }
        }
    }
}
