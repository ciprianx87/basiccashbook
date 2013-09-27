using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Globalization;
using TaxCalculator.Data.Model;
using System.Windows.Media;

namespace TaxCalculator.Controls.Converters
{
    //[ValueConversion(typeof(bool), typeof())]
    public class BackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TaxIndicatorType taxIndicatorType = (TaxIndicatorType)value;
            if (taxIndicatorType == TaxIndicatorType.Calculat || taxIndicatorType == TaxIndicatorType.Text)
            {
                return new SolidColorBrush(Colors.LightGray);
                
            }
            return new SolidColorBrush(Colors.Transparent);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            return (visibility == Visibility.Visible);
        }
    }

    
}
