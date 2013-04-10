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
        }

        void CashBook_Loaded(object sender, RoutedEventArgs e)
        {
            //CashBooks = new ObservableCollection<Data.Model.CashBook>();
            //dataGrid.ItemsSource = CashBooks;
        }

        //public ObservableCollection<Data.Model.CashBookModel> CashBooks { get; set; }

        private void dataGrid_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //CashBooks.Insert(CashBooks.Count, new Data.Model.CashBook() { NrCrt = CashBooks.Count });
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //CashBooks.Add(new Data.Model.CashBook());

        }

        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
            
        }
    }
}
