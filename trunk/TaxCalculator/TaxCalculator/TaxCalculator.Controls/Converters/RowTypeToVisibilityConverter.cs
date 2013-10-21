using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using TaxCalculator.Data.Model;
using System.Windows;

namespace TaxCalculator.Controls.Converters
{
    public class RowTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TaxIndicatorType type = (TaxIndicatorType)value;
            var parType = (TaxIndicatorType)Enum.Parse(typeof(TaxIndicatorType), parameter.ToString());
            if (parType == type)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}