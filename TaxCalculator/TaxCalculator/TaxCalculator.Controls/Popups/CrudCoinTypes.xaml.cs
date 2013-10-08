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
using TaxCalculator.ViewModel.ViewModels.Popups;
using TaxCalculator.ViewModel.Base;

namespace TaxCalculator.Controls.Popups
{
    /// <summary>
    /// Interaction logic for CrudCoinTypes.xaml
    /// </summary>
    public partial class CrudCoinTypes : Window, IDisposable
    {
        public CrudCoinTypes()
        {
            InitializeComponent();
            this.Loaded += CreateCashBook_Loaded;
            this.DataContext = new CrudCoinTypesVm();

        }

        void CreateCashBook_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= CreateCashBook_Loaded;
        }

        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as CrudCoinTypesVm).DeleteItem((sender as Button).Tag);
        }
    }
}
