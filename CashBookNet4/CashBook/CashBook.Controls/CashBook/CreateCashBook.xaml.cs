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

namespace CashBook.Controls
{
    /// <summary>
    /// Interaction logic for CreateCashBook.xaml
    /// </summary>
    public partial class CreateCashBook : UserControl, IDisposable
    {
        public CreateCashBook()
        {
            InitializeComponent();
            this.Loaded += CreateCashBook_Loaded;

        }

        void CreateCashBook_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= CreateCashBook_Loaded;
            this.DataContext = new CreateCashBookViewModel();
        }



        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
        }
    }
}
