using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxCalculator.Data.Model;
using System.Windows;
using TaxCalculator.Common.Mediator;
using TaxCalculator.ViewModel.Base;
using TaxCalculator.Common;
using System.ComponentModel;
using System.Windows.Media;

namespace TaxCalculator.ViewModel.ViewModels.Model
{
    public class TaxIndicatorViewModel : NotificationPoperty, IDataErrorInfo
    {

        public TaxIndicatorViewModel()
        {
            Style = new TaxIndicatorStyleInfo();
            Style.FontWeight = FontWeights.Normal;
            Style.FormulaFieldVisibility = Visibility.Visible;
            Style.ValueFieldVisibility = Visibility.Visible;

        }

        private bool typeEnabled;
        public bool TypeEnabled
        {
            get { return typeEnabled; }
            set
            {
                if (typeEnabled != value)
                {
                    typeEnabled = value;
                    this.NotifyPropertyChanged("TypeEnabled");
                }
            }
        }


        private string initialType;
        public string InitialType
        {
            get { return initialType; }
            set
            {
                if (initialType != value)
                {
                    initialType = value;
                    this.NotifyPropertyChanged("InitialType");


                }
            }
        }

        private bool indicatorFormulaEnabled;
        public bool IndicatorFormulaEnabled
        {
            get { return indicatorFormulaEnabled; }
            set
            {
                if (indicatorFormulaEnabled != value)
                {
                    indicatorFormulaEnabled = value;
                    this.NotifyPropertyChanged("IndicatorFormulaEnabled");
                }
            }
        }

        List<int> fixedItems = new List<int>() { 24, 27, 33 };
        public void UpdateEnabledState()
        {
            if (InitialType.ToLower() == "text")
            {
                TypeEnabled = false;
            }
            else
            {
                TypeEnabled = true;
            }

            IndicatorFormulaEnabled = true;
            if (NrCrt.HasValue && fixedItems.Contains(NrCrt.Value))
            {
                TypeEnabled = false;
                IndicatorFormulaEnabled = false;
            }


        }

        // public int? NrCrt { get; set; }
        public int? InitialNrCrt { get; set; }

        private int innerId;
        public int InnerId
        {
            get { return innerId; }
            set
            {
                if (innerId != value)
                {
                    innerId = value;
                    //this.NotifyPropertyChanged("InnerId");
                }
            }
        }

        private int? nrCrt;
        public int? NrCrt
        {
            get { return nrCrt; }
            set
            {
                if (nrCrt != value)
                {
                    nrCrt = value;
                    this.NotifyPropertyChanged("NrCrt");
                    this.NotifyPropertyChanged("NrCrtString");
                }
            }
        }

        public string NrCrtString
        {
            get
            {
                if (NrCrt == null)
                {
                    return "";
                }
                return NrCrt.ToString();
            }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    this.NotifyPropertyChanged("Description");
                    IsIndicatorValid();
                }
            }
        }

        // public string Description { get; set; }
        public string TypeDescription { get; set; }
        //public string IndicatorFormula { get; set; }
        //public TaxIndicatorType Type { get; set; }

        private TaxIndicatorType type;
        public TaxIndicatorType Type
        {
            get { return type; }
            set
            {
                if (type != value)
                {
                    UpdateNrCrt(type, value);
                    type = value;
                    TypeDescription = type.ToString();
                    //change the style also
                    Style = VmUtils.GetStyleInfo(type);
                    IsIndicatorValid();

                    this.NotifyPropertyChanged("Type");
                }
            }
        }

        private void UpdateNrCrt(TaxIndicatorType previous, TaxIndicatorType current)
        {
            if (current == TaxIndicatorType.Text)
            {
                NrCrt = null;
            }
            else if (previous == TaxIndicatorType.Text)
            {
                NrCrt = InitialNrCrt;
            }
        }

        private string valueField;

        public string ValueField
        {
            get { return valueField; }
            //get
            //{
            //    //var val = DecimalConvertor.Instance.DecimalToString(ValueFieldNumeric);
            //    //return val;
            //    return  ValueFieldNumeric.ToString(); 
            //}
            set
            {
                try
                {
                    if (value == null)
                    {
                        valueField = null;
                        return;
                    }
                    valueField = Utils.PrepareForConversion(value);
                    if (!string.IsNullOrEmpty(valueField))
                    {
                        ValueFieldNumeric = DecimalConvertor.Instance.StringToDecimal(valueField);
                    }
                    valueField = DecimalConvertor.Instance.DecimalToString(ValueFieldNumeric);

                    //ValueFieldNumeric = Convert.ToInt32(value);

                    //execute the function
                    if (Type != TaxIndicatorType.Calculat)
                    {
                        Mediator.Instance.SendMessage(MediatorActionType.ExecuteTaxCalculation, null);
                    }
                    NotifyPropertyChanged("ValueField");
                }
                catch (Exception)
                {

                }
            }
        }

        private string indicatorFormula;

        public string IndicatorFormula
        {
            get { return indicatorFormula; }
            set
            {
                indicatorFormula = value;
                IsIndicatorValid();
                NotifyPropertyChanged("IndicatorFormula");
            }
        }


        private bool isValid;
        public bool IsValid
        {
            get { return isValid; }
            set
            {
                if (isValid != value)
                {
                    isValid = value;
                    this.NotifyPropertyChanged("IsValid");
                }
            }
        }


        public string this[string columnName]
        {
            get
            {
                string result = null;
                return result;
                switch (columnName)
                {
                    case "IndicatorFormula":
                        //return null;
                        if (Type == TaxIndicatorType.Calculat && string.IsNullOrEmpty(IndicatorFormula))
                        {
                            result = "Camp obligatoriu";
                        }
                        break;
                }
                return result;
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



        private System.Windows.Media.Brush formulaTextColor;
        public System.Windows.Media.Brush FormulaTextColor
        {
            get { return formulaTextColor; }
            set
            {
                if (formulaTextColor != value)
                {
                    formulaTextColor = value;
                    this.NotifyPropertyChanged("FormulaTextColor");
                }
            }
        }



        public bool IsIndicatorValid()
        {
            bool result = true;
            string message = "";

            if (Type == TaxIndicatorType.Calculat && string.IsNullOrEmpty(IndicatorFormula))
            {
                message = "IndicatorFormula este obligatoriu";
            }
            if (string.IsNullOrEmpty(Description))
            {
                message = Messages.Error_EmptyIndicatorName;
            }

            ErrorMessage = message;
            result = string.IsNullOrEmpty(message);
            if (result)
            {
                this.FormulaTextColor = new SolidColorBrush(Colors.Black);
                // WindowHelper.OpenErrorDialog(message);
            }
            else
            {
                this.FormulaTextColor = new SolidColorBrush(Colors.Red);
            }
            return result;
        }

        public decimal ValueFieldNumeric { get; set; }
        public string InitialValueField { get; set; }
        //public TaxIndicatorStyleInfo Style { get; set; }

        private TaxIndicatorStyleInfo style;
        public TaxIndicatorStyleInfo Style
        {
            get { return style; }
            set
            {
                if (style != value)
                {
                    style = value;
                    this.NotifyPropertyChanged("Style");
                }
            }
        }



        public static TaxIndicatorViewModel FromTaxIndicator(TaxIndicator ti)
        {

            return new TaxIndicatorViewModel() { };
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        internal void SetError(string message)
        {
            this.ErrorMessage = message;
            this.FormulaTextColor = new SolidColorBrush(Colors.Red);
        }
    }

    public class TaxIndicatorStyleInfo
    {
        public Visibility FormulaFieldVisibility { get; set; }
        public Visibility ValueFieldVisibility { get; set; }
        public FontWeight FontWeight { get; set; }
        public Visibility NrCrtVisibility { get; set; }

    }

}
