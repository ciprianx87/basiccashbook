using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CashBook.Common.Mediator;
using CashBook.Common;
using CashBook.ViewModels;

namespace CashBook.Controls
{
    public class RedLetterDayConverter : IValueConverter
    {
        public RedLetterDayConverter()
        {
            SetCompletedDays(VMUtils.CompletedDates);
            Mediator.Instance.Register(MediatorActionType.SetCompletedDays, SetCompletedDays);
        }

        static RedLetterDayConverter()
        {
            //Mediator.Instance.Register(MediatorActionType.SetCompletedDays, SetCompletedDays);
        }

        public void SetCompletedDays(object param)
        {
            try
            {
                var days = param as List<DateTime>;
                dict.Clear();
                if (days != null)
                {
                    foreach (var item in days)
                    {
                        dict.Add(item, "Completat");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }

        static Dictionary<DateTime, string> dict =
                    new Dictionary<DateTime, string>();

        //static RedLetterDayConverter()
        //{
        //    //dict.Add(new DateTime(2013, 3, 17), "St. Patrick's Day");
        //    //dict.Add(new DateTime(2013, 3, 20), "First day of spring");
        //    //dict.Add(new DateTime(2013, 4, 1), "April Fools");
        //    //dict.Add(new DateTime(2013, 4, 22), "Earth Day");
        //    //dict.Add(new DateTime(2013, 5, 1), "May Day");
        //    //dict.Add(new DateTime(2013, 5, 10), "Mother's Day");
        //    //dict.Add(new DateTime(2013, 6, 21), "First Day of Summer");
        //}

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            string text;
            if (!dict.TryGetValue((DateTime)value, out text))
            {
                text = null;
            }
            else
            {
            }
            return text;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}