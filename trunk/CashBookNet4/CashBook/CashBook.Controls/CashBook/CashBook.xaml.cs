using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for CashBook.xaml
    /// </summary>
    public partial class CashBook : UserControl, IDisposable
    {
        public CashBook()
        {
            InitializeComponent();
            this.Loaded += CashBook_Loaded;
            this.DataContext = new CashBookViewModel();

        }

        void CashBook_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= CashBook_Loaded;
        }

        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
        }

        private void dataGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                (this.DataContext as CashBookViewModel).AddNewItem();
            }
        }

        private void btnPickDate_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.DatePicker datePicker = new DatePicker();
        }
    }
}
