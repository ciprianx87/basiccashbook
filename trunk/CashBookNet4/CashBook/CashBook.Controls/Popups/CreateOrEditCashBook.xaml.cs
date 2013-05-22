using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CashBook.ViewModels;

namespace CashBook.Controls.Popups
{
    /// <summary>
    /// Interaction logic for CreateCashBook.xaml
    /// </summary>
    public partial class CreateOrEditCashBook : Window, IDisposable
    {
        public CreateOrEditCashBook()
        {
            InitializeComponent();
            this.Loaded += CreateCashBook_Loaded;
            this.DataContext = new CreateOrEditCashBookViewModel();

        }

        void CreateCashBook_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= CreateCashBook_Loaded;
            txtName.Focus();
        }



        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
        }
    }

    //public class MyDatePicker : DatePicker
    //{
    //    public MyDatePicker()
    //    {
    //        this.BlackoutDates
    //    }
        
    //}
}
