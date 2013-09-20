using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Controls;

namespace TaxCalculator.Controls.Converters
{
    public class ValidationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ReadOnlyObservableCollection<ValidationError> errors = value as ReadOnlyObservableCollection<ValidationError>;
            if (errors == null) return value;
            if (errors.Count > 0)
            {
                return errors[0].ErrorContent;
            }
            return "";
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("This method should never be called");
        }
    }
}
