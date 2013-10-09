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
using TaxCalculator.ViewModel.ViewModels;
using TaxCalculator.ViewModel.Base;

namespace TaxCalculator.Controls
{
    /// <summary>
    /// Interaction logic for TaxCalculationCompletion.xaml
    /// </summary>
    public partial class TaxCalculationCompletion : UserControl, IDisposable
    {
        public TaxCalculationCompletion()
        {
            InitializeComponent();
            this.DataContext = new TaxCalculationCompletionVm();
            //dgTaxIndicators.ItemsSource = new List<object>() { 1 };
        }

        public void Dispose()
        {
            (this.DataContext as BaseViewModel).Dispose();
        }

        private void AddBefore_Click(object sender, RoutedEventArgs e)
        {
            //var selectedRow = dgTaxIndicators.SelectedItem;
            //this.DataContext = new TaxCalculationVm();
        }

        private void AddAfter_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
