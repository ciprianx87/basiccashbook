using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using TaxCalculator.Data.Model;
using System.Windows;

namespace TaxCalculator.Controls.Converters
{
    public class IndicatorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TaxIndicatorType type = (TaxIndicatorType)value;
            //FontWeight
            Style st = new Style();
            //st.
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //public class IndicatorConverter : IMultiValueConverter
    //{
      
    //    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        TaxIndicatorType type = (TaxIndicatorType)values[0];

    //        return value;
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
