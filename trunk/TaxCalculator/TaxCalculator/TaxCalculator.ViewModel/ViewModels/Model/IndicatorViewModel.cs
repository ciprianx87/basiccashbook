using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.ViewModel.Base;
using System.ComponentModel;

namespace TaxCalculator.ViewModel.ViewModels.Model
{
    public class IndicatorViewModel : NotificationPoperty, IDataErrorInfo
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

        private bool isDefault;
        public bool IsDefault
        {
            get { return isDefault; }
            set
            {
                if (isDefault != value)
                {
                    isDefault = value;
                    this.NotifyPropertyChanged("IsDefault");
                }
            }
        }


        private bool isDefaultEnabled;
        public bool IsDefaultEnabled
        {
            get { return isDefaultEnabled; }
            set
            {
                if (isDefaultEnabled != value)
                {
                    isDefaultEnabled = value;
                    this.NotifyPropertyChanged("IsDefaultEnabled");
                }
            }
        }



        public string Error
        {
            get
            {

                throw new NotImplementedException();
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
                }

                return result;
            }
        }
    }
}
